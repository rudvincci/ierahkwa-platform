'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// File Storage Routes v1.0.0
// File upload via multer, metadata in PostgreSQL
// Soft-delete, owner-only operations
// ============================================================

const { Router } = require('express');
const path = require('path');
const crypto = require('crypto');
const multer = require('multer');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:storage');
const audit = createAuditLogger('sovereign-core:storage');

// ============================================================
// Multer configuration
// ============================================================

const UPLOAD_DIR = process.env.UPLOAD_DIR || path.resolve(__dirname, '../../../../data/uploads');
const MAX_FILE_SIZE = parseInt(process.env.MAX_FILE_SIZE, 10) || 50 * 1024 * 1024; // 50 MB default

// Allowed MIME types
const ALLOWED_MIMES = new Set([
  // Images
  'image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/svg+xml', 'image/avif',
  // Documents
  'application/pdf', 'application/json', 'text/plain', 'text/csv', 'text/html',
  'application/xml', 'text/xml',
  // Office
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  // Archives
  'application/zip', 'application/gzip', 'application/x-tar',
  // Audio/Video
  'audio/mpeg', 'audio/ogg', 'audio/wav', 'audio/webm',
  'video/mp4', 'video/webm', 'video/ogg'
]);

const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, UPLOAD_DIR);
  },
  filename: (req, file, cb) => {
    // Generate unique filename: timestamp-randomhex-originalname
    const timestamp = Date.now();
    const random = crypto.randomBytes(8).toString('hex');
    const ext = path.extname(file.originalname).toLowerCase();
    const safeName = file.originalname
      .replace(/[^a-zA-Z0-9._-]/g, '_')
      .substring(0, 100);
    cb(null, `${timestamp}-${random}-${safeName}`);
  }
});

const upload = multer({
  storage,
  limits: {
    fileSize: MAX_FILE_SIZE,
    files: 1
  },
  fileFilter: (req, file, cb) => {
    if (!ALLOWED_MIMES.has(file.mimetype)) {
      return cb(new AppError('INVALID_FORMAT', `File type ${file.mimetype} is not allowed. Allowed types: images, documents, office files, audio, video, archives.`));
    }
    cb(null, true);
  }
});

// ============================================================
// POST /upload — Upload a file
// ============================================================
router.post('/upload',
  (req, res, next) => {
    if (!req.user) {
      return next(new AppError('AUTH_REQUIRED', 'Authentication required'));
    }
    next();
  },
  (req, res, next) => {
    // Wrap multer to catch its errors
    upload.single('file')(req, res, (err) => {
      if (err instanceof multer.MulterError) {
        if (err.code === 'LIMIT_FILE_SIZE') {
          return next(new AppError('INVALID_INPUT', `File exceeds maximum size of ${MAX_FILE_SIZE / (1024 * 1024)} MB`));
        }
        if (err.code === 'LIMIT_FILE_COUNT') {
          return next(new AppError('INVALID_INPUT', 'Only one file can be uploaded at a time'));
        }
        return next(new AppError('INVALID_INPUT', `Upload error: ${err.message}`));
      }
      if (err instanceof AppError) {
        return next(err);
      }
      if (err) {
        return next(new AppError('INTERNAL', 'File upload failed'));
      }
      next();
    });
  },
  asyncHandler(async (req, res) => {
    if (!req.file) {
      throw new AppError('MISSING_FIELD', 'No file uploaded. Send file in the "file" form field.');
    }

    const file = req.file;
    const platform = req.body.platform || req.platform || null;
    const description = req.body.description || null;

    // Compute file hash for deduplication and integrity
    const fs = require('fs');
    const fileBuffer = fs.readFileSync(file.path);
    const fileHash = crypto.createHash('sha256').update(fileBuffer).digest('hex');

    // Insert file metadata into DB
    const result = await db.query(
      `INSERT INTO files (original_name, stored_name, mime_type, size_bytes, file_hash, file_path, platform, description, owner_id, status, created_at)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, 'active', NOW())
       RETURNING id, original_name, stored_name, mime_type, size_bytes, file_hash, platform, description, owner_id, status, created_at`,
      [
        file.originalname,
        file.filename,
        file.mimetype,
        file.size,
        fileHash,
        file.path,
        platform,
        description ? description.trim() : null,
        req.user.id
      ]
    );

    const fileRecord = result.rows[0];

    audit.dataModify(req, 'file', fileRecord.id, { action: 'upload', originalName: file.originalname, size: file.size });
    log.info('File uploaded', { fileId: fileRecord.id, name: file.originalname, size: file.size, mime: file.mimetype });

    res.status(201).json({
      status: 'ok',
      data: {
        id: fileRecord.id,
        originalName: fileRecord.original_name,
        mimeType: fileRecord.mime_type,
        sizeBytes: fileRecord.size_bytes,
        fileHash: fileRecord.file_hash,
        platform: fileRecord.platform,
        description: fileRecord.description,
        createdAt: fileRecord.created_at,
        downloadUrl: `/v1/storage/${fileRecord.id}`
      }
    });
  })
);

// ============================================================
// GET /:fileId — Get file metadata and serve file
// ============================================================
router.get('/:fileId',
  validate({ params: { fileId: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    const { fileId } = req.params;

    const result = await db.query(
      `SELECT id, original_name, stored_name, mime_type, size_bytes, file_hash, file_path,
              platform, description, owner_id, status, created_at
       FROM files
       WHERE id = $1 AND status = 'active'`,
      [fileId]
    );

    if (result.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'File not found');
    }

    const file = result.rows[0];

    // If query param ?meta=true, return metadata only
    if (req.query.meta === 'true') {
      return res.json({
        status: 'ok',
        data: {
          id: file.id,
          originalName: file.original_name,
          mimeType: file.mime_type,
          sizeBytes: file.size_bytes,
          fileHash: file.file_hash,
          platform: file.platform,
          description: file.description,
          ownerId: file.owner_id,
          createdAt: file.created_at
        }
      });
    }

    // Serve the file
    const fs = require('fs');
    if (!fs.existsSync(file.file_path)) {
      log.error('File missing from disk', { fileId, path: file.file_path });
      throw new AppError('NOT_FOUND', 'File data not found on storage');
    }

    res.set({
      'Content-Type': file.mime_type,
      'Content-Length': file.size_bytes,
      'Content-Disposition': `inline; filename="${encodeURIComponent(file.original_name)}"`,
      'X-File-Hash': file.file_hash,
      'Cache-Control': 'private, max-age=3600'
    });

    const stream = fs.createReadStream(file.file_path);
    stream.pipe(res);
  })
);

// ============================================================
// DELETE /:fileId — Soft-delete a file (owner or admin only)
// ============================================================
router.delete('/:fileId',
  validate({ params: { fileId: t.uuid({ required: true }) } }),
  asyncHandler(async (req, res) => {
    if (!req.user) {
      throw new AppError('AUTH_REQUIRED', 'Authentication required');
    }

    const { fileId } = req.params;

    // Check ownership
    const existing = await db.query(
      `SELECT id, owner_id, original_name FROM files WHERE id = $1 AND status = 'active'`,
      [fileId]
    );

    if (existing.rows.length === 0) {
      throw new AppError('NOT_FOUND', 'File not found');
    }

    if (existing.rows[0].owner_id !== req.user.id && req.user.role !== 'admin') {
      throw new AppError('AUTH_INSUFFICIENT_ROLE', 'You can only delete your own files');
    }

    // Soft delete — do NOT remove from disk (recoverable)
    await db.query(
      `UPDATE files SET status = 'deleted', updated_at = NOW() WHERE id = $1`,
      [fileId]
    );

    audit.dataModify(req, 'file', fileId, { action: 'soft_delete', originalName: existing.rows[0].original_name });
    log.info('File soft-deleted', { fileId, name: existing.rows[0].original_name });

    res.json({
      status: 'ok',
      message: 'File deleted'
    });
  })
);

// ============================================================
// Exports
// ============================================================
module.exports = router;

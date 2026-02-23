/**
 * Ierahkwa Image Upload Service
 * Node.js + Express + Multer + DropzoneJS
 * 
 * Features:
 * - Single & Multiple file upload
 * - Drag & Drop support
 * - Progress bars
 * - Preview images
 * - Thumbnail generation
 * - Cross-platform support
 */

const express = require('express');
const multer = require('multer');
const path = require('path');
const fs = require('fs');
const { v4: uuidv4 } = require('uuid');

const app = express();
const PORT = process.env.PORT || 3500;

// Configuration
const CONFIG = {
    uploadDir: path.join(__dirname, 'public', 'uploads'),
    thumbnailDir: path.join(__dirname, 'public', 'uploads', 'thumbnails'),
    maxFileSize: 10 * 1024 * 1024, // 10MB
    allowedTypes: ['image/jpeg', 'image/png', 'image/gif', 'image/bmp', 'image/webp'],
    allowedExtensions: ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp']
};

// Create directories if they don't exist
[CONFIG.uploadDir, CONFIG.thumbnailDir].forEach(dir => {
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }
});

// Multer storage configuration
const storage = multer.diskStorage({
    destination: (req, file, cb) => {
        cb(null, CONFIG.uploadDir);
    },
    filename: (req, file, cb) => {
        const ext = path.extname(file.originalname).toLowerCase();
        const uniqueName = `${uuidv4()}${ext}`;
        cb(null, uniqueName);
    }
});

// File filter
const fileFilter = (req, file, cb) => {
    const ext = path.extname(file.originalname).toLowerCase();
    
    if (CONFIG.allowedTypes.includes(file.mimetype) && CONFIG.allowedExtensions.includes(ext)) {
        cb(null, true);
    } else {
        cb(new Error(`Tipo de archivo no permitido. Tipos válidos: ${CONFIG.allowedExtensions.join(', ')}`), false);
    }
};

// Multer upload middleware
const upload = multer({
    storage: storage,
    limits: {
        fileSize: CONFIG.maxFileSize
    },
    fileFilter: fileFilter
});

// Middleware
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(express.static(path.join(__dirname, 'public')));

// CORS headers for cross-platform support
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET, POST, DELETE, OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    if (req.method === 'OPTIONS') {
        return res.sendStatus(200);
    }
    next();
});

// ============================================
// API Routes
// ============================================

/**
 * POST /api/upload - Upload single file (Dropzone default)
 */
app.post('/api/upload', upload.single('file'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({
                success: false,
                errorMessage: 'No se proporcionó ningún archivo.'
            });
        }

        const result = await processUploadedFile(req.file);
        res.json(result);

    } catch (error) {
        console.error('Error en upload:', error);
        res.status(500).json({
            success: false,
            errorMessage: error.message || 'Error al subir el archivo.'
        });
    }
});

/**
 * POST /api/upload/single - Upload single file
 */
app.post('/api/upload/single', upload.single('file'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({
                success: false,
                errorMessage: 'No se proporcionó ningún archivo.'
            });
        }

        const result = await processUploadedFile(req.file);
        res.json(result);

    } catch (error) {
        console.error('Error en upload single:', error);
        res.status(500).json({
            success: false,
            errorMessage: error.message || 'Error al subir el archivo.'
        });
    }
});

/**
 * POST /api/upload/multiple - Upload multiple files
 */
app.post('/api/upload/multiple', upload.array('files', 20), async (req, res) => {
    try {
        if (!req.files || req.files.length === 0) {
            return res.status(400).json({
                success: false,
                message: 'No se proporcionaron archivos.'
            });
        }

        const results = [];
        let successCount = 0;
        let failCount = 0;

        for (const file of req.files) {
            try {
                const result = await processUploadedFile(file);
                results.push(result);
                if (result.success) successCount++;
                else failCount++;
            } catch (err) {
                results.push({
                    success: false,
                    fileName: file.originalname,
                    errorMessage: err.message
                });
                failCount++;
            }
        }

        res.json({
            success: successCount > 0,
            totalFiles: req.files.length,
            successfulUploads: successCount,
            failedUploads: failCount,
            results: results,
            message: `Subidos ${successCount} de ${req.files.length} archivos exitosamente.`
        });

    } catch (error) {
        console.error('Error en upload multiple:', error);
        res.status(500).json({
            success: false,
            errorMessage: error.message || 'Error al subir los archivos.'
        });
    }
});

/**
 * DELETE /api/upload/:fileName - Delete file
 */
app.delete('/api/upload/:fileName', (req, res) => {
    try {
        const fileName = path.basename(req.params.fileName); // Sanitize
        const filePath = path.join(CONFIG.uploadDir, fileName);
        const thumbnailPath = path.join(CONFIG.thumbnailDir, fileName);

        // Delete main file
        if (fs.existsSync(filePath)) {
            fs.unlinkSync(filePath);
        }

        // Delete thumbnail if exists
        if (fs.existsSync(thumbnailPath)) {
            fs.unlinkSync(thumbnailPath);
        }

        res.json({
            success: true,
            message: 'Archivo eliminado exitosamente.'
        });

    } catch (error) {
        console.error('Error deleting file:', error);
        res.status(500).json({
            success: false,
            message: 'Error al eliminar el archivo.'
        });
    }
});

/**
 * GET /api/upload/list - Get list of uploaded files
 */
app.get('/api/upload/list', (req, res) => {
    try {
        const files = [];

        if (fs.existsSync(CONFIG.uploadDir)) {
            const uploadedFiles = fs.readdirSync(CONFIG.uploadDir)
                .filter(file => {
                    const ext = path.extname(file).toLowerCase();
                    return CONFIG.allowedExtensions.includes(ext);
                });

            uploadedFiles.forEach(file => {
                const filePath = path.join(CONFIG.uploadDir, file);
                const stats = fs.statSync(filePath);
                
                files.push({
                    id: uuidv4(),
                    fileName: file,
                    originalName: file,
                    filePath: `/uploads/${file}`,
                    thumbnailPath: `/uploads/thumbnails/${file}`,
                    fileSize: stats.size,
                    contentType: getContentType(path.extname(file)),
                    uploadedAt: stats.birthtime
                });
            });
        }

        // Sort by upload date (newest first)
        files.sort((a, b) => new Date(b.uploadedAt) - new Date(a.uploadedAt));

        res.json(files);

    } catch (error) {
        console.error('Error getting file list:', error);
        res.status(500).json({
            success: false,
            message: 'Error al obtener la lista de archivos.'
        });
    }
});

/**
 * GET /api/stats - Get upload statistics
 */
app.get('/api/stats', (req, res) => {
    try {
        let totalFiles = 0;
        let totalSize = 0;

        if (fs.existsSync(CONFIG.uploadDir)) {
            const files = fs.readdirSync(CONFIG.uploadDir)
                .filter(file => {
                    const ext = path.extname(file).toLowerCase();
                    return CONFIG.allowedExtensions.includes(ext);
                });

            totalFiles = files.length;
            files.forEach(file => {
                const filePath = path.join(CONFIG.uploadDir, file);
                const stats = fs.statSync(filePath);
                totalSize += stats.size;
            });
        }

        res.json({
            totalFiles,
            totalSize,
            formattedSize: formatFileSize(totalSize),
            maxFileSize: CONFIG.maxFileSize,
            allowedExtensions: CONFIG.allowedExtensions
        });

    } catch (error) {
        res.status(500).json({ error: 'Error getting stats' });
    }
});

// ============================================
// Helper Functions
// ============================================

/**
 * Process uploaded file and generate response
 */
async function processUploadedFile(file) {
    const result = {
        success: true,
        fileName: file.filename,
        originalName: file.originalname,
        filePath: `/uploads/${file.filename}`,
        thumbnailPath: `/uploads/${file.filename}`,
        fileSize: file.size,
        contentType: file.mimetype
    };

    // Try to generate thumbnail with sharp (if available)
    try {
        const sharp = require('sharp');
        const thumbnailPath = path.join(CONFIG.thumbnailDir, file.filename);
        
        await sharp(file.path)
            .resize(200, 200, { fit: 'cover' })
            .toFile(thumbnailPath);
        
        result.thumbnailPath = `/uploads/thumbnails/${file.filename}`;
    } catch (err) {
        // Sharp not available or error - use original as thumbnail
        console.log('Thumbnail generation skipped:', err.message);
    }

    console.log(`Archivo subido: ${file.originalname} -> ${file.filename} (${formatFileSize(file.size)})`);
    
    return result;
}

/**
 * Get content type from extension
 */
function getContentType(ext) {
    const types = {
        '.jpg': 'image/jpeg',
        '.jpeg': 'image/jpeg',
        '.png': 'image/png',
        '.gif': 'image/gif',
        '.bmp': 'image/bmp',
        '.webp': 'image/webp'
    };
    return types[ext.toLowerCase()] || 'application/octet-stream';
}

/**
 * Format file size to human readable
 */
function formatFileSize(bytes) {
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    if (bytes === 0) return '0 B';
    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    return `${(bytes / Math.pow(1024, i)).toFixed(2)} ${sizes[i]}`;
}

// ============================================
// Error Handling
// ============================================

// Multer error handling
app.use((error, req, res, next) => {
    if (error instanceof multer.MulterError) {
        if (error.code === 'LIMIT_FILE_SIZE') {
            return res.status(400).json({
                success: false,
                errorMessage: `El archivo excede el tamaño máximo permitido (${formatFileSize(CONFIG.maxFileSize)}).`
            });
        }
        return res.status(400).json({
            success: false,
            errorMessage: error.message
        });
    }
    
    if (error) {
        return res.status(400).json({
            success: false,
            errorMessage: error.message
        });
    }
    
    next();
});

// ============================================
// Start Server
// ============================================

app.listen(PORT, () => {
    console.log('╔════════════════════════════════════════════════════════╗');
    console.log('║        IERAHKWA IMAGE UPLOAD SERVICE                   ║');
    console.log('╠════════════════════════════════════════════════════════╣');
    console.log(`║  Server running on: http://localhost:${PORT}             ║`);
    console.log('║  Platform: Ierahkwa Sovereign Government               ║');
    console.log('║  Features: Dropzone, Drag&Drop, Progress, Preview      ║');
    console.log('╚════════════════════════════════════════════════════════╝');
});

module.exports = app;

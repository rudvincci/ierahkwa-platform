'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Authentication Routes v1.0.0
// Registration, login, logout, token refresh, current user
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');
const { generateToken } = require('../../middleware/auth');

const router = Router();
const log = createLogger('sovereign-core:auth');
const audit = createAuditLogger('sovereign-core:auth');

// ============================================================
// Bcrypt-compatible password hashing (using native crypto)
// We use PBKDF2 with 310,000 iterations (OWASP 2023 recommendation)
// ============================================================

const HASH_ITERATIONS = 310000;
const HASH_KEYLEN = 64;
const HASH_DIGEST = 'sha512';
const SALT_BYTES = 32;

/**
 * Hash a password using PBKDF2
 * @param {string} password - Plaintext password
 * @returns {Promise<string>} Encoded hash string: $pbkdf2$iterations$salt$hash
 */
async function hashPassword(password) {
  return new Promise((resolve, reject) => {
    const salt = crypto.randomBytes(SALT_BYTES);
    crypto.pbkdf2(password, salt, HASH_ITERATIONS, HASH_KEYLEN, HASH_DIGEST, (err, derivedKey) => {
      if (err) return reject(err);
      resolve(`$pbkdf2$${HASH_ITERATIONS}$${salt.toString('hex')}$${derivedKey.toString('hex')}`);
    });
  });
}

/**
 * Compare password against stored hash
 * @param {string} password - Plaintext password
 * @param {string} storedHash - Encoded hash string
 * @returns {Promise<boolean>}
 */
async function comparePassword(password, storedHash) {
  return new Promise((resolve, reject) => {
    const parts = storedHash.split('$');
    // Format: $pbkdf2$iterations$salt$hash
    if (parts.length !== 5 || parts[1] !== 'pbkdf2') {
      return resolve(false);
    }

    const iterations = parseInt(parts[2], 10);
    const salt = Buffer.from(parts[3], 'hex');
    const hash = parts[4];

    crypto.pbkdf2(password, salt, iterations, HASH_KEYLEN, HASH_DIGEST, (err, derivedKey) => {
      if (err) return reject(err);
      // Timing-safe comparison
      const derivedHex = derivedKey.toString('hex');
      const a = Buffer.from(derivedHex);
      const b = Buffer.from(hash);
      if (a.length !== b.length) return resolve(false);
      resolve(crypto.timingSafeEqual(a, b));
    });
  });
}

// ============================================================
// POST /register — Create a new user account
// ============================================================
router.post('/register',
  validate({
    body: {
      email: t.string({ required: true, email: true }),
      password: t.string({ required: true, min: 8, max: 128 }),
      display_name: t.string({ required: true, min: 2, max: 100 }),
      nation: t.string({ max: 100 }),
      language: t.string({ enum: ['es', 'en', 'qu', 'ay', 'gn', 'my', 'na', 'mp'] })
    }
  }),
  asyncHandler(async (req, res) => {
    const { email, password, display_name, nation, language } = req.body;

    // Check for existing user
    const existing = await db.query(
      'SELECT id FROM users WHERE email = $1',
      [email.toLowerCase().trim()]
    );

    if (existing.rows.length > 0) {
      throw new AppError('ALREADY_EXISTS', 'A user with this email already exists');
    }

    // Hash password
    const passwordHash = await hashPassword(password);

    // Insert user
    const result = await db.query(
      `INSERT INTO users (email, password_hash, display_name, nation, language, role, status, created_at, updated_at)
       VALUES ($1, $2, $3, $4, $5, 'user', 'active', NOW(), NOW())
       RETURNING id, email, display_name, nation, language, role, status, created_at`,
      [email.toLowerCase().trim(), passwordHash, display_name.trim(), nation || null, language || 'es']
    );

    const user = result.rows[0];

    // Generate token for immediate login
    const token = generateToken(
      { sub: user.id, email: user.email, role: user.role, display_name: user.display_name, nation: user.nation },
      process.env.JWT_SECRET
    );

    audit.record({
      category: audit.CATEGORIES?.AUTH_LOGIN || 'AUTH_LOGIN',
      action: 'user_registered',
      risk: 'MEDIUM',
      req,
      resource: { type: 'user', id: user.id },
      details: { email: user.email }
    });

    log.info('User registered', { userId: user.id, email: user.email });

    res.status(201).json({
      status: 'ok',
      data: {
        user,
        token,
        tokenType: 'Bearer'
      }
    });
  })
);

// ============================================================
// POST /login — Authenticate user with email and password
// ============================================================
router.post('/login',
  validate({
    body: {
      email: t.string({ required: true, email: true }),
      password: t.string({ required: true, min: 1 })
    }
  }),
  asyncHandler(async (req, res) => {
    const { email, password } = req.body;

    // Find user by email
    const result = await db.query(
      `SELECT id, email, password_hash, display_name, nation, role, status
       FROM users
       WHERE email = $1`,
      [email.toLowerCase().trim()]
    );

    if (result.rows.length === 0) {
      audit.loginFailure(req, 'user_not_found');
      throw new AppError('AUTH_INVALID_CREDS', 'Invalid email or password');
    }

    const user = result.rows[0];

    // Check account status
    if (user.status === 'locked' || user.status === 'suspended') {
      audit.loginFailure(req, 'account_locked');
      throw new AppError('AUTH_ACCOUNT_LOCKED', 'Account is locked or suspended. Contact support.');
    }

    // Verify password
    const valid = await comparePassword(password, user.password_hash);
    if (!valid) {
      // Increment failed login counter
      await db.query(
        `UPDATE users SET failed_logins = COALESCE(failed_logins, 0) + 1, updated_at = NOW() WHERE id = $1`,
        [user.id]
      );
      audit.loginFailure(req, 'invalid_password');
      throw new AppError('AUTH_INVALID_CREDS', 'Invalid email or password');
    }

    // Reset failed login counter and update last login
    await db.query(
      `UPDATE users SET failed_logins = 0, last_login_at = NOW(), updated_at = NOW() WHERE id = $1`,
      [user.id]
    );

    // Generate JWT
    const token = generateToken(
      { sub: user.id, email: user.email, role: user.role, display_name: user.display_name, nation: user.nation },
      process.env.JWT_SECRET
    );

    audit.loginSuccess(req, user.id);
    log.info('User logged in', { userId: user.id });

    res.json({
      status: 'ok',
      data: {
        user: {
          id: user.id,
          email: user.email,
          display_name: user.display_name,
          nation: user.nation,
          role: user.role
        },
        token,
        tokenType: 'Bearer'
      }
    });
  })
);

// ============================================================
// POST /logout — Stateless logout (client discards token)
// ============================================================
router.post('/logout', asyncHandler(async (req, res) => {
  // With stateless JWT there is no server-side session to invalidate.
  // In a production system with token revocation, we would insert
  // the token's jti into a blacklist table here.
  if (req.user) {
    audit.record({
      category: 'AUTH_LOGOUT',
      action: 'user_logged_out',
      risk: 'LOW',
      req,
      resource: { type: 'user', id: req.user.id }
    });
    log.info('User logged out', { userId: req.user.id });
  }

  res.json({
    status: 'ok',
    message: 'Logged out successfully. Discard your token on the client side.'
  });
}));

// ============================================================
// GET /me — Return the currently authenticated user
// ============================================================
router.get('/me', asyncHandler(async (req, res) => {
  if (!req.user) {
    throw new AppError('AUTH_REQUIRED', 'Authentication required');
  }

  // Fetch fresh user data from DB
  const result = await db.query(
    `SELECT id, email, display_name, nation, language, role, status, created_at, last_login_at
     FROM users
     WHERE id = $1 AND status != 'deleted'`,
    [req.user.id]
  );

  if (result.rows.length === 0) {
    throw new AppError('NOT_FOUND', 'User account not found');
  }

  res.json({
    status: 'ok',
    data: result.rows[0]
  });
}));

// ============================================================
// POST /refresh — Generate a new token (extends session)
// ============================================================
router.post('/refresh', asyncHandler(async (req, res) => {
  if (!req.user) {
    throw new AppError('AUTH_REQUIRED', 'Authentication required');
  }

  // Verify user still exists and is active
  const result = await db.query(
    `SELECT id, email, display_name, nation, role, status FROM users WHERE id = $1`,
    [req.user.id]
  );

  if (result.rows.length === 0) {
    throw new AppError('NOT_FOUND', 'User account not found');
  }

  const user = result.rows[0];

  if (user.status !== 'active') {
    throw new AppError('AUTH_ACCOUNT_LOCKED', 'Account is no longer active');
  }

  // Issue new token
  const token = generateToken(
    { sub: user.id, email: user.email, role: user.role, display_name: user.display_name, nation: user.nation },
    process.env.JWT_SECRET
  );

  log.debug('Token refreshed', { userId: user.id });

  res.json({
    status: 'ok',
    data: {
      token,
      tokenType: 'Bearer'
    }
  });
}));

// ============================================================
// Exports
// ============================================================
module.exports = router;

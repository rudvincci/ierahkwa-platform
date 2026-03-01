'use strict';

// ============================================================
// Ierahkwa Sovereign Core — Centralized Configuration v1.0.0
// Reads all settings from environment variables.
// Fails fast in production if required vars are missing.
// ============================================================

const crypto = require('crypto');

const ENV = process.env.NODE_ENV || 'development';
const IS_PROD = ENV === 'production';
const IS_TEST = ENV === 'test';

// ── Required vars in production ─────────────────────────────
const REQUIRED_IN_PROD = [
  'DATABASE_URL',
  'JWT_SECRET',
  'CORS_ORIGINS'
];

/**
 * Read env var, optionally required
 */
function env(key, defaultValue) {
  const value = process.env[key];
  if (value !== undefined && value !== '') return value;
  if (defaultValue !== undefined) return defaultValue;
  return undefined;
}

/**
 * Read env var as integer
 */
function envInt(key, defaultValue) {
  const raw = env(key);
  if (raw === undefined) return defaultValue;
  const parsed = parseInt(raw, 10);
  return Number.isNaN(parsed) ? defaultValue : parsed;
}

/**
 * Read env var as boolean
 */
function envBool(key, defaultValue = false) {
  const raw = env(key);
  if (raw === undefined) return defaultValue;
  return ['true', '1', 'yes'].includes(raw.toLowerCase());
}

// ── Fail-fast validation ────────────────────────────────────
if (IS_PROD) {
  const missing = REQUIRED_IN_PROD.filter(key => !process.env[key]);
  if (missing.length > 0) {
    console.error(`[FATAL] Missing required environment variables in production: ${missing.join(', ')}`);
    process.exit(1);
  }
}

// ── JWT Secret ──────────────────────────────────────────────
// In dev/test, generate a random secret if not provided
const jwtSecret = env('JWT_SECRET') || (IS_PROD ? null : crypto.randomBytes(32).toString('hex'));

if (IS_PROD && (!jwtSecret || jwtSecret.length < 32)) {
  console.error('[FATAL] JWT_SECRET must be at least 32 characters in production');
  process.exit(1);
}

// ============================================================
// Export configuration object
// ============================================================

const config = {
  // ── Environment ───────────────────────────────────────────
  env: ENV,
  isProd: IS_PROD,
  isTest: IS_TEST,
  isDev: !IS_PROD && !IS_TEST,

  // ── Server ────────────────────────────────────────────────
  port: envInt('PORT', 3050),
  host: env('HOST', '0.0.0.0'),

  // ── Database (PostgreSQL) ─────────────────────────────────
  databaseUrl: env('DATABASE_URL', 'postgresql://localhost:5432/sovereign_core'),
  dbPoolMin: envInt('DB_POOL_MIN', 2),
  dbPoolMax: envInt('DB_POOL_MAX', 20),
  dbIdleTimeout: envInt('DB_IDLE_TIMEOUT', 30000),
  dbConnectionTimeout: envInt('DB_CONNECTION_TIMEOUT', 5000),
  dbStatementTimeout: envInt('DB_STATEMENT_TIMEOUT', 30000),

  // ── Authentication ────────────────────────────────────────
  jwtSecret,
  jwtExpiresIn: env('JWT_EXPIRES_IN', '24h'),
  jwtRefreshExpiresIn: env('JWT_REFRESH_EXPIRES_IN', '7d'),
  bcryptRounds: envInt('BCRYPT_ROUNDS', 12),

  // ── CORS ──────────────────────────────────────────────────
  corsOrigins: env('CORS_ORIGINS', 'http://localhost:3000,http://localhost:8080')
    .split(',')
    .map(s => s.trim())
    .filter(Boolean),

  // ── Rate Limiting ─────────────────────────────────────────
  rateLimitMax: envInt('RATE_LIMIT_MAX', 200),
  rateLimitWindowMs: envInt('RATE_LIMIT_WINDOW_MS', 15 * 60 * 1000),
  authRateLimitMax: envInt('AUTH_RATE_LIMIT_MAX', 10),

  // ── File Storage ──────────────────────────────────────────
  uploadDir: env('UPLOAD_DIR', './data/uploads'),
  maxFileSize: envInt('MAX_FILE_SIZE', 50 * 1024 * 1024), // 50 MB
  allowedMimeTypes: (env('ALLOWED_MIME_TYPES', 'image/jpeg,image/png,image/gif,image/webp,application/pdf,video/mp4'))
    .split(',')
    .map(s => s.trim()),

  // ── Email (Nodemailer) ────────────────────────────────────
  smtpHost: env('SMTP_HOST', 'localhost'),
  smtpPort: envInt('SMTP_PORT', 587),
  smtpSecure: envBool('SMTP_SECURE', false),
  smtpUser: env('SMTP_USER', ''),
  smtpPass: env('SMTP_PASS', ''),
  emailFrom: env('EMAIL_FROM', 'noreply@ierahkwa.gov'),

  // ── WebSocket ─────────────────────────────────────────────
  wsHeartbeatInterval: envInt('WS_HEARTBEAT_INTERVAL', 30000),
  wsMaxPayload: envInt('WS_MAX_PAYLOAD', 64 * 1024),

  // ── Logging ───────────────────────────────────────────────
  logLevel: env('LOG_LEVEL', IS_PROD ? 'info' : 'debug'),

  // ── Sovereign Identifiers ─────────────────────────────────
  sovereignNodeId: env('SOVEREIGN_NODE_ID', 'mameynode-core-001'),
  blockchainRpc: env('BLOCKCHAIN_RPC', 'http://localhost:8545'),
  nationCode: env('NATION_CODE', 'IK'),

  // ── Feature Flags ─────────────────────────────────────────
  enableWebSocket: envBool('ENABLE_WEBSOCKET', true),
  enableAnalytics: envBool('ENABLE_ANALYTICS', true),
  enableAuditLog: envBool('ENABLE_AUDIT_LOG', true),
  enableEmailNotifications: envBool('ENABLE_EMAIL_NOTIFICATIONS', false),
  maintenanceMode: envBool('MAINTENANCE_MODE', false)
};

// Freeze in production to prevent accidental mutation
if (IS_PROD) Object.freeze(config);

module.exports = config;

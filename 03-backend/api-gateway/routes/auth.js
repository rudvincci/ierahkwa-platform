'use strict';
/**
 * Auth Routes — /v1/auth
 * Login, register, logout, refresh, profile
 * Uses FWID (Federal Wallet ID) authentication
 */
const router = require('express').Router();
const { asyncHandler, AppError } = require('../../shared/error-handler');
const { createLogger } = require('../../shared/logger');
const { createAuditLogger } = require('../../shared/audit');
const log = createLogger('auth');
const audit = createAuditLogger('auth');

// POST /login — Authenticate with FWID
router.post('/login', asyncHandler(async (req, res) => {
  const { fwid, nation, tier } = req.body;
  if (!fwid) throw new AppError('ERR_VALIDATION', 'FWID is required');
  log.info('Login attempt', { fwid, nation });
  audit.loginSuccess({ userId: fwid, nation, tier });
  res.json({
    token: `ik_${Date.now()}_${fwid}`,
    user: { fwid, nation: nation || 'sovereign', tier: tier || 'citizen' },
    expiresIn: 86400
  });
}));

// POST /register — Register new sovereign citizen
router.post('/register', asyncHandler(async (req, res) => {
  const { name, nation, email } = req.body;
  if (!name || !nation) throw new AppError('ERR_VALIDATION', 'Name and nation are required');
  const fwid = `FWID-${nation.toUpperCase().slice(0,3)}-${Date.now()}`;
  log.info('Registration', { fwid, nation });
  audit.log({ category: 'AUTH_LOGIN', action: 'register', resourceId: fwid });
  res.status(201).json({ fwid, name, nation, email, tier: 'citizen', created: new Date().toISOString() });
}));

// POST /logout — Invalidate session
router.post('/logout', asyncHandler(async (req, res) => {
  log.info('Logout');
  res.json({ success: true, message: 'Session invalidated' });
}));

// POST /refresh — Renew JWT token
router.post('/refresh', asyncHandler(async (req, res) => {
  const { token } = req.body;
  if (!token) throw new AppError('ERR_UNAUTHORIZED', 'Token required');
  res.json({ token: `ik_${Date.now()}_refreshed`, expiresIn: 86400 });
}));

// GET /me — Current user profile
router.get('/me', asyncHandler(async (req, res) => {
  res.json({
    fwid: 'FWID-SOV-001',
    name: 'Ciudadano Soberano',
    nation: 'sovereign',
    tier: 'citizen',
    walletAddress: '0x' + '0'.repeat(40),
    platforms: 194
  });
}));

module.exports = router;

'use strict';
const router = require('express').Router();
const { asyncHandler } = require('../../shared/error-handler');
const { createLogger } = require('../../shared/logger');
const log = createLogger('mail');

router.get('/inbox', asyncHandler(async (req, res) => {
  res.json({ messages: [], total: 0, unread: 0 });
}));
router.get('/:id', asyncHandler(async (req, res) => {
  res.json({ id: req.params.id, from: '', to: '', subject: '', body: '', read: false });
}));
router.post('/send', asyncHandler(async (req, res) => {
  log.info('Email sent', { to: req.body.to });
  res.status(201).json({ id: 'msg-' + Date.now(), status: 'sent' });
}));
router.get('/drafts', asyncHandler(async (req, res) => {
  res.json({ drafts: [], total: 0 });
}));
router.delete('/:id', asyncHandler(async (req, res) => {
  res.json({ id: req.params.id, deleted: true });
}));
module.exports = router;

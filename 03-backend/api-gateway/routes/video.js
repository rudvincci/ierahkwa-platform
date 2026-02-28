'use strict';
const router = require('express').Router();
const { asyncHandler } = require('../../shared/error-handler');
const { createLogger } = require('../../shared/logger');
const log = createLogger('video');

router.get('/', asyncHandler(async (req, res) => {
  log.info('List request', { page: req.query.page });
  res.json({ data: [], total: 0, page: parseInt(req.query.page) || 1, limit: 20 });
}));
router.get('/:id', asyncHandler(async (req, res) => {
  res.json({ id: req.params.id, status: 'active' });
}));
router.post('/', asyncHandler(async (req, res) => {
  log.info('Create', { body: Object.keys(req.body) });
  res.status(201).json({ id: Date.now().toString(36), ...req.body, created: new Date().toISOString() });
}));
router.put('/:id', asyncHandler(async (req, res) => {
  res.json({ id: req.params.id, ...req.body, updated: new Date().toISOString() });
}));
router.delete('/:id', asyncHandler(async (req, res) => {
  res.json({ id: req.params.id, deleted: true });
}));
module.exports = router;

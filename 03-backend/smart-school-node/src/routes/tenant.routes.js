const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const config = require('../config/config');
const { Tenant } = require('../models');

const { ADMIN } = config.roles;

// Get all tenants (Admin only)
router.get('/', authorize(ADMIN), async (req, res, next) => {
  try {
    const tenants = await Tenant.findAll({ order: [['name', 'ASC']] });
    res.json({ success: true, data: tenants });
  } catch (error) {
    next(error);
  }
});

// Get tenant by ID
router.get('/:id', authorize(ADMIN), async (req, res, next) => {
  try {
    const tenant = await Tenant.findByPk(req.params.id);
    if (!tenant) {
      return res.status(404).json({ success: false, message: 'Tenant not found' });
    }
    res.json({ success: true, data: tenant });
  } catch (error) {
    next(error);
  }
});

// Create tenant (Admin only)
router.post('/', authorize(ADMIN), async (req, res, next) => {
  try {
    const tenant = await Tenant.create(req.body);
    res.status(201).json({ success: true, data: tenant });
  } catch (error) {
    next(error);
  }
});

// Update tenant
router.put('/:id', authorize(ADMIN), async (req, res, next) => {
  try {
    const tenant = await Tenant.findByPk(req.params.id);
    if (!tenant) {
      return res.status(404).json({ success: false, message: 'Tenant not found' });
    }
    await tenant.update(req.body);
    res.json({ success: true, data: tenant });
  } catch (error) {
    next(error);
  }
});

// Delete tenant (Admin only)
router.delete('/:id', authorize(ADMIN), async (req, res, next) => {
  try {
    const tenant = await Tenant.findByPk(req.params.id);
    if (!tenant) {
      return res.status(404).json({ success: false, message: 'Tenant not found' });
    }
    await tenant.destroy();
    res.json({ success: true, message: 'Tenant deleted successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;

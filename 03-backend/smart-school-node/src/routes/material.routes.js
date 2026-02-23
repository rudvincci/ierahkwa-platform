const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Material } = require('../models');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const materials = await Material.findAll({
      where: addTenantFilter(req),
      order: [['name', 'ASC']]
    });
    res.json({ success: true, data: materials });
  } catch (error) {
    next(error);
  }
});

router.get('/:id', async (req, res, next) => {
  try {
    const material = await Material.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) }
    });
    if (!material) {
      return res.status(404).json({ success: false, message: 'Material not found' });
    }
    res.json({ success: true, data: material });
  } catch (error) {
    next(error);
  }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const material = await Material.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: material });
  } catch (error) {
    next(error);
  }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const material = await Material.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!material) {
      return res.status(404).json({ success: false, message: 'Material not found' });
    }
    await material.update(req.body);
    res.json({ success: true, data: material });
  } catch (error) {
    next(error);
  }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const material = await Material.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!material) {
      return res.status(404).json({ success: false, message: 'Material not found' });
    }
    await material.destroy();
    res.json({ success: true, message: 'Material deleted successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;

const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Fees, SchoolYear } = require('../models');

const { ADMIN, SCHOOL_ADMIN, ACCOUNTANT } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const fees = await Fees.findAll({
      where: addTenantFilter(req),
      include: [{ model: SchoolYear, attributes: ['id', 'name'] }],
      order: [['name', 'ASC']]
    });
    res.json({ success: true, data: fees });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const fee = await Fees.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: SchoolYear }]
    });
    if (!fee) return res.status(404).json({ success: false, message: 'Fee not found' });
    res.json({ success: true, data: fee });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const fee = await Fees.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: fee });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const fee = await Fees.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!fee) return res.status(404).json({ success: false, message: 'Fee not found' });
    await fee.update(req.body);
    res.json({ success: true, data: fee });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const fee = await Fees.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!fee) return res.status(404).json({ success: false, message: 'Fee not found' });
    await fee.destroy();
    res.json({ success: true, message: 'Fee deleted successfully' });
  } catch (error) { next(error); }
});

module.exports = router;

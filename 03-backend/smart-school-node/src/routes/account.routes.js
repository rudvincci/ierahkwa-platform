const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Account } = require('../models');

const { ADMIN, SCHOOL_ADMIN, ACCOUNTANT } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const accounts = await Account.findAll({
      where: addTenantFilter(req),
      order: [['code', 'ASC']]
    });
    res.json({ success: true, data: accounts });
  } catch (error) { next(error); }
});

router.get('/tree', async (req, res, next) => {
  try {
    const accounts = await Account.findAll({
      where: { ...addTenantFilter(req), parentId: null },
      include: [{ model: Account, as: 'children', include: [{ model: Account, as: 'children' }] }],
      order: [['code', 'ASC']]
    });
    res.json({ success: true, data: accounts });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const account = await Account.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: Account, as: 'children' }]
    });
    if (!account) return res.status(404).json({ success: false, message: 'Account not found' });
    res.json({ success: true, data: account });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    let level = 1;
    if (req.body.parentId) {
      const parent = await Account.findByPk(req.body.parentId);
      if (parent) level = parent.level + 1;
    }
    const account = await Account.create({ ...req.body, level, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: account });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const account = await Account.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!account) return res.status(404).json({ success: false, message: 'Account not found' });
    if (account.isSystemAccount) return res.status(400).json({ success: false, message: 'Cannot modify system account' });
    await account.update(req.body);
    res.json({ success: true, data: account });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const account = await Account.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!account) return res.status(404).json({ success: false, message: 'Account not found' });
    if (account.isSystemAccount) return res.status(400).json({ success: false, message: 'Cannot delete system account' });
    await account.destroy();
    res.json({ success: true, message: 'Account deleted successfully' });
  } catch (error) { next(error); }
});

module.exports = router;

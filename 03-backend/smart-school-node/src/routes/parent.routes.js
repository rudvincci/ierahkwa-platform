const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Parent, User, Role, UserRole, Student } = require('../models');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const parents = await Parent.findAll({
      where: addTenantFilter(req),
      include: [{ model: Student, through: { attributes: [] } }],
      order: [['firstName', 'ASC']]
    });
    res.json({ success: true, data: parents });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const parent = await Parent.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: Student, through: { attributes: [] } }]
    });
    if (!parent) return res.status(404).json({ success: false, message: 'Parent not found' });
    res.json({ success: true, data: parent });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const { username, password, email, firstName, lastName, phone, ...parentData } = req.body;
    const user = await User.create({ username, password, email, firstName, lastName, phone, tenantId: req.tenantId });
    const parentRole = await Role.findOne({ where: { name: config.roles.PARENT } });
    if (parentRole) await UserRole.create({ userId: user.id, roleId: parentRole.id });
    const parent = await Parent.create({ ...parentData, userId: user.id, firstName, lastName, email, phone, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: parent });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const parent = await Parent.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!parent) return res.status(404).json({ success: false, message: 'Parent not found' });
    await parent.update(req.body);
    res.json({ success: true, data: parent });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const parent = await Parent.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!parent) return res.status(404).json({ success: false, message: 'Parent not found' });
    await parent.destroy();
    res.json({ success: true, message: 'Parent deleted successfully' });
  } catch (error) { next(error); }
});

module.exports = router;

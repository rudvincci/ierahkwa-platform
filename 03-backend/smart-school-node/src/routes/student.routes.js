const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Student, User, Role, UserRole, ClassRoom, Grade, Parent } = require('../models');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const students = await Student.findAll({
      where: addTenantFilter(req),
      include: [
        { model: ClassRoom, include: [{ model: Grade, attributes: ['id', 'name'] }] },
        { model: Parent, through: { attributes: [] } }
      ],
      order: [['firstName', 'ASC']]
    });
    res.json({ success: true, data: students });
  } catch (error) {
    next(error);
  }
});

router.get('/by-classroom/:classRoomId', async (req, res, next) => {
  try {
    const students = await Student.findAll({
      where: { classRoomId: req.params.classRoomId, ...addTenantFilter(req) },
      order: [['firstName', 'ASC']]
    });
    res.json({ success: true, data: students });
  } catch (error) {
    next(error);
  }
});

router.get('/:id', async (req, res, next) => {
  try {
    const student = await Student.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [
        { model: ClassRoom, include: [{ model: Grade }] },
        { model: Parent, through: { attributes: [] } }
      ]
    });
    if (!student) {
      return res.status(404).json({ success: false, message: 'Student not found' });
    }
    res.json({ success: true, data: student });
  } catch (error) {
    next(error);
  }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const { username, password, email, firstName, lastName, phone, parentIds, ...studentData } = req.body;
    
    // Create user account
    const user = await User.create({
      username,
      password,
      email,
      firstName,
      lastName,
      phone,
      tenantId: req.tenantId
    });
    
    // Assign student role
    const studentRole = await Role.findOne({ where: { name: config.roles.STUDENT } });
    if (studentRole) {
      await UserRole.create({ userId: user.id, roleId: studentRole.id });
    }
    
    // Create student record
    const student = await Student.create({
      ...studentData,
      userId: user.id,
      firstName,
      lastName,
      email,
      phone,
      tenantId: req.tenantId
    });
    
    res.status(201).json({ success: true, data: student });
  } catch (error) {
    next(error);
  }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const student = await Student.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!student) {
      return res.status(404).json({ success: false, message: 'Student not found' });
    }
    await student.update(req.body);
    res.json({ success: true, data: student });
  } catch (error) {
    next(error);
  }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const student = await Student.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!student) {
      return res.status(404).json({ success: false, message: 'Student not found' });
    }
    await student.destroy();
    res.json({ success: true, message: 'Student deleted successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;

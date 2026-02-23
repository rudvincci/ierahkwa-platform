const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Teacher, User, Role, UserRole, Material, TeacherMaterial } = require('../models');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const teachers = await Teacher.findAll({
      where: addTenantFilter(req),
      include: [{ model: Material, through: { attributes: [] } }],
      order: [['firstName', 'ASC']]
    });
    res.json({ success: true, data: teachers });
  } catch (error) {
    next(error);
  }
});

router.get('/:id', async (req, res, next) => {
  try {
    const teacher = await Teacher.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: Material, through: { attributes: [] } }]
    });
    if (!teacher) {
      return res.status(404).json({ success: false, message: 'Teacher not found' });
    }
    res.json({ success: true, data: teacher });
  } catch (error) {
    next(error);
  }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const { username, password, email, firstName, lastName, phone, materialIds, ...teacherData } = req.body;
    
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
    
    // Assign teacher role
    const teacherRole = await Role.findOne({ where: { name: config.roles.TEACHER } });
    if (teacherRole) {
      await UserRole.create({ userId: user.id, roleId: teacherRole.id });
    }
    
    // Create teacher record
    const teacher = await Teacher.create({
      ...teacherData,
      userId: user.id,
      firstName,
      lastName,
      email,
      phone,
      tenantId: req.tenantId
    });
    
    // Assign materials
    if (materialIds && materialIds.length > 0) {
      for (const materialId of materialIds) {
        await TeacherMaterial.create({ teacherId: teacher.id, materialId, tenantId: req.tenantId });
      }
    }
    
    const createdTeacher = await Teacher.findByPk(teacher.id, {
      include: [{ model: Material, through: { attributes: [] } }]
    });
    
    res.status(201).json({ success: true, data: createdTeacher });
  } catch (error) {
    next(error);
  }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const teacher = await Teacher.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!teacher) {
      return res.status(404).json({ success: false, message: 'Teacher not found' });
    }
    
    const { materialIds, ...updateData } = req.body;
    await teacher.update(updateData);
    
    if (materialIds) {
      await TeacherMaterial.destroy({ where: { teacherId: teacher.id } });
      for (const materialId of materialIds) {
        await TeacherMaterial.create({ teacherId: teacher.id, materialId, tenantId: req.tenantId });
      }
    }
    
    const updatedTeacher = await Teacher.findByPk(teacher.id, {
      include: [{ model: Material, through: { attributes: [] } }]
    });
    
    res.json({ success: true, data: updatedTeacher });
  } catch (error) {
    next(error);
  }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const teacher = await Teacher.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!teacher) {
      return res.status(404).json({ success: false, message: 'Teacher not found' });
    }
    await teacher.destroy();
    res.json({ success: true, message: 'Teacher deleted successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;

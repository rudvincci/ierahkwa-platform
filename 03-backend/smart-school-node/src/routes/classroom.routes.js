const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { ClassRoom, Grade, Student } = require('../models');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

// Get all classrooms
router.get('/', async (req, res, next) => {
  try {
    const classrooms = await ClassRoom.findAll({
      where: addTenantFilter(req),
      include: [
        { model: Grade, attributes: ['id', 'name'] },
        { model: Student, attributes: ['id'] }
      ],
      order: [['name', 'ASC']]
    });
    
    const data = classrooms.map(c => ({
      ...c.toJSON(),
      gradeName: c.Grade?.name,
      studentCount: c.Students?.length || 0
    }));
    
    res.json({ success: true, data });
  } catch (error) {
    next(error);
  }
});

// Get by grade
router.get('/by-grade/:gradeId', async (req, res, next) => {
  try {
    const classrooms = await ClassRoom.findAll({
      where: { gradeId: req.params.gradeId, ...addTenantFilter(req) },
      order: [['name', 'ASC']]
    });
    res.json({ success: true, data: classrooms });
  } catch (error) {
    next(error);
  }
});

// Get by ID
router.get('/:id', async (req, res, next) => {
  try {
    const classroom = await ClassRoom.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [
        { model: Grade },
        { model: Student }
      ]
    });
    
    if (!classroom) {
      return res.status(404).json({ success: false, message: 'Classroom not found' });
    }
    
    res.json({ success: true, data: classroom });
  } catch (error) {
    next(error);
  }
});

// Create
router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const classroom = await ClassRoom.create({
      ...req.body,
      tenantId: req.tenantId
    });
    res.status(201).json({ success: true, data: classroom });
  } catch (error) {
    next(error);
  }
});

// Update
router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const classroom = await ClassRoom.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) }
    });
    
    if (!classroom) {
      return res.status(404).json({ success: false, message: 'Classroom not found' });
    }
    
    await classroom.update(req.body);
    res.json({ success: true, data: classroom });
  } catch (error) {
    next(error);
  }
});

// Delete
router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const classroom = await ClassRoom.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) }
    });
    
    if (!classroom) {
      return res.status(404).json({ success: false, message: 'Classroom not found' });
    }
    
    await classroom.destroy();
    res.json({ success: true, message: 'Classroom deleted successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;

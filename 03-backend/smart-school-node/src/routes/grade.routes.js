const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Grade, ClassRoom } = require('../models');

const { ADMIN, SCHOOL_ADMIN } = config.roles;

// Get all grades
router.get('/', async (req, res, next) => {
  try {
    const grades = await Grade.findAll({
      where: addTenantFilter(req),
      include: [{ model: ClassRoom, attributes: ['id'] }],
      order: [['orderIndex', 'ASC'], ['name', 'ASC']]
    });
    
    const data = grades.map(g => ({
      ...g.toJSON(),
      classRoomCount: g.ClassRooms?.length || 0
    }));
    
    res.json({ success: true, data });
  } catch (error) {
    next(error);
  }
});

// Get grade by ID
router.get('/:id', async (req, res, next) => {
  try {
    const grade = await Grade.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: ClassRoom }]
    });
    
    if (!grade) {
      return res.status(404).json({ success: false, message: 'Grade not found' });
    }
    
    res.json({ success: true, data: grade });
  } catch (error) {
    next(error);
  }
});

// Create grade
router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const grade = await Grade.create({
      ...req.body,
      tenantId: req.tenantId
    });
    
    res.status(201).json({ success: true, data: grade });
  } catch (error) {
    next(error);
  }
});

// Update grade
router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const grade = await Grade.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) }
    });
    
    if (!grade) {
      return res.status(404).json({ success: false, message: 'Grade not found' });
    }
    
    await grade.update(req.body);
    res.json({ success: true, data: grade });
  } catch (error) {
    next(error);
  }
});

// Delete grade
router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const grade = await Grade.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) }
    });
    
    if (!grade) {
      return res.status(404).json({ success: false, message: 'Grade not found' });
    }
    
    await grade.destroy();
    res.json({ success: true, message: 'Grade deleted successfully' });
  } catch (error) {
    next(error);
  }
});

module.exports = router;

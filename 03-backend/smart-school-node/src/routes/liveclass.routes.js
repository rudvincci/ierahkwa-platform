const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { LiveClass, LiveClassAttendance, Teacher, ClassRoom, Material, Student, ZoomSettings } = require('../models');

const { ADMIN, SCHOOL_ADMIN, TEACHER, STUDENT } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const classes = await LiveClass.findAll({
      where: addTenantFilter(req),
      include: [
        { model: Teacher, attributes: ['id', 'firstName', 'lastName'] },
        { model: ClassRoom, attributes: ['id', 'name'] },
        { model: Material, attributes: ['id', 'name'] }
      ],
      order: [['startDateTime', 'DESC']]
    });
    res.json({ success: true, data: classes });
  } catch (error) { next(error); }
});

router.get('/upcoming', async (req, res, next) => {
  try {
    const { Op } = require('sequelize');
    const classes = await LiveClass.findAll({
      where: {
        ...addTenantFilter(req),
        status: 'Scheduled',
        startDateTime: { [Op.gte]: new Date() }
      },
      include: [
        { model: Teacher, attributes: ['id', 'firstName', 'lastName'] },
        { model: ClassRoom, attributes: ['id', 'name'] },
        { model: Material, attributes: ['id', 'name'] }
      ],
      order: [['startDateTime', 'ASC']],
      limit: 10
    });
    res.json({ success: true, data: classes });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const liveClass = await LiveClass.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [
        { model: Teacher },
        { model: ClassRoom },
        { model: Material },
        { model: LiveClassAttendance, as: 'attendances', include: [{ model: Student }] }
      ]
    });
    if (!liveClass) return res.status(404).json({ success: false, message: 'Live class not found' });
    res.json({ success: true, data: liveClass });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    // Get teacher ID if teacher role
    let teacherId = req.body.teacherId;
    if (req.roles.includes(TEACHER)) {
      const teacher = await Teacher.findOne({ where: { userId: req.userId } });
      if (teacher) teacherId = teacher.id;
    }
    
    const liveClass = await LiveClass.create({
      ...req.body,
      teacherId,
      tenantId: req.tenantId
    });
    
    // TODO: Integrate with Zoom API to create meeting
    // This would require the zoomSettings configuration
    
    res.status(201).json({ success: true, data: liveClass });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const liveClass = await LiveClass.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!liveClass) return res.status(404).json({ success: false, message: 'Live class not found' });
    await liveClass.update(req.body);
    res.json({ success: true, data: liveClass });
  } catch (error) { next(error); }
});

router.post('/:id/start', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const liveClass = await LiveClass.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!liveClass) return res.status(404).json({ success: false, message: 'Live class not found' });
    await liveClass.update({ status: 'InProgress' });
    res.json({ success: true, data: liveClass });
  } catch (error) { next(error); }
});

router.post('/:id/end', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const liveClass = await LiveClass.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!liveClass) return res.status(404).json({ success: false, message: 'Live class not found' });
    await liveClass.update({ status: 'Completed' });
    res.json({ success: true, data: liveClass });
  } catch (error) { next(error); }
});

router.post('/:id/join', authorize(STUDENT), async (req, res, next) => {
  try {
    const student = await Student.findOne({ where: { userId: req.userId } });
    if (!student) return res.status(404).json({ success: false, message: 'Student not found' });
    
    let attendance = await LiveClassAttendance.findOne({
      where: { liveClassId: req.params.id, studentId: student.id }
    });
    
    if (!attendance) {
      attendance = await LiveClassAttendance.create({
        liveClassId: req.params.id,
        studentId: student.id,
        joinedAt: new Date(),
        tenantId: req.tenantId
      });
    } else {
      await attendance.update({ joinedAt: new Date() });
    }
    
    const liveClass = await LiveClass.findByPk(req.params.id);
    res.json({ success: true, data: { liveClass, attendance } });
  } catch (error) { next(error); }
});

router.post('/:id/leave', authorize(STUDENT), async (req, res, next) => {
  try {
    const student = await Student.findOne({ where: { userId: req.userId } });
    if (!student) return res.status(404).json({ success: false, message: 'Student not found' });
    
    const attendance = await LiveClassAttendance.findOne({
      where: { liveClassId: req.params.id, studentId: student.id }
    });
    
    if (attendance) {
      const duration = Math.ceil((new Date() - attendance.joinedAt) / (1000 * 60));
      await attendance.update({ leftAt: new Date(), durationMinutes: duration });
    }
    
    res.json({ success: true, message: 'Left class successfully' });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const liveClass = await LiveClass.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!liveClass) return res.status(404).json({ success: false, message: 'Live class not found' });
    await liveClass.destroy();
    res.json({ success: true, message: 'Live class deleted successfully' });
  } catch (error) { next(error); }
});

// Zoom Settings
router.get('/settings/zoom', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const settings = await ZoomSettings.findOne({ where: addTenantFilter(req) });
    res.json({ success: true, data: settings });
  } catch (error) { next(error); }
});

router.post('/settings/zoom', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    let settings = await ZoomSettings.findOne({ where: addTenantFilter(req) });
    if (settings) {
      await settings.update(req.body);
    } else {
      settings = await ZoomSettings.create({ ...req.body, tenantId: req.tenantId });
    }
    res.json({ success: true, data: settings });
  } catch (error) { next(error); }
});

module.exports = router;

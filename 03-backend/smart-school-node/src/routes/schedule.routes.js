const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Schedule, ClassRoom, Teacher, Material } = require('../models');

const { ADMIN, SCHOOL_ADMIN, TEACHER } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const schedules = await Schedule.findAll({
      where: addTenantFilter(req),
      include: [
        { model: ClassRoom, attributes: ['id', 'name'] },
        { model: Teacher, attributes: ['id', 'firstName', 'lastName'] },
        { model: Material, attributes: ['id', 'name'] }
      ],
      order: [['dayOfWeek', 'ASC'], ['startTime', 'ASC']]
    });
    res.json({ success: true, data: schedules });
  } catch (error) { next(error); }
});

router.get('/by-classroom/:classRoomId', async (req, res, next) => {
  try {
    const schedules = await Schedule.findAll({
      where: { classRoomId: req.params.classRoomId, ...addTenantFilter(req) },
      include: [{ model: Teacher }, { model: Material }],
      order: [['dayOfWeek', 'ASC'], ['startTime', 'ASC']]
    });
    res.json({ success: true, data: schedules });
  } catch (error) { next(error); }
});

router.get('/by-teacher/:teacherId', async (req, res, next) => {
  try {
    const schedules = await Schedule.findAll({
      where: { teacherId: req.params.teacherId, ...addTenantFilter(req) },
      include: [{ model: ClassRoom }, { model: Material }],
      order: [['dayOfWeek', 'ASC'], ['startTime', 'ASC']]
    });
    res.json({ success: true, data: schedules });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const schedule = await Schedule.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: schedule });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const schedule = await Schedule.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!schedule) return res.status(404).json({ success: false, message: 'Schedule not found' });
    await schedule.update(req.body);
    res.json({ success: true, data: schedule });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN), async (req, res, next) => {
  try {
    const schedule = await Schedule.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!schedule) return res.status(404).json({ success: false, message: 'Schedule not found' });
    await schedule.destroy();
    res.json({ success: true, message: 'Schedule deleted successfully' });
  } catch (error) { next(error); }
});

module.exports = router;

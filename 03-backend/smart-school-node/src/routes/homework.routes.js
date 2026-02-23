const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Homework, HomeworkContent, HomeworkQuestion, HomeworkAnswer, Teacher, Material, ClassRoom, Student } = require('../models');

const { ADMIN, SCHOOL_ADMIN, TEACHER, STUDENT } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const homeworks = await Homework.findAll({
      where: addTenantFilter(req),
      include: [
        { model: Teacher, attributes: ['id', 'firstName', 'lastName'] },
        { model: Material, attributes: ['id', 'name'] },
        { model: ClassRoom, attributes: ['id', 'name'] }
      ],
      order: [['dueDate', 'DESC']]
    });
    res.json({ success: true, data: homeworks });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const homework = await Homework.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [
        { model: Teacher },
        { model: Material },
        { model: ClassRoom },
        { model: HomeworkContent, as: 'contents' },
        { model: HomeworkQuestion, as: 'questions' }
      ]
    });
    if (!homework) return res.status(404).json({ success: false, message: 'Homework not found' });
    res.json({ success: true, data: homework });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const { contents, questions, ...homeworkData } = req.body;
    
    // Get teacher ID from user if teacher role
    let teacherId = req.body.teacherId;
    if (req.roles.includes(TEACHER)) {
      const teacher = await Teacher.findOne({ where: { userId: req.userId } });
      if (teacher) teacherId = teacher.id;
    }
    
    const homework = await Homework.create({ ...homeworkData, teacherId, tenantId: req.tenantId });
    
    if (contents) {
      for (const content of contents) {
        await HomeworkContent.create({ ...content, homeworkId: homework.id, tenantId: req.tenantId });
      }
    }
    
    if (questions) {
      for (const question of questions) {
        await HomeworkQuestion.create({ ...question, homeworkId: homework.id, tenantId: req.tenantId });
      }
    }
    
    const created = await Homework.findByPk(homework.id, {
      include: [{ model: HomeworkContent, as: 'contents' }, { model: HomeworkQuestion, as: 'questions' }]
    });
    
    res.status(201).json({ success: true, data: created });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const homework = await Homework.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!homework) return res.status(404).json({ success: false, message: 'Homework not found' });
    await homework.update(req.body);
    res.json({ success: true, data: homework });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const homework = await Homework.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!homework) return res.status(404).json({ success: false, message: 'Homework not found' });
    await homework.destroy();
    res.json({ success: true, message: 'Homework deleted successfully' });
  } catch (error) { next(error); }
});

// Submit homework answer (Student)
router.post('/:id/submit', authorize(STUDENT), async (req, res, next) => {
  try {
    const student = await Student.findOne({ where: { userId: req.userId } });
    if (!student) return res.status(404).json({ success: false, message: 'Student not found' });
    
    const { answers } = req.body;
    
    let homeworkAnswer = await HomeworkAnswer.findOne({
      where: { homeworkId: req.params.id, studentId: student.id }
    });
    
    if (!homeworkAnswer) {
      homeworkAnswer = await HomeworkAnswer.create({
        homeworkId: req.params.id,
        studentId: student.id,
        tenantId: req.tenantId
      });
    }
    
    // Save question answers
    const { QuestionAnswer } = require('../models');
    for (const answer of answers) {
      await QuestionAnswer.upsert({
        homeworkAnswerId: homeworkAnswer.id,
        questionId: answer.questionId,
        answer: answer.answer,
        filePath: answer.filePath,
        tenantId: req.tenantId
      });
    }
    
    res.json({ success: true, data: homeworkAnswer });
  } catch (error) { next(error); }
});

// Grade homework (Teacher)
router.post('/:id/grade/:answerId', authorize(ADMIN, SCHOOL_ADMIN, TEACHER), async (req, res, next) => {
  try {
    const { score, feedback, questionGrades } = req.body;
    
    const answer = await HomeworkAnswer.findByPk(req.params.answerId);
    if (!answer) return res.status(404).json({ success: false, message: 'Answer not found' });
    
    await answer.update({
      score,
      feedback,
      isGraded: true,
      gradedAt: new Date(),
      gradedBy: req.userId
    });
    
    res.json({ success: true, data: answer });
  } catch (error) { next(error); }
});

module.exports = router;

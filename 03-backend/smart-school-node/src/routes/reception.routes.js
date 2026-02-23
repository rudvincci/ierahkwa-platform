const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { AdmissionEnquiry, VisitorBook, PhoneLog, PostalDispatch, PostalReceive, Complain } = require('../models');

const { ADMIN, SCHOOL_ADMIN, RECEPTIONIST } = config.roles;

// Admission Enquiries
router.get('/enquiries', async (req, res, next) => {
  try {
    const enquiries = await AdmissionEnquiry.findAll({ where: addTenantFilter(req), order: [['enquiryDate', 'DESC']] });
    res.json({ success: true, data: enquiries });
  } catch (error) { next(error); }
});

router.post('/enquiries', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const enquiry = await AdmissionEnquiry.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: enquiry });
  } catch (error) { next(error); }
});

router.put('/enquiries/:id', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const enquiry = await AdmissionEnquiry.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!enquiry) return res.status(404).json({ success: false, message: 'Enquiry not found' });
    await enquiry.update(req.body);
    res.json({ success: true, data: enquiry });
  } catch (error) { next(error); }
});

// Visitor Book
router.get('/visitors', async (req, res, next) => {
  try {
    const visitors = await VisitorBook.findAll({ where: addTenantFilter(req), order: [['checkInTime', 'DESC']] });
    res.json({ success: true, data: visitors });
  } catch (error) { next(error); }
});

router.post('/visitors', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const visitor = await VisitorBook.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: visitor });
  } catch (error) { next(error); }
});

router.put('/visitors/:id/checkout', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const visitor = await VisitorBook.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!visitor) return res.status(404).json({ success: false, message: 'Visitor not found' });
    await visitor.update({ checkOutTime: new Date() });
    res.json({ success: true, data: visitor });
  } catch (error) { next(error); }
});

// Phone Logs
router.get('/phone-logs', async (req, res, next) => {
  try {
    const logs = await PhoneLog.findAll({ where: addTenantFilter(req), order: [['callDate', 'DESC']] });
    res.json({ success: true, data: logs });
  } catch (error) { next(error); }
});

router.post('/phone-logs', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const log = await PhoneLog.create({ ...req.body, receivedBy: req.userId, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: log });
  } catch (error) { next(error); }
});

// Postal Dispatch
router.get('/postal-dispatch', async (req, res, next) => {
  try {
    const dispatches = await PostalDispatch.findAll({ where: addTenantFilter(req), order: [['dispatchDate', 'DESC']] });
    res.json({ success: true, data: dispatches });
  } catch (error) { next(error); }
});

router.post('/postal-dispatch', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const dispatch = await PostalDispatch.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: dispatch });
  } catch (error) { next(error); }
});

// Postal Receive
router.get('/postal-receive', async (req, res, next) => {
  try {
    const receives = await PostalReceive.findAll({ where: addTenantFilter(req), order: [['receiveDate', 'DESC']] });
    res.json({ success: true, data: receives });
  } catch (error) { next(error); }
});

router.post('/postal-receive', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const receive = await PostalReceive.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: receive });
  } catch (error) { next(error); }
});

// Complains
router.get('/complains', async (req, res, next) => {
  try {
    const complains = await Complain.findAll({ where: addTenantFilter(req), order: [['complainDate', 'DESC']] });
    res.json({ success: true, data: complains });
  } catch (error) { next(error); }
});

router.post('/complains', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const count = await Complain.count({ where: { tenantId: req.tenantId } });
    const complainNumber = `CMP-${String(count + 1).padStart(6, '0')}`;
    const complain = await Complain.create({ ...req.body, complainNumber, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: complain });
  } catch (error) { next(error); }
});

router.put('/complains/:id', authorize(ADMIN, SCHOOL_ADMIN, RECEPTIONIST), async (req, res, next) => {
  try {
    const complain = await Complain.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!complain) return res.status(404).json({ success: false, message: 'Complain not found' });
    await complain.update(req.body);
    res.json({ success: true, data: complain });
  } catch (error) { next(error); }
});

module.exports = router;

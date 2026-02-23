const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Journal, JournalEntry, Account, CostCenter } = require('../models');

const { ADMIN, SCHOOL_ADMIN, ACCOUNTANT } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const journals = await Journal.findAll({
      where: addTenantFilter(req),
      include: [{ model: CostCenter, attributes: ['id', 'name'] }],
      order: [['journalDate', 'DESC']]
    });
    res.json({ success: true, data: journals });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const journal = await Journal.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [
        { model: JournalEntry, as: 'entries', include: [{ model: Account }, { model: CostCenter }] },
        { model: CostCenter }
      ]
    });
    if (!journal) return res.status(404).json({ success: false, message: 'Journal not found' });
    res.json({ success: true, data: journal });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const { entries, ...journalData } = req.body;
    
    // Calculate totals
    let totalDebit = 0;
    let totalCredit = 0;
    for (const entry of entries) {
      totalDebit += parseFloat(entry.debitAmount) || 0;
      totalCredit += parseFloat(entry.creditAmount) || 0;
    }
    
    // Validate balance
    if (Math.abs(totalDebit - totalCredit) > 0.01) {
      return res.status(400).json({ success: false, message: 'Journal entries must be balanced' });
    }
    
    // Generate journal number
    const count = await Journal.count({ where: { tenantId: req.tenantId } });
    const journalNumber = `JRN-${String(count + 1).padStart(6, '0')}`;
    
    const journal = await Journal.create({
      ...journalData,
      journalNumber,
      totalDebit,
      totalCredit,
      tenantId: req.tenantId
    });
    
    for (const entry of entries) {
      await JournalEntry.create({
        ...entry,
        journalId: journal.id,
        tenantId: req.tenantId
      });
    }
    
    const created = await Journal.findByPk(journal.id, {
      include: [{ model: JournalEntry, as: 'entries', include: [{ model: Account }] }]
    });
    
    res.status(201).json({ success: true, data: created });
  } catch (error) { next(error); }
});

router.post('/:id/post', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const journal = await Journal.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: JournalEntry, as: 'entries' }]
    });
    if (!journal) return res.status(404).json({ success: false, message: 'Journal not found' });
    
    // Update account balances
    for (const entry of journal.entries) {
      const account = await Account.findByPk(entry.accountId);
      if (account) {
        const balanceChange = parseFloat(entry.debitAmount) - parseFloat(entry.creditAmount);
        await account.update({ currentBalance: parseFloat(account.currentBalance) + balanceChange });
      }
    }
    
    await journal.update({ status: 'Posted', isPosted: true, postedAt: new Date(), postedBy: req.userId });
    res.json({ success: true, data: journal });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const journal = await Journal.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!journal) return res.status(404).json({ success: false, message: 'Journal not found' });
    if (journal.isPosted) return res.status(400).json({ success: false, message: 'Cannot delete posted journal' });
    await journal.destroy();
    res.json({ success: true, message: 'Journal deleted successfully' });
  } catch (error) { next(error); }
});

module.exports = router;

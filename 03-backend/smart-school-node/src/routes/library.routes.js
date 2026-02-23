const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Book, BookCategory, LibraryMember, BorrowTransaction } = require('../models');
const { Op } = require('sequelize');

const { ADMIN, SCHOOL_ADMIN, LIBRARIAN } = config.roles;

// Books
router.get('/books', async (req, res, next) => {
  try {
    const books = await Book.findAll({
      where: addTenantFilter(req),
      include: [{ model: BookCategory, attributes: ['id', 'name'] }],
      order: [['title', 'ASC']]
    });
    res.json({ success: true, data: books });
  } catch (error) { next(error); }
});

router.post('/books', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const book = await Book.create({ ...req.body, availableQuantity: req.body.quantity || 1, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: book });
  } catch (error) { next(error); }
});

router.put('/books/:id', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const book = await Book.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!book) return res.status(404).json({ success: false, message: 'Book not found' });
    await book.update(req.body);
    res.json({ success: true, data: book });
  } catch (error) { next(error); }
});

router.delete('/books/:id', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const book = await Book.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!book) return res.status(404).json({ success: false, message: 'Book not found' });
    await book.destroy();
    res.json({ success: true, message: 'Book deleted successfully' });
  } catch (error) { next(error); }
});

// Book Categories
router.get('/categories', async (req, res, next) => {
  try {
    const categories = await BookCategory.findAll({ where: addTenantFilter(req), order: [['name', 'ASC']] });
    res.json({ success: true, data: categories });
  } catch (error) { next(error); }
});

router.post('/categories', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const category = await BookCategory.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: category });
  } catch (error) { next(error); }
});

// Members
router.get('/members', async (req, res, next) => {
  try {
    const members = await LibraryMember.findAll({ where: addTenantFilter(req), order: [['name', 'ASC']] });
    res.json({ success: true, data: members });
  } catch (error) { next(error); }
});

router.post('/members', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const count = await LibraryMember.count({ where: { tenantId: req.tenantId } });
    const memberNumber = `LIB-${String(count + 1).padStart(6, '0')}`;
    const member = await LibraryMember.create({ ...req.body, memberNumber, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: member });
  } catch (error) { next(error); }
});

// Borrow/Return
router.get('/transactions', async (req, res, next) => {
  try {
    const transactions = await BorrowTransaction.findAll({
      where: addTenantFilter(req),
      include: [{ model: LibraryMember }, { model: Book }],
      order: [['borrowDate', 'DESC']]
    });
    res.json({ success: true, data: transactions });
  } catch (error) { next(error); }
});

router.post('/borrow', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const { memberId, bookId, dueDate } = req.body;
    
    const member = await LibraryMember.findByPk(memberId);
    if (!member) return res.status(404).json({ success: false, message: 'Member not found' });
    if (member.currentBorrowCount >= member.maxBooksAllowed) {
      return res.status(400).json({ success: false, message: 'Maximum borrow limit reached' });
    }
    
    const book = await Book.findByPk(bookId);
    if (!book) return res.status(404).json({ success: false, message: 'Book not found' });
    if (book.availableQuantity < 1) {
      return res.status(400).json({ success: false, message: 'Book not available' });
    }
    
    const count = await BorrowTransaction.count({ where: { tenantId: req.tenantId } });
    const transactionNumber = `BRW-${String(count + 1).padStart(6, '0')}`;
    
    const transaction = await BorrowTransaction.create({
      transactionNumber,
      memberId,
      bookId,
      dueDate,
      issuedBy: req.userId,
      tenantId: req.tenantId
    });
    
    await book.update({ availableQuantity: book.availableQuantity - 1 });
    await member.update({ currentBorrowCount: member.currentBorrowCount + 1 });
    
    res.status(201).json({ success: true, data: transaction });
  } catch (error) { next(error); }
});

router.post('/return/:id', authorize(ADMIN, SCHOOL_ADMIN, LIBRARIAN), async (req, res, next) => {
  try {
    const transaction = await BorrowTransaction.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: LibraryMember }, { model: Book }]
    });
    
    if (!transaction) return res.status(404).json({ success: false, message: 'Transaction not found' });
    
    // Calculate fine if overdue
    let fineAmount = 0;
    const today = new Date();
    const dueDate = new Date(transaction.dueDate);
    if (today > dueDate) {
      const daysOverdue = Math.ceil((today - dueDate) / (1000 * 60 * 60 * 24));
      fineAmount = daysOverdue * 1; // $1 per day
    }
    
    await transaction.update({
      status: 'Returned',
      returnDate: today,
      fineAmount,
      returnedTo: req.userId
    });
    
    await transaction.Book.update({ availableQuantity: transaction.Book.availableQuantity + 1 });
    await transaction.LibraryMember.update({ currentBorrowCount: Math.max(0, transaction.LibraryMember.currentBorrowCount - 1) });
    
    res.json({ success: true, data: transaction, fineAmount });
  } catch (error) { next(error); }
});

router.get('/overdue', async (req, res, next) => {
  try {
    const transactions = await BorrowTransaction.findAll({
      where: {
        ...addTenantFilter(req),
        status: 'Borrowed',
        dueDate: { [Op.lt]: new Date() }
      },
      include: [{ model: LibraryMember }, { model: Book }]
    });
    res.json({ success: true, data: transactions });
  } catch (error) { next(error); }
});

module.exports = router;

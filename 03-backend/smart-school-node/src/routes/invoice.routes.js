const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Invoice, InvoiceItem, InvoicePayment, Student, Supplier, Fees, Product, Tenant } = require('../models');

const { ADMIN, SCHOOL_ADMIN, ACCOUNTANT } = config.roles;

// Generate invoice number
const generateInvoiceNumber = async (tenantId, type) => {
  const prefix = type.includes('Fees') ? 'FEE' : 'PUR';
  const count = await Invoice.count({ where: { tenantId, type } });
  return `${prefix}-${String(count + 1).padStart(6, '0')}`;
};

router.get('/', async (req, res, next) => {
  try {
    const where = { ...addTenantFilter(req) };
    if (req.query.type) where.type = req.query.type;
    
    const invoices = await Invoice.findAll({
      where,
      include: [
        { model: Student, attributes: ['id', 'firstName', 'lastName'] },
        { model: Supplier, attributes: ['id', 'name'] }
      ],
      order: [['invoiceDate', 'DESC']]
    });
    res.json({ success: true, data: invoices });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const invoice = await Invoice.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [
        { model: Student },
        { model: Supplier },
        { model: InvoiceItem, as: 'items', include: [{ model: Product }, { model: Fees }] },
        { model: InvoicePayment, as: 'payments' }
      ]
    });
    if (!invoice) return res.status(404).json({ success: false, message: 'Invoice not found' });
    res.json({ success: true, data: invoice });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const { items, paidAmount, paymentMethod, ...invoiceData } = req.body;
    
    // Get tenant tax rate
    const tenant = await Tenant.findByPk(req.tenantId);
    const taxRate = tenant?.taxRate || 0;
    
    // Calculate totals
    let subTotal = 0;
    let taxAmount = 0;
    
    for (const item of items) {
      const itemTotal = item.quantity * item.unitPrice;
      const itemDiscount = itemTotal * (item.discountPercent || 0) / 100;
      const itemTax = (itemTotal - itemDiscount) * taxRate / 100;
      subTotal += itemTotal - itemDiscount;
      taxAmount += itemTax;
    }
    
    const totalAmount = subTotal + taxAmount - (invoiceData.discountAmount || 0);
    
    const invoice = await Invoice.create({
      ...invoiceData,
      invoiceNumber: await generateInvoiceNumber(req.tenantId, invoiceData.type),
      subTotal,
      taxAmount,
      totalAmount,
      paidAmount: paidAmount || 0,
      paymentStatus: paidAmount >= totalAmount ? 'Paid' : paidAmount > 0 ? 'PartiallyPaid' : 'Pending',
      tenantId: req.tenantId
    });
    
    // Create invoice items
    for (const item of items) {
      const itemTotal = item.quantity * item.unitPrice;
      const itemDiscount = itemTotal * (item.discountPercent || 0) / 100;
      const itemTax = (itemTotal - itemDiscount) * taxRate / 100;
      
      await InvoiceItem.create({
        invoiceId: invoice.id,
        productId: item.productId,
        feesId: item.feesId,
        description: item.description,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        discountPercent: item.discountPercent || 0,
        discountAmount: itemDiscount,
        taxPercent: taxRate,
        taxAmount: itemTax,
        totalAmount: itemTotal - itemDiscount + itemTax,
        tenantId: req.tenantId
      });
      
      // Update product stock if purchase
      if (item.productId && invoiceData.type === 'PurchaseInvoice') {
        await Product.increment('quantity', { by: item.quantity, where: { id: item.productId } });
      }
    }
    
    // Create payment if paid
    if (paidAmount > 0) {
      await InvoicePayment.create({
        invoiceId: invoice.id,
        amount: paidAmount,
        paymentMethod: paymentMethod || 'Cash',
        tenantId: req.tenantId
      });
    }
    
    const created = await Invoice.findByPk(invoice.id, {
      include: [{ model: InvoiceItem, as: 'items' }, { model: InvoicePayment, as: 'payments' }]
    });
    
    res.status(201).json({ success: true, data: created });
  } catch (error) { next(error); }
});

// Add payment to invoice
router.post('/:id/payment', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const invoice = await Invoice.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!invoice) return res.status(404).json({ success: false, message: 'Invoice not found' });
    
    const { amount, paymentMethod, reference, notes } = req.body;
    
    const payment = await InvoicePayment.create({
      invoiceId: invoice.id,
      amount,
      paymentMethod,
      reference,
      notes,
      tenantId: req.tenantId
    });
    
    const newPaidAmount = parseFloat(invoice.paidAmount) + parseFloat(amount);
    await invoice.update({
      paidAmount: newPaidAmount,
      paymentStatus: newPaidAmount >= invoice.totalAmount ? 'Paid' : 'PartiallyPaid'
    });
    
    res.json({ success: true, data: payment });
  } catch (error) { next(error); }
});

module.exports = router;

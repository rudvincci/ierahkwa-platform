const express = require('express');
const router = express.Router();
const { authorize } = require('../middleware/auth.middleware');
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { Product, Category, Unit } = require('../models');

const { ADMIN, SCHOOL_ADMIN, ACCOUNTANT } = config.roles;

router.get('/', async (req, res, next) => {
  try {
    const products = await Product.findAll({
      where: addTenantFilter(req),
      include: [
        { model: Category, attributes: ['id', 'name'] },
        { model: Unit, attributes: ['id', 'name', 'shortName'] }
      ],
      order: [['name', 'ASC']]
    });
    res.json({ success: true, data: products });
  } catch (error) { next(error); }
});

router.get('/low-stock', async (req, res, next) => {
  try {
    const { Op } = require('sequelize');
    const products = await Product.findAll({
      where: {
        ...addTenantFilter(req),
        quantity: { [Op.lte]: require('sequelize').col('minQuantity') }
      }
    });
    res.json({ success: true, data: products });
  } catch (error) { next(error); }
});

router.get('/:id', async (req, res, next) => {
  try {
    const product = await Product.findOne({
      where: { id: req.params.id, ...addTenantFilter(req) },
      include: [{ model: Category }, { model: Unit }]
    });
    if (!product) return res.status(404).json({ success: false, message: 'Product not found' });
    res.json({ success: true, data: product });
  } catch (error) { next(error); }
});

router.post('/', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const product = await Product.create({ ...req.body, tenantId: req.tenantId });
    res.status(201).json({ success: true, data: product });
  } catch (error) { next(error); }
});

router.put('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const product = await Product.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!product) return res.status(404).json({ success: false, message: 'Product not found' });
    await product.update(req.body);
    res.json({ success: true, data: product });
  } catch (error) { next(error); }
});

router.delete('/:id', authorize(ADMIN, SCHOOL_ADMIN, ACCOUNTANT), async (req, res, next) => {
  try {
    const product = await Product.findOne({ where: { id: req.params.id, ...addTenantFilter(req) } });
    if (!product) return res.status(404).json({ success: false, message: 'Product not found' });
    await product.destroy();
    res.json({ success: true, message: 'Product deleted successfully' });
  } catch (error) { next(error); }
});

module.exports = router;

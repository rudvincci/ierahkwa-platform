const express = require('express');
const { getDb } = require('../db');

const router = express.Router();

// Get all categories
router.get('/categories', (req, res) => {
  const db = getDb();
  const categories = db.prepare(`
    SELECT * FROM categories WHERE active = 1 ORDER BY sort_order, name
  `).all();
  res.json(categories);
});

// Get all items
router.get('/', (req, res) => {
  const db = getDb();
  const { category_id } = req.query;
  
  let query = `
    SELECT i.*, c.name as category_name, c.color as category_color
    FROM items i
    LEFT JOIN categories c ON i.category_id = c.id
    WHERE i.active = 1
  `;
  
  const params = [];
  if (category_id) {
    query += ' AND i.category_id = ?';
    params.push(category_id);
  }
  
  query += ' ORDER BY c.sort_order, i.name';
  
  const items = db.prepare(query).all(...params);
  res.json(items);
});

// Get single item
router.get('/:id', (req, res) => {
  const db = getDb();
  const item = db.prepare(`
    SELECT i.*, c.name as category_name
    FROM items i
    LEFT JOIN categories c ON i.category_id = c.id
    WHERE i.id = ?
  `).get(req.params.id);
  
  if (!item) {
    return res.status(404).json({ error: 'Item not found' });
  }
  res.json(item);
});

// Create item
router.post('/', (req, res) => {
  const db = getDb();
  const { name, name_ar, category_id, price, cost, tax_rate, image, barcode, stock } = req.body;
  
  if (!name || !price) {
    return res.status(400).json({ error: 'Name and price are required' });
  }

  const result = db.prepare(`
    INSERT INTO items (name, name_ar, category_id, price, cost, tax_rate, image, barcode, stock)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
  `).run(name, name_ar, category_id, price, cost || 0, tax_rate || 0, image, barcode, stock || -1);

  res.json({ id: result.lastInsertRowid, success: true });
});

// Update item
router.put('/:id', (req, res) => {
  const db = getDb();
  const { name, name_ar, category_id, price, cost, tax_rate, image, barcode, stock, active } = req.body;
  
  db.prepare(`
    UPDATE items SET
      name = COALESCE(?, name),
      name_ar = COALESCE(?, name_ar),
      category_id = COALESCE(?, category_id),
      price = COALESCE(?, price),
      cost = COALESCE(?, cost),
      tax_rate = COALESCE(?, tax_rate),
      image = COALESCE(?, image),
      barcode = COALESCE(?, barcode),
      stock = COALESCE(?, stock),
      active = COALESCE(?, active)
    WHERE id = ?
  `).run(name, name_ar, category_id, price, cost, tax_rate, image, barcode, stock, active, req.params.id);

  res.json({ success: true });
});

// Delete item (soft delete)
router.delete('/:id', (req, res) => {
  const db = getDb();
  db.prepare('UPDATE items SET active = 0 WHERE id = ?').run(req.params.id);
  res.json({ success: true });
});

// Category CRUD
router.post('/categories', (req, res) => {
  const db = getDb();
  const { name, name_ar, color, icon, sort_order } = req.body;
  
  const result = db.prepare(`
    INSERT INTO categories (name, name_ar, color, icon, sort_order)
    VALUES (?, ?, ?, ?, ?)
  `).run(name, name_ar, color || '#3498db', icon || 'folder', sort_order || 0);

  res.json({ id: result.lastInsertRowid, success: true });
});

router.put('/categories/:id', (req, res) => {
  const db = getDb();
  const { name, name_ar, color, icon, sort_order, active } = req.body;
  
  db.prepare(`
    UPDATE categories SET
      name = COALESCE(?, name),
      name_ar = COALESCE(?, name_ar),
      color = COALESCE(?, color),
      icon = COALESCE(?, icon),
      sort_order = COALESCE(?, sort_order),
      active = COALESCE(?, active)
    WHERE id = ?
  `).run(name, name_ar, color, icon, sort_order, active, req.params.id);

  res.json({ success: true });
});

router.delete('/categories/:id', (req, res) => {
  const db = getDb();
  db.prepare('UPDATE categories SET active = 0 WHERE id = ?').run(req.params.id);
  res.json({ success: true });
});

module.exports = router;

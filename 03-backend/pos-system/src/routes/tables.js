const express = require('express');
const { getDb } = require('../db');

const router = express.Router();

// Get all tables
router.get('/', (req, res) => {
  const db = getDb();
  const { floor } = req.query;
  
  let query = `
    SELECT t.*, o.order_number, o.total as order_total
    FROM tables t
    LEFT JOIN orders o ON t.current_order_id = o.id
    WHERE t.active = 1
  `;
  
  const params = [];
  if (floor) {
    query += ' AND t.floor = ?';
    params.push(floor);
  }
  
  query += ' ORDER BY t.name';
  
  const tables = db.prepare(query).all(...params);
  res.json(tables);
});

// Get single table
router.get('/:id', (req, res) => {
  const db = getDb();
  
  const table = db.prepare(`
    SELECT t.*, o.order_number, o.total as order_total, o.status as order_status
    FROM tables t
    LEFT JOIN orders o ON t.current_order_id = o.id
    WHERE t.id = ?
  `).get(req.params.id);
  
  if (!table) {
    return res.status(404).json({ error: 'Table not found' });
  }
  
  // Get current order items if there's an active order
  let orderItems = [];
  if (table.current_order_id) {
    orderItems = db.prepare(`
      SELECT * FROM order_items WHERE order_id = ?
    `).all(table.current_order_id);
  }
  
  res.json({ ...table, order_items: orderItems });
});

// Create table
router.post('/', (req, res) => {
  const db = getDb();
  const { name, capacity, pos_x, pos_y, width, height, shape, floor } = req.body;
  
  if (!name) {
    return res.status(400).json({ error: 'Table name is required' });
  }

  const result = db.prepare(`
    INSERT INTO tables (name, capacity, pos_x, pos_y, width, height, shape, floor)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?)
  `).run(name, capacity || 4, pos_x || 0, pos_y || 0, width || 100, height || 100, shape || 'rectangle', floor || 'main');

  res.json({ id: result.lastInsertRowid, success: true });
});

// Update table
router.put('/:id', (req, res) => {
  const db = getDb();
  const { name, capacity, pos_x, pos_y, width, height, shape, status, floor, active } = req.body;
  
  db.prepare(`
    UPDATE tables SET
      name = COALESCE(?, name),
      capacity = COALESCE(?, capacity),
      pos_x = COALESCE(?, pos_x),
      pos_y = COALESCE(?, pos_y),
      width = COALESCE(?, width),
      height = COALESCE(?, height),
      shape = COALESCE(?, shape),
      status = COALESCE(?, status),
      floor = COALESCE(?, floor),
      active = COALESCE(?, active)
    WHERE id = ?
  `).run(name, capacity, pos_x, pos_y, width, height, shape, status, floor, active, req.params.id);

  res.json({ success: true });
});

// Update table position (for drag and drop)
router.patch('/:id/position', (req, res) => {
  const db = getDb();
  const { pos_x, pos_y } = req.body;
  
  db.prepare(`
    UPDATE tables SET pos_x = ?, pos_y = ? WHERE id = ?
  `).run(pos_x, pos_y, req.params.id);

  res.json({ success: true });
});

// Update table status
router.patch('/:id/status', (req, res) => {
  const db = getDb();
  const { status } = req.body;
  
  const validStatuses = ['available', 'occupied', 'reserved', 'cleaning'];
  if (!validStatuses.includes(status)) {
    return res.status(400).json({ error: 'Invalid status' });
  }

  db.prepare(`
    UPDATE tables SET status = ? WHERE id = ?
  `).run(status, req.params.id);

  res.json({ success: true });
});

// Delete table (soft delete)
router.delete('/:id', (req, res) => {
  const db = getDb();
  
  // Check if table has active order
  const table = db.prepare('SELECT current_order_id FROM tables WHERE id = ?').get(req.params.id);
  if (table && table.current_order_id) {
    return res.status(400).json({ error: 'Cannot delete table with active order' });
  }

  db.prepare('UPDATE tables SET active = 0 WHERE id = ?').run(req.params.id);
  res.json({ success: true });
});

// Get floors
router.get('/floors/list', (req, res) => {
  const db = getDb();
  const floors = db.prepare(`
    SELECT DISTINCT floor FROM tables WHERE active = 1 ORDER BY floor
  `).all();
  res.json(floors.map(f => f.floor));
});

module.exports = router;

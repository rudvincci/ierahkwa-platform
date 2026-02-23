const express = require('express');
const { getDb } = require('../db');
const { v4: uuidv4 } = require('uuid');

const router = express.Router();

// Generate order number
function generateOrderNumber() {
  const date = new Date();
  const dateStr = date.toISOString().slice(0, 10).replace(/-/g, '');
  const random = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
  return `ORD-${dateStr}-${random}`;
}

// Get all orders
router.get('/', (req, res) => {
  const db = getDb();
  const { status, date, table_id } = req.query;
  
  let query = `
    SELECT o.*, t.name as table_name, u.full_name as user_name
    FROM orders o
    LEFT JOIN tables t ON o.table_id = t.id
    LEFT JOIN users u ON o.user_id = u.id
    WHERE 1=1
  `;
  
  const params = [];
  
  if (status) {
    query += ' AND o.status = ?';
    params.push(status);
  }
  
  if (date) {
    query += ' AND DATE(o.created_at) = ?';
    params.push(date);
  }
  
  if (table_id) {
    query += ' AND o.table_id = ?';
    params.push(table_id);
  }
  
  query += ' ORDER BY o.created_at DESC LIMIT 100';
  
  const orders = db.prepare(query).all(...params);
  res.json(orders);
});

// Get single order with items
router.get('/:id', (req, res) => {
  const db = getDb();
  
  const order = db.prepare(`
    SELECT o.*, t.name as table_name, u.full_name as user_name
    FROM orders o
    LEFT JOIN tables t ON o.table_id = t.id
    LEFT JOIN users u ON o.user_id = u.id
    WHERE o.id = ?
  `).get(req.params.id);
  
  if (!order) {
    return res.status(404).json({ error: 'Order not found' });
  }
  
  const items = db.prepare(`
    SELECT oi.*, i.name_ar
    FROM order_items oi
    LEFT JOIN items i ON oi.item_id = i.id
    WHERE oi.order_id = ?
  `).all(req.params.id);
  
  const payments = db.prepare(`
    SELECT * FROM payments WHERE order_id = ?
  `).all(req.params.id);
  
  res.json({ ...order, items, payments });
});

// Create order
router.post('/', (req, res) => {
  const db = getDb();
  const { table_id, customer_name, items, notes } = req.body;
  const user_id = req.session.user.id;
  
  if (!items || items.length === 0) {
    return res.status(400).json({ error: 'Order must have at least one item' });
  }

  const order_number = generateOrderNumber();
  
  // Calculate totals
  let subtotal = 0;
  let tax_amount = 0;
  
  items.forEach(item => {
    const itemTotal = item.price * item.quantity;
    const itemTax = itemTotal * (item.tax_rate || 0);
    subtotal += itemTotal;
    tax_amount += itemTax;
  });
  
  const total = subtotal + tax_amount;

  // Insert order
  const orderResult = db.prepare(`
    INSERT INTO orders (order_number, table_id, user_id, customer_name, subtotal, tax_amount, total, notes, status)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?, 'pending')
  `).run(order_number, table_id, user_id, customer_name, subtotal, tax_amount, total, notes);

  const orderId = orderResult.lastInsertRowid;

  // Insert order items
  const insertItem = db.prepare(`
    INSERT INTO order_items (order_id, item_id, item_name, quantity, unit_price, tax_rate, tax_amount, total, notes)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
  `);

  items.forEach(item => {
    const itemTotal = item.price * item.quantity;
    const itemTax = itemTotal * (item.tax_rate || 0);
    insertItem.run(orderId, item.id, item.name, item.quantity, item.price, item.tax_rate || 0, itemTax, itemTotal + itemTax, item.notes);
  });

  // Update table status if table assigned
  if (table_id) {
    db.prepare(`
      UPDATE tables SET status = 'occupied', current_order_id = ? WHERE id = ?
    `).run(orderId, table_id);
  }

  res.json({ id: orderId, order_number, success: true });
});

// Update order
router.put('/:id', (req, res) => {
  const db = getDb();
  const { status, notes, discount_amount, discount_type } = req.body;
  
  const order = db.prepare('SELECT * FROM orders WHERE id = ?').get(req.params.id);
  if (!order) {
    return res.status(404).json({ error: 'Order not found' });
  }

  // Recalculate total if discount changed
  let newTotal = order.total;
  if (discount_amount !== undefined) {
    if (discount_type === 'percentage') {
      newTotal = order.subtotal + order.tax_amount - (order.subtotal * discount_amount / 100);
    } else {
      newTotal = order.subtotal + order.tax_amount - discount_amount;
    }
  }

  db.prepare(`
    UPDATE orders SET
      status = COALESCE(?, status),
      notes = COALESCE(?, notes),
      discount_amount = COALESCE(?, discount_amount),
      discount_type = COALESCE(?, discount_type),
      total = ?,
      completed_at = CASE WHEN ? = 'completed' THEN CURRENT_TIMESTAMP ELSE completed_at END
    WHERE id = ?
  `).run(status, notes, discount_amount, discount_type, newTotal, status, req.params.id);

  // Free up table if order completed or cancelled
  if (status === 'completed' || status === 'cancelled') {
    if (order.table_id) {
      db.prepare(`
        UPDATE tables SET status = 'available', current_order_id = NULL WHERE id = ?
      `).run(order.table_id);
    }
  }

  res.json({ success: true });
});

// Add item to order
router.post('/:id/items', (req, res) => {
  const db = getDb();
  const { item_id, quantity, notes } = req.body;
  
  const order = db.prepare('SELECT * FROM orders WHERE id = ?').get(req.params.id);
  if (!order) {
    return res.status(404).json({ error: 'Order not found' });
  }

  const item = db.prepare('SELECT * FROM items WHERE id = ?').get(item_id);
  if (!item) {
    return res.status(404).json({ error: 'Item not found' });
  }

  const itemTotal = item.price * quantity;
  const itemTax = itemTotal * item.tax_rate;

  db.prepare(`
    INSERT INTO order_items (order_id, item_id, item_name, quantity, unit_price, tax_rate, tax_amount, total, notes)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
  `).run(req.params.id, item_id, item.name, quantity, item.price, item.tax_rate, itemTax, itemTotal + itemTax, notes);

  // Update order totals
  db.prepare(`
    UPDATE orders SET
      subtotal = subtotal + ?,
      tax_amount = tax_amount + ?,
      total = total + ?
    WHERE id = ?
  `).run(itemTotal, itemTax, itemTotal + itemTax, req.params.id);

  res.json({ success: true });
});

// Remove item from order
router.delete('/:id/items/:itemId', (req, res) => {
  const db = getDb();
  
  const orderItem = db.prepare('SELECT * FROM order_items WHERE id = ? AND order_id = ?').get(req.params.itemId, req.params.id);
  if (!orderItem) {
    return res.status(404).json({ error: 'Order item not found' });
  }

  const itemTotal = orderItem.unit_price * orderItem.quantity;
  const itemTax = orderItem.tax_amount;

  db.prepare('DELETE FROM order_items WHERE id = ?').run(req.params.itemId);

  // Update order totals
  db.prepare(`
    UPDATE orders SET
      subtotal = subtotal - ?,
      tax_amount = tax_amount - ?,
      total = total - ?
    WHERE id = ?
  `).run(itemTotal, itemTax, itemTotal + itemTax, req.params.id);

  res.json({ success: true });
});

// Process payment
router.post('/:id/pay', (req, res) => {
  const db = getDb();
  const { method, amount, reference } = req.body;
  
  const order = db.prepare('SELECT * FROM orders WHERE id = ?').get(req.params.id);
  if (!order) {
    return res.status(404).json({ error: 'Order not found' });
  }

  // Record payment
  db.prepare(`
    INSERT INTO payments (order_id, amount, method, reference)
    VALUES (?, ?, ?, ?)
  `).run(req.params.id, amount || order.total, method, reference);

  // Get total payments
  const totalPaid = db.prepare(`
    SELECT COALESCE(SUM(amount), 0) as total FROM payments WHERE order_id = ?
  `).get(req.params.id).total;

  // Update order payment status
  let paymentStatus = 'unpaid';
  if (totalPaid >= order.total) {
    paymentStatus = 'paid';
  } else if (totalPaid > 0) {
    paymentStatus = 'partial';
  }

  db.prepare(`
    UPDATE orders SET
      payment_method = ?,
      payment_status = ?,
      status = CASE WHEN ? = 'paid' THEN 'completed' ELSE status END,
      completed_at = CASE WHEN ? = 'paid' THEN CURRENT_TIMESTAMP ELSE completed_at END
    WHERE id = ?
  `).run(method, paymentStatus, paymentStatus, paymentStatus, req.params.id);

  // Free up table if paid
  if (paymentStatus === 'paid' && order.table_id) {
    db.prepare(`
      UPDATE tables SET status = 'available', current_order_id = NULL WHERE id = ?
    `).run(order.table_id);
  }

  res.json({ 
    success: true, 
    payment_status: paymentStatus,
    total_paid: totalPaid,
    remaining: order.total - totalPaid
  });
});

module.exports = router;

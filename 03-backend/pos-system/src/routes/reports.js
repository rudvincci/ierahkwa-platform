const express = require('express');
const { getDb } = require('../db');

const router = express.Router();

// Daily sales report
router.get('/daily', (req, res) => {
  const db = getDb();
  const { date } = req.query;
  const targetDate = date || new Date().toISOString().slice(0, 10);
  
  const summary = db.prepare(`
    SELECT 
      COUNT(*) as total_orders,
      COALESCE(SUM(CASE WHEN payment_status = 'paid' THEN 1 ELSE 0 END), 0) as paid_orders,
      COALESCE(SUM(subtotal), 0) as subtotal,
      COALESCE(SUM(tax_amount), 0) as tax_total,
      COALESCE(SUM(discount_amount), 0) as discount_total,
      COALESCE(SUM(CASE WHEN payment_status = 'paid' THEN total ELSE 0 END), 0) as revenue
    FROM orders
    WHERE DATE(created_at) = ?
  `).get(targetDate);

  const byPaymentMethod = db.prepare(`
    SELECT 
      payment_method,
      COUNT(*) as count,
      SUM(total) as amount
    FROM orders
    WHERE DATE(created_at) = ? AND payment_status = 'paid'
    GROUP BY payment_method
  `).all(targetDate);

  const topItems = db.prepare(`
    SELECT 
      oi.item_name,
      SUM(oi.quantity) as quantity,
      SUM(oi.total) as revenue
    FROM order_items oi
    JOIN orders o ON oi.order_id = o.id
    WHERE DATE(o.created_at) = ?
    GROUP BY oi.item_id, oi.item_name
    ORDER BY quantity DESC
    LIMIT 10
  `).all(targetDate);

  const hourlyBreakdown = db.prepare(`
    SELECT 
      strftime('%H', created_at) as hour,
      COUNT(*) as orders,
      SUM(total) as revenue
    FROM orders
    WHERE DATE(created_at) = ? AND payment_status = 'paid'
    GROUP BY strftime('%H', created_at)
    ORDER BY hour
  `).all(targetDate);

  res.json({
    date: targetDate,
    summary,
    by_payment_method: byPaymentMethod,
    top_items: topItems,
    hourly_breakdown: hourlyBreakdown
  });
});

// Sales by date range
router.get('/sales', (req, res) => {
  const db = getDb();
  const { start_date, end_date } = req.query;
  
  const today = new Date().toISOString().slice(0, 10);
  const startDate = start_date || today;
  const endDate = end_date || today;

  const dailySales = db.prepare(`
    SELECT 
      DATE(created_at) as date,
      COUNT(*) as orders,
      SUM(subtotal) as subtotal,
      SUM(tax_amount) as tax,
      SUM(discount_amount) as discount,
      SUM(CASE WHEN payment_status = 'paid' THEN total ELSE 0 END) as revenue
    FROM orders
    WHERE DATE(created_at) BETWEEN ? AND ?
    GROUP BY DATE(created_at)
    ORDER BY date
  `).all(startDate, endDate);

  const totals = db.prepare(`
    SELECT 
      COUNT(*) as total_orders,
      SUM(subtotal) as subtotal,
      SUM(tax_amount) as tax,
      SUM(discount_amount) as discount,
      SUM(CASE WHEN payment_status = 'paid' THEN total ELSE 0 END) as revenue
    FROM orders
    WHERE DATE(created_at) BETWEEN ? AND ?
  `).get(startDate, endDate);

  res.json({
    start_date: startDate,
    end_date: endDate,
    daily_sales: dailySales,
    totals
  });
});

// Items report
router.get('/items', (req, res) => {
  const db = getDb();
  const { start_date, end_date, category_id } = req.query;
  
  const today = new Date().toISOString().slice(0, 10);
  const startDate = start_date || today;
  const endDate = end_date || today;

  let query = `
    SELECT 
      i.id,
      i.name,
      i.name_ar,
      c.name as category,
      COALESCE(SUM(oi.quantity), 0) as quantity_sold,
      COALESCE(SUM(oi.total), 0) as revenue,
      i.price,
      i.cost,
      (i.price - i.cost) * COALESCE(SUM(oi.quantity), 0) as profit
    FROM items i
    LEFT JOIN categories c ON i.category_id = c.id
    LEFT JOIN order_items oi ON i.id = oi.item_id
    LEFT JOIN orders o ON oi.order_id = o.id AND DATE(o.created_at) BETWEEN ? AND ?
    WHERE i.active = 1
  `;

  const params = [startDate, endDate];

  if (category_id) {
    query += ' AND i.category_id = ?';
    params.push(category_id);
  }

  query += ' GROUP BY i.id ORDER BY quantity_sold DESC';

  const items = db.prepare(query).all(...params);

  res.json({
    start_date: startDate,
    end_date: endDate,
    items
  });
});

// Category report
router.get('/categories', (req, res) => {
  const db = getDb();
  const { start_date, end_date } = req.query;
  
  const today = new Date().toISOString().slice(0, 10);
  const startDate = start_date || today;
  const endDate = end_date || today;

  const categories = db.prepare(`
    SELECT 
      c.id,
      c.name,
      c.name_ar,
      c.color,
      COUNT(DISTINCT oi.order_id) as orders,
      COALESCE(SUM(oi.quantity), 0) as items_sold,
      COALESCE(SUM(oi.total), 0) as revenue
    FROM categories c
    LEFT JOIN items i ON c.id = i.category_id
    LEFT JOIN order_items oi ON i.id = oi.item_id
    LEFT JOIN orders o ON oi.order_id = o.id AND DATE(o.created_at) BETWEEN ? AND ?
    WHERE c.active = 1
    GROUP BY c.id
    ORDER BY revenue DESC
  `).all(startDate, endDate);

  res.json({
    start_date: startDate,
    end_date: endDate,
    categories
  });
});

// User performance report
router.get('/users', (req, res) => {
  const db = getDb();
  const { start_date, end_date } = req.query;
  
  const today = new Date().toISOString().slice(0, 10);
  const startDate = start_date || today;
  const endDate = end_date || today;

  const users = db.prepare(`
    SELECT 
      u.id,
      u.username,
      u.full_name,
      COUNT(o.id) as orders,
      COALESCE(SUM(o.total), 0) as total_sales,
      COALESCE(AVG(o.total), 0) as avg_order_value
    FROM users u
    LEFT JOIN orders o ON u.id = o.user_id AND DATE(o.created_at) BETWEEN ? AND ?
    WHERE u.active = 1
    GROUP BY u.id
    ORDER BY total_sales DESC
  `).all(startDate, endDate);

  res.json({
    start_date: startDate,
    end_date: endDate,
    users
  });
});

// Table utilization report
router.get('/tables', (req, res) => {
  const db = getDb();
  const { start_date, end_date } = req.query;
  
  const today = new Date().toISOString().slice(0, 10);
  const startDate = start_date || today;
  const endDate = end_date || today;

  const tables = db.prepare(`
    SELECT 
      t.id,
      t.name,
      t.capacity,
      COUNT(o.id) as orders,
      COALESCE(SUM(o.total), 0) as revenue,
      COALESCE(AVG(o.total), 0) as avg_order_value
    FROM tables t
    LEFT JOIN orders o ON t.id = o.table_id AND DATE(o.created_at) BETWEEN ? AND ?
    WHERE t.active = 1
    GROUP BY t.id
    ORDER BY revenue DESC
  `).all(startDate, endDate);

  res.json({
    start_date: startDate,
    end_date: endDate,
    tables
  });
});

// Tax report
router.get('/tax', (req, res) => {
  const db = getDb();
  const { start_date, end_date } = req.query;
  
  const today = new Date().toISOString().slice(0, 10);
  const startDate = start_date || today;
  const endDate = end_date || today;

  const summary = db.prepare(`
    SELECT 
      COUNT(*) as total_orders,
      SUM(subtotal) as taxable_amount,
      SUM(tax_amount) as tax_collected,
      SUM(total) as total_with_tax
    FROM orders
    WHERE DATE(created_at) BETWEEN ? AND ? AND payment_status = 'paid'
  `).get(startDate, endDate);

  const byDate = db.prepare(`
    SELECT 
      DATE(created_at) as date,
      SUM(subtotal) as taxable_amount,
      SUM(tax_amount) as tax_collected
    FROM orders
    WHERE DATE(created_at) BETWEEN ? AND ? AND payment_status = 'paid'
    GROUP BY DATE(created_at)
    ORDER BY date
  `).all(startDate, endDate);

  res.json({
    start_date: startDate,
    end_date: endDate,
    summary,
    by_date: byDate
  });
});

module.exports = router;

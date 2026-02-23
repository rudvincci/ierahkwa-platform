const express = require('express');
const router = express.Router();
const { getDb } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// Search products (for autocomplete)
router.get('/products/search', (req, res) => {
    const db = getDb();
    const { q } = req.query;

    if (!q || q.length < 1) {
        return res.json([]);
    }

    const products = db.prepare(`
        SELECT id, code, barcode, name, current_stock, purchase_price, sale_price, unit
        FROM products
        WHERE is_active = 1 AND (code LIKE ? OR barcode LIKE ? OR name LIKE ?)
        ORDER BY 
            CASE WHEN code LIKE ? THEN 0
                 WHEN barcode LIKE ? THEN 1
                 WHEN name LIKE ? THEN 2
                 ELSE 3 END,
            name
        LIMIT 15
    `).all(`%${q}%`, `%${q}%`, `%${q}%`, `${q}%`, `${q}%`, `${q}%`);

    res.json(products);
});

// Get product by barcode
router.get('/products/barcode/:barcode', (req, res) => {
    const db = getDb();
    const product = db.prepare(`
        SELECT id, code, barcode, name, current_stock, purchase_price, sale_price, unit
        FROM products
        WHERE is_active = 1 AND (barcode = ? OR code = ?)
    `).get(req.params.barcode, req.params.barcode);

    if (product) {
        res.json(product);
    } else {
        res.status(404).json({ error: 'Product not found' });
    }
});

// Get product details
router.get('/products/:id', (req, res) => {
    const db = getDb();
    const product = db.prepare(`
        SELECT p.*, c.name as category_name, s.name as supplier_name
        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        WHERE p.id = ?
    `).get(req.params.id);

    if (product) {
        res.json(product);
    } else {
        res.status(404).json({ error: 'Product not found' });
    }
});

// Dashboard stats
router.get('/stats', (req, res) => {
    const db = getDb();

    try {
        const stats = {
            totalProducts: db.prepare('SELECT COUNT(*) as count FROM products WHERE is_active = 1').get().count,
            totalValue: db.prepare('SELECT COALESCE(SUM(current_stock * purchase_price), 0) as total FROM products WHERE is_active = 1').get().total,
            lowStock: db.prepare('SELECT COUNT(*) as count FROM products WHERE is_active = 1 AND current_stock <= min_stock').get().count,
            todayMovements: db.prepare(`SELECT COUNT(*) as count FROM stock_movements WHERE DATE(created_at) = DATE('now')`).get().count
        };

        res.json(stats);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Get categories
router.get('/categories', (req, res) => {
    const db = getDb();
    const categories = db.prepare('SELECT id, code, name FROM categories WHERE is_active = 1 ORDER BY name').all();
    res.json(categories);
});

// Get suppliers
router.get('/suppliers', (req, res) => {
    const db = getDb();
    const suppliers = db.prepare('SELECT id, code, name FROM suppliers WHERE is_active = 1 ORDER BY name').all();
    res.json(suppliers);
});

// Recent movements
router.get('/movements/recent', (req, res) => {
    const db = getDb();
    const movements = db.prepare(`
        SELECT m.*, p.name as product_name, p.code as product_code
        FROM stock_movements m
        JOIN products p ON m.product_id = p.id
        ORDER BY m.created_at DESC
        LIMIT 20
    `).all();
    res.json(movements);
});

// Check code availability
router.get('/check-code/:table/:code', (req, res) => {
    const db = getDb();
    const { table, code } = req.params;
    const { exclude } = req.query;

    const validTables = ['products', 'categories', 'suppliers'];
    if (!validTables.includes(table)) {
        return res.status(400).json({ error: 'Invalid table' });
    }

    let sql = `SELECT COUNT(*) as count FROM ${table} WHERE code = ?`;
    const params = [code];

    if (exclude) {
        sql += ` AND id != ?`;
        params.push(exclude);
    }

    const result = db.prepare(sql).get(...params);
    res.json({ available: result.count === 0 });
});

module.exports = router;

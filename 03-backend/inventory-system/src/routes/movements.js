const express = require('express');
const router = express.Router();
const { getDb, logActivity } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// List movements
router.get('/', (req, res) => {
    const db = getDb();
    const { type, start_date, end_date, product_id } = req.query;

    let sql = `
        SELECT m.*, p.name as product_name, p.code as product_code, u.full_name as user_name
        FROM stock_movements m
        JOIN products p ON m.product_id = p.id
        JOIN users u ON m.user_id = u.id
        WHERE 1=1
    `;
    const params = [];

    if (type) {
        sql += ` AND m.type = ?`;
        params.push(type);
    }

    if (start_date) {
        sql += ` AND DATE(m.created_at) >= ?`;
        params.push(start_date);
    }

    if (end_date) {
        sql += ` AND DATE(m.created_at) <= ?`;
        params.push(end_date);
    }

    if (product_id) {
        sql += ` AND m.product_id = ?`;
        params.push(product_id);
    }

    sql += ` ORDER BY m.created_at DESC LIMIT 500`;

    const movements = db.prepare(sql).all(...params);
    const products = db.prepare('SELECT id, code, name FROM products WHERE is_active = 1 ORDER BY name').all();

    res.render('movements/list', {
        title: 'Stock Movements',
        movements,
        products,
        filters: { type, start_date, end_date, product_id }
    });
});

// Stock In page
router.get('/stock-in', (req, res) => {
    const db = getDb();
    const products = db.prepare(`
        SELECT id, code, barcode, name, current_stock, purchase_price, unit 
        FROM products WHERE is_active = 1 ORDER BY name
    `).all();

    // Generate document number
    const today = new Date().toISOString().slice(0, 10).replace(/-/g, '');
    const lastDoc = db.prepare(`
        SELECT document_number FROM stock_movements 
        WHERE document_number LIKE ? ORDER BY id DESC LIMIT 1
    `).get(`PUR${today}%`);

    let docNumber = `PUR${today}001`;
    if (lastDoc) {
        const num = parseInt(lastDoc.document_number.slice(-3)) + 1;
        docNumber = `PUR${today}${num.toString().padStart(3, '0')}`;
    }

    res.render('movements/stock-in', {
        title: 'Stock In - Purchase',
        products: JSON.stringify(products),
        documentNumber: docNumber
    });
});

// Stock Out page
router.get('/stock-out', (req, res) => {
    const db = getDb();
    const products = db.prepare(`
        SELECT id, code, barcode, name, current_stock, sale_price, unit 
        FROM products WHERE is_active = 1 ORDER BY name
    `).all();

    // Generate document number
    const today = new Date().toISOString().slice(0, 10).replace(/-/g, '');
    const lastDoc = db.prepare(`
        SELECT document_number FROM stock_movements 
        WHERE document_number LIKE ? ORDER BY id DESC LIMIT 1
    `).get(`SAL${today}%`);

    let docNumber = `SAL${today}001`;
    if (lastDoc) {
        const num = parseInt(lastDoc.document_number.slice(-3)) + 1;
        docNumber = `SAL${today}${num.toString().padStart(3, '0')}`;
    }

    res.render('movements/stock-out', {
        title: 'Stock Out - Sale',
        products: JSON.stringify(products),
        documentNumber: docNumber
    });
});

// Adjustment page
router.get('/adjustment', (req, res) => {
    const db = getDb();
    const products = db.prepare(`
        SELECT id, code, barcode, name, current_stock, purchase_price, unit 
        FROM products WHERE is_active = 1 ORDER BY name
    `).all();

    const today = new Date().toISOString().slice(0, 10).replace(/-/g, '');
    const docNumber = `ADJ${today}${Date.now().toString().slice(-3)}`;

    res.render('movements/adjustment', {
        title: 'Stock Adjustment',
        products: JSON.stringify(products),
        documentNumber: docNumber
    });
});

// Process stock movement
router.post('/process', (req, res) => {
    const db = getDb();
    const { type, document_number, reference, items } = req.body;

    try {
        const parsedItems = JSON.parse(items);

        if (!parsedItems || parsedItems.length === 0) {
            return res.json({ success: false, message: 'No items to process' });
        }

        const insertMovement = db.prepare(`
            INSERT INTO stock_movements (product_id, type, quantity, previous_stock, new_stock,
                unit_price, total_value, reference, document_number, user_id)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `);

        const updateStock = db.prepare(`
            UPDATE products SET current_stock = ?, updated_at = CURRENT_TIMESTAMP WHERE id = ?
        `);

        const transaction = db.transaction(() => {
            for (const item of parsedItems) {
                const product = db.prepare('SELECT current_stock FROM products WHERE id = ?').get(item.product_id);
                
                if (!product) {
                    throw new Error(`Product ID ${item.product_id} not found`);
                }

                let newStock;
                if (type === 'purchase' || type === 'return') {
                    newStock = product.current_stock + item.quantity;
                } else if (type === 'sale' || type === 'damage') {
                    if (item.quantity > product.current_stock) {
                        throw new Error(`Insufficient stock for product ID ${item.product_id}`);
                    }
                    newStock = product.current_stock - item.quantity;
                } else if (type === 'adjustment') {
                    newStock = item.quantity;
                }

                insertMovement.run(
                    item.product_id,
                    type,
                    type === 'adjustment' ? item.quantity - product.current_stock : item.quantity,
                    product.current_stock,
                    newStock,
                    item.unit_price,
                    item.quantity * item.unit_price,
                    reference,
                    document_number,
                    req.session.user.id
                );

                updateStock.run(newStock, item.product_id);
            }
        });

        transaction();

        logActivity(req.session.user.id, 'STOCK_MOVEMENT', 'stock_movements', null, 
                   JSON.stringify({ type, document_number, items: parsedItems.length }), req.ip);

        res.json({ success: true, message: `Processed ${parsedItems.length} items successfully` });
    } catch (error) {
        console.error('Process movement error:', error);
        res.json({ success: false, message: error.message });
    }
});

// Quick entry page (barcode scanner)
router.get('/quick-entry', (req, res) => {
    const db = getDb();
    const products = db.prepare(`
        SELECT id, code, barcode, name, current_stock, purchase_price, sale_price, unit 
        FROM products WHERE is_active = 1 ORDER BY name
    `).all();

    res.render('movements/quick-entry', {
        title: 'Quick Stock Entry',
        products: JSON.stringify(products)
    });
});

module.exports = router;

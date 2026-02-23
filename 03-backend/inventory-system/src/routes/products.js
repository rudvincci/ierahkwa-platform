const express = require('express');
const router = express.Router();
const multer = require('multer');
const path = require('path');
const { getDb, generateCode, logActivity } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// Configure multer for image uploads
const storage = multer.diskStorage({
    destination: (req, file, cb) => {
        cb(null, path.join(__dirname, '../../public/uploads'));
    },
    filename: (req, file, cb) => {
        cb(null, `product-${Date.now()}${path.extname(file.originalname)}`);
    }
});
const upload = multer({ storage });

// List products
router.get('/', (req, res) => {
    const db = getDb();
    const { search, category, status } = req.query;

    let sql = `
        SELECT p.*, c.name as category_name, s.name as supplier_name
        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        WHERE 1=1
    `;
    const params = [];

    if (search) {
        sql += ` AND (p.code LIKE ? OR p.barcode LIKE ? OR p.name LIKE ?)`;
        params.push(`%${search}%`, `%${search}%`, `%${search}%`);
    }

    if (category) {
        sql += ` AND p.category_id = ?`;
        params.push(category);
    }

    if (status === 'low') {
        sql += ` AND p.current_stock <= p.min_stock`;
    } else if (status !== 'all') {
        sql += ` AND p.is_active = 1`;
    }

    sql += ` ORDER BY p.name`;

    const products = db.prepare(sql).all(...params);
    const categories = db.prepare('SELECT * FROM categories WHERE is_active = 1 ORDER BY name').all();

    res.render('products/list', {
        title: 'Products',
        products,
        categories,
        search: search || '',
        selectedCategory: category || '',
        selectedStatus: status || ''
    });
});

// New product form
router.get('/new', (req, res) => {
    const db = getDb();
    const categories = db.prepare('SELECT * FROM categories WHERE is_active = 1 ORDER BY name').all();
    const suppliers = db.prepare('SELECT * FROM suppliers WHERE is_active = 1 ORDER BY name').all();
    const newCode = generateCode('PRD', 'products');

    res.render('products/form', {
        title: 'New Product',
        product: { code: newCode },
        categories,
        suppliers,
        isNew: true
    });
});

// Create product
router.post('/new', upload.single('image'), (req, res) => {
    const db = getDb();
    const { code, barcode, name, description, category_id, supplier_id,
            purchase_price, sale_price, current_stock, min_stock, max_stock,
            unit, location, notes } = req.body;

    try {
        const image = req.file ? `/uploads/${req.file.filename}` : null;

        const result = db.prepare(`
            INSERT INTO products (code, barcode, name, description, category_id, supplier_id,
                purchase_price, sale_price, current_stock, min_stock, max_stock,
                unit, location, image, created_by, notes)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `).run(code, barcode, name, description, category_id || null, supplier_id || null,
               purchase_price || 0, sale_price || 0, current_stock || 0, min_stock || 0, max_stock || 0,
               unit || 'PCS', location, image, req.session.user.id, notes);

        // Create initial stock movement if stock > 0
        if (parseInt(current_stock) > 0) {
            db.prepare(`
                INSERT INTO stock_movements (product_id, type, quantity, previous_stock, new_stock,
                    unit_price, total_value, reference, user_id)
                VALUES (?, 'initial', ?, 0, ?, ?, ?, 'Initial stock entry', ?)
            `).run(result.lastInsertRowid, current_stock, current_stock, 
                   purchase_price || 0, (current_stock || 0) * (purchase_price || 0), req.session.user.id);
        }

        logActivity(req.session.user.id, 'CREATE', 'products', result.lastInsertRowid, 
                   JSON.stringify({ code, name }), req.ip);

        req.session.success = 'Product created successfully';
        res.redirect('/products');
    } catch (error) {
        console.error('Create product error:', error);
        req.session.error = 'Error creating product: ' + error.message;
        res.redirect('/products/new');
    }
});

// Edit product form
router.get('/:id/edit', (req, res) => {
    const db = getDb();
    const product = db.prepare('SELECT * FROM products WHERE id = ?').get(req.params.id);

    if (!product) {
        req.session.error = 'Product not found';
        return res.redirect('/products');
    }

    const categories = db.prepare('SELECT * FROM categories WHERE is_active = 1 ORDER BY name').all();
    const suppliers = db.prepare('SELECT * FROM suppliers WHERE is_active = 1 ORDER BY name').all();

    res.render('products/form', {
        title: 'Edit Product',
        product,
        categories,
        suppliers,
        isNew: false
    });
});

// Update product
router.post('/:id/edit', upload.single('image'), (req, res) => {
    const db = getDb();
    const { code, barcode, name, description, category_id, supplier_id,
            purchase_price, sale_price, min_stock, max_stock,
            unit, location, notes, is_active } = req.body;

    try {
        let sql = `
            UPDATE products SET 
                code = ?, barcode = ?, name = ?, description = ?, 
                category_id = ?, supplier_id = ?,
                purchase_price = ?, sale_price = ?, 
                min_stock = ?, max_stock = ?,
                unit = ?, location = ?, notes = ?, is_active = ?,
                updated_at = CURRENT_TIMESTAMP
        `;
        const params = [code, barcode, name, description, 
                       category_id || null, supplier_id || null,
                       purchase_price || 0, sale_price || 0, 
                       min_stock || 0, max_stock || 0,
                       unit || 'PCS', location, notes, is_active ? 1 : 0];

        if (req.file) {
            sql += `, image = ?`;
            params.push(`/uploads/${req.file.filename}`);
        }

        sql += ` WHERE id = ?`;
        params.push(req.params.id);

        db.prepare(sql).run(...params);

        logActivity(req.session.user.id, 'UPDATE', 'products', req.params.id, 
                   JSON.stringify({ code, name }), req.ip);

        req.session.success = 'Product updated successfully';
        res.redirect('/products');
    } catch (error) {
        console.error('Update product error:', error);
        req.session.error = 'Error updating product';
        res.redirect(`/products/${req.params.id}/edit`);
    }
});

// View product details
router.get('/:id', (req, res) => {
    const db = getDb();
    
    const product = db.prepare(`
        SELECT p.*, c.name as category_name, s.name as supplier_name, u.full_name as created_by_name
        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        LEFT JOIN users u ON p.created_by = u.id
        WHERE p.id = ?
    `).get(req.params.id);

    if (!product) {
        req.session.error = 'Product not found';
        return res.redirect('/products');
    }

    const movements = db.prepare(`
        SELECT m.*, u.full_name as user_name
        FROM stock_movements m
        JOIN users u ON m.user_id = u.id
        WHERE m.product_id = ?
        ORDER BY m.created_at DESC
        LIMIT 20
    `).all(req.params.id);

    res.render('products/view', {
        title: product.name,
        product,
        movements
    });
});

// Delete product
router.post('/:id/delete', (req, res) => {
    const db = getDb();

    try {
        db.prepare('UPDATE products SET is_active = 0 WHERE id = ?').run(req.params.id);
        logActivity(req.session.user.id, 'DELETE', 'products', req.params.id, null, req.ip);
        req.session.success = 'Product deleted successfully';
    } catch (error) {
        req.session.error = 'Error deleting product';
    }

    res.redirect('/products');
});

module.exports = router;

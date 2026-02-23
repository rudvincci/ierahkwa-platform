const express = require('express');
const router = express.Router();
const { getDb, generateCode, logActivity } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// List suppliers
router.get('/', (req, res) => {
    const db = getDb();
    const { search } = req.query;

    let sql = `
        SELECT s.*,
            (SELECT COUNT(*) FROM products WHERE supplier_id = s.id AND is_active = 1) as product_count
        FROM suppliers s
        WHERE s.is_active = 1
    `;
    const params = [];

    if (search) {
        sql += ` AND (s.code LIKE ? OR s.name LIKE ? OR s.contact_person LIKE ?)`;
        params.push(`%${search}%`, `%${search}%`, `%${search}%`);
    }

    sql += ` ORDER BY s.name`;

    const suppliers = db.prepare(sql).all(...params);

    res.render('suppliers/list', {
        title: 'Suppliers',
        suppliers,
        search: search || ''
    });
});

// New supplier form
router.get('/new', (req, res) => {
    const newCode = generateCode('SUP', 'suppliers');

    res.render('suppliers/form', {
        title: 'New Supplier',
        supplier: { code: newCode },
        isNew: true
    });
});

// Create supplier
router.post('/new', (req, res) => {
    const db = getDb();
    const { code, name, contact_person, phone, email, address, city, country,
            tax_id, payment_terms, notes } = req.body;

    try {
        const result = db.prepare(`
            INSERT INTO suppliers (code, name, contact_person, phone, email, address,
                city, country, tax_id, payment_terms, notes)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `).run(code, name, contact_person, phone, email, address, city, country,
               tax_id, payment_terms, notes);

        logActivity(req.session.user.id, 'CREATE', 'suppliers', result.lastInsertRowid, 
                   JSON.stringify({ code, name }), req.ip);

        req.session.success = 'Supplier created successfully';
        res.redirect('/suppliers');
    } catch (error) {
        console.error('Create supplier error:', error);
        req.session.error = 'Error creating supplier';
        res.redirect('/suppliers/new');
    }
});

// Edit supplier form
router.get('/:id/edit', (req, res) => {
    const db = getDb();
    const supplier = db.prepare('SELECT * FROM suppliers WHERE id = ?').get(req.params.id);

    if (!supplier) {
        req.session.error = 'Supplier not found';
        return res.redirect('/suppliers');
    }

    res.render('suppliers/form', {
        title: 'Edit Supplier',
        supplier,
        isNew: false
    });
});

// Update supplier
router.post('/:id/edit', (req, res) => {
    const db = getDb();
    const { code, name, contact_person, phone, email, address, city, country,
            tax_id, payment_terms, notes, is_active } = req.body;

    try {
        db.prepare(`
            UPDATE suppliers SET 
                code = ?, name = ?, contact_person = ?, phone = ?, email = ?,
                address = ?, city = ?, country = ?, tax_id = ?, payment_terms = ?,
                notes = ?, is_active = ?
            WHERE id = ?
        `).run(code, name, contact_person, phone, email, address, city, country,
               tax_id, payment_terms, notes, is_active ? 1 : 0, req.params.id);

        logActivity(req.session.user.id, 'UPDATE', 'suppliers', req.params.id, 
                   JSON.stringify({ code, name }), req.ip);

        req.session.success = 'Supplier updated successfully';
        res.redirect('/suppliers');
    } catch (error) {
        console.error('Update supplier error:', error);
        req.session.error = 'Error updating supplier';
        res.redirect(`/suppliers/${req.params.id}/edit`);
    }
});

// View supplier
router.get('/:id', (req, res) => {
    const db = getDb();
    const supplier = db.prepare('SELECT * FROM suppliers WHERE id = ?').get(req.params.id);

    if (!supplier) {
        req.session.error = 'Supplier not found';
        return res.redirect('/suppliers');
    }

    const products = db.prepare(`
        SELECT * FROM products WHERE supplier_id = ? AND is_active = 1 ORDER BY name
    `).all(req.params.id);

    res.render('suppliers/view', {
        title: supplier.name,
        supplier,
        products
    });
});

// Delete supplier
router.post('/:id/delete', (req, res) => {
    const db = getDb();

    const productCount = db.prepare('SELECT COUNT(*) as count FROM products WHERE supplier_id = ? AND is_active = 1')
                          .get(req.params.id).count;

    if (productCount > 0) {
        req.session.error = 'Cannot delete supplier with associated products';
        return res.redirect('/suppliers');
    }

    try {
        db.prepare('UPDATE suppliers SET is_active = 0 WHERE id = ?').run(req.params.id);
        logActivity(req.session.user.id, 'DELETE', 'suppliers', req.params.id, null, req.ip);
        req.session.success = 'Supplier deleted successfully';
    } catch (error) {
        req.session.error = 'Error deleting supplier';
    }

    res.redirect('/suppliers');
});

module.exports = router;

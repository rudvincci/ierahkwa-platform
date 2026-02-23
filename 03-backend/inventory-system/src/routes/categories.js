const express = require('express');
const router = express.Router();
const { getDb, generateCode, logActivity } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// List categories
router.get('/', (req, res) => {
    const db = getDb();
    
    const categories = db.prepare(`
        SELECT c.*, 
            parent.name as parent_name,
            (SELECT COUNT(*) FROM products WHERE category_id = c.id AND is_active = 1) as product_count
        FROM categories c
        LEFT JOIN categories parent ON c.parent_id = parent.id
        ORDER BY c.name
    `).all();

    res.render('categories/list', {
        title: 'Categories',
        categories
    });
});

// New category form
router.get('/new', (req, res) => {
    const db = getDb();
    const categories = db.prepare('SELECT * FROM categories WHERE is_active = 1 ORDER BY name').all();
    const newCode = generateCode('CAT', 'categories');

    res.render('categories/form', {
        title: 'New Category',
        category: { code: newCode },
        categories,
        isNew: true
    });
});

// Create category
router.post('/new', (req, res) => {
    const db = getDb();
    const { code, name, description, parent_id } = req.body;

    try {
        const result = db.prepare(`
            INSERT INTO categories (code, name, description, parent_id)
            VALUES (?, ?, ?, ?)
        `).run(code, name, description, parent_id || null);

        logActivity(req.session.user.id, 'CREATE', 'categories', result.lastInsertRowid, 
                   JSON.stringify({ code, name }), req.ip);

        req.session.success = 'Category created successfully';
        res.redirect('/categories');
    } catch (error) {
        console.error('Create category error:', error);
        req.session.error = 'Error creating category';
        res.redirect('/categories/new');
    }
});

// Edit category form
router.get('/:id/edit', (req, res) => {
    const db = getDb();
    const category = db.prepare('SELECT * FROM categories WHERE id = ?').get(req.params.id);

    if (!category) {
        req.session.error = 'Category not found';
        return res.redirect('/categories');
    }

    const categories = db.prepare('SELECT * FROM categories WHERE is_active = 1 AND id != ? ORDER BY name')
                        .all(req.params.id);

    res.render('categories/form', {
        title: 'Edit Category',
        category,
        categories,
        isNew: false
    });
});

// Update category
router.post('/:id/edit', (req, res) => {
    const db = getDb();
    const { code, name, description, parent_id, is_active } = req.body;

    try {
        db.prepare(`
            UPDATE categories SET 
                code = ?, name = ?, description = ?, parent_id = ?, is_active = ?
            WHERE id = ?
        `).run(code, name, description, parent_id || null, is_active ? 1 : 0, req.params.id);

        logActivity(req.session.user.id, 'UPDATE', 'categories', req.params.id, 
                   JSON.stringify({ code, name }), req.ip);

        req.session.success = 'Category updated successfully';
        res.redirect('/categories');
    } catch (error) {
        console.error('Update category error:', error);
        req.session.error = 'Error updating category';
        res.redirect(`/categories/${req.params.id}/edit`);
    }
});

// Delete category
router.post('/:id/delete', (req, res) => {
    const db = getDb();

    // Check if category has products
    const productCount = db.prepare('SELECT COUNT(*) as count FROM products WHERE category_id = ? AND is_active = 1')
                          .get(req.params.id).count;

    if (productCount > 0) {
        req.session.error = 'Cannot delete category with associated products';
        return res.redirect('/categories');
    }

    try {
        db.prepare('UPDATE categories SET is_active = 0 WHERE id = ?').run(req.params.id);
        logActivity(req.session.user.id, 'DELETE', 'categories', req.params.id, null, req.ip);
        req.session.success = 'Category deleted successfully';
    } catch (error) {
        req.session.error = 'Error deleting category';
    }

    res.redirect('/categories');
});

module.exports = router;

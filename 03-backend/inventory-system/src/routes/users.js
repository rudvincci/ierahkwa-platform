const express = require('express');
const router = express.Router();
const bcrypt = require('bcryptjs');
const { getDb, logActivity } = require('../db');
const { requireAuth, requireAdmin } = require('../middleware/auth');

router.use(requireAuth);
router.use(requireAdmin);

// List users
router.get('/', (req, res) => {
    const db = getDb();
    const users = db.prepare('SELECT * FROM users ORDER BY full_name').all();

    res.render('users/list', {
        title: 'User Management',
        users
    });
});

// New user form
router.get('/new', (req, res) => {
    res.render('users/form', {
        title: 'New User',
        user: {},
        isNew: true
    });
});

// Create user
router.post('/new', (req, res) => {
    const db = getDb();
    const { username, password, confirm_password, full_name, email, role } = req.body;

    if (password !== confirm_password) {
        req.session.error = 'Passwords do not match';
        return res.redirect('/users/new');
    }

    try {
        const hashedPassword = bcrypt.hashSync(password, 10);
        const result = db.prepare(`
            INSERT INTO users (username, password, full_name, email, role)
            VALUES (?, ?, ?, ?, ?)
        `).run(username, hashedPassword, full_name, email, role);

        logActivity(req.session.user.id, 'CREATE', 'users', result.lastInsertRowid, 
                   JSON.stringify({ username, full_name, role }), req.ip);

        req.session.success = 'User created successfully';
        res.redirect('/users');
    } catch (error) {
        console.error('Create user error:', error);
        req.session.error = 'Error creating user. Username may already exist.';
        res.redirect('/users/new');
    }
});

// Edit user form
router.get('/:id/edit', (req, res) => {
    const db = getDb();
    const user = db.prepare('SELECT * FROM users WHERE id = ?').get(req.params.id);

    if (!user) {
        req.session.error = 'User not found';
        return res.redirect('/users');
    }

    res.render('users/form', {
        title: 'Edit User',
        user,
        isNew: false
    });
});

// Update user
router.post('/:id/edit', (req, res) => {
    const db = getDb();
    const { username, full_name, email, role, is_active } = req.body;

    try {
        db.prepare(`
            UPDATE users SET 
                username = ?, full_name = ?, email = ?, role = ?, is_active = ?
            WHERE id = ?
        `).run(username, full_name, email, role, is_active ? 1 : 0, req.params.id);

        logActivity(req.session.user.id, 'UPDATE', 'users', req.params.id, 
                   JSON.stringify({ username, full_name, role }), req.ip);

        req.session.success = 'User updated successfully';
        res.redirect('/users');
    } catch (error) {
        console.error('Update user error:', error);
        req.session.error = 'Error updating user';
        res.redirect(`/users/${req.params.id}/edit`);
    }
});

// Reset password form
router.get('/:id/reset-password', (req, res) => {
    const db = getDb();
    const user = db.prepare('SELECT id, username, full_name FROM users WHERE id = ?').get(req.params.id);

    if (!user) {
        req.session.error = 'User not found';
        return res.redirect('/users');
    }

    res.render('users/reset-password', {
        title: 'Reset Password',
        user
    });
});

// Reset password
router.post('/:id/reset-password', (req, res) => {
    const db = getDb();
    const { new_password, confirm_password } = req.body;

    if (new_password !== confirm_password) {
        req.session.error = 'Passwords do not match';
        return res.redirect(`/users/${req.params.id}/reset-password`);
    }

    try {
        const hashedPassword = bcrypt.hashSync(new_password, 10);
        db.prepare('UPDATE users SET password = ? WHERE id = ?').run(hashedPassword, req.params.id);

        logActivity(req.session.user.id, 'RESET_PASSWORD', 'users', req.params.id, null, req.ip);

        req.session.success = 'Password reset successfully';
        res.redirect('/users');
    } catch (error) {
        console.error('Reset password error:', error);
        req.session.error = 'Error resetting password';
        res.redirect(`/users/${req.params.id}/reset-password`);
    }
});

// Activity log
router.get('/activity-log', (req, res) => {
    const db = getDb();
    const { user_id, start_date, end_date } = req.query;

    let sql = `
        SELECT l.*, u.full_name as user_name
        FROM activity_logs l
        LEFT JOIN users u ON l.user_id = u.id
        WHERE 1=1
    `;
    const params = [];

    if (user_id) {
        sql += ` AND l.user_id = ?`;
        params.push(user_id);
    }

    if (start_date) {
        sql += ` AND DATE(l.created_at) >= ?`;
        params.push(start_date);
    }

    if (end_date) {
        sql += ` AND DATE(l.created_at) <= ?`;
        params.push(end_date);
    }

    sql += ` ORDER BY l.created_at DESC LIMIT 500`;

    const logs = db.prepare(sql).all(...params);
    const users = db.prepare('SELECT id, full_name FROM users ORDER BY full_name').all();

    res.render('users/activity-log', {
        title: 'Activity Log',
        logs,
        users,
        filters: { user_id, start_date, end_date }
    });
});

module.exports = router;

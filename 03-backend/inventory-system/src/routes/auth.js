const express = require('express');
const router = express.Router();
const bcrypt = require('bcryptjs');
const { getDb, logActivity } = require('../db');

// Login page
router.get('/login', (req, res) => {
    if (req.session.user) {
        return res.redirect('/dashboard');
    }
    res.render('login', { title: 'Login' });
});

// Login process
router.post('/login', (req, res) => {
    const { username, password } = req.body;
    const db = getDb();

    try {
        const user = db.prepare(`
            SELECT * FROM users WHERE username = ? AND is_active = 1
        `).get(username);

        if (!user || !bcrypt.compareSync(password, user.password)) {
            req.session.error = 'Invalid username or password';
            return res.redirect('/login');
        }

        // Update last login
        db.prepare('UPDATE users SET last_login = CURRENT_TIMESTAMP WHERE id = ?').run(user.id);

        // Set session
        req.session.user = {
            id: user.id,
            username: user.username,
            full_name: user.full_name,
            role: user.role
        };

        // Log activity
        logActivity(user.id, 'LOGIN', 'users', user.id, null, req.ip);

        res.redirect('/dashboard');
    } catch (error) {
        console.error('Login error:', error);
        req.session.error = 'An error occurred during login';
        res.redirect('/login');
    }
});

// Logout
router.get('/logout', (req, res) => {
    if (req.session.user) {
        logActivity(req.session.user.id, 'LOGOUT', 'users', req.session.user.id, null, req.ip);
    }
    req.session.destroy();
    res.redirect('/login');
});

// Change password page
router.get('/change-password', (req, res) => {
    if (!req.session.user) {
        return res.redirect('/login');
    }
    res.render('change-password', { title: 'Change Password' });
});

// Change password process
router.post('/change-password', (req, res) => {
    if (!req.session.user) {
        return res.redirect('/login');
    }

    const { current_password, new_password, confirm_password } = req.body;
    const db = getDb();

    try {
        const user = db.prepare('SELECT password FROM users WHERE id = ?').get(req.session.user.id);

        if (!bcrypt.compareSync(current_password, user.password)) {
            req.session.error = 'Current password is incorrect';
            return res.redirect('/change-password');
        }

        if (new_password !== confirm_password) {
            req.session.error = 'New passwords do not match';
            return res.redirect('/change-password');
        }

        if (new_password.length < 4) {
            req.session.error = 'Password must be at least 4 characters';
            return res.redirect('/change-password');
        }

        const hashedPassword = bcrypt.hashSync(new_password, 10);
        db.prepare('UPDATE users SET password = ? WHERE id = ?').run(hashedPassword, req.session.user.id);

        logActivity(req.session.user.id, 'CHANGE_PASSWORD', 'users', req.session.user.id, null, req.ip);

        req.session.success = 'Password changed successfully';
        res.redirect('/dashboard');
    } catch (error) {
        console.error('Change password error:', error);
        req.session.error = 'An error occurred';
        res.redirect('/change-password');
    }
});

module.exports = router;

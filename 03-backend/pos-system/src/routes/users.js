const express = require('express');
const bcrypt = require('bcryptjs');
const { getDb } = require('../db');

const router = express.Router();

// Check admin permission middleware
const requireAdmin = (req, res, next) => {
  if (req.session.user.role !== 'admin') {
    return res.status(403).json({ error: 'Admin access required' });
  }
  next();
};

// Get all users
router.get('/', requireAdmin, (req, res) => {
  const db = getDb();
  const users = db.prepare(`
    SELECT id, username, full_name, role, permissions, language, active, created_at
    FROM users
    ORDER BY created_at DESC
  `).all();
  
  users.forEach(u => {
    u.permissions = JSON.parse(u.permissions || '{}');
  });
  
  res.json(users);
});

// Get single user
router.get('/:id', requireAdmin, (req, res) => {
  const db = getDb();
  const user = db.prepare(`
    SELECT id, username, full_name, role, permissions, language, active, created_at
    FROM users WHERE id = ?
  `).get(req.params.id);
  
  if (!user) {
    return res.status(404).json({ error: 'User not found' });
  }
  
  user.permissions = JSON.parse(user.permissions || '{}');
  res.json(user);
});

// Create user
router.post('/', requireAdmin, (req, res) => {
  const db = getDb();
  const { username, password, full_name, role, permissions, language } = req.body;
  
  if (!username || !password) {
    return res.status(400).json({ error: 'Username and password are required' });
  }

  // Check if username exists
  const existing = db.prepare('SELECT id FROM users WHERE username = ?').get(username);
  if (existing) {
    return res.status(400).json({ error: 'Username already exists' });
  }

  const hashedPassword = bcrypt.hashSync(password, 10);
  
  const result = db.prepare(`
    INSERT INTO users (username, password, full_name, role, permissions, language)
    VALUES (?, ?, ?, ?, ?, ?)
  `).run(
    username, 
    hashedPassword, 
    full_name || username,
    role || 'cashier',
    JSON.stringify(permissions || {}),
    language || 'en'
  );

  res.json({ id: result.lastInsertRowid, success: true });
});

// Update user
router.put('/:id', requireAdmin, (req, res) => {
  const db = getDb();
  const { username, password, full_name, role, permissions, language, active } = req.body;
  
  // Check if username exists for another user
  if (username) {
    const existing = db.prepare('SELECT id FROM users WHERE username = ? AND id != ?').get(username, req.params.id);
    if (existing) {
      return res.status(400).json({ error: 'Username already exists' });
    }
  }

  let query = `
    UPDATE users SET
      username = COALESCE(?, username),
      full_name = COALESCE(?, full_name),
      role = COALESCE(?, role),
      permissions = COALESCE(?, permissions),
      language = COALESCE(?, language),
      active = COALESCE(?, active)
  `;
  
  const params = [
    username, 
    full_name, 
    role, 
    permissions ? JSON.stringify(permissions) : null,
    language, 
    active
  ];

  // Update password if provided
  if (password) {
    query += ', password = ?';
    params.push(bcrypt.hashSync(password, 10));
  }

  query += ' WHERE id = ?';
  params.push(req.params.id);

  db.prepare(query).run(...params);
  res.json({ success: true });
});

// Delete user (soft delete)
router.delete('/:id', requireAdmin, (req, res) => {
  const db = getDb();
  
  // Prevent deleting self
  if (parseInt(req.params.id) === req.session.user.id) {
    return res.status(400).json({ error: 'Cannot delete your own account' });
  }

  db.prepare('UPDATE users SET active = 0 WHERE id = ?').run(req.params.id);
  res.json({ success: true });
});

// Update own profile
router.put('/profile/me', (req, res) => {
  const db = getDb();
  const { full_name, language, current_password, new_password } = req.body;
  const userId = req.session.user.id;

  // If changing password, verify current password
  if (new_password) {
    if (!current_password) {
      return res.status(400).json({ error: 'Current password required' });
    }

    const user = db.prepare('SELECT password FROM users WHERE id = ?').get(userId);
    if (!bcrypt.compareSync(current_password, user.password)) {
      return res.status(400).json({ error: 'Current password is incorrect' });
    }

    db.prepare('UPDATE users SET password = ? WHERE id = ?').run(
      bcrypt.hashSync(new_password, 10),
      userId
    );
  }

  // Update other fields
  if (full_name || language) {
    db.prepare(`
      UPDATE users SET
        full_name = COALESCE(?, full_name),
        language = COALESCE(?, language)
      WHERE id = ?
    `).run(full_name, language, userId);

    // Update session
    if (full_name) req.session.user.full_name = full_name;
    if (language) req.session.user.language = language;
  }

  res.json({ success: true, user: req.session.user });
});

module.exports = router;

const express = require('express');
const bcrypt = require('bcryptjs');
const { getDb } = require('../db');

const router = express.Router();

// Login
router.post('/login', (req, res) => {
  const { username, password } = req.body;
  
  if (!username || !password) {
    return res.status(400).json({ error: 'Username and password required' });
  }

  const db = getDb();
  const user = db.prepare(`
    SELECT id, username, password, full_name, role, permissions, language, active
    FROM users WHERE username = ?
  `).get(username);

  if (!user) {
    return res.status(401).json({ error: 'Invalid credentials' });
  }

  if (!user.active) {
    return res.status(401).json({ error: 'Account is disabled' });
  }

  const validPassword = bcrypt.compareSync(password, user.password);
  if (!validPassword) {
    return res.status(401).json({ error: 'Invalid credentials' });
  }

  // Set session
  req.session.user = {
    id: user.id,
    username: user.username,
    full_name: user.full_name,
    role: user.role,
    permissions: JSON.parse(user.permissions || '{}'),
    language: user.language
  };

  res.json({
    success: true,
    user: req.session.user
  });
});

// Logout
router.post('/logout', (req, res) => {
  req.session.destroy((err) => {
    if (err) {
      return res.status(500).json({ error: 'Failed to logout' });
    }
    res.json({ success: true });
  });
});

// Check session
router.get('/check', (req, res) => {
  if (req.session && req.session.user) {
    res.json({ authenticated: true, user: req.session.user });
  } else {
    res.json({ authenticated: false });
  }
});

module.exports = router;

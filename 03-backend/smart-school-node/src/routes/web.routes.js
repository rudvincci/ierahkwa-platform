const express = require('express');
const router = express.Router();
const { optionalAuth } = require('../middleware/auth.middleware');

// Home page
router.get('/', (req, res) => {
  res.render('index', { 
    title: 'Smart School & Accounting System',
    user: req.user 
  });
});

// Login page
router.get('/login', (req, res) => {
  res.render('login', { 
    title: 'Login',
    error: req.flash('error')
  });
});

// Dashboard redirect
router.get('/dashboard', optionalAuth, (req, res) => {
  if (!req.user) {
    return res.redirect('/login');
  }
  res.render('dashboard', { 
    title: 'Dashboard',
    user: req.user 
  });
});

// API documentation
router.get('/api-docs', (req, res) => {
  res.render('api-docs', { title: 'API Documentation' });
});

module.exports = router;

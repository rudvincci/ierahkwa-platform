const express = require('express');
const session = require('express-session');
const path = require('path');
const db = require('./src/db');
const authRoutes = require('./src/routes/auth');
const itemRoutes = require('./src/routes/items');
const orderRoutes = require('./src/routes/orders');
const tableRoutes = require('./src/routes/tables');
const reportRoutes = require('./src/routes/reports');
const userRoutes = require('./src/routes/users');
const crmRoutes = require('./src/routes/crm');
const inventoryRoutes = require('./src/routes/inventory');

const app = express();
const PORT = process.env.PORT || 3030;

// Middleware
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(express.static(path.join(__dirname, 'public')));

// Session configuration
app.use(session({
  secret: 'smart-pos-secret-key-2024',
  resave: false,
  saveUninitialized: false,
  cookie: { 
    secure: false,
    maxAge: 24 * 60 * 60 * 1000 // 24 hours
  }
}));

// Auth middleware
const requireAuth = (req, res, next) => {
  if (req.session && req.session.user) {
    return next();
  }
  res.status(401).json({ error: 'Unauthorized' });
};

// Public routes
app.use('/api/auth', authRoutes);

// Protected routes
app.use('/api/items', requireAuth, itemRoutes);
app.use('/api/orders', requireAuth, orderRoutes);
app.use('/api/tables', requireAuth, tableRoutes);
app.use('/api/reports', requireAuth, reportRoutes);
app.use('/api/users', requireAuth, userRoutes);

// CRM Routes
app.use('/api/crm', requireAuth, crmRoutes);

// Inventory Routes
app.use('/api/inventory', requireAuth, inventoryRoutes);

// CRM Panel Pages
app.get('/crm', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'crm', 'index.html'));
});
app.get('/crm/admin', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'crm', 'admin.html'));
});
app.get('/crm/qa', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'crm', 'qa.html'));
});
app.get('/crm/agent', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'crm', 'agent.html'));
});
app.get('/crm/accounts', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'crm', 'accounts.html'));
});

// Inventory Pages
app.get('/inventory', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'index.html'));
});
app.get('/inventory/products', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'products.html'));
});
app.get('/inventory/warehouses', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'warehouses.html'));
});
app.get('/inventory/suppliers', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'suppliers.html'));
});
app.get('/inventory/movements', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'movements.html'));
});
app.get('/inventory/purchase-orders', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'purchase-orders.html'));
});
app.get('/inventory/adjustments', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'adjustments.html'));
});
app.get('/inventory/reports', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'inventory', 'reports.html'));
});

// Serve main app
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

// Serve login page
app.get('/login', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'login.html'));
});

// API endpoint to get current user
app.get('/api/me', (req, res) => {
  if (req.session && req.session.user) {
    res.json(req.session.user);
  } else {
    res.status(401).json({ error: 'Not logged in' });
  }
});

// Initialize database and start server
db.initialize();

app.listen(PORT, () => {
  console.log(`
╔═══════════════════════════════════════════════════════════╗
║          SMART POS + CRM + INVENTORY PLATFORM             ║
║         Restaurant & Business Management Suite            ║
╠═══════════════════════════════════════════════════════════╣
║  Server: http://localhost:${PORT}                            ║
║                                                           ║
║  MODULES:                                                 ║
║    POS:       http://localhost:${PORT}/                      ║
║    CRM:       http://localhost:${PORT}/crm                   ║
║    Inventory: http://localhost:${PORT}/inventory             ║
║                                                           ║
║  CRM Panels:                                              ║
║    Admin:     http://localhost:${PORT}/crm/admin             ║
║    QA:        http://localhost:${PORT}/crm/qa                ║
║    Agent:     http://localhost:${PORT}/crm/agent             ║
║    Accounts:  http://localhost:${PORT}/crm/accounts          ║
║                                                           ║
║  Inventory:                                               ║
║    Dashboard: http://localhost:${PORT}/inventory             ║
║    Products:  http://localhost:${PORT}/inventory/products    ║
║    Warehouses:http://localhost:${PORT}/inventory/warehouses  ║
║    PO:        http://localhost:${PORT}/inventory/purchase-orders║
║                                                           ║
║  Login: a / 123456                                        ║
╚═══════════════════════════════════════════════════════════╝
  `);
});

module.exports = app;

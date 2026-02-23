const express = require('express');
const session = require('express-session');
const path = require('path');
const db = require('./src/db');

const app = express();
const PORT = process.env.PORT || 3500;

// Middleware
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(express.static(path.join(__dirname, 'public')));

// Session configuration
app.use(session({
    secret: 'inventory-secret-key-2026',
    resave: false,
    saveUninitialized: false,
    cookie: { 
        secure: false,
        maxAge: 24 * 60 * 60 * 1000 // 24 hours
    }
}));

// View engine
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));

// Make user available to all views
app.use((req, res, next) => {
    res.locals.user = req.session.user || null;
    res.locals.success = req.session.success;
    res.locals.error = req.session.error;
    delete req.session.success;
    delete req.session.error;
    next();
});

// Routes
app.use('/', require('./src/routes/auth'));
app.use('/dashboard', require('./src/routes/dashboard'));
app.use('/products', require('./src/routes/products'));
app.use('/categories', require('./src/routes/categories'));
app.use('/suppliers', require('./src/routes/suppliers'));
app.use('/movements', require('./src/routes/movements'));
app.use('/reports', require('./src/routes/reports'));
app.use('/users', require('./src/routes/users'));
app.use('/settings', require('./src/routes/settings'));
app.use('/api', require('./src/routes/api'));

// Home redirect
app.get('/', (req, res) => {
    if (req.session.user) {
        res.redirect('/dashboard');
    } else {
        res.redirect('/login');
    }
});

// 404 handler
app.use((req, res) => {
    res.status(404).render('error', { 
        title: 'Page Not Found',
        message: 'The page you are looking for does not exist.'
    });
});

// Error handler
app.use((err, req, res, next) => {
    console.error(err.stack);
    res.status(500).render('error', {
        title: 'Server Error',
        message: 'Something went wrong on our end.'
    });
});

// Initialize database and start server
(async () => {
    await db.initialize();
    
    app.listen(PORT, () => {
        console.log(`
╔════════════════════════════════════════════════════════╗
║     INVENTORY MANAGER PRO - Web Application            ║
║     Sovereign Akwesasne Government                     ║
╠════════════════════════════════════════════════════════╣
║     Server running at: http://localhost:${PORT}          ║
║     Default login: admin / admin123                    ║
╚════════════════════════════════════════════════════════╝
        `);
    });
})();

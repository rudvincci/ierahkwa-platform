// Authentication middleware
function requireAuth(req, res, next) {
    if (req.session.user) {
        next();
    } else {
        req.session.error = 'Please login to continue';
        res.redirect('/login');
    }
}

// Admin only middleware
function requireAdmin(req, res, next) {
    if (req.session.user && req.session.user.role === 'admin') {
        next();
    } else {
        req.session.error = 'Access denied. Admin privileges required.';
        res.redirect('/dashboard');
    }
}

// Manager or Admin middleware
function requireManager(req, res, next) {
    if (req.session.user && ['admin', 'manager'].includes(req.session.user.role)) {
        next();
    } else {
        req.session.error = 'Access denied. Manager privileges required.';
        res.redirect('/dashboard');
    }
}

module.exports = { requireAuth, requireAdmin, requireManager };

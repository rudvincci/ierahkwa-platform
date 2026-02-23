const express = require('express');
const router = express.Router();
const { getDb } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// Dashboard
router.get('/', (req, res) => {
    const db = getDb();

    try {
        // Get statistics
        const stats = {
            totalProducts: db.prepare('SELECT COUNT(*) as count FROM products WHERE is_active = 1').get().count,
            totalValue: db.prepare('SELECT COALESCE(SUM(current_stock * purchase_price), 0) as total FROM products WHERE is_active = 1').get().total,
            lowStock: db.prepare('SELECT COUNT(*) as count FROM products WHERE is_active = 1 AND current_stock <= min_stock').get().count,
            totalCategories: db.prepare('SELECT COUNT(*) as count FROM categories WHERE is_active = 1').get().count,
            totalSuppliers: db.prepare('SELECT COUNT(*) as count FROM suppliers WHERE is_active = 1').get().count
        };

        // Today's movements
        const todayMovements = db.prepare(`
            SELECT COUNT(*) as count FROM stock_movements 
            WHERE DATE(created_at) = DATE('now')
        `).get().count;

        // Low stock items
        const lowStockItems = db.prepare(`
            SELECT p.*, c.name as category_name 
            FROM products p
            LEFT JOIN categories c ON p.category_id = c.id
            WHERE p.is_active = 1 AND p.current_stock <= p.min_stock
            ORDER BY (p.min_stock - p.current_stock) DESC
            LIMIT 10
        `).all();

        // Recent movements
        const recentMovements = db.prepare(`
            SELECT m.*, p.name as product_name, p.code as product_code, u.full_name as user_name
            FROM stock_movements m
            JOIN products p ON m.product_id = p.id
            JOIN users u ON m.user_id = u.id
            ORDER BY m.created_at DESC
            LIMIT 10
        `).all();

        // Stock by category for chart
        const stockByCategory = db.prepare(`
            SELECT c.name, COALESCE(SUM(p.current_stock * p.purchase_price), 0) as value
            FROM categories c
            LEFT JOIN products p ON p.category_id = c.id AND p.is_active = 1
            WHERE c.is_active = 1
            GROUP BY c.id
            ORDER BY value DESC
            LIMIT 8
        `).all();

        // Monthly movements for chart
        const monthlyMovements = db.prepare(`
            SELECT 
                strftime('%Y-%m', created_at) as month,
                SUM(CASE WHEN type = 'purchase' THEN total_value ELSE 0 END) as purchases,
                SUM(CASE WHEN type = 'sale' THEN total_value ELSE 0 END) as sales
            FROM stock_movements
            WHERE created_at >= date('now', '-6 months')
            GROUP BY strftime('%Y-%m', created_at)
            ORDER BY month
        `).all();

        res.render('dashboard', {
            title: 'Dashboard',
            stats,
            todayMovements,
            lowStockItems,
            recentMovements,
            stockByCategory: JSON.stringify(stockByCategory),
            monthlyMovements: JSON.stringify(monthlyMovements)
        });
    } catch (error) {
        console.error('Dashboard error:', error);
        res.render('error', { title: 'Error', message: 'Error loading dashboard' });
    }
});

module.exports = router;

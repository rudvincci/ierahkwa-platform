const express = require('express');
const router = express.Router();
const ExcelJS = require('exceljs');
const PDFDocument = require('pdfkit');
const { getDb } = require('../db');
const { requireAuth } = require('../middleware/auth');

router.use(requireAuth);

// Reports menu
router.get('/', (req, res) => {
    res.render('reports/index', { title: 'Reports' });
});

// Stock report
router.get('/stock', (req, res) => {
    const db = getDb();
    const { category, status } = req.query;

    let sql = `
        SELECT p.*, c.name as category_name, s.name as supplier_name,
            (p.current_stock * p.purchase_price) as stock_value
        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        WHERE p.is_active = 1
    `;
    const params = [];

    if (category) {
        sql += ` AND p.category_id = ?`;
        params.push(category);
    }

    if (status === 'low') {
        sql += ` AND p.current_stock <= p.min_stock`;
    } else if (status === 'over') {
        sql += ` AND p.current_stock >= p.max_stock AND p.max_stock > 0`;
    }

    sql += ` ORDER BY p.name`;

    const products = db.prepare(sql).all(...params);
    const categories = db.prepare('SELECT * FROM categories WHERE is_active = 1 ORDER BY name').all();

    // Calculate totals
    const totals = {
        items: products.reduce((sum, p) => sum + p.current_stock, 0),
        value: products.reduce((sum, p) => sum + (p.current_stock * p.purchase_price), 0),
        products: products.length
    };

    res.render('reports/stock', {
        title: 'Stock Report',
        products,
        categories,
        totals,
        filters: { category, status }
    });
});

// Low stock report
router.get('/low-stock', (req, res) => {
    const db = getDb();

    const products = db.prepare(`
        SELECT p.*, c.name as category_name, s.name as supplier_name,
            (p.min_stock - p.current_stock) as deficit
        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        WHERE p.is_active = 1 AND p.current_stock <= p.min_stock
        ORDER BY deficit DESC
    `).all();

    res.render('reports/low-stock', {
        title: 'Low Stock Alert',
        products
    });
});

// Movement report
router.get('/movements', (req, res) => {
    const db = getDb();
    const { start_date, end_date, type } = req.query;

    const startDate = start_date || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10);
    const endDate = end_date || new Date().toISOString().slice(0, 10);

    let sql = `
        SELECT m.*, p.name as product_name, p.code as product_code, u.full_name as user_name
        FROM stock_movements m
        JOIN products p ON m.product_id = p.id
        JOIN users u ON m.user_id = u.id
        WHERE DATE(m.created_at) BETWEEN ? AND ?
    `;
    const params = [startDate, endDate];

    if (type) {
        sql += ` AND m.type = ?`;
        params.push(type);
    }

    sql += ` ORDER BY m.created_at DESC`;

    const movements = db.prepare(sql).all(...params);

    // Calculate totals by type
    const totals = {
        purchase: movements.filter(m => m.type === 'purchase').reduce((sum, m) => sum + m.total_value, 0),
        sale: movements.filter(m => m.type === 'sale').reduce((sum, m) => sum + m.total_value, 0),
        count: movements.length
    };

    res.render('reports/movements', {
        title: 'Movement Report',
        movements,
        totals,
        filters: { start_date: startDate, end_date: endDate, type }
    });
});

// Export to Excel
router.get('/export/excel/:type', async (req, res) => {
    const db = getDb();
    const { type } = req.params;

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet(type);

    try {
        if (type === 'stock') {
            const products = db.prepare(`
                SELECT p.code, p.name, c.name as category, p.current_stock, p.min_stock,
                    p.purchase_price, p.sale_price, (p.current_stock * p.purchase_price) as value
                FROM products p
                LEFT JOIN categories c ON p.category_id = c.id
                WHERE p.is_active = 1
                ORDER BY p.name
            `).all();

            worksheet.columns = [
                { header: 'Code', key: 'code', width: 12 },
                { header: 'Product', key: 'name', width: 30 },
                { header: 'Category', key: 'category', width: 20 },
                { header: 'Stock', key: 'current_stock', width: 10 },
                { header: 'Min', key: 'min_stock', width: 10 },
                { header: 'Purchase $', key: 'purchase_price', width: 12 },
                { header: 'Sale $', key: 'sale_price', width: 12 },
                { header: 'Value', key: 'value', width: 15 }
            ];

            worksheet.addRows(products);

        } else if (type === 'movements') {
            const movements = db.prepare(`
                SELECT m.document_number, m.created_at, p.code, p.name, m.type,
                    m.quantity, m.unit_price, m.total_value, u.full_name as user_name
                FROM stock_movements m
                JOIN products p ON m.product_id = p.id
                JOIN users u ON m.user_id = u.id
                ORDER BY m.created_at DESC
                LIMIT 1000
            `).all();

            worksheet.columns = [
                { header: 'Document', key: 'document_number', width: 20 },
                { header: 'Date', key: 'created_at', width: 20 },
                { header: 'Code', key: 'code', width: 12 },
                { header: 'Product', key: 'name', width: 30 },
                { header: 'Type', key: 'type', width: 12 },
                { header: 'Qty', key: 'quantity', width: 10 },
                { header: 'Price', key: 'unit_price', width: 12 },
                { header: 'Total', key: 'total_value', width: 12 },
                { header: 'User', key: 'user_name', width: 20 }
            ];

            worksheet.addRows(movements);
        }

        // Style header row
        worksheet.getRow(1).font = { bold: true };
        worksheet.getRow(1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '007ACC' }
        };
        worksheet.getRow(1).font = { bold: true, color: { argb: 'FFFFFF' } };

        res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        res.setHeader('Content-Disposition', `attachment; filename=${type}_report_${Date.now()}.xlsx`);

        await workbook.xlsx.write(res);
        res.end();

    } catch (error) {
        console.error('Excel export error:', error);
        res.status(500).send('Export failed');
    }
});

// Export to PDF
router.get('/export/pdf/:type', (req, res) => {
    const db = getDb();
    const { type } = req.params;

    const doc = new PDFDocument({ margin: 50 });

    res.setHeader('Content-Type', 'application/pdf');
    res.setHeader('Content-Disposition', `attachment; filename=${type}_report_${Date.now()}.pdf`);

    doc.pipe(res);

    // Header
    doc.fontSize(20).text('Inventory Manager Pro', { align: 'center' });
    doc.fontSize(14).text(`${type.charAt(0).toUpperCase() + type.slice(1)} Report`, { align: 'center' });
    doc.fontSize(10).text(`Generated: ${new Date().toLocaleString()}`, { align: 'center' });
    doc.moveDown(2);

    try {
        if (type === 'stock') {
            const products = db.prepare(`
                SELECT p.code, p.name, p.current_stock, p.purchase_price,
                    (p.current_stock * p.purchase_price) as value
                FROM products p
                WHERE p.is_active = 1
                ORDER BY p.name
            `).all();

            // Table header
            doc.fontSize(10).font('Helvetica-Bold');
            doc.text('Code', 50, doc.y, { width: 80, continued: true });
            doc.text('Product', 130, doc.y, { width: 200, continued: true });
            doc.text('Stock', 330, doc.y, { width: 60, continued: true });
            doc.text('Value', 400, doc.y, { width: 80 });
            doc.moveDown(0.5);

            // Table rows
            doc.font('Helvetica');
            for (const p of products.slice(0, 50)) { // Limit to 50 for PDF
                doc.text(p.code, 50, doc.y, { width: 80, continued: true });
                doc.text(p.name.substring(0, 30), 130, doc.y, { width: 200, continued: true });
                doc.text(p.current_stock.toString(), 330, doc.y, { width: 60, continued: true });
                doc.text(`$${p.value.toFixed(2)}`, 400, doc.y, { width: 80 });
            }

            // Totals
            doc.moveDown(2);
            const total = products.reduce((sum, p) => sum + p.value, 0);
            doc.font('Helvetica-Bold').text(`Total Stock Value: $${total.toFixed(2)}`);

        } else if (type === 'low-stock') {
            const products = db.prepare(`
                SELECT p.code, p.name, p.current_stock, p.min_stock
                FROM products p
                WHERE p.is_active = 1 AND p.current_stock <= p.min_stock
                ORDER BY (p.min_stock - p.current_stock) DESC
            `).all();

            doc.fontSize(12).fillColor('red').text(`${products.length} items below minimum stock level`);
            doc.fillColor('black').moveDown();

            for (const p of products) {
                doc.fontSize(10).text(`${p.code} - ${p.name}: ${p.current_stock} (min: ${p.min_stock})`);
            }
        }

    } catch (error) {
        doc.text('Error generating report');
    }

    doc.end();
});

module.exports = router;

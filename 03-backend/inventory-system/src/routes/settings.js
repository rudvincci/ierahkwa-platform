const express = require('express');
const router = express.Router();
const fs = require('fs');
const path = require('path');
const archiver = require('archiver');
const { getDb, logActivity } = require('../db');
const { requireAuth, requireAdmin } = require('../middleware/auth');

router.use(requireAuth);
router.use(requireAdmin);

// Settings page
router.get('/', (req, res) => {
    const db = getDb();
    const settings = {};
    
    const rows = db.prepare('SELECT key, value FROM settings').all();
    for (const row of rows) {
        settings[row.key] = row.value;
    }

    res.render('settings/index', {
        title: 'Settings',
        settings
    });
});

// Save settings
router.post('/', (req, res) => {
    const db = getDb();
    const { company_name, currency, date_format, low_stock_alert, items_per_page } = req.body;

    try {
        const upsert = db.prepare(`
            INSERT INTO settings (key, value, updated_at) VALUES (?, ?, CURRENT_TIMESTAMP)
            ON CONFLICT(key) DO UPDATE SET value = ?, updated_at = CURRENT_TIMESTAMP
        `);

        upsert.run('company_name', company_name, company_name);
        upsert.run('currency', currency, currency);
        upsert.run('date_format', date_format, date_format);
        upsert.run('low_stock_alert', low_stock_alert ? 'true' : 'false', low_stock_alert ? 'true' : 'false');
        upsert.run('items_per_page', items_per_page, items_per_page);

        logActivity(req.session.user.id, 'UPDATE', 'settings', null, 'Settings updated', req.ip);

        req.session.success = 'Settings saved successfully';
        res.redirect('/settings');
    } catch (error) {
        console.error('Save settings error:', error);
        req.session.error = 'Error saving settings';
        res.redirect('/settings');
    }
});

// Backup database
router.get('/backup', (req, res) => {
    const dbPath = path.join(__dirname, '../../data/inventory.db');
    const backupName = `inventory_backup_${new Date().toISOString().slice(0, 10)}.db`;

    try {
        res.setHeader('Content-Type', 'application/octet-stream');
        res.setHeader('Content-Disposition', `attachment; filename=${backupName}`);

        const fileStream = fs.createReadStream(dbPath);
        fileStream.pipe(res);

        logActivity(req.session.user.id, 'BACKUP', 'database', null, null, req.ip);
    } catch (error) {
        console.error('Backup error:', error);
        req.session.error = 'Error creating backup';
        res.redirect('/settings');
    }
});

// Backup all data as ZIP
router.get('/backup-all', (req, res) => {
    const dataPath = path.join(__dirname, '../../data');
    const uploadsPath = path.join(__dirname, '../../public/uploads');
    const backupName = `inventory_full_backup_${new Date().toISOString().slice(0, 10)}.zip`;

    try {
        res.setHeader('Content-Type', 'application/zip');
        res.setHeader('Content-Disposition', `attachment; filename=${backupName}`);

        const archive = archiver('zip', { zlib: { level: 9 } });
        archive.pipe(res);

        // Add database
        archive.directory(dataPath, 'data');

        // Add uploads if exists
        if (fs.existsSync(uploadsPath)) {
            archive.directory(uploadsPath, 'uploads');
        }

        archive.finalize();

        logActivity(req.session.user.id, 'BACKUP_FULL', 'database', null, null, req.ip);
    } catch (error) {
        console.error('Full backup error:', error);
        req.session.error = 'Error creating full backup';
        res.redirect('/settings');
    }
});

// Help page
router.get('/help', (req, res) => {
    res.render('settings/help', { title: 'Help' });
});

// About page
router.get('/about', (req, res) => {
    res.render('settings/about', { title: 'About' });
});

module.exports = router;

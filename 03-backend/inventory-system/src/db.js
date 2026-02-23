const initSqlJs = require('sql.js');
const bcrypt = require('bcryptjs');
const path = require('path');
const fs = require('fs');

const dbPath = path.join(__dirname, '..', 'data', 'inventory.db');
let db = null;
let SQL = null;

// Ensure data directory exists
const dataDir = path.join(__dirname, '..', 'data');
if (!fs.existsSync(dataDir)) {
    fs.mkdirSync(dataDir, { recursive: true });
}

async function initializeSQL() {
    if (!SQL) {
        SQL = await initSqlJs();
    }
    return SQL;
}

function getDb() {
    if (!db) {
        throw new Error('Database not initialized. Call initialize() first.');
    }
    return db;
}

function saveDatabase() {
    if (db) {
        const data = db.export();
        const buffer = Buffer.from(data);
        fs.writeFileSync(dbPath, buffer);
    }
}

// Auto-save every 30 seconds
setInterval(() => {
    if (db) saveDatabase();
}, 30000);

// Save on process exit
process.on('exit', saveDatabase);
process.on('SIGINT', () => { saveDatabase(); process.exit(); });
process.on('SIGTERM', () => { saveDatabase(); process.exit(); });

async function initialize() {
    const SQL = await initializeSQL();
    
    // Load existing database or create new one
    if (fs.existsSync(dbPath)) {
        const fileBuffer = fs.readFileSync(dbPath);
        db = new SQL.Database(fileBuffer);
    } else {
        db = new SQL.Database();
    }

    // Create tables
    db.run(`
        CREATE TABLE IF NOT EXISTS users (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL,
            full_name TEXT NOT NULL,
            email TEXT,
            role TEXT DEFAULT 'user',
            is_active INTEGER DEFAULT 1,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            last_login DATETIME
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS categories (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            code TEXT UNIQUE NOT NULL,
            name TEXT NOT NULL,
            description TEXT,
            parent_id INTEGER,
            is_active INTEGER DEFAULT 1,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (parent_id) REFERENCES categories(id)
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS suppliers (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            code TEXT UNIQUE NOT NULL,
            name TEXT NOT NULL,
            contact_person TEXT,
            phone TEXT,
            email TEXT,
            address TEXT,
            city TEXT,
            country TEXT,
            tax_id TEXT,
            payment_terms TEXT,
            is_active INTEGER DEFAULT 1,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            notes TEXT
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS products (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            code TEXT UNIQUE NOT NULL,
            barcode TEXT,
            name TEXT NOT NULL,
            description TEXT,
            category_id INTEGER,
            supplier_id INTEGER,
            purchase_price REAL DEFAULT 0,
            sale_price REAL DEFAULT 0,
            current_stock INTEGER DEFAULT 0,
            min_stock INTEGER DEFAULT 0,
            max_stock INTEGER DEFAULT 0,
            unit TEXT DEFAULT 'PCS',
            location TEXT,
            image TEXT,
            is_active INTEGER DEFAULT 1,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            created_by INTEGER,
            notes TEXT,
            FOREIGN KEY (category_id) REFERENCES categories(id),
            FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
            FOREIGN KEY (created_by) REFERENCES users(id)
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS stock_movements (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            product_id INTEGER NOT NULL,
            type TEXT NOT NULL,
            quantity INTEGER NOT NULL,
            previous_stock INTEGER NOT NULL,
            new_stock INTEGER NOT NULL,
            unit_price REAL DEFAULT 0,
            total_value REAL DEFAULT 0,
            reference TEXT,
            document_number TEXT,
            notes TEXT,
            user_id INTEGER NOT NULL,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (product_id) REFERENCES products(id),
            FOREIGN KEY (user_id) REFERENCES users(id)
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS activity_logs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            user_id INTEGER,
            action TEXT NOT NULL,
            table_name TEXT,
            record_id INTEGER,
            details TEXT,
            ip_address TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (user_id) REFERENCES users(id)
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS settings (
            key TEXT PRIMARY KEY,
            value TEXT,
            description TEXT,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )
    `);

    // Create indexes
    db.run(`CREATE INDEX IF NOT EXISTS idx_products_code ON products(code)`);
    db.run(`CREATE INDEX IF NOT EXISTS idx_products_barcode ON products(barcode)`);
    db.run(`CREATE INDEX IF NOT EXISTS idx_products_name ON products(name)`);
    db.run(`CREATE INDEX IF NOT EXISTS idx_movements_product ON stock_movements(product_id)`);
    db.run(`CREATE INDEX IF NOT EXISTS idx_movements_date ON stock_movements(created_at)`);

    // Seed default data
    seedDefaultData();
    
    // Save database
    saveDatabase();

    console.log('Database initialized successfully');
}

function seedDefaultData() {
    // Check if admin exists
    const result = db.exec("SELECT id FROM users WHERE username = 'admin'");
    
    if (result.length === 0 || result[0].values.length === 0) {
        // Create admin user
        const hashedPassword = bcrypt.hashSync('admin123', 10);
        db.run(`
            INSERT INTO users (username, password, full_name, email, role)
            VALUES (?, ?, ?, ?, ?)
        `, ['admin', hashedPassword, 'System Administrator', 'admin@inventory.local', 'admin']);

        // Create default categories
        const categories = [
            ['CAT001', 'Electronics', 'Electronic devices and components'],
            ['CAT002', 'Office Supplies', 'Office materials and supplies'],
            ['CAT003', 'Furniture', 'Office and home furniture'],
            ['CAT004', 'Raw Materials', 'Raw materials for production'],
            ['CAT005', 'Finished Goods', 'Ready for sale products']
        ];

        for (const cat of categories) {
            db.run(`INSERT OR IGNORE INTO categories (code, name, description) VALUES (?, ?, ?)`, cat);
        }

        // Create default settings
        const settings = [
            ['company_name', 'Sovereign Akwesasne Government', 'Company name'],
            ['currency', 'USD', 'Default currency'],
            ['date_format', 'YYYY-MM-DD', 'Date format'],
            ['low_stock_alert', 'true', 'Enable low stock alerts'],
            ['items_per_page', '25', 'Items per page in lists']
        ];

        for (const setting of settings) {
            db.run(`INSERT OR IGNORE INTO settings (key, value, description) VALUES (?, ?, ?)`, setting);
        }

        console.log('Default data seeded');
    }
}

// Database wrapper methods to maintain compatibility
const dbWrapper = {
    prepare: function(sql) {
        return {
            run: function(...params) {
                db.run(sql, params);
                saveDatabase();
                const result = db.exec("SELECT last_insert_rowid() as id");
                return { lastInsertRowid: result[0]?.values[0]?.[0] || 0 };
            },
            get: function(...params) {
                const stmt = db.prepare(sql);
                stmt.bind(params);
                if (stmt.step()) {
                    const row = stmt.getAsObject();
                    stmt.free();
                    return row;
                }
                stmt.free();
                return undefined;
            },
            all: function(...params) {
                const results = [];
                const stmt = db.prepare(sql);
                stmt.bind(params);
                while (stmt.step()) {
                    results.push(stmt.getAsObject());
                }
                stmt.free();
                return results;
            }
        };
    },
    exec: function(sql) {
        db.run(sql);
        saveDatabase();
    },
    transaction: function(fn) {
        return function() {
            db.run('BEGIN TRANSACTION');
            try {
                fn();
                db.run('COMMIT');
                saveDatabase();
            } catch (error) {
                db.run('ROLLBACK');
                throw error;
            }
        };
    }
};

function generateCode(prefix, tableName) {
    const result = db.exec(`
        SELECT code FROM ${tableName} 
        WHERE code LIKE '${prefix}%' 
        ORDER BY code DESC LIMIT 1
    `);

    if (result.length === 0 || result[0].values.length === 0) {
        return `${prefix}001`;
    }

    const lastCode = result[0].values[0][0];
    const num = parseInt(lastCode.replace(prefix, '')) + 1;
    return `${prefix}${num.toString().padStart(3, '0')}`;
}

function logActivity(userId, action, tableName, recordId, details, ipAddress) {
    db.run(`
        INSERT INTO activity_logs (user_id, action, table_name, record_id, details, ip_address)
        VALUES (?, ?, ?, ?, ?, ?)
    `, [userId, action, tableName, recordId, details, ipAddress]);
    saveDatabase();
}

module.exports = {
    initialize,
    getDb: () => dbWrapper,
    generateCode,
    logActivity,
    saveDatabase
};

// Simple JSON-based database for Smart POS System
// Works with any Node.js version without native dependencies

const fs = require('fs');
const path = require('path');
const bcrypt = require('bcryptjs');

const dataDir = path.join(__dirname, '..', 'data');
const dbFile = path.join(dataDir, 'database.json');

// Default database structure
const defaultDb = {
  users: [],
  categories: [],
  items: [],
  tables: [],
  orders: [],
  order_items: [],
  payments: [],
  settings: {},
  // CRM Collections
  customers: [],
  leads: [],
  tickets: [],
  interactions: [],
  invoices: [],
  invoice_items: [],
  crm_payments: [],
  qa_reviews: [],
  tasks: [],
  audit_log: [],
  notifications: [],
  // Inventory Collections
  warehouses: [],
  suppliers: [],
  warehouse_stock: [],
  stock_movements: [],
  stock_adjustments: [],
  stock_adjustment_items: [],
  stock_transfers: [],
  stock_transfer_items: [],
  purchase_orders: [],
  purchase_order_items: [],
  _meta: {
    nextId: {
      users: 1,
      categories: 1,
      items: 1,
      tables: 1,
      orders: 1,
      order_items: 1,
      payments: 1,
      customers: 1,
      leads: 1,
      tickets: 1,
      interactions: 1,
      invoices: 1,
      invoice_items: 1,
      crm_payments: 1,
      qa_reviews: 1,
      tasks: 1,
      audit_log: 1,
      notifications: 1,
      warehouses: 1,
      suppliers: 1,
      warehouse_stock: 1,
      stock_movements: 1,
      stock_adjustments: 1,
      stock_adjustment_items: 1,
      stock_transfers: 1,
      stock_transfer_items: 1,
      purchase_orders: 1,
      purchase_order_items: 1
    }
  }
};

let database = null;

// Load database from file
function loadDb() {
  if (!fs.existsSync(dataDir)) {
    fs.mkdirSync(dataDir, { recursive: true });
  }
  
  if (fs.existsSync(dbFile)) {
    try {
      const data = fs.readFileSync(dbFile, 'utf-8');
      database = JSON.parse(data);
      
      // Migrate: Add any missing collections from defaultDb
      let migrated = false;
      for (const key of Object.keys(defaultDb)) {
        if (key === '_meta') continue;
        if (!database[key]) {
          database[key] = defaultDb[key];
          migrated = true;
          console.log(`Migrated: Added collection '${key}'`);
        }
      }
      
      // Migrate: Add any missing nextId entries
      if (database._meta && database._meta.nextId) {
        for (const key of Object.keys(defaultDb._meta.nextId)) {
          if (database._meta.nextId[key] === undefined) {
            database._meta.nextId[key] = defaultDb._meta.nextId[key];
            migrated = true;
            console.log(`Migrated: Added nextId for '${key}'`);
          }
        }
      } else {
        database._meta = { ...defaultDb._meta };
        migrated = true;
      }
      
      if (migrated) {
        saveDb();
        console.log('Database migration completed');
      }
    } catch (e) {
      console.error('Error loading database, creating new one:', e.message);
      database = { ...defaultDb };
    }
  } else {
    database = { ...defaultDb };
  }
  
  return database;
}

// Save database to file
function saveDb() {
  fs.writeFileSync(dbFile, JSON.stringify(database, null, 2));
}

// Get next ID for a collection
function getNextId(collection) {
  const id = database._meta.nextId[collection];
  database._meta.nextId[collection]++;
  return id;
}

// Database query helpers - simulating SQLite-like interface
const db = {
  // Prepare returns an object with get, all, and run methods
  prepare(sql) {
    return {
      get: (...params) => executeQuery(sql, params, 'get'),
      all: (...params) => executeQuery(sql, params, 'all'),
      run: (...params) => executeQuery(sql, params, 'run')
    };
  },
  
  exec(sql) {
    // For schema creation - we handle this in initialize()
    return true;
  }
};

// Simple query executor
function executeQuery(sql, params, mode) {
  const sqlLower = sql.toLowerCase().trim();
  
  // SELECT queries
  if (sqlLower.startsWith('select')) {
    return handleSelect(sql, params, mode);
  }
  
  // INSERT queries
  if (sqlLower.startsWith('insert')) {
    return handleInsert(sql, params);
  }
  
  // UPDATE queries
  if (sqlLower.startsWith('update')) {
    return handleUpdate(sql, params);
  }
  
  // DELETE queries
  if (sqlLower.startsWith('delete')) {
    return handleDelete(sql, params);
  }
  
  return mode === 'all' ? [] : null;
}

// Handle SELECT queries
function handleSelect(sql, params, mode) {
  const sqlLower = sql.toLowerCase();
  
  // Determine which table to query
  let collection = null;
  let results = [];
  
  if (sqlLower.includes('from users')) {
    collection = 'users';
    results = [...database.users];
  } else if (sqlLower.includes('from categories')) {
    collection = 'categories';
    results = [...database.categories];
  } else if (sqlLower.includes('from items')) {
    collection = 'items';
    results = [...database.items];
  } else if (sqlLower.includes('from tables')) {
    collection = 'tables';
    results = [...database.tables];
  } else if (sqlLower.includes('from orders')) {
    collection = 'orders';
    results = [...database.orders];
  } else if (sqlLower.includes('from order_items')) {
    collection = 'order_items';
    results = [...database.order_items];
  } else if (sqlLower.includes('from payments')) {
    collection = 'payments';
    results = [...database.payments];
  } else if (sqlLower.includes('from customers')) {
    collection = 'customers';
    results = [...(database.customers || [])];
  } else if (sqlLower.includes('from leads')) {
    collection = 'leads';
    results = [...(database.leads || [])];
  } else if (sqlLower.includes('from tickets')) {
    collection = 'tickets';
    results = [...(database.tickets || [])];
  } else if (sqlLower.includes('from interactions')) {
    collection = 'interactions';
    results = [...(database.interactions || [])];
  } else if (sqlLower.includes('from invoices')) {
    collection = 'invoices';
    results = [...(database.invoices || [])];
  } else if (sqlLower.includes('from invoice_items')) {
    collection = 'invoice_items';
    results = [...(database.invoice_items || [])];
  } else if (sqlLower.includes('from crm_payments')) {
    collection = 'crm_payments';
    results = [...(database.crm_payments || [])];
  } else if (sqlLower.includes('from qa_reviews')) {
    collection = 'qa_reviews';
    results = [...(database.qa_reviews || [])];
  } else if (sqlLower.includes('from tasks')) {
    collection = 'tasks';
    results = [...(database.tasks || [])];
  } else if (sqlLower.includes('from audit_log')) {
    collection = 'audit_log';
    results = [...(database.audit_log || [])];
  } else if (sqlLower.includes('from notifications')) {
    collection = 'notifications';
    results = [...(database.notifications || [])];
  } else if (sqlLower.includes('from warehouses')) {
    collection = 'warehouses';
    results = [...(database.warehouses || [])];
  } else if (sqlLower.includes('from suppliers')) {
    collection = 'suppliers';
    results = [...(database.suppliers || [])];
  } else if (sqlLower.includes('from warehouse_stock')) {
    collection = 'warehouse_stock';
    results = [...(database.warehouse_stock || [])];
  } else if (sqlLower.includes('from stock_movements')) {
    collection = 'stock_movements';
    results = [...(database.stock_movements || [])];
  } else if (sqlLower.includes('from stock_adjustments')) {
    collection = 'stock_adjustments';
    results = [...(database.stock_adjustments || [])];
  } else if (sqlLower.includes('from stock_adjustment_items')) {
    collection = 'stock_adjustment_items';
    results = [...(database.stock_adjustment_items || [])];
  } else if (sqlLower.includes('from stock_transfers')) {
    collection = 'stock_transfers';
    results = [...(database.stock_transfers || [])];
  } else if (sqlLower.includes('from stock_transfer_items')) {
    collection = 'stock_transfer_items';
    results = [...(database.stock_transfer_items || [])];
  } else if (sqlLower.includes('from purchase_orders')) {
    collection = 'purchase_orders';
    results = [...(database.purchase_orders || [])];
  } else if (sqlLower.includes('from purchase_order_items')) {
    collection = 'purchase_order_items';
    results = [...(database.purchase_order_items || [])];
  }
  
  if (!collection) {
    return mode === 'all' ? [] : null;
  }
  
  // Apply WHERE clauses
  if (sqlLower.includes('where')) {
    results = applyWhere(results, sql, params);
  }
  
  // Apply JOINs for enriching data
  results = applyJoins(results, sql, collection);
  
  // Apply ORDER BY
  if (sqlLower.includes('order by')) {
    results = applyOrderBy(results, sql);
  }
  
  // Apply LIMIT
  const limitMatch = sqlLower.match(/limit\s+(\d+)/);
  if (limitMatch) {
    results = results.slice(0, parseInt(limitMatch[1]));
  }
  
  // Handle aggregate functions
  if (sqlLower.includes('count(') || sqlLower.includes('sum(') || sqlLower.includes('avg(')) {
    return handleAggregates(sql, results, mode);
  }
  
  // Handle DISTINCT
  if (sqlLower.includes('select distinct')) {
    const field = sql.match(/select\s+distinct\s+(\w+)/i)?.[1];
    if (field) {
      const seen = new Set();
      results = results.filter(r => {
        if (seen.has(r[field])) return false;
        seen.add(r[field]);
        return true;
      });
    }
  }
  
  return mode === 'get' ? results[0] || null : results;
}

// Apply WHERE clause filtering
function applyWhere(results, sql, params) {
  const sqlLower = sql.toLowerCase();
  let paramIndex = 0;
  
  // Simple WHERE id = ?
  if (sqlLower.includes('where id = ?')) {
    results = results.filter(r => r.id === params[paramIndex++]);
  }
  
  // WHERE username = ?
  if (sqlLower.includes('where username = ?')) {
    results = results.filter(r => r.username === params[paramIndex++]);
  }
  
  // WHERE active = 1
  if (sqlLower.includes('active = 1')) {
    results = results.filter(r => r.active === 1 || r.active === true);
  }
  
  // WHERE order_id = ?
  if (sqlLower.includes('where order_id = ?') || sqlLower.includes('order_id = ?')) {
    const orderId = params[paramIndex++];
    results = results.filter(r => r.order_id === orderId);
  }
  
  // WHERE category_id = ?
  if (sqlLower.includes('category_id = ?')) {
    const categoryId = params.find(p => typeof p === 'number' || !isNaN(p));
    if (categoryId !== undefined) {
      results = results.filter(r => r.category_id === parseInt(categoryId));
    }
  }
  
  // WHERE table_id = ?
  if (sqlLower.includes('table_id = ?')) {
    const tableId = params.find(p => typeof p === 'number' || !isNaN(p));
    if (tableId !== undefined) {
      results = results.filter(r => r.table_id === parseInt(tableId));
    }
  }
  
  // WHERE status = ?
  if (sqlLower.includes('status = ?')) {
    const statusParamIndex = (sql.match(/\?/g) || []).findIndex((m, i) => {
      const beforeQ = sql.split('?').slice(0, i + 1).join('?').toLowerCase();
      return beforeQ.includes('status =');
    });
    if (statusParamIndex >= 0 && params[statusParamIndex]) {
      results = results.filter(r => r.status === params[statusParamIndex]);
    }
  }
  
  // WHERE DATE(created_at) = ?
  if (sqlLower.includes('date(created_at) =') || sqlLower.includes('date(o.created_at) =')) {
    const dateParam = params.find(p => /^\d{4}-\d{2}-\d{2}$/.test(p));
    if (dateParam) {
      results = results.filter(r => {
        if (!r.created_at) return false;
        return r.created_at.startsWith(dateParam);
      });
    }
  }
  
  // WHERE DATE(created_at) BETWEEN ? AND ?
  if (sqlLower.includes('between') && sqlLower.includes('date(')) {
    const dates = params.filter(p => /^\d{4}-\d{2}-\d{2}$/.test(p));
    if (dates.length >= 2) {
      const [startDate, endDate] = dates;
      results = results.filter(r => {
        if (!r.created_at) return false;
        const date = r.created_at.substring(0, 10);
        return date >= startDate && date <= endDate;
      });
    }
  }
  
  // WHERE floor = ?
  if (sqlLower.includes('floor = ?')) {
    const floor = params[paramIndex];
    results = results.filter(r => r.floor === floor);
  }
  
  return results;
}

// Apply JOINs to enrich data
function applyJoins(results, sql, collection) {
  const sqlLower = sql.toLowerCase();
  
  // Join categories to items
  if (collection === 'items' && sqlLower.includes('join categories')) {
    results = results.map(item => {
      const cat = database.categories.find(c => c.id === item.category_id);
      return {
        ...item,
        category_name: cat?.name || null,
        category_color: cat?.color || null
      };
    });
  }
  
  // Join tables and users to orders
  if (collection === 'orders') {
    if (sqlLower.includes('join tables') || sqlLower.includes('left join tables')) {
      results = results.map(order => {
        const table = database.tables.find(t => t.id === order.table_id);
        return { ...order, table_name: table?.name || null };
      });
    }
    if (sqlLower.includes('join users') || sqlLower.includes('left join users')) {
      results = results.map(order => {
        const user = database.users.find(u => u.id === order.user_id);
        return { ...order, user_name: user?.full_name || null };
      });
    }
  }
  
  // Join orders to tables
  if (collection === 'tables' && sqlLower.includes('join orders')) {
    results = results.map(table => {
      const order = database.orders.find(o => o.id === table.current_order_id);
      return {
        ...table,
        order_number: order?.order_number || null,
        order_total: order?.total || null,
        order_status: order?.status || null
      };
    });
  }
  
  // Join items to order_items
  if (collection === 'order_items' && sqlLower.includes('join items')) {
    results = results.map(oi => {
      const item = database.items.find(i => i.id === oi.item_id);
      return { ...oi, name_ar: item?.name_ar || null };
    });
  }
  
  return results;
}

// Apply ORDER BY
function applyOrderBy(results, sql) {
  const match = sql.match(/order\s+by\s+([\w.,\s]+?)(?:\s+(?:limit|$))/i);
  if (!match) return results;
  
  const orderBy = match[1].trim().toLowerCase();
  
  if (orderBy.includes('created_at desc')) {
    results.sort((a, b) => (b.created_at || '').localeCompare(a.created_at || ''));
  } else if (orderBy.includes('created_at')) {
    results.sort((a, b) => (a.created_at || '').localeCompare(b.created_at || ''));
  } else if (orderBy.includes('sort_order')) {
    results.sort((a, b) => (a.sort_order || 0) - (b.sort_order || 0));
  } else if (orderBy.includes('name')) {
    results.sort((a, b) => (a.name || '').localeCompare(b.name || ''));
  }
  
  return results;
}

// Handle aggregate functions
function handleAggregates(sql, results, mode) {
  const sqlLower = sql.toLowerCase();
  
  // Simple COUNT(*)
  if (sqlLower.includes('count(*)')) {
    const result = { count: results.length };
    return mode === 'get' ? result : [result];
  }
  
  // Build aggregate result
  const result = {};
  
  // COUNT
  const countMatch = sql.match(/count\(\*\)\s+as\s+(\w+)/i);
  if (countMatch) {
    result[countMatch[1]] = results.length;
  }
  
  // SUM
  const sumMatches = sql.matchAll(/sum\((?:case\s+when\s+[\w\s=']+\s+then\s+\d+\s+else\s+\d+\s+end|[\w.]+)\)\s*(?:as\s+(\w+))?/gi);
  for (const match of sumMatches) {
    const sumExpr = match[0].toLowerCase();
    const alias = match[1] || 'sum';
    
    if (sumExpr.includes('case when payment_status')) {
      result[alias] = results.filter(r => r.payment_status === 'paid').length;
    } else if (sumExpr.includes('subtotal')) {
      result[alias] = results.reduce((s, r) => s + (r.subtotal || 0), 0);
    } else if (sumExpr.includes('tax_amount')) {
      result[alias] = results.reduce((s, r) => s + (r.tax_amount || 0), 0);
    } else if (sumExpr.includes('discount_amount')) {
      result[alias] = results.reduce((s, r) => s + (r.discount_amount || 0), 0);
    } else if (sumExpr.includes('total')) {
      if (sumExpr.includes('payment_status')) {
        result[alias] = results.filter(r => r.payment_status === 'paid').reduce((s, r) => s + (r.total || 0), 0);
      } else {
        result[alias] = results.reduce((s, r) => s + (r.total || 0), 0);
      }
    } else if (sumExpr.includes('quantity')) {
      result[alias] = results.reduce((s, r) => s + (r.quantity || 0), 0);
    } else if (sumExpr.includes('amount')) {
      result[alias] = results.reduce((s, r) => s + (r.amount || 0), 0);
    }
  }
  
  // COALESCE wrapping
  if (sqlLower.includes('coalesce')) {
    for (const key of Object.keys(result)) {
      if (result[key] === null || result[key] === undefined) {
        result[key] = 0;
      }
    }
  }
  
  // Set defaults for common aggregate aliases
  const aliases = ['total_orders', 'paid_orders', 'subtotal', 'tax_total', 'discount_total', 'revenue', 'total'];
  for (const alias of aliases) {
    if (sqlLower.includes(alias) && result[alias] === undefined) {
      result[alias] = 0;
    }
  }
  
  return mode === 'get' ? result : [result];
}

// Handle INSERT queries
function handleInsert(sql, params) {
  const tableMatch = sql.match(/insert\s+(?:or\s+replace\s+)?into\s+(\w+)/i);
  if (!tableMatch) return { lastInsertRowid: 0, changes: 0 };
  
  const table = tableMatch[1].toLowerCase();
  const collection = table === 'order_items' ? 'order_items' : table;
  
  if (!database[collection]) {
    return { lastInsertRowid: 0, changes: 0 };
  }
  
  // Extract column names
  const colsMatch = sql.match(/\(([^)]+)\)\s*values/i);
  const columns = colsMatch ? colsMatch[1].split(',').map(c => c.trim().toLowerCase()) : [];
  
  // Build new record
  const record = { id: getNextId(collection) };
  columns.forEach((col, i) => {
    if (params[i] !== undefined) {
      record[col] = params[i];
    }
  });
  
  // Add timestamps
  if (!record.created_at) {
    record.created_at = new Date().toISOString();
  }
  
  // Set defaults
  if (collection === 'users') {
    record.active = record.active !== undefined ? record.active : 1;
  }
  if (collection === 'categories' || collection === 'items' || collection === 'tables') {
    record.active = record.active !== undefined ? record.active : 1;
  }
  if (collection === 'tables') {
    record.status = record.status || 'available';
  }
  if (collection === 'orders') {
    record.status = record.status || 'pending';
    record.payment_status = record.payment_status || 'unpaid';
  }
  
  database[collection].push(record);
  saveDb();
  
  return { lastInsertRowid: record.id, changes: 1 };
}

// Handle UPDATE queries
function handleUpdate(sql, params) {
  const tableMatch = sql.match(/update\s+(\w+)/i);
  if (!tableMatch) return { changes: 0 };
  
  const table = tableMatch[1].toLowerCase();
  const collection = table === 'order_items' ? 'order_items' : table;
  
  if (!database[collection]) return { changes: 0 };
  
  // Find the WHERE clause to identify which record(s) to update
  const whereMatch = sql.match(/where\s+id\s*=\s*\?/i);
  const id = params[params.length - 1]; // ID is typically the last param
  
  let changes = 0;
  
  database[collection] = database[collection].map(record => {
    if (whereMatch && record.id !== id) return record;
    
    // Parse SET clause
    const setMatch = sql.match(/set\s+([\s\S]+?)(?:\s+where|$)/i);
    if (!setMatch) return record;
    
    const setClauses = setMatch[1].split(',').map(s => s.trim());
    let paramIndex = 0;
    
    setClauses.forEach(clause => {
      const match = clause.match(/(\w+)\s*=\s*(?:coalesce\s*\(\s*\?\s*,\s*\w+\s*\)|\?)/i);
      if (match) {
        const field = match[1].toLowerCase();
        const value = params[paramIndex++];
        
        if (value !== undefined && value !== null) {
          record[field] = value;
        }
      }
      
      // Handle CASE WHEN for completed_at
      if (clause.toLowerCase().includes('current_timestamp')) {
        if (params.some(p => p === 'completed' || p === 'paid')) {
          record.completed_at = new Date().toISOString();
        }
      }
    });
    
    changes++;
    return record;
  });
  
  saveDb();
  return { changes };
}

// Handle DELETE queries
function handleDelete(sql, params) {
  const tableMatch = sql.match(/delete\s+from\s+(\w+)/i);
  if (!tableMatch) return { changes: 0 };
  
  const table = tableMatch[1].toLowerCase();
  const collection = table === 'order_items' ? 'order_items' : table;
  
  if (!database[collection]) return { changes: 0 };
  
  const id = params[0];
  const before = database[collection].length;
  database[collection] = database[collection].filter(r => r.id !== id);
  const changes = before - database[collection].length;
  
  saveDb();
  return { changes };
}

// Initialize database with seed data
function initialize() {
  loadDb();
  
  // Check if already seeded
  if (database.users.length > 0) {
    console.log('Database already initialized');
    return database;
  }
  
  // Create default admin user
  const hashedPassword = bcrypt.hashSync(process.env.DEFAULT_ADMIN_PASSWORD || 'changeme-dev', 10);
  database.users.push({
    id: getNextId('users'),
    username: 'a',
    password: hashedPassword,
    full_name: 'Administrator',
    role: 'admin',
    permissions: JSON.stringify({
      sales: true,
      tables: true,
      reports: true,
      items: true,
      users: true,
      settings: true
    }),
    language: 'en',
    active: 1,
    created_at: new Date().toISOString()
  });
  
  // Seed categories
  const categories = [
    { name: 'Appetizers', name_ar: 'المقبلات', color: '#e74c3c', icon: 'utensils' },
    { name: 'Main Courses', name_ar: 'الأطباق الرئيسية', color: '#3498db', icon: 'hamburger' },
    { name: 'Desserts', name_ar: 'الحلويات', color: '#9b59b6', icon: 'ice-cream' },
    { name: 'Beverages', name_ar: 'المشروبات', color: '#2ecc71', icon: 'coffee' },
    { name: 'Specials', name_ar: 'العروض الخاصة', color: '#f39c12', icon: 'star' }
  ];
  
  categories.forEach((cat, index) => {
    database.categories.push({
      id: getNextId('categories'),
      name: cat.name,
      name_ar: cat.name_ar,
      color: cat.color,
      icon: cat.icon,
      sort_order: index,
      active: 1
    });
  });
  
  // Seed items
  const items = [
    { name: 'Caesar Salad', name_ar: 'سلطة سيزر', category_id: 1, price: 8.99, tax_rate: 0.1 },
    { name: 'Garlic Bread', name_ar: 'خبز بالثوم', category_id: 1, price: 4.99, tax_rate: 0.1 },
    { name: 'Soup of the Day', name_ar: 'شوربة اليوم', category_id: 1, price: 5.99, tax_rate: 0.1 },
    { name: 'Grilled Chicken', name_ar: 'دجاج مشوي', category_id: 2, price: 15.99, tax_rate: 0.1 },
    { name: 'Beef Steak', name_ar: 'ستيك لحم', category_id: 2, price: 24.99, tax_rate: 0.1 },
    { name: 'Pasta Carbonara', name_ar: 'باستا كاربونارا', category_id: 2, price: 13.99, tax_rate: 0.1 },
    { name: 'Fish & Chips', name_ar: 'سمك وبطاطس', category_id: 2, price: 14.99, tax_rate: 0.1 },
    { name: 'Vegetable Curry', name_ar: 'كاري الخضار', category_id: 2, price: 12.99, tax_rate: 0.1 },
    { name: 'Chocolate Cake', name_ar: 'كيكة شوكولاتة', category_id: 3, price: 6.99, tax_rate: 0.1 },
    { name: 'Ice Cream', name_ar: 'آيس كريم', category_id: 3, price: 4.99, tax_rate: 0.1 },
    { name: 'Cheesecake', name_ar: 'تشيز كيك', category_id: 3, price: 7.99, tax_rate: 0.1 },
    { name: 'Coffee', name_ar: 'قهوة', category_id: 4, price: 2.99, tax_rate: 0.1 },
    { name: 'Tea', name_ar: 'شاي', category_id: 4, price: 2.49, tax_rate: 0.1 },
    { name: 'Fresh Juice', name_ar: 'عصير طازج', category_id: 4, price: 4.99, tax_rate: 0.1 },
    { name: 'Soft Drink', name_ar: 'مشروب غازي', category_id: 4, price: 2.49, tax_rate: 0.1 },
    { name: 'Chef Special', name_ar: 'طبق الشيف', category_id: 5, price: 19.99, tax_rate: 0.1 }
  ];
  
  items.forEach(item => {
    database.items.push({
      id: getNextId('items'),
      name: item.name,
      name_ar: item.name_ar,
      category_id: item.category_id,
      price: item.price,
      cost: 0,
      tax_rate: item.tax_rate,
      image: null,
      barcode: null,
      stock: -1,
      active: 1,
      created_at: new Date().toISOString()
    });
  });
  
  // Seed tables
  const tables = [
    { name: 'Table 1', capacity: 4, pos_x: 50, pos_y: 50, shape: 'rectangle', width: 100, height: 100 },
    { name: 'Table 2', capacity: 4, pos_x: 200, pos_y: 50, shape: 'rectangle', width: 100, height: 100 },
    { name: 'Table 3', capacity: 6, pos_x: 350, pos_y: 50, shape: 'rectangle', width: 100, height: 100 },
    { name: 'Table 4', capacity: 2, pos_x: 50, pos_y: 200, shape: 'circle', width: 100, height: 100 },
    { name: 'Table 5', capacity: 2, pos_x: 200, pos_y: 200, shape: 'circle', width: 100, height: 100 },
    { name: 'Table 6', capacity: 8, pos_x: 350, pos_y: 200, shape: 'rectangle', width: 150, height: 100 },
    { name: 'Bar 1', capacity: 1, pos_x: 50, pos_y: 350, shape: 'circle', width: 60, height: 60 },
    { name: 'Bar 2', capacity: 1, pos_x: 130, pos_y: 350, shape: 'circle', width: 60, height: 60 },
    { name: 'Bar 3', capacity: 1, pos_x: 210, pos_y: 350, shape: 'circle', width: 60, height: 60 }
  ];
  
  tables.forEach(t => {
    database.tables.push({
      id: getNextId('tables'),
      name: t.name,
      capacity: t.capacity,
      pos_x: t.pos_x,
      pos_y: t.pos_y,
      width: t.width,
      height: t.height,
      shape: t.shape,
      status: 'available',
      current_order_id: null,
      floor: 'main',
      active: 1
    });
  });
  
  // Seed settings
  database.settings = {
    restaurant_name: 'Smart POS Restaurant',
    currency: 'USD',
    currency_symbol: '$',
    tax_enabled: true,
    default_tax_rate: 0.1,
    receipt_header: 'Thank you for dining with us!',
    receipt_footer: 'Please come again!'
  };
  
  saveDb();
  console.log('Database initialized with sample data');
  
  return database;
}

function getDb() {
  if (!database) {
    loadDb();
  }
  return db;
}

module.exports = {
  getDb,
  initialize
};

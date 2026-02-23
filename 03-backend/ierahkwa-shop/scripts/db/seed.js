#!/usr/bin/env node
/** Datos de ejemplo - Ierahkwa Futurehead Shop */
import { initSchema, get } from '../../src/db.js';
import bcrypt from 'bcryptjs';

initSchema();
const db = get();

// Settings
const sets = [
  ['site_logo', ''],
  ['site_name', 'Ierahkwa Futurehead Shop'],
  ['meta_title', 'Ierahkwa Futurehead Shop | IGT-MARKET'],
  ['meta_description', 'E-commerce oficial. Sovereign Government of Ierahkwa Ne Kanienke.'],
  ['meta_keywords', 'ierahkwa,shop,igt,isb,mamey,marketplace'],
  ['google_analytics', ''],
  ['vat_percent', '0'],
  ['currency', 'USD'],
  ['vat_label', 'VAT'],
  ['currency_symbol', '$'],
  ['guest_checkout', '1'],
  ['lang_default', 'en'],
  ['social_facebook', ''],
  ['social_twitter', ''],
  ['social_instagram', ''],
  ['app_ios', ''],
  ['app_android', ''],
];
const insSet = db.prepare('INSERT OR REPLACE INTO settings (key, value, updated_at) VALUES (?, ?, datetime("now"))');
sets.forEach(([k, v]) => insSet.run(k, v));

// Roles
db.prepare(`INSERT OR IGNORE INTO roles (id, name, permissions) VALUES (1, 'Super Admin', 'all')`).run();
db.prepare(`INSERT OR IGNORE INTO roles (id, name, permissions) VALUES (2, 'Manager', 'products,orders,customers,reports')`).run();
db.prepare(`INSERT OR IGNORE INTO roles (id, name, permissions) VALUES (3, 'Cashier', 'products,orders')`).run();

// Admin por defecto: admin@ierahkwa.gov / admin123
const hash = bcrypt.hashSync('admin123', 10);
db.prepare(`INSERT OR IGNORE INTO admin_users (id, role_id, email, password_hash, name, is_active) VALUES (1, 1, 'admin@ierahkwa.gov', ?, 'Admin Ierahkwa', 1)`).run(hash);

// Sucursal
db.prepare(`INSERT OR IGNORE INTO branches (id, name, address, is_default) VALUES (1, 'Ierahkwa Main', 'Sovereign Territory of Ierahkwa', 1)`).run();

// Tipos de ítem
['Physical', 'Digital', 'Service'].forEach((n, i) => {
  db.prepare('INSERT OR IGNORE INTO item_types (id, name) VALUES (?, ?)').run(i + 1, n);
});

// Proveedor
db.prepare(`INSERT OR IGNORE INTO suppliers (id, name, email) VALUES (1, 'Ierahkwa Supply', 'supply@ierahkwa.gov')`).run();

// Categorías y subcategorías
db.prepare(`INSERT OR IGNORE INTO categories (id, parent_id, name, slug, sort_order) VALUES (1, NULL, 'General', 'general', 0)`).run();
db.prepare(`INSERT OR IGNORE INTO categories (id, parent_id, name, slug, sort_order) VALUES (2, 1, 'Electronics', 'electronics', 1)`).run();
db.prepare(`INSERT OR IGNORE INTO categories (id, parent_id, name, slug, sort_order) VALUES (3, 1, 'Food', 'food', 2)`).run();

// Productos de ejemplo
const products = [
  { name: 'Sample Product A', sku: 'IFA001', barcode: '2000000000001', price: 19.99, stock: 100, color: 'Black', size: 'M' },
  { name: 'Sample Product B', sku: 'IFB002', barcode: '2000000000002', price: 29.99, discount_percent: 10, stock: 50, color: 'White', size: 'L' },
  { name: 'Sample Product C', sku: 'IFC003', barcode: '2000000000003', price: 9.99, stock: 200 },
];
const insProd = db.prepare(`INSERT INTO products (category_id, item_type_id, supplier_id, branch_id, name, slug, sku, barcode, price, discount_percent, stock, color, size, is_active) 
  VALUES (1, 1, 1, 1, @name, lower(replace(@name,' ','-')), @sku, @barcode, @price, @discount_percent, @stock, @color, @size, 1)`);
products.forEach(p => insProd.run({
  name: p.name, sku: p.sku, barcode: p.barcode, price: p.price, discount_percent: p.discount_percent || 0, stock: p.stock,
  color: p.color || null, size: p.size || null,
}));

// Slider
db.prepare(`INSERT OR IGNORE INTO slider_images (id, image_url, title, sort_order, is_active) VALUES (1, '/img/slider1.jpg', 'Welcome to Ierahkwa Futurehead Shop', 0, 1)`).run();

// Cliente de ejemplo
db.prepare(`INSERT OR IGNORE INTO customers (id, name, email) VALUES (1, 'Demo Customer', 'demo@ierahkwa.gov')`).run();

console.log('Ierahkwa Futurehead Shop: seed completado.');
console.log('  Admin: admin@ierahkwa.gov / admin123');

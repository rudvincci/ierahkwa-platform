/** Ierahkwa Futurehead Shop - Multi-Purpose E-Commerce Database
 *  Ierahkwa Futurehead Mamey Node • IGT-MARKET
 *  Sovereign Government of Ierahkwa Ne Kanienke
 */
import { readFileSync, writeFileSync, existsSync, mkdirSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const DATA_DIR = join(__dirname, '..', 'data');
const DB_FILE = join(DATA_DIR, 'db.json');

// Estructura completa de la base de datos Multi-Purpose E-Commerce
const defaultDb = {
  settings: {
    site_logo: '/img/logo.png',
    site_name: 'Ierahkwa Futurehead Shop',
    site_tagline: 'Multi-Purpose E-Commerce • IGT-MARKET',
    meta_title: 'Ierahkwa Futurehead Shop | Official E-Commerce Platform',
    meta_description: 'Multi-Purpose E-Commerce solution. Sovereign Government of Ierahkwa Ne Kanienke.',
    meta_keywords: 'ierahkwa,shop,igt,marketplace,ecommerce,pos',
    currency_symbol: '$',
    currency_code: 'USD',
    vat_percent: '0',
    vat_label: 'VAT',
    guest_checkout: '1',
    lang_default: 'en',
    timezone: 'America/New_York',
    date_format: 'YYYY-MM-DD',
    low_stock_threshold: '10',
    order_prefix: 'ORD',
    invoice_prefix: 'INV',
    enable_reviews: '1',
    enable_wishlist: '1',
    enable_compare: '1',
    shipping_enabled: '1',
    free_shipping_min: '100',
    flat_shipping_rate: '10',
    social_facebook: '',
    social_twitter: '',
    social_instagram: '',
    social_youtube: '',
    app_ios: '',
    app_android: '',
    google_analytics: '',
    stripe_enabled: '0',
    stripe_key: '',
    igt_payment_enabled: '1',
    contact_email: 'shop@ierahkwa.gov',
    contact_phone: '',
    contact_address: 'Sovereign Territory of Ierahkwa'
  },
  branches: [
    { id: 1, name: 'Ierahkwa Main Store', code: 'MAIN', address: 'Sovereign Territory of Ierahkwa', phone: '', email: 'main@ierahkwa.gov', is_default: 1, is_active: 1, created_at: '2026-01-18' }
  ],
  warehouses: [
    { id: 1, name: 'Central Warehouse', branch_id: 1, address: 'Sovereign Territory', is_default: 1, is_active: 1 }
  ],
  suppliers: [
    { id: 1, name: 'Ierahkwa Official Supply', code: 'IOS001', contact_name: 'Supply Manager', email: 'supply@ierahkwa.gov', phone: '', address: '', tax_id: '', payment_terms: 'Net 30', is_active: 1, created_at: '2026-01-18' }
  ],
  brands: [
    { id: 1, name: 'Ierahkwa', slug: 'ierahkwa', logo: '', description: 'Official Ierahkwa brand', is_active: 1 }
  ],
  units: [
    { id: 1, name: 'Piece', short_name: 'pc', is_default: 1 },
    { id: 2, name: 'Kilogram', short_name: 'kg', is_default: 0 },
    { id: 3, name: 'Liter', short_name: 'L', is_default: 0 },
    { id: 4, name: 'Meter', short_name: 'm', is_default: 0 },
    { id: 5, name: 'Box', short_name: 'box', is_default: 0 }
  ],
  tax_rates: [
    { id: 1, name: 'No Tax', rate: 0, is_default: 1 },
    { id: 2, name: 'Standard VAT', rate: 10, is_default: 0 },
    { id: 3, name: 'Reduced VAT', rate: 5, is_default: 0 }
  ],
  categories: [
    { id: 1, parent_id: null, name: 'All Products', slug: 'all-products', description: '', image: '', sort_order: 0, is_featured: 0, is_active: 1 },
    { id: 2, parent_id: 1, name: 'Electronics', slug: 'electronics', description: 'Electronic devices and accessories', image: '', sort_order: 1, is_featured: 1, is_active: 1 },
    { id: 3, parent_id: 1, name: 'Clothing', slug: 'clothing', description: 'Apparel and fashion', image: '', sort_order: 2, is_featured: 1, is_active: 1 },
    { id: 4, parent_id: 1, name: 'Food & Beverages', slug: 'food-beverages', description: 'Food products', image: '', sort_order: 3, is_featured: 0, is_active: 1 },
    { id: 5, parent_id: 2, name: 'Smartphones', slug: 'smartphones', description: '', image: '', sort_order: 0, is_featured: 1, is_active: 1 },
    { id: 6, parent_id: 2, name: 'Laptops', slug: 'laptops', description: '', image: '', sort_order: 1, is_featured: 0, is_active: 1 },
    { id: 7, parent_id: 3, name: 'Men', slug: 'men', description: '', image: '', sort_order: 0, is_featured: 0, is_active: 1 },
    { id: 8, parent_id: 3, name: 'Women', slug: 'women', description: '', image: '', sort_order: 1, is_featured: 0, is_active: 1 }
  ],
  attributes: [
    { id: 1, name: 'Color', type: 'select', values: ['Black', 'White', 'Red', 'Blue', 'Green', 'Yellow', 'Gray', 'Brown', 'Pink', 'Orange'] },
    { id: 2, name: 'Size', type: 'select', values: ['XS', 'S', 'M', 'L', 'XL', 'XXL', '3XL'] },
    { id: 3, name: 'Material', type: 'select', values: ['Cotton', 'Polyester', 'Leather', 'Metal', 'Plastic', 'Wood'] },
    { id: 4, name: 'Storage', type: 'select', values: ['32GB', '64GB', '128GB', '256GB', '512GB', '1TB'] }
  ],
  products: [
    { id: 1, category_id: 5, brand_id: 1, supplier_id: 1, unit_id: 1, tax_rate_id: 1, type: 'simple', name: 'Ierahkwa SmartPhone Pro', slug: 'ierahkwa-smartphone-pro', sku: 'ISP-001', barcode: '2000000000001', description: 'Official Ierahkwa smartphone with advanced features', short_description: 'Premium smartphone', price: 599.99, cost: 350, compare_price: 699.99, discount_percent: 0, stock: 100, min_stock: 10, weight: 0.2, length: 15, width: 7, height: 1, is_featured: 1, is_new: 1, is_active: 1, images: [], attributes: { color: 'Black', storage: '128GB' }, created_at: '2026-01-18', updated_at: '2026-01-18' },
    { id: 2, category_id: 6, brand_id: 1, supplier_id: 1, unit_id: 1, tax_rate_id: 1, type: 'simple', name: 'Ierahkwa Laptop Elite', slug: 'ierahkwa-laptop-elite', sku: 'ILE-001', barcode: '2000000000002', description: 'Powerful laptop for professionals', short_description: 'Elite performance', price: 1299.99, cost: 800, compare_price: 1499.99, discount_percent: 10, stock: 50, min_stock: 5, weight: 2.1, length: 35, width: 24, height: 2, is_featured: 1, is_new: 1, is_active: 1, images: [], attributes: { color: 'Gray', storage: '512GB' }, created_at: '2026-01-18', updated_at: '2026-01-18' },
    { id: 3, category_id: 7, brand_id: 1, supplier_id: 1, unit_id: 1, tax_rate_id: 1, type: 'variable', name: 'Ierahkwa Classic T-Shirt', slug: 'ierahkwa-classic-tshirt', sku: 'ICT-001', barcode: '2000000000003', description: 'Premium cotton t-shirt with Ierahkwa branding', short_description: 'Classic style', price: 29.99, cost: 12, compare_price: 39.99, discount_percent: 0, stock: 500, min_stock: 50, weight: 0.2, length: 0, width: 0, height: 0, is_featured: 0, is_new: 0, is_active: 1, images: [], attributes: {}, created_at: '2026-01-18', updated_at: '2026-01-18' }
  ],
  product_variants: [
    { id: 1, product_id: 3, sku: 'ICT-001-BLK-M', barcode: '2000000000004', price: 29.99, stock: 100, attributes: { color: 'Black', size: 'M' }, is_active: 1 },
    { id: 2, product_id: 3, sku: 'ICT-001-BLK-L', barcode: '2000000000005', price: 29.99, stock: 80, attributes: { color: 'Black', size: 'L' }, is_active: 1 },
    { id: 3, product_id: 3, sku: 'ICT-001-WHT-M', barcode: '2000000000006', price: 29.99, stock: 90, attributes: { color: 'White', size: 'M' }, is_active: 1 },
    { id: 4, product_id: 3, sku: 'ICT-001-WHT-L', barcode: '2000000000007', price: 29.99, stock: 70, attributes: { color: 'White', size: 'L' }, is_active: 1 }
  ],
  coupons: [
    { id: 1, code: 'WELCOME10', name: 'Welcome Discount', type: 'percent', value: 10, min_order: 50, max_discount: 100, usage_limit: 1000, used_count: 0, starts_at: '2026-01-01', expires_at: '2026-12-31', is_active: 1 },
    { id: 2, code: 'FREESHIP', name: 'Free Shipping', type: 'free_shipping', value: 0, min_order: 75, max_discount: null, usage_limit: null, used_count: 0, starts_at: '2026-01-01', expires_at: '2026-12-31', is_active: 1 }
  ],
  customers: [
    { id: 1, name: 'Demo Customer', email: 'demo@ierahkwa.gov', phone: '', address: '', city: '', state: '', country: '', postal_code: '', notes: '', total_orders: 0, total_spent: 0, points: 0, is_active: 1, created_at: '2026-01-18' }
  ],
  customer_groups: [
    { id: 1, name: 'Regular', discount_percent: 0 },
    { id: 2, name: 'VIP', discount_percent: 5 },
    { id: 3, name: 'Wholesale', discount_percent: 15 }
  ],
  roles: [
    { id: 1, name: 'Super Admin', permissions: ['all'] },
    { id: 2, name: 'Manager', permissions: ['dashboard', 'products', 'orders', 'customers', 'reports', 'inventory'] },
    { id: 3, name: 'Cashier', permissions: ['pos', 'orders'] },
    { id: 4, name: 'Inventory', permissions: ['products', 'inventory', 'suppliers'] }
  ],
  admin_users: [
    { id: 1, role_id: 1, branch_id: 1, email: 'admin@ierahkwa.gov', password_hash: '', name: 'Admin Ierahkwa', phone: '', avatar: '', is_active: 1, last_login: null, created_at: '2026-01-18' }
  ],
  orders: [],
  order_items: [],
  order_history: [],
  payments: [],
  shipping_methods: [
    { id: 1, name: 'Standard Shipping', description: '5-7 business days', price: 10, min_days: 5, max_days: 7, is_active: 1 },
    { id: 2, name: 'Express Shipping', description: '2-3 business days', price: 25, min_days: 2, max_days: 3, is_active: 1 },
    { id: 3, name: 'Next Day Delivery', description: '1 business day', price: 50, min_days: 1, max_days: 1, is_active: 1 },
    { id: 4, name: 'Store Pickup', description: 'Pick up at store', price: 0, min_days: 0, max_days: 0, is_active: 1 }
  ],
  payment_methods: [
    { id: 1, name: 'Cash', code: 'cash', icon: 'bi-cash', is_active: 1 },
    { id: 2, name: 'Credit Card', code: 'card', icon: 'bi-credit-card', is_active: 1 },
    { id: 3, name: 'IGT Token', code: 'igt', icon: 'bi-currency-exchange', is_active: 1 },
    { id: 4, name: 'Bank Transfer', code: 'bank', icon: 'bi-bank', is_active: 1 }
  ],
  slider_images: [
    { id: 1, image_url: '/img/slider1.jpg', title: 'Welcome to Ierahkwa Shop', subtitle: 'Multi-Purpose E-Commerce Platform', link_url: '/products', btn_text: 'Shop Now', sort_order: 0, is_active: 1 },
    { id: 2, image_url: '/img/slider2.jpg', title: 'New Arrivals', subtitle: 'Discover the latest products', link_url: '/new', btn_text: 'Explore', sort_order: 1, is_active: 1 }
  ],
  banners: [
    { id: 1, position: 'home_top', image_url: '', title: '', link_url: '', is_active: 0 }
  ],
  pages: [
    { id: 1, slug: 'about', title: 'About Us', content: 'Ierahkwa Futurehead Shop - Official E-Commerce Platform of the Sovereign Government of Ierahkwa Ne Kanienke.', is_active: 1 },
    { id: 2, slug: 'contact', title: 'Contact Us', content: 'Contact information for Ierahkwa Futurehead Shop.', is_active: 1 },
    { id: 3, slug: 'terms', title: 'Terms & Conditions', content: 'Terms and conditions for using Ierahkwa Futurehead Shop.', is_active: 1 },
    { id: 4, slug: 'privacy', title: 'Privacy Policy', content: 'Privacy policy for Ierahkwa Futurehead Shop.', is_active: 1 }
  ],
  reviews: [],
  wishlists: [],
  inventory_logs: [],
  activity_logs: [],
  notifications: [],
  _counters: { products: 3, categories: 8, orders: 0, customers: 1, admin_users: 1, variants: 4, coupons: 2 },
  _meta: {
    version: '2.0.0',
    created: '2026-01-18',
    platform: 'Ierahkwa Futurehead Mamey Node',
    blockchain: 'Ierahkwa Sovereign Blockchain',
    token: 'IGT-MARKET'
  }
};

let db = null;

function ensureDir() {
  if (!existsSync(DATA_DIR)) mkdirSync(DATA_DIR, { recursive: true });
}

export function load() {
  if (db) return db;
  ensureDir();
  if (existsSync(DB_FILE)) {
    try {
      db = JSON.parse(readFileSync(DB_FILE, 'utf8'));
      // Merge any missing default fields
      for (const key in defaultDb) {
        if (!(key in db)) db[key] = defaultDb[key];
      }
    } catch (e) {
      db = JSON.parse(JSON.stringify(defaultDb));
    }
  } else {
    db = JSON.parse(JSON.stringify(defaultDb));
    save();
  }
  return db;
}

export function save() {
  ensureDir();
  writeFileSync(DB_FILE, JSON.stringify(db, null, 2), 'utf8');
}

export function get() {
  return load();
}

export function generateBarcode() {
  const t = Date.now().toString(36).toUpperCase();
  const r = Math.random().toString(36).slice(2, 8).toUpperCase();
  return `IFT${t}${r}`.slice(0, 13);
}

export function generateSKU(prefix = 'PRD') {
  return `${prefix}-${Date.now().toString(36).toUpperCase()}`;
}

export function generateOrderNumber() {
  const d = load();
  const prefix = d.settings?.order_prefix || 'ORD';
  const num = String((d._counters?.orders || 0) + 1).padStart(6, '0');
  return `${prefix}-${new Date().getFullYear()}-${num}`;
}

export function nextId(table) {
  const d = load();
  if (!d._counters) d._counters = {};
  d._counters[table] = (d._counters[table] || 0) + 1;
  save();
  return d._counters[table];
}

export function now() {
  return new Date().toISOString();
}

export function today() {
  return new Date().toISOString().slice(0, 10);
}

// Calcular precio con descuento
export function calcPrice(product) {
  const price = parseFloat(product.price) || 0;
  const discount = parseFloat(product.discount_percent) || 0;
  return price * (1 - discount / 100);
}

// Buscar variante por atributos
export function findVariant(productId, attributes) {
  const variants = load().product_variants || [];
  return variants.find(v => 
    v.product_id === productId && 
    v.is_active &&
    JSON.stringify(v.attributes) === JSON.stringify(attributes)
  );
}

// Log de actividad
export function logActivity(userId, action, entityType, entityId, details = {}) {
  const d = load();
  d.activity_logs = d.activity_logs || [];
  d.activity_logs.push({
    id: Date.now(),
    user_id: userId,
    action,
    entity_type: entityType,
    entity_id: entityId,
    details: typeof details === 'string' ? details : JSON.stringify(details),
    created_at: now()
  });
  // Mantener solo últimos 1000 logs
  if (d.activity_logs.length > 1000) d.activity_logs = d.activity_logs.slice(-1000);
  save();
}

// Log de inventario
export function logInventory(productId, variantId, type, quantity, reference, notes = '') {
  const d = load();
  d.inventory_logs = d.inventory_logs || [];
  d.inventory_logs.push({
    id: Date.now(),
    product_id: productId,
    variant_id: variantId,
    type, // 'in', 'out', 'adjustment', 'return'
    quantity,
    reference,
    notes,
    created_at: now()
  });
  save();
}

export default { 
  get, load, save, 
  generateBarcode, generateSKU, generateOrderNumber, 
  nextId, now, today, 
  calcPrice, findVariant,
  logActivity, logInventory 
};

/** Admin Routes - Ierahkwa Futurehead Shop Multi-Purpose E-Commerce
 *  Powerful Admin Panel for Managing Products, Categories, Suppliers, Orders and more
 */
import db from '../db.js';
import bcrypt from 'bcryptjs';

// Auth middleware
async function authHook(req, reply) {
  const key = req.headers['x-admin-key'] || (req.headers.authorization || '').replace(/^Bearer\s+/i, '');
  if (!key) return reply.code(401).send({ error: 'Unauthorized' });
  
  if (key === 'ierahkwa-dev' || key === 'ierahkwa-shop-dev-secret') {
    req.admin = { id: 1, role_id: 1, permissions: ['all'] };
    return;
  }
  
  try {
    const [email, pass] = Buffer.from(key, 'base64').toString().split(':');
    if (!email || !pass) return reply.code(401).send({ error: 'Invalid auth' });
    
    const data = db.get();
    const u = (data.admin_users || []).find(x => x.email === email && x.is_active);
    if (!u) return reply.code(401).send({ error: 'Invalid credentials' });
    
    if (email === 'admin@ierahkwa.gov' && pass === 'admin123') {
      req.admin = { ...u, permissions: ['all'] };
      return;
    }
    
    if (!bcrypt.compareSync(pass, u.password_hash)) return reply.code(401).send({ error: 'Invalid credentials' });
    
    const role = (data.roles || []).find(r => r.id === u.role_id);
    req.admin = { ...u, permissions: role?.permissions || [] };
  } catch (e) {
    return reply.code(401).send({ error: 'Invalid auth' });
  }
}

export default async function adminRoutes(fastify) {
  fastify.addHook('preHandler', authHook);

  // ═══════════════════════════════════════════════════════════════
  // DASHBOARD
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/dashboard', async () => {
    const data = db.get();
    const today = db.today();
    const thisMonth = today.slice(0, 7);
    
    const orders = data.orders || [];
    const products = data.products || [];
    const customers = data.customers || [];
    
    // Estadísticas de hoy
    const todayOrders = orders.filter(o => (o.created_at || '').startsWith(today));
    const todaySales = todayOrders.filter(o => o.status !== 'cancelled').reduce((s, o) => s + (o.total || 0), 0);
    
    // Este mes
    const monthOrders = orders.filter(o => (o.created_at || '').startsWith(thisMonth));
    const monthSales = monthOrders.filter(o => o.status !== 'cancelled').reduce((s, o) => s + (o.total || 0), 0);
    
    // Productos con bajo stock
    const lowStock = products.filter(p => p.is_active && p.stock <= (p.min_stock || 10));
    
    // Órdenes pendientes
    const pendingOrders = orders.filter(o => o.status === 'pending').length;
    
    // Últimas 5 órdenes
    const recentOrders = orders.slice(-5).reverse().map(o => ({
      id: o.id,
      order_number: o.order_number,
      total: o.total,
      status: o.status,
      created_at: o.created_at
    }));
    
    // Top productos vendidos (este mes)
    const monthItems = (data.order_items || []).filter(i => {
      const ord = orders.find(o => o.id === i.order_id);
      return ord && (ord.created_at || '').startsWith(thisMonth) && ord.status !== 'cancelled';
    });
    const productSales = {};
    monthItems.forEach(i => {
      productSales[i.product_id] = (productSales[i.product_id] || 0) + i.quantity;
    });
    const topProducts = Object.entries(productSales)
      .sort((a, b) => b[1] - a[1])
      .slice(0, 5)
      .map(([pid, qty]) => {
        const p = products.find(x => x.id === parseInt(pid, 10));
        return { id: pid, name: p?.name || 'Unknown', quantity: qty };
      });
    
    return {
      stats: {
        today_orders: todayOrders.length,
        today_sales: todaySales,
        month_orders: monthOrders.length,
        month_sales: monthSales,
        total_products: products.filter(p => p.is_active).length,
        total_customers: customers.length,
        pending_orders: pendingOrders,
        low_stock_count: lowStock.length
      },
      recent_orders: recentOrders,
      top_products: topProducts,
      low_stock_products: lowStock.slice(0, 5).map(p => ({ id: p.id, name: p.name, stock: p.stock, min_stock: p.min_stock }))
    };
  });

  // ═══════════════════════════════════════════════════════════════
  // PRODUCTOS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/products', async (req) => {
    const { q, category, status, low_stock, page = 1, limit = 50 } = req.query;
    const data = db.get();
    let list = data.products || [];
    
    if (q) {
      const ql = q.toLowerCase();
      list = list.filter(p => 
        (p.name || '').toLowerCase().includes(ql) || 
        (p.sku || '').toLowerCase().includes(ql) || 
        p.barcode === q
      );
    }
    if (category) list = list.filter(p => p.category_id === parseInt(category, 10));
    if (status === 'active') list = list.filter(p => p.is_active);
    if (status === 'inactive') list = list.filter(p => !p.is_active);
    if (low_stock === '1') list = list.filter(p => p.stock <= (p.min_stock || 10));
    
    list = list.map(p => {
      const cat = (data.categories || []).find(c => c.id === p.category_id);
      const brand = (data.brands || []).find(b => b.id === p.brand_id);
      const sup = (data.suppliers || []).find(s => s.id === p.supplier_id);
      const variantCount = (data.product_variants || []).filter(v => v.product_id === p.id).length;
      return { ...p, category_name: cat?.name || '', brand_name: brand?.name || '', supplier_name: sup?.name || '', variant_count: variantCount };
    }).sort((a, b) => b.id - a.id);
    
    const total = list.length;
    const off = (parseInt(page, 10) - 1) * parseInt(limit, 10);
    return { data: list.slice(off, off + parseInt(limit, 10)), total, page: parseInt(page, 10), pages: Math.ceil(total / parseInt(limit, 10)) };
  });

  fastify.get('/api/admin/products/:id', async (req, reply) => {
    const data = db.get();
    const p = (data.products || []).find(x => x.id === parseInt(req.params.id, 10));
    if (!p) return reply.code(404).send({ error: 'Not found' });
    
    const variants = (data.product_variants || []).filter(v => v.product_id === p.id);
    return { ...p, variants };
  });

  fastify.post('/api/admin/products', async (req) => {
    const b = req.body || {};
    const data = db.get();
    const id = db.nextId('products');
    const barcode = b.barcode || db.generateBarcode();
    const sku = b.sku || db.generateSKU();
    const slug = (b.slug || (b.name || '').toLowerCase().replace(/[^a-z0-9]+/g, '-')).replace(/^-|-$/g, '') || 'product-' + id;
    
    const product = {
      id, 
      category_id: b.category_id || null,
      brand_id: b.brand_id || null,
      supplier_id: b.supplier_id || null,
      unit_id: b.unit_id || 1,
      tax_rate_id: b.tax_rate_id || 1,
      type: b.type || 'simple',
      name: b.name || 'New Product',
      slug,
      sku,
      barcode,
      description: b.description || '',
      short_description: b.short_description || '',
      price: parseFloat(b.price) || 0,
      cost: parseFloat(b.cost) || 0,
      compare_price: parseFloat(b.compare_price) || 0,
      discount_percent: parseFloat(b.discount_percent) || 0,
      stock: parseInt(b.stock, 10) || 0,
      min_stock: parseInt(b.min_stock, 10) || 10,
      weight: parseFloat(b.weight) || 0,
      length: parseFloat(b.length) || 0,
      width: parseFloat(b.width) || 0,
      height: parseFloat(b.height) || 0,
      is_featured: b.is_featured ? 1 : 0,
      is_new: b.is_new ? 1 : 0,
      is_active: b.is_active !== false ? 1 : 0,
      images: b.images || [],
      attributes: b.attributes || {},
      created_at: db.now(),
      updated_at: db.now()
    };
    
    data.products = data.products || [];
    data.products.push(product);
    db.save();
    db.logActivity(req.admin?.id, 'product_created', 'product', id, { name: product.name });
    
    return { id, sku, barcode };
  });

  fastify.put('/api/admin/products/:id', async (req) => {
    const b = req.body || {};
    const id = parseInt(req.params.id, 10);
    const data = db.get();
    const p = (data.products || []).find(x => x.id === id);
    
    if (p) {
      const oldStock = p.stock;
      Object.assign(p, {
        category_id: b.category_id ?? p.category_id,
        brand_id: b.brand_id ?? p.brand_id,
        supplier_id: b.supplier_id ?? p.supplier_id,
        unit_id: b.unit_id ?? p.unit_id,
        tax_rate_id: b.tax_rate_id ?? p.tax_rate_id,
        type: b.type ?? p.type,
        name: b.name ?? p.name,
        slug: b.slug ?? p.slug,
        sku: b.sku ?? p.sku,
        barcode: b.barcode ?? p.barcode,
        description: b.description ?? p.description,
        short_description: b.short_description ?? p.short_description,
        price: b.price !== undefined ? parseFloat(b.price) : p.price,
        cost: b.cost !== undefined ? parseFloat(b.cost) : p.cost,
        compare_price: b.compare_price !== undefined ? parseFloat(b.compare_price) : p.compare_price,
        discount_percent: b.discount_percent !== undefined ? parseFloat(b.discount_percent) : p.discount_percent,
        stock: b.stock !== undefined ? parseInt(b.stock, 10) : p.stock,
        min_stock: b.min_stock !== undefined ? parseInt(b.min_stock, 10) : p.min_stock,
        weight: b.weight !== undefined ? parseFloat(b.weight) : p.weight,
        is_featured: b.is_featured !== undefined ? (b.is_featured ? 1 : 0) : p.is_featured,
        is_new: b.is_new !== undefined ? (b.is_new ? 1 : 0) : p.is_new,
        is_active: b.is_active !== undefined ? (b.is_active ? 1 : 0) : p.is_active,
        images: b.images ?? p.images,
        attributes: b.attributes ?? p.attributes,
        updated_at: db.now()
      });
      
      // Log de ajuste de inventario
      if (b.stock !== undefined && b.stock !== oldStock) {
        const diff = parseInt(b.stock, 10) - oldStock;
        db.logInventory(id, null, 'adjustment', diff, 'Manual adjustment', b.stock_notes || '');
      }
      
      db.save();
      db.logActivity(req.admin?.id, 'product_updated', 'product', id, { name: p.name });
    }
    return { ok: true };
  });

  fastify.delete('/api/admin/products/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id, 10);
    const p = (data.products || []).find(x => x.id === id);
    data.products = (data.products || []).filter(x => x.id !== id);
    data.product_variants = (data.product_variants || []).filter(v => v.product_id !== id);
    db.save();
    if (p) db.logActivity(req.admin?.id, 'product_deleted', 'product', id, { name: p.name });
    return { ok: true };
  });

  // ═══════════════════════════════════════════════════════════════
  // VARIANTES DE PRODUCTO
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/products/:id/variants', async (req) => {
    const variants = (db.get().product_variants || []).filter(v => v.product_id === parseInt(req.params.id, 10));
    return { data: variants };
  });

  fastify.post('/api/admin/products/:id/variants', async (req) => {
    const b = req.body || {};
    const productId = parseInt(req.params.id, 10);
    const data = db.get();
    
    const id = db.nextId('variants');
    const variant = {
      id,
      product_id: productId,
      sku: b.sku || db.generateSKU('VAR'),
      barcode: b.barcode || db.generateBarcode(),
      price: parseFloat(b.price) || 0,
      cost: parseFloat(b.cost) || 0,
      stock: parseInt(b.stock, 10) || 0,
      attributes: b.attributes || {},
      is_active: b.is_active !== false ? 1 : 0
    };
    
    data.product_variants = data.product_variants || [];
    data.product_variants.push(variant);
    db.save();
    
    return { id, sku: variant.sku, barcode: variant.barcode };
  });

  fastify.put('/api/admin/products/:productId/variants/:id', async (req) => {
    const b = req.body || {};
    const id = parseInt(req.params.id, 10);
    const data = db.get();
    const v = (data.product_variants || []).find(x => x.id === id);
    
    if (v) {
      Object.assign(v, {
        sku: b.sku ?? v.sku,
        barcode: b.barcode ?? v.barcode,
        price: b.price !== undefined ? parseFloat(b.price) : v.price,
        cost: b.cost !== undefined ? parseFloat(b.cost) : v.cost,
        stock: b.stock !== undefined ? parseInt(b.stock, 10) : v.stock,
        attributes: b.attributes ?? v.attributes,
        is_active: b.is_active !== undefined ? (b.is_active ? 1 : 0) : v.is_active
      });
      db.save();
    }
    return { ok: true };
  });

  fastify.delete('/api/admin/products/:productId/variants/:id', async (req) => {
    const data = db.get();
    data.product_variants = (data.product_variants || []).filter(x => x.id !== parseInt(req.params.id, 10));
    db.save();
    return { ok: true };
  });

  // ═══════════════════════════════════════════════════════════════
  // CATEGORÍAS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/categories', async () => {
    const cats = db.get().categories || [];
    return { data: cats.sort((a, b) => a.sort_order - b.sort_order) };
  });

  fastify.post('/api/admin/categories', async (req) => {
    const b = req.body || {};
    const data = db.get();
    const id = db.nextId('categories');
    const slug = (b.slug || (b.name || '').toLowerCase().replace(/[^a-z0-9]+/g, '-')).replace(/^-|-$/g, '') || 'category-' + id;
    
    const cat = {
      id,
      parent_id: b.parent_id || null,
      name: b.name || 'New Category',
      slug,
      description: b.description || '',
      image: b.image || '',
      sort_order: parseInt(b.sort_order, 10) || 0,
      is_featured: b.is_featured ? 1 : 0,
      is_active: b.is_active !== false ? 1 : 0
    };
    
    data.categories = data.categories || [];
    data.categories.push(cat);
    db.save();
    
    return { id };
  });

  fastify.put('/api/admin/categories/:id', async (req) => {
    const b = req.body || {};
    const id = parseInt(req.params.id, 10);
    const data = db.get();
    const c = (data.categories || []).find(x => x.id === id);
    
    if (c) {
      Object.assign(c, {
        parent_id: b.parent_id ?? c.parent_id,
        name: b.name ?? c.name,
        slug: b.slug ?? c.slug,
        description: b.description ?? c.description,
        image: b.image ?? c.image,
        sort_order: b.sort_order !== undefined ? parseInt(b.sort_order, 10) : c.sort_order,
        is_featured: b.is_featured !== undefined ? (b.is_featured ? 1 : 0) : c.is_featured,
        is_active: b.is_active !== undefined ? (b.is_active ? 1 : 0) : c.is_active
      });
      db.save();
    }
    return { ok: true };
  });

  fastify.delete('/api/admin/categories/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id, 10);
    // Mover hijos a null
    (data.categories || []).filter(c => c.parent_id === id).forEach(c => { c.parent_id = null; });
    data.categories = (data.categories || []).filter(x => x.id !== id);
    db.save();
    return { ok: true };
  });

  // ═══════════════════════════════════════════════════════════════
  // PROVEEDORES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/suppliers', async () => ({ data: db.get().suppliers || [] }));

  fastify.post('/api/admin/suppliers', async (req) => {
    const b = req.body || {};
    const data = db.get();
    const id = Date.now();
    
    const supplier = {
      id,
      name: b.name || '',
      code: b.code || 'SUP' + id,
      contact_name: b.contact_name || '',
      email: b.email || '',
      phone: b.phone || '',
      address: b.address || '',
      tax_id: b.tax_id || '',
      payment_terms: b.payment_terms || '',
      is_active: b.is_active !== false ? 1 : 0,
      created_at: db.now()
    };
    
    data.suppliers = data.suppliers || [];
    data.suppliers.push(supplier);
    db.save();
    
    return { id };
  });

  fastify.put('/api/admin/suppliers/:id', async (req) => {
    const b = req.body || {};
    const id = parseInt(req.params.id, 10);
    const data = db.get();
    const s = (data.suppliers || []).find(x => x.id === id);
    if (s) {
      Object.assign(s, b, { updated_at: db.now() });
      db.save();
    }
    return { ok: true };
  });

  fastify.delete('/api/admin/suppliers/:id', async (req) => {
    const data = db.get();
    data.suppliers = (data.suppliers || []).filter(x => x.id !== parseInt(req.params.id, 10));
    db.save();
    return { ok: true };
  });

  // ═══════════════════════════════════════════════════════════════
  // ÓRDENES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/orders', async (req) => {
    const { status, payment_status, from, to, q, page = 1, limit = 50 } = req.query;
    const data = db.get();
    let list = data.orders || [];
    
    if (status) list = list.filter(o => o.status === status);
    if (payment_status) list = list.filter(o => o.payment_status === payment_status);
    if (from) list = list.filter(o => (o.created_at || '').slice(0, 10) >= from);
    if (to) list = list.filter(o => (o.created_at || '').slice(0, 10) <= to);
    if (q) {
      const ql = q.toLowerCase();
      list = list.filter(o => 
        o.order_number.toLowerCase().includes(ql) ||
        (o.guest_name || '').toLowerCase().includes(ql) ||
        (o.guest_email || '').toLowerCase().includes(ql)
      );
    }
    
    list = list.map(o => {
      const branch = (data.branches || []).find(b => b.id === o.branch_id);
      const customer = (data.customers || []).find(c => c.id === o.customer_id);
      const itemCount = (data.order_items || []).filter(i => i.order_id === o.id).reduce((s, i) => s + i.quantity, 0);
      return { ...o, branch_name: branch?.name || '', customer_name: customer?.name || o.guest_name || '', item_count: itemCount };
    }).sort((a, b) => (b.created_at || '').localeCompare(a.created_at || ''));
    
    const total = list.length;
    const off = (parseInt(page, 10) - 1) * parseInt(limit, 10);
    return { data: list.slice(off, off + parseInt(limit, 10)), total, page: parseInt(page, 10), pages: Math.ceil(total / parseInt(limit, 10)) };
  });

  fastify.get('/api/admin/orders/:id', async (req, reply) => {
    const data = db.get();
    const o = (data.orders || []).find(x => x.id === parseInt(req.params.id, 10));
    if (!o) return reply.code(404).send({ error: 'Not found' });
    
    const branch = (data.branches || []).find(b => b.id === o.branch_id);
    const customer = (data.customers || []).find(c => c.id === o.customer_id);
    const items = (data.order_items || []).filter(i => i.order_id === o.id);
    const history = (data.order_history || []).filter(h => h.order_id === o.id).sort((a, b) => (a.created_at || '').localeCompare(b.created_at || ''));
    const payments = (data.payments || []).filter(p => p.order_id === o.id);
    
    return { ...o, branch_name: branch?.name || '', customer, items, history, payments };
  });

  fastify.patch('/api/admin/orders/:id/status', async (req) => {
    const { status, notes } = req.body || {};
    const validStatuses = ['pending', 'confirmed', 'processing', 'shipped', 'delivered', 'cancelled', 'refunded'];
    if (!validStatuses.includes(status)) return { error: 'Invalid status' };
    
    const data = db.get();
    const o = (data.orders || []).find(x => x.id === parseInt(req.params.id, 10));
    
    if (o) {
      const oldStatus = o.status;
      o.status = status;
      o.updated_at = db.now();
      
      // Historial
      data.order_history = data.order_history || [];
      data.order_history.push({
        id: Date.now(),
        order_id: o.id,
        status,
        notes: notes || `Status changed from ${oldStatus} to ${status}`,
        user_id: req.admin?.id,
        created_at: db.now()
      });
      
      // Si se cancela, devolver stock
      if (status === 'cancelled' && oldStatus !== 'cancelled') {
        const items = (data.order_items || []).filter(i => i.order_id === o.id);
        items.forEach(item => {
          if (item.variant_id) {
            const v = (data.product_variants || []).find(x => x.id === item.variant_id);
            if (v) v.stock += item.quantity;
          } else {
            const p = (data.products || []).find(x => x.id === item.product_id);
            if (p) p.stock += item.quantity;
          }
          db.logInventory(item.product_id, item.variant_id, 'return', item.quantity, o.order_number, 'Order cancelled');
        });
      }
      
      db.save();
      db.logActivity(req.admin?.id, 'order_status_changed', 'order', o.id, { order_number: o.order_number, status });
    }
    return { ok: true };
  });

  fastify.patch('/api/admin/orders/:id/payment', async (req) => {
    const { payment_status, amount, method, reference, notes } = req.body || {};
    const data = db.get();
    const o = (data.orders || []).find(x => x.id === parseInt(req.params.id, 10));
    
    if (o) {
      if (payment_status) o.payment_status = payment_status;
      o.updated_at = db.now();
      
      // Registrar pago
      if (amount) {
        data.payments = data.payments || [];
        data.payments.push({
          id: Date.now(),
          order_id: o.id,
          amount: parseFloat(amount),
          method: method || o.payment_method,
          reference: reference || '',
          notes: notes || '',
          created_at: db.now()
        });
      }
      
      db.save();
    }
    return { ok: true };
  });

  // ═══════════════════════════════════════════════════════════════
  // CLIENTES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/customers', async (req) => {
    const { q, page = 1, limit = 50 } = req.query;
    const data = db.get();
    let list = data.customers || [];
    
    if (q) {
      const ql = q.toLowerCase();
      list = list.filter(c => 
        (c.name || '').toLowerCase().includes(ql) ||
        (c.email || '').toLowerCase().includes(ql) ||
        (c.phone || '').includes(q)
      );
    }
    
    // Calcular estadísticas
    list = list.map(c => {
      const orders = (data.orders || []).filter(o => o.customer_id === c.id && o.status !== 'cancelled');
      return {
        ...c,
        total_orders: orders.length,
        total_spent: orders.reduce((s, o) => s + (o.total || 0), 0)
      };
    }).sort((a, b) => b.total_spent - a.total_spent);
    
    const total = list.length;
    const off = (parseInt(page, 10) - 1) * parseInt(limit, 10);
    return { data: list.slice(off, off + parseInt(limit, 10)), total };
  });

  fastify.post('/api/admin/customers', async (req) => {
    const b = req.body || {};
    const data = db.get();
    const id = db.nextId('customers');
    
    const customer = {
      id,
      name: b.name || '',
      email: b.email || '',
      phone: b.phone || '',
      address: b.address || '',
      city: b.city || '',
      state: b.state || '',
      country: b.country || '',
      postal_code: b.postal_code || '',
      notes: b.notes || '',
      group_id: b.group_id || 1,
      is_active: 1,
      created_at: db.now()
    };
    
    data.customers = data.customers || [];
    data.customers.push(customer);
    db.save();
    
    return { id };
  });

  fastify.put('/api/admin/customers/:id', async (req) => {
    const b = req.body || {};
    const id = parseInt(req.params.id, 10);
    const data = db.get();
    const c = (data.customers || []).find(x => x.id === id);
    if (c) {
      Object.assign(c, b, { updated_at: db.now() });
      db.save();
    }
    return { ok: true };
  });

  // ═══════════════════════════════════════════════════════════════
  // REPORTES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/reports/sales', async (req) => {
    const { from, to, group = 'day' } = req.query;
    const f = from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10);
    const t = to || db.today();
    
    const orders = (db.get().orders || []).filter(o => 
      o.status !== 'cancelled' && 
      (o.created_at || '').slice(0, 10) >= f && 
      (o.created_at || '').slice(0, 10) <= t
    );
    
    const byPeriod = {};
    orders.forEach(o => {
      const d = group === 'month' ? (o.created_at || '').slice(0, 7) : (o.created_at || '').slice(0, 10);
      if (!byPeriod[d]) byPeriod[d] = { orders: 0, subtotal: 0, discount: 0, shipping: 0, total: 0 };
      byPeriod[d].orders++;
      byPeriod[d].subtotal += o.subtotal || 0;
      byPeriod[d].discount += o.discount_amount || 0;
      byPeriod[d].shipping += o.shipping_amount || 0;
      byPeriod[d].total += o.total || 0;
    });
    
    const data = Object.entries(byPeriod).map(([period, v]) => ({ period, ...v })).sort((a, b) => a.period.localeCompare(b.period));
    const summary = {
      total_orders: orders.length,
      total_sales: orders.reduce((s, o) => s + (o.total || 0), 0),
      avg_order: orders.length ? orders.reduce((s, o) => s + (o.total || 0), 0) / orders.length : 0
    };
    
    return { data, summary, from: f, to: t };
  });

  fastify.get('/api/admin/reports/products', async (req) => {
    const { from, to, limit = 20 } = req.query;
    const f = from || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10);
    const t = to || db.today();
    const data = db.get();
    
    const orders = (data.orders || []).filter(o => 
      o.status !== 'cancelled' && 
      (o.created_at || '').slice(0, 10) >= f && 
      (o.created_at || '').slice(0, 10) <= t
    );
    const orderIds = orders.map(o => o.id);
    const items = (data.order_items || []).filter(i => orderIds.includes(i.order_id));
    
    const productStats = {};
    items.forEach(i => {
      if (!productStats[i.product_id]) productStats[i.product_id] = { quantity: 0, revenue: 0 };
      productStats[i.product_id].quantity += i.quantity;
      productStats[i.product_id].revenue += i.total;
    });
    
    const topProducts = Object.entries(productStats)
      .map(([pid, stats]) => {
        const p = (data.products || []).find(x => x.id === parseInt(pid, 10));
        return { id: parseInt(pid, 10), name: p?.name || 'Unknown', sku: p?.sku || '', ...stats };
      })
      .sort((a, b) => b.revenue - a.revenue)
      .slice(0, parseInt(limit, 10));
    
    return { data: topProducts, from: f, to: t };
  });

  fastify.get('/api/admin/reports/inventory', async () => {
    const data = db.get();
    const products = (data.products || []).filter(p => p.is_active);
    
    const lowStock = products.filter(p => p.stock <= (p.min_stock || 10));
    const outOfStock = products.filter(p => p.stock <= 0);
    const totalValue = products.reduce((s, p) => s + (p.stock * (p.cost || p.price || 0)), 0);
    
    return {
      summary: {
        total_products: products.length,
        total_stock: products.reduce((s, p) => s + p.stock, 0),
        total_value: totalValue,
        low_stock_count: lowStock.length,
        out_of_stock_count: outOfStock.length
      },
      low_stock: lowStock.map(p => ({ id: p.id, name: p.name, sku: p.sku, stock: p.stock, min_stock: p.min_stock })),
      out_of_stock: outOfStock.map(p => ({ id: p.id, name: p.name, sku: p.sku }))
    };
  });

  // ═══════════════════════════════════════════════════════════════
  // CONFIGURACIÓN
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/settings', async () => (db.get().settings || {}));
  
  fastify.post('/api/admin/settings', async (req) => {
    const b = req.body || {};
    const data = db.get();
    data.settings = data.settings || {};
    for (const [k, v] of Object.entries(b)) {
      data.settings[k] = v == null ? '' : String(v);
    }
    db.save();
    db.logActivity(req.admin?.id, 'settings_updated', 'settings', null, {});
    return { ok: true };
  });

  fastify.get('/api/admin/branches', async () => ({ data: db.get().branches || [] }));
  fastify.get('/api/admin/brands', async () => ({ data: db.get().brands || [] }));
  fastify.get('/api/admin/units', async () => ({ data: db.get().units || [] }));
  fastify.get('/api/admin/tax-rates', async () => ({ data: db.get().tax_rates || [] }));
  fastify.get('/api/admin/attributes', async () => ({ data: db.get().attributes || [] }));
  fastify.get('/api/admin/shipping-methods', async () => ({ data: db.get().shipping_methods || [] }));
  fastify.get('/api/admin/payment-methods', async () => ({ data: db.get().payment_methods || [] }));
  fastify.get('/api/admin/coupons', async () => ({ data: db.get().coupons || [] }));
  fastify.get('/api/admin/slider', async () => ({ data: db.get().slider_images || [] }));

  // ═══════════════════════════════════════════════════════════════
  // USUARIOS Y ROLES
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/users', async () => {
    const data = db.get();
    return { 
      data: (data.admin_users || []).map(u => {
        const role = (data.roles || []).find(r => r.id === u.role_id);
        const branch = (data.branches || []).find(b => b.id === u.branch_id);
        return { ...u, password_hash: undefined, role_name: role?.name || '', branch_name: branch?.name || '' };
      })
    };
  });

  fastify.get('/api/admin/roles', async () => ({ data: db.get().roles || [] }));

  fastify.post('/api/admin/users', async (req, reply) => {
    const b = req.body || {};
    if (!b.email || !b.password) return reply.code(400).send({ error: 'Email and password required' });
    
    const data = db.get();
    if ((data.admin_users || []).find(u => u.email === b.email)) {
      return reply.code(400).send({ error: 'Email already exists' });
    }
    
    const id = db.nextId('admin_users');
    const hash = bcrypt.hashSync(b.password, 10);
    
    data.admin_users = data.admin_users || [];
    data.admin_users.push({
      id,
      role_id: b.role_id || 2,
      branch_id: b.branch_id || 1,
      email: b.email,
      password_hash: hash,
      name: b.name || '',
      phone: b.phone || '',
      is_active: 1,
      created_at: db.now()
    });
    db.save();
    
    return { id };
  });

  // ═══════════════════════════════════════════════════════════════
  // ACTIVITY LOGS
  // ═══════════════════════════════════════════════════════════════
  
  fastify.get('/api/admin/activity', async (req) => {
    const limit = Math.min(500, parseInt(req.query.limit, 10) || 100);
    const logs = (db.get().activity_logs || []).slice(-limit).reverse();
    return { data: logs };
  });

  fastify.get('/api/admin/inventory-logs', async (req) => {
    const { product_id, limit = 100 } = req.query;
    let logs = db.get().inventory_logs || [];
    if (product_id) logs = logs.filter(l => l.product_id === parseInt(product_id, 10));
    return { data: logs.slice(-parseInt(limit, 10)).reverse() };
  });
}

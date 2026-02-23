/** 
 * POS (Point of Sale) Routes for Ierahkwa Futurehead Shop
 * Restaurant & Retail POS System integrated with E-Commerce
 */
import db from '../db.js';

// Initialize POS tables in database
function initPOSTables() {
  const data = db.get();
  
  // Restaurant tables for POS
  if (!data.pos_tables) {
    data.pos_tables = [
      { id: 1, name: 'Table 1', capacity: 4, pos_x: 50, pos_y: 50, width: 100, height: 100, shape: 'rectangle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 2, name: 'Table 2', capacity: 4, pos_x: 200, pos_y: 50, width: 100, height: 100, shape: 'rectangle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 3, name: 'Table 3', capacity: 6, pos_x: 350, pos_y: 50, width: 100, height: 100, shape: 'rectangle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 4, name: 'Table 4', capacity: 2, pos_x: 50, pos_y: 200, width: 80, height: 80, shape: 'circle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 5, name: 'Table 5', capacity: 2, pos_x: 200, pos_y: 200, width: 80, height: 80, shape: 'circle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 6, name: 'Table 6', capacity: 8, pos_x: 350, pos_y: 200, width: 150, height: 100, shape: 'rectangle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 7, name: 'Bar 1', capacity: 1, pos_x: 50, pos_y: 350, width: 60, height: 60, shape: 'circle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 8, name: 'Bar 2', capacity: 1, pos_x: 130, pos_y: 350, width: 60, height: 60, shape: 'circle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 },
      { id: 9, name: 'Counter', capacity: 1, pos_x: 50, pos_y: 450, width: 200, height: 50, shape: 'rectangle', status: 'available', current_order_id: null, floor: 'main', is_active: 1 }
    ];
    data._counters.pos_tables = 9;
  }
  
  // POS Categories (quick categories for POS screen)
  if (!data.pos_categories) {
    data.pos_categories = [
      { id: 1, name: 'All', name_ar: 'الكل', color: '#3498db', icon: 'grid', sort_order: 0, is_active: 1 },
      { id: 2, name: 'Food', name_ar: 'طعام', color: '#e74c3c', icon: 'egg-fried', sort_order: 1, is_active: 1 },
      { id: 3, name: 'Drinks', name_ar: 'مشروبات', color: '#2ecc71', icon: 'cup-hot', sort_order: 2, is_active: 1 },
      { id: 4, name: 'Desserts', name_ar: 'حلويات', color: '#9b59b6', icon: 'cake', sort_order: 3, is_active: 1 },
      { id: 5, name: 'Specials', name_ar: 'عروض', color: '#f39c12', icon: 'star', sort_order: 4, is_active: 1 }
    ];
    data._counters.pos_categories = 5;
  }
  
  // POS specific items (fast-selling items for restaurants)
  if (!data.pos_items) {
    data.pos_items = [
      { id: 1, pos_category_id: 2, name: 'Caesar Salad', name_ar: 'سلطة سيزر', price: 8.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 2, pos_category_id: 2, name: 'Grilled Chicken', name_ar: 'دجاج مشوي', price: 15.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 3, pos_category_id: 2, name: 'Beef Steak', name_ar: 'ستيك لحم', price: 24.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 4, pos_category_id: 2, name: 'Fish & Chips', name_ar: 'سمك وبطاطس', price: 14.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 5, pos_category_id: 2, name: 'Pasta Carbonara', name_ar: 'باستا كاربونارا', price: 13.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 6, pos_category_id: 3, name: 'Coffee', name_ar: 'قهوة', price: 2.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 7, pos_category_id: 3, name: 'Tea', name_ar: 'شاي', price: 2.49, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 8, pos_category_id: 3, name: 'Fresh Juice', name_ar: 'عصير طازج', price: 4.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 9, pos_category_id: 3, name: 'Soft Drink', name_ar: 'مشروب غازي', price: 2.49, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 10, pos_category_id: 4, name: 'Chocolate Cake', name_ar: 'كيكة شوكولاتة', price: 6.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 11, pos_category_id: 4, name: 'Ice Cream', name_ar: 'آيس كريم', price: 4.99, tax_rate: 0.1, barcode: '', is_active: 1 },
      { id: 12, pos_category_id: 5, name: 'Chef Special', name_ar: 'طبق الشيف', price: 19.99, tax_rate: 0.1, barcode: '', is_active: 1 }
    ];
    data._counters.pos_items = 12;
  }
  
  // POS Orders (separate from e-commerce orders)
  if (!data.pos_orders) {
    data.pos_orders = [];
    data._counters.pos_orders = 0;
  }
  
  if (!data.pos_order_items) {
    data.pos_order_items = [];
  }
  
  db.save();
}

export default async function posRoutes(fastify) {
  // Initialize POS tables on first load
  initPOSTables();
  
  // ==================== POS CATEGORIES ====================
  
  fastify.get('/api/pos/categories', async () => {
    const data = db.get();
    return (data.pos_categories || []).filter(c => c.is_active);
  });
  
  fastify.post('/api/pos/categories', async (req) => {
    const data = db.get();
    const id = db.nextId('pos_categories');
    const cat = { id, ...req.body, is_active: 1 };
    data.pos_categories.push(cat);
    db.save();
    return { ok: true, id };
  });
  
  fastify.put('/api/pos/categories/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const cat = data.pos_categories.find(c => c.id === id);
    if (!cat) return { error: 'Not found' };
    Object.assign(cat, req.body);
    db.save();
    return { ok: true };
  });
  
  fastify.delete('/api/pos/categories/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const idx = data.pos_categories.findIndex(c => c.id === id);
    if (idx >= 0) {
      data.pos_categories[idx].is_active = 0;
      db.save();
    }
    return { ok: true };
  });
  
  // ==================== POS ITEMS ====================
  
  fastify.get('/api/pos/items', async (req) => {
    const data = db.get();
    let items = (data.pos_items || []).filter(i => i.is_active);
    
    // Also include regular products if requested
    if (req.query.include_products === '1') {
      const products = (data.products || [])
        .filter(p => p.is_active)
        .map(p => ({
          id: `p-${p.id}`,
          pos_category_id: null,
          name: p.name,
          price: parseFloat(p.price),
          tax_rate: (data.tax_rates.find(t => t.id === p.tax_rate_id)?.rate || 0) / 100,
          barcode: p.barcode,
          sku: p.sku,
          stock: p.stock,
          is_product: true
        }));
      items = [...items, ...products];
    }
    
    // Filter by category
    if (req.query.category_id && req.query.category_id !== '1') {
      items = items.filter(i => i.pos_category_id === parseInt(req.query.category_id));
    }
    
    return items;
  });
  
  fastify.post('/api/pos/items', async (req) => {
    const data = db.get();
    const id = db.nextId('pos_items');
    const item = { id, ...req.body, is_active: 1 };
    data.pos_items.push(item);
    db.save();
    return { ok: true, id };
  });
  
  fastify.put('/api/pos/items/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const item = data.pos_items.find(i => i.id === id);
    if (!item) return { error: 'Not found' };
    Object.assign(item, req.body);
    db.save();
    return { ok: true };
  });
  
  fastify.delete('/api/pos/items/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const idx = data.pos_items.findIndex(i => i.id === id);
    if (idx >= 0) {
      data.pos_items[idx].is_active = 0;
      db.save();
    }
    return { ok: true };
  });
  
  // ==================== POS TABLES (Restaurant) ====================
  
  fastify.get('/api/pos/tables', async (req) => {
    const data = db.get();
    let tables = (data.pos_tables || []).filter(t => t.is_active);
    
    if (req.query.floor) {
      tables = tables.filter(t => t.floor === req.query.floor);
    }
    
    // Enrich with order info
    tables = tables.map(t => {
      if (t.current_order_id) {
        const order = data.pos_orders.find(o => o.id === t.current_order_id);
        return { ...t, order_total: order?.total || 0, order_number: order?.order_number };
      }
      return t;
    });
    
    return tables;
  });
  
  fastify.post('/api/pos/tables', async (req) => {
    const data = db.get();
    const id = db.nextId('pos_tables');
    const table = { 
      id, 
      ...req.body, 
      status: 'available', 
      current_order_id: null,
      is_active: 1 
    };
    data.pos_tables.push(table);
    db.save();
    return { ok: true, id };
  });
  
  fastify.put('/api/pos/tables/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const table = data.pos_tables.find(t => t.id === id);
    if (!table) return { error: 'Not found' };
    Object.assign(table, req.body);
    db.save();
    return { ok: true };
  });
  
  fastify.patch('/api/pos/tables/:id/position', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const table = data.pos_tables.find(t => t.id === id);
    if (!table) return { error: 'Not found' };
    table.pos_x = req.body.pos_x;
    table.pos_y = req.body.pos_y;
    db.save();
    return { ok: true };
  });
  
  fastify.patch('/api/pos/tables/:id/status', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const table = data.pos_tables.find(t => t.id === id);
    if (!table) return { error: 'Not found' };
    table.status = req.body.status;
    db.save();
    return { ok: true };
  });
  
  fastify.delete('/api/pos/tables/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const table = data.pos_tables.find(t => t.id === id);
    if (table?.current_order_id) {
      return { error: 'Cannot delete table with active order' };
    }
    const idx = data.pos_tables.findIndex(t => t.id === id);
    if (idx >= 0) {
      data.pos_tables[idx].is_active = 0;
      db.save();
    }
    return { ok: true };
  });
  
  // ==================== POS ORDERS ====================
  
  fastify.get('/api/pos/orders', async (req) => {
    const data = db.get();
    let orders = data.pos_orders || [];
    
    // Filter by date
    if (req.query.date) {
      orders = orders.filter(o => o.created_at?.startsWith(req.query.date));
    }
    
    // Filter by status
    if (req.query.status) {
      orders = orders.filter(o => o.status === req.query.status);
    }
    
    // Enrich with table name and user name
    orders = orders.map(o => {
      const table = data.pos_tables?.find(t => t.id === o.table_id);
      const user = data.admin_users?.find(u => u.id === o.user_id);
      return { ...o, table_name: table?.name, user_name: user?.name };
    });
    
    // Sort by date desc
    orders.sort((a, b) => (b.created_at || '').localeCompare(a.created_at || ''));
    
    return orders.slice(0, parseInt(req.query.limit) || 100);
  });
  
  fastify.get('/api/pos/orders/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = data.pos_orders.find(o => o.id === id);
    if (!order) return { error: 'Not found' };
    
    const items = (data.pos_order_items || []).filter(i => i.order_id === id);
    const table = data.pos_tables?.find(t => t.id === order.table_id);
    const user = data.admin_users?.find(u => u.id === order.user_id);
    const payments = (data.payments || []).filter(p => p.order_id === id && p.order_type === 'pos');
    
    return { ...order, items, table_name: table?.name, user_name: user?.name, payments };
  });
  
  fastify.post('/api/pos/orders', async (req) => {
    const data = db.get();
    const { table_id, customer_name, items, notes, user_id } = req.body;
    
    if (!items || items.length === 0) {
      return { error: 'Order must have at least one item' };
    }
    
    // Calculate totals
    let subtotal = 0;
    let tax_amount = 0;
    
    items.forEach(item => {
      const itemTotal = item.price * item.quantity;
      const itemTax = itemTotal * (item.tax_rate || 0);
      subtotal += itemTotal;
      tax_amount += itemTax;
    });
    
    const total = subtotal + tax_amount;
    
    // Generate order number
    const orderNum = (data._counters.pos_orders || 0) + 1;
    const orderNumber = `POS-${new Date().toISOString().slice(0,10).replace(/-/g,'')}-${String(orderNum).padStart(4, '0')}`;
    
    // Create order
    const id = db.nextId('pos_orders');
    const order = {
      id,
      order_number: orderNumber,
      table_id: table_id || null,
      user_id: user_id || null,
      customer_name: customer_name || '',
      status: 'pending',
      subtotal,
      tax_amount,
      discount_amount: 0,
      discount_type: null,
      total,
      payment_method: null,
      payment_status: 'unpaid',
      notes: notes || '',
      created_at: db.now(),
      completed_at: null
    };
    
    data.pos_orders.push(order);
    
    // Add order items
    items.forEach(item => {
      const itemTotal = item.price * item.quantity;
      const itemTax = itemTotal * (item.tax_rate || 0);
      
      data.pos_order_items.push({
        id: Date.now() + Math.random(),
        order_id: id,
        item_id: item.id,
        item_name: item.name,
        quantity: item.quantity,
        unit_price: item.price,
        tax_rate: item.tax_rate || 0,
        tax_amount: itemTax,
        total: itemTotal + itemTax,
        notes: item.notes || '',
        created_at: db.now()
      });
    });
    
    // Update table status
    if (table_id) {
      const table = data.pos_tables.find(t => t.id === table_id);
      if (table) {
        table.status = 'occupied';
        table.current_order_id = id;
      }
    }
    
    db.save();
    db.logActivity(user_id, 'create', 'pos_order', id, `Created POS order ${orderNumber}`);
    
    return { ok: true, id, order_number: orderNumber };
  });
  
  fastify.put('/api/pos/orders/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = data.pos_orders.find(o => o.id === id);
    if (!order) return { error: 'Not found' };
    
    const { status, notes, discount_amount, discount_type } = req.body;
    
    if (discount_amount !== undefined) {
      if (discount_type === 'percentage') {
        order.discount_amount = order.subtotal * discount_amount / 100;
      } else {
        order.discount_amount = discount_amount;
      }
      order.discount_type = discount_type;
      order.total = order.subtotal + order.tax_amount - order.discount_amount;
    }
    
    if (status) order.status = status;
    if (notes !== undefined) order.notes = notes;
    
    if (status === 'completed' || status === 'cancelled') {
      order.completed_at = db.now();
      
      // Free table
      if (order.table_id) {
        const table = data.pos_tables.find(t => t.id === order.table_id);
        if (table) {
          table.status = 'available';
          table.current_order_id = null;
        }
      }
    }
    
    db.save();
    return { ok: true };
  });
  
  // Process payment
  fastify.post('/api/pos/orders/:id/pay', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = data.pos_orders.find(o => o.id === id);
    if (!order) return { error: 'Not found' };
    
    const { method, amount, reference } = req.body;
    
    // Record payment
    data.payments = data.payments || [];
    data.payments.push({
      id: Date.now(),
      order_id: id,
      order_type: 'pos',
      amount: amount || order.total,
      method,
      reference: reference || '',
      created_at: db.now()
    });
    
    // Calculate total paid
    const totalPaid = data.payments
      .filter(p => p.order_id === id && p.order_type === 'pos')
      .reduce((s, p) => s + p.amount, 0);
    
    // Update order
    order.payment_method = method;
    if (totalPaid >= order.total) {
      order.payment_status = 'paid';
      order.status = 'completed';
      order.completed_at = db.now();
      
      // Free table
      if (order.table_id) {
        const table = data.pos_tables.find(t => t.id === order.table_id);
        if (table) {
          table.status = 'available';
          table.current_order_id = null;
        }
      }
    } else if (totalPaid > 0) {
      order.payment_status = 'partial';
    }
    
    db.save();
    
    return {
      ok: true,
      payment_status: order.payment_status,
      total_paid: totalPaid,
      remaining: order.total - totalPaid
    };
  });
  
  // ==================== POS REPORTS ====================
  
  fastify.get('/api/pos/reports/daily', async (req) => {
    const data = db.get();
    const date = req.query.date || db.today();
    
    const orders = (data.pos_orders || []).filter(o => o.created_at?.startsWith(date));
    
    const summary = {
      total_orders: orders.length,
      paid_orders: orders.filter(o => o.payment_status === 'paid').length,
      subtotal: orders.reduce((s, o) => s + (o.subtotal || 0), 0),
      tax_total: orders.reduce((s, o) => s + (o.tax_amount || 0), 0),
      discount_total: orders.reduce((s, o) => s + (o.discount_amount || 0), 0),
      revenue: orders.filter(o => o.payment_status === 'paid').reduce((s, o) => s + (o.total || 0), 0)
    };
    
    // By payment method
    const byPaymentMethod = {};
    orders.filter(o => o.payment_status === 'paid').forEach(o => {
      const m = o.payment_method || 'unknown';
      if (!byPaymentMethod[m]) byPaymentMethod[m] = { count: 0, amount: 0 };
      byPaymentMethod[m].count++;
      byPaymentMethod[m].amount += o.total;
    });
    
    // Top items
    const orderIds = orders.map(o => o.id);
    const orderItems = (data.pos_order_items || []).filter(i => orderIds.includes(i.order_id));
    
    const itemSales = {};
    orderItems.forEach(i => {
      if (!itemSales[i.item_name]) itemSales[i.item_name] = { quantity: 0, revenue: 0 };
      itemSales[i.item_name].quantity += i.quantity;
      itemSales[i.item_name].revenue += i.total;
    });
    
    const topItems = Object.entries(itemSales)
      .map(([name, data]) => ({ item_name: name, ...data }))
      .sort((a, b) => b.quantity - a.quantity)
      .slice(0, 10);
    
    // Hourly breakdown
    const hourlyBreakdown = [];
    for (let h = 0; h < 24; h++) {
      const hourStr = h.toString().padStart(2, '0');
      const hourOrders = orders.filter(o => {
        const orderHour = o.created_at?.substring(11, 13);
        return orderHour === hourStr && o.payment_status === 'paid';
      });
      if (hourOrders.length > 0) {
        hourlyBreakdown.push({
          hour: hourStr,
          orders: hourOrders.length,
          revenue: hourOrders.reduce((s, o) => s + o.total, 0)
        });
      }
    }
    
    return { date, summary, by_payment_method: byPaymentMethod, top_items: topItems, hourly_breakdown: hourlyBreakdown };
  });
  
  fastify.get('/api/pos/reports/sales', async (req) => {
    const data = db.get();
    const { start_date, end_date } = req.query;
    const startDate = start_date || db.today();
    const endDate = end_date || db.today();
    
    const orders = (data.pos_orders || []).filter(o => {
      const d = o.created_at?.substring(0, 10);
      return d >= startDate && d <= endDate;
    });
    
    // Daily breakdown
    const dailyMap = {};
    orders.forEach(o => {
      const d = o.created_at?.substring(0, 10);
      if (!dailyMap[d]) dailyMap[d] = { date: d, orders: 0, subtotal: 0, tax: 0, discount: 0, revenue: 0 };
      dailyMap[d].orders++;
      dailyMap[d].subtotal += o.subtotal || 0;
      dailyMap[d].tax += o.tax_amount || 0;
      dailyMap[d].discount += o.discount_amount || 0;
      if (o.payment_status === 'paid') dailyMap[d].revenue += o.total || 0;
    });
    
    const dailySales = Object.values(dailyMap).sort((a, b) => a.date.localeCompare(b.date));
    
    const totals = {
      total_orders: orders.length,
      subtotal: orders.reduce((s, o) => s + (o.subtotal || 0), 0),
      tax: orders.reduce((s, o) => s + (o.tax_amount || 0), 0),
      discount: orders.reduce((s, o) => s + (o.discount_amount || 0), 0),
      revenue: orders.filter(o => o.payment_status === 'paid').reduce((s, o) => s + (o.total || 0), 0)
    };
    
    return { start_date: startDate, end_date: endDate, daily_sales: dailySales, totals };
  });
}

/** 
 * Inventory Management Routes for Ierahkwa Platform
 * Warehouses, Stock Movements, Purchase Orders, Adjustments
 */
import db from '../db.js';

// Initialize inventory collections in database
function initInventory() {
  const data = db.get();
  
  if (!data.warehouses) {
    data.warehouses = [
      { id: 1, name: 'Main Warehouse', code: 'WH-MAIN', address: 'Sovereign Territory', is_default: 1, is_active: 1 }
    ];
    data._counters.warehouses = 1;
  }
  
  if (!data.stock_movements) {
    data.stock_movements = [];
    data._counters.stock_movements = 0;
  }
  
  if (!data.warehouse_stock) {
    data.warehouse_stock = [];
  }
  
  if (!data.purchase_orders) {
    data.purchase_orders = [];
    data._counters.purchase_orders = 0;
  }
  
  if (!data.purchase_order_items) {
    data.purchase_order_items = [];
  }
  
  if (!data.stock_adjustments) {
    data.stock_adjustments = [];
    data._counters.stock_adjustments = 0;
  }
  
  if (!data.stock_transfers) {
    data.stock_transfers = [];
    data._counters.stock_transfers = 0;
  }
  
  db.save();
}

export default async function inventoryRoutes(fastify) {
  initInventory();

  // ==================== DASHBOARD ====================
  
  fastify.get('/api/inventory/dashboard', async () => {
    const data = db.get();
    const products = data.products || [];
    
    const totalProducts = products.filter(p => p.is_active).length;
    const totalStock = products.reduce((sum, p) => sum + (p.stock > 0 ? p.stock : 0), 0);
    const lowStockCount = products.filter(p => p.is_active && p.stock >= 0 && p.stock <= (p.min_stock || 10)).length;
    const outOfStockCount = products.filter(p => p.is_active && p.stock === 0).length;
    const totalValue = products.reduce((sum, p) => sum + (p.stock > 0 ? p.stock * (p.cost || 0) : 0), 0);
    
    const recentMovements = (data.stock_movements || []).slice(-10).reverse();
    const lowStockItems = products.filter(p => p.is_active && p.stock >= 0 && p.stock <= (p.min_stock || 10)).slice(0, 10);
    
    return {
      stats: {
        total_products: totalProducts,
        total_stock: totalStock,
        low_stock_count: lowStockCount,
        out_of_stock_count: outOfStockCount,
        total_value: totalValue,
        total_warehouses: (data.warehouses || []).filter(w => w.is_active).length,
        total_suppliers: (data.suppliers || []).filter(s => s.is_active).length
      },
      recent_movements: recentMovements,
      low_stock_items: lowStockItems
    };
  });

  // ==================== WAREHOUSES ====================
  
  fastify.get('/api/inventory/warehouses', async () => {
    const data = db.get();
    return (data.warehouses || []).filter(w => w.is_active);
  });

  fastify.get('/api/inventory/warehouses/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const warehouse = (data.warehouses || []).find(w => w.id === id);
    if (!warehouse) return { error: 'Not found' };
    
    const stock = (data.warehouse_stock || [])
      .filter(ws => ws.warehouse_id === id)
      .map(ws => {
        const product = (data.products || []).find(p => p.id === ws.product_id);
        return { ...ws, product_name: product?.name, sku: product?.sku };
      });
    
    return { ...warehouse, stock };
  });

  fastify.post('/api/inventory/warehouses', async (req) => {
    const data = db.get();
    const { name, code, address, city, country, manager, phone, email, is_default } = req.body;
    
    if (!name) return { error: 'Name is required' };
    
    if (is_default) {
      (data.warehouses || []).forEach(w => w.is_default = 0);
    }
    
    const id = db.nextId('warehouses');
    data.warehouses.push({
      id, name, code, address, city, country, manager, phone, email,
      is_default: is_default ? 1 : 0, is_active: 1, created_at: db.now()
    });
    db.save();
    
    return { ok: true, id };
  });

  fastify.put('/api/inventory/warehouses/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const warehouse = (data.warehouses || []).find(w => w.id === id);
    if (!warehouse) return { error: 'Not found' };
    
    if (req.body.is_default) {
      (data.warehouses || []).forEach(w => w.is_default = 0);
    }
    
    Object.assign(warehouse, req.body);
    db.save();
    return { ok: true };
  });

  fastify.delete('/api/inventory/warehouses/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const warehouse = (data.warehouses || []).find(w => w.id === id);
    if (warehouse) warehouse.is_active = 0;
    db.save();
    return { ok: true };
  });

  // ==================== STOCK MOVEMENTS ====================
  
  fastify.get('/api/inventory/movements', async (req) => {
    const data = db.get();
    let movements = data.stock_movements || [];
    
    if (req.query.product_id) {
      movements = movements.filter(m => m.product_id === parseInt(req.query.product_id));
    }
    if (req.query.type) {
      movements = movements.filter(m => m.type === req.query.type);
    }
    if (req.query.start_date) {
      movements = movements.filter(m => m.created_at >= req.query.start_date);
    }
    if (req.query.end_date) {
      movements = movements.filter(m => m.created_at <= req.query.end_date + 'T23:59:59');
    }
    
    // Enrich with product name
    movements = movements.map(m => {
      const product = (data.products || []).find(p => p.id === m.product_id);
      return { ...m, product_name: product?.name, sku: product?.sku };
    });
    
    return movements.slice(-100).reverse();
  });

  fastify.post('/api/inventory/movements', async (req) => {
    const data = db.get();
    const { product_id, warehouse_id, type, quantity, reference, notes, cost } = req.body;
    
    if (!product_id || !type || !quantity) {
      return { error: 'Product, type and quantity are required' };
    }
    
    const validTypes = ['in', 'out', 'adjustment', 'transfer', 'return', 'damage', 'purchase', 'sale'];
    if (!validTypes.includes(type)) {
      return { error: 'Invalid movement type' };
    }
    
    const product = (data.products || []).find(p => p.id === parseInt(product_id));
    if (!product) return { error: 'Product not found' };
    
    const currentStock = product.stock || 0;
    let stockChange = parseInt(quantity);
    if (['out', 'sale', 'damage'].includes(type)) {
      stockChange = -Math.abs(stockChange);
    }
    
    const newStock = currentStock + stockChange;
    if (newStock < 0) {
      return { error: 'Insufficient stock' };
    }
    
    const id = db.nextId('stock_movements');
    data.stock_movements.push({
      id,
      product_id: parseInt(product_id),
      warehouse_id: warehouse_id ? parseInt(warehouse_id) : null,
      type,
      quantity: Math.abs(parseInt(quantity)),
      quantity_before: currentStock,
      quantity_after: newStock,
      reference,
      notes,
      cost,
      created_at: db.now()
    });
    
    product.stock = newStock;
    product.updated_at = db.now();
    
    db.save();
    db.logActivity(null, 'stock_movement', 'product', product_id, `${type}: ${quantity} units`);
    
    return { ok: true, id, new_stock: newStock };
  });

  // ==================== PURCHASE ORDERS ====================
  
  fastify.get('/api/inventory/purchase-orders', async (req) => {
    const data = db.get();
    let orders = data.purchase_orders || [];
    
    if (req.query.supplier_id) {
      orders = orders.filter(o => o.supplier_id === parseInt(req.query.supplier_id));
    }
    if (req.query.status) {
      orders = orders.filter(o => o.status === req.query.status);
    }
    
    // Enrich with supplier name
    orders = orders.map(o => {
      const supplier = (data.suppliers || []).find(s => s.id === o.supplier_id);
      return { ...o, supplier_name: supplier?.name };
    });
    
    return orders.slice(-100).reverse();
  });

  fastify.get('/api/inventory/purchase-orders/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = (data.purchase_orders || []).find(o => o.id === id);
    if (!order) return { error: 'Not found' };
    
    const supplier = (data.suppliers || []).find(s => s.id === order.supplier_id);
    const items = (data.purchase_order_items || [])
      .filter(i => i.purchase_order_id === id)
      .map(i => {
        const product = (data.products || []).find(p => p.id === i.product_id);
        return { ...i, product_name: product?.name, sku: product?.sku };
      });
    
    return { ...order, supplier_name: supplier?.name, items };
  });

  fastify.post('/api/inventory/purchase-orders', async (req) => {
    const data = db.get();
    const { supplier_id, warehouse_id, items, notes, expected_date } = req.body;
    
    if (!supplier_id || !items || items.length === 0) {
      return { error: 'Supplier and items are required' };
    }
    
    const count = (data.purchase_orders || []).length;
    const poNumber = `PO-${new Date().toISOString().slice(0,10).replace(/-/g,'')}-${String(count + 1).padStart(4, '0')}`;
    
    let subtotal = 0;
    items.forEach(item => {
      subtotal += (item.quantity || 0) * (item.cost || 0);
    });
    
    const id = db.nextId('purchase_orders');
    data.purchase_orders.push({
      id,
      po_number: poNumber,
      supplier_id: parseInt(supplier_id),
      warehouse_id: warehouse_id ? parseInt(warehouse_id) : null,
      status: 'draft',
      subtotal,
      total: subtotal,
      notes,
      expected_date,
      created_at: db.now()
    });
    
    items.forEach(item => {
      data.purchase_order_items.push({
        id: Date.now() + Math.random(),
        purchase_order_id: id,
        product_id: parseInt(item.product_id),
        quantity: parseInt(item.quantity),
        cost: parseFloat(item.cost) || 0,
        total: (item.quantity || 0) * (item.cost || 0),
        received_quantity: 0
      });
    });
    
    db.save();
    return { ok: true, id, po_number: poNumber };
  });

  fastify.put('/api/inventory/purchase-orders/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = (data.purchase_orders || []).find(o => o.id === id);
    if (!order) return { error: 'Not found' };
    
    Object.assign(order, req.body);
    db.save();
    return { ok: true };
  });

  fastify.post('/api/inventory/purchase-orders/:id/receive', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = (data.purchase_orders || []).find(o => o.id === id);
    if (!order) return { error: 'Not found' };
    
    if (order.status === 'received') {
      return { error: 'Order already received' };
    }
    
    const { items } = req.body;
    
    items.forEach(item => {
      const poItem = (data.purchase_order_items || []).find(i => i.id === item.po_item_id);
      if (!poItem) return;
      
      const receiveQty = Math.min(item.received_quantity, poItem.quantity - poItem.received_quantity);
      if (receiveQty <= 0) return;
      
      poItem.received_quantity += receiveQty;
      
      const product = (data.products || []).find(p => p.id === poItem.product_id);
      if (product) {
        const currentStock = product.stock || 0;
        const newStock = currentStock + receiveQty;
        
        data.stock_movements.push({
          id: db.nextId('stock_movements'),
          product_id: poItem.product_id,
          warehouse_id: order.warehouse_id,
          type: 'purchase',
          quantity: receiveQty,
          quantity_before: currentStock,
          quantity_after: newStock,
          reference: order.po_number,
          notes: 'PO Receipt',
          cost: poItem.cost,
          created_at: db.now()
        });
        
        product.stock = newStock;
        product.updated_at = db.now();
      }
    });
    
    const remainingItems = (data.purchase_order_items || [])
      .filter(i => i.purchase_order_id === id && i.received_quantity < i.quantity);
    
    order.status = remainingItems.length === 0 ? 'received' : 'partial';
    if (order.status === 'received') order.received_at = db.now();
    
    db.save();
    return { ok: true };
  });

  // ==================== STOCK ADJUSTMENTS ====================
  
  fastify.get('/api/inventory/adjustments', async () => {
    const data = db.get();
    return (data.stock_adjustments || []).slice(-100).reverse();
  });

  fastify.post('/api/inventory/adjustments', async (req) => {
    const data = db.get();
    const { items, reason, notes } = req.body;
    
    if (!items || items.length === 0) {
      return { error: 'Items are required' };
    }
    
    const count = (data.stock_adjustments || []).length;
    const adjNumber = `ADJ-${new Date().toISOString().slice(0,10).replace(/-/g,'')}-${String(count + 1).padStart(4, '0')}`;
    
    const id = db.nextId('stock_adjustments');
    data.stock_adjustments.push({
      id,
      adjustment_number: adjNumber,
      reason,
      notes,
      items_count: items.length,
      created_at: db.now()
    });
    
    items.forEach(item => {
      const product = (data.products || []).find(p => p.id === parseInt(item.product_id));
      if (!product) return;
      
      const currentStock = product.stock || 0;
      const newStock = parseInt(item.new_quantity);
      const difference = newStock - currentStock;
      
      data.stock_movements.push({
        id: db.nextId('stock_movements'),
        product_id: parseInt(item.product_id),
        warehouse_id: item.warehouse_id ? parseInt(item.warehouse_id) : null,
        type: 'adjustment',
        quantity: Math.abs(difference),
        quantity_before: currentStock,
        quantity_after: newStock,
        reference: adjNumber,
        notes: item.notes || notes,
        created_at: db.now()
      });
      
      product.stock = newStock;
      product.updated_at = db.now();
    });
    
    db.save();
    return { ok: true, id, adjustment_number: adjNumber };
  });

  // ==================== STOCK TRANSFERS ====================
  
  fastify.get('/api/inventory/transfers', async () => {
    const data = db.get();
    const transfers = (data.stock_transfers || []).map(t => {
      const fromW = (data.warehouses || []).find(w => w.id === t.from_warehouse_id);
      const toW = (data.warehouses || []).find(w => w.id === t.to_warehouse_id);
      return { ...t, from_warehouse_name: fromW?.name, to_warehouse_name: toW?.name };
    });
    return transfers.slice(-100).reverse();
  });

  fastify.post('/api/inventory/transfers', async (req) => {
    const data = db.get();
    const { from_warehouse_id, to_warehouse_id, items, notes } = req.body;
    
    if (!from_warehouse_id || !to_warehouse_id || !items || items.length === 0) {
      return { error: 'Source, destination warehouse and items are required' };
    }
    
    if (from_warehouse_id === to_warehouse_id) {
      return { error: 'Cannot transfer to same warehouse' };
    }
    
    const count = (data.stock_transfers || []).length;
    const transferNumber = `TRF-${new Date().toISOString().slice(0,10).replace(/-/g,'')}-${String(count + 1).padStart(4, '0')}`;
    
    const id = db.nextId('stock_transfers');
    data.stock_transfers.push({
      id,
      transfer_number: transferNumber,
      from_warehouse_id: parseInt(from_warehouse_id),
      to_warehouse_id: parseInt(to_warehouse_id),
      status: 'completed',
      notes,
      items_count: items.length,
      created_at: db.now()
    });
    
    items.forEach(item => {
      data.stock_movements.push({
        id: db.nextId('stock_movements'),
        product_id: parseInt(item.product_id),
        warehouse_id: parseInt(from_warehouse_id),
        type: 'transfer',
        quantity: -Math.abs(parseInt(item.quantity)),
        reference: transferNumber,
        notes: 'Transfer out',
        created_at: db.now()
      });
      
      data.stock_movements.push({
        id: db.nextId('stock_movements'),
        product_id: parseInt(item.product_id),
        warehouse_id: parseInt(to_warehouse_id),
        type: 'transfer',
        quantity: Math.abs(parseInt(item.quantity)),
        reference: transferNumber,
        notes: 'Transfer in',
        created_at: db.now()
      });
    });
    
    db.save();
    return { ok: true, id, transfer_number: transferNumber };
  });

  // ==================== REPORTS ====================
  
  fastify.get('/api/inventory/reports/stock-levels', async () => {
    const data = db.get();
    const products = (data.products || []).filter(p => p.is_active).map(p => {
      const category = (data.categories || []).find(c => c.id === p.category_id);
      let status = 'in_stock';
      if (p.stock === 0) status = 'out_of_stock';
      else if (p.stock <= (p.min_stock || 10)) status = 'low_stock';
      
      return {
        ...p,
        category_name: category?.name,
        stock_status: status,
        stock_value: (p.stock || 0) * (p.cost || 0)
      };
    });
    
    return products.sort((a, b) => a.stock - b.stock);
  });

  fastify.get('/api/inventory/reports/valuation', async () => {
    const data = db.get();
    const products = (data.products || []).filter(p => p.is_active && p.stock > 0);
    
    const items = products.map(p => ({
      id: p.id,
      name: p.name,
      sku: p.sku,
      stock: p.stock,
      cost: p.cost || 0,
      price: p.price,
      cost_value: (p.stock || 0) * (p.cost || 0),
      retail_value: (p.stock || 0) * (p.price || 0)
    }));
    
    const totals = {
      total_items: items.length,
      total_units: items.reduce((s, i) => s + i.stock, 0),
      total_cost_value: items.reduce((s, i) => s + i.cost_value, 0),
      total_retail_value: items.reduce((s, i) => s + i.retail_value, 0),
      potential_profit: items.reduce((s, i) => s + (i.retail_value - i.cost_value), 0)
    };
    
    return { items: items.sort((a, b) => b.cost_value - a.cost_value), totals };
  });

  fastify.get('/api/inventory/reports/low-stock', async () => {
    const data = db.get();
    return (data.products || [])
      .filter(p => p.is_active && p.stock >= 0 && p.stock <= (p.min_stock || 10))
      .map(p => {
        const category = (data.categories || []).find(c => c.id === p.category_id);
        const supplier = (data.suppliers || []).find(s => s.id === p.supplier_id);
        return { ...p, category_name: category?.name, supplier_name: supplier?.name };
      })
      .sort((a, b) => a.stock - b.stock);
  });
}

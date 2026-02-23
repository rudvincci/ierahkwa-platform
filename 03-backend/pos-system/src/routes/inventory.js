const express = require('express');
const { v4: uuidv4 } = require('uuid');
const router = express.Router();

const getDb = (req) => req.app.locals.db || require('../db').getDb();

// ==================== WAREHOUSES ====================

router.get('/warehouses', (req, res) => {
  try {
    const db = getDb(req);
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    
    res.json({
      warehouses: warehouses.map(w => ({
        id: w.id,
        uuid: w.uuid,
        name: w.name,
        code: w.code,
        address: w.address,
        city: w.city,
        country: w.country,
        phone: w.phone,
        email: w.email,
        manager: w.manager,
        type: w.type,
        status: w.status,
        capacity: w.capacity,
        createdAt: w.created_at
      }))
    });
  } catch (err) {
    console.error('Get warehouses error:', err);
    res.status(500).json({ error: 'Error fetching warehouses' });
  }
});

router.post('/warehouses', (req, res) => {
  try {
    const db = getDb(req);
    const { name, code, address, city, country, phone, email, manager, type, capacity } = req.body;
    
    if (!name || !code) {
      return res.status(400).json({ error: 'Name and code required' });
    }
    
    const uuid = uuidv4();
    const result = db.prepare(`
      INSERT INTO warehouses (uuid, name, code, address, city, country, phone, email, manager, type, status, capacity, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, name, code, address || null, city || null, country || null, phone || null, email || null, manager || null, type || 'warehouse', 'active', capacity || 0, new Date().toISOString());
    
    res.status(201).json({ success: true, warehouse: { id: result.lastInsertRowid, uuid, name, code } });
  } catch (err) {
    console.error('Create warehouse error:', err);
    res.status(500).json({ error: 'Error creating warehouse' });
  }
});

router.put('/warehouses/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    const warehouse = warehouses.find(w => w.id === parseInt(id));
    
    if (!warehouse) {
      return res.status(404).json({ error: 'Warehouse not found' });
    }
    
    const { name, code, address, city, country, phone, email, manager, type, status, capacity } = req.body;
    
    db.prepare(`UPDATE warehouses SET name = ?, code = ?, address = ?, city = ?, country = ?, phone = ?, email = ?, manager = ?, type = ?, status = ?, capacity = ?, updated_at = ? WHERE id = ?`)
      .run(name || warehouse.name, code || warehouse.code, address !== undefined ? address : warehouse.address, city !== undefined ? city : warehouse.city, country !== undefined ? country : warehouse.country, phone !== undefined ? phone : warehouse.phone, email !== undefined ? email : warehouse.email, manager !== undefined ? manager : warehouse.manager, type || warehouse.type, status || warehouse.status, capacity !== undefined ? capacity : warehouse.capacity, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Warehouse updated' });
  } catch (err) {
    console.error('Update warehouse error:', err);
    res.status(500).json({ error: 'Error updating warehouse' });
  }
});

// ==================== SUPPLIERS ====================

router.get('/suppliers', (req, res) => {
  try {
    const db = getDb(req);
    const suppliers = db.prepare('SELECT * FROM suppliers').all();
    
    res.json({
      suppliers: suppliers.map(s => ({
        id: s.id,
        uuid: s.uuid,
        name: s.name,
        code: s.code,
        contactName: s.contact_name,
        email: s.email,
        phone: s.phone,
        address: s.address,
        city: s.city,
        country: s.country,
        taxId: s.tax_id,
        paymentTerms: s.payment_terms,
        status: s.status,
        rating: s.rating,
        notes: s.notes,
        createdAt: s.created_at
      }))
    });
  } catch (err) {
    console.error('Get suppliers error:', err);
    res.status(500).json({ error: 'Error fetching suppliers' });
  }
});

router.post('/suppliers', (req, res) => {
  try {
    const db = getDb(req);
    const { name, code, contactName, email, phone, address, city, country, taxId, paymentTerms, notes } = req.body;
    
    if (!name) {
      return res.status(400).json({ error: 'Name required' });
    }
    
    const uuid = uuidv4();
    const supplierCode = code || `SUP-${Date.now().toString().slice(-6)}`;
    
    const result = db.prepare(`
      INSERT INTO suppliers (uuid, name, code, contact_name, email, phone, address, city, country, tax_id, payment_terms, status, notes, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, name, supplierCode, contactName || null, email || null, phone || null, address || null, city || null, country || null, taxId || null, paymentTerms || 'net30', 'active', notes || null, new Date().toISOString());
    
    res.status(201).json({ success: true, supplier: { id: result.lastInsertRowid, uuid, name, code: supplierCode } });
  } catch (err) {
    console.error('Create supplier error:', err);
    res.status(500).json({ error: 'Error creating supplier' });
  }
});

router.put('/suppliers/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const suppliers = db.prepare('SELECT * FROM suppliers').all();
    const supplier = suppliers.find(s => s.id === parseInt(id));
    
    if (!supplier) {
      return res.status(404).json({ error: 'Supplier not found' });
    }
    
    const { name, contactName, email, phone, address, city, country, taxId, paymentTerms, status, notes } = req.body;
    
    db.prepare(`UPDATE suppliers SET name = ?, contact_name = ?, email = ?, phone = ?, address = ?, city = ?, country = ?, tax_id = ?, payment_terms = ?, status = ?, notes = ?, updated_at = ? WHERE id = ?`)
      .run(name || supplier.name, contactName !== undefined ? contactName : supplier.contact_name, email !== undefined ? email : supplier.email, phone !== undefined ? phone : supplier.phone, address !== undefined ? address : supplier.address, city !== undefined ? city : supplier.city, country !== undefined ? country : supplier.country, taxId !== undefined ? taxId : supplier.tax_id, paymentTerms || supplier.payment_terms, status || supplier.status, notes !== undefined ? notes : supplier.notes, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Supplier updated' });
  } catch (err) {
    console.error('Update supplier error:', err);
    res.status(500).json({ error: 'Error updating supplier' });
  }
});

// ==================== WAREHOUSE STOCK ====================

router.get('/stock', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId, itemId, lowStock } = req.query;
    
    let stock = db.prepare('SELECT * FROM warehouse_stock').all();
    
    if (warehouseId) stock = stock.filter(s => s.warehouse_id === parseInt(warehouseId));
    if (itemId) stock = stock.filter(s => s.item_id === parseInt(itemId));
    if (lowStock === 'true') stock = stock.filter(s => s.quantity <= (s.min_quantity || 0));
    
    // Join with items and warehouses
    const items = db.prepare('SELECT * FROM items').all();
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    
    stock = stock.map(s => {
      const item = items.find(i => i.id === s.item_id);
      const warehouse = warehouses.find(w => w.id === s.warehouse_id);
      return {
        id: s.id,
        warehouseId: s.warehouse_id,
        warehouseName: warehouse?.name,
        warehouseCode: warehouse?.code,
        itemId: s.item_id,
        itemName: item?.name,
        itemSku: item?.barcode,
        quantity: s.quantity,
        reservedQuantity: s.reserved_quantity || 0,
        availableQuantity: s.quantity - (s.reserved_quantity || 0),
        minQuantity: s.min_quantity,
        maxQuantity: s.max_quantity,
        reorderPoint: s.reorder_point,
        unitCost: s.unit_cost,
        totalValue: s.quantity * (s.unit_cost || 0),
        location: s.location,
        lastCountedAt: s.last_counted_at,
        updatedAt: s.updated_at
      };
    });
    
    res.json({ stock });
  } catch (err) {
    console.error('Get stock error:', err);
    res.status(500).json({ error: 'Error fetching stock' });
  }
});

router.post('/stock', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId, itemId, quantity, minQuantity, maxQuantity, reorderPoint, unitCost, location } = req.body;
    
    if (!warehouseId || !itemId) {
      return res.status(400).json({ error: 'Warehouse and item required' });
    }
    
    // Check if stock record exists
    const existing = db.prepare('SELECT * FROM warehouse_stock').all()
      .find(s => s.warehouse_id === warehouseId && s.item_id === itemId);
    
    if (existing) {
      db.prepare(`UPDATE warehouse_stock SET quantity = ?, min_quantity = ?, max_quantity = ?, reorder_point = ?, unit_cost = ?, location = ?, updated_at = ? WHERE id = ?`)
        .run(quantity || existing.quantity, minQuantity !== undefined ? minQuantity : existing.min_quantity, maxQuantity !== undefined ? maxQuantity : existing.max_quantity, reorderPoint !== undefined ? reorderPoint : existing.reorder_point, unitCost !== undefined ? unitCost : existing.unit_cost, location !== undefined ? location : existing.location, new Date().toISOString(), existing.id);
      
      res.json({ success: true, message: 'Stock updated', stockId: existing.id });
    } else {
      const result = db.prepare(`
        INSERT INTO warehouse_stock (warehouse_id, item_id, quantity, reserved_quantity, min_quantity, max_quantity, reorder_point, unit_cost, location, created_at, updated_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
      `).run(warehouseId, itemId, quantity || 0, 0, minQuantity || 0, maxQuantity || 0, reorderPoint || 0, unitCost || 0, location || null, new Date().toISOString(), new Date().toISOString());
      
      res.status(201).json({ success: true, stockId: result.lastInsertRowid });
    }
  } catch (err) {
    console.error('Create/Update stock error:', err);
    res.status(500).json({ error: 'Error managing stock' });
  }
});

// ==================== STOCK MOVEMENTS ====================

router.get('/movements', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId, itemId, type, limit = 50 } = req.query;
    
    let movements = db.prepare('SELECT * FROM stock_movements').all();
    
    if (warehouseId) movements = movements.filter(m => m.warehouse_id === parseInt(warehouseId));
    if (itemId) movements = movements.filter(m => m.item_id === parseInt(itemId));
    if (type) movements = movements.filter(m => m.type === type);
    
    // Sort by date desc and limit
    movements = movements.sort((a, b) => new Date(b.created_at) - new Date(a.created_at)).slice(0, parseInt(limit));
    
    // Join with items and warehouses
    const items = db.prepare('SELECT * FROM items').all();
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    const users = db.prepare('SELECT * FROM users').all();
    
    movements = movements.map(m => {
      const item = items.find(i => i.id === m.item_id);
      const warehouse = warehouses.find(w => w.id === m.warehouse_id);
      const user = users.find(u => u.id === m.user_id);
      return {
        id: m.id,
        uuid: m.uuid,
        warehouseId: m.warehouse_id,
        warehouseName: warehouse?.name,
        itemId: m.item_id,
        itemName: item?.name,
        type: m.type,
        quantity: m.quantity,
        quantityBefore: m.quantity_before,
        quantityAfter: m.quantity_after,
        unitCost: m.unit_cost,
        totalCost: m.total_cost,
        reference: m.reference,
        referenceType: m.reference_type,
        referenceId: m.reference_id,
        notes: m.notes,
        userId: m.user_id,
        userName: user?.full_name,
        createdAt: m.created_at
      };
    });
    
    res.json({ movements });
  } catch (err) {
    console.error('Get movements error:', err);
    res.status(500).json({ error: 'Error fetching movements' });
  }
});

router.post('/movements', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId, itemId, type, quantity, unitCost, reference, referenceType, referenceId, notes } = req.body;
    
    if (!warehouseId || !itemId || !type || !quantity) {
      return res.status(400).json({ error: 'Warehouse, item, type and quantity required' });
    }
    
    const validTypes = ['in', 'out', 'adjustment', 'transfer_in', 'transfer_out', 'return', 'damage', 'expired'];
    if (!validTypes.includes(type)) {
      return res.status(400).json({ error: 'Invalid movement type' });
    }
    
    // Get current stock
    let stock = db.prepare('SELECT * FROM warehouse_stock').all()
      .find(s => s.warehouse_id === warehouseId && s.item_id === itemId);
    
    const quantityBefore = stock?.quantity || 0;
    let quantityAfter = quantityBefore;
    
    if (['in', 'transfer_in', 'return'].includes(type)) {
      quantityAfter = quantityBefore + quantity;
    } else if (['out', 'transfer_out', 'damage', 'expired'].includes(type)) {
      quantityAfter = quantityBefore - quantity;
      if (quantityAfter < 0) {
        return res.status(400).json({ error: 'Insufficient stock' });
      }
    } else if (type === 'adjustment') {
      quantityAfter = quantity; // Adjustment sets absolute value
    }
    
    // Update or create stock
    if (stock) {
      db.prepare(`UPDATE warehouse_stock SET quantity = ?, unit_cost = ?, updated_at = ? WHERE id = ?`)
        .run(quantityAfter, unitCost || stock.unit_cost, new Date().toISOString(), stock.id);
    } else {
      db.prepare(`
        INSERT INTO warehouse_stock (warehouse_id, item_id, quantity, reserved_quantity, unit_cost, created_at, updated_at)
        VALUES (?, ?, ?, ?, ?, ?, ?)
      `).run(warehouseId, itemId, quantityAfter, 0, unitCost || 0, new Date().toISOString(), new Date().toISOString());
    }
    
    // Create movement record
    const uuid = uuidv4();
    const totalCost = quantity * (unitCost || 0);
    
    const result = db.prepare(`
      INSERT INTO stock_movements (uuid, warehouse_id, item_id, type, quantity, quantity_before, quantity_after, unit_cost, total_cost, reference, reference_type, reference_id, notes, user_id, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, warehouseId, itemId, type, quantity, quantityBefore, quantityAfter, unitCost || 0, totalCost, reference || null, referenceType || null, referenceId || null, notes || null, req.session.user?.id || 1, new Date().toISOString());
    
    res.status(201).json({ 
      success: true, 
      movement: { id: result.lastInsertRowid, uuid, quantityBefore, quantityAfter }
    });
  } catch (err) {
    console.error('Create movement error:', err);
    res.status(500).json({ error: 'Error creating movement' });
  }
});

// ==================== PURCHASE ORDERS ====================

router.get('/purchase-orders', (req, res) => {
  try {
    const db = getDb(req);
    const { status, supplierId } = req.query;
    
    let orders = db.prepare('SELECT * FROM purchase_orders').all();
    
    if (status) orders = orders.filter(o => o.status === status);
    if (supplierId) orders = orders.filter(o => o.supplier_id === parseInt(supplierId));
    
    const suppliers = db.prepare('SELECT * FROM suppliers').all();
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    
    orders = orders.map(o => {
      const supplier = suppliers.find(s => s.id === o.supplier_id);
      const warehouse = warehouses.find(w => w.id === o.warehouse_id);
      return {
        id: o.id,
        uuid: o.uuid,
        orderNumber: o.order_number,
        supplierId: o.supplier_id,
        supplierName: supplier?.name,
        warehouseId: o.warehouse_id,
        warehouseName: warehouse?.name,
        subtotal: o.subtotal,
        taxAmount: o.tax_amount,
        shippingCost: o.shipping_cost,
        total: o.total,
        status: o.status,
        expectedDate: o.expected_date,
        receivedDate: o.received_date,
        notes: o.notes,
        createdAt: o.created_at
      };
    });
    
    res.json({ orders });
  } catch (err) {
    console.error('Get purchase orders error:', err);
    res.status(500).json({ error: 'Error fetching purchase orders' });
  }
});

router.get('/purchase-orders/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    
    const orders = db.prepare('SELECT * FROM purchase_orders').all();
    const order = orders.find(o => o.id === parseInt(id));
    
    if (!order) {
      return res.status(404).json({ error: 'Purchase order not found' });
    }
    
    const orderItems = db.prepare('SELECT * FROM purchase_order_items').all()
      .filter(i => i.purchase_order_id === order.id);
    
    const items = db.prepare('SELECT * FROM items').all();
    const suppliers = db.prepare('SELECT * FROM suppliers').all();
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    
    const supplier = suppliers.find(s => s.id === order.supplier_id);
    const warehouse = warehouses.find(w => w.id === order.warehouse_id);
    
    res.json({
      order: {
        id: order.id,
        uuid: order.uuid,
        orderNumber: order.order_number,
        supplierId: order.supplier_id,
        supplierName: supplier?.name,
        warehouseId: order.warehouse_id,
        warehouseName: warehouse?.name,
        subtotal: order.subtotal,
        taxAmount: order.tax_amount,
        shippingCost: order.shipping_cost,
        total: order.total,
        status: order.status,
        expectedDate: order.expected_date,
        receivedDate: order.received_date,
        notes: order.notes,
        createdAt: order.created_at,
        items: orderItems.map(oi => {
          const item = items.find(i => i.id === oi.item_id);
          return {
            id: oi.id,
            itemId: oi.item_id,
            itemName: item?.name,
            quantity: oi.quantity,
            receivedQuantity: oi.received_quantity || 0,
            unitCost: oi.unit_cost,
            total: oi.total
          };
        })
      }
    });
  } catch (err) {
    console.error('Get purchase order error:', err);
    res.status(500).json({ error: 'Error fetching purchase order' });
  }
});

router.post('/purchase-orders', (req, res) => {
  try {
    const db = getDb(req);
    const { supplierId, warehouseId, items, taxRate, shippingCost, expectedDate, notes } = req.body;
    
    if (!supplierId || !warehouseId || !items || items.length === 0) {
      return res.status(400).json({ error: 'Supplier, warehouse and items required' });
    }
    
    let subtotal = 0;
    items.forEach(item => {
      subtotal += item.quantity * item.unitCost;
    });
    
    const taxAmount = subtotal * ((taxRate || 0) / 100);
    const total = subtotal + taxAmount + (shippingCost || 0);
    
    const uuid = uuidv4();
    const orderNumber = `PO-${Date.now().toString().slice(-8)}`;
    
    const result = db.prepare(`
      INSERT INTO purchase_orders (uuid, order_number, supplier_id, warehouse_id, subtotal, tax_amount, shipping_cost, total, status, expected_date, notes, created_by, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, orderNumber, supplierId, warehouseId, subtotal, taxAmount, shippingCost || 0, total, 'draft', expectedDate || null, notes || null, req.session.user?.id || 1, new Date().toISOString());
    
    const orderId = result.lastInsertRowid;
    
    items.forEach(item => {
      const itemTotal = item.quantity * item.unitCost;
      db.prepare(`
        INSERT INTO purchase_order_items (purchase_order_id, item_id, quantity, received_quantity, unit_cost, total, created_at)
        VALUES (?, ?, ?, ?, ?, ?, ?)
      `).run(orderId, item.itemId, item.quantity, 0, item.unitCost, itemTotal, new Date().toISOString());
    });
    
    res.status(201).json({ success: true, order: { id: orderId, uuid, orderNumber, total } });
  } catch (err) {
    console.error('Create purchase order error:', err);
    res.status(500).json({ error: 'Error creating purchase order' });
  }
});

router.put('/purchase-orders/:id/status', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const { status } = req.body;
    
    const validStatuses = ['draft', 'sent', 'confirmed', 'partial', 'received', 'cancelled'];
    if (!validStatuses.includes(status)) {
      return res.status(400).json({ error: 'Invalid status' });
    }
    
    const orders = db.prepare('SELECT * FROM purchase_orders').all();
    const order = orders.find(o => o.id === parseInt(id));
    
    if (!order) {
      return res.status(404).json({ error: 'Purchase order not found' });
    }
    
    let receivedDate = order.received_date;
    if (status === 'received' && !order.received_date) {
      receivedDate = new Date().toISOString().split('T')[0];
    }
    
    db.prepare(`UPDATE purchase_orders SET status = ?, received_date = ?, updated_at = ? WHERE id = ?`)
      .run(status, receivedDate, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Status updated' });
  } catch (err) {
    console.error('Update PO status error:', err);
    res.status(500).json({ error: 'Error updating status' });
  }
});

router.post('/purchase-orders/:id/receive', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const { items } = req.body; // Array of { itemId, receivedQuantity }
    
    const orders = db.prepare('SELECT * FROM purchase_orders').all();
    const order = orders.find(o => o.id === parseInt(id));
    
    if (!order) {
      return res.status(404).json({ error: 'Purchase order not found' });
    }
    
    const orderItems = db.prepare('SELECT * FROM purchase_order_items').all()
      .filter(i => i.purchase_order_id === order.id);
    
    let allReceived = true;
    let anyReceived = false;
    
    items.forEach(({ itemId, receivedQuantity }) => {
      const orderItem = orderItems.find(oi => oi.item_id === itemId);
      if (orderItem) {
        const newReceivedQty = (orderItem.received_quantity || 0) + receivedQuantity;
        
        db.prepare(`UPDATE purchase_order_items SET received_quantity = ? WHERE id = ?`)
          .run(newReceivedQty, orderItem.id);
        
        // Create stock movement
        const uuid = uuidv4();
        
        // Get current stock
        const stock = db.prepare('SELECT * FROM warehouse_stock').all()
          .find(s => s.warehouse_id === order.warehouse_id && s.item_id === itemId);
        
        const quantityBefore = stock?.quantity || 0;
        const quantityAfter = quantityBefore + receivedQuantity;
        
        // Update stock
        if (stock) {
          db.prepare(`UPDATE warehouse_stock SET quantity = ?, unit_cost = ?, updated_at = ? WHERE id = ?`)
            .run(quantityAfter, orderItem.unit_cost, new Date().toISOString(), stock.id);
        } else {
          db.prepare(`
            INSERT INTO warehouse_stock (warehouse_id, item_id, quantity, reserved_quantity, unit_cost, created_at, updated_at)
            VALUES (?, ?, ?, ?, ?, ?, ?)
          `).run(order.warehouse_id, itemId, quantityAfter, 0, orderItem.unit_cost, new Date().toISOString(), new Date().toISOString());
        }
        
        // Create movement
        db.prepare(`
          INSERT INTO stock_movements (uuid, warehouse_id, item_id, type, quantity, quantity_before, quantity_after, unit_cost, total_cost, reference, reference_type, reference_id, user_id, created_at)
          VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `).run(uuid, order.warehouse_id, itemId, 'in', receivedQuantity, quantityBefore, quantityAfter, orderItem.unit_cost, receivedQuantity * orderItem.unit_cost, order.order_number, 'purchase_order', order.id, req.session.user?.id || 1, new Date().toISOString());
        
        if (newReceivedQty < orderItem.quantity) allReceived = false;
        if (receivedQuantity > 0) anyReceived = true;
      }
    });
    
    // Update PO status
    let newStatus = order.status;
    if (allReceived) {
      newStatus = 'received';
    } else if (anyReceived) {
      newStatus = 'partial';
    }
    
    const receivedDate = newStatus === 'received' ? new Date().toISOString().split('T')[0] : order.received_date;
    
    db.prepare(`UPDATE purchase_orders SET status = ?, received_date = ?, updated_at = ? WHERE id = ?`)
      .run(newStatus, receivedDate, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Items received', status: newStatus });
  } catch (err) {
    console.error('Receive PO error:', err);
    res.status(500).json({ error: 'Error receiving items' });
  }
});

// ==================== STOCK ADJUSTMENTS ====================

router.get('/adjustments', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId, status } = req.query;
    
    let adjustments = db.prepare('SELECT * FROM stock_adjustments').all();
    
    if (warehouseId) adjustments = adjustments.filter(a => a.warehouse_id === parseInt(warehouseId));
    if (status) adjustments = adjustments.filter(a => a.status === status);
    
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    const users = db.prepare('SELECT * FROM users').all();
    
    adjustments = adjustments.map(a => {
      const warehouse = warehouses.find(w => w.id === a.warehouse_id);
      const user = users.find(u => u.id === a.created_by);
      return {
        id: a.id,
        uuid: a.uuid,
        adjustmentNumber: a.adjustment_number,
        warehouseId: a.warehouse_id,
        warehouseName: warehouse?.name,
        reason: a.reason,
        status: a.status,
        totalItems: a.total_items,
        totalValue: a.total_value,
        notes: a.notes,
        createdBy: a.created_by,
        createdByName: user?.full_name,
        approvedBy: a.approved_by,
        createdAt: a.created_at,
        approvedAt: a.approved_at
      };
    });
    
    res.json({ adjustments });
  } catch (err) {
    console.error('Get adjustments error:', err);
    res.status(500).json({ error: 'Error fetching adjustments' });
  }
});

router.post('/adjustments', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId, reason, items, notes } = req.body;
    
    if (!warehouseId || !items || items.length === 0) {
      return res.status(400).json({ error: 'Warehouse and items required' });
    }
    
    const uuid = uuidv4();
    const adjustmentNumber = `ADJ-${Date.now().toString().slice(-8)}`;
    
    let totalItems = 0;
    let totalValue = 0;
    
    items.forEach(item => {
      totalItems++;
      totalValue += Math.abs(item.adjustmentQuantity) * (item.unitCost || 0);
    });
    
    const result = db.prepare(`
      INSERT INTO stock_adjustments (uuid, adjustment_number, warehouse_id, reason, status, total_items, total_value, notes, created_by, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, adjustmentNumber, warehouseId, reason || 'manual', 'draft', totalItems, totalValue, notes || null, req.session.user?.id || 1, new Date().toISOString());
    
    const adjustmentId = result.lastInsertRowid;
    
    items.forEach(item => {
      db.prepare(`
        INSERT INTO stock_adjustment_items (adjustment_id, item_id, quantity_before, quantity_after, adjustment_quantity, unit_cost, reason, created_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
      `).run(adjustmentId, item.itemId, item.quantityBefore || 0, item.quantityAfter, item.adjustmentQuantity, item.unitCost || 0, item.reason || null, new Date().toISOString());
    });
    
    res.status(201).json({ success: true, adjustment: { id: adjustmentId, uuid, adjustmentNumber } });
  } catch (err) {
    console.error('Create adjustment error:', err);
    res.status(500).json({ error: 'Error creating adjustment' });
  }
});

router.post('/adjustments/:id/approve', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    
    const adjustments = db.prepare('SELECT * FROM stock_adjustments').all();
    const adjustment = adjustments.find(a => a.id === parseInt(id));
    
    if (!adjustment) {
      return res.status(404).json({ error: 'Adjustment not found' });
    }
    
    if (adjustment.status !== 'draft') {
      return res.status(400).json({ error: 'Only draft adjustments can be approved' });
    }
    
    // Get adjustment items
    const adjustmentItems = db.prepare('SELECT * FROM stock_adjustment_items').all()
      .filter(i => i.adjustment_id === adjustment.id);
    
    // Apply adjustments to stock
    adjustmentItems.forEach(item => {
      const stock = db.prepare('SELECT * FROM warehouse_stock').all()
        .find(s => s.warehouse_id === adjustment.warehouse_id && s.item_id === item.item_id);
      
      const quantityBefore = stock?.quantity || 0;
      const quantityAfter = item.quantity_after;
      
      // Update stock
      if (stock) {
        db.prepare(`UPDATE warehouse_stock SET quantity = ?, updated_at = ? WHERE id = ?`)
          .run(quantityAfter, new Date().toISOString(), stock.id);
      } else {
        db.prepare(`
          INSERT INTO warehouse_stock (warehouse_id, item_id, quantity, reserved_quantity, unit_cost, created_at, updated_at)
          VALUES (?, ?, ?, ?, ?, ?, ?)
        `).run(adjustment.warehouse_id, item.item_id, quantityAfter, 0, item.unit_cost, new Date().toISOString(), new Date().toISOString());
      }
      
      // Create movement
      const uuid = uuidv4();
      db.prepare(`
        INSERT INTO stock_movements (uuid, warehouse_id, item_id, type, quantity, quantity_before, quantity_after, unit_cost, total_cost, reference, reference_type, reference_id, notes, user_id, created_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
      `).run(uuid, adjustment.warehouse_id, item.item_id, 'adjustment', Math.abs(item.adjustment_quantity), quantityBefore, quantityAfter, item.unit_cost, Math.abs(item.adjustment_quantity) * item.unit_cost, adjustment.adjustment_number, 'stock_adjustment', adjustment.id, item.reason, req.session.user?.id || 1, new Date().toISOString());
    });
    
    // Update adjustment status
    db.prepare(`UPDATE stock_adjustments SET status = ?, approved_by = ?, approved_at = ? WHERE id = ?`)
      .run('approved', req.session.user?.id || 1, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Adjustment approved and applied' });
  } catch (err) {
    console.error('Approve adjustment error:', err);
    res.status(500).json({ error: 'Error approving adjustment' });
  }
});

// ==================== DASHBOARD / REPORTS ====================

router.get('/dashboard', (req, res) => {
  try {
    const db = getDb(req);
    
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    const suppliers = db.prepare('SELECT * FROM suppliers').all();
    const stock = db.prepare('SELECT * FROM warehouse_stock').all();
    const movements = db.prepare('SELECT * FROM stock_movements').all();
    const purchaseOrders = db.prepare('SELECT * FROM purchase_orders').all();
    const items = db.prepare('SELECT * FROM items').all();
    
    // Calculate totals
    const totalStockValue = stock.reduce((sum, s) => sum + (s.quantity * (s.unit_cost || 0)), 0);
    const totalItems = stock.reduce((sum, s) => sum + s.quantity, 0);
    const lowStockItems = stock.filter(s => s.quantity <= (s.min_quantity || 0) && s.quantity > 0);
    const outOfStockItems = stock.filter(s => s.quantity === 0);
    
    // Recent movements
    const recentMovements = movements
      .sort((a, b) => new Date(b.created_at) - new Date(a.created_at))
      .slice(0, 10)
      .map(m => {
        const item = items.find(i => i.id === m.item_id);
        const warehouse = warehouses.find(w => w.id === m.warehouse_id);
        return {
          id: m.id,
          type: m.type,
          itemName: item?.name,
          warehouseName: warehouse?.name,
          quantity: m.quantity,
          createdAt: m.created_at
        };
      });
    
    // Pending POs
    const pendingPOs = purchaseOrders.filter(o => ['draft', 'sent', 'confirmed', 'partial'].includes(o.status));
    
    res.json({
      summary: {
        totalWarehouses: warehouses.filter(w => w.status === 'active').length,
        totalSuppliers: suppliers.filter(s => s.status === 'active').length,
        totalItems,
        totalStockValue,
        lowStockCount: lowStockItems.length,
        outOfStockCount: outOfStockItems.length,
        pendingPOsCount: pendingPOs.length,
        pendingPOsValue: pendingPOs.reduce((sum, o) => sum + (o.total || 0), 0)
      },
      lowStockItems: lowStockItems.slice(0, 10).map(s => {
        const item = items.find(i => i.id === s.item_id);
        const warehouse = warehouses.find(w => w.id === s.warehouse_id);
        return {
          itemName: item?.name,
          warehouseName: warehouse?.name,
          quantity: s.quantity,
          minQuantity: s.min_quantity
        };
      }),
      recentMovements,
      warehouseStock: warehouses.map(w => {
        const warehouseStock = stock.filter(s => s.warehouse_id === w.id);
        return {
          id: w.id,
          name: w.name,
          totalItems: warehouseStock.reduce((sum, s) => sum + s.quantity, 0),
          totalValue: warehouseStock.reduce((sum, s) => sum + (s.quantity * (s.unit_cost || 0)), 0)
        };
      })
    });
  } catch (err) {
    console.error('Dashboard error:', err);
    res.status(500).json({ error: 'Error fetching dashboard' });
  }
});

router.get('/reports/valuation', (req, res) => {
  try {
    const db = getDb(req);
    const { warehouseId } = req.query;
    
    let stock = db.prepare('SELECT * FROM warehouse_stock').all();
    if (warehouseId) stock = stock.filter(s => s.warehouse_id === parseInt(warehouseId));
    
    const items = db.prepare('SELECT * FROM items').all();
    const warehouses = db.prepare('SELECT * FROM warehouses').all();
    const categories = db.prepare('SELECT * FROM categories').all();
    
    const valuation = stock.map(s => {
      const item = items.find(i => i.id === s.item_id);
      const warehouse = warehouses.find(w => w.id === s.warehouse_id);
      const category = categories.find(c => c.id === item?.category_id);
      return {
        itemId: s.item_id,
        itemName: item?.name,
        categoryName: category?.name,
        warehouseId: s.warehouse_id,
        warehouseName: warehouse?.name,
        quantity: s.quantity,
        unitCost: s.unit_cost,
        totalValue: s.quantity * (s.unit_cost || 0)
      };
    });
    
    const totalValue = valuation.reduce((sum, v) => sum + v.totalValue, 0);
    
    // Group by category
    const byCategory = {};
    valuation.forEach(v => {
      const cat = v.categoryName || 'Uncategorized';
      if (!byCategory[cat]) byCategory[cat] = { quantity: 0, value: 0 };
      byCategory[cat].quantity += v.quantity;
      byCategory[cat].value += v.totalValue;
    });
    
    res.json({
      items: valuation,
      summary: {
        totalValue,
        totalItems: valuation.reduce((sum, v) => sum + v.quantity, 0),
        uniqueProducts: new Set(valuation.map(v => v.itemId)).size
      },
      byCategory: Object.entries(byCategory).map(([name, data]) => ({ name, ...data }))
    });
  } catch (err) {
    console.error('Valuation report error:', err);
    res.status(500).json({ error: 'Error fetching valuation report' });
  }
});

module.exports = router;

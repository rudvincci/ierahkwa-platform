'use strict';

// ============================================================================
// EMPRESA SOBERANA — Sovereign ERP API
// Enterprise Resource Planning: CRM, Inventory, Invoicing
// Ierahkwa Ne Kanienke / MameyNode Platform
// ============================================================================

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { v4: uuidv4 } = require('uuid');
const { corsConfig, applySecurityMiddleware, errorHandler } = require('../shared/security');
const db = require('./db');

const app = express();

// ============================================================================
// MIDDLEWARE
// ============================================================================

app.use(helmet());
app.use(compression());
app.use(cors(corsConfig()));
app.use(express.json({ limit: '5mb' }));

// Apply shared Ierahkwa security middleware
const logger = applySecurityMiddleware(app, 'empresa-soberana');

// ============================================================================
// ERP MODULES DEFINITION
// ============================================================================

const ERP_MODULES = [
  {
    id: 'crm',
    name: 'CRM',
    description: 'Gestion de relaciones con clientes, contactos y oportunidades',
    icon: 'users',
    status: 'active',
    endpoints: ['/api/contacts'],
    features: ['Contact Management', 'Lead Tracking', 'Pipeline', 'Activity Log']
  },
  {
    id: 'accounting',
    name: 'Accounting',
    description: 'Facturacion, cuentas por cobrar y pagar, reportes financieros',
    icon: 'calculator',
    status: 'active',
    endpoints: ['/api/invoices'],
    features: ['Invoicing', 'Accounts Receivable', 'Accounts Payable', 'Financial Reports']
  },
  {
    id: 'inventory',
    name: 'Inventory',
    description: 'Catalogo de productos, control de stock y gestion de almacenes',
    icon: 'package',
    status: 'active',
    endpoints: ['/api/products'],
    features: ['Product Catalog', 'Stock Control', 'Warehouse Management', 'Barcode Support']
  },
  {
    id: 'hr',
    name: 'Human Resources',
    description: 'Gestion de empleados, nomina, asistencia y evaluaciones',
    icon: 'briefcase',
    status: 'planned',
    endpoints: [],
    features: ['Employee Management', 'Payroll', 'Attendance', 'Performance Reviews']
  },
  {
    id: 'manufacturing',
    name: 'Manufacturing',
    description: 'Ordenes de produccion, BOM, control de calidad',
    icon: 'settings',
    status: 'planned',
    endpoints: [],
    features: ['Production Orders', 'Bill of Materials', 'Quality Control', 'Work Centers']
  }
];

// ============================================================================
// HELPER FUNCTIONS
// ============================================================================

/** Map camelCase sort field names to snake_case DB columns */
const SORT_COLUMN_MAP = {
  createdAt: 'created_at',
  updatedAt: 'updated_at',
  name: 'name',
  email: 'email',
  company: 'company',
  status: 'status',
  sku: 'sku',
  price: 'price',
  stock: 'stock',
  category: 'category',
  invoiceNumber: 'invoice_number',
  customer: 'customer',
  total: 'total'
};

function validateRequired(body, fields) {
  const missing = fields.filter(f => !body[f] && body[f] !== 0);
  if (missing.length > 0) {
    return `Campos requeridos: ${missing.join(', ')}`;
  }
  return null;
}

/** Resolve a sort column safely, defaulting to created_at */
function resolveSortColumn(field) {
  return SORT_COLUMN_MAP[field] || 'created_at';
}

/** Resolve sort order safely */
function resolveSortOrder(order) {
  return order === 'asc' ? 'ASC' : 'DESC';
}

// ============================================================================
// REST API — HEALTH
// ============================================================================

app.get('/health', async (req, res) => {
  try {
    const [contactsRes, productsRes, invoicesRes, revenueRes] = await Promise.all([
      db.query('SELECT COUNT(*)::int AS count FROM contacts'),
      db.query('SELECT COUNT(*)::int AS count FROM products'),
      db.query('SELECT COUNT(*)::int AS count FROM invoices'),
      db.query("SELECT COALESCE(SUM(total), 0) AS revenue FROM invoices WHERE status = 'paid'")
    ]);

    res.json({
      status: 'ok',
      service: 'empresa-soberana',
      version: '1.0.0',
      uptime: process.uptime(),
      timestamp: new Date().toISOString(),
      modules: ERP_MODULES.filter(m => m.status === 'active').length,
      contacts: contactsRes.rows[0].count,
      products: productsRes.rows[0].count,
      invoices: invoicesRes.rows[0].count,
      revenue: Number(revenueRes.rows[0].revenue),
      poweredBy: 'MameyNode'
    });
  } catch (error) {
    res.status(500).json({ status: 'error', service: 'empresa-soberana', error: 'Database health check failed' });
  }
});

// ============================================================================
// REST API — CONTACTS (CRM)
// ============================================================================

// Create contact
app.post('/api/contacts', async (req, res) => {
  try {
    const { name, email, phone, company, status, notes, tags } = req.body;
    const error = validateRequired(req.body, ['name', 'email']);
    if (error) {
      return res.status(400).json({ success: false, error });
    }

    // Check duplicate email
    const existing = await db.query('SELECT id FROM contacts WHERE email = $1', [email]);
    if (existing.rows.length > 0) {
      return res.status(409).json({ success: false, error: 'Ya existe un contacto con ese email' });
    }

    const id = uuidv4();
    const now = new Date().toISOString();

    const result = await db.query(
      `INSERT INTO contacts (id, name, email, phone, company, status, notes, tags, created_at, updated_at)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)
       RETURNING *`,
      [id, name, email, phone || null, company || null, status || 'lead', notes || '', JSON.stringify(tags || []), now, now]
    );

    const contact = db.mapContactRow(result.rows[0]);
    logger.dataAccess(req, 'contacts', 'create');

    res.status(201).json({ success: true, contact });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear contacto' });
  }
});

// List contacts
app.get('/api/contacts', async (req, res) => {
  try {
    const { status, company, search, limit, offset, sort, order } = req.query;

    const conditions = [];
    const params = [];
    let paramIndex = 1;

    if (status) {
      conditions.push(`status = $${paramIndex++}`);
      params.push(status);
    }
    if (company) {
      conditions.push(`company ILIKE $${paramIndex++}`);
      params.push(`%${company}%`);
    }
    if (search) {
      conditions.push(`(name ILIKE $${paramIndex} OR email ILIKE $${paramIndex} OR company ILIKE $${paramIndex})`);
      params.push(`%${search}%`);
      paramIndex++;
    }

    const whereClause = conditions.length > 0 ? 'WHERE ' + conditions.join(' AND ') : '';
    const sortCol = resolveSortColumn(sort || 'createdAt');
    const sortDir = resolveSortOrder(order || 'desc');
    const lim = Number(limit) || 50;
    const off = Number(offset) || 0;

    const countResult = await db.query(`SELECT COUNT(*)::int AS total FROM contacts ${whereClause}`, params);
    const total = countResult.rows[0].total;

    const dataResult = await db.query(
      `SELECT * FROM contacts ${whereClause} ORDER BY ${sortCol} ${sortDir} LIMIT $${paramIndex++} OFFSET $${paramIndex++}`,
      [...params, lim, off]
    );

    const contacts = dataResult.rows.map(db.mapContactRow);

    res.json({ success: true, contacts, total, limit: lim, offset: off });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar contactos' });
  }
});

// Get contact by ID
app.get('/api/contacts/:id', async (req, res) => {
  try {
    const result = await db.query('SELECT * FROM contacts WHERE id = $1', [req.params.id]);
    if (result.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Contacto no encontrado' });
    }
    const contact = db.mapContactRow(result.rows[0]);
    res.json({ success: true, contact });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener contacto' });
  }
});

// Update contact
app.put('/api/contacts/:id', async (req, res) => {
  try {
    const existing = await db.query('SELECT * FROM contacts WHERE id = $1', [req.params.id]);
    if (existing.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Contacto no encontrado' });
    }

    const row = existing.rows[0];
    const { name, email, phone, company, status, notes, tags } = req.body;
    const now = new Date().toISOString();

    const result = await db.query(
      `UPDATE contacts SET
        name = $1, email = $2, phone = $3, company = $4,
        status = $5, notes = $6, tags = $7, updated_at = $8
       WHERE id = $9
       RETURNING *`,
      [
        name !== undefined ? name : row.name,
        email !== undefined ? email : row.email,
        phone !== undefined ? phone : row.phone,
        company !== undefined ? company : row.company,
        status !== undefined ? status : row.status,
        notes !== undefined ? notes : row.notes,
        JSON.stringify(tags !== undefined ? tags : row.tags),
        now,
        req.params.id
      ]
    );

    const contact = db.mapContactRow(result.rows[0]);
    logger.dataAccess(req, 'contacts', 'update');

    res.json({ success: true, contact });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al actualizar contacto' });
  }
});

// Delete contact
app.delete('/api/contacts/:id', async (req, res) => {
  try {
    const result = await db.query('DELETE FROM contacts WHERE id = $1 RETURNING id', [req.params.id]);
    if (result.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Contacto no encontrado' });
    }

    logger.dataAccess(req, 'contacts', 'delete');

    res.json({ success: true, message: 'Contacto eliminado exitosamente' });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al eliminar contacto' });
  }
});

// ============================================================================
// REST API — PRODUCTS (Inventory)
// ============================================================================

// Create product
app.post('/api/products', async (req, res) => {
  try {
    const { name, sku, description, price, stock, category, unit, minStock } = req.body;
    const error = validateRequired(req.body, ['name', 'sku', 'price']);
    if (error) {
      return res.status(400).json({ success: false, error });
    }

    // Check duplicate SKU
    const existing = await db.query('SELECT id FROM products WHERE sku = $1', [sku]);
    if (existing.rows.length > 0) {
      return res.status(409).json({ success: false, error: 'Ya existe un producto con ese SKU' });
    }

    const id = uuidv4();
    const now = new Date().toISOString();

    const result = await db.query(
      `INSERT INTO products (id, name, sku, description, price, stock, category, unit, min_stock, is_active, created_at, updated_at)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12)
       RETURNING *`,
      [id, name, sku, description || '', Number(price), Number(stock) || 0, category || 'general', unit || 'unidad', Number(minStock) || 0, true, now, now]
    );

    const product = db.mapProductRow(result.rows[0]);
    logger.dataAccess(req, 'products', 'create');

    res.status(201).json({ success: true, product });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear producto' });
  }
});

// List products
app.get('/api/products', async (req, res) => {
  try {
    const { category, search, inStock, limit, offset, sort, order } = req.query;

    const conditions = [];
    const params = [];
    let paramIndex = 1;

    if (category) {
      conditions.push(`category = $${paramIndex++}`);
      params.push(category);
    }
    if (search) {
      conditions.push(`(name ILIKE $${paramIndex} OR sku ILIKE $${paramIndex} OR description ILIKE $${paramIndex})`);
      params.push(`%${search}%`);
      paramIndex++;
    }
    if (inStock === 'true') {
      conditions.push('stock > 0');
    }
    if (inStock === 'false') {
      conditions.push('stock = 0');
    }

    const whereClause = conditions.length > 0 ? 'WHERE ' + conditions.join(' AND ') : '';
    const sortCol = resolveSortColumn(sort || 'createdAt');
    const sortDir = resolveSortOrder(order || 'desc');
    const lim = Number(limit) || 50;
    const off = Number(offset) || 0;

    const [countResult, dataResult, lowStockResult] = await Promise.all([
      db.query(`SELECT COUNT(*)::int AS total FROM products ${whereClause}`, params),
      db.query(
        `SELECT * FROM products ${whereClause} ORDER BY ${sortCol} ${sortDir} LIMIT $${paramIndex++} OFFSET $${paramIndex++}`,
        [...params, lim, off]
      ),
      db.query('SELECT COUNT(*)::int AS count FROM products WHERE stock > 0 AND stock <= min_stock')
    ]);

    const total = countResult.rows[0].total;
    const products = dataResult.rows.map(db.mapProductRow);

    res.json({ success: true, products, total, limit: lim, offset: off, lowStockCount: lowStockResult.rows[0].count });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar productos' });
  }
});

// Get product by ID
app.get('/api/products/:id', async (req, res) => {
  try {
    const result = await db.query('SELECT * FROM products WHERE id = $1', [req.params.id]);
    if (result.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Producto no encontrado' });
    }
    const product = db.mapProductRow(result.rows[0]);
    res.json({ success: true, product });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener producto' });
  }
});

// Update product
app.put('/api/products/:id', async (req, res) => {
  try {
    const existing = await db.query('SELECT * FROM products WHERE id = $1', [req.params.id]);
    if (existing.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Producto no encontrado' });
    }

    const row = existing.rows[0];
    const { name, sku, description, price, stock, category, unit, minStock, isActive } = req.body;
    const now = new Date().toISOString();

    const result = await db.query(
      `UPDATE products SET
        name = $1, sku = $2, description = $3, price = $4,
        stock = $5, category = $6, unit = $7, min_stock = $8,
        is_active = $9, updated_at = $10
       WHERE id = $11
       RETURNING *`,
      [
        name !== undefined ? name : row.name,
        sku !== undefined ? sku : row.sku,
        description !== undefined ? description : row.description,
        price !== undefined ? Number(price) : Number(row.price),
        stock !== undefined ? Number(stock) : row.stock,
        category !== undefined ? category : row.category,
        unit !== undefined ? unit : row.unit,
        minStock !== undefined ? Number(minStock) : row.min_stock,
        isActive !== undefined ? isActive : row.is_active,
        now,
        req.params.id
      ]
    );

    const product = db.mapProductRow(result.rows[0]);
    logger.dataAccess(req, 'products', 'update');

    res.json({ success: true, product });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al actualizar producto' });
  }
});

// Delete product
app.delete('/api/products/:id', async (req, res) => {
  try {
    const result = await db.query('DELETE FROM products WHERE id = $1 RETURNING id', [req.params.id]);
    if (result.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Producto no encontrado' });
    }

    logger.dataAccess(req, 'products', 'delete');

    res.json({ success: true, message: 'Producto eliminado exitosamente' });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al eliminar producto' });
  }
});

// ============================================================================
// REST API — INVOICES (Accounting)
// ============================================================================

// Create invoice
app.post('/api/invoices', async (req, res) => {
  try {
    const { customer, customerEmail, items, notes, dueDate, currency } = req.body;
    const error = validateRequired(req.body, ['customer', 'items']);
    if (error) {
      return res.status(400).json({ success: false, error });
    }

    if (!Array.isArray(items) || items.length === 0) {
      return res.status(400).json({ success: false, error: 'La factura debe tener al menos un item' });
    }

    // Calculate totals
    const lineItems = items.map(item => {
      const quantity = Number(item.quantity) || 1;
      const unitPrice = Number(item.unitPrice) || 0;
      const subtotal = quantity * unitPrice;
      return {
        id: uuidv4(),
        description: item.description || '',
        productId: item.productId || null,
        quantity,
        unitPrice,
        subtotal
      };
    });

    const subtotal = lineItems.reduce((sum, item) => sum + item.subtotal, 0);
    const taxRate = 0; // 0% taxes - Sovereign Article VII
    const taxAmount = subtotal * taxRate;
    const total = subtotal + taxAmount;

    // Generate invoice number based on current count in DB
    const countResult = await db.query('SELECT COUNT(*)::int AS count FROM invoices');
    const invoiceCount = countResult.rows[0].count + 1;
    const invoiceNumber = `INV-${new Date().getFullYear()}-${String(invoiceCount).padStart(5, '0')}`;

    const id = uuidv4();
    const now = new Date().toISOString();
    const defaultDueDate = dueDate || new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];

    const result = await db.query(
      `INSERT INTO invoices (id, invoice_number, customer, customer_email, items, subtotal, tax_rate, tax_amount, total, currency, status, notes, due_date, issued_at, paid_at, created_at, updated_at)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16, $17)
       RETURNING *`,
      [
        id, invoiceNumber, customer, customerEmail || null,
        JSON.stringify(lineItems),
        Math.round(subtotal * 100) / 100, taxRate, Math.round(taxAmount * 100) / 100, Math.round(total * 100) / 100,
        currency || 'USD', 'draft', notes || '', defaultDueDate,
        now, null, now, now
      ]
    );

    const invoice = db.mapInvoiceRow(result.rows[0]);
    logger.dataAccess(req, 'invoices', 'create');

    res.status(201).json({
      success: true,
      invoice,
      message: `Factura ${invoiceNumber} creada exitosamente`
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear factura' });
  }
});

// List invoices
app.get('/api/invoices', async (req, res) => {
  try {
    const { status, customer, search, limit, offset, sort, order } = req.query;

    const conditions = [];
    const params = [];
    let paramIndex = 1;

    if (status) {
      conditions.push(`status = $${paramIndex++}`);
      params.push(status);
    }
    if (customer) {
      conditions.push(`customer ILIKE $${paramIndex++}`);
      params.push(`%${customer}%`);
    }
    if (search) {
      conditions.push(`(invoice_number ILIKE $${paramIndex} OR customer ILIKE $${paramIndex})`);
      params.push(`%${search}%`);
      paramIndex++;
    }

    const whereClause = conditions.length > 0 ? 'WHERE ' + conditions.join(' AND ') : '';
    const sortCol = resolveSortColumn(sort || 'createdAt');
    const sortDir = resolveSortOrder(order || 'desc');
    const lim = Number(limit) || 50;
    const off = Number(offset) || 0;

    const [countResult, dataResult] = await Promise.all([
      db.query(`SELECT COUNT(*)::int AS total FROM invoices ${whereClause}`, params),
      db.query(
        `SELECT * FROM invoices ${whereClause} ORDER BY ${sortCol} ${sortDir} LIMIT $${paramIndex++} OFFSET $${paramIndex++}`,
        [...params, lim, off]
      )
    ]);

    const total = countResult.rows[0].total;
    const invoices = dataResult.rows.map(db.mapInvoiceRow);

    res.json({ success: true, invoices, total, limit: lim, offset: off });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar facturas' });
  }
});

// Get invoice by ID
app.get('/api/invoices/:id', async (req, res) => {
  try {
    const result = await db.query('SELECT * FROM invoices WHERE id = $1', [req.params.id]);
    if (result.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Factura no encontrada' });
    }
    const invoice = db.mapInvoiceRow(result.rows[0]);
    res.json({ success: true, invoice });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener factura' });
  }
});

// Update invoice
app.put('/api/invoices/:id', async (req, res) => {
  try {
    const existing = await db.query('SELECT * FROM invoices WHERE id = $1', [req.params.id]);
    if (existing.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Factura no encontrada' });
    }

    const row = existing.rows[0];

    // Only allow editing draft invoices
    if (row.status !== 'draft' && req.body.items) {
      return res.status(400).json({
        success: false,
        error: 'Solo se pueden editar items en facturas con estado draft'
      });
    }

    const { customer, customerEmail, items, notes, dueDate, status } = req.body;

    let newCustomer = customer !== undefined ? customer : row.customer;
    let newCustomerEmail = customerEmail !== undefined ? customerEmail : row.customer_email;
    let newNotes = notes !== undefined ? notes : row.notes;
    let newDueDate = dueDate !== undefined ? dueDate : row.due_date;
    let newStatus = row.status;
    let newPaidAt = row.paid_at;
    let newItems = row.items;
    let newSubtotal = Number(row.subtotal);
    let newTaxRate = Number(row.tax_rate);
    let newTaxAmount = Number(row.tax_amount);
    let newTotal = Number(row.total);

    // Update status
    if (status !== undefined) {
      const validStatuses = ['draft', 'sent', 'paid', 'overdue', 'cancelled'];
      if (!validStatuses.includes(status)) {
        return res.status(400).json({
          success: false,
          error: `Estado invalido. Valores validos: ${validStatuses.join(', ')}`
        });
      }
      newStatus = status;
      if (status === 'paid') {
        newPaidAt = new Date().toISOString();
      }
    }

    // Recalculate if items changed
    if (items && Array.isArray(items) && items.length > 0) {
      newItems = items.map(item => {
        const quantity = Number(item.quantity) || 1;
        const unitPrice = Number(item.unitPrice) || 0;
        return {
          id: item.id || uuidv4(),
          description: item.description || '',
          productId: item.productId || null,
          quantity,
          unitPrice,
          subtotal: quantity * unitPrice
        };
      });

      const subtotal = newItems.reduce((sum, item) => sum + item.subtotal, 0);
      newSubtotal = Math.round(subtotal * 100) / 100;
      newTaxAmount = Math.round(subtotal * newTaxRate * 100) / 100;
      newTotal = Math.round((newSubtotal + newTaxAmount) * 100) / 100;
    }

    const now = new Date().toISOString();

    const result = await db.query(
      `UPDATE invoices SET
        customer = $1, customer_email = $2, items = $3, subtotal = $4,
        tax_rate = $5, tax_amount = $6, total = $7, status = $8,
        notes = $9, due_date = $10, paid_at = $11, updated_at = $12
       WHERE id = $13
       RETURNING *`,
      [
        newCustomer, newCustomerEmail, JSON.stringify(newItems), newSubtotal,
        newTaxRate, newTaxAmount, newTotal, newStatus,
        newNotes, newDueDate, newPaidAt, now,
        req.params.id
      ]
    );

    const invoice = db.mapInvoiceRow(result.rows[0]);
    logger.dataAccess(req, 'invoices', 'update');

    res.json({ success: true, invoice });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al actualizar factura' });
  }
});

// Delete invoice
app.delete('/api/invoices/:id', async (req, res) => {
  try {
    const existing = await db.query('SELECT id, status FROM invoices WHERE id = $1', [req.params.id]);
    if (existing.rows.length === 0) {
      return res.status(404).json({ success: false, error: 'Factura no encontrada' });
    }

    if (existing.rows[0].status === 'paid') {
      return res.status(400).json({
        success: false,
        error: 'No se puede eliminar una factura pagada'
      });
    }

    await db.query('DELETE FROM invoices WHERE id = $1', [req.params.id]);
    logger.dataAccess(req, 'invoices', 'delete');

    res.json({ success: true, message: 'Factura eliminada exitosamente' });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al eliminar factura' });
  }
});

// ============================================================================
// REST API — DASHBOARD
// ============================================================================

app.get('/api/dashboard', async (req, res) => {
  try {
    const [
      contactsCountRes,
      productsCountRes,
      invoicesCountRes,
      revenueRes,
      pendingRevenueRes,
      overdueRevenueRes,
      contactsByStatusRes,
      stockStatsRes,
      invoicesByStatusRes,
      recentInvoicesRes
    ] = await Promise.all([
      db.query('SELECT COUNT(*)::int AS count FROM contacts'),
      db.query('SELECT COUNT(*)::int AS count FROM products'),
      db.query('SELECT COUNT(*)::int AS count FROM invoices'),
      db.query("SELECT COALESCE(SUM(total), 0) AS revenue FROM invoices WHERE status = 'paid'"),
      db.query("SELECT COALESCE(SUM(total), 0) AS revenue FROM invoices WHERE status IN ('sent', 'draft')"),
      db.query("SELECT COALESCE(SUM(total), 0) AS revenue FROM invoices WHERE status = 'overdue'"),
      db.query('SELECT status, COUNT(*)::int AS count FROM contacts GROUP BY status'),
      db.query(`SELECT
        COUNT(*)::int AS total,
        COUNT(*) FILTER (WHERE stock > 0)::int AS in_stock,
        COUNT(*) FILTER (WHERE stock = 0)::int AS out_of_stock,
        COUNT(*) FILTER (WHERE stock > 0 AND stock <= min_stock)::int AS low_stock,
        COALESCE(SUM(price * stock), 0) AS total_stock_value
       FROM products`),
      db.query('SELECT status, COUNT(*)::int AS count FROM invoices GROUP BY status'),
      db.query('SELECT id, invoice_number, customer, total, status, created_at FROM invoices ORDER BY created_at DESC LIMIT 5')
    ]);

    const totalContacts = contactsCountRes.rows[0].count;
    const totalProducts = productsCountRes.rows[0].count;
    const totalInvoices = invoicesCountRes.rows[0].count;
    const totalRevenue = Number(revenueRes.rows[0].revenue);
    const pendingRevenue = Number(pendingRevenueRes.rows[0].revenue);
    const overdueRevenue = Number(overdueRevenueRes.rows[0].revenue);

    // Contact statistics
    const contactsByStatus = {};
    for (const row of contactsByStatusRes.rows) {
      contactsByStatus[row.status] = row.count;
    }

    // Product statistics
    const stockStats = stockStatsRes.rows[0];
    const totalStockValue = Number(stockStats.total_stock_value);

    // Invoice status breakdown
    const invoicesByStatus = {};
    for (const row of invoicesByStatusRes.rows) {
      invoicesByStatus[row.status] = row.count;
    }

    // Recent invoices
    const recentInvoices = recentInvoicesRes.rows.map(row => ({
      id: row.id,
      invoiceNumber: row.invoice_number,
      customer: row.customer,
      total: Number(row.total),
      status: row.status,
      createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at
    }));

    res.json({
      success: true,
      dashboard: {
        summary: {
          totalContacts,
          totalProducts,
          totalInvoices,
          totalRevenue: Math.round(totalRevenue * 100) / 100,
          pendingRevenue: Math.round(pendingRevenue * 100) / 100,
          overdueRevenue: Math.round(overdueRevenue * 100) / 100,
          totalStockValue: Math.round(totalStockValue * 100) / 100
        },
        contacts: {
          total: totalContacts,
          byStatus: contactsByStatus
        },
        products: {
          total: totalProducts,
          inStock: stockStats.in_stock,
          outOfStock: stockStats.out_of_stock,
          lowStock: stockStats.low_stock,
          totalStockValue: Math.round(totalStockValue * 100) / 100
        },
        invoices: {
          total: totalInvoices,
          byStatus: invoicesByStatus,
          totalRevenue: Math.round(totalRevenue * 100) / 100,
          recentInvoices
        },
        activeModules: ERP_MODULES.filter(m => m.status === 'active').length,
        taxRate: '0% --- Sovereign Article VII',
        lastUpdated: new Date().toISOString()
      }
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al generar dashboard' });
  }
});

// ============================================================================
// REST API — MODULES
// ============================================================================

app.get('/api/modules', (req, res) => {
  res.json({
    success: true,
    modules: ERP_MODULES,
    total: ERP_MODULES.length,
    active: ERP_MODULES.filter(m => m.status === 'active').length,
    planned: ERP_MODULES.filter(m => m.status === 'planned').length
  });
});

// ============================================================================
// ERROR HANDLING
// ============================================================================

// 404 handler
app.use((req, res) => {
  res.status(404).json({
    error: 'Endpoint no encontrado',
    path: req.path,
    method: req.method
  });
});

// Global error handler
app.use(errorHandler('empresa-soberana'));

// ============================================================================
// START SERVER
// ============================================================================

const PORT = process.env.PORT || 3092;
let server;

async function start() {
  await db.initialize();

  server = app.listen(PORT, () => {
    console.log('');
    console.log('  ============================================================');
    console.log('  ||                                                        ||');
    console.log('  ||     EMPRESA SOBERANA                                   ||');
    console.log('  ||     Sovereign ERP --- Enterprise Resource Planning     ||');
    console.log('  ||                                                        ||');
    console.log('  ||     Modules: CRM, Accounting, Inventory, HR, Mfg      ||');
    console.log('  ||     Tax Rate: 0% --- Sovereign Article VII             ||');
    console.log('  ||     Storage: PostgreSQL                                ||');
    console.log('  ||                                                        ||');
    console.log(`  ||     Port: ${PORT}                                         ||`);
    console.log('  ||     Status: OPERATIONAL                                ||');
    console.log('  ||                                                        ||');
    console.log('  ||     Powered by MameyNode                               ||');
    console.log('  ||     Ierahkwa Ne Kanienke Sovereign Platform            ||');
    console.log('  ||                                                        ||');
    console.log('  ============================================================');
    console.log('');
    console.log(`  [INFO] ERP API ready on http://localhost:${PORT}`);
    console.log(`  [INFO] Active modules: ${ERP_MODULES.filter(m => m.status === 'active').length}/${ERP_MODULES.length}`);
    console.log(`  [INFO] Endpoints: /api/contacts, /api/products, /api/invoices`);
    console.log(`  [INFO] Dashboard: /api/dashboard`);
    console.log(`  [INFO] Tax Rate: 0% (Constitutional Article VII)`);
    console.log(`  [INFO] Database: PostgreSQL connected`);
    console.log('');
  });
}

// Graceful shutdown
async function shutdown(signal) {
  console.log(`\n  [INFO] ${signal} received. Shutting down gracefully...`);
  if (server) {
    server.close(() => {
      console.log('  [INFO] HTTP server closed');
    });
  }
  await db.end();
  console.log('  [INFO] Database pool closed');
  process.exit(0);
}

process.on('SIGTERM', () => shutdown('SIGTERM'));
process.on('SIGINT', () => shutdown('SIGINT'));

start().catch((err) => {
  console.error('  [FATAL] Failed to start server:', err.message);
  process.exit(1);
});

module.exports = app;

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
// IN-MEMORY STORAGE
// ============================================================================

const contacts = new Map();
const products = new Map();
const invoices = new Map();

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

function paginateMap(map, query = {}) {
  const { limit = 50, offset = 0, sort = 'createdAt', order = 'desc' } = query;
  let items = Array.from(map.values());

  // Sort
  items.sort((a, b) => {
    const aVal = a[sort] || '';
    const bVal = b[sort] || '';
    if (order === 'desc') return bVal > aVal ? 1 : -1;
    return aVal > bVal ? 1 : -1;
  });

  const total = items.length;
  const paginated = items.slice(Number(offset), Number(offset) + Number(limit));

  return { items: paginated, total, limit: Number(limit), offset: Number(offset) };
}

function validateRequired(body, fields) {
  const missing = fields.filter(f => !body[f] && body[f] !== 0);
  if (missing.length > 0) {
    return `Campos requeridos: ${missing.join(', ')}`;
  }
  return null;
}

// ============================================================================
// REST API — HEALTH
// ============================================================================

app.get('/health', (req, res) => {
  const totalRevenue = Array.from(invoices.values())
    .filter(inv => inv.status === 'paid')
    .reduce((sum, inv) => sum + inv.total, 0);

  res.json({
    status: 'ok',
    service: 'empresa-soberana',
    version: '1.0.0',
    uptime: process.uptime(),
    timestamp: new Date().toISOString(),
    modules: ERP_MODULES.filter(m => m.status === 'active').length,
    contacts: contacts.size,
    products: products.size,
    invoices: invoices.size,
    revenue: totalRevenue,
    poweredBy: 'MameyNode'
  });
});

// ============================================================================
// REST API — CONTACTS (CRM)
// ============================================================================

// Create contact
app.post('/api/contacts', (req, res) => {
  try {
    const { name, email, phone, company, status, notes, tags } = req.body;
    const error = validateRequired(req.body, ['name', 'email']);
    if (error) {
      return res.status(400).json({ success: false, error });
    }

    // Check duplicate email
    const existing = Array.from(contacts.values()).find(c => c.email === email);
    if (existing) {
      return res.status(409).json({ success: false, error: 'Ya existe un contacto con ese email' });
    }

    const contact = {
      id: uuidv4(),
      name,
      email,
      phone: phone || null,
      company: company || null,
      status: status || 'lead',
      notes: notes || '',
      tags: tags || [],
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    contacts.set(contact.id, contact);
    logger.dataAccess(req, 'contacts', 'create');

    res.status(201).json({ success: true, contact });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear contacto' });
  }
});

// List contacts
app.get('/api/contacts', (req, res) => {
  try {
    const { status, company, search, limit, offset, sort, order } = req.query;
    let items = Array.from(contacts.values());

    if (status) {
      items = items.filter(c => c.status === status);
    }
    if (company) {
      items = items.filter(c => c.company && c.company.toLowerCase().includes(company.toLowerCase()));
    }
    if (search) {
      const term = search.toLowerCase();
      items = items.filter(c =>
        c.name.toLowerCase().includes(term) ||
        c.email.toLowerCase().includes(term) ||
        (c.company && c.company.toLowerCase().includes(term))
      );
    }

    // Sort
    const sortField = sort || 'createdAt';
    const sortOrder = order || 'desc';
    items.sort((a, b) => {
      const aVal = a[sortField] || '';
      const bVal = b[sortField] || '';
      if (sortOrder === 'desc') return bVal > aVal ? 1 : -1;
      return aVal > bVal ? 1 : -1;
    });

    const total = items.length;
    const lim = Number(limit) || 50;
    const off = Number(offset) || 0;
    const paginated = items.slice(off, off + lim);

    res.json({ success: true, contacts: paginated, total, limit: lim, offset: off });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar contactos' });
  }
});

// Get contact by ID
app.get('/api/contacts/:id', (req, res) => {
  try {
    const contact = contacts.get(req.params.id);
    if (!contact) {
      return res.status(404).json({ success: false, error: 'Contacto no encontrado' });
    }
    res.json({ success: true, contact });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener contacto' });
  }
});

// Update contact
app.put('/api/contacts/:id', (req, res) => {
  try {
    const contact = contacts.get(req.params.id);
    if (!contact) {
      return res.status(404).json({ success: false, error: 'Contacto no encontrado' });
    }

    const { name, email, phone, company, status, notes, tags } = req.body;

    if (name !== undefined) contact.name = name;
    if (email !== undefined) contact.email = email;
    if (phone !== undefined) contact.phone = phone;
    if (company !== undefined) contact.company = company;
    if (status !== undefined) contact.status = status;
    if (notes !== undefined) contact.notes = notes;
    if (tags !== undefined) contact.tags = tags;
    contact.updatedAt = new Date().toISOString();

    logger.dataAccess(req, 'contacts', 'update');

    res.json({ success: true, contact });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al actualizar contacto' });
  }
});

// Delete contact
app.delete('/api/contacts/:id', (req, res) => {
  try {
    if (!contacts.has(req.params.id)) {
      return res.status(404).json({ success: false, error: 'Contacto no encontrado' });
    }

    contacts.delete(req.params.id);
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
app.post('/api/products', (req, res) => {
  try {
    const { name, sku, description, price, stock, category, unit, minStock } = req.body;
    const error = validateRequired(req.body, ['name', 'sku', 'price']);
    if (error) {
      return res.status(400).json({ success: false, error });
    }

    // Check duplicate SKU
    const existing = Array.from(products.values()).find(p => p.sku === sku);
    if (existing) {
      return res.status(409).json({ success: false, error: 'Ya existe un producto con ese SKU' });
    }

    const product = {
      id: uuidv4(),
      name,
      sku,
      description: description || '',
      price: Number(price),
      stock: Number(stock) || 0,
      category: category || 'general',
      unit: unit || 'unidad',
      minStock: Number(minStock) || 0,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    products.set(product.id, product);
    logger.dataAccess(req, 'products', 'create');

    res.status(201).json({ success: true, product });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear producto' });
  }
});

// List products
app.get('/api/products', (req, res) => {
  try {
    const { category, search, inStock, limit, offset, sort, order } = req.query;
    let items = Array.from(products.values());

    if (category) {
      items = items.filter(p => p.category === category);
    }
    if (search) {
      const term = search.toLowerCase();
      items = items.filter(p =>
        p.name.toLowerCase().includes(term) ||
        p.sku.toLowerCase().includes(term) ||
        p.description.toLowerCase().includes(term)
      );
    }
    if (inStock === 'true') {
      items = items.filter(p => p.stock > 0);
    }
    if (inStock === 'false') {
      items = items.filter(p => p.stock === 0);
    }

    // Sort
    const sortField = sort || 'createdAt';
    const sortOrder = order || 'desc';
    items.sort((a, b) => {
      const aVal = a[sortField] || '';
      const bVal = b[sortField] || '';
      if (sortOrder === 'desc') return bVal > aVal ? 1 : -1;
      return aVal > bVal ? 1 : -1;
    });

    const total = items.length;
    const lim = Number(limit) || 50;
    const off = Number(offset) || 0;
    const paginated = items.slice(off, off + lim);

    // Low stock alerts
    const lowStock = Array.from(products.values()).filter(p => p.stock > 0 && p.stock <= p.minStock);

    res.json({ success: true, products: paginated, total, limit: lim, offset: off, lowStockCount: lowStock.length });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar productos' });
  }
});

// Get product by ID
app.get('/api/products/:id', (req, res) => {
  try {
    const product = products.get(req.params.id);
    if (!product) {
      return res.status(404).json({ success: false, error: 'Producto no encontrado' });
    }
    res.json({ success: true, product });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener producto' });
  }
});

// Update product
app.put('/api/products/:id', (req, res) => {
  try {
    const product = products.get(req.params.id);
    if (!product) {
      return res.status(404).json({ success: false, error: 'Producto no encontrado' });
    }

    const { name, sku, description, price, stock, category, unit, minStock, isActive } = req.body;

    if (name !== undefined) product.name = name;
    if (sku !== undefined) product.sku = sku;
    if (description !== undefined) product.description = description;
    if (price !== undefined) product.price = Number(price);
    if (stock !== undefined) product.stock = Number(stock);
    if (category !== undefined) product.category = category;
    if (unit !== undefined) product.unit = unit;
    if (minStock !== undefined) product.minStock = Number(minStock);
    if (isActive !== undefined) product.isActive = isActive;
    product.updatedAt = new Date().toISOString();

    logger.dataAccess(req, 'products', 'update');

    res.json({ success: true, product });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al actualizar producto' });
  }
});

// Delete product
app.delete('/api/products/:id', (req, res) => {
  try {
    if (!products.has(req.params.id)) {
      return res.status(404).json({ success: false, error: 'Producto no encontrado' });
    }

    products.delete(req.params.id);
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
app.post('/api/invoices', (req, res) => {
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

    // Generate invoice number
    const invoiceCount = invoices.size + 1;
    const invoiceNumber = `INV-${new Date().getFullYear()}-${String(invoiceCount).padStart(5, '0')}`;

    const invoice = {
      id: uuidv4(),
      invoiceNumber,
      customer,
      customerEmail: customerEmail || null,
      items: lineItems,
      subtotal: Math.round(subtotal * 100) / 100,
      taxRate,
      taxAmount: Math.round(taxAmount * 100) / 100,
      total: Math.round(total * 100) / 100,
      currency: currency || 'USD',
      status: 'draft',
      notes: notes || '',
      dueDate: dueDate || new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      issuedAt: new Date().toISOString(),
      paidAt: null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    invoices.set(invoice.id, invoice);
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
app.get('/api/invoices', (req, res) => {
  try {
    const { status, customer, search, limit, offset, sort, order } = req.query;
    let items = Array.from(invoices.values());

    if (status) {
      items = items.filter(inv => inv.status === status);
    }
    if (customer) {
      items = items.filter(inv => inv.customer.toLowerCase().includes(customer.toLowerCase()));
    }
    if (search) {
      const term = search.toLowerCase();
      items = items.filter(inv =>
        inv.invoiceNumber.toLowerCase().includes(term) ||
        inv.customer.toLowerCase().includes(term)
      );
    }

    // Sort
    const sortField = sort || 'createdAt';
    const sortOrder = order || 'desc';
    items.sort((a, b) => {
      const aVal = a[sortField] || '';
      const bVal = b[sortField] || '';
      if (sortOrder === 'desc') return bVal > aVal ? 1 : -1;
      return aVal > bVal ? 1 : -1;
    });

    const total = items.length;
    const lim = Number(limit) || 50;
    const off = Number(offset) || 0;
    const paginated = items.slice(off, off + lim);

    res.json({ success: true, invoices: paginated, total, limit: lim, offset: off });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar facturas' });
  }
});

// Get invoice by ID
app.get('/api/invoices/:id', (req, res) => {
  try {
    const invoice = invoices.get(req.params.id);
    if (!invoice) {
      return res.status(404).json({ success: false, error: 'Factura no encontrada' });
    }
    res.json({ success: true, invoice });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener factura' });
  }
});

// Update invoice
app.put('/api/invoices/:id', (req, res) => {
  try {
    const invoice = invoices.get(req.params.id);
    if (!invoice) {
      return res.status(404).json({ success: false, error: 'Factura no encontrada' });
    }

    // Only allow editing draft invoices
    if (invoice.status !== 'draft' && req.body.items) {
      return res.status(400).json({
        success: false,
        error: 'Solo se pueden editar items en facturas con estado draft'
      });
    }

    const { customer, customerEmail, items, notes, dueDate, status } = req.body;

    if (customer !== undefined) invoice.customer = customer;
    if (customerEmail !== undefined) invoice.customerEmail = customerEmail;
    if (notes !== undefined) invoice.notes = notes;
    if (dueDate !== undefined) invoice.dueDate = dueDate;

    // Update status
    if (status !== undefined) {
      const validStatuses = ['draft', 'sent', 'paid', 'overdue', 'cancelled'];
      if (!validStatuses.includes(status)) {
        return res.status(400).json({
          success: false,
          error: `Estado invalido. Valores validos: ${validStatuses.join(', ')}`
        });
      }
      invoice.status = status;
      if (status === 'paid') {
        invoice.paidAt = new Date().toISOString();
      }
    }

    // Recalculate if items changed
    if (items && Array.isArray(items) && items.length > 0) {
      invoice.items = items.map(item => {
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

      const subtotal = invoice.items.reduce((sum, item) => sum + item.subtotal, 0);
      invoice.subtotal = Math.round(subtotal * 100) / 100;
      invoice.taxAmount = Math.round(subtotal * invoice.taxRate * 100) / 100;
      invoice.total = Math.round((invoice.subtotal + invoice.taxAmount) * 100) / 100;
    }

    invoice.updatedAt = new Date().toISOString();

    logger.dataAccess(req, 'invoices', 'update');

    res.json({ success: true, invoice });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al actualizar factura' });
  }
});

// Delete invoice
app.delete('/api/invoices/:id', (req, res) => {
  try {
    const invoice = invoices.get(req.params.id);
    if (!invoice) {
      return res.status(404).json({ success: false, error: 'Factura no encontrada' });
    }

    if (invoice.status === 'paid') {
      return res.status(400).json({
        success: false,
        error: 'No se puede eliminar una factura pagada'
      });
    }

    invoices.delete(req.params.id);
    logger.dataAccess(req, 'invoices', 'delete');

    res.json({ success: true, message: 'Factura eliminada exitosamente' });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al eliminar factura' });
  }
});

// ============================================================================
// REST API — DASHBOARD
// ============================================================================

app.get('/api/dashboard', (req, res) => {
  try {
    const allInvoices = Array.from(invoices.values());
    const paidInvoices = allInvoices.filter(inv => inv.status === 'paid');
    const pendingInvoices = allInvoices.filter(inv => inv.status === 'sent' || inv.status === 'draft');
    const overdueInvoices = allInvoices.filter(inv => inv.status === 'overdue');

    const totalRevenue = paidInvoices.reduce((sum, inv) => sum + inv.total, 0);
    const pendingRevenue = pendingInvoices.reduce((sum, inv) => sum + inv.total, 0);
    const overdueRevenue = overdueInvoices.reduce((sum, inv) => sum + inv.total, 0);

    // Contact statistics
    const contactsByStatus = {};
    for (const contact of contacts.values()) {
      contactsByStatus[contact.status] = (contactsByStatus[contact.status] || 0) + 1;
    }

    // Product statistics
    const allProducts = Array.from(products.values());
    const totalStockValue = allProducts.reduce((sum, p) => sum + (p.price * p.stock), 0);
    const lowStockProducts = allProducts.filter(p => p.stock > 0 && p.stock <= p.minStock);
    const outOfStockProducts = allProducts.filter(p => p.stock === 0);

    // Invoice status breakdown
    const invoicesByStatus = {};
    for (const inv of allInvoices) {
      invoicesByStatus[inv.status] = (invoicesByStatus[inv.status] || 0) + 1;
    }

    // Recent invoices
    const recentInvoices = allInvoices
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, 5)
      .map(inv => ({
        id: inv.id,
        invoiceNumber: inv.invoiceNumber,
        customer: inv.customer,
        total: inv.total,
        status: inv.status,
        createdAt: inv.createdAt
      }));

    res.json({
      success: true,
      dashboard: {
        summary: {
          totalContacts: contacts.size,
          totalProducts: products.size,
          totalInvoices: invoices.size,
          totalRevenue: Math.round(totalRevenue * 100) / 100,
          pendingRevenue: Math.round(pendingRevenue * 100) / 100,
          overdueRevenue: Math.round(overdueRevenue * 100) / 100,
          totalStockValue: Math.round(totalStockValue * 100) / 100
        },
        contacts: {
          total: contacts.size,
          byStatus: contactsByStatus
        },
        products: {
          total: products.size,
          inStock: allProducts.filter(p => p.stock > 0).length,
          outOfStock: outOfStockProducts.length,
          lowStock: lowStockProducts.length,
          totalStockValue: Math.round(totalStockValue * 100) / 100
        },
        invoices: {
          total: invoices.size,
          byStatus: invoicesByStatus,
          totalRevenue: Math.round(totalRevenue * 100) / 100,
          recentInvoices
        },
        activeModules: ERP_MODULES.filter(m => m.status === 'active').length,
        taxRate: '0% — Sovereign Article VII',
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

app.listen(PORT, () => {
  console.log('');
  console.log('  ============================================================');
  console.log('  ||                                                        ||');
  console.log('  ||     EMPRESA SOBERANA                                   ||');
  console.log('  ||     Sovereign ERP — Enterprise Resource Planning       ||');
  console.log('  ||                                                        ||');
  console.log('  ||     Modules: CRM, Accounting, Inventory, HR, Mfg      ||');
  console.log('  ||     Tax Rate: 0% — Sovereign Article VII               ||');
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
  console.log('');
});

module.exports = app;

'use strict';

// ============================================================================
// EMPRESA SOBERANA — PostgreSQL Database Layer
// Persistent storage for CRM, Inventory, Invoicing
// Ierahkwa Ne Kanienke / MameyNode Platform
// ============================================================================

const { Pool } = require('pg');
const { createLogger } = require('../shared/logger');

const logger = createLogger('empresa-soberana-db');

const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://localhost:5432/empresa_soberana';

const pool = new Pool({
  connectionString: DATABASE_URL,
  max: 20,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 5000
});

pool.on('error', (err) => {
  logger.error('Unexpected pool error', { err });
});

// ============================================================================
// CORE FUNCTIONS
// ============================================================================

/**
 * Execute a parameterized query
 * @param {string} text - SQL query with $1, $2, ... placeholders
 * @param {Array} params - Parameter values
 * @returns {Promise<pg.QueryResult>}
 */
async function query(text, params) {
  const start = Date.now();
  const result = await pool.query(text, params);
  const duration = Date.now() - start;
  logger.debug('query executed', { text: text.substring(0, 80), duration, rows: result.rowCount });
  return result;
}

/**
 * Get a client from the pool for manual transaction control
 * Caller MUST call client.release() when done
 * @returns {Promise<pg.PoolClient>}
 */
async function getClient() {
  return pool.connect();
}

/**
 * Execute a function inside a database transaction
 * Automatically commits on success, rolls back on error
 * @param {Function} fn - async function(client) => result
 * @returns {Promise<*>} Result of fn
 */
async function transaction(fn) {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    const result = await fn(client);
    await client.query('COMMIT');
    return result;
  } catch (err) {
    await client.query('ROLLBACK');
    throw err;
  } finally {
    client.release();
  }
}

/**
 * Initialize database tables
 * Creates all tables if they do not exist
 */
async function initialize() {
  logger.info('Initializing database tables');

  await query(`
    CREATE TABLE IF NOT EXISTS contacts (
      id UUID PRIMARY KEY,
      name VARCHAR(255) NOT NULL,
      email VARCHAR(255) NOT NULL UNIQUE,
      phone VARCHAR(100),
      company VARCHAR(255),
      status VARCHAR(50) NOT NULL DEFAULT 'lead',
      notes TEXT NOT NULL DEFAULT '',
      tags JSONB NOT NULL DEFAULT '[]',
      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
      updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS products (
      id UUID PRIMARY KEY,
      name VARCHAR(255) NOT NULL,
      sku VARCHAR(100) NOT NULL UNIQUE,
      description TEXT NOT NULL DEFAULT '',
      price NUMERIC(12,2) NOT NULL,
      stock INTEGER NOT NULL DEFAULT 0,
      category VARCHAR(100) NOT NULL DEFAULT 'general',
      unit VARCHAR(50) NOT NULL DEFAULT 'unidad',
      min_stock INTEGER NOT NULL DEFAULT 0,
      is_active BOOLEAN NOT NULL DEFAULT true,
      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
      updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  await query(`
    CREATE TABLE IF NOT EXISTS invoices (
      id UUID PRIMARY KEY,
      invoice_number VARCHAR(50) NOT NULL UNIQUE,
      customer VARCHAR(255) NOT NULL,
      customer_email VARCHAR(255),
      items JSONB NOT NULL DEFAULT '[]',
      subtotal NUMERIC(12,2) NOT NULL DEFAULT 0,
      tax_rate NUMERIC(5,4) NOT NULL DEFAULT 0,
      tax_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
      total NUMERIC(12,2) NOT NULL DEFAULT 0,
      currency VARCHAR(10) NOT NULL DEFAULT 'USD',
      status VARCHAR(50) NOT NULL DEFAULT 'draft',
      notes TEXT NOT NULL DEFAULT '',
      due_date VARCHAR(20),
      issued_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
      paid_at TIMESTAMPTZ,
      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
      updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);

  // Indexes for common query patterns
  await query(`CREATE INDEX IF NOT EXISTS idx_contacts_email ON contacts (email)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_contacts_status ON contacts (status)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_contacts_company ON contacts (company)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_products_sku ON products (sku)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_products_category ON products (category)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_products_stock ON products (stock)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_invoices_status ON invoices (status)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_invoices_customer ON invoices (customer)`);
  await query(`CREATE INDEX IF NOT EXISTS idx_invoices_invoice_number ON invoices (invoice_number)`);

  logger.info('Database tables initialized successfully');
}

/**
 * Close the connection pool gracefully
 */
async function end() {
  logger.info('Closing database connection pool');
  await pool.end();
}

/**
 * Return pool statistics
 * @returns {object} Pool stats
 */
function stats() {
  return {
    totalCount: pool.totalCount,
    idleCount: pool.idleCount,
    waitingCount: pool.waitingCount
  };
}

// ============================================================================
// ROW MAPPERS — Convert snake_case DB rows to camelCase API objects
// ============================================================================

function mapContactRow(row) {
  return {
    id: row.id,
    name: row.name,
    email: row.email,
    phone: row.phone,
    company: row.company,
    status: row.status,
    notes: row.notes,
    tags: row.tags,
    createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at,
    updatedAt: row.updated_at instanceof Date ? row.updated_at.toISOString() : row.updated_at
  };
}

function mapProductRow(row) {
  return {
    id: row.id,
    name: row.name,
    sku: row.sku,
    description: row.description,
    price: Number(row.price),
    stock: row.stock,
    category: row.category,
    unit: row.unit,
    minStock: row.min_stock,
    isActive: row.is_active,
    createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at,
    updatedAt: row.updated_at instanceof Date ? row.updated_at.toISOString() : row.updated_at
  };
}

function mapInvoiceRow(row) {
  return {
    id: row.id,
    invoiceNumber: row.invoice_number,
    customer: row.customer,
    customerEmail: row.customer_email,
    items: row.items,
    subtotal: Number(row.subtotal),
    taxRate: Number(row.tax_rate),
    taxAmount: Number(row.tax_amount),
    total: Number(row.total),
    currency: row.currency,
    status: row.status,
    notes: row.notes,
    dueDate: row.due_date,
    issuedAt: row.issued_at instanceof Date ? row.issued_at.toISOString() : row.issued_at,
    paidAt: row.paid_at instanceof Date ? row.paid_at.toISOString() : row.paid_at,
    createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at,
    updatedAt: row.updated_at instanceof Date ? row.updated_at.toISOString() : row.updated_at
  };
}

// ============================================================================
// Exports
// ============================================================================

module.exports = {
  query,
  getClient,
  transaction,
  initialize,
  end,
  stats,
  pool,
  mapContactRow,
  mapProductRow,
  mapInvoiceRow
};

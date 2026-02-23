/** 
 * ╔═══════════════════════════════════════════════════════════════════════════╗
 * ║      IERAHKWA SOVEREIGN BACKUP SYSTEM                                     ║
 * ║                                                                           ║
 * ║  Complete Backup Infrastructure for All Financial Systems:               ║
 * ║  • Node Backup          • Bank Backup           • Exchange Backup        ║
 * ║  • Trading Deck Backup  • Monetary System Backup                         ║
 * ║  • Humanitarian Fund Backup  • Full Platform Backup                      ║
 * ║                                                                           ║
 * ║  Ierahkwa Sovereign Blockchain (ISB) • IGT Tokens                        ║
 * ║  Ierahkwa Futurehead Mamey Node                                          ║
 * ╚═══════════════════════════════════════════════════════════════════════════╝
 */
import db from '../db.js';
import { existsSync, mkdirSync, writeFileSync, readFileSync, readdirSync, statSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const BACKUP_ROOT = join(__dirname, '..', '..', 'backups');

// Backup Categories
const BACKUP_CATEGORIES = {
  NODE: {
    id: 'node',
    name: 'Ierahkwa Futurehead Mamey Node',
    description: 'Core blockchain node data and configuration',
    icon: 'hdd-network',
    color: '#6366f1',
    tables: ['node_config', 'blockchain_state', 'blocks', 'validators', 'smart_contracts', 'tx_pool', 'ledger', 'blockchain_accounts', 'network_peers', 'blockchain_events']
  },
  BANK: {
    id: 'bank',
    name: 'Ierahkwa Futurehead BDET Bank',
    description: 'Central bank accounts, transactions, reserves',
    icon: 'bank',
    color: '#10b981',
    tables: ['central_banks', 'futurehead_entities', 'government_accounts', 'department_accounts', 'bank_transactions']
  },
  GLOBAL_BANKS: {
    id: 'global-banks',
    name: 'Global Banking System',
    description: 'Regional, National, Commercial, Fintech, Investment banks',
    icon: 'globe2',
    color: '#0ea5e9',
    tables: ['regional_banks', 'national_banks', 'commercial_banks', 'fintech_banks', 'investment_banks', 'bank_types', 'correspondent_banking']
  },
  CLEARINGHOUSE: {
    id: 'clearinghouse',
    name: 'Clearinghouse & Settlement',
    description: 'Clearinghouse config, settlement queue, settlement history',
    icon: 'arrow-left-right',
    color: '#14b8a6',
    tables: ['clearinghouse', 'settlement_queue', 'settlement_history']
  },
  EXCHANGE: {
    id: 'exchange',
    name: 'Exchange System',
    description: 'Currency exchange rates, swap history, liquidity pools',
    icon: 'currency-exchange',
    color: '#f59e0b',
    tables: ['exchange_rates', 'exchange_history', 'liquidity_pools', 'forex_pairs']
  },
  TRADING: {
    id: 'trading',
    name: 'Trading Deck',
    description: 'Trading orders, history, positions, market data',
    icon: 'graph-up-arrow',
    color: '#ef4444',
    tables: ['trade_orders', 'trade_history', 'trading_pairs', 'market_data', 'positions']
  },
  MONETARY: {
    id: 'monetary',
    name: 'Monetary System M0-M4',
    description: 'Asset holdings, conversions, monetary classifications',
    icon: 'stack',
    color: '#8b5cf6',
    tables: ['monetary_classifications', 'asset_holdings', 'asset_conversions', 'mclass_balances']
  },
  HUMANITARIAN: {
    id: 'humanitarian',
    name: 'Humanitarian Fund',
    description: 'Humanitarian fund allocations, distributions, transactions',
    icon: 'heart',
    color: '#ec4899',
    tables: ['humanitarian_fund']
  },
  TOKENS: {
    id: 'tokens',
    name: 'Token Registry',
    description: 'IGT tokens, department tokens, token metadata',
    icon: 'coin',
    color: '#06b6d4',
    tables: ['token_registry', 'token_holders', 'token_transfers']
  },
  SHOP: {
    id: 'shop',
    name: 'E-Commerce Shop',
    description: 'Products, orders, customers, inventory',
    icon: 'shop',
    color: '#84cc16',
    tables: ['products', 'orders', 'customers', 'inventory', 'categories']
  },
  USERS: {
    id: 'users',
    name: 'Users & Auth',
    description: 'User accounts, authentication, permissions',
    icon: 'people',
    color: '#a855f7',
    tables: ['users', 'admins', 'sessions', 'permissions']
  },
  SERVICES: {
    id: 'services',
    name: 'Platform Services',
    description: 'Service registry, external integrations, platform config',
    icon: 'grid-3x3-gap',
    color: '#f97316',
    tables: ['platform_config', 'service_registry', 'external_services']
  }
};

// Ensure backup directories exist
function ensureBackupDirs() {
  if (!existsSync(BACKUP_ROOT)) {
    mkdirSync(BACKUP_ROOT, { recursive: true });
  }
  
  Object.values(BACKUP_CATEGORIES).forEach(cat => {
    const catDir = join(BACKUP_ROOT, cat.id);
    if (!existsSync(catDir)) {
      mkdirSync(catDir, { recursive: true });
    }
  });
  
  // Full backups directory
  const fullDir = join(BACKUP_ROOT, 'full');
  if (!existsSync(fullDir)) {
    mkdirSync(fullDir, { recursive: true });
  }
}

// Generate backup filename with timestamp
function generateBackupFilename(category, type = 'manual') {
  const now = new Date();
  const timestamp = now.toISOString().replace(/[:.]/g, '-').slice(0, 19);
  return `${category}-${type}-${timestamp}.json`;
}

// Create backup for specific category
function createCategoryBackup(categoryId, type = 'manual') {
  const category = Object.values(BACKUP_CATEGORIES).find(c => c.id === categoryId);
  if (!category) return null;
  
  const data = db.get();
  const backupData = {
    meta: {
      category: category.id,
      category_name: category.name,
      type,
      created_at: new Date().toISOString(),
      version: '1.0.0',
      node: 'Ierahkwa Futurehead Mamey Node',
      blockchain: 'Ierahkwa Sovereign Blockchain (ISB)'
    },
    data: {}
  };
  
  // Extract relevant tables
  category.tables.forEach(table => {
    if (data[table] !== undefined) {
      backupData.data[table] = data[table];
    }
  });
  
  // Calculate stats
  backupData.meta.tables_count = Object.keys(backupData.data).length;
  backupData.meta.total_records = Object.values(backupData.data).reduce((sum, arr) => {
    return sum + (Array.isArray(arr) ? arr.length : 1);
  }, 0);
  
  // Save backup file
  const filename = generateBackupFilename(category.id, type);
  const filepath = join(BACKUP_ROOT, category.id, filename);
  writeFileSync(filepath, JSON.stringify(backupData, null, 2));
  
  return {
    filename,
    filepath,
    category: category.id,
    category_name: category.name,
    size: statSync(filepath).size,
    tables: backupData.meta.tables_count,
    records: backupData.meta.total_records,
    created_at: backupData.meta.created_at
  };
}

// Create full platform backup
function createFullBackup(type = 'manual') {
  const data = db.get();
  const now = new Date();
  const timestamp = now.toISOString().replace(/[:.]/g, '-').slice(0, 19);
  
  const backupData = {
    meta: {
      type: 'full_platform',
      backup_type: type,
      created_at: now.toISOString(),
      version: '1.0.0',
      platform: 'Ierahkwa Futurehead Platform',
      node: 'Ierahkwa Futurehead Mamey Node',
      blockchain: 'Ierahkwa Sovereign Blockchain (ISB)',
      bank: 'Ierahkwa Futurehead BDET Bank'
    },
    categories: {},
    full_data: data
  };
  
  // Include category summaries
  Object.values(BACKUP_CATEGORIES).forEach(cat => {
    const categoryData = {};
    cat.tables.forEach(table => {
      if (data[table] !== undefined) {
        categoryData[table] = Array.isArray(data[table]) ? data[table].length : 1;
      }
    });
    backupData.categories[cat.id] = {
      name: cat.name,
      tables: categoryData
    };
  });
  
  // Save full backup
  const filename = `full-platform-${type}-${timestamp}.json`;
  const filepath = join(BACKUP_ROOT, 'full', filename);
  writeFileSync(filepath, JSON.stringify(backupData, null, 2));
  
  return {
    filename,
    filepath,
    type: 'full_platform',
    size: statSync(filepath).size,
    created_at: backupData.meta.created_at
  };
}

// List backups for a category
function listBackups(categoryId = null) {
  const backups = [];
  
  const dirs = categoryId 
    ? [categoryId] 
    : [...Object.values(BACKUP_CATEGORIES).map(c => c.id), 'full'];
  
  dirs.forEach(dir => {
    const dirPath = join(BACKUP_ROOT, dir);
    if (existsSync(dirPath)) {
      const files = readdirSync(dirPath).filter(f => f.endsWith('.json'));
      files.forEach(file => {
        const filepath = join(dirPath, file);
        const stats = statSync(filepath);
        backups.push({
          filename: file,
          category: dir,
          size: stats.size,
          created_at: stats.mtime.toISOString()
        });
      });
    }
  });
  
  return backups.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
}

// Restore from backup
function restoreBackup(categoryId, filename) {
  const filepath = join(BACKUP_ROOT, categoryId, filename);
  if (!existsSync(filepath)) return { error: 'Backup file not found' };
  
  try {
    const backupData = JSON.parse(readFileSync(filepath, 'utf8'));
    const data = db.get();
    
    if (backupData.full_data) {
      // Full restore
      Object.assign(data, backupData.full_data);
    } else if (backupData.data) {
      // Category restore
      Object.entries(backupData.data).forEach(([table, tableData]) => {
        data[table] = tableData;
      });
    }
    
    db.save();
    
    return {
      ok: true,
      restored_from: filename,
      category: categoryId,
      restored_at: new Date().toISOString()
    };
  } catch (error) {
    return { error: 'Failed to restore backup: ' + error.message };
  }
}

export default async function backupRoutes(fastify) {
  ensureBackupDirs();

  // ==================== BACKUP CATEGORIES ====================
  
  fastify.get('/api/backup/categories', async () => {
    return Object.values(BACKUP_CATEGORIES).map(cat => ({
      ...cat,
      backup_count: listBackups(cat.id).length
    }));
  });

  // ==================== CREATE BACKUPS ====================
  
  fastify.post('/api/backup/create/:category', async (req) => {
    const { category } = req.params;
    const { type } = req.body || {};
    
    if (category === 'full') {
      const result = createFullBackup(type || 'manual');
      return { ok: true, backup: result };
    }
    
    const catExists = Object.values(BACKUP_CATEGORIES).find(c => c.id === category);
    if (!catExists) return { error: 'Invalid backup category' };
    
    const result = createCategoryBackup(category, type || 'manual');
    if (!result) return { error: 'Failed to create backup' };
    
    return { ok: true, backup: result };
  });

  fastify.post('/api/backup/create-all', async (req) => {
    const { type } = req.body || {};
    const results = [];
    
    // Create individual category backups
    for (const cat of Object.values(BACKUP_CATEGORIES)) {
      const result = createCategoryBackup(cat.id, type || 'manual');
      if (result) results.push(result);
    }
    
    // Create full backup
    const fullBackup = createFullBackup(type || 'manual');
    results.push({ ...fullBackup, category: 'full' });
    
    return { ok: true, backups: results, count: results.length };
  });

  // ==================== LIST BACKUPS ====================
  
  fastify.get('/api/backup/list', async (req) => {
    const { category } = req.query;
    return listBackups(category || null);
  });

  fastify.get('/api/backup/list/:category', async (req) => {
    return listBackups(req.params.category);
  });

  // ==================== RESTORE BACKUPS ====================
  
  fastify.post('/api/backup/restore/:category/:filename', async (req) => {
    const { category, filename } = req.params;
    
    // Create safety backup before restore
    if (category === 'full') {
      createFullBackup('pre-restore');
    } else {
      createCategoryBackup(category, 'pre-restore');
    }
    
    const result = restoreBackup(category, filename);
    return result;
  });

  // ==================== DOWNLOAD BACKUP ====================
  
  fastify.get('/api/backup/download/:category/:filename', async (req, reply) => {
    const { category, filename } = req.params;
    const filepath = join(BACKUP_ROOT, category, filename);
    
    if (!existsSync(filepath)) {
      return { error: 'Backup file not found' };
    }
    
    const content = readFileSync(filepath, 'utf8');
    reply.header('Content-Type', 'application/json');
    reply.header('Content-Disposition', `attachment; filename="${filename}"`);
    return content;
  });

  // ==================== DELETE BACKUP ====================
  
  fastify.delete('/api/backup/:category/:filename', async (req) => {
    const { category, filename } = req.params;
    const filepath = join(BACKUP_ROOT, category, filename);
    
    if (!existsSync(filepath)) {
      return { error: 'Backup file not found' };
    }
    
    try {
      const { unlinkSync } = await import('fs');
      unlinkSync(filepath);
      return { ok: true, deleted: filename };
    } catch (error) {
      return { error: 'Failed to delete backup' };
    }
  });

  // ==================== BACKUP STATS ====================
  
  fastify.get('/api/backup/stats', async () => {
    const allBackups = listBackups();
    const data = db.get();
    
    const stats = {
      total_backups: allBackups.length,
      total_size: allBackups.reduce((sum, b) => sum + b.size, 0),
      by_category: {},
      latest_backup: allBackups[0] || null,
      database_stats: {
        central_banks: (data.central_banks || []).length,
        futurehead_entities: (data.futurehead_entities || []).length,
        government_accounts: (data.government_accounts || []).length,
        department_accounts: (data.department_accounts || []).length,
        trade_orders: (data.trade_orders || []).length,
        trade_history: (data.trade_history || []).length,
        asset_holdings: (data.asset_holdings || []).length,
        bank_transactions: (data.bank_transactions || []).length,
        tokens: (data.token_registry || []).length,
        products: (data.products || []).length,
        orders: (data.orders || []).length
      }
    };
    
    // Group by category
    Object.values(BACKUP_CATEGORIES).forEach(cat => {
      const catBackups = allBackups.filter(b => b.category === cat.id);
      stats.by_category[cat.id] = {
        name: cat.name,
        count: catBackups.length,
        size: catBackups.reduce((sum, b) => sum + b.size, 0),
        latest: catBackups[0] || null
      };
    });
    
    // Full backups
    const fullBackups = allBackups.filter(b => b.category === 'full');
    stats.by_category['full'] = {
      name: 'Full Platform Backup',
      count: fullBackups.length,
      size: fullBackups.reduce((sum, b) => sum + b.size, 0),
      latest: fullBackups[0] || null
    };
    
    return stats;
  });

  // ==================== AUTO BACKUP SCHEDULE ====================
  
  fastify.post('/api/backup/auto', async (req) => {
    // Create automatic backup
    const results = [];
    
    for (const cat of Object.values(BACKUP_CATEGORIES)) {
      const result = createCategoryBackup(cat.id, 'auto');
      if (result) results.push(result);
    }
    
    const fullBackup = createFullBackup('auto');
    results.push({ ...fullBackup, category: 'full' });
    
    return { 
      ok: true, 
      message: 'Automatic backup completed',
      backups: results,
      timestamp: new Date().toISOString()
    };
  });

  // ==================== EXPORT/IMPORT ====================
  
  fastify.get('/api/backup/export/full', async (req, reply) => {
    const data = db.get();
    const now = new Date();
    
    const exportData = {
      meta: {
        exported_at: now.toISOString(),
        platform: 'Ierahkwa Futurehead Platform',
        version: '1.0.0'
      },
      data
    };
    
    reply.header('Content-Type', 'application/json');
    reply.header('Content-Disposition', `attachment; filename="ierahkwa-full-export-${now.toISOString().slice(0, 10)}.json"`);
    return JSON.stringify(exportData, null, 2);
  });

  fastify.post('/api/backup/import', async (req) => {
    const { data: importData, overwrite } = req.body;
    
    if (!importData) return { error: 'No data provided' };
    
    // Create safety backup
    createFullBackup('pre-import');
    
    const data = db.get();
    
    if (overwrite) {
      // Full overwrite
      Object.assign(data, importData.data || importData);
    } else {
      // Merge (add new, don't overwrite existing)
      const toImport = importData.data || importData;
      Object.entries(toImport).forEach(([key, value]) => {
        if (Array.isArray(value) && Array.isArray(data[key])) {
          // Merge arrays
          const existingIds = new Set(data[key].map(i => i.id));
          value.forEach(item => {
            if (!existingIds.has(item.id)) {
              data[key].push(item);
            }
          });
        } else if (!data[key]) {
          data[key] = value;
        }
      });
    }
    
    db.save();
    
    return { 
      ok: true, 
      message: overwrite ? 'Data imported (overwrite)' : 'Data merged',
      imported_at: new Date().toISOString()
    };
  });

  // ==================== DEPARTMENT BACKUPS ====================
  
  fastify.get('/api/backup/departments', async () => {
    const data = db.get();
    const departments = data.department_accounts || [];
    
    // Ensure department backup folders exist
    const deptBackupRoot = join(BACKUP_ROOT, 'departments');
    if (!existsSync(deptBackupRoot)) {
      mkdirSync(deptBackupRoot, { recursive: true });
    }
    
    return departments.map(dept => {
      const deptDir = join(deptBackupRoot, dept.token_symbol || `DEPT-${dept.id}`);
      let backupCount = 0;
      if (existsSync(deptDir)) {
        backupCount = readdirSync(deptDir).filter(f => f.endsWith('.json')).length;
      }
      return {
        id: dept.id,
        name: dept.department_name,
        symbol: dept.token_symbol,
        category: dept.category,
        backup_count: backupCount
      };
    });
  });

  fastify.post('/api/backup/departments/:id', async (req) => {
    const data = db.get();
    const deptId = parseInt(req.params.id);
    const dept = (data.department_accounts || []).find(d => d.id === deptId);
    
    if (!dept) return { error: 'Department not found' };
    
    const deptBackupRoot = join(BACKUP_ROOT, 'departments');
    const deptDir = join(deptBackupRoot, dept.token_symbol || `DEPT-${dept.id}`);
    
    if (!existsSync(deptDir)) {
      mkdirSync(deptDir, { recursive: true });
    }
    
    // Gather all department-related data
    const deptData = {
      meta: {
        department_id: dept.id,
        department_name: dept.department_name,
        token_symbol: dept.token_symbol,
        category: dept.category,
        created_at: new Date().toISOString(),
        backup_type: 'department'
      },
      account: dept,
      transactions: (data.bank_transactions || []).filter(t => 
        t.account_id === deptId || t.entity_id === deptId || 
        t.entity_id === String(deptId)
      ),
      trades: (data.trade_history || []).filter(t => 
        t.account_id === deptId || t.entity_id === deptId
      ),
      orders: (data.trade_orders || []).filter(o => 
        o.account_id === deptId || o.entity_id === deptId
      ),
      holdings: (data.asset_holdings || []).filter(h => 
        h.entity_id === deptId || h.entity_id === String(deptId)
      ),
      token: (data.token_registry || []).find(t => t.symbol === dept.token_symbol)
    };
    
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
    const filename = `${dept.token_symbol}-backup-${timestamp}.json`;
    const filepath = join(deptDir, filename);
    
    writeFileSync(filepath, JSON.stringify(deptData, null, 2));
    
    return {
      ok: true,
      department: dept.department_name,
      filename,
      size: statSync(filepath).size,
      records: {
        transactions: deptData.transactions.length,
        trades: deptData.trades.length,
        orders: deptData.orders.length,
        holdings: deptData.holdings.length
      }
    };
  });

  fastify.post('/api/backup/departments/all', async () => {
    const data = db.get();
    const departments = data.department_accounts || [];
    const results = [];
    
    const deptBackupRoot = join(BACKUP_ROOT, 'departments');
    if (!existsSync(deptBackupRoot)) {
      mkdirSync(deptBackupRoot, { recursive: true });
    }
    
    for (const dept of departments) {
      const deptDir = join(deptBackupRoot, dept.token_symbol || `DEPT-${dept.id}`);
      if (!existsSync(deptDir)) {
        mkdirSync(deptDir, { recursive: true });
      }
      
      const deptData = {
        meta: {
          department_id: dept.id,
          department_name: dept.department_name,
          token_symbol: dept.token_symbol,
          category: dept.category,
          created_at: new Date().toISOString(),
          backup_type: 'department'
        },
        account: dept,
        transactions: (data.bank_transactions || []).filter(t => 
          t.account_id === dept.id || t.entity_id === dept.id
        ),
        trades: (data.trade_history || []).filter(t => 
          t.account_id === dept.id || t.entity_id === dept.id
        ),
        token: (data.token_registry || []).find(t => t.symbol === dept.token_symbol)
      };
      
      const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
      const filename = `${dept.token_symbol}-backup-${timestamp}.json`;
      const filepath = join(deptDir, filename);
      
      writeFileSync(filepath, JSON.stringify(deptData, null, 2));
      
      results.push({
        department: dept.department_name,
        symbol: dept.token_symbol,
        filename
      });
    }
    
    return { ok: true, backed_up: results.length, departments: results };
  });

  fastify.get('/api/backup/departments/:id/list', async (req) => {
    const data = db.get();
    const deptId = parseInt(req.params.id);
    const dept = (data.department_accounts || []).find(d => d.id === deptId);
    
    if (!dept) return { error: 'Department not found' };
    
    const deptDir = join(BACKUP_ROOT, 'departments', dept.token_symbol || `DEPT-${dept.id}`);
    
    if (!existsSync(deptDir)) return [];
    
    const files = readdirSync(deptDir).filter(f => f.endsWith('.json'));
    return files.map(file => {
      const filepath = join(deptDir, file);
      const stats = statSync(filepath);
      return {
        filename: file,
        size: stats.size,
        created_at: stats.mtime.toISOString()
      };
    }).sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
  });

  // ==================== SOURCE CODE BACKUP ====================
  
  fastify.post('/api/backup/source-code', async () => {
    const sourceDir = join(__dirname, '..', '..');
    const codeBackupDir = join(BACKUP_ROOT, 'source-code');
    
    if (!existsSync(codeBackupDir)) {
      mkdirSync(codeBackupDir, { recursive: true });
    }
    
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
    
    // List all source files to backup
    const sourceFiles = {
      routes: [],
      public: [],
      config: []
    };
    
    // Routes
    const routesDir = join(sourceDir, 'src', 'routes');
    if (existsSync(routesDir)) {
      sourceFiles.routes = readdirSync(routesDir).filter(f => f.endsWith('.js')).map(f => {
        const content = readFileSync(join(routesDir, f), 'utf8');
        return { filename: f, content, lines: content.split('\n').length };
      });
    }
    
    // DB
    const dbFile = join(sourceDir, 'src', 'db.js');
    if (existsSync(dbFile)) {
      sourceFiles.db = {
        filename: 'db.js',
        content: readFileSync(dbFile, 'utf8')
      };
    }
    
    // Server
    const serverFile = join(sourceDir, 'server.js');
    if (existsSync(serverFile)) {
      sourceFiles.server = {
        filename: 'server.js',
        content: readFileSync(serverFile, 'utf8')
      };
    }
    
    // Package.json
    const pkgFile = join(sourceDir, 'package.json');
    if (existsSync(pkgFile)) {
      sourceFiles.package = {
        filename: 'package.json',
        content: readFileSync(pkgFile, 'utf8')
      };
    }
    
    const backupData = {
      meta: {
        type: 'source_code',
        created_at: new Date().toISOString(),
        platform: 'Ierahkwa Futurehead Platform',
        total_routes: sourceFiles.routes.length
      },
      files: sourceFiles
    };
    
    const filename = `source-code-backup-${timestamp}.json`;
    const filepath = join(codeBackupDir, filename);
    writeFileSync(filepath, JSON.stringify(backupData, null, 2));
    
    return {
      ok: true,
      filename,
      size: statSync(filepath).size,
      files_backed_up: {
        routes: sourceFiles.routes.length,
        has_server: !!sourceFiles.server,
        has_db: !!sourceFiles.db,
        has_package: !!sourceFiles.package
      }
    };
  });

  fastify.get('/api/backup/source-code/list', async () => {
    const codeBackupDir = join(BACKUP_ROOT, 'source-code');
    if (!existsSync(codeBackupDir)) return [];
    
    const files = readdirSync(codeBackupDir).filter(f => f.endsWith('.json'));
    return files.map(file => {
      const filepath = join(codeBackupDir, file);
      const stats = statSync(filepath);
      return {
        filename: file,
        size: stats.size,
        created_at: stats.mtime.toISOString()
      };
    }).sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
  });

  // ==================== COMPLETE SYSTEM BACKUP ====================
  
  fastify.post('/api/backup/complete-system', async () => {
    const results = {
      categories: [],
      departments: [],
      source_code: null,
      full_backup: null
    };
    
    // 1. Backup all categories
    for (const cat of Object.values(BACKUP_CATEGORIES)) {
      const result = createCategoryBackup(cat.id, 'complete');
      if (result) results.categories.push(result);
    }
    
    // 2. Backup all departments
    const data = db.get();
    const deptBackupRoot = join(BACKUP_ROOT, 'departments');
    if (!existsSync(deptBackupRoot)) {
      mkdirSync(deptBackupRoot, { recursive: true });
    }
    
    for (const dept of (data.department_accounts || [])) {
      const deptDir = join(deptBackupRoot, dept.token_symbol || `DEPT-${dept.id}`);
      if (!existsSync(deptDir)) {
        mkdirSync(deptDir, { recursive: true });
      }
      
      const deptData = {
        meta: { department_id: dept.id, department_name: dept.department_name, created_at: new Date().toISOString() },
        account: dept
      };
      
      const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
      const filename = `${dept.token_symbol}-complete-${timestamp}.json`;
      writeFileSync(join(deptDir, filename), JSON.stringify(deptData, null, 2));
      results.departments.push({ symbol: dept.token_symbol, filename });
    }
    
    // 3. Backup source code
    const sourceDir = join(__dirname, '..', '..');
    const codeBackupDir = join(BACKUP_ROOT, 'source-code');
    if (!existsSync(codeBackupDir)) mkdirSync(codeBackupDir, { recursive: true });
    
    const routesDir = join(sourceDir, 'src', 'routes');
    const sourceFiles = { routes: [] };
    if (existsSync(routesDir)) {
      sourceFiles.routes = readdirSync(routesDir).filter(f => f.endsWith('.js')).map(f => ({
        filename: f,
        content: readFileSync(join(routesDir, f), 'utf8')
      }));
    }
    
    const serverFile = join(sourceDir, 'server.js');
    if (existsSync(serverFile)) sourceFiles.server = readFileSync(serverFile, 'utf8');
    
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
    const codeFilename = `source-complete-${timestamp}.json`;
    writeFileSync(join(codeBackupDir, codeFilename), JSON.stringify({ meta: { created_at: new Date().toISOString() }, files: sourceFiles }, null, 2));
    results.source_code = codeFilename;
    
    // 4. Full platform backup
    const fullBackup = createFullBackup('complete');
    results.full_backup = fullBackup;
    
    return {
      ok: true,
      message: 'Complete system backup finished',
      timestamp: new Date().toISOString(),
      summary: {
        categories_backed_up: results.categories.length,
        departments_backed_up: results.departments.length,
        source_code_backed_up: !!results.source_code,
        full_backup_created: !!results.full_backup
      },
      results
    };
  });
}

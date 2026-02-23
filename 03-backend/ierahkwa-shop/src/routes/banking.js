/** 
 * ╔═══════════════════════════════════════════════════════════════════════════╗
 * ║      IERAHKWA SOVEREIGN BANKING, EXCHANGE & TRADING SYSTEM               ║
 * ║                                                                           ║
 * ║  Complete Banking Infrastructure:                                         ║
 * ║  • Central Banks • Futurehead BDET Bank • Futurehead Group Holding       ║
 * ║  • Government Accounts • Department Accounts                              ║
 * ║  • Bank Accounts • Exchange Accounts • Trading Deck Accounts             ║
 * ║                                                                           ║
 * ║  Ierahkwa Sovereign Blockchain (ISB) • IGT Tokens                        ║
 * ║  Sovereign Government of Ierahkwa Ne Kanienke                            ║
 * ╚═══════════════════════════════════════════════════════════════════════════╝
 */
import db from '../db.js';
import { readFileSync, existsSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));

// Load departments from JSON
function loadDepartments() {
  const deptFile = join(__dirname, '..', '..', '..', 'government-departments.json');
  if (existsSync(deptFile)) {
    return JSON.parse(readFileSync(deptFile, 'utf8'));
  }
  return { departments: [] };
}

// ============================================
// ENTITY DEFINITIONS
// ============================================

// Central Banks
const CENTRAL_BANKS = [
  { id: 'CB-001', name: 'Ierahkwa Central Bank', code: 'ICB', country: 'Ierahkwa Ne Kanienke', currency: 'IGT', reserve: 1000000000000 },
  { id: 'CB-002', name: 'Federal Reserve Bank', code: 'FRB', country: 'United States', currency: 'USD', reserve: 500000000000 },
  { id: 'CB-003', name: 'European Central Bank', code: 'ECB', country: 'European Union', currency: 'EUR', reserve: 450000000000 },
  { id: 'CB-004', name: 'Bank of England', code: 'BOE', country: 'United Kingdom', currency: 'GBP', reserve: 300000000000 },
  { id: 'CB-005', name: 'Bank of Japan', code: 'BOJ', country: 'Japan', currency: 'JPY', reserve: 400000000000 },
  { id: 'CB-006', name: 'People\'s Bank of China', code: 'PBOC', country: 'China', currency: 'CNY', reserve: 600000000000 },
  { id: 'CB-007', name: 'Swiss National Bank', code: 'SNB', country: 'Switzerland', currency: 'CHF', reserve: 200000000000 },
  { id: 'CB-008', name: 'Reserve Bank of Australia', code: 'RBA', country: 'Australia', currency: 'AUD', reserve: 150000000000 },
  { id: 'CB-009', name: 'Bank of Canada', code: 'BOC', country: 'Canada', currency: 'CAD', reserve: 180000000000 },
  { id: 'CB-010', name: 'Banco de Mexico', code: 'BANXICO', country: 'Mexico', currency: 'MXN', reserve: 120000000000 }
];

// Futurehead Entities
const FUTUREHEAD_ENTITIES = [
  { 
    id: 'FH-BDET', 
    name: 'Ierahkwa Futurehead BDET Bank', 
    type: 'Primary Bank',
    code: 'BDET',
    description: 'Central Banking Entity for Ierahkwa Sovereign Blockchain',
    capital: 10000000000000,
    tier: 1
  },
  { 
    id: 'FH-GROUP', 
    name: 'Futurehead Group Holding', 
    type: 'Holding Company',
    code: 'FGH',
    description: 'Parent Holding Company for all Futurehead Subsidiaries',
    capital: 50000000000000,
    tier: 0
  },
  { 
    id: 'FH-INVEST', 
    name: 'Futurehead Investment Bank', 
    type: 'Investment Bank',
    code: 'FIB',
    description: 'Investment Banking and Asset Management',
    capital: 5000000000000,
    tier: 2
  },
  { 
    id: 'FH-DIGITAL', 
    name: 'Futurehead Digital Assets', 
    type: 'Digital Bank',
    code: 'FDA',
    description: 'Cryptocurrency and Digital Asset Management',
    capital: 2000000000000,
    tier: 2
  },
  { 
    id: 'FH-TRADE', 
    name: 'Futurehead Trading Corp', 
    type: 'Trading Firm',
    code: 'FTC',
    description: 'Proprietary Trading and Market Making',
    capital: 1000000000000,
    tier: 2
  }
];

// Governments
const GOVERNMENTS = [
  { id: 'GOV-001', name: 'Sovereign Government of Ierahkwa Ne Kanienke', code: 'SGINK', type: 'Sovereign', capital: 100000000000000 },
  { id: 'GOV-002', name: 'Haudenosaunee Confederacy', code: 'HC', type: 'Confederacy', capital: 50000000000000 },
  { id: 'GOV-003', name: 'Mohawk Nation Council', code: 'MNC', type: 'Nation', capital: 25000000000000 },
  { id: 'GOV-004', name: 'Oneida Nation', code: 'ON', type: 'Nation', capital: 20000000000000 },
  { id: 'GOV-005', name: 'Onondaga Nation', code: 'ONN', type: 'Nation', capital: 20000000000000 },
  { id: 'GOV-006', name: 'Cayuga Nation', code: 'CN', type: 'Nation', capital: 18000000000000 },
  { id: 'GOV-007', name: 'Seneca Nation', code: 'SN', type: 'Nation', capital: 22000000000000 },
  { id: 'GOV-008', name: 'Tuscarora Nation', code: 'TN', type: 'Nation', capital: 15000000000000 }
];

// ============================================
// INITIALIZE BANKING TABLES
// ============================================

function initBankingTables() {
  const data = db.get();
  const deptData = loadDepartments();
  
  // ========== CENTRAL BANK ACCOUNTS ==========
  if (!data.central_banks) {
    data.central_banks = CENTRAL_BANKS.map(cb => ({
      ...cb,
      // Bank Account
      bank_account: {
        account_number: `CB-BANK-${cb.code}-001`,
        type: 'Central Bank Reserve',
        igt_balance: cb.reserve,
        usd_balance: cb.reserve * 0.5,
        eur_balance: cb.reserve * 0.3,
        btc_balance: cb.reserve * 0.0001,
        gold_reserve_oz: cb.reserve * 0.00001,
        status: 'active'
      },
      // Exchange Account
      exchange_account: {
        account_number: `CB-EXCH-${cb.code}-001`,
        type: 'Central Bank Exchange',
        trading_limit_daily: cb.reserve * 0.1,
        forex_enabled: true,
        crypto_enabled: true,
        commodities_enabled: true,
        liquidity_pool: cb.reserve * 0.05,
        status: 'active'
      },
      // Trading Deck Account
      trading_deck: {
        account_number: `CB-TRADE-${cb.code}-001`,
        type: 'Central Bank Trading Desk',
        margin_available: cb.reserve * 0.2,
        leverage_limit: 10,
        open_positions: [],
        daily_pnl: 0,
        total_pnl: 0,
        risk_limit: cb.reserve * 0.05,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }

  // ========== FUTUREHEAD ENTITIES ==========
  if (!data.futurehead_entities) {
    data.futurehead_entities = FUTUREHEAD_ENTITIES.map(fh => ({
      ...fh,
      // Bank Account
      bank_account: {
        account_number: `FH-BANK-${fh.code}-001`,
        type: `${fh.type} Account`,
        igt_balance: fh.capital,
        usd_balance: fh.capital * 0.3,
        eur_balance: fh.capital * 0.1,
        btc_balance: fh.capital * 0.0002,
        eth_balance: fh.capital * 0.001,
        status: 'active'
      },
      // Exchange Account
      exchange_account: {
        account_number: `FH-EXCH-${fh.code}-001`,
        type: `${fh.type} Exchange`,
        trading_limit_daily: fh.capital * 0.2,
        forex_enabled: true,
        crypto_enabled: true,
        commodities_enabled: true,
        derivatives_enabled: fh.tier <= 1,
        liquidity_pool: fh.capital * 0.1,
        maker_fee: 0.001,
        taker_fee: 0.002,
        status: 'active'
      },
      // Trading Deck Account
      trading_deck: {
        account_number: `FH-TRADE-${fh.code}-001`,
        type: `${fh.type} Trading Desk`,
        margin_available: fh.capital * 0.5,
        leverage_limit: fh.tier === 0 ? 100 : fh.tier === 1 ? 50 : 20,
        open_positions: [],
        daily_pnl: 0,
        total_pnl: 0,
        risk_limit: fh.capital * 0.1,
        algo_trading_enabled: true,
        api_access: true,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }

  // ========== GOVERNMENT ACCOUNTS ==========
  if (!data.government_accounts) {
    data.government_accounts = GOVERNMENTS.map(gov => ({
      ...gov,
      // Bank Account
      bank_account: {
        account_number: `GOV-BANK-${gov.code}-001`,
        type: 'Government Treasury',
        igt_balance: gov.capital,
        usd_balance: gov.capital * 0.2,
        eur_balance: gov.capital * 0.1,
        btc_balance: gov.capital * 0.00005,
        gold_reserve_oz: gov.capital * 0.000001,
        sovereign_bonds: gov.capital * 0.3,
        status: 'active'
      },
      // Exchange Account
      exchange_account: {
        account_number: `GOV-EXCH-${gov.code}-001`,
        type: 'Government Exchange',
        trading_limit_daily: gov.capital * 0.05,
        forex_enabled: true,
        crypto_enabled: true,
        bond_trading_enabled: true,
        liquidity_pool: gov.capital * 0.02,
        status: 'active'
      },
      // Trading Deck Account
      trading_deck: {
        account_number: `GOV-TRADE-${gov.code}-001`,
        type: 'Government Trading Desk',
        margin_available: gov.capital * 0.1,
        leverage_limit: 5,
        open_positions: [],
        daily_pnl: 0,
        total_pnl: 0,
        risk_limit: gov.capital * 0.02,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }

  // ========== DEPARTMENT ACCOUNTS ==========
  if (!data.department_accounts) {
    data.department_accounts = deptData.departments.map(dept => ({
      id: dept.id,
      department_id: dept.id,
      department_name: dept.name,
      token_symbol: dept.symbol,
      category: dept.category,
      // Bank Account
      bank_account: {
        account_number: `DEPT-BANK-${dept.symbol}-001`,
        type: 'Department Account',
        igt_balance: 10000000000,
        usd_balance: 1000000,
        eur_balance: 500000,
        status: 'active'
      },
      // Exchange Account
      exchange_account: {
        account_number: `DEPT-EXCH-${dept.symbol}-001`,
        type: 'Department Exchange',
        trading_limit_daily: 100000000,
        forex_enabled: true,
        crypto_enabled: true,
        liquidity_pool: 10000000,
        status: 'active'
      },
      // Trading Deck Account
      trading_deck: {
        account_number: `DEPT-TRADE-${dept.symbol}-001`,
        type: 'Department Trading Desk',
        margin_available: 50000000,
        leverage_limit: 10,
        open_positions: [],
        daily_pnl: 0,
        total_pnl: 0,
        risk_limit: 10000000,
        status: 'active'
      },
      trading_enabled: true,
      exchange_enabled: true,
      created_at: new Date().toISOString(),
      status: 'active'
    }));
    data._counters.department_accounts = deptData.departments.length;
  }

  // ========== TOKEN REGISTRY ==========
  if (!data.token_registry) {
    const tokens = [];
    
    // Department tokens
    deptData.departments.forEach(dept => {
      tokens.push({
        symbol: dept.symbol,
        name: dept.name + ' Token',
        type: 'Department Token',
        department_id: dept.id,
        total_supply: 10000000000000,
        circulating_supply: 10000000000,
        decimals: 9,
        price_usd: 1.00,
        price_btc: 0.000025,
        market_cap: 10000000000,
        volume_24h: 0,
        change_24h: 0,
        is_active: true
      });
    });
    
    // Main IGT token
    tokens.unshift({
      symbol: 'IGT',
      name: 'Ierahkwa Government Token',
      type: 'Main Currency',
      total_supply: 1000000000000000,
      circulating_supply: 100000000000000,
      decimals: 9,
      price_usd: 1.00,
      price_btc: 0.000025,
      market_cap: 100000000000000,
      volume_24h: 0,
      change_24h: 0,
      is_active: true
    });
    
    // Futurehead tokens
    tokens.push({
      symbol: 'FHT',
      name: 'Futurehead Token',
      type: 'Corporate Token',
      total_supply: 100000000000000,
      circulating_supply: 10000000000000,
      decimals: 9,
      price_usd: 10.00,
      price_btc: 0.00025,
      market_cap: 100000000000000,
      volume_24h: 0,
      change_24h: 0,
      is_active: true
    });
    
    data.token_registry = tokens;
  }

  // ========== TRADING PAIRS ==========
  if (!data.trading_pairs) {
    const pairs = [];
    
    // Main pairs
    ['IGT', 'FHT'].forEach(base => {
      ['USD', 'EUR', 'BTC', 'ETH'].forEach(quote => {
        pairs.push({
          id: pairs.length + 1,
          base,
          quote,
          symbol: `${base}/${quote}`,
          price: base === 'FHT' ? 10.00 : 1.00,
          bid: base === 'FHT' ? 9.95 : 0.99,
          ask: base === 'FHT' ? 10.05 : 1.01,
          spread: 0.02,
          volume_24h: 0,
          is_active: true
        });
      });
    });
    
    // Department token pairs
    deptData.departments.forEach(dept => {
      pairs.push({
        id: pairs.length + 1,
        base: dept.symbol,
        quote: 'USD',
        symbol: `${dept.symbol}/USD`,
        price: 1.00,
        bid: 0.99,
        ask: 1.01,
        spread: 0.02,
        volume_24h: 0,
        is_active: true
      });
      pairs.push({
        id: pairs.length + 1,
        base: dept.symbol,
        quote: 'IGT',
        symbol: `${dept.symbol}/IGT`,
        price: 1.00,
        bid: 0.99,
        ask: 1.01,
        spread: 0.02,
        volume_24h: 0,
        is_active: true
      });
    });
    
    data.trading_pairs = pairs;
  }

  // ========== OTHER TABLES ==========
  if (!data.trade_orders) {
    data.trade_orders = [];
    data._counters.trade_orders = 0;
  }
  
  if (!data.trade_history) {
    data.trade_history = [];
    data._counters.trade_history = 0;
  }
  
  if (!data.bank_transactions) {
    data.bank_transactions = [];
    data._counters.bank_transactions = 0;
  }
  
  if (!data.exchange_rates) {
    data.exchange_rates = {
      IGT: 1,
      USD: 1,
      EUR: 1.08,
      GBP: 1.27,
      JPY: 0.0067,
      CHF: 1.13,
      CAD: 0.74,
      AUD: 0.65,
      CNY: 0.14,
      MXN: 0.058,
      BTC: 42000,
      ETH: 2500,
      FHT: 10,
      updated_at: new Date().toISOString()
    };
  }

  db.save();
}

export default async function bankingRoutes(fastify) {
  initBankingTables();

  // ==================== DASHBOARD ====================
  
  fastify.get('/api/banking/dashboard', async () => {
    const data = db.get();
    
    const centralBanks = data.central_banks || [];
    const futurehead = data.futurehead_entities || [];
    const governments = data.government_accounts || [];
    const departments = data.department_accounts || [];
    const tokens = data.token_registry || [];
    
    const totalIGT = [
      ...centralBanks.map(cb => cb.bank_account?.igt_balance || 0),
      ...futurehead.map(fh => fh.bank_account?.igt_balance || 0),
      ...governments.map(g => g.bank_account?.igt_balance || 0),
      ...departments.map(d => d.bank_account?.igt_balance || 0)
    ].reduce((s, v) => s + v, 0);
    
    const totalUSD = [
      ...centralBanks.map(cb => cb.bank_account?.usd_balance || 0),
      ...futurehead.map(fh => fh.bank_account?.usd_balance || 0),
      ...governments.map(g => g.bank_account?.usd_balance || 0),
      ...departments.map(d => d.bank_account?.usd_balance || 0)
    ].reduce((s, v) => s + v, 0);
    
    const totalMarketCap = tokens.reduce((s, t) => s + (t.market_cap || 0), 0);
    const totalVolume = (data.trade_history || [])
      .filter(t => t.created_at > new Date(Date.now() - 86400000).toISOString())
      .reduce((s, t) => s + (t.total || 0), 0);
    
    const recentTrades = (data.trade_history || []).slice(-10).reverse();
    const topTokens = [...tokens].sort((a, b) => b.volume_24h - a.volume_24h).slice(0, 10);
    
    return {
      stats: {
        central_banks: centralBanks.length,
        futurehead_entities: futurehead.length,
        governments: governments.length,
        departments: departments.length,
        total_entities: centralBanks.length + futurehead.length + governments.length + departments.length,
        total_igt_supply: totalIGT,
        total_usd_reserves: totalUSD,
        total_market_cap: totalMarketCap,
        volume_24h: totalVolume,
        active_pairs: (data.trading_pairs || []).filter(p => p.is_active).length,
        total_tokens: tokens.length
      },
      recent_trades: recentTrades,
      top_tokens: topTokens,
      exchange_rates: data.exchange_rates
    };
  });

  // ==================== CENTRAL BANKS ====================
  
  fastify.get('/api/banking/central-banks', async () => {
    const data = db.get();
    return data.central_banks || [];
  });

  fastify.get('/api/banking/central-banks/:id', async (req) => {
    const data = db.get();
    const bank = (data.central_banks || []).find(cb => cb.id === req.params.id);
    if (!bank) return { error: 'Central bank not found' };
    
    const transactions = (data.bank_transactions || [])
      .filter(t => t.entity_id === req.params.id)
      .slice(-50)
      .reverse();
    
    return { ...bank, transactions };
  });

  // ==================== FUTUREHEAD ENTITIES ====================
  
  fastify.get('/api/banking/futurehead', async () => {
    const data = db.get();
    return data.futurehead_entities || [];
  });

  fastify.get('/api/banking/futurehead/:id', async (req) => {
    const data = db.get();
    const entity = (data.futurehead_entities || []).find(fh => fh.id === req.params.id);
    if (!entity) return { error: 'Futurehead entity not found' };
    
    const transactions = (data.bank_transactions || [])
      .filter(t => t.entity_id === req.params.id)
      .slice(-50)
      .reverse();
    
    return { ...entity, transactions };
  });

  // ==================== GOVERNMENT ACCOUNTS ====================
  
  fastify.get('/api/banking/governments', async () => {
    const data = db.get();
    return data.government_accounts || [];
  });

  fastify.get('/api/banking/governments/:id', async (req) => {
    const data = db.get();
    const gov = (data.government_accounts || []).find(g => g.id === req.params.id);
    if (!gov) return { error: 'Government account not found' };
    
    const transactions = (data.bank_transactions || [])
      .filter(t => t.entity_id === req.params.id)
      .slice(-50)
      .reverse();
    
    return { ...gov, transactions };
  });

  // ==================== DEPARTMENT ACCOUNTS ====================
  
  fastify.get('/api/banking/accounts', async () => {
    const data = db.get();
    return data.department_accounts || [];
  });

  fastify.get('/api/banking/accounts/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const account = (data.department_accounts || []).find(a => a.id === id);
    if (!account) return { error: 'Account not found' };
    
    const transactions = (data.bank_transactions || [])
      .filter(t => t.account_id === id)
      .slice(-50)
      .reverse();
    
    const trades = (data.trade_history || [])
      .filter(t => t.account_id === id)
      .slice(-50)
      .reverse();
    
    return { ...account, transactions, trades };
  });

  // ==================== ALL ENTITIES (UNIFIED) ====================
  
  fastify.get('/api/banking/all-entities', async () => {
    const data = db.get();
    
    const entities = [
      ...(data.central_banks || []).map(cb => ({ 
        ...cb, 
        entity_type: 'central_bank',
        display_name: cb.name,
        igt_balance: cb.bank_account?.igt_balance || 0,
        usd_balance: cb.bank_account?.usd_balance || 0
      })),
      ...(data.futurehead_entities || []).map(fh => ({ 
        ...fh, 
        entity_type: 'futurehead',
        display_name: fh.name,
        igt_balance: fh.bank_account?.igt_balance || 0,
        usd_balance: fh.bank_account?.usd_balance || 0
      })),
      ...(data.government_accounts || []).map(g => ({ 
        ...g, 
        entity_type: 'government',
        display_name: g.name,
        igt_balance: g.bank_account?.igt_balance || 0,
        usd_balance: g.bank_account?.usd_balance || 0
      })),
      ...(data.department_accounts || []).map(d => ({ 
        ...d, 
        entity_type: 'department',
        display_name: d.department_name,
        igt_balance: d.bank_account?.igt_balance || 0,
        usd_balance: d.bank_account?.usd_balance || 0
      }))
    ];
    
    return entities;
  });

  // ==================== BANKING OPERATIONS ====================

  fastify.post('/api/banking/deposit', async (req) => {
    const data = db.get();
    const { entity_type, entity_id, account_type, currency, amount, reference } = req.body;
    
    let entity;
    if (entity_type === 'central_bank') {
      entity = (data.central_banks || []).find(e => e.id === entity_id);
    } else if (entity_type === 'futurehead') {
      entity = (data.futurehead_entities || []).find(e => e.id === entity_id);
    } else if (entity_type === 'government') {
      entity = (data.government_accounts || []).find(e => e.id === entity_id);
    } else if (entity_type === 'department') {
      entity = (data.department_accounts || []).find(e => e.id === parseInt(entity_id));
    }
    
    if (!entity) return { error: 'Entity not found' };
    
    const account = entity[account_type] || entity.bank_account;
    if (!account) return { error: 'Account not found' };
    
    const balanceKey = `${currency.toLowerCase()}_balance`;
    account[balanceKey] = (account[balanceKey] || 0) + parseFloat(amount);
    
    data.bank_transactions.push({
      id: db.nextId('bank_transactions'),
      entity_type,
      entity_id,
      entity_name: entity.name || entity.department_name,
      account_type: account_type || 'bank_account',
      type: 'deposit',
      currency,
      amount: parseFloat(amount),
      balance_after: account[balanceKey],
      reference: reference || '',
      created_at: db.now()
    });
    
    db.save();
    return { ok: true, new_balance: account[balanceKey] };
  });

  fastify.post('/api/banking/withdraw', async (req) => {
    const data = db.get();
    const { entity_type, entity_id, account_type, currency, amount, reference } = req.body;
    
    let entity;
    if (entity_type === 'central_bank') {
      entity = (data.central_banks || []).find(e => e.id === entity_id);
    } else if (entity_type === 'futurehead') {
      entity = (data.futurehead_entities || []).find(e => e.id === entity_id);
    } else if (entity_type === 'government') {
      entity = (data.government_accounts || []).find(e => e.id === entity_id);
    } else if (entity_type === 'department') {
      entity = (data.department_accounts || []).find(e => e.id === parseInt(entity_id));
    }
    
    if (!entity) return { error: 'Entity not found' };
    
    const account = entity[account_type] || entity.bank_account;
    if (!account) return { error: 'Account not found' };
    
    const balanceKey = `${currency.toLowerCase()}_balance`;
    if ((account[balanceKey] || 0) < amount) return { error: 'Insufficient balance' };
    
    account[balanceKey] -= parseFloat(amount);
    
    data.bank_transactions.push({
      id: db.nextId('bank_transactions'),
      entity_type,
      entity_id,
      entity_name: entity.name || entity.department_name,
      account_type: account_type || 'bank_account',
      type: 'withdrawal',
      currency,
      amount: -parseFloat(amount),
      balance_after: account[balanceKey],
      reference: reference || '',
      created_at: db.now()
    });
    
    db.save();
    return { ok: true, new_balance: account[balanceKey] };
  });

  fastify.post('/api/banking/transfer', async (req) => {
    const data = db.get();
    const { from_type, from_id, to_type, to_id, currency, amount, reference } = req.body;
    
    // Find source entity
    let fromEntity;
    if (from_type === 'central_bank') {
      fromEntity = (data.central_banks || []).find(e => e.id === from_id);
    } else if (from_type === 'futurehead') {
      fromEntity = (data.futurehead_entities || []).find(e => e.id === from_id);
    } else if (from_type === 'government') {
      fromEntity = (data.government_accounts || []).find(e => e.id === from_id);
    } else if (from_type === 'department') {
      fromEntity = (data.department_accounts || []).find(e => e.id === parseInt(from_id));
    }
    
    // Find destination entity
    let toEntity;
    if (to_type === 'central_bank') {
      toEntity = (data.central_banks || []).find(e => e.id === to_id);
    } else if (to_type === 'futurehead') {
      toEntity = (data.futurehead_entities || []).find(e => e.id === to_id);
    } else if (to_type === 'government') {
      toEntity = (data.government_accounts || []).find(e => e.id === to_id);
    } else if (to_type === 'department') {
      toEntity = (data.department_accounts || []).find(e => e.id === parseInt(to_id));
    }
    
    if (!fromEntity || !toEntity) return { error: 'Entity not found' };
    
    const balanceKey = `${currency.toLowerCase()}_balance`;
    const fromAccount = fromEntity.bank_account;
    const toAccount = toEntity.bank_account;
    
    if ((fromAccount[balanceKey] || 0) < amount) return { error: 'Insufficient balance' };
    
    fromAccount[balanceKey] -= parseFloat(amount);
    toAccount[balanceKey] = (toAccount[balanceKey] || 0) + parseFloat(amount);
    
    const txId = db.nextId('bank_transactions');
    
    data.bank_transactions.push({
      id: txId,
      entity_type: from_type,
      entity_id: from_id,
      entity_name: fromEntity.name || fromEntity.department_name,
      type: 'transfer_out',
      currency,
      amount: -parseFloat(amount),
      to_entity_type: to_type,
      to_entity_id: to_id,
      to_entity_name: toEntity.name || toEntity.department_name,
      balance_after: fromAccount[balanceKey],
      reference: reference || '',
      created_at: db.now()
    });
    
    data.bank_transactions.push({
      id: txId + 1,
      entity_type: to_type,
      entity_id: to_id,
      entity_name: toEntity.name || toEntity.department_name,
      type: 'transfer_in',
      currency,
      amount: parseFloat(amount),
      from_entity_type: from_type,
      from_entity_id: from_id,
      from_entity_name: fromEntity.name || fromEntity.department_name,
      balance_after: toAccount[balanceKey],
      reference: reference || '',
      created_at: db.now()
    });
    
    db.save();
    return { ok: true, transaction_id: txId };
  });

  // ==================== TOKEN REGISTRY ====================
  
  fastify.get('/api/banking/tokens', async () => {
    const data = db.get();
    return data.token_registry || [];
  });

  fastify.get('/api/banking/tokens/:symbol', async (req) => {
    const data = db.get();
    const token = (data.token_registry || []).find(t => t.symbol === req.params.symbol);
    if (!token) return { error: 'Token not found' };
    
    const trades = (data.trade_history || [])
      .filter(t => t.base === req.params.symbol || t.quote === req.params.symbol)
      .slice(-100)
      .reverse();
    
    return { ...token, recent_trades: trades };
  });

  // ==================== TRADING PAIRS ====================
  
  fastify.get('/api/trading/pairs', async () => {
    const data = db.get();
    return (data.trading_pairs || []).filter(p => p.is_active);
  });

  fastify.get('/api/trading/pairs/:symbol', async (req) => {
    const data = db.get();
    const pair = (data.trading_pairs || []).find(p => p.symbol === req.params.symbol);
    if (!pair) return { error: 'Pair not found' };
    
    const orders = (data.trade_orders || []).filter(o => 
      o.pair_symbol === req.params.symbol && o.status === 'open'
    );
    
    const bids = orders.filter(o => o.side === 'buy').sort((a, b) => b.price - a.price);
    const asks = orders.filter(o => o.side === 'sell').sort((a, b) => a.price - b.price);
    
    return { ...pair, order_book: { bids, asks } };
  });

  // ==================== TRADING ====================
  
  fastify.post('/api/trading/order', async (req) => {
    const data = db.get();
    const { entity_type, entity_id, pair_symbol, side, type, price, amount } = req.body;
    
    // Find entity
    let entity;
    if (entity_type === 'central_bank') {
      entity = (data.central_banks || []).find(e => e.id === entity_id);
    } else if (entity_type === 'futurehead') {
      entity = (data.futurehead_entities || []).find(e => e.id === entity_id);
    } else if (entity_type === 'government') {
      entity = (data.government_accounts || []).find(e => e.id === entity_id);
    } else if (entity_type === 'department') {
      entity = (data.department_accounts || []).find(e => e.id === parseInt(entity_id));
    }
    
    if (!entity) return { error: 'Entity not found' };
    
    const tradingDeck = entity.trading_deck;
    if (!tradingDeck || tradingDeck.status !== 'active') {
      return { error: 'Trading deck not active for this entity' };
    }
    
    const pair = (data.trading_pairs || []).find(p => p.symbol === pair_symbol);
    if (!pair) return { error: 'Trading pair not found' };
    
    const [base, quote] = pair_symbol.split('/');
    const orderPrice = type === 'market' ? pair.price : parseFloat(price);
    const orderAmount = parseFloat(amount);
    const total = orderPrice * orderAmount;
    
    const bankAccount = entity.bank_account;
    
    // Check balance
    if (side === 'buy') {
      const quoteBalance = bankAccount[`${quote.toLowerCase()}_balance`] || 0;
      if (quoteBalance < total) return { error: `Insufficient ${quote} balance` };
    } else {
      const baseBalance = bankAccount[`${base.toLowerCase()}_balance`] || bankAccount.igt_balance || 0;
      if (baseBalance < orderAmount) return { error: `Insufficient ${base} balance` };
    }
    
    const orderId = db.nextId('trade_orders');
    
    if (type === 'market') {
      // Execute immediately
      if (side === 'buy') {
        bankAccount[`${quote.toLowerCase()}_balance`] -= total;
        const baseKey = base === 'IGT' || base.startsWith('IGT-') ? 'igt_balance' : `${base.toLowerCase()}_balance`;
        bankAccount[baseKey] = (bankAccount[baseKey] || 0) + orderAmount;
      } else {
        const baseKey = base === 'IGT' || base.startsWith('IGT-') ? 'igt_balance' : `${base.toLowerCase()}_balance`;
        bankAccount[baseKey] -= orderAmount;
        bankAccount[`${quote.toLowerCase()}_balance`] = (bankAccount[`${quote.toLowerCase()}_balance`] || 0) + total;
      }
      
      pair.volume_24h = (pair.volume_24h || 0) + total;
      
      // Update trading deck PnL
      tradingDeck.daily_pnl = (tradingDeck.daily_pnl || 0) + (side === 'buy' ? -total : total);
      
      data.trade_history.push({
        id: db.nextId('trade_history'),
        entity_type,
        entity_id,
        entity_name: entity.name || entity.department_name,
        pair_symbol,
        base,
        quote,
        side,
        price: orderPrice,
        amount: orderAmount,
        total,
        fee: total * 0.001,
        status: 'filled',
        created_at: db.now()
      });
      
      db.save();
      return { ok: true, order_id: orderId, status: 'filled', executed_price: orderPrice };
    } else {
      // Limit order
      data.trade_orders.push({
        id: orderId,
        entity_type,
        entity_id,
        entity_name: entity.name || entity.department_name,
        pair_symbol,
        base,
        quote,
        side,
        type,
        price: orderPrice,
        amount: orderAmount,
        filled: 0,
        total,
        status: 'open',
        created_at: db.now()
      });
      
      db.save();
      return { ok: true, order_id: orderId, status: 'open' };
    }
  });

  fastify.delete('/api/trading/order/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const order = (data.trade_orders || []).find(o => o.id === id);
    
    if (!order) return { error: 'Order not found' };
    if (order.status !== 'open') return { error: 'Order cannot be cancelled' };
    
    order.status = 'cancelled';
    order.cancelled_at = db.now();
    
    db.save();
    return { ok: true };
  });

  fastify.get('/api/trading/orders', async (req) => {
    const data = db.get();
    let orders = data.trade_orders || [];
    
    if (req.query.entity_id) {
      orders = orders.filter(o => o.entity_id === req.query.entity_id);
    }
    if (req.query.status) {
      orders = orders.filter(o => o.status === req.query.status);
    }
    
    return orders.slice(-100).reverse();
  });

  fastify.get('/api/trading/history', async (req) => {
    const data = db.get();
    let trades = data.trade_history || [];
    
    if (req.query.entity_id) {
      trades = trades.filter(t => t.entity_id === req.query.entity_id);
    }
    if (req.query.pair) {
      trades = trades.filter(t => t.pair_symbol === req.query.pair);
    }
    
    return trades.slice(-100).reverse();
  });

  // ==================== EXCHANGE ====================
  
  fastify.post('/api/exchange/swap', async (req) => {
    const data = db.get();
    const { entity_type, entity_id, from_currency, to_currency, amount } = req.body;
    
    // Find entity
    let entity;
    if (entity_type === 'central_bank') {
      entity = (data.central_banks || []).find(e => e.id === entity_id);
    } else if (entity_type === 'futurehead') {
      entity = (data.futurehead_entities || []).find(e => e.id === entity_id);
    } else if (entity_type === 'government') {
      entity = (data.government_accounts || []).find(e => e.id === entity_id);
    } else if (entity_type === 'department') {
      entity = (data.department_accounts || []).find(e => e.id === parseInt(entity_id));
    }
    
    if (!entity) return { error: 'Entity not found' };
    
    const exchangeAccount = entity.exchange_account;
    if (!exchangeAccount || exchangeAccount.status !== 'active') {
      return { error: 'Exchange not enabled for this entity' };
    }
    
    const bankAccount = entity.bank_account;
    const fromBalance = bankAccount[`${from_currency.toLowerCase()}_balance`];
    if (fromBalance === undefined) return { error: 'Invalid source currency' };
    if (fromBalance < amount) return { error: 'Insufficient balance' };
    
    const rates = data.exchange_rates || {};
    const fromRate = rates[from_currency] || 1;
    const toRate = rates[to_currency] || 1;
    
    const usdValue = parseFloat(amount) * fromRate;
    const toAmount = usdValue / toRate;
    const fee = toAmount * (exchangeAccount.maker_fee || 0.005);
    const netAmount = toAmount - fee;
    
    bankAccount[`${from_currency.toLowerCase()}_balance`] -= parseFloat(amount);
    bankAccount[`${to_currency.toLowerCase()}_balance`] = (bankAccount[`${to_currency.toLowerCase()}_balance`] || 0) + netAmount;
    
    data.bank_transactions.push({
      id: db.nextId('bank_transactions'),
      entity_type,
      entity_id,
      entity_name: entity.name || entity.department_name,
      type: 'exchange',
      currency: from_currency,
      amount: -parseFloat(amount),
      to_currency,
      to_amount: netAmount,
      rate: toRate / fromRate,
      fee,
      created_at: db.now()
    });
    
    db.save();
    return { ok: true, from_amount: amount, to_amount: netAmount, fee, rate: toRate / fromRate };
  });

  fastify.get('/api/exchange/rates', async () => {
    const data = db.get();
    return data.exchange_rates || {};
  });

  fastify.put('/api/exchange/rates', async (req) => {
    const data = db.get();
    data.exchange_rates = { ...data.exchange_rates, ...req.body, updated_at: db.now() };
    db.save();
    return { ok: true, rates: data.exchange_rates };
  });

  // ==================== REPORTS ====================
  
  fastify.get('/api/banking/reports/summary', async () => {
    const data = db.get();
    
    const centralBanks = data.central_banks || [];
    const futurehead = data.futurehead_entities || [];
    const governments = data.government_accounts || [];
    const departments = data.department_accounts || [];
    
    return {
      central_banks: {
        count: centralBanks.length,
        total_igt: centralBanks.reduce((s, cb) => s + (cb.bank_account?.igt_balance || 0), 0),
        total_usd: centralBanks.reduce((s, cb) => s + (cb.bank_account?.usd_balance || 0), 0),
        entities: centralBanks.map(cb => ({
          id: cb.id,
          name: cb.name,
          code: cb.code,
          igt_balance: cb.bank_account?.igt_balance || 0,
          usd_balance: cb.bank_account?.usd_balance || 0
        }))
      },
      futurehead: {
        count: futurehead.length,
        total_igt: futurehead.reduce((s, fh) => s + (fh.bank_account?.igt_balance || 0), 0),
        total_usd: futurehead.reduce((s, fh) => s + (fh.bank_account?.usd_balance || 0), 0),
        entities: futurehead.map(fh => ({
          id: fh.id,
          name: fh.name,
          type: fh.type,
          igt_balance: fh.bank_account?.igt_balance || 0,
          usd_balance: fh.bank_account?.usd_balance || 0
        }))
      },
      governments: {
        count: governments.length,
        total_igt: governments.reduce((s, g) => s + (g.bank_account?.igt_balance || 0), 0),
        total_usd: governments.reduce((s, g) => s + (g.bank_account?.usd_balance || 0), 0),
        entities: governments.map(g => ({
          id: g.id,
          name: g.name,
          type: g.type,
          igt_balance: g.bank_account?.igt_balance || 0,
          usd_balance: g.bank_account?.usd_balance || 0
        }))
      },
      departments: {
        count: departments.length,
        total_igt: departments.reduce((s, d) => s + (d.bank_account?.igt_balance || 0), 0),
        total_usd: departments.reduce((s, d) => s + (d.bank_account?.usd_balance || 0), 0)
      }
    };
  });

  fastify.get('/api/banking/reports/transactions', async (req) => {
    const data = db.get();
    let transactions = data.bank_transactions || [];
    
    if (req.query.entity_type) {
      transactions = transactions.filter(t => t.entity_type === req.query.entity_type);
    }
    if (req.query.entity_id) {
      transactions = transactions.filter(t => t.entity_id === req.query.entity_id);
    }
    if (req.query.type) {
      transactions = transactions.filter(t => t.type === req.query.type);
    }
    if (req.query.start_date) {
      transactions = transactions.filter(t => t.created_at >= req.query.start_date);
    }
    if (req.query.end_date) {
      transactions = transactions.filter(t => t.created_at <= req.query.end_date + 'T23:59:59');
    }
    
    return transactions.slice(-500).reverse();
  });
}

/** 
 * ╔═══════════════════════════════════════════════════════════════════════════╗
 * ║      IERAHKWA GLOBAL BANKING & CLEARINGHOUSE SYSTEM                      ║
 * ║                                                                           ║
 * ║  Complete Banking Infrastructure:                                         ║
 * ║  • Central Banks       • Regional Banks      • National Banks            ║
 * ║  • Commercial Banks    • Fintech Banks       • Investment Banks          ║
 * ║  • Digital Banks       • Cooperative Banks   • Development Banks         ║
 * ║  • Clearinghouse       • Settlement System   • SWIFT/RTGS Alternative    ║
 * ║                                                                           ║
 * ║  Ierahkwa Futurehead BDET Bank - Primary Settlement Institution          ║
 * ║  Indigenous Settlement System (ISS)                                       ║
 * ║  Ierahkwa Sovereign Blockchain (ISB) • IGT Tokens                        ║
 * ╚═══════════════════════════════════════════════════════════════════════════╝
 */
import db from '../db.js';

// ============================================
// BANK TYPE DEFINITIONS
// ============================================

const BANK_TYPES = {
  CENTRAL: {
    code: 'CENTRAL',
    name: 'Central Bank',
    description: 'National monetary authority and lender of last resort',
    tier: 0,
    color: '#ef4444',
    capabilities: ['monetary_policy', 'currency_issue', 'reserves', 'lender_last_resort', 'clearing', 'settlement']
  },
  REGIONAL: {
    code: 'REGIONAL',
    name: 'Regional Bank',
    description: 'Regional monetary authority serving multiple territories',
    tier: 1,
    color: '#f97316',
    capabilities: ['regional_clearing', 'reserves', 'lending', 'deposits', 'forex']
  },
  NATIONAL: {
    code: 'NATIONAL',
    name: 'National Bank',
    description: 'Major national commercial and retail banking institution',
    tier: 1,
    color: '#eab308',
    capabilities: ['retail', 'commercial', 'lending', 'deposits', 'forex', 'trade_finance']
  },
  COMMERCIAL: {
    code: 'COMMERCIAL',
    name: 'Commercial Bank',
    description: 'Business and corporate banking services',
    tier: 2,
    color: '#22c55e',
    capabilities: ['commercial', 'lending', 'deposits', 'trade_finance', 'treasury']
  },
  INVESTMENT: {
    code: 'INVESTMENT',
    name: 'Investment Bank',
    description: 'Capital markets, M&A, securities trading',
    tier: 2,
    color: '#06b6d4',
    capabilities: ['investment', 'underwriting', 'trading', 'advisory', 'asset_management']
  },
  RETAIL: {
    code: 'RETAIL',
    name: 'Retail Bank',
    description: 'Consumer banking and personal financial services',
    tier: 2,
    color: '#8b5cf6',
    capabilities: ['retail', 'deposits', 'lending', 'cards', 'mortgages']
  },
  FINTECH: {
    code: 'FINTECH',
    name: 'Fintech Bank',
    description: 'Digital-first banking with innovative technology',
    tier: 2,
    color: '#ec4899',
    capabilities: ['digital', 'payments', 'lending', 'crypto', 'api_banking']
  },
  DIGITAL: {
    code: 'DIGITAL',
    name: 'Digital Bank',
    description: 'Fully digital banking without physical branches',
    tier: 2,
    color: '#14b8a6',
    capabilities: ['digital', 'mobile', 'payments', 'deposits', 'cards']
  },
  COOPERATIVE: {
    code: 'COOPERATIVE',
    name: 'Cooperative Bank',
    description: 'Member-owned financial cooperative',
    tier: 3,
    color: '#84cc16',
    capabilities: ['retail', 'deposits', 'lending', 'community']
  },
  DEVELOPMENT: {
    code: 'DEVELOPMENT',
    name: 'Development Bank',
    description: 'Long-term financing for economic development',
    tier: 1,
    color: '#0ea5e9',
    capabilities: ['development', 'project_finance', 'infrastructure', 'sovereign_lending']
  },
  PRIVATE: {
    code: 'PRIVATE',
    name: 'Private Bank',
    description: 'Wealth management and private banking services',
    tier: 2,
    color: '#a855f7',
    capabilities: ['wealth_management', 'private_banking', 'trust', 'estate']
  },
  ISLAMIC: {
    code: 'ISLAMIC',
    name: 'Islamic Bank',
    description: 'Sharia-compliant banking services',
    tier: 2,
    color: '#10b981',
    capabilities: ['islamic_finance', 'sukuk', 'murabaha', 'deposits']
  }
};

// ============================================
// REGIONAL BANKS DATA
// ============================================

const REGIONAL_BANKS = [
  { id: 'RB-NA01', name: 'North American Regional Bank', code: 'NARB', region: 'North America', headquarters: 'New York', currency: 'USD' },
  { id: 'RB-EU01', name: 'European Regional Bank', code: 'ERB', region: 'Europe', headquarters: 'Frankfurt', currency: 'EUR' },
  { id: 'RB-AS01', name: 'Asian Pacific Regional Bank', code: 'APRB', region: 'Asia Pacific', headquarters: 'Singapore', currency: 'SGD' },
  { id: 'RB-LA01', name: 'Latin American Regional Bank', code: 'LARB', region: 'Latin America', headquarters: 'São Paulo', currency: 'BRL' },
  { id: 'RB-AF01', name: 'African Regional Bank', code: 'ARB', region: 'Africa', headquarters: 'Johannesburg', currency: 'ZAR' },
  { id: 'RB-ME01', name: 'Middle East Regional Bank', code: 'MERB', region: 'Middle East', headquarters: 'Dubai', currency: 'AED' },
  { id: 'RB-IND01', name: 'Indigenous Nations Regional Bank', code: 'INRB', region: 'Indigenous Territories', headquarters: 'Akwesasne', currency: 'IGT' }
];

// ============================================
// NATIONAL BANKS DATA
// ============================================

const NATIONAL_BANKS = [
  { id: 'NB-US01', name: 'United States National Bank', code: 'USNB', country: 'United States', currency: 'USD' },
  { id: 'NB-CA01', name: 'Canada National Bank', code: 'CANB', country: 'Canada', currency: 'CAD' },
  { id: 'NB-UK01', name: 'United Kingdom National Bank', code: 'UKNB', country: 'United Kingdom', currency: 'GBP' },
  { id: 'NB-DE01', name: 'Germany National Bank', code: 'DENB', country: 'Germany', currency: 'EUR' },
  { id: 'NB-FR01', name: 'France National Bank', code: 'FRNB', country: 'France', currency: 'EUR' },
  { id: 'NB-JP01', name: 'Japan National Bank', code: 'JPNB', country: 'Japan', currency: 'JPY' },
  { id: 'NB-CN01', name: 'China National Bank', code: 'CNNB', country: 'China', currency: 'CNY' },
  { id: 'NB-AU01', name: 'Australia National Bank', code: 'AUNB', country: 'Australia', currency: 'AUD' },
  { id: 'NB-BR01', name: 'Brazil National Bank', code: 'BRNB', country: 'Brazil', currency: 'BRL' },
  { id: 'NB-IN01', name: 'India National Bank', code: 'INNB', country: 'India', currency: 'INR' },
  { id: 'NB-MX01', name: 'Mexico National Bank', code: 'MXNB', country: 'Mexico', currency: 'MXN' },
  { id: 'NB-CH01', name: 'Switzerland National Bank', code: 'CHNB', country: 'Switzerland', currency: 'CHF' }
];

// ============================================
// COMMERCIAL BANKS DATA
// ============================================

const COMMERCIAL_BANKS = [
  { id: 'CB-JP01', name: 'JPMorgan Chase Bank', code: 'JPMC', country: 'United States', type: 'COMMERCIAL' },
  { id: 'CB-BA01', name: 'Bank of America', code: 'BOFA', country: 'United States', type: 'COMMERCIAL' },
  { id: 'CB-CI01', name: 'Citibank Global', code: 'CITI', country: 'United States', type: 'COMMERCIAL' },
  { id: 'CB-WF01', name: 'Wells Fargo Bank', code: 'WFGO', country: 'United States', type: 'COMMERCIAL' },
  { id: 'CB-HS01', name: 'HSBC Holdings', code: 'HSBC', country: 'United Kingdom', type: 'COMMERCIAL' },
  { id: 'CB-BA02', name: 'Barclays Bank', code: 'BARC', country: 'United Kingdom', type: 'COMMERCIAL' },
  { id: 'CB-DB01', name: 'Deutsche Bank', code: 'DBAG', country: 'Germany', type: 'COMMERCIAL' },
  { id: 'CB-UB01', name: 'UBS Group', code: 'UBSG', country: 'Switzerland', type: 'COMMERCIAL' },
  { id: 'CB-CS01', name: 'Credit Suisse', code: 'CSGN', country: 'Switzerland', type: 'COMMERCIAL' },
  { id: 'CB-BN01', name: 'BNP Paribas', code: 'BNPP', country: 'France', type: 'COMMERCIAL' },
  { id: 'CB-SG01', name: 'Société Générale', code: 'SOCG', country: 'France', type: 'COMMERCIAL' },
  { id: 'CB-SN01', name: 'Santander Bank', code: 'SANT', country: 'Spain', type: 'COMMERCIAL' },
  { id: 'CB-IC01', name: 'ICBC - Industrial Commercial Bank of China', code: 'ICBC', country: 'China', type: 'COMMERCIAL' },
  { id: 'CB-BC01', name: 'Bank of China', code: 'BOCH', country: 'China', type: 'COMMERCIAL' },
  { id: 'CB-MU01', name: 'Mitsubishi UFJ Financial', code: 'MUFG', country: 'Japan', type: 'COMMERCIAL' },
  { id: 'CB-TD01', name: 'TD Bank', code: 'TDGB', country: 'Canada', type: 'COMMERCIAL' },
  { id: 'CB-RB01', name: 'Royal Bank of Canada', code: 'ROYL', country: 'Canada', type: 'COMMERCIAL' }
];

// ============================================
// FINTECH BANKS DATA
// ============================================

const FINTECH_BANKS = [
  { id: 'FT-RV01', name: 'Revolut', code: 'RVLT', country: 'United Kingdom', type: 'FINTECH', specialty: 'Multi-currency digital banking' },
  { id: 'FT-N201', name: 'N26 Bank', code: 'N26B', country: 'Germany', type: 'FINTECH', specialty: 'Mobile-first banking' },
  { id: 'FT-CH01', name: 'Chime Financial', code: 'CHIM', country: 'United States', type: 'FINTECH', specialty: 'Fee-free banking' },
  { id: 'FT-NB01', name: 'Nubank', code: 'NUBK', country: 'Brazil', type: 'FINTECH', specialty: 'Digital banking Latin America' },
  { id: 'FT-MZ01', name: 'Monzo Bank', code: 'MNZO', country: 'United Kingdom', type: 'FINTECH', specialty: 'Smart money management' },
  { id: 'FT-ST01', name: 'Starling Bank', code: 'STRL', country: 'United Kingdom', type: 'FINTECH', specialty: 'Business & personal banking' },
  { id: 'FT-WS01', name: 'Wise (TransferWise)', code: 'WISE', country: 'United Kingdom', type: 'FINTECH', specialty: 'International transfers' },
  { id: 'FT-SQ01', name: 'Square Financial', code: 'SQFI', country: 'United States', type: 'FINTECH', specialty: 'Merchant services & Cash App' },
  { id: 'FT-SP01', name: 'Stripe Treasury', code: 'STRP', country: 'United States', type: 'FINTECH', specialty: 'Embedded finance' },
  { id: 'FT-PL01', name: 'Plaid Financial', code: 'PLAD', country: 'United States', type: 'FINTECH', specialty: 'Banking API infrastructure' },
  { id: 'FT-CB01', name: 'Coinbase Bank', code: 'COIN', country: 'United States', type: 'FINTECH', specialty: 'Crypto banking' },
  { id: 'FT-KR01', name: 'Kraken Bank', code: 'KRKN', country: 'United States', type: 'FINTECH', specialty: 'Crypto custody & trading' }
];

// ============================================
// INVESTMENT BANKS DATA
// ============================================

const INVESTMENT_BANKS = [
  { id: 'IB-GS01', name: 'Goldman Sachs', code: 'GSAC', country: 'United States', type: 'INVESTMENT' },
  { id: 'IB-MS01', name: 'Morgan Stanley', code: 'MORG', country: 'United States', type: 'INVESTMENT' },
  { id: 'IB-BK01', name: 'BlackRock', code: 'BLCK', country: 'United States', type: 'INVESTMENT' },
  { id: 'IB-VG01', name: 'Vanguard Group', code: 'VNGD', country: 'United States', type: 'INVESTMENT' },
  { id: 'IB-FD01', name: 'Fidelity Investments', code: 'FIDE', country: 'United States', type: 'INVESTMENT' },
  { id: 'IB-LZ01', name: 'Lazard', code: 'LAZD', country: 'United States', type: 'INVESTMENT' },
  { id: 'IB-NM01', name: 'Nomura Holdings', code: 'NOMR', country: 'Japan', type: 'INVESTMENT' },
  { id: 'IB-MC01', name: 'Macquarie Group', code: 'MACQ', country: 'Australia', type: 'INVESTMENT' }
];

// ============================================
// CLEARINGHOUSE & SETTLEMENT
// ============================================

const CLEARINGHOUSE_CONFIG = {
  name: 'Ierahkwa Global Clearinghouse (IGC)',
  code: 'IGC',
  operator: 'Ierahkwa Futurehead BDET Bank',
  settlement_currency: 'IGT',
  backup_currencies: ['USD', 'EUR', 'BTC'],
  settlement_times: {
    T0: 'Real-time (instant)',
    T1: 'Next business day',
    T2: 'Two business days',
    T3: 'Three business days'
  },
  fees: {
    domestic: 0.0001,  // 0.01%
    regional: 0.0005,  // 0.05%
    international: 0.001, // 0.1%
    instant: 0.002     // 0.2%
  }
};

const SETTLEMENT_TYPES = {
  RTGS: { name: 'Real-Time Gross Settlement', code: 'RTGS', speed: 'T0', description: 'Instant final settlement' },
  ACH: { name: 'Automated Clearing House', code: 'ACH', speed: 'T1', description: 'Batch processing settlement' },
  WIRE: { name: 'Wire Transfer', code: 'WIRE', speed: 'T0', description: 'Direct bank-to-bank transfer' },
  SWIFT: { name: 'SWIFT Network', code: 'SWIFT', speed: 'T1-T3', description: 'International messaging & settlement' },
  ISS: { name: 'Indigenous Settlement System', code: 'ISS', speed: 'T0', description: 'Sovereign indigenous nation settlement' },
  CRYPTO: { name: 'Blockchain Settlement', code: 'CRYPTO', speed: 'T0', description: 'Cryptocurrency/token settlement' }
};

// ============================================
// INITIALIZE GLOBAL BANKING
// ============================================

function initGlobalBanking() {
  const data = db.get();
  
  // Bank Types Reference
  if (!data.bank_types) {
    data.bank_types = BANK_TYPES;
  }
  
  // Regional Banks
  if (!data.regional_banks) {
    data.regional_banks = REGIONAL_BANKS.map(bank => ({
      ...bank,
      type: 'REGIONAL',
      bank_account: {
        account_number: `RB-${bank.code}-001`,
        igt_balance: 100000000000,  // 100B
        usd_balance: 50000000000,
        eur_balance: 40000000000,
        reserve_ratio: 0.10,
        status: 'active'
      },
      exchange_account: {
        account_number: `RB-EXCH-${bank.code}-001`,
        trading_limit_daily: 10000000000,
        liquidity_pool: 5000000000,
        status: 'active'
      },
      trading_deck: {
        account_number: `RB-TRADE-${bank.code}-001`,
        margin_available: 20000000000,
        leverage_limit: 20,
        status: 'active'
      },
      clearing_account: {
        account_number: `RB-CLR-${bank.code}-001`,
        settlement_balance: 10000000000,
        pending_settlements: 0,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }
  
  // National Banks
  if (!data.national_banks) {
    data.national_banks = NATIONAL_BANKS.map(bank => ({
      ...bank,
      type: 'NATIONAL',
      bank_account: {
        account_number: `NB-${bank.code}-001`,
        igt_balance: 50000000000,
        usd_balance: 30000000000,
        local_currency_balance: 25000000000,
        reserve_ratio: 0.08,
        status: 'active'
      },
      exchange_account: {
        account_number: `NB-EXCH-${bank.code}-001`,
        trading_limit_daily: 5000000000,
        liquidity_pool: 2000000000,
        status: 'active'
      },
      trading_deck: {
        account_number: `NB-TRADE-${bank.code}-001`,
        margin_available: 10000000000,
        leverage_limit: 15,
        status: 'active'
      },
      clearing_account: {
        account_number: `NB-CLR-${bank.code}-001`,
        settlement_balance: 5000000000,
        pending_settlements: 0,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }
  
  // Commercial Banks
  if (!data.commercial_banks) {
    data.commercial_banks = COMMERCIAL_BANKS.map(bank => ({
      ...bank,
      bank_account: {
        account_number: `CB-${bank.code}-001`,
        igt_balance: 10000000000,
        usd_balance: 8000000000,
        eur_balance: 5000000000,
        reserve_ratio: 0.05,
        status: 'active'
      },
      exchange_account: {
        account_number: `CB-EXCH-${bank.code}-001`,
        trading_limit_daily: 2000000000,
        liquidity_pool: 500000000,
        status: 'active'
      },
      trading_deck: {
        account_number: `CB-TRADE-${bank.code}-001`,
        margin_available: 5000000000,
        leverage_limit: 10,
        status: 'active'
      },
      clearing_account: {
        account_number: `CB-CLR-${bank.code}-001`,
        settlement_balance: 1000000000,
        pending_settlements: 0,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }
  
  // Fintech Banks
  if (!data.fintech_banks) {
    data.fintech_banks = FINTECH_BANKS.map(bank => ({
      ...bank,
      bank_account: {
        account_number: `FT-${bank.code}-001`,
        igt_balance: 5000000000,
        usd_balance: 3000000000,
        btc_balance: 10000,
        eth_balance: 50000,
        status: 'active'
      },
      exchange_account: {
        account_number: `FT-EXCH-${bank.code}-001`,
        trading_limit_daily: 1000000000,
        liquidity_pool: 200000000,
        crypto_enabled: true,
        status: 'active'
      },
      trading_deck: {
        account_number: `FT-TRADE-${bank.code}-001`,
        margin_available: 2000000000,
        leverage_limit: 5,
        algo_trading_enabled: true,
        api_access: true,
        status: 'active'
      },
      clearing_account: {
        account_number: `FT-CLR-${bank.code}-001`,
        settlement_balance: 500000000,
        pending_settlements: 0,
        instant_settlement: true,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }
  
  // Investment Banks
  if (!data.investment_banks) {
    data.investment_banks = INVESTMENT_BANKS.map(bank => ({
      ...bank,
      bank_account: {
        account_number: `IB-${bank.code}-001`,
        igt_balance: 50000000000,
        usd_balance: 40000000000,
        assets_under_management: 500000000000,
        status: 'active'
      },
      exchange_account: {
        account_number: `IB-EXCH-${bank.code}-001`,
        trading_limit_daily: 20000000000,
        liquidity_pool: 10000000000,
        derivatives_enabled: true,
        status: 'active'
      },
      trading_deck: {
        account_number: `IB-TRADE-${bank.code}-001`,
        margin_available: 100000000000,
        leverage_limit: 50,
        algo_trading_enabled: true,
        hft_enabled: true,
        status: 'active'
      },
      clearing_account: {
        account_number: `IB-CLR-${bank.code}-001`,
        settlement_balance: 20000000000,
        pending_settlements: 0,
        status: 'active'
      },
      created_at: new Date().toISOString(),
      status: 'active'
    }));
  }
  
  // Clearinghouse
  if (!data.clearinghouse) {
    data.clearinghouse = {
      config: CLEARINGHOUSE_CONFIG,
      settlement_types: SETTLEMENT_TYPES,
      total_settled: 0,
      total_pending: 0,
      daily_volume: 0,
      participants: [],
      created_at: new Date().toISOString()
    };
  }
  
  // Settlement Queue
  if (!data.settlement_queue) {
    data.settlement_queue = [];
    data._counters.settlement_queue = 0;
  }
  
  // Settlement History
  if (!data.settlement_history) {
    data.settlement_history = [];
    data._counters.settlement_history = 0;
  }
  
  // Correspondent Banking Relationships
  if (!data.correspondent_banking) {
    data.correspondent_banking = [];
  }
  
  db.save();
}

export default async function globalBankingRoutes(fastify) {
  initGlobalBanking();

  // ==================== BANK TYPES ====================
  
  fastify.get('/api/global/bank-types', async () => {
    return BANK_TYPES;
  });

  // ==================== ALL BANKS ====================
  
  fastify.get('/api/global/banks', async (req) => {
    const data = db.get();
    const { type } = req.query;
    
    let banks = [];
    
    if (!type || type === 'all') {
      banks = [
        ...(data.central_banks || []).map(b => ({ ...b, bank_type: 'CENTRAL' })),
        ...(data.regional_banks || []).map(b => ({ ...b, bank_type: 'REGIONAL' })),
        ...(data.national_banks || []).map(b => ({ ...b, bank_type: 'NATIONAL' })),
        ...(data.commercial_banks || []).map(b => ({ ...b, bank_type: 'COMMERCIAL' })),
        ...(data.fintech_banks || []).map(b => ({ ...b, bank_type: 'FINTECH' })),
        ...(data.investment_banks || []).map(b => ({ ...b, bank_type: 'INVESTMENT' })),
        ...(data.futurehead_entities || []).map(b => ({ ...b, bank_type: 'FUTUREHEAD' }))
      ];
    } else {
      const typeMap = {
        'central': 'central_banks',
        'regional': 'regional_banks',
        'national': 'national_banks',
        'commercial': 'commercial_banks',
        'fintech': 'fintech_banks',
        'investment': 'investment_banks',
        'futurehead': 'futurehead_entities'
      };
      
      const tableKey = typeMap[type.toLowerCase()];
      if (tableKey && data[tableKey]) {
        banks = data[tableKey].map(b => ({ ...b, bank_type: type.toUpperCase() }));
      }
    }
    
    return banks;
  });

  // ==================== REGIONAL BANKS ====================
  
  fastify.get('/api/global/banks/regional', async () => {
    const data = db.get();
    return data.regional_banks || [];
  });

  fastify.get('/api/global/banks/regional/:id', async (req) => {
    const data = db.get();
    return (data.regional_banks || []).find(b => b.id === req.params.id) || { error: 'Not found' };
  });

  // ==================== NATIONAL BANKS ====================
  
  fastify.get('/api/global/banks/national', async () => {
    const data = db.get();
    return data.national_banks || [];
  });

  fastify.get('/api/global/banks/national/:id', async (req) => {
    const data = db.get();
    return (data.national_banks || []).find(b => b.id === req.params.id) || { error: 'Not found' };
  });

  // ==================== COMMERCIAL BANKS ====================
  
  fastify.get('/api/global/banks/commercial', async () => {
    const data = db.get();
    return data.commercial_banks || [];
  });

  fastify.get('/api/global/banks/commercial/:id', async (req) => {
    const data = db.get();
    return (data.commercial_banks || []).find(b => b.id === req.params.id) || { error: 'Not found' };
  });

  // ==================== FINTECH BANKS ====================
  
  fastify.get('/api/global/banks/fintech', async () => {
    const data = db.get();
    return data.fintech_banks || [];
  });

  fastify.get('/api/global/banks/fintech/:id', async (req) => {
    const data = db.get();
    return (data.fintech_banks || []).find(b => b.id === req.params.id) || { error: 'Not found' };
  });

  // ==================== INVESTMENT BANKS ====================
  
  fastify.get('/api/global/banks/investment', async () => {
    const data = db.get();
    return data.investment_banks || [];
  });

  fastify.get('/api/global/banks/investment/:id', async (req) => {
    const data = db.get();
    return (data.investment_banks || []).find(b => b.id === req.params.id) || { error: 'Not found' };
  });

  // ==================== CLEARINGHOUSE ====================
  
  fastify.get('/api/global/clearinghouse', async () => {
    const data = db.get();
    return data.clearinghouse || {};
  });

  fastify.get('/api/global/clearinghouse/config', async () => {
    return { config: CLEARINGHOUSE_CONFIG, settlement_types: SETTLEMENT_TYPES };
  });

  // ==================== SETTLEMENT ====================
  
  fastify.post('/api/global/settlement/initiate', async (req) => {
    const data = db.get();
    const { 
      sender_bank_type, sender_bank_id, 
      receiver_bank_type, receiver_bank_id,
      amount, currency, settlement_type, reference 
    } = req.body;
    
    // Validate banks exist
    const bankTables = {
      'CENTRAL': 'central_banks',
      'REGIONAL': 'regional_banks',
      'NATIONAL': 'national_banks',
      'COMMERCIAL': 'commercial_banks',
      'FINTECH': 'fintech_banks',
      'INVESTMENT': 'investment_banks',
      'FUTUREHEAD': 'futurehead_entities',
      'GOVERNMENT': 'government_accounts',
      'DEPARTMENT': 'department_accounts'
    };
    
    const senderTable = bankTables[sender_bank_type];
    const receiverTable = bankTables[receiver_bank_type];
    
    if (!senderTable || !receiverTable) return { error: 'Invalid bank type' };
    
    const sender = (data[senderTable] || []).find(b => b.id === sender_bank_id || b.id === parseInt(sender_bank_id));
    const receiver = (data[receiverTable] || []).find(b => b.id === receiver_bank_id || b.id === parseInt(receiver_bank_id));
    
    if (!sender || !receiver) return { error: 'Bank not found' };
    
    const settleType = SETTLEMENT_TYPES[settlement_type] || SETTLEMENT_TYPES.RTGS;
    const fee = amount * (CLEARINGHOUSE_CONFIG.fees.international || 0.001);
    
    const settlementId = db.nextId('settlement_queue');
    const settlement = {
      id: settlementId,
      reference: reference || `STL-${settlementId}-${Date.now()}`,
      sender: {
        bank_type: sender_bank_type,
        bank_id: sender_bank_id,
        bank_name: sender.name || sender.department_name,
        bank_code: sender.code || sender.token_symbol
      },
      receiver: {
        bank_type: receiver_bank_type,
        bank_id: receiver_bank_id,
        bank_name: receiver.name || receiver.department_name,
        bank_code: receiver.code || receiver.token_symbol
      },
      amount: parseFloat(amount),
      currency,
      fee,
      net_amount: parseFloat(amount) - fee,
      settlement_type: settleType.code,
      settlement_name: settleType.name,
      speed: settleType.speed,
      status: settleType.speed === 'T0' ? 'processing' : 'pending',
      initiated_at: db.now(),
      expected_completion: settleType.speed
    };
    
    // For instant settlement (T0), process immediately
    if (settleType.speed === 'T0') {
      const senderAccount = sender.bank_account || sender;
      const receiverAccount = receiver.bank_account || receiver;
      const balanceKey = `${currency.toLowerCase()}_balance`;
      
      if ((senderAccount[balanceKey] || 0) < amount) {
        return { error: 'Insufficient funds' };
      }
      
      senderAccount[balanceKey] = (senderAccount[balanceKey] || 0) - parseFloat(amount);
      receiverAccount[balanceKey] = (receiverAccount[balanceKey] || 0) + (parseFloat(amount) - fee);
      
      settlement.status = 'completed';
      settlement.completed_at = db.now();
      
      data.settlement_history.push(settlement);
      data.clearinghouse.total_settled += parseFloat(amount);
      data.clearinghouse.daily_volume += parseFloat(amount);
    } else {
      data.settlement_queue.push(settlement);
      data.clearinghouse.total_pending += parseFloat(amount);
    }
    
    db.save();
    return { ok: true, settlement };
  });

  fastify.get('/api/global/settlement/queue', async () => {
    const data = db.get();
    return (data.settlement_queue || []).filter(s => s.status !== 'completed');
  });

  fastify.get('/api/global/settlement/history', async (req) => {
    const data = db.get();
    let history = data.settlement_history || [];
    
    if (req.query.bank_id) {
      history = history.filter(s => 
        s.sender.bank_id === req.query.bank_id || 
        s.receiver.bank_id === req.query.bank_id
      );
    }
    
    return history.slice(-100).reverse();
  });

  fastify.post('/api/global/settlement/process/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    
    const settlement = (data.settlement_queue || []).find(s => s.id === id);
    if (!settlement) return { error: 'Settlement not found' };
    if (settlement.status === 'completed') return { error: 'Already completed' };
    
    const bankTables = {
      'CENTRAL': 'central_banks',
      'REGIONAL': 'regional_banks',
      'NATIONAL': 'national_banks',
      'COMMERCIAL': 'commercial_banks',
      'FINTECH': 'fintech_banks',
      'INVESTMENT': 'investment_banks',
      'FUTUREHEAD': 'futurehead_entities',
      'GOVERNMENT': 'government_accounts',
      'DEPARTMENT': 'department_accounts'
    };
    
    const senderTable = bankTables[settlement.sender.bank_type];
    const receiverTable = bankTables[settlement.receiver.bank_type];
    
    const sender = (data[senderTable] || []).find(b => b.id === settlement.sender.bank_id || b.id === parseInt(settlement.sender.bank_id));
    const receiver = (data[receiverTable] || []).find(b => b.id === settlement.receiver.bank_id || b.id === parseInt(settlement.receiver.bank_id));
    
    if (!sender || !receiver) return { error: 'Bank not found' };
    
    const senderAccount = sender.bank_account || sender;
    const receiverAccount = receiver.bank_account || receiver;
    const balanceKey = `${settlement.currency.toLowerCase()}_balance`;
    
    if ((senderAccount[balanceKey] || 0) < settlement.amount) {
      settlement.status = 'failed';
      settlement.failed_reason = 'Insufficient funds';
      db.save();
      return { error: 'Insufficient funds' };
    }
    
    senderAccount[balanceKey] -= settlement.amount;
    receiverAccount[balanceKey] = (receiverAccount[balanceKey] || 0) + settlement.net_amount;
    
    settlement.status = 'completed';
    settlement.completed_at = db.now();
    
    // Move to history
    data.settlement_history.push(settlement);
    data.settlement_queue = data.settlement_queue.filter(s => s.id !== id);
    
    data.clearinghouse.total_settled += settlement.amount;
    data.clearinghouse.total_pending -= settlement.amount;
    data.clearinghouse.daily_volume += settlement.amount;
    
    db.save();
    return { ok: true, settlement };
  });

  // ==================== INDIGENOUS SETTLEMENT SYSTEM (ISS) ====================
  
  fastify.post('/api/global/iss/transfer', async (req) => {
    const data = db.get();
    const { from_nation, to_nation, amount, purpose, treaty_reference } = req.body;
    
    // ISS is designed for instant settlement between indigenous nations
    const settlement = {
      id: db.nextId('settlement_history'),
      type: 'ISS',
      from_nation,
      to_nation,
      amount: parseFloat(amount),
      currency: 'IGT',
      fee: 0, // No fees for indigenous nations
      purpose,
      treaty_reference,
      status: 'completed',
      created_at: db.now(),
      completed_at: db.now()
    };
    
    data.settlement_history.push(settlement);
    data.clearinghouse.total_settled += parseFloat(amount);
    
    db.save();
    return { ok: true, settlement };
  });

  // ==================== DASHBOARD ====================
  
  fastify.get('/api/global/dashboard', async () => {
    const data = db.get();
    
    const bankCounts = {
      central: (data.central_banks || []).length,
      regional: (data.regional_banks || []).length,
      national: (data.national_banks || []).length,
      commercial: (data.commercial_banks || []).length,
      fintech: (data.fintech_banks || []).length,
      investment: (data.investment_banks || []).length,
      futurehead: (data.futurehead_entities || []).length,
      governments: (data.government_accounts || []).length,
      departments: (data.department_accounts || []).length
    };
    
    const totalBanks = Object.values(bankCounts).reduce((s, v) => s + v, 0);
    
    const clearinghouse = data.clearinghouse || {};
    
    const recentSettlements = (data.settlement_history || []).slice(-10).reverse();
    const pendingSettlements = (data.settlement_queue || []).filter(s => s.status === 'pending');
    
    return {
      stats: {
        total_banks: totalBanks,
        bank_counts: bankCounts,
        total_settled: clearinghouse.total_settled || 0,
        total_pending: clearinghouse.total_pending || 0,
        daily_volume: clearinghouse.daily_volume || 0,
        pending_count: pendingSettlements.length
      },
      bank_types: Object.values(BANK_TYPES),
      clearinghouse_config: CLEARINGHOUSE_CONFIG,
      settlement_types: SETTLEMENT_TYPES,
      recent_settlements: recentSettlements,
      pending_settlements: pendingSettlements
    };
  });

  // ==================== CORRESPONDENT BANKING ====================
  
  fastify.post('/api/global/correspondent/establish', async (req) => {
    const data = db.get();
    const { bank1_type, bank1_id, bank2_type, bank2_id, relationship_type, credit_line } = req.body;
    
    const relationship = {
      id: (data.correspondent_banking || []).length + 1,
      bank1: { type: bank1_type, id: bank1_id },
      bank2: { type: bank2_type, id: bank2_id },
      relationship_type: relationship_type || 'bilateral',
      credit_line: parseFloat(credit_line) || 0,
      status: 'active',
      established_at: db.now()
    };
    
    data.correspondent_banking.push(relationship);
    db.save();
    
    return { ok: true, relationship };
  });

  fastify.get('/api/global/correspondent', async () => {
    const data = db.get();
    return data.correspondent_banking || [];
  });
}

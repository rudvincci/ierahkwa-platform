/** 
 * ╔═══════════════════════════════════════════════════════════════════════════╗
 * ║      IERAHKWA FUTUREHEAD SERVICES REGISTRY                               ║
 * ║                                                                           ║
 * ║  101 Modules • 40 Departments • 12 Finance • 49 Services                 ║
 * ║                                                                           ║
 * ║  Unified Service Discovery & Management                                   ║
 * ║  Ierahkwa Sovereign Blockchain (ISB) • IGT Tokens                        ║
 * ╚═══════════════════════════════════════════════════════════════════════════╝
 */
import db from '../db.js';

// ============================================
// COMPLETE MODULE REGISTRY (101 MODULES)
// ============================================

const GOVERNMENT_DEPARTMENTS = [
  { id: '01', symbol: 'IGT-PM', name: 'Office of the Prime Minister', category: 'executive', icon: 'star', status: 'active' },
  { id: '02', symbol: 'IGT-MFA', name: 'Ministry of Foreign Affairs', category: 'executive', icon: 'globe2', status: 'active' },
  { id: '03', symbol: 'IGT-MFT', name: 'Ministry of Finance & Treasury', category: 'finance', icon: 'cash-stack', status: 'active' },
  { id: '04', symbol: 'IGT-MJ', name: 'Ministry of Justice', category: 'justice', icon: 'balance-scale', status: 'active' },
  { id: '05', symbol: 'IGT-MI', name: 'Ministry of Interior', category: 'executive', icon: 'house', status: 'active' },
  { id: '06', symbol: 'IGT-MD', name: 'Ministry of Defense', category: 'security', icon: 'shield', status: 'active' },
  { id: '07', symbol: 'IGT-BDET', name: 'Ierahkwa Futurehead BDET Bank', category: 'finance', icon: 'bank', status: 'active' },
  { id: '08', symbol: 'IGT-NT', name: 'National Treasury', category: 'finance', icon: 'safe', status: 'active' },
  { id: '09', symbol: 'IGT-AG', name: 'Attorney General Office', category: 'justice', icon: 'briefcase', status: 'active' },
  { id: '10', symbol: 'IGT-SC', name: 'Supreme Court', category: 'justice', icon: 'hammer', status: 'active' },
  { id: '11', symbol: 'IGT-MH', name: 'Ministry of Health', category: 'social', icon: 'heart-pulse', status: 'active' },
  { id: '12', symbol: 'IGT-ME', name: 'Ministry of Education', category: 'social', icon: 'mortarboard', status: 'active' },
  { id: '13', symbol: 'IGT-MLE', name: 'Ministry of Labor & Employment', category: 'economy', icon: 'people', status: 'active' },
  { id: '14', symbol: 'IGT-MSD', name: 'Ministry of Social Development', category: 'social', icon: 'people-fill', status: 'active' },
  { id: '15', symbol: 'IGT-MHO', name: 'Ministry of Housing', category: 'infrastructure', icon: 'buildings', status: 'active' },
  { id: '16', symbol: 'IGT-MCH', name: 'Ministry of Culture & Heritage', category: 'culture', icon: 'palette', status: 'active' },
  { id: '17', symbol: 'IGT-MSR', name: 'Ministry of Sports & Recreation', category: 'culture', icon: 'trophy', status: 'active' },
  { id: '18', symbol: 'IGT-MFC', name: 'Ministry of Family & Children', category: 'social', icon: 'heart', status: 'active' },
  { id: '19', symbol: 'IGT-SSA', name: 'Social Security Administration', category: 'social', icon: 'shield-check', status: 'active' },
  { id: '20', symbol: 'IGT-PHS', name: 'Public Health Service', category: 'social', icon: 'hospital', status: 'active' },
  { id: '21', symbol: 'IGT-MA', name: 'Ministry of Agriculture', category: 'economy', icon: 'tree', status: 'active' },
  { id: '22', symbol: 'IGT-MEN', name: 'Ministry of Environment', category: 'environment', icon: 'flower1', status: 'active' },
  { id: '23', symbol: 'IGT-MEG', name: 'Ministry of Energy', category: 'infrastructure', icon: 'lightning', status: 'active' },
  { id: '24', symbol: 'IGT-MMR', name: 'Ministry of Mining & Resources', category: 'economy', icon: 'gem', status: 'active' },
  { id: '25', symbol: 'IGT-MCT', name: 'Ministry of Commerce & Trade', category: 'economy', icon: 'shop', status: 'active' },
  { id: '26', symbol: 'IGT-MIN', name: 'Ministry of Industry', category: 'economy', icon: 'building', status: 'active' },
  { id: '27', symbol: 'IGT-MT', name: 'Ministry of Tourism', category: 'economy', icon: 'airplane', status: 'active' },
  { id: '28', symbol: 'IGT-MTR', name: 'Ministry of Transportation', category: 'infrastructure', icon: 'truck', status: 'active' },
  { id: '29', symbol: 'IGT-MST', name: 'Ministry of Science & Technology', category: 'technology', icon: 'cpu', status: 'active' },
  { id: '30', symbol: 'IGT-MC', name: 'Ministry of Communications', category: 'technology', icon: 'broadcast', status: 'active' },
  { id: '31', symbol: 'IGT-NPS', name: 'National Police Service', category: 'security', icon: 'shield-shaded', status: 'active' },
  { id: '32', symbol: 'IGT-AFI', name: 'Armed Forces of Ierahkwa', category: 'security', icon: 'flag', status: 'active' },
  { id: '33', symbol: 'IGT-NIS', name: 'National Intelligence Service', category: 'security', icon: 'eye', status: 'active' },
  { id: '34', symbol: 'IGT-CBP', name: 'Customs & Border Protection', category: 'security', icon: 'sign-stop', status: 'active' },
  { id: '35', symbol: 'IGT-CRO', name: 'Civil Registry Office', category: 'admin', icon: 'person-badge', status: 'active' },
  { id: '36', symbol: 'IGT-EC', name: 'Electoral Commission', category: 'admin', icon: 'check2-square', status: 'active' },
  { id: '37', symbol: 'IGT-OCG', name: 'Office of Comptroller General', category: 'oversight', icon: 'clipboard-check', status: 'active' },
  { id: '38', symbol: 'IGT-OO', name: 'Ombudsman Office', category: 'oversight', icon: 'megaphone', status: 'active' },
  { id: '39', symbol: 'IGT-NA', name: 'National Archives', category: 'admin', icon: 'archive', status: 'active' },
  { id: '40', symbol: 'IGT-PSI', name: 'Postal Service of Ierahkwa', category: 'services', icon: 'envelope', status: 'active' }
];

const FINANCE_TOKENS = [
  { id: '41', symbol: 'IGT-MAIN', name: 'Ierahkwa Main Currency', type: 'currency', status: 'active' },
  { id: '42', symbol: 'IGT-STABLE', name: 'Ierahkwa Stablecoin', type: 'stablecoin', status: 'active' },
  { id: '43', symbol: 'IGT-GOV', name: 'Governance Token', type: 'governance', status: 'active' },
  { id: '44', symbol: 'IGT-STAKE', name: 'Staking Token', type: 'utility', status: 'active' },
  { id: '45', symbol: 'IGT-LIQ', name: 'Liquidity Token', type: 'defi', status: 'active' },
  { id: '46', symbol: 'IGT-REWARD', name: 'Rewards Token', type: 'utility', status: 'active' },
  { id: '47', symbol: 'IGT-FEE', name: 'Fee Token', type: 'utility', status: 'active' },
  { id: '48', symbol: 'IGT-BRIDGE', name: 'Bridge Token', type: 'utility', status: 'active' },
  { id: '49', symbol: 'IGT-RESERVE', name: 'Reserve Token', type: 'reserve', status: 'active' },
  { id: '50', symbol: 'IGT-TRADE', name: 'Trade Token', type: 'utility', status: 'active' },
  { id: '51', symbol: 'IGT-DEFI', name: 'DeFi Token', type: 'defi', status: 'active' },
  { id: '52', symbol: 'IGT-ASSET', name: 'Asset Token', type: 'asset', status: 'active' }
];

const FUTUREHEAD_SERVICES = [
  { id: '53', symbol: 'IGT-EXCHANGE', name: 'Ierahkwa Exchange', url: '/trading', port: 3100, status: 'active' },
  { id: '54', symbol: 'IGT-TRADING', name: 'Trading Deck', url: '/trading', port: 3100, status: 'active' },
  { id: '55', symbol: 'IGT-CASINO', name: 'Ierahkwa Casino', url: null, status: 'planned' },
  { id: '56', symbol: 'IGT-SOCIAL', name: 'Social Network', url: '/chat', port: 3100, status: 'active' },
  { id: '57', symbol: 'IGT-LOTTO', name: 'Lottery', url: null, status: 'planned' },
  { id: '58', symbol: 'IGT-GLOBAL', name: 'Global Services', url: '/global-banking', port: 3100, status: 'active' },
  { id: '59', symbol: 'IGT-NET', name: 'Futurehead Network', url: '/node', port: 3100, status: 'active' },
  { id: '60', symbol: 'IGT-SWIFT', name: 'SWIFT Gateway', url: '/global-banking', port: 3100, status: 'active' },
  { id: '61', symbol: 'IGT-CLEAR', name: 'Clearinghouse', url: '/global-banking', port: 3100, status: 'active' },
  { id: '62', symbol: 'IGT-PAY', name: 'Payment Gateway', url: '/pos', port: 3100, status: 'active' },
  { id: '63', symbol: 'IGT-WALLET', name: 'Digital Wallet', url: null, status: 'planned' },
  { id: '64', symbol: 'IGT-INSURANCE', name: 'Insurance', url: null, status: 'planned' },
  { id: '65', symbol: 'IGT-LOANS', name: 'Loans', url: null, status: 'planned' },
  { id: '66', symbol: 'IGT-MARKET', name: 'Marketplace', url: '/', port: 3100, status: 'active' },
  { id: '67', symbol: 'IGT-HEALTH', name: 'Health Services', url: null, status: 'planned' },
  { id: '68', symbol: 'IGT-EDU', name: 'Education Platform', url: null, port: null, status: 'development' },
  { id: '69', symbol: 'IGT-TRAVEL', name: 'Travel', url: null, status: 'planned' },
  { id: '70', symbol: 'IGT-SHIP', name: 'Shipping', url: null, status: 'planned' },
  { id: '71', symbol: 'IGT-CLOUD', name: 'Cloud Services', url: null, status: 'planned' },
  { id: '72', symbol: 'IGT-AI', name: 'AI Services', url: null, status: 'planned' },
  { id: '73', symbol: 'IGT-VPN', name: 'VPN', url: null, status: 'planned' },
  { id: '74', symbol: 'IGT-STREAM', name: 'Streaming', url: null, status: 'planned' },
  { id: '75', symbol: 'IGT-GAMING', name: 'Gaming', url: null, status: 'planned' },
  { id: '76', symbol: 'IGT-MUSIC', name: 'Music', url: null, status: 'planned' },
  { id: '77', symbol: 'IGT-NEWS', name: 'News', url: null, status: 'planned' },
  { id: '78', symbol: 'IGT-SPORTS', name: 'Sports', url: null, status: 'planned' },
  { id: '79', symbol: 'IGT-REALTY', name: 'Real Estate', url: null, status: 'planned' },
  { id: '80', symbol: 'IGT-AUTO', name: 'Auto', url: null, status: 'planned' },
  { id: '81', symbol: 'IGT-ENERGY', name: 'Energy', url: null, status: 'planned' },
  { id: '82', symbol: 'IGT-TELECOM', name: 'Telecom', url: null, status: 'planned' },
  { id: '83', symbol: 'IGT-MAIL', name: 'Mail', url: null, status: 'planned' },
  { id: '84', symbol: 'IGT-FOOD', name: 'Food Delivery', url: null, status: 'planned' },
  { id: '85', symbol: 'IGT-RIDE', name: 'Ride Sharing', url: null, status: 'planned' },
  { id: '86', symbol: 'IGT-JOBS', name: 'Jobs', url: null, status: 'planned' },
  { id: '87', symbol: 'IGT-DATING', name: 'Dating', url: null, status: 'planned' },
  { id: '88', symbol: 'IGT-HOTEL', name: 'Hotels', url: null, status: 'planned' },
  { id: '89', symbol: 'IGT-FLIGHTS', name: 'Flights', url: null, status: 'planned' },
  { id: '90', symbol: 'IGT-LEGAL', name: 'Legal Services', url: null, status: 'planned' },
  { id: '91', symbol: 'IGT-ID', name: 'Digital ID', url: null, status: 'planned' },
  { id: '92', symbol: 'IGT-VOTE', name: 'Voting', url: null, status: 'planned' },
  { id: '93', symbol: 'IGT-CHARITY', name: 'Charity', url: '/monetary', port: 3100, status: 'active' },
  { id: '94', symbol: 'IGT-CROWDFUND', name: 'Crowdfunding', url: null, status: 'planned' },
  { id: '95', symbol: 'IGT-METAVERSE', name: 'Metaverse', url: null, status: 'planned' },
  { id: '96', symbol: 'IGT-NFT', name: 'NFT Marketplace', url: null, status: 'planned' },
  { id: '97', symbol: 'IGT-LAUNCHPAD', name: 'Launchpad', url: null, status: 'planned' },
  { id: '98', symbol: 'IGT-DAO', name: 'DAO', url: null, status: 'planned' },
  { id: '99', symbol: 'IGT-ORACLE', name: 'Oracle', url: null, status: 'planned' },
  { id: '100', symbol: 'IGT-SOVEREIGN', name: 'Sovereign Master Token', url: null, status: 'active' },
  { id: '101', symbol: 'IGT-IISB', name: 'International Settlement Bank', url: '/global-banking', port: 3100, status: 'active' }
];

// ============================================
// EXTERNAL SERVICES (From workspace)
// ============================================

const EXTERNAL_SERVICES = [
  { id: 'forex-trading', name: 'Forex Trading Server', path: 'forex-trading-server', port: null, status: 'available' },
  { id: 'pos-system', name: 'POS System', path: 'pos-system', port: null, status: 'available' },
  { id: 'inventory-system', name: 'Inventory System', path: 'inventory-system', port: null, status: 'available' },
  { id: 'smart-school', name: 'SmartSchool Node', path: 'smart-school-node', port: null, status: 'available' },
  { id: 'image-upload', name: 'Image Upload', path: 'image-upload', port: 3500, status: 'available' },
  { id: 'ierahkwa-bank', name: 'Ierahkwa Bank Platform (.NET)', path: 'IerahkwaBankPlatform', port: null, status: 'available' },
  { id: 'inventory-manager', name: 'Inventory Manager (.NET)', path: 'InventoryManager', port: null, status: 'available' },
  { id: 'smart-school-net', name: 'SmartSchool (.NET)', path: 'SmartSchool', port: null, status: 'available' },
  { id: 'tradex', name: 'TradeX (.NET)', path: 'TradeX', port: null, status: 'available' }
];

export default async function servicesRoutes(fastify) {

  // ==================== ALL MODULES ====================
  
  fastify.get('/api/services/modules', async () => {
    return {
      total: 101,
      government: { count: 40, modules: GOVERNMENT_DEPARTMENTS },
      finance: { count: 12, modules: FINANCE_TOKENS },
      services: { count: 49, modules: FUTUREHEAD_SERVICES }
    };
  });

  // ==================== GOVERNMENT DEPARTMENTS ====================
  
  fastify.get('/api/services/departments', async () => {
    return GOVERNMENT_DEPARTMENTS;
  });

  fastify.get('/api/services/departments/:symbol', async (req) => {
    return GOVERNMENT_DEPARTMENTS.find(d => d.symbol === req.params.symbol) || { error: 'Not found' };
  });

  // ==================== FINANCE TOKENS ====================
  
  fastify.get('/api/services/finance-tokens', async () => {
    return FINANCE_TOKENS;
  });

  // ==================== FUTUREHEAD SERVICES ====================
  
  fastify.get('/api/services/futurehead', async () => {
    return FUTUREHEAD_SERVICES;
  });

  fastify.get('/api/services/futurehead/active', async () => {
    return FUTUREHEAD_SERVICES.filter(s => s.status === 'active');
  });

  // ==================== EXTERNAL SERVICES ====================
  
  fastify.get('/api/services/external', async () => {
    return EXTERNAL_SERVICES;
  });

  // ==================== PLATFORM STATS ====================
  
  fastify.get('/api/services/stats', async () => {
    const activeServices = FUTUREHEAD_SERVICES.filter(s => s.status === 'active').length;
    const plannedServices = FUTUREHEAD_SERVICES.filter(s => s.status === 'planned').length;
    
    return {
      total_modules: 101,
      government_departments: 40,
      finance_tokens: 12,
      futurehead_services: 49,
      active_services: activeServices,
      planned_services: plannedServices,
      external_services: EXTERNAL_SERVICES.length,
      platform: {
        name: 'Ierahkwa Futurehead Platform',
        version: '2.0.0',
        blockchain: 'Ierahkwa Sovereign Blockchain (ISB)',
        node: 'Ierahkwa Futurehead Mamey Node',
        bank: 'Ierahkwa Futurehead BDET Bank'
      }
    };
  });

  // ==================== SEARCH ====================
  
  fastify.get('/api/services/search', async (req) => {
    const { q } = req.query;
    if (!q) return { error: 'Query required' };
    
    const query = q.toLowerCase();
    const results = [];
    
    GOVERNMENT_DEPARTMENTS.forEach(d => {
      if (d.name.toLowerCase().includes(query) || d.symbol.toLowerCase().includes(query)) {
        results.push({ ...d, type: 'department' });
      }
    });
    
    FINANCE_TOKENS.forEach(t => {
      if (t.name.toLowerCase().includes(query) || t.symbol.toLowerCase().includes(query)) {
        results.push({ ...t, type: 'token' });
      }
    });
    
    FUTUREHEAD_SERVICES.forEach(s => {
      if (s.name.toLowerCase().includes(query) || s.symbol.toLowerCase().includes(query)) {
        results.push({ ...s, type: 'service' });
      }
    });
    
    return results;
  });

  // ==================== TOKEN WHITEPAPER INFO ====================
  
  fastify.get('/api/services/whitepaper/:symbol', async (req) => {
    const { symbol } = req.params;
    const dept = GOVERNMENT_DEPARTMENTS.find(d => d.symbol === symbol);
    const token = FINANCE_TOKENS.find(t => t.symbol === symbol);
    const service = FUTUREHEAD_SERVICES.find(s => s.symbol === symbol);
    
    const module = dept || token || service;
    if (!module) return { error: 'Module not found' };
    
    return {
      symbol: module.symbol,
      name: module.name,
      type: dept ? 'government' : token ? 'finance' : 'service',
      tokenomics: {
        supply: '10,000,000,000,000',
        decimals: 9,
        standard: 'IGT-20'
      },
      whitepaper_available: true,
      languages: ['en', 'es', 'fr', 'moh']
    };
  });
}

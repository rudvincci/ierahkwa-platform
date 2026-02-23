/**
 * ═══════════════════════════════════════════════════════════════════════════════
 * PLATAFORMA GLOBAL — Un solo código para todas las rutas y servicios
 * IERAHKWA Futurehead - BDET Bank • Blockchain • Trading • Exchange • Bancos
 * ═══════════════════════════════════════════════════════════════════════════════
 * Fuente única de verdad: rutas, redirects y servicios.
 * server.js usa ROUTES + REDIRECTS. Frontend usa /api/platform-global (SERVICES).
 */

const path = require('path');

const ROOT = path.join(__dirname, '..');
const PLATFORM_DIR = path.join(ROOT, 'platform');

// ─── REDIRECTS (alias → canónico) ─────────────────────────────────────────────
const REDIRECTS = [
  { from: '/4-banks', to: '/central-banks' },
  { from: '/settlement', to: '/siis' },
  { from: '/deudas', to: '/debt-collection' },
  { from: '/soberania', to: '/sovereignty' },
  { from: '/futurehead-group', to: '/futurehead' },
  { from: '/mamey', to: '/mamey-futures' },
  { from: '/trading', to: '/mamey-futures' },
  { from: '/futures', to: '/mamey-futures' },
  { from: '/commodities', to: '/mamey-futures' },
  { from: '/options', to: '/mamey-futures' },
  { from: '/crypto', to: '/bitcoin-hemp' },
  { from: '/atm-manufacturing', to: '/atm' },
  { from: '/bdet', to: '/bdet-bank' },
  { from: '/banking', to: '/bdet-bank' },
  { from: '/global-banking', to: '/bank-worker' },
];

// ─── REDIRECTS EXTRA (más alias; /crypto, /atm-manufacturing, /global-banking ya en REDIRECTS) ─
const REDIRECTS_EXTRA = [
  { from: '/social-media.html', to: '/social-media' },
  { from: '/launchpad', to: '/citizen-launchpad' },
  { from: '/tokenize', to: '/citizen-launchpad' },
  { from: '/register-project', to: '/citizen-launchpad' },
  { from: '/backup-department', to: '/backup' },
  { from: '/103-departments', to: '/departments' },
  { from: '/depts', to: '/departments' },
  { from: '/playtube', to: '/social-media-department' },
  { from: '/pixelphoto', to: '/social-media-department' },
  { from: '/wowonder', to: '/social-media-department' },
  { from: '/block30', to: '/social-media-department' },
  { from: '/chat-encriptado', to: '/secure-chat' },
  { from: '/video-encriptado', to: '/video-call' },
  { from: '/monetizacion', to: '/monetization' },
];

// ─── ROUTES (path canónico → archivo en platform/) ────────────────────────────
const ROUTES = [
  // Soberano: Bancos, SIIS, Super Bank, Maletas, Deudas, Soberanía, Futurehead
  { path: '/central-banks', file: 'central-banks.html', name: '4 Bancos / Central Banks', category: 'bank' },
  { path: '/siis', file: 'siis-settlement.html', name: 'SIIS Settlement', category: 'settlement' },
  { path: '/super-bank-global', file: 'super-bank-global.html', name: 'Super Bank Global', category: 'bank' },
  { path: '/maletas', file: 'maletas.html', name: 'Maletas', category: 'bank' },
  { path: '/debt-collection', file: 'debt-collection.html', name: 'Cobro de Deudas', category: 'department' },
  { path: '/sovereignty', file: 'sovereignty-education.html', name: 'Soberanía y Educación', category: 'department' },
  { path: '/futurehead', file: 'futurehead-group.html', name: 'Futurehead Group', category: 'department' },
  // Trading • Exchange • Blockchain
  { path: '/mamey-futures', file: 'mamey-futures.html', name: 'Mamey Futures', category: 'trading' },
  { path: '/bitcoin-hemp', file: 'bitcoin-hemp.html', name: 'Bitcoin Hemp', category: 'crypto' },
  { path: '/bridge', file: 'bridge.html', name: 'Bridge', category: 'blockchain' },
  { path: '/blockchain', file: 'blockchain-platform.html', name: 'Blockchain Platform', category: 'blockchain' },
  // BDET Bank • Bancos • Wallet • Forex
  { path: '/atm', file: 'atm-manufacturing.html', name: 'ATM Manufacturing', category: 'hardware' },
  { path: '/bdet-bank', file: 'bdet-bank.html', name: 'BDET Bank', category: 'bank' },
  { path: '/bank-worker', file: 'bank-worker.html', name: 'Bank Worker', category: 'bank' },
  { path: '/wallet', file: 'wallet.html', name: 'Wallet', category: 'bank' },
  { path: '/forex', file: 'forex.html', name: 'Forex', category: 'trading' },
  // Seguridad y control
  { path: '/security', file: 'security-fortress.html', name: 'Security Fortress', category: 'security' },
  { path: '/leader-control', file: 'leader-control.html', name: 'Leader Control', category: 'security' },
  { path: '/app-studio', file: 'app-studio.html', name: 'App Studio', category: 'other' },
  { path: '/monitor', file: 'monitor.html', name: 'Monitor', category: 'security' },
  // Servicios financieros y plataforma
  { path: '/cryptohost', file: 'cryptohost.html', name: 'CryptoHost', category: 'crypto' },
  { path: '/net10', file: 'net10-defi.html', name: 'NET10 DeFi', category: 'defi' },
  { path: '/farmfactory', file: 'farmfactory.html', name: 'FarmFactory', category: 'defi' },
  { path: '/dao', file: 'dao-governance.html', name: 'DAO Governance', category: 'blockchain' },
  { path: '/ido-factory', file: 'ido-factory.html', name: 'IDO Factory', category: 'crypto' },
  { path: '/financial-instruments', file: 'financial-instruments.html', name: 'Financial Instruments', category: 'bank' },
  { path: '/invoicer', file: 'invoicer.html', name: 'Invoicer', category: 'bank' },
  { path: '/digital-vault', file: 'digital-vault.html', name: 'Digital Vault', category: 'bank' },
  { path: '/sistema-bancario', file: 'sistema-bancario.html', name: 'Sistema Bancario', category: 'bank' },
  { path: '/citizen-launchpad', file: 'citizen-launchpad.html', name: 'Citizen Launchpad', category: 'crypto' },
  { path: '/vip-transactions', file: 'vip-transactions.html', name: 'VIP Transactions', category: 'bank' },
  // Gaming, educación, oficina, AI, etc.
  { path: '/gaming', file: 'gaming-platform.html', name: 'Gaming', category: 'other' },
  { path: '/casino', file: 'casino.html', name: 'Casino', category: 'other' },
  { path: '/lotto', file: 'lotto.html', name: 'Lotto', category: 'other' },
  { path: '/raffle', file: 'raffle.html', name: 'Raffle', category: 'other' },
  { path: '/documents', file: 'documents.html', name: 'Documents', category: 'other' },
  { path: '/login', file: 'login.html', name: 'Login', category: 'auth' },
  { path: '/forgot-password', file: 'forgot-password.html', name: 'Recuperar contraseña', category: 'auth' },
  { path: '/reset-password', file: 'reset-password.html', name: 'Nueva contraseña', category: 'auth' },
  { path: '/admin', file: 'admin.html', name: 'Admin', category: 'auth' },
  { path: '/bank-login', file: 'bank-login.html', name: 'Bank Login', category: 'auth' },
  { path: '/bank-admin', file: 'bank-admin.html', name: 'Bank Admin', category: 'auth' },
  { path: '/spike-office', file: 'spike-office.html', name: 'Spike Office', category: 'other' },
  { path: '/rnbcal', file: 'rnbcal.html', name: 'RnBCal', category: 'other' },
  { path: '/appbuilder', file: 'appbuilder.html', name: 'AppBuilder', category: 'other' },
  { path: '/advocate', file: 'advocate.html', name: 'Advocate', category: 'other' },
  { path: '/esignature', file: 'esignature.html', name: 'ESignature', category: 'other' },
  { path: '/citizen-crm', file: 'citizen-crm.html', name: 'Citizen CRM', category: 'other' },
  { path: '/health-dashboard', file: 'health-dashboard.html', name: 'Health Dashboard', category: 'other' },
  { path: '/support-ai', file: 'support-ai.html', name: 'Support AI', category: 'ai' },
  { path: '/audit-log', file: 'audit-log.html', name: 'Audit Log', category: 'security' },
  { path: '/notifications', file: 'notifications.html', name: 'Notifications', category: 'other' },
  { path: '/settings', file: 'settings.html', name: 'Settings', category: 'other' },
  { path: '/video-call', file: 'video-call.html', name: 'Video Call', category: 'other' },
  { path: '/secure-chat', file: 'secure-chat.html', name: 'Secure Chat', category: 'other' },
  { path: '/monetization', file: 'monetization-dashboard.html', name: 'Monetización', category: 'other' },
  { path: '/contribution-graph', file: 'contribution-graph.html', name: 'Contribution Graph', category: 'other' },
  { path: '/biometrics', file: 'biometrics.html', name: 'Biometrics', category: 'other' },
  { path: '/budget-control', file: 'budget-control.html', name: 'Budget Control', category: 'other' },
  { path: '/chat', file: 'chat.html', name: 'Chat', category: 'other' },
  { path: '/dashboard', file: 'dashboard.html', name: 'Dashboard', category: 'other' },
  { path: '/dashboard-full', file: 'dashboard-full.html', name: 'Dashboard Full', category: 'other' },
  { path: '/user-dashboard', file: 'user-dashboard.html', name: 'User Dashboard', category: 'other' },
  { path: '/email-studio', file: 'email-studio.html', name: 'Email Studio', category: 'other' },
  { path: '/meeting-hub', file: 'meeting-hub.html', name: 'Meeting Hub', category: 'other' },
  { path: '/project-hub', file: 'project-hub.html', name: 'Project Hub', category: 'other' },
  { path: '/service-desk', file: 'service-desk.html', name: 'Service Desk', category: 'other' },
  { path: '/smartschool', file: 'smartschool.html', name: 'SmartSchool', category: 'other' },
  { path: '/social-codes', file: 'social-media-codes.html', name: 'Social Codes', category: 'other' },
  { path: '/sports-betting', file: 'sports-betting.html', name: 'Sports Betting', category: 'other' },
  { path: '/workflow', file: 'workflow-engine.html', name: 'Workflow', category: 'other' },
  { path: '/animstorm-ai', file: 'animstorm-ai.html', name: 'Animstorm AI', category: 'ai' },
  { path: '/ai-hub', file: 'ai-hub-dashboard.html', name: 'AI Hub', category: 'ai' },
  { path: '/atabey', file: 'atabey-dashboard.html', name: 'Atabey', category: 'ai' },
  { path: '/editor', file: 'editor-complete.html', name: 'Editor', category: 'other' },
  { path: '/social-media', file: 'social-media.html', name: 'Social Media', category: 'other' },
  { path: '/social-platform', file: 'social-platform.html', name: 'Social Platform (Reels & Live)', category: 'other' },
  { path: '/ierahkwa-video', file: 'ierahkwa-video.html', name: 'IERAHKWA Video (YouTube)', category: 'other' },
  { path: '/sistass-video', file: 'sistass-video.html', name: 'Sistass Video - 4 plataformas', category: 'other' },
  { path: '/social-media-department', file: 'social-media-department.html', name: 'Departamento Social Media (PlayTube, PixelPhoto, WoWonder, Block 30)', category: 'other' },
  { path: '/app-ai-studio', file: 'app-ai-studio.html', name: 'App AI Studio', category: 'ai' },
  // Backup, departments, ROADMAP r7–r12
  { path: '/backup', file: 'backup-department.html', name: 'Backup', category: 'other' },
  { path: '/departments', file: 'departments.html', name: 'Departments', category: 'other' },
  { path: '/analytics-dashboard', file: 'analytics-dashboard.html', name: 'Analytics Dashboard', category: 'other' },
  { path: '/privacy-preferences', file: 'privacy-preferences.html', name: 'Preferencias de privacidad', category: 'other' },
  { path: '/tickets', file: 'tickets.html', name: 'Tickets / Soporte', category: 'other' },
  { path: '/maintenance', file: 'maintenance.html', name: 'Mantenimiento', category: 'other' },
];

// ─── SERVICES (para All-in-One, BDET Bank, frontend) ───────────────────────────
// Categorías: blockchain, trading, exchange, defi, crypto, bank, settlement
const SERVICES = [
  { id: 'blockchain', path: '/blockchain', name: 'Blockchain Platform', category: 'blockchain', icon: '⛓️' },
  { id: 'mamey', path: '/mamey-futures', name: 'Mamey Futures', category: 'trading', icon: '🎯' },
  { id: 'tradex', path: '/tradex', name: 'TradeX Exchange', category: 'exchange', icon: '📈' },
  { id: 'net10', path: '/net10', name: 'NET10 DeFi', category: 'defi', icon: '🌐' },
  { id: 'cryptohost', path: '/cryptohost', name: 'CryptoHost', category: 'crypto', icon: '₿' },
  { id: 'bridge', path: '/bridge', name: 'Bridge', category: 'blockchain', icon: '🔗' },
  { id: 'bitcoin-hemp', path: '/bitcoin-hemp', name: 'Bitcoin Hemp', category: 'crypto', icon: '₿' },
  { id: 'siis', path: '/siis', name: 'SIIS Settlement', category: 'settlement', icon: '🌐' },
  { id: 'central-banks', path: '/central-banks', name: '4 Bancos / Central Banks', category: 'bank', icon: '🏛️' },
  { id: 'super-bank-global', path: '/super-bank-global', name: 'Super Bank Global', category: 'bank', icon: '🌐' },
  { id: 'maletas', path: '/maletas', name: 'Maletas', category: 'bank', icon: '🧳' },
  { id: 'bdet-bank', path: '/bdet-bank', name: 'BDET Bank', category: 'bank', icon: '🏦' },
  { id: 'bdet-accounts', path: '/bdet-accounts', name: 'Cuentas BDET', category: 'bank', icon: '📒' },
  { id: 'wallet', path: '/wallet', name: 'Wallet', category: 'bank', icon: '👛' },
  { id: 'forex', path: '/forex', name: 'Forex', category: 'trading', icon: '💱' },
  { id: 'debt-collection', path: '/debt-collection', name: 'Cobro de Deudas', category: 'bank', icon: '💰' },
  { id: 'bank-worker', path: '/bank-worker', name: 'Bank Worker', category: 'bank', icon: '🏦' },
  { id: 'dao', path: '/dao', name: 'DAO', category: 'blockchain', icon: '🗳️' },
  { id: 'ido-factory', path: '/ido-factory', name: 'IDO Factory', category: 'crypto', icon: '🏭' },
  { id: 'financial-instruments', path: '/financial-instruments', name: 'Financial Instruments', category: 'bank', icon: '📊' },
  { id: 'invoicer', path: '/invoicer', name: 'Invoicer', category: 'bank', icon: '📄' },
  { id: 'digital-vault', path: '/digital-vault', name: 'Digital Vault', category: 'bank', icon: '🔐' },
  { id: 'sistema-bancario', path: '/sistema-bancario', name: 'Sistema Bancario', category: 'bank', icon: '🏦' },
  { id: 'citizen-launchpad', path: '/citizen-launchpad', name: 'Citizen Launchpad', category: 'crypto', icon: '🚀' },
  { id: 'farmfactory', path: '/farmfactory', name: 'FarmFactory', category: 'defi', icon: '🌾' },
  { id: 'token-factory', path: '/token-factory', name: 'Token Factory', category: 'crypto', icon: '🪙' },
];

// Combinar todos los redirects
const ALL_REDIRECTS = [...REDIRECTS, ...REDIRECTS_EXTRA];

/** Genera el objeto platform-urls (key->path) desde ROUTES y REDIRECTS. Usado por /api/platform-urls y scripts/generate-platform-urls.js */
function buildPlatformUrls() {
  const u = {};
  ROUTES.forEach(r => {
    const key = (r.path || '').replace(/^\//, '');
    if (key) u[key] = r.path;
    const base = (r.file || '').replace(/\.html$/, '');
    if (base && base !== key) u[base] = r.path;
  });
  ALL_REDIRECTS.forEach(r => {
    const k = (r.from || '').replace(/^\//, '');
    if (k) u[k] = r.to;
  });
  return u;
}

module.exports = {
  PLATFORM_DIR,
  ROOT,
  ROUTES,
  REDIRECTS: ALL_REDIRECTS,
  SERVICES,
  buildPlatformUrls,
};

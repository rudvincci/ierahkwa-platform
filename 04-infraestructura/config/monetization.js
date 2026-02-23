/**
 * ═══════════════════════════════════════════════════════════════════════════════
 * IERAHKWA MONETIZATION — Canales de Ingresos y Tiers
 * Sovereign Government of Ierahkwa Ne Kanienke
 * ═══════════════════════════════════════════════════════════════════════════════
 */

// ─── CANALES DE INGRESOS ─────────────────────────────────────────────────────
const REVENUE_STREAMS = [
  {
    id: 'financial',
    name: 'Comisiones Financieras',
    icon: '🏦',
    path: '/bdet-bank',
    description: 'Transferencias, wire, ACH, tarjetas WAMPUM, forex, custody',
    feeExamples: [
      'Wire: 0.1% (mín $10)',
      'Tarjetas: 1–2.5% interchange',
      'Forex: 0.5% spread',
      'Custody: 0.1–0.5%/año',
    ],
    currency: 'USD / IGT',
    weight: 'Alto',
  },
  {
    id: 'igt-tokens',
    name: 'Tokens IGT',
    icon: '🪙',
    path: '/bitcoin-hemp',
    description: 'Fees en DEX, trading, staking, gas, 103 tokens',
    feeExamples: [
      'Trading: maker 0.05%, taker 0.1%',
      'Staking: 5–15% APY (parte a treasury)',
      'IDO/Launchpad: 2–5% de lo recaudado',
    ],
    currency: 'IGT',
    weight: 'Alto',
  },
  {
    id: 'subscriptions',
    name: 'Suscripciones y Tiers',
    icon: '👑',
    path: '/citizen-launchpad',
    description: 'Ciudadano Free, Premium, Empresas, VIP',
    feeExamples: [
      'Premium: 9.99 IGT/mes',
      'Empresas: 99 IGT/mes',
      'VIP: 499 IGT/mes o invitación',
    ],
    currency: 'IGT',
    weight: 'Medio',
  },
  {
    id: 'gaming-casino',
    name: 'Casino, Gaming, Apuestas',
    icon: '🎰',
    path: '/casino',
    description: 'House edge, slots, live dealer, sports betting, poker',
    feeExamples: [
      'Slots: 2–15% house edge',
      'Blackjack: ~0.5%',
      'Sports: 5–10% margen',
      'Poker: rake 2–5%',
    ],
    currency: 'IGT / USD',
    weight: 'Alto',
  },
  {
    id: 'launchpad-ido',
    name: 'Launchpad e IDO',
    icon: '🚀',
    path: '/citizen-launchpad',
    description: 'Tokenización de proyectos, listing, fees de éxito',
    feeExamples: [
      'Fee de listing: 1–5% del raise',
      'Success fee: 2–5%',
      'Staking para acceso: IGT bloqueados',
    ],
    currency: 'IGT',
    weight: 'Medio',
  },
  {
    id: 'apis-b2b',
    name: 'APIs y B2B',
    icon: '🔌',
    path: '/siis',
    description: 'SIIS, KYC, pagos, datos agregados para instituciones',
    feeExamples: [
      'SIIS: por transacción o mensual',
      'KYC as a Service: por verificación',
      'Data/Reports: suscripción anual',
    ],
    currency: 'USD / IGT',
    weight: 'Medio',
  },
  {
    id: 'comms-premium',
    name: 'Comunicaciones Premium',
    icon: '🔐',
    path: '/secure-chat',
    description: 'Chat E2E y video base gratis; premium: salas grandes, almacenamiento, históricos',
    feeExamples: [
      'Base: gratuito',
      'Salas >10: 4.99 IGT/mes',
      'Histórico >90 días: 2.99 IGT/mes',
    ],
    currency: 'IGT',
    weight: 'Bajo',
  },
  {
    id: 'licensing',
    name: 'Licencias y White-Label',
    icon: '📜',
    path: '/platform',
    description: 'Otras naciones o empresas que usen el stack IERAHKWA',
    feeExamples: [
      'Setup: one-time',
      'SaaS: % de volumen o mensual',
    ],
    currency: 'USD',
    weight: 'Medio',
  },
];

// ─── TIERS DE USUARIO ────────────────────────────────────────────────────────
const TIERS = [
  {
    id: 'free',
    name: 'Ciudadano',
    price: 0,
    priceUnit: 'IGT/mes',
    features: ['Chat E2E', 'Video 1:1', 'Wallet básica', 'Red social', 'Acceso plataforma'],
    path: '/platform',
  },
  {
    id: 'premium',
    name: 'Premium',
    price: 9.99,
    priceUnit: 'IGT/mes',
    features: ['Todo Free', 'Video grupal 10', 'Histórico chat 1 año', 'Menos fees en trading', 'Soporte prioritario'],
    path: '/citizen-launchpad',
  },
  {
    id: 'business',
    name: 'Empresas',
    price: 99,
    priceUnit: 'IGT/mes',
    features: ['Todo Premium', 'APIs', 'Múltiples usuarios', 'Invoicer', 'KYC masivo'],
    path: '/invoicer',
  },
  {
    id: 'vip',
    name: 'VIP',
    price: 499,
    priceUnit: 'IGT/mes',
    inviteOnly: true,
    features: ['Todo Empresas', 'Account manager', 'Trust services', 'Concierge financiero'],
    path: '/vip-transactions',
  },
];

// ─── FEE SCHEDULE RESUMIDO (para UI) ──────────────────────────────────────────
const FEE_SUMMARY = {
  wire: { rate: '0.1%', min: 10, currency: 'USD' },
  cardInterchange: { rate: '1–2.5%', currency: 'USD' },
  trading: { maker: '0.05%', taker: '0.1%', currency: 'IGT' },
  custody: { rate: '0.1–0.5%/año', currency: 'USD' },
  launchpad: { listing: '1–5%', success: '2–5%', currency: 'IGT' },
  ido: { fee: '2–5% del raise', currency: 'IGT' },
};

// ─── IGT: USO EN MONETIZACIÓN ─────────────────────────────────────────────────
const IGT_USE = {
  acceptedIn: ['subscriptions', 'trading', 'casino', 'gaming', 'launchpad', 'comms_premium'],
  stakingFor: ['Launchpad access', 'Fee discounts', 'Governance'],
  treasuryShare: 'Parte de fees y house edge se destina al treasury soberano',
};

module.exports = {
  REVENUE_STREAMS,
  TIERS,
  FEE_SUMMARY,
  IGT_USE,
};

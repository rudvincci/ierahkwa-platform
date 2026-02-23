/**
 * i18n Configuration - Multi-language Support
 * Languages: English, Spanish, Mohawk (Kanien'kéha), Taíno
 */

import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

const resources = {
  en: {
    translation: {
      // General
      welcome: 'Welcome to Ierahkwa Sovereign Platform',
      dashboard: 'Dashboard',
      tokens: 'Tokens',
      trading: 'Trading',
      wallet: 'Wallet',
      bridge: 'Bridge',
      voting: 'Voting',
      rewards: 'Rewards',
      settings: 'Settings',
      
      // Actions
      connect_wallet: 'Connect Wallet',
      swap: 'Swap',
      stake: 'Stake',
      send: 'Send',
      receive: 'Receive',
      
      // Wallet
      total_balance: 'Total Balance',
      recent_transactions: 'Recent Transactions',
      your_assets: 'Your Assets',
      
      // Governance
      governance: 'Governance',
      proposals: 'Proposals',
      vote_now: 'Vote Now',
      create_proposal: 'Create Proposal',
      active: 'Active',
      ended: 'Ended',
      
      // Rewards
      daily_rewards: 'Daily Rewards',
      achievements: 'Achievements',
      leaderboard: 'Leaderboard',
      claim: 'Claim',
      level: 'Level',
      points: 'Points',
      
      // Bridge
      bridge_tokens: 'Bridge Tokens',
      from_chain: 'From Chain',
      to_chain: 'To Chain',
      
      // Settings
      language: 'Language',
      theme: 'Theme',
      notifications: 'Notifications',
      security: 'Security',
      about: 'About',
      
      // Network
      network_status: 'Network Status',
      block_height: 'Block Height',
      gas_price: 'Gas Price',
    },
  },
  es: {
    translation: {
      // General
      welcome: 'Bienvenido a la Plataforma Soberana Ierahkwa',
      dashboard: 'Panel',
      tokens: 'Tokens',
      trading: 'Trading',
      wallet: 'Billetera',
      bridge: 'Puente',
      voting: 'Votación',
      rewards: 'Recompensas',
      settings: 'Configuración',
      
      // Actions
      connect_wallet: 'Conectar Billetera',
      swap: 'Intercambiar',
      stake: 'Stakear',
      send: 'Enviar',
      receive: 'Recibir',
      
      // Wallet
      total_balance: 'Balance Total',
      recent_transactions: 'Transacciones Recientes',
      your_assets: 'Tus Activos',
      
      // Governance
      governance: 'Gobernanza',
      proposals: 'Propuestas',
      vote_now: 'Votar Ahora',
      create_proposal: 'Crear Propuesta',
      active: 'Activo',
      ended: 'Finalizado',
      
      // Rewards
      daily_rewards: 'Recompensas Diarias',
      achievements: 'Logros',
      leaderboard: 'Tabla de Líderes',
      claim: 'Reclamar',
      level: 'Nivel',
      points: 'Puntos',
      
      // Bridge
      bridge_tokens: 'Transferir Tokens',
      from_chain: 'Desde Cadena',
      to_chain: 'Hacia Cadena',
      
      // Settings
      language: 'Idioma',
      theme: 'Tema',
      notifications: 'Notificaciones',
      security: 'Seguridad',
      about: 'Acerca de',
      
      // Network
      network_status: 'Estado de Red',
      block_height: 'Altura de Bloque',
      gas_price: 'Precio de Gas',
    },
  },
  moh: {
    translation: {
      // Kanien'kéha (Mohawk)
      welcome: 'Kwe - Ierahkwa Tsi Niionkwarihoten',
      dashboard: 'Tsi Nikanataroten',
      tokens: 'Ohenton',
      trading: 'Atennhawiehstani',
      wallet: 'Kahwatsira',
      bridge: 'Tekawennahskohon',
      voting: 'Iewennenhawitha',
      rewards: 'Kahwatsirowanens',
      settings: 'Tsi Niionkwarihoten',
      
      connect_wallet: 'Sahontatken Kahwatsira',
      swap: 'Teiatatenonhwarori',
      stake: 'Tekawennahretsiaraks',
      send: 'Kasenna',
      receive: 'Takwennahkwa',
      
      total_balance: 'Akwekon Ohwista',
      recent_transactions: 'Onkwe Tsi Nahe',
      your_assets: 'Sohwista',
      
      governance: 'Ionterennaienthos',
      proposals: 'Tsi Nahe Kanatien',
      vote_now: 'Tekahswate Nonwa',
      create_proposal: 'Satahsontehre',
      
      daily_rewards: 'Wennisera Kahwatsirowanens',
      achievements: 'Tsi Ionkwaienterihne',
      leaderboard: 'Ratirihwakste',
      claim: 'Takwatho',
      level: 'Niwennake',
      points: 'Oia',
    },
  },
  tai: {
    translation: {
      // Taíno
      welcome: 'Taíno - Cacike Ierahkwa',
      dashboard: 'Bohío Principal',
      tokens: 'Güaicas',
      trading: 'Comercio',
      wallet: 'Canoa',
      bridge: 'Puente',
      voting: 'Areyto',
      rewards: 'Cemíes',
      settings: 'Configuración',
      
      connect_wallet: 'Conectar Canoa',
      swap: 'Trueque',
      stake: 'Guardar',
      send: 'Enviar',
      receive: 'Recibir',
      
      total_balance: 'Balance Cacical',
      recent_transactions: 'Últimos Trueques',
      your_assets: 'Tus Riquezas',
      
      governance: 'Consejo',
      proposals: 'Propuestas',
      vote_now: 'Votar',
      create_proposal: 'Nueva Propuesta',
      
      daily_rewards: 'Recompensas del Sol',
      achievements: 'Hazañas',
      leaderboard: 'Caciques',
      claim: 'Reclamar',
      level: 'Rango',
      points: 'Güiros',
    },
  },
};

i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: 'en', // Default language
    fallbackLng: 'en',
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n;

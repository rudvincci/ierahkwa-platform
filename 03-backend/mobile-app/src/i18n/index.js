/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  i18n Configuration - Multi-language Support
 *  Languages: English, Spanish, Mohawk (Kanien'keha), Taino
 * ═══════════════════════════════════════════════════════════════════════════════
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
      view_all: 'View All',
      cancel: 'Cancel',

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
      settings_dark_mode: 'Dark Mode',
      settings_font_size: 'Font Size',
      settings_biometric: 'Biometric Authentication',
      settings_change_pin: 'Change PIN',
      settings_backup_keys: 'Backup Recovery Keys',
      settings_2fa: 'Two-Factor Authentication',
      settings_rpc: 'RPC Endpoint',
      settings_explorer: 'Block Explorer',
      settings_version: 'Version',
      settings_terms: 'Terms of Service',
      settings_privacy: 'Privacy Policy',
      settings_support: 'Support',
      settings_logout: 'Log Out',
      settings_logout_title: 'Log Out',
      settings_logout_message: 'Are you sure you want to log out?',
      notif_transactions: 'Transactions',
      notif_governance: 'Governance',
      notif_security: 'Security',
      notif_health: 'Health',
      notif_education: 'Education',
      notif_marketing: 'Marketing',

      // Network
      network_status: 'Network Status',
      block_height: 'Block Height',
      gas_price: 'Gas Price',

      // Government
      gov_screen_title: 'Government Services Screen',
      gov_citizen_services: 'Citizen Services',
      gov_subtitle: 'Ierahkwa Ne Kanienke - Sovereign Government',
      gov_citizen_id: 'Sovereign Citizen ID',
      gov_verified: 'Verified Citizen',
      gov_search: 'Search government services...',
      gov_services: 'Services',
      gov_identity: 'Identity',
      gov_permits: 'Permits',
      gov_taxes: 'Taxes',
      gov_census: 'Census',
      gov_legal: 'Legal',
      gov_land: 'Land Registry',
      gov_pending: 'Pending Requests',
      gov_quick_actions: 'Quick Actions',
      gov_new_request: 'New Request',
      gov_contact: 'Contact Agency',
      gov_appointment: 'Book Appointment',
      gov_alerts: 'Alerts',

      // Health
      health_screen_title: 'Health Services Screen',
      health_title: 'Health & Wellness',
      health_subtitle: 'Sovereign Healthcare System',
      health_score: 'Wellness Score',
      health_score_desc: 'Based on your latest health metrics and activity',
      health_metrics: 'Health Metrics',
      health_heart_rate: 'Heart Rate',
      health_blood_pressure: 'Blood Pressure',
      health_glucose: 'Glucose',
      health_weight: 'Weight',
      health_services: 'Health Services',
      health_telemedicine: 'Telemedicine',
      health_prescriptions: 'Prescriptions',
      health_vaccines: 'Vaccines',
      health_insurance: 'Insurance',
      health_records: 'Records',
      health_mental: 'Mental Health',
      health_appointments: 'Upcoming Appointments',
      health_book: 'Book',
      health_vaccinations: 'Vaccination Records',
      health_emergency: 'EMERGENCY',

      // Education
      edu_screen_title: 'Education Portal Screen',
      edu_title: 'Learning Portal',
      edu_subtitle: 'Ierahkwa Sovereign Academy',
      edu_lessons: 'Lessons',
      edu_streak: 'Day Streak',
      edu_enrolled: 'Enrolled',
      edu_xp: 'XP',
      edu_courses: 'Courses',
      edu_languages: 'Languages',
      edu_certs: 'Certifications',
      edu_library: 'Library',
      edu_my_courses: 'My Courses',
      edu_lessons_done: 'lessons completed',
      edu_featured: 'Featured Courses',
      edu_achievements: 'Achievements',

      // Security
      sec_screen_title: 'Security Dashboard Screen',
      sec_title: 'Security Center',
      sec_subtitle: 'NEXUS Escudo - AI Defense Network',
      sec_system_status: 'System Status',
      sec_threat_level: 'Threat Level',
      sec_firewall: 'Firewall',
      sec_encryption: 'Encryption',
      sec_blocked_today: 'Blocked today',
      sec_last_scan: 'Last scan',
      sec_ai_agents: 'AI Security Agents',
      sec_online: 'Online',
      sec_threats: 'Threats',
      sec_uptime: 'Uptime',
      sec_alerts: 'Recent Alerts',
      sec_scan: 'Full Scan',
      sec_report: 'Report',

      // Profile
      profile_screen_title: 'Profile Screen',
      profile_verified: 'Verified on Mamey Blockchain',
      profile_kyc: 'KYC Verification',
      profile_wallets: 'Connected Wallets',
      profile_badges: 'Citizen Badges',
      profile_edit: 'Edit Profile',
      profile_documents: 'Documents',
      profile_activity: 'Activity Log',
      profile_connections: 'Connected Apps',
      profile_export: 'Export Data',
      profile_delete: 'Delete Account',

      // Marketplace
      market_screen_title: 'Marketplace Screen',
      market_title: 'Marketplace',
      market_subtitle: 'Sovereign Commerce',
      market_search: 'Search products, services, NFTs...',
      market_all: 'All',
      market_artisan: 'Artisan',
      market_digital: 'Digital',
      market_nft: 'NFTs',
      market_services: 'Services',
      market_food: 'Food',
      market_featured: 'Featured',
      market_trending: 'Trending',
      market_sold: 'sold',
      market_start_selling: 'Start Selling',
      market_sell_desc: 'Open your sovereign store and reach 72M citizens',
      market_open_store: 'Open Store',

      // Communication
      comm_screen_title: 'Communication Screen',
      comm_title: 'Messages',
      comm_subtitle: 'Quantum-Encrypted Communication',
      comm_search: 'Search conversations...',
      comm_chats: 'Chats',
      comm_calls: 'Calls',
      comm_channels: 'Channels',
      comm_forums: 'Forums',
      comm_subscribers: 'subscribers',
      comm_encrypted: 'All messages are end-to-end encrypted with quantum-safe algorithms',
      comm_forums_soon: 'Community Forums',
      comm_forums_desc: 'Launching March 2026',
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
      voting: 'Votacion',
      rewards: 'Recompensas',
      settings: 'Configuracion',
      view_all: 'Ver Todo',
      cancel: 'Cancelar',

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
      leaderboard: 'Tabla de Lideres',
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
      settings_dark_mode: 'Modo Oscuro',
      settings_font_size: 'Tamano de Fuente',
      settings_biometric: 'Autenticacion Biometrica',
      settings_change_pin: 'Cambiar PIN',
      settings_backup_keys: 'Respaldar Claves de Recuperacion',
      settings_2fa: 'Autenticacion de Dos Factores',
      settings_rpc: 'Punto RPC',
      settings_explorer: 'Explorador de Bloques',
      settings_version: 'Version',
      settings_terms: 'Terminos de Servicio',
      settings_privacy: 'Politica de Privacidad',
      settings_support: 'Soporte',
      settings_logout: 'Cerrar Sesion',
      settings_logout_title: 'Cerrar Sesion',
      settings_logout_message: 'Estas seguro de que deseas cerrar sesion?',
      notif_transactions: 'Transacciones',
      notif_governance: 'Gobernanza',
      notif_security: 'Seguridad',
      notif_health: 'Salud',
      notif_education: 'Educacion',
      notif_marketing: 'Marketing',

      // Network
      network_status: 'Estado de Red',
      block_height: 'Altura de Bloque',
      gas_price: 'Precio de Gas',

      // Government
      gov_citizen_services: 'Servicios Ciudadanos',
      gov_subtitle: 'Ierahkwa Ne Kanienke - Gobierno Soberano',
      gov_citizen_id: 'ID Ciudadano Soberano',
      gov_verified: 'Ciudadano Verificado',
      gov_search: 'Buscar servicios gubernamentales...',
      gov_services: 'Servicios',
      gov_identity: 'Identidad',
      gov_permits: 'Permisos',
      gov_taxes: 'Impuestos',
      gov_census: 'Censo',
      gov_legal: 'Legal',
      gov_land: 'Registro de Tierras',
      gov_pending: 'Solicitudes Pendientes',
      gov_quick_actions: 'Acciones Rapidas',
      gov_new_request: 'Nueva Solicitud',
      gov_contact: 'Contactar Agencia',
      gov_appointment: 'Reservar Cita',
      gov_alerts: 'Alertas',

      // Health
      health_title: 'Salud y Bienestar',
      health_subtitle: 'Sistema de Salud Soberano',
      health_score: 'Puntuacion de Bienestar',
      health_score_desc: 'Basado en tus ultimas metricas de salud y actividad',
      health_metrics: 'Metricas de Salud',
      health_heart_rate: 'Ritmo Cardiaco',
      health_blood_pressure: 'Presion Arterial',
      health_glucose: 'Glucosa',
      health_weight: 'Peso',
      health_services: 'Servicios de Salud',
      health_telemedicine: 'Telemedicina',
      health_prescriptions: 'Recetas',
      health_vaccines: 'Vacunas',
      health_insurance: 'Seguro',
      health_records: 'Expedientes',
      health_mental: 'Salud Mental',
      health_appointments: 'Citas Proximas',
      health_book: 'Reservar',
      health_vaccinations: 'Registro de Vacunacion',
      health_emergency: 'EMERGENCIA',

      // Education
      edu_title: 'Portal de Aprendizaje',
      edu_subtitle: 'Academia Soberana Ierahkwa',
      edu_lessons: 'Lecciones',
      edu_streak: 'Racha',
      edu_enrolled: 'Inscritos',
      edu_xp: 'XP',
      edu_courses: 'Cursos',
      edu_languages: 'Idiomas',
      edu_certs: 'Certificaciones',
      edu_library: 'Biblioteca',
      edu_my_courses: 'Mis Cursos',
      edu_lessons_done: 'lecciones completadas',
      edu_featured: 'Cursos Destacados',
      edu_achievements: 'Logros',

      // Security
      sec_title: 'Centro de Seguridad',
      sec_subtitle: 'NEXUS Escudo - Red de Defensa IA',
      sec_system_status: 'Estado del Sistema',
      sec_threat_level: 'Nivel de Amenaza',
      sec_firewall: 'Cortafuegos',
      sec_encryption: 'Cifrado',
      sec_blocked_today: 'Bloqueados hoy',
      sec_last_scan: 'Ultimo escaneo',
      sec_ai_agents: 'Agentes de Seguridad IA',
      sec_online: 'En linea',
      sec_threats: 'Amenazas',
      sec_uptime: 'Disponibilidad',
      sec_alerts: 'Alertas Recientes',
      sec_scan: 'Escaneo Completo',
      sec_report: 'Reporte',

      // Profile
      profile_verified: 'Verificado en Blockchain Mamey',
      profile_kyc: 'Verificacion KYC',
      profile_wallets: 'Billeteras Conectadas',
      profile_badges: 'Insignias Ciudadanas',
      profile_edit: 'Editar Perfil',
      profile_documents: 'Documentos',
      profile_activity: 'Registro de Actividad',
      profile_connections: 'Apps Conectadas',
      profile_export: 'Exportar Datos',
      profile_delete: 'Eliminar Cuenta',

      // Marketplace
      market_title: 'Mercado',
      market_subtitle: 'Comercio Soberano',
      market_search: 'Buscar productos, servicios, NFTs...',
      market_all: 'Todo',
      market_artisan: 'Artesanal',
      market_digital: 'Digital',
      market_nft: 'NFTs',
      market_services: 'Servicios',
      market_food: 'Comida',
      market_featured: 'Destacados',
      market_trending: 'Tendencias',
      market_sold: 'vendidos',
      market_start_selling: 'Comienza a Vender',
      market_sell_desc: 'Abre tu tienda soberana y alcanza 72M ciudadanos',
      market_open_store: 'Abrir Tienda',

      // Communication
      comm_title: 'Mensajes',
      comm_subtitle: 'Comunicacion Cifrada Cuantica',
      comm_search: 'Buscar conversaciones...',
      comm_chats: 'Chats',
      comm_calls: 'Llamadas',
      comm_channels: 'Canales',
      comm_forums: 'Foros',
      comm_subscribers: 'suscriptores',
      comm_encrypted: 'Todos los mensajes estan cifrados de extremo a extremo con algoritmos cuanticos',
      comm_forums_soon: 'Foros Comunitarios',
      comm_forums_desc: 'Lanzamiento Marzo 2026',
    },
  },
  moh: {
    translation: {
      // Kanien'keha (Mohawk)
      welcome: 'Kwe - Ierahkwa Tsi Niionkwarihoten',
      dashboard: 'Tsi Nikanataroten',
      tokens: 'Ohenton',
      trading: 'Atennhawiehstani',
      wallet: 'Kahwatsira',
      bridge: 'Tekawennahskohon',
      voting: 'Iewennenhawitha',
      rewards: 'Kahwatsirowanens',
      settings: 'Tsi Niionkwarihoten',
      view_all: 'Akwekon Satkahtho',
      cancel: 'Tehsatste',

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
      active: 'Iekonnien',
      ended: 'Owistaha',

      daily_rewards: 'Wennisera Kahwatsirowanens',
      achievements: 'Tsi Ionkwaienterihne',
      leaderboard: 'Ratirihwakste',
      claim: 'Takwatho',
      level: 'Niwennake',
      points: 'Oia',

      bridge_tokens: 'Tekawennahskohon Ohenton',
      from_chain: 'Tsi Thaiehte',
      to_chain: 'Tsi Ieiehte',

      language: "Owenna'shon:a",
      theme: 'Tsi Nahotiieriton',
      notifications: 'Iontateriwahtontie',
      security: 'Ateriennaweron',
      about: 'Tsi Nahe',

      network_status: 'Tsi Nikanataroten Teierihwaienthos',
      block_height: 'Niwennake Iotohetston',
      gas_price: "O'non:wa Kahnekaien",

      // Government
      gov_citizen_services: 'Onkwehonwe Ratirihwaienthos',
      gov_citizen_id: 'Onkwehonwe Karihwatehkwen',

      // Health
      health_title: 'Tsi Ionkwaienterihne',
      health_emergency: "KANON'WARON:KWA",

      // Education
      edu_title: 'Ionterihwaienstahkhwa',

      // Security
      sec_title: 'Ateriennaweron',

      // Communication
      comm_title: 'Iontateriwahtontie',
    },
  },
  tai: {
    translation: {
      // Taino
      welcome: 'Taino - Cacike Ierahkwa',
      dashboard: 'Bohio Principal',
      tokens: 'Guaicas',
      trading: 'Comercio',
      wallet: 'Canoa',
      bridge: 'Puente',
      voting: 'Areyto',
      rewards: 'Cemies',
      settings: 'Configuracion',
      view_all: 'Ver Todo',
      cancel: 'Cancelar',

      connect_wallet: 'Conectar Canoa',
      swap: 'Trueque',
      stake: 'Guardar',
      send: 'Enviar',
      receive: 'Recibir',

      total_balance: 'Balance Cacical',
      recent_transactions: 'Ultimos Trueques',
      your_assets: 'Tus Riquezas',

      governance: 'Consejo',
      proposals: 'Propuestas',
      vote_now: 'Votar',
      create_proposal: 'Nueva Propuesta',
      active: 'Activo',
      ended: 'Terminado',

      daily_rewards: 'Recompensas del Sol',
      achievements: 'Hazanas',
      leaderboard: 'Caciques',
      claim: 'Reclamar',
      level: 'Rango',
      points: 'Guiros',

      bridge_tokens: 'Cruzar Guaicas',
      from_chain: 'Desde Isla',
      to_chain: 'Hacia Isla',

      language: 'Lengua',
      theme: 'Apariencia',
      notifications: 'Avisos',
      security: 'Proteccion',
      about: 'Acerca',

      network_status: 'Estado de la Red',
      block_height: 'Altura del Bloque',
      gas_price: 'Precio del Envio',

      // Government
      gov_citizen_services: 'Servicios del Cacicazgo',
      gov_citizen_id: 'ID Cacical',

      // Health
      health_title: 'Salud y Bienestar',
      health_emergency: 'EMERGENCIA',

      // Education
      edu_title: 'Portal de Aprendizaje',

      // Security
      sec_title: 'Centro de Proteccion',

      // Marketplace
      market_title: 'Mercado',

      // Communication
      comm_title: 'Mensajes',
    },
  },
};

i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: 'en',
    fallbackLng: 'en',
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n;

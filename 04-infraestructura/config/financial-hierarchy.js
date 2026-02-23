/**
 * ═══════════════════════════════════════════════════════════════════════════════
 * IERAHKWA FINANCIAL SYSTEM HIERARCHY - FULL IMPLEMENTATION
 * Sistema Financiero Completo con Licencias Bancarias
 * Basado en modelo BIS (Bank for International Settlements) + Panama Banking Model
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * JERARQUÍA COMPLETA:
 * 1. IFIs - Supervisión global (SIIS, IMF, World Bank equivalents)
 * 2. CLEARING HOUSE - Cámara de compensación (dinero entrando/saliendo, conecta todos los bancos)
 * 3. Bancos Centrales - Política monetaria, emisión de moneda, supervisión
 * 4. Banca de Desarrollo - Financiamiento de actividades económicas
 * 5. Bancos Regionales - Medianos, enfoque geográfico ($10B-$100B activos)
 * 6. Bancos Nacionales - Oficiales y Privados (modelo Panamá)
 * 7. Bancos Comerciales - Depósitos, préstamos, comercio exterior
 * 8. Servicios Especializados - Trading, DeFi, Crypto
 * 
 * LICENCIAS BANCARIAS:
 * - Licencia General: Operaciones nacionales e internacionales
 * - Licencia Internacional: Solo operaciones offshore
 * - Licencia de Representación: Oficinas de representación
 * 
 * MODELOS DE NEGOCIO (según BIS):
 * - Financiación Minorista: depósitos estables, préstamos, alta rentabilidad
 * - Financiación Mayorista: mercados de capitales, deuda, interbancario
 * - Negociación (Trading): derivados, títulos, mayor volatilidad
 */

const FINANCIAL_HIERARCHY = {
  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 1: INSTITUCIONES FINANCIERAS INTERNACIONALES (IFIs)
  // Supervisión global, estabilidad monetaria, desarrollo económico
  // ═══════════════════════════════════════════════════════════════════════════
  level1_IFIs: {
    name: 'Instituciones Financieras Internacionales',
    code: 'IFI',
    description: 'Supervisión global y estabilidad del sistema financiero soberano',
    institutions: [
      {
        id: 'siis-global',
        name: 'SIIS - Sovereign Ierahkwa International Settlement',
        code: 'SIIS',
        role: 'Banco central de los bancos centrales',
        description: 'Promueve la cooperación monetaria y financiera internacional',
        path: '/siis',
        icon: '🌐',
        functions: [
          'Liquidación internacional interbancaria',
          'Cooperación entre bancos centrales',
          'Estabilidad financiera global',
          'Estándares bancarios internacionales',
          'Investigación económica y financiera'
        ]
      },
      {
        id: 'ierahkwa-monetary-fund',
        name: 'Ierahkwa Monetary Fund (IMF)',
        code: 'IMF-I',
        role: 'Estabilidad monetaria internacional',
        description: 'Monitorea la estabilidad global y apoya el desarrollo',
        path: '/imf',
        icon: '🏛️',
        functions: [
          'Supervisión del sistema monetario',
          'Asistencia financiera de emergencia',
          'Políticas económicas sostenibles',
          'Reservas internacionales'
        ]
      },
      {
        id: 'ierahkwa-development-group',
        name: 'Ierahkwa Development Group (World Bank)',
        code: 'IDG',
        role: 'Desarrollo económico y reducción de pobreza',
        description: 'Financiamiento para desarrollo sostenible',
        path: '/development-group',
        icon: '🌍',
        functions: [
          'Financiamiento de proyectos',
          'Asistencia técnica',
          'Desarrollo de infraestructura',
          'Programas de reducción de pobreza'
        ]
      }
    ]
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 2: CLEARING HOUSE & INFRAESTRUCTURA DE PAGOS SOBERANA
  // ★ SISTEMA 100% INDEPENDIENTE - No dependemos de sistemas externos
  // ★ El mundo se conecta a NOSOTROS, no nosotros a ellos
  // ★ Conexiones externas (SWIFT, Visa) son OPCIONALES solo para compliance
  // ═══════════════════════════════════════════════════════════════════════════
  level2_ClearingInfrastructure: {
    name: 'Infraestructura de Compensación y Pagos SOBERANA',
    code: 'CLR',
    description: 'Sistema 100% independiente - El mundo se conecta a nosotros',
    sovereignty: {
      principle: 'FULL INDEPENDENCE - No dependemos de sistemas externos para operar',
      primary: 'Sistemas propios soberanos para transacciones locales',
      secondary: 'Conexiones externas OPCIONALES solo para cumplimiento regulatorio',
      motto: 'El mundo se conecta a nosotros, no nosotros a ellos'
    },
    // CLEARING HOUSE PRINCIPAL
    clearingHouse: {
      id: 'ierahkwa-clearing-house',
      name: 'IERAHKWA FUTUREHEAD BDET Clearing House',
      code: 'IERCLRH',
      swift: 'IERCLRHXXX',
      role: 'Cámara de Compensación Central',
      description: 'Procesa todas las transacciones entre bancos del sistema',
      path: '/clearhouse',
      icon: '🏛️',
      status: 'OPERATIONAL',
      connectedBanks: 'ALL',
      dailyVolume: '$50B+',
      functions: [
        'Compensación multilateral neta (Netting)',
        'Liquidación de operaciones interbancarias',
        'Gestión de riesgo de contraparte',
        'Administración de colateral y garantías',
        'Novación de contratos',
        'Reportes regulatorios en tiempo real',
        'Conexión con todos los bancos del sistema'
      ],
      services: {
        clearing: ['Securities Clearing', 'Derivatives Clearing', 'FX Clearing', 'Repo Clearing'],
        settlement: ['DVP Settlement', 'PVP Settlement', 'Net Settlement', 'Gross Settlement'],
        collateral: ['Margin Management', 'Default Management', 'Collateral Optimization']
      }
    },
    // RTGS - LIQUIDACIÓN BRUTA EN TIEMPO REAL
    rtgs: {
      id: 'ierahkwa-rtgs',
      name: 'IERAHKWA RTGS - Real-Time Gross Settlement',
      code: 'IERRTGS',
      role: 'Liquidación instantánea de alto valor',
      description: 'Pagos de alto valor liquidados en tiempo real, uno por uno',
      path: '/rtgs',
      icon: '⚡',
      status: 'OPERATIONAL',
      operatingHours: '24/7/365',
      minAmount: '$10,000',
      settlementTime: 'Instantáneo (<2 segundos)',
      functions: [
        'Liquidación bruta en tiempo real',
        'Pagos de alto valor (>$10,000)',
        'Irrevocabilidad inmediata',
        'Finalidad de pago garantizada',
        'Gestión de liquidez intradía'
      ]
    },
    // ACH - CÁMARA DE COMPENSACIÓN AUTOMATIZADA
    ach: {
      id: 'ierahkwa-ach',
      name: 'IERAHKWA ACH - Automated Clearing House',
      code: 'IERACH',
      role: 'Pagos automatizados en lote',
      description: 'Procesa pagos en lote: nóminas, débitos, transferencias',
      path: '/ach',
      icon: '🔄',
      status: 'OPERATIONAL',
      batchCycles: ['8:00 AM', '12:00 PM', '4:00 PM', '8:00 PM'],
      settlementTime: 'Same-day o Next-day',
      functions: [
        'Pagos de nómina masivos',
        'Débitos automáticos recurrentes',
        'Transferencias entre cuentas',
        'Pagos a proveedores',
        'Cobro de facturas',
        'Devoluciones y rechazos'
      ],
      transactionTypes: {
        credits: ['Direct Deposit', 'Payroll', 'Tax Refunds', 'Benefits'],
        debits: ['Bill Pay', 'Mortgage', 'Insurance', 'Subscriptions']
      }
    },
    // ═══════════════════════════════════════════════════════════════════════
    // ★ SISTEMAS SOBERANOS PRIMARIOS (100% INDEPENDIENTES)
    // ═══════════════════════════════════════════════════════════════════════
    // RED DE TARJETAS SOBERANA - NO DEPENDE DE VISA/MASTERCARD
    cardNetwork: {
      id: 'ierahkwa-card-network',
      name: 'WAMPUM - Red de Tarjetas Soberana',
      code: 'IERWMPUM',
      role: 'Red de tarjetas 100% PROPIA e INDEPENDIENTE',
      description: 'Sistema de tarjetas soberano - NO depende de Visa/Mastercard',
      path: '/card-network',
      icon: '💳',
      status: 'OPERATIONAL',
      sovereignty: 'FULL - Sistema propio, no dependemos de redes externas',
      cardTypes: ['Debit', 'Credit', 'Prepaid', 'Corporate', 'Virtual'],
      brands: ['WAMPUM Classic', 'WAMPUM Gold', 'WAMPUM Platinum', 'WAMPUM Black', 'WAMPUM Indigenous'],
      functions: [
        'Autorización de transacciones PROPIA',
        'Procesamiento de pagos SOBERANO',
        'Emisión de tarjetas PROPIA',
        'Prevención de fraude PROPIA',
        'Programa de lealtad PROPIO',
        'Red de ATM/POS PROPIA'
      ],
      fees: {
        interchange: '1.0%',
        international: '1.5%',
        atmWithdrawal: 'GRATIS en red WAMPUM'
      },
      acceptance: {
        internalNetwork: '100% - Todos los comercios IERAHKWA',
        externalNetworks: 'OPCIONAL - Visa/MC si ellos quieren conectarse'
      }
    },
    // SIIS - NUESTRO PROPIO SISTEMA DE LIQUIDACIÓN INTERNACIONAL
    siisInternal: {
      id: 'siis-internal',
      name: 'SIIS - Sistema de Liquidación Internacional Soberano',
      code: 'SIISINT',
      role: 'Nuestra propia red de mensajería y liquidación internacional',
      description: 'Alternativa soberana a SWIFT - 100% independiente',
      path: '/siis-internal',
      icon: '🌐',
      status: 'OPERATIONAL',
      sovereignty: 'FULL - No necesitamos SWIFT para operar',
      protocol: 'SIIS Protocol (compatible ISO 20022)',
      functions: [
        'Mensajería financiera soberana',
        'Liquidación internacional entre bancos IERAHKWA',
        'Transferencias instantáneas',
        'Sin dependencia de terceros',
        'Encriptación soberana'
      ],
      connectedBanks: 'Todos los bancos del sistema IERAHKWA'
    },
    // ═══════════════════════════════════════════════════════════════════════
    // ○ CONEXIONES EXTERNAS (OPCIONALES - Solo para compliance/interoperabilidad)
    // ═══════════════════════════════════════════════════════════════════════
    externalConnections: {
      disclaimer: 'OPCIONAL - Solo para cumplimiento regulatorio. NO dependemos de estos sistemas.',
      swiftGateway: {
        id: 'ierahkwa-swift',
        name: 'SWIFT Gateway (OPCIONAL)',
        code: 'IERSWIFT',
        role: 'Conexión OPCIONAL con red SWIFT',
        description: 'Solo para cuando bancos externos quieren conectarse a nosotros',
        path: '/swift-gateway',
        icon: '🔗',
        status: 'AVAILABLE',
        optional: true,
        bic: 'IABORWXXXX',
        note: 'Usamos SIIS internamente. SWIFT es solo para compliance externo.',
        messageTypes: ['MT103', 'MT202', 'MT940', 'MT950', 'ISO20022']
      },
      visaMastercard: {
        id: 'visa-mc-gateway',
        name: 'Visa/Mastercard Gateway (OPCIONAL)',
        code: 'VISMC',
        role: 'Interoperabilidad OPCIONAL con redes externas',
        description: 'Si visitantes quieren usar sus tarjetas Visa/MC aquí',
        path: '/visa-mc-gateway',
        icon: '💳',
        status: 'AVAILABLE',
        optional: true,
        note: 'WAMPUM es nuestra red primaria. Visa/MC es solo para visitantes.'
      }
    },
    // ═══════════════════════════════════════════════════════════════════════
    // ★★★ QUANTUM AI BANKING SYSTEM - OPERACIÓN 24/7 ★★★
    // Sistema de Inteligencia Artificial Cuántica operando el banco
    // ═══════════════════════════════════════════════════════════════════════
    quantumAISystem: {
      id: 'quantum-ai-banking',
      name: 'IERAHKWA Quantum AI Banking System',
      code: 'IERQAI',
      role: 'Sistema de IA Cuántica operando el banco 24/7',
      description: 'Infraestructura de computación cuántica e IA para operaciones bancarias autónomas',
      path: '/quantum-ai',
      icon: '🧠',
      status: 'OPERATIONAL 24/7/365',
      uptime: '99.9999%',
      // COMPONENTES DEL SISTEMA
      components: {
        quantumCore: {
          name: 'Quantum Processing Core',
          description: 'Núcleo de computación cuántica',
          qubits: 1000,
          capabilities: [
            'Optimización de portfolio cuántica',
            'Criptografía post-cuántica',
            'Análisis de riesgo cuántico',
            'Detección de fraude cuántica',
            'Simulación de mercados'
          ]
        },
        aiEngine: {
          name: 'AI Banking Engine',
          description: 'Motor de IA para decisiones bancarias',
          models: [
            'Risk Assessment AI',
            'Fraud Detection AI',
            'Credit Scoring AI',
            'Trading AI',
            'Customer Service AI',
            'Compliance AI',
            'Market Prediction AI'
          ]
        },
        autonomousOps: {
          name: 'Autonomous Operations',
          description: 'Operaciones bancarias autónomas 24/7',
          functions: [
            'Procesamiento de transacciones',
            'Liquidación automática',
            'Gestión de liquidez',
            'Rebalanceo de portfolios',
            'Ejecución de órdenes',
            'Monitoreo de cumplimiento',
            'Alertas en tiempo real'
          ]
        }
      },
      // SERVICIOS AI 24/7
      services24x7: {
        transactionProcessing: {
          name: 'Procesamiento de Transacciones',
          description: 'Procesa todas las transacciones automáticamente',
          capacity: '1M+ transacciones/segundo',
          latency: '<1ms'
        },
        riskManagement: {
          name: 'Gestión de Riesgo AI',
          description: 'Evalúa y gestiona riesgos en tiempo real',
          models: ['VaR', 'CVaR', 'Stress Testing', 'Monte Carlo'],
          frequency: 'Continuo'
        },
        fraudDetection: {
          name: 'Detección de Fraude Cuántica',
          description: 'Detecta fraudes usando algoritmos cuánticos',
          accuracy: '99.99%',
          responseTime: '<100ms'
        },
        tradingAI: {
          name: 'Trading AI Autónomo',
          description: 'Ejecuta estrategias de trading 24/7',
          strategies: ['Market Making', 'Arbitrage', 'Trend Following', 'Mean Reversion'],
          markets: ['Forex', 'Crypto', 'Commodities', 'Bonds']
        },
        customerService: {
          name: 'Servicio al Cliente AI',
          description: 'Atención al cliente 24/7 por IA',
          channels: ['Chat', 'Voice', 'Email', 'Video'],
          languages: ['Spanish', 'English', 'Portuguese', 'Indigenous languages'],
          resolution: '95% sin intervención humana'
        },
        compliance: {
          name: 'Compliance AI',
          description: 'Monitoreo de cumplimiento regulatorio',
          checks: ['AML', 'KYC', 'Sanctions', 'Transaction monitoring'],
          reporting: 'Automático a reguladores'
        },
        liquidityManagement: {
          name: 'Gestión de Liquidez AI',
          description: 'Optimiza liquidez del sistema bancario',
          functions: [
            'Predicción de flujos de caja',
            'Optimización de reservas',
            'Gestión de colateral',
            'Acceso a mercados interbancarios'
          ]
        }
      },
      // SEGURIDAD CUÁNTICA
      quantumSecurity: {
        encryption: 'Post-Quantum Cryptography (PQC)',
        keyDistribution: 'Quantum Key Distribution (QKD)',
        authentication: 'Quantum-resistant signatures',
        dataProtection: 'Quantum-safe encryption at rest and in transit'
      },
      // INFRAESTRUCTURA
      infrastructure: {
        datacenters: ['Primary (Sovereign Territory)', 'Backup (Distributed)', 'Disaster Recovery'],
        redundancy: 'Triple redundancy',
        failover: 'Automatic failover < 1 second',
        backup: 'Real-time replication'
      },
      // MONITOREO
      monitoring: {
        realtime: true,
        alerts: ['System health', 'Security', 'Performance', 'Anomalies'],
        dashboard: '/quantum-ai/dashboard',
        api: '/api/v1/quantum-ai/status'
      }
    },

    // ═══════════════════════════════════════════════════════════════════════
    // ★ ADQUIRENTE DE TARJETAS EXTRANJERAS - RECIBIMOS PAGOS DE ELLOS
    // Nosotros COBRAMOS a los extranjeros que usan Visa/Mastercard aquí
    // ═══════════════════════════════════════════════════════════════════════
    foreignCardAcquiring: {
      id: 'foreign-card-acquiring',
      name: 'Sistema Adquirente de Tarjetas Extranjeras',
      code: 'IERFCA',
      role: 'RECIBIMOS pagos de tarjetas Visa/Mastercard de extranjeros',
      description: 'Los extranjeros nos PAGAN a nosotros con sus tarjetas',
      path: '/foreign-acquiring',
      icon: '💰',
      status: 'OPERATIONAL',
      principle: 'Ellos nos pagan A NOSOTROS - No es dependencia, es un servicio que ofrecemos',
      supportedCards: {
        visa: { accepted: true, fee: '2.5%', note: 'Visa Internacional' },
        mastercard: { accepted: true, fee: '2.5%', note: 'Mastercard Internacional' },
        amex: { accepted: true, fee: '3.0%', note: 'American Express' },
        discover: { accepted: true, fee: '2.5%', note: 'Discover/Diners' },
        unionpay: { accepted: true, fee: '2.0%', note: 'UnionPay China' },
        jcb: { accepted: true, fee: '2.5%', note: 'JCB Japan' }
      },
      howItWorks: {
        step1: 'Extranjero usa su tarjeta Visa/MC en comercio IERAHKWA',
        step2: 'Transacción procesada por nuestro sistema WAMPUM',
        step3: 'Cobramos comisión del 2.5% al banco emisor extranjero',
        step4: 'Comercio recibe pago en WPM (moneda soberana)',
        step5: 'Nosotros RECIBIMOS divisas (USD, EUR, etc.)'
      },
      benefits: {
        forUs: [
          'Recibimos divisas extranjeras',
          'Cobramos comisiones a bancos externos',
          'Turistas pueden gastar en nuestro territorio',
          'No tenemos dependencia - ellos dependen de nosotros para operar aquí'
        ],
        forVisitors: [
          'Pueden usar sus tarjetas existentes',
          'No necesitan cambiar dinero',
          'Conveniencia para turistas'
        ]
      },
      fees: {
        merchantFee: '1.5%',
        foreignExchangeFee: '2.5%',
        processingFee: '$0.25 por transacción',
        settlement: 'T+1 en WPM'
      },
      terminals: {
        pos: 50000,
        atm: 5000,
        online: 'Todos los comercios e-commerce',
        mobile: 'App IERAHKWA'
      },
      settlementCurrency: 'WPM (Moneda Soberana)',
      note: 'Los comercios SIEMPRE reciben WPM. La conversión es automática.'
    },
    // ═══════════════════════════════════════════════════════════════════════
    // ★ RECEPCIÓN DE REMESAS - FAMILIAS RECIBEN DINERO DEL EXTERIOR
    // ═══════════════════════════════════════════════════════════════════════
    inboundRemittances: {
      id: 'inbound-remittances',
      name: 'Recepción de Remesas Internacionales',
      code: 'IERREMIT',
      role: 'Familias indígenas RECIBEN dinero del exterior',
      description: 'Sistema para recibir remesas de familiares en el extranjero',
      path: '/recibir-remesas',
      icon: '💸',
      status: 'OPERATIONAL',
      principle: 'El dinero ENTRA a nuestro sistema - Recibimos divisas',
      channels: {
        directDeposit: {
          name: 'Depósito Directo a Cuenta BDET',
          description: 'Remesa llega directo a cuenta del beneficiario',
          time: 'Instantáneo',
          fee: '0% para el receptor'
        },
        cashPickup: {
          name: 'Retiro en Efectivo',
          description: 'Beneficiario recoge en sucursal o agente',
          time: 'Disponible en minutos',
          fee: '0% para el receptor',
          locations: 'Sucursales bancarias, agentes autorizados, tiendas'
        },
        mobileWallet: {
          name: 'Wallet IERAHKWA',
          description: 'Recibe directo en billetera móvil',
          time: 'Instantáneo',
          fee: '0% para el receptor'
        },
        homeDelivery: {
          name: 'Entrega a Domicilio',
          description: 'Para comunidades remotas',
          time: '24-48 horas',
          fee: '$5 para entregas remotas'
        }
      },
      corridors: {
        usa: { name: 'USA → IERAHKWA', volume: 'Alto', senderFee: '1.5%' },
        canada: { name: 'Canadá → IERAHKWA', volume: 'Medio', senderFee: '1.5%' },
        europe: { name: 'Europa → IERAHKWA', volume: 'Medio', senderFee: '2.0%' },
        latam: { name: 'Latam → IERAHKWA', volume: 'Alto', senderFee: '1.0%' },
        asia: { name: 'Asia → IERAHKWA', volume: 'Bajo', senderFee: '2.5%' }
      },
      partners: {
        note: 'Estos son EMISORES que nos envían dinero - No dependemos de ellos',
        list: ['Western Union', 'MoneyGram', 'Remitly', 'Wise', 'WorldRemit', 'Xoom']
      },
      receiverBenefits: [
        'Sin comisión para recibir',
        'Tipo de cambio justo',
        'Múltiples opciones de recepción',
        'Notificación instantánea',
        'Retiro en WPM o efectivo local'
      ],
      annualVolume: '$2B+ USD recibidos'
    },
    // ═══════════════════════════════════════════════════════════════════════
    // ★ COBRO INTERNACIONAL - COBRAMOS A EMPRESAS EXTRANJERAS
    // ═══════════════════════════════════════════════════════════════════════
    internationalCollections: {
      id: 'international-collections',
      name: 'Cobros Internacionales',
      code: 'IERCOLL',
      role: 'Cobramos a empresas y gobiernos extranjeros',
      description: 'Sistema para recibir pagos de clientes internacionales',
      path: '/cobros-internacionales',
      icon: '🏦',
      status: 'OPERATIONAL',
      services: {
        wireTransfer: {
          name: 'Transferencia Bancaria Internacional',
          description: 'Recibimos wires de cualquier banco del mundo',
          currencies: ['USD', 'EUR', 'GBP', 'CAD', 'MXN', 'BRL', 'JPY', 'CNY'],
          settlementTime: 'T+1',
          fee: '0.1% (mínimo $10)'
        },
        achInternational: {
          name: 'ACH Internacional',
          description: 'Débitos automáticos de cuentas extranjeras',
          regions: ['USA (ACH)', 'Europe (SEPA)', 'UK (BACS)'],
          fee: '$5 por transacción'
        },
        checkCollection: {
          name: 'Cobro de Cheques Internacionales',
          description: 'Procesamos cheques de bancos extranjeros',
          clearingTime: '5-10 días',
          fee: '$25 por cheque'
        },
        cryptoReceive: {
          name: 'Recepción Crypto',
          description: 'Recibimos pagos en criptomonedas',
          currencies: ['BTC', 'ETH', 'USDT', 'USDC'],
          conversion: 'Automática a WPM',
          fee: '0.5%'
        }
      },
      useCases: [
        'Exportaciones de productos indígenas',
        'Turismo y servicios',
        'Reparaciones G2G (Gobierno a Gobierno)',
        'Licencias y royalties',
        'Inversión extranjera'
      ]
    },
    // PROCESADORES DE PAGO
    paymentProcessors: {
      id: 'ierahkwa-processors',
      name: 'IERAHKWA Payment Processors',
      code: 'IERPROC',
      role: 'Procesamiento de pagos retail',
      description: 'ATM, POS, Online, Mobile payments',
      path: '/payment-processors',
      icon: '💰',
      services: [
        {
          id: 'atm-network',
          name: 'Red ATM IERAHKWA',
          terminals: 5000,
          functions: ['Cash withdrawal', 'Deposits', 'Transfers', 'Balance inquiry']
        },
        {
          id: 'pos-network',
          name: 'Red POS IERAHKWA',
          terminals: 50000,
          functions: ['Card payments', 'Contactless', 'QR payments', 'Tips']
        },
        {
          id: 'online-gateway',
          name: 'Payment Gateway Online',
          path: '/payment-gateway',
          functions: ['E-commerce', 'Subscriptions', 'Invoicing', 'API integration']
        },
        {
          id: 'mobile-payments',
          name: 'Mobile Payments',
          path: '/mobile-pay',
          functions: ['NFC', 'QR codes', 'P2P transfers', 'Bill split']
        }
      ]
    }
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 3: REGULACIÓN Y SUPERVISIÓN BANCARIA
  // Superintendencia, Seguro de Depósitos, AML/KYC
  // ═══════════════════════════════════════════════════════════════════════════
  level3_Regulation: {
    name: 'Regulación y Supervisión Bancaria',
    code: 'REG',
    description: 'Entidades que regulan, supervisan y protegen el sistema financiero',
    institutions: [
      {
        id: 'superintendencia',
        name: 'Superintendencia de Bancos de IERAHKWA',
        code: 'SBI',
        role: 'Regulador y supervisor bancario principal',
        description: 'Autoridad que regula, supervisa y sanciona entidades financieras',
        path: '/superintendencia',
        icon: '⚖️',
        functions: [
          'Otorgamiento de licencias bancarias',
          'Supervisión prudencial de bancos',
          'Inspecciones in-situ y extra-situ',
          'Requisitos de capital (Basilea III)',
          'Sanciones y multas',
          'Intervención de bancos en problemas',
          'Protección al consumidor financiero'
        ],
        requirements: {
          capitalAdequacy: '8% mínimo (Tier 1)',
          liquidityCoverage: '100% LCR',
          leverage: '3% mínimo',
          reporting: 'Mensual obligatorio'
        }
      },
      {
        id: 'deposit-insurance',
        name: 'Fondo de Seguro de Depósitos (FSD)',
        code: 'FSD',
        role: 'Garantía de depósitos bancarios',
        description: 'Protege depósitos de ciudadanos hasta $250,000 por cuenta',
        path: '/deposit-insurance',
        icon: '🛡️',
        coverage: {
          amount: '$250,000 USD por depositante por banco',
          types: ['Cuentas corrientes', 'Cuentas de ahorro', 'Depósitos a plazo', 'Cuentas de jubilación'],
          exclusions: ['Inversiones', 'Acciones', 'Bonos', 'Crypto']
        },
        functions: [
          'Garantizar depósitos hasta $250,000',
          'Pago a depositantes en caso de quiebra',
          'Resolución ordenada de bancos fallidos',
          'Cobro de primas a bancos miembros',
          'Fondo de contingencia'
        ],
        fundSize: '$5B USD'
      },
      {
        id: 'bank-guarantee-fund',
        name: 'Fondo de Garantía Bancaria (FGB)',
        code: 'FGB',
        role: 'Estabilidad del sistema bancario',
        description: 'Fondo para rescate y estabilización de bancos en crisis',
        path: '/bank-guarantee',
        icon: '🏦',
        functions: [
          'Inyección de capital de emergencia',
          'Préstamos puente a bancos',
          'Garantías de liquidez',
          'Resolución de crisis bancarias',
          'Fusiones asistidas'
        ],
        fundSize: '$10B USD'
      },
      {
        id: 'aml-kyc-central',
        name: 'Unidad de Inteligencia Financiera (UIF)',
        code: 'UIF',
        role: 'Prevención de lavado de dinero',
        description: 'Centro de análisis y prevención de delitos financieros',
        path: '/uif',
        icon: '🔍',
        functions: [
          'Análisis de reportes de operaciones sospechosas',
          'Coordinación con autoridades internacionales',
          'Listas de sanciones (OFAC, UN, EU)',
          'KYC centralizado',
          'Monitoreo de transacciones',
          'Investigación de fraudes'
        ],
        compliance: ['FATF', 'Basel AML', 'Wolfsberg Principles']
      },
      {
        id: 'credit-bureau',
        name: 'Buró de Crédito IERAHKWA',
        code: 'BCI',
        role: 'Historial crediticio centralizado',
        description: 'Base de datos de historial crediticio de personas y empresas',
        path: '/credit-bureau',
        icon: '📊',
        functions: [
          'Registro de historial crediticio',
          'Scoring crediticio',
          'Reportes de crédito',
          'Alertas de fraude de identidad',
          'Consultas autorizadas'
        ],
        scoreRange: { min: 300, max: 850 },
        factors: ['Historial de pagos', 'Utilización de crédito', 'Antigüedad', 'Tipos de crédito', 'Consultas']
      }
    ]
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 4: BANCOS CENTRALES - INSTITUCIONES PÚBLICAS AUTÓNOMAS
  // Política monetaria, emisión de moneda, supervisión bancaria
  // Operan como "banco de bancos" y del gobierno soberano
  // ═══════════════════════════════════════════════════════════════════════════
  level4_CentralBanks: {
    name: 'Bancos Centrales Soberanos',
    code: 'CB',
    description: 'Instituciones públicas autónomas que gestionan política monetaria',
    // FUNCIONES PRINCIPALES DE UN BANCO CENTRAL
    coreFunctions: {
      priceStability: {
        name: 'Estabilidad de Precios',
        description: 'Objetivo principal: controlar inflación para mantener valor del dinero',
        target: '2-3% inflación anual',
        tools: ['Tasas de interés', 'Reserva legal', 'Operaciones de mercado abierto']
      },
      monetaryPolicy: {
        name: 'Política Monetaria',
        description: 'Ajustar tasas de referencia para influir en gasto e inversión',
        operations: [
          'Fijar tasas de interés de referencia',
          'Subir tasas para frenar inflación',
          'Bajar tasas para estimular economía',
          'Flexibilización cuantitativa (QE) en crisis'
        ]
      },
      currencyIssuance: {
        name: 'Emisión de Moneda',
        description: 'Derecho exclusivo de emitir moneda de curso legal',
        currencies: ['WPM Token', 'ISB Token', 'Stablecoins soberanas']
      },
      bankOfBanks: {
        name: 'Banco de Bancos',
        description: 'Custodia reservas bancarias y facilita liquidación de pagos',
        services: [
          'Custodia de reservas bancarias',
          'Liquidación de pagos interbancarios',
          'Prestamista de última instancia',
          'Garantía de liquidez del sistema'
        ]
      },
      forexManagement: {
        name: 'Gestión de Divisas',
        description: 'Administrar reservas internacionales y estabilizar moneda',
        operations: [
          'Gestión de reservas internacionales',
          'Intervención en mercado cambiario',
          'Estabilización de moneda nacional',
          'Acuerdos swap con otros bancos centrales'
        ]
      },
      supervision: {
        name: 'Supervisión Bancaria',
        description: 'Monitorear infraestructura de pagos y regular bancos',
        scope: [
          'Supervisión prudencial de bancos',
          'Infraestructura de pagos',
          'Requisitos de capital (Basilea III)',
          'Licencias bancarias'
        ]
      }
    },
    // INSTRUMENTOS DE POLÍTICA MONETARIA
    monetaryInstruments: {
      interestRates: {
        name: 'Tasas de Interés',
        description: 'Tasa de referencia para el sistema bancario',
        currentRate: 5.25,
        corridor: { floor: 5.00, ceiling: 5.50 }
      },
      openMarketOps: {
        name: 'Operaciones de Mercado Abierto',
        description: 'Compra/venta de valores gubernamentales',
        types: ['Repos', 'Reverse Repos', 'Compra directa', 'QE']
      },
      reserveRequirements: {
        name: 'Encaje Legal',
        description: 'Porcentaje de depósitos que bancos deben mantener en reserva',
        rate: 10,
        calculation: 'Promedio diario de depósitos'
      },
      discountWindow: {
        name: 'Ventana de Descuento',
        description: 'Préstamos de emergencia a bancos comerciales',
        rate: 5.75,
        collateral: ['Bonos soberanos', 'Títulos AAA']
      }
    },
    institutions: [
      {
        id: 'bdet-central',
        name: 'BDET Central Bank',
        code: 'IERBDETXXX',
        swift: 'IERBDETXXX',
        role: 'Banco Central Soberano Principal',
        description: 'Banco central de bancos centrales del sistema Ierahkwa',
        path: '/bdet-bank',
        icon: '🏦',
        founded: 2020,
        independence: 'Autónomo del gobierno para evitar presión política',
        functions: [
          'Emisión de moneda soberana (WPM, ISB Token)',
          'Política monetaria para pueblos indígenas de las Américas',
          'Reservas internacionales en oro, crypto y divisas',
          'Supervisión de todos los bancos del sistema',
          'Estabilidad de precios objetivo 2%',
          'Prestamista de última instancia',
          'Liquidación interbancaria RTGS',
          'Gestión de tipo de cambio'
        ],
        reserves: {
          gold: '50,000 oz',
          crypto: '10,000 BTC, 100,000 ETH',
          fiat: 'USD 5B equivalent',
          sdrs: '500M SDRs'
        }
      },
      {
        id: 'aguila-central',
        name: 'Banco Central Águila',
        code: 'IERAGUILAX',
        swift: 'IERAGUILAX',
        role: 'Banco Central Regional - Norte',
        description: 'Naciones Indígenas de Norte América (Turtle Island)',
        path: '/central-banks/aguila',
        icon: '🦅',
        region: 'Norte',
        coverage: ['Navajo', 'Cherokee', 'Lakota', 'Ojibwe', 'Haudenosaunee', 'Apache', 'Seminole'],
        headquarters: 'Territorio Navajo',
        functions: [
          'Política monetaria regional Norte',
          'Supervisión bancaria regional',
          'Reservas en WPM y USD',
          'Liquidación pagos tribales',
          'Financiamiento desarrollo indígena'
        ]
      },
      {
        id: 'quetzal-central',
        name: 'Banco Central Quetzal',
        code: 'IERQUETZAX',
        swift: 'IERQUETZAX',
        role: 'Banco Central Regional - Centro',
        description: 'Naciones Indígenas de Centro América y México',
        path: '/central-banks/quetzal',
        icon: '🐦',
        region: 'Centro',
        coverage: ['Maya', 'Azteca', 'Zapoteca', 'Mixteca', 'Kuna', 'Garifuna', 'Lenca'],
        headquarters: 'Territorio Maya',
        functions: [
          'Política monetaria regional Centro',
          'Supervisión bancaria regional',
          'Reservas en WPM y MXN/GTQ',
          'Liquidación pagos regionales',
          'Desarrollo económico Maya-Azteca'
        ]
      },
      {
        id: 'condor-central',
        name: 'Banco Central Cóndor',
        code: 'IERCONDORX',
        swift: 'IERCONDORX',
        role: 'Banco Central Regional - Sur',
        description: 'Naciones Indígenas de Sur América',
        path: '/central-banks/condor',
        icon: '🦅',
        region: 'Sur',
        coverage: ['Quechua', 'Aymara', 'Mapuche', 'Guaraní', 'Yanomami', 'Wayuu'],
        headquarters: 'Territorio Quechua',
        functions: [
          'Política monetaria regional Sur',
          'Supervisión bancaria regional',
          'Reservas en WPM y monedas locales',
          'Liquidación pagos Andinos',
          'Desarrollo económico Andino-Amazónico'
        ]
      },
      {
        id: 'caribe-central',
        name: 'Banco Central Caribe',
        code: 'IERCARIBXX',
        swift: 'IERCARIBXX',
        role: 'Banco Central Regional - Caribe',
        description: 'Naciones Indígenas del Caribe',
        path: '/central-banks/caribe',
        icon: '🌴',
        region: 'Caribe',
        coverage: ['Taíno', 'Kalinago', 'Garifuna', 'Arawak', 'Ciboney'],
        headquarters: 'Territorio Taíno',
        functions: [
          'Política monetaria regional Caribe',
          'Supervisión bancaria regional',
          'Reservas en WPM y USD',
          'Liquidación pagos insulares',
          'Desarrollo económico caribeño'
        ]
      }
    ]
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 5: BANCA DE DESARROLLO
  // Financiamiento de actividades económicas específicas, comercio exterior
  // ═══════════════════════════════════════════════════════════════════════════
  level5_DevelopmentBanks: {
    name: 'Banca de Desarrollo',
    code: 'DEV',
    description: 'Financiamiento de actividades económicas y promoción de exportaciones',
    institutions: [
      {
        id: 'futurehead-dev',
        name: 'Futurehead Development Bank',
        code: 'FHDEV',
        role: 'Banco de Desarrollo Principal',
        description: 'Financiamiento de proyectos estratégicos',
        path: '/futurehead',
        icon: '🏗️',
        functions: [
          'Financiamiento de infraestructura',
          'Proyectos de tecnología',
          'Desarrollo empresarial',
          'Inversión en innovación'
        ]
      },
      {
        id: 'bancomext-ierahkwa',
        name: 'BANCOMEXT Ierahkwa',
        code: 'BCOMEXT',
        role: 'Banco de Comercio Exterior',
        description: 'Promoción de exportaciones y comercio internacional',
        path: '/bancomext',
        icon: '🚢',
        functions: [
          'Financiamiento de exportaciones',
          'Cartas de crédito',
          'Seguros de crédito',
          'Asesoría en comercio exterior'
        ]
      },
      {
        id: 'agricultural-dev',
        name: 'Banco de Desarrollo Agrícola',
        code: 'AGRIDEV',
        role: 'Desarrollo Agrícola y Rural',
        description: 'Financiamiento del sector agropecuario',
        path: '/farmfactory',
        icon: '🌾',
        functions: [
          'Créditos agrícolas',
          'Financiamiento de cosechas',
          'Desarrollo rural',
          'Agroindustria'
        ]
      },
      {
        id: 'citizen-dev',
        name: 'Banco de Desarrollo Ciudadano',
        code: 'CITDEV',
        role: 'Desarrollo de PyMEs y Emprendedores',
        description: 'Tokenización y lanzamiento de negocios ciudadanos',
        path: '/citizen-launchpad',
        icon: '🚀',
        functions: [
          'Microfinanzas',
          'Tokenización de negocios',
          'IDO/IEO para ciudadanos',
          'Incubación de startups'
        ]
      }
    ]
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 6: BANCOS REGIONALES - MEDIANOS ($10B-$100B activos)
  // Enfoque geográfico, atención personalizada, conocimiento del mercado local
  // Puente entre banca comunitaria y grandes bancos nacionales
  // ═══════════════════════════════════════════════════════════════════════════
  level6_RegionalBanks: {
    name: 'Bancos Regionales',
    code: 'REG',
    description: 'Instituciones financieras medianas con enfoque geográfico específico',
    characteristics: {
      assetRange: '$10,000M - $100,000M USD',
      focus: 'Servicio completo en áreas geográficas específicas',
      strength: 'Atención personalizada y conocimiento del mercado local',
      agility: 'Más ágiles en decisiones que bancos nacionales',
      services: ['Depósitos', 'Préstamos', 'Hipotecas', 'Tarjetas', 'PyMEs', 'Seguros'],
      insurance: 'Depósitos asegurados hasta $250,000 por cuenta',
      branches: 'Presencia física en múltiples ciudades/estados'
    },
    institutions: [
      {
        id: 'aguila-regional',
        name: 'Banco Regional Águila',
        code: 'IERAGLREG',
        swift: 'IERAGLREG',
        role: 'Banco Regional - Naciones del Norte',
        description: 'Servicio bancario para comunidades indígenas de Norte América',
        path: '/regional-banks/aguila',
        icon: '🦅',
        region: 'Norte',
        assets: '$45,000M',
        branches: 150,
        coverage: ['Navajo Nation', 'Cherokee Nation', 'Lakota Territory', 'Haudenosaunee'],
        services: [
          'Cuentas corrientes y ahorro tribales',
          'Préstamos para vivienda en tierras tribales',
          'Financiamiento PyMEs indígenas',
          'Tarjetas de crédito/débito',
          'Banca móvil en territorios remotos',
          'Remesas familiares'
        ],
        businessModel: 'retail'
      },
      {
        id: 'quetzal-regional',
        name: 'Banco Regional Quetzal',
        code: 'IERQTZREG',
        swift: 'IERQTZREG',
        role: 'Banco Regional - Naciones del Centro',
        description: 'Servicio bancario para comunidades Maya-Azteca',
        path: '/regional-banks/quetzal',
        icon: '🐦',
        region: 'Centro',
        assets: '$38,000M',
        branches: 200,
        coverage: ['Maya Territory', 'Azteca Territory', 'Zapoteca', 'Kuna Yala'],
        services: [
          'Cuentas en WPM y monedas locales',
          'Microcréditos artesanales',
          'Financiamiento agrícola',
          'Préstamos cooperativos',
          'Banca comunitaria'
        ],
        businessModel: 'retail'
      },
      {
        id: 'condor-regional',
        name: 'Banco Regional Cóndor',
        code: 'IERCNDREG',
        swift: 'IERCNDREG',
        role: 'Banco Regional - Naciones del Sur',
        description: 'Servicio bancario para comunidades Andinas y Amazónicas',
        path: '/regional-banks/condor',
        icon: '🦅',
        region: 'Sur',
        assets: '$52,000M',
        branches: 180,
        coverage: ['Quechua Territory', 'Aymara Territory', 'Mapuche', 'Guaraní', 'Amazonia'],
        services: [
          'Cuentas comunitarias',
          'Préstamos agrícolas Andinos',
          'Financiamiento minería artesanal',
          'Comercio justo y exportación',
          'Banca rural'
        ],
        businessModel: 'retail'
      },
      {
        id: 'caribe-regional',
        name: 'Banco Regional Caribe',
        code: 'IERCRBREG',
        swift: 'IERCRBREG',
        role: 'Banco Regional - Naciones del Caribe',
        description: 'Servicio bancario para comunidades Taíno y Caribeñas',
        path: '/regional-banks/caribe',
        icon: '🌴',
        region: 'Caribe',
        assets: '$28,000M',
        branches: 95,
        coverage: ['Taíno Territory', 'Kalinago', 'Garifuna', 'Arawak'],
        services: [
          'Cuentas turismo comunitario',
          'Préstamos pesca artesanal',
          'Financiamiento hotelería indígena',
          'Remesas internacionales',
          'Banca offshore'
        ],
        businessModel: 'retail'
      }
    ]
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 7: BANCOS NACIONALES - MODELO PANAMÁ
  // Bancos Oficiales (Estado) + Bancos Privados con licencia
  // ═══════════════════════════════════════════════════════════════════════════
  level7_NationalBanks: {
    name: 'Bancos Nacionales',
    code: 'NAT',
    description: 'Bancos oficiales y privados bajo supervisión de Superintendencia',
    // TIPOS DE LICENCIA BANCARIA
    licenses: {
      general: {
        name: 'Licencia General',
        code: 'LG',
        description: 'Operaciones bancarias nacionales e internacionales',
        scope: 'Puede operar dentro y fuera del territorio',
        requirements: {
          capitalMinimo: '$10,000,000 USD',
          reservaLegal: '10% de utilidades',
          encaje: '10% de depósitos',
          auditoria: 'Anual externa obligatoria'
        }
      },
      international: {
        name: 'Licencia Internacional',
        code: 'LI',
        description: 'Solo operaciones offshore con no residentes',
        scope: 'Operaciones exclusivamente internacionales',
        requirements: {
          capitalMinimo: '$3,000,000 USD',
          reservaLegal: '10% de utilidades',
          clientela: 'Solo no residentes'
        }
      },
      representation: {
        name: 'Licencia de Representación',
        code: 'LR',
        description: 'Oficinas de representación de bancos extranjeros',
        scope: 'Promoción y enlace, sin operaciones bancarias',
        requirements: {
          capitalMinimo: '$250,000 USD',
          actividades: 'Solo promoción y contacto'
        }
      }
    },
    // BANCOS OFICIALES (ESTADO)
    official: {
      name: 'Bancos Oficiales del Estado',
      description: 'Bancos estatales bajo supervisión de Superintendencia',
      institutions: [
        {
          id: 'banconal-ierahkwa',
          name: 'Banco Nacional de Ierahkwa (BANCONAL)',
          code: 'IERBNCNAL',
          swift: 'IERBNCNAL',
          role: 'Agente financiero principal del Estado Soberano',
          description: 'Banco estatal con más de 90 sucursales',
          path: '/banconal',
          icon: '🏛️',
          founded: 2020,
          license: 'general',
          branches: 95,
          functions: [
            'Agente financiero del gobierno soberano',
            'Custodia de fondos públicos',
            'Pago de nómina gubernamental',
            'Recaudación de impuestos',
            'Financiamiento de obras públicas',
            'Banca comercial ciudadana',
            'Préstamos hipotecarios',
            'Tarjetas de crédito/débito'
          ],
          products: [
            'Cuenta Soberana (sin comisiones para indígenas)',
            'Préstamo Vivienda Indígena',
            'Crédito PyME Comunitario',
            'Tarjeta Águila Dorada',
            'Depósitos a plazo fijo',
            'Certificados de inversión'
          ]
        },
        {
          id: 'caja-ahorros',
          name: 'Caja de Ahorros Ierahkwa',
          code: 'IERCAJAHO',
          swift: 'IERCAJAHO',
          role: 'Institución de ahorro y vivienda',
          description: 'Enfocada en ahorro popular y financiamiento de vivienda',
          path: '/caja-ahorros',
          icon: '🏠',
          founded: 2020,
          license: 'general',
          branches: 60,
          functions: [
            'Fomento del ahorro popular',
            'Financiamiento de vivienda social',
            'Préstamos hipotecarios accesibles',
            'Ahorro programado',
            'Microseguros'
          ],
          products: [
            'Cuenta de Ahorro Programado',
            'Préstamo Vivienda Social',
            'Ahorro Escolar',
            'Ahorro Navideño',
            'Microseguro de vida'
          ]
        }
      ]
    },
    // BANCOS PRIVADOS CON LICENCIA GENERAL
    private: {
      name: 'Bancos Privados con Licencia General',
      description: 'Bancos privados autorizados para operaciones nacionales e internacionales',
      institutions: [
        {
          id: 'super-bank-global',
          name: 'Super Bank Global',
          code: 'IERSBGLOB',
          swift: 'IERSBGLOB',
          role: 'Banca comercial global - El mundo se conecta a nosotros',
          description: 'Mayor banco privado del sistema, SWIFT, SIIS, ISO 20022',
          path: '/super-bank-global',
          icon: '🌐',
          license: 'general',
          assets: '$150,000M',
          branches: 45,
          businessModel: 'wholesale',
          services: [
            'Banca corporativa internacional',
            'Trade Finance',
            'Cartas de crédito',
            'Corresponsalía bancaria',
            'Treasury services',
            'FX y derivados'
          ]
        },
        {
          id: 'banco-general-ier',
          name: 'Banco General Ierahkwa',
          code: 'IERBANGEN',
          swift: 'IERBANGEN',
          role: 'Banco privado líder en activos',
          description: 'Uno de los mayores bancos por activos',
          path: '/banco-general',
          icon: '🏦',
          license: 'general',
          assets: '$85,000M',
          branches: 70,
          businessModel: 'retail',
          services: ['Banca personal', 'Banca empresarial', 'Inversiones', 'Seguros']
        },
        {
          id: 'banco-taino',
          name: 'Banco Taíno S.A.',
          code: 'IERTAINO',
          swift: 'IERTAINO',
          role: 'Banco del Caribe Indígena',
          description: 'Enfocado en comunidades Taíno y caribeñas',
          path: '/banco-taino',
          icon: '🌴',
          license: 'general',
          region: 'Caribe',
          assets: '$12,000M',
          branches: 35,
          businessModel: 'retail'
        },
        {
          id: 'banco-maya',
          name: 'Banco Maya S.A.',
          code: 'IERMAYA',
          swift: 'IERMAYA',
          role: 'Banco de Mesoamérica',
          description: 'Enfocado en comunidades Maya y Centro América',
          path: '/banco-maya',
          icon: '🐆',
          license: 'general',
          region: 'Centro',
          assets: '$18,000M',
          branches: 55,
          businessModel: 'retail'
        },
        {
          id: 'banco-inca',
          name: 'Banco Inca S.A.',
          code: 'IERINCA',
          swift: 'IERINCA',
          role: 'Banco Andino',
          description: 'Enfocado en comunidades Quechua, Aymara y Andinas',
          path: '/banco-inca',
          icon: '⛰️',
          license: 'general',
          region: 'Sur',
          assets: '$22,000M',
          branches: 65,
          businessModel: 'retail'
        },
        {
          id: 'banco-azteca-sov',
          name: 'Banco Azteca Soberano S.A.',
          code: 'IERAZTECA',
          swift: 'IERAZTECA',
          role: 'Banco del Norte Indígena',
          description: 'Enfocado en comunidades de México y Norte América',
          path: '/banco-azteca',
          icon: '🦅',
          license: 'general',
          region: 'Norte',
          assets: '$25,000M',
          branches: 80,
          businessModel: 'retail'
        }
      ]
    },
    // BANCOS INTERNACIONALES (Licencia Internacional - Offshore)
    international: {
      name: 'Bancos con Licencia Internacional',
      description: 'Operaciones exclusivamente con no residentes (offshore)',
      institutions: [
        {
          id: 'bladex-ier',
          name: 'BLADEX Ierahkwa',
          code: 'IERBLADEX',
          swift: 'IERBLADEX',
          role: 'Banco de comercio exterior latinoamericano',
          description: 'Financiamiento de comercio internacional',
          path: '/bladex',
          icon: '🚢',
          license: 'international',
          services: ['Trade Finance', 'Syndicated loans', 'Treasury']
        },
        {
          id: 'offshore-trust',
          name: 'Ierahkwa Offshore Trust Bank',
          code: 'IEROFFSHO',
          swift: 'IEROFFSHO',
          role: 'Banca offshore y trust',
          description: 'Servicios fiduciarios y banca privada internacional',
          path: '/offshore-trust',
          icon: '🔐',
          license: 'international',
          services: ['Private Banking', 'Trust services', 'Asset protection', 'Estate planning']
        }
      ]
    },
    // SUCURSALES DE BANCOS EXTRANJEROS
    foreignBranches: {
      name: 'Sucursales de Bancos Extranjeros',
      description: 'Bancos internacionales conectados al sistema BDET',
      institutions: [
        { id: 'citi-ier', name: 'Citibank Ierahkwa Branch', swift: 'CITIUS33', status: 'CONNECTED', license: 'general' },
        { id: 'jpm-ier', name: 'JP Morgan Ierahkwa Branch', swift: 'CHASUS33', status: 'CONNECTED', license: 'general' },
        { id: 'hsbc-ier', name: 'HSBC Ierahkwa Branch', swift: 'HSBCHKHH', status: 'CONNECTED', license: 'general' },
        { id: 'santander-ier', name: 'Santander Ierahkwa Branch', swift: 'BSCHESMM', status: 'CONNECTED', license: 'general' },
        { id: 'deutsche-ier', name: 'Deutsche Bank Ierahkwa Branch', swift: 'DEUTDEFF', status: 'CONNECTED', license: 'general' },
        { id: 'boa-ier', name: 'Bank of America Ierahkwa Branch', swift: 'BOFAUS3N', status: 'CONNECTED', license: 'general' },
        { id: 'ubs-ier', name: 'UBS Ierahkwa Branch', swift: 'UBSWCHZH', status: 'CONNECTED', license: 'international' },
        { id: 'credit-suisse-ier', name: 'Credit Suisse Ierahkwa Branch', swift: 'CRESCHZZ', status: 'CONNECTED', license: 'international' }
      ]
    }
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 8: BANCOS COMERCIALES - RETAIL Y CORPORATIVO
  // Depósitos, préstamos, comercio exterior, banca corporativa
  // ═══════════════════════════════════════════════════════════════════════════
  level8_CommercialBanks: {
    name: 'Banca Comercial',
    code: 'COM',
    description: 'Servicios bancarios de depósitos, préstamos y comercio',
    categories: {
      // ═══════════════════════════════════════════════════════════════════════
      // ★ BANCA CIUDADANA - 100% TRUST ACCOUNTS
      // Todas las cuentas de ciudadanos son administradas como Trust
      // ═══════════════════════════════════════════════════════════════════════
      citizenBanking: {
        name: 'Banca Ciudadana (100% Trust-Based)',
        description: 'Todas las cuentas de ciudadanos son Trust Accounts',
        businessModel: 'trust_retail',
        trustCompany: 'IERAHKWA Citizen Trust Company',
        principle: 'Ciudadano = Beneficiario de su propio Trust',
        services: [
          {
            id: 'wallet',
            name: 'IERAHKWA Trust Wallet',
            description: 'Billetera digital dentro del Trust del ciudadano',
            path: '/wallet',
            icon: '👛',
            accountType: 'Trust Digital Wallet',
            products: ['Pagos móviles', 'Transferencias P2P', 'QR payments', 'Contactless'],
            trustProtection: true
          },
          {
            id: 'bdet-accounts',
            name: 'BDET Trust Accounts',
            description: 'Cuentas Trust para ciudadanos soberanos',
            path: '/bdet-accounts',
            icon: '📒',
            accountType: 'Citizen Trust Account',
            products: [
              'Trust Checking Account (Cuenta corriente)',
              'Trust Savings Account (Cuenta de ahorro)',
              'Trust CD (Depósito a plazo)',
              'Trust Payroll Account (Cuenta nómina)'
            ],
            trustProtection: true,
            assetProtection: 'Protegido contra embargo y acreedores'
          },
          {
            id: 'family-accounts',
            name: 'Family Trust Accounts',
            description: 'Cuentas Trust familiares',
            path: '/family-trust',
            icon: '👨‍👩‍👧‍👦',
            accountType: 'Family Trust Account',
            products: [
              'Joint Family Trust',
              'Children Trust Account',
              'Education Trust',
              'Emergency Family Fund'
            ],
            beneficiaries: 'Múltiples miembros familiares'
          },
          {
            id: 'bank-worker',
            name: 'Bank Worker Portal',
            description: 'Portal de operaciones bancarias para Trust Accounts',
            path: '/bank-worker',
            icon: '🏦',
            products: ['Trust Account Management', 'Beneficiary Services', 'Trust Reporting']
          }
        ],
        citizenRights: [
          'Todas las cuentas son Trust Accounts por defecto',
          'Ciudadano es beneficiario 100%',
          'Protección automática de activos',
          'Privacidad garantizada',
          'Sucesión sin proceso judicial',
          'Sin embargo por deudas personales'
        ]
      },
      corporate: {
        name: 'Banca Corporativa',
        description: 'Servicios para empresas grandes y corporaciones',
        businessModel: 'wholesale',
        services: [
          {
            id: 'corporate-banking',
            name: 'Corporate Banking',
            description: 'Servicios bancarios corporativos',
            path: '/corporate',
            icon: '🏢',
            products: ['Cash management', 'Payroll', 'Collections', 'Disbursements']
          },
          {
            id: 'trade-finance',
            name: 'Trade Finance',
            description: 'Financiamiento de comercio internacional',
            path: '/trade-finance',
            icon: '🚢',
            products: ['Cartas de crédito', 'Cobranzas', 'Garantías', 'Forfaiting']
          }
        ]
      }
    }
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // NIVEL 9: SERVICIOS FINANCIEROS ESPECIALIZADOS
  // Trading, DeFi, Crypto, Mercados
  // ═══════════════════════════════════════════════════════════════════════════
  level9_SpecializedServices: {
    name: 'Servicios Financieros Especializados',
    code: 'SFS',
    description: 'Trading, DeFi, Crypto, Cámara de Compensación',
    categories: {
      trading: {
        name: 'Trading & Exchange',
        businessModel: 'trading', // Banca de negociación
        services: [
          {
            id: 'mamey-futures',
            name: 'Mamey Futures',
            description: 'Futures, Options, Perpetuals, 100x leverage',
            path: '/mamey-futures',
            icon: '🎯',
            products: ['Futures', 'Options', 'Perpetuals', 'Commodities']
          },
          {
            id: 'tradex',
            name: 'TradeX Exchange',
            description: 'Spot, P2P, Staking, Derivados',
            path: '/tradex',
            icon: '📈',
            products: ['Spot Trading', 'P2P', 'Staking', 'Margin Trading']
          },
          {
            id: 'forex',
            name: 'Forex Trading',
            description: 'Cambio de divisas en tiempo real',
            path: '/forex',
            icon: '💱',
            products: ['FX Spot', 'FX Forwards', 'Currency Pairs']
          }
        ]
      },
      defi: {
        name: 'DeFi & Blockchain',
        businessModel: 'trading',
        services: [
          {
            id: 'net10',
            name: 'NET10 DeFi',
            description: 'Swap, Pools, Yield Farming',
            path: '/net10',
            icon: '🌐',
            products: ['DEX', 'Liquidity Pools', 'Yield Farming', 'Staking']
          },
          {
            id: 'bridge',
            name: 'Bridge',
            description: 'Puente entre cadenas blockchain',
            path: '/bridge',
            icon: '🔗',
            products: ['Cross-chain Transfer', 'Token Wrapping']
          },
          {
            id: 'dao',
            name: 'DAO Governance',
            description: 'Gobernanza descentralizada',
            path: '/dao',
            icon: '🗳️',
            products: ['Voting', 'Proposals', 'Treasury']
          }
        ]
      },
      crypto: {
        name: 'Crypto & Digital Assets',
        businessModel: 'wholesale',
        services: [
          {
            id: 'cryptohost',
            name: 'CryptoHost - Global Asset Monetization System',
            description: 'Monetización de activos, conversión M0-M4, cobro de bonos históricos',
            path: '/cryptohost',
            icon: '₿',
            status: 'OPERATIONAL 24/7',
            // ═══════════════════════════════════════════════════════════════
            // ★ CRYPTOHOST - SISTEMA GLOBAL DE MONETIZACIÓN
            // ═══════════════════════════════════════════════════════════════
            globalServices: {
              assetMonetization: {
                name: 'Monetización de Activos',
                description: 'Convertir cualquier activo en valor digital',
                assets: [
                  'Bonos históricos',
                  'Títulos de deuda',
                  'Propiedades',
                  'Commodities',
                  'Recursos naturales',
                  'Propiedad intelectual',
                  'Contratos futuros'
                ]
              },
              historicalBonds: {
                name: 'Cobro de Bonos Históricos',
                description: 'Sistema para monetizar y cobrar bonos históricos',
                types: [
                  'Bonos de reparación histórica',
                  'Bonos soberanos antiguos',
                  'Títulos de deuda G2G',
                  'Certificados de indemnización',
                  'Bonos de guerra',
                  'Títulos coloniales'
                ],
                process: [
                  'Verificación de autenticidad',
                  'Valuación histórica',
                  'Conversión a valor actual',
                  'Tokenización',
                  'Liquidación en crypto o fiat'
                ]
              },
              // CONVERSIÓN M0-M4 (Agregados Monetarios) con % de Desvaluación
              moneySupplyConversion: {
                name: 'Conversión de Agregados Monetarios M0-M4',
                description: 'Convertir activos entre diferentes niveles de liquidez con tasa de desvaluación',
                devaluationNote: 'El % de desvaluación depende del tipo de activo, antigüedad y condiciones de mercado',
                levels: {
                  M0: {
                    name: 'M0 - Base Monetaria',
                    description: 'Efectivo en circulación + reservas bancarias',
                    assets: ['Cash', 'Reservas en Banco Central', 'Crypto base'],
                    convertTo: ['WPM Cash', 'BTC', 'Stablecoins'],
                    devaluation: {
                      rate: '0-2%',
                      reason: 'Mínima desvaluación - activos más líquidos',
                      factors: ['Inflación anual', 'Política monetaria']
                    }
                  },
                  M1: {
                    name: 'M1 - Dinero Estrecho',
                    description: 'M0 + depósitos a la vista',
                    assets: ['M0', 'Cuentas corrientes', 'Depósitos a la vista'],
                    convertTo: ['USDT', 'USDC', 'WPM Token'],
                    devaluation: {
                      rate: '2-5%',
                      reason: 'Baja desvaluación - alta liquidez',
                      factors: ['Inflación', 'Tasas de interés', 'Tipo de cambio']
                    }
                  },
                  M2: {
                    name: 'M2 - Dinero Amplio',
                    description: 'M1 + depósitos de ahorro + depósitos a plazo corto',
                    assets: ['M1', 'Cuentas de ahorro', 'CDs < 1 año', 'Money Market'],
                    convertTo: ['Yield tokens', 'Staking tokens', 'DeFi LP tokens'],
                    devaluation: {
                      rate: '5-10%',
                      reason: 'Desvaluación moderada - liquidez media',
                      factors: ['Inflación', 'Rendimiento de mercado', 'Riesgo de crédito']
                    }
                  },
                  M3: {
                    name: 'M3 - Dinero Amplio Plus',
                    description: 'M2 + depósitos a plazo largo + repos',
                    assets: ['M2', 'CDs > 1 año', 'Repos', 'Fondos institucionales'],
                    convertTo: ['Bonos tokenizados', 'Security tokens', 'Wrapped assets'],
                    devaluation: {
                      rate: '10-20%',
                      reason: 'Desvaluación significativa - menor liquidez',
                      factors: ['Duración', 'Riesgo de tasa', 'Condiciones de mercado', 'Antigüedad']
                    }
                  },
                  M4: {
                    name: 'M4 - Dinero Total',
                    description: 'M3 + todos los instrumentos financieros líquidos',
                    assets: ['M3', 'T-Bills', 'Papeles comerciales', 'Aceptaciones bancarias'],
                    convertTo: ['Todos los crypto assets', 'NFTs financieros', 'Synthetic assets'],
                    devaluation: {
                      rate: '15-35%',
                      reason: 'Mayor desvaluación - activos menos líquidos y más antiguos',
                      factors: ['Antigüedad del instrumento', 'Riesgo de default', 'Condiciones legales', 'Mercado secundario']
                    }
                  },
                  // BONOS HISTÓRICOS - Desvaluación especial
                  historicalBonds: {
                    name: 'Bonos Históricos',
                    description: 'Bonos antiguos, títulos coloniales, bonos de guerra',
                    assets: ['Bonos de reparación', 'Títulos coloniales', 'Bonos de guerra', 'Certificados antiguos'],
                    convertTo: ['BTC', 'ETH', 'WPM', 'Stablecoins'],
                    devaluation: {
                      rate: '20-50%',
                      reason: 'Alta desvaluación por antigüedad y riesgo legal',
                      factors: ['Antigüedad (50-200+ años)', 'Validez legal', 'Estado físico', 'Autenticidad', 'Precedentes de cobro'],
                      bonusForAuthenticity: 'Reducción de 10-15% si hay documentación completa'
                    }
                  }
                },
                // TABLA DE DESVALUACIÓN RÁPIDA
                quickDevaluationTable: {
                  'M0_to_Crypto': '0-2%',
                  'M1_to_Crypto': '2-5%',
                  'M2_to_Crypto': '5-10%',
                  'M3_to_Crypto': '10-20%',
                  'M4_to_Crypto': '15-35%',
                  'HistoricalBonds_to_Crypto': '20-50%',
                  'note': 'Tasas negociables según volumen y documentación'
                },
                conversionProcess: {
                  step1: 'Identificar nivel de activo (M0-M4)',
                  step2: 'Verificar y validar activo',
                  step3: 'Calcular valor de conversión',
                  step4: 'Seleccionar crypto destino',
                  step5: 'Ejecutar conversión instantánea',
                  step6: 'Depositar en wallet CryptoHost'
                },
                // ═══════════════════════════════════════════════════════════════
                // ★ M0-M4 TRADING - Hacer trading con todos los niveles
                // ═══════════════════════════════════════════════════════════════
                m0m4Trading: {
                  name: 'M0-M4 Trading Platform',
                  description: 'Trading de activos monetarios en todos los niveles',
                  enabled: true,
                  tradingPairs: {
                    M0_pairs: ['M0/BTC', 'M0/ETH', 'M0/USDT', 'M0/WPM'],
                    M1_pairs: ['M1/BTC', 'M1/ETH', 'M1/USDT', 'M1/WPM'],
                    M2_pairs: ['M2/BTC', 'M2/ETH', 'M2/USDT', 'M2/WPM'],
                    M3_pairs: ['M3/BTC', 'M3/ETH', 'M3/USDT', 'M3/WPM'],
                    M4_pairs: ['M4/BTC', 'M4/ETH', 'M4/USDT', 'M4/WPM'],
                    cross_pairs: ['M0/M1', 'M1/M2', 'M2/M3', 'M3/M4', 'M0/M4']
                  },
                  orderTypes: ['Market', 'Limit', 'Stop-Loss', 'Take-Profit', 'OTC'],
                  leverage: '1x-10x según nivel',
                  fees: { maker: '0.05%', taker: '0.1%' }
                },
                // ═══════════════════════════════════════════════════════════════
                // ★★★ PANTALLAS DE TRADING LIVE - CÓMO ACCEDER ★★★
                // ═══════════════════════════════════════════════════════════════
                tradingScreens: {
                  name: 'Pantallas de Trading Live',
                  description: 'Todas las pantallas de trading en tiempo real',
                  howToAccess: {
                    method1_browser: {
                      name: 'Abrir en Navegador',
                      steps: [
                        '1. Iniciar el servidor: cd platform && npm start (o node server.js)',
                        '2. Abrir navegador en http://localhost:3000',
                        '3. Navegar a la pantalla deseada'
                      ]
                    },
                    method2_directFile: {
                      name: 'Abrir Archivo Directo',
                      steps: [
                        '1. Abrir archivo HTML directamente en navegador',
                        '2. Funcionalidad limitada sin servidor (solo UI)',
                        '3. Para funcionalidad completa, usar servidor'
                      ]
                    },
                    method3_vscode: {
                      name: 'Live Server en VS Code/Cursor',
                      steps: [
                        '1. Instalar extensión "Live Server"',
                        '2. Click derecho en archivo HTML → "Open with Live Server"',
                        '3. Se abre automáticamente en navegador'
                      ]
                    }
                  },
                  // PANTALLAS DISPONIBLES
                  screens: {
                    tradex: {
                      name: 'TradeX Exchange',
                      description: 'Spot Trading, P2P, Staking, Margin Trading',
                      file: '/platform/tradex.html',
                      url: 'http://localhost:3000/tradex',
                      features: ['Spot trading', 'P2P marketplace', 'Staking rewards', 'Margin 5x'],
                      markets: ['Crypto', 'Forex', 'Commodities', 'M0-M4'],
                      icon: '📈'
                    },
                    mameyFutures: {
                      name: 'Mamey Futures',
                      description: 'Futures, Options, Perpetuals, 100x Leverage',
                      file: '/platform/mamey-futures.html',
                      url: 'http://localhost:3000/mamey-futures',
                      features: ['Futures contracts', 'Options trading', 'Perpetual swaps', 'Up to 100x leverage'],
                      markets: ['Crypto futures', 'Commodity futures', 'Index futures'],
                      icon: '🎯'
                    },
                    net10Defi: {
                      name: 'NET10 DeFi',
                      description: 'DEX, Liquidity Pools, Yield Farming, Staking',
                      file: '/platform/net10-defi.html',
                      url: 'http://localhost:3000/net10-defi',
                      features: ['Decentralized exchange', 'Liquidity pools', 'Yield farming', 'Auto-compounding'],
                      protocols: ['AMM', 'Lending', 'Borrowing', 'Flash loans'],
                      icon: '🌐'
                    },
                    cryptohost: {
                      name: 'CryptoHost',
                      description: 'Asset Monetization, M0-M4 Conversion, Custody',
                      file: '/platform/cryptohost.html',
                      url: 'http://localhost:3000/cryptohost',
                      features: ['Asset custody', 'M0-M4 conversion', 'Historical bonds', 'Tokenization'],
                      icon: '₿'
                    },
                    forex: {
                      name: 'Forex Trading',
                      description: 'Currency pairs, Real-time quotes, Technical analysis',
                      file: '/platform/forex.html',
                      url: 'http://localhost:3000/forex',
                      features: ['Major pairs', 'Minor pairs', 'Exotic pairs', 'Cross rates'],
                      pairs: ['EUR/USD', 'GBP/USD', 'USD/JPY', 'WPM/USD'],
                      icon: '💱'
                    },
                    bridge: {
                      name: 'Bridge',
                      description: 'Cross-chain transfers, Token wrapping',
                      file: '/platform/bridge.html',
                      url: 'http://localhost:3000/bridge',
                      features: ['Cross-chain bridge', 'Token wrapping', 'Multi-chain support'],
                      chains: ['Ethereum', 'Polygon', 'BNB', 'Solana', 'IERAHKWA Chain'],
                      icon: '🔗'
                    },
                    tokenFactory: {
                      name: 'Token Factory',
                      description: 'Create and deploy tokens, ICO/IDO platform',
                      file: '/platform/token-factory.html',
                      url: 'http://localhost:3000/token-factory',
                      features: ['Token creation', 'Smart contract deployment', 'ICO/IDO launch'],
                      icon: '🪙'
                    },
                    daoGovernance: {
                      name: 'DAO Governance',
                      description: 'Voting, Proposals, Treasury management',
                      file: '/platform/dao-governance.html',
                      url: 'http://localhost:3000/dao-governance',
                      features: ['Proposal creation', 'Voting', 'Treasury', 'Delegation'],
                      icon: '🗳️'
                    }
                  },
                  // DASHBOARD PRINCIPAL
                  mainDashboard: {
                    name: 'Abrir Todas las Plataformas',
                    file: '/platform/abrir-todas-plataformas.html',
                    url: 'http://localhost:3000/abrir-todas-plataformas',
                    description: 'Dashboard central con acceso a todas las plataformas de trading'
                  },
                  // DATOS EN VIVO
                  liveDataFeeds: {
                    priceFeeds: {
                      crypto: 'WebSocket: wss://api.ierahkwa.bank/ws/prices/crypto',
                      forex: 'WebSocket: wss://api.ierahkwa.bank/ws/prices/forex',
                      commodities: 'WebSocket: wss://api.ierahkwa.bank/ws/prices/commodities',
                      m0m4: 'WebSocket: wss://api.ierahkwa.bank/ws/prices/m0m4'
                    },
                    orderBook: 'WebSocket: wss://api.ierahkwa.bank/ws/orderbook/{pair}',
                    trades: 'WebSocket: wss://api.ierahkwa.bank/ws/trades/{pair}',
                    portfolio: 'WebSocket: wss://api.ierahkwa.bank/ws/portfolio/{userId}'
                  },
                  // APIS DE TRADING
                  tradingAPIs: {
                    rest: {
                      baseUrl: 'https://api.ierahkwa.bank/v1',
                      endpoints: {
                        markets: 'GET /markets',
                        ticker: 'GET /ticker/{symbol}',
                        orderbook: 'GET /orderbook/{symbol}',
                        trades: 'GET /trades/{symbol}',
                        placeOrder: 'POST /orders',
                        cancelOrder: 'DELETE /orders/{orderId}',
                        balance: 'GET /account/balance',
                        history: 'GET /account/history'
                      }
                    },
                    websocket: {
                      url: 'wss://api.ierahkwa.bank/ws',
                      channels: ['ticker', 'orderbook', 'trades', 'account']
                    }
                  }
                }
              },
              // ═══════════════════════════════════════════════════════════════
              // ★★★ PROTOCOLOS DE COMUNICACIÓN - RECIBIR Y ENVIAR ★★★
              // ═══════════════════════════════════════════════════════════════
              communicationProtocols: {
                name: 'Protocolos de Comunicación Global',
                description: 'Todos los protocolos para recibir y enviar activos/datos',
                // S2S - SERVER TO SERVER
                s2s: {
                  name: 'S2S - Server to Server',
                  code: 'S2S',
                  description: 'Comunicación directa entre servidores bancarios',
                  direction: 'BIDIRECTIONAL',
                  receive: {
                    enabled: true,
                    endpoints: ['/api/s2s/receive', '/api/s2s/inbound'],
                    formats: ['JSON', 'XML', 'ISO20022', 'FIX'],
                    authentication: ['mTLS', 'API Key', 'OAuth2', 'JWT'],
                    accepts: ['Wire transfers', 'Settlement messages', 'Status updates', 'Confirmations']
                  },
                  send: {
                    enabled: true,
                    endpoints: ['/api/s2s/send', '/api/s2s/outbound'],
                    formats: ['JSON', 'XML', 'ISO20022', 'FIX'],
                    authentication: ['mTLS', 'API Key', 'OAuth2', 'JWT'],
                    sends: ['Payment instructions', 'Settlement requests', 'Status responses', 'Confirmations']
                  },
                  useCases: ['Inter-bank transfers', 'Clearing house communication', 'Real-time settlements']
                },
                // IPTIP - IP TO IP
                iptip: {
                  name: 'IPTIP - IP to IP Protocol',
                  code: 'IPTIP',
                  description: 'Protocolo de comunicación IP directa entre instituciones',
                  direction: 'BIDIRECTIONAL',
                  receive: {
                    enabled: true,
                    ports: [443, 8443, 9443],
                    protocols: ['HTTPS', 'gRPC', 'WebSocket'],
                    encryption: 'TLS 1.3 / Post-Quantum',
                    accepts: ['Direct transfers', 'Real-time data', 'Streaming feeds']
                  },
                  send: {
                    enabled: true,
                    ports: [443, 8443, 9443],
                    protocols: ['HTTPS', 'gRPC', 'WebSocket'],
                    encryption: 'TLS 1.3 / Post-Quantum',
                    sends: ['Direct transfers', 'Real-time data', 'Streaming feeds']
                  },
                  useCases: ['High-frequency trading', 'Real-time market data', 'Instant settlements']
                },
                // API TO API
                apiToApi: {
                  name: 'API to API',
                  code: 'A2A',
                  description: 'Integración REST/GraphQL entre sistemas',
                  direction: 'BIDIRECTIONAL',
                  receive: {
                    enabled: true,
                    endpoints: {
                      rest: '/api/v1/*',
                      graphql: '/graphql',
                      webhook: '/webhooks/*'
                    },
                    methods: ['GET', 'POST', 'PUT', 'DELETE', 'PATCH'],
                    formats: ['JSON', 'Protocol Buffers'],
                    rateLimit: '10,000 req/min',
                    accepts: ['Account inquiries', 'Transfer requests', 'KYC data', 'Trade orders']
                  },
                  send: {
                    enabled: true,
                    methods: ['POST', 'PUT'],
                    formats: ['JSON', 'Protocol Buffers'],
                    sends: ['Notifications', 'Confirmations', 'Status updates', 'Settlement data']
                  },
                  documentation: '/api/docs',
                  sandbox: '/api/sandbox',
                  useCases: ['Third-party integrations', 'Fintech partners', 'Mobile apps', 'Web platforms']
                },
                // EXT TO EXT - EXTERNAL TO EXTERNAL
                extToExt: {
                  name: 'EXT to EXT - External Networks',
                  code: 'E2E',
                  description: 'Conexión con redes externas globales',
                  direction: 'BIDIRECTIONAL',
                  networks: {
                    swift: {
                      name: 'SWIFT Network',
                      receive: { enabled: true, messageTypes: ['MT103', 'MT202', 'MT940', 'MT950'] },
                      send: { enabled: true, messageTypes: ['MT103', 'MT202', 'MT940', 'MT950'] }
                    },
                    sepa: {
                      name: 'SEPA (Europe)',
                      receive: { enabled: true, types: ['SCT', 'SDD', 'SCT Inst'] },
                      send: { enabled: true, types: ['SCT', 'SDD', 'SCT Inst'] }
                    },
                    fedwire: {
                      name: 'Fedwire (USA)',
                      receive: { enabled: true, types: ['Funds Transfer', 'Securities'] },
                      send: { enabled: true, types: ['Funds Transfer', 'Securities'] }
                    },
                    ach: {
                      name: 'ACH (USA)',
                      receive: { enabled: true, types: ['Direct Deposit', 'Direct Payment'] },
                      send: { enabled: true, types: ['Direct Deposit', 'Direct Payment'] }
                    },
                    spei: {
                      name: 'SPEI (Mexico)',
                      receive: { enabled: true },
                      send: { enabled: true }
                    },
                    pix: {
                      name: 'PIX (Brazil)',
                      receive: { enabled: true },
                      send: { enabled: true }
                    },
                    crypto: {
                      name: 'Blockchain Networks',
                      receive: { enabled: true, chains: ['Bitcoin', 'Ethereum', 'Polygon', 'Solana', 'BNB'] },
                      send: { enabled: true, chains: ['Bitcoin', 'Ethereum', 'Polygon', 'Solana', 'BNB'] }
                    }
                  },
                  useCases: ['International transfers', 'Cross-border payments', 'Global settlements']
                },
                // SIIS - Sistema Internacional Interno Soberano
                siisProtocol: {
                  name: 'SIIS Protocol',
                  code: 'SIIS',
                  description: 'Protocolo soberano interno (alternativa a SWIFT)',
                  direction: 'BIDIRECTIONAL',
                  receive: { enabled: true, instant: true, fee: '0%' },
                  send: { enabled: true, instant: true, fee: '0%' },
                  compatible: 'ISO 20022',
                  useCases: ['Internal sovereign transfers', 'Inter-IERAHKWA bank transfers']
                }
              },
              // ═══════════════════════════════════════════════════════════════
              // ★★★ SERVICIOS DE CHEQUES - DEPÓSITO Y EMISIÓN ★★★
              // ═══════════════════════════════════════════════════════════════
              checkServices: {
                name: 'Servicios de Cheques',
                description: 'Depósito y emisión de cheques nacionales e internacionales',
                // DEPÓSITO DE CHEQUES
                checkDeposit: {
                  name: 'Depósito de Cheques',
                  description: 'Depositar cheques de cualquier banco del mundo',
                  methods: {
                    mobile: {
                      name: 'Mobile Check Deposit',
                      description: 'Depositar foto del cheque desde app',
                      limit: '$50,000/día',
                      clearingTime: '1-2 días hábiles',
                      fee: '$0'
                    },
                    atm: {
                      name: 'ATM Check Deposit',
                      description: 'Depositar cheque físico en ATM',
                      limit: '$100,000/día',
                      clearingTime: '1-2 días hábiles',
                      fee: '$0'
                    },
                    branch: {
                      name: 'Branch Deposit',
                      description: 'Depositar en sucursal bancaria',
                      limit: 'Sin límite',
                      clearingTime: '1-3 días hábiles',
                      fee: '$0'
                    },
                    remoteCapture: {
                      name: 'Remote Deposit Capture',
                      description: 'Para empresas - scanner de cheques',
                      limit: '$500,000/día',
                      clearingTime: 'Same day',
                      fee: '$0.10/cheque'
                    }
                  },
                  acceptedChecks: {
                    domestic: ['Cheques personales', 'Cheques de nómina', 'Cheques de gobierno', 'Cheques empresariales'],
                    international: ['USD checks', 'EUR checks', 'CAD checks', 'GBP checks'],
                    special: ['Cheques de caja (cashier checks)', 'Money orders', 'Certified checks', 'Traveler checks']
                  },
                  internationalClearing: {
                    time: '5-10 días hábiles',
                    fee: '$25 por cheque extranjero',
                    currencies: ['USD', 'EUR', 'CAD', 'GBP', 'MXN', 'BRL']
                  }
                },
                // EMISIÓN DE CHEQUES
                checkIssuance: {
                  name: 'Emisión de Cheques',
                  description: 'Emitir cheques desde cuenta IERAHKWA',
                  types: {
                    personalChecks: {
                      name: 'Cheques Personales',
                      description: 'Chequera personal estándar',
                      cost: '$25 por 100 cheques',
                      features: ['Nombre personalizado', 'Dirección', 'Diseños disponibles']
                    },
                    businessChecks: {
                      name: 'Cheques Empresariales',
                      description: 'Cheques para empresas',
                      cost: '$50 por 100 cheques',
                      features: ['Logo de empresa', 'Múltiples firmantes', 'Seguridad avanzada']
                    },
                    cashierCheck: {
                      name: 'Cheque de Caja (Cashier Check)',
                      description: 'Cheque garantizado por el banco',
                      cost: '$15 por cheque',
                      maxAmount: 'Sin límite',
                      availability: 'Inmediato'
                    },
                    certifiedCheck: {
                      name: 'Cheque Certificado',
                      description: 'Cheque con fondos verificados y reservados',
                      cost: '$10 por cheque',
                      validity: '90 días'
                    },
                    internationalDraft: {
                      name: 'Giro Internacional (Bank Draft)',
                      description: 'Cheque para pagos internacionales',
                      cost: '$35 por giro',
                      currencies: ['USD', 'EUR', 'GBP', 'CAD'],
                      clearingTime: '5-10 días'
                    }
                  },
                  security: {
                    features: ['Watermark', 'Microprint', 'Security thread', 'Color-shifting ink', 'VOID pantograph'],
                    fraud_protection: ['Positive Pay', 'Check verification', 'Stop payment']
                  }
                }
              },
              // ═══════════════════════════════════════════════════════════════
              // ★★★ DEPOSITORY - CUSTODIA DE ACTIVOS FÍSICOS ★★★
              // Gold, Silver, Commodities, Precious Stones, Art, Property Titles
              // ═══════════════════════════════════════════════════════════════
              depository: {
                name: 'IERAHKWA Global Depository',
                description: 'Custodia y monetización de activos físicos para trading M0-M4',
                path: '/depository',
                icon: '🏦',
                status: 'OPERATIONAL',
                // METALES PRECIOSOS
                preciousMetals: {
                  name: 'Precious Metals Depository',
                  description: 'Custodia de oro, plata y metales preciosos',
                  assets: {
                    gold: {
                      name: 'Gold (Oro)',
                      symbol: 'XAU',
                      forms: ['Bullion bars (lingotes)', 'Coins (monedas)', 'Jewelry (joyería)', 'Gold certificates'],
                      purity: ['24K (999.9)', '22K (916)', '18K (750)'],
                      storage: ['Segregated vault', 'Allocated storage', 'Pool allocated'],
                      monetization: {
                        toM0: { enabled: true, rate: '0-2%' },
                        toM1: { enabled: true, rate: '2-5%' },
                        toCrypto: { enabled: true, tokens: ['PAXG', 'XAUT', 'WPM-Gold'] }
                      },
                      trading: { enabled: true, pairs: ['XAU/USD', 'XAU/BTC', 'XAU/WPM'] }
                    },
                    silver: {
                      name: 'Silver (Plata)',
                      symbol: 'XAG',
                      forms: ['Bullion bars', 'Coins', 'Industrial silver', 'Silver certificates'],
                      purity: ['999 Fine', '925 Sterling', '900 Coin'],
                      storage: ['Segregated vault', 'Allocated storage'],
                      monetization: {
                        toM0: { enabled: true, rate: '0-2%' },
                        toM1: { enabled: true, rate: '2-5%' },
                        toCrypto: { enabled: true, tokens: ['WPM-Silver'] }
                      },
                      trading: { enabled: true, pairs: ['XAG/USD', 'XAG/BTC', 'XAG/WPM'] }
                    },
                    platinum: {
                      name: 'Platinum (Platino)',
                      symbol: 'XPT',
                      forms: ['Bullion bars', 'Coins'],
                      storage: ['Segregated vault'],
                      monetization: { toM0: { enabled: true }, toCrypto: { enabled: true } },
                      trading: { enabled: true, pairs: ['XPT/USD', 'XPT/BTC'] }
                    },
                    palladium: {
                      name: 'Palladium (Paladio)',
                      symbol: 'XPD',
                      forms: ['Bullion bars'],
                      storage: ['Segregated vault'],
                      monetization: { toM0: { enabled: true }, toCrypto: { enabled: true } },
                      trading: { enabled: true, pairs: ['XPD/USD', 'XPD/BTC'] }
                    }
                  },
                  services: ['Secure storage', 'Insurance', 'Assay/Testing', 'Delivery', 'Buy/Sell', 'Tokenization'],
                  fees: { storage: '0.5%/año', insurance: '0.25%/año', withdrawal: '$50/envío' }
                },
                // PIEDRAS PRECIOSAS
                preciousStones: {
                  name: 'Precious Stones Depository',
                  description: 'Custodia de piedras preciosas y gemas',
                  assets: {
                    diamonds: {
                      name: 'Diamantes',
                      certification: ['GIA', 'AGS', 'IGI', 'HRD'],
                      grading: '4Cs (Cut, Color, Clarity, Carat)',
                      storage: 'Individual sealed containers',
                      monetization: { toM2: { enabled: true, rate: '5-15%' }, toCrypto: { enabled: true } },
                      trading: { enabled: true, pairs: ['DIAMOND/USD', 'DIAMOND/WPM'] }
                    },
                    emeralds: {
                      name: 'Esmeraldas',
                      origin: ['Colombia', 'Zambia', 'Brazil'],
                      certification: ['GIA', 'Gübelin', 'SSEF'],
                      monetization: { toM2: { enabled: true }, toCrypto: { enabled: true } },
                      trading: { enabled: true }
                    },
                    rubies: {
                      name: 'Rubíes',
                      origin: ['Burma', 'Mozambique', 'Thailand'],
                      certification: ['GIA', 'Gübelin', 'SSEF'],
                      monetization: { toM2: { enabled: true }, toCrypto: { enabled: true } },
                      trading: { enabled: true }
                    },
                    sapphires: {
                      name: 'Zafiros',
                      origin: ['Kashmir', 'Sri Lanka', 'Madagascar'],
                      certification: ['GIA', 'Gübelin', 'SSEF'],
                      monetization: { toM2: { enabled: true }, toCrypto: { enabled: true } },
                      trading: { enabled: true }
                    },
                    other: ['Opals', 'Tanzanite', 'Alexandrite', 'Jade', 'Pearls']
                  },
                  services: ['Certification', 'Appraisal', 'Secure storage', 'Insurance', 'Tokenization'],
                  fees: { storage: '1%/año', insurance: '0.5%/año', appraisal: '$200-$1000' }
                },
                // COMMODITIES
                commodities: {
                  name: 'Commodities Depository',
                  description: 'Custodia y trading de materias primas',
                  categories: {
                    energy: {
                      name: 'Energía',
                      assets: ['Crude Oil (WTI/Brent)', 'Natural Gas', 'Gasoline', 'Heating Oil'],
                      unit: 'Barrels/MMBtu',
                      storage: 'Warehouse receipts',
                      monetization: { toM3: { enabled: true, rate: '10-15%' } },
                      trading: { enabled: true, type: 'Futures/Spot' }
                    },
                    agriculture: {
                      name: 'Agricultura',
                      assets: ['Corn (Maíz)', 'Wheat (Trigo)', 'Soybeans (Soja)', 'Coffee (Café)', 'Sugar (Azúcar)', 'Cocoa (Cacao)', 'Cotton (Algodón)'],
                      unit: 'Bushels/Pounds',
                      storage: 'Warehouse receipts',
                      monetization: { toM3: { enabled: true, rate: '10-20%' } },
                      trading: { enabled: true, type: 'Futures/Spot' }
                    },
                    metals: {
                      name: 'Metales Industriales',
                      assets: ['Copper (Cobre)', 'Aluminum (Aluminio)', 'Zinc', 'Nickel (Níquel)', 'Lead (Plomo)'],
                      unit: 'Metric Tons',
                      storage: 'LME Warehouses',
                      monetization: { toM2: { enabled: true, rate: '5-10%' } },
                      trading: { enabled: true, type: 'Futures/Spot' }
                    },
                    livestock: {
                      name: 'Ganado',
                      assets: ['Live Cattle', 'Feeder Cattle', 'Lean Hogs'],
                      unit: 'Pounds',
                      monetization: { toM3: { enabled: true } },
                      trading: { enabled: true, type: 'Futures' }
                    }
                  },
                  services: ['Warehouse receipts', 'Futures trading', 'Physical delivery', 'Tokenization'],
                  fees: { storage: 'Variable by commodity', trading: '0.1%' }
                },
                // ARTE Y COLECCIONABLES
                artAndCollectibles: {
                  name: 'Art & Collectibles Depository',
                  description: 'Custodia de arte, cuadros y coleccionables',
                  categories: {
                    fineArt: {
                      name: 'Arte (Cuadros/Pinturas)',
                      types: ['Oil paintings', 'Watercolors', 'Prints', 'Photographs', 'Sculptures'],
                      authentication: ['Provenance research', 'Expert authentication', 'Scientific analysis'],
                      storage: 'Climate-controlled vault (18-21°C, 45-55% RH)',
                      monetization: { toM4: { enabled: true, rate: '15-30%' }, toCrypto: { enabled: true, type: 'NFT fractional' } },
                      trading: { enabled: true, type: 'Auction/Private sale/Fractional' }
                    },
                    antiquities: {
                      name: 'Antigüedades',
                      types: ['Furniture', 'Porcelain', 'Textiles', 'Artifacts'],
                      authentication: ['Expert appraisal', 'Provenance documentation'],
                      storage: 'Climate-controlled vault',
                      monetization: { toM4: { enabled: true, rate: '20-35%' } },
                      trading: { enabled: true }
                    },
                    collectibles: {
                      name: 'Coleccionables',
                      types: ['Rare coins', 'Stamps', 'Sports memorabilia', 'Wine', 'Watches', 'Classic cars'],
                      authentication: ['Grading services (PCGS, NGC, PSA)', 'Expert authentication'],
                      monetization: { toM4: { enabled: true, rate: '15-25%' } },
                      trading: { enabled: true }
                    },
                    indigenousArt: {
                      name: 'Arte Indígena',
                      types: ['Textiles tradicionales', 'Cerámica', 'Artesanías', 'Artefactos ceremoniales'],
                      authentication: ['Tribal authentication', 'Cultural heritage verification'],
                      specialStatus: 'Protected cultural heritage',
                      monetization: { toM4: { enabled: true, rate: '10-20%', specialTerms: true } },
                      trading: { enabled: true, restricted: 'Some items non-transferable' }
                    }
                  },
                  services: ['Professional storage', 'Conservation', 'Insurance', 'Authentication', 'Fractional ownership', 'NFT tokenization'],
                  fees: { storage: '1-2%/año', insurance: '0.5-1%/año', authentication: '$500-$5000' }
                },
                // TÍTULOS DE PROPIEDAD
                propertyTitles: {
                  name: 'Property Titles Depository',
                  description: 'Custodia y monetización de títulos de propiedad',
                  types: {
                    realEstate: {
                      name: 'Bienes Raíces',
                      documents: ['Land titles (Títulos de tierra)', 'Property deeds', 'Mortgage notes', 'Lease agreements'],
                      verification: ['Title search', 'Survey', 'Legal review'],
                      monetization: {
                        toM3: { enabled: true, rate: '10-20%' },
                        toM4: { enabled: true, rate: '15-25%' },
                        toCrypto: { enabled: true, type: 'Security Token (STO)' }
                      },
                      trading: { enabled: true, type: 'Fractional ownership/REITs/STOs' }
                    },
                    vehicleTitles: {
                      name: 'Títulos de Vehículos',
                      documents: ['Car titles', 'Boat titles', 'Aircraft titles'],
                      verification: ['DMV verification', 'Lien check'],
                      monetization: { toM3: { enabled: true, rate: '10-15%' } },
                      trading: { enabled: true }
                    },
                    intellectualProperty: {
                      name: 'Propiedad Intelectual',
                      documents: ['Patents', 'Trademarks', 'Copyrights', 'Trade secrets', 'Licensing agreements'],
                      verification: ['USPTO/WIPO verification', 'Legal review'],
                      monetization: { toM4: { enabled: true, rate: '20-40%' } },
                      trading: { enabled: true, type: 'Licensing/Sale/Fractional' }
                    },
                    mineralRights: {
                      name: 'Derechos Mineros',
                      documents: ['Mining rights', 'Mineral leases', 'Exploration permits'],
                      verification: ['Government verification', 'Geological survey'],
                      monetization: { toM4: { enabled: true, rate: '15-30%' } },
                      trading: { enabled: true }
                    },
                    sovereignLands: {
                      name: 'Tierras Soberanas Indígenas',
                      documents: ['Tribal land certificates', 'Treaty documents', 'Sovereignty documentation'],
                      verification: ['Tribal council verification', 'Federal recognition'],
                      specialStatus: 'Protected sovereign territory - non-alienable',
                      monetization: { toM4: { enabled: true, rate: '5-15%', restrictions: 'Revenue only, not ownership' } },
                      trading: { enabled: false, note: 'Sovereignty protection - development rights only' }
                    }
                  },
                  services: ['Secure custody', 'Title insurance', 'Legal verification', 'Tokenization', 'Fractional ownership'],
                  fees: { custody: '0.25%/año', titleInsurance: '0.5%', verification: '$500-$2000' }
                },
                // BONOS Y TÍTULOS FINANCIEROS
                financialInstruments: {
                  name: 'Financial Instruments Depository',
                  description: 'Custodia de bonos y títulos financieros',
                  types: {
                    governmentBonds: {
                      name: 'Bonos de Gobierno',
                      types: ['T-Bills', 'T-Notes', 'T-Bonds', 'TIPS', 'Sovereign bonds'],
                      monetization: { toM2: { enabled: true, rate: '2-5%' } },
                      trading: { enabled: true }
                    },
                    corporateBonds: {
                      name: 'Bonos Corporativos',
                      types: ['Investment grade', 'High yield', 'Convertible bonds'],
                      monetization: { toM3: { enabled: true, rate: '5-15%' } },
                      trading: { enabled: true }
                    },
                    historicalBonds: {
                      name: 'Bonos Históricos',
                      types: ['War bonds', 'Colonial bonds', 'Reparation bonds', 'Defaulted sovereign bonds'],
                      authentication: ['Historical verification', 'Legal status review'],
                      monetization: { toM4: { enabled: true, rate: '20-50%' } },
                      trading: { enabled: true, note: 'Specialized market' }
                    },
                    certificates: {
                      name: 'Certificados',
                      types: ['Stock certificates', 'Bond certificates', 'Deposit certificates'],
                      monetization: { toM2: { enabled: true }, toM3: { enabled: true } },
                      trading: { enabled: true }
                    }
                  },
                  services: ['Safekeeping', 'Coupon collection', 'Maturity tracking', 'Corporate actions'],
                  fees: { custody: '0.1%/año', transactions: '$10/operación' }
                },
                // PROCESO DE MONETIZACIÓN UNIVERSAL
                monetizationProcess: {
                  steps: [
                    '1. Depositar activo físico/documento en Depository',
                    '2. Verificación y autenticación del activo',
                    '3. Valuación profesional',
                    '4. Asignación de nivel M0-M4 según liquidez',
                    '5. Aplicar tasa de desvaluación correspondiente',
                    '6. Tokenizar o convertir a crypto/fiat',
                    '7. Disponible para trading en plataforma'
                  ],
                  outputOptions: ['WPM Token', 'BTC', 'ETH', 'USDT', 'USDC', 'Fiat (USD/EUR)', 'Security Token', 'NFT']
                },
                // UBICACIONES DE DEPOSITORY
                locations: [
                  { name: 'IERAHKWA Sovereign Vault', location: 'Sovereign Territory', primary: true },
                  { name: 'Panama Vault', location: 'Panama City', type: 'International' },
                  { name: 'Swiss Vault', location: 'Zurich', type: 'European hub' },
                  { name: 'Singapore Vault', location: 'Singapore', type: 'Asian hub' },
                  { name: 'Dubai Vault', location: 'Dubai', type: 'Middle East hub' }
                ]
              }
            },
            cryptoConversion: {
              name: 'Conversión a Crypto',
              description: 'Convertir cualquier activo a criptomonedas',
              supportedCrypto: [
                { symbol: 'BTC', name: 'Bitcoin', type: 'Store of value' },
                { symbol: 'ETH', name: 'Ethereum', type: 'Smart contracts' },
                { symbol: 'USDT', name: 'Tether', type: 'Stablecoin USD' },
                { symbol: 'USDC', name: 'USD Coin', type: 'Stablecoin USD' },
                { symbol: 'WPM', name: 'Wampum Token', type: 'Sovereign stablecoin' },
                { symbol: 'ISB', name: 'ISB Token', type: 'Sovereign utility' },
                { symbol: 'DAI', name: 'DAI', type: 'Decentralized stablecoin' },
                { symbol: 'WBTC', name: 'Wrapped BTC', type: 'ERC-20 Bitcoin' }
              ],
              fees: {
                conversion: '0.1%',
                withdrawal: '0.05%',
                custody: '0% (free)'
              }
            },
            products: ['Custody', 'Stablecoin Gateway', 'Multi-Sig Vaults', 'Asset Tokenization', 'M0-M4 Conversion', 'Historical Bond Collection']
          },
          {
            id: 'bitcoin-hemp',
            name: 'Bitcoin Hemp',
            description: 'Crypto y activos digitales',
            path: '/bitcoin-hemp',
            icon: '₿',
            products: ['Bitcoin', 'Altcoins', 'NFTs']
          },
          {
            id: 'token-factory',
            name: 'Token Factory',
            description: 'Crear y emitir tokens',
            path: '/token-factory',
            icon: '🪙',
            products: ['Token Creation', 'ICO/IDO', 'Tokenization']
          }
        ]
      },
      clearing: {
        name: 'Clearing & Settlement',
        businessModel: 'wholesale',
        services: [
          {
            id: 'maletas',
            name: 'Maletas',
            description: 'Paquetes de valor en tránsito (SWIFT, SIIS, ISO 20022)',
            path: '/maletas',
            icon: '🧳',
            products: ['Value Packages', 'Secure Transit', 'Settlement']
          }
        ]
      },
      // BANCA DE INVERSIÓN
      investmentBanking: {
        name: 'Banca de Inversión',
        businessModel: 'trading',
        services: [
          {
            id: 'investment-bank',
            name: 'IERAHKWA Investment Bank',
            description: 'Underwriting, M&A, Capital Markets',
            path: '/investment-bank',
            icon: '📊',
            products: ['IPO Underwriting', 'M&A Advisory', 'Debt Issuance', 'Equity Capital Markets']
          },
          {
            id: 'securities-underwriting',
            name: 'Securities Underwriting',
            description: 'Emisión de valores y bonos',
            path: '/underwriting',
            icon: '📈',
            products: ['IPOs', 'Secondary Offerings', 'Bond Issuance', 'Private Placements']
          },
          {
            id: 'ma-advisory',
            name: 'M&A Advisory',
            description: 'Fusiones y adquisiciones',
            path: '/ma-advisory',
            icon: '🤝',
            products: ['Buy-side Advisory', 'Sell-side Advisory', 'Valuations', 'Due Diligence']
          }
        ]
      },
      // MERCADOS DE CAPITALES
      capitalMarkets: {
        name: 'Mercados de Capitales',
        businessModel: 'trading',
        services: [
          {
            id: 'stock-exchange',
            name: 'Bolsa de Valores IERAHKWA (BVI)',
            description: 'Mercado de acciones y ETFs',
            path: '/stock-exchange',
            icon: '📈',
            products: ['Acciones', 'ETFs', 'ADRs', 'REITs'],
            indices: ['BVI-100', 'BVI-Indigenous', 'BVI-Tech', 'BVI-Commodities']
          },
          {
            id: 'bond-market',
            name: 'Mercado de Bonos Soberanos',
            description: 'Deuda soberana y corporativa',
            path: '/bond-market',
            icon: '📜',
            products: ['Bonos Soberanos', 'Bonos Corporativos', 'T-Bills', 'Municipal Bonds']
          },
          {
            id: 'commodities-market',
            name: 'Mercado de Commodities',
            description: 'Materias primas y futuros',
            path: '/commodities',
            icon: '🌾',
            products: ['Oro', 'Plata', 'Petróleo', 'Maíz', 'Café', 'Cacao']
          },
          {
            id: 'central-custody',
            name: 'Custodia Central de Valores',
            description: 'Depósito centralizado de valores',
            path: '/central-custody',
            icon: '🔐',
            products: ['Securities Custody', 'Settlement', 'Corporate Actions', 'Asset Servicing']
          }
        ]
      },
      // ═══════════════════════════════════════════════════════════════════════
      // ★★★ TRUST COMPANIES - SISTEMA 100% BASADO EN FIDEICOMISOS ★★★
      // Todas las cuentas de ciudadanos son TRUST ACCOUNTS
      // Ciudadano = Beneficiario del Trust
      // ═══════════════════════════════════════════════════════════════════════
      trustSystem: {
        name: 'Sistema Fiduciario IERAHKWA - 100% Citizen Trust',
        businessModel: 'trust',
        principle: 'TODAS las cuentas de ciudadanos son Trust Accounts',
        description: 'Sistema bancario 100% basado en fideicomisos para ciudadanos soberanos',
        structure: {
          grantor: 'Ciudadano Soberano (aporta activos)',
          trustee: 'Trust Company IERAHKWA (administra)',
          beneficiary: 'Ciudadano Soberano (recibe beneficios)',
          protector: 'Superintendencia de Bancos (supervisa)'
        },
        benefits: [
          'Protección de activos soberana',
          'Privacidad financiera',
          'Planificación patrimonial',
          'Protección contra embargo',
          'Sucesión automática',
          'Separación legal de activos'
        ],
        services: [
          {
            id: 'citizen-trust-company',
            name: 'IERAHKWA Citizen Trust Company',
            code: 'IERCTRUST',
            description: 'Trust Company principal para cuentas de ciudadanos',
            path: '/citizen-trust',
            icon: '🏛️',
            role: 'Administra TODAS las cuentas de ciudadanos como Trust Accounts',
            license: 'Trust Company License - Full Fiduciary',
            services: [
              'Citizen Trust Accounts (Cuentas personales)',
              'Family Trust (Fideicomiso familiar)',
              'Business Trust (Fideicomiso empresarial)',
              'Investment Trust (Fideicomiso de inversión)',
              'Retirement Trust (Fideicomiso de jubilación)',
              'Education Trust (Fideicomiso educativo)',
              'Charitable Trust (Fideicomiso caritativo)'
            ]
          },
          {
            id: 'sovereign-trust-company',
            name: 'Sovereign Trust Company',
            code: 'IERSOVTRUST',
            description: 'Trust Company para ciudadanos con alto patrimonio',
            path: '/sovereign-trust',
            icon: '👑',
            role: 'Servicios premium de trust para ciudadanos VIP',
            minimumBalance: '$100,000 WPM',
            services: [
              'Dynasty Trust (multi-generacional)',
              'Asset Protection Trust',
              'Spendthrift Trust',
              'Blind Trust',
              'Discretionary Trust',
              'Private Family Foundation'
            ]
          },
          {
            id: 'community-trust-company',
            name: 'Community Trust Company',
            code: 'IERCOMTRUST',
            description: 'Trust Company para comunidades y tribus',
            path: '/community-trust',
            icon: '🏘️',
            role: 'Fideicomisos comunitarios y tribales',
            services: [
              'Tribal Trust (Fideicomiso tribal)',
              'Community Land Trust',
              'Collective Investment Trust',
              'Cooperative Trust',
              'Cultural Heritage Trust',
              'Environmental Trust'
            ]
          },
          {
            id: 'corporate-trust-company',
            name: 'Corporate Trust Company',
            code: 'IERCORPTRUST',
            description: 'Trust Company para empresas',
            path: '/corporate-trust',
            icon: '🏢',
            role: 'Servicios fiduciarios corporativos',
            services: [
              'Business Trust',
              'Employee Benefit Trust',
              'Pension Trust',
              'ESOP Trust',
              'Escrow Services',
              'Bond Trustee Services'
            ]
          }
        ],
        // TIPOS DE CUENTAS TRUST PARA CIUDADANOS
        citizenAccountTypes: {
          personalTrust: {
            name: 'Personal Trust Account',
            code: 'PTA',
            description: 'Cuenta personal del ciudadano como fideicomiso',
            features: [
              'Protección de activos automática',
              'Sin embargo por deudas personales',
              'Sucesión sin probate',
              'Privacidad total'
            ],
            accounts: [
              { type: 'checking', name: 'Trust Checking Account', minBalance: 0 },
              { type: 'savings', name: 'Trust Savings Account', minBalance: 0, interest: '4% APY' },
              { type: 'money_market', name: 'Trust Money Market', minBalance: 1000, interest: '5% APY' },
              { type: 'cd', name: 'Trust Certificate of Deposit', terms: ['3mo', '6mo', '1yr', '2yr', '5yr'] }
            ]
          },
          familyTrust: {
            name: 'Family Trust Account',
            code: 'FTA',
            description: 'Fideicomiso familiar multi-beneficiario',
            features: [
              'Múltiples beneficiarios',
              'Distribución controlada',
              'Protección familiar',
              'Planificación sucesoria'
            ],
            beneficiaries: 'Ilimitados'
          },
          childrenTrust: {
            name: 'Children Trust Account',
            code: 'CTA',
            description: 'Fideicomiso para menores de edad',
            features: [
              'Distribución por edad',
              'Protección hasta mayoría de edad',
              'Incentivos educativos',
              'Control parental'
            ],
            distributionAge: 18
          },
          retirementTrust: {
            name: 'Retirement Trust Account',
            code: 'RTA',
            description: 'Fideicomiso de jubilación',
            features: [
              'Crecimiento tax-advantaged',
              'Distribución a los 60+',
              'Pensión garantizada',
              'Beneficios de sobreviviente'
            ],
            contributionLimit: '$50,000/año'
          },
          investmentTrust: {
            name: 'Investment Trust Account',
            code: 'ITA',
            description: 'Fideicomiso de inversión',
            features: [
              'Inversiones diversificadas',
              'Gestión profesional',
              'Reinversión automática',
              'Reporting trimestral'
            ],
            products: ['Stocks', 'Bonds', 'Crypto', 'Real Estate', 'Commodities']
          }
        },
        // PROTECCIÓN LEGAL
        legalProtection: {
          assetProtection: 'Activos en trust separados de activos personales',
          creditorProtection: 'Protegido contra acreedores del beneficiario',
          divorceProtection: 'No se considera propiedad marital',
          lawsuitProtection: 'Aislado de demandas personales',
          probateAvoidance: 'Transferencia directa sin proceso judicial',
          privacy: 'No aparece en registros públicos'
        },
        // GOBIERNO DEL TRUST
        governance: {
          trustee: 'IERAHKWA Trust Company (fiduciario profesional)',
          successor: 'Designado por el ciudadano',
          protector: 'Superintendencia de Bancos IERAHKWA',
          governing_law: 'Ley Soberana de IERAHKWA',
          jurisdiction: 'Territorio Soberano IERAHKWA'
        },
        // ═══════════════════════════════════════════════════════════════════════
        // ★★★ ELEGIBILIDAD - QUIÉN PUEDE ABRIR CUENTAS Y TRUST COMPANIES ★★★
        // ═══════════════════════════════════════════════════════════════════════
        eligibility: {
          principle: 'Solo CIUDADANOS y RESIDENTES pueden abrir cuentas',
          foreignersNote: 'Extranjeros NO pueden abrir cuentas - solo pueden pagar con sus tarjetas',
          categories: {
            citizen: {
              status: 'CITIZEN',
              name: 'Ciudadano Soberano',
              canOpenAccounts: true,
              canOpenTrustCompany: true,
              canBeShareHolder: true,
              accountTypes: ['ALL'],
              description: 'Ciudadano pleno con todos los derechos bancarios',
              requirements: [
                'Certificado de Ciudadanía IERAHKWA',
                'ID Soberano válido',
                'Verificación biométrica'
              ],
              benefits: [
                'Abrir cualquier tipo de cuenta Trust',
                'Crear Trust Companies',
                'Ser accionista de bancos',
                'Sin límites de transacción',
                'Acceso a todos los servicios financieros'
              ]
            },
            resident: {
              status: 'RESIDENT',
              name: 'Residente',
              canOpenAccounts: true,
              canOpenTrustCompany: false,
              canBeShareHolder: false,
              accountTypes: ['Personal Trust', 'Savings', 'Checking'],
              description: 'Residente con permiso de residencia válido',
              requirements: [
                'Permiso de Residencia IERAHKWA',
                'ID de Residente válido',
                'Verificación de domicilio',
                'Verificación biométrica'
              ],
              benefits: [
                'Abrir cuentas Trust personales',
                'Cuenta corriente y ahorro',
                'Wallet digital',
                'Transferencias internas'
              ],
              restrictions: [
                'No puede crear Trust Companies',
                'No puede ser accionista de bancos',
                'Límite de transacción: $50,000/mes'
              ]
            },
            citizenProbation: {
              status: 'CITIZEN_PROBATION',
              name: 'Ciudadano en Período de Prueba',
              canOpenAccounts: true,
              canOpenTrustCompany: false,
              canBeShareHolder: false,
              accountTypes: ['Personal Trust', 'Savings', 'Checking'],
              description: 'Ciudadano en proceso de obtención de ciudadanía plena',
              probationPeriod: '1-3 años',
              requirements: [
                'Certificado de Ciudadanía en Prueba',
                'ID provisional válido',
                'Verificación biométrica',
                'Sponsor ciudadano (opcional)'
              ],
              benefits: [
                'Abrir cuentas Trust personales',
                'Cuenta corriente y ahorro',
                'Wallet digital',
                'Transferencias internas'
              ],
              restrictions: [
                'No puede crear Trust Companies (hasta ciudadanía plena)',
                'No puede ser accionista (hasta ciudadanía plena)',
                'Límite de transacción: $25,000/mes',
                'Revisión periódica de estatus'
              ],
              pathToCitizenship: 'Al completar período de prueba → Ciudadano pleno'
            },
            foreigner: {
              status: 'FOREIGNER',
              name: 'Extranjero / Visitante',
              canOpenAccounts: false,
              canOpenTrustCompany: false,
              canBeShareHolder: false,
              accountTypes: [],
              description: 'NO puede abrir cuentas - PERO puede ENVIAR y RECIBIR dinero globalmente',
              // ═══════════════════════════════════════════════════════════════
              // ★ LO QUE SÍ PUEDEN HACER LOS EXTRANJEROS
              // ═══════════════════════════════════════════════════════════════
              allowedActions: {
                payments: {
                  name: 'Pagos con Tarjeta',
                  description: 'Pagar en comercios IERAHKWA',
                  methods: ['Visa', 'Mastercard', 'Amex', 'Discover', 'UnionPay', 'JCB'],
                  where: ['POS', 'ATM', 'Online', 'Mobile']
                },
                sendMoney: {
                  name: 'ENVIAR Dinero a IERAHKWA',
                  description: 'Extranjeros pueden enviar dinero a ciudadanos/residentes',
                  methods: [
                    'Wire Transfer (transferencia bancaria)',
                    'Remesas (Western Union, MoneyGram, etc.)',
                    'Crypto (BTC, ETH, USDT)',
                    'Tarjeta de crédito/débito',
                    'PayPal, Wise, Remitly',
                    'ACH Internacional',
                    'SWIFT'
                  ],
                  recipients: 'Ciudadanos y Residentes con cuenta Trust',
                  limits: 'Sin límite de envío',
                  fees: 'Según método de envío'
                },
                receiveMoney: {
                  name: 'RECIBIR Dinero desde IERAHKWA',
                  description: 'Extranjeros pueden recibir pagos de ciudadanos/residentes',
                  methods: [
                    'Wire Transfer a su banco',
                    'Remesas internacionales',
                    'Crypto a su wallet',
                    'PayPal, Wise, etc.',
                    'SWIFT a cualquier banco del mundo',
                    'Western Union cash pickup'
                  ],
                  senders: 'Ciudadanos y Residentes',
                  useCases: [
                    'Pago por servicios prestados',
                    'Compra de productos',
                    'Pagos comerciales',
                    'Freelance / Trabajo remoto',
                    'Exportaciones'
                  ]
                },
                cashServices: {
                  name: 'Servicios en Efectivo',
                  description: 'Cambio de divisas y retiros',
                  services: [
                    'Retiro ATM con tarjeta extranjera',
                    'Cambio de divisas en casas de cambio',
                    'Recibir efectivo (cash pickup) de remesas'
                  ]
                },
                commerce: {
                  name: 'Comercio',
                  description: 'Comprar y vender',
                  allowed: [
                    'Comprar productos y servicios',
                    'Vender productos y servicios (reciben pago)',
                    'Turismo y hospedaje',
                    'Alquiler de vehículos',
                    'Servicios profesionales'
                  ]
                }
              },
              // ═══════════════════════════════════════════════════════════════
              // ✗ LO QUE NO PUEDEN HACER LOS EXTRANJEROS
              // ═══════════════════════════════════════════════════════════════
              prohibited: [
                'Abrir cualquier tipo de cuenta bancaria',
                'Crear Trust o Trust Company',
                'Ser accionista de bancos',
                'Tener Wallet IERAHKWA',
                'Guardar dinero en el sistema (solo transitar)',
                'Acceso a servicios bancarios internos'
              ],
              globalConnectivity: {
                description: 'Sistema CONECTADO GLOBALMENTE para enviar y recibir',
                inbound: {
                  name: 'Dinero ENTRANDO (del mundo hacia IERAHKWA)',
                  channels: ['SWIFT', 'SIIS', 'Remesas', 'Crypto', 'Cards', 'Wire', 'ACH'],
                  status: 'OPERATIONAL'
                },
                outbound: {
                  name: 'Dinero SALIENDO (de IERAHKWA hacia el mundo)',
                  channels: ['SWIFT', 'SIIS', 'Remesas', 'Crypto', 'Wire', 'Western Union'],
                  status: 'OPERATIONAL'
                }
              },
              note: 'Extranjeros pueden TRANSACCIONAR globalmente, solo no pueden TENER CUENTAS. Para tener cuenta → Solicitar Residencia o Ciudadanía'
            }
          },
          verificationProcess: {
            step1: 'Verificar estatus (Ciudadano/Residente/Probation)',
            step2: 'Verificar documentos de identidad',
            step3: 'Verificación biométrica (huella, rostro)',
            step4: 'Verificación de domicilio (si aplica)',
            step5: 'Aprobación por Trust Company',
            step6: 'Apertura de cuenta Trust'
          },
          kycLevels: {
            basic: {
              name: 'KYC Básico',
              forStatus: ['RESIDENT', 'CITIZEN_PROBATION'],
              requires: ['ID', 'Selfie', 'Domicilio'],
              limits: { daily: 5000, monthly: 25000 }
            },
            full: {
              name: 'KYC Completo',
              forStatus: ['CITIZEN'],
              requires: ['ID', 'Biométricos', 'Domicilio', 'Ingresos'],
              limits: { daily: 'Sin límite', monthly: 'Sin límite' }
            },
            enhanced: {
              name: 'KYC Mejorado',
              forStatus: ['CITIZEN'],
              requires: ['ID', 'Biométricos', 'Domicilio', 'Ingresos', 'Patrimonio'],
              limits: { daily: 'Sin límite', monthly: 'Sin límite' },
              forServices: ['Trust Company', 'Private Banking', 'Investment Trust']
            }
          }
        }
      },
      // PRIVATE BANKING (dentro del sistema Trust)
      privateBanking: {
        name: 'Private Banking',
        description: 'Banca privada para ciudadanos de alto patrimonio',
        path: '/private-banking',
        icon: '👔',
        minimumAssets: '$500,000 WPM',
        services: [
          'Wealth Management',
          'Portfolio Advisory',
          'Tax Planning',
          'Family Office',
          'Concierge Banking',
          'Dedicated Relationship Manager'
        ]
      },
      // ASSET MANAGEMENT (dentro del sistema Trust)
      assetManagement: {
        name: 'Asset Management',
        description: 'Gestión de activos para Trust Accounts',
        path: '/asset-management',
        icon: '💼',
        products: [
          { name: 'IERAHKWA Growth Fund', type: 'Mutual Fund', risk: 'High', return: '12% target' },
          { name: 'IERAHKWA Income Fund', type: 'Bond Fund', risk: 'Low', return: '6% target' },
          { name: 'IERAHKWA Balanced Fund', type: 'Balanced', risk: 'Medium', return: '8% target' },
          { name: 'IERAHKWA Indigenous Fund', type: 'ESG', risk: 'Medium', return: '9% target' },
          { name: 'IERAHKWA Crypto Fund', type: 'Crypto', risk: 'High', return: '15% target' },
          { name: 'IERAHKWA Real Estate Fund', type: 'REIT', risk: 'Medium', return: '7% target' }
        ]
      },
      // REMESAS Y TRANSFERENCIAS
      remittances: {
        name: 'Remesas y Transferencias',
        businessModel: 'retail',
        services: [
          {
            id: 'remesas',
            name: 'IERAHKWA Remesas',
            description: 'Transferencias internacionales para familias',
            path: '/remesas',
            icon: '💸',
            products: ['Envío a familia', 'Recepción', 'Cash pickup', 'Mobile wallet'],
            corridors: ['USA-México', 'USA-Centro América', 'USA-Caribe', 'Europa-Américas'],
            fees: '1.5% o $5 mínimo'
          },
          {
            id: 'money-transfer',
            name: 'Money Transfer Service',
            description: 'Transferencias rápidas globales',
            path: '/money-transfer',
            icon: '🌍',
            products: ['Wire Transfer', 'Express Transfer', 'Same-day', 'Economy']
          }
        ]
      },
      // MICROFINANZAS
      microfinance: {
        name: 'Microfinanzas',
        businessModel: 'retail',
        services: [
          {
            id: 'microfinance-bank',
            name: 'Banco de Microfinanzas IERAHKWA',
            description: 'Microcréditos para comunidades',
            path: '/microfinance',
            icon: '🌱',
            products: ['Microcréditos', 'Crédito grupal', 'Ahorro comunitario', 'Microseguros'],
            targetAudience: 'Comunidades indígenas, artesanos, pequeños agricultores',
            loanRange: '$100 - $10,000'
          },
          {
            id: 'grameen-model',
            name: 'Crédito Comunitario Grameen',
            description: 'Modelo de préstamos grupales',
            path: '/grameen',
            icon: '👥',
            products: ['Préstamos grupales', 'Garantía solidaria', 'Ahorro obligatorio']
          }
        ]
      },
      // COOPERATIVAS
      cooperatives: {
        name: 'Cooperativas de Ahorro y Crédito',
        businessModel: 'retail',
        services: [
          {
            id: 'coop-aguila',
            name: 'Cooperativa Águila',
            description: 'Cooperativa región Norte',
            path: '/coop-aguila',
            icon: '🦅',
            region: 'Norte',
            products: ['Ahorro', 'Crédito', 'Vivienda', 'Educación']
          },
          {
            id: 'coop-quetzal',
            name: 'Cooperativa Quetzal',
            description: 'Cooperativa región Centro',
            path: '/coop-quetzal',
            icon: '🐦',
            region: 'Centro',
            products: ['Ahorro', 'Crédito', 'Agricultura', 'Artesanías']
          },
          {
            id: 'coop-condor',
            name: 'Cooperativa Cóndor',
            description: 'Cooperativa región Sur',
            path: '/coop-condor',
            icon: '🦅',
            region: 'Sur',
            products: ['Ahorro', 'Crédito', 'Minería', 'Textiles']
          },
          {
            id: 'coop-taino',
            name: 'Cooperativa Taíno',
            description: 'Cooperativa región Caribe',
            path: '/coop-taino',
            icon: '🌴',
            region: 'Caribe',
            products: ['Ahorro', 'Crédito', 'Turismo', 'Pesca']
          }
        ]
      },
      // CASAS DE CAMBIO
      exchangeHouses: {
        name: 'Casas de Cambio',
        businessModel: 'retail',
        services: [
          {
            id: 'casa-cambio',
            name: 'Casa de Cambio IERAHKWA',
            description: 'Cambio de divisas retail',
            path: '/casa-cambio',
            icon: '💱',
            products: ['Compra/venta divisas', 'Cheques viajero', 'Giros'],
            currencies: ['USD', 'EUR', 'MXN', 'BRL', 'WPM', 'BTC']
          }
        ]
      },
      // FACTORING Y LEASING
      alternativeFinance: {
        name: 'Financiamiento Alternativo',
        businessModel: 'wholesale',
        services: [
          {
            id: 'factoring',
            name: 'Factoring IERAHKWA',
            description: 'Descuento de facturas',
            path: '/factoring',
            icon: '📄',
            products: ['Factoring doméstico', 'Factoring internacional', 'Confirming', 'Reverse factoring']
          },
          {
            id: 'leasing',
            name: 'Leasing IERAHKWA',
            description: 'Arrendamiento financiero',
            path: '/leasing',
            icon: '🚗',
            products: ['Leasing equipos', 'Leasing vehículos', 'Leasing inmobiliario', 'Sale & leaseback']
          }
        ]
      },
      // SEGUROS
      insurance: {
        name: 'Seguros',
        businessModel: 'retail',
        services: [
          {
            id: 'insurance-company',
            name: 'Seguros IERAHKWA',
            description: 'Compañía de seguros soberana',
            path: '/seguros',
            icon: '🛡️',
            products: ['Vida', 'Salud', 'Auto', 'Hogar', 'Empresarial', 'Agrícola']
          },
          {
            id: 'reinsurance',
            name: 'Reaseguro IERAHKWA',
            description: 'Reaseguro para aseguradoras',
            path: '/reaseguro',
            icon: '🔄',
            products: ['Treaty Reinsurance', 'Facultative', 'Catastrophe bonds']
          }
        ]
      }
    }
  }
};

// ═══════════════════════════════════════════════════════════════════════════════
// MODELOS DE NEGOCIO BANCARIO (según BIS)
// ═══════════════════════════════════════════════════════════════════════════════
const BUSINESS_MODELS = {
  retail: {
    name: 'Banca Comercial - Financiación Minorista',
    code: 'RETAIL',
    description: 'Alta proporción de préstamos, financiación estable, depósitos de clientes',
    characteristics: {
      loans_ratio: 'Alto (62%+)',
      deposits_ratio: 'Alto (66%+)',
      stable_funding: 'Muy alto (74%+)',
      cost_income: 'Moderado',
      roe_volatility: 'Baja',
      profitability: 'Alta y estable'
    },
    services: ['Wallet', 'Cuentas', 'Préstamos', 'Depósitos', 'Remesas'],
    paths: ['/wallet', '/bdet-accounts', '/bank-worker']
  },
  wholesale: {
    name: 'Banca Comercial - Financiación Mayorista',
    code: 'WHOLESALE',
    description: 'Financiación en mercados de capitales, deuda mayorista, interbancario',
    characteristics: {
      loans_ratio: 'Alto (65%+)',
      wholesale_debt: 'Alto (37%+)',
      interbank: 'Alto (14%+)',
      deposits_ratio: 'Moderado (36%)',
      cost_income: 'Bajo (más eficiente)',
      capital_buffers: 'Delgados'
    },
    services: ['Super Bank Global', 'SIIS', 'Clearing House', 'CryptoHost'],
    paths: ['/super-bank-global', '/siis', '/cryptohost']
  },
  trading: {
    name: 'Banca de Negociación (Trading)',
    code: 'TRADING',
    description: 'Orientada a mercados de capitales, títulos negociables, derivados',
    characteristics: {
      trading_assets: 'Alto (51%+)',
      interbank_activity: 'Muy alto (20%+)',
      fee_income: 'Alto (44%+)',
      cost_income: 'Alto',
      roe_volatility: 'Alta',
      size: 'Mayor que otros modelos'
    },
    services: ['Mamey Futures', 'TradeX', 'NET10', 'Forex', 'Derivados'],
    paths: ['/mamey-futures', '/tradex', '/net10', '/forex']
  }
};

// ═══════════════════════════════════════════════════════════════════════════════
// TIPOS DE LICENCIA BANCARIA
// ═══════════════════════════════════════════════════════════════════════════════
const BANKING_LICENSES = {
  general: {
    code: 'LG',
    name: 'Licencia General',
    description: 'Operaciones bancarias nacionales e internacionales',
    capitalMinimo: 10000000,
    scope: ['national', 'international'],
    activities: ['deposits', 'loans', 'forex', 'trade_finance', 'investments']
  },
  international: {
    code: 'LI',
    name: 'Licencia Internacional',
    description: 'Solo operaciones offshore con no residentes',
    capitalMinimo: 3000000,
    scope: ['international'],
    activities: ['offshore_deposits', 'offshore_loans', 'forex', 'trade_finance']
  },
  representation: {
    code: 'LR',
    name: 'Licencia de Representación',
    description: 'Oficinas de representación',
    capitalMinimo: 250000,
    scope: ['representation'],
    activities: ['promotion', 'liaison']
  },
  microfinance: {
    code: 'LM',
    name: 'Licencia de Microfinanzas',
    description: 'Microcréditos y ahorro popular',
    capitalMinimo: 1000000,
    scope: ['national'],
    activities: ['microloans', 'savings', 'remittances']
  },
  cooperative: {
    code: 'LC',
    name: 'Licencia Cooperativa',
    description: 'Cooperativas de ahorro y crédito',
    capitalMinimo: 500000,
    scope: ['local'],
    activities: ['member_deposits', 'member_loans', 'community_services']
  }
};

// ═══════════════════════════════════════════════════════════════════════════════
// API DE CONSULTA - FUNCIONES COMPLETAS
// ═══════════════════════════════════════════════════════════════════════════════

function getHierarchyLevel(level) {
  const levels = {
    1: FINANCIAL_HIERARCHY.level1_IFIs,
    2: FINANCIAL_HIERARCHY.level2_ClearingInfrastructure,
    3: FINANCIAL_HIERARCHY.level3_Regulation,
    4: FINANCIAL_HIERARCHY.level4_CentralBanks,
    5: FINANCIAL_HIERARCHY.level5_DevelopmentBanks,
    6: FINANCIAL_HIERARCHY.level6_RegionalBanks,
    7: FINANCIAL_HIERARCHY.level7_NationalBanks,
    8: FINANCIAL_HIERARCHY.level8_CommercialBanks,
    9: FINANCIAL_HIERARCHY.level9_SpecializedServices
  };
  return levels[level] || null;
}

function getBusinessModel(model) {
  return BUSINESS_MODELS[model] || null;
}

function getLicense(type) {
  return BANKING_LICENSES[type] || null;
}

function getAllLicenses() {
  return BANKING_LICENSES;
}

function getAllInstitutions() {
  const all = [];
  // Level 1 - IFIs
  if (FINANCIAL_HIERARCHY.level1_IFIs?.institutions) {
    all.push(...FINANCIAL_HIERARCHY.level1_IFIs.institutions);
  }
  // Level 2 - Clearing Infrastructure
  if (FINANCIAL_HIERARCHY.level2_ClearingInfrastructure) {
    const cl = FINANCIAL_HIERARCHY.level2_ClearingInfrastructure;
    if (cl.clearingHouse) all.push(cl.clearingHouse);
    if (cl.rtgs) all.push(cl.rtgs);
    if (cl.ach) all.push(cl.ach);
    if (cl.swiftGateway) all.push(cl.swiftGateway);
    if (cl.cardNetwork) all.push(cl.cardNetwork);
  }
  // Level 3 - Regulation
  if (FINANCIAL_HIERARCHY.level3_Regulation?.institutions) {
    all.push(...FINANCIAL_HIERARCHY.level3_Regulation.institutions);
  }
  // Level 4 - Central Banks
  if (FINANCIAL_HIERARCHY.level4_CentralBanks?.institutions) {
    all.push(...FINANCIAL_HIERARCHY.level4_CentralBanks.institutions);
  }
  // Level 5 - Development Banks
  if (FINANCIAL_HIERARCHY.level5_DevelopmentBanks?.institutions) {
    all.push(...FINANCIAL_HIERARCHY.level5_DevelopmentBanks.institutions);
  }
  // Level 6 - Regional Banks
  if (FINANCIAL_HIERARCHY.level6_RegionalBanks?.institutions) {
    all.push(...FINANCIAL_HIERARCHY.level6_RegionalBanks.institutions);
  }
  // Level 7 - National Banks
  if (FINANCIAL_HIERARCHY.level7_NationalBanks) {
    const nb = FINANCIAL_HIERARCHY.level7_NationalBanks;
    if (nb.official?.institutions) all.push(...nb.official.institutions);
    if (nb.private?.institutions) all.push(...nb.private.institutions);
    if (nb.international?.institutions) all.push(...nb.international.institutions);
    if (nb.foreignBranches?.institutions) all.push(...nb.foreignBranches.institutions);
  }
  // Level 8 - Commercial Banks
  if (FINANCIAL_HIERARCHY.level8_CommercialBanks?.categories) {
    Object.values(FINANCIAL_HIERARCHY.level8_CommercialBanks.categories).forEach(cat => {
      if (cat.services) all.push(...cat.services);
    });
  }
  // Level 9 - Specialized Services
  if (FINANCIAL_HIERARCHY.level9_SpecializedServices?.categories) {
    Object.values(FINANCIAL_HIERARCHY.level9_SpecializedServices.categories).forEach(cat => {
      if (cat.services) all.push(...cat.services);
    });
  }
  return all;
}

function getInstitutionById(id) {
  return getAllInstitutions().find(i => i.id === id);
}

function getCentralBankFunctions() {
  return FINANCIAL_HIERARCHY.level4_CentralBanks.coreFunctions;
}

function getMonetaryInstruments() {
  return FINANCIAL_HIERARCHY.level4_CentralBanks.monetaryInstruments;
}

function getRegionalBanks() {
  return FINANCIAL_HIERARCHY.level6_RegionalBanks.institutions;
}

function getNationalBanks() {
  const nb = FINANCIAL_HIERARCHY.level7_NationalBanks;
  return {
    official: nb.official?.institutions || [],
    private: nb.private?.institutions || [],
    international: nb.international?.institutions || [],
    foreign: nb.foreignBranches?.institutions || []
  };
}

function getClearingInfrastructure() {
  return FINANCIAL_HIERARCHY.level2_ClearingInfrastructure;
}

function getRegulators() {
  return FINANCIAL_HIERARCHY.level3_Regulation.institutions;
}

function getBanksByRegion(region) {
  const all = getAllInstitutions();
  return all.filter(i => i.region && i.region.toLowerCase() === region.toLowerCase());
}

function getBanksByLicense(licenseType) {
  const all = getAllInstitutions();
  return all.filter(i => i.license === licenseType);
}

function getBanksByBusinessModel(model) {
  const all = getAllInstitutions();
  return all.filter(i => i.businessModel === model);
}

// Simulación de operaciones de banco central
const CentralBankOperations = {
  // Ajustar tasa de interés
  setInterestRate(newRate, reason) {
    FINANCIAL_HIERARCHY.level2_CentralBanks.monetaryInstruments.interestRates.currentRate = newRate;
    return {
      success: true,
      operation: 'SET_INTEREST_RATE',
      newRate,
      reason,
      timestamp: new Date().toISOString(),
      effectiveDate: new Date(Date.now() + 86400000).toISOString() // Efectivo mañana
    };
  },
  // Operación de mercado abierto
  openMarketOperation(type, amount, security) {
    return {
      success: true,
      operation: 'OPEN_MARKET_OPERATION',
      type, // 'BUY' or 'SELL'
      amount,
      security,
      effect: type === 'BUY' ? 'Aumenta liquidez' : 'Reduce liquidez',
      timestamp: new Date().toISOString()
    };
  },
  // Modificar encaje legal
  setReserveRequirement(newRate) {
    FINANCIAL_HIERARCHY.level2_CentralBanks.monetaryInstruments.reserveRequirements.rate = newRate;
    return {
      success: true,
      operation: 'SET_RESERVE_REQUIREMENT',
      newRate,
      timestamp: new Date().toISOString()
    };
  },
  // Préstamo de ventana de descuento
  discountWindowLoan(bankId, amount, collateral) {
    return {
      success: true,
      operation: 'DISCOUNT_WINDOW_LOAN',
      bankId,
      amount,
      collateral,
      rate: FINANCIAL_HIERARCHY.level2_CentralBanks.monetaryInstruments.discountWindow.rate,
      timestamp: new Date().toISOString()
    };
  }
};

module.exports = {
  FINANCIAL_HIERARCHY,
  BUSINESS_MODELS,
  BANKING_LICENSES,
  getHierarchyLevel,
  getBusinessModel,
  getLicense,
  getAllLicenses,
  getAllInstitutions,
  getInstitutionById,
  getCentralBankFunctions,
  getMonetaryInstruments,
  getRegionalBanks,
  getNationalBanks,
  getClearingInfrastructure,
  getRegulators,
  getBanksByRegion,
  getBanksByLicense,
  getBanksByBusinessModel,
  CentralBankOperations
};

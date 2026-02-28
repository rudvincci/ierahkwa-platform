#!/usr/bin/env python3
"""
create-crypto-extras.py
Genera 8 plataformas cripto/blockchain adicionales para el ecosistema Ierahkwa.
Cada una sigue Pattern B con ~200-220 lineas de HTML.
"""
import os, pathlib

BASE = pathlib.Path(__file__).resolve().parent.parent  # 02-plataformas-html/

PLATFORMS = [
    {
        "dir": "trading-bot-soberano",
        "title": "Trading Bot Soberano",
        "subtitle": "Bot de Trading Automatizado con IA Soberana",
        "icon": "\U0001f916",
        "accent": "#ffd600",
        "description": "Bot de trading automatizado que reemplaza Freqtrade, 3Commas y Cryptohopper. Estrategias personalizables en MameyLang, backtesting con datos hist\u00f3ricos, ejecuci\u00f3n multi-exchange, machine learning para predicci\u00f3n de mercado, risk management autom\u00e1tico, zero cloud \u2014 se ejecuta en nodos soberanos.",
        "metrics": [("50+", "Estrategias"), ("ML", "Predicci\u00f3n"), ("Multi", "Exchange"), ("Backtest", "Hist\u00f3rico"), ("Risk", "Mgmt"), ("0", "Cloud")],
        "cards": [
            ("\U0001f4dc", "Estrategias en MameyLang", "Define estrategias de trading con el lenguaje soberano MameyLang. Tipado est\u00e1tico, backtesting integrado y compilaci\u00f3n a bytecode optimizado para ejecuci\u00f3n determinista."),
            ("\U0001f4ca", "Backtesting con Datos Hist\u00f3ricos", "Motor de backtesting que simula estrategias contra a\u00f1os de datos hist\u00f3ricos. M\u00e9tricas de Sharpe ratio, max drawdown, win rate y profit factor en segundos."),
            ("\U0001f9e0", "ML Predicci\u00f3n de Mercado", "Modelos de machine learning entrenados en datos de mercado soberano. LSTM para series temporales, random forest para se\u00f1ales, ensemble para decisi\u00f3n final."),
            ("\U0001f310", "Ejecuci\u00f3n Multi-Exchange", "Conexi\u00f3n simult\u00e1nea a m\u00faltiples exchanges para arbitraje y ejecuci\u00f3n \u00f3ptima. API unificada que abstrae diferencias entre plataformas."),
            ("\U0001f6e1\ufe0f", "Risk Management Autom\u00e1tico", "L\u00edmites de p\u00e9rdida diaria, exposici\u00f3n m\u00e1xima por par, correlaci\u00f3n de portafolio y circuit breakers autom\u00e1ticos ante volatilidad extrema."),
            ("\U0001f4b9", "Grid Trading Inteligente", "Estrategia de grid trading con niveles din\u00e1micos basados en volatilidad. Ajuste autom\u00e1tico de spread y tama\u00f1o de posici\u00f3n seg\u00fan condiciones de mercado."),
            ("\U0001f504", "DCA Automatizado", "Dollar-cost averaging programable con frecuencia, monto y condiciones personalizables. Soporte para DCA inverso en toma de ganancias."),
            ("\u26a1", "Stop-Loss Din\u00e1mico", "Stop-loss que se ajusta autom\u00e1ticamente seg\u00fan ATR, volatilidad y momentum. Trailing stop inteligente que maximiza ganancias y minimiza p\u00e9rdidas."),
            ("\U0001f3ae", "Paper Trading Sandbox", "Entorno de simulaci\u00f3n con datos reales pero sin riesgo financiero. Prueba estrategias con capital virtual antes de desplegar en producci\u00f3n."),
            ("\U0001f4c8", "Dashboard de Rendimiento", "Panel con m\u00e9tricas de rendimiento en tiempo real: PnL, ROI, Sharpe, drawdown, operaciones activas y gr\u00e1ficos de equity curve."),
        ],
        "apis": [
            ("POST", "/api/v1/bot/start", "Iniciar bot con estrategia y par\u00e1metros. Params: strategy, pair, amount."),
            ("POST", "/api/v1/bot/stop", "Detener bot activo. Cierra posiciones abiertas de forma ordenada."),
            ("GET", "/api/v1/bot/strategies", "Lista de estrategias disponibles con m\u00e9tricas de backtesting."),
            ("GET", "/api/v1/bot/performance", "Rendimiento del bot: PnL, trades, win rate, Sharpe ratio."),
            ("POST", "/api/v1/bot/backtest", "Ejecutar backtest de estrategia. Params: strategy, period, capital."),
            ("GET", "/api/v1/bot/positions", "Posiciones abiertas con P&L, entry price y stop-loss activos."),
        ],
        "db_stores": ["bot-strategies", "backtest-results", "trade-history"],
        "arch": [
            ("var(--accent)", "MARKET DATA", "Feeds en tiempo real \u00b7 Orderbook \u00b7 Velas", "Multi-exchange \u00b7 WebSocket \u00b7 Normalizaci\u00f3n"),
            ("#7c4dff", "ML ENGINE", "LSTM \u00b7 Random Forest \u00b7 Ensemble", "Predicci\u00f3n de precio \u00b7 Se\u00f1ales \u00b7 Confianza"),
            ("#00bcd4", "STRATEGY EXECUTOR", "MameyLang VM \u00b7 Risk Manager \u00b7 Signals", "Ejecuci\u00f3n determin\u00edstica \u00b7 Position sizing \u00b7 Limits"),
            ("var(--accent)", "EXCHANGE API", "Order Management \u00b7 Settlement \u00b7 Balance", "Multi-exchange \u00b7 Retry \u00b7 Audit trail"),
        ],
    },
    {
        "dir": "portfolio-soberano",
        "title": "Portfolio Soberano",
        "subtitle": "Rastreador de Portafolio Cripto con Privacidad Total",
        "icon": "\U0001f4c8",
        "accent": "#ffd600",
        "description": "Rastreador de portafolio que reemplaza Rotki, CoinGecko Portfolio y Delta. Tracking local sin enviar datos a terceros, soporte para 500+ tokens WAMPUM y cripto externas, P&L en tiempo real, alertas de precio, gr\u00e1ficos interactivos, sincronizaci\u00f3n con wallets soberanas.",
        "metrics": [("500+", "Tokens"), ("P&L", "Real-Time"), ("0", "Tracking"), ("Alertas", "Precio"), ("Gr\u00e1ficos", "Interactivos"), ("Sync", "Wallets")],
        "cards": [
            ("\U0001f512", "Tracking 100% Local", "Todos los datos de portafolio se almacenan localmente en IndexedDB y almacenamiento cifrado. Cero datos enviados a servidores externos. Privacidad absoluta."),
            ("\U0001f4b0", "500+ Tokens Soportados", "Soporte nativo para todos los tokens WAMPUM del ecosistema soberano m\u00e1s 500+ criptomonedas externas. Precios actualizados v\u00eda or\u00e1culos descentralizados."),
            ("\U0001f4ca", "P&L en Tiempo Real", "C\u00e1lculo de ganancias y p\u00e9rdidas actualizado cada segundo. Desglose por token, por wallet, por per\u00edodo. Incluye fees y comisiones."),
            ("\U0001f514", "Alertas de Precio Personalizadas", "Configura alertas por precio, porcentaje de cambio, volumen o condiciones m\u00faltiples. Notificaciones locales sin dependencia de servidores push."),
            ("\U0001f4c8", "Gr\u00e1ficos Interactivos", "Gr\u00e1ficos de portafolio con zoom, filtros temporales, comparaci\u00f3n entre assets y visualizaci\u00f3n de composici\u00f3n. Renderizado WebGL para fluidez."),
            ("\U0001f504", "Sync con Wallets Soberanas", "Sincronizaci\u00f3n autom\u00e1tica con wallets del ecosistema soberano. Importaci\u00f3n de saldos y transacciones sin exponer claves privadas."),
            ("\U0001f310", "Multi-Exchange Tracking", "Conecta cuentas de m\u00faltiples exchanges con API keys de solo lectura. Visi\u00f3n consolidada de todos tus activos en un solo dashboard."),
            ("\U0001f4c5", "Rendimiento por Per\u00edodo", "An\u00e1lisis de rendimiento diario, semanal, mensual y anual. Comparaci\u00f3n contra benchmarks como BTC, ETH y WAMPUM index."),
            ("\U0001f4ca", "Distribuci\u00f3n por Asset", "Pie charts y treemaps de distribuci\u00f3n de portafolio por token, por blockchain, por sector. Alertas de concentraci\u00f3n excesiva."),
            ("\U0001f4e5", "Export CSV/PDF", "Exportaci\u00f3n de historial completo en CSV para an\u00e1lisis externo o PDF para reportes formales. Formatos compatibles con contadores."),
        ],
        "apis": [
            ("GET", "/api/v1/portfolio/balance", "Balance total del portafolio con desglose por token y wallet."),
            ("GET", "/api/v1/portfolio/pnl", "P&L realizado y no realizado por per\u00edodo y por asset."),
            ("GET", "/api/v1/portfolio/alerts", "Alertas configuradas con estado actual y historial de triggers."),
            ("POST", "/api/v1/portfolio/sync", "Sincronizar wallets y exchanges. Params: wallet_addresses, api_keys."),
            ("GET", "/api/v1/portfolio/history", "Historial de valor del portafolio con granularidad configurable."),
            ("GET", "/api/v1/portfolio/export", "Exportar datos en CSV o PDF. Params: format, date_range."),
        ],
        "db_stores": ["portfolio-assets", "price-history", "alert-config"],
        "arch": [
            ("var(--accent)", "WALLETS + EXCHANGES", "Blockchain Scan \u00b7 API Keys \u00b7 Import", "Multi-chain \u00b7 Read-only \u00b7 Cifrado local"),
            ("#00bcd4", "AGGREGATOR", "Normalizaci\u00f3n \u00b7 Dedup \u00b7 Merge", "Unificaci\u00f3n de saldos \u00b7 Fee tracking \u00b7 Reconciliaci\u00f3n"),
            ("#7c4dff", "ANALYTICS ENGINE", "P&L \u00b7 ROI \u00b7 Distribuci\u00f3n \u00b7 Alertas", "C\u00e1lculo real-time \u00b7 Benchmarks \u00b7 Tendencias"),
            ("var(--accent)", "DASHBOARD LOCAL", "Gr\u00e1ficos \u00b7 Tablas \u00b7 Export \u00b7 Alertas", "WebGL render \u00b7 IndexedDB \u00b7 Zero cloud"),
        ],
    },
    {
        "dir": "indexador-soberano",
        "title": "Indexador Soberano",
        "subtitle": "Protocolo de Indexaci\u00f3n Blockchain como The Graph",
        "icon": "\U0001f4ca",
        "accent": "#7c4dff",
        "description": "Protocolo de indexaci\u00f3n blockchain que reemplaza The Graph y Subsquid. Indexa todos los eventos de MameyNode blockchain, queries GraphQL en tiempo real, subgraphs soberanos personalizables, curators y indexers incentivados con WAMPUM, datos abiertos para DApps.",
        "metrics": [("1M+", "Eventos"), ("GraphQL", "Queries"), ("Subgraphs", "Custom"), ("Curators", "WAMPUM"), ("Real-Time", "Sync"), ("Incentivos", "WAMPUM")],
        "cards": [
            ("\U0001f50d", "Indexaci\u00f3n de Eventos MameyNode", "Indexa todos los eventos emitidos por smart contracts en MameyNode blockchain. Procesamiento paralelo de bloques con confirmaci\u00f3n de finality antes de commit."),
            ("\u26a1", "Queries GraphQL Real-Time", "Endpoint GraphQL con subscripciones en tiempo real v\u00eda WebSocket. Queries optimizados con cach\u00e9, paginaci\u00f3n cursor-based y filtros complejos."),
            ("\U0001f4dc", "Subgraphs Personalizables", "Define subgraphs con schema YAML para indexar contratos espec\u00edficos. Mappings en MameyLang para transformar eventos en entidades queryables."),
            ("\U0001f3c6", "Curators Incentivados WAMPUM", "Los curators se\u00f1alizan subgraphs de calidad con stake de WAMPUM. Ganan recompensas proporcionales al uso de los subgraphs que curan."),
            ("\U0001f310", "Indexers Distribuidos", "Red de indexers distribuidos en 19 naciones que procesan y sirven datos. Redundancia geogr\u00e1fica y selecci\u00f3n por latencia."),
            ("\U0001f50e", "Schema Auto-Discovery", "Detecci\u00f3n autom\u00e1tica de ABIs y schemas de contratos desplegados. Generaci\u00f3n de subgraphs sugeridos para contratos nuevos."),
            ("\U0001f4dd", "Full-Text Search en Blockchain", "B\u00fasqueda de texto completo sobre datos indexados. \u00cdndices invertidos optimizados para buscar por nombre, direcci\u00f3n, hash o metadata."),
            ("\u23f3", "Time-Travel Queries", "Consulta el estado de cualquier entidad en cualquier bloque hist\u00f3rico. Reconstrucci\u00f3n de snapshots para auditor\u00eda y an\u00e1lisis forense."),
            ("\U0001f517", "Multi-Chain Indexing", "Soporte para indexar m\u00faltiples blockchains adem\u00e1s de MameyNode. Bridge events y cross-chain data unificados en un solo endpoint."),
            ("\U0001f6a6", "API Rate Limiting Justo", "Rate limiting basado en stake de WAMPUM, no en pago a corporaciones. Acceso gratuito para DApps comunitarias con l\u00edmites generosos."),
        ],
        "apis": [
            ("POST", "/api/v1/indexer/subgraph", "Crear subgraph. Params: schema, mappings, contracts, start_block."),
            ("GET", "/api/v1/indexer/query", "Ejecutar query GraphQL contra subgraph. Params: subgraph_id, query."),
            ("GET", "/api/v1/indexer/status", "Estado de sincronizaci\u00f3n del indexer: bloque actual, lag, health."),
            ("GET", "/api/v1/indexer/schema", "Schema GraphQL del subgraph con tipos, queries y subscriptions."),
            ("POST", "/api/v1/indexer/deploy", "Deploy de subgraph compilado a la red de indexers soberanos."),
            ("GET", "/api/v1/indexer/curators", "Lista de curators con stake, recompensas y subgraphs curados."),
        ],
        "db_stores": ["indexer-events", "subgraph-schemas", "curator-stakes"],
        "arch": [
            ("#7c4dff", "MAMEYNODE EVENTS", "Block Processing \u00b7 Event Parsing \u00b7 Finality", "Smart contracts \u00b7 Logs \u00b7 State changes \u00b7 Receipts"),
            ("#00bcd4", "INDEXER NODES", "Mapping Execution \u00b7 Entity Store \u00b7 Cache", "Distribuidos 19 naciones \u00b7 Redundancia \u00b7 Consensus"),
            ("var(--accent)", "GRAPHQL API", "Queries \u00b7 Subscriptions \u00b7 Pagination", "Schema-typed \u00b7 Real-time \u00b7 Rate limited"),
            ("#7c4dff", "DAPPS QUERIES", "Frontend Integration \u00b7 SDK \u00b7 Webhooks", "Auto-generated clients \u00b7 Type-safe \u00b7 Cached"),
        ],
    },
    {
        "dir": "payment-gateway-soberano",
        "title": "Payment Gateway Soberano",
        "subtitle": "Pasarela de Pagos Cripto para Comercios Soberanos",
        "icon": "\U0001f4b3",
        "accent": "#ffd600",
        "description": "Pasarela de pagos cripto que reemplaza BitPay, CoinGate y Stripe Crypto. Acepta WAMPUM y 50+ criptomonedas, conversi\u00f3n autom\u00e1tica, checkout embebido, facturaci\u00f3n recurrente, API para e-commerce, QR codes, POS para tiendas f\u00edsicas, liquidaci\u00f3n en moneda soberana.",
        "metrics": [("50+", "Cryptos"), ("<1s", "Checkout"), ("POS", "F\u00edsico"), ("Factura", "Recurrente"), ("QR", "Code"), ("API", "E-Commerce")],
        "cards": [
            ("\U0001f6d2", "Checkout Embebido Universal", "Widget de checkout que se embebe en cualquier sitio web con 3 l\u00edneas de c\u00f3digo. Detecci\u00f3n autom\u00e1tica de wallet, selecci\u00f3n de moneda y confirmaci\u00f3n instant\u00e1nea."),
            ("\U0001f504", "Conversi\u00f3n Autom\u00e1tica WAMPUM", "Conversi\u00f3n instant\u00e1nea de cualquier criptomoneda recibida a WAMPUM o stablecoin soberana. El comercio recibe siempre el monto exacto sin riesgo de volatilidad."),
            ("\U0001f3ea", "POS para Tiendas F\u00edsicas", "Aplicaci\u00f3n POS que funciona en tablets y smartphones. Genera QR din\u00e1micos, acepta NFC y muestra confirmaci\u00f3n visual y sonora al completar pago."),
            ("\U0001f4c4", "Facturaci\u00f3n Recurrente Cripto", "Subscripciones y pagos recurrentes con smart contracts. El cliente autoriza una vez y los cobros se ejecutan autom\u00e1ticamente seg\u00fan frecuencia."),
            ("\U0001f4f1", "QR Code Payments", "Generaci\u00f3n de QR codes din\u00e1micos con monto, moneda y metadata embebidos. Escaneo con cualquier wallet compatible con est\u00e1ndares soberanos."),
            ("\U0001f50c", "API REST para E-Commerce", "API RESTful completa para integraci\u00f3n con WooCommerce, Shopify, Magento y plataformas custom. SDKs en 8 lenguajes de programaci\u00f3n."),
            ("\U0001f514", "Webhook Notificaciones", "Webhooks configurables para eventos de pago: creado, confirmado, completado, expirado, reembolsado. Retry autom\u00e1tico con backoff exponencial."),
            ("\U0001f4b1", "Multi-Currency Settlement", "Liquidaci\u00f3n en la moneda que elija el comercio: WAMPUM, stablecoin, BTC o fiat equivalente. Frecuencia de liquidaci\u00f3n configurable."),
            ("\u21a9\ufe0f", "Refund Management", "Reembolsos parciales o totales con un click. Smart contract ejecuta devoluci\u00f3n a la wallet original del comprador autom\u00e1ticamente."),
            ("\U0001f4ca", "Dashboard de Ventas", "Panel de control con m\u00e9tricas de ventas, conversi\u00f3n, monedas m\u00e1s usadas, clientes recurrentes y gr\u00e1ficos de tendencia."),
        ],
        "apis": [
            ("POST", "/api/v1/gateway/charge", "Crear cargo de pago. Params: amount, currency, description, redirect_url."),
            ("GET", "/api/v1/gateway/status", "Estado de un pago: pending, confirmed, completed, expired, refunded."),
            ("POST", "/api/v1/gateway/invoice", "Crear factura con items, impuestos y descuentos. Genera link de pago."),
            ("GET", "/api/v1/gateway/transactions", "Historial de transacciones con filtros por fecha, moneda y estado."),
            ("POST", "/api/v1/gateway/refund", "Reembolsar pago completo o parcial. Params: charge_id, amount."),
            ("POST", "/api/v1/gateway/webhook", "Configurar webhook endpoint. Params: url, events, secret."),
        ],
        "db_stores": ["gateway-transactions", "invoice-cache", "webhook-queue"],
        "arch": [
            ("var(--accent)", "COMERCIO", "Checkout SDK \u00b7 POS App \u00b7 API REST", "E-commerce \u00b7 Tienda f\u00edsica \u00b7 Subscripciones"),
            ("#00bcd4", "CHECKOUT SDK", "Widget \u00b7 QR Code \u00b7 Wallet Detection", "Multi-currency \u00b7 NFC \u00b7 Instant confirm"),
            ("#7c4dff", "PAYMENT PROCESSOR", "Conversion \u00b7 Routing \u00b7 Settlement", "Atomic swaps \u00b7 Rate locking \u00b7 Fee calc"),
            ("var(--accent)", "BLOCKCHAIN SETTLEMENT", "MameyNode \u00b7 Smart Contracts \u00b7 Finality", "Confirmaci\u00f3n <1s \u00b7 Immutable \u00b7 Audit trail"),
        ],
    },
    {
        "dir": "supply-chain-soberano",
        "title": "Supply Chain Soberano",
        "subtitle": "Trazabilidad de Cadena de Suministro con Blockchain",
        "icon": "\U0001f4e6",
        "accent": "#43a047",
        "description": "Plataforma de trazabilidad que reemplaza VeChain e IBM Food Trust. Seguimiento de productos desde origen hasta consumidor, certificaci\u00f3n de origen para artesan\u00edas ind\u00edgenas, IoT con sensores soberanos, smart contracts de compliance, trazabilidad alimentaria, anti-falsificaci\u00f3n NFT.",
        "metrics": [("19", "Naciones"), ("IoT", "Sensores"), ("NFT", "Certificados"), ("100%", "Trazable"), ("Anti", "Falsificaci\u00f3n"), ("Blockchain", "Inmutable")],
        "cards": [
            ("\U0001f50d", "Trazabilidad Origen-Consumidor", "Seguimiento completo del producto desde la materia prima hasta el consumidor final. Cada paso registrado en blockchain con timestamp, ubicaci\u00f3n y responsable."),
            ("\U0001f3a8", "Certificaci\u00f3n de Artesan\u00edas Ind\u00edgenas", "NFT de certificaci\u00f3n para artesan\u00edas ind\u00edgenas que garantiza origen, artesano y comunidad. Combate la apropiaci\u00f3n cultural y falsificaciones."),
            ("\U0001f4e1", "IoT Sensores Soberanos", "Red de sensores IoT que monitorean temperatura, humedad, vibraci\u00f3n y ubicaci\u00f3n durante transporte. Datos registrados en blockchain cada 5 minutos."),
            ("\U0001f4dc", "Smart Contracts de Compliance", "Contratos inteligentes que verifican autom\u00e1ticamente cumplimiento de regulaciones: temperatura de cadena de fr\u00edo, tiempos de transporte, certificaciones."),
            ("\U0001f96c", "Trazabilidad Alimentaria", "Track de alimentos desde la parcela hasta el plato. Registro de pesticidas, fertilizantes, fecha de cosecha, procesamiento y distribuci\u00f3n."),
            ("\U0001f6e1\ufe0f", "Anti-Falsificaci\u00f3n con NFT", "Cada producto recibe un NFT \u00fanico vinculado a QR f\u00edsico. El consumidor escanea y verifica autenticidad, origen y cadena de custodia completa."),
            ("\U0001f69a", "Documentos de Transporte", "Digitalizaci\u00f3n de gu\u00edas de remisi\u00f3n, facturas y certificados fitosanitarios en blockchain. Eliminaci\u00f3n de papeleo con validez legal."),
            ("\U0001f50e", "Auditor\u00eda Autom\u00e1tica", "Smart contracts que auditan autom\u00e1ticamente cada paso de la cadena. Alertas instant\u00e1neas ante desviaciones de temperatura, retrasos o manipulaci\u00f3n."),
            ("\U0001f4ca", "Dashboard de Cadena", "Visualizaci\u00f3n en mapa de toda la cadena de suministro. Estado en tiempo real de cada lote, env\u00edo y punto de control."),
            ("\U0001f50c", "API para Retailers", "API para que retailers y marketplaces consulten trazabilidad de productos. Widget embebible para mostrar origen en p\u00e1ginas de producto."),
        ],
        "apis": [
            ("POST", "/api/v1/supply/track", "Registrar evento de tracking. Params: product_id, location, status, data."),
            ("GET", "/api/v1/supply/trace", "Trazar historial completo de un producto por ID o QR code."),
            ("POST", "/api/v1/supply/certify", "Emitir certificado NFT de origen. Params: product, artisan, community."),
            ("GET", "/api/v1/supply/products", "Lista de productos registrados con estado actual y \u00faltima ubicaci\u00f3n."),
            ("POST", "/api/v1/supply/iot", "Registrar lectura de sensor IoT. Params: sensor_id, readings, location."),
            ("GET", "/api/v1/supply/audit", "Reporte de auditor\u00eda con desviaciones, alertas y cumplimiento."),
        ],
        "db_stores": ["supply-products", "trace-events", "iot-readings"],
        "arch": [
            ("#43a047", "IoT SENSORES", "Temperatura \u00b7 Humedad \u00b7 GPS \u00b7 Vibraci\u00f3n", "LoRaWAN mesh \u00b7 Solar powered \u00b7 5min intervals"),
            ("#00bcd4", "BLOCKCHAIN REGISTRO", "Event Log \u00b7 Hash Chain \u00b7 Timestamps", "Inmutable \u00b7 Verificable \u00b7 P\u00fablico"),
            ("#7c4dff", "SMART CONTRACTS", "Compliance \u00b7 NFT Certs \u00b7 Alerts", "Validaci\u00f3n autom\u00e1tica \u00b7 Reglas configurables"),
            ("#43a047", "CONSUMIDOR VERIFICACI\u00d3N", "QR Scan \u00b7 NFT Check \u00b7 Origen", "App m\u00f3vil \u00b7 Widget web \u00b7 Trust score"),
        ],
    },
    {
        "dir": "web-descentralizada-soberana",
        "title": "Web Descentralizada Soberana",
        "subtitle": "Hosting Web P2P Sin Servidores Centrales",
        "icon": "\U0001f310",
        "accent": "#00e676",
        "description": "Plataforma de web descentralizada que reemplaza ZeroNet y Web3 hosting. Sitios web alojados en red P2P de 847 nodos, resistente a censura, dominio .nation nativo, SSL descentralizado, CDN P2P, actualizaci\u00f3n en tiempo real, zero downtime, indestructible por dise\u00f1o.",
        "metrics": [("847", "Nodos"), (".nation", "DNS"), ("0", "Downtime"), ("Anti", "Censura"), ("CDN", "P2P"), ("SSL", "Desc.")],
        "cards": [
            ("\U0001f5a5\ufe0f", "Hosting P2P en 847 Nodos", "Tu sitio web se replica autom\u00e1ticamente en 847 nodos soberanos distribuidos en 19 naciones. Si 846 nodos caen, tu sitio sigue online desde el \u00faltimo."),
            ("\U0001f310", "Dominio .nation Nativo", "Sistema de dominios soberano con extensi\u00f3n .nation. Registro descentralizado, sin ICANN, sin censura posible. DNS resuelto por consenso de nodos."),
            ("\U0001f512", "SSL Descentralizado", "Certificados SSL emitidos por la red soberana sin depender de autoridades de certificaci\u00f3n centralizadas. Rotaci\u00f3n autom\u00e1tica y pinning."),
            ("\u26a1", "CDN P2P Soberano", "Red de distribuci\u00f3n de contenido peer-to-peer. Los visitantes cercanos sirven contenido a nuevos visitantes, reduciendo latencia y carga."),
            ("\U0001f6e1\ufe0f", "Resistente a Censura", "Arquitectura dise\u00f1ada para resistir censura gubernamental y corporativa. Sin punto \u00fanico de fallo, sin kill switch, sin takedown posible."),
            ("\U0001f504", "Actualizaci\u00f3n en Tiempo Real", "Despliega cambios y se propagan a los 847 nodos en menos de 30 segundos. Rollback instant\u00e1neo a cualquier versi\u00f3n anterior."),
            ("\U0001f4e6", "Git-Based Deploys", "Deploy con git push a repositorio soberano. CI/CD integrado que compila, optimiza y distribuye autom\u00e1ticamente a la red P2P."),
            ("\U0001f3d7\ufe0f", "Static Site Generator", "Generador de sitios est\u00e1ticos integrado compatible con Markdown, HTML y templates. Optimizaci\u00f3n autom\u00e1tica de im\u00e1genes, CSS y JavaScript."),
            ("\U0001f4e1", "Bandwidth Compartido", "Los nodos comparten ancho de banda proporcionalmente. Sitios populares no sobrecargan un solo servidor \u2014 la red escala horizontalmente."),
            ("\U0001f4ca", "Analytics Zero-Track", "M\u00e9tricas de visitas, p\u00e1ginas vistas y rendimiento sin tracking de usuarios. Datos agregados y an\u00f3nimos almacenados localmente."),
        ],
        "apis": [
            ("POST", "/api/v1/web/deploy", "Desplegar sitio web a la red P2P. Params: files, domain, config."),
            ("GET", "/api/v1/web/sites", "Lista de sitios desplegados con estado, dominio y m\u00e9tricas."),
            ("POST", "/api/v1/web/domain", "Registrar dominio .nation. Params: name, owner, dns_records."),
            ("GET", "/api/v1/web/status", "Estado de propagaci\u00f3n del sitio en los 847 nodos."),
            ("POST", "/api/v1/web/ssl", "Solicitar certificado SSL soberano. Params: domain, key_type."),
            ("GET", "/api/v1/web/analytics", "Analytics agregados sin tracking: visitas, p\u00e1ginas, rendimiento."),
        ],
        "db_stores": ["web-sites", "domain-registry", "ssl-certificates"],
        "arch": [
            ("#00e676", "C\u00d3DIGO FUENTE", "Git Push \u00b7 CI/CD \u00b7 Build \u00b7 Optimize", "Markdown \u00b7 HTML \u00b7 Static generator \u00b7 Minify"),
            ("#00bcd4", "GIT DEPLOY", "Compile \u00b7 Hash \u00b7 Sign \u00b7 Distribute", "Content-addressable \u00b7 Verificable \u00b7 Versioned"),
            ("#7c4dff", "P2P NETWORK (847 NODOS)", "Replication \u00b7 CDN \u00b7 Load Balance", "19 naciones \u00b7 Geo-distributed \u00b7 Fault-tolerant"),
            ("#00e676", ".NATION DNS", "Domain Resolution \u00b7 SSL \u00b7 Routing", "Descentralizado \u00b7 Anti-censura \u00b7 Consensus"),
        ],
    },
    {
        "dir": "nodo-soberano",
        "title": "Nodo Soberano",
        "subtitle": "Infraestructura de Nodos RPC y API Blockchain",
        "icon": "\U0001f5a5\ufe0f",
        "accent": "#00bcd4",
        "description": "Infraestructura de nodos que reemplaza Infura, Alchemy y NOWNodes. Nodos RPC de MameyNode blockchain auto-hospedados, API endpoints dedicados, WebSocket para subscripciones en tiempo real, archive nodes para datos hist\u00f3ricos, nodos ligeros para mobile, load balancing entre 847 nodos.",
        "metrics": [("847", "Nodos"), ("RPC+WSS", "Protocols"), ("Archive", "Nodes"), ("Mobile", "Light"), ("Load", "Balanced"), ("99.999%", "Uptime")],
        "cards": [
            ("\U0001f5a5\ufe0f", "Nodos RPC Dedicados", "Endpoints RPC dedicados con baja latencia y alta disponibilidad. Soporte para eth_call, eth_sendTransaction y todos los m\u00e9todos est\u00e1ndar de MameyNode."),
            ("\U0001f50c", "WebSocket Subscriptions", "Conexiones WebSocket persistentes para subscripciones en tiempo real: nuevos bloques, logs de contratos, transacciones pendientes y cambios de estado."),
            ("\U0001f4da", "Archive Nodes Hist\u00f3ricos", "Nodos archive que almacenan el estado completo de cada bloque desde el genesis. Queries hist\u00f3ricos sin l\u00edmite temporal para auditor\u00eda y an\u00e1lisis."),
            ("\U0001f4f1", "Light Nodes para Mobile", "Nodos ligeros optimizados para dispositivos m\u00f3viles. Verificaci\u00f3n de transacciones sin descargar toda la blockchain, usando pruebas Merkle."),
            ("\u2696\ufe0f", "Load Balancer Inteligente", "Distribuci\u00f3n inteligente de requests entre 847 nodos considerando latencia, carga y geolocalizaci\u00f3n. Failover autom\u00e1tico sin interrupci\u00f3n."),
            ("\U0001f30d", "Multi-Region 19 Naciones", "Nodos distribuidos en las 19 naciones del ecosistema. Routing geogr\u00e1fico para m\u00ednima latencia y m\u00e1xima resiliencia ante desconexiones regionales."),
            ("\U0001f511", "API Key Management", "Sistema de API keys con permisos granulares, rate limits configurables y m\u00e9tricas de uso por key. Rotaci\u00f3n autom\u00e1tica y revocaci\u00f3n instant\u00e1nea."),
            ("\U0001f6a6", "Rate Limiting Justo", "Rate limiting basado en necesidad, no en capacidad de pago. Cuotas generosas para DApps comunitarias y l\u00edmites proporcionales al stake WAMPUM."),
            ("\U0001f4df", "Health Monitoring", "Monitoreo continuo de salud de cada nodo: latencia, sync status, peer count, disk usage. Alertas autom\u00e1ticas y auto-healing."),
            ("\U0001f4ca", "Dashboard de Infraestructura", "Panel de control con m\u00e9tricas de toda la red: nodos activos, requests/segundo, latencia promedio, distribuci\u00f3n geogr\u00e1fica y uptime."),
        ],
        "apis": [
            ("POST", "/api/v1/node/rpc", "Ejecutar llamada RPC a MameyNode. Params: method, params, block."),
            ("GET", "/api/v1/node/status", "Estado de la red de nodos: activos, sync, latencia, throughput."),
            ("POST", "/api/v1/node/subscribe", "Crear subscripci\u00f3n WebSocket. Params: event_type, filters."),
            ("GET", "/api/v1/node/archive", "Query a archive node. Params: method, params, block_number."),
            ("GET", "/api/v1/node/health", "Health check de nodo espec\u00edfico o de toda la red."),
            ("POST", "/api/v1/node/apikey", "Crear API key con permisos y rate limits. Params: name, permissions."),
        ],
        "db_stores": ["node-config", "rpc-cache", "subscription-state"],
        "arch": [
            ("#00bcd4", "DAPP REQUEST", "JSON-RPC \u00b7 WebSocket \u00b7 REST", "Multi-protocol \u00b7 API key auth \u00b7 Rate limited"),
            ("#7c4dff", "LOAD BALANCER", "Geo-routing \u00b7 Health Check \u00b7 Failover", "Latency-based \u00b7 Weight-based \u00b7 Auto-scale"),
            ("#00e676", "RPC NODE CLUSTER", "Full Nodes \u00b7 Archive \u00b7 Light \u00b7 Validator", "847 nodos \u00b7 19 naciones \u00b7 Redundancia total"),
            ("#00bcd4", "MAMEYNODE BLOCKCHAIN", "Consensus \u00b7 State \u00b7 Execution \u00b7 Storage", "PoS soberano \u00b7 Finality <2s \u00b7 50K TPS"),
        ],
    },
    {
        "dir": "tax-crypto-soberano",
        "title": "Tax Crypto Soberano",
        "subtitle": "Declaraci\u00f3n Fiscal de Criptomonedas Automatizada",
        "icon": "\U0001f9fe",
        "accent": "#ffd600",
        "description": "Herramienta fiscal cripto que reemplaza Raccoin, CoinTracker y Koinly. C\u00e1lculo autom\u00e1tico de ganancias de capital, importaci\u00f3n de transacciones de wallets y exchanges, reportes fiscales para 19 jurisdicciones, FIFO/LIFO/HIFO seleccionable, export para contadores, 100% privado y local.",
        "metrics": [("19", "Jurisdicciones"), ("Auto", "C\u00e1lculo"), ("FIFO/LIFO", "M\u00e9todos"), ("PDF", "Export"), ("100%", "Local"), ("0", "Datos Enviados")],
        "cards": [
            ("\U0001f4b0", "C\u00e1lculo Autom\u00e1tico de Capital Gains", "Motor fiscal que calcula autom\u00e1ticamente ganancias y p\u00e9rdidas de capital para cada transacci\u00f3n. Soporta FIFO, LIFO, HIFO y costo promedio."),
            ("\U0001f4e5", "Importaci\u00f3n Multi-Exchange", "Importa historial de transacciones de 50+ exchanges y wallets. Parseo autom\u00e1tico de CSVs, conexi\u00f3n API y scan de blockchain."),
            ("\U0001f30d", "Reportes para 19 Jurisdicciones", "Formatos de reporte fiscal espec\u00edficos para cada una de las 19 naciones del ecosistema. Adaptaci\u00f3n a legislaci\u00f3n local y formularios oficiales."),
            ("\U0001f4ca", "FIFO/LIFO/HIFO Seleccionable", "Elige el m\u00e9todo de c\u00e1lculo que minimice tu carga fiscal legalmente. Comparaci\u00f3n lado a lado del impacto de cada m\u00e9todo."),
            ("\U0001f4c4", "Export PDF para Contadores", "Genera reportes PDF profesionales listos para entregar a tu contador o autoridad fiscal. Incluye resumen ejecutivo y detalle transacci\u00f3n por transacci\u00f3n."),
            ("\U0001f3e6", "DeFi Tax Tracking", "C\u00e1lculo fiscal para operaciones DeFi: swaps, yield farming, liquidity providing, staking rewards. Cada interacci\u00f3n con smart contract clasificada."),
            ("\U0001f5bc\ufe0f", "NFT Tax Reporting", "Tracking fiscal de compra, venta y royalties de NFTs. C\u00e1lculo de cost basis para colecciones y ediciones m\u00faltiples."),
            ("\U0001f4b9", "Staking Rewards Tax", "Clasificaci\u00f3n y c\u00e1lculo fiscal de recompensas de staking. Tratamiento como ingreso o ganancia de capital seg\u00fan jurisdicci\u00f3n."),
            ("\u26cf\ufe0f", "Mining Income Tracking", "Registro de ingresos de miner\u00eda con costo de energ\u00eda, hardware y depreciaci\u00f3n deducibles. C\u00e1lculo de ingreso neto por per\u00edodo."),
            ("\U0001f50d", "Audit Trail Completo", "Registro inmutable de cada c\u00e1lculo fiscal con trazabilidad completa. Si la autoridad fiscal pregunta, cada n\u00famero tiene respaldo verificable."),
        ],
        "apis": [
            ("GET", "/api/v1/tax/report", "Generar reporte fiscal completo. Params: year, jurisdiction, method."),
            ("POST", "/api/v1/tax/import", "Importar transacciones. Params: source, file/api_key, date_range."),
            ("GET", "/api/v1/tax/gains", "Ganancias de capital por per\u00edodo con desglose por asset."),
            ("POST", "/api/v1/tax/calculate", "Ejecutar c\u00e1lculo fiscal con m\u00e9todo espec\u00edfico. Params: method, year."),
            ("GET", "/api/v1/tax/export", "Exportar reporte en PDF o CSV. Params: format, year, jurisdiction."),
            ("GET", "/api/v1/tax/jurisdictions", "Lista de jurisdicciones soportadas con reglas fiscales vigentes."),
        ],
        "db_stores": ["tax-transactions", "gain-calculations", "report-cache"],
        "arch": [
            ("var(--accent)", "WALLETS + EXCHANGES", "CSV Import \u00b7 API Sync \u00b7 Blockchain Scan", "50+ fuentes \u00b7 Dedup \u00b7 Normalizaci\u00f3n"),
            ("#00bcd4", "TRANSACTION PARSER", "Classify \u00b7 Match \u00b7 Cost Basis", "Trade \u00b7 DeFi \u00b7 NFT \u00b7 Staking \u00b7 Mining"),
            ("#7c4dff", "TAX ENGINE", "FIFO/LIFO/HIFO \u00b7 Gains Calc \u00b7 Rules", "19 jurisdicciones \u00b7 Legislaci\u00f3n actual"),
            ("var(--accent)", "REPORT GENERATOR", "PDF \u00b7 CSV \u00b7 Formularios \u00b7 Audit Trail", "Export profesional \u00b7 100% local \u00b7 Verificable"),
        ],
    },
]


def build_html(p):
    slug = p["dir"]
    title = p["title"]
    subtitle = p["subtitle"]
    icon = p["icon"]
    accent = p["accent"]
    desc = p["description"]
    metrics = p["metrics"]
    cards = p["cards"]
    apis = p["apis"]
    db_stores = p["db_stores"]

    # Build architecture diagram
    arch_layers = p["arch"]
    arch_lines = []
    dash = "\u2500"
    for i, (color, label, content, detail) in enumerate(arch_layers):
        pad_label = dash * max(1, 42 - len(label))
        pad_content = " " * max(1, 45 - len(content))
        pad_detail = " " * max(1, 45 - len(detail))
        arch_lines.append(f'<span style="color:{color}">\u250c\u2500 {label} {pad_label}\u2510</span>')
        arch_lines.append(f'\u2502  {content}{pad_content}\u2502')
        arch_lines.append(f'\u2502  {detail}{pad_detail}\u2502')
        if i < len(arch_layers) - 1:
            arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u252c\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
            arch_lines.append('                   \u2502')
        else:
            arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
    NL = chr(10)
    arch_html = NL.join(arch_lines)

    # Stats HTML
    stats_lines = []
    for val, lbl in metrics:
        stats_lines.append(f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>')
    stats_html = NL.join(stats_lines)

    # Cards HTML
    cards_lines = []
    for cicon, ctitle, cdesc in cards:
        cards_lines.append(f'<article class="card">')
        cards_lines.append(f'<div class="card-icon" aria-hidden="true">{cicon}</div>')
        cards_lines.append(f'<h4>{ctitle}</h4>')
        cards_lines.append(f'<p>{cdesc}</p>')
        cards_lines.append(f'</article>')
    cards_html = NL.join(cards_lines)

    # API HTML
    api_lines = []
    for method, endpoint, adesc in apis:
        color = "#ffd600" if method == "POST" else "#00FF41"
        api_lines.append(f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{endpoint}</code></div>')
        api_lines.append(f'<p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{adesc}</p>')
    api_html = NL.join(api_lines)

    # DB stores JSON
    stores_json = str(db_stores).replace("'", '"')

    html = f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total.">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta property="og:description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total.">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta name="twitter:description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle}.">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} \u2014 Ierahkwa Ne Kanienke</title>
<style>:root{{--accent:{accent}}}</style>
</head>
<body role="document">
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header>
<div class="logo"><div class="logo-icon" aria-hidden="true">{icon}</div><h1>{title}</h1></div>
<nav aria-label="Navegacion principal">
<a href="#dashboard" aria-current="page">Dashboard</a>
<a href="#features">Modulos</a>
<a href="#api">API</a>
<a href="#pricing">Precios</a>
</nav>
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">\u269b\ufe0f</span> Quantum-Safe</span>
</header>

<main id="main">

<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar M\u00f3dulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<div class="stats" role="list" aria-label="Metricas del sistema">
{stats_html}
</div>

<div class="section" id="architecture">
<h2><span aria-hidden="true">\U0001f3d7\ufe0f</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}
</div>
</div>

<div class="section-title" id="features">
<h3>M\u00f3dulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>

<div class="grid">
{cards_html}
</div>

<div class="section" id="api">
<h2><span aria-hidden="true">\U0001f50c</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">
{api_html}
</div>
</div>

<div class="section-title" id="pricing">
<h3>Planes Soberanos</h3>
<p>Empieza gratis. Escala soberanamente.</p>
</div>

<div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(220px,1fr))">
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Guerrero</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">0 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 100 operaciones/mes</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Dashboard b\u00e1sico</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 1 proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte comunidad</li>
</ul>
</div>
<div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Ilimitado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Analytics avanzados</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 API completa</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte prioritario</li>
</ul>
</div>
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Naci\u00f3n</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-naci\u00f3n</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 SLA 99.99%</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Auditor dedicado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte 24/7</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Custom integrations</li>
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">200+ plataformas soberanas &middot; 15 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">\U0001f6e1\ufe0f</span> Seguro</span></div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script>
(function(){{
  var DB_NAME='ierahkwa-{slug}';
  var DB_VER=1;
  var STORES={stores_json};
  var db=null;
  function openDB(){{
    return new Promise(function(resolve,reject){{
      var req=indexedDB.open(DB_NAME,DB_VER);
      req.onupgradeneeded=function(){{
        var d=req.result;
        STORES.forEach(function(s){{
          if(!d.objectStoreNames.contains(s))d.createObjectStore(s,{{keyPath:'id'}})
        }});
      }};
      req.onsuccess=function(){{db=req.result;resolve(db)}};
      req.onerror=function(){{reject(req.error)}}
    }})
  }}
  function showOfflineBanner(show){{
    var b=document.getElementById('offline-banner');
    if(!b){{
      b=document.createElement('div');
      b.id='offline-banner';
      b.style.cssText='position:fixed;bottom:0;left:0;right:0;background:var(--accent);color:#09090d;text-align:center;padding:8px;font-size:13px;font-weight:700;z-index:9999;transform:translateY(100%);transition:transform .3s';
      b.textContent='Modo Offline \u2014 Datos y operaciones pendientes disponibles offline.';
      document.body.appendChild(b)
    }}
    b.style.transform=show?'translateY(0)':'translateY(100%)'
  }}
  function init(){{
    openDB().then(function(){{
      window.addEventListener('online',function(){{showOfflineBanner(false)}});
      window.addEventListener('offline',function(){{showOfflineBanner(true)}});
      if(!navigator.onLine)showOfflineBanner(true);
      console.log('[{slug}] Offline module ready')
    }})
  }}
  init()
}})();
</script>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body>
</html>'''
    return html


def main():
    created = []
    for p in PLATFORMS:
        dirpath = BASE / p["dir"]
        dirpath.mkdir(parents=True, exist_ok=True)
        filepath = dirpath / "index.html"
        html = build_html(p)
        filepath.write_text(html, encoding="utf-8")
        line_count = html.count('\n') + 1
        created.append((p["dir"], line_count))
        print(f"  OK {p['dir']}/index.html -- {line_count} lineas")

    print(f"\n{len(created)} plataformas cripto extras creadas exitosamente.")
    print(f"Base: {BASE}")


if __name__ == "__main__":
    main()

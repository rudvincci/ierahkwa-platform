# 🌺 Resumen completo — Plataforma AI Ierahkwa

**Sovereign Government of Ierahkwa Ne Kanienke**  
Todo lo implementado en el sistema de inteligencia artificial.

---

## 📋 Índice

1. [Visión general](#visión-general)
2. [AI Hub (núcleo)](#ai-hub-núcleo)
3. [ATABEY — Asistente y maestra](#atabey--asistente-y-maestra)
4. [World Intelligence](#world-intelligence)
5. [AI Banker BDET](#ai-banker-bdet)
6. [Otros sistemas AI](#otros-sistemas-ai)
7. [Datos y almacenamiento](#datos-y-almacenamiento)
8. [APIs y accesos](#apis-y-accesos)

---

## Visión general

La plataforma AI está organizada en capas:

```
                    ┌─────────────────────────────────────┐
                    │         ATABEY (Maestra)             │
                    │  Asistente + Control de todos los AI │
                    └─────────────────┬───────────────────┘
                                      │
    ┌─────────────────────────────────┼─────────────────────────────────┐
    │                                 │                                 │
    ▼                                 ▼                                 ▼
┌───────────────┐           ┌─────────────────┐           ┌───────────────────┐
│ AI Hub Core   │           │ World Intel      │           │ AI Banker BDET     │
│ Registry      │           │ Mercados, news   │           │ Banco completo     │
│ Collector     │           │ Trading signals  │           │ Cuentas, préstamos │
│ Learning      │           │ Alertas          │           │ Tarjetas, KYC      │
└───────────────┘           └─────────────────┘           └───────────────────┘
```

- **ATABEY**: nombre indígena taíno (Diosa Madre Suprema). Es la IA que “manda” y organiza al resto.
- **AI Hub**: registro de proyectos, recolección de datos, aprendizaje y auto-mejora.
- **World Intelligence**: información del mundo (mercados, noticias, señales de trading).
- **AI Banker BDET**: banco completo (cuentas, transferencias, préstamos, tarjetas, KYC) dentro de la plataforma AI.

---

## AI Hub (núcleo)

**Ubicación:** `node/ai-hub/`

| Archivo | Función |
|--------|---------|
| `index.js` | Rutas API, inicialización y cierre de todo el AI Hub |
| `project-registry.js` | Registro de todos los proyectos Ierahkwa (25+) |
| `data-collector.js` | Health de servicios, métricas, errores, patrones |
| `learning-engine.js` | Análisis de errores/performance, generación de mejoras, buenas prácticas |
| `world-intelligence.js` | Recolección de datos globales (crypto, forex, noticias, alertas) |
| `atabey-system.js` | Asistente personal ATABEY (comandos, familia, briefing) |
| `atabey-master-controller.js` | Control maestro: asigna tareas a todos los AI workers |

**Qué hace:**

- Registra proyectos por categoría (Infrastructure, Finance, AI, Government, DeFi, etc.).
- Recolecta cada 1 min: health de servicios, métricas, errores.
- Cada 5 min: ciclo de aprendizaje, sugerencias de mejora, prioridad (high/medium/low).
- Endpoints: proyectos, health-status, improvements, learn, analyze-code, errors, patterns.

---

## ATABEY — Asistente y maestra

**Nombre:** ATABEY (Diosa Madre Suprema Taína — Madre Tierra, fertilidad, agua dulce).

### 1. Asistente personal (`atabey-system.js`)

- Responde en español a comandos en lenguaje natural.
- Comandos: Hola, Estado del sistema, Precios de mercado, Señales de trading, Proyectos, Mejoras, Familia, Briefing diario, Ayuda.
- Sistema familiar: miembros, roles (cacique, nitaíno, bohique, naboría), “Family First - 7 Generaciones”.
- Historial de conversaciones y recordatorios.
- APIs: `POST /api/ai-hub/atabey`, `GET /api/ai-hub/atabey/briefing`, `GET /api/ai-hub/atabey/history`, `GET/POST/PUT /api/ai-hub/family`.

### 2. Control maestra (`atabey-master-controller.js`)

- Mantiene registro de todos los **AI workers** (AI Banker BDET, AI Trader, AI Orchestrator, AI Master Builder, AI Replicator, AI Growth Engine, AI Guardian, Sovereign AI, AI Code Generator, World Intelligence, Learning Engine).
- Asigna tareas por tipo (banking, trading, development, security, intelligence, growth).
- Comandos maestros: **producir** (pone a todos a trabajar), **estado**, **workers**, **parar**, **optimizar**.
- Ciclos automáticos: recolección de datos (1 min), análisis de mercado (2 min), seguridad (5 min), optimización (10 min).
- APIs: `POST /api/ai-hub/atabey/master`, `GET /api/ai-hub/atabey/workers`, `GET /api/ai-hub/atabey/production`, `POST /api/ai-hub/atabey/production/start|stop`, `POST /api/ai-hub/atabey/tasks`.

### 3. Dashboard ATABEY

- **Archivo:** `platform/atabey-dashboard.html`
- Chat con ATABEY, briefing diario, estado del sistema, mercado, familia, señales de trading.
- Panel de control maestro: iniciar/parar producción, lista de AI workers, tareas activas/completadas.

---

## World Intelligence

**Ubicación:** `node/ai-hub/world-intelligence.js`

- **Crypto:** Bitcoin, Ethereum, Solana, Cardano, Polkadot, Chainlink, Uniswap (CoinGecko).
- **Forex:** USD, EUR, GBP, JPY, CHF, CAD, AUD, MXN.
- Recolección cada 1 minuto.
- Alertas de precio (above/below) y comprobación automática.
- Análisis de tendencias y generación de **señales de trading** (strong_buy, buy, hold, sell, strong_sell) con confianza.
- APIs: `GET /api/ai-hub/market`, `GET /api/ai-hub/trading-signals`, `GET /api/ai-hub/market-analysis`, `POST/GET /api/ai-hub/alerts`, `POST /api/ai-hub/market/collect`.
- Datos en: `node/data/ai-hub/world-intelligence/` (market-data.json, alerts.json, news.json, predictions.json).

---

## AI Banker BDET

**Ubicación:** `node/ai/ai-banker-bdet.js`  
**BDET:** Banco de Desarrollo Económico y Tecnológico.

### Funcionalidades

| Módulo | Descripción |
|--------|-------------|
| **Cuentas** | Crear cuenta (Corriente, Ahorros, Empresarial, Inversión, Gubernamental, Fideicomiso), IBAN, número de cuenta BDET |
| **Transacciones** | Transferencias, depósitos, retiros; verificación de fraude y AML; estados PENDING → COMPLETED/FAILED/REVIEW/BLOCKED |
| **Préstamos** | Solicitud y aprobación automática (Personal, Empresa, Hipoteca, Auto, Educación); scoring crediticio, DTI, tasas |
| **Tarjetas** | Emisión de débito/crédito/prepago; número con Luhn; CVV, PIN, límites |
| **KYC/Compliance** | Verificación de identidad, domicilio, ingresos; nivel de riesgo; actualización de estado de cuenta |
| **Intereses** | Cálculo y acreditación de intereses por tipo de cuenta |
| **Reportes** | Reportes diarios (y estructura para mensual/anual) |

### Automatización

- Cola de transacciones (procesamiento cada 1 s).
- Cola de préstamos (cada 5 s).
- KYC (cada 10 s).
- Intereses (cada 1 h).
- Reporte diario (cada 24 h).
- Actualización de estado del banco (cada 30 s).

### API (bajo `/api/ai-hub/bdet`)

- `GET /status`, `GET /stats`, `GET /config`
- `POST/GET /accounts`, `GET /accounts/:id`, `GET /accounts/:id/transactions`, `GET /accounts/:id/cards`, `GET /accounts/:id/loans`
- `POST /transactions`, `POST /transactions/transfer`, `POST /transactions/deposit`, `POST /transactions/withdrawal`
- `POST /loans/apply`, `GET /loans`
- `POST /cards`
- `POST /kyc/:accountId`
- `GET /reports/daily`

### Integración en BDET Bank (plataforma web)

- En **BDET Bank** (`platform/bdet-bank.html`):
  - Menú lateral: ítem **🤖 AI Banker BDET** (Operaciones).
  - Overview: tarjeta “AI Banker BDET” en Servicios en Vivo y en Servicios Integrados.
  - Panel **AI Banker BDET**: estado (cuentas, depósitos, préstamos, transacciones hoy), acciones rápidas (crear cuenta, transferencia, depósito, préstamo, tarjeta, ATABEY), referencia de APIs.
- Estado en tiempo real vía `GET /api/ai-hub/bdet/status` (loadAIBankerStatus).

### Datos

- `node/data/bdet-bank/`: accounts.json, transactions.json, loans.json, cards.json, compliance.json, reports.json.

---

## Otros sistemas AI

Estos están **registrados** bajo ATABEY y pueden recibir tareas; la lógica específica puede estar en otros repos o módulos:

| Sistema | Archivo | Categoría |
|---------|---------|-----------|
| AI Banker (legacy) | `node/ai/ai-banker.js` | Banking |
| AI Trader | `node/ai/ai-trader.js` | Trading |
| AI Orchestrator | `node/ai/ai-orchestrator.js` | Operations |
| AI Master Builder | `node/ai-master-builder.js` | Development |
| AI Replicator | `node/ai-replicator.js` | Infrastructure |
| AI Growth Engine | `node/ai-growth-engine.js` | Business |
| AI Guardian | `platform/ai-guardian.js` | Security |
| Sovereign AI | `node/modules/sovereign-ai.js` | Government |
| AI Code Generator | `node/api/ai-code-generator.js` | Development |
| Learning Engine | `node/ai-hub/learning-engine.js` | Intelligence |

---

## Datos y almacenamiento

```
node/data/
├── ai-hub/
│   ├── projects-registry.json      # Proyectos
│   ├── ai-learnings.json           # Aprendizajes
│   ├── improvements-log.json       # Mejoras
│   ├── collected-data/             # Métricas, errores, performance
│   ├── atabey/                     # ATABEY
│   │   ├── family-members.json
│   │   ├── ai-workers.json
│   │   ├── ai-tasks.json
│   │   ├── production-log.json
│   │   └── master-commands.json
│   └── world-intelligence/
│       ├── market-data.json
│       ├── alerts.json
│       ├── news.json
│       └── predictions.json
└── bdet-bank/
    ├── accounts.json
    ├── transactions.json
    ├── loans.json
    ├── cards.json
    ├── compliance.json
    └── reports.json
```

---

## APIs y accesos

### Base

- Si el front se sirve desde el mismo Node: rutas relativas `/api/ai-hub/...`.
- Origen por defecto: `http://localhost:8545` (configurable vía `ierahkwa_unified_origin`).

### Resumen de rutas AI

| Prefijo | Uso |
|---------|-----|
| `GET /api/ai-hub/health` | Health del AI Hub |
| `GET /api/ai-hub/dashboard` | Dashboard completo (proyectos, health, mejoras, mercado) |
| `GET/POST/PUT /api/ai-hub/projects` | Proyectos |
| `GET /api/ai-hub/health-status` | Estado de servicios |
| `GET/POST /api/ai-hub/improvements`, `POST .../generate`, `POST .../:id/apply` | Mejoras |
| `POST /api/ai-hub/learn` | Ciclo de aprendizaje |
| `POST /api/ai-hub/atabey` | Comando a ATABEY |
| `GET /api/ai-hub/atabey/briefing` | Briefing diario |
| `GET /api/ai-hub/atabey/history` | Historial de conversaciones |
| `GET/POST/PUT /api/ai-hub/family` | Familia |
| `POST /api/ai-hub/atabey/master` | Comando maestro (ej. "producir") |
| `GET /api/ai-hub/atabey/workers` | Lista de AI workers |
| `GET /api/ai-hub/atabey/production` | Estado de producción |
| `POST /api/ai-hub/atabey/production/start` | Iniciar producción |
| `POST /api/ai-hub/atabey/production/stop` | Parar producción |
| `POST /api/ai-hub/atabey/tasks` | Crear tarea para AI |
| `GET /api/ai-hub/market` | Datos de mercado |
| `GET /api/ai-hub/trading-signals` | Señales de trading |
| `GET /api/ai-hub/market-analysis` | Análisis de tendencias |
| `POST/GET /api/ai-hub/alerts` | Alertas de precio |
| `POST /api/ai-hub/market/collect` | Forzar recolección de mercado |
| **Todas bajo `/api/ai-hub/bdet/`** | AI Banker BDET (status, accounts, transactions, loans, cards, kyc, reports) |

### Páginas / dashboards

| Página | Ruta típica |
|--------|--------------|
| AI Hub Dashboard | `/platform/ai-hub-dashboard.html` |
| ATABEY (chat + control maestro) | `/platform/atabey-dashboard.html` |
| BDET Bank (incluye panel AI Banker) | `/platform/bdet-bank.html` → menú “🤖 AI Banker BDET” |
| **Document Flow** (va con la plataforma AI) | `/platform/documents.html` — búsqueda con AI, carpetas, panel asistente; enlazado desde ATABEY y registrado en AI Hub |

---

## Inicio y parada

- **Inicio:** al arrancar el servidor Node se llama `initializeAIHub()`:
  - Registry, proyectos, data collector, world intelligence, ATABEY (sistema + maestro), AI Banker BDET, colección cada 1 min, world intelligence cada 1 min, learning cada 5 min, producción de ATABEY.
- **Parada:** `shutdownAIHub()` detiene automatización del AI Banker BDET, producción de ATABEY, colección, world intelligence y learning.

---

**Documento generado a partir del estado actual de la plataforma AI.**  
Para más detalle por componente: `docs/AI-HUB-DOCUMENTATION.md`.

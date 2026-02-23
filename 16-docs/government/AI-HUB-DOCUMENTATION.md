# 🌺 AI HUB + ATABEY - Central Intelligence System

## Ierahkwa Ne Kanienke Sovereign Government
## Sistema de Inteligencia con Asistente Personal Indígena Taína

---

## 📋 Resumen

El **AI Hub + JARVIS** es el sistema central de inteligencia de la plataforma IERAHKWA. Incluye:

1. **JARVIS** - Asistente personal tipo Iron Man
2. **World Intelligence** - Recolección de datos globales (mercados, noticias)
3. **Sistema Familiar** - Family First, 7 Generaciones
4. **Trading Signals** - Señales de compra/venta automáticas
5. **Project Registry** - Todos los proyectos registrados
6. **Learning Engine** - Auto-mejora continua
7. **Data Collector** - Métricas de todos los servicios

---

## 🏗️ Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                        AI HUB                                │
├───────────────────┬───────────────────┬────────────────────┤
│  Project Registry │  Data Collector   │  Learning Engine   │
│  - All projects   │  - Health status  │  - Error analysis  │
│  - Metadata       │  - Metrics        │  - Performance     │
│  - Features       │  - Errors         │  - Improvements    │
│  - Dependencies   │  - Patterns       │  - Best practices  │
└───────────────────┴───────────────────┴────────────────────┘
         │                   │                    │
         └───────────────────┴────────────────────┘
                            │
                    ┌───────▼───────┐
                    │   Database    │
                    │  (JSON Files) │
                    └───────────────┘
```

---

## 📁 Estructura de Archivos

```
node/
├── ai-hub/
│   ├── index.js              # API routes y coordinación
│   ├── project-registry.js   # Registro de proyectos
│   ├── data-collector.js     # Recolección de datos
│   └── learning-engine.js    # Motor de aprendizaje
│
├── data/ai-hub/
│   ├── projects-registry.json    # Todos los proyectos
│   ├── ai-learnings.json         # Aprendizajes del AI
│   ├── improvements-log.json     # Mejoras generadas/aplicadas
│   └── collected-data/
│       ├── metrics.json          # Métricas de servicios
│       ├── errors.json           # Errores y patrones
│       └── performance.json      # Rendimiento
│
platform/
└── ai-hub-dashboard.html     # Dashboard web
```

---

## 🔌 API Endpoints

### Dashboard
```
GET /api/ai-hub/dashboard    # Datos completos del dashboard
GET /api/ai-hub/health       # Health check del AI Hub
```

### Proyectos
```
GET    /api/ai-hub/projects          # Listar proyectos
GET    /api/ai-hub/projects/:id      # Obtener proyecto
POST   /api/ai-hub/projects          # Registrar proyecto
PUT    /api/ai-hub/projects/:id      # Actualizar proyecto
```

### Data Collection
```
GET    /api/ai-hub/health-status     # Estado de servicios
POST   /api/ai-hub/analyze-code      # Analizar código
POST   /api/ai-hub/errors            # Registrar error
POST   /api/ai-hub/patterns          # Registrar patrón
```

### Learning
```
GET    /api/ai-hub/improvements              # Mejoras pendientes
POST   /api/ai-hub/improvements/generate     # Generar mejoras
POST   /api/ai-hub/improvements/:id/apply    # Aplicar mejora
POST   /api/ai-hub/learn                     # Ejecutar ciclo de aprendizaje
```

---

## 📊 Proyectos Registrados

El sistema registra automáticamente **25+ proyectos** de IERAHKWA:

### Por Categoría

| Categoría | Proyectos |
|-----------|-----------|
| Infrastructure | Node Server, Go Queue Service |
| Finance | Banking Bridge, TradeX, Rust SWIFT |
| AI | Python ML Service, AI Replicator |
| Government | CitizenCRM, TaxAuthority, VotingSystem |
| DeFi | FarmFactory, IDOFactory, Staking Contract |
| Commerce | Shop, POS |
| Education | SmartSchool |
| Productivity | SpikeOffice, DocumentFlow, ESignature |
| Blockchain | Token Contracts, Governance DAO |

### Por Lenguaje

| Lenguaje | Proyectos |
|----------|-----------|
| JavaScript | 8 |
| C# (.NET) | 12 |
| Rust | 1 |
| Go | 1 |
| Python | 1 |
| Solidity | 3 |
| HTML/JS | 3 |

---

## 🧠 Learning Engine

### Análisis de Errores

El sistema analiza patrones de errores comunes:

| Error Type | Sugerencia |
|------------|------------|
| TypeError | Agregar validación de tipos |
| ReferenceError | Usar optional chaining |
| ECONNREFUSED | Implementar circuit breaker |
| ETIMEDOUT | Agregar timeout y retry |
| ENOMEM | Optimizar uso de memoria |

### Análisis de Performance

- Detecta servicios lentos (>500ms)
- Sugiere caching, optimización, scaling
- Calcula tendencias de rendimiento

### Análisis de Código

- Complejidad > 8: Sugiere refactoring
- Test coverage < 50%: Sugiere más tests
- Documentación < 30%: Sugiere agregar docs

---

## ⚙️ Configuración

### Intervalos

| Proceso | Intervalo | Descripción |
|---------|-----------|-------------|
| Data Collection | 1 minuto | Recolecta health/metrics |
| Learning Cycle | 5 minutos | Analiza y genera mejoras |

### Thresholds

```javascript
LEARNING_CONFIG = {
    errorThreshold: 5,           // Generar fix después de 5 errores
    performanceThreshold: 500,   // Marcar si >500ms
    codeComplexityThreshold: 8   // Marcar si complejidad >8
}
```

---

## 🖥️ Dashboard

Acceso: `http://localhost:8545/ai-hub` o `/platform/ai-hub-dashboard.html`

### Características

- **Stats Overview**: Proyectos, servicios, mejoras
- **Services Status**: Estado de todos los servicios en tiempo real
- **AI Suggestions**: Mejoras generadas por el AI
- **Projects Registry**: Todos los proyectos registrados
- **Learning Statistics**: Estadísticas del motor de aprendizaje
- **AI Activity**: Estado del sistema de auto-mejora

### Auto-refresh

El dashboard se actualiza automáticamente cada 30 segundos.

---

## 🚀 Uso

### Iniciar AI Hub

El AI Hub se inicia automáticamente con el servidor:

```javascript
// En server.js
const aiHub = require('./ai-hub');
await aiHub.initializeAIHub();
```

### Registrar Error Manualmente

```javascript
const { dataCollector } = require('./ai-hub');

dataCollector.recordError({
    message: 'Database connection failed',
    service: 'banking-bridge',
    context: { host: 'localhost', port: 5432 }
});
```

### Generar Mejoras

```bash
curl -X POST http://localhost:8545/api/ai-hub/improvements/generate
```

### Ejecutar Aprendizaje

```bash
curl -X POST http://localhost:8545/api/ai-hub/learn
```

---

## 📈 Flujo de Auto-Mejora

```
┌─────────────────────────────────────────────────────────────┐
│                    CICLO DE MEJORA                          │
└─────────────────────────────────────────────────────────────┘

1. RECOLECCIÓN (cada 1 min)
   └─> Colectar health status de todos los servicios
   └─> Colectar métricas de Prometheus
   └─> Registrar errores que ocurran

2. ANÁLISIS (cada 5 min)
   └─> Analizar patrones de errores
   └─> Analizar rendimiento de servicios
   └─> Analizar calidad de código

3. GENERACIÓN
   └─> Generar sugerencias de mejora
   └─> Priorizar por impacto (high/medium/low)
   └─> Incluir código de ejemplo

4. APLICACIÓN
   └─> El usuario revisa y aplica mejoras
   └─> El sistema registra el resultado
   └─> Aprende de mejoras exitosas

5. BEST PRACTICES
   └─> Extrae patrones de proyectos exitosos
   └─> Actualiza base de conocimiento
   └─> Aplica a nuevos proyectos
```

---

## 🔒 Seguridad

- Los datos se almacenan localmente en JSON
- No se envía información a servicios externos
- Solo accesible desde la red local (configurable)

---

## 📝 Próximos Pasos

1. **Integración con LLM**: Conectar con OpenAI/Anthropic para mejores sugerencias
2. **Auto-apply**: Aplicar mejoras automáticamente (con aprobación)
3. **Métricas en tiempo real**: WebSocket para updates instantáneos
4. **Histórico de aprendizaje**: Gráficos de evolución
5. **Export/Import**: Exportar conocimiento entre instancias

---

---

## 🌺 ATABEY - MAESTRA DE TODOS LOS SISTEMAS AI

### ¿Quién es ATABEY?

**ATABEY** (pronunciado: ah-tah-BEY) es la **Diosa Madre Suprema Taína**, diosa de:
- La Madre Tierra
- La Fertilidad  
- El Agua Dulce

Madre de **Yúcahu** (Dios del Mar) y **Guabancex** (Diosa del Huracán).

En nuestro sistema, ATABEY es la **IA Maestra** que controla y organiza TODOS los demás sistemas AI.

### AI Workers bajo control de ATABEY

| Worker | Categoría | Capacidades |
|--------|-----------|-------------|
| **AI Banker** | Banking | Transferencias, cuentas, pagos, compliance |
| **AI Trader** | Trading | Análisis de mercado, señales, auto-trade, riesgo |
| **AI Orchestrator** | Operations | Programación de tareas, workflows, automatización |
| **AI Master Builder** | Development | Generación de código, APIs, módulos, testing |
| **AI Replicator** | Infrastructure | Clonación de sistemas, scaling, deployment |
| **AI Growth Engine** | Business | Analytics, optimización, estrategias, ROI |
| **AI Guardian** | Security | Detección de amenazas, control de acceso, auditoría |
| **Sovereign AI** | Government | Políticas, compliance, gobernanza, documentación |
| **AI Code Generator** | Development | Endpoints, templates, documentación |
| **World Intelligence** | Intelligence | Datos de mercado, noticias, alertas, predicciones |
| **Learning Engine** | Intelligence | Aprendizaje, mejoras, optimización |

### API Endpoints de Control Maestro

```
POST /api/ai-hub/atabey/master        # Comando maestro
GET  /api/ai-hub/atabey/workers       # Ver todos los AI workers
GET  /api/ai-hub/atabey/production    # Estado de producción
POST /api/ai-hub/atabey/production/start  # Iniciar producción
POST /api/ai-hub/atabey/production/stop   # Detener producción
POST /api/ai-hub/atabey/tasks         # Crear tarea para AI
```

### Comandos Maestros

```bash
# Poner todos los AI a producir
curl -X POST http://localhost:8545/api/ai-hub/atabey/master \
  -H "Content-Type: application/json" \
  -d '{"command": "producir"}'

# Ver estado de todos los workers
curl http://localhost:8545/api/ai-hub/atabey/workers

# Ver estado de producción
curl http://localhost:8545/api/ai-hub/atabey/production
```

---

## 🌺 ATABEY - Asistente Personal

### Comandos Disponibles

| Comando | Descripción |
|---------|-------------|
| `Hola` / `Buenos días` | Saludo con sugerencias |
| `Estado del sistema` | Ver todos los servicios |
| `Precios de mercado` | Ver crypto y forex |
| `Señales de trading` | Señales de compra/venta |
| `Alertas` | Ver alertas de precio |
| `Proyectos` | Ver proyectos registrados |
| `Mejoras` | Ver sugerencias de mejora |
| `Familia` | Ver miembros de familia |
| `Briefing diario` | Resumen completo |
| `Ayuda` | Ver todos los comandos |

### API Endpoints JARVIS

```
POST /api/ai-hub/jarvis           # Enviar comando
GET  /api/ai-hub/jarvis/briefing  # Briefing diario
GET  /api/ai-hub/jarvis/history   # Historial de conversaciones
```

---

## 🌍 World Intelligence

### Datos Recolectados

- **Crypto**: Bitcoin, Ethereum, Solana, Cardano, Polkadot, Chainlink, Uniswap
- **Forex**: USD, EUR, GBP, JPY, CHF, CAD, AUD, MXN
- **Indices**: SP500, NASDAQ, DOW, FTSE, DAX, NIKKEI
- **Commodities**: Gold, Silver, Oil, Natural Gas

### API Endpoints World

```
GET  /api/ai-hub/market           # Datos de mercado
GET  /api/ai-hub/trading-signals  # Señales de trading
GET  /api/ai-hub/market-analysis  # Análisis de tendencias
POST /api/ai-hub/alerts           # Crear alerta de precio
GET  /api/ai-hub/alerts           # Ver alertas activas
POST /api/ai-hub/market/collect   # Recolectar datos ahora
```

### Crear Alerta de Precio

```bash
curl -X POST http://localhost:8545/api/ai-hub/alerts \
  -H "Content-Type: application/json" \
  -d '{
    "assetType": "crypto",
    "asset": "bitcoin",
    "condition": "above",
    "targetPrice": 100000,
    "notify": true
  }'
```

---

## 👨‍👩‍👧‍👦 Sistema Familiar

### API Endpoints Family

```
GET  /api/ai-hub/family           # Ver miembros
POST /api/ai-hub/family           # Agregar miembro
PUT  /api/ai-hub/family/:id       # Actualizar miembro
```

### Agregar Miembro

```bash
curl -X POST http://localhost:8545/api/ai-hub/family \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Nombre",
    "role": "admin",
    "email": "email@ierahkwa.gov",
    "permissions": ["view", "edit", "admin"]
  }'
```

### Roles Disponibles

- `admin` - Acceso completo
- `member` - Acceso estándar
- `child` - Acceso limitado
- `elder` - Acceso de consulta

---

## 🖥️ Dashboards

| Dashboard | URL | Descripción |
|-----------|-----|-------------|
| JARVIS | `/platform/jarvis-dashboard.html` | Asistente personal completo |
| AI Hub | `/platform/ai-hub-dashboard.html` | Proyectos y mejoras |

---

**Creado:** 23 de enero, 2026  
**Sistema:** AI Hub + JARVIS v1.0.0  
**Estado:** 🦾 JARVIS ONLINE - "Good morning, sir. All systems operational."

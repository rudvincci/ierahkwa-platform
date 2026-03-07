# Forex Trading Server

> Servidor de trading en tiempo real con Forex, indices, commodities y economia soberana (WAMPUM, IGT, BDET, SNT-574) via Socket.IO.

## Descripcion

El Forex Trading Server es el motor de mercados financieros de la plataforma Ierahkwa Futurehead. Simula datos de mercado en tiempo real para 14 pares Forex, 5 indices bursatiles, 3 commodities y 26 tokens de la economia soberana, incluyendo WAMPUM (WMP), IGT, BDET y 19 tokens SNT de naciones indigenas. Los precios se actualizan cada 100ms y se transmiten via Socket.IO a todos los clientes suscritos.

El servidor incluye un sistema completo de planes de inversion con 5 niveles (Starter, Growth, Premium, VIP Elite, Institutional), proveedores de senales de trading con metricas de rendimiento, y un motor de trading soberano que permite operar pares SNT/WMP con liquidacion en la blockchain MameyNode (Chain ID 574). Las comisiones de trading se dividen: 30% para la plataforma y 70% para la tesoreria de la nacion correspondiente.

Adicionalmente, el sistema genera los 574 tokens SNT que representan a todas las naciones tribales, con un indice compuesto SNT-574 (SNTIDXWMP). Todas las operaciones de trading soberano generan hashes de transaccion simulados de la blockchain MameyNode.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 5.x
- **WebSocket**: Socket.IO 4.x
- **Cache**: node-cache (mercado 1s TTL, senales 300s TTL)
- **Autenticacion**: jsonwebtoken, bcryptjs
- **Base de Datos**: MongoDB (mongoose 8.x), Redis 4.x
- **Logging**: Winston 3.x
- **Rate Limiting**: rate-limiter-flexible
- **Modulos**: ES Modules (type: module)
- **Puerto**: 3500

## API Endpoints

### Mercado

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check |
| GET | /api/instruments | Todos los instrumentos con precios actuales |
| GET | /api/instruments/sovereign | Solo instrumentos soberanos (WMP, IGT, BDET, SNT) |
| GET | /api/market/:symbol | Datos de mercado de un simbolo |

### Planes de Inversion

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /api/plans | Todos los planes de inversion |
| GET | /api/plans/trending | Planes en tendencia |
| GET | /api/plans/:id | Plan por ID |
| POST | /api/calculate | Calcular ROI de inversion |

### Senales de Trading

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /api/signals/providers | Lista de proveedores de senales |
| GET | /api/signals/providers/:id | Proveedor por ID |
| GET | /api/signals/providers/:id/signals | Senales activas del proveedor |

### Trading Soberano

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/sovereign/trade | Ejecutar trade soberano (WMP/IGT/BDET/SNT) |
| GET | /api/sovereign/nations | Listar 574 naciones con tokens SNT (paginado) |
| GET | /api/analytics/platform | Analiticas de la plataforma |

### Socket.IO Events

| Evento | Direccion | Descripcion |
|--------|-----------|-------------|
| market:init | Server → Client | Datos iniciales de todos los instrumentos |
| market:subscribe | Client → Server | Suscribirse a actualizaciones de mercado |
| market:update | Server → Client | Actualizacion de precios (cada 100ms) |
| signals:subscribe | Client → Server | Suscribirse a senales de un proveedor |
| signal:create | Client → Server | Crear nueva senal (proveedores) |
| signal:new | Server → Client | Nueva senal creada |
| signal:close | Client → Server | Cerrar senal |
| signal:closed | Server → Client | Senal cerrada |
| investment:create | Client → Server | Crear inversion |
| investment:created | Server → Client | Inversion creada |
| sovereign:trade | Server → Client | Trade soberano ejecutado |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3500 |
| CORS_ORIGINS | Origenes permitidos (coma-separados) | http://localhost:3000 |
| MONGODB_URI | URI de conexion MongoDB | mongodb://localhost/forex |
| REDIS_URL | URL de conexion Redis | redis://localhost:6379 |
| JWT_SECRET | Clave secreta JWT | your-secret |

## Instalacion

```bash
npm install
npm start
```

### Desarrollo

```bash
npm run dev   # --watch mode
```

## Docker

```bash
docker build -t forex-trading-server .
docker run -p 3500:3500 \
  -e CORS_ORIGINS=https://ierahkwa.gov \
  forex-trading-server
```

## Arquitectura

```
Clientes REST ──→ Express API
                    ├── /api/instruments ──→ Precios en tiempo real
                    ├── /api/plans ──→ Planes de inversion
                    ├── /api/signals ──→ Proveedores y senales
                    └── /api/sovereign ──→ Trading soberano MameyNode

Clientes WS ──→ Socket.IO
                    ├── market:update (100ms) ──→ 48+ instrumentos
                    ├── signal:* ──→ Senales de trading
                    └── sovereign:trade ──→ Trades en blockchain

Motor de Precios:
  FOREX (14) + Indices (5) + Commodities (3) = 22 instrumentos globales
  WAMPUM (3) + IGT (2) + BDET (2) + SNT (19) + Index (1) = 27 soberanos
  Total: 49 instrumentos con actualizacion cada 100ms

Economia Soberana:
  MameyNode (Chain ID 574) → WAMPUM (WMP) → BDET Bank
  574 naciones → 574 tokens SNT → Indice SNTIDXWMP
  Comisiones: 30% plataforma + 70% tesoreria de nacion
```

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital

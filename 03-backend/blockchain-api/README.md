# Blockchain API

> API REST y WebSocket para la blockchain soberana MameyNode v4.2 con motor de gobernanza on-chain.

## Descripcion

Blockchain API es el servicio central de la cadena de bloques soberana MameyNode v4.2. Proporciona una API REST completa para operaciones de blockchain: gestion de bloques, transacciones, wallets, tokens (IGT, WAMPUM, SNT), validadores y nodos. Ademas incluye un motor de gobernanza on-chain con propuestas, votaciones, conteo y ejecucion automatica.

El servicio implementa conexiones WebSocket para transmision en tiempo real de nuevos bloques y transacciones pendientes, permitiendo que los clientes se suscriban a canales especificos. En modo desarrollo, incluye auto-mining que mina bloques automaticamente cuando hay transacciones pendientes.

La documentacion de la API esta disponible via Swagger en `/api-docs`. El servicio integra las utilidades compartidas de seguridad de Ierahkwa incluyendo CORS, rate limiting y sanitizacion de entradas.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **WebSocket**: ws 8.x (nativo)
- **Blockchain**: MameyNode v4.2 (motor propio)
- **Gobernanza**: Motor de propuestas/votaciones on-chain
- **Documentacion**: Swagger/OpenAPI
- **Puerto**: 3000

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con altura de cadena y wallets |
| GET | /ready | Readiness check de blockchain, gobernanza y WebSocket |
| GET | /api-docs | Documentacion Swagger |
| GET | /v1/blocks | Listar bloques recientes (limit max 100) |
| GET | /v1/blocks/:ref | Obtener bloque por altura o hash |
| GET | /v1/transactions/pending | Transacciones pendientes |
| GET | /v1/transactions/:hash | Obtener transaccion por hash |
| POST | /v1/transactions | Crear transaccion (from, to, amount) |
| GET | /v1/wallets | Listar wallets (paginado) |
| POST | /v1/wallets | Crear wallet nueva |
| GET | /v1/wallets/:address | Obtener wallet por direccion |
| GET | /v1/wallets/:address/history | Historial de transacciones del wallet |
| GET | /v1/tokens | Listar tokens registrados |
| GET | /v1/tokens/:symbol | Obtener token por simbolo |
| POST | /v1/tokens | Registrar nuevo token |
| GET | /v1/validators | Listar validadores |
| POST | /v1/validators | Registrar validador (address, stake) |
| GET | /v1/governance/proposals | Listar propuestas (filtro por estado) |
| GET | /v1/governance/proposals/:id | Detalle de propuesta |
| POST | /v1/governance/proposals | Crear propuesta |
| POST | /v1/governance/proposals/:id/vote | Votar en propuesta |
| POST | /v1/governance/proposals/:id/tally | Contar votos |
| POST | /v1/governance/proposals/:id/execute | Ejecutar propuesta aprobada |
| GET | /v1/governance/proposals/:id/votes | Listar votos de propuesta |
| GET | /v1/nodes/stats | Estadisticas de red |
| POST | /v1/mine | Minar bloque (desarrollo/testing) |

### WebSocket (ws://localhost:3000/ws)

| Evento | Direccion | Descripcion |
|--------|-----------|-------------|
| sync | Server → Client | Estado inicial de la cadena al conectar |
| subscribe | Client → Server | Suscribirse a canales (blocks, transactions) |
| new_block | Server → Client | Nuevo bloque minado |
| pending_tx | Server → Client | Nueva transaccion pendiente |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3000 |
| NODE_ENV | Entorno (development activa auto-mining) | production |
| MAMEYNODE_RPC | URL del nodo RPC MameyNode | http://localhost:8545 |
| CORS_ORIGINS | Origenes CORS permitidos | http://localhost:3000 |

## Instalacion

```bash
npm install
npm start
```

### Desarrollo

```bash
npm run dev   # --watch mode con auto-mining
npm test      # Jest
```

## Docker

```bash
docker build -t blockchain-api .
docker run -p 3000:3000 \
  -e MAMEYNODE_RPC=http://mameynode:8545 \
  -e NODE_ENV=production \
  blockchain-api
```

## Arquitectura

```
Clientes REST ──→ Express API ──→ Blockchain Engine (MameyNode v4.2)
                                    ├── Chain (bloques, transacciones)
                                    ├── Wallets (WAMPUM, tokens)
                                    ├── Tokens (IGT, SNT, BDET)
                                    ├── Validators (PoS)
                                    └── Events (EventEmitter)

Clientes WS ───→ WebSocket Server ──→ Broadcast (new_block, pending_tx)

Motor de Gobernanza ──→ Propuestas → Votaciones → Conteo → Ejecucion
```

En modo desarrollo, un auto-miner verifica periodicamente si hay transacciones pendientes y las mina automaticamente en bloques nuevos.

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital

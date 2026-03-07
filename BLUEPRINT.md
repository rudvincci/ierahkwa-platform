# BLUEPRINT TECNICO: Ierahkwa Ne Kanienke — Nacion Digital Soberana

**Version:** 5.5.0
**Fecha:** Marzo 2026
**Tipo:** Documento tecnico de arquitectura e implementacion

---

## Tabla de Contenidos

1. [Diagrama de Arquitectura General](#1-diagrama-de-arquitectura-general)
2. [Stack Tecnologico Completo](#2-stack-tecnologico-completo)
3. [Mapa de Servicios](#3-mapa-de-servicios)
4. [Base de Datos](#4-base-de-datos)
5. [Mapa de API](#5-mapa-de-api)
6. [Flujo de Datos](#6-flujo-de-datos)
7. [Red de Comunicacion](#7-red-de-comunicacion)
8. [Seguridad — Capas de Defensa](#8-seguridad--capas-de-defensa)
9. [Monitoreo](#9-monitoreo)
10. [Despliegue — CI/CD Pipeline](#10-despliegue--cicd-pipeline)
11. [Blockchain — MameyNode](#11-blockchain--mameynode)
12. [Diagrama de Red](#12-diagrama-de-red)

---

## 1. Diagrama de Arquitectura General

```
                            ┌─────────────────────────────────────────────────────┐
                            │                    INTERNET                         │
                            │   Usuarios (1B+ personas, 35+ paises, 43 idiomas)  │
                            └──────────────────────┬──────────────────────────────┘
                                                   │
                                        ┌──────────▼──────────┐
                                        │   Tor Hidden Svc    │
                                        │   (.onion)          │
                                        └──────────┬──────────┘
                                                   │
                            ┌──────────────────────▼──────────────────────┐
                            │           NGINX REVERSE PROXY               │
                            │         Puerto 80 (HTTP) / 443 (HTTPS)     │
                            │   SSL/TLS Termination | Rate Limiting      │
                            │   gzip | Cache | Security Headers          │
                            └────┬────────────┬────────────┬─────────────┘
                                 │            │            │
            ┌────────────────────┘            │            └────────────────────┐
            │                                 │                                │
┌───────────▼───────────┐    ┌───────────────▼───────────────┐   ┌────────────▼────────────┐
│   PLATAFORMAS HTML    │    │      SERVICIOS NODE.JS        │   │   SERVICIOS .NET        │
│   422+ Plataformas    │    │                               │   │                         │
│   18 NEXUS Portales   │    │  Sovereign Core    :3050      │   │  Identity     :5001     │
│                       │    │  BDET Bank         :3001      │   │  ZKP          :5002     │
│  ┌─────────────────┐  │    │  Voz Soberana      :3002      │   │  Treasury     :5003     │
│  │ ierahkwa.css    │  │    │  Red Social        :3003      │   │  76+ Micros   :5010+    │
│  │ ierahkwa.js     │  │    │  Correo Soberano   :3004      │   │                         │
│  │ ierahkwa-agents │  │    │  Reservas          :3005      │   │  Mamey Framework (139)  │
│  │ sw.js           │  │    │  Voto Soberano     :3006      │   │  Pupitre (28 svcs)      │
│  └─────────────────┘  │    │  Trading           :3007      │   │  INKG Bank              │
│                       │    │  POS System        :3030      │   │  MameyNode.UI (Blazor)  │
│  7 AI Agents (client) │    │  Conferencia       :3090      │   │                         │
│                       │    │  Vigilancia        :3091      │   └─────────────┬───────────┘
└───────────────────────┘    │  Empresa           :3092      │                 │
                             │  Ierahkwa Shop     :3100      │                 │
                             │  Inventory         :3200      │                 │
                             │  Image Upload      :3300      │   ┌─────────────▼───────────┐
                             │  Forex Trading     :3400      │   │  SERVICIOS RUST/GO      │
                             │  Smart School      :3500      │   │                         │
                             └──────────────┬────────────────┘   │  MameyForge CLI  (Rust) │
                                            │                    │  gRPC SDK        (Rust) │
                                            │                    │  Financiero     :8590   │
                             ┌──────────────▼────────────────┐   │  Colas Go       :8591   │
                             │       CAPA DE DATOS           │   │  Python ML      :8592   │
                             │                               │   └─────────────────────────┘
                             │  PostgreSQL 16     :5432      │
                             │  Redis 7           :6379      │
                             │  RabbitMQ          :15692     │
                             │  Elasticsearch     :9200      │
                             └──────────────┬────────────────┘
                                            │
                             ┌──────────────▼────────────────┐
                             │      MAMEYNODE BLOCKCHAIN     │
                             │                               │
                             │  RPC               :8545      │
                             │  Chain ID:          777777    │
                             │  Consenso:          PoSov     │
                             │  TPS:               12,847    │
                             │  Smart Contracts (Solidity)   │
                             │  103 IGT + 574 SNT tokens    │
                             └──────────────┬────────────────┘
                                            │
                             ┌──────────────▼────────────────┐
                             │        MONITOREO              │
                             │                               │
                             │  Prometheus         :9090     │
                             │  Grafana            :3000     │
                             │  Node Exporter      :9100     │
                             │  Blackbox Exporter  :9115     │
                             │  Alertmanager       :9093     │
                             └───────────────────────────────┘
```

---

## 2. Stack Tecnologico Completo

### 2.1 Lenguajes de Programacion

| Lenguaje | Version | Uso | Lineas Estimadas |
|----------|---------|-----|-----------------|
| HTML/CSS/JS | ES2024 | 422+ plataformas frontend, 7 AI agents | ~800K |
| C# (.NET) | 9.0 | 139 framework + 76+ microservicios | ~500K |
| Node.js | 22 LTS | 20 servicios backend + Sovereign Core | ~300K |
| Rust | stable | MameyForge CLI, gRPC SDK, servicio financiero | ~100K |
| Go | 1.22+ | Microservicios, SDKs, colas | ~80K |
| Solidity | 0.8.x | Smart contracts MameyNode | ~50K |
| Python | 3.12+ | ML/AI backends, scripts, Atabey translator | ~80K |
| SQL | PostgreSQL 16 | Esquemas, migraciones, procedimientos | ~20K |
| Bash | 5.x | Scripts de despliegue, operaciones | ~15K |
| YAML | - | Docker Compose, K8s, CI/CD, Prometheus | ~30K |
| HCL | - | Terraform IaC | ~10K |
| **Total** | - | **11+ lenguajes** | **~2M lineas** |

### 2.2 Frameworks y Librerias Principales

| Categoria | Tecnologia | Proposito |
|-----------|-----------|-----------|
| Backend Node.js | Express.js | Framework HTTP para 20 servicios |
| Backend .NET | ASP.NET Core 9 | Microservicios .NET |
| Frontend .NET | Blazor WebAssembly | MameyNode.UI, Pupitre |
| Autenticacion | JWT (HS256) | Tokens de acceso y refresco |
| Encriptacion | bcrypt (12 rounds) | Hash de passwords |
| Base de datos | node-postgres (pg) | Driver PostgreSQL |
| Cache | ioredis | Cliente Redis |
| WebSocket | ws | Mensajeria en tiempo real |
| Testing | Jest | Tests unitarios e integracion |
| Blockchain | ethers.js / web3.js | Interaccion con MameyNode |
| Rust gRPC | tonic | SDK blockchain de alto rendimiento |
| CLI | clap (Rust) | MameyForge CLI |
| Containerizacion | Docker + Compose | Orquestacion de contenedores |
| Orquestacion | Kubernetes | Produccion en nube |
| IaC | Terraform | Infraestructura AWS |
| Proxy | Nginx | Proxy inverso + SSL |
| Metricas | prom-client | Exportacion de metricas Prometheus |
| CI/CD | GitHub Actions | 9 workflows agenticos |

### 2.3 Criptografia

| Algoritmo | Estandar | Uso |
|-----------|----------|-----|
| ML-DSA-65 | NIST FIPS 204 | Firmas digitales post-cuanticas |
| ML-KEM-1024 | NIST FIPS 203 | Encapsulacion de claves post-cuantica |
| CRYSTALS-Kyber | NIST | Archivo eterno soberano |
| ZK-SNARKs | - | Identidad zero-knowledge |
| Groth16 | - | Proofs compactas ZK |
| BBS+ | W3C | Credenciales verificables selectivas |
| bcrypt | OpenBSD | Hash de contrasenas (12 rounds) |
| AES-256 | NIST | Encriptacion de backups |
| pgcrypto | PostgreSQL | Encriptacion a nivel de columna |

---

## 3. Mapa de Servicios

### 3.1 Servicios de Infraestructura

```
Puerto    Servicio              Tipo          Imagen Docker
──────    ────────              ────          ─────────────
80/443    Nginx Proxy           Proxy         nginx:alpine
5432      PostgreSQL 16         Base Datos    postgres:16-alpine
6379      Redis 7               Cache         redis:7-alpine
9090      Prometheus            Monitoreo     prom/prometheus
3000      Grafana               Dashboards    grafana/grafana
9100      Node Exporter         Metricas SO   prom/node-exporter
9115      Blackbox Exporter     Sondas HTTP   prom/blackbox-exporter
9187      Postgres Exporter     Metricas DB   prometheuscommunity/postgres-exporter
9121      Redis Exporter        Metricas      oliver006/redis_exporter
9113      Nginx Exporter        Metricas      nginx/nginx-prometheus-exporter
15692     RabbitMQ              Cola Msgs     rabbitmq:management-alpine
9200      Elasticsearch         Busqueda      elasticsearch:8
```

### 3.2 Servicios Node.js (Puertos 3001-3500)

```
Puerto    Servicio              Modulos/Descripcion
──────    ────────              ───────────────────
3001      BDET Bank             Banca: cuentas, transferencias, pagos, prestamos
3002      Voz Soberana          Microblogging: posts, follows, timeline, trending
3003      Red Social            Red social: perfiles, amigos, grupos, eventos
3004      Correo Soberano       Email: SMTP encriptado, bandeja, adjuntos
3005      Reservas              Booking: hoteles, tours, actividades, calendario
3006      Voto Soberano         Votacion: propuestas, ballots, resultados blockchain
3007      Trading               Exchange: ordenes, libro, pares, trades
3030      POS System            Punto de venta: transacciones, recibos, inventario
3050      Sovereign Core        Backend unificado: 18 modulos (ver seccion API)
3090      Conferencia Soberana  Video: salas, screenshare, chat, grabacion
3091      Vigilancia Soberana   SIEM: logs, alertas, incidentes, correlacion
3092      Empresa Soberana      ERP: contabilidad, RRHH, CRM, facturacion
3100      Ierahkwa Shop         E-commerce: productos, carrito, checkout, envios
3200      Inventory System      Inventario: stock, almacenes, ordenes compra
3300      Image Upload          Media: subida, redimensionamiento, CDN, EXIF
3400      Forex Trading         Forex: pares divisas, ordenes, graficos, noticias
3500      Smart School          Educacion: cursos, lecciones, examenes, calificaciones
```

### 3.3 Servicios .NET (Puertos 5001+)

```
Puerto    Servicio              Descripcion
──────    ────────              ───────────
5001      Identity Service      Identidad auto-soberana: FWID, Face, DID/SSI
5002      ZKP Service           Zero-knowledge proofs: ZK-SNARKs, Groth16, BBS+
5003      Treasury Service      Tesoreria WMP: reservas, emisiones, quema
5054      TradeX                Trading avanzado .NET
5055      RNB-Cal               Calculadora financiera
5056      Spike Office          Suite ofimática
5060      App Builder           Constructor de apps low-code
5061      Farm Factory          Agricultura digital
5071      NET10                 Red de comunicaciones
5080      Document Flow         Flujo de documentos
5081      eSignature            Firmas electronicas
5090      Citizen CRM           CRM ciudadano
5091      Tax Authority         Autoridad fiscal (0% impuestos, solo comisiones)
5092      Voting System         Sistema de votacion .NET
5093      Service Desk          Mesa de servicios
5097      IDO Factory           Ofertas descentralizadas
7070      Project Hub           Gestion de proyectos
7071      Meeting Hub           Reuniones y agenda
```

### 3.4 Servicios Multi-Lenguaje

```
Puerto    Servicio              Lenguaje    Descripcion
──────    ────────              ────────    ───────────
8545      MameyNode             Solidity    Blockchain soberana (RPC)
8590      Rust Financial        Rust        Procesamiento financiero ultra-rapido
8591      Go Queue Service      Go          Gestion de colas de mensajes
8592      Python ML Service     Python      Machine learning y IA
```

---

## 4. Base de Datos

### 4.1 Esquema Principal — PostgreSQL 16

```
┌─────────────────────────────────────────────────────────┐
│                    IERAHKWA DB SCHEMA                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌───────────────┐         ┌──────────────────┐        │
│  │    users       │────────>│  refresh_tokens  │        │
│  │───────────────│         │──────────────────│        │
│  │ id (UUID PK)  │         │ id (UUID PK)     │        │
│  │ email (UNIQUE)│         │ user_id (FK)     │        │
│  │ full_name     │         │ token (UNIQUE)   │        │
│  │ password_hash │         │ expires_at       │        │
│  │ roles[]       │         │ is_revoked       │        │
│  │ permissions[] │         └──────────────────┘        │
│  │ is_active     │                                      │
│  │ is_verified   │         ┌──────────────────┐        │
│  │ last_login_at │────────>│  user_sessions   │        │
│  │ locked_until  │         │──────────────────│        │
│  └───────┬───────┘         │ id (UUID PK)     │        │
│          │                 │ user_id (FK)     │        │
│          │                 │ ip_address       │        │
│          │                 │ expires_at       │        │
│          │                 └──────────────────┘        │
│          │                                              │
│          │    ┌──────────────┐    ┌──────────────┐     │
│          ├───>│  accounts    │───>│ transactions │     │
│          │    │──────────────│    │──────────────│     │
│          │    │ id (UUID PK) │    │ id (UUID PK) │     │
│          │    │ user_id (FK) │    │ from_acct(FK)│     │
│          │    │ bank_id (FK) │    │ to_acct (FK) │     │
│          │    │ acct_number  │    │ amount       │     │
│          │    │ balance      │    │ fee          │     │
│          │    │ currency     │    │ status       │     │
│          │    │ credit_limit │    │ metadata     │     │
│          │    └──────┬───────┘    └──────┬───────┘     │
│          │           │                   │              │
│          │    ┌──────▼───────┐    ┌──────▼───────────┐ │
│          │    │   banks      │    │ transaction_docs │ │
│          │    │──────────────│    │──────────────────│ │
│          │    │ id (UUID PK) │    │ id (UUID PK)     │ │
│          │    │ code (UNIQUE)│    │ transaction_id   │ │
│          │    │ name         │    │ document_type    │ │
│          │    │ swift_code   │    │ file_path        │ │
│          │    │ is_central   │    │ checksum (SHA256)│ │
│          │    └──────────────┘    └──────────────────┘ │
│          │                                              │
│          │    ┌───────────────┐   ┌──────────────────┐ │
│          ├───>│ trading_pairs │──>│    orders         │ │
│          │    │───────────────│   │──────────────────│ │
│          │    │ id (UUID PK)  │   │ id (UUID PK)     │ │
│          │    │ base_currency │   │ user_id (FK)     │ │
│          │    │ quote_currency│   │ pair_id (FK)     │ │
│          │    │ bid_price     │   │ side (buy/sell)  │ │
│          │    │ ask_price     │   │ price            │ │
│          │    │ volume_24h    │   │ quantity         │ │
│          │    └───────────────┘   │ status           │ │
│          │                       └─────────┬────────┘ │
│          │                                 │          │
│          │                       ┌─────────▼────────┐ │
│          │                       │    trades         │ │
│          │                       │──────────────────│ │
│          │                       │ id (UUID PK)     │ │
│          │                       │ order_id (FK)    │ │
│          │                       │ buyer_id (FK)    │ │
│          │                       │ seller_id (FK)   │ │
│          │                       │ price            │ │
│          │                       │ fee              │ │
│          │                       └──────────────────┘ │
│          │                                              │
│          │    ┌─────────────────┐  ┌─────────────────┐ │
│          ├───>│ biometric_data  │─>│ biometric_logs  │ │
│          │    │─────────────────│  │─────────────────│ │
│          │    │ id (UUID PK)    │  │ id (UUID PK)    │ │
│          │    │ user_id (FK)    │  │ biometric_id(FK)│ │
│          │    │ type            │  │ action          │ │
│          │    │ template_hash   │  │ success         │ │
│          │    └─────────────────┘  │ device_info     │ │
│          │                        └─────────────────┘ │
│          │                                              │
│          │    ┌─────────────────┐                       │
│          └───>│   audit_logs    │                       │
│               │─────────────────│                       │
│               │ id (UUID PK)    │                       │
│               │ user_id (FK)    │                       │
│               │ action          │                       │
│               │ entity_type     │                       │
│               │ old_values      │                       │
│               │ new_values      │                       │
│               │ ip_address      │                       │
│               └─────────────────┘                       │
│                                                         │
│  Extensiones: uuid-ossp, pgcrypto                      │
│  Indices: 13 indices optimizados                       │
│  Seed Data: 5 bancos centrales + admin user            │
└─────────────────────────────────────────────────────────┘
```

### 4.2 Indices de Rendimiento

```sql
-- 13 indices para consultas de alto rendimiento
idx_users_email          ON users(email)
idx_accounts_user        ON accounts(user_id)
idx_accounts_bank        ON accounts(bank_id)
idx_accounts_number      ON accounts(account_number)
idx_transactions_code    ON transactions(transaction_code)
idx_transactions_from    ON transactions(from_account_id)
idx_transactions_to      ON transactions(to_account_id)
idx_transactions_status  ON transactions(status)
idx_transactions_date    ON transactions(created_at DESC)
idx_orders_user          ON orders(user_id)
idx_orders_status        ON orders(status)
idx_trades_pair          ON trades(pair_id)
idx_audit_user           ON audit_logs(user_id)
idx_audit_entity         ON audit_logs(entity_type, entity_id)
```

### 4.3 Pool de Conexiones

```
Configuracion del pool (sovereign-core/config.js):
  DB_POOL_MIN:              2
  DB_POOL_MAX:              20
  DB_IDLE_TIMEOUT:          30,000ms
  DB_CONNECTION_TIMEOUT:    5,000ms
  DB_STATEMENT_TIMEOUT:     30,000ms
```

### 4.4 Redis — Esquema de Cache

```
Prefijos de clave Redis:
  session:{userId}          → Datos de sesion (TTL: segun JWT)
  rate:{ip}:{endpoint}      → Contador rate limit (TTL: 15min)
  cache:user:{userId}       → Perfil de usuario cacheado (TTL: 5min)
  cache:pairs               → Pares de trading actualizados (TTL: 10s)
  pubsub:notifications      → Canal de notificaciones en tiempo real
  pubsub:trading            → Canal de actualizaciones de trading
  queue:emails              → Cola de emails pendientes
  lock:{resource}           → Locks distribuidos (TTL: 30s)
```

---

## 5. Mapa de API

### 5.1 Sovereign Core (Puerto 3050) — 18 Modulos

```
MODULO AUTH          POST   /api/auth/register
                     POST   /api/auth/login
                     POST   /api/auth/refresh
                     POST   /api/auth/logout
                     POST   /api/auth/forgot-password
                     POST   /api/auth/reset-password
                     GET    /api/auth/verify/:token

MODULO USERS         GET    /api/users/profile
                     PUT    /api/users/profile
                     GET    /api/users/:id
                     GET    /api/users
                     PUT    /api/users/:id/roles
                     DELETE /api/users/:id

MODULO PAYMENTS      POST   /api/payments/process
                     GET    /api/payments/:id
                     GET    /api/payments/history
                     POST   /api/payments/refund/:id

MODULO MESSAGING     GET    /api/messaging/conversations
                     POST   /api/messaging/send
                     GET    /api/messaging/messages/:conversationId
                     WS     /api/messaging/ws (WebSocket)

MODULO VOTING        POST   /api/voting/proposals
                     GET    /api/voting/proposals
                     POST   /api/voting/vote/:proposalId
                     GET    /api/voting/results/:proposalId
                     POST   /api/voting/delegate

MODULO STORAGE       POST   /api/storage/upload
                     GET    /api/storage/files
                     GET    /api/storage/download/:id
                     DELETE /api/storage/files/:id

MODULO ANALYTICS     GET    /api/analytics/dashboard
                     GET    /api/analytics/metrics
                     GET    /api/analytics/reports
                     POST   /api/analytics/events

MODULO CONTENT       POST   /api/content/publish
                     GET    /api/content/feed
                     PUT    /api/content/:id
                     DELETE /api/content/:id
                     POST   /api/content/:id/moderate

MODULO EXCHANGE      GET    /api/exchange/pairs
                     GET    /api/exchange/orderbook/:pair
                     POST   /api/exchange/orders
                     DELETE /api/exchange/orders/:id
                     GET    /api/exchange/trades

MODULO BANK          GET    /api/bank/accounts
                     POST   /api/bank/accounts
                     POST   /api/bank/transfer
                     GET    /api/bank/statements/:accountId
                     GET    /api/bank/balance/:accountId

MODULO ATM           GET    /api/atm/locations
                     POST   /api/atm/withdraw
                     POST   /api/atm/deposit
                     GET    /api/atm/status/:id

MODULO CRYPTO        POST   /api/crypto/wallet/create
                     GET    /api/crypto/wallet/:address
                     POST   /api/crypto/sign
                     POST   /api/crypto/verify
                     GET    /api/crypto/balance/:address

MODULO HOSTING       POST   /api/hosting/sites
                     GET    /api/hosting/sites
                     PUT    /api/hosting/sites/:id
                     DELETE /api/hosting/sites/:id
                     POST   /api/hosting/domains

MODULO MARKETPLACE   GET    /api/marketplace/listings
                     POST   /api/marketplace/listings
                     PUT    /api/marketplace/listings/:id
                     POST   /api/marketplace/purchase/:id
                     GET    /api/marketplace/orders

MODULO LOANS         POST   /api/loans/apply
                     GET    /api/loans/:id
                     GET    /api/loans/my-loans
                     POST   /api/loans/:id/repay
                     GET    /api/loans/credit-score

MODULO STAKING       POST   /api/staking/stake
                     POST   /api/staking/unstake
                     GET    /api/staking/rewards
                     GET    /api/staking/validators
                     GET    /api/staking/pools

MODULO NOTIFICATIONS GET    /api/notifications
                     PUT    /api/notifications/:id/read
                     PUT    /api/notifications/read-all
                     POST   /api/notifications/subscribe
                     DELETE /api/notifications/unsubscribe

MODULO WIFI-BRIDGE   GET    /api/wifi-bridge/status
                     POST   /api/wifi-bridge/connect
                     GET    /api/wifi-bridge/networks
                     POST   /api/wifi-bridge/provision
```

### 5.2 Endpoint de Salud

Todos los servicios exponen:

```
GET /health          → { status: "healthy", service: "nombre", uptime: N }
GET /metrics         → Metricas Prometheus (formato OpenMetrics)
```

---

## 6. Flujo de Datos

### 6.1 Flujo de Autenticacion

```
  Cliente             Nginx            Sovereign Core        PostgreSQL         Redis
    │                   │                    │                    │                │
    │ POST /auth/login  │                    │                    │                │
    │──────────────────>│                    │                    │                │
    │                   │ proxy_pass :3050   │                    │                │
    │                   │───────────────────>│                    │                │
    │                   │                    │ SELECT user        │                │
    │                   │                    │───────────────────>│                │
    │                   │                    │ user row           │                │
    │                   │                    │<───────────────────│                │
    │                   │                    │                    │                │
    │                   │                    │ bcrypt.compare()   │                │
    │                   │                    │                    │                │
    │                   │                    │ JWT sign (HS256)   │                │
    │                   │                    │                    │                │
    │                   │                    │ SET session        │                │
    │                   │                    │───────────────────────────────────>│
    │                   │                    │                    │                │
    │                   │ { token, refresh } │                    │                │
    │<──────────────────│<───────────────────│                    │                │
    │                   │                    │                    │                │
```

### 6.2 Flujo de Transaccion Bancaria

```
  Cliente        Nginx       BDET Bank       PostgreSQL      MameyNode      Redis
    │               │            │                │               │            │
    │ POST transfer │            │                │               │            │
    │──────────────>│            │                │               │            │
    │               │───────────>│                │               │            │
    │               │            │                │               │            │
    │               │            │ Verificar JWT  │               │            │
    │               │            │ Rate limit     │               │            │
    │               │            │───────────────────────────────────────────>│
    │               │            │ <rate check ok>│               │            │
    │               │            │<───────────────────────────────────────────│
    │               │            │                │               │            │
    │               │            │ BEGIN TX       │               │            │
    │               │            │───────────────>│               │            │
    │               │            │ Debitar origen │               │            │
    │               │            │───────────────>│               │            │
    │               │            │ Acreditar dest │               │            │
    │               │            │───────────────>│               │            │
    │               │            │ Registro audit │               │            │
    │               │            │───────────────>│               │            │
    │               │            │ COMMIT TX      │               │            │
    │               │            │───────────────>│               │            │
    │               │            │                │               │            │
    │               │            │ Registrar en   │               │            │
    │               │            │ blockchain     │               │            │
    │               │            │───────────────────────────────>│            │
    │               │            │ tx_hash        │               │            │
    │               │            │<───────────────────────────────│            │
    │               │            │                │               │            │
    │               │ { success, │                │               │            │
    │               │   tx_hash }│                │               │            │
    │<──────────────│<───────────│                │               │            │
```

### 6.3 Flujo de Votacion Blockchain

```
  Ciudadano     Voto Soberano     Identity Svc     MameyNode      Smart Contract
    │                │                 │                │               │
    │ POST /vote     │                 │                │               │
    │───────────────>│                 │                │               │
    │                │ Verificar FWID  │                │               │
    │                │────────────────>│                │               │
    │                │ identidad ok    │                │               │
    │                │<────────────────│                │               │
    │                │                 │                │               │
    │                │ Generar ZK proof│                │               │
    │                │ (anonimato)     │                │               │
    │                │                 │                │               │
    │                │ Enviar TX       │                │               │
    │                │────────────────────────────────>│               │
    │                │                 │               │ VoteContract   │
    │                │                 │               │──────────────>│
    │                │                 │               │ emit VoteCast │
    │                │                 │               │<──────────────│
    │                │ tx_hash         │               │               │
    │                │<────────────────────────────────│               │
    │                │                 │                │               │
    │ { voted, hash }│                 │                │               │
    │<───────────────│                 │                │               │
```

---

## 7. Red de Comunicacion

### 7.1 Red Docker

```
Red: sovereign-net
Tipo: bridge
Subred: 172.28.0.0/16
Gateway: 172.28.0.1

Todos los servicios se comunican internamente via nombres Docker.
Todos los puertos bind a 127.0.0.1 (no expuestos al exterior).
Solo Nginx (80/443) esta expuesto al internet.
```

### 7.2 Enrutamiento Nginx

```nginx
# Configuracion principal de enrutamiento

upstream sovereign_core    { server 127.0.0.1:3050; }
upstream bdet_bank         { server 127.0.0.1:3001; }
upstream voz_soberana      { server 127.0.0.1:3002; }
upstream red_social        { server 127.0.0.1:3003; }
upstream correo_soberano   { server 127.0.0.1:3004; }
upstream reservas          { server 127.0.0.1:3005; }
upstream voto_soberano     { server 127.0.0.1:3006; }
upstream trading           { server 127.0.0.1:3007; }
upstream conferencia       { server 127.0.0.1:3090; }
upstream vigilancia        { server 127.0.0.1:3091; }
upstream empresa           { server 127.0.0.1:3092; }

# Rutas
/api/core/*       →  sovereign_core
/api/bank/*       →  bdet_bank
/api/voz/*        →  voz_soberana
/api/social/*     →  red_social
/api/correo/*     →  correo_soberano
/api/reservas/*   →  reservas
/api/voto/*       →  voto_soberano
/api/trading/*    →  trading
/api/conf/*       →  conferencia
/api/siem/*       →  vigilancia
/api/erp/*        →  empresa
/                 →  plataformas HTML (contenido estatico)
```

### 7.3 Comunicacion Inter-Servicio

```
Patron                Tecnologia          Uso
───────               ──────────          ───
Sincrono              HTTP/REST           API calls entre servicios
Asincrono             RabbitMQ            Colas de procesamiento, eventos
Tiempo Real           WebSocket (ws)      Mensajeria, notificaciones, trading
Streaming             gRPC (Rust/Go)      Operaciones blockchain de alto rendimiento
Cache Compartido      Redis Pub/Sub       Eventos entre servicios, invalidacion de cache
```

---

## 8. Seguridad — Capas de Defensa

```
┌─────────────────────────────────────────────────────────────┐
│  CAPA 7: AI AGENTS (cliente)                                │
│  Guardian | Pattern | Anomaly | Trust | Shield | Forensic   │
│  Evolution — 7 agentes en 400+ plataformas                  │
├─────────────────────────────────────────────────────────────┤
│  CAPA 6: IDENTIDAD                                          │
│  FWID | DID/SSI | ZK-SNARKs | Biometria (7 modalidades)    │
│  Identity Service (:5001) | ZKP Service (:5002)             │
├─────────────────────────────────────────────────────────────┤
│  CAPA 5: BLOCKCHAIN                                         │
│  MameyNode PoSov | Smart Contracts Auditados                │
│  Transacciones Firmadas | Immutabilidad                     │
├─────────────────────────────────────────────────────────────┤
│  CAPA 4: APLICACION                                         │
│  JWT HS256 (exp 24h) | bcrypt 12 rounds                     │
│  CORS restrictivo | Validacion de entrada                   │
│  Rate Limiting: 200 req/15min general, 10/15min auth        │
│  Helmet.js headers | CSRF protection                        │
├─────────────────────────────────────────────────────────────┤
│  CAPA 3: DATOS                                              │
│  PostgreSQL pgcrypto | UUIDs v4                             │
│  Audit trail (old/new values) | Backups AES-256             │
│  Redis auth | Indices optimizados                           │
├─────────────────────────────────────────────────────────────┤
│  CAPA 2: RED                                                │
│  Nginx SSL/TLS | Headers de seguridad                       │
│  HSTS | CSP | X-Frame-Options | X-Content-Type-Options      │
│  Docker network aislada (172.28.0.0/16)                     │
│  Bind 127.0.0.1 (solo local)                                │
├─────────────────────────────────────────────────────────────┤
│  CAPA 1: ENCRIPTACION                                       │
│  ML-DSA-65 + ML-KEM-1024 (post-cuantica NIST)              │
│  CRYSTALS-Kyber | TLS 1.3                                   │
├─────────────────────────────────────────────────────────────┤
│  CAPA 0: RESISTENCIA A CENSURA                              │
│  Tor Hidden Service | Handshake DNS (.ierahkwa)             │
│  Mesh LoRa (Meshtastic) | IPFS Archivo Eterno              │
│  CBDC NFC Offline | P2P directo                             │
└─────────────────────────────────────────────────────────────┘
```

### 8.1 Rate Limiting Detallado

```
Endpoint                   Limite              Ventana
────────                   ──────              ───────
General (todas las rutas)  200 requests        15 minutos
Autenticacion (/auth/*)    10 requests         15 minutos
Intentos de login fallidos 5 intentos          Bloqueo temporal
Subida de archivos         50 MB max/archivo   N/A
WebSocket payload          64 KB max           N/A
```

### 8.2 Cadena de Suministro

```
Componente                    Herramienta
──────────                    ───────────
Audit de dependencias npm     npm audit + Dependabot
Audit de dependencias NuGet   dotnet list --vulnerable + Dependabot
Audit de dependencias Cargo   cargo audit + Dependabot
Imagenes Docker               Trivy + Dependabot
GitHub Actions                Dependabot
SBOM                          syft / cyclonedx
Lifecycle scripts             Proteccion Shai-Hulud
Firma de commits              GPG signing
```

---

## 9. Monitoreo

### 9.1 Prometheus — Configuracion de Scraping

```
Job                    Target                        Intervalo    Tipo
───                    ──────                        ─────────    ────
prometheus             localhost:9090                15s          Self
ierahkwa-node          node-app:8545                 10s          Blockchain
banking-bridge         banking-bridge:3001           15s          Banking
platform               localhost:8545                15s          Web
dotnet-services        15 targets (:5054-:7071)      15s          .NET
rust-service           rust-service:8590             15s          Financial
go-service             go-service:8591               15s          Queue
python-service         python-service:8592           15s          AI/ML
postgres               postgres-exporter:9187        15s          Database
mongodb                mongodb-exporter:9216         15s          Database
redis                  redis-exporter:9121           15s          Cache
nginx                  nginx-exporter:9113           15s          Proxy
rabbitmq               rabbitmq:15692                15s          Queue
elasticsearch          elasticsearch:9200            15s          Search
node-exporter          node-exporter:9100            15s          System
blackbox-http          3 endpoints HTTPS             15s          Probes
```

### 9.2 Endpoints Monitoreados (Blackbox)

```
https://ierahkwa.gov/health
https://api.ierahkwa.gov/health
https://bdet.ierahkwa.gov/health
```

### 9.3 Alertas

El archivo `monitoring/alerts/critical.yml` define alertas criticas para:

- Servicio caido (instancia no responde en 5 minutos)
- Uso de CPU > 90% durante 10 minutos
- Memoria disponible < 10%
- Disco > 85% de uso
- Latencia de base de datos > 1 segundo
- Tasa de errores HTTP > 5%
- Blockchain nodo desconectado
- Cola RabbitMQ > 10,000 mensajes pendientes

### 9.4 Grafana — Dashboards

```
Dashboard                  Contenido
─────────                  ─────────
Sovereign Overview         Estado general de todos los servicios
Blockchain Health          TPS, bloques, validadores, gas
Banking Operations         Transacciones, balances, comisiones
Platform Traffic           Requests/s, latencia, errores por servicio
Database Performance       Queries/s, pool, locks, replication lag
Infrastructure             CPU, RAM, disco, red por contenedor
Security Events            Intentos de login, bloqueos, anomalias
Trading Activity           Ordenes, volumen, pares, spreads
```

---

## 10. Despliegue — CI/CD Pipeline

### 10.1 Genesis Boot Script

El script `deploy-ierahkwa.sh` ejecuta el despliegue completo en 6 pasos:

```
Paso 1/6 — Verificar prerequisitos
  ├── Docker + Docker Compose
  ├── docker-compose.sovereign.yml existe
  └── Docker daemon activo

Paso 1b/6 — Verificar servicios soberanos
  ├── Tor (hidden service)
  ├── IPFS (archivo eterno)
  ├── Ollama (IA soberana)
  ├── Handshake DNS (.ierahkwa TLD)
  └── Meshtastic (mesh LoRa offline)

Paso 2/6 — Configurar variables de entorno
  ├── Copiar .env.example → .env (si no existe)
  ├── Generar JWT_SECRET (openssl rand -base64 48)
  ├── Generar IDENTITY_JWT_SECRET
  ├── Generar POSTGRES_PASSWORD (32 chars alfanumerico)
  ├── Generar REDIS_PASSWORD (32 chars alfanumerico)
  └── Generar GRAFANA_ADMIN_PASSWORD (24 chars)

Paso 3/6 — Preparar despliegue
  └── Detener contenedores existentes (docker compose down)

Paso 4/6 — Construir e iniciar servicios
  ├── docker compose build --parallel
  └── docker compose up -d

Paso 5/6 — Verificar salud de servicios
  ├── 26 servicios con health check
  ├── Timeout maximo: 120 segundos
  └── Intervalo de verificacion: 5 segundos

Paso 6/6 — Resumen final
  └── Listado de todos los servicios con URLs
```

### 10.2 GitHub Actions — 9 Workflows Agenticos

```
Workflow                     Trigger              Proposito
────────                     ───────              ─────────
Continuous Triage            Issues nuevos        Auto-clasificar, etiquetar, bienvenida
Continuous Docs              Diario               Detectar drift de documentacion
Continuous Testing           Semanal              Analisis de cobertura de tests
Continuous Security          Diario + push        Monitoreo de cumplimiento OWASP
Continuous Quality           Pull Requests        Revision de calidad arquitectonica
Continuous Reporting         Semanal              Reportes de salud de plataforma
Continuous Translation       Push (i18n)          Detectar drift de traducciones
Continuous Performance       PRs + semanal        Deteccion de regresiones de rendimiento
Continuous Supply Chain      Diario + push        Defensa Shai-Hulud, audit de dependencias
```

### 10.3 Archivos Docker Compose

```
docker-compose.sovereign.yml     Produccion: todos los 26 servicios
docker-compose.dev.yml            Desarrollo: hot-reload, debug
docker-compose.infra.yml          Solo infra: PostgreSQL, Redis, Prometheus
docker-compose.cloud.yml          Nube: adaptado para EKS/ECS
```

### 10.4 Makefile

```makefile
make start          # Inicia todos los servicios
make stop           # Detiene todos los servicios
make status         # Health check de todos los servicios
make build-all      # Construye todo (.NET, Rust, Docker)
make test           # Ejecuta todos los tests
make clean          # Remueve artifacts de build
make help           # Muestra comandos disponibles
```

---

## 11. Blockchain — MameyNode

### 11.1 Arquitectura de MameyNode

```
┌──────────────────────────────────────────────────────┐
│                  MAMEYNODE v4.2                      │
│               Chain ID: 777777                       │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │            CAPA DE CONSENSO                    │  │
│  │         Proof-of-Sovereignty (PoSov)           │  │
│  │                                                │  │
│  │  Validadores: entidades territoriales (574)    │  │
│  │  Finality: < 3 segundos                        │  │
│  │  TPS: 12,847                                   │  │
│  │  Participacion civica = peso de voto           │  │
│  └────────────────────────────────────────────────┘  │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │            CAPA DE EJECUCION                   │  │
│  │         EVM-Compatible (Solidity 0.8.x)        │  │
│  │                                                │  │
│  │  Smart Contracts:                              │  │
│  │    WampumToken.sol    — Token WMP (ERC-20)     │  │
│  │    SICBDC.sol         — Stablecoin soberana     │  │
│  │    IGTGovernance.sol  — 103 tokens gobernanza  │  │
│  │    SNTTerritory.sol   — 574 tokens territorio  │  │
│  │    VoteContract.sol   — Votacion inmutable     │  │
│  │    StakingPool.sol    — Staking y rewards      │  │
│  │    FeeDistributor.sol — Distribucion auto       │  │
│  │    IdentityRegistry.sol — Registro FWID        │  │
│  └────────────────────────────────────────────────┘  │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │            CAPA DE ENCRIPTACION                │  │
│  │         Post-Cuantica (NIST Standardized)      │  │
│  │                                                │  │
│  │  Firmas:  ML-DSA-65 (FIPS 204)                │  │
│  │  Claves:  ML-KEM-1024 (FIPS 203)              │  │
│  │  Archivo: CRYSTALS-Kyber                       │  │
│  └────────────────────────────────────────────────┘  │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │            CAPA DE RED P2P                     │  │
│  │                                                │  │
│  │  Puerto RPC:     8545                          │  │
│  │  Protocolo:      JSON-RPC 2.0                  │  │
│  │  Nodos semilla:  5 (fase Genesis)              │  │
│  │  Nodos target:   574 (uno por nacion tribal)   │  │
│  └────────────────────────────────────────────────┘  │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │            HERRAMIENTAS                        │  │
│  │                                                │  │
│  │  MameyForge CLI (Rust):                        │  │
│  │    mameyforge deploy <contract>                │  │
│  │    mameyforge wallet create                    │  │
│  │    mameyforge tx send                          │  │
│  │    mameyforge node status                      │  │
│  │                                                │  │
│  │  gRPC SDK (Rust + tonic):                      │  │
│  │    BlockchainService.SendTransaction           │  │
│  │    BlockchainService.GetBlock                  │  │
│  │    BlockchainService.Subscribe                 │  │
│  │                                                │  │
│  │  SDKs:                                         │  │
│  │    Go     — 11-sdks/go/                        │  │
│  │    Python — 11-sdks/python/                    │  │
│  │    TypeScript — 11-sdks/typescript/            │  │
│  └────────────────────────────────────────────────┘  │
│                                                      │
└──────────────────────────────────────────────────────┘
```

### 11.2 Tokens en la Red

```
Token     Nombre                  Tipo          Suministro        Quema
─────     ──────                  ────          ──────────        ─────
WMP       Wampum                  Nativo/Gas    10T               0.1%/tx
SICBDC    Stablecoin Indigena     Stablecoin    Dinamico (1:1)    N/A
IGT       Indigenous Governance   Gobernanza    103               N/A (soul-bound)
SNT       Sovereign Nation        Territorial   574               N/A (soul-bound)
```

---

## 12. Diagrama de Red

```
                     ┌───────────────────────────────────────────────┐
                     │              INTERNET PUBLICO                 │
                     │                                               │
                     │   Usuarios: 1B+ personas                     │
                     │   Paises: 35+ (Americas)                     │
                     │   Idiomas: 43 (37 indigenas + 6 globales)    │
                     └────────────────────┬──────────────────────────┘
                                          │
            ┌─────────────────────────────┼─────────────────────────────┐
            │                             │                             │
  ┌─────────▼─────────┐       ┌──────────▼──────────┐      ┌──────────▼──────────┐
  │  ACCESO TOR       │       │  ACCESO HTTPS       │      │  ACCESO DNS SOBERANO│
  │  .onion           │       │  ierahkwa.gov       │      │  .ierahkwa (HNS)    │
  │  Anti-censura     │       │  Puerto 443         │      │  Handshake DNS      │
  └─────────┬─────────┘       └──────────┬──────────┘      └──────────┬──────────┘
            │                             │                             │
            └─────────────────────────────┼─────────────────────────────┘
                                          │
                               ┌──────────▼──────────┐
                               │    FIREWALL / WAF    │
                               │    AWS ALB + WAF     │
                               └──────────┬──────────┘
                                          │
           ┌──────────────────────────────▼────────────────────────────────┐
           │                    SOVEREIGN NETWORK                          │
           │                  172.28.0.0/16 (Docker Bridge)               │
           │                                                               │
           │  ┌─────────────────────────────────────────────────────────┐  │
           │  │                    NGINX PROXY                          │  │
           │  │                 172.28.0.2:80/443                       │  │
           │  │  SSL/TLS | gzip | rate-limit | cache | headers         │  │
           │  └──────────┬──────────┬──────────┬───────────────────────┘  │
           │             │          │          │                           │
           │  ┌──────────▼──┐  ┌───▼────┐  ┌─▼──────────────────────┐   │
           │  │ ESTATICO    │  │ API    │  │ WEBSOCKET              │   │
           │  │ HTML/CSS/JS │  │ REST   │  │ Mensajeria/Trading     │   │
           │  │ 422+ plats  │  │        │  │ Notificaciones         │   │
           │  └─────────────┘  └───┬────┘  └───────────┬────────────┘   │
           │                       │                    │                 │
           │     ┌─────────────────┼────────────────────┘                │
           │     │                 │                                      │
           │  ┌──▼─────────────────▼─────────────────────────────────┐   │
           │  │              SERVICIOS BACKEND                       │   │
           │  │                                                      │   │
           │  │  Node.js (172.28.1.x)      .NET (172.28.2.x)        │   │
           │  │  ────────────────────      ──────────────────        │   │
           │  │  :3001  BDET Bank          :5001  Identity           │   │
           │  │  :3002  Voz Soberana       :5002  ZKP                │   │
           │  │  :3003  Red Social         :5003  Treasury           │   │
           │  │  :3004  Correo             :5054  TradeX             │   │
           │  │  :3005  Reservas           :5061  Farm               │   │
           │  │  :3006  Voto               :5080  DocFlow            │   │
           │  │  :3007  Trading            :5090  CRM                │   │
           │  │  :3030  POS                :5092  Voting             │   │
           │  │  :3050  Sovereign Core     :7070  ProjectHub         │   │
           │  │  :3090  Conferencia        +73 mas microservicios    │   │
           │  │  :3091  Vigilancia                                   │   │
           │  │  :3092  Empresa            Multi-lenguaje (172.28.3) │   │
           │  │  :3100  Shop               ──────────────────────    │   │
           │  │  :3200  Inventory          :8590  Rust Financial     │   │
           │  │  :3300  Image Upload       :8591  Go Queue           │   │
           │  │  :3400  Forex              :8592  Python ML          │   │
           │  │  :3500  Smart School                                 │   │
           │  └──────────────────┬───────────────────────────────────┘   │
           │                     │                                       │
           │  ┌──────────────────▼───────────────────────────────────┐   │
           │  │               CAPA DE DATOS (172.28.4.x)            │   │
           │  │                                                      │   │
           │  │  ┌────────────┐ ┌─────────┐ ┌──────────┐ ┌────────┐│   │
           │  │  │PostgreSQL  │ │ Redis 7 │ │ RabbitMQ │ │Elastic ││   │
           │  │  │16          │ │         │ │          │ │search  ││   │
           │  │  │:5432       │ │ :6379   │ │ :15692   │ │:9200   ││   │
           │  │  │            │ │         │ │          │ │        ││   │
           │  │  │uuid-ossp   │ │Sessions │ │Colas     │ │Full-   ││   │
           │  │  │pgcrypto    │ │Cache    │ │Eventos   │ │text    ││   │
           │  │  │12+ tablas  │ │Pub/Sub  │ │Async     │ │Search  ││   │
           │  │  │13 indices  │ │Rate lim │ │          │ │Logs    ││   │
           │  │  └────────────┘ └─────────┘ └──────────┘ └────────┘│   │
           │  └──────────────────────────────────────────────────────┘   │
           │                                                             │
           │  ┌──────────────────────────────────────────────────────┐   │
           │  │         MAMEYNODE BLOCKCHAIN (172.28.5.x)            │   │
           │  │                                                      │   │
           │  │  RPC :8545 | Chain ID 777777 | PoSov | 12,847 TPS  │   │
           │  │  Smart Contracts: WMP, SICBDC, IGT, SNT, Vote      │   │
           │  │  Post-cuantica: ML-DSA-65 + ML-KEM-1024            │   │
           │  └──────────────────────────────────────────────────────┘   │
           │                                                             │
           │  ┌──────────────────────────────────────────────────────┐   │
           │  │            MONITOREO (172.28.6.x)                    │   │
           │  │                                                      │   │
           │  │  Prometheus :9090  | Grafana :3000                  │   │
           │  │  Node Exporter :9100 | Blackbox :9115               │   │
           │  │  Alertmanager :9093                                 │   │
           │  │                                                      │   │
           │  │  Exporters: postgres:9187, redis:9121, nginx:9113   │   │
           │  └──────────────────────────────────────────────────────┘   │
           │                                                             │
           └─────────────────────────────────────────────────────────────┘

           SERVICIOS SOBERANOS OPCIONALES (externos a la red Docker):
           ┌────────────────────────────────────────────────────────────┐
           │  Tor         → Hidden service (.onion)                    │
           │  IPFS        → Archivo eterno descentralizado             │
           │  Ollama      → IA soberana local                          │
           │  Handshake   → DNS soberano (.ierahkwa TLD)               │
           │  Meshtastic  → Mesh LoRa comunicacion offline             │
           └────────────────────────────────────────────────────────────┘
```

### 12.1 Resumen de Puertos

```
RANGO         USO                              CANTIDAD
─────         ───                              ────────
80, 443       Nginx (HTTP/HTTPS)               2
3001-3007     Servicios Node.js originales      7
3030          POS System                        1
3050          Sovereign Core                    1
3090-3092     Servicios soberanos extendidos    3
3100-3500     Servicios Node.js adicionales     5
5001-5003     .NET Core (Identity, ZKP, Tres.)  3
5054-7071     .NET Microservicios               15+
8545          MameyNode Blockchain              1
8590-8592     Rust, Go, Python services          3
9090          Prometheus                         1
3000          Grafana                            1
9100-9200     Exporters + Elasticsearch          6+
5432          PostgreSQL                         1
6379          Redis                              1
15692         RabbitMQ                           1
                                          TOTAL: ~55+
```

---

## Notas de Implementacion

### Estructura del Repositorio

```
Soberano-Organizado/
├── 01-documentos/              → Marco legal, inversores, whitepapers
├── 02-plataformas-html/        → 422+ plataformas (18 NEXUS)
│   ├── shared/                 → ierahkwa.css, .js, agents.js, sw.js
│   └── [18 NEXUS portales]/   → Subdirectorios por portal
├── 03-backend/                 → 20 servicios Node.js
│   ├── sovereign-core/         → 18 modulos (auth, bank, exchange...)
│   ├── api-gateway/            → Gateway API
│   ├── blockchain-api/         → Interfaz RPC MameyNode
│   └── [16+ servicios mas]/   → Cada uno con su propio package.json
├── 04-infraestructura/         → Docker, K8s, Terraform, Nginx
│   ├── database/init/          → 01-schema.sql (12+ tablas, 13 indices)
│   ├── monitoring/             → prometheus.yml, alerts/critical.yml
│   └── deploy/                 → Scripts y compose cloud
├── 05-api/                     → OpenAPI specs, gRPC contracts, protos
├── 06-dashboards/              → Centro de comando, Maestro
├── 07-scripts/                 → Scripts operacionales
├── 08-dotnet/                  → Ecosistema .NET completo
│   ├── framework/              → Mamey Framework (139 proyectos)
│   ├── microservices/          → 84 microservicios soberanos
│   ├── government/             → Identity, Monolith, Portal, Pupitre
│   └── banking/                → INKG Bank, NET10
├── 09-assets/                  → Logo, branding
├── 10-core/                    → Mamey core libraries
├── 11-sdks/                    → SDKs (Go, Python, TypeScript)
├── 12-rust/                    → MameyForge CLI + gRPC SDK
├── 13-ai/                      → MameyFutureAI + code generator
├── 14-blockchain/              → 103 IGT tokens, FutureWampum, MameyNode
├── 15-utilities/               → Barcode, image processing, templates
├── 16-docs/                    → 368 documentos gubernamentales
└── 17-files-originales/        → Archivo de archivos originales
```

### Variables de Entorno Criticas

```
Variable                    Servicio            Requerida en Prod
────────                    ────────            ─────────────────
DATABASE_URL                Sovereign Core      Si
JWT_SECRET                  Todos (auth)        Si (min 32 chars)
CORS_ORIGINS                Todos               Si
POSTGRES_PASSWORD           PostgreSQL          Si
REDIS_PASSWORD              Redis               Si
MAMEYNODE_RPC               Blockchain          Si
WAMPUM_CONTRACT_ADDRESS     Trading/Bank        Si
IDENTITY_JWT_SECRET         Identity .NET       Si
GRAFANA_ADMIN_PASSWORD      Grafana             Si
TLS_CERT_PATH               Nginx               Si
TLS_KEY_PATH                Nginx               Si
BACKUP_ENCRYPTION_KEY       Backups             Si
```

### Comandos Git

```bash
# IMPORTANTE: Siempre usar GIT_NO_OPTIONAL_LOCKS=1
GIT_NO_OPTIONAL_LOCKS=1 git status
GIT_NO_OPTIONAL_LOCKS=1 git add .
GIT_NO_OPTIONAL_LOCKS=1 git commit -m "mensaje"

# Pack config para repositorios grandes
git config pack.threads 1
git config pack.windowMemory 256m
git config pack.packSizeLimit 128m
git config http.postBuffer 524288000
```

---

*Copyright 2026 Gobierno Soberano de Ierahkwa Ne Kanienke. Sovereign License 1.0.*
*Documento tecnico para uso interno y de desarrollo.*

# BLUEPRINT: StarLink Soberano — Internet Satelital Soberano

**Planos Tecnicos y Diagramas de Arquitectura**
**Version**: 2.0.0
**Fecha**: 2026-03-06
**NEXUS**: NEXUS Cosmos (Espacio & Satelital)

---

## 1. Diagrama de Sistema Completo

```
┌───────────────────────────────────────────────────────────────┐
│                    STARLINK SOBERANO                           │
│                  Internet Satelital Soberano                   │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │                 CAPA FRONTEND                           │  │
│  │                                                         │  │
│  │  ┌───────────────────┐    ┌───────────────────────┐    │  │
│  │  │  CAPTIVE PORTAL   │    │  ADMIN DASHBOARD      │    │  │
│  │  │  (index.html)     │    │  (index.html)         │    │  │
│  │  │                   │    │                       │    │  │
│  │  │  ● Plans Grid     │    │  ● Stats Cards (6)    │    │  │
│  │  │  ● WAMPUM Pay     │    │  ● Revenue Chart      │    │  │
│  │  │  ● Session Timer  │    │  ● Hotspot Cards (6)  │    │  │
│  │  │  ● Speed Display  │    │  ● Fleet Cards (4)    │    │  │
│  │  └───────────────────┘    │  ● Vigilancia Log     │    │  │
│  │                           │  ● VIP Protection     │    │  │
│  │                           └───────────────────────┘    │  │
│  │                                                         │  │
│  │  Shared: ierahkwa.css + ierahkwa.js + ierahkwa-agents.js│  │
│  │  Storage: 8 IndexedDB stores                            │  │
│  └─────────────────────────────────────────────────────────┘  │
│                              │                                │
│  ┌───────────────────────────▼─────────────────────────────┐  │
│  │                 CAPA BACKEND (wifi-soberano:3095)       │  │
│  │                                                         │  │
│  │  Express 4.21 ─── 7 Route Files ─── 2 Middleware        │  │
│  │       │                                                  │  │
│  │  ┌────┴────┐  ┌──────────┐  ┌──────────┐  ┌─────────┐ │  │
│  │  │ WebSocket│  │PostgreSQL│  │  Redis 7 │  │MameyNode│ │  │
│  │  │ /ws/wifi │  │  16-alp  │  │ Sessions │  │Chain 574│ │  │
│  │  │Real-time │  │ 9 tables │  │Vigilancia│  │ WAMPUM  │ │  │
│  │  └─────────┘  └──────────┘  └──────────┘  └─────────┘ │  │
│  └─────────────────────────────────────────────────────────┘  │
│                              │                                │
│  ┌───────────────────────────▼─────────────────────────────┐  │
│  │                 CAPA INFRAESTRUCTURA                     │  │
│  │                                                         │  │
│  │  Nginx (reverse proxy + rate limit + bandwidth enforce) │  │
│  │  Docker Compose (2 replicas, 1GB RAM limit)             │  │
│  │  Kubernetes (HPA 2-10, health/readiness probes)         │  │
│  │  SSL/TLS (Let's Encrypt, HSTS, Kyber-768)              │  │
│  └─────────────────────────────────────────────────────────┘  │
│                              │                                │
│  ┌───────────────────────────▼─────────────────────────────┐  │
│  │                 CAPA HARDWARE                            │  │
│  │                                                         │  │
│  │  8x Starlink Kits (Gen1, Gen3, Mini, Mesh, Standard)    │  │
│  │  Routers Mesh (multiplicadores de senal)                │  │
│  │  6 Hotspots en Panama (Tocumen→Guna Yala→Embera)        │  │
│  └─────────────────────────────────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
```

## 2. Flujo de Conexion WiFi

```
Usuario                 Captive Portal          Backend              Redis/DB
  │                          │                     │                    │
  │── Conecta a WiFi ──────▶│                     │                    │
  │                          │                     │                    │
  │◀── Redirect /portal ────│                     │                    │
  │                          │                     │                    │
  │── Selecciona Plan ─────▶│                     │                    │
  │                          │── POST /payment ───▶│                    │
  │                          │                     │── Verify WAMPUM ──▶│
  │                          │                     │◀── Confirmed ──────│
  │                          │                     │                    │
  │                          │── POST /connect ───▶│                    │
  │                          │                     │── Create Session ─▶│
  │                          │                     │── Set Redis TTL ──▶│
  │                          │◀── JWT Token ───────│                    │
  │◀── Access Granted ──────│                     │                    │
  │                          │                     │                    │
  │── Navega Internet ─────▶│── Captive MW ──────▶│── Check Redis ────▶│
  │                          │                     │◀── Session OK ─────│
  │                          │── Bandwidth MW ───▶│── Get Plan ────────▶│
  │                          │                     │◀── 50 Mbps ────────│
  │                          │                     │                    │
  │── (Tiempo expira) ─────▶│                     │                    │
  │                          │── Session Check ──▶│── TTL Expired ────▶│
  │◀── Redirect /portal ────│                     │                    │
```

## 3. Flujo de Vigilancia

```
TODA CONEXION
     │
     ▼
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ Vigilancia   │────▶│ Redis Log    │────▶│ WebSocket    │
│ Middleware   │     │ (real-time)  │     │ Broadcast    │
│              │     │              │     │ → Admin      │
│ IP, MAC, UA  │     └──────────────┘     └──────────────┘
│ Path, Query  │
│ Geo, Time    │     ┌──────────────┐
│              │────▶│ VIP Check    │
└──────────────┘     │              │
                     │ ¿Busca VIP?  │
                     │   SI → ALERT │
                     │   NO → Log   │
                     └──────────────┘
                           │ SI
                           ▼
                     ┌──────────────┐
                     │ ATABEY AI    │
                     │              │
                     │ ● Alert CRIT │
                     │ ● Monitor 24/7│
                     │ ● Track All  │
                     │ ● Block if   │
                     │   hostile    │
                     └──────────────┘
```

## 4. Estructura de Archivos

```
starlink-soberano/                     (Frontend)
├── index.html                         ← Portal WiFi + Admin Dashboard
├── README.md                          ← Documentacion
├── WHITEPAPER.md                      ← Documento tecnico
├── BLUEPRINT.md                       ← Este archivo (planos)
└── ../shared/
    ├── ierahkwa.css                   ← Design system
    ├── ierahkwa.js                    ← Core JavaScript
    ├── ierahkwa-security.js           ← Post-quantum crypto
    ├── ierahkwa-quantum.js            ← Quantum computing
    ├── ierahkwa-protocols.js          ← P2P protocols
    ├── ierahkwa-interconnect.js       ← Platform interconnect
    ├── ierahkwa-api.js                ← API client
    └── ierahkwa-agents.js             ← 7 AI Agents

wifi-soberano/                         (Backend — 03-backend/)
├── package.json                       ← Dependencies
├── server.js                          ← Express server + WS
├── Dockerfile                         ← Node 22 Alpine, port 3095
├── .env.example                       ← Environment template
├── routes/
│   ├── auth.js                        ← Portal login + session
│   ├── plans.js                       ← Plan listing
│   ├── sessions.js                    ← Session management
│   ├── payments.js                    ← WAMPUM payments
│   ├── analytics.js                   ← User analytics
│   ├── fleet.js                       ← Starlink fleet CRUD
│   └── admin.js                       ← Admin dashboard API
├── middleware/
│   ├── captive.js                     ← Captive portal redirect
│   └── bandwidth.js                   ← Bandwidth enforcement
├── models/
│   └── migrations.sql                 ← 9 tables + views + triggers
└── tests/
    └── wifi.test.js                   ← Integration tests

Infraestructura:
├── 04-infraestructura/docker/docker-compose.yml  ← wifi-soberano service
├── 04-infraestructura/nginx/nginx.conf           ← WiFi upstream + routes
└── 04-infraestructura/kubernetes/sovereign-cluster.yaml ← WiFi deployment + HPA
```

## 5. Schema de Base de Datos

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ wifi_plans   │     │   hotspots   │     │starlink_fleet│
│              │     │              │     │              │
│ id (PK)      │     │ id (PK)      │◀────│ id (PK)      │
│ name         │     │ name         │     │ utid (UNIQ)  │
│ duration_hrs │     │ lat, lng     │     │ model        │
│ price_wampum │     │ territory    │     │ account_name │
│ bandwidth    │     │ kit_id (FK)──│────▶│ activation   │
│ data_limit   │     │ max_users    │     │ transfer_date│
└──────┬───────┘     └──────┬───────┘     │ status       │
       │                    │              └──────────────┘
       │                    │
┌──────▼───────┐     ┌──────▼───────┐
│wifi_sessions │     │wifi_analytics│
│              │     │              │
│ id (PK)      │     │ id (PK)      │
│ ip_address   │     │ session_id   │
│ mac_address  │     │ hotspot_id   │
│ plan_id (FK) │     │ bytes_up/dn  │
│ hotspot_id   │     │ device_type  │
│ started_at   │     │ os, browser  │
│ expires_at   │     │ timestamp    │
│ data_used_mb │     └──────────────┘
│ status       │
│ payment_id   │
└──────┬───────┘
       │
┌──────▼───────┐     ┌──────────────┐     ┌──────────────┐
│wifi_payments │     │vip_protected │     │  vigilancia  │
│              │     │              │     │   _alerts    │
│ id (PK)      │     │ id (PK)      │     │              │
│ session_id   │     │ name         │     │ id (PK)      │
│ amount       │     │ role         │     │ alert_type   │
│ tx_hash      │     │ keywords     │     │ severity     │
│ wallet_addr  │     │ protection   │     │ ip, mac, ua  │
│ status       │     │ atabey_monit │     │ matched_vip  │
└──────────────┘     └──────────────┘     │ details JSON │
                                          └──────────────┘
                     ┌──────────────┐
                     │  vigilancia  │
                     │ _connections │
                     │              │
                     │ id (PK)      │
                     │ ip, mac, ua  │
                     │ method, path │
                     │ geo data     │
                     │ timestamp    │
                     └──────────────┘
```

## 6. Mapa de Hotspots

```
                    PANAMA
         ┌─────────────────────────┐
         │                         │
         │    ┏━━━━━━━━━━━━━━━┓    │
    ●────│────┃ Ft Lauderdale ┃    │  (relay hub)
    │    │    ┗━━━━━━━━━━━━━━━┛    │
    │    │                         │
    │    │  ●─── Tocumen (100u)    │
    │    │  │                      │
    │    │  ●─── Chepo (75u)       │
    │    │  │                      │
    │    │  ●─── Darien (50u)      │
    │    │  │                      │
    │    │  ●─── Guna Yala (60u)   │
    │    │  │                      │
    │    │  ●─── Embera (40u)      │
    │    │  │                      │
    │    │  ●─── Carti (80u)       │
    │    │                         │
    │    └─────────────────────────┘
    │
    └── Expansion: Colombia, Ecuador, Peru, Bolivia...
        574 territorios × 19 naciones
```

## 7. Docker & Kubernetes

### Docker Compose Service
```yaml
wifi-soberano:
  build: ./03-backend/wifi-soberano
  ports: ["3095:3095"]
  replicas: 2
  memory: 1G
  depends_on: [postgres, redis, mameynode]
  network: soberana
```

### Kubernetes Deployment
```yaml
Deployment: wifi-soberano (2 replicas)
Service: ClusterIP :3095
HPA: min=2, max=10, cpu=70%
Probes: /health (liveness + readiness)
Resources: 250m-1000m CPU, 256Mi-1Gi RAM
```

### Nginx Routes
```
/wifi/          → wifi-soberano:3095 (captive portal)
/api/v1/wifi/   → wifi-soberano:3095 (API)
/ws/wifi        → wifi-soberano:3095 (WebSocket)
```

## 8. Especificaciones de Rendimiento

| Metrica | Objetivo | Estado |
|---------|----------|--------|
| Captive Portal Load | < 1s | ~0.5s |
| API Response Time | < 100ms | ~50ms |
| WebSocket Latency | < 50ms | ~20ms |
| Concurrent Sessions | 500+ | Configurado |
| Throughput | 10K req/min | Nginx optimizado |
| Database Queries | < 50ms | Indexado |
| Redis TTL Check | < 5ms | In-memory |

## 9. Seguridad Multi-Capa

```
Capa 1: CDN + WAF (Cloudflare)
          │
Capa 2: Nginx (rate limit + headers + SSL)
          │
Capa 3: Captive Middleware (session check)
          │
Capa 4: Bandwidth Middleware (plan enforcement)
          │
Capa 5: Vigilancia Middleware (log EVERYTHING)
          │
Capa 6: 7 AI Agents (Guardian→Evolution)
          │
Capa 7: Atabey AI (VIP Protection 24/7)
          │
Capa 8: Post-Quantum Crypto (Kyber-768)
```

## 10. Requisitos de Despliegue

| Requisito | Especificacion |
|-----------|---------------|
| Server OS | Ubuntu 22.04+ / Alpine Linux |
| Runtime | Node.js 22 LTS |
| Database | PostgreSQL 16 |
| Cache | Redis 7 |
| Container | Docker 24+ / Kubernetes 1.28+ |
| Proxy | Nginx 1.25+ |
| SSL | Let's Encrypt (auto-renew) |
| DNS | wifi.ierahkwa.gov / wifi.soberano.bo |
| Puerto | 3095 (internal), 443 (external) |
| RAM | 1GB per replica (min 2 replicas) |
| Storage | 50GB PostgreSQL + 10GB Redis |

---

**NEXUS Cosmos (Espacio & Satelital)** · Ierahkwa Ne Kanienke · Nacion Digital Soberana

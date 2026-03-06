# NAE Turismo Inteligente — Technical Blueprint

**Version**: 1.0.0 | **Date**: March 2026

## Architecture Diagram

```
┌──────────────────────────────────────────────────────┐
│              INTERFACES (5 Roles + Tourist)            │
│  Turista App · Transportista · Lanchero · Cabaña      │
│  Tour Operador · Admin Zonal · Congreso Guna          │
│  QR Scanner · Calendar · Payments · Offline Mode      │
└──────────┬───────────────────────────────────────────┘
           │ REST + WebSocket + P2P Mesh
┌──────────┴───────────────────────────────────────────┐
│         API GATEWAY (Kyber-768)                       │
│  Rate Limit · JWT · Geofence Guna · Role-Based       │
│  Island Capacity Engine · Maritime Safety             │
└──────────┬───────────────────────────────────────────┘
           │
┌──────────┴───────────────────────────────────────────┐
│         CORE SERVICES                                 │
│  ┌──────────┐ ┌──────────┐ ┌──────────────────────┐ │
│  │Booking   │ │Capacity  │ │Payment WAMPUM        │ │
│  │Engine    │ │Per Island│ │40/20/15/10/10/5      │ │
│  └──────────┘ └──────────┘ └──────────────────────┘ │
│  ┌──────────┐ ┌──────────┐ ┌──────────────────────┐ │
│  │QR Flow   │ │Transport │ │Role Dashboard        │ │
│  │Tracker   │ │Coord.    │ │Engine (5 roles)      │ │
│  └──────────┘ └──────────┘ └──────────────────────┘ │
│  ┌──────────┐ ┌──────────┐ ┌──────────────────────┐ │
│  │Calendar  │ │Weather   │ │Mola Marketplace      │ │
│  │Occupancy │ │Maritime  │ │+ Blockchain Auth     │ │
│  └──────────┘ └──────────┘ └──────────────────────┘ │
└──────────┬───────────────────────────────────────────┘
           │
┌──────────┴───────────────────────────────────────────┐
│         DATA STORES                                   │
│  PostgreSQL+PostGIS · Redis · TimescaleDB             │
│  MameyNode Blockchain · IndexedDB (offline)           │
│  P2P Mesh Sync (island-to-island)                     │
└──────────────────────────────────────────────────────┘
```

## Tourist Flow Diagram

```
┌─────────────────────────────────────────────────────┐
│ GUNA YALA TOURIST FLOW (QR-tracked)                 │
├─────────────────────────────────────────────────────┤
│                                                     │
│  [1] RESERVE ONLINE ──→ QR Code Generated           │
│        │                                            │
│  [2] 4x4 PANAMA→CARTI ──→ Transportista scans QR   │
│        │                    (2.5 hrs, GPS tracked)  │
│        │                                            │
│  [3] EMBARCADERO CARTI ──→ Lanchero scans QR        │
│        │                    (maritime manifest)     │
│        │                                            │
│  [4] ISLAND ARRIVAL ──→ Cabaña owner scans QR       │
│        │                 (occupancy updated)        │
│        │                                            │
│  [5] EXCURSIONS ──→ Each activity QR-tracked        │
│        │             (snorkel, molas, islands)      │
│        │                                            │
│  [6] ADMIN MONITORS ──→ Real-time dashboard         │
│        │                 (capacity, revenue, flow)  │
│        │                                            │
│  [7] CHECK-OUT ──→ QR exit, capacity released       │
│        │            blockchain invoice generated    │
│        │                                            │
│  [8] DISTRIBUTION ──→ Smart contract auto-pays      │
│        Cabaña 40% · Lancha 20% · 4x4 15%           │
│        Tour Op 10% · Congreso 10% · Platform 5%    │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## Zone Capacity Model

```
┌─ ISLAND CAPACITY ENGINE ────────────────────────────┐
│                                                      │
│  Zone 1 (Carti):     ████████████████░░░░  300/day  │
│  Zone 2 (Río Sidra): ████████████░░░░░░░░  200/day  │
│  Zone 3 (Playón):    ██████████░░░░░░░░░░  150/day  │
│  Zone 4 (Achutupu):  ████░░░░░░░░░░░░░░░░   80/day  │
│  Zone 5 (Ailitupu):  ██████░░░░░░░░░░░░░░  100/day  │
│  Zone 6 (Ustupu):    ██████████████████████ 500/day  │
│                                                      │
│  Total Guna Yala capacity: 1,330 tourists/day        │
│  Auto-close at 95% · Redirect to available zones     │
└──────────────────────────────────────────────────────┘
```

## API Design

```
# Zones & Availability
GET  /api/v1/nae/zones                      6 zones with occupancy
GET  /api/v1/nae/zone/{id}/availability     Calendar: cabañas, lanchas
GET  /api/v1/nae/capacity/{zone_id}         Real-time capacity

# Booking & Flow
POST /api/v1/nae/booking/create             Reserve: zone, cabaña, transport
POST /api/v1/nae/qr/scan                    QR scan (transport/boat/cabin/exit)

# Payments
POST /api/v1/nae/payment/wampum             WAMPUM payment (40/20/15/10/10/5)

# Dashboards
GET  /api/v1/nae/dashboard/{role}           Role dashboard data
GET  /api/v1/nae/analytics/congreso         Congreso General analytics

# Support
GET  /api/v1/nae/weather/maritime           Maritime weather and safety
GET  /api/v1/nae/molas/catalog              Mola artisan marketplace
```

## Offline Architecture

```
┌─ ISLAND (No Signal) ────────┐    ┌─ MAINLAND (Signal) ─────┐
│                              │    │                          │
│  IndexedDB (local store)     │    │  PostgreSQL (main DB)    │
│  QR Scanner (offline)        │    │  Redis (real-time)       │
│  Booking Cache               │    │  API Gateway             │
│  Calendar Cache              │    │                          │
│         │                    │    │                          │
│         └───── P2P Mesh ─────┼────┤  Sync on reconnect      │
│              (boat WiFi)     │    │                          │
└──────────────────────────────┘    └──────────────────────────┘
```

## Deployment

```
┌─────────────────────────────────────┐
│ NAE Guna Yala Cluster               │
│  ├── booking-engine (x2)            │
│  ├── capacity-control (x2, RT)      │
│  ├── qr-flow-tracker (x2, RT)      │
│  ├── wampum-gateway (x2)           │
│  ├── role-dashboard (x3)           │
│  ├── calendar-occupancy (x2)       │
│  ├── weather-maritime (x1)         │
│  ├── mola-marketplace (x1)         │
│  └── analytics (x1, TimescaleDB)   │
│                                     │
│ Edge Nodes                          │
│  ├── Carti embarcadero (WiFi hub)  │
│  ├── El Porvenir airport           │
│  ├── Playón Chico airport          │
│  └── Ustupu (Congreso)            │
└─────────────────────────────────────┘
```

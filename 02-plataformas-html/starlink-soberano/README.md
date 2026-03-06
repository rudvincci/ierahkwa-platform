# StarLink Soberano — Internet Satelital Soberano

> Plataforma WiFi satelital para venta de acceso a internet via captive portal + dashboard administrativo — Ierahkwa Ne Kanienke

## Resumen

**StarLink Soberano** es la plataforma de negocio WiFi satelital del ecosistema **Ierahkwa Ne Kanienke**, parte de **NEXUS Cosmos (Espacio & Satelital)**. Modelo de negocio: pagar 1 Starlink por ubicacion, multiplicar la senal con routers mesh, vender acceso WiFi por tiempo como en aeropuertos.

## Modelo de Negocio

```
1 Starlink ($50-100/mes) → Multiplicador de senal → Captive Portal → Venta por tiempo
                                                                        ↓
                                                              ROI 10x+ por ubicacion
```

## Planes de Acceso (WAMPUM)

| Plan | Duracion | Precio | Bandwidth |
|------|----------|--------|-----------|
| 1 Hora | 1h | 9.99 | 25 Mbps |
| 1 Dia | 24h | 24.99 | 50 Mbps |
| 1 Semana | 7d | 99.99 | 75 Mbps |
| 1 Mes | 30d | 249.99 | 100 Mbps |
| 6 Meses | 180d | 999.99 | 150 Mbps |
| 1 Ano | 365d | 1,499.99 | 200 Mbps |

## Componentes

### A) Captive Portal (Vista Publica)
- Pantalla de bienvenida "Internet Soberano"
- Seleccion de plan + pago WAMPUM
- Contador de tiempo restante
- Estado de conexion y velocidad

### B) Dashboard Administrador (Vista Privada)
- Metricas en tiempo real (usuarios, revenue, data)
- Gestion de hotspots (6 ubicaciones activas)
- Fleet Management (8+ kits Starlink)
- Revenue analytics (diario/semanal/mensual)
- Gestion de planes y precios

### C) Vigilancia Soberana
- Log de TODA conexion (IP, MAC, device, geo)
- Deteccion de busquedas sobre personas VIP
- Atabey AI: monitoreo 24/7 de personas protegidas
- 7 Agentes AI + alertas en tiempo real

## Backend Service

```
wifi-soberano (Puerto 3095)
├── POST /api/v1/wifi/connect       → Conectar sesion WiFi
├── GET  /api/v1/wifi/plans         → Planes disponibles
├── GET  /api/v1/wifi/session/status → Estado de sesion
├── POST /api/v1/wifi/payment/create → Pago WAMPUM
├── GET  /api/v1/wifi/admin/dashboard → Metricas admin
├── GET  /api/v1/wifi/admin/fleet    → Estado flota Starlink
├── GET  /api/v1/wifi/admin/revenue  → Analytics de revenue
└── GET  /api/v1/wifi/admin/vigilancia → Log vigilancia
```

## Flota Starlink Activa

| Kit | Modelo | Cuenta | Ubicacion | Estado |
|-----|--------|--------|-----------|--------|
| UT-GOMEZ-001 | Performance Gen 3 | Gomez | Tocumen, Panama | Online |
| UT-OMAR-001 | Starlink Mini | Omar | Chepo, Darien | Online |
| UT-SEGURA-001 | Performance Gen 1 | Segura | Darien | RMA |
| UT-MIKEKOL-001 | Standard | Mikekol | Fort Lauderdale | Online |
| UT-WILSON-001 | Performance Gen 1 | Wilson | Guna Yala | Online |
| UT-ERICK-001 | Mesh Node | Erick | Embera | Online |
| UT-FELIX-001 | Performance Gen 3 | Felix | Carti, San Blas | Online |
| UT-GARY-001 | Standard | Gary/Appel | Fort Lauderdale | Offline |

## Stack Tecnologico

### Frontend
- HTML5 + CSS3 + JavaScript vanilla
- Design System: `ierahkwa.css`
- 8 stores IndexedDB para offline
- PWA + Service Worker

### Backend
- Node.js 22 + Express 4.21
- PostgreSQL 16 + Redis 7
- WebSocket (dashboard real-time)
- JWT authentication

### Infraestructura
- Docker (Node 22 Alpine, tini PID 1)
- Kubernetes (2-10 replicas, HPA)
- Nginx reverse proxy + rate limiting
- Puerto: 3095

## Instalacion

```bash
# Clonar
git clone https://github.com/rudvincci/ierahkwa-platform.git
cd 02-plataformas-html/starlink-soberano

# Frontend (no requiere servidor)
open index.html

# Backend
cd ../../03-backend/wifi-soberano
npm install
cp .env.example .env
node server.js

# Docker
docker build -t wifi-soberano .
docker run -p 3095:3095 wifi-soberano
```

## Seguridad

- Encriptacion post-quantum (CRYSTALS-Kyber-768)
- 7 Agentes AI de vigilancia continua
- Atabey AI: proteccion VIP 24/7
- Rate limiting: 200 req/15min global
- Helmet + CORS + CSP headers
- JWT para sesiones autenticadas
- Zero dependencias externas en frontend

## NEXUS

Esta plataforma pertenece a **NEXUS Cosmos (Espacio & Satelital)** del ecosistema Ierahkwa.

## Licencia

Propiedad de Ierahkwa Ne Kanienke — Nacion Digital Soberana.

## Contacto

- **Proyecto**: [Ierahkwa Platform](https://github.com/rudvincci/ierahkwa-platform)
- **NEXUS**: NEXUS Cosmos (Espacio & Satelital)

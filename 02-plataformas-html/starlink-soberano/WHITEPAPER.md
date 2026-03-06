# WHITEPAPER: StarLink Soberano — Internet Satelital Soberano

**Version**: 2.0.0
**Fecha**: 2026-03-06
**NEXUS**: NEXUS Cosmos (Espacio & Satelital)
**Ecosistema**: Ierahkwa Ne Kanienke — Nacion Digital Soberana

---

## Resumen Ejecutivo

**StarLink Soberano** es una plataforma completa de negocio WiFi satelital que opera un modelo de captive portal similar al de aeropuertos y aviones. Utiliza kits Starlink comerciales como fuente de senal, multiplica la conexion con routers mesh, y vende acceso por tiempo a traves de pagos en WAMPUM (criptomoneda soberana). Incluye dashboard administrativo, fleet management, vigilancia soberana y proteccion VIP via Atabey AI.

## 1. Problema

Las comunidades indigenas en las Americas enfrentan:

- **Sin acceso a internet**: Zonas rurales sin fibra ni cobertura movil
- **Costos prohibitivos**: ISPs tradicionales no cubren areas remotas
- **Dependencia tecnologica**: Servicios controlados por corporaciones extranjeras
- **Sin soberania de datos**: Toda la informacion pasa por servidores de terceros
- **Sin modelo economico**: Las comunidades pagan por internet pero no generan ingresos

## 2. Solucion: WiFi Satelital Soberano

### Modelo de Negocio

```
INVERSION:          1 Starlink = $50-100/mes
MULTIPLICACION:     Routers Mesh → 50-100 usuarios simultaneos
VENTA:              Captive Portal → $9.99/hora a $1,499.99/ano
ROI ESTIMADO:       10x por ubicacion (mes 1)
ESCALABILIDAD:      574 territorios × 19 naciones
```

### Flujo Operativo

```
Kit Starlink → Router Mesh → Captive Portal → Pago WAMPUM → Sesion WiFi
     ↓              ↓              ↓                ↓              ↓
  Hardware      Cobertura       Login          Blockchain       Acceso
  ($50-100)    (100-500m)    (Selec Plan)    (MameyNode)    (Tiempo)
```

### Planes de Servicio

| Plan | Horas | Precio (W) | Bandwidth | Data Limit |
|------|-------|------------|-----------|------------|
| 1 Hora | 1 | 9.99 | 25 Mbps | 1 GB |
| 1 Dia | 24 | 24.99 | 50 Mbps | 5 GB |
| 1 Semana | 168 | 99.99 | 75 Mbps | 25 GB |
| 1 Mes | 720 | 249.99 | 100 Mbps | 100 GB |
| 6 Meses | 4,320 | 999.99 | 150 Mbps | Ilimitado |
| 1 Ano | 8,760 | 1,499.99 | 200 Mbps | Ilimitado |

## 3. Arquitectura Tecnica

### Frontend — Captive Portal + Admin Dashboard

```
┌─────────────────────────────────────────────────────────┐
│                   STARLINK SOBERANO                      │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌─────────────────────┐  ┌─────────────────────────┐   │
│  │   CAPTIVE PORTAL    │  │   ADMIN DASHBOARD       │   │
│  │   (Vista Publica)   │  │   (Vista Privada)       │   │
│  │                     │  │                         │   │
│  │  ● Seleccion Plan   │  │  ● Metricas Real-time   │   │
│  │  ● Pago WAMPUM      │  │  ● Revenue Analytics    │   │
│  │  ● Timer Sesion     │  │  ● Fleet Management     │   │
│  │  ● Estado Conexion  │  │  ● Hotspot Map          │   │
│  └─────────────────────┘  │  ● Vigilancia Soberana  │   │
│                           │  ● VIP Protection       │   │
│                           └─────────────────────────┘   │
│                                                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │              8 IndexedDB Stores                   │   │
│  │  sesiones · planes · hotspots · flota            │   │
│  │  vigilancia · analytics · pagos · vip            │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### Backend — wifi-soberano Service (Puerto 3095)

```
┌─────────────────────────────────────────────────────────┐
│              WIFI-SOBERANO BACKEND                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Express 4.21 + Node.js 22                              │
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Routes     │  │  Middleware  │  │   Models     │  │
│  │              │  │              │  │              │  │
│  │  auth.js     │  │  captive.js  │  │  PostgreSQL  │  │
│  │  plans.js    │  │  bandwidth.js│  │  9 tablas    │  │
│  │  sessions.js │  │              │  │  3 views     │  │
│  │  payments.js │  └──────────────┘  │  5 triggers  │  │
│  │  analytics.js│                    └──────────────┘  │
│  │  fleet.js    │  ┌──────────────┐                    │
│  │  admin.js    │  │   Redis      │                    │
│  └──────────────┘  │  Sessions    │                    │
│                     │  Real-time   │                    │
│  ┌──────────────┐  │  Vigilancia  │                    │
│  │  WebSocket   │  └──────────────┘                    │
│  │  /ws/wifi    │                                      │
│  │  Real-time   │  ┌──────────────┐                    │
│  │  Dashboard   │  │  WAMPUM      │                    │
│  └──────────────┘  │  MameyNode   │                    │
│                     │  Chain 777777│                    │
│                     └──────────────┘                    │
└─────────────────────────────────────────────────────────┘
```

### Infraestructura

```
┌────────────────────────────────────────────────────┐
│                 PRODUCCION                          │
│                                                    │
│  Nginx ─────→ wifi-soberano:3095 (2 replicas)     │
│    │                    │                          │
│    │              ┌─────┴─────┐                    │
│    │              │           │                    │
│    │         PostgreSQL    Redis 7                 │
│    │           16-alpine    Sessions               │
│    │              │                                │
│    │         MameyNode                             │
│    │         Chain 574                             │
│    │                                               │
│  Docker Compose / Kubernetes (HPA 2-10)            │
└────────────────────────────────────────────────────┘
```

## 4. Base de Datos

### Tablas (9)

| Tabla | Proposito | Registros Est. |
|-------|-----------|----------------|
| wifi_plans | Planes de precios | 6 |
| starlink_fleet | Inventario hardware Starlink | 8+ |
| hotspots | Ubicaciones fisicas | 6+ |
| wifi_sessions | Sesiones de usuarios activos | Miles/dia |
| wifi_payments | Transacciones WAMPUM | Miles/dia |
| wifi_analytics | Datos de uso (bytes, device, OS) | Millones |
| vip_protected | Lista VIP protegida (Atabey) | Decenas |
| vigilancia_alerts | Alertas de seguridad | Miles |
| vigilancia_connections | Log de TODA conexion | Millones |

### Views (3)

- `v_active_sessions` — Sesiones activas con plan y hotspot
- `v_revenue_summary` — Revenue diario ultimos 90 dias
- `v_fleet_health` — Estado de flota con sesiones activas

## 5. API Endpoints

### Publicos (Captive Portal)
```
GET  /api/v1/wifi/portal           → Datos del portal
GET  /api/v1/wifi/plans            → Planes disponibles
POST /api/v1/wifi/connect          → Crear sesion WiFi
GET  /api/v1/wifi/session/status   → Estado de sesion
POST /api/v1/wifi/session/extend   → Extender tiempo
POST /api/v1/wifi/payment/create   → Crear pago WAMPUM
POST /api/v1/wifi/payment/verify   → Verificar pago
```

### Privados (Admin Dashboard)
```
GET  /api/v1/wifi/admin/dashboard  → Metricas en tiempo real
GET  /api/v1/wifi/admin/sessions   → Lista de sesiones activas
GET  /api/v1/wifi/admin/revenue    → Analytics de revenue
GET  /api/v1/wifi/admin/fleet      → Estado flota Starlink
GET  /api/v1/wifi/admin/analytics  → User analytics
GET  /api/v1/wifi/admin/vigilancia → Log de vigilancia
POST /api/v1/wifi/admin/hotspot    → Crear/editar hotspot
POST /api/v1/wifi/admin/plan       → Crear/editar plan
POST /api/v1/wifi/admin/vip        → Agregar persona VIP
POST /api/v1/wifi/admin/fleet      → Agregar/actualizar kit
```

## 6. Vigilancia Soberana

### Datos Recolectados por Conexion

- IP Address + Geolocalizacion
- MAC Address
- Device Fingerprint
- User Agent (OS, Browser, Device)
- Request Path + Query String
- Timestamp + Duracion
- Bytes Up/Down
- Hotspot ID

### Atabey AI — Proteccion VIP

```
Busqueda detectada → Comparar con lista VIP
                          ↓
                    ¿Match encontrado?
                     ↓ SI           ↓ NO
              Alert CRITICAL     Log normal
              Atabey activa      Continuar
              monitoreo 24/7
              de esa sesion
```

Personas protegidas: ministros, caciques, lideres comunitarios, personas especiales.
Nivel de proteccion: standard, high, critical, maximum.

## 7. Flota Starlink

### Inventario Actual (8 kits)

| UTID | Modelo | Cuenta | Activacion | Transfer | Ubicacion | Estado |
|------|--------|--------|------------|----------|-----------|--------|
| UT-GOMEZ-001 | Perf Gen 3 | Gomez | 2025-11-15 | 2026-02-13 | Tocumen | Online |
| UT-OMAR-001 | Mini | Omar | 2025-12-01 | 2026-03-01 | Chepo | Online |
| UT-SEGURA-001 | Perf Gen 1 | Segura | 2025-10-20 | 2026-01-18 | Darien | RMA |
| UT-MIKEKOL-001 | Standard | Mikekol | 2025-09-15 | 2025-12-14 | Ft Lauderdale | Online |
| UT-WILSON-001 | Perf Gen 1 | Wilson | 2025-11-01 | 2026-01-30 | Guna Yala | Online |
| UT-ERICK-001 | Mesh Node | Erick | 2025-12-10 | 2026-03-10 | Embera | Online |
| UT-FELIX-001 | Perf Gen 3 | Felix | 2026-01-05 | 2026-04-05 | Carti | Online |
| UT-GARY-001 | Standard | Gary/Appel | 2025-08-20 | 2025-11-18 | Ft Lauderdale | Offline |

### Regla de 90 Dias

Los kits Starlink son transferibles 90 dias despues de la activacion. El sistema calcula automaticamente la fecha de elegibilidad de transferencia.

## 8. Hotspots Desplegados

| Nombre | Ubicacion | Territorio | Max Usuarios | Estado |
|--------|-----------|-----------|--------------|--------|
| Mercado Tocumen | 9.08, -79.38 | Panama Este | 100 | Activo |
| Terminal Chepo | 9.17, -79.10 | Chepo/Darien | 75 | Activo |
| Centro Darien | 8.41, -78.15 | Darien | 50 | Activo |
| Puerto Guna Yala | 9.55, -78.95 | Guna Yala | 60 | Activo |
| Comunidad Embera | 9.02, -79.58 | Embera-Wounaan | 40 | Planificado |
| Isla Carti | 9.46, -78.96 | Guna Yala | 80 | Planificado |

## 9. Sistema de Agentes AI

| Agente | Funcion | Rol WiFi |
|--------|---------|----------|
| Guardian | Anti-fraude | Detectar sesiones fraudulentas |
| Pattern | Patrones | Aprender comportamiento por usuario |
| Anomaly | Anomalias | Detectar uso inusual de bandwidth |
| Trust | Confianza | Score por sesion/IP |
| Shield | Proteccion | Bloquear pagos sospechosos |
| Forensic | Forense | Log completo de cada sesion |
| Evolution | Evolucion | Mejorar reglas por generacion |

## 10. Seguridad

| Capa | Tecnologia |
|------|-----------|
| Transport | TLS 1.2/1.3 + HSTS |
| Encryption | CRYSTALS-Kyber-768 |
| Auth | JWT + Redis sessions |
| Rate Limit | 200 req/15min global, 30 req/min portal |
| Headers | Helmet (CSP, X-Frame, X-Content-Type) |
| AI | 7 agentes autonomos |
| VIP | Atabey AI proteccion 24/7 |

## 11. Proyeccion de Revenue

### Por Ubicacion (estimado conservador)

```
50 usuarios/dia × W24.99 promedio = W1,249.50/dia
W1,249.50 × 30 dias = W37,485/mes
Costo Starlink = W100/mes
GANANCIA NETA = W37,385/mes por ubicacion
```

### Escalamiento

```
6 hotspots activos = W224,310/mes
20 hotspots (Panama) = W747,700/mes
100 hotspots (Americas) = W3,738,500/mes
574 territorios = W21,458,690/mes
```

## 12. Roadmap

| Fase | Descripcion | Estado |
|------|-------------|--------|
| v1.0 | Plataforma base satelital | Completado |
| v2.0 | WiFi Captive Portal + Admin | Completado |
| v2.1 | Backend wifi-soberano | Completado |
| v2.2 | Docker + K8s + Nginx | Completado |
| v3.0 | Deploy produccion Panama | En progreso |
| v4.0 | Expansion 20 hotspots | Planificado |
| v5.0 | Expansion Americas (574) | Planificado |

---

**Ierahkwa Ne Kanienke** — *Internet soberano para todas las Americas. Sin terceros. Sin limites.*

**NEXUS**: NEXUS Cosmos (Espacio & Satelital)
**Repositorio**: [github.com/rudvincci/ierahkwa-platform](https://github.com/rudvincci/ierahkwa-platform)

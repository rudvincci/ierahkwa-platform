# Vigilancia Soberana — SIEM / Monitoreo de Seguridad

> Sistema SIEM soberano con ingestion de eventos en tiempo real, motor de reglas de alerta y seguimiento de cumplimiento (OWASP, PCI-DSS, HIPAA).

## Descripcion

Vigilancia Soberana es el sistema de Gestion de Informacion y Eventos de Seguridad (SIEM) de la plataforma Ierahkwa Ne Kanienke. Proporciona capacidades completas de ingestion, almacenamiento y analisis de eventos de seguridad, combinadas con un motor de reglas de alerta configurable y seguimiento de cumplimiento normativo.

El servicio permite ingestar eventos de seguridad clasificados por severidad (critical, high, medium, low, info), fuente, categoria y metadatos arbitrarios. Los eventos se almacenan en PostgreSQL con un buffer circular de 10,000 eventos maximos. Un motor de reglas evalua cada evento entrante contra reglas configurables que pueden disparar alertas basadas en condiciones como severidad, fuente, mensaje o categoria, con soporte para umbrales acumulativos.

El modulo de cumplimiento incluye frameworks pre-configurados de OWASP Top 10:2025, PCI DSS v4.0 y HIPAA Security Rule, cada uno con controles individuales y puntuaciones de conformidad. El dashboard agrega datos de severidad, fuentes principales, alertas recientes, eventos por hora y desglose por categoria para una vision operativa completa.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **Base de Datos**: PostgreSQL 16 (driver `pg` 8.x)
- **Compresion**: compression 1.7.x
- **UUID**: uuid 9.x
- **Seguridad**: Helmet 7.x, CORS configurado, middleware de seguridad compartido
- **Puerto**: 3091 (configurable via `PORT`)

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con conteo de eventos, alertas y uptime |
| **Eventos** | | |
| POST | /api/events | Ingestar eventos de seguridad (individual o batch) |
| GET | /api/events | Consultar eventos con filtros (severity, source, timeFrom, timeTo, category, search, limit, offset) |
| **Dashboard** | | |
| GET | /api/dashboard | Resumen completo: severidades, top fuentes, alertas recientes, eventos/hora, categorias, cumplimiento |
| **Reglas de Alerta** | | |
| POST | /api/alerts | Crear regla de alerta (severity_equals, source_contains, message_contains, category_equals) |
| GET | /api/alerts | Listar reglas de alerta con alertas disparadas recientes |
| **Cumplimiento** | | |
| GET | /api/compliance | Estado de cumplimiento global o por framework (OWASP, PCI-DSS, HIPAA) |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3091 |
| DATABASE_URL | Cadena de conexion PostgreSQL | postgresql://localhost:5432/vigilancia_soberana |

## Instalacion

```bash
npm install
npm start
```

## Docker

```bash
docker build -t vigilancia-soberana .
docker run -p 3091:3091 vigilancia-soberana
```

## Esquema de Base de Datos

### Tablas

| Tabla | Descripcion |
|-------|-------------|
| `events` | Eventos de seguridad con severidad, fuente, categoria, metadata, IP, user-agent |
| `alert_rules` | Reglas de alerta configurables con condicion, valor, umbral, accion |
| `triggered_alerts` | Historial de alertas disparadas con referencia al evento y regla |
| `compliance_frameworks` | Frameworks de cumplimiento (OWASP, PCI-DSS, HIPAA) |
| `compliance_controls` | Controles individuales por framework con estado y puntuacion |
| `counters` | Contadores de sistema (totalIngested) |

### Indices

- `idx_events_severity` -- Busqueda por severidad
- `idx_events_source` -- Busqueda por fuente
- `idx_events_category` -- Busqueda por categoria
- `idx_events_timestamp` -- Ordenamiento cronologico descendente
- `idx_triggered_alerts_triggered_at` -- Alertas por fecha

## Motor de Reglas de Alerta

Las reglas soportan 4 tipos de condicion:

| Condicion | Descripcion | Ejemplo |
|-----------|-------------|---------|
| `severity_equals` | Coincidencia exacta de severidad | `value: "critical"` |
| `source_contains` | Fuente contiene texto | `value: "firewall"` |
| `message_contains` | Mensaje contiene texto | `value: "brute force"` |
| `category_equals` | Coincidencia exacta de categoria | `value: "intrusion"` |

Cada regla tiene un `threshold` (umbral) que indica cuantas coincidencias se necesitan antes de disparar la alerta. Las acciones disponibles incluyen `log`, `notify`, `block`.

## Frameworks de Cumplimiento

### OWASP Top 10:2025
10 controles (A01-A10): Broken Access Control, Cryptographic Failures, Injection, Insecure Design, Security Misconfiguration, Vulnerable Components, Authentication Failures, Software Integrity, Logging & Monitoring, SSRF.

### PCI DSS v4.0
12 controles (R1-R12): Network Security, Secure Config, Stored Data Protection, Encrypt Transmission, Malware, Secure Software, Restrict Access, Auth, Physical Access, Log & Monitor, Test Security, Security Policies.

### HIPAA Security Rule
13 controles (AD1-AD6, PH1-PH3, TE1-TE4): Security Management, Assigned Responsibility, Workforce Security, Access Management, Security Training, Incident Procedures, Facility Access, Workstation, Device Controls, Access Control, Audit, Integrity, Transmission Security.

## Arquitectura

```
Fuentes de Eventos --> [POST /api/events] --> [Vigilancia Soberana :3091]
                                                       |
                                              +--------+--------+
                                              |                 |
                                         [PostgreSQL]    [Motor de Reglas]
                                              |                 |
                                    +----+----+----+     Evaluar condiciones
                                    |    |    |    |         |
                                 Events Rules Alerts  Disparar alertas
                                              |         |
                                         Compliance  triggered_alerts
                                              |
                                    +----+----+----+
                                    |    |         |
                                 OWASP PCI-DSS  HIPAA

Dashboard: GET /api/dashboard
     |
     +--> Severidad (critico/alto/medio/bajo/info)
     +--> Top 10 fuentes
     +--> Alertas recientes (ultimas 20)
     +--> Eventos por hora (ultimas 24h)
     +--> Desglose por categoria
     +--> Cumplimiento (OWASP/PCI/HIPAA scores)
```

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital

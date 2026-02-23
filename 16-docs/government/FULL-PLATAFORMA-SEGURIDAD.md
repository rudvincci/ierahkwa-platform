# Full plataforma de seguridad — ATABEY

Una sola página: **atabey-platform.html**. Todo junto, todo propio.

---

## Implementado (visible y operativo)

| Módulo | Descripción | API / Fuente |
|--------|-------------|--------------|
| **Vista Global** | Estado conjunto: Fortress, Quantum, Telecom, AI, Backup, Face, Watchlist | `GET /api/v1/atabey/status` |
| **AI · Servicios** | Estado ATABEY + enlaces a AI Platform, Support AI, ATABEY Dashboard, App Studio, App Builder | Datos de atabey/status |
| **Seguridad · Fortress** | Buscar persona por foto (Face propio), lista Watchlist | `POST /api/v1/face/search`, `GET /api/v1/watchlist` |
| **Quantum** | Estado post-cuántica (Kyber, Dilithium) | atabey/status → conjunto |
| **Telecom** | Estado satélite, VoIP, móvil | `GET /api/v1/telecom/status` |
| **Vigilancia · Cámaras** | Categorías, regiones, grid de cámaras, modal de stream | `data/webcams-registry.json`, `webcam-view.html` |
| **Firewall · Ghost** | Estado Ghost Mode, nodos, consenso, enlace a Security Fortress | `GET /api/v1/ghost/status` |
| **Eventos** | Log unificado: alertas watchlist + vigilancia | `GET /api/v1/security/events` |
| **Backup** | Estado backup, enlace a Backup Department | atabey/status + `backup-department.html` |
| **Cumplimiento** | Emergencias activas, watchlist count | atabey/status |

---

## APIs de seguridad disponibles (backend)

- `GET /api/v1/atabey/status` — Estado unificado
- `GET /api/v1/security/conjunto` — Fortress + AI + Quantum
- `GET /api/v1/security/vigilance` — Vigilancia ATABEY, último scan, acuerdos
- `GET /api/v1/security/nodes` — Lista de nodos de seguridad
- `GET /api/v1/security/events` — Log unificado (watchlist + vigilancia)
- `GET /api/v1/ghost/status` — Ghost Mode
- `GET/POST /api/v1/watchlist` — Watchlist, categorías
- `GET /api/v1/watchlist/alerts` — Alertas watchlist
- `GET /api/v1/face/status`, `POST /api/v1/face/search` — Face propio
- `GET /api/v1/emergencies/alerts` — Emergencias
- `GET /api/v1/telecom/status` — Telecom

---

## Qué más se puede implementar (roadmap)

1. **Nodos (tab)** — Tab "Nodos" con lista de endpoints (security/nodes) y estado de cada uno (verde/rojo).
2. **Inteligencia de amenazas** — Lista propia de IPs/dominios bloqueados, indicadores (IOCs). API `GET/POST /api/v1/security/threats`, tab "Inteligencia".
3. **Sesiones activas** — Quién está conectado (JWT/sesiones), revocar. API `GET /api/v1/security/sessions`.
4. **Anomaly AI en UI** — Ya existe `GET /api/v1/security/anomaly/status` y `/anomalies`; añadir tarjetas en Seguridad o tab "Anomalías".
5. **Reglas firewall** — Ver/editar reglas Ghost (si el módulo ghost expone CRUD).
6. **Reportes de seguridad** — Exportar eventos a PDF/JSON, informe diario/semanal automático.
7. **Integración KMS** — Estado de claves (rotación, uso) en Vista Global o tab "Cripto".
8. **Drill-down vigilancia** — En Eventos, al hacer clic en un evento de vigilancia ver detalle (acuerdos NDCA, nivel, etc.).

---

## Servicio de Inteligencia — Doctrina (todo propio)

Funciones tipo agencia de inteligencia/seguridad y dónde se cubren en la plataforma:

| Función tipo “servicio de inteligencia” | En la plataforma (todo propio) |
|----------------------------------------|--------------------------------|
| **Inteligencia / análisis** | ATABEY, AI, vigilancia, log de eventos, alertas watchlist |
| **Contrainteligencia / amenazas internas** | Watchlist, registro personas interés público, face recognition propio, eventos unificados |
| **Seguridad perimetral / operativa** | Fortress, Ghost, firewall, Quantum, Telecom |
| **Protección de personas / eventos** | Watchlist, alertas, emergencias, cumplimiento |

Todo soberano: sin dependencias de agencias externas ni APIs de terceros.

---

## Principio

**Ellos no nos encuentran. Nosotros sí los encontramos.**  
Todo propio: sin Regula, sin PimEyes, sin AWS/Google/Twilio. Infraestructura y código soberanos.

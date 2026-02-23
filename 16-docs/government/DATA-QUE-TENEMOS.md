# Data que tenemos — inventario único

**Sovereign Government of Ierahkwa Ne Kanienke**  
Un solo lugar donde está **toda la data** del proyecto: archivos, carpetas y APIs que la exponen. Para conectar banco/bonos ver [CONECTAR-DATA-BANCO-Y-BONOS.md](CONECTAR-DATA-BANCO-Y-BONOS.md).

---

## 1. Data en el Node (`RuddieSolution/node/data`)

Origen único de verdad para banca, VIP, registros y estado. El Node (8545) lee/escribe aquí.

### Banco y BDET
| Data | Ruta | API / uso |
|------|------|-----------|
| Registro de bancos | `node/data/bank-registry.json` | `GET /api/v1/bdet/bank-registry` |
| Cuentas gobierno | `node/data/bdet-bank/gov-accounts.json` | Government Banking, `GET /api/v1/bdet-server/api/gov-accounts` |
| Transacciones gobierno | `node/data/bdet-bank/gov-transactions.json` | `GET /api/v1/bdet-server/api/gov-transactions` |
| Solicitudes banco (SIP) | `node/data/bank-submissions/*.json` | Submissions BDET |
| Identificadores banco | `node/data/bank-identifiers.json` | Open banking / identificadores |

### VIP y bonos
| Data | Ruta | API / uso |
|------|------|-----------|
| Transacciones VIP (incl. bonos) | `node/data/vip-transactions.json` | `GET/POST /api/v1/vip/transactions`, `/api/v1/vip/stats`, `/api/v1/vip/report` |

### Estado y reportes
| Data | Ruta | Uso |
|------|------|-----|
| Estado blockchain | `node/data/blockchain-state.json` | Chain, bloques |
| Estado final sistema | `node/data/estado-final-sistema.json` | Producción / verificación |
| Resumen soberano | `node/data/resumen-soberano.json` (si existe) | API soberanía |
| Auditoría global | `node/data/auditoria-global-plataformas.json` | Auditorías |
| Verificación 100% prod | `node/data/verificacion-100-production.json` | Checklist |
| Backup última ejecución | `node/data/backup-last-run.json` | Backups |
| Reporte servicios 1-a-1 | `node/data/reporte-servicios-uno-a-uno.json` | Reportes |
| Record global plataforma | `node/data/record-global-platforma-completa.json` | Historial |
| Reporte valor proyecciones | `node/data/reporte-valor-proyecciones.json` | Proyecciones |

### Soberanía y configuración (JSON en raíz de data)
| Data | Ruta | API / uso |
|------|------|-----------|
| Tokens plataforma | `node/data/platform-tokens.json` | Tokens IGT |
| Ecosistema Futurehead | `node/data/ecosistema-futurehead.json` | `GET /api/v1/sovereignty/ecosistema-futurehead` |
| Whitepaper Futurehead | `node/data/whitepaper-futurehead.json` (si existe) | `GET /api/v1/sovereignty/whitepaper-futurehead` |
| Plan implementación | `node/data/plan-implementacion-futurehead.json` (si existe) | `GET /api/v1/sovereignty/plan-implementacion` |
| Beneficios empleados | `node/data/beneficios-empleados.json` | `GET /api/v1/sovereignty/beneficios-empleados` |
| Ofertas corporativas | `node/data/ofertas-corporativas.json` (si existe) | `GET /api/v1/sovereignty/ofertas-corporativas` |
| Ciberseguridad 101 | `node/data/ciberseguridad-101.json` | Contenido referencia |
| Cloud / endpoint security | `node/data/cloud-security-soberano.json`, `endpoint-security-soberano.json` | Referencia |

### Carpetas de data (por dominio)
| Carpeta | Contenido típico |
|---------|------------------|
| `node/data/accounting/` | bank-accounts, parties, companies, products, transactions |
| `node/data/ai-hub/` | ATABEY, conversaciones, tareas, workers |
| `node/data/bdet-bank/` | gov-accounts, gov-transactions (y otros según módulo) |
| `node/data/casino/` | plataforma-mundial, apuestas-deportivas-algoritmos, promo-codes, redeemed |
| `node/data/central-banks/` | Datos bancos centrales |
| `node/data/compliance-watch/` | pep, sanctions, checks-log |
| `node/data/estaty/` | properties, agents, projects |
| `node/data/gaming/` | leaderboards, purchases, games-catalog |
| `node/data/hospital-management/` | patients, doctors, appointments, billing, medical-records |
| `node/data/infrastructure-engine/` | pki-certs, firewall-rules, swift-messages, dns, hsm-keys, vpn-clients |
| `node/data/settlements/` | STL-*.json (liquidaciones) |
| `node/data/departments/` | Por departamento |
| `node/data/atabey-nerve-center/` | Estado ATABEY |
| `node/data/evidence-intake/` | Evidencia |
| `node/data/land-assets-registry.json` | Registro activos tierra |
| `node/data/agriculture-bhbk-*.json` | Cuentas y préstamos agricultura BHBK |
| `node/data/americas-caribbean-products.json` | Productos Américas/Caribe |
| `node/data/audit.log` | Log de auditoría |

---

## 2. Data en la platform (`RuddieSolution/platform/data`)

Usada por el frontend y por las APIs del Node que leen desde `platform/data` (p. ej. `/api/platform/links`).

| Data | Ruta | API / uso |
|------|------|-----------|
| Enlaces plataformas | `platform/data/platform-links.json` | `GET /api/platform/links` |
| Departamentos gobierno | `platform/data/government-departments.json` | `GET /api/platform/departments` |
| Texto oficial plataformas | `platform/data/TEXTO-OFICIAL-PLATAFORMAS.json` | Contenido oficial |
| Landing info | `platform/data/platform-landing-info.json` | Landings por plataforma |
| Servicios comerciales (renta) | `platform/data/commercial-services-rental.json`, `commercial-services-monthly.json`, `servicios-comerciales-renta.json` | Rentas, mensualidades |
| Commerce types global | `platform/data/commerce-types-global-rental.json` | Tipos comercio |
| Estado soberanía | `platform/data/sovereignty-status.json` | Estado soberano |
| Dashboards | `platform/data/platform-dashboards.json` | Mapeo dashboards |
| Categorías | `platform/data/platform-category-map.json` | Categorías plataforma |
| Módulos por plataforma | `platform/data/platform-module-map.json` | Módulos/docs por HTML |
| Mensajes bienvenida | `platform/data/platform-welcome-messages.json` | Welcome por sección |
| Nombres indígenas | `platform/data/platforms-indigenous-names.json`, `lenguas-originarias.json` | Nombres y lenguas |
| Red comunicación | `platform/data/communication-network.json` | Red y enlaces |
| Regiones Américas | `platform/data/americas-regions.json` | Regiones |
| Idiomas ATABEY | `platform/data/atabey-languages.json` | Idiomas |
| Otros | `glosario.json`, `faq-soberano.json`, `principios.json`, `historias-ancestrales.json`, `madre-tierra.json`, `frases-clave.json`, `calendario-tortuga.json`, `webcams-registry.json`, `tenants.json` | Contenido y config |

---

## 3. Resumen por “qué quiero usar”

- **Banco / BDET / bancos centrales:** `node/data/bank-registry.json`, `node/data/bdet-bank/`, APIs `/api/v1/bdet/*`, `/api/v1/bdet-server/*`. Ver [CONECTAR-DATA-BANCO-Y-BONOS.md](CONECTAR-DATA-BANCO-Y-BONOS.md).
- **Bonos (bonds):** Dentro de `node/data/vip-transactions.json` (`assets.bonds`); APIs `/api/v1/vip/transactions`, `/api/v1/vip/stats`, `/api/v1/vip/report`. Ver [CONECTAR-DATA-BANCO-Y-BONOS.md](CONECTAR-DATA-BANCO-Y-BONOS.md).
- **Plataformas y enlaces:** `platform/data/platform-links.json`, `government-departments.json` → `/api/platform/links`, `/api/platform/departments`, `/api/platform/overview`.
- **Estado producción / verificaciones:** `node/data/estado-final-sistema.json`, `verificacion-100-production.json`, `record-global-*.json`, `auditoria-global-plataformas.json`.
- **Soberanía y Futurehead:** `node/data/ecosistema-futurehead.json`, `beneficios-empleados.json`, `sovereignty-status` (en platform/data), APIs bajo `/api/v1/sovereignty/`.

---

## 4. Regla: no duplicar

- **Inventario de “qué data tenemos”:** este documento.
- **Cómo conectar banco y bonos:** [CONECTAR-DATA-BANCO-Y-BONOS.md](CONECTAR-DATA-BANCO-Y-BONOS.md).
- **Estructura general del proyecto (carpetas, scripts, puertos):** [INDICE-COMPLETO-PROYECTO-SOBERANOS.md](INDICE-COMPLETO-PROYECTO-SOBERANOS.md).

*Última actualización: febrero 2026.*

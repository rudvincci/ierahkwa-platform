# 📋 REPORTE — LINKS Y BOTONES DE LA PLATAFORMA

Inventario de enlaces, botones y version-badges para **prender/apagar, ordenar, mover y editar** desde Admin → 🔗 LINKS Y BOTONES.

---

## 1. VERSION BADGES (header, `platform/index.html`)

| # | ID | Label | URL | Tipo |
|---|-----|-------|-----|------|
| 1 | vb-gov | 🏛️ GOV | /platform/government-portal.html | href |
| 2 | vb-admin | ⚙️ ADMIN | /platform/admin.html | href |
| 3 | vb-health | 💚 HEALTH | /platform/health-dashboard.html | href |
| 4 | vb-mega | 📊 MEGA | /mega-dashboard.html | href |
| 5 | vb-live | 🔴 LIVE | /live-connect.html | href |
| 6 | vb-ai | 🤖 AI 24/7 | /platform/support-ai.html | href |
| 7 | vb-monitor | 📊 MONITOR | /platform/monitor.html | href |
| 8 | vb-chat | 🔐 CHAT | /platform/secure-chat.html | href |
| 9 | vb-alerts | 🔔 ALERTS | /platform/notifications.html | href |
| 10 | vb-bank | 🏦 BANK | /bdet-bank | href |
| 11 | vb-ruddie | 👑 RUDDIE | /leader-control | href |
| 12 | vb-familia | 👨‍👩‍👧‍👦 FAMILIA | /IERAHKWA-PLATFORM-DEPLOY/family-system/family-portal.html | href |
| 13 | vb-video | 📹 VIDEO | /platform/video-call.html | href |
| 14 | vb-config | ⚙️ CONFIG | /platform/settings.html | href |
| 15 | vb-fortress | 🛡️ FORTRESS | /platform/security-fortress.html | href |
| 16 | vb-soberania | 📜 SOBERANÍA | /platform/sovereignty-education.html | href |
| 17 | vb-siis | 🌐 SIIS | /platform/siis-settlement.html | href |
| 18 | vb-4banks | 🏦 4 BANKS | /central-banks | href |
| 19 | vb-futurehead | 🚀 FUTUREHEAD | /platform/futurehead-group.html | href |
| 20 | vb-deudas | 💰 DEUDAS | /platform/debt-collection.html | href |
| 21 | vb-backup | 🔄 BACKUP | /platform/backup-department.html | href |
| 22 | vb-103depts | 🏛️ 103 DEPTS | /departments | href |
| 23 | vb-launchpad | 🚀 LAUNCHPAD | /platform/citizen-launchpad.html | href |
| 24 | vb-activity | 📊 ACTIVITY | /platform/contribution-graph.html | href |

*Nota: `V1.0` (id: cfgVersion) es un `<span>`, no enlace.*

---

## 2. BOTONES "📊 Open X Dashboard"

| # | ID | Label | URL (window.open) | Sección |
|---|-----|-------|-------------------|---------|
| 1 | dash-banking | 📊 Open Banking Dashboard | /bdet-bank | BANKING & FINANCE |
| 2 | dash-commerce | 📊 Open Commerce Dashboard | /commerce-business-dashboard.html | COMMERCE & BUSINESS |
| 3 | dash-gaming | 📊 Open Gaming Dashboard | /gaming | GAMING |
| 4 | dash-documents | 📊 Open Documents Dashboard | /documents | DOCUMENT MANAGEMENT |
| 5 | dash-projects | 📊 Open Projects Dashboard | /project-hub | PROJECT & MEETING |
| 6 | dash-hr | 📊 Open HR & Office Dashboard | /spike-office | HR & OFFICE |
| 7 | dash-social | 📊 Open Social Media Dashboard | /social-media | SOCIAL MEDIA |
| 8 | dash-education | 📊 Open Education Dashboard | /smartschool | EDUCATION & TOOLS |
| 9 | dash-legal | 📊 Open Legal Dashboard | /platform | LEGAL |
| 10 | dash-departments | 📊 Open Departments Dashboard | /departments | SOVEREIGN DEPARTMENTS |
| 11 | dash-government | 📊 Open Government Dashboard | /platform/government-portal.html | GOVERNMENT OPERATIONS |
| 12 | dash-tokens | 📊 Open Tokens Dashboard | /platform/blockchain-platform.html | 103 IGT TOKENS |
| 13 | dash-platform | 📊 Open Platform Dashboard | /platform | PLATFORM SUMMARY |

---

## 3. HEADER NAV (`config.json` → headerNav)

| # | ID | Label | href | Tipo |
|---|-----|-------|------|------|
| 1 | hn-gov | 🏛️ GOV | /platform/government-portal.html | href |
| 2 | hn-admin | ⚙️ ADMIN | /platform/admin.html | href |
| 3 | hn-bank | 🏦 BANK | /bdet-bank | href |
| 4 | hn-blockchain | ⛓️ BLOCKCHAIN | /platform/blockchain-platform.html | href |
| 5 | hn-gaming | 🎰 GAMING | /platform/gaming-platform.html | href |
| 6 | hn-social | 📱 SOCIAL | /platform/social-media.html | href |
| 7 | hn-ai | 🤖 AI | /platform/ai-platform.html | href |
| 8 | hn-quantum | ⚛️ QUANTUM | /platform/quantum-platform.html | href |
| 9 | hn-education | 🎓 EDUCATION | /platform/education-platform.html | href |
| 10 | hn-health | 🏥 HEALTH | /platform/health-platform.html | href |
| 11 | hn-insurance | 🛡️ INSURANCE | /platform/insurance-platform.html | href |
| 12 | hn-services | 🛠️ SERVICES | /platform/services-platform.html | href |
| 13 | hn-appstudio | 📲 APP STUDIO | /platform/app-studio.html | href |
| 14 | hn-ruddie | 👑 RUDDIE | /leader-control | href |
| 15 | hn-security | 🛡️ SECURITY | /platform/security-fortress.html | href |

---

## 4. QUICK ACTIONS (`config.json` → quickActions)

| # | ID | platformKey | Label | Tipo |
|---|-----|-------------|-------|------|
| 1 | qa-tradex | tradex | TRADEX | openPlatform |
| 2 | qa-net10 | net10 | NET10 DEFI | openPlatform |
| 3 | qa-farmfactory | farmfactory | FARMFACTORY | openPlatform |
| 4 | qa-vip | vip | VIP TRANSACTIONS | openPlatform |
| 5 | qa-casino | casino | CASINO | openPlatform |
| 6 | qa-social | social | SOCIAL | openPlatform |
| 7 | qa-lotto | lotto | LOTTO | openPlatform |
| 8 | qa-shop | shop | SHOP | openPlatform |
| 9 | qa-spikeoffice | spikeoffice | SPIKE OFFICE | openPlatform |
| 10 | qa-rnbcal | rnbcal | RnBCAL | openPlatform |
| 11 | qa-appbuilder | appbuilder | APPBUILDER | openPlatform |

---

## 5. CONFIG.SERVICES (`config.json` → services)

Cada clave es usada por `openPlatform(key)` o por URLs directas. Tabla resumida:

| # | key | URL |
|---|-----|-----|
| 1 | gov | /platform/government-portal.html |
| 2 | admin | /platform/admin.html |
| 3 | bank | /bdet-bank |
| 4 | blockchain | /platform/blockchain-platform.html |
| 5 | gaming | /platform/gaming-platform.html |
| 6 | social | /platform/social-media.html |
| 7 | ai | /platform/ai-platform.html |
| 8 | quantum | /platform/quantum-platform.html |
| 9 | education | /platform/education-platform.html |
| 10 | health | /platform/health-platform.html |
| 11 | insurance | /platform/insurance-platform.html |
| 12 | services | /platform/services-platform.html |
| 13 | appstudio | /platform/app-studio.html |
| 14 | security | /platform/security-fortress.html |
| 15 | leader | /leader-control |
| 16 | globalbank | /bdet-bank |
| 17 | tradex | /platform/blockchain-platform.html#tradex |
| 18 | net10 | /platform/blockchain-platform.html#net10 |
| 19 | farmfactory | /platform/blockchain-platform.html#farming |
| 20 | cryptohost | /platform/blockchain-platform.html#cryptohost |
| 21 | banking | /bdet-bank |
| 22 | tokens | /platform/blockchain-platform.html#tokens |
| 23 | casino | /platform/casino.html |
| 24 | lotto | /platform/lotto.html |
| 25 | raffle | /platform/raffle.html |
| 26 | vip | /vip-transactions |
| 27 | videocall | /platform/social-platform.html#video |
| 28 | securechat | /platform/social-platform.html#chat |
| 29 | chat | /chat |
| 30 | shop | /ierahkwa-shop/public/index.html |
| 31 | pos | /ierahkwa-shop/public/pos/index.html |
| 32 | inventory | /ierahkwa-shop/public/inventory/index.html |
| 33 | crm | /pos-system/public/crm/index.html |
| 34 | rnbcal | http://localhost:5055 |
| 35 | appbuilder | http://localhost:5060 |
| 36 | spikeoffice | http://localhost:5056 |
| 37 | advocate | http://localhost:3010 |
| 38 | school-node | http://localhost:8545 |
| 39 | dao | /docs/ANALISIS-DAO-WIDGET-CODECANYON.html |
| 40 | forex | /forex |
| 41 | wallet | /wallet |
| 42 | monetary | /ierahkwa-shop/public/monetary/index.html |
| 43 | global-service | /ierahkwa-shop/public/global-banking/index.html |
| 44 | clearhouse | / |
| 45 | idofactory | http://localhost:5097 |
| 46 | documentflow | /DocumentFlow/index.html |
| 47 | esignature | /ESignature/index.html |
| 48 | projecthub | http://localhost:7070 |
| 49 | meetinghub | http://localhost:7071 |
| 50 | images | /image-upload/public/index.html |
| 51 | portal | /ierahkwa-shop/public/portal/index.html |

---

## 6. PLATFORM-CARDS / NODE-CARDS (openPlatform)

Claves usadas en `onclick="openPlatform('key')"` en node-cards, platform-cards y action-btns:

**Node-cards:** global-service, cryptohost, clearhouse, tradex, net10, tokens, quantum, ai, security, erp, geocoder, contributions, spikeoffice, farmfactory, ghost, vip-transactions, banking, animstorm, invoicer, emailstudio.

**Action-btns (GAMING + accion-rapida):** tradex, cryptohost, shop, tokens, casino, social, lotto, raffle, spikeoffice, rnbcal, appbuilder, dao, school-node, net10.

**Platform-cards por sección:**
- Banking: tradex, farmfactory, vip, cryptohost, tokens, forex, banking, monetary, net10
- Commerce: shop, pos, inventory, crm, rnbcal
- Gaming: casino, lotto, raffle, sports-betting
- Documents: documents, esignature, outlookextractor
- Projects: projecthub, meetinghub
- HR & Office: spikeoffice
- Social: chat, social, images, portal, community
- Education: school, school-node, appbuilder, dao, idofactory
- Legal: advocate
- Departments: (links directos window.location.href / window.open: central-banks, siis, debt-collection, sovereignty, futurehead, mamey-futures, bitcoin-hemp, atm-manufacturing, backup, bdet-bank, security, leader-control)
- Government: assettracker, audittrail, budgetcontrol, citizencrm, contractmanager, datahub, digitalvault, formbuilder, notifyhub, procurementhub, reportengine, servicedesk, taxauthority, votingsystem, workflowengine, biometrics

**Objeto `platforms` en JS:** incluye todas las keys anteriores y más (ver `index.html` líneas 1887–2072).

---

## 7. RESUMEN POR TIPO

| Tipo | Cantidad aprox. | Origen |
|------|-----------------|--------|
| version-badges | 24 | index.html header |
| open-dashboard | 13 | index.html section headers |
| headerNav | 15 | config.json |
| quickActions | 11 | config.json |
| config.services | 51 | config.json |
| openPlatform (platforms) | 120+ | index.html `const platforms` |

---

## 8. CÓMO SE USA EN ADMIN

En **Admin → 🔗 LINKS Y BOTONES** puedes:

- **Prender/apagar:** checkbox `enabled` por ítem.
- **Ordenar:** campo `order` numérico; botones ↑ ↓.
- **Mover:** cambiar `section` (version-badges | dashboard | headerNav | quickActions | services | cards).
- **Editar:** label, URL (o platformKey si es openPlatform).

La fuente de verdad se guarda en:

- `localStorage.ierahkwa_platform_links` (JSON array), o
- `platform/data/platform-links.json` (carga inicial / reset).

`platform/index.html` puede leer `ierahkwa_platform_links` al cargar y aplicar `enabled`, `order` y `url` a los elementos correspondientes (fase opcional de integración).

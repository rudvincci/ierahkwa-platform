# REPORTE TESTING GLOBAL — PLATAFORMA IERAHKWA
**Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister**

**Fecha:** 2026-02-05T14:52:55.381Z
**Base URL:** http://localhost:8545

### Significado de estados
| Estado (inglés) | Significado |
|-----------------|-------------|
| **down** | Caído: el servicio no responde o no está encendido |
| **healthy** | OK: el servicio responde correctamente |
| **unhealthy** | No saludable: responde con error (4xx/5xx) |

---

## 1. RESUMEN EJECUTIVO

| Métrica | Valor |
|--------|-------|
| Total servicios/páginas probados | 206 |
| ✓ Saludables | 23 |
| ✗ Caídos | 183 |
| ● No saludables (4xx/5xx) | 0 |
| Latencia media (ms) | 4 |
| Latencia P95 (ms) | 14 |
| Latencia P99 (ms) | 25 |

---

## 2. SEGURIDAD

| Aspecto | Implementación |
|---------|----------------|
| Cifrado en reposo | AES-256 |
| Cifrado en tránsito | TLS 1.3 |
| Gestión de claves | HSM / Propio |
| Estándares | Sovereign Government Security, ISO 27001, SOC 2, AML, KYC, GDPR |
| Infraestructura | Todo propio — sin dependencias de terceros |

---

## 3. VELOCIDAD

- **Latencia media:** 4 ms
- **P95:** 14 ms
- **P99:** 25 ms

---

## 4. ALCANCE GLOBAL

- **Regiones:** Norteamérica, Europa, Asia Pacífico, Sudamérica
- **Nodos:** Ierahkwa Node Alpha, Ierahkwa Node Beta, Ierahkwa Node Gamma, Ierahkwa Node Delta
- **Data centers:** Ierahkwa Primary DC, Ierahkwa DR Site
- **Alcance:** Global — Todas las plataformas, departamentos y servicios bajo un solo ecosistema

---

## 5. DETALLE POR CATEGORÍA

| Categoría | Total | Saludables | Caídos | No saludables |
|-----------|-------|------------|--------|---------------|
| core | 4 | 2 | 2 | 0 |
| platform_servers | 10 | 7 | 3 | 0 |
| trading | 5 | 0 | 5 | 0 |
| banking_hierarchy | 9 | 9 | 0 | 0 |
| office | 5 | 0 | 5 | 0 |
| government | 5 | 1 | 4 | 0 |
| document | 3 | 0 | 3 | 0 |
| security | 3 | 0 | 3 | 0 |
| blockchain | 4 | 0 | 4 | 0 |
| ai | 3 | 3 | 0 | 0 |
| multilang | 3 | 1 | 2 | 0 |
| platform | 152 | 0 | 152 | 0 |

---

## 6. DETALLE DE SERVICIOS Y PÁGINAS

| Nombre | URL | Estado | Latencia (ms) |
|--------|-----|--------|---------------|
| Ierahkwa Futurehead Mamey Node | http://localhost:8545/health | down | 11 |
| Banking Bridge API | http://localhost:3001/api/health | healthy | 12 |
| Banking API .NET | http://localhost:3000/api/health | down | 1 |
| Platform Frontend | http://localhost:8080/health | healthy | 5 |
| BDET Bank Server | http://localhost:4001/health | down | 1 |
| TradeX Server | http://localhost:4002/health | down | 1 |
| SIIS Settlement Server | http://localhost:4003/health | down | 0 |
| Clearing House Server | http://localhost:4004/health | healthy | 8 |
| Banco Águila (Norte) | http://localhost:4100/health | healthy | 9 |
| Banco Quetzal (Centro) | http://localhost:4200/health | healthy | 5 |
| Banco Cóndor (Sur) | http://localhost:4300/health | healthy | 5 |
| Banco Caribe (Taínos) | http://localhost:4400/health | healthy | 16 |
| AI Hub / ATABEY Server | http://localhost:4500/health | healthy | 6 |
| Government Portal Server | http://localhost:4600/health | healthy | 11 |
| TradeX Exchange | http://localhost:5054/health | down | 1 |
| NET10 DeFi | http://localhost:5071/health | down | 0 |
| FarmFactory | http://localhost:5061/health | down | 1 |
| IDOFactory | http://localhost:5097/health | down | 1 |
| Forex Trading Server | http://localhost:5200/health | down | 1 |
| SIIS - International Settlement | http://localhost:6000/api/health | healthy | 54 |
| Clearing House | http://localhost:6001/api/health | healthy | 21 |
| Global Services | http://localhost:6002/api/health | healthy | 25 |
| Receiver Bank | http://localhost:6003/api/health | healthy | 11 |
| BDET Master Bank | http://localhost:6010/api/health | healthy | 15 |
| Águila Central Bank (Norte) | http://localhost:6100/api/health | healthy | 12 |
| Quetzal Central Bank (Centro) | http://localhost:6200/api/health | healthy | 17 |
| Cóndor Central Bank (Sur) | http://localhost:6300/api/health | healthy | 14 |
| Caribe Central Bank (Caribe) | http://localhost:6400/api/health | healthy | 14 |
| RnBCal Calendar | http://localhost:5055/health | down | 5 |
| SpikeOffice | http://localhost:5056/health | down | 2 |
| AppBuilder | http://localhost:5062/health | down | 6 |
| ProjectHub | http://localhost:7070/health | down | 4 |
| MeetingHub | http://localhost:7071/health | down | 2 |
| CitizenCRM | http://localhost:5090/health | down | 0 |
| TaxAuthority | http://localhost:5091/health | down | 1 |
| VotingSystem | http://localhost:5092/health | down | 1 |
| ServiceDesk | http://localhost:5093/health | down | 0 |
| License Authority | http://localhost:5094/health | healthy | 5 |
| DocumentFlow | http://localhost:5080/health | down | 1 |
| ESignature | http://localhost:5081/health | down | 1 |
| OutlookExtractor | http://localhost:5082/health | down | 0 |
| BioMetrics | http://localhost:5120/health | down | 1 |
| DigitalVault | http://localhost:5121/health | down | 0 |
| AI Fraud Detection | http://localhost:5144/health | down | 2 |
| DeFi Soberano | http://localhost:5140/health | down | 0 |
| NFT Certificates | http://localhost:5141/health | down | 1 |
| Governance DAO | http://localhost:5142/health | down | 0 |
| Multichain Bridge | http://localhost:5143/health | down | 1 |
| AI Hub / ATABEY | http://localhost:5300/health | healthy | 8 |
| AI Banker BDET | http://localhost:5301/health | healthy | 4 |
| AI Trader | http://localhost:5302/health | healthy | 7 |
| Rust SWIFT Service | http://localhost:8590/health | healthy | 9 |
| Go Queue Service | http://localhost:8591/health | down | 1 |
| Python ML Service | http://localhost:8592/health | down | 3 |
| POS | http://localhost:8545/pos | down | 2 |
| Chat | http://localhost:8545/chat | down | 3 |
| BDET Bank | http://localhost:8545/bdet-bank | down | 3 |
| Wallet | http://localhost:8545/wallet | down | 1 |
| Forex | http://localhost:8545/forex | down | 2 |
| Tokens | http://localhost:8545/tokens | down | 4 |
| Departments | http://localhost:8545/departments | down | 2 |
| VIP Transactions | http://localhost:8545/vip-transactions | down | 2 |
| abrir-todas-plataformas.html | http://localhost:8545/platform/abrir-todas-plataformas.html | down | 4 |
| admin.html | http://localhost:8545/platform/admin.html | down | 4 |
| ai-hub-dashboard.html | http://localhost:8545/platform/ai-hub-dashboard.html | down | 2 |
| ai-platform.html | http://localhost:8545/platform/ai-platform.html | down | 1 |
| americas-communication-platform.html | http://localhost:8545/platform/americas-communication-platform.html | down | 1 |
| analytics-dashboard.html | http://localhost:8545/platform/analytics-dashboard.html | down | 1 |
| animstorm-ai.html | http://localhost:8545/platform/animstorm-ai.html | down | 0 |
| app-ai-studio.html | http://localhost:8545/platform/app-ai-studio.html | down | 0 |
| app-studio.html | http://localhost:8545/platform/app-studio.html | down | 1 |
| appbuilder.html | http://localhost:8545/platform/appbuilder.html | down | 0 |
| atabey-dashboard.html | http://localhost:8545/platform/atabey-dashboard.html | down | 1 |
| atabey-platform.html | http://localhost:8545/platform/atabey-platform.html | down | 0 |
| atm-manufacturing.html | http://localhost:8545/platform/atm-manufacturing.html | down | 1 |
| backup-department.html | http://localhost:8545/platform/backup-department.html | down | 0 |
| bank-worker.html | http://localhost:8545/platform/bank-worker.html | down | 1 |
| bdet-bank.html | http://localhost:8545/platform/bdet-bank.html | down | 1 |
| biometrics.html | http://localhost:8545/platform/biometrics.html | down | 1 |
| bitcoin-hemp.html | http://localhost:8545/platform/bitcoin-hemp.html | down | 1 |
| blockchain-platform.html | http://localhost:8545/platform/blockchain-platform.html | down | 0 |
| bridge.html | http://localhost:8545/platform/bridge.html | down | 0 |
| budget-control.html | http://localhost:8545/platform/budget-control.html | down | 1 |
| casino.html | http://localhost:8545/platform/casino.html | down | 0 |
| central-banks.html | http://localhost:8545/platform/central-banks.html | down | 0 |
| chat.html | http://localhost:8545/platform/chat.html | down | 1 |
| citizen-crm.html | http://localhost:8545/platform/citizen-crm.html | down | 0 |
| citizen-emergency-monitor.html | http://localhost:8545/platform/citizen-emergency-monitor.html | down | 0 |
| citizen-launchpad.html | http://localhost:8545/platform/citizen-launchpad.html | down | 1 |
| citizen-membership.html | http://localhost:8545/platform/citizen-membership.html | down | 0 |
| citizen-portal.html | http://localhost:8545/platform/citizen-portal.html | down | 0 |
| comando-conjunto-fortress-ai-quantum.html | http://localhost:8545/platform/comando-conjunto-fortress-ai-quantum.html | down | 1 |
| comercio-global.html | http://localhost:8545/platform/comercio-global.html | down | 0 |
| commerce-platform.html | http://localhost:8545/platform/commerce-platform.html | down | 1 |
| comparativas-criminalidad.html | http://localhost:8545/platform/comparativas-criminalidad.html | down | 0 |
| compliance-center.html | http://localhost:8545/platform/compliance-center.html | down | 1 |
| concienciacion-seguridad.html | http://localhost:8545/platform/concienciacion-seguridad.html | down | 0 |
| conoce-la-plataforma.html | http://localhost:8545/platform/conoce-la-plataforma.html | down | 1 |
| contribution-graph.html | http://localhost:8545/platform/contribution-graph.html | down | 3 |
| cryptohost-conversion.html | http://localhost:8545/platform/cryptohost-conversion.html | down | 4 |
| cryptohost.html | http://localhost:8545/platform/cryptohost.html | down | 1 |
| dao-governance.html | http://localhost:8545/platform/dao-governance.html | down | 0 |
| dashboard-full.html | http://localhost:8545/platform/dashboard-full.html | down | 1 |
| dashboard-tests-live.html | http://localhost:8545/platform/dashboard-tests-live.html | down | 1 |
| dashboard.html | http://localhost:8545/platform/dashboard.html | down | 0 |
| debt-collection.html | http://localhost:8545/platform/debt-collection.html | down | 1 |
| department.html | http://localhost:8545/platform/department.html | down | 0 |
| departments.html | http://localhost:8545/platform/departments.html | down | 0 |
| depository-skr.html | http://localhost:8545/platform/depository-skr.html | down | 0 |
| developer-portal.html | http://localhost:8545/platform/developer-portal.html | down | 1 |
| digital-vault.html | http://localhost:8545/platform/digital-vault.html | down | 0 |
| documents-platform.html | http://localhost:8545/platform/documents-platform.html | down | 1 |
| documents.html | http://localhost:8545/platform/documents.html | down | 0 |
| editor-complete.html | http://localhost:8545/platform/editor-complete.html | down | 0 |
| education-platform.html | http://localhost:8545/platform/education-platform.html | down | 1 |
| email-studio.html | http://localhost:8545/platform/email-studio.html | down | 0 |
| emergencies.html | http://localhost:8545/platform/emergencies.html | down | 0 |
| esignature.html | http://localhost:8545/platform/esignature.html | down | 0 |
| estado-soberano-atabey.html | http://localhost:8545/platform/estado-soberano-atabey.html | down | 0 |
| face-recognition-propio.html | http://localhost:8545/platform/face-recognition-propio.html | down | 1 |
| farmfactory.html | http://localhost:8545/platform/farmfactory.html | down | 0 |
| finance-platform.html | http://localhost:8545/platform/finance-platform.html | down | 1 |
| financial-instruments.html | http://localhost:8545/platform/financial-instruments.html | down | 0 |
| firewall-plus.html | http://localhost:8545/platform/firewall-plus.html | down | 1 |
| forex.html | http://localhost:8545/platform/forex.html | down | 2 |
| fuentes-oficiales-justicia-global.html | http://localhost:8545/platform/fuentes-oficiales-justicia-global.html | down | 1 |
| futurehead-group.html | http://localhost:8545/platform/futurehead-group.html | down | 1 |
| gaming-platform-unified.html | http://localhost:8545/platform/gaming-platform-unified.html | down | 0 |
| gaming-platform.html | http://localhost:8545/platform/gaming-platform.html | down | 1 |
| government-platform.html | http://localhost:8545/platform/government-platform.html | down | 0 |
| government-portal.html | http://localhost:8545/platform/government-portal.html | down | 1 |
| health-dashboard.html | http://localhost:8545/platform/health-dashboard.html | down | 0 |
| health-platform.html | http://localhost:8545/platform/health-platform.html | down | 1 |
| ido-factory.html | http://localhost:8545/platform/ido-factory.html | down | 0 |
| incidentes-dr.html | http://localhost:8545/platform/incidentes-dr.html | down | 0 |
| index.html | http://localhost:8545/platform/index.html | down | 1 |
| insurance-platform.html | http://localhost:8545/platform/insurance-platform.html | down | 0 |
| internet-propio-platform.html | http://localhost:8545/platform/internet-propio-platform.html | down | 1 |
| invoicer.html | http://localhost:8545/platform/invoicer.html | down | 0 |
| iptv-admin.html | http://localhost:8545/platform/iptv-admin.html | down | 0 |
| leader-control.html | http://localhost:8545/platform/leader-control.html | down | 1 |
| licenses-department.html | http://localhost:8545/platform/licenses-department.html | down | 0 |
| login.html | http://localhost:8545/platform/login.html | down | 0 |
| lotto.html | http://localhost:8545/platform/lotto.html | down | 1 |
| mamey-futures.html | http://localhost:8545/platform/mamey-futures.html | down | 2 |
| meeting-hub.html | http://localhost:8545/platform/meeting-hub.html | down | 1 |
| monitor-live.html | http://localhost:8545/platform/monitor-live.html | down | 2 |
| monitor.html | http://localhost:8545/platform/monitor.html | down | 1 |
| net10-defi.html | http://localhost:8545/platform/net10-defi.html | down | 5 |
| notifications.html | http://localhost:8545/platform/notifications.html | down | 1 |
| office-platform.html | http://localhost:8545/platform/office-platform.html | down | 0 |
| operations-dashboard.html | http://localhost:8545/platform/operations-dashboard.html | down | 1 |
| plataforma-finance-unificada.html | http://localhost:8545/platform/plataforma-finance-unificada.html | down | 0 |
| plataforma-unificada.html | http://localhost:8545/platform/plataforma-unificada.html | down | 1 |
| plataforma-vigilancia-inteligencia.html | http://localhost:8545/platform/plataforma-vigilancia-inteligencia.html | down | 0 |
| project-hub.html | http://localhost:8545/platform/project-hub.html | down | 1 |
| proteccion-plataformas.html | http://localhost:8545/platform/proteccion-plataformas.html | down | 0 |
| quantum-platform.html | http://localhost:8545/platform/quantum-platform.html | down | 1 |
| raffle.html | http://localhost:8545/platform/raffle.html | down | 0 |
| registro-personas-interes-publico.html | http://localhost:8545/platform/registro-personas-interes-publico.html | down | 1 |
| requisitos-negocios-gobierno.html | http://localhost:8545/platform/requisitos-negocios-gobierno.html | down | 0 |
| rewards.html | http://localhost:8545/platform/rewards.html | down | 0 |
| rnbcal.html | http://localhost:8545/platform/rnbcal.html | down | 1 |
| safety-link-click.html | http://localhost:8545/platform/safety-link-click.html | down | 0 |
| safety-link-create.html | http://localhost:8545/platform/safety-link-create.html | down | 1 |
| secure-chat.html | http://localhost:8545/platform/secure-chat.html | down | 0 |
| security-fortress.html | http://localhost:8545/platform/security-fortress.html | down | 1 |
| service-desk.html | http://localhost:8545/platform/service-desk.html | down | 0 |
| services-platform.html | http://localhost:8545/platform/services-platform.html | down | 1 |
| servicios-plataformas.html | http://localhost:8545/platform/servicios-plataformas.html | down | 0 |
| servicios-renta.html | http://localhost:8545/platform/servicios-renta.html | down | 1 |
| settings.html | http://localhost:8545/platform/settings.html | down | 0 |
| siis-settlement.html | http://localhost:8545/platform/siis-settlement.html | down | 0 |
| sistema-bancario.html | http://localhost:8545/platform/sistema-bancario.html | down | 0 |
| smart-cell-platform.html | http://localhost:8545/platform/smart-cell-platform.html | down | 1 |
| smartschool.html | http://localhost:8545/platform/smartschool.html | down | 0 |
| social-media-codes.html | http://localhost:8545/platform/social-media-codes.html | down | 1 |
| social-media.html | http://localhost:8545/platform/social-media.html | down | 0 |
| social-platform.html | http://localhost:8545/platform/social-platform.html | down | 0 |
| sovereign-identity.html | http://localhost:8545/platform/sovereign-identity.html | down | 1 |
| sovereign-public-affairs.html | http://localhost:8545/platform/sovereign-public-affairs.html | down | 0 |
| sovereign-status.html | http://localhost:8545/platform/sovereign-status.html | down | 1 |
| sovereignty-education.html | http://localhost:8545/platform/sovereignty-education.html | down | 0 |
| spike-office.html | http://localhost:8545/platform/spike-office.html | down | 0 |
| sports-betting.html | http://localhost:8545/platform/sports-betting.html | down | 1 |
| stack-empresarial.html | http://localhost:8545/platform/stack-empresarial.html | down | 0 |
| support-ai.html | http://localhost:8545/platform/support-ai.html | down | 1 |
| tax-authority.html | http://localhost:8545/platform/tax-authority.html | down | 1 |
| telecom-platform.html | http://localhost:8545/platform/telecom-platform.html | down | 0 |
| template-unified.html | http://localhost:8545/platform/template-unified.html | down | 0 |
| tenant-dashboard.html | http://localhost:8545/platform/tenant-dashboard.html | down | 1 |
| token-factory.html | http://localhost:8545/platform/token-factory.html | down | 0 |
| tradex.html | http://localhost:8545/platform/tradex.html | down | 1 |
| user-dashboard.html | http://localhost:8545/platform/user-dashboard.html | down | 0 |
| video-call.html | http://localhost:8545/platform/video-call.html | down | 1 |
| vigilancia-brain.html | http://localhost:8545/platform/vigilancia-brain.html | down | 3 |
| vip-transactions.html | http://localhost:8545/platform/vip-transactions.html | down | 1 |
| vms-gestion-video.html | http://localhost:8545/platform/vms-gestion-video.html | down | 1 |
| voip-telephone-platform.html | http://localhost:8545/platform/voip-telephone-platform.html | down | 1 |
| voting.html | http://localhost:8545/platform/voting.html | down | 0 |
| wallet.html | http://localhost:8545/platform/wallet.html | down | 0 |
| watchlist-alerta-proteccion.html | http://localhost:8545/platform/watchlist-alerta-proteccion.html | down | 1 |
| webcam-view.html | http://localhost:8545/platform/webcam-view.html | down | 0 |
| webcams-platform.html | http://localhost:8545/platform/webcams-platform.html | down | 1 |
| whistleblower.html | http://localhost:8545/platform/whistleblower.html | down | 0 |
| workflow-engine.html | http://localhost:8545/platform/workflow-engine.html | down | 0 |

---

*Reporte generado por scripts/test-toda-plataforma.js*
# REPORTE TESTING GLOBAL — PLATAFORMA IERAHKWA SOBERANA
**Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister**
**Verdad y seguridad para nuestros clientes. La mejor plataforma soberana del mundo.**
**Un amor. Una vida. Las Américas — Sí podemos.**

**Date:** 2026-02-15T22:59:36.885Z
**Base URL:** http://localhost:8545

### Status meanings
| Status | Meaning |
|--------|---------|
| **down** | Caído: el servicio no responde |
| **healthy** | OK: el servicio responde correctamente |
| **unhealthy** | No saludable: responde con error (4xx/5xx) |

---

## 1. EXECUTIVE SUMMARY

| Metric | Value |
|--------|-------|
| Total servicios/páginas probados | 376 |
| ✓ Saludables | 359 |
| ✗ Caídos | 2 |
| ● No saludables (4xx/5xx) | 15 |
| Latencia media (ms) | 10 |
| Latencia P95 (ms) | 16 |
| Latencia P99 (ms) | 86 |

---

## 2. SEGURIDAD

| Aspect | Implementation |
|---------|----------------|
| Cifrado en reposo | AES-256 |
| Cifrado en tránsito | TLS 1.3 |
| Gestión de claves | HSM / Own |
| Standards | Sovereign Government Security, ISO 27001, SOC 2, AML, KYC, GDPR |
| Infrastructure | Infraestructura propia — sin dependencias de terceros |

---

## 3. VELOCIDAD

- **Latencia media (ms):** 10 ms
- **P95:** 16 ms
- **P99:** 86 ms

---

## 4. ALCANCE

- **Regiones:** North America, Europe, Asia Pacific, South America
- **Nodos:** Ierahkwa Node Alpha, Ierahkwa Node Beta, Ierahkwa Node Gamma, Ierahkwa Node Delta
- **Data centers:** Ierahkwa Primary DC, Ierahkwa DR Site
- **Scope:** Global — All platforms, departments and services under one ecosystem

---

## 5. TRUTH & SECURITY

**Verdad y seguridad para nuestros clientes. La mejor plataforma soberana del mundo.**

---

## 6. DETAIL BY CATEGORY

| Category | Total | Healthy | Down | Unhealthy |
|-----------|-------|------------|--------|---------------|
| core | 8 | 7 | 1 | 0 |
| platform_servers | 23 | 23 | 0 | 0 |
| trading | 5 | 5 | 0 | 0 |
| banking_hierarchy | 9 | 9 | 0 | 0 |
| office | 5 | 3 | 0 | 2 |
| government | 5 | 1 | 0 | 4 |
| document | 6 | 3 | 1 | 2 |
| security | 3 | 0 | 0 | 3 |
| blockchain | 4 | 0 | 0 | 4 |
| ai | 3 | 3 | 0 | 0 |
| multilang | 3 | 3 | 0 | 0 |
| platform | 302 | 302 | 0 | 0 |

---

## 7. SERVICE AND PAGE DETAIL

| Name | URL | Status | Latency (ms) |
|--------|-----|--------|---------------|
| Ierahkwa Futurehead Mamey Node | http://localhost:8545/health | healthy | 947 |
| Palm Reader | http://localhost:8545/api/v1/palm-reader/health | healthy | 183 |
| WhatsApp POS | http://localhost:8545/api/v1/whatsapp/health | healthy | 15 |
| Sovereign Logistics (SLS/CDE) | http://localhost:8545/api/v1/logistics/health | healthy | 24 |
| Virtual Cards | http://localhost:8545/api/v1/virtual-cards/health | healthy | 85 |
| Banking Bridge API | http://localhost:3001/api/health | healthy | 18 |
| Banking API .NET | http://localhost:3000/api/health | healthy | 17 |
| Platform Frontend | http://localhost:8080/health | down | 8 |
| BDET Bank Server | http://localhost:8545/api/v1/bdet-server/health | healthy | 29 |
| TradeX Server | http://localhost:4002/health | healthy | 401 |
| SIIS Settlement Server | http://localhost:8545/api/v1/siis/health | healthy | 5 |
| Clearing House Server | http://localhost:8545/api/v1/clearing/health | healthy | 5 |
| Global Service | http://localhost:8545/api/v1/global-service/health | healthy | 10 |
| Receiver Bank | http://localhost:8545/api/v1/receiver/health | healthy | 11 |
| CryptoHost | http://localhost:8545/api/v1/cryptohost/health | healthy | 7 |
| Accounting | http://localhost:8545/api/v1/accounting/health | healthy | 4 |
| ISP Billing | http://localhost:8545/api/v1/isp-billing/health | healthy | 8 |
| Logistics ERP | http://localhost:8545/api/v1/logistics-erp/health | healthy | 3 |
| Hospital Management System | http://localhost:8545/api/v1/hospital-management/health | healthy | 7 |
| CMS Platform | http://localhost:8545/api/v1/cms/health | healthy | 8 |
| Affiliate Program | http://localhost:8545/api/v1/affiliate/health | healthy | 7 |
| Membership | http://localhost:8545/api/v1/membership/health | healthy | 17 |
| All Protocols Global | http://localhost:8545/health | healthy | 7 |
| Banking Hierarchy | http://localhost:8545/health | healthy | 2 |
| Banco Águila (Norte) | http://localhost:8545/health | healthy | 1 |
| Banco Quetzal (Centro) | http://localhost:8545/health | healthy | 1 |
| Banco Cóndor (Sur) | http://localhost:8545/health | healthy | 2 |
| Banco Caribe (Taínos) | http://localhost:8545/health | healthy | 2 |
| AI Hub / ATABEY Server | http://localhost:8545/health | healthy | 2 |
| Government Portal Server | http://localhost:8545/api/v1/government-portal/health | healthy | 4 |
| Mamey Gateway | http://localhost:8545/api/v1/mamey/gateway/health | healthy | 5 |
| TradeX Exchange | http://localhost:5054/health | healthy | 16 |
| NET10 DeFi | http://localhost:5071/health | healthy | 7 |
| FarmFactory | http://localhost:5061/health | healthy | 5 |
| IDOFactory | http://localhost:5097/health | healthy | 3 |
| Forex Trading Server | http://localhost:5200/health | healthy | 86 |
| SIIS - International Settlement | http://localhost:6000/api/health | healthy | 6 |
| Clearing House | http://localhost:6001/api/health | healthy | 3 |
| Global Services | http://localhost:6002/api/health | healthy | 2 |
| Receiver Bank | http://localhost:6003/api/health | healthy | 2 |
| BDET Master Bank | http://localhost:6010/api/health | healthy | 2 |
| Águila Central Bank (Norte) | http://localhost:6100/api/health | healthy | 2 |
| Quetzal Central Bank (Centro) | http://localhost:6200/api/health | healthy | 2 |
| Cóndor Central Bank (Sur) | http://localhost:6300/api/health | healthy | 2 |
| Caribe Central Bank (Caribe) | http://localhost:6400/api/health | healthy | 4 |
| RnBCal Calendar | http://localhost:5055/health | healthy | 3 |
| SpikeOffice | http://localhost:5056/health | healthy | 7 |
| AppBuilder | http://localhost:5062/health | healthy | 9 |
| ProjectHub | http://localhost:7070/health | unhealthy | 22 |
| MeetingHub | http://localhost:7071/health | unhealthy | 25 |
| CitizenCRM | http://localhost:5090/health | unhealthy | 5 |
| TaxAuthority | http://localhost:5091/health | unhealthy | 7 |
| VotingSystem | http://localhost:5092/health | unhealthy | 5 |
| ServiceDesk | http://localhost:5093/health | unhealthy | 4 |
| License Authority | http://localhost:8545/api/v1/license-authority/health | healthy | 5 |
| DocumentFlow | http://localhost:5080/health | unhealthy | 9 |
| ESignature | http://localhost:5081/health | unhealthy | 3 |
| OutlookExtractor | http://localhost:5082/health | healthy | 4 |
| Invoice Extraction | http://localhost:8545/api/v1/invoices/health | healthy | 3 |
| Mamey FileReader | http://localhost:5083/health | down | 1 |
| Places Extractor | http://localhost:8545/api/v1/places-extractor/health | healthy | 4 |
| BioMetrics | http://localhost:5120/health | unhealthy | 2 |
| DigitalVault | http://localhost:5121/health | unhealthy | 3 |
| AI Fraud Detection | http://localhost:5144/health | unhealthy | 3 |
| DeFi Soberano | http://localhost:5140/health | unhealthy | 2 |
| NFT Certificates | http://localhost:5141/health | unhealthy | 3 |
| Governance DAO | http://localhost:5142/health | unhealthy | 5 |
| Multichain Bridge | http://localhost:5143/health | unhealthy | 4 |
| AI Hub / ATABEY | http://localhost:8545/api/ai-hub/health | healthy | 9 |
| AI Banker BDET | http://localhost:8545/api/ai-hub/bdet/status | healthy | 5 |
| AI Trader | http://localhost:8545/api/ai-hub/trader/status | healthy | 3 |
| Rust SWIFT Service | http://localhost:8590/health | healthy | 4 |
| Go Queue Service | http://localhost:8591/health | healthy | 2 |
| Python ML Service | http://localhost:8592/health | healthy | 6 |
| POS | http://localhost:8545/pos | healthy | 2 |
| Chat | http://localhost:8545/chat | healthy | 4 |
| BDET Bank | http://localhost:8545/bdet-bank | healthy | 7 |
| Wallet | http://localhost:8545/wallet | healthy | 5 |
| Forex | http://localhost:8545/forex | healthy | 2 |
| Tokens | http://localhost:8545/tokens | healthy | 3 |
| Departments | http://localhost:8545/departments | healthy | 2 |
| VIP Transactions | http://localhost:8545/vip-transactions | healthy | 3 |
| abrir-todas-plataformas.html | http://localhost:8545/platform/abrir-todas-plataformas.html | healthy | 4 |
| admin.html | http://localhost:8545/platform/admin.html | healthy | 8 |
| affiliate-platform.html | http://localhost:8545/platform/affiliate-platform.html | healthy | 3 |
| agriculture-department.html | http://localhost:8545/platform/agriculture-department.html | healthy | 9 |
| ai-hub-dashboard.html | http://localhost:8545/platform/ai-hub-dashboard.html | healthy | 6 |
| ai-hub-status.html | http://localhost:8545/platform/ai-hub-status.html | healthy | 9 |
| ai-platform.html | http://localhost:8545/platform/ai-platform.html | healthy | 10 |
| ai-voice-agents.html | http://localhost:8545/platform/ai-voice-agents.html | healthy | 14 |
| akashic-records.html | http://localhost:8545/platform/akashic-records.html | healthy | 22 |
| americas-communication-platform.html | http://localhost:8545/platform/americas-communication-platform.html | healthy | 24 |
| analytics-dashboard.html | http://localhost:8545/platform/analytics-dashboard.html | healthy | 13 |
| ancestral-security.html | http://localhost:8545/platform/ancestral-security.html | healthy | 9 |
| animstorm-ai.html | http://localhost:8545/platform/animstorm-ai.html | healthy | 9 |
| anti-surveillance.html | http://localhost:8545/platform/anti-surveillance.html | healthy | 13 |
| app-ai-studio.html | http://localhost:8545/platform/app-ai-studio.html | healthy | 7 |
| app-studio.html | http://localhost:8545/platform/app-studio.html | healthy | 3 |
| appbuilder.html | http://localhost:8545/platform/appbuilder.html | healthy | 6 |
| atabey-dashboard.html | http://localhost:8545/platform/atabey-dashboard.html | healthy | 8 |
| atabey-nerve-center.html | http://localhost:8545/platform/atabey-nerve-center.html | healthy | 3 |
| atabey-platform.html | http://localhost:8545/platform/atabey-platform.html | healthy | 7 |
| atm-manufacturing.html | http://localhost:8545/platform/atm-manufacturing.html | healthy | 5 |
| audit-dashboard.html | http://localhost:8545/platform/audit-dashboard.html | healthy | 6 |
| backup-department.html | http://localhost:8545/platform/backup-department.html | healthy | 6 |
| bank-submission.html | http://localhost:8545/platform/bank-submission.html | healthy | 3 |
| bank-worker.html | http://localhost:8545/platform/bank-worker.html | healthy | 3 |
| bdet-bank.html | http://localhost:8545/platform/bdet-bank.html | healthy | 5 |
| bio-resonance.html | http://localhost:8545/platform/bio-resonance.html | healthy | 3 |
| biometrics.html | http://localhost:8545/platform/biometrics.html | healthy | 3 |
| bitcoin-hemp.html | http://localhost:8545/platform/bitcoin-hemp.html | healthy | 3 |
| block-explorer.html | http://localhost:8545/platform/block-explorer.html | healthy | 3 |
| blockchain-platform.html | http://localhost:8545/platform/blockchain-platform.html | healthy | 4 |
| bridge.html | http://localhost:8545/platform/bridge.html | healthy | 2 |
| budget-control.html | http://localhost:8545/platform/budget-control.html | healthy | 3 |
| buscar.html | http://localhost:8545/platform/buscar.html | healthy | 4 |
| calendar-agenda.html | http://localhost:8545/platform/calendar-agenda.html | healthy | 3 |
| calendario-tortuga.html | http://localhost:8545/platform/calendario-tortuga.html | healthy | 2 |
| captive-portal.html | http://localhost:8545/platform/captive-portal.html | healthy | 4 |
| casino.html | http://localhost:8545/platform/casino.html | healthy | 3 |
| cdn-platform.html | http://localhost:8545/platform/cdn-platform.html | healthy | 3 |
| central-banks.html | http://localhost:8545/platform/central-banks.html | healthy | 6 |
| chat.html | http://localhost:8545/platform/chat.html | healthy | 2 |
| citizen-ai-agent.html | http://localhost:8545/platform/citizen-ai-agent.html | healthy | 10 |
| citizen-crm.html | http://localhost:8545/platform/citizen-crm.html | healthy | 8 |
| citizen-emergency-monitor.html | http://localhost:8545/platform/citizen-emergency-monitor.html | healthy | 4 |
| citizen-launchpad.html | http://localhost:8545/platform/citizen-launchpad.html | healthy | 4 |
| citizen-membership.html | http://localhost:8545/platform/citizen-membership.html | healthy | 4 |
| citizen-portal.html | http://localhost:8545/platform/citizen-portal.html | healthy | 10 |
| cloaking.html | http://localhost:8545/platform/cloaking.html | healthy | 7 |
| cms-platform.html | http://localhost:8545/platform/cms-platform.html | healthy | 5 |
| collective-memory.html | http://localhost:8545/platform/collective-memory.html | healthy | 6 |
| comando-conjunto-fortress-ai-quantum.html | http://localhost:8545/platform/comando-conjunto-fortress-ai-quantum.html | healthy | 6 |
| comercio-global.html | http://localhost:8545/platform/comercio-global.html | healthy | 6 |
| command-center.html | http://localhost:8545/platform/command-center.html | healthy | 3 |
| commerce-platform.html | http://localhost:8545/platform/commerce-platform.html | healthy | 3 |
| comparativas-criminalidad.html | http://localhost:8545/platform/comparativas-criminalidad.html | healthy | 5 |
| compliance-center.html | http://localhost:8545/platform/compliance-center.html | healthy | 3 |
| concienciacion-seguridad.html | http://localhost:8545/platform/concienciacion-seguridad.html | healthy | 3 |
| conoce-la-plataforma.html | http://localhost:8545/platform/conoce-la-plataforma.html | healthy | 2 |
| consciousness-lock.html | http://localhost:8545/platform/consciousness-lock.html | healthy | 4 |
| consciousness-transfer.html | http://localhost:8545/platform/consciousness-transfer.html | healthy | 3 |
| contribution-graph.html | http://localhost:8545/platform/contribution-graph.html | healthy | 5 |
| corrections-protection.html | http://localhost:8545/platform/corrections-protection.html | healthy | 3 |
| cryptohost-conversion.html | http://localhost:8545/platform/cryptohost-conversion.html | healthy | 6 |
| cryptohost.html | http://localhost:8545/platform/cryptohost.html | healthy | 5 |
| cultura.html | http://localhost:8545/platform/cultura.html | healthy | 4 |
| dao-governance.html | http://localhost:8545/platform/dao-governance.html | healthy | 3 |
| dashboard-full.html | http://localhost:8545/platform/dashboard-full.html | healthy | 6 |
| dashboard-tests-live.html | http://localhost:8545/platform/dashboard-tests-live.html | healthy | 4 |
| dashboard.html | http://localhost:8545/platform/dashboard.html | healthy | 6 |
| data-marketplace.html | http://localhost:8545/platform/data-marketplace.html | healthy | 4 |
| dating-platform.html | http://localhost:8545/platform/dating-platform.html | healthy | 3 |
| de-extinction.html | http://localhost:8545/platform/de-extinction.html | healthy | 4 |
| debt-collection.html | http://localhost:8545/platform/debt-collection.html | healthy | 5 |
| defense-command.html | http://localhost:8545/platform/defense-command.html | healthy | 9 |
| defense-intelligence.html | http://localhost:8545/platform/defense-intelligence.html | healthy | 3 |
| dental-clinic.html | http://localhost:8545/platform/dental-clinic.html | healthy | 3 |
| department-nodes.html | http://localhost:8545/platform/department-nodes.html | healthy | 3 |
| department.html | http://localhost:8545/platform/department.html | healthy | 4 |
| departments.html | http://localhost:8545/platform/departments.html | healthy | 3 |
| depository-skr.html | http://localhost:8545/platform/depository-skr.html | healthy | 4 |
| developer-portal.html | http://localhost:8545/platform/developer-portal.html | healthy | 4 |
| did-ssi.html | http://localhost:8545/platform/did-ssi.html | healthy | 3 |
| digital-immune.html | http://localhost:8545/platform/digital-immune.html | healthy | 5 |
| digital-vault.html | http://localhost:8545/platform/digital-vault.html | healthy | 3 |
| dimensional-gateway.html | http://localhost:8545/platform/dimensional-gateway.html | healthy | 3 |
| docs-viewer.html | http://localhost:8545/platform/docs-viewer.html | healthy | 4 |
| documents-platform.html | http://localhost:8545/platform/documents-platform.html | healthy | 3 |
| documents.html | http://localhost:8545/platform/documents.html | healthy | 4 |
| dossier-inteligencia.html | http://localhost:8545/platform/dossier-inteligencia.html | healthy | 3 |
| dream-engine.html | http://localhost:8545/platform/dream-engine.html | healthy | 5 |
| editor-complete.html | http://localhost:8545/platform/editor-complete.html | healthy | 3 |
| education-healthcare.html | http://localhost:8545/platform/education-healthcare.html | healthy | 19 |
| education-platform.html | http://localhost:8545/platform/education-platform.html | healthy | 12 |
| egovernment-portal.html | http://localhost:8545/platform/egovernment-portal.html | healthy | 11 |
| el-pueblo-vive-del-gobierno.html | http://localhost:8545/platform/el-pueblo-vive-del-gobierno.html | healthy | 10 |
| email-studio.html | http://localhost:8545/platform/email-studio.html | healthy | 5 |
| email-summarizer.html | http://localhost:8545/platform/email-summarizer.html | healthy | 3 |
| emergencies.html | http://localhost:8545/platform/emergencies.html | healthy | 3 |
| emotion-economy.html | http://localhost:8545/platform/emotion-economy.html | healthy | 5 |
| employee-contracts.html | http://localhost:8545/platform/employee-contracts.html | healthy | 4 |
| esignature.html | http://localhost:8545/platform/esignature.html | healthy | 3 |
| estado-soberano-atabey.html | http://localhost:8545/platform/estado-soberano-atabey.html | healthy | 4 |
| estaty.html | http://localhost:8545/platform/estaty.html | healthy | 3 |
| evidence-intake.html | http://localhost:8545/platform/evidence-intake.html | healthy | 3 |
| face-recognition-propio.html | http://localhost:8545/platform/face-recognition-propio.html | healthy | 3 |
| faq.html | http://localhost:8545/platform/faq.html | healthy | 4 |
| farmfactory.html | http://localhost:8545/platform/farmfactory.html | healthy | 9 |
| ficha-indigenas-america.html | http://localhost:8545/platform/ficha-indigenas-america.html | healthy | 5 |
| file-reader.html | http://localhost:8545/platform/file-reader.html | healthy | 3 |
| finance-all-in-one.html | http://localhost:8545/platform/finance-all-in-one.html | healthy | 6 |
| finance-platform.html | http://localhost:8545/platform/finance-platform.html | healthy | 4 |
| financial-instruments.html | http://localhost:8545/platform/financial-instruments.html | healthy | 5 |
| financial-universe.html | http://localhost:8545/platform/financial-universe.html | healthy | 7 |
| firewall-plus.html | http://localhost:8545/platform/firewall-plus.html | healthy | 7 |
| force-field.html | http://localhost:8545/platform/force-field.html | healthy | 6 |
| forex.html | http://localhost:8545/platform/forex.html | healthy | 4 |
| forgot-password.html | http://localhost:8545/platform/forgot-password.html | healthy | 3 |
| fuentes-oficiales-justicia-global.html | http://localhost:8545/platform/fuentes-oficiales-justicia-global.html | healthy | 2 |
| fusion-brain.html | http://localhost:8545/platform/fusion-brain.html | healthy | 3 |
| futurehead-group.html | http://localhost:8545/platform/futurehead-group.html | healthy | 7 |
| games-console.html | http://localhost:8545/platform/games-console.html | healthy | 7 |
| gaming-platform-unified.html | http://localhost:8545/platform/gaming-platform-unified.html | healthy | 24 |
| gaming-platform.html | http://localhost:8545/platform/gaming-platform.html | healthy | 4 |
| global-bank.html | http://localhost:8545/platform/global-bank.html | healthy | 7 |
| glosario.html | http://localhost:8545/platform/glosario.html | healthy | 6 |
| government-platform.html | http://localhost:8545/platform/government-platform.html | healthy | 5 |
| government-portal.html | http://localhost:8545/platform/government-portal.html | healthy | 11 |
| government-systems.html | http://localhost:8545/platform/government-systems.html | healthy | 5 |
| gran-alianza-continental.html | http://localhost:8545/platform/gran-alianza-continental.html | healthy | 5 |
| graph-engine.html | http://localhost:8545/platform/graph-engine.html | healthy | 3 |
| gravity-comm.html | http://localhost:8545/platform/gravity-comm.html | healthy | 7 |
| health-dashboard.html | http://localhost:8545/platform/health-dashboard.html | healthy | 3 |
| health-platform.html | http://localhost:8545/platform/health-platform.html | healthy | 5 |
| historias.html | http://localhost:8545/platform/historias.html | healthy | 11 |
| hrm-node.html | http://localhost:8545/platform/hrm-node.html | healthy | 5 |
| identity-documents.html | http://localhost:8545/platform/identity-documents.html | healthy | 3 |
| ido-factory.html | http://localhost:8545/platform/ido-factory.html | healthy | 4 |
| immutable-ethics.html | http://localhost:8545/platform/immutable-ethics.html | healthy | 3 |
| incidentes-dr.html | http://localhost:8545/platform/incidentes-dr.html | healthy | 11 |
| index.html | http://localhost:8545/platform/index.html | healthy | 6 |
| info-servicio.html | http://localhost:8545/platform/info-servicio.html | healthy | 8 |
| infrastructure-energy.html | http://localhost:8545/platform/infrastructure-energy.html | healthy | 7 |
| insurance-platform.html | http://localhost:8545/platform/insurance-platform.html | healthy | 7 |
| international-continuity.html | http://localhost:8545/platform/international-continuity.html | healthy | 8 |
| internet-propio-platform.html | http://localhost:8545/platform/internet-propio-platform.html | healthy | 3 |
| interstellar-comm.html | http://localhost:8545/platform/interstellar-comm.html | healthy | 3 |
| invoice-extraction.html | http://localhost:8545/platform/invoice-extraction.html | healthy | 2 |
| invoicer.html | http://localhost:8545/platform/invoicer.html | healthy | 3 |
| iptv-admin.html | http://localhost:8545/platform/iptv-admin.html | healthy | 3 |
| karma-ledger.html | http://localhost:8545/platform/karma-ledger.html | healthy | 3 |
| land-assets.html | http://localhost:8545/platform/land-assets.html | healthy | 4 |
| landing-ai.html | http://localhost:8545/platform/landing-ai.html | healthy | 4 |
| landing-banking.html | http://localhost:8545/platform/landing-banking.html | healthy | 3 |
| landing-government.html | http://localhost:8545/platform/landing-government.html | healthy | 4 |
| landing-security.html | http://localhost:8545/platform/landing-security.html | healthy | 3 |
| landing-services.html | http://localhost:8545/platform/landing-services.html | healthy | 3 |
| landing-social.html | http://localhost:8545/platform/landing-social.html | healthy | 4 |
| language-resurrection.html | http://localhost:8545/platform/language-resurrection.html | healthy | 8 |
| leader-control.html | http://localhost:8545/platform/leader-control.html | healthy | 3 |
| legal-contracts.html | http://localhost:8545/platform/legal-contracts.html | healthy | 6 |
| lenguas.html | http://localhost:8545/platform/lenguas.html | healthy | 4 |
| licenses-department.html | http://localhost:8545/platform/licenses-department.html | healthy | 5 |
| live-conversion.html | http://localhost:8545/platform/live-conversion.html | healthy | 2 |
| login.html | http://localhost:8545/platform/login.html | healthy | 15 |
| logistics-erp.html | http://localhost:8545/platform/logistics-erp.html | healthy | 3 |
| logistics.html | http://localhost:8545/platform/logistics.html | healthy | 4 |
| logit.html | http://localhost:8545/platform/logit.html | healthy | 3 |
| lotto.html | http://localhost:8545/platform/lotto.html | healthy | 7 |
| madre-tierra.html | http://localhost:8545/platform/madre-tierra.html | healthy | 6 |
| mamey-futures.html | http://localhost:8545/platform/mamey-futures.html | healthy | 4 |
| marketplace.html | http://localhost:8545/platform/marketplace.html | healthy | 4 |
| matter-replicator.html | http://localhost:8545/platform/matter-replicator.html | healthy | 3 |
| meeting-hub.html | http://localhost:8545/platform/meeting-hub.html | healthy | 3 |
| membership-platform.html | http://localhost:8545/platform/membership-platform.html | healthy | 2 |
| mesh-network.html | http://localhost:8545/platform/mesh-network.html | healthy | 3 |
| mineral-custody.html | http://localhost:8545/platform/mineral-custody.html | healthy | 2 |
| ml-pipeline.html | http://localhost:8545/platform/ml-pipeline.html | healthy | 3 |
| monitor-live.html | http://localhost:8545/platform/monitor-live.html | healthy | 3 |
| monitor.html | http://localhost:8545/platform/monitor.html | healthy | 2 |
| national-registries.html | http://localhost:8545/platform/national-registries.html | healthy | 4 |
| national-services.html | http://localhost:8545/platform/national-services.html | healthy | 2 |
| net10-defi.html | http://localhost:8545/platform/net10-defi.html | healthy | 2 |
| neural-download.html | http://localhost:8545/platform/neural-download.html | healthy | 4 |
| notifications.html | http://localhost:8545/platform/notifications.html | healthy | 3 |
| nuestra-historia.html | http://localhost:8545/platform/nuestra-historia.html | healthy | 2 |
| office-platform.html | http://localhost:8545/platform/office-platform.html | healthy | 3 |
| offline.html | http://localhost:8545/platform/offline.html | healthy | 2 |
| onboarding.html | http://localhost:8545/platform/onboarding.html | healthy | 3 |
| operations-dashboard.html | http://localhost:8545/platform/operations-dashboard.html | healthy | 6 |
| parallel-reality.html | http://localhost:8545/platform/parallel-reality.html | healthy | 2 |
| pharmacy-management.html | http://localhost:8545/platform/pharmacy-management.html | healthy | 5 |
| places-extractor.html | http://localhost:8545/platform/places-extractor.html | healthy | 3 |
| plataforma-finance-unificada.html | http://localhost:8545/platform/plataforma-finance-unificada.html | healthy | 7 |
| plataforma-unificada.html | http://localhost:8545/platform/plataforma-unificada.html | healthy | 2 |
| plataforma-vigilancia-inteligencia.html | http://localhost:8545/platform/plataforma-vigilancia-inteligencia.html | healthy | 3 |
| predictive-governance.html | http://localhost:8545/platform/predictive-governance.html | healthy | 3 |
| pricing-catalog.html | http://localhost:8545/platform/pricing-catalog.html | healthy | 3 |
| project-hub.html | http://localhost:8545/platform/project-hub.html | healthy | 4 |
| proteccion-plataformas.html | http://localhost:8545/platform/proteccion-plataformas.html | healthy | 2 |
| protocol-honors.html | http://localhost:8545/platform/protocol-honors.html | healthy | 3 |
| push-notifications.html | http://localhost:8545/platform/push-notifications.html | healthy | 2 |
| quantum-platform.html | http://localhost:8545/platform/quantum-platform.html | healthy | 4 |
| raffle.html | http://localhost:8545/platform/raffle.html | healthy | 3 |
| registro-personas-interes-publico.html | http://localhost:8545/platform/registro-personas-interes-publico.html | healthy | 3 |
| regulatory-agencies.html | http://localhost:8545/platform/regulatory-agencies.html | healthy | 3 |
| requisitos-negocios-gobierno.html | http://localhost:8545/platform/requisitos-negocios-gobierno.html | healthy | 5 |
| rescate-americas.html | http://localhost:8545/platform/rescate-americas.html | healthy | 3 |
| resource-rights.html | http://localhost:8545/platform/resource-rights.html | healthy | 3 |
| rewards.html | http://localhost:8545/platform/rewards.html | healthy | 3 |
| rnbcal.html | http://localhost:8545/platform/rnbcal.html | healthy | 3 |
| sacred-geometry.html | http://localhost:8545/platform/sacred-geometry.html | healthy | 3 |
| safety-link-click.html | http://localhost:8545/platform/safety-link-click.html | healthy | 5 |
| safety-link-create.html | http://localhost:8545/platform/safety-link-create.html | healthy | 6 |
| secure-chat.html | http://localhost:8545/platform/secure-chat.html | healthy | 3 |
| security-events.html | http://localhost:8545/platform/security-events.html | healthy | 4 |
| security-fortress.html | http://localhost:8545/platform/security-fortress.html | healthy | 5 |
| service-desk.html | http://localhost:8545/platform/service-desk.html | healthy | 5 |
| services-platform.html | http://localhost:8545/platform/services-platform.html | healthy | 11 |
| servicios-plataformas.html | http://localhost:8545/platform/servicios-plataformas.html | healthy | 3 |
| servicios-renta.html | http://localhost:8545/platform/servicios-renta.html | healthy | 2 |
| settings.html | http://localhost:8545/platform/settings.html | healthy | 3 |
| seven-generations.html | http://localhost:8545/platform/seven-generations.html | healthy | 2 |
| siis-settlement.html | http://localhost:8545/platform/siis-settlement.html | healthy | 3 |
| singularity.html | http://localhost:8545/platform/singularity.html | healthy | 4 |
| sistema-bancario.html | http://localhost:8545/platform/sistema-bancario.html | healthy | 3 |
| smart-cell-platform.html | http://localhost:8545/platform/smart-cell-platform.html | healthy | 3 |
| smart-contracts.html | http://localhost:8545/platform/smart-contracts.html | healthy | 3 |
| smartschool.html | http://localhost:8545/platform/smartschool.html | healthy | 4 |
| social-media-codes.html | http://localhost:8545/platform/social-media-codes.html | healthy | 3 |
| social-media.html | http://localhost:8545/platform/social-media.html | healthy | 16 |
| social-platform.html | http://localhost:8545/platform/social-platform.html | healthy | 6 |
| social-systems.html | http://localhost:8545/platform/social-systems.html | healthy | 4 |
| sovereign-appstore.html | http://localhost:8545/platform/sovereign-appstore.html | healthy | 4 |
| sovereign-cloud.html | http://localhost:8545/platform/sovereign-cloud.html | healthy | 5 |
| sovereign-email.html | http://localhost:8545/platform/sovereign-email.html | healthy | 5 |
| sovereign-identity.html | http://localhost:8545/platform/sovereign-identity.html | healthy | 3 |
| sovereign-infrastructure.html | http://localhost:8545/platform/sovereign-infrastructure.html | healthy | 8 |
| sovereign-internet.html | http://localhost:8545/platform/sovereign-internet.html | healthy | 4 |
| sovereign-investment-departments.html | http://localhost:8545/platform/sovereign-investment-departments.html | healthy | 7 |
| sovereign-maps.html | http://localhost:8545/platform/sovereign-maps.html | healthy | 4 |
| sovereign-music.html | http://localhost:8545/platform/sovereign-music.html | healthy | 3 |
| sovereign-news.html | http://localhost:8545/platform/sovereign-news.html | healthy | 2 |
| sovereign-public-affairs.html | http://localhost:8545/platform/sovereign-public-affairs.html | healthy | 6 |
| sovereign-search.html | http://localhost:8545/platform/sovereign-search.html | healthy | 4 |
| sovereign-status.html | http://localhost:8545/platform/sovereign-status.html | healthy | 4 |
| sovereign-telecom-command.html | http://localhost:8545/platform/sovereign-telecom-command.html | healthy | 4 |
| sovereign-video.html | http://localhost:8545/platform/sovereign-video.html | healthy | 4 |
| sovereignty-education.html | http://localhost:8545/platform/sovereignty-education.html | healthy | 4 |
| specialized-agencies.html | http://localhost:8545/platform/specialized-agencies.html | healthy | 3 |
| spike-office.html | http://localhost:8545/platform/spike-office.html | healthy | 3 |
| sports-betting.html | http://localhost:8545/platform/sports-betting.html | healthy | 5 |
| stack-empresarial.html | http://localhost:8545/platform/stack-empresarial.html | healthy | 4 |
| status.html | http://localhost:8545/platform/status.html | healthy | 4 |
| store-locator.html | http://localhost:8545/platform/store-locator.html | healthy | 3 |
| support-ai.html | http://localhost:8545/platform/support-ai.html | healthy | 4 |
| tax-authority.html | http://localhost:8545/platform/tax-authority.html | healthy | 6 |
| telecom-platform.html | http://localhost:8545/platform/telecom-platform.html | healthy | 4 |
| telepathy-platform.html | http://localhost:8545/platform/telepathy-platform.html | healthy | 3 |
| template-unified.html | http://localhost:8545/platform/template-unified.html | healthy | 3 |
| tenant-dashboard.html | http://localhost:8545/platform/tenant-dashboard.html | healthy | 5 |
| terraforming.html | http://localhost:8545/platform/terraforming.html | healthy | 8 |
| threat-oracle.html | http://localhost:8545/platform/threat-oracle.html | healthy | 4 |
| time-navigation.html | http://localhost:8545/platform/time-navigation.html | healthy | 4 |
| todo-propio.html | http://localhost:8545/platform/todo-propio.html | healthy | 3 |
| token-factory.html | http://localhost:8545/platform/token-factory.html | healthy | 3 |
| tradex.html | http://localhost:8545/platform/tradex.html | healthy | 3 |
| traductor.html | http://localhost:8545/platform/traductor.html | healthy | 2 |
| translator.html | http://localhost:8545/platform/translator.html | healthy | 5 |
| treasury-command.html | http://localhost:8545/platform/treasury-command.html | healthy | 3 |
| treaties.html | http://localhost:8545/platform/treaties.html | healthy | 4 |
| truth-protocol.html | http://localhost:8545/platform/truth-protocol.html | healthy | 3 |
| tsdb.html | http://localhost:8545/platform/tsdb.html | healthy | 2 |
| un-corazon-una-vida.html | http://localhost:8545/platform/un-corazon-una-vida.html | healthy | 5 |
| user-dashboard.html | http://localhost:8545/platform/user-dashboard.html | healthy | 3 |
| verificacion-antecedentes.html | http://localhost:8545/platform/verificacion-antecedentes.html | healthy | 3 |
| verificar-plataformas.html | http://localhost:8545/platform/verificar-plataformas.html | healthy | 3 |
| video-call.html | http://localhost:8545/platform/video-call.html | healthy | 3 |
| vigilancia-brain.html | http://localhost:8545/platform/vigilancia-brain.html | healthy | 3 |
| vip-transactions.html | http://localhost:8545/platform/vip-transactions.html | healthy | 3 |
| virtual-cards.html | http://localhost:8545/platform/virtual-cards.html | healthy | 6 |
| vision.html | http://localhost:8545/platform/vision.html | healthy | 3 |
| vms-gestion-video.html | http://localhost:8545/platform/vms-gestion-video.html | healthy | 4 |
| voip-telephone-platform.html | http://localhost:8545/platform/voip-telephone-platform.html | healthy | 3 |
| voting.html | http://localhost:8545/platform/voting.html | healthy | 16 |
| wallet.html | http://localhost:8545/platform/wallet.html | healthy | 4 |
| watchlist-alerta-proteccion.html | http://localhost:8545/platform/watchlist-alerta-proteccion.html | healthy | 4 |
| weather-engine.html | http://localhost:8545/platform/weather-engine.html | healthy | 3 |
| webcam-view.html | http://localhost:8545/platform/webcam-view.html | healthy | 5 |
| webcams-platform.html | http://localhost:8545/platform/webcams-platform.html | healthy | 3 |
| whatsapp-pos.html | http://localhost:8545/platform/whatsapp-pos.html | healthy | 5 |
| whistleblower.html | http://localhost:8545/platform/whistleblower.html | healthy | 3 |
| workflow-engine.html | http://localhost:8545/platform/workflow-engine.html | healthy | 8 |
| workforce-soberano.html | http://localhost:8545/platform/workforce-soberano.html | healthy | 6 |
| zero-point-energy.html | http://localhost:8545/platform/zero-point-energy.html | healthy | 3 |

---

*Report generated by scripts/test-toda-plataforma.js. Sovereign Government of Ierahkwa Ne Kanienke. Lang: es*
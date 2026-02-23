# REPORTE COMPLETO — PLATAFORMA BANCARIA IERAHKWA

**Fecha:** 2026-01-23  
**Fuentes:** `config/financial-hierarchy.js`, `config/platform-global.js`, `platform/data/platform-links.json`, `platform/data/platform-urls.js`, tests

---

## 1. DUPLICADOS CORREGIDOS

| Archivo | Problema | Corrección |
|---------|----------|------------|
| **config/platform-global.js** | REDIRECTS_EXTRA duplicaba `from` ya en REDIRECTS: `/crypto`, `/atm-manufacturing`, `/global-banking` | Eliminados de REDIRECTS_EXTRA (se mantienen solo en REDIRECTS) |
| **platform/data/platform-links.json** | `id` duplicado: `svc-documentflow` (order 99 y 127) | El segundo renombrado a `svc-documentflow-app` |

---

## 2. TESTS

| Suite | Resultado | Nota |
|-------|-----------|------|
| **unit.test.js** | ✅ 22 passed, 0 failed | M0–M4, validadores, Luhn, fees, formateo |
| **api.test.js** | ⚠️ 0 passed, 25 failed | `AbortController is not defined` (Node); requieren servidor en :8545 y red |

---

## 3. ESTRUCTURA DE RUTAS Y REDIRECTS

### 3.1 platform-global.js — ROUTES (canónicos)

- **Total ROUTES:** 86  
- **Categorías:** bank, settlement, trading, crypto, blockchain, defi, security, auth, other, ai, hardware, department  

**Rutas bancarias / financieras (path → file):**

| path | file | categoría |
|------|------|-----------|
| /central-banks | central-banks.html | bank |
| /siis | siis-settlement.html | settlement |
| /super-bank-global | super-bank-global.html | bank |
| /maletas | maletas.html | bank |
| /bdet-bank | bdet-bank.html | bank |
| /bank-worker | bank-worker.html | bank |
| /wallet | wallet.html | bank |
| /forex | forex.html | trading |
| /financial-instruments | financial-instruments.html | bank |
| /invoicer | invoicer.html | bank |
| /digital-vault | digital-vault.html | bank |
| /sistema-bancario | sistema-bancario.html | bank |
| /vip-transactions | vip-transactions.html | bank |
| /debt-collection | debt-collection.html | department |

### 3.2 REDIRECTS (alias → canónico)

- **Total REDIRECTS:** 22 (sin duplicados de `from`)
- Ejemplos: `/4-banks`→`/central-banks`, `/bdet`→`/bdet-bank`, `/banking`→`/bdet-bank`, `/mamey`→`/mamey-futures`, `/crypto`→`/bitcoin-hemp`, `/launchpad`→`/citizen-launchpad`, `/social-media.html`→`/social-media`.

### 3.3 platform-urls.js

- **Claves:** ~165 (objeto; sin duplicados de clave).
- Incluye: `app-ai-studio`→`/app-ai-studio`, `appai`→`/app-ai-studio`, `backup`→`/backup-department`, etc.

### 3.4 platform-links.json

- **Entradas:** 147 (tras corrección del `id` duplicado).
- **Secciones:** version-badges, dashboard, headerNav (hn-*), quickAccess (qa-*), services (svc-*).

---

## 4. JERARQUÍA FINANCIERA — VALORES

### 4.1 Nivel 1 — IFIs (Instituciones Financieras Internacionales)

| Institución | Código | Rol |
|-------------|--------|-----|
| SIIS - Sovereign Ierahkwa International Settlement | SIIS | Banco central de los bancos centrales |
| Ierahkwa Monetary Fund (IMF) | IMF-I | Estabilidad monetaria internacional |
| Ierahkwa Development Group (World Bank) | IDG | Desarrollo económico |

### 4.2 Nivel 2 — Clearing e infraestructura de pagos

| Sistema | Código | Estado | Valor/Detalle |
|---------|--------|--------|----------------|
| **IERAHKWA FUTUREHEAD BDET Clearing House** | IERCLRH | OPERATIONAL | Volumen diario: **$50B+** |
| **RTGS** (Real-Time Gross Settlement) | IERRTGS | OPERATIONAL | Mínimo **$10,000**; liquidación **<2 s** |
| **ACH** (Automated Clearing House) | IERACH | OPERATIONAL | Ciclos: 8:00, 12:00, 16:00, 20:00 |
| **WAMPUM** (Red de Tarjetas Soberana) | IERWMPUM | OPERATIONAL | Interchange 1.0%; internacional 1.5% |
| **SIIS Interno** | SIISINT | OPERATIONAL | Alternativa soberana a SWIFT |
| **Seguro de depósitos (FSD)** | FSD | — | **$250,000** por depositante por banco; Fondo **$5B USD** |
| **Fondo de Garantía Bancaria (FGB)** | FGB | — | **$10B USD** |

### 4.3 Nivel 3 — Regulación

| Entidad | Código | Detalle |
|---------|--------|---------|
| Superintendencia de Bancos (SBI) | SBI | Capital Tier 1 ≥8%; LCR 100%; leverage ≥3% |
| Fondo de Seguro de Depósitos (FSD) | FSD | **$250,000** por cuenta; Fondo **$5B** |
| Fondo de Garantía Bancaria (FGB) | FGB | Fondo **$10B** |
| Unidad de Inteligencia Financiera (UIF) | UIF | AML/KYC; FATF, Basel AML, Wolfsberg |
| Buró de Crédito IERAHKWA (BCI) | BCI | Score 300–850 |

### 4.4 Nivel 4 — Bancos centrales

**Funciones core:** Estabilidad de precios (meta 2–3% inflación), política monetaria, emisión de moneda, banco de bancos, gestión de divisas, supervisión.

**Instrumentos monetarios:**

| Instrumento | Valor |
|-------------|--------|
| Tasa de interés de referencia | **5.25%** (corredor 5.00–5.50%) |
| Encaje legal | **10%** |
| Ventana de descuento | **5.75%** |

**Instituciones:**

| Banco central | Código SWIFT | Región | Reservas (ejemplo) |
|---------------|--------------|--------|--------------------|
| BDET Central Bank | IERBDETXXX | Principal | Oro 50,000 oz; Crypto 10k BTC, 100k ETH; Fiat USD 5B; 500M SDRs |
| Banco Central Águila | IERAGUILAX | Norte | WPM, USD |
| Banco Central Quetzal | IERQUETZAX | Centro | WPM, MXN/GTQ |
| Banco Central Cóndor | IERCONDORX | Sur | WPM, monedas locales |
| Banco Central Caribe | IERCARIBXX | Caribe | WPM, USD |

### 4.5 Nivel 5 — Banca de desarrollo

| Institución | Código | Enfoque |
|-------------|--------|---------|
| Futurehead Development Bank | FHDEV | Infraestructura, tecnología, innovación |
| BANCOMEXT Ierahkwa | BCOMEXT | Comercio exterior, cartas de crédito |
| Banco de Desarrollo Agrícola | AGRIDEV | Agro, FarmFactory |
| Banco de Desarrollo Ciudadano | CITDEV | PyMEs, Citizen Launchpad, tokenización |

### 4.6 Nivel 6 — Bancos regionales

| Banco | Código | Región | Activos | Sucursales |
|-------|--------|--------|---------|------------|
| Banco Regional Águila | IERAGLREG | Norte | **$45,000M** | 150 |
| Banco Regional Quetzal | IERQTZREG | Centro | **$38,000M** | 200 |
| Banco Regional Cóndor | IERCNDREG | Sur | **$52,000M** | 180 |
| Banco Regional Caribe | IERCRBREG | Caribe | **$28,000M** | 95 |

**Rango de activos nivel 6:** $10,000M – $100,000M USD. Depósitos asegurados hasta **$250,000** por cuenta.

### 4.7 Nivel 7 — Bancos nacionales

**Licencias:**

| Licencia | Código | Capital mínimo | Alcance |
|----------|--------|----------------|---------|
| General | LG | **$10,000,000** | Nacional e internacional |
| Internacional | LI | **$3,000,000** | Solo offshore |
| Representación | LR | **$250,000** | Promoción y enlace |
| Microfinanzas | LM | **$1,000,000** | Nacional |
| Cooperativa | LC | **$500,000** | Local |

**Bancos oficiales (ejemplos):**

| Banco | Código | Sucursales | Licencia |
|-------|--------|------------|----------|
| Banco Nacional de Ierahkwa (BANCONAL) | IERBNCNAL | 95 | general |
| Caja de Ahorros Ierahkwa | IERCAJAHO | 60 | general |

**Bancos privados (ejemplos):**

| Banco | Código | Activos | Sucursales | Modelo |
|-------|--------|---------|------------|--------|
| Super Bank Global | IERSBGLOB | **$150,000M** | 45 | wholesale |
| Banco General Ierahkwa | — | — | — | — |

### 4.8 Licencias bancarias (BANKING_LICENSES)

| Tipo | code | capitalMinimo (USD) | scope |
|------|------|---------------------|-------|
| general | LG | 10,000,000 | national, international |
| international | LI | 3,000,000 | international |
| representation | LR | 250,000 | representation |
| microfinance | LM | 1,000,000 | national |
| cooperative | LC | 500,000 | local |

---

## 5. MODELOS DE NEGOCIO (BUSINESS_MODELS)

| Modelo | Código | Servicios principales |
|--------|--------|------------------------|
| retail | RETAIL | Wallet, Cuentas, Préstamos, Depósitos, Remesas; paths: /wallet, /bdet-accounts, /bank-worker |
| wholesale | WHOLESALE | Super Bank Global, SIIS, Clearing House, CryptoHost; paths: /super-bank-global, /siis, /cryptohost |
| trading | TRADING | Mamey Futures, TradeX, NET10, Forex, Derivados; paths: /mamey-futures, /tradex, /net10, /forex |

---

## 6. SERVICIOS PLATAFORMA (platform-global.SERVICES, extracto banca/trading)

| id | path | name | categoría |
|----|------|------|-----------|
| central-banks | /central-banks | 4 Bancos / Central Banks | bank |
| super-bank-global | /super-bank-global | Super Bank Global | bank |
| maletas | /maletas | Maletas | bank |
| bdet-bank | /bdet-bank | BDET Bank | bank |
| bdet-accounts | /bdet-accounts | Cuentas BDET | bank |
| wallet | /wallet | Wallet | bank |
| forex | /forex | Forex | trading |
| debt-collection | /debt-collection | Cobro de Deudas | bank |
| bank-worker | /bank-worker | Bank Worker | bank |
| siis | /siis | SIIS Settlement | settlement |
| financial-instruments | /financial-instruments | Financial Instruments | bank |
| invoicer | /invoicer | Invoicer | bank |
| digital-vault | /digital-vault | Digital Vault | bank |
| sistema-bancario | /sistema-bancario | Sistema Bancario | bank |
| citizen-launchpad | /citizen-launchpad | Citizen Launchpad | crypto |
| mamey | /mamey-futures | Mamey Futures | trading |
| net10 | /net10 | NET10 DeFi | defi |
| cryptohost | /cryptohost | CryptoHost | crypto |
| bridge | /bridge | Bridge | blockchain |
| dao | /dao | DAO | blockchain |
| ido-factory | /ido-factory | IDO Factory | crypto |
| farmfactory | /farmfactory | FarmFactory | defi |
| token-factory | /token-factory | Token Factory | crypto |

---

## 7. APIs Y RUTAS DE SERVIDOR

**Rutas /api/v1 (extracto banca/finanzas):**

- `/api/v1/financial-hierarchy`, `/level/:level`, `/business-model/:model`, `/institutions`, `/financial-hierarchy/licenses`
- `/api/v1/central-bank/functions`, `/instruments`, `POST /set-rate`, `POST /open-market`
- `/api/v1/banks/regional`, `/national`, `/region/:region`, `/license/:type`
- `/api/v1/clearing`, `POST /clearing/connect`, `/transactions`, `/status`
- `/api/v1/regulators`, `/api/v1/regulations`
- `POST /api/v1/siis/connect`, `/settlement`, `/status`
- `/api/v1/global/status`, `/protocols`, `/correspondents`, `POST /connect`
- `/api/v1/maletas/stats/summary`, `GET/POST /maletas`, `POST /maletas/:id/settle`
- `/api/platform-global` (ROUTES, REDIRECTS, SERVICES)

**Módulos montados:** `routes/financial-hierarchy.js`, `routes/global-maletas-stubs.js`.

---

## 8. VALORES CLAVE RESUMIDOS

| Concepto | Valor |
|----------|--------|
| Volumen diario Clearing House | **$50B+** |
| Seguro de depósitos por cuenta | **$250,000** |
| Fondo Seguro Depósitos (FSD) | **$5B** |
| Fondo Garantía Bancaria (FGB) | **$10B** |
| Tasa de interés de referencia | **5.25%** |
| Encaje legal | **10%** |
| Ventana de descuento | **5.75%** |
| RTGS mínimo | **$10,000** |
| Capital mínimo Licencia General | **$10,000,000** |
| Capital mínimo Licencia Internacional | **$3,000,000** |
| Capital mínimo Licencia Representación | **$250,000** |
| Activos Super Bank Global | **$150,000M** |
| Activos Bancos Regionales (rango) | **$28,000M – $52,000M** |
| Reservas BDET Central (oro) | **50,000 oz** |
| Reservas BDET Central (crypto) | **10,000 BTC, 100,000 ETH** |
| Reservas BDET Central (fiat) | **USD 5B equivalent** |

---

## 9. ARCHIVOS MODIFICADOS EN ESTA REVISIÓN

- `config/platform-global.js`: eliminados duplicados en REDIRECTS_EXTRA.
- `platform/data/platform-links.json`: `id` duplicado `svc-documentflow` → `svc-documentflow-app`.

---

*Generado a partir de `financial-hierarchy.js`, `platform-global.js`, `platform-links.json`, `platform-urls.js` y resultados de tests.*

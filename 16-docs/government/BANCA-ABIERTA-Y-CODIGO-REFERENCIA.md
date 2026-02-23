# Banca abierta, software bancario abierto e identificadores

**Sovereign Government of Ierahkwa Ne Kanienke** · Referencia  
Modelo de banca abierta (Open Banking), plataformas de código abierto para banca e identificadores estándar (código de entidad, BIC/SWIFT, IBAN).

---

## 1. Banca abierta (Open Banking)

**Qué es:** Modelo financiero que permite a los bancos **compartir datos de clientes con terceros autorizados** mediante **APIs seguras**.

| Aspecto | Descripción |
|---------|-------------|
| **Finalidad** | Que aplicaciones externas (fintechs, agregadores) ofrezcan servicios personalizados: préstamos rápidos, consolidación de cuentas en una sola plataforma, comparadores, etc. |
| **Control del usuario** | Los datos solo se comparten con **consentimiento explícito** del cliente; el cliente puede **revocar el acceso** en cualquier momento. |
| **Estándares** | En Europa, PSD2 (Payment Services Directive 2) regula el Open Banking; otras regiones tienen marcos similares (APIs estándar, autenticación fuerte). |

En Ierahkwa, la capa bancaria (BDET, banking-bridge, APIs) puede exponer APIs propias controladas por el gobierno soberano; el principio “todo propio” implica que los terceros autorizados y el consentimiento se gestionan dentro del ecosistema soberano, sin depender de agregadores externos no controlados.

---

## 2. Software bancario de código abierto

Plataformas cuya **base de código está disponible** para ser modificada y utilizada por instituciones financieras:

| Plataforma | Descripción |
|------------|-------------|
| **Open Bank Project** | Solución que ofrece **APIs de código abierto** alineadas con estándares globales (p. ej. PSD2 en Europa). Conecta fintechs con bancos mediante APIs estándar. |
| **MyBanco** | Herramienta **gratuita** para gestionar **transacciones básicas** y reducir costes operativos en modelos de negocio bancarios. |
| **Odoo Banking** | **Módulo** dentro del ERP **Odoo** que permite personalizar flujos de trabajo para **banca móvil** y operaciones bancarias integradas en el ERP. |

Referencia: en este proyecto el núcleo bancario está en **`RuddieSolution/node/banking-bridge.js`**, **`RuddieSolution/node/server.js`** (RPC, cuentas, tokens) y las interfaces en **`RuddieSolution/platform/bdet-bank.html`**, **support-ai.html** (transferencias, wire). No dependemos de Open Bank Project ni Odoo para el core; estas herramientas sirven como referencia de estándares y de qué existe en ecosistema abierto.

---

## 3. Identificadores bancarios (“códigos”)

Códigos numéricos/alfanuméricos usados en transacciones y identificación de entidades y cuentas:

| Código | Descripción |
|--------|-------------|
| **Código de entidad** | Número **único** (suele ser de **4 dígitos**) que identifica a **cada banco** dentro de un país. |
| **BIC / SWIFT** | Código **internacional** (Bank Identifier Code / Society for Worldwide Interbank Financial Telecommunication) necesario para **transferencias fuera del país** de forma segura (identifica banco y a veces sucursal). |
| **IBAN** | **International Bank Account Number**: número de cuenta internacional que incluye **código del país** y **código del banco**; permite identificar de forma unívoca la cuenta en transferencias (sobre todo en Europa y muchos otros países). |

En la plataforma: **support-ai.html** y flujos de wire permiten indicar **IBAN** y **SWIFT/BIC** para transferencias internacionales; **bdet-bank.html** y el backend referencian SWIFT, MT103 e ISO 20022. Los códigos de entidad se pueden usar internamente para identificar las entidades del sistema bancario soberano (p. ej. los 4 bancos centrales o BDET).

---

## Resumen

- **Open Banking:** APIs seguras, consentimiento del usuario, revocación; en Ierahkwa con control soberano de datos y terceros autorizados.
- **Software abierto de referencia:** Open Bank Project (APIs estándar), MyBanco (transacciones básicas), Odoo Banking (banca móvil en ERP).
- **Identificadores:** Código de entidad (banco en un país), BIC/SWIFT (internacional), IBAN (cuenta internacional).

Documentación técnica del sistema: `docs/INDEX-DOCUMENTACION.md`, `SISTEMA-BANCARIO-REPORTE-COMPLETO.md` (si existe); código: `RuddieSolution/node/banking-bridge.js`, `RuddieSolution/platform/bdet-bank.html`, `RuddieSolution/platform/support-ai.html` (wire/IBAN/SWIFT).

**Véase también:** Estándares de cifrado y APIs (AES, TLS, OAuth, JWE, ISO 20022, post-cuántico): `docs/ESTANDARES-CIFRADO-BANCA-ABIERTA.md`. CryptoHost y servidores globales: `docs/CRYPTOHOST-Y-SERVIDORES-GLOBALES-REFERENCIA.md`.

# MultiEstate (CodeCanyon) — Sin certificar · Real Estate multitenant soberano

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister**

---

## Posición: MultiEstate (CodeCanyon) no se certifica

**MultiEstate** es un producto comercial de CodeCanyon (Laravel 11, PHP 8.2): plataforma multitenant de bienes raíces (SAAS) donde el admin crea paquetes (mensual/anual/lifetime), los tenants compran paquetes y obtienen un sitio white-label de real estate, con IA (Gemini, Pollinations), 19 pasarelas de pago (PayPal, Stripe, Razorpay, etc.) y múltiples temas.

- **No lo certificamos ni lo integramos** en nuestra plataforma porque:
  - **Licencia de terceros** — Viola el principio **TODO PROPIO · NADA DE 3ra COMPAÑÍA** (ver `PRINCIPIO-TODO-PROPIO.md`). No dependemos de productos CodeCanyon ni de sus certificados.
  - **Pagos externos** — Depende de PayPal, Stripe, Authorize.net, Mollie, Razorpay, PayTm, etc. Nosotros usamos **nuestro banco (BDET)** y **pagos soberanos (IGT)** en nuestro Node.
  - **IA externa** — Usa Google Gemini y Pollinations.AI. En modo soberano usamos **IA propia** (Ollama / servicios soberanos) cuando `USE_SOBERANO=true`.

Por tanto: **sin certificar MultiEstate en nuestro node con nuestros servicios y banco** — no es nuestro stack ni nuestro principio.

---

## Cómo sí: Real Estate multitenant soberano (nuestro Node + servicios + banco)

La **idea** (admin → paquetes → tenants → sitios white-label de real estate) sí puede existir **certificada en nuestro Node, con nuestros servicios y banco**:

| Componente | En MultiEstate (CodeCanyon) | En nuestra plataforma soberana |
|------------|-----------------------------|---------------------------------|
| Backend | Laravel 11, PHP | **Node** (`RuddieSolution/node/server.js`) — Mamey Node |
| Pagos | 19 gateways (PayPal, Stripe, …) | **BDET + IGT** — `services/pagos-soberano.js`, `USE_SOBERANO=true` |
| Banco | N/A (gateways externos) | **Government Banking** — `modules/government-banking.js` (ISR: Ierahkwa Sovereign Realty) |
| Tokens / sector Real Estate | N/A | **IGT-REALTY** — `tokens/79-IGT-REALTY/`, ecosistema Futurehead |
| IA (contenido/imágenes) | Google Gemini, Pollinations | **IA soberana** — `api/ai-code-generator.js` con Ollama / servicios propios cuando `USE_SOBERANO=true` |
| Multitenant / paquetes | Laravel packages & tenants | **Implementación propia** en Node: tenants, planes (mensual/anual/lifetime), features por plan |
| Frontend / app listados | Temas PHP + posible app | **Estaty (Flutter)** conectado a nuestro Node — ver `docs/ESTATY-REAL-ESTATE-APP-NODE.md` |

### Servicios y rutas existentes que apoyan Real Estate soberano

- **Pagos:** `services/pagos-soberano.js` — `createPaymentIntent`, `createCheckoutSession` (IGT/BDET).  
  Uso desde `services/payments.js` cuando `USE_SOBERANO=true` (método `igt` o `card` con flujo soberano).
- **Banco / sector:** `modules/government-banking.js` — entidad **ISR (Ierahkwa Sovereign Realty)**, sector Real Estate.
- **Licencias:** `modules/license-authority.js` — tipo `REAL_ESTATE` para licencias de actividad inmobiliaria.
- **Ecosistema:** `node/data/ecosistema-futurehead.json`, `whitepaper-futurehead.json` — Dpt. Real Estate, FHTC, Bóveda Real Estate.
- **Token:** IGT-REALTY (id 79) en `node/data/platform-tokens.json`, `ierahkwa-futurehead-mamey-node.json`.

### Qué falta para cerrar un “MultiEstate soberano”

1. **API multitenant en el Node**
   - CRUD de **tenants** (organizaciones/sitios white-label).
   - **Paquetes/planes** (free, trial, premium; mensual/anual/lifetime) y **features por plan**.
   - **Suscripciones** de tenant a plan: checkout con `pagos-soberano` (IGT/BDET), sin gateways externos.
2. **API de listados**
   - Propiedades y proyectos por tenant (crear/editar/listar/filtrar).
   - Compatible con lo que espera **Estaty** (ver `ESTATY-REAL-ESTATE-APP-NODE.md`): propiedades, proyectos, agentes, consultas.
3. **Subdominio / path / custom domain**
   - Resolver tenant por subdominio, path o dominio propio (configuración en Node o reverse proxy).
4. **IA de contenido/imágenes**
   - Generación de textos e imágenes para propiedades/proyectos usando **solo** servicios soberanos (sin Gemini/Pollinations).

Cuando eso exista, la plataforma multitenant de real estate quedará **certificada en nuestro node con nuestros servicios y banco**, sin CodeCanyon ni pasarelas externas.

---

## Resumen

| Pregunta | Respuesta |
|----------|-----------|
| ¿Certificamos MultiEstate (CodeCanyon) en nuestro node? | **No.** No usamos ni certificamos productos CodeCanyon con nuestro Node, servicios ni banco. |
| ¿Podemos tener una plataforma multitenant de real estate “tipo MultiEstate”? | **Sí**, como **producto propio**: mismo flujo (admin, paquetes, tenants, white-label) sobre Node + BDET/IGT + pagos soberanos + IGT-REALTY + IA propia. |
| ¿Dónde se documenta la conexión de una app de listados (Estaty) al Node? | `docs/ESTATY-REAL-ESTATE-APP-NODE.md` |

**Conclusión:** MultiEstate (CodeCanyon) queda **sin certificar** en nuestra infraestructura. La alternativa soberana es construir (o completar) el módulo multitenant de real estate en el Node, usando solo nuestros servicios y banco.

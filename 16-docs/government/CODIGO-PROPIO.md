# Código propio — Definición y auditoría

**Sovereign Government of Ierahkwa Ne Kanienke**  
Principio: **TODO PROPIO · NADA DE 3ra COMPAÑÍA** (ver [PRINCIPIO-TODO-PROPIO.md](PRINCIPIO-TODO-PROPIO.md))

---

## Qué es “código propio”

- **Código propio:** todo lo escrito en este repo, servicios self-hosted en nuestra infraestructura, y uso solo de runtime/estándares (Node `crypto`, `fs`, `http`, etc.). **Sin certificados ni licencias ajenos** (ver PRINCIPIO-TODO-PROPIO.md).
- **No es código propio:** llamadas a APIs de Google, AWS, Stripe, Meta, Twitter, SendGrid, Twilio; cargar fuentes o scripts desde dominios externos (fonts.googleapis.com, cdn.jsdelivr.net, connect.facebook.net, platform.twitter.com); usar librerías npm que solo envuelven servicios de terceros; depender de licencias o certificados comerciales de terceros (CodeCanyon, WoWonder, QuickDate, etc.).

---

## Ya propio en el repo

| Área | Implementación |
|------|----------------|
| Criptografía | `crypto` nativo Node (ver regla .cursor) |
| Almacenamiento | `node/services/storage-soberano.js` — Sin AWS S3, sistema de archivos local |
| Email | `node/services/email-soberano.js` — Sin SendGrid, cola a disco o SMTP self-hosted |
| Reconocimiento facial | `node/services/face-recognition-propio.js` — Sin TensorFlow/Google/Regula |
| Red social backend | `node/modules/social-network.js` — IERAHKWA Sovereign Social, cero dependencias de redes externas |
| Plataforma | HTML/JS/CSS en `RuddieSolution/platform/`, estilos `assets/unified-styles.css`, lógica `assets/unified-core.js` |

---

## Dependencias externas actuales (alinear con código propio)

### 1. Fuentes (Google Fonts)

Varias páginas cargan fuentes desde `https://fonts.googleapis.com/...` (Orbitron, Exo 2, Inter, JetBrains Mono, etc.).

**Objetivo código propio:**  
- Descargar los `.woff2` / `.ttf` y servirlos desde `/platform/assets/fonts/` (o desde el mismo origen de la plataforma).  
- Sustituir el `<link href="https://fonts.googleapis.com/...">` por un `<link>` o `@font-face` apuntando a esos recursos locales.

### 2. Iconos (Bootstrap Icons vía jsDelivr)

Muchas páginas usan `https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.x/.../bootstrap-icons.css`.

**Objetivo código propio:**  
- Copiar el CSS y la carpeta de fuentes de Bootstrap Icons al repo (p. ej. `platform/assets/icons/`) y servirlos desde nuestro dominio.  
- O usar solo iconos SVG propios / un set mínimo de SVG en el repo.

### 3. Códigos de redes sociales (Facebook / Twitter)

En `RuddieSolution/platform/social-media-codes.html` se muestran ejemplos de código que incluyen:
- `connect.facebook.net/.../sdk.js`
- `platform.twitter.com/widgets.js`

**Objetivo código propio:**  
- Dejar claro en la página que esos ejemplos son **de integración con terceros** y **no cumplen principio propio** si se usan en producción.  
- Para compartir soberano: solo enlaces `https://...?u=URL` (sin cargar SDKs de Meta/Twitter) o implementación propia de share (p. ej. copiar enlace, abrir ventana con URL de share sin scripts externos).

### 4. Referencias a servicios de pago / nube (Stripe, Google)

- `node/middleware/webhook-verify.js`: usa `STRIPE_WEBHOOK_SECRET` (opcional). Para código propio estricto: no usar Stripe; flujos de pago con APIs propias.
- `node/banking-bridge.js`: texto que menciona “Google Authenticator” en instrucciones; se puede cambiar por “app TOTP compatible” o “app autenticación propia”.
- Algunos JSON/docs (p. ej. whitepaper, ecosistema-futurehead): mencionan Stripe como integración; en versión “todo propio” no depender de Stripe.
- `index.html`: etiquetas “GOOGLE API” en cards; sustituir por servicio propio o quitar si no hay uso real.

---

## Checklist “código propio”

- [ ] Fuentes: ninguna petición a `fonts.googleapis.com`; todas desde `/platform/assets/fonts/` (o mismo origen).
- [ ] Iconos: ninguna petición a `cdn.jsdelivr.net` para Bootstrap Icons; servir desde `/platform/assets/icons/` o SVG propio.
- [ ] Scripts en producción: ningún `connect.facebook.net`, `platform.twitter.com`, ni otros SDKs de redes externas.
- [ ] Criptografía: solo `crypto` (Node) o estándares equivalentes; sin librerías npm que llamen a servicios externos.
- [ ] Pagos / webhooks: sin Stripe ni otros proveedores externos; APIs propias o protocolos estándar self-hosted.
- [ ] Email: sin SendGrid ni SaaS; cola local o SMTP propio (ya cubierto en email-soberano).
- [ ] Almacenamiento: sin S3 ni blob externo; storage-soberano o equivalente propio.
- [ ] **Sin certificados ajenos:** sin licencias comerciales (CodeCanyon, WoWonder, QuickDate, PlayTube, etc.); SSL con PKI propia o self-signed, no obligación de CA comercial.

---

## Resumen

**Código propio** aquí significa: implementaciones en este repo, servidas desde nuestra infraestructura (**nuestros servicios y banco** en el Node, ver [SERVICIOS-NUESTRO-NODE.md](docs/SERVICIOS-NUESTRO-NODE.md)), sin depender de dominios o APIs de terceros (Google, AWS, Stripe, Meta, Twitter, jsDelivr para producción, etc.) y **sin certificados ni licencias ajenos**.  
Este documento y [PRINCIPIO-TODO-PROPIO.md](PRINCIPIO-TODO-PROPIO.md) son la referencia para revisar y seguir migrando a “todo propio, sin certificados”.

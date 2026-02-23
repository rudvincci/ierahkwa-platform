# Qué más se le puede implementar — Roadmap de implementaciones

**Fecha:** 2026-01-23  
**Base:** RuddieSolution (platform, node, config, mobile, services, deploy)

---

## Resumen de lo que ya tienes

| Área | Implementado |
|------|--------------|
| **Front** | 95+ HTML, security-global.js, platform-nav.js, platform-api.js, PWA (ierahkwa-pwa.js, sw.js), offline, unified-core, i18n, encrypted-comms, session-activity |
| **Backend** | Node (8545), /health, /ready, /live, proxy Banking/Editor/Platform-Casino, financial-hierarchy, global/maletas, backup, bridge, analytics, voting, gamification, notifications, membership, AI, WebRTC signaling |
| **Stubs** | /api/security/status, /api/platform-history/track/* |
| **Mobile** | Expo, Login, Home, Accounts, Transfer, Trading, Profile, CheckDeposit, Depository, api, websocket, authStore |
| **Deploy** | Docker, deploy/Dockerfile.node, docker-compose, fly, render, railway |
| **Docs** | OpenAPI, README, varios .md |

---

## 1. Sincronizar `platform-urls.js` con `platform-global`

**Estado:** ROUTES y REDIRECTS están en `config/platform-global.js`; `platform/data/platform-urls.js` es un objeto manual que puede quedar desfasado.

**Implementar:**
- Script que genere `platform-urls.js` (o `platform-urls.json`) desde `platform-global.ROUTES` y `REDIRECTS`: cada `path` → clave canónica; aliases desde REDIRECTS.
- Añadir en `platform-urls` las rutas nuevas: `playtube`, `pixelphoto`, `wowonder`, `block30` → `/social-media-department`; `social-media-department` → `/social-media-department`; `app-studio` → `/app-studio` (canónico); `secure-chat` y `video-call` ya en ROUTES canónicos.
- Endpoint opcional: `GET /api/platform-urls` que devuelva el mismo objeto (o desde `/api/platform-global` un campo `urls` derivado).

---

## 2. Notificaciones push (Web Push)

**Estado:** ` notifications/subscribe`, `unsubscribe`, `preferences`, `send` existen; falta el backend de **Web Push** (VAPID, subscription, envío real).

**Implementar:**
- `node/services/push-notifications.js`:  
  - Guardar suscripciones (almacén o DB).  
  - Endpoint `POST /api/v1/notifications/push/subscribe` (body: `{ endpoint, keys }`).  
  - Envío con `web-push` (VAPID).  
- Generar y guardar claves VAPID; exponer la clave pública en `/api/v1/notifications/push/vapid-public` o en el HTML.  
- En el front: `ierahkwa-pwa.js` o un `push-manager.js` que pida permiso, suscriba con `Service Worker` y registre en el backend.  
- Opcional: plantillas (alertas, vencimientos, transferencias, 2FA).

---

## 3. 2FA / Biometric en el flujo de login

**Estado:** Existe `auth-2fa-biometric.js` y `auth-system`; no está claro si el login HTML y el flujo de `login.html` / `bank-login.html` lo usan.

**Implementar:**
- En `login.html` y `bank-login.html`: tras usuario/password correctos, si el usuario tiene 2FA:
  - Pantalla/modal “Código 2FA” o “Usar biometría”.
  - Llamada a `POST /api/auth/2fa/verify` (TOTP) o a un endpoint de verificación biométrica.
- Endpoint `POST /api/auth/2fa/setup` (generar secreto TOTP, QR) y `POST /api/auth/2fa/disable` (con re-auth).  
- En mobile: uso de `expo-local-authentication` en `LoginScreen` después del login básico.

---

## 4. Panel de analíticas / BI para líderes

**Estado:** Hay `POST /api/v1/analytics/pageview`, `event`, `GET summary`, `realtime`; no hay un dashboard que los consuma para administradores.

**Implementar:**
- `platform/analytics-dashboard.html` (o `admin-analytics.html`):  
  - Gráficos (Chart.js, ApexCharts o similar): visitas por ruta, por hora, por dispositivo, eventos.  
  - Filtros: rango de fechas, sección, evento.  
  - Uso de `GET /api/v1/analytics/summary` y `realtime` (y, si se añade, `export`).  
- Ruta en `platform-global`: `/analytics-dashboard` o bajo `/admin`.  
- Opcional: `GET /api/v1/analytics/export?from=&to=&format=csv` para descargas.

---

## 5. Auditoría centralizada (audit log)

**Estado:** `AuditTrail` y `audittrail` en `analytics`; no hay un API unificado de “audit log” ni una pantalla que lo explote.

**Implementar:**
- `node/routes/audit.js` o lógica en un servicio:  
  - `POST /api/v1/audit` (interno): registro de acciones (login, transfer, cambio de rol, acceso a KMS, etc.) con: `userId`, `action`, `resource`, `metadata`, `ip`, `timestamp`.  
  - `GET /api/v1/audit?from=&to=&user=&action=&limit=` para consulta.  
- Almacenamiento: fichero rotado, DB o ambos.  
- `platform/audit-log.html`: tabla/filtros y export CSV, solo para roles admin/auditor.  
- En `server.js`: middleware que, para rutas sensibles, llame al servicio de audit tras la acción.

---

## 6. Límites de transferencia y políticas por producto

**Estado:** Lógica de negocio en Banking Bridge / .NET; en Mamey no hay un módulo claro de “límites” o “políticas”.

**Implementar:**
- Config (o DB): por tipo de producto (cuenta, wire, ACH, crypto) y por nivel de usuario:  
  - Límite diario, semanal, mensual; límite por operación.  
- `GET /api/v1/limits/policies` y `GET /api/v1/limits/usage?userId=` (o por cuenta).  
- Antes de ejecutar una transfer: comprobar `usage + amount <= limit`; si se supera, 403 con mensaje claro.  
- Opcional: pantalla en `bank-admin` o `wallet` para que el usuario vea sus límites y uso.

---

## 7. Firma de documentos (e-signature) integrada

**Estado:** Existe `/esignature` y ESignature en rutas; no hay un flujo unificado “subir PDF → enviar a firmantes → firma → descarga”.

**Implementar:**
- `POST /api/v1/esignature/upload` (documento); `POST /api/v1/esignature/send` (lista de firmantes, orden, email); `GET /api/v1/esignature/status/:id`; `POST /api/v1/esignature/sign` (token + firma).  
- Almacenar documentos y firmas (store local o S3) con retención configurable.  
- `platform/esignature.html` (o mejorar el existente): subir, configurar firmantes, ver estado, ver documento firmado.  
- Opcional: dibujo de firma en canvas o carga de imagen; sello de tiempo.

---

## 8. Recuperación de cuenta (forgot password / reset)

**Estado:** `login.html` y `bank-login` no muestran “¿Olvidaste tu contraseña?” ni flujo de reset.

**Implementar:**
- `POST /api/auth/forgot-password` (email): genera token de un solo uso (TTL 1h), envía link por email (SendGrid/SES o stub en dev).  
- `GET /platform/reset-password.html?token=...`: formulario “Nueva contraseña + repetir”.  
- `POST /api/auth/reset-password` (token, newPassword): valida token, actualiza contraseña, invalida token.  
- En `login.html` y `bank-login.html`: enlace “¿Olvidaste tu contraseña?” → `/platform/forgot-password.html` o similar.

---

## 9. Consentimiento y preferencias de privacidad (GDPR-style)

**Estado:** No hay flujo de consent (cookies, marketing, analíticas, sharing).

**Implementar:**
- `GET /api/v1/consent/preferences` y `PUT /api/v1/consent/preferences` (categorías: necessary, analytics, marketing, etc.).  
- `platform/privacy-preferences.html`: checkboxes por categoría, guardar, y si no hay consent previo, mostrar banner/modal en la primera visita.  
- `platform/assets/consent-banner.js`: lee si ya hay consent; si no, muestra banner y al aceptar/rechazar llama a `PUT /api/v1/consent` y guarda en `localStorage`/cookie.  
- En analíticas: no enviar `pageview`/`event` si el usuario no ha aceptado “analytics”.

---

## 10. Feature flags

**Estado:** No hay sistema de feature flags.

**Implementar:**
- Config (o DB) con flags: `{ id, name, enabled, percent, allowedUsers[] }`.  
- `GET /api/v1/feature-flags` (público o con Auth) devuelve para el usuario actual: `{ "newDashboard": true, "betaCasino": false }`.  
- En el front: `if (window.IerahkwaFlags?.newDashboard) { ... }` o helper.  
- En `platform-global` o en un `features.json`: default flags; en producción se sobreescriben desde DB o env.

---

## 11. API versionado y deprecación

**Estado:** Rutas tipo `/api/v1/...`; no hay cabeceras ni reglas de deprecación.

**Implementar:**
- Cabecera de respuesta `X-API-Version: 1` y `X-API-Deprecation: ...` cuando la ruta esté deprecada.  
- En OpenAPI: `deprecated: true` y `x-sunset` (fecha) para rutas viejas.  
- Plan (futuro): ` /api/v2/...` con un router; redirigir o avisar en v1.

---

## 12. WebRTC: TURN/STUN y política de medios

**Estado:** `webrtc-signaling` en `/ws/signaling`; la videollamada puede fallar en redes restrictivas si no hay TURN.

**Implementar:**
- Servidor TURN (coturn) o uso de un TURN público/contratado; config `ICE_SERVERS` en el cliente ( `platform/video-call.html` o `encrypted-comms`).  
- Opcional: `GET /api/v1/webrtc/ice-servers` que devuelva `{ urls, username, credential }` según env.  
- En `video-call`: policy de medios (solo audio, solo vídeo, o ambos) según permisos o preferencia del usuario.

---

## 13. Mobile: profundizar pantallas y notificaciones

**Estado:** Pantallas stub (Accounts, Transfer, Trading, etc.) y `expo-notifications` en `package.json`.

**Implementar:**
- En cada pantalla: al menos una llamada a `api.ts` y algo de UI (lista, formulario, gráfico simple).  
- `expo-notifications`: `registerForPushNotificationsAsync`, enviar `expoPushToken` al backend; en el backend, usar SDK de Expo para enviar a ese token.  
- Deep links: `ierahkwa://transfer`, `ierahkwa://notifications` para abrir la pantalla concreta.  
- Biometric en `LoginScreen` (ya tienes `expo-local-authentication`): opción “Iniciar con biometría” si ya hay sesión previa.

---

## 14. PWA: manifest y scope

**Estado:** `platform/sw.js`, `ierahkwa-pwa.js`; `manifest.json` en platform.  
**Implementar:**
- Revisar `manifest.json`: `scope: "/"` o `"/platform/"` según dónde esté el sw; `start_url`, iconos 192/512, `display: standalone`, `theme_color`, `background_color`.  
- En `sw.js`: que la lista `STATIC_ASSETS` incluya las rutas canónicas nuevas (`/app-studio`, `/social-media-department`, `/secure-chat`, `/video-call`).  
- En `ierahkwa-pwa.js`: `showInstallPrompt` conectado a un botón “Instalar app” en `platform/index.html` o en el header.

---

## 15. Búsqueda global en la plataforma

**Estado:** No hay búsqueda unificada.

**Implementar:**
- `platform/assets/global-search.js`: caja de búsqueda en header (o en `platform-nav`).  
- Fuentes: `ROUTES` (name, path), `platform-urls`, o `GET /api/platform-global` (services + routes).  
- Filtro en el cliente por nombre; al elegir ítem, `location.href = path`.  
- Opcional: `GET /api/v1/search/suggest?q=` que devuelva sugerencias desde routes + DB si hay entidades indexadas.

---

## 16. Mantenimiento / modo “read-only”

**Estado:** No hay un “modo mantenimiento” o “solo lectura”.

**Implementar:**
- Variable de entorno `MAINTENANCE_MODE=1` o `READ_ONLY=1`.  
- Middleware: si `MAINTENANCE_MODE`, responder 503 con `platform/maintenance.html` en todas las rutas salvo `/health`, `/ready`, `/live` y estáticos básicos.  
- Si `READ_ONLY`: en `POST`/`PUT`/`DELETE` de APIs sensibles, responder 503 con `{ maintenance: true, readOnly: true }`.  
- `platform/maintenance.html`: mensaje y cuenta atrás opcional si se conoce la ventana.

---

## 17. Reportes programados (PDF/email)

**Estado:** `pdf-reports.js` existe; no está claro si hay tareas programadas que envíen informes.

**Implementar:**
- Cola (Bull/BullMQ con Redis o similar) o `node-cron`: trabajos “diario”, “semanal” que generen:  
  - Resumen de transacciones, backups, alertas.  
- `pdf-reports` para generar PDF; `email.js` o SendGrid para enviar.  
- Endpoint `POST /api/v1/reports/schedule` (solo admin) para crear/cancelar un reporte recurrente.  
- Opcional: `platform/reports-scheduled.html` para ver y gestionar estos reportes.

---

## 18. Módulo de quejas y reclamaciones (tickets)

**Estado:** `service-desk`, `Support AI`; no hay un flujo claro de “abrir ticket” con estados y asignación.

**Implementar:**
- `POST /api/v1/tickets`, `GET /api/v1/tickets`, `GET /api/v1/tickets/:id`, `PATCH /api/v1/tickets/:id` (estado, asignación, comentarios).  
- Estados: open, in_progress, waiting, resolved, closed.  
- `platform/tickets.html` o sección en `service-desk`: formulario “Nueva queja/reclamación”, lista de mis tickets, detalle.  
- Notificaciones al cambiar estado (in-app o push si ya hay push).

---

## 19. Integración con `social-media-department`

**Estado:** Ruta `/social-media-department` y redirects `playtube`, `pixelphoto`, `wowonder`, `block30`; el HTML existe.  
**Implementar:**
- En `social-media-department.html`: enlaces o iframes (según política) a PlayTube, PixelPhoto, WoWonder, Block 30, o landing por producto.  
- Si hay APIs propias de esas apps: `GET /api/v1/social-media-department/playtube/...` como proxy o agregador.  
- En `platform-urls` y `unified-platforms.json`: entradas para `playtube`, `pixelphoto`, `wowonder`, `block30`, `social-media-department`.

---

## 20. Testing E2E y de integración

**Estado:** `api.test.js` (requiere servidor y sufre `AbortController`); `unit.test.js` bien.

**Implementar:**
- Playwright o Cypress: 3–5 flujos E2E: login, navegar a BDET, hacer una transfer (mock), abrir secure-chat.  
- En `api.test.js`: polyfill de `AbortController` si Node < 18, o usar `node-fetch` con `signal` compatible; que los tests arranquen un servidor en un puerto fijo o usen `before/after` con `child_process`.  
- `npm run test:e2e` y que CI (GitHub Actions) ejecute E2E en un job con servidor.

---

## Prioridad sugerida (orden de impacto / esfuerzo)

| Prioridad | Ítem | Motivo |
|-----------|------|--------|
| **P0** | Sincronizar platform-urls con platform-global (1) | Evita enlaces rotos y duplicar mantenimiento. |
| **P0** | Recuperación de cuenta – forgot/reset (8) | Básico en cualquier login. |
| **P1** | 2FA / Biometric en login (3) | Seguridad alta. |
| **P1** | Notificaciones push (2) | Mejora retención y alertas. |
| **P1** | Límites de transferencia (6) | Compliance y control de riesgo. |
| **P2** | Audit log centralizado (5) | Compliance y trazabilidad. |
| **P2** | Analytics dashboard para líderes (4) | Decisión y producto. |
| **P2** | Consentimiento / privacidad (9) | Regulación y confianza. |
| **P3** | Feature flags (10), E-signature (7), TURN WebRTC (12), Mobile (13), PWA (14), Búsqueda global (15), Mantenimiento (16), Reportes programados (17), Tickets (18), Social-media-department (19), E2E (20). | Mejoran UX, operación y escalabilidad. |

---

## Checklist rápido por categoría

- **Seguridad:** 2FA en login (3), Límites (6), Audit (5), Recuperación de cuenta (8).  
- **UX / Front:** Búsqueda global (15), PWA/manifest (14), Consent (9), E-signature (7).  
- **Backend / Infra:** Audit (5), Feature flags (10), Mantenimiento (16), Reportes (17), Tickets (18), API versionado (11).  
- **Mobile:** Pantallas reales, push, deep links, biometría (13).  
- **Compliance / Legal:** Consent (9), Audit (5), E-signature (7).  
- **DevOps / Calidad:** E2E (20), platform-urls sync (1).

Si indicas 2–3 ítems (por número o nombre), se puede bajar a tareas concretas (archivos, endpoints y cambios de UI) paso a paso.

# Algo más para implementar

Lista priorizada de mejoras y features que puedes añadir al proyecto Ierahkwa.

---

## ✅ Ya implementado en esta sesión

- **/ready** extendido: incluye Live Connect + Rust/Go/Python (opcional).
- **CORS** configurable por `CORS_ORIGIN` en `.env`.
- **X-Request-Id**: middleware para trazabilidad (header `X-Request-Id`).

---

## Prioridad ALTA (producción)

| # | Qué | Dónde | Notas |
|---|-----|--------|--------|
| 1 | **JWT secrets en .env** | `node/.env` | Nunca usar valores por defecto en prod. |
| 2 | **HTTPS/WSS** | Nginx / Cloudflare | Terminar TLS en proxy; Node sigue en HTTP. |
| 3 | **LIVE_REQUIRE_AUTH=1** | `node/.env` | Obligar auth para canales kms/ml en Live Connect. |
| 4 | **CORS restringido** | `node/.env` | `CORS_ORIGIN=https://tudominio.gov` en prod. |

---

## Prioridad MEDIA (observabilidad y DX)

| # | Qué | Dónde | Notas |
|---|-----|--------|--------|
| 5 | **Request-ID en logs** | `node/logging` | Incluir `req.id` en cada log (ya existe header X-Request-Id). |
| 6 | **Health de dependencias en /ready** | `node/server.js` | ✅ Implementado: Live Connect + Rust/Go/Python en `/ready`. |
| 7 | **Alertas** | PagerDuty / Slack | Alertar si /ready devuelve 503 o /health falla. |
| 8 | **Docker Compose full stack** | raíz | Un `docker-compose.yml` con Node + Rust + Go + Python. |
| 9 | **Backup automático de state** | `scripts/` | Script que guarde `state` (blockchain) y clave maestra KMS. |

---

## Prioridad MEDIA (seguridad)

| # | Qué | Dónde | Notas |
|---|-----|--------|--------|
| 10 | **Helmet CSP** | `node/server.js` | Activar `contentSecurityPolicy` con whitelist. |
| 11 | **API Key para server-to-server** | `node/middleware` | Alternativa a JWT para servicios internos. |
| 12 | **Rotación de secrets** | docs | Procedimiento para rotar JWT/KMS sin downtime. |

---

## Prioridad BAJA (features)

| # | Qué | Dónde | Notas |
|---|-----|--------|--------|
| 13 | **Reconnect en live-connect.html** | `node/public/live-connect.html` | Reconexión WebSocket con backoff. |
| 14 | **Webhook signature** | `node/middleware` | Verificar firma (HMAC) en webhooks entrantes. |
| 15 | **Changelog / API versioning** | `docs/` | CHANGELOG.md y prefijo `/api/v2` cuando rompas compat. |
| 16 | **PWA / Service Worker** | `platform/` | Offline básico para dashboard. |
| 17 | **i18n completo** | `platform/` | Todas las cadenas en JSON por idioma. |

---

## Referencias

- **GO-LIVE-CHECKLIST.md** – Pasos para ir a producción.
- **FALTANTES-PARA-PRODUCCION.md** – Gaps de negocio/infra (MameyNode, SICB, etc.).
- **node/.env.example** – Variables de entorno.

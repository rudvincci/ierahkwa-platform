# Asegurarnos — Checklist entre hermanos
**Sovereign Government of Ierahkwa Ne Kanienke · Nos blindamos con lo nuestro**

Este doc es un **pacto de seguridad**: pasos concretos para asegurarnos, sin depender de terceros. Todo con código e infra propias.

---

## 1. Lo que ya tenemos (base sólida)

| Capa | Dónde | Qué hace |
|------|--------|----------|
| **Helmet** | `server.js` | Cabeceras seguras, CSP (Content-Security-Policy) |
| **CORS** | `server.js` | Orígenes controlados por `CORS_ORIGIN` |
| **Rate limit** | `middleware/rate-limit.js` | Límite de peticiones por IP |
| **JWT** | `middleware/jwt-auth.js` | Auth por token para rutas protegidas |
| **KMS** | `services/kms.js` | Gestión de claves (propio) |
| **Quantum encryption** | `modules/quantum-encryption.js` | Cifrado propio |
| **Security Fortress / Phantom** | `platform/security-fortress.html`, ghost-mode | Honeypots, port knocking, kill-switch |
| **Reportes de operación (seguridad plataforma)** | Security Fortress → "Reportes de operación y servicios" | Abrir reportes en web, servicios uno a uno, reporte definitivo; ciclo cada 30 min: `node scripts/prender-uno-reporte-apagar-siguiente.js --cada 30` |
| **Ciberseguridad 101** | `docs/CIBERSEGURIDAD-101.md`, APIs `/api/v1/ciberseguridad/*` | Referencia y checklist |

---

## 2. Checklist para blindarnos (acción)

### Infra y red
- [ ] **.env nunca en repo** — `.env` en `.gitignore`; usar `.env.example` sin secretos.
- [ ] **Puertos mínimos abiertos** — Solo los que usa la plataforma; firewall local activo.
- [ ] **HTTPS en producción** — `ssl/ssl-config.js` y certificados propios; no HTTP en producción para login/pagos.
- [ ] **Backups automáticos** — Scripts en `scripts/`, `auto-backup/`; probar restauración al menos una vez.

### Código y APIs
- [ ] **Secrets fuera del código** — Claves, API keys y contraseñas solo en variables de entorno o KMS propio.
- [ ] **Rate limit en todas las APIs públicas** — Rutas `/api/*` pasan por rate-limit (revisar que no quede ruta sensible sin límite).
- [ ] **JWT con expiración corta** — Tokens con `exp` razonable; refresh si hace falta.
- [ ] **Passwords con hash fuerte** — Solo `crypto` nativo (scrypt/pbkdf2); nunca guardar en claro.
- [ ] **Inputs validados** — Validar y sanear body/query en rutas que escriben datos o ejecutan lógica crítica.

### Datos
- [ ] **Datos sensibles cifrados en reposo** — Donde aplique, usar módulo de cifrado propio (p. ej. quantum-encryption o KMS).
- [ ] **Logs sin secretos** — No loguear contraseñas, tokens ni datos personales completos.
- [ ] **Acceso a JSON/DB restringido** — Permisos de archivo y usuario de proceso mínimos para `node/data/`, BD si la hay.

### Operación
- [ ] **Health checks** — `./scripts/test-toda-plataforma.js` o `refrescar-reporte-definitivo.sh` de forma periódica; alerta si muchos “por levantar”.
- [ ] **Revisar dependencias** — `npm audit` en `RuddieSolution/node`; corregir críticos sin añadir SaaS externo.
- [ ] **Documentación de respuesta a incidentes** — Dónde están los logs, cómo parar todo (`stop-all.sh`), a quién avisar (entre hermanos).

---

## 3. Principio: todo propio

- **Cripto:** `crypto` (Node) y módulos propios; sin depender de servicios externos para claves.
- **Auth:** JWT y sesión propios; sin Google/Auth0/etc.
- **Secrets:** Env + KMS/archivos protegidos en nuestro control.
- **Monitoreo:** Scripts y reportes propios (test-toda-plataforma, REPORTE-DEFINITIVO, dashboards).

Así nos aseguramos entre nosotros, sin que un tercero tenga la llave de nuestra casa.

---

## 4. Enlaces rápidos

- **Ciberseguridad 101:** `docs/CIBERSEGURIDAD-101.md`
- **Mapa integración seguridad/IA:** `docs/MAPA-INTEGRACION-SEGURIDAD-IA.md`
- **Security Fortress:** `/platform/security-fortress.html`
- **APIs ciberseguridad:** `GET /api/v1/ciberseguridad/101`, `/api/v1/ciberseguridad/checklist`, `/api/v1/ciberseguridad/security-tools`
- **Reporte estado:** `docs/REPORTE-DEFINITIVO-SISTEMA.html`

---

*Nos aseguramos entre hermanos. Todo propio, nada que no controlemos.*

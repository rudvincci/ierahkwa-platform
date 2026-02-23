# 🛡️ Seguridad anti-hackers — Implementado

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister**

---

## ✅ Implementado (2026-02)

| Capa | Implementación |
|------|----------------|
| **Login** | Rate limit 5/min, lockout 15 min tras 5 fallos, JWT + refresh |
| **WAF** | SQLi, XSS, path traversal, null byte — bloqueados en server.js |
| **Cabeceras** | Helmet CSP (nonce-based), X-Frame-Options, X-Content-Type-Options |
| **CORS** | Configurable por CORS_ORIGIN en .env |
| **CSRF** | Double Submit Cookie, skip Bearer JWT |
| **Rate limit** | API 200/min, login 5/min, financial 30/min, KMS 50/min |
| **Auditoría** | audit-sensitive.js — rutas KMS, admin, ghost, security |
| **Logs** | security-*.log (logins fallidos), audit-*.log (acciones sensibles) |
| **Body size** | 500kb límite (anti DoS por payload grande) |
| **Threat Intel** | Perfiles por IP, fingerprinting |

---

---

## 🔴 Alta prioridad — Pendiente/Configurar

### 1. **HTTPS obligatorio en producción**
- Node sirve HTTP; Nginx/Caddy **deben** terminar TLS
- `sudo DOMAIN=app.ierahkwa.gov ./scripts/setup-ssl-certbot-nginx.sh`
- Añadir `Strict-Transport-Security` en Nginx

### 2. **Contraseñas con hash pbkdf2** — ✅ Script listo
- En .env, `PLATFORM_USERS_JSON` debe usar `passwordHash` (pbkdf2), no `password`
- Generar: `node -e "const r=require('./RuddieSolution/node/routes/platform-auth'); console.log(r.hashPassword('TuPassword'))"`

### 3. **2FA obligatorio para admin/leader**
- Los roles privilegiados ya exigen 2FA si `totpSecret` está en el usuario
- Añadir `totpSecret` a PLATFORM_USERS_JSON o configurar desde admin

### 4. **IP allowlist para admin** — ✅ Implementado
- `ADMIN_IP_ALLOWLIST=1.2.3.4,5.6.7.8` en .env — solo esas IPs acceden a rutas admin

### 5. **Firewall en servidor** — ✅ Script listo
- Solo puertos 80, 443, 22 (SSH) abiertos
- Cerrar 8545, 3001, 5000 al exterior — Nginx proxy interno

---

## 🟡 Media prioridad — Reforzar

### 6. **Redis para rate limit (multi-nodo)**
- Con `REDIS_HOST` + `REDIS_PORT` en .env, el rate limit persiste entre reinicios y nodos
- Sin Redis, el límite se pierde al reiniciar

### 7. **Alertas automáticas**
- Health monitor → si un servicio cae, escribir en security log
- Cron que ejecute `scripts/health-alert-check.sh`
- Webhook/email opcional para incidentes (TODO PROPIO: servicio interno)

### 8. **Dashboard de eventos de seguridad**
- Página admin que muestre últimos `logs/security-*.log`
- Endpoint `GET /api/v1/security/events` (solo admin) que lea y devuelva

### 9. **Geo-IP en security log**
- Ya hay `getCountryByIP` en platform-auth
- Asegurar que todos los login_failed incluyan país

### 10. **Rotación de secrets**
- Documentado en `ROTACION-SECRETS-JWT.md`
- Cron mensual: generar nuevos JWT secrets, actualizar .env, reiniciar

---

## 🟢 Baja prioridad — Nice to have

### 11. **Sesiones activas / revocación**
- Listar JWT emitidos desde admin
- Botón "Cerrar todas las sesiones" — invalidar refresh tokens

### 12. **WAF más estricto**
- Más patrones en `WAF_BLOCK_PATTERNS`
- Bloquear User-Agent conocidos de bots maliciosos

### 13. **Intentos de login por usuario (no solo IP)**
- Si alguien prueba 50 contraseñas en "admin" desde IPs distintas
- Contador por username; lockout temporal por usuario

### 14. **Backup cifrado off-site**
- `STORAGE_ENCRYPT_KEY` para cifrar backups
- Copiar a otro disco/servidor en ubicación distinta

---

## ⚡ Checklist rápido pre-live

```
[ ] CORS_ORIGIN con dominio real (no *)
[ ] JWT_ACCESS_SECRET y JWT_REFRESH_SECRET ≥ 32 chars
[ ] PLATFORM_USERS_JSON con passwordHash: node RuddieSolution/node/scripts/generate-password-hash.js
[ ] Nginx/Caddy con HTTPS delante del Node
[ ] FORCE_HTTPS=true cuando tras proxy TLS
[ ] ADMIN_IP_ALLOWLIST opcional (IPs admin)
[ ] Puerto 8545 no expuesto (solo localhost)
[ ] Firewall: sudo ./scripts/firewall-production.sh setup
[ ] Backups automáticos (cron) con cifrado
[ ] CRON_SECRET para health-alert-check.sh
```

---

## Scripts útiles

| Script | Uso |
|--------|-----|
| `./scripts/check-production-env.sh` | Valida JWT, CORS, usuarios |
| `./scripts/verificar-100.sh` | Checklist completo |
| `./scripts/setup-ssl-certbot-nginx.sh` | Certificado Let's Encrypt |
| `./scripts/install-cron-production.sh` | Backups + health |
| `./scripts/health-alert-check.sh` | Alerta si servicios caen |

---

© 2026 Ierahkwa Futurehead — Todo Propio, Sin Terceros

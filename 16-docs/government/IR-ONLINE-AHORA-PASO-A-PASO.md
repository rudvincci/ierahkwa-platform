# Ir online ahora — Paso a paso (para hacer ya)

**Sovereign Government of Ierahkwa Ne Kanienke**  
Sigue los pasos en orden. Cada uno tiene un comando o acción concreta.

---

## Paso 1 — Tener el archivo .env

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution/node"
```

Si **no** existe `.env`:

```bash
cp .env.example .env
```

Abre `.env` en el editor (no lo subas a Git).

- [ ] Tengo el archivo `.env` en `RuddieSolution/node`

---

## Paso 2 — Secretos JWT en .env

En la terminal, genera dos claves (copia cada resultado):

```bash
openssl rand -hex 32
```

```bash
openssl rand -hex 32
```

En `.env` pon (reemplazando por los valores que te salieron):

```
JWT_ACCESS_SECRET=el_primer_valor_de_64_caracteres
JWT_REFRESH_SECRET=el_segundo_valor_de_64_caracteres
```

Guarda el archivo.

- [ ] JWT_ACCESS_SECRET y JWT_REFRESH_SECRET están en .env (32+ caracteres cada uno)

---

## Paso 3 — CORS (dominio permitido) en .env

En `.env` busca o añade `CORS_ORIGIN`:

- **Probar en tu máquina:**  
  `CORS_ORIGIN=http://localhost:8545`

- **Dominio real (producción):**  
  `CORS_ORIGIN=https://app.ierahkwa.gov`  
  (sin barra al final; usa tu dominio)

Guarda el archivo.

- [ ] CORS_ORIGIN está definido en .env

---

## Paso 4 — Contraseñas de acceso (recomendado)

**Opción rápida** — En `.env` añade:

```
PLATFORM_LEADER_PASSWORD=tu_password_seguro
PLATFORM_ADMIN_PASSWORD=otro_password_seguro
```

**Opción producción (hash)** — Genera el hash:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution/node"
node -e "const c=require('crypto'); console.log(c.createHash('sha256').update('TU_PASSWORD_AQUI').digest('hex'))"
```

Luego en `.env` pon `PLATFORM_USERS_JSON` con ese hash (ver `.env.example` o PASO-A-PASO-PARA-IR-LIVE.md Paso 4 Opción B).

Guarda el archivo.

- [ ] Tengo contraseñas (leader/admin) o PLATFORM_USERS_JSON en .env

---

## Paso 5 — Comprobar .env y enlaces

Desde la raíz del proyecto (donde está `GO-LIVE-PRODUCTION.sh`):

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/RuddieSolution/node"
npm run production:check
```

Debe salir **OK** y **0 Fallos**.

Verificar enlaces:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
node scripts/verificar-links.js
```

Debe salir **0 enlaces rotos**. Si hay rotos, corrígelos antes de seguir.

- [ ] production:check OK
- [ ] verificar-links.js → 0 rotos

---

## Paso 6 — Iniciar todo (GO-LIVE)

Desde la raíz del proyecto:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
./GO-LIVE-PRODUCTION.sh
```

Lee lo que imprima; si pide confirmación, responde. Al terminar, Node (8545) y Banking Bridge (3001) estarán en marcha.

- [ ] GO-LIVE-PRODUCTION.sh terminó sin errores

---

## Paso 7 — Verificar que está en vivo

Abre en el navegador:

1. **Plataforma:**  
   http://localhost:8545/platform

2. **Health:**  
   http://localhost:8545/health  
   → debe responder algo como `"ok": true` o `"status": "healthy"`

3. **VIP Transactions (opcional):**  
   http://localhost:8545/vip-transactions  
   → pulsa Actualizar y deberían verse datos o mensaje coherente.

- [ ] La plataforma carga en el navegador
- [ ] /health responde OK

---

## Paso 8 — (Opcional) Backups y cron

Si quieres backups y comprobaciones de salud automáticas:

```bash
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
./scripts/install-cron-production.sh
```

Sigue lo que indique el script.

- [ ] (Opcional) Cron de backups/health instalado

---

## Paso 9 — (Cuando tengas dominio) HTTPS

1. En el servidor donde corre la plataforma: instalar Nginx (o Caddy).
2. Configurar el proxy: tu dominio (ej. `https://app.ierahkwa.gov`) → `http://127.0.0.1:8545`.
3. Certificado SSL (Let's Encrypt o el tuyo).
4. En `.env`: `CORS_ORIGIN=https://app.ierahkwa.gov` (tu dominio HTTPS).

Detalle: `RuddieSolution/node/ENV-PRODUCTION-SETUP.md`, `RuddieSolution/nginx/nginx.conf`.

- [ ] (Cuando aplique) HTTPS y proxy configurados

---

## Resumen

| Paso | Acción |
|------|--------|
| 1 | Tener `.env` en RuddieSolution/node (cp .env.example .env si no existe) |
| 2 | JWT_ACCESS_SECRET y JWT_REFRESH_SECRET en .env (openssl rand -hex 32) |
| 3 | CORS_ORIGIN en .env (localhost:8545 o tu dominio) |
| 4 | Contraseñas leader/admin o PLATFORM_USERS_JSON en .env |
| 5 | `npm run production:check` y `node scripts/verificar-links.js` → todo OK, 0 rotos |
| 6 | `./GO-LIVE-PRODUCTION.sh` desde la raíz |
| 7 | Abrir http://localhost:8545/platform y http://localhost:8545/health |
| 8 | (Opcional) ./scripts/install-cron-production.sh |
| 9 | (Con dominio) Nginx + SSL + CORS_ORIGIN con tu dominio |

Si en el paso 5 todo está OK y en el paso 6 el script termina sin errores, **estás online**.

---

*Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister · One Love, One Life.*

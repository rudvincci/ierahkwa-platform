# Qué tienes que dar para poner todo live online

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister**

Para que la plataforma esté **online** (internet, con dominio y HTTPS), esto es lo que **tú** tienes que tener o darme/configurar.

---

## 1. Servidor (máquina donde corre todo)

| Qué | Descripción |
|-----|-------------|
| **Servidor** | Una máquina (VPS, dedicado o tu propio rack) con IP pública o detrás de un balanceador. Linux recomendado (Ubuntu 22.04 LTS o similar). |
| **Acceso** | SSH con usuario que pueda usar `sudo` (para nginx, certbot, puertos 80/443). |
| **Mínimo** | 2 GB RAM, 2 CPU, 20 GB disco (para Node + Bridge + nginx). Para producción seria: 4+ GB RAM, 4+ CPU. |

**Lo que yo necesito de ti:** que el código esté desplegado en ese servidor (git clone o copia) y que los puertos **80** (HTTP) y **443** (HTTPS) estén abiertos hacia internet.

---

## 2. Dominio y DNS

| Qué | Descripción |
|-----|-------------|
| **Dominio** | Un dominio que controles, por ejemplo: `ierahkwa.gov`, `app.ierahkwa.gov`, `www.inkg.org`, etc. |
| **DNS** | Registros tipo **A** (o CNAME) apuntando al **IP del servidor** donde corre la app. |

Ejemplo:
- `app.ierahkwa.gov` → A → `123.45.67.89` (IP del servidor)
- `api.ierahkwa.gov` → A → misma IP (o CNAME a `app.ierahkwa.gov`)
- `platform.ierahkwa.gov` → A → misma IP

Para Let's Encrypt (certificado gratis) los dominios **tienen que estar ya apuntando** a ese servidor antes de ejecutar certbot.

**Lo que me tienes que dar:**
- **Dominio principal** (ej. `app.ierahkwa.gov` o `ierahkwa.gov`).
- **Email** para avisos de Let's Encrypt (ej. `admin@ierahkwa.gov`).

---

## 3. Variables de entorno (`.env` en producción)

Todo esto va en `RuddieSolution/node/.env` en el servidor. **Tú** (o quien administre) debe definir:

| Variable | Qué dar / generar | Ejemplo |
|----------|-------------------|---------|
| **JWT_ACCESS_SECRET** | 32+ caracteres aleatorios | `openssl rand -hex 32` |
| **JWT_REFRESH_SECRET** | Otro valor 32+ caracteres | `openssl rand -hex 32` |
| **SOVEREIGN_MASTER_KEY** | 64 caracteres hex (32 bytes) | `openssl rand -hex 32` (x2 o 64 chars) |
| **CORS_ORIGIN** | URL pública de la plataforma | `https://app.ierahkwa.gov` |
| **API_ORIGIN** | URL base de la API | `https://api.ierahkwa.gov` o `https://app.ierahkwa.gov` |
| **PLATFORM_DOMAIN** | Misma que CORS o dominio principal | `https://app.ierahkwa.gov` |

Opcional pero recomendado:
- **STORAGE_ENCRYPT_KEY** — 64 hex: `openssl rand -hex 32`
- **INTERNAL_SERVICE_TOKEN** — para cron/health: `openssl rand -hex 24`
- **PLATFORM_USERS_JSON** — usuarios de back office (leader/admin) con `passwordHash` (pbkdf2), no contraseña en claro.

**Lo que me tienes que dar:**  
Los valores que quieras usar para **dominio y URLs** (CORS_ORIGIN, API_ORIGIN, PLATFORM_DOMAIN). Los secrets (JWT, SOVEREIGN_MASTER_KEY) los puede generar quien despliegue en el servidor; no hace falta que me los pases a mí, solo que estén en `.env`.

---

## 4. SSL (HTTPS)

Opciones:

**A) Let's Encrypt (recomendado, gratis)**  
- Requiere: dominio apuntando al servidor + nginx instalado.  
- Comando (en el servidor):  
  `sudo DOMAIN=app.ierahkwa.gov EMAIL=admin@ierahkwa.gov ./scripts/setup-ssl-certbot-nginx.sh`  
- **Lo que me das:** `DOMAIN` y `EMAIL` (los mismos de la sección 2).

**B) Certificado propio (self-signed o PKI interna)**  
- Si ya tienes `cert.pem` y `key.pem`, se configuran en `RuddieSolution/node/ssl/` y en nginx.  
- **Lo que me das:** ruta o contenido del cert y de la key (o que estén en el servidor en una ruta conocida).

---

## 5. Resumen: qué me tienes que dar

| # | Qué | Formato / ejemplo |
|---|-----|--------------------|
| 1 | **Dominio principal** | `app.ierahkwa.gov` o `ierahkwa.gov` |
| 2 | **Email (SSL/avisos)** | `admin@ierahkwa.gov` |
| 3 | **Confirmación DNS** | “Ya están creados los A (o CNAME) hacia la IP del servidor” |
| 4 | **URL pública de la plataforma** | `https://app.ierahkwa.gov` (para CORS_ORIGIN, API_ORIGIN, PLATFORM_DOMAIN) |
| 5 | **Acceso al servidor** | SSH (o confirmar que el código y scripts están en el servidor y quién ejecutará los comandos) |

Con eso se puede:
- Configurar `.env` con CORS/API/PLATFORM según tu dominio.
- Ejecutar en el servidor: verificación, GO-LIVE y (si aplica) script de SSL con tu `DOMAIN` y `EMAIL`.
- Dejar la plataforma **live online** con HTTPS.

---

## 6. Si quieres que yo lo deje listo (resumen ejecutivo)

Dime:

1. **Dominio** (ej. `app.ierahkwa.gov`).
2. **Email** para certificado (ej. `admin@ierahkwa.gov`).
3. Que **DNS** ya apunta al servidor (o la IP del servidor si quieres que te diga qué registros crear).
4. Si el **código ya está en el servidor** (git clone o copia) y si tienes **nginx** instalado (para HTTPS con Let's Encrypt).

Con eso te devuelvo:
- Los valores exactos para `CORS_ORIGIN`, `API_ORIGIN`, `PLATFORM_DOMAIN` en `.env`.
- La secuencia de comandos a ejecutar en el servidor (verificar, GO-LIVE, SSL con tu dominio y email).

*Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister*

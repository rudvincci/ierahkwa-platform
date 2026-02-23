# Certificados SSL/TLS — Plataforma Ierahkwa

Guía de certificados para HTTPS en producción: rutas, obtención, renovación y configuración nginx.

---

## 1. Rutas usadas en nginx (producción)

El load balancer y los `server` blocks de nginx esperan:

| Archivo | Ruta típica en el servidor |
|---------|----------------------------|
| **Certificado (público)** | `/etc/ssl/certs/ierahkwa.gov.crt` |
| **Clave privada** | `/etc/ssl/private/ierahkwa.gov.key` |

Configuración de referencia: `DEPLOY-SERVERS/nginx-load-balancer.conf` (ierahkwa.gov, platform.ierahkwa.gov, node.ierahkwa.gov, api.ierahkwa.gov).

**Permisos recomendados:**
- `ierahkwa.gov.crt`: `644` (root:root o root:nginx).
- `ierahkwa.gov.key`: `600` u `640`, solo root (y nginx si aplica). No exponer la clave.

---

## 2. Opción A — Let's Encrypt (certbot)

Certificados gratuitos y reconocidos por los navegadores. Renovación automática cada ~90 días.

### Instalación (Debian/Ubuntu)

```bash
apt-get update
apt-get install -y certbot python3-certbot-nginx
```

### Obtener certificados para todos los dominios

```bash
certbot --nginx -d ierahkwa.gov -d www.ierahkwa.gov -d platform.ierahkwa.gov -d api.ierahkwa.gov -d node.ierahkwa.gov -d rpc.ierahkwa.gov
```

Certbot suele guardar en:
- Certificado: `/etc/letsencrypt/live/ierahkwa.gov/fullchain.pem`
- Clave: `/etc/letsencrypt/live/ierahkwa.gov/privkey.pem`

Para usar las **mismas rutas** que el nginx actual (`/etc/ssl/certs/` y `/etc/ssl/private/`), puedes enlazar o copiar después de emitir:

```bash
# Tras certbot --nginx, enlazar a las rutas que usa nginx
sudo ln -sf /etc/letsencrypt/live/ierahkwa.gov/fullchain.pem /etc/ssl/certs/ierahkwa.gov.crt
sudo ln -sf /etc/letsencrypt/live/ierahkwa.gov/privkey.pem /etc/ssl/private/ierahkwa.gov.key
sudo nginx -t && sudo systemctl reload nginx
```

O bien editar `nginx-load-balancer.conf` para apuntar directamente a las rutas de Let's Encrypt (por ejemplo `ssl_certificate /etc/letsencrypt/live/ierahkwa.gov/fullchain.pem;`).

### Renovación automática

```bash
# Probar renovación (dry-run)
certbot renew --dry-run

# Cron ya instalado por certbot (comprobar)
# /etc/cron.d/certbot o systemd timer certbot.timer
systemctl status certbot.timer
```

Tras renovar, recargar nginx: `certbot renew --post-hook "systemctl reload nginx"` o un script en `renewal-hooks/deploy/`.

---

## 3. Opción B — Certificado propio (self-signed)

Para entornos internos o pruebas sin CA pública. Los navegadores mostrarán advertencia.

```bash
sudo mkdir -p /etc/ssl/private /etc/ssl/certs
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout /etc/ssl/private/ierahkwa.gov.key \
  -out /etc/ssl/certs/ierahkwa.gov.crt \
  -subj "/CN=ierahkwa.gov/O=Sovereign Government of Ierahkwa Ne Kanienke/C=US"
sudo chmod 600 /etc/ssl/private/ierahkwa.gov.key
```

Las rutas coinciden con las que usa `nginx-load-balancer.conf`.

---

## 4. Opción C — Certificado comercial (CA externa)

Si usas un certificado comprado (DigiCert, Sectigo, etc.):

1. Recibir del proveedor: `.crt` (o `.pem`) y la clave privada (o CSR + clave).
2. Colocar en el servidor:
   - Certificado: `/etc/ssl/certs/ierahkwa.gov.crt`
   - Clave: `/etc/ssl/private/ierahkwa.gov.key`
3. `chmod 600` para la clave; recargar nginx.
4. Anotar fecha de vencimiento y renovar según política del proveedor.

---

## 5. Scripts implementados

| Script | Uso |
|--------|-----|
| **`scripts/setup-ssl-certbot-nginx.sh`** | En el servidor de producción (con nginx): instala certbot, obtiene Let's Encrypt para todos los dominios, crea symlinks en `/etc/ssl/certs/` y `/etc/ssl/private/`, recarga nginx, instala hook de renovación. Ejecutar con **sudo**. |
| **`scripts/setup-ssl-selfsigned.sh`** | Genera certificado self-signed en `ssl/certs/ierahkwa.gov.crt` y `ssl/private/ierahkwa.gov.key` (desarrollo). Con `DEST=system sudo` escribe en `/etc/ssl/`. |
| **`scripts/setup-ssl.sh`** | Menú interactivo: opciones 1–3 (Docker, standalone, self-signed) y opción **4** que invoca `setup-ssl-certbot-nginx.sh` para producción. |

**`status.sh`** muestra si hay certificados (en `/etc/ssl/` o en `ssl/` del repo).

## 6. Resumen rápido

| Objetivo | Comando / Acción |
|----------|------------------|
| **Producción (servidor con nginx)** | `sudo scripts/setup-ssl-certbot-nginx.sh` |
| **Producción con dominio público (manual)** | Let's Encrypt: `certbot --nginx -d ierahkwa.gov -d platform.ierahkwa.gov -d api.ierahkwa.gov -d node.ierahkwa.gov` |
| **Probar renovación** | `certbot renew --dry-run` |
| **Self-signed (desarrollo)** | `./scripts/setup-ssl-selfsigned.sh` o opción 3 de `./scripts/setup-ssl.sh` |
| **Rutas en nginx** | Cert: `/etc/ssl/certs/ierahkwa.gov.crt` — Key: `/etc/ssl/private/ierahkwa.gov.key` |

---

## 7. Referencias en el repo

| Recurso | Ubicación |
|---------|-----------|
| Nginx load balancer (SSL) | `DEPLOY-SERVERS/nginx-load-balancer.conf` |
| Ejemplo HTTPS reverse proxy | `DEPLOY-SERVERS/HTTPS-REVERSE-PROXY-EXAMPLE.md` |
| Deploy y certbot | `DEPLOY-SERVERS/DEPLOY-GUIDE.md` (sección certificados SSL) |
| GO-LIVE (HTTPS) | `docs/GO-LIVE-CHECKLIST.md` |
| PKI propia (futuro) | `RuddieSolution/platform/docs/STACK-EMPRESARIAL-Y-SERVICIOS.md` (PKI soberana) |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*

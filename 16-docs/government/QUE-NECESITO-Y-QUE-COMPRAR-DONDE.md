# Qué necesito de ti · Qué comprar y dónde

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister**

Respuesta directa: qué tienes que dar y qué tienes que comprar (y en qué sitio).

---

## Parte 1 — Qué necesito de ti (datos y acceso)

| # | Qué | Ejemplo / qué dar |
|---|-----|--------------------|
| 1 | **Dominio principal** | Ej. `app.ierahkwa.gov` o `ierahkwa.gov` |
| 2 | **Email** (para SSL y avisos) | Ej. `admin@ierahkwa.gov` |
| 3 | **Confirmación DNS** | Que los registros A (o CNAME) del dominio apunten a la **IP del servidor** donde corre la plataforma |
| 4 | **URL pública** (para .env) | Ej. `https://app.ierahkwa.gov` (sin barra al final) — para CORS_ORIGIN, API_ORIGIN, PLATFORM_DOMAIN |
| 5 | **Acceso al servidor** | SSH con usuario que pueda usar `sudo` (para nginx, certbot, puertos 80/443), o confirmar quién ejecutará los comandos |

Los **secretos** (JWT, SOVEREIGN_MASTER_KEY, etc.) los genera quien despliegue en el servidor con `openssl rand -hex 32`; no hace falta que me los pases, solo que estén en `.env`.

---

## Parte 2 — Qué comprar y dónde

### 1. Dominio (nombre del sitio)

| Dónde comprar | Web | Notas |
|---------------|-----|--------|
| **Namecheap** | namecheap.com | .com, .org, etc. |
| **Cloudflare** | cloudflare.com/products/registrar | Barato, DNS incluido |
| **Porkbun** | porkbun.com | Precios buenos |
| **GoDaddy** | godaddy.com | Soporte en español |
| **Squarespace** (antes Google Domains) | squarespace.com/domains | Búsqueda sencilla |

**Qué hacer:** Registrar el dominio y luego en el panel DNS crear un registro **A** apuntando a la **IP del servidor** (la que contrates abajo).

---

### 2. Servidor (donde corre Node + plataforma)

| Dónde comprar | Web | Tipo | Precio aprox. |
|---------------|-----|------|----------------|
| **Hetzner** | hetzner.com | VPS / dedicado | Barato Europa |
| **OVH** | ovh.com | VPS / dedicado | Europa / LATAM |
| **DigitalOcean** | digitalocean.com | VPS (Droplet) | ~5–6 USD/mes |
| **Vultr** | vultr.com | VPS | Por hora/mes |
| **Linode (Akamai)** | linode.com | VPS | Planes bajos |
| **Contabo** | contabo.com | VPS | Muy económico |

**Mínimo:** 2 GB RAM, 2 CPU, 20 GB disco.  
**Producción seria:** 4+ GB RAM, 4+ CPU.

**Qué hacer:** Contratar un VPS con **Ubuntu 22.04 LTS**, tener la **IP pública**, instalar Node.js, subir el proyecto (git clone o copia), configurar `.env` y ejecutar `./GO-LIVE-PRODUCTION.sh`. Puertos **80** y **443** abiertos hacia internet.

---

### 3. SSL (HTTPS) — normalmente no compras

| Opción | Dónde | Coste |
|--------|--------|--------|
| **Let's Encrypt** | letsencrypt.org (vía certbot en el servidor) | **Gratis** |
| **Cloudflare** (proxy + SSL) | cloudflare.com | Plan gratis con SSL |

No hace falta comprar certificado si usas Let's Encrypt o Cloudflare. En el servidor: nginx + certbot con tu dominio y email.

---

### 4. (Opcional) Correo para alertas/notificaciones

| Dónde | Web | Notas |
|-------|-----|--------|
| **Resend** | resend.com | Para desarrolladores |
| **SendGrid** | sendgrid.com | Plan gratis con límite |
| **Mailgun** | mailgun.com | API transaccional |
| **Amazon SES** | aws.amazon.com/ses | Barato si usas AWS |

Solo si quieres envío de correo desde la plataforma (alertas, restablecer contraseña, etc.). Se configura con API key o SMTP en `.env`.

---

## Resumen rápido

| Qué | Comprar/contratar | Dónde |
|-----|--------------------|--------|
| **Dominio** | Sí | Namecheap, Cloudflare, Porkbun, GoDaddy, Squarespace |
| **Servidor (VPS)** | Sí | Hetzner, OVH, DigitalOcean, Vultr, Linode, Contabo |
| **SSL (HTTPS)** | No (gratis) | Let's Encrypt + certbot en el servidor, o Cloudflare |
| **Correo** | Opcional | Resend, SendGrid, Mailgun, SES |

**Orden sugerido:**  
1) Comprar **dominio** → 2) Comprar **servidor (VPS)** → 3) Apuntar el dominio (DNS A) a la IP del servidor → 4) En el servidor: instalar Node, nginx, certbot, configurar `.env`, ejecutar GO-LIVE y script SSL.

---

## Lo que me das a mí (para dejar todo listo)

1. **Dominio** (ej. `app.ierahkwa.gov`).  
2. **Email** para certificado (ej. `admin@ierahkwa.gov`).  
3. Confirmación de que **DNS ya apunta al servidor** (o la IP del servidor para decirte qué registros crear).  
4. Si el **código ya está en el servidor** y si tienes **nginx** instalado (para HTTPS con Let's Encrypt).

Con eso se te puede devolver la secuencia exacta de comandos y los valores de CORS_ORIGIN / API_ORIGIN / PLATFORM_DOMAIN para tu `.env`.

---

*Detalle completo: `docs/DONDE-IR-Y-QUE-COMPRAR-PARA-LIVE.md`, `QUE-DAR-PARA-LIVE-ONLINE.md`.*

*Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister · One Love, One Life.*

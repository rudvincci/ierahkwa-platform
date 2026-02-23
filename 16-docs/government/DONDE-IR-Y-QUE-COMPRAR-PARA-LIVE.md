# Dónde ir y qué comprar para ir en vivo

**Sovereign Government of Ierahkwa Ne Kanienke**  
Guía práctica: qué necesitas contratar o comprar y dónde hacerlo.

---

## 1. Dominio (nombre de tu sitio)

**Qué es:** La dirección que usarás (ej. `ierahkwa.gov`, `app.ierahkwa.gov`).

**Dónde comprarlo / registrarlo:**

| Proveedor | Web | Notas |
|-----------|-----|--------|
| **Namecheap** | namecheap.com | Dominios .com, .org, etc. |
| **Google Domains** (ahora Squarespace) | squarespace.com/domains | Búsqueda y registro sencillo |
| **Cloudflare** | cloudflare.com/products/registrar | Precios bajos, DNS incluido |
| **GoDaddy** | godaddy.com | Muy conocido, soporte en español |
| **Porkbun** | porkbun.com | Precios competitivos |

**Qué hacer:** Elegir el nombre, registrarlo y luego configurar los DNS (apuntar al servidor donde corra la plataforma). Si es un dominio .gov puede haber requisitos especiales según el país.

---

## 2. Servidor (donde corre el Node y la plataforma)

**Qué es:** Una máquina (VPS o servidor dedicado) donde ejecutas `GO-LIVE-PRODUCTION.sh` y tienes siempre encendido el servicio.

**Dónde comprarlo / contratarlo:**

| Proveedor | Web | Tipo | Notas |
|-----------|-----|------|--------|
| **DigitalOcean** | digitalocean.com | VPS | Droplets desde ~5–6 USD/mes, documentación buena |
| **Linode (Akamai)** | linode.com | VPS | Planes desde bajos, buena red |
| **Vultr** | vultr.com | VPS | Varias regiones, precios por hora/mes |
| **Hetzner** | hetzner.com | VPS / dedicado | Precios bajos en Europa |
| **AWS** | aws.amazon.com | VPS (EC2) y más | Escalable, más complejo |
| **Google Cloud** | cloud.google.com | VPS (Compute Engine) | Similar a AWS |
| **OVH** | ovh.com | VPS y dedicado | Opción fuerte en Europa/LATAM |
| **Contabo** | contabo.com | VPS | Muy económico |

**Qué hacer:** Contratar un VPS (Ubuntu 22.04 o similar), instalar Node.js, subir tu proyecto, configurar `.env` y ejecutar el script de producción. En el repo hay referencias a DigitalOcean en `RuddieSolution/deploy/digitalocean.md`.

---

## 3. Certificado SSL (HTTPS)

**Qué es:** Lo que hace que la web use `https://` y el navegador marque la conexión como segura.

**Dónde conseguirlo:**

| Opción | Dónde | Coste |
|--------|--------|--------|
| **Let's Encrypt** | letsencrypt.org | **Gratis** |
| **Cloudflare** (proxy + SSL) | cloudflare.com | Plan gratis con SSL |
| **Certificado de pago** | Tu mismo proveedor de dominio (Namecheap, GoDaddy, etc.) o Comodo, DigiCert | De pago |

**Qué hacer:** En un VPS con Nginx o Caddy suele usarse **Let's Encrypt** (certbot). El repo tiene referencias en `RuddieSolution/node/ENV-PRODUCTION-SETUP.md` y scripts de SSL. No hace falta “comprar” si usas Let's Encrypt o Cloudflare gratis.

---

## 4. (Opcional) Correo para alertas y notificaciones

**Qué es:** Un servicio de envío de correo para alertas, restablecer contraseñas, etc.

**Dónde contratarlo:**

| Proveedor | Web | Notas |
|-----------|-----|--------|
| **SendGrid** | sendgrid.com | Plan gratis con límite de envíos |
| **Mailgun** | mailgun.com | API para correo transaccional |
| **Resend** | resend.com | Orientado a desarrolladores |
| **Amazon SES** | aws.amazon.com/ses | Barato si ya usas AWS |

**Qué hacer:** Crear cuenta, obtener API key o SMTP y poner en `.env` las variables que use el Node para envío de correo (si las tienes definidas en el código).

---

## 5. Resumen: orden sugerido

| Paso | Dónde ir | Qué comprar/contratar |
|------|----------|------------------------|
| 1 | Registrador de dominios (Namecheap, Cloudflare, etc.) | **Dominio** (ej. tu-dominio.com o .gov según procedimiento) |
| 2 | Proveedor VPS (DigitalOcean, Hetzner, Vultr, etc.) | **Servidor** (VPS mensual o anual) |
| 3 | Let's Encrypt / Certbot en tu servidor (o Cloudflare) | **SSL** (normalmente gratis) |
| 4 | (Opcional) SendGrid, Mailgun, etc. | **Correo** para alertas/notificaciones |

No hace falta comprar hardware físico si usas VPS: todo se contrata por internet (dominio + servidor en la nube). El código y los pasos de configuración ya están en el repo; tú aportas dominio, servidor y, si quieres, servicio de correo.

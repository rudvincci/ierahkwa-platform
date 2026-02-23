# SSL/TLS con Let's Encrypt — IERAHKWA

Configuración de certificados para HTTPS en nginx. **Crítico para producción.**

## 0. Primera vez (sin certificados)

nginx exige que existan los ficheros `ssl_certificate` y `ssl_certificate_key`. Si `/etc/letsencrypt` está vacío o no existe, nginx no arrancará.

- **Opción 1:** Obtener certificados con `certbot --standalone` (parar nginx antes) y luego levantar nginx con los volúmenes de LetsEncrypt.
- **Opción 2:** Usar certificados autofirmados en `nginx/ssl` (sección 4) y, si hace falta, un `server` 443 en `nginx.conf` que los use hasta tener Let's Encrypt.

## 1. Rutas que usa nginx

En `nginx/nginx.conf` los bloques `server` (puerto 443) esperan:

```
ssl_certificate     /etc/letsencrypt/live/ierahkwa.gov/fullchain.pem;
ssl_certificate_key /etc/letsencrypt/live/ierahkwa.gov/privkey.pem;
ssl_trusted_certificate /etc/letsencrypt/live/ierahkwa.gov/chain.pem;
```

El primer `server_name` debe coincidir con el directorio bajo `live/` (certbot usa el primer `-d` como nombre de carpeta).

## 2. Opción A: certbot en el host (recomendado)

### Requisitos

- `certbot` instalado: `apt install certbot` / `brew install certbot`
- Nginx sirviendo `/.well-known/acme-challenge/` desde `/var/www/certbot` (ya está en `nginx.conf`)
- Dominio apuntando al host (DNS A/AAAA)

### Pasos

1. **Montar webroot y LetsEncrypt en nginx (Docker)**

   En `docker-compose.yml`, servicio `nginx`, `volumes`:

   ```yaml
   volumes:
     - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
     - ./nginx/ssl:/etc/nginx/ssl:ro
     - /var/www/certbot:/var/www/certbot:ro
     - /etc/letsencrypt:/etc/letsencrypt:ro
     - nginx_logs:/var/log/nginx
   ```

2. **Crear webroot en el host**

   ```bash
   sudo mkdir -p /var/www/certbot
   sudo chown $USER /var/www/certbot
   ```

3. **Obtener certificados (con nginx levantado)**

   ```bash
   export LETSENCRYPT_EMAIL=admin@ierahkwa.gov
   export SSL_DOMAINS="ierahkwa.gov api.ierahkwa.gov"
   ./scripts/ssl-letsencrypt.sh
   ```

   O manual:

   ```bash
   sudo certbot certonly --webroot -w /var/www/certbot \
     -d ierahkwa.gov -d api.ierahkwa.gov \
     --email admin@ierahkwa.gov --agree-tos --no-eff-email
   ```

4. **Reiniciar nginx** para que cargue los certificados:

   ```bash
   docker compose restart nginx
   ```

### Renovación

```bash
sudo certbot renew
docker compose exec nginx nginx -s reload
```

Cron (ej. diario a las 03:00):

```
0 3 * * * certbot renew --quiet --deploy-hook "docker compose -f /path/to/docker-compose.yml exec -T nginx nginx -s reload"
```

## 3. Opción B: certbot standalone (sin nginx en 80)

Si nginx no puede servir en 80 aún:

1. Parar nginx: `docker compose stop nginx`
2. `sudo certbot certonly --standalone -d ierahkwa.gov -d api.ierahkwa.gov --email admin@ierahkwa.gov --agree-tos`
3. Añadir los volúmenes de `/etc/letsencrypt` y `/var/www/certbot` a nginx
4. Levantar nginx de nuevo

## 4. Desarrollo / staging (autofirmados)

Para pruebas sin dominio público:

```bash
mkdir -p nginx/ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/privkey.pem -out nginx/ssl/fullchain.pem \
  -subj "/CN=localhost"
```

Luego en `nginx.conf` (solo en entornos de prueba) se puede usar un `include` condicional o cambiar temporalmente:

```
ssl_certificate     /etc/nginx/ssl/fullchain.pem;
ssl_certificate_key /etc/nginx/ssl/privkey.pem;
```

(nginx ya monta `./nginx/ssl` en `/etc/nginx/ssl`.)

## 5. Resumen

| Qué              | Dónde / comando |
|------------------|------------------|
| Certificados     | `/etc/letsencrypt/live/ierahkwa.gov/` |
| Script auxiliar  | `scripts/ssl-letsencrypt.sh` |
| Webroot ACME     | `/var/www/certbot` |
| Renovación       | `certbot renew` + `nginx -s reload` |

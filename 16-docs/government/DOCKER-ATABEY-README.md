# Infraestructura ATABEY Fortress (Docker Compose)

Servicios soberanos: mensajería (Matrix), censo (Baserow), nube (Nextcloud), seguridad (Wazuh).  
Alineado con buenas prácticas de [Ciberseguridad 101](https://www.sentinelone.com/es/cybersecurity-101/) (SentinelOne); ver **docs/CIBERSEGURIDAD-101.md** para el mapa completo.

## Servicios

| Servicio             | Contenedor        | Puerto(s)           | Uso                    |
|----------------------|-------------------|---------------------|------------------------|
| Matrix Synapse       | atabey_messenger  | 8008                | Comunicación (Matrix)  |
| Baserow              | atabey_database   | 8080                | Base de datos / censo  |
| Nextcloud            | atabey_cloud      | 8081                | Oficina y nube        |
| Wazuh Manager        | atabey_guardian   | 1514, 1515, 55000   | SIEM / monitoreo      |
| Nginx Proxy Manager  | atabey_gateway    | 80, 81, 443         | Gateway & firewall    |

## Uso

```bash
# Levantar todos los servicios
docker-compose up -d

# Ver contenedores (tabla: nombre, estado, puertos)
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Ver logs
docker-compose logs -f

# Parar
docker-compose down
```

## Primera vez

- **Matrix Synapse**: la primera vez puede requerir generar configuración (`docker run -it --rm -v ./synapse_data:/data matrixdotorg/synapse:latest generate_signing_key` o ver [docs Synapse](https://matrix-org.github.io/synapse/latest/setup/installation.html)).
- **Baserow**: crear usuario admin en http://localhost:8080 la primera vez.
- **Nextcloud**: completar el asistente en http://localhost:8081 (usuario admin y datos).
- **Wazuh**: suele usarse con Wazuh Dashboard (Elastic Stack) para la UI; el manager solo recibe agentes y eventos.
- **Nginx Proxy Manager**: panel de administración en http://localhost:81 — login por defecto `admin@example.com` / `changeme` (cambiar en el primer acceso). Desde ahí configuras proxy inverso, SSL (Let’s Encrypt) y reglas de firewall.

## Datos locales

Los volúmenes `./synapse_data`, `./baserow_data`, `./nextcloud_data` guardan datos en el host. Conviene añadirlos a `.gitignore` si no quieres versionar datos:

```
synapse_data/
baserow_data/
nextcloud_data/
nginx_data/
letsencrypt/
sovereign_backups/
```

## Backup nocturno (bunker soberano)

El script `sovereign-backup.sh` clona Census, Vault, Messenger y Gateway a `sovereign_backups/` (y opcionalmente a un bunker offline). Ver **docs/ATABEY-FORTRESS-LIVE.md** para uso y cron.

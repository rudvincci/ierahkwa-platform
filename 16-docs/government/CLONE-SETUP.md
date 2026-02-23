# Clone — Repositorio y datos soberanos

Cómo **clonar** el proyecto y, opcionalmente, **restaurar datos** desde un backup para levantar una copia idéntica en otra máquina o bunker. Este flujo es la base de **recuperación rápida** (alineado al vertical [Manufacturing \| SentinelOne](https://www.sentinelone.com/platform/manufacturing/): “Stop threats. Keep producing.” — uptime y recuperación tras incidente).

---

## 1. Clonar el repositorio (código y configuración)

En la máquina donde quieras una copia nueva del proyecto:

```bash
# Opción A: script (desde dentro del repo actual)
./scripts/clone-repo.sh /ruta/destino

# Opción B: git directamente (sustituye URL por tu repo)
git clone --recurse-submodules https://github.com/tu-org/soberanos-natives.git /ruta/destino
cd /ruta/destino
```

Variables opcionales para `clone-repo.sh`:

| Variable | Uso |
|----------|-----|
| `CLONE_REPO_URL` | URL del repo a clonar (por defecto: `origin` del repo actual) |
| `CLONE_DEST` | Directorio destino (alternativa al primer argumento) |

Después del clone: instalar dependencias (Node, etc.), levantar Docker si usas Atabey (`docker-compose up -d`). Ver [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md) y [DOCKER-ATABEY-README.md](../DOCKER-ATABEY-README.md).

---

## 2. Clonar datos desde un backup (restaurar Census, Vault, etc.)

Para tener una **copia de los datos** (Baserow, Nextcloud, Synapse, Nginx) en este mismo repo o en otra máquina donde ya clonaste el repo:

```bash
# Desde el directorio raíz del repo (donde está docker-compose.yml)
./scripts/clone-from-backup.sh [directorio_backups]
```

Ejemplos:

```bash
# Restaurar desde la carpeta local de backups (último atabey_YYYY-MM-DD_HH-MM)
./scripts/clone-from-backup.sh

# Restaurar desde un bunker (ej. disco externo)
./scripts/clone-from-backup.sh /Volumes/AtabeyBunker/sovereign_backups

# Usar variable de entorno
SOVEREIGN_RESTORE_FROM=/ruta/a/sovereign_backups ./scripts/clone-from-backup.sh
```

El script descomprime el **último** backup encontrado en ese directorio (por nombre `atabey_*`) en el directorio actual, creando o sobrescribiendo `baserow_data`, `nextcloud_data`, `synapse_data`, `nginx_data`, `letsencrypt` según existan en el backup.

Después: `docker-compose up -d` para levantar los contenedores con los datos restaurados.

---

## 3. Flujo completo: clone en otra máquina

1. **Clonar repo** en el nuevo host:  
   `./scripts/clone-repo.sh /opt/soberanos-natives` (o `git clone ...`).
2. **Copiar backups** al nuevo host (rsync, USB, etc.) en `/opt/soberanos-natives/sovereign_backups` o en una ruta conocida.
3. **Restaurar datos** (opcional):  
   `cd /opt/soberanos-natives && ./scripts/clone-from-backup.sh [ruta_backups]`.
4. **Levantar servicios**:  
   `docker-compose up -d`  
   Ver: [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md), [DOCKER-ATABEY-README.md](../DOCKER-ATABEY-README.md).

---

## Scripts relacionados

| Script | Función |
|--------|---------|
| `scripts/clone-repo.sh` | Clona el repositorio en un directorio destino. |
| `scripts/clone-from-backup.sh` | Restaura datos desde `sovereign_backups` (o bunker) al directorio actual. |
| `sovereign-backup.sh` | Genera backups (Census, Vault, Messenger, Gateway); opcional copia a bunker. |

Ver también: [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md) (backup nocturno y bunker). Vertical Manufacturing y resto de verticales: [SENTINELONE-VERTICALES-FEDERAL-FINANCE.md](./SENTINELONE-VERTICALES-FEDERAL-FINANCE.md).

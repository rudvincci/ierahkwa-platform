# Backups y restauración

**Sovereign Government of Ierahkwa Ne Kanienke**  
Lista de scripts de backup y qué cubre cada uno para que no falte nada.

---

## Backups principales (qué hace cada uno)

| Script | Qué incluye |
|--------|--------------|
| **./backup.sh** | Backup general: Node (sin node_modules/data), platform, config, docs, tokens, scripts. Crea `backups/ierahkwa_backup_YYYYMMDD_HHMMSS/`. |
| **./scripts/backup-platforms.sh** | Plataformas por nombre (casino, lotto, raffle, sports-betting, social-media) + **platform completo** (todos los HTML, CSS, JS) → `backups/platform-full-*.tar.gz`. |
| **./scripts/backup-todas-plataformas.sh** | Backup de todas las plataformas con manifest. |
| **./RuddieSolution/scripts/backup-node-data.js** o **backup-node-data-encrypted.js** | Datos críticos de `RuddieSolution/node/data/` (usuarios, 2FA, AI hub, etc.). |
| **./RuddieSolution/scripts/backup-production.sh** | Backup orientado a producción. |
| **./scripts/backup-soberano.sh** | Backup soberano (infra propia). |
| **./auto-backup/auto-backup-24-7.sh** | Automatización 24/7 (ejecutar por cron). |
| **./scripts/backup-a-extreme-pro.sh** | Backup hacia disco Extreme Pro (SanDisk). |

## Dónde se guardan

- Por defecto: **`./backups/`** en la raíz del proyecto.
- Con variable: **`BACKUP_DESTINATION=/ruta`** (ej. `BACKUP_DESTINATION="/Volumes/EXTREME_PRO/ierahkwa_backups" ./backup.sh`).

## Qué no se incluye por defecto (para no pesar)

- **node_modules** (se reinstala con `npm install`).
- **.git** en algunos scripts (clonar de nuevo si hace falta).
- **logs** (opcional).

## Restauración

- **Platform completo:** extraer `platform-full-*.tar.gz` sobre `RuddieSolution/platform/`.
- **Backup general:** copiar las carpetas de `backups/ierahkwa_backup_*/` a la raíz del proyecto (node, platform, etc.) según corresponda.
- **Datos Node:** restaurar `RuddieSolution/node/data/` desde el backup de node-data (o encrypted).

## Cron recomendado

Para no depender solo de ejecución manual:

```bash
# Ejemplo: backup completo cada día a las 2:00
0 2 * * * cd /ruta/al/proyecto && ./backup.sh
# Backup platform completo 2 veces por semana
0 3 * * 0,3 cd /ruta/al/proyecto && ./scripts/backup-platforms.sh
```

Ver también: `./scripts/install-cron-production.sh`, `./RuddieSolution/scripts/backup-cron.sh`.

---

Con esto tienes claro **qué backups existen** y **qué cubren**; si algo falta, se puede añadir al script que corresponda.

# 🛡️ Sistema de backup — recordar siempre

Hay **un solo sistema de backup** para todo el proyecto. Funciona manual o automático.

## Backup manual (cuando quieras)

```bash
./backup.sh
```

Crea en `backups/` un archivo `ierahkwa_backup_YYYYMMDD_HHMMSS.tar.gz` con Node, Platform, servicios, .NET, AI, mobile, configs y data. Se mantienen los últimos 10.

## Backup automático (que trabaje siempre solo)

Para que el backup se ejecute **automáticamente** cada día a las 2:00, instala el cron una vez:

```bash
./scripts/install-cron-production.sh
```

Después de eso el sistema hace backup **solo**, sin que tengas que acordarte. Los logs van a `logs/backup-cron.log`.

## Resumen mental

| Qué | Dónde |
|-----|--------|
| Script principal | `backup.sh` |
| Activar automático | `./scripts/install-cron-production.sh` |
| Dónde se guardan | `backups/` (últimos 10 .tar.gz) |
| Log automático | `logs/backup-cron.log` |

Siempre que quieras respaldo: `./backup.sh`. Para que sea automático para siempre: `./scripts/install-cron-production.sh` (una vez).

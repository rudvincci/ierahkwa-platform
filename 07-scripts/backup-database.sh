#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════════════
#  IERAHKWA DATABASE BACKUP SCRIPT
#  Automated PostgreSQL and Redis backup for 24/7 operation
# ═══════════════════════════════════════════════════════════════════════════════

set -e

# Configuration
BACKUP_DIR="${BACKUP_DIR:-./backups/database}"
RETENTION_DAYS="${RETENTION_DAYS:-30}"
PGHOST="${PGHOST:-localhost}"
PGPORT="${PGPORT:-5432}"
PGUSER="${PGUSER:-postgres}"
PGDATABASE="${PGDATABASE:-ierahkwa}"
REDIS_HOST="${REDIS_HOST:-localhost}"
REDIS_PORT="${REDIS_PORT:-6379}"

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Timestamp
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

echo "╔══════════════════════════════════════════════════════════════════════════════╗"
echo "║     🗄️  IERAHKWA DATABASE BACKUP - $(date '+%Y-%m-%d %H:%M:%S')              ║"
echo "╚══════════════════════════════════════════════════════════════════════════════╝"
echo ""

# PostgreSQL Backup
echo "📊 Backing up PostgreSQL database: $PGDATABASE"
PG_BACKUP_FILE="$BACKUP_DIR/postgresql_${PGDATABASE}_${TIMESTAMP}.sql.gz"

if command -v pg_dump &> /dev/null; then
    export PGPASSWORD="${PGPASSWORD:-}"
    pg_dump -h "$PGHOST" -p "$PGPORT" -U "$PGUSER" -d "$PGDATABASE" \
        --no-owner --no-acl | gzip > "$PG_BACKUP_FILE"
    
    if [ $? -eq 0 ]; then
        PG_SIZE=$(du -h "$PG_BACKUP_FILE" | cut -f1)
        echo "✅ PostgreSQL backup created: $PG_BACKUP_FILE ($PG_SIZE)"
    else
        echo "❌ PostgreSQL backup failed!"
        exit 1
    fi
else
    echo "⚠️  pg_dump not found. Skipping PostgreSQL backup."
fi

# Redis Backup
echo ""
echo "🔴 Backing up Redis database"
REDIS_BACKUP_FILE="$BACKUP_DIR/redis_${TIMESTAMP}.rdb"

if command -v redis-cli &> /dev/null; then
    # Trigger Redis BGSAVE
    redis-cli -h "$REDIS_HOST" -p "$REDIS_PORT" BGSAVE
    
    # Wait for BGSAVE to complete (max 60 seconds)
    for i in {1..60}; do
        if redis-cli -h "$REDIS_HOST" -p "$REDIS_PORT" LASTSAVE | grep -q "$(date +%s)"; then
            break
        fi
        sleep 1
    done
    
    # Copy RDB file (default location)
    REDIS_RDB_PATH="${REDIS_RDB_PATH:-/var/lib/redis/dump.rdb}"
    if [ -f "$REDIS_RDB_PATH" ]; then
        cp "$REDIS_RDB_PATH" "$REDIS_BACKUP_FILE"
        REDIS_SIZE=$(du -h "$REDIS_BACKUP_FILE" | cut -f1)
        echo "✅ Redis backup created: $REDIS_BACKUP_FILE ($REDIS_SIZE)"
    else
        echo "⚠️  Redis RDB file not found at $REDIS_RDB_PATH"
    fi
else
    echo "⚠️  redis-cli not found. Skipping Redis backup."
fi

# Cleanup old backups
echo ""
echo "🧹 Cleaning up backups older than $RETENTION_DAYS days..."
find "$BACKUP_DIR" -name "postgresql_*.sql.gz" -mtime +$RETENTION_DAYS -delete
find "$BACKUP_DIR" -name "redis_*.rdb" -mtime +$RETENTION_DAYS -delete
echo "✅ Cleanup complete"

# Summary
echo ""
echo "═══════════════════════════════════════════════════════════════════════════════"
echo "📦 Backup Summary:"
echo "   PostgreSQL: $PG_BACKUP_FILE"
if [ -f "$REDIS_BACKUP_FILE" ]; then
    echo "   Redis: $REDIS_BACKUP_FILE"
fi
echo "   Retention: $RETENTION_DAYS days"
echo "═══════════════════════════════════════════════════════════════════════════════"

#!/bin/bash
# ============================================================
# Red Soberana — Backup Script
# PostgreSQL + Redis automated backup
# Usage: ./backup-soberano.sh [daily|weekly|manual]
# Cron:  0 3 * * * /path/to/backup-soberano.sh daily
# ============================================================

set -euo pipefail

# Configuration
BACKUP_DIR="${BACKUP_DIR:-/var/backups/soberana}"
PG_HOST="${PG_HOST:-localhost}"
PG_PORT="${PG_PORT:-5432}"
PG_DB="${PG_DB:-soberana}"
PG_USER="${PG_USER:-soberano}"
REDIS_HOST="${REDIS_HOST:-localhost}"
REDIS_PORT="${REDIS_PORT:-6379}"
RETENTION_DAYS="${RETENTION_DAYS:-30}"
BACKUP_TYPE="${1:-manual}"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_PATH="${BACKUP_DIR}/${BACKUP_TYPE}/${DATE}"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

log() { echo -e "${GREEN}[$(date +%H:%M:%S)]${NC} $1"; }
warn() { echo -e "${YELLOW}[$(date +%H:%M:%S)] WARN:${NC} $1"; }
error() { echo -e "${RED}[$(date +%H:%M:%S)] ERROR:${NC} $1"; }

# Create backup directory
mkdir -p "${BACKUP_PATH}"
log "Backup started: ${BACKUP_TYPE} → ${BACKUP_PATH}"

# ──────────────────────────────────────────────
# PostgreSQL Backup
# ──────────────────────────────────────────────
log "Backing up PostgreSQL..."
if command -v pg_dump &> /dev/null; then
  pg_dump -h "${PG_HOST}" -p "${PG_PORT}" -U "${PG_USER}" -d "${PG_DB}" \
    --format=custom --compress=9 --verbose \
    --file="${BACKUP_PATH}/soberana_${DATE}.pgdump" 2>&1 | tail -3

  # Also export critical tables as CSV
  for TABLE in wifi_plans starlink_fleet hotspots vip_protected; do
    psql -h "${PG_HOST}" -p "${PG_PORT}" -U "${PG_USER}" -d "${PG_DB}" \
      -c "COPY ${TABLE} TO STDOUT WITH CSV HEADER" > "${BACKUP_PATH}/${TABLE}_${DATE}.csv" 2>/dev/null || true
  done

  log "PostgreSQL backup complete"
else
  # Docker mode
  docker exec postgres-soberana pg_dump -U "${PG_USER}" -d "${PG_DB}" \
    --format=custom --compress=9 \
    > "${BACKUP_PATH}/soberana_${DATE}.pgdump" 2>/dev/null

  log "PostgreSQL backup complete (Docker mode)"
fi

# ──────────────────────────────────────────────
# Redis Backup
# ──────────────────────────────────────────────
log "Backing up Redis..."
if command -v redis-cli &> /dev/null; then
  redis-cli -h "${REDIS_HOST}" -p "${REDIS_PORT}" BGSAVE 2>/dev/null
  sleep 2
  REDIS_DIR=$(redis-cli -h "${REDIS_HOST}" -p "${REDIS_PORT}" CONFIG GET dir 2>/dev/null | tail -1)
  if [ -f "${REDIS_DIR}/dump.rdb" ]; then
    cp "${REDIS_DIR}/dump.rdb" "${BACKUP_PATH}/redis_${DATE}.rdb"
    log "Redis backup complete"
  else
    warn "Redis dump.rdb not found at ${REDIS_DIR}"
  fi
else
  # Docker mode
  docker exec redis-soberana redis-cli BGSAVE 2>/dev/null
  sleep 2
  docker cp redis-soberana:/data/dump.rdb "${BACKUP_PATH}/redis_${DATE}.rdb" 2>/dev/null
  log "Redis backup complete (Docker mode)"
fi

# ──────────────────────────────────────────────
# Compress
# ──────────────────────────────────────────────
log "Compressing backup..."
cd "${BACKUP_DIR}/${BACKUP_TYPE}"
tar -czf "${DATE}.tar.gz" "${DATE}/" 2>/dev/null
rm -rf "${DATE}/"
BACKUP_SIZE=$(du -sh "${DATE}.tar.gz" | cut -f1)
log "Compressed: ${DATE}.tar.gz (${BACKUP_SIZE})"

# ──────────────────────────────────────────────
# Cleanup old backups
# ──────────────────────────────────────────────
log "Cleaning backups older than ${RETENTION_DAYS} days..."
CLEANED=$(find "${BACKUP_DIR}" -name "*.tar.gz" -mtime +${RETENTION_DAYS} -delete -print | wc -l)
if [ "${CLEANED}" -gt 0 ]; then
  log "Removed ${CLEANED} old backup(s)"
fi

# ──────────────────────────────────────────────
# Summary
# ──────────────────────────────────────────────
echo ""
log "═══════════════════════════════════════════"
log " Backup Complete — Red Soberana"
log "═══════════════════════════════════════════"
log " Type:     ${BACKUP_TYPE}"
log " File:     ${BACKUP_DIR}/${BACKUP_TYPE}/${DATE}.tar.gz"
log " Size:     ${BACKUP_SIZE}"
log " Date:     $(date)"
log "═══════════════════════════════════════════"

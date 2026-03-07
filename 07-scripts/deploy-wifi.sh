#!/bin/bash
# ============================================================================
# WIFI SOBERANO - DEPLOY SCRIPT
# Despliega actualizaciones del sistema WiFi Soberano
# Ejecutar en el servidor de producción
# ============================================================================

set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

log()  { echo -e "${GREEN}[✓]${NC} $1"; }
warn() { echo -e "${YELLOW}[!]${NC} $1"; }
err()  { echo -e "${RED}[✗]${NC} $1"; exit 1; }
step() { echo -e "\n${CYAN}═══ $1 ═══${NC}"; }

# ── Variables ──────────────────────────────────────────────────
PROJECT_DIR="${PROJECT_DIR:-/opt/soberano}"
WIFI_DIR="${PROJECT_DIR}/03-backend/wifi-soberano"
CORE_DIR="${PROJECT_DIR}/03-backend/sovereign-core"
BRANCH="${DEPLOY_BRANCH:-main}"
BACKUP_DIR="/var/backups/soberano"
TIMESTAMP=$(date '+%Y%m%d_%H%M%S')
ROLLBACK_FILE="${BACKUP_DIR}/rollback_${TIMESTAMP}.txt"

echo -e "${CYAN}"
echo "📡 ═══════════════════════════════════════════════════════════"
echo "   WIFI SOBERANO — DEPLOY"
echo "   $(date)"
echo "═══════════════════════════════════════════════════════════"
echo -e "${NC}"

# ── Pre-flight checks ────────────────────────────────────────
step "Pre-flight Checks"

[ -d "$PROJECT_DIR/.git" ] || err "Proyecto no encontrado en $PROJECT_DIR"
[ -d "$WIFI_DIR" ] || err "wifi-soberano no encontrado en $WIFI_DIR"
[ -d "$CORE_DIR" ] || err "sovereign-core no encontrado en $CORE_DIR"

# Verificar que los servicios existen
systemctl is-enabled wifi-soberano &>/dev/null || warn "wifi-soberano no registrado en systemd"
systemctl is-enabled sovereign-core &>/dev/null || warn "sovereign-core no registrado en systemd"

log "Pre-flight checks OK"

# ── Crear backup ──────────────────────────────────────────────
step "Backup"

mkdir -p "$BACKUP_DIR"
CURRENT_COMMIT=$(cd "$PROJECT_DIR" && git rev-parse HEAD)
echo "$CURRENT_COMMIT" > "$ROLLBACK_FILE"

# Backup de .env files
cp -f "${WIFI_DIR}/.env" "${BACKUP_DIR}/wifi-env_${TIMESTAMP}" 2>/dev/null || true
cp -f "${CORE_DIR}/.env" "${BACKUP_DIR}/core-env_${TIMESTAMP}" 2>/dev/null || true

# Backup de base de datos
docker exec postgres-soberano pg_dump -U soberano soberana | gzip > "${BACKUP_DIR}/db_${TIMESTAMP}.sql.gz" 2>/dev/null || warn "DB backup falló"

log "Backup creado (commit: ${CURRENT_COMMIT:0:8})"
log "Rollback file: $ROLLBACK_FILE"

# ── Pull código ──────────────────────────────────────────────
step "Pull Código"

cd "$PROJECT_DIR"
git fetch origin "$BRANCH" 2>/dev/null || err "Git fetch falló"

NEW_COMMIT=$(git rev-parse "origin/$BRANCH")
if [ "$CURRENT_COMMIT" = "$NEW_COMMIT" ]; then
  warn "Ya estás en el último commit ($BRANCH: ${NEW_COMMIT:0:8})"
  echo "  ¿Continuar de todos modos? (rebuild sin cambios de código)"
  # En producción, continuar de todos modos para rebuild
fi

git reset --hard "origin/$BRANCH" 2>/dev/null || err "Git reset falló"
log "Código actualizado: ${CURRENT_COMMIT:0:8} → ${NEW_COMMIT:0:8}"

# Mostrar cambios
echo ""
echo "  Commits nuevos:"
git log --oneline "${CURRENT_COMMIT}..HEAD" 2>/dev/null | head -10 | while read line; do
  echo "    $line"
done
echo ""

# ── Instalar dependencias ─────────────────────────────────────
step "Dependencias"

# WiFi Soberano
cd "$WIFI_DIR"
if [ -f package-lock.json ]; then
  npm ci --production 2>/dev/null || npm install --production
else
  npm install --production
fi
log "WiFi Soberano: dependencias instaladas"

# Sovereign Core
cd "$CORE_DIR"
if [ -f package-lock.json ]; then
  npm ci --production 2>/dev/null || npm install --production
else
  npm install --production
fi
log "Sovereign Core: dependencias instaladas"

# ── Ejecutar migraciones ──────────────────────────────────────
step "Migraciones"

if [ -f "${WIFI_DIR}/models/migrations.sql" ]; then
  docker exec -i postgres-soberano psql -U soberano -d soberana < "${WIFI_DIR}/models/migrations.sql" 2>/dev/null && \
    log "Migraciones WiFi ejecutadas" || \
    warn "Migraciones WiFi: ya ejecutadas o error"
fi

if [ -f "${CORE_DIR}/migrations" ]; then
  # Si sovereign-core tiene migraciones propias
  for f in "${CORE_DIR}"/migrations/*.sql; do
    docker exec -i postgres-soberano psql -U soberano -d soberana < "$f" 2>/dev/null || true
  done
  log "Migraciones Core ejecutadas"
fi

# ── Reiniciar servicios (rolling) ─────────────────────────────
step "Reinicio de Servicios"

# 1. Primero reiniciar sovereign-core (no es público)
echo "  Reiniciando sovereign-core..."
systemctl restart sovereign-core 2>/dev/null || warn "sovereign-core restart falló"
sleep 2

# Verificar que sovereign-core arrancó
if curl -s --max-time 5 "http://127.0.0.1:3050/health" | jq -e '.status' &>/dev/null; then
  log "Sovereign Core: OK"
else
  warn "Sovereign Core: no responde en /health (puede tardar)"
fi

# 2. Reiniciar wifi-soberano
echo "  Reiniciando wifi-soberano..."
systemctl restart wifi-soberano 2>/dev/null || warn "wifi-soberano restart falló"
sleep 2

# Verificar
if curl -s --max-time 5 "http://127.0.0.1:3095/health" | jq -e '.status' &>/dev/null; then
  log "WiFi Soberano: OK"
else
  warn "WiFi Soberano: no responde en /health (puede tardar)"
fi

# 3. Reiniciar session expiry worker
systemctl restart wifi-session-expiry 2>/dev/null || warn "wifi-session-expiry restart falló"
log "Session Expiry Worker: reiniciado"

# 4. Reload nginx (no restart, para zero downtime)
nginx -t 2>/dev/null && systemctl reload nginx 2>/dev/null && \
  log "Nginx: reload OK" || \
  warn "Nginx: reload falló"

# ── Verificación post-deploy ──────────────────────────────────
step "Verificación Post-Deploy"

DEPLOY_OK=true

# Health checks
for service_url in "http://127.0.0.1:3095/health" "http://127.0.0.1:3050/health"; do
  if curl -s --max-time 10 "$service_url" | jq -e '.status' &>/dev/null; then
    log "$service_url: OK"
  else
    warn "$service_url: no responde"
    DEPLOY_OK=false
  fi
done

# Docker containers
for container in postgres-soberano redis-soberano; do
  if docker inspect --format='{{.State.Status}}' "$container" 2>/dev/null | grep -q running; then
    log "$container: running"
  else
    warn "$container: no running"
    DEPLOY_OK=false
  fi
done

echo ""

if $DEPLOY_OK; then
  echo -e "${GREEN}"
  echo "═══════════════════════════════════════════════════════════"
  echo "  ✓ DEPLOY EXITOSO"
  echo "═══════════════════════════════════════════════════════════"
  echo -e "${NC}"
  echo "  Commit: ${NEW_COMMIT:0:8}"
  echo "  Branch: $BRANCH"
  echo "  Fecha:  $(date)"
  echo ""
  echo "  Para rollback:"
  echo "    cd $PROJECT_DIR && git reset --hard $CURRENT_COMMIT"
  echo "    systemctl restart wifi-soberano sovereign-core"
  echo ""

  # Limpiar backups viejos (mantener últimos 10)
  ls -t "${BACKUP_DIR}"/db_*.sql.gz 2>/dev/null | tail -n +11 | xargs rm -f 2>/dev/null || true
  ls -t "${BACKUP_DIR}"/rollback_*.txt 2>/dev/null | tail -n +11 | xargs rm -f 2>/dev/null || true

else
  echo -e "${RED}"
  echo "═══════════════════════════════════════════════════════════"
  echo "  ⚠ DEPLOY CON ADVERTENCIAS"
  echo "═══════════════════════════════════════════════════════════"
  echo -e "${NC}"
  echo "  Algunos servicios no respondieron correctamente."
  echo "  Verificar logs:"
  echo "    journalctl -u wifi-soberano -n 50"
  echo "    journalctl -u sovereign-core -n 50"
  echo ""
  echo "  Para rollback automático:"
  echo "    $0 rollback"
  echo ""
fi

# ── Subcomando: Rollback ──────────────────────────────────────
if [ "${1:-}" = "rollback" ]; then
  step "ROLLBACK"

  # Encontrar último rollback file
  LAST_ROLLBACK=$(ls -t "${BACKUP_DIR}"/rollback_*.txt 2>/dev/null | head -1)
  if [ -z "$LAST_ROLLBACK" ]; then
    err "No hay punto de rollback disponible"
  fi

  ROLLBACK_COMMIT=$(cat "$LAST_ROLLBACK")
  warn "Haciendo rollback a commit: ${ROLLBACK_COMMIT:0:8}"

  cd "$PROJECT_DIR"
  git reset --hard "$ROLLBACK_COMMIT"

  # Reinstalar deps
  cd "$WIFI_DIR" && npm ci --production 2>/dev/null
  cd "$CORE_DIR" && npm ci --production 2>/dev/null

  # Restaurar DB si existe backup
  LAST_DB=$(ls -t "${BACKUP_DIR}"/db_*.sql.gz 2>/dev/null | head -1)
  if [ -n "$LAST_DB" ]; then
    warn "Restaurando DB desde $LAST_DB"
    zcat "$LAST_DB" | docker exec -i postgres-soberano psql -U soberano -d soberana 2>/dev/null || \
      warn "Restauración de DB falló"
  fi

  # Reiniciar servicios
  systemctl restart wifi-soberano sovereign-core wifi-session-expiry
  systemctl reload nginx

  log "Rollback completado a ${ROLLBACK_COMMIT:0:8}"
fi

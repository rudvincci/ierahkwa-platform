#!/bin/bash
# ============================================================================
# WIFI SOBERANO - HEALTH MONITOR
# Monitoreo continuo de todos los servicios de la plataforma
# Ejecutar via cron cada 5 minutos o como servicio systemd
# ============================================================================

set -uo pipefail

# ── Config ────────────────────────────────────────────────────
DOMAIN="${WIFI_DOMAIN:-wifi.soberano.bo}"
ALERT_WEBHOOK="${ALERT_WEBHOOK:-}"          # Webhook para alertas (Slack/Discord/Telegram)
ALERT_EMAIL="${ALERT_EMAIL:-admin@ierahkwa.nation}"
LOG_FILE="/var/log/soberano/health-monitor.log"
STATE_DIR="/var/lib/soberano/health"
MAX_FAILURES=3                               # Alertar después de N fallos consecutivos

# Servicios a monitorear
declare -A SERVICES=(
  ["wifi-soberano"]="http://127.0.0.1:3095/health"
  ["sovereign-core"]="http://127.0.0.1:3050/health"
  ["nginx"]="http://127.0.0.1:80/"
  ["postgres"]="docker:postgres-soberano"
  ["redis"]="docker:redis-soberano"
)

# ── Colores ───────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

# ── Setup ─────────────────────────────────────────────────────
mkdir -p "$(dirname "$LOG_FILE")" "$STATE_DIR"

timestamp() { date '+%Y-%m-%d %H:%M:%S'; }

log() {
  local msg="[$(timestamp)] $1"
  echo "$msg" >> "$LOG_FILE"
  [ -t 1 ] && echo -e "${GREEN}[✓]${NC} $1"
}

warn() {
  local msg="[$(timestamp)] WARN: $1"
  echo "$msg" >> "$LOG_FILE"
  [ -t 1 ] && echo -e "${YELLOW}[!]${NC} $1"
}

alert() {
  local msg="[$(timestamp)] ALERT: $1"
  echo "$msg" >> "$LOG_FILE"
  [ -t 1 ] && echo -e "${RED}[✗]${NC} $1"
  send_alert "$1"
}

# ── Enviar alerta ─────────────────────────────────────────────
send_alert() {
  local message="$1"
  local payload="{\"text\":\"🚨 SOBERANO ALERT: ${message}\",\"service\":\"health-monitor\",\"timestamp\":\"$(timestamp)\",\"server\":\"$(hostname)\"}"

  # Webhook (Slack/Discord/Telegram)
  if [ -n "$ALERT_WEBHOOK" ]; then
    curl -s -X POST "$ALERT_WEBHOOK" \
      -H 'Content-Type: application/json' \
      -d "$payload" \
      --max-time 10 > /dev/null 2>&1 || true
  fi

  # Alerta vía wifi-soberano API (si está disponible)
  curl -s -X POST "http://127.0.0.1:3095/api/v1/wifi/admin/alert" \
    -H 'Content-Type: application/json' \
    -d "{\"alert_type\":\"health\",\"severity\":\"critical\",\"details\":\"${message}\"}" \
    --max-time 5 > /dev/null 2>&1 || true

  # Log del sistema
  logger -t soberano-health "ALERT: $message"
}

# ── Check: Servicio HTTP ──────────────────────────────────────
check_http() {
  local name="$1"
  local url="$2"
  local response
  local http_code

  http_code=$(curl -s -o /dev/null -w '%{http_code}' --max-time 10 "$url" 2>/dev/null)

  if [ "$http_code" -ge 200 ] && [ "$http_code" -lt 400 ]; then
    echo "ok"
  else
    echo "fail:http_${http_code}"
  fi
}

# ── Check: Docker container ──────────────────────────────────
check_docker() {
  local container_name="$1"

  if ! command -v docker &>/dev/null; then
    echo "fail:no_docker"
    return
  fi

  local status
  status=$(docker inspect --format='{{.State.Status}}' "$container_name" 2>/dev/null)

  if [ "$status" = "running" ]; then
    echo "ok"
  elif [ -z "$status" ]; then
    echo "fail:not_found"
  else
    echo "fail:${status}"
  fi
}

# ── Check: Disk space ────────────────────────────────────────
check_disk() {
  local usage
  usage=$(df / | tail -1 | awk '{print $5}' | sed 's/%//')

  if [ "$usage" -ge 95 ]; then
    echo "critical:${usage}%"
  elif [ "$usage" -ge 85 ]; then
    echo "warn:${usage}%"
  else
    echo "ok:${usage}%"
  fi
}

# ── Check: Memory ────────────────────────────────────────────
check_memory() {
  local mem_total mem_available pct_used

  if [ -f /proc/meminfo ]; then
    mem_total=$(grep MemTotal /proc/meminfo | awk '{print $2}')
    mem_available=$(grep MemAvailable /proc/meminfo | awk '{print $2}')
    pct_used=$(( (mem_total - mem_available) * 100 / mem_total ))
  else
    # macOS fallback
    pct_used=$(vm_stat 2>/dev/null | awk '/Pages active/ {print int($3/256)}' || echo "0")
  fi

  if [ "$pct_used" -ge 95 ]; then
    echo "critical:${pct_used}%"
  elif [ "$pct_used" -ge 85 ]; then
    echo "warn:${pct_used}%"
  else
    echo "ok:${pct_used}%"
  fi
}

# ── Check: SSL Certificate ───────────────────────────────────
check_ssl() {
  local domain="$1"
  local expiry_days

  expiry_days=$(echo | openssl s_client -servername "$domain" -connect "$domain:443" 2>/dev/null | \
    openssl x509 -noout -dates 2>/dev/null | \
    grep notAfter | cut -d= -f2 | \
    xargs -I{} bash -c 'echo $(( ( $(date -d "{}" +%s 2>/dev/null || date -j -f "%b %d %T %Y %Z" "{}" +%s 2>/dev/null) - $(date +%s) ) / 86400 ))')

  if [ -z "$expiry_days" ]; then
    echo "fail:no_cert"
  elif [ "$expiry_days" -le 7 ]; then
    echo "critical:${expiry_days}d"
  elif [ "$expiry_days" -le 30 ]; then
    echo "warn:${expiry_days}d"
  else
    echo "ok:${expiry_days}d"
  fi
}

# ── Check: PostgreSQL queries ─────────────────────────────────
check_postgres_health() {
  local result
  result=$(docker exec postgres-soberano psql -U soberano -d soberana -t -c "SELECT count(*) FROM wifi_sessions WHERE status='active';" 2>/dev/null | tr -d ' ')

  if [ -n "$result" ]; then
    echo "ok:${result}_active_sessions"
  else
    echo "fail:query_error"
  fi
}

# ── Check: Redis ──────────────────────────────────────────────
check_redis_health() {
  local result
  result=$(docker exec redis-soberano redis-cli ping 2>/dev/null)

  if [ "$result" = "PONG" ]; then
    echo "ok"
  else
    echo "fail:no_pong"
  fi
}

# ── Failure tracking ─────────────────────────────────────────
get_failure_count() {
  local service="$1"
  local file="${STATE_DIR}/${service}.failures"
  cat "$file" 2>/dev/null || echo "0"
}

increment_failures() {
  local service="$1"
  local file="${STATE_DIR}/${service}.failures"
  local count
  count=$(get_failure_count "$service")
  echo $((count + 1)) > "$file"
}

reset_failures() {
  local service="$1"
  local file="${STATE_DIR}/${service}.failures"
  echo "0" > "$file"
}

# ── Auto-recovery ─────────────────────────────────────────────
try_recover() {
  local service="$1"
  warn "Intentando recuperar: $service"

  case "$service" in
    wifi-soberano|sovereign-core|wifi-session-expiry)
      systemctl restart "$service" 2>/dev/null
      log "Reiniciado servicio systemd: $service"
      ;;
    postgres)
      docker restart postgres-soberano 2>/dev/null
      log "Reiniciado container: postgres-soberano"
      ;;
    redis)
      docker restart redis-soberano 2>/dev/null
      log "Reiniciado container: redis-soberano"
      ;;
    nginx)
      nginx -t 2>/dev/null && systemctl restart nginx 2>/dev/null
      log "Reiniciado: nginx"
      ;;
  esac
}

# ── Ejecutar todos los checks ─────────────────────────────────
run_checks() {
  local all_ok=true
  local report=""

  echo -e "\n${CYAN}═══ SOBERANO HEALTH CHECK — $(timestamp) ═══${NC}\n"

  # Check services
  for service in "${!SERVICES[@]}"; do
    local url="${SERVICES[$service]}"
    local result

    if [[ "$url" == docker:* ]]; then
      local container="${url#docker:}"
      result=$(check_docker "$container")
    else
      result=$(check_http "$service" "$url")
    fi

    local status="${result%%:*}"
    local detail="${result#*:}"

    if [ "$status" = "ok" ]; then
      [ -t 1 ] && printf "  ${GREEN}●${NC} %-20s %s\n" "$service" "$detail"
      reset_failures "$service"
    else
      all_ok=false
      [ -t 1 ] && printf "  ${RED}●${NC} %-20s %s\n" "$service" "$result"

      increment_failures "$service"
      local failures
      failures=$(get_failure_count "$service")

      if [ "$failures" -ge "$MAX_FAILURES" ]; then
        alert "$service CAÍDO (${failures} fallos consecutivos): $result"
        try_recover "$service"
        reset_failures "$service"
      fi
    fi

    report+="$service: $result\n"
  done

  # System checks
  echo ""

  # Disk
  local disk_result
  disk_result=$(check_disk)
  local disk_status="${disk_result%%:*}"
  local disk_detail="${disk_result#*:}"
  case "$disk_status" in
    ok)       [ -t 1 ] && printf "  ${GREEN}●${NC} %-20s %s\n" "disk" "$disk_detail" ;;
    warn)     [ -t 1 ] && printf "  ${YELLOW}●${NC} %-20s %s\n" "disk" "$disk_detail"; warn "Disco en ${disk_detail}" ;;
    critical) [ -t 1 ] && printf "  ${RED}●${NC} %-20s %s\n" "disk" "$disk_detail"; alert "DISCO CRÍTICO: ${disk_detail}" ;;
  esac

  # Memory
  local mem_result
  mem_result=$(check_memory)
  local mem_status="${mem_result%%:*}"
  local mem_detail="${mem_result#*:}"
  case "$mem_status" in
    ok)       [ -t 1 ] && printf "  ${GREEN}●${NC} %-20s %s\n" "memory" "$mem_detail" ;;
    warn)     [ -t 1 ] && printf "  ${YELLOW}●${NC} %-20s %s\n" "memory" "$mem_detail"; warn "Memoria en ${mem_detail}" ;;
    critical) [ -t 1 ] && printf "  ${RED}●${NC} %-20s %s\n" "memory" "$mem_detail"; alert "MEMORIA CRÍTICA: ${mem_detail}" ;;
  esac

  # SSL (solo si el dominio resuelve)
  if host "$DOMAIN" &>/dev/null 2>&1; then
    local ssl_result
    ssl_result=$(check_ssl "$DOMAIN")
    local ssl_status="${ssl_result%%:*}"
    local ssl_detail="${ssl_result#*:}"
    case "$ssl_status" in
      ok)       [ -t 1 ] && printf "  ${GREEN}●${NC} %-20s expira en %s\n" "ssl" "$ssl_detail" ;;
      warn)     [ -t 1 ] && printf "  ${YELLOW}●${NC} %-20s expira en %s\n" "ssl" "$ssl_detail"; warn "SSL expira en ${ssl_detail}" ;;
      critical) [ -t 1 ] && printf "  ${RED}●${NC} %-20s expira en %s\n" "ssl" "$ssl_detail"; alert "SSL EXPIRA EN ${ssl_detail}" ;;
      fail)     [ -t 1 ] && printf "  ${RED}●${NC} %-20s %s\n" "ssl" "$ssl_detail" ;;
    esac
  fi

  # PostgreSQL detailed
  local pg_result
  pg_result=$(check_postgres_health)
  [ -t 1 ] && printf "  ${GREEN}◆${NC} %-20s %s\n" "pg-sessions" "${pg_result#*:}"

  echo ""

  if $all_ok; then
    log "Health check OK — todos los servicios funcionando"
  fi
}

# ── Modo daemon (loop continuo) ───────────────────────────────
run_daemon() {
  local interval="${1:-300}"  # Default: 5 minutos
  log "Health monitor daemon iniciado (intervalo: ${interval}s)"

  while true; do
    run_checks 2>/dev/null
    sleep "$interval"
  done
}

# ── Instalar como servicio systemd ────────────────────────────
install_service() {
  cat > /etc/systemd/system/soberano-health.service << SVCEOF
[Unit]
Description=Soberano Health Monitor
After=network.target wifi-soberano.service

[Service]
Type=simple
User=root
ExecStart=/bin/bash $(readlink -f "$0") daemon 300
Restart=always
RestartSec=30
StandardOutput=journal
StandardError=journal
SyslogIdentifier=soberano-health

[Install]
WantedBy=multi-user.target
SVCEOF

  # También instalar cron como backup
  (crontab -l 2>/dev/null; echo "*/5 * * * * $(readlink -f "$0") check >> /var/log/soberano/health-cron.log 2>&1") | sort -u | crontab -

  systemctl daemon-reload
  systemctl enable soberano-health
  systemctl start soberano-health

  log "Health monitor instalado como servicio systemd + cron"
}

# ── CLI ───────────────────────────────────────────────────────
case "${1:-check}" in
  check)
    run_checks
    ;;
  daemon)
    run_daemon "${2:-300}"
    ;;
  install)
    install_service
    ;;
  status)
    run_checks
    ;;
  *)
    echo "Uso: $0 {check|daemon [interval_s]|install|status}"
    echo ""
    echo "  check   — Ejecutar health check una vez"
    echo "  daemon  — Ejecutar en modo continuo (default 5min)"
    echo "  install — Instalar como servicio systemd + cron"
    echo "  status  — Igual que check"
    exit 1
    ;;
esac

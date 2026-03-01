#!/usr/bin/env bash
# ============================================================
# deploy-ierahkwa.sh — Despliegue Soberano con Un Solo Clic
# Gobierno Soberano de Ierahkwa Ne Kanienke
# ============================================================
# Uso: ./deploy-ierahkwa.sh
# ============================================================

set -euo pipefail

# -----------------------------------------------------------
# Colores y utilidades
# -----------------------------------------------------------
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
PURPLE='\033[0;35m'
NC='\033[0m'
BOLD='\033[1m'

log_info()  { echo -e "${CYAN}[INFO]${NC}  $1"; }
log_ok()    { echo -e "${GREEN}[OK]${NC}    $1"; }
log_warn()  { echo -e "${YELLOW}[WARN]${NC}  $1"; }
log_error() { echo -e "${RED}[ERROR]${NC} $1"; }
log_step()  { echo -e "\n${PURPLE}${BOLD}>>> $1${NC}"; }

COMPOSE_FILE="docker-compose.sovereign.yml"
ENV_FILE=".env"
ENV_EXAMPLE=".env.example"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

cd "$SCRIPT_DIR"

# -----------------------------------------------------------
# Banner
# -----------------------------------------------------------
echo -e "${PURPLE}${BOLD}"
cat << 'GENESIS'

    ██╗███████╗██████╗  █████╗ ██╗  ██╗██╗  ██╗██╗    ██╗ █████╗
    ██║██╔════╝██╔══██╗██╔══██╗██║  ██║██║ ██╔╝██║    ██║██╔══██╗
    ██║█████╗  ██████╔╝███████║███████║█████╔╝ ██║ █╗ ██║███████║
    ██║██╔══╝  ██╔══██╗██╔══██║██╔══██║██╔═██╗ ██║███╗██║██╔══██║
    ██║███████╗██║  ██║██║  ██║██║  ██║██║  ██╗╚███╔███╔╝██║  ██║
    ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═══╝╚═╝ ╚═╝  ╚═╝

    N E   K A N I E N K E  —  G E N E S I S   B O O T
    ═══════════════════════════════════════════════════
    Gobierno Soberano — Nacion Digital
    35+ Naciones | 1B+ Personas | 574 Tribus
    Soberania Tecnologica para TODAS las Americas
    ═══════════════════════════════════════════════════

GENESIS
echo -e "${NC}"
echo -e "${CYAN}  Iniciando secuencia Genesis...${NC}"
sleep 1

# -----------------------------------------------------------
# 1. Verificar prerequisitos
# -----------------------------------------------------------
log_step "1/6 — Verificando prerequisitos"

if ! command -v docker &>/dev/null; then
    log_error "Docker no esta instalado."
    echo "  Instala Docker: https://docs.docker.com/get-docker/"
    exit 1
fi
log_ok "Docker encontrado: $(docker --version | head -1)"

# Detectar docker compose (plugin o standalone)
COMPOSE_CMD=""
if docker compose version &>/dev/null 2>&1; then
    COMPOSE_CMD="docker compose"
    log_ok "Docker Compose (plugin): $(docker compose version --short 2>/dev/null || echo 'detectado')"
elif command -v docker-compose &>/dev/null; then
    COMPOSE_CMD="docker-compose"
    log_ok "Docker Compose (standalone): $(docker-compose --version | head -1)"
else
    log_error "Docker Compose no esta instalado."
    echo "  Instala Docker Compose: https://docs.docker.com/compose/install/"
    exit 1
fi

if [ ! -f "$COMPOSE_FILE" ]; then
    log_error "Archivo $COMPOSE_FILE no encontrado en $(pwd)"
    exit 1
fi
log_ok "Archivo $COMPOSE_FILE encontrado"

# Verificar que Docker daemon esta corriendo
if ! docker info &>/dev/null 2>&1; then
    log_error "Docker daemon no esta corriendo. Inicia Docker Desktop o el servicio dockerd."
    exit 1
fi
log_ok "Docker daemon activo"

# -----------------------------------------------------------
# 1b. Verificar servicios soberanos opcionales
# -----------------------------------------------------------
log_step "1b/6 — Verificando servicios soberanos"

# Tor
if command -v tor &>/dev/null; then
    log_ok "Tor encontrado: $(tor --version | head -1)"
    TOR_AVAILABLE=true
else
    log_warn "Tor no instalado — Hidden services desactivados"
    log_info "  Instala Tor: sudo apt install tor"
    TOR_AVAILABLE=false
fi

# IPFS
if command -v ipfs &>/dev/null; then
    log_ok "IPFS encontrado: $(ipfs version 2>/dev/null | head -1)"
    IPFS_AVAILABLE=true
else
    log_warn "IPFS no instalado — Archivo eterno desactivado"
    log_info "  Instala IPFS: https://docs.ipfs.tech/install/"
    IPFS_AVAILABLE=false
fi

# Ollama (AI Soberano)
if command -v ollama &>/dev/null; then
    log_ok "Ollama encontrado: $(ollama --version 2>/dev/null | head -1)"
    OLLAMA_AVAILABLE=true
else
    log_warn "Ollama no instalado — IA Soberana desactivada"
    log_info "  Instala Ollama: https://ollama.ai/download"
    OLLAMA_AVAILABLE=false
fi

# Handshake DNS
if command -v hsd &>/dev/null; then
    log_ok "Handshake DNS encontrado"
    HNS_AVAILABLE=true
else
    log_warn "Handshake DNS no instalado — DNS soberano desactivado"
    log_info "  Instala HSD: npm i -g hsd"
    HNS_AVAILABLE=false
fi

# Meshtastic
if command -v meshtastic &>/dev/null || python3 -c "import meshtastic" 2>/dev/null; then
    log_ok "Meshtastic encontrado — LoRa mesh activo"
    MESH_AVAILABLE=true
else
    log_warn "Meshtastic no instalado — Mesh offline desactivado"
    log_info "  Instala: pip install meshtastic"
    MESH_AVAILABLE=false
fi

# -----------------------------------------------------------
# 2. Configurar variables de entorno
# -----------------------------------------------------------
log_step "2/6 — Configurando variables de entorno"

generate_secret() {
    openssl rand -base64 48 2>/dev/null || head -c 48 /dev/urandom | base64 | tr -d '\n/+='
}

if [ ! -f "$ENV_FILE" ]; then
    if [ -f "$ENV_EXAMPLE" ]; then
        cp "$ENV_EXAMPLE" "$ENV_FILE"
        log_info "Copiado $ENV_EXAMPLE -> $ENV_FILE"
    else
        log_info "Creando $ENV_FILE desde cero"
        touch "$ENV_FILE"
    fi

    # Generar secretos si no existen en el archivo
    JWT_SECRET=$(generate_secret)
    IDENTITY_JWT_SECRET=$(generate_secret)
    POSTGRES_PASSWORD=$(generate_secret | tr -dc 'a-zA-Z0-9' | head -c 32)
    REDIS_PASSWORD=$(generate_secret | tr -dc 'a-zA-Z0-9' | head -c 32)
    GRAFANA_PASSWORD=$(generate_secret | tr -dc 'a-zA-Z0-9' | head -c 24)

    # Escribir variables necesarias (solo si no existen)
    grep -q '^JWT_SECRET=' "$ENV_FILE" 2>/dev/null || echo "JWT_SECRET=$JWT_SECRET" >> "$ENV_FILE"
    grep -q '^IDENTITY_JWT_SECRET=' "$ENV_FILE" 2>/dev/null || echo "IDENTITY_JWT_SECRET=$IDENTITY_JWT_SECRET" >> "$ENV_FILE"
    grep -q '^POSTGRES_USER=' "$ENV_FILE" 2>/dev/null || echo "POSTGRES_USER=ierahkwa_admin" >> "$ENV_FILE"
    grep -q '^POSTGRES_PASSWORD=' "$ENV_FILE" 2>/dev/null || echo "POSTGRES_PASSWORD=$POSTGRES_PASSWORD" >> "$ENV_FILE"
    grep -q '^POSTGRES_DB=' "$ENV_FILE" 2>/dev/null || echo "POSTGRES_DB=ierahkwa_sovereign" >> "$ENV_FILE"
    grep -q '^REDIS_PASSWORD=' "$ENV_FILE" 2>/dev/null || echo "REDIS_PASSWORD=$REDIS_PASSWORD" >> "$ENV_FILE"
    grep -q '^GRAFANA_ADMIN_PASSWORD=' "$ENV_FILE" 2>/dev/null || echo "GRAFANA_ADMIN_PASSWORD=$GRAFANA_PASSWORD" >> "$ENV_FILE"
    grep -q '^CORS_ORIGINS=' "$ENV_FILE" 2>/dev/null || echo "CORS_ORIGINS=https://ierahkwa.gov,https://ierahkwa.nation" >> "$ENV_FILE"
    grep -q '^SMTP_HOST=' "$ENV_FILE" 2>/dev/null || echo "SMTP_HOST=localhost" >> "$ENV_FILE"
    grep -q '^SMTP_PORT=' "$ENV_FILE" 2>/dev/null || echo "SMTP_PORT=587" >> "$ENV_FILE"

    log_ok "Secretos generados y escritos en $ENV_FILE"
    log_warn "IMPORTANTE: Revisa $ENV_FILE y ajusta los valores para produccion"
else
    log_ok "$ENV_FILE ya existe — usando configuracion existente"
fi

# -----------------------------------------------------------
# 3. Detener servicios existentes (si los hay)
# -----------------------------------------------------------
log_step "3/6 — Preparando despliegue"

RUNNING=$($COMPOSE_CMD -f "$COMPOSE_FILE" ps -q 2>/dev/null | wc -l | tr -d ' ')
if [ "$RUNNING" -gt 0 ]; then
    log_info "Deteniendo $RUNNING contenedores existentes..."
    $COMPOSE_CMD -f "$COMPOSE_FILE" down --remove-orphans 2>/dev/null || true
    log_ok "Contenedores anteriores detenidos"
else
    log_ok "No hay contenedores previos corriendo"
fi

# -----------------------------------------------------------
# 4. Construir e iniciar servicios
# -----------------------------------------------------------
log_step "4/6 — Construyendo e iniciando servicios soberanos"

log_info "Construyendo imagenes..."
$COMPOSE_CMD -f "$COMPOSE_FILE" build --parallel 2>&1 | tail -5 || true

log_info "Iniciando servicios..."
$COMPOSE_CMD -f "$COMPOSE_FILE" up -d

log_ok "Todos los servicios iniciados"

# -----------------------------------------------------------
# 5. Esperar health checks
# -----------------------------------------------------------
log_step "5/6 — Verificando salud de servicios"

SERVICES=(
    "postgres:5432"
    "redis:6379"
    "mameynode:8545"
    "sovereign-core:3050"
    "identity-service:5001"
    "zkp-service:5002"
    "treasury-service:5003"
    "bdet-bank:3001"
    "voz-soberana:3002"
    "red-social:3003"
    "correo-soberano:3004"
    "reservas:3005"
    "voto-soberano:3006"
    "trading:3007"
    "conferencia-soberana:3090"
    "vigilancia-soberana:3091"
    "empresa-soberana:3092"
    "pos-system:3030"
    "ierahkwa-shop:3100"
    "inventory-system:3200"
    "image-upload:3300"
    "forex-trading:3400"
    "smart-school:3500"
    "nginx-proxy:80"
    "prometheus:9090"
    "grafana:3000"
)

MAX_WAIT=120
INTERVAL=5
ELAPSED=0
HEALTHY=0
TOTAL=${#SERVICES[@]}

echo ""
log_info "Esperando que $TOTAL servicios pasen health check (max ${MAX_WAIT}s)..."

while [ $ELAPSED -lt $MAX_WAIT ]; do
    HEALTHY=0
    for SVC_PORT in "${SERVICES[@]}"; do
        SVC="${SVC_PORT%%:*}"
        STATUS=$(docker inspect --format='{{.State.Health.Status}}' "$SVC" 2>/dev/null || echo "unknown")
        if [ "$STATUS" = "healthy" ]; then
            HEALTHY=$((HEALTHY + 1))
        fi
    done

    printf "\r  Servicios saludables: ${GREEN}%d${NC}/${TOTAL}  (${ELAPSED}s)" "$HEALTHY"

    if [ "$HEALTHY" -eq "$TOTAL" ]; then
        break
    fi

    sleep $INTERVAL
    ELAPSED=$((ELAPSED + INTERVAL))
done

echo ""

if [ "$HEALTHY" -eq "$TOTAL" ]; then
    log_ok "Todos los servicios estan saludables"
else
    log_warn "$HEALTHY/$TOTAL servicios saludables despues de ${MAX_WAIT}s"
    log_info "Servicios no saludables:"
    for SVC_PORT in "${SERVICES[@]}"; do
        SVC="${SVC_PORT%%:*}"
        STATUS=$(docker inspect --format='{{.State.Health.Status}}' "$SVC" 2>/dev/null || echo "not_found")
        if [ "$STATUS" != "healthy" ]; then
            echo -e "  ${RED}x${NC} $SVC ($STATUS)"
        fi
    done
    log_info "Ejecuta '$COMPOSE_CMD -f $COMPOSE_FILE logs <servicio>' para diagnosticar"
fi

# -----------------------------------------------------------
# 6. Resumen final
# -----------------------------------------------------------
log_step "6/6 — Despliegue Soberano Completo"

echo ""
echo -e "${GREEN}${BOLD}  ================================================${NC}"
echo -e "${GREEN}${BOLD}   IERAHKWA NE KANIENKE — SERVICIOS ACTIVOS       ${NC}"
echo -e "${GREEN}${BOLD}  ================================================${NC}"
echo ""
echo -e "  ${BOLD}INFRAESTRUCTURA${NC}"
echo -e "  ───────────────────────────────────────────────"
echo -e "  Nginx Proxy           ${CYAN}http://localhost:80${NC}"
echo -e "  PostgreSQL             localhost:5432"
echo -e "  Redis                  localhost:6379"
echo -e "  MameyNode (RPC)       ${CYAN}http://localhost:8545${NC}"
echo ""
echo -e "  ${BOLD}.NET MICROSERVICIOS${NC}"
echo -e "  ───────────────────────────────────────────────"
echo -e "  Identity Service      ${CYAN}http://localhost:5001${NC}"
echo -e "  ZKP Service           ${CYAN}http://localhost:5002${NC}"
echo -e "  Treasury Service      ${CYAN}http://localhost:5003${NC}"
echo ""
echo -e "  ${BOLD}NODE.JS PLATAFORMAS${NC}"
echo -e "  ───────────────────────────────────────────────"
echo -e "  Sovereign Core        ${CYAN}http://localhost:3050${NC}"
echo -e "  BDET Bank             ${CYAN}http://localhost:3001${NC}"
echo -e "  Voz Soberana          ${CYAN}http://localhost:3002${NC}"
echo -e "  Red Social            ${CYAN}http://localhost:3003${NC}"
echo -e "  Correo Soberano       ${CYAN}http://localhost:3004${NC}"
echo -e "  Reservas              ${CYAN}http://localhost:3005${NC}"
echo -e "  Voto Soberano         ${CYAN}http://localhost:3006${NC}"
echo -e "  Trading               ${CYAN}http://localhost:3007${NC}"
echo -e "  POS System            ${CYAN}http://localhost:3030${NC}"
echo -e "  Conferencia Soberana  ${CYAN}http://localhost:3090${NC}"
echo -e "  Vigilancia Soberana   ${CYAN}http://localhost:3091${NC}"
echo -e "  Empresa Soberana      ${CYAN}http://localhost:3092${NC}"
echo -e "  Ierahkwa Shop         ${CYAN}http://localhost:3100${NC}"
echo -e "  Inventory System      ${CYAN}http://localhost:3200${NC}"
echo -e "  Image Upload          ${CYAN}http://localhost:3300${NC}"
echo -e "  Forex Trading         ${CYAN}http://localhost:3400${NC}"
echo -e "  Smart School          ${CYAN}http://localhost:3500${NC}"
echo ""
echo -e "  ${BOLD}MONITOREO${NC}"
echo -e "  ───────────────────────────────────────────────"
echo -e "  Prometheus            ${CYAN}http://localhost:9090${NC}"
echo -e "  Grafana               ${CYAN}http://localhost:3000${NC}"
echo ""
echo -e "  ${BOLD}SERVICIOS SOBERANOS${NC}"
echo -e "  ───────────────────────────────────────────────"
if [ "$TOR_AVAILABLE" = true ]; then
    echo -e "  Tor Hidden Service   ${GREEN}ACTIVO${NC}  .onion"
else
    echo -e "  Tor Hidden Service   ${YELLOW}DESACTIVADO${NC}"
fi
if [ "$IPFS_AVAILABLE" = true ]; then
    echo -e "  IPFS Node            ${GREEN}ACTIVO${NC}  Archivo Eterno"
else
    echo -e "  IPFS Node            ${YELLOW}DESACTIVADO${NC}"
fi
if [ "$OLLAMA_AVAILABLE" = true ]; then
    echo -e "  Ollama AI            ${GREEN}ACTIVO${NC}  Mediador Soberano"
else
    echo -e "  Ollama AI            ${YELLOW}DESACTIVADO${NC}"
fi
if [ "$HNS_AVAILABLE" = true ]; then
    echo -e "  Handshake DNS        ${GREEN}ACTIVO${NC}  .ierahkwa TLD"
else
    echo -e "  Handshake DNS        ${YELLOW}DESACTIVADO${NC}"
fi
if [ "$MESH_AVAILABLE" = true ]; then
    echo -e "  LoRa Mesh            ${GREEN}ACTIVO${NC}  Comunicacion Offline"
else
    echo -e "  LoRa Mesh            ${YELLOW}DESACTIVADO${NC}"
fi
echo ""
echo -e "  ${BOLD}PLATAFORMAS HTML${NC}"
echo -e "  ───────────────────────────────────────────────"
echo -e "  Portal Principal      ${CYAN}http://localhost/index.html${NC}"
echo -e "  Admin Dashboard       ${CYAN}http://localhost/admin-dashboard/${NC}"
echo ""
echo -e "  ───────────────────────────────────────────────"
echo -e "  Blockchain:  MameyNode v4.2 (Chain ID 777777)"
echo -e "  Servicios:   $TOTAL contenedores"
echo -e "  Plataformas: 422+ HTML soberanas"
echo -e "  Red:         sovereign-net (172.28.0.0/16)"
echo -e "  ───────────────────────────────────────────────"
echo ""
echo -e "  ${YELLOW}Comandos utiles:${NC}"
echo -e "  Ver logs:     $COMPOSE_CMD -f $COMPOSE_FILE logs -f <servicio>"
echo -e "  Estado:       $COMPOSE_CMD -f $COMPOSE_FILE ps"
echo -e "  Detener:      $COMPOSE_CMD -f $COMPOSE_FILE down"
echo -e "  Reiniciar:    $COMPOSE_CMD -f $COMPOSE_FILE restart <servicio>"
echo ""
echo -e "${GREEN}${BOLD}"
echo "  ╔══════════════════════════════════════════════╗"
echo "  ║                                              ║"
echo "  ║   G E N E S I S   B O O T   C O M P L E T E ║"
echo "  ║                                              ║"
echo "  ║   Soberania Digital Desplegada.              ║"
echo "  ║   La red no puede ser apagada.               ║"
echo "  ║   Los datos pertenecen al pueblo.            ║"
echo "  ║                                              ║"
echo "  ║   Ierahkwa Ne Kanienke — Siempre Soberana    ║"
echo "  ║                                              ║"
echo "  ╚══════════════════════════════════════════════╝"
echo -e "${NC}"
echo ""

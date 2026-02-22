#!/usr/bin/env bash
# ============================================================
# levantar-todo.sh - Levantador Maestro de la Plataforma Soberana
# Ierahkwa Ne Kanienke - Gobierno Soberano Digital
# ============================================================

set -euo pipefail

# -------------------- Colores --------------------
ROJO='\033[0;31m'
VERDE='\033[0;32m'
AMARILLO='\033[1;33m'
AZUL='\033[0;34m'
MORADO='\033[0;35m'
CYAN='\033[0;36m'
BLANCO='\033[1;37m'
NC='\033[0m'

# -------------------- Variables --------------------
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
COMPOSE_FILE="$PROJECT_DIR/docker-compose.sovereign.yml"
ENV_FILE="$PROJECT_DIR/.env"
ENV_EXAMPLE="$PROJECT_DIR/.env.example"

# -------------------- Funciones --------------------

banner() {
    echo -e "${MORADO}"
    echo "  =============================================="
    echo "    IERAHKWA NE KANIENKE"
    echo "    Plataforma Soberana Digital"
    echo "    MameyNode v4.2 | BDET Bank | 35+ Plataformas"
    echo "  =============================================="
    echo -e "${NC}"
}

info()    { echo -e "${AZUL}[INFO]${NC}    $1"; }
exito()   { echo -e "${VERDE}[EXITO]${NC}   $1"; }
aviso()   { echo -e "${AMARILLO}[AVISO]${NC}   $1"; }
error_msg() { echo -e "${ROJO}[ERROR]${NC}   $1"; }
paso()    { echo -e "${CYAN}[PASO]${NC}    $1"; }

check_command() {
    if command -v "$1" &> /dev/null; then
        exito "$2 encontrado: $(command -v "$1")"
        return 0
    else
        error_msg "$2 NO encontrado. Por favor instale $2."
        return 1
    fi
}

# -------------------- Inicio --------------------

banner

echo ""
paso "1/7 - Verificando prerequisitos..."
echo ""

FALLOS=0

# Docker
if check_command docker "Docker"; then
    DOCKER_VERSION=$(docker --version 2>/dev/null || echo "desconocida")
    info "  Version: $DOCKER_VERSION"
else
    FALLOS=$((FALLOS + 1))
fi

# Docker Compose
if docker compose version &> /dev/null; then
    exito "Docker Compose (plugin) encontrado"
    COMPOSE_CMD="docker compose"
elif command -v docker-compose &> /dev/null; then
    exito "docker-compose (standalone) encontrado"
    COMPOSE_CMD="docker-compose"
else
    error_msg "Docker Compose NO encontrado."
    FALLOS=$((FALLOS + 1))
    COMPOSE_CMD=""
fi

# Node.js (opcional - corre en Docker)
if check_command node "Node.js"; then
    NODE_VERSION=$(node --version 2>/dev/null || echo "desconocida")
    info "  Version: $NODE_VERSION"
else
    aviso "Node.js no encontrado localmente. Los servicios Node corren en Docker."
fi

# .NET SDK (opcional - corre en Docker)
if check_command dotnet ".NET SDK"; then
    DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "desconocida")
    info "  Version: $DOTNET_VERSION"
else
    aviso ".NET SDK no encontrado localmente. Los servicios .NET corren en Docker."
fi

echo ""

if [ "$FALLOS" -gt 0 ]; then
    error_msg "Hay $FALLOS prerequisitos faltantes obligatorios."
    error_msg "Instale Docker y Docker Compose antes de continuar."
    exit 1
fi

exito "Todos los prerequisitos obligatorios verificados."

# -------------------- Archivo .env --------------------

echo ""
paso "2/7 - Verificando archivo de configuracion (.env)..."

if [ ! -f "$ENV_FILE" ]; then
    if [ -f "$ENV_EXAMPLE" ]; then
        aviso "Archivo .env no encontrado. Generando desde .env.example..."
        cp "$ENV_EXAMPLE" "$ENV_FILE"

        # Generar passwords aleatorios si openssl esta disponible
        if command -v openssl &> /dev/null; then
            DB_PASS=$(openssl rand -base64 24 | tr -d '/+=' | head -c 32)
            REDIS_PASS=$(openssl rand -base64 24 | tr -d '/+=' | head -c 32)
            JWT_SEC=$(openssl rand -base64 48 | tr -d '/+=' | head -c 64)
            GRAFANA_PASS=$(openssl rand -base64 16 | tr -d '/+=' | head -c 20)

            if [[ "$OSTYPE" == "darwin"* ]]; then
                SED_CMD="sed -i ''"
            else
                SED_CMD="sed -i"
            fi

            $SED_CMD "s/CHANGE_THIS_PASSWORD/$DB_PASS/1" "$ENV_FILE"
            $SED_CMD "s/CHANGE_THIS_PASSWORD/$REDIS_PASS/1" "$ENV_FILE"
            $SED_CMD "s/CHANGE_THIS_256BIT_SECRET/$JWT_SEC/" "$ENV_FILE"
            $SED_CMD "s/CHANGE_THIS_PASSWORD/$GRAFANA_PASS/1" "$ENV_FILE"

            exito "Passwords generados automaticamente con OpenSSL."
            aviso "Revise y personalice $ENV_FILE antes de produccion."
        else
            aviso "OpenSSL no encontrado. Edite $ENV_FILE manualmente."
        fi
    else
        error_msg "No se encontro .env ni .env.example"
        error_msg "Cree el archivo .env con las variables necesarias."
        exit 1
    fi
else
    exito "Archivo .env encontrado."
fi

# -------------------- Verificar docker-compose --------------------

echo ""
paso "3/7 - Verificando archivo docker-compose..."

if [ ! -f "$COMPOSE_FILE" ]; then
    error_msg "Archivo no encontrado: $COMPOSE_FILE"
    exit 1
fi

exito "docker-compose.sovereign.yml encontrado."

# -------------------- Construir imagenes --------------------

echo ""
paso "4/7 - Construyendo imagenes Docker..."

cd "$PROJECT_DIR"
$COMPOSE_CMD -f "$COMPOSE_FILE" build --parallel 2>&1 | while IFS= read -r line; do
    echo -e "  ${BLANCO}$line${NC}"
done

exito "Imagenes construidas exitosamente."

# -------------------- Levantar servicios --------------------

echo ""
paso "5/7 - Levantando todos los servicios..."

$COMPOSE_CMD -f "$COMPOSE_FILE" up -d

exito "Todos los servicios iniciados."

# -------------------- Esperar health checks --------------------

echo ""
paso "6/7 - Esperando health checks (maximo 2 minutos por servicio)..."

SERVICIOS=(
    "postgres:5432:PostgreSQL"
    "redis:6379:Redis"
    "mameynode:8545:MameyNode"
    "identity-service:5001:Identity Service"
    "zkp-service:5002:ZKP Service"
    "treasury-service:5003:Treasury Service"
    "bdet-bank:3001:BDET Bank"
    "voz-soberana:3002:Voz Soberana"
    "red-social:3003:Red Social"
    "correo-soberano:3004:Correo Soberano"
    "reservas:3005:Reservas"
    "voto-soberano:3006:Voto Soberano"
    "trading:3007:Trading"
    "nginx-proxy:80:Nginx Proxy"
    "prometheus:9090:Prometheus"
    "grafana:3000:Grafana"
)

MAX_WAIT=120
INTERVALO=5
ESPERANDO=0

for srv in "${SERVICIOS[@]}"; do
    IFS=':' read -r container port nombre <<< "$srv"
    printf "  %-22s " "$nombre"

    HEALTHY=false
    ELAPSED=0
    while [ "$ELAPSED" -lt "$MAX_WAIT" ]; do
        STATUS=$(docker inspect --format='{{.State.Health.Status}}' "$container" 2>/dev/null || echo "unknown")
        if [ "$STATUS" = "healthy" ]; then
            HEALTHY=true
            break
        fi
        RUNNING=$(docker inspect --format='{{.State.Status}}' "$container" 2>/dev/null || echo "none")
        if [ "$RUNNING" != "running" ]; then
            break
        fi
        sleep "$INTERVALO"
        ELAPSED=$((ELAPSED + INTERVALO))
    done

    if [ "$HEALTHY" = true ]; then
        echo -e "${VERDE}LISTO${NC} (puerto $port)"
    else
        echo -e "${AMARILLO}VERIFICAR${NC} (puerto $port)"
        ESPERANDO=$((ESPERANDO + 1))
    fi
done

echo ""
if [ "$ESPERANDO" -eq 0 ]; then
    exito "Todos los 16 servicios estan saludables."
else
    aviso "$ESPERANDO servicio(s) requieren atencion. Ejecute: scripts/estado.sh"
fi

# -------------------- Mostrar estado final --------------------

echo ""
paso "7/7 - Estado final de la plataforma..."
echo ""

echo -e "${MORADO}  ==============================================${NC}"
echo -e "${MORADO}  SERVICIOS ACTIVOS${NC}"
echo -e "${MORADO}  ==============================================${NC}"
echo ""
echo -e "  ${CYAN}Blockchain${NC}"
echo -e "    MameyNode v4.2       http://127.0.0.1:8545"
echo ""
echo -e "  ${CYAN}Servicios .NET${NC}"
echo -e "    Identity Service     http://127.0.0.1:5001"
echo -e "    ZKP Service          http://127.0.0.1:5002"
echo -e "    Treasury Service     http://127.0.0.1:5003"
echo ""
echo -e "  ${CYAN}Plataformas Node.js${NC}"
echo -e "    BDET Bank            http://127.0.0.1:3001"
echo -e "    Voz Soberana         http://127.0.0.1:3002"
echo -e "    Red Social           http://127.0.0.1:3003"
echo -e "    Correo Soberano      http://127.0.0.1:3004"
echo -e "    Reservas             http://127.0.0.1:3005"
echo -e "    Voto Soberano        http://127.0.0.1:3006"
echo -e "    Trading              http://127.0.0.1:3007"
echo ""
echo -e "  ${CYAN}Infraestructura${NC}"
echo -e "    Nginx Proxy          http://127.0.0.1:80"
echo -e "    Nginx (HTTPS)        https://127.0.0.1:443"
echo -e "    PostgreSQL 16        127.0.0.1:5432"
echo -e "    Redis 7              127.0.0.1:6379"
echo ""
echo -e "  ${CYAN}Monitoreo${NC}"
echo -e "    Prometheus           http://127.0.0.1:9090"
echo -e "    Grafana              http://127.0.0.1:3000"
echo ""

# -------------------- Abrir dashboard en navegador --------------------

if [[ "$OSTYPE" == "darwin"* ]]; then
    open "http://127.0.0.1:3000" 2>/dev/null || true
elif command -v xdg-open &> /dev/null; then
    xdg-open "http://127.0.0.1:3000" 2>/dev/null || true
fi

echo -e "${VERDE}"
echo "  =============================================="
echo "  PLATAFORMA SOBERANA ACTIVA"
echo "  16 servicios | 0% impuestos | 35+ plataformas"
echo "  Dashboard: http://127.0.0.1:3000"
echo "  Estado:    bash scripts/estado.sh"
echo "  Soberania siempre."
echo "  =============================================="
echo -e "${NC}"

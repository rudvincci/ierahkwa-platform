#!/usr/bin/env bash
# ============================================================
# estado.sh - Verificador de Estado de la Plataforma Soberana
# Ierahkwa Ne Kanienke - Gobierno Soberano Digital
# ============================================================

set -uo pipefail

# -------------------- Colores --------------------
ROJO='\033[0;31m'
VERDE='\033[0;32m'
AMARILLO='\033[1;33m'
AZUL='\033[0;34m'
MORADO='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m'

# -------------------- Banner --------------------

echo -e "${MORADO}"
echo "  =============================================="
echo "    IERAHKWA NE KANIENKE"
echo "    Estado de la Plataforma Soberana"
echo "  =============================================="
echo -e "${NC}"

# -------------------- Verificar Docker --------------------

if ! command -v docker &> /dev/null; then
    echo -e "${ROJO}[ERROR]${NC} Docker no esta instalado o no esta en PATH."
    exit 1
fi

if ! docker info &> /dev/null; then
    echo -e "${ROJO}[ERROR]${NC} Docker daemon no esta corriendo."
    exit 1
fi

# -------------------- Funcion de verificacion --------------------

check_service() {
    local container="$1"
    local port="$2"
    local nombre="$3"
    local tipo="$4"

    local status
    local health

    status=$(docker inspect --format='{{.State.Status}}' "$container" 2>/dev/null || echo "no existe")
    health=$(docker inspect --format='{{.State.Health.Status}}' "$container" 2>/dev/null || echo "n/a")

    local status_icon
    local health_text

    if [ "$status" = "running" ]; then
        if [ "$health" = "healthy" ]; then
            status_icon="${VERDE}ACTIVO ${NC}"
            health_text="${VERDE}sano     ${NC}"
        elif [ "$health" = "unhealthy" ]; then
            status_icon="${ROJO}MAL    ${NC}"
            health_text="${ROJO}enfermo  ${NC}"
        elif [ "$health" = "starting" ]; then
            status_icon="${AMARILLO}INIC.  ${NC}"
            health_text="${AMARILLO}iniciando${NC}"
        else
            status_icon="${VERDE}ACTIVO ${NC}"
            health_text="${AZUL}sin check${NC}"
        fi
    elif [ "$status" = "no existe" ]; then
        status_icon="${ROJO}NULO   ${NC}"
        health_text="${ROJO}no existe${NC}"
    elif [ "$status" = "exited" ]; then
        status_icon="${ROJO}PARADO ${NC}"
        health_text="${ROJO}detenido ${NC}"
    else
        status_icon="${ROJO}$status${NC}"
        health_text="${ROJO}desconocido${NC}"
    fi

    printf "  %-5s %-24s %b  %b  %s\n" "$tipo" "$nombre" "$status_icon" "$health_text" "127.0.0.1:$port"
}

# -------------------- Tabla de servicios --------------------

echo ""
printf "  ${CYAN}%-5s %-24s %-12s %-14s %s${NC}\n" "TIPO" "SERVICIO" "ESTADO" "SALUD" "DIRECCION"
echo "  ----- ------------------------ ---------- ------------ -----------------"

# Blockchain
check_service "mameynode" "8545" "MameyNode v4.2" "BC"

echo ""

# Servicios .NET
check_service "identity-service" "5001" "Identity Service" ".NET"
check_service "zkp-service" "5002" "ZKP Service" ".NET"
check_service "treasury-service" "5003" "Treasury Service" ".NET"

echo ""

# Servicios Node.js
check_service "bdet-bank" "3001" "BDET Bank" "NODE"
check_service "voz-soberana" "3002" "Voz Soberana" "NODE"
check_service "red-social" "3003" "Red Social" "NODE"
check_service "correo-soberano" "3004" "Correo Soberano" "NODE"
check_service "reservas" "3005" "Reservas" "NODE"
check_service "voto-soberano" "3006" "Voto Soberano" "NODE"
check_service "trading" "3007" "Trading" "NODE"

echo ""

# Infraestructura
check_service "nginx-proxy" "80" "Nginx Proxy" "PROXY"
check_service "postgres" "5432" "PostgreSQL 16" "DB"
check_service "redis" "6379" "Redis 7" "DB"

echo ""

# Monitoreo
check_service "prometheus" "9090" "Prometheus" "MON"
check_service "grafana" "3000" "Grafana" "MON"

# -------------------- Resumen --------------------

echo ""
echo -e "  ${CYAN}--- Resumen ---${NC}"

CONTAINERS=(mameynode identity-service zkp-service treasury-service bdet-bank voz-soberana red-social correo-soberano reservas voto-soberano trading nginx-proxy postgres redis prometheus grafana)
TOTAL=${#CONTAINERS[@]}
RUNNING=0
HEALTHY=0
STOPPED=0

for c in "${CONTAINERS[@]}"; do
    status=$(docker inspect --format='{{.State.Status}}' "$c" 2>/dev/null || echo "none")
    health=$(docker inspect --format='{{.State.Health.Status}}' "$c" 2>/dev/null || echo "none")
    if [ "$status" = "running" ]; then
        RUNNING=$((RUNNING + 1))
        if [ "$health" = "healthy" ]; then
            HEALTHY=$((HEALTHY + 1))
        fi
    else
        STOPPED=$((STOPPED + 1))
    fi
done

if [ "$RUNNING" -eq "$TOTAL" ]; then
    echo -e "  ${VERDE}Servicios activos:    $RUNNING / $TOTAL${NC}"
elif [ "$RUNNING" -gt 0 ]; then
    echo -e "  ${AMARILLO}Servicios activos:    $RUNNING / $TOTAL${NC}"
else
    echo -e "  ${ROJO}Servicios activos:    $RUNNING / $TOTAL${NC}"
fi

echo -e "  ${AZUL}Servicios sanos:      $HEALTHY / $TOTAL${NC}"

if [ "$STOPPED" -gt 0 ]; then
    echo -e "  ${ROJO}Servicios detenidos:  $STOPPED${NC}"
fi

# -------------------- Uso de recursos --------------------

echo ""
echo -e "  ${CYAN}--- Uso de Recursos ---${NC}"
echo ""
printf "  %-24s %-10s %s\n" "CONTENEDOR" "CPU" "MEMORIA"
echo "  ------------------------ ---------- -------------------"

docker stats --no-stream --format "  {{printf \"%-24s\" .Name}} {{printf \"%-10s\" .CPUPerc}} {{.MemUsage}}" 2>/dev/null | sort || echo "  No se pudo obtener estadisticas."

# -------------------- Volumenes --------------------

echo ""
echo -e "  ${CYAN}--- Volumenes ---${NC}"
echo ""
docker volume ls --filter "name=sovereign" --filter "name=postgres" --filter "name=redis" --filter "name=mameynode" --filter "name=grafana" --filter "name=prometheus" --format "  {{.Name}}  ({{.Driver}})" 2>/dev/null || echo "  Sin volumenes encontrados."

echo ""
echo -e "${MORADO}  Soberania siempre. | Dashboard: http://127.0.0.1:3000${NC}"
echo ""

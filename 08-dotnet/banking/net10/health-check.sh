#!/bin/bash

# ════════════════════════════════════════════════════════════════
# IERAHKWA NET10 - HEALTH CHECK SCRIPT
# Verificación de salud del sistema
# ════════════════════════════════════════════════════════════════

BASE_URL=${1:-http://localhost:5071}
TIMEOUT=5

echo "╔═══════════════════════════════════════════════════════════════╗"
echo "║   IERAHKWA NET10 - HEALTH CHECK                              ║"
echo "║   Verificando estado del sistema...                           ║"
echo "╚═══════════════════════════════════════════════════════════════╝"
echo ""

# Función para verificar endpoint
check_endpoint() {
    local endpoint=$1
    local name=$2
    
    echo -n "🔍 Verificando $name... "
    
    response=$(curl -s -w "\n%{http_code}" --max-time $TIMEOUT "$BASE_URL$endpoint" 2>/dev/null)
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    if [ "$http_code" = "200" ] || [ "$http_code" = "200" ]; then
        echo "✅ OK (HTTP $http_code)"
        return 0
    else
        echo "❌ FAILED (HTTP $http_code)"
        return 1
    fi
}

# Verificar endpoints principales
echo "📡 Verificando endpoints principales:"
echo ""

check_endpoint "/health" "Health Check"
check_endpoint "/api/health" "API Health"
check_endpoint "/api/health/services" "Services Status"
check_endpoint "/swagger/index.html" "Swagger UI"
check_endpoint "/index.html" "DeFi Frontend"
check_endpoint "/dashboard.html" "Dashboard"
check_endpoint "/erp.html" "ERP Frontend"
check_endpoint "/gomoney.html" "GoMoney Frontend"
check_endpoint "/geocoder.html" "Geocoder Frontend"
check_endpoint "/contributions.html" "Contributions Frontend"

echo ""
echo "📊 Verificando APIs de productos:"
echo ""

check_endpoint "/api/college" "College API"
check_endpoint "/api/cybercafe" "CyberCafe API"
check_endpoint "/api/hospital" "Hospital API"
check_endpoint "/api/inventory" "Inventory API"
check_endpoint "/api/finance" "Finance API"
check_endpoint "/api/erp/invoices" "ERP API"
check_endpoint "/api/swap" "DeFi Swap API"
check_endpoint "/api/pool" "Pools API"
check_endpoint "/api/farm" "Farms API"
check_endpoint "/api/token" "Token API"
check_endpoint "/api/hotel" "Hotel API"
check_endpoint "/api/geocoding" "Geocoding API"
check_endpoint "/api/web-erp" "Web ERP API"
check_endpoint "/api/bank" "Banking API"
check_endpoint "/api/contribution" "Contribution API"

echo ""
echo "╔═══════════════════════════════════════════════════════════════╗"
echo "║   ✅ HEALTH CHECK COMPLETADO                                  ║"
echo "╚═══════════════════════════════════════════════════════════════╝"

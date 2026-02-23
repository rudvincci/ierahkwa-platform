#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════════════
#  IERAHKWA - SMOKE TEST PARA SERVICIOS LIVE (local o nube)
#  Comprueba /health, /ready, /api/health, etc.
#  Uso:
#    ./scripts/test-live.sh
#    NODE_URL=https://node.railway.app BRIDGE_URL=https://bridge.railway.app ./scripts/test-live.sh
#  Variables: NODE_URL, BRIDGE_URL, BANKING_URL, PLATFORM_URL (default: localhost)
# ═══════════════════════════════════════════════════════════════════════════════

NODE_URL="${NODE_URL:-http://localhost:8545}"
BRIDGE_URL="${BRIDGE_URL:-http://localhost:3001}"
BANKING_URL="${BANKING_URL:-http://localhost:5000}"
PLATFORM_URL="${PLATFORM_URL:-http://localhost:8080}"

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

OK=0
FAIL=0

check() {
    local name="$1"
    local url="$2"
    local method="${3:-GET}"
    if curl -sf --connect-timeout 5 --max-time 10 -X "$method" "$url" >/dev/null 2>&1; then
        echo -e "  ${GREEN}✓${NC} $name"
        ((OK++)) || true
        return 0
    else
        echo -e "  ${RED}✗${NC} $name  ($url)"
        ((FAIL++)) || true
        return 1
    fi
}

echo "╔══════════════════════════════════════════════════════════════════════════════╗"
echo "║  IERAHKWA - SMOKE TEST (live / local)                                        ║"
echo "╚══════════════════════════════════════════════════════════════════════════════╝"
echo ""
echo "  NODE_URL=$NODE_URL"
echo "  BRIDGE_URL=$BRIDGE_URL"
echo "  BANKING_URL=$BANKING_URL"
echo "  PLATFORM_URL=$PLATFORM_URL"
echo ""

echo "Comprobando..."
check "Node /health"           "$NODE_URL/health"
check "Node /ready"            "$NODE_URL/ready"
check "Banking Bridge /api/health"  "$BRIDGE_URL/api/health"
check "Banking Bridge /api/status"   "$BRIDGE_URL/api/status"
check "Banking .NET /health"   "$BANKING_URL/health"
check "Platform /health"       "$PLATFORM_URL/health"

# Opcional: /api/ready del Bridge (puede fallar si Banking .NET no está)
if curl -sf --connect-timeout 3 --max-time 5 "$BRIDGE_URL/api/ready" >/dev/null 2>&1; then
    echo -e "  ${GREEN}✓${NC} Banking Bridge /api/ready (Banking .NET conectado)"
    ((OK++)) || true
else
    echo -e "  ${YELLOW}○${NC} Banking Bridge /api/ready (Banking .NET no alcanzable o no configurado)"
fi

echo ""
echo "──────────────────────────────────────────────────────────────────────────────"
echo -e "  ${GREEN}OK: $OK${NC}   ${RED}Fallos: $FAIL${NC}"
echo ""

if [ "$FAIL" -gt 0 ]; then
    exit 1
fi
exit 0

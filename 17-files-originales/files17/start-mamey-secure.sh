#!/bin/bash
#
# 🦅 MAMEY SECURE STARTUP — Ierahkwa Sovereign Platform
# All services bind to 127.0.0.1 ONLY
#
set -euo pipefail

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$DIR"

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

BIND="127.0.0.1"
LOGS="$DIR/logs"

echo ""
echo -e "${CYAN}╔═══════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  🦅 IERAHKWA SOVEREIGN PLATFORM                  ║${NC}"
echo -e "${CYAN}║     Chain 777777 · Secure Startup                 ║${NC}"
echo -e "${CYAN}╚═══════════════════════════════════════════════════╝${NC}"
echo ""

mkdir -p "$LOGS"

# ── Pre-flight checks ──
echo -e "${CYAN}[1/5] Pre-flight checks...${NC}"

check_port() {
    if lsof -i ":$1" >/dev/null 2>&1; then
        echo -e "  ${RED}✗ Port $1 in use!${NC}"
        lsof -i ":$1" | head -2
        return 1
    fi
    return 0
}

PORTS_OK=true
for p in 8545 5001 5002 5003; do
    check_port "$p" || PORTS_OK=false
done

if [ "$PORTS_OK" = false ]; then
    echo -e "${YELLOW}  Stop existing services first: ./stop-mamey.sh${NC}"
    echo -e "${YELLOW}  Or kill: lsof -ti :8545,:5001,:5002,:5003 | xargs kill${NC}"
    exit 1
fi
echo -e "  ${GREEN}✓ All ports free${NC}"

# ── Load config ──
echo -e "${CYAN}[2/5] Loading config...${NC}"
if [ -f ".env" ]; then
    source .env 2>/dev/null || true
    echo -e "  ${GREEN}✓ .env loaded${NC}"
else
    echo -e "  ${YELLOW}⚠ No .env — using defaults${NC}"
fi

# ── Build ──
echo -e "${CYAN}[3/5] Building services...${NC}"

# MameyNode (Rust)
if [ -d "MameyNode.Rust" ] && [ -f "MameyNode.Rust/Cargo.toml" ]; then
    echo -e "  Building MameyNode.Rust..."
    cd MameyNode.Rust
    cargo build --release 2>"$LOGS/rust-build.log" && echo -e "  ${GREEN}✓ Rust node built${NC}" || echo -e "  ${YELLOW}⚠ Rust build skipped${NC}"
    cd "$DIR"
fi

# .NET services
for svc in Mamey.Government.Identity; do
    if [ -d "$svc" ] && [ -f "$svc/src"/*.csproj 2>/dev/null ] || [ -f "$svc"/*.csproj 2>/dev/null ]; then
        echo -e "  Building $svc..."
        cd "$svc"
        dotnet build -c Release 2>"$LOGS/${svc}-build.log" && echo -e "  ${GREEN}✓ $svc built${NC}" || echo -e "  ${YELLOW}⚠ $svc build skipped${NC}"
        cd "$DIR"
    fi
done
echo -e "  ${GREEN}✓ Build complete${NC}"

# ── Start services ──
echo -e "${CYAN}[4/5] Starting services on $BIND...${NC}"

# MameyNode
if [ -f "MameyNode.Rust/target/release/mamey-node" ]; then
    ./MameyNode.Rust/target/release/mamey-node --bind "$BIND:8545" > "$LOGS/mamey-node.log" 2>&1 &
    echo $! > "$LOGS/mamey-node.pid"
    echo -e "  ${GREEN}✓ MameyNode.Rust on $BIND:8545${NC}"
elif [ -f "node/server.js" ]; then
    node node/server.js > "$LOGS/mamey-node.log" 2>&1 &
    echo $! > "$LOGS/mamey-node.pid"
    echo -e "  ${GREEN}✓ MameyNode.JS on $BIND:8545${NC}"
else
    echo -e "  ${YELLOW}⚠ No node binary found${NC}"
fi

# Identity service — SECURE: 127.0.0.1 only
if find Mamey.Government.Identity -name "*.dll" -path "*/Release/*" 2>/dev/null | head -1 | grep -q .; then
    DLL=$(find Mamey.Government.Identity -name "Mamey.Government.Identity.dll" -path "*/Release/*" 2>/dev/null | head -1)
    if [ -n "$DLL" ]; then
        dotnet "$DLL" --urls="http://$BIND:5001" > "$LOGS/identity.log" 2>&1 &
        echo $! > "$LOGS/identity.pid"
        echo -e "  ${GREEN}✓ Identity on $BIND:5001${NC}"
    fi
fi

# ── Health checks ──
echo -e "${CYAN}[5/5] Health checks...${NC}"
sleep 2

for port in 8545 5001; do
    if curl -s "http://$BIND:$port/health" >/dev/null 2>&1 || lsof -i ":$port" >/dev/null 2>&1; then
        echo -e "  ${GREEN}✓ :$port responding${NC}"
    else
        echo -e "  ${YELLOW}⚠ :$port not responding yet${NC}"
    fi
done

echo ""
echo -e "${GREEN}╔═══════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  🦅 IERAHKWA SOVEREIGN PLATFORM RUNNING           ║${NC}"
echo -e "${GREEN}╠═══════════════════════════════════════════════════╣${NC}"
echo -e "${GREEN}║${NC}  ⛓️  MameyNode       http://$BIND:8545           ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  👤 Identity        http://$BIND:5001           ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}                                                 ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  🔒 Bind: $BIND (not 0.0.0.0)             ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  🪙 Tokens: WAMPUM · SICBDC · IGT              ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  📁 Logs: $LOGS/                     ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  🛑 Stop: ./stop-mamey.sh                       ${GREEN}║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════╝${NC}"
echo ""

#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════════════
#  IERAHKWA SOVEREIGN PLATFORM - PRODUCTION START SCRIPT
#  Starts all services with PM2 and systemd for 24/7 operation
# ═══════════════════════════════════════════════════════════════════════════════

set -e

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo "╔══════════════════════════════════════════════════════════════════════════════╗"
echo "║     🚀 IERAHKWA SOVEREIGN PLATFORM - PRODUCTION START                       ║"
echo "╚══════════════════════════════════════════════════════════════════════════════╝"
echo ""

# Check prerequisites
echo "🔍 Checking prerequisites..."

# Check PM2
if ! command -v pm2 &> /dev/null; then
    echo -e "${RED}❌ PM2 not found. Installing...${NC}"
    npm install -g pm2
fi

# Check Node.js
if ! command -v node &> /dev/null; then
    echo -e "${RED}❌ Node.js not found!${NC}"
    exit 1
fi

# Check .NET
if ! command -v dotnet &> /dev/null; then
    echo -e "${YELLOW}⚠️  .NET not found. .NET services will be skipped.${NC}"
fi

echo -e "${GREEN}✅ Prerequisites OK${NC}"
echo ""

# Start Node.js services with PM2
echo "📡 Starting Node.js services with PM2..."

# Node server
if [ -f "$ROOT/node/ecosystem.config.js" ]; then
    cd "$ROOT/node"
    pm2 start ecosystem.config.js
    echo -e "${GREEN}✅ Node services started${NC}"
else
    echo -e "${YELLOW}⚠️  node/ecosystem.config.js not found${NC}"
fi

# Platform API
if [ -f "$ROOT/platform/ecosystem.config.js" ]; then
    cd "$ROOT/platform"
    pm2 start ecosystem.config.js
    echo -e "${GREEN}✅ Platform services started${NC}"
fi

# Save PM2 configuration
pm2 save

echo ""
echo "═══════════════════════════════════════════════════════════════════════════════"
echo -e "${GREEN}  ✅ PRODUCTION SERVICES STARTED${NC}"
echo "═══════════════════════════════════════════════════════════════════════════════"
echo ""
echo "📊 PM2 Status:"
pm2 list
echo ""
echo "📝 Useful commands:"
echo "   pm2 logs              - View logs"
echo "   pm2 monit             - Monitor resources"
echo "   pm2 restart all       - Restart all services"
echo "   pm2 stop all          - Stop all services"
echo "   pm2 save              - Save current process list"
echo ""
echo "🔗 Services:"
echo "   Main Platform:       http://localhost:8545"
echo "   Banking Bridge:       http://localhost:3001"
echo "   Platform API:         http://localhost:3000"
echo ""

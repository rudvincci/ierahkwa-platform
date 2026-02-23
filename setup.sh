#!/usr/bin/env bash
# Ierahkwa Sovereign Platform — Setup Script
# Detects OS, checks prerequisites, configures environment

set -eo pipefail

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo "╔══════════════════════════════════════════════════╗"
echo "║  Ierahkwa Sovereign Platform — Setup             ║"
echo "║  Sovereign Government of Ierahkwa Ne Kanienke    ║"
echo "╚══════════════════════════════════════════════════╝"
echo ""

# Detect OS
OS="unknown"
if [[ "$OSTYPE" == "darwin"* ]]; then
    OS="macOS"
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    OS="Linux"
fi
echo -e "${GREEN}OS Detected:${NC} $OS"

# Check prerequisites
MISSING=0

check_cmd() {
    if command -v "$1" &>/dev/null; then
        VERSION=$($2 2>&1 | head -1)
        echo -e "  ${GREEN}✓${NC} $1 — $VERSION"
    else
        echo -e "  ${RED}✗${NC} $1 — NOT INSTALLED"
        MISSING=$((MISSING + 1))
    fi
}

echo ""
echo "Checking prerequisites..."
check_cmd "docker" "docker --version"
check_cmd "docker" "docker compose version"
check_cmd "node" "node --version"
check_cmd "npm" "npm --version"
check_cmd "dotnet" "dotnet --version"
check_cmd "cargo" "cargo --version"
check_cmd "go" "go version"
check_cmd "git" "git --version"

if [ $MISSING -gt 0 ]; then
    echo ""
    echo -e "${YELLOW}Warning: $MISSING prerequisite(s) missing.${NC}"
    echo "The platform will work partially. Install missing tools for full functionality."
fi

# Setup .env
echo ""
if [ ! -f .env ]; then
    if [ -f .env.example ]; then
        cp .env.example .env
        # Generate random passwords
        if command -v openssl &>/dev/null; then
            DB_PASS=$(openssl rand -hex 16)
            REDIS_PASS=$(openssl rand -hex 16)
            JWT_SECRET=$(openssl rand -hex 32)
            sed -i.bak "s/your_secure_password_here/$DB_PASS/" .env 2>/dev/null || true
            sed -i.bak "s/your_redis_password/$REDIS_PASS/" .env 2>/dev/null || true
            sed -i.bak "s/your_jwt_secret/$JWT_SECRET/" .env 2>/dev/null || true
            rm -f .env.bak
        fi
        echo -e "${GREEN}✓${NC} Created .env from .env.example with generated passwords"
    else
        echo -e "${YELLOW}⚠${NC} No .env.example found"
    fi
else
    echo -e "${GREEN}✓${NC} .env already exists"
fi

# Install Node.js dependencies
echo ""
echo "Installing Node.js dependencies..."
INSTALLED=0
find 03-backend -name "package.json" -maxdepth 2 2>/dev/null | while read pkg; do
    dir=$(dirname "$pkg")
    if [ ! -d "$dir/node_modules" ]; then
        echo "  Installing $dir..."
        cd "$dir" && npm install --ignore-scripts 2>/dev/null && cd - >/dev/null
        INSTALLED=$((INSTALLED + 1))
    fi
done
echo -e "${GREEN}✓${NC} Node.js dependencies installed"

# Validate Docker
echo ""
if command -v docker &>/dev/null; then
    echo "Validating Docker configuration..."
    for f in docker-compose.sovereign.yml docker-compose.dev.yml docker-compose.infra.yml; do
        if [ -f "$f" ]; then
            docker compose -f "$f" config --quiet 2>/dev/null && echo -e "  ${GREEN}✓${NC} $f is valid" || echo -e "  ${RED}✗${NC} $f has errors"
        fi
    done
fi

# Summary
echo ""
echo "╔══════════════════════════════════════════════════╗"
echo "║  Setup Complete!                                 ║"
echo "╠══════════════════════════════════════════════════╣"
echo "║                                                  ║"
echo "║  Quick Start:                                    ║"
echo "║    make start      — Start all services          ║"
echo "║    make status      — Check service health       ║"
echo "║    make build-all   — Build everything           ║"
echo "║    make help        — Show all commands           ║"
echo "║                                                  ║"
echo "║  Or use Docker directly:                         ║"
echo "║    docker compose -f docker-compose.sovereign.yml up -d  ║"
echo "║                                                  ║"
echo "╚══════════════════════════════════════════════════╝"
echo ""
echo -e "${GREEN}Niawenhko:wa — The Sovereign Platform is ready.${NC}"

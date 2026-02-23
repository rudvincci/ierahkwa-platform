#!/bin/bash
# ============================================================================
# IERAHKWA SOVEREIGN PLATFORM - DEPLOYMENT SCRIPT
# Complete deployment for Docker Compose stack
# ============================================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

echo -e "${CYAN}"
echo "🪶 ═══════════════════════════════════════════════════════════"
echo "   IERAHKWA SOVEREIGN PLATFORM - DEPLOYMENT"
echo "🪶 ═══════════════════════════════════════════════════════════"
echo -e "${NC}"

cd "$PROJECT_DIR"

# Check requirements
echo -e "${CYAN}[1/6] Checking requirements...${NC}"

if ! command -v docker &> /dev/null; then
    echo -e "${RED}Error: Docker is not installed${NC}"
    exit 1
fi

if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    echo -e "${RED}Error: Docker Compose is not installed${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Docker and Docker Compose available${NC}"

# Create environment file if not exists
echo -e "${CYAN}[2/6] Setting up environment...${NC}"

if [ ! -f ".env" ]; then
    echo "Creating .env file..."
    cat > .env << EOF
# IERAHKWA Sovereign Platform - Environment Variables
# Generated on $(date)

# Database
POSTGRES_PASSWORD=$(openssl rand -base64 32 | tr -dc 'a-zA-Z0-9' | head -c 32)

# Redis
REDIS_PASSWORD=$(openssl rand -base64 32 | tr -dc 'a-zA-Z0-9' | head -c 32)

# JWT
JWT_SECRET=$(openssl rand -base64 64 | tr -dc 'a-zA-Z0-9' | head -c 64)

# Environment
ASPNETCORE_ENVIRONMENT=Production
NODE_ENV=production
EOF
    echo -e "${GREEN}✓ Environment file created${NC}"
else
    echo -e "${GREEN}✓ Environment file exists${NC}"
fi

# Setup SSL
echo -e "${CYAN}[3/6] Setting up SSL certificates...${NC}"

if [ ! -f "nginx/ssl/fullchain.pem" ]; then
    bash scripts/setup-ssl.sh dev
else
    echo -e "${GREEN}✓ SSL certificates exist${NC}"
fi

# Build images
echo -e "${CYAN}[4/6] Building Docker images...${NC}"

docker compose build --parallel

echo -e "${GREEN}✓ Images built${NC}"

# Pull base images
echo -e "${CYAN}[5/6] Pulling base images...${NC}"

docker compose pull postgres redis nginx adminer

echo -e "${GREEN}✓ Base images pulled${NC}"

# Start services
echo -e "${CYAN}[6/6] Starting services...${NC}"

docker compose up -d

echo -e "${GREEN}✓ Services started${NC}"

# Wait for services to be healthy
echo ""
echo "Waiting for services to be ready..."
sleep 10

# Check health
echo ""
echo -e "${CYAN}Service Status:${NC}"
docker compose ps

# Show URLs
echo ""
echo -e "${GREEN}🪶 ═══════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}   DEPLOYMENT COMPLETE!${NC}"
echo -e "${GREEN}🪶 ═══════════════════════════════════════════════════════════${NC}"
echo ""
echo -e "   ${CYAN}Platform:${NC}        https://localhost"
echo -e "   ${CYAN}API:${NC}             https://localhost/api"
echo -e "   ${CYAN}Swagger:${NC}         https://localhost/swagger"
echo -e "   ${CYAN}Node Mamey:${NC}      https://localhost/mamey"
echo -e "   ${CYAN}Adminer (DB):${NC}    http://localhost:8080"
echo ""
echo -e "   ${YELLOW}Default Admin:${NC}"
echo -e "   Email: admin@ierahkwa.gov"
echo -e "   Password: Admin123!"
echo ""
echo -e "${CYAN}Commands:${NC}"
echo "   View logs:    docker compose logs -f"
echo "   Stop:         docker compose down"
echo "   Restart:      docker compose restart"
echo "   Status:       docker compose ps"
echo ""

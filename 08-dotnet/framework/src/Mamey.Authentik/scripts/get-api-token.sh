#!/bin/bash

# Script to help get an API token from the running Authentik instance
# Usage: ./scripts/get-api-token.sh

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
AUTHENTIK_BASE_URL="${AUTHENTIK_BASE_URL:-http://localhost:9100}"
AUTHENTIK_ADMIN_URL="${AUTHENTIK_BASE_URL}/if/admin/"

echo -e "${GREEN}Authentik API Token Helper${NC}"
echo "================================"
echo ""
echo "This script helps you get an API token from your running Authentik instance."
echo ""
echo "Base URL: ${AUTHENTIK_BASE_URL}"
echo "Admin URL: ${AUTHENTIK_ADMIN_URL}"
echo ""

# Check if container is running
if ! docker ps --filter name=mamey-authentik-server --format "{{.Status}}" | grep -q "Up"; then
    echo -e "${RED}Error: Authentik container 'mamey-authentik-server' is not running.${NC}"
    echo ""
    echo "Start it with:"
    echo "  cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith"
    echo "  docker-compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d"
    exit 1
fi

echo -e "${GREEN}✓${NC} Authentik container is running"
echo ""

# Check if container is accessible
if ! curl -s -f "${AUTHENTIK_BASE_URL}/api/v3/" > /dev/null 2>&1; then
    echo -e "${YELLOW}Warning: Cannot access Authentik at ${AUTHENTIK_BASE_URL}${NC}"
    echo "The container may still be starting up. Please wait a moment and try again."
    exit 1
fi

echo -e "${GREEN}✓${NC} Authentik is accessible"
echo ""

echo "To get an API token:"
echo ""
echo "1. Open the Authentik admin interface in your browser:"
echo "   ${AUTHENTIK_ADMIN_URL}"
echo ""
echo "2. Navigate to: Applications → Tokens"
echo ""
echo "3. Click 'Create Token'"
echo ""
echo "4. Fill in the form:"
echo "   - Identifier: e.g., 'integration-tests'"
echo "   - Expires: Set expiration date (or leave blank for no expiration)"
echo "   - Permissions: Select appropriate permissions"
echo ""
echo "5. Click 'Create' and copy the token value"
echo ""
echo "6. Set the environment variable:"
echo "   export AUTHENTIK_API_TOKEN='your-token-here'"
echo ""
echo "Or add it to your shell profile (~/.zshrc or ~/.bashrc):"
echo "   echo 'export AUTHENTIK_API_TOKEN=\"your-token-here\"' >> ~/.zshrc"
echo ""
echo "7. Run the integration tests:"
echo "   cd /Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Authentik"
echo "   dotnet test --filter \"Category=Integration\""
echo ""

# Try to open browser if on macOS
if [[ "$OSTYPE" == "darwin"* ]]; then
    read -p "Would you like to open the admin interface in your browser? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        open "${AUTHENTIK_ADMIN_URL}"
    fi
fi

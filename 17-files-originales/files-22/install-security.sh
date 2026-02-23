#!/bin/bash
#
# INSTALAR SEGURIDAD EN MAMEY-MAIN
# Corre esto desde ~/Desktop/Mamey-main
#
set -euo pipefail

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$DIR"

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

echo ""
echo -e "${CYAN}╔══════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  🔒 INSTALANDO SEGURIDAD — Mamey-main          ║${NC}"
echo -e "${CYAN}╚══════════════════════════════════════════════════╝${NC}"
echo ""

# ── 1. Create security directory structure ──
echo -e "${CYAN}[1/7] Creando estructura de seguridad...${NC}"
mkdir -p .security/certs
mkdir -p .security/auth
mkdir -p .security/firewall
mkdir -p .security/backups
mkdir -p .security/audit-logs
mkdir -p .security/nginx
echo -e "  ${GREEN}✓ Estructura creada${NC}"

# ── 2. Generate .env ──
echo -e "${CYAN}[2/7] Generando configuración...${NC}"
if [ ! -f ".env" ]; then
cat > .env << 'ENVEOF'
# ═══════════════════════════════════════════
# MAMEY ECOSYSTEM — SECURE CONFIG
# NUNCA subir a git
# ═══════════════════════════════════════════

# Network — SIEMPRE 127.0.0.1
MAMEY_BIND_HOST=127.0.0.1
MAMEY_ALLOW_PUBLIC=false

# Ports
MAMEY_NODE_PORT=8545
MAMEY_IDENTITY_PORT=5001
MAMEY_ZKP_PORT=5002
MAMEY_TREASURY_PORT=5003

# Chain
MAMEY_CHAIN_ID=777777
MAMEY_NETWORK_NAME="Ierahkwa Sovereign Network"

# Logs
MAMEY_LOG_DIR=./logs
MAMEY_LOG_LEVEL=info
ENVEOF
echo -e "  ${GREEN}✓ .env creado${NC}"
else
echo -e "  ${GREEN}✓ .env ya existe${NC}"
fi

# ── 3. Generate JWT secret ──
echo -e "${CYAN}[3/7] Generando JWT secret...${NC}"
if [ ! -f ".security/auth/jwt-secret.key" ]; then
    openssl rand -hex 64 > .security/auth/jwt-secret.key
    chmod 600 .security/auth/jwt-secret.key
    echo -e "  ${GREEN}✓ JWT secret generado (256-bit)${NC}"
else
    echo -e "  ${GREEN}✓ JWT secret ya existe${NC}"
fi

# ── 4. Generate API keys ──
echo -e "${CYAN}[4/7] Generando API keys...${NC}"
if [ ! -f ".security/auth/api-keys.env" ]; then
cat > .security/auth/api-keys.env << EOF
# API Keys — Generadas $(date '+%Y-%m-%d %H:%M:%S')
IDENTITY_API_KEY=$(openssl rand -hex 32)
ZKP_API_KEY=$(openssl rand -hex 32)
TREASURY_API_KEY=$(openssl rand -hex 32)
INTER_SERVICE_KEY=$(openssl rand -hex 32)
ADMIN_API_KEY=$(openssl rand -hex 32)
EOF
    chmod 600 .security/auth/api-keys.env
    echo -e "  ${GREEN}✓ 5 API keys generadas (256-bit cada una)${NC}"
else
    echo -e "  ${GREEN}✓ API keys ya existen${NC}"
fi

# ── 5. Generate TLS certificates ──
echo -e "${CYAN}[5/7] Generando certificados TLS RSA-4096...${NC}"
if [ ! -f ".security/certs/sovereign.crt" ]; then
    # CA
    openssl req -x509 -nodes -newkey rsa:4096 \
        -keyout .security/certs/sovereign-ca.key \
        -out .security/certs/sovereign-ca.crt \
        -days 3650 \
        -subj "/C=XX/ST=Sovereign/L=Ierahkwa/O=Sovereign Government of Ierahkwa Ne Kanienke/CN=Mamey Sovereign CA" \
        2>/dev/null

    # Server cert
    openssl req -nodes -newkey rsa:4096 \
        -keyout .security/certs/sovereign.key \
        -out .security/certs/sovereign.csr \
        -subj "/C=XX/ST=Sovereign/L=Ierahkwa/O=Sovereign Government of Ierahkwa Ne Kanienke/CN=localhost" \
        2>/dev/null

    cat > .security/certs/_san.cnf << 'SANEOF'
[req]
distinguished_name = req_distinguished_name
[req_distinguished_name]
[v3_ext]
subjectAltName = DNS:localhost,DNS:*.mamey.sovereign,DNS:*.ierahkwa.sovereign,IP:127.0.0.1
keyUsage = digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
SANEOF

    openssl x509 -req \
        -in .security/certs/sovereign.csr \
        -CA .security/certs/sovereign-ca.crt \
        -CAkey .security/certs/sovereign-ca.key \
        -CAcreateserial \
        -out .security/certs/sovereign.crt \
        -days 365 \
        -extfile .security/certs/_san.cnf \
        -extensions v3_ext \
        2>/dev/null

    rm -f .security/certs/sovereign.csr .security/certs/_san.cnf .security/certs/*.srl
    chmod 600 .security/certs/*.key
    chmod 644 .security/certs/*.crt

    echo -e "  ${GREEN}✓ TLS RSA-4096 — CA + Server cert generados${NC}"
    echo -e "  ${GREEN}  Válidos: 365 días${NC}"
else
    echo -e "  ${GREEN}✓ Certificados ya existen${NC}"
fi

# ── 6. Create firewall rules ──
echo -e "${CYAN}[6/7] Configurando firewall...${NC}"
cat > .security/firewall/pf-mamey.conf << 'PFEOF'
# Mamey Sovereign Firewall — macOS pf
# Activar: sudo pfctl -ef .security/firewall/pf-mamey.conf

# Block external access to internal ports
block drop in proto tcp from any to any port 5001
block drop in proto tcp from any to any port 5002
block drop in proto tcp from any to any port 5003
block drop in proto tcp from any to any port 8545

# Allow localhost
pass in proto tcp from 127.0.0.1 to 127.0.0.1 port {5001, 5002, 5003, 8545}

# Allow HTTPS
pass in proto tcp from any to any port 443
PFEOF
echo -e "  ${GREEN}✓ Firewall rules generadas${NC}"

# ── 7. Update .gitignore ──
echo -e "${CYAN}[7/7] Protegiendo secretos en git...${NC}"

# Check if .gitignore exists and add security entries
SECURITY_ENTRIES="
# ═══ MAMEY SECURITY — NEVER COMMIT ═══
.security/certs/*.key
.security/auth/
.env
*.key
*.pem
*.p12
logs/
*.pid
*.log
"

if [ -f ".gitignore" ]; then
    if ! grep -q "MAMEY SECURITY" .gitignore 2>/dev/null; then
        echo "$SECURITY_ENTRIES" >> .gitignore
        echo -e "  ${GREEN}✓ .gitignore actualizado${NC}"
    else
        echo -e "  ${GREEN}✓ .gitignore ya protegido${NC}"
    fi
else
    echo "$SECURITY_ENTRIES" > .gitignore
    echo -e "  ${GREEN}✓ .gitignore creado${NC}"
fi

# Set permissions
chmod 750 .security
chmod 700 .security/auth
chmod 700 .security/certs
find .security -name "*.key" -exec chmod 600 {} \; 2>/dev/null
find . -name ".env" -exec chmod 640 {} \; 2>/dev/null

echo ""
echo -e "${GREEN}╔══════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  ✅ SEGURIDAD INSTALADA                         ║${NC}"
echo -e "${GREEN}╠══════════════════════════════════════════════════╣${NC}"
echo -e "${GREEN}║${NC}  .security/certs/    TLS RSA-4096 + CA soberana  ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  .security/auth/     JWT secret + 5 API keys    ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  .security/firewall/ macOS pf rules             ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  .env                Config segura               ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  .gitignore          Secretos protegidos         ${GREEN}║${NC}"
echo -e "${GREEN}║                                                  ║${NC}"
echo -e "${GREEN}║${NC}  ${YELLOW}Para activar firewall:${NC}                          ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  sudo pfctl -ef .security/firewall/pf-mamey.conf${GREEN}║${NC}"
echo -e "${GREEN}║                                                  ║${NC}"
echo -e "${GREEN}║${NC}  ${YELLOW}Para confiar CA en macOS:${NC}                       ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  sudo security add-trusted-cert -d -r trustRoot ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  -k /Library/Keychains/System.keychain           ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  .security/certs/sovereign-ca.crt                ${GREEN}║${NC}"
echo -e "${GREEN}╚══════════════════════════════════════════════════╝${NC}"
echo ""

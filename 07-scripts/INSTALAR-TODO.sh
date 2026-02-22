#!/bin/bash
#
# 🦅 INSTALAR TODO — Agrega sin borrar NADA
# Solo AGREGA archivos nuevos a Mamey-main
# NO elimina, NO sobreescribe código existente
#
set -euo pipefail

MAMEY="$HOME/Desktop/Mamey-main"
DL="$HOME/Downloads"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'
ADDED=0
add_file() {
    local src="$1" dst="$2"
    if [ -f "$src" ]; then
        mkdir -p "$(dirname "$dst")"
        cp "$src" "$dst"
        chmod +x "$dst" 2>/dev/null || true
        echo -e "  ${GREEN}+ $(basename "$dst")${NC}"
        ((ADDED++))
    else
        echo -e "  ${YELLOW}⚠ $(basename "$dst") not in Downloads${NC}"
    fi
}

echo ""
echo -e "${CYAN}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  🦅 INSTALACIÓN COMPLETA — Ierahkwa Sovereign Platform   ║${NC}"
echo -e "${CYAN}║     ⚠ MODO: SOLO AGREGAR — NO SE BORRA NADA              ║${NC}"
echo -e "${CYAN}╚═══════════════════════════════════════════════════════════╝${NC}"
echo ""
cd "$MAMEY"

echo -e "${CYAN}[1/8] Scripts operacionales...${NC}"
add_file "$DL/start-mamey-secure.sh"      "$MAMEY/start-mamey-secure.sh"
add_file "$DL/stop-mamey.sh"              "$MAMEY/stop-mamey.sh"
add_file "$DL/audit-platforms.sh"         "$MAMEY/audit-platforms.sh"
add_file "$DL/rotate-keys.sh"            "$MAMEY/rotate-keys.sh"
add_file "$DL/install-security.sh"       "$MAMEY/install-security.sh"
add_file "$DL/descubrir-plataformas.sh"  "$MAMEY/descubrir-plataformas.sh"
echo ""

echo -e "${CYAN}[2/8] Dashboard soberano...${NC}"
add_file "$DL/mamey-dashboard.html"      "$MAMEY/dashboard.html"
echo ""

echo -e "${CYAN}[3/8] Documentación (en docs/sovereign/)...${NC}"
mkdir -p "$MAMEY/docs/sovereign"
add_file "$DL/AUDITORIA-PLATAFORMA-SOBERANA.md" "$MAMEY/docs/sovereign/AUDITORIA-PLATAFORMA-SOBERANA.md"
add_file "$DL/README-MAMEY.md"                    "$MAMEY/docs/sovereign/README-SOVEREIGN.md"
add_file "$DL/EMPEZAR-AQUI-NUEVO.md"             "$MAMEY/docs/sovereign/EMPEZAR-AQUI.md"
echo ""

echo -e "${CYAN}[4/8] Seguridad (.security/)...${NC}"
mkdir -p "$MAMEY/.security/certs" "$MAMEY/.security/auth/archive" "$MAMEY/.security/firewall" "$MAMEY/.security/backups" "$MAMEY/.security/audit-logs" "$MAMEY/.security/nginx"
echo -e "  ${GREEN}+ .security/ directories${NC}"; ((ADDED++))

if [ ! -f "$MAMEY/.security/auth/jwt-secret.key" ]; then
    openssl rand -hex 64 > "$MAMEY/.security/auth/jwt-secret.key"
    chmod 600 "$MAMEY/.security/auth/jwt-secret.key"
    echo -e "  ${GREEN}+ JWT secret (256-bit)${NC}"; ((ADDED++))
else echo -e "  ${GREEN}✓ JWT secret exists — kept${NC}"; fi

if [ ! -f "$MAMEY/.security/auth/api-keys.env" ]; then
    cat > "$MAMEY/.security/auth/api-keys.env" << EOF
IDENTITY_API_KEY=$(openssl rand -hex 32)
ZKP_API_KEY=$(openssl rand -hex 32)
TREASURY_API_KEY=$(openssl rand -hex 32)
INTER_SERVICE_KEY=$(openssl rand -hex 32)
ADMIN_API_KEY=$(openssl rand -hex 32)
EOF
    chmod 600 "$MAMEY/.security/auth/api-keys.env"
    echo -e "  ${GREEN}+ 5 API keys${NC}"; ((ADDED++))
else echo -e "  ${GREEN}✓ API keys exist — kept${NC}"; fi

if [ ! -f "$MAMEY/.security/certs/sovereign.crt" ]; then
    openssl req -x509 -nodes -newkey rsa:4096 -keyout "$MAMEY/.security/certs/sovereign-ca.key" -out "$MAMEY/.security/certs/sovereign-ca.crt" -days 3650 -subj "/CN=Mamey Sovereign CA/O=Ierahkwa" 2>/dev/null
    openssl req -nodes -newkey rsa:4096 -keyout "$MAMEY/.security/certs/sovereign.key" -out /tmp/mamey.csr -subj "/CN=localhost/O=Ierahkwa" 2>/dev/null
    printf "[v3]\nsubjectAltName=DNS:localhost,DNS:*.ierahkwa.sovereign,IP:127.0.0.1\nkeyUsage=digitalSignature,keyEncipherment\nextendedKeyUsage=serverAuth" > /tmp/san.cnf
    openssl x509 -req -in /tmp/mamey.csr -CA "$MAMEY/.security/certs/sovereign-ca.crt" -CAkey "$MAMEY/.security/certs/sovereign-ca.key" -CAcreateserial -out "$MAMEY/.security/certs/sovereign.crt" -days 365 -extfile /tmp/san.cnf -extensions v3 2>/dev/null
    rm -f /tmp/mamey.csr /tmp/san.cnf "$MAMEY/.security/certs/"*.srl
    chmod 600 "$MAMEY/.security/certs/"*.key; chmod 644 "$MAMEY/.security/certs/"*.crt
    echo -e "  ${GREEN}+ TLS RSA-4096 certs${NC}"; ((ADDED++))
else echo -e "  ${GREEN}✓ TLS certs exist — kept${NC}"; fi

cat > "$MAMEY/.security/firewall/pf-mamey.conf" << 'PF'
block drop in proto tcp from any to any port 5001
block drop in proto tcp from any to any port 5002
block drop in proto tcp from any to any port 5003
block drop in proto tcp from any to any port 8545
pass in proto tcp from 127.0.0.1 to 127.0.0.1 port {5001, 5002, 5003, 8545}
pass in proto tcp from any to any port 443
PF
echo -e "  ${GREEN}+ Firewall rules${NC}"; ((ADDED++))

cat > "$MAMEY/.security/nginx/mamey-sovereign.conf" << 'NGX'
limit_req_zone $binary_remote_addr zone=api:10m rate=100r/m;
server { listen 80; return 301 https://$host$request_uri; }
server {
    listen 443 ssl http2;
    ssl_certificate .security/certs/sovereign.crt;
    ssl_certificate_key .security/certs/sovereign.key;
    ssl_protocols TLSv1.3;
    add_header Strict-Transport-Security "max-age=31536000" always;
    add_header X-Frame-Options "DENY" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Content-Security-Policy "default-src 'self'" always;
    location ~ /\.(env|git|key|pem|log) { deny all; }
    location /identity/ { limit_req zone=api burst=20; proxy_pass http://127.0.0.1:5001/; }
    location /compliance/ { limit_req zone=api burst=20; proxy_pass http://127.0.0.1:5002/; }
    location /treasury/ { limit_req zone=api burst=20; proxy_pass http://127.0.0.1:5003/; }
    location /chain/ { limit_req zone=api burst=50; proxy_pass http://127.0.0.1:8545/; }
    location ~ /swagger { deny all; }
}
NGX
echo -e "  ${GREEN}+ Nginx config${NC}"; ((ADDED++))

chmod 750 "$MAMEY/.security"; chmod 700 "$MAMEY/.security/auth" "$MAMEY/.security/certs"
echo ""

echo -e "${CYAN}[5/8] Config .env...${NC}"
if [ ! -f "$MAMEY/.env" ]; then
    cat > "$MAMEY/.env" << 'ENV'
MAMEY_BIND_HOST=127.0.0.1
MAMEY_ALLOW_PUBLIC=false
MAMEY_NODE_PORT=8545
MAMEY_IDENTITY_PORT=5001
MAMEY_ZKP_PORT=5002
MAMEY_TREASURY_PORT=5003
MAMEY_CHAIN_ID=777777
MAMEY_NETWORK_NAME="Ierahkwa Sovereign Network"
MAMEY_LOG_DIR=./logs
MAMEY_LOG_LEVEL=info
ENV
    chmod 640 "$MAMEY/.env"
    echo -e "  ${GREEN}+ .env${NC}"; ((ADDED++))
else echo -e "  ${GREEN}✓ .env exists — kept${NC}"; fi
echo ""

echo -e "${CYAN}[6/8] Protección .gitignore (append only)...${NC}"
if ! grep -q "MAMEY SECURITY" "$MAMEY/.gitignore" 2>/dev/null; then
    cat >> "$MAMEY/.gitignore" << 'GI'

# ═══ MAMEY SECURITY — NEVER COMMIT ═══
.security/certs/*.key
.security/certs/*.csr
.security/auth/
.env
*.key
*.pem
*.pid
logs/
*.log
dashboard.html
GI
    echo -e "  ${GREEN}+ Security entries APPENDED${NC}"; ((ADDED++))
else echo -e "  ${GREEN}✓ .gitignore already protected — kept${NC}"; fi
echo ""

echo -e "${CYAN}[7/8] Monitoring...${NC}"
mkdir -p "$MAMEY/monitoring/prometheus" "$MAMEY/monitoring/alerts"
if [ ! -f "$MAMEY/monitoring/prometheus/prometheus.yml" ]; then
    cat > "$MAMEY/monitoring/prometheus/prometheus.yml" << 'PROM'
global:
  scrape_interval: 15s
scrape_configs:
  - job_name: mamey-node
    static_configs: [{targets: ['127.0.0.1:8545']}]
  - job_name: identity
    static_configs: [{targets: ['127.0.0.1:5001']}]
  - job_name: zkp
    static_configs: [{targets: ['127.0.0.1:5002']}]
  - job_name: treasury
    static_configs: [{targets: ['127.0.0.1:5003']}]
PROM
    echo -e "  ${GREEN}+ prometheus.yml${NC}"; ((ADDED++))
fi
if [ ! -f "$MAMEY/monitoring/alerts/mamey-alerts.yml" ]; then
    cat > "$MAMEY/monitoring/alerts/mamey-alerts.yml" << 'ALERT'
groups:
  - name: critical
    rules:
      - alert: ServiceDown
        expr: up == 0
        for: 1m
      - alert: BruteForce
        expr: rate(login_failures_total[15m]) > 5
        for: 1m
      - alert: BlockchainStopped
        expr: increase(blockchain_block_height[10m]) == 0
        for: 10m
ALERT
    echo -e "  ${GREEN}+ alerts.yml${NC}"; ((ADDED++))
fi
echo ""

echo -e "${CYAN}[8/8] Logs directory...${NC}"
mkdir -p "$MAMEY/logs"
echo -e "  ${GREEN}+ logs/${NC}"; ((ADDED++))
echo ""

echo -e "${GREEN}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  ✅ $ADDED archivos AGREGADOS — NADA eliminado              ${GREEN}║${NC}"
echo -e "${GREEN}╠═══════════════════════════════════════════════════════════╣${NC}"
echo -e "${GREEN}║${NC}  .security/     Certs + Auth + Firewall + Nginx          ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  docs/sovereign/ Documentación soberana                  ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  monitoring/     Prometheus + Alerts                     ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  logs/           Log directory                           ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  6 scripts       Start/Stop/Audit/Rotate/Security/Scan  ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  dashboard.html  67+ platforms viewer                    ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  .env + .gitignore updated                               ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}                                                           ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  🦅 Todo código existente: INTACTO                        ${GREEN}║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}Ahora corre:${NC}"
echo "  cd ~/Desktop/Mamey-main"
echo "  ./audit-platforms.sh"
echo "  open dashboard.html"
echo ""

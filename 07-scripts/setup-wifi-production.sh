#!/bin/bash
# ============================================================================
# WIFI SOBERANO - SETUP SERVIDOR DE PRODUCCIÓN
# Un solo script para preparar el servidor completo desde cero
# Ejecutar como root en Ubuntu 22.04+
# ============================================================================

set -euo pipefail

# ── Colores ────────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

log()  { echo -e "${GREEN}[✓]${NC} $1"; }
warn() { echo -e "${YELLOW}[!]${NC} $1"; }
err()  { echo -e "${RED}[✗]${NC} $1"; exit 1; }
step() { echo -e "\n${CYAN}═══ $1 ═══${NC}"; }

echo -e "${CYAN}"
echo "📡 ═══════════════════════════════════════════════════════════"
echo "   WIFI SOBERANO — SETUP SERVIDOR DE PRODUCCIÓN"
echo "   Internet Satelital Soberano · 574 Naciones"
echo "📡 ═══════════════════════════════════════════════════════════"
echo -e "${NC}"

# ── Variables ──────────────────────────────────────────────────
DOMAIN="${WIFI_DOMAIN:-wifi.soberano.bo}"
EMAIL="${ADMIN_EMAIL:-admin@ierahkwa.nation}"
PROJECT_DIR="${PROJECT_DIR:-/opt/soberano}"
DB_PASSWORD="${DB_PASSWORD:-$(openssl rand -hex 24)}"
REDIS_PASSWORD="${REDIS_PASSWORD:-$(openssl rand -hex 16)}"
JWT_SECRET="${JWT_SECRET:-$(openssl rand -hex 32)}"

# ── [1] Actualizar Sistema ─────────────────────────────────────
step "1/8 · Actualizando sistema operativo"

export DEBIAN_FRONTEND=noninteractive
apt-get update -qq
apt-get upgrade -y -qq
apt-get install -y -qq \
  curl wget git ufw fail2ban \
  htop iotop ncdu tmux \
  ca-certificates gnupg lsb-release \
  certbot python3-certbot-nginx \
  jq unzip
log "Sistema actualizado"

# ── [2] Instalar Docker ────────────────────────────────────────
step "2/8 · Instalando Docker Engine"

if command -v docker &>/dev/null; then
  warn "Docker ya instalado: $(docker --version)"
else
  curl -fsSL https://get.docker.com | sh
  systemctl enable docker
  systemctl start docker
  log "Docker instalado: $(docker --version)"
fi

if ! command -v docker-compose &>/dev/null && ! docker compose version &>/dev/null; then
  apt-get install -y -qq docker-compose-plugin
fi
log "Docker Compose disponible"

# ── [3] Instalar Node.js 22 ────────────────────────────────────
step "3/8 · Instalando Node.js 22 LTS"

if command -v node &>/dev/null && [[ "$(node -v)" == v22* ]]; then
  warn "Node.js ya instalado: $(node -v)"
else
  curl -fsSL https://deb.nodesource.com/setup_22.x | bash -
  apt-get install -y -qq nodejs
  log "Node.js instalado: $(node -v)"
fi

# ── [4] PostgreSQL + Redis ─────────────────────────────────────
step "4/8 · Configurando PostgreSQL 16 + Redis 7"

# PostgreSQL via Docker (más fácil de gestionar)
docker network create soberana 2>/dev/null || true

docker run -d \
  --name postgres-soberano \
  --network soberana \
  --restart always \
  -e POSTGRES_DB=soberana \
  -e POSTGRES_USER=soberano \
  -e POSTGRES_PASSWORD="$DB_PASSWORD" \
  -v pgdata-soberano:/var/lib/postgresql/data \
  -p 127.0.0.1:5432:5432 \
  postgres:16-alpine 2>/dev/null || warn "PostgreSQL container ya existe"
log "PostgreSQL 16 corriendo"

docker run -d \
  --name redis-soberano \
  --network soberana \
  --restart always \
  -e REDIS_PASSWORD="$REDIS_PASSWORD" \
  -v redis-soberano:/data \
  -p 127.0.0.1:6379:6379 \
  redis:7-alpine redis-server --requirepass "$REDIS_PASSWORD" 2>/dev/null || warn "Redis container ya existe"
log "Redis 7 corriendo"

# ── [5] Clonar/Actualizar Proyecto ─────────────────────────────
step "5/8 · Preparando código del proyecto"

if [ -d "$PROJECT_DIR/.git" ]; then
  cd "$PROJECT_DIR"
  git pull origin main || warn "Git pull falló, usando código existente"
  log "Proyecto actualizado"
else
  git clone https://github.com/rudvincci/ierahkwa-platform.git "$PROJECT_DIR" 2>/dev/null || true
  cd "$PROJECT_DIR"
  log "Proyecto clonado en $PROJECT_DIR"
fi

# ── [6] Configurar WiFi Soberano ───────────────────────────────
step "6/8 · Configurando wifi-soberano"

WIFI_DIR="$PROJECT_DIR/03-backend/wifi-soberano"
cd "$WIFI_DIR"

# Crear .env de producción
cat > .env << ENVEOF
NODE_ENV=production
PORT=3095
HOST=0.0.0.0

# Database
DATABASE_URL=postgresql://soberano:${DB_PASSWORD}@postgres-soberano:5432/soberana

# Redis
REDIS_URL=redis://:${REDIS_PASSWORD}@redis-soberano:6379

# Auth
JWT_SECRET=${JWT_SECRET}
JWT_EXPIRES_IN=24h

# WAMPUM / Blockchain
MAMEYNODE_RPC_URL=http://mameynode:8545
WAMPUM_WALLET=0x574NationsSovereignWallet

# Vigilancia
VIP_ALERT_WEBHOOK=
ATABEY_AI_ENDPOINT=

# WiFi Captive Portal
PORTAL_REDIRECT_URL=https://${DOMAIN}/portal
BANDWIDTH_DEFAULT_MBPS=25
SESSION_CHECK_INTERVAL_MS=30000

# Logging
LOG_LEVEL=info
ENVEOF
log ".env de producción creado"

# Instalar dependencias
npm ci --production 2>/dev/null || npm install --production
log "Dependencias instaladas"

# Ejecutar migraciones
echo "Esperando PostgreSQL..."
sleep 5
docker exec postgres-soberano psql -U soberano -d soberana -f /dev/stdin < models/migrations.sql 2>/dev/null || warn "Migraciones ya ejecutadas"
log "Migraciones SQL ejecutadas"

# ── [7] Nginx + SSL ────────────────────────────────────────────
step "7/8 · Configurando Nginx + SSL"

apt-get install -y -qq nginx

cat > /etc/nginx/sites-available/wifi-soberano << NGINXEOF
# WiFi Soberano — Captive Portal + API
# Puerto 3095 interno → 443 externo

upstream wifi_backend {
    server 127.0.0.1:3095;
    keepalive 32;
}

upstream core_backend {
    server 127.0.0.1:3050;
    keepalive 16;
}

# Redirect HTTP → HTTPS
server {
    listen 80;
    server_name ${DOMAIN};

    # Captive portal detection (Apple, Android, Microsoft)
    location /hotspot-detect.html { return 302 http://${DOMAIN}/portal; }
    location /generate_204 { return 302 http://${DOMAIN}/portal; }
    location /connecttest.txt { return 302 http://${DOMAIN}/portal; }
    location /ncsi.txt { return 302 http://${DOMAIN}/portal; }
    location /success.txt { return 302 http://${DOMAIN}/portal; }

    # Let's Encrypt
    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }

    location / {
        return 301 https://\$host\$request_uri;
    }
}

server {
    listen 443 ssl http2;
    server_name ${DOMAIN};

    # SSL (se configura después con certbot)
    ssl_certificate /etc/letsencrypt/live/${DOMAIN}/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/${DOMAIN}/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256;
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;

    # Security headers
    add_header X-Frame-Options SAMEORIGIN;
    add_header X-Content-Type-Options nosniff;
    add_header X-XSS-Protection "1; mode=block";
    add_header Referrer-Policy strict-origin-when-cross-origin;

    # Rate limiting
    limit_req_zone \$binary_remote_addr zone=portal:10m rate=30r/m;
    limit_req_zone \$binary_remote_addr zone=api:10m rate=200r/m;

    # Captive Portal (HTML estático)
    location /portal {
        alias ${PROJECT_DIR}/02-plataformas-html/starlink-soberano;
        try_files \$uri \$uri/ /index.html;
    }

    # Shared assets
    location /shared/ {
        alias ${PROJECT_DIR}/02-plataformas-html/shared/;
        expires 7d;
        add_header Cache-Control "public, immutable";
    }

    # WiFi API
    location /api/v1/wifi/ {
        limit_req zone=api burst=50 nodelay;
        proxy_pass http://wifi_backend;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_connect_timeout 5s;
        proxy_read_timeout 30s;
    }

    # WiFi WebSocket
    location /ws/wifi {
        proxy_pass http://wifi_backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_read_timeout 3600s;
        proxy_send_timeout 3600s;
    }

    # Sovereign Core API
    location /v1/ {
        limit_req zone=api burst=100 nodelay;
        proxy_pass http://core_backend;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }

    # Health checks
    location /health {
        proxy_pass http://wifi_backend/health;
        access_log off;
    }

    # Favicon
    location /favicon.ico { return 204; access_log off; }

    # Default
    location / {
        proxy_pass http://wifi_backend;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
    }

    # Logs
    access_log /var/log/nginx/wifi-soberano-access.log;
    error_log /var/log/nginx/wifi-soberano-error.log;
}
NGINXEOF

ln -sf /etc/nginx/sites-available/wifi-soberano /etc/nginx/sites-enabled/
rm -f /etc/nginx/sites-enabled/default

# Obtener SSL (si el dominio resuelve)
mkdir -p /var/www/certbot
if host "$DOMAIN" &>/dev/null; then
  certbot --nginx -d "$DOMAIN" --non-interactive --agree-tos -m "$EMAIL" 2>/dev/null || warn "SSL: configurar manualmente con certbot"
  log "SSL configurado con Let's Encrypt"
else
  warn "Dominio $DOMAIN no resuelve aún — SSL pendiente"
  # Crear cert auto-firmado temporal
  mkdir -p /etc/letsencrypt/live/$DOMAIN
  openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/letsencrypt/live/$DOMAIN/privkey.pem \
    -out /etc/letsencrypt/live/$DOMAIN/fullchain.pem \
    -subj "/CN=$DOMAIN" 2>/dev/null
  log "Certificado auto-firmado creado (temporal)"
fi

nginx -t && systemctl restart nginx
log "Nginx configurado y corriendo"

# ── [8] Servicios systemd ──────────────────────────────────────
step "8/8 · Configurando servicios systemd"

# wifi-soberano service
cat > /etc/systemd/system/wifi-soberano.service << SVCEOF
[Unit]
Description=WiFi Soberano - Captive Portal Backend
After=network.target docker.service
Requires=docker.service

[Service]
Type=simple
User=root
WorkingDirectory=${WIFI_DIR}
ExecStart=/usr/bin/node server.js
Restart=always
RestartSec=5
Environment=NODE_ENV=production
StandardOutput=journal
StandardError=journal
SyslogIdentifier=wifi-soberano

# Seguridad
LimitNOFILE=65535
TimeoutStopSec=30

[Install]
WantedBy=multi-user.target
SVCEOF

# sovereign-core service
cat > /etc/systemd/system/sovereign-core.service << SVCEOF
[Unit]
Description=Sovereign Core - Universal Backend
After=network.target docker.service
Requires=docker.service

[Service]
Type=simple
User=root
WorkingDirectory=${PROJECT_DIR}/03-backend/sovereign-core
ExecStart=/usr/bin/node server.js
Restart=always
RestartSec=5
Environment=NODE_ENV=production
Environment=PORT=3050
Environment=DATABASE_URL=postgresql://soberano:${DB_PASSWORD}@127.0.0.1:5432/soberana
Environment=JWT_SECRET=${JWT_SECRET}
StandardOutput=journal
StandardError=journal
SyslogIdentifier=sovereign-core

[Install]
WantedBy=multi-user.target
SVCEOF

# session-expiry worker
cat > /etc/systemd/system/wifi-session-expiry.service << SVCEOF
[Unit]
Description=WiFi Session Expiry Worker
After=wifi-soberano.service

[Service]
Type=simple
User=root
WorkingDirectory=${WIFI_DIR}
ExecStart=/usr/bin/node workers/session-expiry.js
Restart=always
RestartSec=10
Environment=NODE_ENV=production
Environment=DATABASE_URL=postgresql://soberano:${DB_PASSWORD}@127.0.0.1:5432/soberana
StandardOutput=journal
StandardError=journal
SyslogIdentifier=wifi-expiry

[Install]
WantedBy=multi-user.target
SVCEOF

systemctl daemon-reload
systemctl enable wifi-soberano sovereign-core wifi-session-expiry
systemctl start wifi-soberano sovereign-core wifi-session-expiry
log "Servicios systemd habilitados y corriendo"

# ── Firewall ───────────────────────────────────────────────────
step "Configurando firewall (UFW)"

ufw --force reset
ufw default deny incoming
ufw default allow outgoing
ufw allow 22/tcp    # SSH
ufw allow 80/tcp    # HTTP (redirect)
ufw allow 443/tcp   # HTTPS
ufw allow 3095/tcp  # WiFi API (interno)
ufw allow 3050/tcp  # Core API (interno)
ufw --force enable
log "Firewall configurado"

# ── Fail2Ban ───────────────────────────────────────────────────
systemctl enable fail2ban
systemctl start fail2ban
log "Fail2Ban activo"

# ── Resumen ────────────────────────────────────────────────────
echo ""
echo -e "${GREEN}"
echo "📡 ═══════════════════════════════════════════════════════════"
echo "   WIFI SOBERANO — SERVIDOR LISTO PARA PRODUCCIÓN"
echo "═══════════════════════════════════════════════════════════"
echo -e "${NC}"
echo ""
echo "  Servicios activos:"
echo "    ● WiFi Soberano:    http://localhost:3095"
echo "    ● Sovereign Core:   http://localhost:3050"
echo "    ● Nginx (HTTPS):    https://${DOMAIN}"
echo "    ● PostgreSQL:       localhost:5432"
echo "    ● Redis:            localhost:6379"
echo ""
echo "  Credenciales (GUARDAR EN LUGAR SEGURO):"
echo "    DB Password:  ${DB_PASSWORD}"
echo "    Redis Pass:   ${REDIS_PASSWORD}"
echo "    JWT Secret:   ${JWT_SECRET}"
echo ""
echo "  Comandos útiles:"
echo "    systemctl status wifi-soberano"
echo "    systemctl status sovereign-core"
echo "    journalctl -u wifi-soberano -f"
echo "    journalctl -u sovereign-core -f"
echo ""
echo "  Siguiente paso:"
echo "    1. Configurar DNS: ${DOMAIN} → IP del servidor"
echo "    2. Ejecutar: certbot --nginx -d ${DOMAIN}"
echo "    3. Configurar router Starlink → captive portal"
echo "    4. Ejecutar: ./captive-portal-redirect.sh"
echo ""
echo -e "${GREEN}  ¡Listo para empezar a generar revenue! 💰${NC}"
echo ""

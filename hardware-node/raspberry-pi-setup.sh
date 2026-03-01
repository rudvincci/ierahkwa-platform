#!/usr/bin/env bash
# ============================================================================
# IERAHKWA NODE -- Raspberry Pi 5 Setup Script
# Sets up a complete Ierahkwa sovereign node on Raspberry Pi OS (64-bit).
#
# Components installed:
#   - Docker CE + Docker Compose
#   - Node.js 22 LTS (via NodeSource)
#   - PostgreSQL 16 client
#   - Ierahkwa Platform repository
#   - LoRa/Meshtastic serial interface
#   - UFW firewall rules
#   - Systemd service for auto-start
#
# Usage:
#   chmod +x raspberry-pi-setup.sh
#   sudo ./raspberry-pi-setup.sh
#
# Tested on: Raspberry Pi 5 (8GB), Raspberry Pi OS Bookworm 64-bit
# ============================================================================

set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

IERAHKWA_USER="${IERAHKWA_USER:-ierahkwa}"
IERAHKWA_HOME="/home/${IERAHKWA_USER}"
IERAHKWA_DIR="${IERAHKWA_HOME}/ierahkwa-platform"
IERAHKWA_REPO="https://github.com/rudvincci/ierahkwa-platform.git"
IERAHKWA_BRANCH="main"
NODE_VERSION="22"
POSTGRES_VERSION="16"
SWAP_SIZE_MB=2048
HEALTH_CHECK_PORT=3000
MESHTASTIC_DEVICE="/dev/ttyUSB0"
MESHTASTIC_BAUD=115200

# ---------------------------------------------------------------------------
# Colors for output
# ---------------------------------------------------------------------------

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

log_info()  { echo -e "${CYAN}[INFO]${NC}  $1"; }
log_ok()    { echo -e "${GREEN}[OK]${NC}    $1"; }
log_warn()  { echo -e "${YELLOW}[WARN]${NC}  $1"; }
log_error() { echo -e "${RED}[ERROR]${NC} $1"; }

# ---------------------------------------------------------------------------
# Pre-flight checks
# ---------------------------------------------------------------------------

if [[ $EUID -ne 0 ]]; then
    log_error "This script must be run as root (use sudo)."
    exit 1
fi

if ! grep -q "Raspberry Pi" /proc/device-tree/model 2>/dev/null; then
    log_warn "This does not appear to be a Raspberry Pi. Proceeding anyway."
fi

log_info "Starting Ierahkwa Node setup on $(hostname)..."
log_info "Date: $(date -u '+%Y-%m-%d %H:%M:%S UTC')"

# ---------------------------------------------------------------------------
# Step 1: System update
# ---------------------------------------------------------------------------

log_info "Updating system packages..."
apt-get update -qq
apt-get upgrade -y -qq
apt-get install -y -qq \
    curl \
    wget \
    git \
    build-essential \
    ca-certificates \
    gnupg \
    lsb-release \
    software-properties-common \
    jq \
    htop \
    screen \
    ufw \
    openssl \
    python3 \
    python3-pip \
    python3-serial \
    libffi-dev \
    libssl-dev
log_ok "System packages updated."

# ---------------------------------------------------------------------------
# Step 2: Create dedicated user
# ---------------------------------------------------------------------------

if ! id "${IERAHKWA_USER}" &>/dev/null; then
    log_info "Creating user '${IERAHKWA_USER}'..."
    useradd -m -s /bin/bash "${IERAHKWA_USER}"
    usermod -aG sudo "${IERAHKWA_USER}"
    log_ok "User '${IERAHKWA_USER}' created."
else
    log_ok "User '${IERAHKWA_USER}' already exists."
fi

# ---------------------------------------------------------------------------
# Step 3: Configure swap
# ---------------------------------------------------------------------------

log_info "Configuring ${SWAP_SIZE_MB}MB swap..."
if [ -f /swapfile ]; then
    swapoff /swapfile 2>/dev/null || true
    rm -f /swapfile
fi
dd if=/dev/zero of=/swapfile bs=1M count=${SWAP_SIZE_MB} status=none
chmod 600 /swapfile
mkswap /swapfile >/dev/null
swapon /swapfile

# Ensure swap persists across reboots
if ! grep -q "/swapfile" /etc/fstab; then
    echo "/swapfile none swap sw 0 0" >> /etc/fstab
fi

# Optimize swap behavior for Raspberry Pi
sysctl -w vm.swappiness=10 >/dev/null
if ! grep -q "vm.swappiness" /etc/sysctl.conf; then
    echo "vm.swappiness=10" >> /etc/sysctl.conf
fi
log_ok "Swap configured (${SWAP_SIZE_MB}MB)."

# ---------------------------------------------------------------------------
# Step 4: Memory limits (cgroup v2)
# ---------------------------------------------------------------------------

log_info "Configuring memory limits..."
# Limit total memory usage for node services to 6GB (leave 2GB for OS on 8GB Pi)
if ! grep -q "cgroup_memory=1" /boot/firmware/cmdline.txt 2>/dev/null; then
    sed -i 's/$/ cgroup_memory=1 cgroup_enable=memory/' /boot/firmware/cmdline.txt 2>/dev/null || true
    log_warn "cgroup memory enabled -- reboot required for full effect."
fi
log_ok "Memory limits configured."

# ---------------------------------------------------------------------------
# Step 5: Install Docker
# ---------------------------------------------------------------------------

log_info "Installing Docker CE..."
if ! command -v docker &>/dev/null; then
    curl -fsSL https://get.docker.com | sh
    usermod -aG docker "${IERAHKWA_USER}"

    # Configure Docker daemon for Raspberry Pi resource constraints
    mkdir -p /etc/docker
    cat > /etc/docker/daemon.json <<'DOCKER_CONF'
{
    "log-driver": "json-file",
    "log-opts": {
        "max-size": "10m",
        "max-file": "3"
    },
    "storage-driver": "overlay2",
    "default-ulimits": {
        "nofile": {
            "Name": "nofile",
            "Hard": 65536,
            "Soft": 65536
        }
    },
    "default-shm-size": "128M"
}
DOCKER_CONF

    systemctl enable docker
    systemctl restart docker
    log_ok "Docker CE installed and configured."
else
    log_ok "Docker already installed."
fi

# ---------------------------------------------------------------------------
# Step 6: Install Node.js 22
# ---------------------------------------------------------------------------

log_info "Installing Node.js ${NODE_VERSION}..."
if ! command -v node &>/dev/null || ! node -v | grep -q "v${NODE_VERSION}"; then
    curl -fsSL https://deb.nodesource.com/setup_${NODE_VERSION}.x | bash -
    apt-get install -y -qq nodejs
    log_ok "Node.js $(node -v) installed."
else
    log_ok "Node.js $(node -v) already installed."
fi

# Install global npm packages
npm install -g pm2 --silent 2>/dev/null
log_ok "PM2 process manager installed."

# ---------------------------------------------------------------------------
# Step 7: Install PostgreSQL client
# ---------------------------------------------------------------------------

log_info "Installing PostgreSQL ${POSTGRES_VERSION} client..."
if ! command -v psql &>/dev/null; then
    apt-get install -y -qq postgresql-client-${POSTGRES_VERSION} 2>/dev/null || \
    apt-get install -y -qq postgresql-client
    log_ok "PostgreSQL client installed."
else
    log_ok "PostgreSQL client already installed."
fi

# ---------------------------------------------------------------------------
# Step 8: Clone Ierahkwa Platform
# ---------------------------------------------------------------------------

log_info "Cloning Ierahkwa Platform repository..."
if [ ! -d "${IERAHKWA_DIR}" ]; then
    sudo -u "${IERAHKWA_USER}" git clone \
        --depth 1 \
        --branch "${IERAHKWA_BRANCH}" \
        "${IERAHKWA_REPO}" \
        "${IERAHKWA_DIR}"
    log_ok "Repository cloned to ${IERAHKWA_DIR}."
else
    log_ok "Repository already exists at ${IERAHKWA_DIR}."
    log_info "Pulling latest changes..."
    sudo -u "${IERAHKWA_USER}" bash -c "cd ${IERAHKWA_DIR} && GIT_NO_OPTIONAL_LOCKS=1 git pull origin ${IERAHKWA_BRANCH}"
fi

# Configure git for the ierahkwa user
sudo -u "${IERAHKWA_USER}" git config --global pack.threads 1
sudo -u "${IERAHKWA_USER}" git config --global pack.windowMemory "256m"
sudo -u "${IERAHKWA_USER}" git config --global pack.packSizeLimit "128m"

# ---------------------------------------------------------------------------
# Step 9: Generate .env from template
# ---------------------------------------------------------------------------

log_info "Generating environment configuration..."
ENV_FILE="${IERAHKWA_DIR}/.env"
ENV_EXAMPLE="${IERAHKWA_DIR}/.env.example"

if [ -f "${ENV_EXAMPLE}" ] && [ ! -f "${ENV_FILE}" ]; then
    cp "${ENV_EXAMPLE}" "${ENV_FILE}"

    # Generate cryptographically secure secrets
    DB_PASSWORD=$(openssl rand -hex 32)
    JWT_SECRET=$(openssl rand -hex 64)
    API_KEY=$(openssl rand -hex 32)
    NODE_ID=$(openssl rand -hex 16)
    SESSION_SECRET=$(openssl rand -hex 48)

    # Replace placeholder values in .env
    sed -i "s|DB_PASSWORD=.*|DB_PASSWORD=${DB_PASSWORD}|g" "${ENV_FILE}"
    sed -i "s|JWT_SECRET=.*|JWT_SECRET=${JWT_SECRET}|g" "${ENV_FILE}"
    sed -i "s|API_KEY=.*|API_KEY=${API_KEY}|g" "${ENV_FILE}"
    sed -i "s|NODE_ID=.*|NODE_ID=${NODE_ID}|g" "${ENV_FILE}"
    sed -i "s|SESSION_SECRET=.*|SESSION_SECRET=${SESSION_SECRET}|g" "${ENV_FILE}"

    # Set node-specific values
    sed -i "s|NODE_ENV=.*|NODE_ENV=production|g" "${ENV_FILE}"
    sed -i "s|PORT=.*|PORT=${HEALTH_CHECK_PORT}|g" "${ENV_FILE}"

    # Lock down permissions
    chown "${IERAHKWA_USER}:${IERAHKWA_USER}" "${ENV_FILE}"
    chmod 600 "${ENV_FILE}"
    log_ok "Environment file generated with auto-generated secrets."
elif [ -f "${ENV_FILE}" ]; then
    log_ok "Environment file already exists (not overwritten)."
else
    log_warn "No .env.example found. Skipping .env generation."
fi

# ---------------------------------------------------------------------------
# Step 10: Configure UFW firewall
# ---------------------------------------------------------------------------

log_info "Configuring UFW firewall..."

# Reset to defaults
ufw --force reset >/dev/null 2>&1

# Default policies: deny incoming, allow outgoing
ufw default deny incoming >/dev/null
ufw default allow outgoing >/dev/null

# SSH access
ufw allow 22/tcp comment "SSH" >/dev/null

# Ierahkwa node API
ufw allow ${HEALTH_CHECK_PORT}/tcp comment "Ierahkwa Node API" >/dev/null

# Meshtastic / LoRa gateway (MQTT)
ufw allow 1883/tcp comment "MQTT (Meshtastic)" >/dev/null

# Peer-to-peer node discovery
ufw allow 4001/tcp comment "IPFS Swarm" >/dev/null
ufw allow 8080/tcp comment "IPFS Gateway" >/dev/null

# MameyNode blockchain P2P
ufw allow 30303/tcp comment "MameyNode P2P" >/dev/null
ufw allow 30303/udp comment "MameyNode P2P Discovery" >/dev/null

# Enable firewall
ufw --force enable >/dev/null
log_ok "UFW firewall configured and enabled."

# ---------------------------------------------------------------------------
# Step 11: LoRa / Meshtastic serial interface
# ---------------------------------------------------------------------------

log_info "Configuring LoRa/Meshtastic serial interface..."

# Add user to dialout group for serial access
usermod -aG dialout "${IERAHKWA_USER}"

# Create udev rule for consistent device naming
cat > /etc/udev/rules.d/99-meshtastic.rules <<'UDEV_RULE'
# Meshtastic LoRa devices -- create consistent symlink
# LilyGo T-Beam (CP2104)
SUBSYSTEM=="tty", ATTRS{idVendor}=="10c4", ATTRS{idProduct}=="ea60", SYMLINK+="meshtastic", MODE="0666"
# Heltec LoRa 32 (CH9102)
SUBSYSTEM=="tty", ATTRS{idVendor}=="1a86", ATTRS{idProduct}=="55d4", SYMLINK+="meshtastic", MODE="0666"
# RAK WisBlock (various)
SUBSYSTEM=="tty", ATTRS{idVendor}=="239a", SYMLINK+="meshtastic", MODE="0666"
UDEV_RULE

udevadm control --reload-rules
udevadm trigger

# Install Meshtastic Python CLI
pip3 install --quiet meshtastic 2>/dev/null || true

# Create Meshtastic configuration script
cat > "${IERAHKWA_HOME}/configure-meshtastic.sh" <<MESH_SCRIPT
#!/usr/bin/env bash
# Configure Meshtastic device for Ierahkwa mesh network
# Run this after connecting a LoRa device via USB

DEVICE="\${1:-/dev/meshtastic}"

if [ ! -e "\${DEVICE}" ]; then
    echo "No Meshtastic device found at \${DEVICE}"
    echo "Available serial devices:"
    ls -la /dev/ttyUSB* /dev/ttyACM* /dev/meshtastic 2>/dev/null || echo "  None found"
    exit 1
fi

echo "Configuring Meshtastic on \${DEVICE}..."

# Set region to Americas (US/CA/MX)
meshtastic --port "\${DEVICE}" --set lora.region US

# Set long-range mode for rural coverage
meshtastic --port "\${DEVICE}" --set lora.modem_preset LONG_SLOW

# Set channel for Ierahkwa mesh
meshtastic --port "\${DEVICE}" --ch-set name "IERAHKWA" --ch-index 0
meshtastic --port "\${DEVICE}" --ch-set psk "random" --ch-index 0

# Enable MQTT uplink for internet-connected nodes
meshtastic --port "\${DEVICE}" --set mqtt.enabled true
meshtastic --port "\${DEVICE}" --set mqtt.address "localhost"
meshtastic --port "\${DEVICE}" --set mqtt.username "ierahkwa"

# Set node name
meshtastic --port "\${DEVICE}" --set-owner "IRHK-\$(hostname)"

echo "Meshtastic configuration complete."
MESH_SCRIPT

chmod +x "${IERAHKWA_HOME}/configure-meshtastic.sh"
chown "${IERAHKWA_USER}:${IERAHKWA_USER}" "${IERAHKWA_HOME}/configure-meshtastic.sh"

log_ok "LoRa/Meshtastic serial interface configured."

# ---------------------------------------------------------------------------
# Step 12: Systemd service for auto-start
# ---------------------------------------------------------------------------

log_info "Creating systemd service..."

cat > /etc/systemd/system/ierahkwa-node.service <<SERVICE_UNIT
[Unit]
Description=Ierahkwa Sovereign Node
Documentation=https://github.com/rudvincci/ierahkwa-platform
After=network-online.target docker.service
Wants=network-online.target
Requires=docker.service

[Service]
Type=forking
User=${IERAHKWA_USER}
Group=${IERAHKWA_USER}
WorkingDirectory=${IERAHKWA_DIR}
Environment=NODE_ENV=production
Environment=GIT_NO_OPTIONAL_LOCKS=1
ExecStartPre=/usr/bin/docker compose pull --quiet
ExecStart=/usr/local/bin/pm2 start ecosystem.config.js --env production
ExecStop=/usr/local/lib/nodejs/bin/pm2 stop all
ExecReload=/usr/local/lib/nodejs/bin/pm2 reload all
PIDFile=${IERAHKWA_HOME}/.pm2/pm2.pid
Restart=on-failure
RestartSec=10
LimitNOFILE=65536

# Resource limits for Raspberry Pi 5
MemoryMax=6G
CPUQuota=350%

# Security hardening
ProtectSystem=full
ProtectHome=read-only
ReadWritePaths=${IERAHKWA_DIR} ${IERAHKWA_HOME}/.pm2
NoNewPrivileges=true
PrivateTmp=true

[Install]
WantedBy=multi-user.target
SERVICE_UNIT

systemctl daemon-reload
systemctl enable ierahkwa-node.service
log_ok "Systemd service created and enabled."

# ---------------------------------------------------------------------------
# Step 13: Health check script
# ---------------------------------------------------------------------------

log_info "Installing health check script..."

cat > "${IERAHKWA_HOME}/health-check.sh" <<'HEALTH_SCRIPT'
#!/usr/bin/env bash
# Ierahkwa Node Health Check
# Returns 0 if healthy, 1 if unhealthy

ENDPOINT="http://localhost:3000/health"
TIMEOUT=10

response=$(curl -sf --max-time ${TIMEOUT} "${ENDPOINT}" 2>/dev/null)

if [ $? -eq 0 ]; then
    echo "[HEALTHY] Node is running."
    echo "  Response: ${response}"

    # Check Docker containers
    running=$(docker ps --format '{{.Names}}' 2>/dev/null | wc -l)
    echo "  Docker containers running: ${running}"

    # Check disk usage
    disk_usage=$(df -h / | awk 'NR==2{print $5}')
    echo "  Disk usage: ${disk_usage}"

    # Check memory
    mem_info=$(free -h | awk 'NR==2{printf "Used: %s / Total: %s (%.1f%%)", $3, $2, $3/$2*100}')
    echo "  Memory: ${mem_info}"

    # Check swap
    swap_info=$(free -h | awk 'NR==3{printf "Used: %s / Total: %s", $3, $2}')
    echo "  Swap: ${swap_info}"

    exit 0
else
    echo "[UNHEALTHY] Node is not responding at ${ENDPOINT}."
    echo "  Checking services..."

    # Check if PM2 is running
    if pm2 list 2>/dev/null | grep -q "online"; then
        echo "  PM2: running"
    else
        echo "  PM2: not running"
    fi

    # Check Docker
    if systemctl is-active --quiet docker; then
        echo "  Docker: running"
    else
        echo "  Docker: stopped"
    fi

    exit 1
fi
HEALTH_SCRIPT

chmod +x "${IERAHKWA_HOME}/health-check.sh"
chown "${IERAHKWA_USER}:${IERAHKWA_USER}" "${IERAHKWA_HOME}/health-check.sh"

# Add health check to crontab (every 5 minutes)
crontab -u "${IERAHKWA_USER}" -l 2>/dev/null | grep -v "health-check" > /tmp/crontab.tmp || true
echo "*/5 * * * * ${IERAHKWA_HOME}/health-check.sh >> ${IERAHKWA_HOME}/health-check.log 2>&1" >> /tmp/crontab.tmp
crontab -u "${IERAHKWA_USER}" /tmp/crontab.tmp
rm -f /tmp/crontab.tmp

log_ok "Health check installed (runs every 5 minutes)."

# ---------------------------------------------------------------------------
# Step 14: Install Node.js dependencies
# ---------------------------------------------------------------------------

log_info "Installing Node.js dependencies..."
if [ -f "${IERAHKWA_DIR}/package.json" ]; then
    sudo -u "${IERAHKWA_USER}" bash -c "cd ${IERAHKWA_DIR} && npm ci --production --silent 2>/dev/null" || \
    sudo -u "${IERAHKWA_USER}" bash -c "cd ${IERAHKWA_DIR} && npm install --production --silent 2>/dev/null" || true
    log_ok "Node.js dependencies installed."
else
    log_warn "No package.json found. Skipping npm install."
fi

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------

echo ""
echo "============================================================================"
echo "  IERAHKWA SOVEREIGN NODE -- Setup Complete"
echo "============================================================================"
echo ""
echo "  Node User:        ${IERAHKWA_USER}"
echo "  Install Dir:      ${IERAHKWA_DIR}"
echo "  Health Check:     http://localhost:${HEALTH_CHECK_PORT}/health"
echo "  Firewall:         UFW enabled (SSH, API, MQTT, IPFS, MameyNode)"
echo "  Auto-Start:       systemctl start ierahkwa-node"
echo "  Health Cron:      Every 5 minutes (${IERAHKWA_HOME}/health-check.log)"
echo "  Meshtastic:       Run ~/configure-meshtastic.sh after connecting LoRa device"
echo ""
echo "  Next steps:"
echo "    1. Reboot:           sudo reboot"
echo "    2. Start node:       sudo systemctl start ierahkwa-node"
echo "    3. Check health:     ~/health-check.sh"
echo "    4. Configure LoRa:   ~/configure-meshtastic.sh"
echo "    5. View logs:        pm2 logs"
echo ""
echo "  Ierahkwa Ne Kanienke -- Sovereign Digital Nation"
echo "============================================================================"

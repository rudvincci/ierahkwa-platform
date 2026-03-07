#!/bin/bash
# ============================================================================
# WIFI SOBERANO - CAPTIVE PORTAL REDIRECT (iptables/nftables)
# Redirige TODO el tráfico HTTP de usuarios WiFi al portal cautivo
# Ejecutar como root en el router/gateway
# ============================================================================

set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

log()  { echo -e "${GREEN}[✓]${NC} $1"; }
warn() { echo -e "${YELLOW}[!]${NC} $1"; }
err()  { echo -e "${RED}[✗]${NC} $1"; exit 1; }

echo -e "${CYAN}"
echo "📡 ═══════════════════════════════════════════════════════════"
echo "   WIFI SOBERANO — CAPTIVE PORTAL REDIRECT"
echo "   Configuración de iptables para portal cautivo"
echo "📡 ═══════════════════════════════════════════════════════════"
echo -e "${NC}"

# ── Variables ──────────────────────────────────────────────────
PORTAL_IP="${PORTAL_IP:-192.168.1.1}"          # IP del servidor con el portal
PORTAL_PORT="${PORTAL_PORT:-80}"                # Puerto HTTP del portal
WIFI_INTERFACE="${WIFI_INTERFACE:-wlan0}"        # Interfaz WiFi
LAN_SUBNET="${LAN_SUBNET:-192.168.1.0/24}"      # Subred WiFi
DNS_SERVER="${DNS_SERVER:-8.8.8.8}"              # DNS permitido
DOMAIN="${WIFI_DOMAIN:-wifi.soberano.bo}"        # Dominio del portal
MARK_AUTH="0x1"                                  # Mark para usuarios autenticados

# IPs que siempre tienen acceso (servidores internos)
WHITELIST_IPS=(
  "$PORTAL_IP"                # Servidor portal
  "192.168.1.2"               # Servidor DNS local (si existe)
)

# Puertos siempre permitidos (DNS, DHCP)
ALLOWED_PORTS="53 67 68"

# ── Detectar backend de firewall ──────────────────────────────
USE_NFT=false
if command -v nft &>/dev/null; then
  USE_NFT=true
  log "Usando nftables"
else
  if ! command -v iptables &>/dev/null; then
    err "Necesitas iptables o nftables instalado"
  fi
  log "Usando iptables"
fi

# ── Función: Limpiar reglas anteriores ────────────────────────
clean_rules() {
  if $USE_NFT; then
    nft delete table inet soberano 2>/dev/null || true
    log "Reglas nftables anteriores limpiadas"
  else
    # Limpiar chains custom
    iptables -t nat -F SOBERANO_PORTAL 2>/dev/null || true
    iptables -t nat -X SOBERANO_PORTAL 2>/dev/null || true
    iptables -t mangle -F SOBERANO_AUTH 2>/dev/null || true
    iptables -t mangle -X SOBERANO_AUTH 2>/dev/null || true
    iptables -t filter -F SOBERANO_FWD 2>/dev/null || true
    iptables -t filter -X SOBERANO_FWD 2>/dev/null || true
    log "Reglas iptables anteriores limpiadas"
  fi
}

# ── Función: Configurar con nftables ─────────────────────────
setup_nftables() {
  nft -f - << 'NFTEOF'
table inet soberano {
  # Set de MACs autenticadas (se agregan dinámicamente)
  set authenticated {
    type ether_addr
    timeout 24h
    flags dynamic
  }

  # Set de IPs autenticadas
  set auth_ips {
    type ipv4_addr
    timeout 24h
    flags dynamic
  }

  chain prerouting {
    type nat hook prerouting priority -100; policy accept;

    # DNS siempre permitido
    udp dport 53 accept

    # Si está autenticado, no redirigir
    ether saddr @authenticated accept

    # Redirigir HTTP al portal cautivo
    tcp dport 80 redirect to :PORTAL_PORT_PLACEHOLDER

    # Redirigir HTTPS al portal (para captive portal detection)
    tcp dport 443 redirect to :PORTAL_PORT_PLACEHOLDER
  }

  chain forward {
    type filter hook forward priority 0; policy drop;

    # Tráfico establecido/relacionado
    ct state established,related accept

    # DNS siempre
    udp dport 53 accept
    tcp dport 53 accept

    # DHCP
    udp dport { 67, 68 } accept

    # Autenticados pueden navegar
    ether saddr @authenticated accept

    # No autenticados solo pueden llegar al portal
    ip daddr PORTAL_IP_PLACEHOLDER accept

    # Todo lo demás: drop (redirigido al portal)
    counter drop
  }

  chain postrouting {
    type nat hook postrouting priority 100; policy accept;

    # NAT para salida a internet
    oifname != "WIFI_IF_PLACEHOLDER" masquerade
  }
}
NFTEOF

  # Reemplazar placeholders
  nft replace rule inet soberano prerouting tcp dport 80 redirect to :${PORTAL_PORT} 2>/dev/null || true

  log "nftables configurado"
}

# ── Función: Configurar con iptables ─────────────────────────
setup_iptables() {
  # Habilitar forwarding
  sysctl -w net.ipv4.ip_forward=1 > /dev/null
  echo "net.ipv4.ip_forward=1" >> /etc/sysctl.conf 2>/dev/null || true

  # ── Chain: SOBERANO_AUTH (mangle) — marcar usuarios autenticados
  iptables -t mangle -N SOBERANO_AUTH
  # Los MACs en ipset "wifi_auth" se marcan como autenticados
  # (ipset se gestiona desde el backend wifi-soberano)
  if command -v ipset &>/dev/null; then
    ipset create wifi_auth hash:mac timeout 86400 2>/dev/null || true
    iptables -t mangle -A SOBERANO_AUTH -m set --match-set wifi_auth src -j MARK --set-mark ${MARK_AUTH}
  fi
  iptables -t mangle -A PREROUTING -i ${WIFI_INTERFACE} -j SOBERANO_AUTH

  # ── Chain: SOBERANO_PORTAL (nat) — redirigir al portal
  iptables -t nat -N SOBERANO_PORTAL

  # No redirigir tráfico del propio servidor
  for ip in "${WHITELIST_IPS[@]}"; do
    iptables -t nat -A SOBERANO_PORTAL -d "$ip" -j RETURN
  done

  # No redirigir DNS
  iptables -t nat -A SOBERANO_PORTAL -p udp --dport 53 -j RETURN
  iptables -t nat -A SOBERANO_PORTAL -p tcp --dport 53 -j RETURN

  # No redirigir DHCP
  iptables -t nat -A SOBERANO_PORTAL -p udp --dport 67:68 -j RETURN

  # No redirigir usuarios autenticados (marcados)
  iptables -t nat -A SOBERANO_PORTAL -m mark --mark ${MARK_AUTH} -j RETURN

  # ── Captive Portal Detection endpoints ──
  # Apple: /hotspot-detect.html → portal
  # Android: /generate_204 → portal
  # Microsoft: /connecttest.txt → portal
  # (Estos se manejan en Nginx, pero iptables asegura que lleguen)

  # Redirigir HTTP al portal
  iptables -t nat -A SOBERANO_PORTAL -p tcp --dport 80 -j DNAT --to-destination ${PORTAL_IP}:${PORTAL_PORT}

  # Redirigir HTTPS al portal (para captive portal detection)
  iptables -t nat -A SOBERANO_PORTAL -p tcp --dport 443 -j DNAT --to-destination ${PORTAL_IP}:443

  # Aplicar chain a tráfico WiFi
  iptables -t nat -A PREROUTING -i ${WIFI_INTERFACE} -s ${LAN_SUBNET} -j SOBERANO_PORTAL

  # ── Chain: SOBERANO_FWD (filter) — controlar forwarding
  iptables -t filter -N SOBERANO_FWD

  # Permitir established/related
  iptables -t filter -A SOBERANO_FWD -m state --state ESTABLISHED,RELATED -j ACCEPT

  # DNS siempre permitido
  iptables -t filter -A SOBERANO_FWD -p udp --dport 53 -j ACCEPT
  iptables -t filter -A SOBERANO_FWD -p tcp --dport 53 -j ACCEPT

  # DHCP permitido
  iptables -t filter -A SOBERANO_FWD -p udp --dport 67:68 -j ACCEPT

  # Tráfico al servidor portal siempre permitido
  iptables -t filter -A SOBERANO_FWD -d ${PORTAL_IP} -j ACCEPT

  # Usuarios autenticados pueden navegar
  iptables -t filter -A SOBERANO_FWD -m mark --mark ${MARK_AUTH} -j ACCEPT

  # Drop todo lo demás (no autenticados solo ven el portal)
  iptables -t filter -A SOBERANO_FWD -j DROP

  # Aplicar a FORWARD
  iptables -t filter -A FORWARD -i ${WIFI_INTERFACE} -j SOBERANO_FWD

  # ── NAT (masquerade) para salida a internet
  iptables -t nat -A POSTROUTING -s ${LAN_SUBNET} ! -o ${WIFI_INTERFACE} -j MASQUERADE

  log "iptables configurado"
}

# ── Scripts auxiliares: autenticar/desautenticar MACs ──────────
create_auth_scripts() {
  # Script para autenticar un dispositivo (llamado por wifi-soberano backend)
  cat > /usr/local/bin/wifi-auth-device << 'AUTHEOF'
#!/bin/bash
# Uso: wifi-auth-device <MAC_ADDRESS> [TIMEOUT_SECONDS]
# Autenticar un dispositivo WiFi (darle acceso a internet)
MAC="${1:?Uso: wifi-auth-device <MAC> [timeout_s]}"
TIMEOUT="${2:-86400}"  # Default 24 horas

if command -v ipset &>/dev/null; then
  ipset add wifi_auth "$MAC" timeout "$TIMEOUT" 2>/dev/null || \
  ipset -exist add wifi_auth "$MAC" timeout "$TIMEOUT"
  echo "✓ Autenticado: $MAC (${TIMEOUT}s)"
elif command -v nft &>/dev/null; then
  nft add element inet soberano authenticated "{ $MAC timeout ${TIMEOUT}s }"
  echo "✓ Autenticado: $MAC (${TIMEOUT}s)"
else
  echo "✗ No se puede autenticar: ni ipset ni nft disponibles"
  exit 1
fi

# Log para analytics
logger -t wifi-soberano "AUTH: $MAC timeout=${TIMEOUT}s"
AUTHEOF
  chmod +x /usr/local/bin/wifi-auth-device

  # Script para desautenticar un dispositivo
  cat > /usr/local/bin/wifi-deauth-device << 'DEAUTHEOF'
#!/bin/bash
# Uso: wifi-deauth-device <MAC_ADDRESS>
# Desautenticar un dispositivo WiFi (cortarle internet, vuelve al portal)
MAC="${1:?Uso: wifi-deauth-device <MAC>}"

if command -v ipset &>/dev/null; then
  ipset del wifi_auth "$MAC" 2>/dev/null
  echo "✓ Desautenticado: $MAC"
elif command -v nft &>/dev/null; then
  nft delete element inet soberano authenticated "{ $MAC }"
  echo "✓ Desautenticado: $MAC"
else
  echo "✗ No se puede desautenticar"
  exit 1
fi

# Matar conexiones existentes del dispositivo
conntrack -D -s "$MAC" 2>/dev/null || true

logger -t wifi-soberano "DEAUTH: $MAC"
DEAUTHEOF
  chmod +x /usr/local/bin/wifi-deauth-device

  # Script para listar dispositivos autenticados
  cat > /usr/local/bin/wifi-list-auth << 'LISTEOF'
#!/bin/bash
# Listar todos los dispositivos WiFi autenticados actualmente
echo "═══ Dispositivos WiFi Autenticados ═══"
if command -v ipset &>/dev/null; then
  ipset list wifi_auth 2>/dev/null | grep -E "^[0-9a-f]{2}:" || echo "(ninguno)"
elif command -v nft &>/dev/null; then
  nft list set inet soberano authenticated 2>/dev/null || echo "(ninguno)"
else
  echo "✗ Ni ipset ni nft disponibles"
fi
LISTEOF
  chmod +x /usr/local/bin/wifi-list-auth

  log "Scripts de autenticación creados en /usr/local/bin/"
}

# ── Configurar dnsmasq para captive portal detection ──────────
setup_dns_redirect() {
  if command -v dnsmasq &>/dev/null; then
    cat >> /etc/dnsmasq.conf << DNSEOF

# WiFi Soberano — Captive Portal DNS
# Redirigir dominios de detección de captive portal
address=/captive.apple.com/${PORTAL_IP}
address=/connectivitycheck.gstatic.com/${PORTAL_IP}
address=/clients3.google.com/${PORTAL_IP}
address=/www.msftconnecttest.com/${PORTAL_IP}
address=/www.msftncsi.com/${PORTAL_IP}
address=/detectportal.firefox.com/${PORTAL_IP}

# Redirigir nuestro dominio
address=/${DOMAIN}/${PORTAL_IP}
DNSEOF
    systemctl restart dnsmasq
    log "dnsmasq configurado para captive portal detection"
  else
    warn "dnsmasq no instalado — instalar para mejor captive portal detection"
    warn "  apt-get install dnsmasq"
  fi
}

# ── Ejecutar ──────────────────────────────────────────────────
case "${1:-setup}" in
  setup)
    clean_rules
    if $USE_NFT; then
      setup_nftables
    else
      setup_iptables
    fi
    create_auth_scripts
    setup_dns_redirect
    ;;
  clean|reset)
    clean_rules
    log "Todas las reglas de captive portal eliminadas"
    ;;
  auth)
    /usr/local/bin/wifi-auth-device "${2:-}" "${3:-86400}"
    ;;
  deauth)
    /usr/local/bin/wifi-deauth-device "${2:-}"
    ;;
  list)
    /usr/local/bin/wifi-list-auth
    ;;
  status)
    echo -e "${CYAN}═══ Estado del Captive Portal ═══${NC}"
    echo ""
    if $USE_NFT; then
      echo "Backend: nftables"
      nft list table inet soberano 2>/dev/null || echo "  (tabla no encontrada)"
    else
      echo "Backend: iptables"
      echo ""
      echo "── NAT (SOBERANO_PORTAL) ──"
      iptables -t nat -L SOBERANO_PORTAL -n -v 2>/dev/null || echo "  (chain no encontrada)"
      echo ""
      echo "── MANGLE (SOBERANO_AUTH) ──"
      iptables -t mangle -L SOBERANO_AUTH -n -v 2>/dev/null || echo "  (chain no encontrada)"
      echo ""
      echo "── FILTER (SOBERANO_FWD) ──"
      iptables -t filter -L SOBERANO_FWD -n -v 2>/dev/null || echo "  (chain no encontrada)"
    fi
    echo ""
    echo "── Dispositivos autenticados ──"
    /usr/local/bin/wifi-list-auth 2>/dev/null || echo "  (script no instalado)"
    ;;
  *)
    echo "Uso: $0 {setup|clean|auth <MAC> [timeout]|deauth <MAC>|list|status}"
    exit 1
    ;;
esac

echo ""
echo -e "${GREEN}📡 Captive Portal Redirect configurado${NC}"
echo ""
echo "  Comandos disponibles:"
echo "    wifi-auth-device <MAC> [timeout_s]   — Dar acceso a un dispositivo"
echo "    wifi-deauth-device <MAC>             — Cortar acceso"
echo "    wifi-list-auth                       — Listar autenticados"
echo "    $0 status                            — Ver estado completo"
echo ""
echo "  El backend wifi-soberano llama automáticamente a estos"
echo "  scripts cuando un usuario paga y su sesión expira."
echo ""

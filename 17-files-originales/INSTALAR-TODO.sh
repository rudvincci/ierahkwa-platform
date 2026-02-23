#!/bin/bash
#
# 🦅 INSTALAR TODO — Mamey Sovereign Platform
# Corre esto UNA VEZ desde cualquier lugar
#
set -euo pipefail

MAMEY="$HOME/Desktop/Mamey-main"
SCRIPTS_URL="$HOME/Downloads" # donde se descargan los archivos de Claude

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

echo ""
echo -e "${CYAN}╔═══════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  🦅 INSTALACIÓN COMPLETA — Ierahkwa Sovereign        ║${NC}"
echo -e "${CYAN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""

cd "$MAMEY"

# ── 1. Copy scripts ──
echo -e "${CYAN}[1/4] Instalando scripts...${NC}"

for f in install-security.sh audit-platforms.sh start-mamey-secure.sh stop-mamey.sh rotate-keys.sh; do
    if [ -f "$SCRIPTS_URL/$f" ]; then
        cp "$SCRIPTS_URL/$f" "$MAMEY/$f"
        chmod +x "$MAMEY/$f"
        echo -e "  ${GREEN}✓ $f${NC}"
    else
        echo -e "  ${YELLOW}⚠ $f not found in Downloads${NC}"
    fi
done

# ── 2. Run security ──
echo -e "${CYAN}[2/4] Instalando seguridad...${NC}"
if [ -f "$MAMEY/install-security.sh" ]; then
    bash "$MAMEY/install-security.sh"
else
    echo -e "  ${RED}✗ install-security.sh not found${NC}"
fi

# ── 3. Run audit ──
echo -e "${CYAN}[3/4] Auditando plataformas...${NC}"
if [ -f "$MAMEY/audit-platforms.sh" ]; then
    bash "$MAMEY/audit-platforms.sh"
else
    echo -e "  ${RED}✗ audit-platforms.sh not found${NC}"
fi

# ── 4. Dashboard ──
echo -e "${CYAN}[4/4] Abriendo dashboard...${NC}"
if [ -f "$SCRIPTS_URL/mamey-dashboard.html" ]; then
    cp "$SCRIPTS_URL/mamey-dashboard.html" "$MAMEY/dashboard.html"
    open "$MAMEY/dashboard.html"
    echo -e "  ${GREEN}✓ Dashboard abierto en Chrome${NC}"
fi

echo ""
echo -e "${GREEN}╔═══════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  ✅ TODO INSTALADO                                    ║${NC}"
echo -e "${GREEN}╠═══════════════════════════════════════════════════════╣${NC}"
echo -e "${GREEN}║${NC}  Scripts en:   ~/Desktop/Mamey-main/                  ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  Seguridad en: ~/Desktop/Mamey-main/.security/        ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  Dashboard:    ~/Desktop/Mamey-main/dashboard.html    ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}                                                       ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  ${CYAN}Comandos:${NC}                                          ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}    ./start-mamey-secure.sh   ← Arrancar              ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}    ./stop-mamey.sh           ← Parar                 ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}    ./audit-platforms.sh      ← Auditar               ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}    ./rotate-keys.sh          ← Rotar credenciales    ${GREEN}║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""

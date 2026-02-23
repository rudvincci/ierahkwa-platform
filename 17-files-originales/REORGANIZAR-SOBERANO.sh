#!/bin/bash
#
# REORGANIZAR-SOBERANO.sh
# Organiza todos los archivos regados en una estructura limpia
# NO BORRA NADA — solo COPIA a la nueva estructura
# Las carpetas originales quedan intactas hasta que tú decidas borrarlas
#
set -eo pipefail

BASE="/Users/ruddie/Desktop/files"
DEST="$BASE/Soberano-Organizado"

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
RED='\033[0;31m'
NC='\033[0m'
COPIED=0

copy_file() {
    local src="$1" dst="$2"
    if [ -f "$src" ]; then
        mkdir -p "$(dirname "$dst")"
        cp "$src" "$dst"
        echo -e "  ${GREEN}+${NC} $(basename "$dst")"
        ((COPIED++))
    else
        echo -e "  ${RED}!${NC} No encontrado: $src"
    fi
}

echo -e "${CYAN}"
echo "================================================================"
echo "  REORGANIZACIÓN — Plataforma Soberana Ierahkwa"
echo "  Fecha: $(date '+%Y-%m-%d %H:%M')"
echo "  MODO SEGURO: Solo copia, NO borra nada"
echo "================================================================"
echo -e "${NC}"

# Verificar que existe la carpeta base
if [ ! -d "$BASE/files" ]; then
    echo -e "${RED}Error: No se encuentra $BASE/files${NC}"
    exit 1
fi

# Crear estructura
echo -e "${CYAN}[1/9] Creando estructura de carpetas...${NC}"
mkdir -p "$DEST"/{01-documentos/{legal,inversores,tecnico,auditoria},02-plataformas-html/{correo-soberano,red-soberana,busqueda-soberana,canal-soberano,musica-soberana,hospedaje-soberano,artesania-soberana,cortos-indigenas,comercio-soberano,invertir-soberano,docs-soberanos,mapa-soberano,voz-soberana,trabajo-soberano,bdet-bank,soberano-ecosystem,portal-soberano,bdet-wallet,blockchain-explorer,fiscal-dashboard,fiscal-transparency,trading-dashboard,education-dashboard,healthcare-dashboard,admin-dashboard,landing-page,infographic,code-soberano,landing-ierahkwa},03-backend/{api-gateway,blockchain-api,red-social,social-media,voto-soberano,reservas,plataforma-principal,trading},04-infraestructura/{docker,kubernetes,ci-cd,nginx,terraform,blockchain},05-api,06-dashboards,07-scripts,08-dotnet,09-assets,10-outreach}
echo -e "  ${GREEN}Estructura creada${NC}"

# ==========================================
# 01 - DOCUMENTOS LEGALES
# ==========================================
echo -e "\n${CYAN}[2/9] Copiando documentos legales y de gobierno...${NC}"
copy_file "$BASE/files-8/GOVERNANCE-CONSTITUTION.md"   "$DEST/01-documentos/legal/GOVERNANCE-CONSTITUTION.md"
copy_file "$BASE/files-10/BILL-OF-DIGITAL-RIGHTS.md"   "$DEST/01-documentos/legal/BILL-OF-DIGITAL-RIGHTS.md"
copy_file "$BASE/files-10/DATA-SOVEREIGNTY-ACT.md"      "$DEST/01-documentos/legal/DATA-SOVEREIGNTY-ACT.md"
copy_file "$BASE/files-9/FISCAL-POLICY.md"              "$DEST/01-documentos/legal/FISCAL-POLICY.md"
copy_file "$BASE/files-8/TOKENOMICS-WAMPUM.md"          "$DEST/01-documentos/legal/TOKENOMICS-WAMPUM.md"
copy_file "$BASE/files-4/LICENSE.md"                     "$DEST/01-documentos/legal/LICENSE.md"

# Inversores
echo -e "\n${CYAN}   Documentos para inversores...${NC}"
copy_file "$BASE/files-4/INVESTOR-ONE-PAGER.md"         "$DEST/01-documentos/inversores/INVESTOR-ONE-PAGER.md"
copy_file "$BASE/files-13/INVESTOR-DATA-ROOM.md"        "$DEST/01-documentos/inversores/INVESTOR-DATA-ROOM.md"
copy_file "$BASE/files-8/BUSINESS-PLAN.md"              "$DEST/01-documentos/inversores/BUSINESS-PLAN.md"
copy_file "$BASE/files-10/WHITEPAPER-IERAHKWA.md"       "$DEST/01-documentos/inversores/WHITEPAPER-IERAHKWA.md"

# Técnico
echo -e "\n${CYAN}   Documentos técnicos...${NC}"
copy_file "$BASE/files-14/DEVELOPER-ONBOARDING.md"      "$DEST/01-documentos/tecnico/DEVELOPER-ONBOARDING.md"
copy_file "$BASE/files-4/ONBOARDING.md"                  "$DEST/01-documentos/tecnico/ONBOARDING.md"
copy_file "$BASE/files-8/DISASTER-RECOVERY.md"           "$DEST/01-documentos/tecnico/DISASTER-RECOVERY.md"
copy_file "$BASE/files-7/CHANGELOG.md"                   "$DEST/01-documentos/tecnico/CHANGELOG.md"
copy_file "$BASE/files-2/EMAILS-OUTREACH.md"             "$DEST/01-documentos/tecnico/EMAILS-OUTREACH.md"

# Auditoría (usar las versiones de files/files/ que son las consolidadas)
echo -e "\n${CYAN}   Documentos de auditoría y mapeo...${NC}"
copy_file "$BASE/files/AUDITORIA-PLATAFORMA-SOBERANA.md" "$DEST/01-documentos/auditoria/AUDITORIA-PLATAFORMA-SOBERANA.md"
copy_file "$BASE/files/MAPA-COMPLETO-PLATAFORMA-SOBERANA.md" "$DEST/01-documentos/auditoria/MAPA-COMPLETO-PLATAFORMA-SOBERANA.md"
copy_file "$BASE/files/INDEX.md"                          "$DEST/01-documentos/auditoria/INDEX.md"
copy_file "$BASE/files/INDIGENOUS-OUTREACH-DATABASE.md"   "$DEST/01-documentos/auditoria/INDIGENOUS-OUTREACH-DATABASE.md"
copy_file "$BASE/files/EMPEZAR-AQUI-NUEVO.md"            "$DEST/01-documentos/auditoria/EMPEZAR-AQUI-NUEVO.md"
copy_file "$BASE/files/README-MAMEY.md"                   "$DEST/01-documentos/auditoria/README-MAMEY.md"

# ==========================================
# 02 - PLATAFORMAS HTML
# ==========================================
echo -e "\n${CYAN}[3/9] Copiando plataformas HTML...${NC}"

# Las 20 plataformas principales (de files/files/)
copy_file "$BASE/files/index.html"                "$DEST/02-plataformas-html/correo-soberano/index.html"
copy_file "$BASE/files/red-soberana.html"          "$DEST/02-plataformas-html/red-soberana/index.html"
copy_file "$BASE/files/busqueda-soberana.html"     "$DEST/02-plataformas-html/busqueda-soberana/index.html"
copy_file "$BASE/files/canal-soberano.html"        "$DEST/02-plataformas-html/canal-soberano/index.html"
copy_file "$BASE/files/musica-soberana.html"       "$DEST/02-plataformas-html/musica-soberana/index.html"
copy_file "$BASE/files/hospedaje-soberano.html"    "$DEST/02-plataformas-html/hospedaje-soberano/index.html"
copy_file "$BASE/files/artesania-soberana.html"    "$DEST/02-plataformas-html/artesania-soberana/index.html"
copy_file "$BASE/files/cortos-indigenas.html"      "$DEST/02-plataformas-html/cortos-indigenas/index.html"
copy_file "$BASE/files/comercio-soberano.html"     "$DEST/02-plataformas-html/comercio-soberano/index.html"
copy_file "$BASE/files/invertir-soberano.html"     "$DEST/02-plataformas-html/invertir-soberano/index.html"
copy_file "$BASE/files/docs-soberanos.html"        "$DEST/02-plataformas-html/docs-soberanos/index.html"
copy_file "$BASE/files/mapa-soberano.html"         "$DEST/02-plataformas-html/mapa-soberano/index.html"
copy_file "$BASE/files/voz-soberana.html"          "$DEST/02-plataformas-html/voz-soberana/index.html"
copy_file "$BASE/files/trabajo-soberano.html"      "$DEST/02-plataformas-html/trabajo-soberano/index.html"
copy_file "$BASE/files/bdet-bank-payment-system.html" "$DEST/02-plataformas-html/bdet-bank/index.html"
copy_file "$BASE/files/soberano-ecosystem.html"    "$DEST/02-plataformas-html/soberano-ecosystem/index.html"

# Plataformas de carpetas numeradas (ÚNICAS - no están en files/files/)
copy_file "$BASE/files-3/portal-soberano.html"     "$DEST/02-plataformas-html/portal-soberano/index.html"
copy_file "$BASE/files-7/bdet-wallet.html"         "$DEST/02-plataformas-html/bdet-wallet/index.html"
copy_file "$BASE/files-7/blockchain-explorer.html" "$DEST/02-plataformas-html/blockchain-explorer/index.html"
copy_file "$BASE/files-9/fiscal-dashboard.html"    "$DEST/02-plataformas-html/fiscal-dashboard/index.html"
copy_file "$BASE/files-17/fiscal-transparency.html" "$DEST/02-plataformas-html/fiscal-transparency/index.html"
copy_file "$BASE/files-17/trading-dashboard.html"  "$DEST/02-plataformas-html/trading-dashboard/index.html"
copy_file "$BASE/files-10/education-dashboard.html" "$DEST/02-plataformas-html/education-dashboard/index.html"
copy_file "$BASE/files-10/healthcare-dashboard.html" "$DEST/02-plataformas-html/healthcare-dashboard/index.html"
copy_file "$BASE/files-4/admin-dashboard.html"     "$DEST/02-plataformas-html/admin-dashboard/index.html"
copy_file "$BASE/files-4/landing.html"             "$DEST/02-plataformas-html/landing-page/index.html"
copy_file "$BASE/files-10/INFOGRAPHIC.html"        "$DEST/02-plataformas-html/infographic/index.html"

# Plataformas que son DIFERENTES aunque tengan el mismo nombre
copy_file "$BASE/files-4/index.html"               "$DEST/02-plataformas-html/code-soberano/index.html"
copy_file "$BASE/files-13/index.html"              "$DEST/02-plataformas-html/landing-ierahkwa/index.html"

# ==========================================
# 03 - BACKEND
# ==========================================
echo -e "\n${CYAN}[4/9] Copiando código backend...${NC}"

# Cada server.js es un servicio DIFERENTE - van a carpetas con nombre
copy_file "$BASE/files-6/server.js"                "$DEST/03-backend/api-gateway/server.js"
copy_file "$BASE/files-7/server.js"                "$DEST/03-backend/blockchain-api/server.js"
copy_file "$BASE/files-11/server.js"               "$DEST/03-backend/red-social/server.js"
copy_file "$BASE/files-11/chat.js"                 "$DEST/03-backend/red-social/chat.js"
copy_file "$BASE/files-11/posts.js"                "$DEST/03-backend/red-social/posts.js"
copy_file "$BASE/files-11/trading.js"              "$DEST/03-backend/red-social/trading.js"
copy_file "$BASE/files-14/server.js"               "$DEST/03-backend/voto-soberano/server.js"
copy_file "$BASE/files-15/server.js"               "$DEST/03-backend/reservas/server.js"
copy_file "$BASE/files-15/bookings.js"             "$DEST/03-backend/reservas/bookings.js"
copy_file "$BASE/files-15/categories.js"           "$DEST/03-backend/reservas/categories.js"
copy_file "$BASE/files-16/server.js"               "$DEST/03-backend/plataforma-principal/server.js"
copy_file "$BASE/files-4/index.js"                 "$DEST/03-backend/plataforma-principal/index.js"
copy_file "$BASE/files-17/index.js"                "$DEST/03-backend/trading/index.js"

# Backend de mnt/ (subcarpetas con rutas más profundas)
copy_file "$BASE/files-12/server.js"               "$DEST/03-backend/social-media/server.js"

# Backend de mnt paths
if [ -f "$BASE/files-14/mnt/user-data/outputs/red-soberana-repo/backend/platforms/voto-soberano/server.js" ]; then
    copy_file "$BASE/files-14/mnt/user-data/outputs/red-soberana-repo/backend/platforms/voto-soberano/server.js" "$DEST/03-backend/voto-soberano/server-mnt-version.js"
fi

# ==========================================
# 04 - INFRAESTRUCTURA
# ==========================================
echo -e "\n${CYAN}[5/9] Copiando infraestructura y DevOps...${NC}"

copy_file "$BASE/files-2/docker-compose.yml"       "$DEST/04-infraestructura/docker/docker-compose.yml"
copy_file "$BASE/files-16/docker-compose.full.yml" "$DEST/04-infraestructura/docker/docker-compose.full.yml"
copy_file "$BASE/files-2/sovereign-cluster.yaml"   "$DEST/04-infraestructura/kubernetes/sovereign-cluster.yaml"
copy_file "$BASE/files-2/github-actions.yml"       "$DEST/04-infraestructura/ci-cd/github-actions.yml"
copy_file "$BASE/files-13/ci.yml"                  "$DEST/04-infraestructura/ci-cd/ci.yml"
copy_file "$BASE/files-6/nginx.conf"               "$DEST/04-infraestructura/nginx/nginx.conf"
copy_file "$BASE/files-7/main.tf"                  "$DEST/04-infraestructura/terraform/main.tf"
copy_file "$BASE/files-6/hardhat.config.js"        "$DEST/04-infraestructura/blockchain/hardhat.config.js"
copy_file "$BASE/files-9/FiscalAllocation.sol"     "$DEST/04-infraestructura/blockchain/FiscalAllocation.sol"

# ==========================================
# 05 - API
# ==========================================
echo -e "\n${CYAN}[6/9] Copiando especificaciones API...${NC}"
copy_file "$BASE/files-14/openapi.yaml"            "$DEST/05-api/openapi.yaml"

# ==========================================
# 06 - DASHBOARDS
# ==========================================
echo -e "\n${CYAN}[7/9] Copiando dashboards principales...${NC}"
copy_file "$BASE/files/mamey-dashboard.html"       "$DEST/06-dashboards/mamey-dashboard.html"
copy_file "$BASE/files/dashboard.html"             "$DEST/06-dashboards/dashboard-command-center.html"

# ==========================================
# 07 - SCRIPTS (usar versiones MEJORADAS cuando existan)
# ==========================================
echo -e "\n${CYAN}[8/9] Copiando scripts (versiones mejoradas)...${NC}"

# INSTALAR-TODO.sh: La de files-22 es la MEJORADA (10KB vs 3.5KB)
copy_file "$BASE/files-22/INSTALAR-TODO.sh"        "$DEST/07-scripts/INSTALAR-TODO.sh"
copy_file "$BASE/files/UNIFICAR-TODO.sh"           "$DEST/07-scripts/UNIFICAR-TODO.sh"
copy_file "$BASE/files/UPGRADE-MAMEY.sh"           "$DEST/07-scripts/UPGRADE-MAMEY.sh"
copy_file "$BASE/files/install-security.sh"        "$DEST/07-scripts/install-security.sh"
copy_file "$BASE/files/start-mamey-secure.sh"      "$DEST/07-scripts/start-mamey-secure.sh"
copy_file "$BASE/files/stop-mamey.sh"              "$DEST/07-scripts/stop-mamey.sh"
copy_file "$BASE/files/audit-platforms.sh"         "$DEST/07-scripts/audit-platforms.sh"
copy_file "$BASE/files/rotate-keys.sh"             "$DEST/07-scripts/rotate-keys.sh"
copy_file "$BASE/files/descubrir-plataformas.sh"   "$DEST/07-scripts/descubrir-plataformas.sh"
copy_file "$BASE/files/generate-sln.sh"            "$DEST/07-scripts/generate-sln.sh"

# ==========================================
# 08 - .NET
# ==========================================
copy_file "$BASE/files/IerahkwaMamey.sln"          "$DEST/08-dotnet/IerahkwaMamey.sln"

# ==========================================
# 09 - ASSETS
# ==========================================
echo -e "\n${CYAN}[9/9] Copiando assets y extras...${NC}"
copy_file "$BASE/files-8/logo.svg"                 "$DEST/09-assets/logo.svg"

# README principal — usar el de files-16 que es el más completo para GitHub
copy_file "$BASE/files-16/README.md"               "$DEST/README-github.md"
# Y el de files/files/ que es el índice de plataformas
copy_file "$BASE/files/README.md"                  "$DEST/README-indice-plataformas.md"

# Documentos de producción
if [ -f "$BASE/files/Sovereign-Platform-Plan-Produccion.docx" ]; then
    copy_file "$BASE/files/Sovereign-Platform-Plan-Produccion.docx" "$DEST/01-documentos/inversores/Sovereign-Platform-Plan-Produccion.docx"
fi
if [ -f "$BASE/files/IERAHKWA-SECURITY-ARCHITECTURE.docx" ]; then
    copy_file "$BASE/files/IERAHKWA-SECURITY-ARCHITECTURE.docx" "$DEST/01-documentos/tecnico/IERAHKWA-SECURITY-ARCHITECTURE.docx"
fi

# Copiar plataformas de mnt/user-data/outputs/plataformas/ si existen
if [ -d "$BASE/files/mnt/user-data/outputs/plataformas" ]; then
    echo -e "\n${CYAN}   Copiando plataformas de mnt/...${NC}"
    cp -R "$BASE/files/mnt/user-data/outputs/plataformas/" "$DEST/02-plataformas-html/mnt-plataformas/" 2>/dev/null && echo -e "  ${GREEN}+ mnt-plataformas/ (directorio completo)${NC}" || true
fi

# CloudSoberana de files-3/mnt
if [ -f "$BASE/files-3/mnt/user-data/outputs/red-soberana-repo/plataformas/98-CloudSoberana/README.md" ]; then
    mkdir -p "$DEST/02-plataformas-html/cloud-soberana"
    copy_file "$BASE/files-3/mnt/user-data/outputs/red-soberana-repo/plataformas/98-CloudSoberana/README.md" "$DEST/02-plataformas-html/cloud-soberana/README.md"
fi

# ==========================================
# ARCHIVOS ÚNICOS DENTRO DE files/files/files-2/ y files/files/files-3/
# (Subcarpetas anidadas con contenido que NO existe en otro lugar)
# ==========================================
echo -e "\n${CYAN}   Copiando archivos únicos de subcarpetas anidadas...${NC}"

# files/files/files-3/ tiene contenido técnico ÚNICO
mkdir -p "$DEST/04-infraestructura/blockchain"
mkdir -p "$DEST/04-infraestructura/database"
mkdir -p "$DEST/05-api"
mkdir -p "$DEST/02-plataformas-html/pitch-deck"

copy_file "$BASE/files/files-3/BDETContracts.sol"          "$DEST/04-infraestructura/blockchain/BDETContracts.sol"
copy_file "$BASE/files/files-3/schema.sql"                  "$DEST/04-infraestructura/database/schema.sql"
copy_file "$BASE/files/files-3/API-SPECIFICATION.md"        "$DEST/05-api/API-SPECIFICATION.md"
copy_file "$BASE/files/files-3/pitch-deck-soberano.html"    "$DEST/02-plataformas-html/pitch-deck/index.html"

# files/files/files-2/ tiene WHITEPAPERS y READMEs por plataforma (ÚNICOS)
if [ -d "$BASE/files/files-2/mnt/user-data/outputs/plataformas" ]; then
    echo -e "\n${CYAN}   Copiando whitepapers por plataforma...${NC}"
    mkdir -p "$DEST/01-documentos/whitepapers"
    # Whitepaper general
    copy_file "$BASE/files/files-2/WHITEPAPER.md" "$DEST/01-documentos/whitepapers/WHITEPAPER-GENERAL.md"
    # Whitepapers por plataforma
    for platdir in "$BASE/files/files-2/mnt/user-data/outputs/plataformas/"*/; do
        platname=$(basename "$platdir")
        if [ -f "$platdir/WHITEPAPER.md" ]; then
            mkdir -p "$DEST/01-documentos/whitepapers/$platname"
            cp "$platdir/WHITEPAPER.md" "$DEST/01-documentos/whitepapers/$platname/WHITEPAPER.md"
            [ -f "$platdir/README.md" ] && cp "$platdir/README.md" "$DEST/01-documentos/whitepapers/$platname/README.md"
            echo -e "  ${GREEN}+${NC} $platname/WHITEPAPER.md"
            ((COPIED++))
        fi
    done
fi

# Plataformas HTML que están en mnt/ pero NO en el root de files/files/
# (15-RentaSoberano, 18-SabiduriaSoberana, 19-UniversidadSoberana, 20-NoticiaSoberana)
echo -e "\n${CYAN}   Copiando plataformas adicionales de mnt/...${NC}"
MNT_PLAT="$BASE/files/mnt/user-data/outputs/plataformas"
declare -A extra_platforms=(
    ["15-RentaSoberano"]="renta-soberano"
    ["18-SabiduriaSoberana"]="sabiduria-soberana"
    ["19-UniversidadSoberana"]="universidad-soberana"
    ["20-NoticiaSoberana"]="noticia-soberana"
)
for plat_src in "${!extra_platforms[@]}"; do
    plat_dst="${extra_platforms[$plat_src]}"
    if [ -f "$MNT_PLAT/$plat_src/index.html" ]; then
        mkdir -p "$DEST/02-plataformas-html/$plat_dst"
        copy_file "$MNT_PLAT/$plat_src/index.html" "$DEST/02-plataformas-html/$plat_dst/index.html"
    fi
done

# ==========================================
# RESUMEN
# ==========================================
echo -e "\n${CYAN}================================================================${NC}"
echo -e "${GREEN}  REORGANIZACIÓN COMPLETADA${NC}"
echo -e "${CYAN}================================================================${NC}"
echo ""
echo -e "  Archivos copiados: ${GREEN}$COPIED${NC}"
echo -e "  Destino: ${CYAN}$DEST${NC}"
echo ""
echo -e "${YELLOW}  IMPORTANTE:${NC}"
echo -e "  - Las carpetas originales NO fueron borradas"
echo -e "  - Verifica que todo está correcto en Soberano-Organizado/"
echo -e "  - Cuando estés seguro, puedes borrar las carpetas files-N"
echo ""

# Mostrar estructura final
echo -e "${CYAN}  Estructura creada:${NC}"
echo ""
find "$DEST" -maxdepth 2 -type d | sort | while read d; do
    depth=$(($(echo "$d" | sed "s|$DEST||" | tr -cd '/' | wc -c)))
    indent=""
    for ((i=0; i<depth; i++)); do indent="  $indent"; done
    name=$(basename "$d")
    count=$(find "$d" -maxdepth 1 -type f 2>/dev/null | wc -l | tr -d ' ')
    if [ "$count" -gt 0 ]; then
        echo -e "  ${indent}${name}/ ${CYAN}(${count} archivos)${NC}"
    else
        echo -e "  ${indent}${name}/"
    fi
done

echo ""
echo -e "${GREEN}Listo. Revisa la carpeta Soberano-Organizado/ antes de borrar nada.${NC}"

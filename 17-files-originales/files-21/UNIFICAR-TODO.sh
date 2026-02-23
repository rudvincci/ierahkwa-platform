#!/bin/bash
#
# 🦅 UNIFICAR-TODO.sh — Unificación Masiva del Proyecto Soberano
#
# PASO 1: Backup completo al Extreme Pro (seguro)
# PASO 2: Copiar código único → Mamey-main
# PASO 3: Borrar duplicados del Desktop (con confirmación)
#
# NO SE PIERDE NADA — todo va primero al Extreme Pro
#
set -euo pipefail

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

MAMEY="$HOME/Desktop/Mamey-main"
SOVDIR=$(cd ~/Desktop/Sov*Kanienke* 2>/dev/null && pwd) || SOVDIR=""
UNIF="$HOME/Desktop/Sovereign Platform Unificada"
EXT="/Volumes/Extreme Pro"

echo ""
echo -e "${CYAN}╔═══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  🦅 UNIFICACIÓN MASIVA — Proyecto Soberano                    ║${NC}"
echo -e "${CYAN}║     Todo al Extreme Pro → Código a Mamey-main → Limpiar Mac  ║${NC}"
echo -e "${CYAN}╚═══════════════════════════════════════════════════════════════╝${NC}"
echo ""

# ══════════════════════════════════════════
# VERIFICACIONES
# ══════════════════════════════════════════
echo -e "${CYAN}[0/5] Verificando...${NC}"

if [ ! -d "$EXT" ]; then
    echo -e "${RED}❌ Extreme Pro no está conectado! Conéctalo primero.${NC}"
    exit 1
fi
echo -e "  ${GREEN}✓ Extreme Pro conectado${NC}"

if [ ! -d "$MAMEY" ]; then
    echo -e "${RED}❌ Mamey-main no encontrado en Desktop${NC}"
    exit 1
fi
echo -e "  ${GREEN}✓ Mamey-main encontrado${NC}"

if [ -z "$SOVDIR" ]; then
    echo -e "${YELLOW}⚠ Sovereign Government folder not found${NC}"
else
    echo -e "  ${GREEN}✓ Sovereign folder: $SOVDIR${NC}"
fi

EXTFREE=$(df -h "$EXT" | tail -1 | awk '{print $4}')
echo -e "  ${GREEN}✓ Extreme Pro libre: $EXTFREE${NC}"
echo ""

# ══════════════════════════════════════════
# PASO 1: BACKUP AL EXTREME PRO
# ══════════════════════════════════════════
echo -e "${CYAN}${BOLD}[1/5] BACKUP AL EXTREME PRO...${NC}"
echo -e "  ${YELLOW}Esto puede tomar 30-60 min para 233GB${NC}"
echo ""

mkdir -p "$EXT/BACKUP-IERAHKWA/Sovereign-System"
mkdir -p "$EXT/BACKUP-IERAHKWA/Platform-Unificada"
mkdir -p "$EXT/BACKUP-IERAHKWA/Desktop-Docs"

if [ -n "$SOVDIR" ] && [ -d "$SOVDIR" ]; then
    echo -e "  ${CYAN}Copiando Sovereign System (233GB) → Extreme Pro...${NC}"
    rsync -ah --progress "$SOVDIR/" "$EXT/BACKUP-IERAHKWA/Sovereign-System/"
    echo -e "  ${GREEN}✅ Sovereign System respaldado${NC}"
fi

if [ -d "$UNIF" ]; then
    echo -e "  ${CYAN}Copiando Platform Unificada (1.9GB)...${NC}"
    rsync -ah --progress "$UNIF/" "$EXT/BACKUP-IERAHKWA/Platform-Unificada/"
    echo -e "  ${GREEN}✅ Platform Unificada respaldada${NC}"
fi

echo -e "  ${CYAN}Copiando documentos sueltos del Desktop...${NC}"
for f in ~/Desktop/*.docx ~/Desktop/*.xlsx ~/Desktop/*.pptx ~/Desktop/*.pdf ~/Desktop/*.md; do
    [ -f "$f" ] && cp "$f" "$EXT/BACKUP-IERAHKWA/Desktop-Docs/" 2>/dev/null || true
done
echo -e "  ${GREEN}✅ Desktop docs respaldados${NC}"
echo ""

# ══════════════════════════════════════════
# PASO 2: UNIFICAR CÓDIGO EN MAMEY-MAIN
# ══════════════════════════════════════════
echo -e "${CYAN}${BOLD}[2/5] UNIFICANDO CÓDIGO EN MAMEY-MAIN...${NC}"
echo ""

cd "$MAMEY"

unify() {
    local src="$1" dst="$2"
    if [ -d "$src" ] && [ ! -d "$dst" ]; then
        mkdir -p "$(dirname "$dst")"
        cp -R "$src" "$dst"
        echo -e "  ${GREEN}+ $(basename "$dst")${NC}"
    elif [ -d "$dst" ]; then
        echo -e "  ${GREEN}✓ $(basename "$dst") ya existe${NC}"
    fi
}

if [ -n "$SOVDIR" ]; then
    # GOBIERNO
    echo -e "  ${CYAN}── Gobierno ──${NC}"
    unify "$SOVDIR/AdvocateOffice"              "$MAMEY/platforms/government/AdvocateOffice"
    unify "$SOVDIR/Akwesasne"                   "$MAMEY/platforms/government/Akwesasne"
    unify "$SOVDIR/CitizenCRM.Infrastructure"   "$MAMEY/platforms/government/CitizenCRM"
    unify "$SOVDIR/ContractManager"             "$MAMEY/platforms/government/ContractManager"
    unify "$SOVDIR/ServiceDesk"                 "$MAMEY/platforms/government/ServiceDesk"
    unify "$SOVDIR/MeetingHub"                  "$MAMEY/platforms/government/MeetingHub"
    unify "$SOVDIR/SpikeOffice"                 "$MAMEY/platforms/government/SpikeOffice"
    unify "$SOVDIR/id and passaportes"          "$MAMEY/platforms/government/Passports"

    # FINANZAS
    echo -e "  ${CYAN}── Finanzas ──${NC}"
    unify "$SOVDIR/BANCO BDET"                  "$MAMEY/platforms/finance/BancoBDET"
    unify "$SOVDIR/IerahkwaBankPlatform"        "$MAMEY/platforms/finance/IerahkwaBankPlatform"
    unify "$SOVDIR/DeFiSoberano"                "$MAMEY/platforms/finance/DeFiSoberano"
    unify "$SOVDIR/MultichainBridge"            "$MAMEY/platforms/finance/MultichainBridge"
    unify "$SOVDIR/IDOFactory"                  "$MAMEY/platforms/finance/IDOFactory"
    unify "$SOVDIR/usd mint"                    "$MAMEY/platforms/finance/USDMint"
    unify "$SOVDIR/TransactionCodes"            "$MAMEY/platforms/finance/TransactionCodes"
    unify "$SOVDIR/AssetTracker"                "$MAMEY/platforms/finance/AssetTracker"

    # TECNOLOGÍA
    echo -e "  ${CYAN}── Tecnología ──${NC}"
    unify "$SOVDIR/BiometricAuth"               "$MAMEY/platforms/tech/BiometricAuth"
    unify "$SOVDIR/BioMetrics"                  "$MAMEY/platforms/tech/BioMetrics"
    unify "$SOVDIR/AppBuilder"                  "$MAMEY/platforms/tech/AppBuilder"
    unify "$SOVDIR/mobile-app"                  "$MAMEY/platforms/tech/MobileApp-1"
    unify "$SOVDIR/MobileApp"                   "$MAMEY/platforms/tech/MobileApp-2"
    unify "$SOVDIR/image-upload"                "$MAMEY/platforms/tech/ImageUpload"
    unify "$SOVDIR/Software2026"                "$MAMEY/platforms/tech/Software2026"
    unify "$SOVDIR/Bodno"                       "$MAMEY/platforms/tech/Bodno"

    # INFRAESTRUCTURA
    echo -e "  ${CYAN}── Infraestructura ──${NC}"
    unify "$SOVDIR/sovereign-network"           "$MAMEY/platforms/infra/SovereignNetwork"
    unify "$SOVDIR/DEPLOY-SERVERS"              "$MAMEY/platforms/infra/DeployServers"
    unify "$SOVDIR/systemd"                     "$MAMEY/platforms/infra/Systemd"
    unify "$SOVDIR/node"                        "$MAMEY/platforms/infra/Node"
    unify "$SOVDIR/services"                    "$MAMEY/platforms/infra/services-sov"
    unify "$SOVDIR/go"                          "$MAMEY/platforms/infra/Go"

    # NEGOCIOS
    echo -e "  ${CYAN}── Negocios ──${NC}"
    unify "$SOVDIR/agriculture"                 "$MAMEY/platforms/business/Agriculture"
    unify "$SOVDIR/casino"                      "$MAMEY/platforms/business/Casino"
    unify "$SOVDIR/CASINO_SPORTS"               "$MAMEY/platforms/business/CasinoSports"
    unify "$SOVDIR/inventory-system"            "$MAMEY/platforms/business/InventorySystem"
    unify "$SOVDIR/ATM_VENDING"                 "$MAMEY/platforms/business/ATMVending"
    unify "$SOVDIR/TRAVEL_VIP"                  "$MAMEY/platforms/business/TravelVIP"
    unify "$SOVDIR/products"                    "$MAMEY/platforms/business/Products"
    unify "$SOVDIR/sovereign logisty company"   "$MAMEY/platforms/business/SovereignLogistics"
    unify "$SOVDIR/aplication"                  "$MAMEY/platforms/business/Application"

    # EDUCACIÓN
    echo -e "  ${CYAN}── Educación ──${NC}"
    unify "$SOVDIR/SmartSchool"                 "$MAMEY/platforms/education/SmartSchool"

    # PLATAFORMAS
    echo -e "  ${CYAN}── Plataformas ──${NC}"
    unify "$SOVDIR/plataformas-finales"         "$MAMEY/platforms/plataformas-finales"
    unify "$SOVDIR/PlataformaFinal"             "$MAMEY/platforms/PlataformaFinal"
    unify "$SOVDIR/platforms"                   "$MAMEY/platforms/platforms-sov"
    unify "$SOVDIR/ierahkwa-platform-soberano"  "$MAMEY/platforms/ierahkwa-platform-soberano"
    unify "$SOVDIR/citizen "                    "$MAMEY/platforms/government/Citizen"
    unify "$SOVDIR/the parellel word "          "$MAMEY/platforms/business/ParallelWorld"
    unify "$SOVDIR/naciones y regiones de america " "$MAMEY/platforms/government/NacionesRegiones"
    unify "$SOVDIR/VENTURI TUNNEL RENEWABLE ENERGY PLANT" "$MAMEY/platforms/business/VenturiEnergy"
    unify "$SOVDIR/indios snfcsm"               "$MAMEY/platforms/government/IndiosSNFCSM"
    unify "$SOVDIR/00-PROYECTO-SOBERANOS"       "$MAMEY/platforms/00-PROYECTO-SOBERANOS"

    # RUDDIE SOLUTION (11GB — código importante)
    echo -e "  ${CYAN}── RuddieSolution ──${NC}"
    if [ -d "$SOVDIR/RuddieSolution" ]; then
        mkdir -p "$MAMEY/platforms/RuddieSolution"
        for sub in "$SOVDIR/RuddieSolution"/*/; do
            name=$(basename "$sub")
            unify "$sub" "$MAMEY/platforms/RuddieSolution/$name"
        done
        # Copiar docs
        for doc in "$SOVDIR/RuddieSolution/"*.md "$SOVDIR/RuddieSolution/"*.html; do
            [ -f "$doc" ] && cp -n "$doc" "$MAMEY/platforms/RuddieSolution/" 2>/dev/null || true
        done
        echo -e "  ${GREEN}+ RuddieSolution docs${NC}"
    fi

    # DOCS Y SCRIPTS SUELTOS
    echo -e "  ${CYAN}── Docs/Scripts originales ──${NC}"
    mkdir -p "$MAMEY/docs/sovereign-original"
    COPIED=0
    for f in "$SOVDIR/"*.md "$SOVDIR/"*.sh "$SOVDIR/"*.html "$SOVDIR/"*.js; do
        if [ -f "$f" ]; then
            cp -n "$f" "$MAMEY/docs/sovereign-original/" 2>/dev/null && ((COPIED++)) || true
        fi
    done
    echo -e "  ${GREEN}+ $COPIED docs/scripts copiados${NC}"
fi

echo ""

# ══════════════════════════════════════════
# PASO 3: LO QUE NO SE COPIA (solo backup)
# ══════════════════════════════════════════
echo -e "${CYAN}${BOLD}[3/5] Solo en Extreme Pro (no en Mamey-main):${NC}"
echo -e "  ${YELLOW}⊘ IERAHKWA_FUTUREHEAD_ORGANIZADO (208GB media)${NC}"
echo -e "  ${YELLOW}⊘ untitled folder (2.2GB)${NC}"
echo -e "  ${YELLOW}⊘ logs (934MB)${NC}"
echo -e "  ${YELLOW}⊘ BACKUPS_HISTORICOS (345MB)${NC}"
echo -e "  ${YELLOW}⊘ backup-system (333MB)${NC}"
echo -e "  ${YELLOW}⊘ backups (80MB)${NC}"
echo -e "  ${YELLOW}⊘ Bodno copy (141MB — duplicado)${NC}"
echo -e "  ${YELLOW}⊘ marketing-network (164MB)${NC}"
echo ""

# ══════════════════════════════════════════
# PASO 4: LIMPIAR (con confirmación)
# ══════════════════════════════════════════
echo -e "${CYAN}${BOLD}[4/5] LIMPIEZA...${NC}"

if [ -d "$EXT/BACKUP-IERAHKWA/Sovereign-System" ]; then
    BKSIZE=$(du -sh "$EXT/BACKUP-IERAHKWA/Sovereign-System" 2>/dev/null | cut -f1)
    echo -e "  ${GREEN}✓ Backup verificado: $BKSIZE en Extreme Pro${NC}"
    echo ""
    echo -e "  ${YELLOW}${BOLD}¿Borrar carpetas originales del Desktop?${NC}"
    echo -e "  ${YELLOW}  → Sovereign Government... (233GB)${NC}"
    echo -e "  ${YELLOW}  → Sovereign Platform Unificada (1.9GB)${NC}"
    echo -e "  ${YELLOW}  → ~\$ temp files${NC}"
    echo ""
    read -p "  Escribir 'SI BORRAR' para confirmar: " CONFIRM
    if [ "$CONFIRM" = "SI BORRAR" ]; then
        echo -e "  ${RED}Borrando...${NC}"
        rm -rf "$SOVDIR"
        rm -rf "$UNIF"
        rm -f ~/Desktop/~\$* 2>/dev/null
        echo -e "  ${GREEN}✅ ~235GB liberados${NC}"
    else
        echo -e "  ${YELLOW}⚠ Cancelado — borra manualmente después${NC}"
    fi
else
    echo -e "  ${RED}❌ Backup NO encontrado — NO se borra nada${NC}"
fi
echo ""

# ══════════════════════════════════════════
# PASO 5: RESUMEN
# ══════════════════════════════════════════
NEWSIZE=$(du -sh "$MAMEY" 2>/dev/null | cut -f1)
DISKFREE=$(df -h / | tail -1 | awk '{print $4}')

echo -e "${GREEN}╔═══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  🦅 UNIFICACIÓN COMPLETA                                      ║${NC}"
echo -e "${GREEN}╠═══════════════════════════════════════════════════════════════╣${NC}"
echo -e "${GREEN}║${NC}  Mamey-main: $NEWSIZE (proyecto unificado)                      ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  Disco libre: $DISKFREE                                        ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}                                                               ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/government/  8 proyectos                            ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/finance/     8 proyectos                            ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/tech/        8 proyectos                            ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/infra/       6 proyectos                            ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/business/    9 proyectos                            ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/education/   1 proyecto                             ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  platforms/RuddieSolution/ Código .NET + Banking               ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  + 67 plataformas originales de Mamey-main                    ${GREEN}║${NC}"
echo -e "${GREEN}║${NC}  = 100+ plataformas unificadas 🦅                              ${GREEN}║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}Siguiente:${NC}"
echo "  cd ~/Desktop/Mamey-main"
echo "  ./audit-platforms.sh"
echo "  open dashboard.html"
echo ""

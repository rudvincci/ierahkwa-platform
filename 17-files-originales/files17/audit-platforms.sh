#!/bin/bash
#
# 🔍 AUDITORÍA DE LAS 67+ PLATAFORMAS — Mamey-main
# Corre desde ~/Desktop/Mamey-main
#
set -euo pipefail

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$DIR"

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'
OK=0
WARN=0
FAIL=0

check_ok() { echo -e "  ${GREEN}✓${NC} $1"; ((OK++)); }
check_warn() { echo -e "  ${YELLOW}⚠${NC} $1"; ((WARN++)); }
check_fail() { echo -e "  ${RED}✗${NC} $1"; ((FAIL++)); }

echo ""
echo -e "${CYAN}╔══════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║  🔍 AUDITORÍA COMPLETA — 67+ PLATAFORMAS            ║${NC}"
echo -e "${CYAN}║  Mamey-main · $(date '+%Y-%m-%d %H:%M')                          ║${NC}"
echo -e "${CYAN}╚══════════════════════════════════════════════════════╝${NC}"
echo ""

# ── 1. BLOCKCHAIN NODES ──
echo -e "${CYAN}⛓️  BLOCKCHAIN / NODES${NC}"
for d in MameyNode MameyNode.Rust MameyNode.Go MameyNode.JavaScript MameyNode.TypeScript MameyNode.Python MameyFutureNode MameyFutureNode.Protos; do
    if [ -d "$d" ]; then
        files=$(find "$d" -type f -not -path "*/node_modules/*" -not -path "*/obj/*" -not -path "*/bin/*" 2>/dev/null | wc -l | tr -d ' ')
        check_ok "$d ($files files)"
    else
        check_fail "$d — MISSING"
    fi
done
[ -d "Mamey.MameyFutureNode.Clients" ] && check_ok "MameyFutureNode.Clients" || check_fail "MameyFutureNode.Clients — MISSING"
[ -d "mamey-contracts" ] && check_ok "mamey-contracts (Solidity)" || check_warn "mamey-contracts — not found"
[ -d "node/wasm-runtime" ] && check_ok "node/wasm-runtime (WASM)" || check_warn "node/wasm-runtime — not found"
echo ""

# ── 2. GOBIERNO ──
echo -e "${CYAN}🏛️  GOBIERNO${NC}"
for d in "Government/INKG.CitizenPortal.ApiGataway" "Government/Mamey.Inkg.Blazor" "Mamey.Government" "Mamey.Government.Identity"; do
    if [ -d "$d" ]; then
        files=$(find "$d" -type f -not -path "*/obj/*" -not -path "*/bin/*" 2>/dev/null | wc -l | tr -d ' ')
        check_ok "$(basename "$d") ($files files)"
    else
        check_fail "$(basename "$d") — MISSING"
    fi
done
echo ""

# ── 3. FINANZAS ──
echo -e "${CYAN}💰 FINANZAS${NC}"
[ -d "FutureWampum" ] && check_ok "FutureWampum" || check_fail "FutureWampum — MISSING"
[ -d "FutureWampum/FutureWampumId" ] && check_ok "FutureWampumId" || check_fail "FutureWampumId — MISSING"
[ -d "FutureWampum/FutureWampumId/Mamey.FWID.Identities" ] && check_ok "Mamey.FWID.Identities ✅ SUBMODULE OK" || check_fail "Mamey.FWID.Identities — SUBMODULE MISSING"
echo ""

# ── 4. AI ──
echo -e "${CYAN}🤖 AI${NC}"
[ -d "MameyFutureAI" ] && check_ok "MameyFutureAI" || check_fail "MameyFutureAI — MISSING"
echo ""

# ── 5. UI / PORTALES ──
echo -e "${CYAN}🖥️  UI / PORTALES${NC}"
for d in MameyNode.UI MameyNode.Portals Mamey.MameyNode.Portal Mamey.Info; do
    if [ -d "$d" ]; then
        files=$(find "$d" -type f -not -path "*/node_modules/*" -not -path "*/obj/*" 2>/dev/null | wc -l | tr -d ' ')
        check_ok "$d ($files files)"
    else
        check_fail "$d — MISSING"
    fi
done
echo ""

# ── 6. TOOLS ──
echo -e "${CYAN}🔧 TOOLS / UTILITIES${NC}"
for d in MameyForge Pupitre "Utilities/BaGetter" "Utilities/bookstack" "Utilities/Mamey.TemplateEngine" "Utilities/MameyBarcode"; do
    if [ -d "$d" ]; then
        check_ok "$(basename "$d")"
    else
        check_warn "$(basename "$d") — not found"
    fi
done
echo ""

# ── 7. DEPARTMENTS ──
echo -e "${CYAN}📋 DEPARTAMENTOS (scrum-master-plans1)${NC}"
for d in Banks Contracts FutureWampum Government GovernmentCore HolisticMedicine MameyFutureAI MameyFutureNode MameyNode MameyNode.EnergyUtilities Pupitre RedWebNetwork; do
    if [ -d "scrum-master-plans1/$d" ]; then
        check_ok "$d"
    else
        check_warn "$d — not found"
    fi
done
echo ""

# ── 8. SPECIAL ──
echo -e "${CYAN}🦅 ESPECIALES${NC}"
[ -d "_aSoul" ] && check_ok "_aSoul (Cultura)" || check_warn "_aSoul — not found"
[ -d "api-contracts" ] && check_ok "api-contracts" || check_warn "api-contracts — not found"
[ -d "assets" ] && check_ok "assets/branding" || check_warn "assets — not found"
[ -d "investor" ] && check_ok "investor/" || check_warn "investor — not found"
[ -d ".designs" ] && check_ok ".designs/ (Whitepapers, UI, Wallets)" || check_warn ".designs — not found"
[ -d ".maestro" ] && check_ok ".maestro/ (Orchestrator)" || check_warn ".maestro — not found"
echo ""

# ── 9. SECURITY ──
echo -e "${CYAN}🔒 SEGURIDAD${NC}"
[ -d ".security" ] && check_ok ".security/ directory" || check_warn ".security/ — run install-security.sh"
[ -f ".security/certs/sovereign.crt" ] && check_ok "TLS certs" || check_warn "TLS certs — not generated"
[ -f ".security/auth/jwt-secret.key" ] && check_ok "JWT secret" || check_warn "JWT secret — not generated"
[ -f ".security/auth/api-keys.env" ] && check_ok "API keys" || check_warn "API keys — not generated"
[ -f ".env" ] && check_ok ".env config" || check_warn ".env — not created"

if grep -q "MAMEY SECURITY" .gitignore 2>/dev/null; then
    check_ok ".gitignore protects secrets"
else
    check_fail ".gitignore NOT protecting secrets!"
fi

# Check for 0.0.0.0 exposure
if grep -rn "0\.0\.0\.0" scripts/ *.sh 2>/dev/null | grep -v ".git" | grep -q "0.0.0.0"; then
    check_fail "⚠ CRITICAL: Services binding to 0.0.0.0 — EXPOSED!"
else
    check_ok "No 0.0.0.0 exposure detected"
fi

echo ""

# ── 10. CODE COUNT ──
echo -e "${CYAN}📊 CÓDIGO${NC}"
CS=$(find . -name "*.cs" -not -path "*/obj/*" -not -path "*/bin/*" -type f 2>/dev/null | wc -l | tr -d ' ')
RS=$(find . -name "*.rs" -type f 2>/dev/null | wc -l | tr -d ' ')
GO=$(find . -name "*.go" -type f 2>/dev/null | wc -l | tr -d ' ')
JS=$(find . -name "*.js" -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' ')
TS=$(find . -name "*.ts" -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' ')
PY=$(find . -name "*.py" -type f 2>/dev/null | wc -l | tr -d ' ')
SOL=$(find . -name "*.sol" -type f 2>/dev/null | wc -l | tr -d ' ')
SH=$(find . -name "*.sh" -type f 2>/dev/null | wc -l | tr -d ' ')
HTML=$(find . -name "*.html" -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' ')
MD=$(find . -name "*.md" -type f 2>/dev/null | wc -l | tr -d ' ')
CSPROJ=$(find . -name "*.csproj" -type f 2>/dev/null | wc -l | tr -d ' ')
CARGO=$(find . -name "Cargo.toml" -type f 2>/dev/null | wc -l | tr -d ' ')

echo -e "  C# (.cs):         $CS files"
echo -e "  Rust (.rs):        $RS files"
echo -e "  Go (.go):          $GO files"
echo -e "  JavaScript (.js):  $JS files"
echo -e "  TypeScript (.ts):  $TS files"
echo -e "  Python (.py):      $PY files"
echo -e "  Solidity (.sol):   $SOL files"
echo -e "  Shell (.sh):       $SH files"
echo -e "  HTML (.html):      $HTML files"
echo -e "  Markdown (.md):    $MD files"
echo -e "  .NET projects:     $CSPROJ .csproj"
echo -e "  Rust projects:     $CARGO Cargo.toml"
TOTAL=$((CS + RS + GO + JS + TS + PY + SOL + SH + HTML))
echo -e "  ${CYAN}TOTAL CODE FILES:   $TOTAL${NC}"
echo ""

# ── 11. DISK ──
echo -e "${CYAN}💾 DISCO${NC}"
SIZE=$(du -sh . 2>/dev/null | cut -f1)
echo -e "  Mamey-main: $SIZE"
df -h / | tail -1 | awk '{print "  Disco: " $2 " total, " $4 " libre (" $5 " usado)"}'
echo ""

# ── SUMMARY ──
echo -e "${CYAN}══════════════════════════════════════════════════════${NC}"
echo -e "  ${GREEN}✓ OK: $OK${NC}  ${YELLOW}⚠ WARN: $WARN${NC}  ${RED}✗ FAIL: $FAIL${NC}"
echo -e "${CYAN}══════════════════════════════════════════════════════${NC}"
echo ""

if [ "$FAIL" -eq 0 ]; then
    echo -e "${GREEN}🦅 PLATAFORMA SOBERANA LISTA${NC}"
else
    echo -e "${YELLOW}⚠ Hay $FAIL problemas que resolver${NC}"
fi
echo ""

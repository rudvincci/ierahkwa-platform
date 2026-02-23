#!/usr/bin/env bash
# ──────────────────────────────────────────────────────────────────────────────
# MameyForge CI Check Script
#
# Validates that all templates and examples have the expected file structure.
# This is a placeholder — future iterations will compile WASM artifacts.
# ──────────────────────────────────────────────────────────────────────────────

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

ERRORS=0

check_file() {
    local file="$1"
    if [[ -f "$file" ]]; then
        printf "${GREEN}  ✓${NC} %s\n" "$file"
    else
        printf "${RED}  ✗${NC} %s (MISSING)\n" "$file"
        ((ERRORS++))
    fi
}

check_dir() {
    local dir="$1"
    if [[ -d "$dir" ]]; then
        printf "${GREEN}  ✓${NC} %s/\n" "$dir"
    else
        printf "${RED}  ✗${NC} %s/ (MISSING)\n" "$dir"
        ((ERRORS++))
    fi
}

echo "═══════════════════════════════════════════════════════════"
echo "  MameyForge CI Check"
echo "═══════════════════════════════════════════════════════════"
echo ""

# ── Templates ────────────────────────────────────────────────────────────────

echo "📦 Templates"
echo ""

for template in basic token nft; do
    echo "  Template: ${YELLOW}${template}${NC}"
    TDIR="$ROOT_DIR/templates/$template"
    check_dir "$TDIR"
    check_file "$TDIR/Cargo.toml.template"
    check_file "$TDIR/README.md.template"
    check_file "$TDIR/mameyforge.toml.template"
    check_file "$TDIR/src/lib.rs.template"
    echo ""
done

# ── Standalone Reference Files ───────────────────────────────────────────────

echo "📄 Standalone Reference Files"
echo ""

for template in basic token; do
    echo "  Template: ${YELLOW}${template}${NC}"
    TDIR="$ROOT_DIR/templates/$template"
    check_file "$TDIR/contract.rs"
    check_file "$TDIR/Cargo.toml"
    check_file "$TDIR/README.md"
    echo ""
done

# ── Examples ─────────────────────────────────────────────────────────────────

echo "🔧 Examples"
echo ""

for example in hello-world erc20-token nft counter escrow; do
    EDIR="$ROOT_DIR/examples/$example"
    if [[ -d "$EDIR" ]]; then
        echo "  Example: ${YELLOW}${example}${NC}"
        check_file "$EDIR/Cargo.toml"
        check_file "$EDIR/README.md"
        check_file "$EDIR/src/lib.rs"
        echo ""
    fi
done

# ── Documentation ────────────────────────────────────────────────────────────

echo "📚 Documentation"
echo ""

for doc in getting-started.md contract-api.md testing.md deployment.md commands.md configuration.md project-structure.md templates.md; do
    check_file "$ROOT_DIR/docs/$doc"
done
echo ""

# ── Summary ──────────────────────────────────────────────────────────────────

echo "═══════════════════════════════════════════════════════════"
if [[ $ERRORS -eq 0 ]]; then
    printf "${GREEN}  ✓ All checks passed!${NC}\n"
    exit 0
else
    printf "${RED}  ✗ ${ERRORS} check(s) failed.${NC}\n"
    exit 1
fi

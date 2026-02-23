#!/usr/bin/env bash
# MameyForge CI — Build verification for templates and examples
#
# Checks:
#   1. MameyForge itself compiles
#   2. Template .template files have valid Rust (after placeholder substitution)
#   3. All examples compile
#   4. Clippy passes on examples
#
# Usage:
#   ./scripts/ci.sh          # run all checks
#   ./scripts/ci.sh --quick  # skip clippy

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
QUICK=false

if [[ "${1:-}" == "--quick" ]]; then
    QUICK=true
fi

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

pass() { echo -e "${GREEN}✓${NC} $1"; }
fail() { echo -e "${RED}✗${NC} $1"; }
info() { echo -e "${CYAN}→${NC} $1"; }

ERRORS=0

# ── 1. Build MameyForge CLI ─────────────────────────────────────────────────

info "Building MameyForge CLI..."
if cargo check --manifest-path "$ROOT_DIR/Cargo.toml" 2>&1; then
    pass "MameyForge CLI compiles"
else
    fail "MameyForge CLI failed to compile"
    ERRORS=$((ERRORS + 1))
fi

# ── 2. Validate templates ───────────────────────────────────────────────────
# Templates have .template extension and {{placeholders}}.
# We substitute placeholders and try to parse the Rust files.

TEMPLATES_DIR="$ROOT_DIR/templates"
if [[ -d "$TEMPLATES_DIR" ]]; then
    info "Validating templates..."

    TMPDIR_TEMPL=$(mktemp -d)
    trap 'rm -rf "$TMPDIR_TEMPL"' EXIT

    for template_name in "$TEMPLATES_DIR"/*/; do
        tname=$(basename "$template_name")
        info "  Template: $tname"

        # Find all .template files and do placeholder substitution
        find "$template_name" -name '*.template' -type f | while read -r tfile; do
            rel="${tfile#$template_name}"
            dest="$TMPDIR_TEMPL/$tname/${rel%.template}"
            mkdir -p "$(dirname "$dest")"
            sed \
                -e 's/{{project_name}}/ci_test_project/g' \
                -e 's/{{version}}/0.1.0/g' \
                -e 's/{{author}}/CI/g' \
                "$tfile" > "$dest"
        done

        # Check that lib.rs has valid syntax (basic parse check)
        lib_rs="$TMPDIR_TEMPL/$tname/src/lib.rs"
        if [[ -f "$lib_rs" ]]; then
            # We can't fully compile without dependencies, but we can check
            # that the file is not obviously broken
            if grep -q 'wasm_bindgen' "$lib_rs" && grep -q 'pub fn' "$lib_rs"; then
                pass "  $tname/src/lib.rs has expected structure"
            else
                fail "  $tname/src/lib.rs missing expected markers"
                ERRORS=$((ERRORS + 1))
            fi
        fi

        # Check Cargo.toml was generated
        cargo_toml="$TMPDIR_TEMPL/$tname/Cargo.toml"
        if [[ -f "$cargo_toml" ]]; then
            if grep -q 'ci_test_project' "$cargo_toml"; then
                pass "  $tname/Cargo.toml placeholder substitution works"
            else
                fail "  $tname/Cargo.toml placeholder substitution failed"
                ERRORS=$((ERRORS + 1))
            fi
        fi
    done
else
    info "No templates directory found — skipping template validation"
fi

# ── 3. Build examples ───────────────────────────────────────────────────────

EXAMPLES_DIR="$ROOT_DIR/examples"
if [[ -d "$EXAMPLES_DIR" ]]; then
    info "Building examples..."

    for example_dir in "$EXAMPLES_DIR"/*/; do
        ename=$(basename "$example_dir")
        cargo_toml="$example_dir/Cargo.toml"

        if [[ ! -f "$cargo_toml" ]]; then
            info "  Skipping $ename (no Cargo.toml)"
            continue
        fi

        info "  Example: $ename"

        if cargo check --manifest-path "$cargo_toml" 2>&1; then
            pass "  $ename compiles"
        else
            fail "  $ename failed to compile"
            ERRORS=$((ERRORS + 1))
        fi
    done
else
    info "No examples directory found — skipping examples"
fi

# ── 4. Clippy on examples ───────────────────────────────────────────────────

if [[ "$QUICK" == false && -d "$EXAMPLES_DIR" ]]; then
    info "Running clippy on examples..."

    for example_dir in "$EXAMPLES_DIR"/*/; do
        ename=$(basename "$example_dir")
        cargo_toml="$example_dir/Cargo.toml"

        if [[ ! -f "$cargo_toml" ]]; then
            continue
        fi

        if cargo clippy --manifest-path "$cargo_toml" -- -D warnings 2>&1; then
            pass "  $ename clippy clean"
        else
            fail "  $ename has clippy warnings"
            ERRORS=$((ERRORS + 1))
        fi
    done
fi

# ── Summary ──────────────────────────────────────────────────────────────────

echo
if [[ $ERRORS -eq 0 ]]; then
    pass "All CI checks passed!"
    exit 0
else
    fail "$ERRORS check(s) failed"
    exit 1
fi

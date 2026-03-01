#!/usr/bin/env bash
# ============================================================
#  fix-aria-labels.sh
#  Adds missing aria-label attributes to Ierahkwa platform
#  HTML files for accessibility compliance.
#
#  Targets:
#    1. First <section tag without aria-label
#       -> adds aria-label="Contenido principal"
#    2. <div class="dash" or <div class="dashboard" without aria-label
#       -> adds aria-label="Panel de control"
#
#  Usage:  bash scripts/fix-aria-labels.sh
#  Dry-run: DRY_RUN=1 bash scripts/fix-aria-labels.sh
# ============================================================

set -euo pipefail

BASE_DIR="$(cd "$(dirname "$0")/../02-plataformas-html" && pwd)"
FIXED_COUNT=0
SECTION_FIXES=0
DASH_FIXES=0
SKIPPED_COUNT=0

echo "============================================"
echo "  Ierahkwa — fix-aria-labels.sh"
echo "  Scanning: $BASE_DIR"
echo "============================================"
echo ""

for dir in "$BASE_DIR"/*/; do
    # Skip the shared directory
    dirname="$(basename "$dir")"
    if [[ "$dirname" == "shared" ]]; then
        continue
    fi

    file="$dir/index.html"

    # Skip if no index.html exists
    if [[ ! -f "$file" ]]; then
        continue
    fi

    # Skip files that already have aria-label
    if grep -q 'aria-label' "$file"; then
        SKIPPED_COUNT=$((SKIPPED_COUNT + 1))
        continue
    fi

    file_modified=false

    # --- Fix 1: Add aria-label to the first <section without one ---
    # Uses awk to only modify the FIRST <section tag (macOS compatible).
    if grep -q '<section' "$file"; then
        if [[ -z "${DRY_RUN:-}" ]]; then
            awk '
                BEGIN { done=0 }
                {
                    if (done == 0 && /<section/ && !/aria-label/) {
                        sub(/<section/, "<section aria-label=\"Contenido principal\"")
                        done = 1
                    }
                    print
                }
            ' "$file" > "${file}.tmp" && mv "${file}.tmp" "$file"
        fi
        SECTION_FIXES=$((SECTION_FIXES + 1))
        file_modified=true
    fi

    # --- Fix 2: Add aria-label to <div class="dash" or <div class="dashboard" ---
    if grep -qE '<div class="dash(board)?"' "$file"; then
        if [[ -z "${DRY_RUN:-}" ]]; then
            # Add aria-label to all matching dash/dashboard divs
            sed -i '' 's/<div class="dash"/<div class="dash" aria-label="Panel de control"/g; s/<div class="dashboard"/<div class="dashboard" aria-label="Panel de control"/g' "$file"
        fi
        DASH_FIXES=$((DASH_FIXES + 1))
        file_modified=true
    fi

    if [[ "$file_modified" == true ]]; then
        FIXED_COUNT=$((FIXED_COUNT + 1))
        echo "[FIXED] $dirname/index.html"
    fi
done

echo ""
echo "============================================"
echo "  Results"
echo "============================================"
echo "  Files already OK (skipped):  $SKIPPED_COUNT"
echo "  Files fixed:                 $FIXED_COUNT"
echo "    - <section> aria-label:    $SECTION_FIXES"
echo "    - dashboard aria-label:    $DASH_FIXES"
echo "  Total processed:             $((SKIPPED_COUNT + FIXED_COUNT))"
echo "============================================"

if [[ -n "${DRY_RUN:-}" ]]; then
    echo ""
    echo "  ** DRY RUN — no files were modified **"
fi

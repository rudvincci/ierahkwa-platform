#!/bin/bash
# ============================================
# Ierahkwa Futurehead Migration Script
# Migrates inline-CSS platforms to shared ierahkwa.css v3.5.0
# ============================================

BASE_DIR="$(cd "$(dirname "$0")/.." && pwd)"
SHARED_CSS='<link rel="stylesheet" href="../shared/ierahkwa.css">'
MIGRATED=0
SKIPPED=0
ERRORS=0

# Directories to SKIP (already use shared CSS, are special, or should not be touched)
SKIP_DIRS="shared commerce-business-dashboard admin-dashboard portal-central"

echo "================================================"
echo "  Ierahkwa Futurehead Migration"
echo "  Base: $BASE_DIR"
echo "================================================"
echo ""

for dir in "$BASE_DIR"/*/; do
    dirname=$(basename "$dir")
    htmlfile="$dir/index.html"

    # Skip if no index.html
    if [ ! -f "$htmlfile" ]; then
        continue
    fi

    # Skip special directories
    skip=false
    for s in $SKIP_DIRS; do
        if [ "$dirname" = "$s" ]; then
            skip=true
            break
        fi
    done
    if $skip; then
        echo "  SKIP (special): $dirname"
        SKIPPED=$((SKIPPED + 1))
        continue
    fi

    # Skip if already imports ierahkwa.css
    if grep -q 'ierahkwa\.css' "$htmlfile"; then
        echo "  SKIP (has CSS): $dirname"
        SKIPPED=$((SKIPPED + 1))
        continue
    fi

    # Extract the --accent color from inline CSS
    # Pattern: --accent:#XXXXXX or --accent: #XXXXXX
    ACCENT=$(grep -o '\-\-accent:[[:space:]]*#[0-9a-fA-F]*' "$htmlfile" | head -1 | sed 's/--accent:[[:space:]]*//')

    if [ -z "$ACCENT" ]; then
        # No accent found, use default
        ACCENT="#00FF41"
    fi

    # Check if file has inline <style> block
    if ! grep -q '<style>' "$htmlfile"; then
        echo "  SKIP (no style): $dirname"
        SKIPPED=$((SKIPPED + 1))
        continue
    fi

    # Create backup
    cp "$htmlfile" "$htmlfile.bak"

    # Step 1: Inject shared CSS link after <meta viewport> or after <title>
    if grep -q '<meta name="viewport"' "$htmlfile"; then
        # Insert after the viewport meta tag line
        sed -i '' 's|<meta name="viewport"[^>]*>|&\n'"$SHARED_CSS"'|' "$htmlfile"
    elif grep -q '</title>' "$htmlfile"; then
        sed -i '' 's|</title>|&\n'"$SHARED_CSS"'|' "$htmlfile"
    else
        echo "  ERROR (no insert point): $dirname"
        mv "$htmlfile.bak" "$htmlfile"
        ERRORS=$((ERRORS + 1))
        continue
    fi

    # Step 2: Replace the entire <style>...</style> block with just the accent override
    # Use perl for multi-line replacement (sed can't handle multi-line easily)
    perl -i -0pe 's|<style>.*?</style>|<style>:root{--accent:'"$ACCENT"'}</style>|s' "$htmlfile"

    # Verify the file is still valid HTML (has <html> and </html>)
    if grep -q '</html>' "$htmlfile"; then
        echo "  OK: $dirname (accent: $ACCENT)"
        rm "$htmlfile.bak"
        MIGRATED=$((MIGRATED + 1))
    else
        echo "  ERROR (broken): $dirname — restoring backup"
        mv "$htmlfile.bak" "$htmlfile"
        ERRORS=$((ERRORS + 1))
    fi
done

echo ""
echo "================================================"
echo "  Migration Complete"
echo "  Migrated: $MIGRATED"
echo "  Skipped:  $SKIPPED"
echo "  Errors:   $ERRORS"
echo "================================================"

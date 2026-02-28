#!/bin/bash
# ============================================
# Ierahkwa Platform — PWA + SEO Injection Script
# Injects meta tags, manifest, SW, icons, OG tags
# into ALL platform HTML files
# v1.0.0 — 26 Feb 2026
# ============================================

BASE="/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"
DOMAIN="https://ierahkwa.nation"
MODIFIED=0
SKIPPED=0

# Skip these directories
SKIP_DIRS="shared icons screenshots admin-dashboard"

echo "=== Ierahkwa PWA + SEO Injection ==="
echo ""

for dir in "$BASE"/*/; do
    dirname=$(basename "$dir")

    # Skip non-platform directories
    skip=false
    for s in $SKIP_DIRS; do
        if [ "$dirname" = "$s" ]; then
            skip=true
            break
        fi
    done
    if $skip; then continue; fi

    file="$dir/index.html"
    if [ ! -f "$file" ]; then
        continue
    fi

    # Skip redirect stubs (< 500 bytes)
    size=$(wc -c < "$file" | tr -d ' ')
    if [ "$size" -lt 500 ]; then
        SKIPPED=$((SKIPPED + 1))
        continue
    fi

    # Extract title from existing <title> tag
    title=$(grep -o '<title>[^<]*</title>' "$file" | sed 's/<title>//;s/<\/title>//' | head -1)
    if [ -z "$title" ]; then
        title="Ierahkwa — $dirname"
    fi

    # Generate description from dirname
    pretty_name=$(echo "$dirname" | sed 's/-soberan[oa]//;s/-soberano//;s/-soberana//;s/-/ /g' | awk '{for(i=1;i<=NF;i++) $i=toupper(substr($i,1,1)) tolower(substr($i,2))}1')
    description="Plataforma soberana de ${pretty_name} para la infraestructura digital de 574 naciones tribales — Ierahkwa Ne Kanienke"

    # Determine accent color from :root{--accent:...}
    accent=$(grep -o '\-\-accent:[^;}"]*' "$file" | head -1 | sed 's/--accent://' | tr -d ' ')
    if [ -z "$accent" ]; then
        accent="#00FF41"
    fi

    # Check what's already present
    has_description=$(grep -c 'meta name="description"' "$file")
    has_manifest=$(grep -c 'rel="manifest"' "$file")
    has_theme=$(grep -c 'name="theme-color"' "$file")
    has_icon=$(grep -c 'rel="icon"' "$file")
    has_og=$(grep -c 'property="og:title"' "$file")
    has_sw=$(grep -c 'serviceWorker' "$file")
    has_canonical=$(grep -c 'rel="canonical"' "$file")

    # Fix lang="en" to lang="es"
    if grep -q 'lang="en"' "$file"; then
        sed -i '' 's/lang="en"/lang="es"/' "$file"
    fi

    # Build the meta block to inject after <meta name="viewport"...>
    META_BLOCK=""

    if [ "$has_description" -eq 0 ]; then
        META_BLOCK="${META_BLOCK}<meta name=\"description\" content=\"${description}\">\n"
    fi
    if [ "$has_theme" -eq 0 ]; then
        META_BLOCK="${META_BLOCK}<meta name=\"theme-color\" content=\"${accent}\">\n"
    fi
    if [ "$has_canonical" -eq 0 ]; then
        META_BLOCK="${META_BLOCK}<link rel=\"canonical\" href=\"${DOMAIN}/${dirname}/\">\n"
    fi
    if [ "$has_manifest" -eq 0 ]; then
        META_BLOCK="${META_BLOCK}<link rel=\"manifest\" href=\"../shared/manifest.json\">\n"
    fi
    if [ "$has_icon" -eq 0 ]; then
        META_BLOCK="${META_BLOCK}<link rel=\"icon\" href=\"../icons/icon-96.svg\" type=\"image/svg+xml\">\n<link rel=\"apple-touch-icon\" href=\"../icons/icon-192.svg\">\n"
    fi
    if [ "$has_og" -eq 0 ]; then
        META_BLOCK="${META_BLOCK}<meta property=\"og:title\" content=\"${title}\">\n<meta property=\"og:description\" content=\"${description}\">\n<meta property=\"og:type\" content=\"website\">\n<meta property=\"og:url\" content=\"${DOMAIN}/${dirname}/\">\n<meta property=\"og:image\" content=\"${DOMAIN}/icons/icon-512.svg\">\n<meta name=\"twitter:card\" content=\"summary\">\n<meta name=\"twitter:title\" content=\"${title}\">\n<meta name=\"twitter:description\" content=\"${description}\">\n"
    fi

    # Inject meta block after viewport meta tag
    if [ -n "$META_BLOCK" ]; then
        # Use perl for multiline insert (more reliable than sed on macOS)
        perl -i -pe "
            if (/viewport/ && !\$done) {
                \$_ .= \"${META_BLOCK}\";
                \$done = 1;
            }
        " "$file"
    fi

    # Inject Service Worker registration before </body> if not present
    if [ "$has_sw" -eq 0 ]; then
        SW_SCRIPT='<script>if("serviceWorker"in navigator){navigator.serviceWorker.register("../shared/sw.js").catch(function(){})}<\/script>'
        sed -i '' "s|</body>|${SW_SCRIPT}\n</body>|" "$file"
    fi

    # Remove Google Fonts external dependency if present
    if grep -q 'fonts.googleapis.com' "$file"; then
        sed -i '' '/fonts.googleapis.com/d' "$file"
        sed -i '' '/fonts.gstatic.com/d' "$file"
    fi

    MODIFIED=$((MODIFIED + 1))
    echo "  [OK] $dirname"
done

# Also fix the main index.html
MAIN="$BASE/index.html"
if [ -f "$MAIN" ]; then
    has_manifest_main=$(grep -c 'rel="manifest"' "$MAIN")
    has_theme_main=$(grep -c 'name="theme-color"' "$MAIN")
    has_sw_main=$(grep -c 'serviceWorker' "$MAIN")
    has_desc_main=$(grep -c 'meta name="description"' "$MAIN")

    MAIN_META=""
    if [ "$has_desc_main" -eq 0 ]; then
        MAIN_META="${MAIN_META}<meta name=\"description\" content=\"Portal Central de Ierahkwa Ne Kanienke — 190 plataformas soberanas, 15 dominios NEXUS, infraestructura digital para 574 naciones tribales\">\n"
    fi
    if [ "$has_theme_main" -eq 0 ]; then
        MAIN_META="${MAIN_META}<meta name=\"theme-color\" content=\"#00FF41\">\n"
    fi
    if [ "$has_manifest_main" -eq 0 ]; then
        MAIN_META="${MAIN_META}<link rel=\"manifest\" href=\"shared/manifest.json\">\n<link rel=\"icon\" href=\"icons/icon-96.svg\" type=\"image/svg+xml\">\n<link rel=\"apple-touch-icon\" href=\"icons/icon-192.svg\">\n"
    fi
    MAIN_META="${MAIN_META}<meta property=\"og:title\" content=\"Ierahkwa Ne Kanienke — Portal Central\">\n<meta property=\"og:description\" content=\"190 plataformas soberanas para 72M personas indigenas — 15 dominios NEXUS\">\n<meta property=\"og:type\" content=\"website\">\n<meta property=\"og:image\" content=\"${DOMAIN}/icons/icon-512.svg\">\n<meta name=\"twitter:card\" content=\"summary_large_image\">\n<link rel=\"canonical\" href=\"${DOMAIN}/\">\n"

    if [ -n "$MAIN_META" ]; then
        perl -i -pe "
            if (/viewport/ && !\$done) {
                \$_ .= \"${MAIN_META}\";
                \$done = 1;
            }
        " "$MAIN"
    fi

    if [ "$has_sw_main" -eq 0 ]; then
        sed -i '' 's|</body>|<script>if("serviceWorker"in navigator){navigator.serviceWorker.register("shared/sw.js").catch(function(){})}</script>\n</body>|' "$MAIN"
    fi

    # Remove external Google Fonts from main index
    if grep -q 'fonts.googleapis.com' "$MAIN"; then
        sed -i '' '/fonts.googleapis.com/d' "$MAIN"
        sed -i '' '/fonts.gstatic.com/d' "$MAIN"
    fi

    echo "  [OK] index.html (Portal Central)"
    MODIFIED=$((MODIFIED + 1))
fi

echo ""
echo "=== Completado ==="
echo "  Modificados: $MODIFIED"
echo "  Omitidos (stubs): $SKIPPED"
echo ""

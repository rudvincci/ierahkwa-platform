#!/bin/bash
# Ierahkwa Platform — PWA Injection Script v3.3.0
# Inyecta manifest.json y service worker registration en todas las plataformas HTML
# Usage: bash scripts/inject-pwa.sh

set -euo pipefail

BASE_DIR="$(cd "$(dirname "$0")/.." && pwd)"
HTML_DIR="$BASE_DIR/02-plataformas-html"
COUNT=0
SKIPPED=0

echo "═══════════════════════════════════════════════════"
echo "  Ierahkwa PWA Injection — v3.3.0"
echo "═══════════════════════════════════════════════════"
echo ""

# PWA meta tags and manifest link to inject after <head>
PWA_HEAD='<link rel="manifest" href="../shared/manifest.json" crossorigin="use-credentials">
<meta name="theme-color" content="#d4a853">
<meta name="apple-mobile-web-app-capable" content="yes">
<meta name="apple-mobile-web-app-status-bar-style" content="black-translucent">
<meta name="apple-mobile-web-app-title" content="Ierahkwa">'

# Service worker registration to inject before </body>
SW_SCRIPT='<script>
if("serviceWorker"in navigator){navigator.serviceWorker.register("../shared/sw.js",{scope:"/"}).then(r=>console.log("Ierahkwa SW registrado:",r.scope)).catch(e=>console.warn("SW error:",e))}
</script>'

for dir in "$HTML_DIR"/*/; do
  index="$dir/index.html"
  [ -f "$index" ] || continue
  dirname=$(basename "$dir")

  # Skip shared directory
  if [ "$dirname" = "shared" ]; then
    continue
  fi

  # Check if already injected
  if grep -q 'manifest.json' "$index" 2>/dev/null; then
    SKIPPED=$((SKIPPED + 1))
    continue
  fi

  # Inject manifest link after first <head> or after <meta charset>
  if grep -q '<meta charset' "$index"; then
    sed -i '' '/<meta charset/{
      a\
'"$(echo "$PWA_HEAD" | sed 's/$/\\/' | sed '$ s/\\$//')"'
    }' "$index" 2>/dev/null || true
  fi

  # Inject SW registration before </body>
  if grep -q '</body>' "$index"; then
    sed -i '' 's|</body>|'"$(echo "$SW_SCRIPT" | tr '\n' ' ')"'</body>|' "$index" 2>/dev/null || true
  fi

  COUNT=$((COUNT + 1))
  echo "  ✓ $dirname"
done

echo ""
echo "═══════════════════════════════════════════════════"
echo "  Inyectado: $COUNT plataformas"
echo "  Saltado:   $SKIPPED (ya tienen manifest)"
echo "═══════════════════════════════════════════════════"

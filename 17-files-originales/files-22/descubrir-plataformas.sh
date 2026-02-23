#!/bin/bash
#
# 🔍 DISCOVERY SCRIPT — Encuentra TODAS las plataformas en tu Desktop
# Corre esto en Terminal y mándame el resultado
#
# Uso:
#   chmod +x descubrir-plataformas.sh
#   ./descubrir-plataformas.sh
#

DESKTOP="$HOME/Desktop"
OUT="$HOME/Desktop/INVENTARIO-COMPLETO.txt"

echo "🔍 Escaneando todo..." 

{
echo "═══════════════════════════════════════════════════════════════"
echo "  INVENTARIO COMPLETO — Sovereign Platform Ierahkwa"
echo "  Generado: $(date)"
echo "  Usuario: $(whoami)"
echo "═══════════════════════════════════════════════════════════════"

# ── 1. IERAHKWA — Carpetas de primer nivel ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  1. IERAHKWA — Carpetas principales"
echo "═══════════════════════════════════════════════════════════════"
for dir in "$DESKTOP/Sovereign Government of Ierahkwa Ne Kanienke system" "$DESKTOP/Sovereign Government of Ierahkwa Ne Kanienke system "; do
  if [ -d "$dir" ]; then
    echo ""
    echo "Ruta: $dir"
    echo "───────────────────────────────────────"
    ls -1 "$dir" 2>/dev/null
    echo ""
    echo "Total carpetas: $(find "$dir" -maxdepth 1 -type d 2>/dev/null | wc -l | tr -d ' ')"
    echo "Total archivos: $(find "$dir" -maxdepth 1 -type f 2>/dev/null | wc -l | tr -d ' ')"
  fi
done

# ── 2. PLATAFORMAFINAL ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  2. PLATAFORMAFINAL — Sub-plataformas"
echo "═══════════════════════════════════════════════════════════════"
for dir in "$DESKTOP/Sovereign Government of Ierahkwa Ne Kanienke system/PlataformaFinal" "$DESKTOP/Sovereign Government of Ierahkwa Ne Kanienke system /PlataformaFinal"; do
  if [ -d "$dir" ]; then
    echo ""
    echo "Ruta: $dir"
    echo "───────────────────────────────────────"
    ls -1 "$dir" 2>/dev/null
    echo ""
    echo "Sub-carpetas (2 niveles):"
    find "$dir" -maxdepth 2 -type d 2>/dev/null | sed "s|$dir/||" | sort
  fi
done

# ── 3. AKWESASNE / PM ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  3. AKWESASNE / PM — Contenido"
echo "═══════════════════════════════════════════════════════════════"
AKWDIR="$DESKTOP/Sovereign Akwesasne Government - Office of the Prime Minister - Photos"
if [ -d "$AKWDIR" ]; then
  echo "Ruta: $AKWDIR"
  echo "───────────────────────────────────────"
  ls -1 "$AKWDIR" 2>/dev/null
  
  if [ -d "$AKWDIR/soberanos natives" ]; then
    echo ""
    echo "soberanos natives:"
    ls -1 "$AKWDIR/soberanos natives" 2>/dev/null
  fi
  
  if [ -d "$AKWDIR/platform" ]; then
    echo ""
    echo "platform:"
    ls -1 "$AKWDIR/platform" 2>/dev/null
  fi
fi

# ── 4. MAMEY ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  4. MAMEY — Framework técnico"
echo "═══════════════════════════════════════════════════════════════"
for dir in "$DESKTOP/Mamey" "$DESKTOP/Sovereign Platform Unificada/Mamey"; do
  if [ -d "$dir" ]; then
    echo "Ruta: $dir"
    echo "───────────────────────────────────────"
    find "$dir" -maxdepth 2 -type d 2>/dev/null | sed "s|$dir/||" | sort
  fi
done

# ── 5. SOVEREIGN PLATFORM UNIFICADA ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  5. SOVEREIGN PLATFORM UNIFICADA — Estructura"
echo "═══════════════════════════════════════════════════════════════"
SPDIR="$DESKTOP/Sovereign Platform Unificada"
if [ -d "$SPDIR" ]; then
  echo "Ruta: $SPDIR"
  echo "───────────────────────────────────────"
  find "$SPDIR" -maxdepth 3 -type d 2>/dev/null | sed "s|$SPDIR/||" | sort
  
  echo ""
  echo "Enlaces simbólicos:"
  find "$SPDIR" -maxdepth 2 -type l 2>/dev/null | while read f; do
    echo "  $(basename "$f") → $(readlink "$f")"
  done
fi

# ── 6. TODOS LOS SCRIPTS (.sh) ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  6. TODOS LOS SCRIPTS (.sh)"
echo "═══════════════════════════════════════════════════════════════"
find "$DESKTOP" -name "*.sh" -type f 2>/dev/null | sort

# ── 7. TODOS LOS ARCHIVOS DE CÓDIGO ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  7. ARCHIVOS DE CÓDIGO (por tipo)"
echo "═══════════════════════════════════════════════════════════════"

echo ""
echo ".cs (C# / .NET):"
find "$DESKTOP" -name "*.cs" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"
find "$DESKTOP" -name "*.cs" -type f 2>/dev/null | head -30

echo ""
echo ".rs (Rust):"
find "$DESKTOP" -name "*.rs" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"
find "$DESKTOP" -name "*.rs" -type f 2>/dev/null | head -20

echo ""
echo ".js (JavaScript):"
find "$DESKTOP" -name "*.js" -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"
find "$DESKTOP" -name "*.js" -not -path "*/node_modules/*" -type f 2>/dev/null | head -30

echo ""
echo ".ts (TypeScript):"
find "$DESKTOP" -name "*.ts" -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"

echo ""
echo ".py (Python):"
find "$DESKTOP" -name "*.py" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"

echo ""
echo ".sol (Solidity / Smart Contracts):"
find "$DESKTOP" -name "*.sol" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"
find "$DESKTOP" -name "*.sol" -type f 2>/dev/null

echo ""
echo ".html:"
find "$DESKTOP" -name "*.html" -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' '
echo " archivos"

echo ""
echo ".json (configs):"
find "$DESKTOP" -name "package.json" -not -path "*/node_modules/*" -type f 2>/dev/null | head -30

echo ""
echo ".csproj (.NET projects):"
find "$DESKTOP" -name "*.csproj" -type f 2>/dev/null

echo ""
echo "Cargo.toml (Rust projects):"
find "$DESKTOP" -name "Cargo.toml" -type f 2>/dev/null

echo ""
echo "docker-compose*.yml:"
find "$DESKTOP" -name "docker-compose*" -type f 2>/dev/null

echo ""
echo "Dockerfile:"
find "$DESKTOP" -name "Dockerfile" -type f 2>/dev/null

# ── 8. DOCUMENTOS MARKDOWN ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  8. DOCUMENTOS (.md)"
echo "═══════════════════════════════════════════════════════════════"
find "$DESKTOP" -name "*.md" -type f 2>/dev/null | sort

# ── 9. BANCOS ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  9. BANCOS — Contenido detallado"
echo "═══════════════════════════════════════════════════════════════"
for banco in "BANCO BDET" "BANCO_CENTRAL"; do
  for base in "$DESKTOP/Sovereign Government of Ierahkwa Ne Kanienke system" "$DESKTOP/Sovereign Government of Ierahkwa Ne Kanienke system "; do
    dir="$base/$banco"
    if [ -d "$dir" ]; then
      echo ""
      echo "$banco:"
      echo "───────────────────────────────────────"
      find "$dir" -maxdepth 2 -type f 2>/dev/null | sed "s|$dir/||" | sort | head -30
    fi
  done
done

# ── 10. RESUMEN FINAL ──
echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  10. RESUMEN"
echo "═══════════════════════════════════════════════════════════════"
echo ""
echo "Total carpetas en Desktop (Sovereign):"
find "$DESKTOP" -maxdepth 1 -type d -iname "*sovereign*" -o -iname "*ierahkwa*" -o -iname "*mamey*" -o -iname "*akwesasne*" 2>/dev/null

echo ""
echo "Total archivos de código:"
TOTAL_CODE=$(find "$DESKTOP" \( -name "*.cs" -o -name "*.rs" -o -name "*.js" -o -name "*.ts" -o -name "*.py" -o -name "*.sol" -o -name "*.html" -o -name "*.css" \) -not -path "*/node_modules/*" -type f 2>/dev/null | wc -l | tr -d ' ')
echo "  $TOTAL_CODE archivos"

echo ""
echo "Total scripts .sh:"
find "$DESKTOP" -name "*.sh" -type f 2>/dev/null | wc -l | tr -d ' '

echo ""
echo "Total documentos .md:"
find "$DESKTOP" -name "*.md" -type f 2>/dev/null | wc -l | tr -d ' '

echo ""
echo "Total proyectos .NET (.csproj):"
find "$DESKTOP" -name "*.csproj" -type f 2>/dev/null | wc -l | tr -d ' '

echo ""
echo "Total proyectos Rust (Cargo.toml):"
find "$DESKTOP" -name "Cargo.toml" -type f 2>/dev/null | wc -l | tr -d ' '

echo ""
echo "Total Docker files:"
find "$DESKTOP" \( -name "Dockerfile" -o -name "docker-compose*" \) -type f 2>/dev/null | wc -l | tr -d ' '

echo ""
echo "Tamaño total de todo:"
du -sh "$DESKTOP"/Sovereign* "$DESKTOP"/Mamey 2>/dev/null

} > "$OUT" 2>&1

echo ""
echo "✅ Inventario guardado en: $OUT"
echo ""
echo "Ahora sube el archivo INVENTARIO-COMPLETO.txt a Claude"
echo "y te organizo todo."
echo ""

# Also open it
open "$OUT" 2>/dev/null || cat "$OUT"

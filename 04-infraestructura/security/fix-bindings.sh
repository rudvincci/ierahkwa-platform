#!/usr/bin/env bash
# fix-bindings.sh — Auditoria de enlaces 0.0.0.0 (SOLO LECTURA)
# Plataforma Soberana Ierahkwa · Chain 777777
set -euo pipefail

PROYECTO="${1:-$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)}"
echo "Buscando 0.0.0.0 en: $PROYECTO"
echo "=================================="
echo ""

FOUND=0
while IFS= read -r line; do
    file=$(echo "$line" | cut -d: -f1)
    num=$(echo "$line" | cut -d: -f2)
    content=$(echo "$line" | cut -d: -f3-)
    rel=$(echo "$file" | sed "s|$PROYECTO/||")
    echo "[CRITICO] $rel:$num"
    echo "  Actual:   $(echo "$content" | sed 's/^[[:space:]]*//')"
    echo "  Corregir: $(echo "$content" | sed 's/0\.0\.0\.0/127.0.0.1/g' | sed 's/^[[:space:]]*//')"
    echo ""
    FOUND=$((FOUND + 1))
done < <(grep -rn --include="*.js" --include="*.cs" --include="*.yaml" --include="*.yml" \
    --include="*.json" --include="*.conf" --include="*.sh" --include="*.toml" \
    --exclude-dir=node_modules --exclude-dir=.git --exclude-dir=target \
    --exclude-dir=bin --exclude-dir=obj \
    "0\.0\.0\.0" "$PROYECTO" 2>/dev/null || true)

echo "=================================="
if [ "$FOUND" -gt 0 ]; then
    echo "ENCONTRADOS: $FOUND referencias a 0.0.0.0"
    echo "ACCION: Cambiar todas a 127.0.0.1"
else
    echo "OK: No se encontraron referencias a 0.0.0.0"
fi

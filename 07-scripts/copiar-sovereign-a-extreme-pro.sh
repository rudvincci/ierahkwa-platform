#!/bin/bash
# Copia todo Sovereign Platform a Extreme Pro
# Ejecutar: bash copiar-sovereign-a-extreme-pro.sh

DEST="/Volumes/Extreme Pro/Sovereign-Platform-Completo"
mkdir -p "$DEST"

echo "=== 1/4 Akwesasne ==="
rsync -av "/Users/ruddie/Desktop/Sovereign Akwesasne Government - Office of the Prime Minister - Photos" "$DEST/"

echo "=== 2/4 Ierahkwa ==="
rsync -av "/Users/ruddie/Desktop/Sovereign Government of Ierahkwa Ne Kanienke system " "$DEST/"

echo "=== 3/4 Mamey ==="
rsync -av "/Users/ruddie/Desktop/Mamey-main" "$DEST/"

echo "=== 4/4 Sovereign Platform Unificada (docs + estructura) ==="
rsync -av "/Users/ruddie/Desktop/Sovereign Platform Unificada" "$DEST/"

echo "=== LISTO ==="
du -sh "$DEST"

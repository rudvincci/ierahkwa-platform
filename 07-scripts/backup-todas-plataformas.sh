#!/bin/bash
# ═══════════════════════════════════════════════════════════════════════════════
#  🔒 BACKUP TODAS LAS PLATAFORMAS
#  Protege todo lo que hemos hecho — Software2026, Inkg FrontEnd, Inkg BackOffice
# ═══════════════════════════════════════════════════════════════════════════════

set -e

# Destino: una sola carpeta para todos los backups de plataformas
BACKUP_ROOT="${BACKUP_ROOT:-/Users/ruddie/Documents/Software2026/BACKUPS-PLATAFORMAS}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="$BACKUP_ROOT/backup_$TIMESTAMP"
LOG_FILE="$BACKUP_ROOT/backup_log.txt"

# Rutas de cada plataforma
SOFTWARE2026="/Users/ruddie/Documents/Software2026"
INKG_FRONT="/Users/ruddie/Desktop/Inkg-FrontEnd-main"
INKG_BACK="/Users/ruddie/Desktop/Inkg-BackOffice-main"

# Excluir para reducir tamaño y tiempo (sin node_modules, .git, bin/obj, etc.)
TAR_EXCLUDES=(
  --exclude='node_modules'
  --exclude='.git'
  --exclude='.next'
  --exclude='.nuxt'
  --exclude='dist'
  --exclude='build'
  --exclude='bin'
  --exclude='obj'
  --exclude='*.db'
  --exclude='*.db-shm'
  --exclude='*.db-wal'
  --exclude='*.cache'
  --exclude='.turbo'
  --exclude='.pnpm-store'
  --exclude='coverage'
  --exclude='.nyc_output'
  --exclude='*.log'
  --exclude='.DS_Store'
)

# Para Software2026: además excluir carpetas de backups previas
SOFTWARE2026_EXCLUDES=(
  "${TAR_EXCLUDES[@]}"
  --exclude='BACKUPS-PLATAFORMAS'
  --exclude='backup'
  --exclude='backup-2026-01-18'
  --exclude='backups'
  --exclude='RuddieSolution/backup-system/backups'
)

mkdir -p "$BACKUP_DIR"

# Opción: solo Inkg (rápido, sin Software2026)
SOLO_INKG="${1:-}"

log() {
  local msg="[$(date '+%Y-%m-%d %H:%M:%S')] $1"
  echo "$msg" | tee -a "$LOG_FILE"
}

log "══════════════════════════════════════════════════════════════════════════"
log "🔒 BACKUP TODAS LAS PLATAFORMAS — Inicio"
log "   Destino: $BACKUP_DIR"
log "══════════════════════════════════════════════════════════════════════════"

# ─── 1. Software2026 (omitir si: ./script solo-inkg) ───────────────────
if [ "$SOLO_INKG" != "solo-inkg" ] && [ -d "$SOFTWARE2026" ]; then
  log "📦 1/3 Software2026..."
  OUT="$BACKUP_DIR/Software2026_$TIMESTAMP.tar.gz"
  (cd "$(dirname "$SOFTWARE2026")" && \
   tar -czf "$OUT" "${SOFTWARE2026_EXCLUDES[@]}" "$(basename "$SOFTWARE2026")" 2>/dev/null) || true
  if [ -f "$OUT" ]; then
    log "   ✅ $(basename "$OUT") — $(du -h "$OUT" | cut -f1)"
  else
    log "   ⚠️ No se pudo crear backup de Software2026"
  fi
elif [ "$SOLO_INKG" = "solo-inkg" ]; then
  log "⏭️  Omitiendo Software2026 (solo-inkg)"
fi

# ─── 2. Inkg-FrontEnd-main ───────────────────────────────────────────────
if [ -d "$INKG_FRONT" ]; then
  log "📦 2/3 Inkg-FrontEnd-main..."
  OUT="$BACKUP_DIR/Inkg-FrontEnd_$TIMESTAMP.tar.gz"
  (cd "$(dirname "$INKG_FRONT")" && \
   tar -czf "$OUT" "${TAR_EXCLUDES[@]}" "$(basename "$INKG_FRONT")" 2>/dev/null) || true
  if [ -f "$OUT" ]; then
    log "   ✅ $(basename "$OUT") — $(du -h "$OUT" | cut -f1)"
  else
    log "   ⚠️ No se pudo crear backup de Inkg-FrontEnd"
  fi
else
  log "   ⚠️ No existe: $INKG_FRONT"
fi

# ─── 3. Inkg-BackOffice-main ─────────────────────────────────────────────
if [ -d "$INKG_BACK" ]; then
  log "📦 3/3 Inkg-BackOffice-main..."
  OUT="$BACKUP_DIR/Inkg-BackOffice_$TIMESTAMP.tar.gz"
  (cd "$(dirname "$INKG_BACK")" && \
   tar -czf "$OUT" "${TAR_EXCLUDES[@]}" "$(basename "$INKG_BACK")" 2>/dev/null) || true
  if [ -f "$OUT" ]; then
    log "   ✅ $(basename "$OUT") — $(du -h "$OUT" | cut -f1)"
  else
    log "   ⚠️ No se pudo crear backup de Inkg-BackOffice"
  fi
else
  log "   ⚠️ No existe: $INKG_BACK"
fi

# ─── Resumen en carpeta del backup ───────────────────────────────────────
MANIFEST="$BACKUP_DIR/MANIFIESTO.txt"
{
  echo "══════════════════════════════════════════════════════════════════════════"
  echo "🔒 BACKUP TODAS LAS PLATAFORMAS — Resumen"
  echo "   Fecha: $(date)"
  echo "   ID: backup_$TIMESTAMP"
  if [ "$SOLO_INKG" = "solo-inkg" ]; then
    echo "   Modo: solo Inkg (FrontEnd + BackOffice)"
  fi
  echo "──────────────────────────────────────────────────────────────────────────"
  echo "PLATAFORMAS INCLUIDAS:"
  [ "$SOLO_INKG" != "solo-inkg" ] && echo "  1. Software2026   — $SOFTWARE2026"
  echo "  2. Inkg-FrontEnd  — $INKG_FRONT"
  echo "  3. Inkg-BackOffice — $INKG_BACK"
  echo "──────────────────────────────────────────────────────────────────────────"
  echo "ARCHIVOS EN ESTE BACKUP:"
  ls -lh "$BACKUP_DIR" 2>/dev/null | grep -v MANIFIESTO || true
  echo "══════════════════════════════════════════════════════════════════════════"
  echo "RESTAURAR: tar -xzf <archivo.tar.gz> -C /ruta/destino"
  echo "══════════════════════════════════════════════════════════════════════════"
} > "$MANIFEST"

log "📄 Manifiesto: $MANIFEST"
log "✅ BACKUP TODAS LAS PLATAFORMAS — Completado: backup_$TIMESTAMP"
log ""

# Mantener solo los últimos 5 backups de plataformas
if [ -d "$BACKUP_ROOT" ]; then
  COUNT=$(ls -1d "$BACKUP_ROOT"/backup_* 2>/dev/null | wc -l)
  if [ "$COUNT" -gt 5 ]; then
    log "🧹 Eliminando backups antiguos (se mantienen 5)..."
    ls -1td "$BACKUP_ROOT"/backup_* 2>/dev/null | tail -n +6 | xargs rm -rf 2>/dev/null || true
    log "   Listo."
  fi
fi

echo ""
echo "╔══════════════════════════════════════════════════════════════════════════╗"
echo "║  🔒 BACKUP COMPLETADO — backup_$TIMESTAMP"
echo "║  📁 $BACKUP_DIR"
echo "╚══════════════════════════════════════════════════════════════════════════╝"

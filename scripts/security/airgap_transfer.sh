#!/bin/bash
# ============================================================================
# Ierahkwa Air-Gap Transfer — Protocolo de Bóveda Fría
# Transfiere llaves privadas a hardware desconectado (USB cifrado)
# cuando el Oráculo de Paz detecta Nivel Rojo o manualmente.
#
# Uso:
#   ./airgap_transfer.sh              # Interactivo
#   ./airgap_transfer.sh --auto       # Automático (llamado por sentinel)
#   ./airgap_transfer.sh --restore    # Restaurar desde bóveda
#
# Requisitos: gpg, shred, lsblk
# ============================================================================

set -euo pipefail

KEYS_DIR="${KEYS_DIR:-$HOME/.ierahkwa/keys}"
VAULT_LABEL="${VAULT_LABEL:-IERAHKWA_VAULT}"
VAULT_MOUNT="/mnt/vault_hardware"
PASSPHRASE_FILE="${VAULT_PASSPHRASE_FILE:-$HOME/.vault_pass}"
LOG_DIR="${LOG_DIR:-logs}"
SHRED_PASSES=7

mkdir -p "$LOG_DIR"

log() {
    local msg="[$(date -u '+%Y-%m-%d %H:%M:%S UTC')] $1"
    echo "$msg"
    echo "$msg" >> "$LOG_DIR/airgap_transfer.log"
}

die() { log "ERROR: $1"; exit 1; }

detect_vault() {
    # Buscar dispositivo USB con label IERAHKWA_VAULT
    local dev
    dev=$(lsblk -o NAME,LABEL -rn 2>/dev/null | grep "$VAULT_LABEL" | awk '{print $1}' | head -1)
    if [ -z "$dev" ]; then
        return 1
    fi
    echo "/dev/$dev"
}

mount_vault() {
    local dev="$1"
    mkdir -p "$VAULT_MOUNT"
    if mountpoint -q "$VAULT_MOUNT" 2>/dev/null; then
        log "Vault ya montada en $VAULT_MOUNT"
        return 0
    fi
    mount "$dev" "$VAULT_MOUNT" || die "No se pudo montar $dev"
    log "Vault montada: $dev -> $VAULT_MOUNT"
}

unmount_vault() {
    if mountpoint -q "$VAULT_MOUNT" 2>/dev/null; then
        sync
        umount "$VAULT_MOUNT"
        log "Vault desmontada"
    fi
}

transfer_keys() {
    if [ ! -d "$KEYS_DIR" ]; then
        die "Directorio de llaves no encontrado: $KEYS_DIR"
    fi

    local key_count
    key_count=$(find "$KEYS_DIR" -type f | wc -l)
    if [ "$key_count" -eq 0 ]; then
        die "No hay llaves en $KEYS_DIR"
    fi

    log "Transfiriendo $key_count archivo(s) a la bóveda..."

    local vault_dest="$VAULT_MOUNT/keys_$(date -u '+%Y%m%d_%H%M%S')"
    mkdir -p "$vault_dest"

    for keyfile in "$KEYS_DIR"/*; do
        [ -f "$keyfile" ] || continue
        local basename
        basename=$(basename "$keyfile")

        if [ -f "$PASSPHRASE_FILE" ]; then
            gpg --batch --yes --passphrase-file "$PASSPHRASE_FILE" \
                --symmetric --cipher-algo AES256 \
                --output "$vault_dest/${basename}.gpg" "$keyfile"
        else
            cp "$keyfile" "$vault_dest/"
            log "AVISO: Sin passphrase — copiado sin cifrar: $basename"
        fi

        # Borrado seguro del original
        shred -u -n "$SHRED_PASSES" "$keyfile"
        log "Transferido y destruido: $basename"
    done

    # Escribir manifiesto
    cat > "$vault_dest/MANIFEST.txt" <<MANIFEST
Ierahkwa Air-Gap Transfer
Fecha: $(date -u)
Archivos: $key_count
Host: $(hostname)
Cifrado: $([ -f "$PASSPHRASE_FILE" ] && echo "AES-256 (GPG)" || echo "SIN CIFRAR")
MANIFEST

    log "Transferencia completada: $vault_dest"
}

restore_keys() {
    if ! mountpoint -q "$VAULT_MOUNT" 2>/dev/null; then
        die "Vault no montada"
    fi

    local latest
    latest=$(ls -1d "$VAULT_MOUNT"/keys_* 2>/dev/null | tail -1)
    if [ -z "$latest" ]; then
        die "No hay backups en la bóveda"
    fi

    mkdir -p "$KEYS_DIR"
    log "Restaurando desde $latest..."

    for gpgfile in "$latest"/*.gpg; do
        [ -f "$gpgfile" ] || continue
        local out="$KEYS_DIR/$(basename "${gpgfile%.gpg}")"
        if [ -f "$PASSPHRASE_FILE" ]; then
            gpg --batch --yes --passphrase-file "$PASSPHRASE_FILE" \
                --decrypt --output "$out" "$gpgfile"
        else
            die "Se necesita passphrase para descifrar"
        fi
        log "Restaurado: $(basename "$out")"
    done

    log "Restauración completada"
}

# --- Main ---

MODE="${1:-}"

case "$MODE" in
    --auto)
        log "MODO AUTOMÁTICO — Air-Gap de emergencia"
        dev=$(detect_vault) || die "Bóveda USB ($VAULT_LABEL) no detectada"
        mount_vault "$dev"
        transfer_keys
        unmount_vault
        log "Air-gap completado. Desconectar USB ahora."
        ;;
    --restore)
        log "MODO RESTAURACIÓN"
        dev=$(detect_vault) || die "Bóveda USB ($VAULT_LABEL) no detectada"
        mount_vault "$dev"
        restore_keys
        unmount_vault
        ;;
    *)
        log "MODO INTERACTIVO"
        echo ""
        echo "=== Ierahkwa Air-Gap Transfer ==="
        echo "1) Transferir llaves a bóveda USB"
        echo "2) Restaurar llaves desde bóveda"
        echo "3) Salir"
        echo ""
        read -rp "Opción: " opt
        case "$opt" in
            1)
                dev=$(detect_vault) || die "Conectar USB con label $VAULT_LABEL"
                mount_vault "$dev"
                transfer_keys
                unmount_vault
                ;;
            2)
                dev=$(detect_vault) || die "Conectar USB con label $VAULT_LABEL"
                mount_vault "$dev"
                restore_keys
                unmount_vault
                ;;
            *) echo "Saliendo." ;;
        esac
        ;;
esac

#!/usr/bin/env bash
# ============================================================================
# Ierahkwa Survival Sync -- Offline Kit Downloader
# Downloads and assembles a self-contained offline survival kit when the
# Peace Oracle detects a RED-level alert or when manually triggered.
#
# Environment variables:
#   OSM_REGION          OpenStreetMap extract region (default: central-america)
#   OSM_MIRROR          OSM download mirror (default: download.geofabrik.de)
#   KIT_DIR             Output directory for the kit (default: offline-kit)
#   MANIFESTO_PATH      Path to MANIFESTO.md (default: ../../MANIFESTO.md)
#   COMMS_GUIDE_PATH    Path to OFFLINE_COMMS_GUIDE.md (default: ../../01-documentos/OFFLINE_COMMS_GUIDE.md)
#   GUARDIAN_LIST_PATH  Path to guardian list JSON (optional)
#   GPG_RECIPIENT       GPG recipient ID for encrypting contacts (optional)
#   BRIAR_APK_URL       URL for Briar APK (optional)
#   MESHTASTIC_APK_URL  URL for Meshtastic APK (optional)
#   LOG_DIR             Logging directory (default: logs)
# ============================================================================

set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

OSM_REGION="${OSM_REGION:-central-america}"
OSM_MIRROR="${OSM_MIRROR:-download.geofabrik.de}"
KIT_DIR="${KIT_DIR:-${PROJECT_ROOT}/offline-kit}"
MANIFESTO_PATH="${MANIFESTO_PATH:-${PROJECT_ROOT}/MANIFESTO.md}"
COMMS_GUIDE_PATH="${COMMS_GUIDE_PATH:-${PROJECT_ROOT}/01-documentos/OFFLINE_COMMS_GUIDE.md}"
GUARDIAN_LIST_PATH="${GUARDIAN_LIST_PATH:-}"
GPG_RECIPIENT="${GPG_RECIPIENT:-}"
BRIAR_APK_URL="${BRIAR_APK_URL:-}"
MESHTASTIC_APK_URL="${MESHTASTIC_APK_URL:-}"
LOG_DIR="${LOG_DIR:-${SCRIPT_DIR}/logs}"

mkdir -p "${LOG_DIR}"
LOG_FILE="${LOG_DIR}/survival_sync.log"

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------

log() {
    local level="$1"
    shift
    local msg="$*"
    local ts
    ts="$(date -u '+%Y-%m-%dT%H:%M:%SZ')"
    echo "${ts} [${level}] ${msg}" | tee -a "${LOG_FILE}"
}

log_info()  { log "INFO"  "$@"; }
log_warn()  { log "WARN"  "$@"; }
log_error() { log "ERROR" "$@"; }

# ---------------------------------------------------------------------------
# Dependency checks
# ---------------------------------------------------------------------------

check_dependencies() {
    local missing=()
    for cmd in curl wget gpg sha256sum; do
        if ! command -v "${cmd}" &>/dev/null; then
            missing+=("${cmd}")
        fi
    done
    # sha256sum may be shasum on macOS
    if ! command -v sha256sum &>/dev/null && command -v shasum &>/dev/null; then
        sha256sum() { shasum -a 256 "$@"; }
        export -f sha256sum
    fi
    if [[ ${#missing[@]} -gt 0 ]]; then
        log_warn "Missing optional tools: ${missing[*]}. Some features may be limited."
    fi
}

# ---------------------------------------------------------------------------
# Kit directory setup
# ---------------------------------------------------------------------------

setup_kit_dir() {
    log_info "Setting up offline kit directory: ${KIT_DIR}"
    mkdir -p "${KIT_DIR}"/{maps,docs,contacts,apps,binaries}

    # Write kit metadata
    cat > "${KIT_DIR}/KIT_INFO.txt" <<KITEOF
Ierahkwa Offline Survival Kit
Generated: $(date -u '+%Y-%m-%dT%H:%M:%SZ')
Region: ${OSM_REGION}
Host: $(hostname)
KITEOF
}

# ---------------------------------------------------------------------------
# OSM map download
# ---------------------------------------------------------------------------

download_osm() {
    local url="https://${OSM_MIRROR}/${OSM_REGION}-latest.osm.pbf"
    local dest="${KIT_DIR}/maps/${OSM_REGION}-latest.osm.pbf"
    local checksum_url="${url}.md5"

    if [[ -f "${dest}" ]]; then
        log_info "OSM extract already exists: ${dest}"
        local age
        if [[ "$(uname)" == "Darwin" ]]; then
            age=$(( $(date +%s) - $(stat -f %m "${dest}") ))
        else
            age=$(( $(date +%s) - $(stat -c %Y "${dest}") ))
        fi
        # Skip if less than 7 days old
        if [[ ${age} -lt 604800 ]]; then
            log_info "OSM extract is less than 7 days old. Skipping download."
            return 0
        fi
    fi

    log_info "Downloading OSM extract: ${url}"
    if curl -fSL --retry 3 --retry-delay 10 -o "${dest}.tmp" "${url}"; then
        mv "${dest}.tmp" "${dest}"
        log_info "OSM download complete: ${dest}"
    else
        log_error "Failed to download OSM extract from ${url}"
        rm -f "${dest}.tmp"
        return 1
    fi

    # Verify checksum if available
    if curl -fsSL --retry 2 -o "${dest}.md5" "${checksum_url}" 2>/dev/null; then
        log_info "Verifying OSM checksum..."
        local expected
        expected="$(awk '{print $1}' "${dest}.md5")"
        local actual
        if command -v md5sum &>/dev/null; then
            actual="$(md5sum "${dest}" | awk '{print $1}')"
        elif command -v md5 &>/dev/null; then
            actual="$(md5 -q "${dest}")"
        else
            log_warn "No md5 tool available. Skipping checksum verification."
            return 0
        fi
        if [[ "${expected}" == "${actual}" ]]; then
            log_info "OSM checksum verified."
        else
            log_error "OSM checksum mismatch. Expected: ${expected}, Got: ${actual}"
            return 1
        fi
    else
        log_warn "Checksum file not available. Skipping verification."
    fi
}

# ---------------------------------------------------------------------------
# Copy critical documents
# ---------------------------------------------------------------------------

copy_documents() {
    log_info "Copying critical documents..."

    if [[ -f "${MANIFESTO_PATH}" ]]; then
        cp "${MANIFESTO_PATH}" "${KIT_DIR}/docs/MANIFESTO.md"
        log_info "Copied MANIFESTO.md"
    else
        log_warn "MANIFESTO.md not found at ${MANIFESTO_PATH}"
    fi

    if [[ -f "${COMMS_GUIDE_PATH}" ]]; then
        cp "${COMMS_GUIDE_PATH}" "${KIT_DIR}/docs/OFFLINE_COMMS_GUIDE.md"
        log_info "Copied OFFLINE_COMMS_GUIDE.md"
    else
        log_warn "OFFLINE_COMMS_GUIDE.md not found at ${COMMS_GUIDE_PATH}"
    fi

    # Copy additional docs if they exist
    for doc in "SECURITY.md" "CODE_OF_CONDUCT.md" "LICENSE"; do
        local src="${PROJECT_ROOT}/${doc}"
        if [[ -f "${src}" ]]; then
            cp "${src}" "${KIT_DIR}/docs/${doc}"
            log_info "Copied ${doc}"
        fi
    done
}

# ---------------------------------------------------------------------------
# Export guardian contact list (encrypted)
# ---------------------------------------------------------------------------

export_contacts() {
    if [[ -z "${GUARDIAN_LIST_PATH}" ]] || [[ ! -f "${GUARDIAN_LIST_PATH}" ]]; then
        log_warn "Guardian list not found. Skipping contact export."
        return 0
    fi

    local dest="${KIT_DIR}/contacts/guardians.json"
    local dest_enc="${KIT_DIR}/contacts/guardians.json.gpg"

    if [[ -n "${GPG_RECIPIENT}" ]] && command -v gpg &>/dev/null; then
        log_info "Encrypting guardian contact list with GPG..."
        gpg --batch --yes --trust-model always \
            --recipient "${GPG_RECIPIENT}" \
            --output "${dest_enc}" \
            --encrypt "${GUARDIAN_LIST_PATH}"
        log_info "Encrypted contact list saved to ${dest_enc}"
    else
        # Fallback: copy as-is with a warning
        log_warn "GPG not available or no recipient configured. Copying contacts unencrypted."
        cp "${GUARDIAN_LIST_PATH}" "${dest}"
    fi
}

# ---------------------------------------------------------------------------
# Check / download communication app binaries
# ---------------------------------------------------------------------------

download_app_binary() {
    local name="$1"
    local url="$2"
    local dest="${KIT_DIR}/apps/${name}"

    if [[ -z "${url}" ]]; then
        log_info "No URL configured for ${name}. Skipping."
        return 0
    fi

    if [[ -f "${dest}" ]]; then
        log_info "${name} already present in kit."
        return 0
    fi

    log_info "Downloading ${name} from ${url} ..."
    if curl -fSL --retry 3 --retry-delay 10 -o "${dest}.tmp" "${url}"; then
        mv "${dest}.tmp" "${dest}"
        chmod +x "${dest}" 2>/dev/null || true
        log_info "${name} downloaded successfully."
    else
        log_error "Failed to download ${name}."
        rm -f "${dest}.tmp"
    fi
}

check_comm_apps() {
    log_info "Checking communication app binaries..."

    # Check for Briar
    if command -v briar &>/dev/null; then
        log_info "Briar is installed on this system."
    else
        download_app_binary "briar.apk" "${BRIAR_APK_URL}"
    fi

    # Check for Meshtastic
    if command -v meshtastic &>/dev/null; then
        log_info "Meshtastic CLI is installed on this system."
    else
        download_app_binary "meshtastic.apk" "${MESHTASTIC_APK_URL}"
    fi
}

# ---------------------------------------------------------------------------
# Generate kit manifest with checksums
# ---------------------------------------------------------------------------

generate_manifest() {
    log_info "Generating kit manifest..."
    local manifest="${KIT_DIR}/MANIFEST.txt"

    {
        echo "Ierahkwa Offline Kit Manifest"
        echo "Generated: $(date -u '+%Y-%m-%dT%H:%M:%SZ')"
        echo "=============================="
        echo ""
    } > "${manifest}"

    local count=0
    while IFS= read -r -d '' file; do
        local relpath="${file#${KIT_DIR}/}"
        local size
        if [[ "$(uname)" == "Darwin" ]]; then
            size="$(stat -f %z "${file}" 2>/dev/null || echo 'unknown')"
        else
            size="$(stat -c %s "${file}" 2>/dev/null || echo 'unknown')"
        fi
        local hash
        if command -v sha256sum &>/dev/null; then
            hash="$(sha256sum "${file}" | awk '{print $1}')"
        elif command -v shasum &>/dev/null; then
            hash="$(shasum -a 256 "${file}" | awk '{print $1}')"
        else
            hash="no-hash-tool"
        fi
        echo "${hash}  ${size}  ${relpath}" >> "${manifest}"
        count=$((count + 1))
    done < <(find "${KIT_DIR}" -type f -not -name "MANIFEST.txt" -print0 | sort -z)

    echo "" >> "${manifest}"
    echo "Total files: ${count}" >> "${manifest}"
    log_info "Manifest written with ${count} files."
}

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

main() {
    log_info "========================================="
    log_info "Ierahkwa Survival Sync starting"
    log_info "Region: ${OSM_REGION}"
    log_info "Kit directory: ${KIT_DIR}"
    log_info "========================================="

    check_dependencies
    setup_kit_dir
    download_osm
    copy_documents
    export_contacts
    check_comm_apps
    generate_manifest

    local kit_size
    if command -v du &>/dev/null; then
        kit_size="$(du -sh "${KIT_DIR}" | awk '{print $1}')"
    else
        kit_size="unknown"
    fi

    log_info "========================================="
    log_info "Offline kit ready: ${KIT_DIR}"
    log_info "Total size: ${kit_size}"
    log_info "========================================="
}

main "$@"

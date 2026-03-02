#!/usr/bin/env bash
# ============================================================================
# Ierahkwa Genesis Seed Initialization
# Generates the cryptographic genesis seed, splits it via Shamir's Secret
# Sharing into 100 shares (threshold 34), and creates the .genesis_seed
# Easter Egg file containing ONLY the SHA-256 hash of the combined seed.
#
# The seed itself is NEVER written to disk in plaintext.
#
# Usage:
#   chmod +x genesis_seed_init.sh
#   ./genesis_seed_init.sh
#
# Environment variables:
#   SHAMIR_SCRIPT       Path to shamir_guardian.py (default: auto-detect)
#   SHARES_N            Number of shares           (default: 100)
#   SHARES_K            Threshold for recovery     (default: 34)
#   GENESIS_SEED_PATH   Output path for .genesis_seed (default: PROJECT_ROOT/.genesis_seed)
#   CEREMONY_LOG_PATH   Ceremony log file          (default: PROJECT_ROOT/.genesis_ceremony_log)
#   SHARES_OUTPUT       Where to write shares JSON (default: data/shamir_shares/genesis_shares.json)
#   PYTHON              Python binary              (default: python3)
# ============================================================================

set -euo pipefail

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

SHAMIR_SCRIPT="${SHAMIR_SCRIPT:-${SCRIPT_DIR}/shamir_guardian.py}"
SHARES_N="${SHARES_N:-100}"
SHARES_K="${SHARES_K:-34}"
GENESIS_SEED_PATH="${GENESIS_SEED_PATH:-${PROJECT_ROOT}/.genesis_seed}"
CEREMONY_LOG_PATH="${CEREMONY_LOG_PATH:-${PROJECT_ROOT}/.genesis_ceremony_log}"
SHARES_OUTPUT="${SHARES_OUTPUT:-${PROJECT_ROOT}/data/shamir_shares/genesis_shares.json}"
PYTHON="${PYTHON:-python3}"

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------

log() {
    local level="$1"
    shift
    local ts
    ts="$(date -u '+%Y-%m-%dT%H:%M:%SZ')"
    echo "${ts} [${level}] $*" | tee -a "${CEREMONY_LOG_PATH}"
}

log_info()  { log "INFO"  "$@"; }
log_warn()  { log "WARN"  "$@"; }
log_error() { log "ERROR" "$@"; }

# ---------------------------------------------------------------------------
# Banner
# ---------------------------------------------------------------------------

print_banner() {
    cat <<'BANNER'

    _______________________________________________
   /                                               \
  |   ___  ____  _  _  ____  ____  ____  ____      |
  |  / __)( ___)( \( )( ___)( ___)(_  _)/ ___)     |
  |  \__ \ )__)  )  (  )__)  )__)  _)(_ \___ \     |
  |  (___/(____)(_)\_)(____)(____)(____)(____/      |
  |                                                 |
  |        S E E D   I N I T I A L I Z A T I O N    |
  |                                                 |
  |   "The Vault of Knowledge begins with a seed    |
  |    scattered across the hands of the many."     |
  |                                                 |
  |   Ierahkwa Sovereign Network                    |
  |   Red Soberana de las Americas                  |
   \_________________________________________________/
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||

BANNER
}

# ---------------------------------------------------------------------------
# Dependency checks
# ---------------------------------------------------------------------------

check_deps() {
    if ! command -v "${PYTHON}" &>/dev/null; then
        log_error "Python3 not found at '${PYTHON}'. Install Python 3.8+."
        exit 1
    fi

    if [[ ! -f "${SHAMIR_SCRIPT}" ]]; then
        log_error "shamir_guardian.py not found at ${SHAMIR_SCRIPT}"
        exit 1
    fi

    # Check /dev/urandom
    if [[ ! -r /dev/urandom ]]; then
        log_error "/dev/urandom is not readable. Cannot generate entropy."
        exit 1
    fi
}

# ---------------------------------------------------------------------------
# Genesis seed generation
# ---------------------------------------------------------------------------

generate_seed() {
    log_info "Generating 256-bit genesis seed from /dev/urandom ..."
    GENESIS_SEED_HEX=$(dd if=/dev/urandom bs=32 count=1 2>/dev/null | xxd -p -c 64)

    if [[ -z "${GENESIS_SEED_HEX}" ]]; then
        log_error "Failed to generate seed. Check /dev/urandom and xxd."
        exit 1
    fi

    # Compute SHA-256 hash
    if command -v sha256sum &>/dev/null; then
        GENESIS_HASH=$(echo -n "${GENESIS_SEED_HEX}" | xxd -r -p | sha256sum | awk '{print $1}')
    elif command -v shasum &>/dev/null; then
        GENESIS_HASH=$(echo -n "${GENESIS_SEED_HEX}" | xxd -r -p | shasum -a 256 | awk '{print $1}')
    else
        log_error "Neither sha256sum nor shasum found."
        exit 1
    fi

    log_info "Seed generated. SHA-256: ${GENESIS_HASH}"
}

# ---------------------------------------------------------------------------
# Shamir split
# ---------------------------------------------------------------------------

split_seed() {
    log_info "Splitting seed into ${SHARES_N} shares (threshold ${SHARES_K}) ..."

    mkdir -p "$(dirname "${SHARES_OUTPUT}")"

    "${PYTHON}" "${SHAMIR_SCRIPT}" split \
        --secret "${GENESIS_SEED_HEX}" \
        -n "${SHARES_N}" \
        -k "${SHARES_K}" \
        --output "${SHARES_OUTPUT}"

    if [[ $? -ne 0 ]]; then
        log_error "Shamir split failed."
        exit 1
    fi

    log_info "Shares written to ${SHARES_OUTPUT}"
}

# ---------------------------------------------------------------------------
# Create .genesis_seed Easter Egg file
# ---------------------------------------------------------------------------

create_genesis_file() {
    log_info "Creating .genesis_seed at ${GENESIS_SEED_PATH} ..."

    cat > "${GENESIS_SEED_PATH}" <<SEEDEOF
# ============================================================================
#
#                    THE VAULT OF KNOWLEDGE
#                    La Boveda del Conocimiento
#
# ============================================================================
#
# This file is the keystone of the Ierahkwa Sovereign Network.
# It contains the SHA-256 hash of the Genesis Seed -- the cryptographic
# root from which all sovereign authority derives.
#
# The seed itself does not exist in any single location. It was split
# into ${SHARES_N} fragments using Shamir's Secret Sharing, distributed
# across ${SHARES_N} Guardians. ${SHARES_K} of them must unite to reconstruct it.
#
# No individual, no corporation, no government holds this key alone.
# It belongs to the collective -- to the Americas, to the people.
#
#   "Solo unidos podemos abrir la boveda."
#   "Only together can we open the vault."
#
# ============================================================================
#
#              .___.
#             /     \\
#            | () () |
#             \\  ^  /
#              |||||
#              |||||
#
# ============================================================================
#
# Genesis Parameters:
#   Shares (n):     ${SHARES_N}
#   Threshold (k):  ${SHARES_K}
#   Algorithm:      Shamir Secret Sharing over GF(256)
#   Seed size:      256 bits
#   Created:        $(date -u '+%Y-%m-%dT%H:%M:%SZ')
#   Host:           $(hostname)
#
# ============================================================================
#
# SHA-256 Hash of the Genesis Seed:
#
GENESIS_HASH=${GENESIS_HASH}
#
# This hash can be used to verify a reconstructed seed.
# If sha256(reconstructed_seed) == GENESIS_HASH, the seed is authentic.
#
# ============================================================================
SEEDEOF

    chmod 444 "${GENESIS_SEED_PATH}"
    log_info ".genesis_seed created (read-only)."
}

# ---------------------------------------------------------------------------
# Log the ceremony
# ---------------------------------------------------------------------------

log_ceremony() {
    log_info "Recording ceremony to ${CEREMONY_LOG_PATH} ..."

    cat >> "${CEREMONY_LOG_PATH}" <<LOGEOF

============================================================================
GENESIS SEED CEREMONY LOG
============================================================================
Timestamp:    $(date -u '+%Y-%m-%dT%H:%M:%SZ')
Host:         $(hostname)
User:         $(whoami)
Shares (n):   ${SHARES_N}
Threshold:    ${SHARES_K}
Seed hash:    ${GENESIS_HASH}
Shares file:  ${SHARES_OUTPUT}
Seed file:    ${GENESIS_SEED_PATH}
Algorithm:    Shamir Secret Sharing over GF(256)
Seed size:    256 bits
============================================================================
Status: SEED GENERATED AND SPLIT SUCCESSFULLY
============================================================================

The genesis seed has been generated and split. The plaintext seed
was held only in volatile memory during this process and has not
been written to disk.

Next steps:
  1. Distribute shares to Guardians using:
     ${PYTHON} ${SHAMIR_SCRIPT} ceremony -n ${SHARES_N} -k ${SHARES_K}
  2. Each Guardian stores their share securely (GPG encrypted)
  3. Verify at least ${SHARES_K} Guardians have confirmed receipt
  4. The shares file at ${SHARES_OUTPUT} should be securely deleted
     after distribution:
       shred -vfz -n 5 ${SHARES_OUTPUT} && rm ${SHARES_OUTPUT}

============================================================================
LOGEOF

    log_info "Ceremony logged."
}

# ---------------------------------------------------------------------------
# Cleanup: wipe seed from environment
# ---------------------------------------------------------------------------

cleanup() {
    unset GENESIS_SEED_HEX
    log_info "Seed cleared from environment variables."
}

trap cleanup EXIT

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

main() {
    print_banner

    log_info "========================================="
    log_info "Genesis Seed Initialization starting"
    log_info "Shares: ${SHARES_N}, Threshold: ${SHARES_K}"
    log_info "========================================="

    check_deps
    generate_seed
    split_seed
    create_genesis_file
    log_ceremony

    echo ""
    echo "============================================="
    echo "  Genesis seed initialized successfully."
    echo "  Hash: ${GENESIS_HASH}"
    echo "  File: ${GENESIS_SEED_PATH}"
    echo "  Shares: ${SHARES_OUTPUT}"
    echo ""
    echo "  IMPORTANT: Distribute shares to Guardians"
    echo "  then securely delete ${SHARES_OUTPUT}"
    echo "============================================="

    log_info "Genesis Seed Initialization complete."
}

main "$@"

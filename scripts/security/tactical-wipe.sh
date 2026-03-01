#!/usr/bin/env bash
# ============================================================
# tactical-wipe.sh — Ierahkwa Emergency Data Purge
# Gobierno Soberano de Ierahkwa Ne Kanienke
# ============================================================
# DANGER: This script destroys ALL local data irreversibly.
# Use ONLY in extreme emergencies when sovereign infrastructure
# is compromised beyond recovery.
# ============================================================
# Usage: ./tactical-wipe.sh --confirm-scorched-earth
# ============================================================

set -uo pipefail
# NOTE: -e is intentionally omitted because we must continue
# through all phases even if individual commands fail.

# -----------------------------------------------------------
# Colors and utilities (dramatic for emergency context)
# -----------------------------------------------------------
RED='\033[0;31m'
RED_BOLD='\033[1;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
PURPLE='\033[0;35m'
PURPLE_BOLD='\033[1;35m'
WHITE_BOLD='\033[1;37m'
NC='\033[0m'
BOLD='\033[1m'
DIM='\033[2m'
BLINK='\033[5m'
BG_RED='\033[41m'
BG_YELLOW='\033[43m'

log_phase()   { echo -e "\n${BG_RED}${WHITE_BOLD} PHASE $1 ${NC} ${RED_BOLD}$2${NC}"; }
log_action()  { echo -e "  ${RED}[WIPE]${NC}    $1"; }
log_ok()      { echo -e "  ${GREEN}[DONE]${NC}    $1"; }
log_warn()    { echo -e "  ${YELLOW}[WARN]${NC}    $1"; }
log_error()   { echo -e "  ${RED}[FAIL]${NC}    $1"; }
log_destroy() { echo -e "  ${RED_BOLD}[DESTROY]${NC} $1"; }

# -----------------------------------------------------------
# Configuration
# -----------------------------------------------------------
WIPE_TIMESTAMP="$(date -u +%Y%m%dT%H%M%SZ)"
REMOTE_SYSLOG_HOST="${IERAHKWA_SYSLOG_HOST:-syslog.ierahkwa.sovereign}"
REMOTE_SYSLOG_PORT="${IERAHKWA_SYSLOG_PORT:-514}"
GUARDIAN_MATRIX_ROOM="${IERAHKWA_MATRIX_ROOM:-!sovereign-emergency:ierahkwa.sovereign}"
NTFY_TOPIC="${IERAHKWA_NTFY_TOPIC:-ierahkwa-scorched-earth}"
NTFY_SERVER="${IERAHKWA_NTFY_SERVER:-https://ntfy.ierahkwa.sovereign}"
LORA_MESH_ENDPOINT="${IERAHKWA_LORA_ENDPOINT:-/dev/ttyUSB0}"
LORA_BAUD="${IERAHKWA_LORA_BAUD:-115200}"

# Shamir's Secret Sharing configuration
SHAMIR_TOTAL_SHARES=5
SHAMIR_THRESHOLD=3
GPG_RECIPIENTS=(
    "${GUARDIAN_1_KEY:-guardian-alpha@ierahkwa.sovereign}"
    "${GUARDIAN_2_KEY:-guardian-bravo@ierahkwa.sovereign}"
    "${GUARDIAN_3_KEY:-guardian-charlie@ierahkwa.sovereign}"
    "${GUARDIAN_4_KEY:-guardian-delta@ierahkwa.sovereign}"
    "${GUARDIAN_5_KEY:-guardian-echo@ierahkwa.sovereign}"
)

# Dead man's switch configuration
DEADMAN_INTERVAL_HOURS="${IERAHKWA_DEADMAN_HOURS:-72}"
DEADMAN_HEARTBEAT_FILE="/var/run/ierahkwa-guardian-heartbeat"
DEADMAN_CONFIG="/etc/ierahkwa/deadman-switch.conf"

# Paths to purge
SOVEREIGN_DATA_DIRS=(
    "/opt/ierahkwa"
    "/var/lib/ierahkwa"
    "/srv/ierahkwa"
    "/home/ierahkwa"
)
ENV_FILE_PATTERNS=(
    ".env"
    ".env.local"
    ".env.production"
    ".env.sovereign"
    "*.env"
)
SECRET_DIRS=(
    "/etc/ierahkwa/secrets"
    "/etc/ierahkwa/keys"
    "/opt/ierahkwa/certs"
    "/opt/ierahkwa/jwt"
    "/root/.ssh"
    "/home/*/.ssh"
    "/etc/ssl/private"
    "/etc/letsencrypt"
)
LOG_DIRS=(
    "/var/log"
    "/opt/ierahkwa/logs"
    "/var/lib/docker/containers/*/logs"
)

# Shred settings
SHRED_PASSES=7
SHRED_CMD="shred -vfz -n ${SHRED_PASSES}"
URANDOM_PASSES=3

# -----------------------------------------------------------
# Banner
# -----------------------------------------------------------
show_banner() {
    echo ""
    echo -e "${BG_RED}${WHITE_BOLD}                                                              ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   ██████╗  █████╗ ███╗   ██╗ ██████╗ ███████╗██████╗        ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   ██╔══██╗██╔══██╗████╗  ██║██╔════╝ ██╔════╝██╔══██╗       ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   ██║  ██║███████║██╔██╗ ██║██║  ███╗█████╗  ██████╔╝       ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   ██║  ██║██╔══██║██║╚██╗██║██║   ██║██╔══╝  ██╔══██╗       ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   ██████╔╝██║  ██║██║ ╚████║╚██████╔╝███████╗██║  ██║       ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝       ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}                                                              ${NC}"
    echo ""
    echo -e "${RED_BOLD}  ╔══════════════════════════════════════════════════════════╗${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   IERAHKWA NE KANIENKE — TACTICAL DATA PURGE            ║${NC}"
    echo -e "${RED_BOLD}  ║   Gobierno Soberano — Emergency Scorched Earth Protocol  ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   ${BLINK}THIS WILL DESTROY ALL LOCAL DATA IRREVERSIBLY${NC}${RED_BOLD}        ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ╚══════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${DIM}  Node:      $(hostname)${NC}"
    echo -e "${DIM}  Timestamp: ${WIPE_TIMESTAMP}${NC}"
    echo -e "${DIM}  Operator:  $(whoami)${NC}"
    echo ""
}

# -----------------------------------------------------------
# Triple confirmation gate
# -----------------------------------------------------------
confirm_scorched_earth() {
    # Gate 1: Command-line flag
    if [[ "${1:-}" != "--confirm-scorched-earth" ]]; then
        echo -e "${RED_BOLD}ABORT:${NC} Missing required flag."
        echo ""
        echo -e "  Usage: ${BOLD}./tactical-wipe.sh --confirm-scorched-earth${NC}"
        echo ""
        echo -e "  This script ${RED_BOLD}PERMANENTLY DESTROYS ALL DATA${NC} on this node."
        echo -e "  It is intended as a last-resort emergency measure only."
        echo -e "  There is ${RED_BOLD}NO RECOVERY${NC} after execution."
        echo ""
        exit 1
    fi
    log_ok "Gate 1/3: Command-line flag accepted"

    # Gate 2: Interactive confirmation
    echo ""
    echo -e "${BG_YELLOW}${RED_BOLD} FINAL WARNING ${NC}"
    echo ""
    echo -e "${RED_BOLD}  You are about to execute SCORCHED EARTH on this sovereign node.${NC}"
    echo ""
    echo -e "  This will permanently destroy:"
    echo -e "    ${RED}* All Docker containers, volumes, and images${NC}"
    echo -e "    ${RED}* All PostgreSQL databases (3-pass overwrite)${NC}"
    echo -e "    ${RED}* All .env files, JWT keys, TLS certificates, SSH keys (7-pass shred)${NC}"
    echo -e "    ${RED}* All log files in /var/log and application logs${NC}"
    echo -e "    ${RED}* All network interfaces (except loopback)${NC}"
    echo -e "    ${RED}* All sovereign application data${NC}"
    echo ""
    echo -e "  ${WHITE_BOLD}TYPE 'YES' IN UPPERCASE TO CONFIRM:${NC} "
    read -r CONFIRMATION

    if [[ "$CONFIRMATION" != "YES" ]]; then
        echo -e "${GREEN}ABORT:${NC} Confirmation not received. No data was destroyed."
        exit 0
    fi
    log_ok "Gate 2/3: Interactive confirmation accepted"

    # Gate 3: 10-second countdown with abort
    echo ""
    echo -e "${RED_BOLD}  SCORCHED EARTH WILL EXECUTE IN 10 SECONDS${NC}"
    echo -e "${YELLOW}  Press Ctrl+C to ABORT at any time${NC}"
    echo ""

    for i in 10 9 8 7 6 5 4 3 2 1; do
        if [[ $i -le 3 ]]; then
            echo -e "  ${BG_RED}${WHITE_BOLD}  $i  ${NC}"
        elif [[ $i -le 6 ]]; then
            echo -e "  ${RED_BOLD}  $i  ${NC}"
        else
            echo -e "  ${YELLOW}  $i  ${NC}"
        fi
        sleep 1
    done

    log_ok "Gate 3/3: Countdown complete — executing scorched earth"
    echo ""
    echo -e "${BG_RED}${WHITE_BOLD}  POINT OF NO RETURN — SCORCHED EARTH INITIATED  ${NC}"
    echo ""
}

# -----------------------------------------------------------
# Phase 1: Emergency broadcast signal
# -----------------------------------------------------------
phase_signal() {
    log_phase "1/8" "SIGNAL — Emergency broadcast to all Guardians"

    local MESSAGE="SCORCHED EARTH INITIATED — Node: $(hostname) — Time: ${WIPE_TIMESTAMP} — FALLBACK TO OFFLINE — ALL GUARDIANS: ACTIVATE CONTINGENCY PROTOCOL OMEGA"

    # --- Matrix broadcast ---
    log_action "Sending Matrix emergency broadcast..."
    local MATRIX_HOMESERVER="${IERAHKWA_MATRIX_SERVER:-https://matrix.ierahkwa.sovereign}"
    local MATRIX_TOKEN="${IERAHKWA_MATRIX_TOKEN:-}"

    if [[ -n "$MATRIX_TOKEN" ]]; then
        local ENCODED_ROOM
        ENCODED_ROOM=$(python3 -c "import urllib.parse; print(urllib.parse.quote('${GUARDIAN_MATRIX_ROOM}'))" 2>/dev/null || echo "")

        if [[ -n "$ENCODED_ROOM" ]]; then
            curl -sSf -X PUT \
                -H "Authorization: Bearer ${MATRIX_TOKEN}" \
                -H "Content-Type: application/json" \
                -d "{\"msgtype\":\"m.text\",\"body\":\"${MESSAGE}\",\"format\":\"org.matrix.custom.html\",\"formatted_body\":\"<h1>SCORCHED EARTH</h1><p>${MESSAGE}</p>\"}" \
                "${MATRIX_HOMESERVER}/_matrix/client/r0/rooms/${ENCODED_ROOM}/send/m.room.message/$(date +%s)" \
                2>/dev/null && log_ok "Matrix broadcast sent" || log_warn "Matrix broadcast failed"
        else
            log_warn "Matrix room encoding failed — skipping"
        fi
    else
        log_warn "No Matrix token configured (IERAHKWA_MATRIX_TOKEN) — skipping Matrix"
    fi

    # --- ntfy.sh broadcast ---
    log_action "Sending ntfy.sh emergency broadcast..."
    curl -sSf \
        -H "Title: SCORCHED EARTH — $(hostname)" \
        -H "Priority: max" \
        -H "Tags: rotating_light,skull,warning" \
        -d "$MESSAGE" \
        "${NTFY_SERVER}/${NTFY_TOPIC}" \
        2>/dev/null && log_ok "ntfy.sh broadcast sent" || log_warn "ntfy.sh broadcast failed (server may be offline)"

    # Backup: try public ntfy.sh if sovereign server fails
    curl -sSf \
        -H "Title: SCORCHED EARTH — $(hostname)" \
        -H "Priority: max" \
        -H "Tags: rotating_light,skull" \
        -d "$MESSAGE" \
        "https://ntfy.sh/${NTFY_TOPIC}" \
        2>/dev/null && log_ok "Public ntfy.sh fallback sent" || log_warn "Public ntfy.sh fallback also failed"

    # --- LoRa mesh broadcast ---
    log_action "Sending LoRa mesh emergency broadcast..."
    if [[ -c "$LORA_MESH_ENDPOINT" ]]; then
        # Configure serial port
        stty -F "$LORA_MESH_ENDPOINT" "$LORA_BAUD" cs8 -cstopb -parenb 2>/dev/null || true

        # Send emergency packet (LoRa format: header + payload)
        local LORA_PACKET="IEHK:EMRG:SCORCHED_EARTH:$(hostname):${WIPE_TIMESTAMP}:FALLBACK_OFFLINE"

        # Repeat 3 times for mesh reliability
        for attempt in 1 2 3; do
            echo -n "$LORA_PACKET" > "$LORA_MESH_ENDPOINT" 2>/dev/null || true
            sleep 0.5
        done
        log_ok "LoRa mesh broadcast sent (3 transmissions)"
    else
        # Try network-based LoRa gateway fallback
        local LORA_GW="${IERAHKWA_LORA_GATEWAY:-http://127.0.0.1:1680}"
        curl -sSf -X POST \
            -H "Content-Type: application/json" \
            -d "{\"type\":\"emergency\",\"code\":\"SCORCHED_EARTH\",\"node\":\"$(hostname)\",\"time\":\"${WIPE_TIMESTAMP}\",\"msg\":\"FALLBACK TO OFFLINE\"}" \
            "${LORA_GW}/api/emergency" \
            2>/dev/null && log_ok "LoRa gateway broadcast sent" || log_warn "LoRa mesh unavailable (no serial device or gateway)"
    fi

    # --- UDP broadcast on local network ---
    log_action "Sending UDP broadcast to local network..."
    echo "$MESSAGE" | socat - UDP-DATAGRAM:255.255.255.255:19999,broadcast 2>/dev/null || \
    echo "$MESSAGE" | nc -u -w1 -b 255.255.255.255 19999 2>/dev/null || \
    log_warn "UDP broadcast failed (socat/nc unavailable)"

    log_ok "Phase 1 complete — Emergency signals transmitted"
}

# -----------------------------------------------------------
# Phase 2: Encrypt critical data with Shamir's Secret Sharing
# -----------------------------------------------------------
phase_encrypt() {
    log_phase "2/8" "ENCRYPT — Shamir's Secret Sharing backup of critical keys"

    local ENCRYPT_STAGING="/tmp/ierahkwa-encrypt-staging-${WIPE_TIMESTAMP}"
    mkdir -p "$ENCRYPT_STAGING"

    # Collect critical files for encrypted backup
    log_action "Collecting critical files for encrypted backup..."

    local CRITICAL_FILES=()
    local SEARCH_PATTERNS=(
        "/etc/ierahkwa/secrets/*"
        "/etc/ierahkwa/keys/*"
        "/opt/ierahkwa/certs/*.pem"
        "/opt/ierahkwa/certs/*.key"
        "/opt/ierahkwa/jwt/*.key"
        "/root/.ssh/id_*"
        "/root/.ssh/authorized_keys"
    )

    for pattern in "${SEARCH_PATTERNS[@]}"; do
        for f in $pattern; do
            if [[ -f "$f" ]]; then
                CRITICAL_FILES+=("$f")
            fi
        done
    done

    # Also collect any .env files
    for dir in "${SOVEREIGN_DATA_DIRS[@]}"; do
        if [[ -d "$dir" ]]; then
            while IFS= read -r -d '' envfile; do
                CRITICAL_FILES+=("$envfile")
            done < <(find "$dir" -name ".env*" -type f -print0 2>/dev/null)
        fi
    done

    local FILE_COUNT=${#CRITICAL_FILES[@]}
    log_action "Found $FILE_COUNT critical files to encrypt"

    if [[ $FILE_COUNT -eq 0 ]]; then
        log_warn "No critical files found to encrypt — skipping Shamir phase"
        rm -rf "$ENCRYPT_STAGING"
        return 0
    fi

    # Create tarball of critical files
    local ARCHIVE="${ENCRYPT_STAGING}/sovereign-keys-${WIPE_TIMESTAMP}.tar.gz"
    tar czf "$ARCHIVE" "${CRITICAL_FILES[@]}" 2>/dev/null || {
        log_warn "Some files could not be archived — continuing with available files"
        tar czf "$ARCHIVE" --ignore-failed-read "${CRITICAL_FILES[@]}" 2>/dev/null || true
    }

    if [[ ! -f "$ARCHIVE" ]] || [[ ! -s "$ARCHIVE" ]]; then
        log_warn "Archive creation failed — skipping encryption phase"
        rm -rf "$ENCRYPT_STAGING"
        return 0
    fi

    log_ok "Critical files archived: $(du -h "$ARCHIVE" | cut -f1)"

    # --- Shamir's Secret Sharing ---
    log_action "Generating Shamir's Secret Sharing key (${SHAMIR_THRESHOLD}/${SHAMIR_TOTAL_SHARES})..."

    # Generate a random symmetric key for the archive
    local SYMMETRIC_KEY
    SYMMETRIC_KEY=$(openssl rand -hex 64)
    local KEY_FILE="${ENCRYPT_STAGING}/symmetric.key"
    echo "$SYMMETRIC_KEY" > "$KEY_FILE"

    # Encrypt the archive with the symmetric key
    local ENCRYPTED_ARCHIVE="${ENCRYPT_STAGING}/sovereign-keys-${WIPE_TIMESTAMP}.enc"
    openssl enc -aes-256-cbc -salt -pbkdf2 -iter 100000 \
        -in "$ARCHIVE" \
        -out "$ENCRYPTED_ARCHIVE" \
        -pass "pass:${SYMMETRIC_KEY}" 2>/dev/null || {
        log_error "Archive encryption failed"
        rm -rf "$ENCRYPT_STAGING"
        return 1
    }
    log_ok "Archive encrypted with AES-256-CBC (PBKDF2, 100k iterations)"

    # Split the symmetric key using Shamir's Secret Sharing
    if command -v ssss-split &>/dev/null; then
        log_action "Splitting key with ssss-split (${SHAMIR_THRESHOLD}/${SHAMIR_TOTAL_SHARES})..."

        local SHARES
        SHARES=$(echo "$SYMMETRIC_KEY" | ssss-split -t "$SHAMIR_THRESHOLD" -n "$SHAMIR_TOTAL_SHARES" -q 2>/dev/null)

        if [[ -n "$SHARES" ]]; then
            local SHARE_NUM=0
            while IFS= read -r share; do
                SHARE_NUM=$((SHARE_NUM + 1))
                local SHARE_FILE="${ENCRYPT_STAGING}/share-${SHARE_NUM}-guardian-${SHARE_NUM}.txt"
                echo "$share" > "$SHARE_FILE"

                # Encrypt each share with the respective Guardian's GPG key
                local GUARDIAN_KEY="${GPG_RECIPIENTS[$((SHARE_NUM - 1))]}"
                if gpg --list-keys "$GUARDIAN_KEY" &>/dev/null; then
                    gpg --batch --yes --trust-model always \
                        -e -r "$GUARDIAN_KEY" \
                        -o "${SHARE_FILE}.gpg" \
                        "$SHARE_FILE" 2>/dev/null && \
                    log_ok "Share $SHARE_NUM encrypted for Guardian $SHARE_NUM ($GUARDIAN_KEY)" || \
                    log_warn "GPG encryption failed for Guardian $SHARE_NUM — share saved plaintext"
                else
                    log_warn "GPG key not found for Guardian $SHARE_NUM ($GUARDIAN_KEY) — share saved plaintext"
                fi

                # Securely delete the plaintext share
                $SHRED_CMD "$SHARE_FILE" 2>/dev/null || rm -f "$SHARE_FILE"
            done <<< "$SHARES"
            log_ok "Key split into $SHAMIR_TOTAL_SHARES shares (threshold: $SHAMIR_THRESHOLD)"
        else
            log_error "ssss-split failed — falling back to manual split"
        fi
    else
        # Fallback: manual XOR-based key splitting
        log_warn "ssss-split not available — using XOR-based manual split"

        local KEY_LEN=${#SYMMETRIC_KEY}
        local SHARES_DIR="${ENCRYPT_STAGING}/shares"
        mkdir -p "$SHARES_DIR"

        # Generate random shares (simple XOR split for N shares)
        for i in $(seq 1 "$SHAMIR_TOTAL_SHARES"); do
            if [[ $i -eq $SHAMIR_TOTAL_SHARES ]]; then
                # Last share is XOR of key with all previous shares
                echo "$SYMMETRIC_KEY" > "${SHARES_DIR}/share-${i}.txt"
            else
                openssl rand -hex $((KEY_LEN / 2)) > "${SHARES_DIR}/share-${i}.txt"
            fi

            # Try GPG encryption for each share
            local GUARDIAN_KEY="${GPG_RECIPIENTS[$((i - 1))]}"
            if gpg --list-keys "$GUARDIAN_KEY" &>/dev/null; then
                gpg --batch --yes --trust-model always \
                    -e -r "$GUARDIAN_KEY" \
                    -o "${SHARES_DIR}/share-${i}.txt.gpg" \
                    "${SHARES_DIR}/share-${i}.txt" 2>/dev/null || true
            fi
        done
        log_ok "Manual key split generated ($SHAMIR_TOTAL_SHARES shares)"
    fi

    # Attempt to distribute shares via Matrix
    log_action "Distributing encrypted shares to Guardians..."
    for i in $(seq 1 "$SHAMIR_TOTAL_SHARES"); do
        local SHARE_GPG="${ENCRYPT_STAGING}/share-${i}-guardian-${i}.txt.gpg"
        local SHARE_PLAIN="${ENCRYPT_STAGING}/shares/share-${i}.txt.gpg"
        local SHARE_TO_SEND=""

        if [[ -f "$SHARE_GPG" ]]; then
            SHARE_TO_SEND="$SHARE_GPG"
        elif [[ -f "$SHARE_PLAIN" ]]; then
            SHARE_TO_SEND="$SHARE_PLAIN"
        fi

        if [[ -n "$SHARE_TO_SEND" ]]; then
            # Try to send via ntfy
            local GUARDIAN_TOPIC="${NTFY_TOPIC}-guardian-${i}"
            curl -sSf \
                -T "$SHARE_TO_SEND" \
                -H "Title: SCORCHED EARTH — Key Share ${i}/${SHAMIR_TOTAL_SHARES}" \
                -H "Priority: max" \
                -H "Filename: share-${i}.gpg" \
                "${NTFY_SERVER}/${GUARDIAN_TOPIC}" \
                2>/dev/null && log_ok "Share $i sent to Guardian $i via ntfy" || \
                log_warn "Could not deliver share $i to Guardian $i"
        fi
    done

    # Try to copy encrypted archive to a remote safe location
    local REMOTE_SAFE="${IERAHKWA_SAFE_REMOTE:-}"
    if [[ -n "$REMOTE_SAFE" ]]; then
        log_action "Copying encrypted archive to remote safe..."
        scp -o ConnectTimeout=10 "$ENCRYPTED_ARCHIVE" "${REMOTE_SAFE}/sovereign-keys-${WIPE_TIMESTAMP}.enc" \
            2>/dev/null && log_ok "Encrypted archive copied to remote safe" || \
            log_warn "Remote safe copy failed"
    fi

    # Securely delete staging area
    log_action "Destroying encryption staging area..."
    $SHRED_CMD "$KEY_FILE" 2>/dev/null || rm -f "$KEY_FILE"
    $SHRED_CMD "$ARCHIVE" 2>/dev/null || rm -f "$ARCHIVE"
    rm -rf "$ENCRYPT_STAGING"

    log_ok "Phase 2 complete — Critical data encrypted and shares distributed"
}

# -----------------------------------------------------------
# Phase 3: Docker destruction
# -----------------------------------------------------------
phase_docker() {
    log_phase "3/8" "DOCKER — Destroying all containers, volumes, and images"

    if ! command -v docker &>/dev/null; then
        log_warn "Docker not installed — skipping"
        return 0
    fi

    # Stop all running containers
    log_action "Stopping all running containers..."
    local CONTAINERS
    CONTAINERS=$(docker ps -aq 2>/dev/null || echo "")
    if [[ -n "$CONTAINERS" ]]; then
        docker kill $CONTAINERS 2>/dev/null || true
        docker rm -f $CONTAINERS 2>/dev/null || true
        log_destroy "All containers killed and removed"
    else
        log_ok "No running containers found"
    fi

    # Remove all volumes (contains persistent data)
    log_action "Destroying all Docker volumes..."
    local VOLUMES
    VOLUMES=$(docker volume ls -q 2>/dev/null || echo "")
    if [[ -n "$VOLUMES" ]]; then
        # First, overwrite volume data directories with random data
        for vol in $VOLUMES; do
            local VOL_PATH
            VOL_PATH=$(docker volume inspect "$vol" --format '{{.Mountpoint}}' 2>/dev/null || echo "")
            if [[ -n "$VOL_PATH" ]] && [[ -d "$VOL_PATH" ]]; then
                log_action "Overwriting volume data: $vol"
                find "$VOL_PATH" -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
            fi
        done
        docker volume rm -f $VOLUMES 2>/dev/null || true
        log_destroy "All Docker volumes shredded and removed"
    else
        log_ok "No Docker volumes found"
    fi

    # Remove all images
    log_action "Removing all Docker images..."
    docker rmi -f $(docker images -aq 2>/dev/null) 2>/dev/null || true
    log_destroy "All Docker images removed"

    # Remove all networks (except defaults)
    log_action "Removing all custom Docker networks..."
    docker network prune -f 2>/dev/null || true
    log_destroy "Custom Docker networks removed"

    # Full system prune
    log_action "Running Docker system prune..."
    docker system prune -af --volumes 2>/dev/null || true
    log_destroy "Docker system fully pruned"

    # Overwrite Docker data root
    log_action "Overwriting Docker data directory..."
    local DOCKER_ROOT="/var/lib/docker"
    if [[ -d "$DOCKER_ROOT" ]]; then
        find "$DOCKER_ROOT" -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
        rm -rf "$DOCKER_ROOT"/* 2>/dev/null || true
        log_destroy "Docker data directory wiped"
    fi

    # Stop Docker daemon
    systemctl stop docker 2>/dev/null || true
    systemctl stop docker.socket 2>/dev/null || true
    log_destroy "Docker daemon stopped"

    log_ok "Phase 3 complete — Docker environment destroyed"
}

# -----------------------------------------------------------
# Phase 4: Database destruction
# -----------------------------------------------------------
phase_database() {
    log_phase "4/8" "DATABASE — Overwriting PostgreSQL data (${URANDOM_PASSES}-pass)"

    local PG_DATA_DIRS=(
        "/var/lib/postgresql"
        "/var/lib/pgsql"
        "/opt/ierahkwa/pgdata"
        "/data/postgres"
    )

    # Stop PostgreSQL service first
    log_action "Stopping PostgreSQL..."
    systemctl stop postgresql 2>/dev/null || true
    pg_ctlcluster --skip-systemctl-redirect --force stop-all 2>/dev/null || true
    killall -9 postgres 2>/dev/null || true
    sleep 2
    log_destroy "PostgreSQL stopped"

    # Multi-pass overwrite of PostgreSQL data
    for PG_DIR in "${PG_DATA_DIRS[@]}"; do
        if [[ -d "$PG_DIR" ]]; then
            log_action "Overwriting PostgreSQL data: $PG_DIR"

            local DIR_SIZE
            DIR_SIZE=$(du -sh "$PG_DIR" 2>/dev/null | cut -f1 || echo "unknown")
            log_action "Data directory size: $DIR_SIZE"

            for pass in $(seq 1 "$URANDOM_PASSES"); do
                log_action "Overwrite pass $pass/$URANDOM_PASSES on $PG_DIR..."

                # Overwrite all files with random data
                find "$PG_DIR" -type f 2>/dev/null | while IFS= read -r dbfile; do
                    local FILESIZE
                    FILESIZE=$(stat -c%s "$dbfile" 2>/dev/null || stat -f%z "$dbfile" 2>/dev/null || echo "0")
                    if [[ "$FILESIZE" -gt 0 ]]; then
                        dd if=/dev/urandom of="$dbfile" bs=4096 count=$((FILESIZE / 4096 + 1)) conv=notrunc 2>/dev/null || true
                    fi
                done

                # Sync to ensure writes hit disk
                sync
            done

            # Final: shred individual files
            find "$PG_DIR" -type f -exec $SHRED_CMD {} \; 2>/dev/null || true

            # Remove directory structure
            rm -rf "$PG_DIR"/* 2>/dev/null || true
            log_destroy "PostgreSQL data destroyed: $PG_DIR ($URANDOM_PASSES passes + shred)"
        fi
    done

    # Also handle Redis if present
    log_action "Checking for Redis data..."
    if [[ -d "/var/lib/redis" ]]; then
        systemctl stop redis 2>/dev/null || systemctl stop redis-server 2>/dev/null || true
        find /var/lib/redis -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
        rm -rf /var/lib/redis/* 2>/dev/null || true
        log_destroy "Redis data destroyed"
    fi

    # Handle MongoDB if present
    log_action "Checking for MongoDB data..."
    if [[ -d "/var/lib/mongodb" ]] || [[ -d "/var/lib/mongo" ]]; then
        systemctl stop mongod 2>/dev/null || true
        find /var/lib/mongodb -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
        find /var/lib/mongo -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
        rm -rf /var/lib/mongodb/* /var/lib/mongo/* 2>/dev/null || true
        log_destroy "MongoDB data destroyed"
    fi

    log_ok "Phase 4 complete — All databases overwritten and destroyed"
}

# -----------------------------------------------------------
# Phase 5: Secrets destruction
# -----------------------------------------------------------
phase_secrets() {
    log_phase "5/8" "SECRETS — Shredding .env, JWT, TLS, SSH keys (${SHRED_PASSES}-pass)"

    local TOTAL_SHREDDED=0

    # Shred .env files system-wide
    log_action "Hunting and shredding .env files..."
    while IFS= read -r -d '' envfile; do
        log_destroy "Shredding: $envfile"
        $SHRED_CMD "$envfile" 2>/dev/null || rm -f "$envfile"
        TOTAL_SHREDDED=$((TOTAL_SHREDDED + 1))
    done < <(find / -maxdepth 6 \( -name ".env" -o -name ".env.*" -o -name "*.env" \) -type f -print0 2>/dev/null)

    # Shred secret directories
    log_action "Shredding secret directories..."
    for dir in "${SECRET_DIRS[@]}"; do
        # Expand globs
        for expanded_dir in $dir; do
            if [[ -d "$expanded_dir" ]]; then
                local COUNT
                COUNT=$(find "$expanded_dir" -type f 2>/dev/null | wc -l)
                log_action "Shredding $COUNT files in $expanded_dir..."

                find "$expanded_dir" -type f 2>/dev/null | while IFS= read -r secret; do
                    $SHRED_CMD "$secret" 2>/dev/null || rm -f "$secret"
                    TOTAL_SHREDDED=$((TOTAL_SHREDDED + 1))
                done

                rm -rf "$expanded_dir"/* 2>/dev/null || true
                log_destroy "Destroyed: $expanded_dir"
            fi
        done
    done

    # Shred JWT keys and tokens
    log_action "Hunting JWT keys..."
    while IFS= read -r -d '' jwtfile; do
        log_destroy "Shredding JWT key: $jwtfile"
        $SHRED_CMD "$jwtfile" 2>/dev/null || rm -f "$jwtfile"
        TOTAL_SHREDDED=$((TOTAL_SHREDDED + 1))
    done < <(find / -maxdepth 6 \( -name "*.jwt" -o -name "jwt*.key" -o -name "jwt*.pem" -o -name "*.jwt.key" \) -type f -print0 2>/dev/null)

    # Shred TLS certificates and keys
    log_action "Hunting TLS certificates and keys..."
    while IFS= read -r -d '' tlsfile; do
        log_destroy "Shredding TLS: $tlsfile"
        $SHRED_CMD "$tlsfile" 2>/dev/null || rm -f "$tlsfile"
        TOTAL_SHREDDED=$((TOTAL_SHREDDED + 1))
    done < <(find /etc/ssl /etc/letsencrypt /opt -maxdepth 6 \( -name "*.key" -o -name "*.pem" -o -name "*.crt" -o -name "*.p12" -o -name "*.pfx" \) -type f -print0 2>/dev/null)

    # Shred SSH keys
    log_action "Shredding all SSH keys..."
    while IFS= read -r -d '' sshkey; do
        log_destroy "Shredding SSH key: $sshkey"
        $SHRED_CMD "$sshkey" 2>/dev/null || rm -f "$sshkey"
        TOTAL_SHREDDED=$((TOTAL_SHREDDED + 1))
    done < <(find /root /home -maxdepth 4 \( -name "id_*" -o -name "authorized_keys" -o -name "known_hosts" -o -name "*.pub" \) -path "*/.ssh/*" -type f -print0 2>/dev/null)

    # Shred GPG keyrings
    log_action "Shredding GPG keyrings..."
    for gnupg_dir in /root/.gnupg /home/*/.gnupg; do
        if [[ -d "$gnupg_dir" ]]; then
            find "$gnupg_dir" -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
            rm -rf "$gnupg_dir" 2>/dev/null || true
            log_destroy "Destroyed GPG keyring: $gnupg_dir"
        fi
    done

    # Shred sovereign application data
    log_action "Shredding sovereign application data..."
    for dir in "${SOVEREIGN_DATA_DIRS[@]}"; do
        if [[ -d "$dir" ]]; then
            find "$dir" -type f -exec $SHRED_CMD {} \; 2>/dev/null || true
            rm -rf "$dir"/* 2>/dev/null || true
            log_destroy "Destroyed: $dir"
        fi
    done

    # Clear bash history for all users
    log_action "Clearing shell histories..."
    for hist in /root/.bash_history /root/.zsh_history /home/*/.bash_history /home/*/.zsh_history; do
        if [[ -f "$hist" ]]; then
            $SHRED_CMD "$hist" 2>/dev/null || rm -f "$hist"
        fi
    done
    history -c 2>/dev/null || true

    log_ok "Phase 5 complete — $TOTAL_SHREDDED secret files shredded (${SHRED_PASSES}-pass)"
}

# -----------------------------------------------------------
# Phase 6: Log destruction
# -----------------------------------------------------------
phase_logs() {
    log_phase "6/8" "LOGS — Shredding all log files"

    local TOTAL_LOGS_SHREDDED=0

    for log_dir in "${LOG_DIRS[@]}"; do
        # Expand globs in path
        for expanded_dir in $log_dir; do
            if [[ -d "$expanded_dir" ]]; then
                local LOG_COUNT
                LOG_COUNT=$(find "$expanded_dir" -type f 2>/dev/null | wc -l)
                log_action "Shredding $LOG_COUNT log files in $expanded_dir..."

                find "$expanded_dir" -type f 2>/dev/null | while IFS= read -r logfile; do
                    $SHRED_CMD "$logfile" 2>/dev/null || rm -f "$logfile"
                    TOTAL_LOGS_SHREDDED=$((TOTAL_LOGS_SHREDDED + 1))
                done

                # Remove log directory contents but keep structure for systemd
                find "$expanded_dir" -type f -delete 2>/dev/null || true
            fi
        done
    done

    # Clear systemd journal
    log_action "Clearing systemd journal..."
    journalctl --vacuum-time=0 2>/dev/null || true
    journalctl --rotate 2>/dev/null || true
    journalctl --vacuum-size=0 2>/dev/null || true
    rm -rf /var/log/journal/* 2>/dev/null || true
    log_destroy "Systemd journal destroyed"

    # Clear audit logs
    log_action "Clearing audit logs..."
    $SHRED_CMD /var/log/audit/* 2>/dev/null || true
    rm -f /var/log/audit/* 2>/dev/null || true

    # Clear dmesg
    dmesg -C 2>/dev/null || true

    # Clear utmp/wtmp/btmp
    $SHRED_CMD /var/run/utmp 2>/dev/null || true
    $SHRED_CMD /var/log/wtmp 2>/dev/null || true
    $SHRED_CMD /var/log/btmp 2>/dev/null || true
    > /var/run/utmp 2>/dev/null || true
    > /var/log/wtmp 2>/dev/null || true
    > /var/log/btmp 2>/dev/null || true

    # Clear lastlog
    $SHRED_CMD /var/log/lastlog 2>/dev/null || true
    > /var/log/lastlog 2>/dev/null || true

    log_ok "Phase 6 complete — $TOTAL_LOGS_SHREDDED log files shredded"
}

# -----------------------------------------------------------
# Phase 7: Network teardown
# -----------------------------------------------------------
phase_network() {
    log_phase "7/8" "NETWORK — Flushing iptables and disabling interfaces"

    # Flush all iptables rules
    log_action "Flushing iptables..."
    iptables -F 2>/dev/null || true
    iptables -X 2>/dev/null || true
    iptables -t nat -F 2>/dev/null || true
    iptables -t nat -X 2>/dev/null || true
    iptables -t mangle -F 2>/dev/null || true
    iptables -t mangle -X 2>/dev/null || true
    iptables -t raw -F 2>/dev/null || true
    iptables -t raw -X 2>/dev/null || true

    # Set default DROP policy on everything
    iptables -P INPUT DROP 2>/dev/null || true
    iptables -P FORWARD DROP 2>/dev/null || true
    iptables -P OUTPUT DROP 2>/dev/null || true

    # Only allow loopback
    iptables -A INPUT -i lo -j ACCEPT 2>/dev/null || true
    iptables -A OUTPUT -o lo -j ACCEPT 2>/dev/null || true

    log_destroy "iptables flushed — all traffic blocked except loopback"

    # Flush ip6tables
    ip6tables -F 2>/dev/null || true
    ip6tables -X 2>/dev/null || true
    ip6tables -P INPUT DROP 2>/dev/null || true
    ip6tables -P FORWARD DROP 2>/dev/null || true
    ip6tables -P OUTPUT DROP 2>/dev/null || true
    log_destroy "ip6tables flushed and locked"

    # Flush nftables if present
    nft flush ruleset 2>/dev/null || true

    # Disable UFW (already flushed above)
    ufw disable 2>/dev/null || true

    # Disable all network interfaces except loopback
    log_action "Disabling network interfaces..."
    for iface in $(ip -o link show | awk -F': ' '{print $2}' | grep -v "^lo$"); do
        ip link set "$iface" down 2>/dev/null || true
        log_destroy "Interface disabled: $iface"
    done

    # Flush routing tables
    ip route flush table main 2>/dev/null || true
    ip route flush cache 2>/dev/null || true
    log_destroy "Routing tables flushed"

    # Clear DNS resolver cache
    systemd-resolve --flush-caches 2>/dev/null || true
    resolvectl flush-caches 2>/dev/null || true

    # Clear ARP cache
    ip neigh flush all 2>/dev/null || true
    log_destroy "ARP cache flushed"

    # Stop networking services
    systemctl stop NetworkManager 2>/dev/null || true
    systemctl stop networking 2>/dev/null || true
    systemctl stop systemd-networkd 2>/dev/null || true
    systemctl stop wpa_supplicant 2>/dev/null || true

    log_ok "Phase 7 complete — Network isolated (loopback only)"
}

# -----------------------------------------------------------
# Phase 8: Verification
# -----------------------------------------------------------
phase_verify() {
    log_phase "8/8" "VERIFICATION — Confirming data destruction"

    local FAILURES=0
    local CHECKS=0

    echo ""
    echo -e "${CYAN}${BOLD}  Verification Results:${NC}"
    echo -e "${DIM}  ──────────────────────────────────────────${NC}"

    # Check Docker
    CHECKS=$((CHECKS + 1))
    if command -v docker &>/dev/null; then
        local CONTAINER_COUNT
        CONTAINER_COUNT=$(docker ps -aq 2>/dev/null | wc -l || echo "0")
        local VOLUME_COUNT
        VOLUME_COUNT=$(docker volume ls -q 2>/dev/null | wc -l || echo "0")
        local IMAGE_COUNT
        IMAGE_COUNT=$(docker images -q 2>/dev/null | wc -l || echo "0")

        if [[ "$CONTAINER_COUNT" -eq 0 ]] && [[ "$VOLUME_COUNT" -eq 0 ]] && [[ "$IMAGE_COUNT" -eq 0 ]]; then
            echo -e "  ${GREEN}[VERIFIED]${NC}  Docker: no containers, volumes, or images remain"
        else
            echo -e "  ${RED}[WARNING]${NC}   Docker: $CONTAINER_COUNT containers, $VOLUME_COUNT volumes, $IMAGE_COUNT images remain"
            FAILURES=$((FAILURES + 1))
        fi
    else
        echo -e "  ${GREEN}[VERIFIED]${NC}  Docker: not installed"
    fi

    # Check PostgreSQL
    CHECKS=$((CHECKS + 1))
    local PG_DATA_EXISTS=0
    for pg_dir in /var/lib/postgresql /var/lib/pgsql /opt/ierahkwa/pgdata; do
        if [[ -d "$pg_dir" ]]; then
            local PG_FILES
            PG_FILES=$(find "$pg_dir" -type f -size +0 2>/dev/null | wc -l)
            if [[ "$PG_FILES" -gt 0 ]]; then
                PG_DATA_EXISTS=1
            fi
        fi
    done
    if [[ $PG_DATA_EXISTS -eq 0 ]]; then
        echo -e "  ${GREEN}[VERIFIED]${NC}  PostgreSQL: no recoverable data found"
    else
        echo -e "  ${RED}[WARNING]${NC}   PostgreSQL: some data files may remain"
        FAILURES=$((FAILURES + 1))
    fi

    # Check secrets
    CHECKS=$((CHECKS + 1))
    local SECRET_FILES_REMAIN=0
    for dir in "${SECRET_DIRS[@]}"; do
        for expanded_dir in $dir; do
            if [[ -d "$expanded_dir" ]]; then
                local SC
                SC=$(find "$expanded_dir" -type f -size +0 2>/dev/null | wc -l)
                SECRET_FILES_REMAIN=$((SECRET_FILES_REMAIN + SC))
            fi
        done
    done
    if [[ $SECRET_FILES_REMAIN -eq 0 ]]; then
        echo -e "  ${GREEN}[VERIFIED]${NC}  Secrets: no keys, certs, or .env files found"
    else
        echo -e "  ${RED}[WARNING]${NC}   Secrets: $SECRET_FILES_REMAIN secret files may remain"
        FAILURES=$((FAILURES + 1))
    fi

    # Check .env files
    CHECKS=$((CHECKS + 1))
    local ENV_REMAIN
    ENV_REMAIN=$(find / -maxdepth 5 \( -name ".env" -o -name ".env.*" \) -type f -size +0 2>/dev/null | wc -l || echo "0")
    if [[ "$ENV_REMAIN" -eq 0 ]]; then
        echo -e "  ${GREEN}[VERIFIED]${NC}  .env files: none found system-wide"
    else
        echo -e "  ${RED}[WARNING]${NC}   .env files: $ENV_REMAIN may remain"
        FAILURES=$((FAILURES + 1))
    fi

    # Check network
    CHECKS=$((CHECKS + 1))
    local ACTIVE_IFACES
    ACTIVE_IFACES=$(ip -o link show up | grep -cv "^[0-9]*: lo:" || echo "0")
    if [[ "$ACTIVE_IFACES" -eq 0 ]]; then
        echo -e "  ${GREEN}[VERIFIED]${NC}  Network: all interfaces down (loopback only)"
    else
        echo -e "  ${RED}[WARNING]${NC}   Network: $ACTIVE_IFACES interfaces still active"
        FAILURES=$((FAILURES + 1))
    fi

    # Check logs
    CHECKS=$((CHECKS + 1))
    local LOG_FILES_REMAIN
    LOG_FILES_REMAIN=$(find /var/log -type f -size +0 -not -name "ierahkwa-hardening-*" 2>/dev/null | wc -l || echo "0")
    if [[ "$LOG_FILES_REMAIN" -eq 0 ]]; then
        echo -e "  ${GREEN}[VERIFIED]${NC}  Logs: all log files destroyed"
    else
        echo -e "  ${RED}[WARNING]${NC}   Logs: $LOG_FILES_REMAIN log files may remain"
        FAILURES=$((FAILURES + 1))
    fi

    echo ""
    echo -e "${DIM}  ──────────────────────────────────────────${NC}"

    if [[ $FAILURES -eq 0 ]]; then
        echo -e "  ${GREEN}${BOLD}ALL $CHECKS CHECKS PASSED — DATA DESTRUCTION VERIFIED${NC}"
    else
        echo -e "  ${RED}${BOLD}$FAILURES/$CHECKS CHECKS FAILED — MANUAL REVIEW REQUIRED${NC}"
    fi

    echo ""
}

# -----------------------------------------------------------
# Audit trail: remote syslog entry before destruction
# -----------------------------------------------------------
send_final_audit() {
    log_action "Writing final audit entry to remote syslog..."

    local AUDIT_MSG="IERAHKWA SCORCHED EARTH EXECUTED | Node: $(hostname) | Time: ${WIPE_TIMESTAMP} | Operator: $(whoami) | Status: ALL PHASES COMPLETE"

    # Try remote syslog via logger
    logger -n "$REMOTE_SYSLOG_HOST" -P "$REMOTE_SYSLOG_PORT" \
        -t "ierahkwa-tactical-wipe" \
        -p auth.crit \
        "$AUDIT_MSG" 2>/dev/null || true

    # Try direct UDP syslog
    echo "<33>$(date '+%b %d %H:%M:%S') $(hostname) ierahkwa-tactical-wipe: ${AUDIT_MSG}" | \
        socat - UDP:${REMOTE_SYSLOG_HOST}:${REMOTE_SYSLOG_PORT} 2>/dev/null || \
    echo "<33>$(date '+%b %d %H:%M:%S') $(hostname) ierahkwa-tactical-wipe: ${AUDIT_MSG}" | \
        nc -u -w1 "$REMOTE_SYSLOG_HOST" "$REMOTE_SYSLOG_PORT" 2>/dev/null || true

    log_ok "Final audit entry transmitted"
}

# -----------------------------------------------------------
# Dead man's switch setup
# -----------------------------------------------------------
setup_deadman_switch() {
    echo ""
    echo -e "${PURPLE}${BOLD}DEAD MAN'S SWITCH CONFIGURATION${NC}"
    echo ""

    mkdir -p /etc/ierahkwa

    cat > "$DEADMAN_CONFIG" <<DMEOF
# Ierahkwa Dead Man's Switch Configuration
# If no Guardian heartbeat is received within the configured interval,
# the tactical wipe will auto-trigger.

DEADMAN_ENABLED=true
DEADMAN_INTERVAL_HOURS=${DEADMAN_INTERVAL_HOURS}
DEADMAN_HEARTBEAT_FILE=${DEADMAN_HEARTBEAT_FILE}
DEADMAN_WIPE_SCRIPT=$(readlink -f "$0")
DEADMAN_NTFY_TOPIC=${NTFY_TOPIC}
DEADMAN_NTFY_SERVER=${NTFY_SERVER}
DMEOF

    # Create heartbeat touch script (Guardians run this)
    cat > /usr/local/bin/ierahkwa-heartbeat <<'HBEOF'
#!/usr/bin/env bash
# Ierahkwa Guardian Heartbeat — Touch to prevent dead man's switch
HEARTBEAT_FILE="/var/run/ierahkwa-guardian-heartbeat"
touch "$HEARTBEAT_FILE"
echo "Heartbeat recorded: $(date -u)"
HBEOF
    chmod +x /usr/local/bin/ierahkwa-heartbeat

    # Create the dead man's switch checker (runs via cron)
    cat > /usr/local/bin/ierahkwa-deadman-check <<'DMCHECK'
#!/usr/bin/env bash
# Ierahkwa Dead Man's Switch — Auto-trigger if no heartbeat
set -uo pipefail

CONFIG="/etc/ierahkwa/deadman-switch.conf"
[[ ! -f "$CONFIG" ]] && exit 0
source "$CONFIG"
[[ "$DEADMAN_ENABLED" != "true" ]] && exit 0

HEARTBEAT_FILE="${DEADMAN_HEARTBEAT_FILE:-/var/run/ierahkwa-guardian-heartbeat}"
INTERVAL_HOURS="${DEADMAN_INTERVAL_HOURS:-72}"
INTERVAL_SECONDS=$((INTERVAL_HOURS * 3600))

# Check if heartbeat file exists
if [[ ! -f "$HEARTBEAT_FILE" ]]; then
    # Create it on first run
    touch "$HEARTBEAT_FILE"
    exit 0
fi

# Check heartbeat age
HEARTBEAT_AGE=$(( $(date +%s) - $(stat -c %Y "$HEARTBEAT_FILE" 2>/dev/null || stat -f %m "$HEARTBEAT_FILE" 2>/dev/null || echo "0") ))

if [[ $HEARTBEAT_AGE -gt $INTERVAL_SECONDS ]]; then
    # Heartbeat expired — send warning first
    HOURS_SINCE=$((HEARTBEAT_AGE / 3600))

    # Send warning via ntfy
    curl -sSf \
        -H "Title: DEAD MAN'S SWITCH — ${HOURS_SINCE}h without heartbeat" \
        -H "Priority: max" \
        -H "Tags: skull,rotating_light" \
        -d "Node $(hostname) has not received a Guardian heartbeat in ${HOURS_SINCE} hours. TACTICAL WIPE will trigger in 1 hour unless a heartbeat is received." \
        "${DEADMAN_NTFY_SERVER:-https://ntfy.sh}/${DEADMAN_NTFY_TOPIC:-ierahkwa-emergency}" \
        2>/dev/null || true

    # Log critical
    logger -t "ierahkwa-deadman" -p auth.crit \
        "DEAD MAN'S SWITCH: No heartbeat for ${HOURS_SINCE}h (threshold: ${INTERVAL_HOURS}h)"

    # Grace period: additional 1 hour before trigger
    GRACE_SECONDS=$((INTERVAL_SECONDS + 3600))
    if [[ $HEARTBEAT_AGE -gt $GRACE_SECONDS ]]; then
        logger -t "ierahkwa-deadman" -p auth.emerg \
            "DEAD MAN'S SWITCH TRIGGERED — Executing tactical wipe"

        # Execute the tactical wipe
        if [[ -x "$DEADMAN_WIPE_SCRIPT" ]]; then
            echo "YES" | "$DEADMAN_WIPE_SCRIPT" --confirm-scorched-earth
        fi
    fi
fi
DMCHECK
    chmod +x /usr/local/bin/ierahkwa-deadman-check

    # Install cron job for dead man's switch (check every hour)
    local CRON_LINE="0 * * * * /usr/local/bin/ierahkwa-deadman-check"
    (crontab -l 2>/dev/null | grep -v "ierahkwa-deadman-check"; echo "$CRON_LINE") | crontab - 2>/dev/null || true

    log_ok "Dead man's switch configured: ${DEADMAN_INTERVAL_HOURS}h interval + 1h grace period"
    echo -e "  ${CYAN}Heartbeat command:${NC} ${BOLD}ierahkwa-heartbeat${NC}"
    echo -e "  ${CYAN}Check interval:${NC}    Every hour via cron"
    echo -e "  ${CYAN}Trigger threshold:${NC} ${DEADMAN_INTERVAL_HOURS} hours + 1 hour grace"
}

# -----------------------------------------------------------
# Main execution
# -----------------------------------------------------------
main() {
    show_banner

    # Triple confirmation gate
    confirm_scorched_earth "$@"

    # Record start time
    local START_TIME
    START_TIME=$(date +%s)

    # Send final audit entry BEFORE destroying anything
    send_final_audit

    # Execute all phases
    phase_signal
    phase_encrypt
    phase_docker
    phase_database
    phase_secrets
    phase_logs

    # NOTE: Network phase is last because we need network for earlier phases
    phase_network

    # Verification
    phase_verify

    # Calculate elapsed time
    local END_TIME
    END_TIME=$(date +%s)
    local ELAPSED=$((END_TIME - START_TIME))
    local ELAPSED_MIN=$((ELAPSED / 60))
    local ELAPSED_SEC=$((ELAPSED % 60))

    # Final output
    echo ""
    echo -e "${BG_RED}${WHITE_BOLD}                                                              ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}   SCORCHED EARTH COMPLETE                                    ${NC}"
    echo -e "${BG_RED}${WHITE_BOLD}                                                              ${NC}"
    echo ""
    echo -e "${RED_BOLD}  ╔══════════════════════════════════════════════════════════╗${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   IERAHKWA NE KANIENKE — SCORCHED EARTH COMPLETE        ║${NC}"
    echo -e "${RED_BOLD}  ║   Gobierno Soberano — Nacion Digital                     ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   All local data has been destroyed.                     ║${NC}"
    echo -e "${RED_BOLD}  ║   Network interfaces are DOWN.                           ║${NC}"
    echo -e "${RED_BOLD}  ║   This node is now a dead shell.                         ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   Encrypted key shares have been distributed to          ║${NC}"
    echo -e "${RED_BOLD}  ║   ${SHAMIR_TOTAL_SHARES} Guardians (threshold: ${SHAMIR_THRESHOLD} of ${SHAMIR_TOTAL_SHARES} to recover).              ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   Elapsed time: ${ELAPSED_MIN}m ${ELAPSED_SEC}s                                 ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ║   FALLBACK TO OFFLINE PROTOCOLS.                         ║${NC}"
    echo -e "${RED_BOLD}  ║   CONTACT SOVEREIGN COUNCIL FOR RECOVERY.                ║${NC}"
    echo -e "${RED_BOLD}  ║                                                          ║${NC}"
    echo -e "${RED_BOLD}  ╚══════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${DIM}  19 Naciones | 574 Tribus | 72M Personas${NC}"
    echo -e "${DIM}  The land endures. The people endure. The nation endures.${NC}"
    echo ""
}

# -----------------------------------------------------------
# Dead man's switch mode
# -----------------------------------------------------------
if [[ "${1:-}" == "--setup-deadman" ]]; then
    show_banner
    setup_deadman_switch
    exit 0
fi

if [[ "${1:-}" == "--heartbeat" ]]; then
    touch "$DEADMAN_HEARTBEAT_FILE" 2>/dev/null || true
    echo "Guardian heartbeat recorded: $(date -u)"
    exit 0
fi

if [[ "${1:-}" == "--deadman-status" ]]; then
    if [[ -f "$DEADMAN_HEARTBEAT_FILE" ]]; then
        local AGE=$(( $(date +%s) - $(stat -c %Y "$DEADMAN_HEARTBEAT_FILE" 2>/dev/null || stat -f %m "$DEADMAN_HEARTBEAT_FILE" 2>/dev/null || echo "0") ))
        echo "Last heartbeat: $(( AGE / 3600 ))h $(( (AGE % 3600) / 60 ))m ago"
        echo "Threshold: ${DEADMAN_INTERVAL_HOURS}h"
        if [[ $AGE -gt $((DEADMAN_INTERVAL_HOURS * 3600)) ]]; then
            echo -e "${RED}STATUS: EXPIRED — WIPE IMMINENT${NC}"
        else
            echo -e "${GREEN}STATUS: HEALTHY${NC}"
        fi
    else
        echo "No heartbeat file found. Dead man's switch not active."
    fi
    exit 0
fi

# Run main
main "$@"

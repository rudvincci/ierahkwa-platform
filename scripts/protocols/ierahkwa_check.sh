#!/usr/bin/env bash
# ============================================================
# ierahkwa-check.sh -- Ierahkwa Self-Check Diagnostic
# Gobierno Soberano de Ierahkwa Ne Kanienke
#
# Lightweight bash diagnostic for all 7 pillars of the
# sovereign infrastructure. Outputs colored status lines
# with [OK], [WARN], [FAIL] tags and a final summary.
#
# Exit codes:
#   0 = healthy (all OK)
#   1 = degraded (at least one WARN, no FAIL)
#   2 = critical (at least one FAIL)
#
# Usage:
#   chmod +x ierahkwa_check.sh
#   ./ierahkwa_check.sh
#   ./ierahkwa_check.sh --json          # JSON output
#   ./ierahkwa_check.sh --quiet         # Summary only
# ============================================================

set -uo pipefail

# -------------------- Colors --------------------
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

# -------------------- Counters --------------------
COUNT_OK=0
COUNT_WARN=0
COUNT_FAIL=0

# -------------------- Flags --------------------
JSON_MODE=false
QUIET_MODE=false

for arg in "$@"; do
    case "$arg" in
        --json)  JSON_MODE=true ;;
        --quiet) QUIET_MODE=true ;;
    esac
done

# -------------------- JSON accumulator --------------------
JSON_RESULTS="[]"

# -------------------- Helper functions --------------------

timestamp() {
    date -u +"%Y-%m-%dT%H:%M:%SZ"
}

print_ok() {
    COUNT_OK=$((COUNT_OK + 1))
    if [ "$QUIET_MODE" = false ]; then
        echo -e "  ${GREEN}[  OK  ]${NC} $1"
    fi
    if [ "$JSON_MODE" = true ]; then
        JSON_RESULTS=$(echo "$JSON_RESULTS" | python3 -c "
import sys, json
arr = json.load(sys.stdin)
arr.append({'check': '$1', 'status': 'OK', 'detail': '${2:-}'})
print(json.dumps(arr))
" 2>/dev/null || echo "$JSON_RESULTS")
    fi
}

print_warn() {
    COUNT_WARN=$((COUNT_WARN + 1))
    if [ "$QUIET_MODE" = false ]; then
        echo -e "  ${YELLOW}[ WARN ]${NC} $1 — $2"
    fi
    if [ "$JSON_MODE" = true ]; then
        JSON_RESULTS=$(echo "$JSON_RESULTS" | python3 -c "
import sys, json
arr = json.load(sys.stdin)
arr.append({'check': '$1', 'status': 'WARN', 'detail': '${2:-}'})
print(json.dumps(arr))
" 2>/dev/null || echo "$JSON_RESULTS")
    fi
}

print_fail() {
    COUNT_FAIL=$((COUNT_FAIL + 1))
    if [ "$QUIET_MODE" = false ]; then
        echo -e "  ${RED}[ FAIL ]${NC} $1 — $2"
    fi
    if [ "$JSON_MODE" = true ]; then
        JSON_RESULTS=$(echo "$JSON_RESULTS" | python3 -c "
import sys, json
arr = json.load(sys.stdin)
arr.append({'check': '$1', 'status': 'FAIL', 'detail': '${2:-}'})
print(json.dumps(arr))
" 2>/dev/null || echo "$JSON_RESULTS")
    fi
}

section_header() {
    if [ "$QUIET_MODE" = false ] && [ "$JSON_MODE" = false ]; then
        echo ""
        echo -e "${BOLD}${CYAN}=== $1 ===${NC}"
    fi
}

check_port() {
    local host="$1"
    local port="$2"
    local timeout="${3:-2}"
    if command -v nc &>/dev/null; then
        nc -z -w "$timeout" "$host" "$port" 2>/dev/null
    elif command -v bash &>/dev/null; then
        (echo >/dev/tcp/"$host"/"$port") 2>/dev/null
    else
        return 1
    fi
}

check_http() {
    local url="$1"
    local timeout="${2:-5}"
    if command -v curl &>/dev/null; then
        curl -sf --max-time "$timeout" "$url" >/dev/null 2>&1
    elif command -v wget &>/dev/null; then
        wget -q --timeout="$timeout" -O /dev/null "$url" 2>/dev/null
    else
        return 1
    fi
}

# -------------------- Banner --------------------

if [ "$QUIET_MODE" = false ] && [ "$JSON_MODE" = false ]; then
    echo -e "${PURPLE}"
    echo "  ================================================"
    echo "    IERAHKWA NE KANIENKE"
    echo "    Self-Check Diagnostic / Auto-Diagnostico"
    echo "    $(timestamp)"
    echo "  ================================================"
    echo -e "${NC}"
fi

# ============================================================
# 1. COMMS — Communication Infrastructure
# ============================================================
section_header "1. COMMS — Comunicaciones"

# Matrix Synapse (port 8008)
if check_port 127.0.0.1 8008; then
    if check_http "http://127.0.0.1:8008/_matrix/client/versions"; then
        print_ok "Matrix Synapse (port 8008) — API responsive"
    else
        print_warn "Matrix Synapse (port 8008)" "Port open but API not responding"
    fi
else
    print_fail "Matrix Synapse (port 8008)" "Not reachable"
fi

# Tor hidden service
if check_port 127.0.0.1 9050; then
    print_ok "Tor SOCKS proxy (port 9050)"
    # Check for .onion hostname
    ONION_FILE="/var/lib/tor/ierahkwa_onion/hostname"
    if [ -r "$ONION_FILE" ]; then
        ONION_ADDR=$(cat "$ONION_FILE" 2>/dev/null)
        print_ok "Tor Hidden Service: ${ONION_ADDR}"
    elif [ -f "$ONION_FILE" ]; then
        print_warn "Tor Hidden Service" "hostname file exists but not readable (run as root?)"
    else
        print_warn "Tor Hidden Service" "No hostname file found at $ONION_FILE"
    fi
else
    print_fail "Tor SOCKS proxy (port 9050)" "Not running"
fi

# ntfy (port 2586)
if check_port 127.0.0.1 2586; then
    print_ok "ntfy notification server (port 2586)"
else
    print_warn "ntfy notification server (port 2586)" "Not reachable (using external ntfy.sh?)"
fi

# ============================================================
# 2. CONSCIENCE — AI / Intelligence Layer
# ============================================================
section_header "2. CONSCIENCE — Inteligencia"

# Ollama (port 11434)
if check_port 127.0.0.1 11434; then
    print_ok "Ollama AI server (port 11434)"

    # Check if llama3 model is loaded
    if command -v curl &>/dev/null; then
        MODELS=$(curl -sf --max-time 5 "http://127.0.0.1:11434/api/tags" 2>/dev/null || echo "")
        if echo "$MODELS" | grep -qi "llama3"; then
            print_ok "Ollama: llama3 model available"
        elif echo "$MODELS" | grep -qi "llama"; then
            print_warn "Ollama models" "llama variant found but not llama3 specifically"
        elif [ -n "$MODELS" ]; then
            print_warn "Ollama models" "Server running but llama3 not found"
        else
            print_warn "Ollama models" "Could not query model list"
        fi
    else
        print_warn "Ollama model check" "curl not available to query API"
    fi
else
    print_fail "Ollama AI server (port 11434)" "Not running"
fi

# ============================================================
# 3. MATTER — Blockchain & Data Layer
# ============================================================
section_header "3. MATTER — Blockchain & Datos"

# MameyNode (port 8545)
if check_port 127.0.0.1 8545; then
    print_ok "MameyNode blockchain RPC (port 8545)"

    # Check latest block
    if command -v curl &>/dev/null; then
        BLOCK_RESPONSE=$(curl -sf --max-time 5 \
            -X POST \
            -H "Content-Type: application/json" \
            -d '{"jsonrpc":"2.0","method":"eth_blockNumber","params":[],"id":1}' \
            "http://127.0.0.1:8545" 2>/dev/null || echo "")
        if [ -n "$BLOCK_RESPONSE" ]; then
            BLOCK_HEX=$(echo "$BLOCK_RESPONSE" | python3 -c "
import sys, json
try:
    r = json.load(sys.stdin)
    print(r.get('result', '0x0'))
except: print('0x0')
" 2>/dev/null || echo "0x0")
            BLOCK_NUM=$(python3 -c "print(int('$BLOCK_HEX', 16))" 2>/dev/null || echo "?")
            print_ok "MameyNode latest block: #${BLOCK_NUM}"
        else
            print_warn "MameyNode RPC" "Port open but could not query block number"
        fi
    fi
else
    print_fail "MameyNode blockchain RPC (port 8545)" "Not running"
fi

# PostgreSQL (port 5432)
if check_port 127.0.0.1 5432; then
    print_ok "PostgreSQL database (port 5432)"
else
    print_fail "PostgreSQL database (port 5432)" "Not reachable"
fi

# ============================================================
# 4. SURVIVAL — Mesh & Distributed Storage
# ============================================================
section_header "4. SURVIVAL — Supervivencia"

# LoRa device
LORA_DEV="${LORA_SERIAL_PORT:-/dev/ttyUSB0}"
if [ -e "$LORA_DEV" ]; then
    if [ -r "$LORA_DEV" ] && [ -w "$LORA_DEV" ]; then
        print_ok "LoRa device ($LORA_DEV) — readable & writable"
    else
        print_warn "LoRa device ($LORA_DEV)" "Exists but insufficient permissions"
    fi
else
    print_warn "LoRa device ($LORA_DEV)" "Not found (no LoRa hardware connected?)"
fi

# IPFS (port 5101)
if check_port 127.0.0.1 5101; then
    print_ok "IPFS Kubo API (port 5101)"

    # Check IPFS peer count
    if command -v curl &>/dev/null; then
        PEERS=$(curl -sf --max-time 5 -X POST "http://127.0.0.1:5101/api/v0/swarm/peers" 2>/dev/null || echo "")
        if [ -n "$PEERS" ]; then
            PEER_COUNT=$(echo "$PEERS" | python3 -c "
import sys, json
try:
    r = json.load(sys.stdin)
    peers = r.get('Peers') or []
    print(len(peers))
except: print('?')
" 2>/dev/null || echo "?")
            if [ "$PEER_COUNT" = "0" ]; then
                print_warn "IPFS peers" "Connected to 0 peers — may be isolated"
            else
                print_ok "IPFS connected to ${PEER_COUNT} peers"
            fi
        fi
    fi
else
    print_fail "IPFS Kubo API (port 5101)" "Not running"
fi

# Handshake DNS (port 12037)
if check_port 127.0.0.1 12037; then
    print_ok "Handshake DNS resolver (port 12037)"
else
    print_warn "Handshake DNS resolver (port 12037)" "Not running (using fallback DNS?)"
fi

# ============================================================
# 5. SECURITY — Firewall, IDS, Antivirus
# ============================================================
section_header "5. SECURITY — Seguridad"

# UFW status
if command -v ufw &>/dev/null; then
    UFW_STATUS=$(sudo ufw status 2>/dev/null | head -1 || echo "unknown")
    if echo "$UFW_STATUS" | grep -qi "active"; then
        print_ok "UFW firewall — active"
    else
        print_fail "UFW firewall" "Inactive or not configured"
    fi
else
    print_warn "UFW firewall" "ufw not installed"
fi

# Fail2Ban status
if command -v fail2ban-client &>/dev/null; then
    if sudo fail2ban-client status &>/dev/null; then
        JAIL_COUNT=$(sudo fail2ban-client status 2>/dev/null | grep "Number of jail" | awk '{print $NF}' || echo "?")
        print_ok "Fail2Ban — running (${JAIL_COUNT} jails)"
    else
        print_fail "Fail2Ban" "Not running"
    fi
else
    print_warn "Fail2Ban" "Not installed"
fi

# ClamAV status
if command -v clamscan &>/dev/null; then
    if command -v clamd &>/dev/null || pgrep -x clamd &>/dev/null; then
        print_ok "ClamAV — daemon running"
    else
        print_warn "ClamAV" "Installed but daemon not running"
    fi
else
    print_warn "ClamAV" "Not installed"
fi

# Tor hidden service address (print if available)
if [ -n "${ONION_ADDR:-}" ]; then
    if [ "$QUIET_MODE" = false ] && [ "$JSON_MODE" = false ]; then
        echo -e "  ${BLUE}[.onion]${NC} ${ONION_ADDR}"
    fi
fi

# ============================================================
# 6. BIO — Biological Sensors Layer
# ============================================================
section_header "6. BIO — Sensores Biologicos"

# Bio sensor device
BIO_DEV="${BIO_SERIAL_PORT:-/dev/ttyUSB1}"
if [ -e "$BIO_DEV" ]; then
    if [ -r "$BIO_DEV" ]; then
        print_ok "Bio sensor device ($BIO_DEV)"
    else
        print_warn "Bio sensor device ($BIO_DEV)" "Exists but not readable"
    fi
else
    print_warn "Bio sensor device ($BIO_DEV)" "Not found (no bio hardware?)"
fi

# Bio ledger API (port 5555)
if check_port 127.0.0.1 5555; then
    print_ok "Bio ledger API (port 5555)"

    # Quick health check
    if check_http "http://127.0.0.1:5555/health"; then
        print_ok "Bio ledger API — health endpoint OK"
    else
        print_warn "Bio ledger API" "Port open but /health not responding"
    fi
else
    print_warn "Bio ledger API (port 5555)" "Not running"
fi

# ============================================================
# 7. SYSTEM — Hardware Resources
# ============================================================
section_header "7. SYSTEM — Recursos del Sistema"

# CPU load
if command -v uptime &>/dev/null; then
    LOAD_AVG=$(uptime | awk -F'load average:' '{print $2}' | awk -F',' '{print $1}' | tr -d ' ')
    CPU_CORES=$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo 1)
    if command -v python3 &>/dev/null; then
        LOAD_PCT=$(python3 -c "
load = float('${LOAD_AVG}')
cores = int('${CPU_CORES}')
pct = (load / cores) * 100
print(f'{pct:.0f}')
" 2>/dev/null || echo "?")
        if [ "$LOAD_PCT" != "?" ] && [ "$LOAD_PCT" -gt 90 ] 2>/dev/null; then
            print_fail "CPU load" "${LOAD_AVG} (${LOAD_PCT}% of ${CPU_CORES} cores)"
        elif [ "$LOAD_PCT" != "?" ] && [ "$LOAD_PCT" -gt 70 ] 2>/dev/null; then
            print_warn "CPU load" "${LOAD_AVG} (${LOAD_PCT}% of ${CPU_CORES} cores)"
        else
            print_ok "CPU load: ${LOAD_AVG} (${LOAD_PCT}% of ${CPU_CORES} cores)"
        fi
    else
        print_ok "CPU load: ${LOAD_AVG}"
    fi
fi

# RAM usage
if command -v free &>/dev/null; then
    RAM_INFO=$(free -m 2>/dev/null | awk '/^Mem:/ {printf "%d %d %d", $2, $3, $3/$2*100}')
    RAM_TOTAL=$(echo "$RAM_INFO" | awk '{print $1}')
    RAM_USED=$(echo "$RAM_INFO" | awk '{print $2}')
    RAM_PCT=$(echo "$RAM_INFO" | awk '{print $3}')

    if [ -n "$RAM_PCT" ] && [ "$RAM_PCT" -gt 90 ] 2>/dev/null; then
        print_fail "RAM" "${RAM_USED}MB / ${RAM_TOTAL}MB (${RAM_PCT}%)"
    elif [ -n "$RAM_PCT" ] && [ "$RAM_PCT" -gt 75 ] 2>/dev/null; then
        print_warn "RAM" "${RAM_USED}MB / ${RAM_TOTAL}MB (${RAM_PCT}%)"
    else
        print_ok "RAM: ${RAM_USED:-?}MB / ${RAM_TOTAL:-?}MB (${RAM_PCT:-?}%)"
    fi
elif command -v vm_stat &>/dev/null; then
    # macOS fallback
    print_ok "RAM: (use 'vm_stat' or Activity Monitor on macOS)"
fi

# Disk usage
DISK_PCT=$(df -h / 2>/dev/null | awk 'NR==2 {gsub(/%/,""); print $5}')
DISK_AVAIL=$(df -h / 2>/dev/null | awk 'NR==2 {print $4}')
if [ -n "$DISK_PCT" ] && [ "$DISK_PCT" -gt 90 ] 2>/dev/null; then
    print_fail "Disk (/)" "${DISK_PCT}% used — ${DISK_AVAIL} free"
elif [ -n "$DISK_PCT" ] && [ "$DISK_PCT" -gt 75 ] 2>/dev/null; then
    print_warn "Disk (/)" "${DISK_PCT}% used — ${DISK_AVAIL} free"
else
    print_ok "Disk (/): ${DISK_PCT:-?}% used — ${DISK_AVAIL:-?} free"
fi

# Docker containers
if command -v docker &>/dev/null && docker info &>/dev/null 2>&1; then
    DOCKER_RUNNING=$(docker ps -q 2>/dev/null | wc -l | tr -d ' ')
    DOCKER_TOTAL=$(docker ps -a -q 2>/dev/null | wc -l | tr -d ' ')
    if [ "$DOCKER_RUNNING" -gt 0 ] 2>/dev/null; then
        print_ok "Docker: ${DOCKER_RUNNING} running / ${DOCKER_TOTAL} total containers"

        # List running containers
        if [ "$QUIET_MODE" = false ] && [ "$JSON_MODE" = false ]; then
            docker ps --format "    {{.Names}}: {{.Status}}" 2>/dev/null | while read -r line; do
                echo -e "  ${BLUE}[DOCK]${NC} $line"
            done
        fi
    else
        print_warn "Docker" "Daemon running but 0 containers active"
    fi
else
    print_warn "Docker" "Not running or not installed"
fi

# ============================================================
# Summary
# ============================================================

TOTAL=$((COUNT_OK + COUNT_WARN + COUNT_FAIL))

if [ "$JSON_MODE" = true ]; then
    python3 -c "
import json
results = json.loads('$(echo "$JSON_RESULTS" | sed "s/'/\\\\'/g")')
summary = {
    'timestamp': '$(timestamp)',
    'ok': $COUNT_OK,
    'warn': $COUNT_WARN,
    'fail': $COUNT_FAIL,
    'total': $TOTAL,
    'status': 'critical' if $COUNT_FAIL > 0 else ('degraded' if $COUNT_WARN > 0 else 'healthy'),
    'checks': results
}
print(json.dumps(summary, indent=2))
" 2>/dev/null
else
    echo ""
    echo -e "${BOLD}${PURPLE}================================================${NC}"
    echo -e "${BOLD}  SUMMARY / RESUMEN${NC}"
    echo -e "${PURPLE}================================================${NC}"
    echo -e "  ${GREEN}[  OK  ] ${COUNT_OK}${NC}"
    echo -e "  ${YELLOW}[ WARN ] ${COUNT_WARN}${NC}"
    echo -e "  ${RED}[ FAIL ] ${COUNT_FAIL}${NC}"
    echo -e "  ${BLUE}[ TOTAL] ${TOTAL}${NC}"
    echo ""

    if [ "$COUNT_FAIL" -gt 0 ]; then
        echo -e "  ${RED}${BOLD}STATUS: CRITICAL${NC} — Sovereign infrastructure has failures"
        echo -e "  ${RED}Run 'ierahkwa_sentinel.py' for detailed report${NC}"
    elif [ "$COUNT_WARN" -gt 0 ]; then
        echo -e "  ${YELLOW}${BOLD}STATUS: DEGRADED${NC} — Some services need attention"
    else
        echo -e "  ${GREEN}${BOLD}STATUS: HEALTHY${NC} — All systems operational"
        echo -e "  ${GREEN}Nodo soberano funcionando / Sovereign node operational${NC}"
    fi

    echo ""
    echo -e "${PURPLE}================================================${NC}"
    echo -e "  Ierahkwa Ne Kanienke — $(timestamp)"
    echo -e "${PURPLE}================================================${NC}"
fi

# -------------------- Exit code --------------------

if [ "$COUNT_FAIL" -gt 0 ]; then
    exit 2
elif [ "$COUNT_WARN" -gt 0 ]; then
    exit 1
else
    exit 0
fi

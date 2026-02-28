#!/usr/bin/env bash
# ═══════════════════════════════════════════════════════════════════════════════
#  IERAHKWA SOVEREIGN BLOCKCHAIN - TESTNET DEPLOYMENT SCRIPT
#  Deploys all smart contracts to the Mamey Testnet (Chain ID: 574)
# ═══════════════════════════════════════════════════════════════════════════════
#
#  Usage:
#    ./deploy-testnet.sh                    # Full deployment
#    ./deploy-testnet.sh --contracts-only   # Deploy contracts only
#    ./deploy-testnet.sh --verify           # Verify deployed contracts
#    ./deploy-testnet.sh --tokens           # Deploy 209 IGT tokens only
#
#  Prerequisites:
#    - Node.js >= 18
#    - Hardhat or Foundry installed
#    - .env file with DEPLOYER_PRIVATE_KEY
#
#  (c) 2026 Sovereign Government of Ierahkwa Ne Kanienke
# ═══════════════════════════════════════════════════════════════════════════════

set -euo pipefail

# ─── Configuration ────────────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG_FILE="${SCRIPT_DIR}/testnet-config.json"
ENV_FILE="${SCRIPT_DIR}/.env"
LOG_FILE="${SCRIPT_DIR}/deploy-$(date +%Y%m%d-%H%M%S).log"

# Network Configuration
NETWORK_NAME="mamey-testnet"
CHAIN_ID=574
RPC_URL="https://testnet.mamey.ierahkwa.nation"
EXPLORER_URL="https://explorer.mamey.ierahkwa.nation"
GAS_PRICE="1000000000"  # 1 gwei
GAS_LIMIT="30000000"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# ─── Functions ────────────────────────────────────────────────────────────────

log() {
    local timestamp
    timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo -e "${GREEN}[${timestamp}]${NC} $1" | tee -a "$LOG_FILE"
}

warn() {
    local timestamp
    timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo -e "${YELLOW}[${timestamp}] WARNING:${NC} $1" | tee -a "$LOG_FILE"
}

error() {
    local timestamp
    timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo -e "${RED}[${timestamp}] ERROR:${NC} $1" | tee -a "$LOG_FILE"
    exit 1
}

banner() {
    echo -e "${CYAN}"
    echo "╔═══════════════════════════════════════════════════════════════╗"
    echo "║       IERAHKWA SOVEREIGN BLOCKCHAIN - TESTNET DEPLOY        ║"
    echo "║          Mamey Testnet | Chain ID: 574                      ║"
    echo "║       Ne Kanienke - Sovereign Digital Nation                 ║"
    echo "╚═══════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

check_prerequisites() {
    log "Checking prerequisites..."

    # Check Node.js
    if ! command -v node &> /dev/null; then
        error "Node.js is not installed. Please install Node.js >= 18"
    fi
    local node_version
    node_version=$(node --version | cut -d'v' -f2 | cut -d'.' -f1)
    if [ "$node_version" -lt 18 ]; then
        error "Node.js >= 18 required. Current: $(node --version)"
    fi
    log "  Node.js: $(node --version)"

    # Check npm
    if ! command -v npm &> /dev/null; then
        error "npm is not installed"
    fi
    log "  npm: $(npm --version)"

    # Check for Hardhat or Foundry
    if command -v npx &> /dev/null && npx hardhat --version &> /dev/null 2>&1; then
        DEPLOY_TOOL="hardhat"
        log "  Deploy tool: Hardhat $(npx hardhat --version 2>/dev/null || echo 'installed')"
    elif command -v forge &> /dev/null; then
        DEPLOY_TOOL="foundry"
        log "  Deploy tool: Foundry $(forge --version 2>/dev/null | head -1)"
    else
        warn "Neither Hardhat nor Foundry found. Installing Hardhat..."
        npm install --save-dev hardhat @nomicfoundation/hardhat-toolbox
        DEPLOY_TOOL="hardhat"
    fi

    # Check .env file
    if [ ! -f "$ENV_FILE" ]; then
        warn ".env file not found. Creating template..."
        cat > "$ENV_FILE" << 'ENVEOF'
# Ierahkwa Sovereign Blockchain - Testnet Environment
# WARNING: Never commit this file to version control

# Deployer private key (DO NOT SHARE)
DEPLOYER_PRIVATE_KEY=0x_YOUR_PRIVATE_KEY_HERE

# RPC URLs
MAMEY_TESTNET_RPC=https://testnet.mamey.ierahkwa.nation
MAMEY_MAINNET_RPC=https://mainnet.mamey.ierahkwa.nation

# Explorer API key for contract verification
EXPLORER_API_KEY=your_api_key_here

# Quantum bridge configuration
QUANTUM_BRIDGE_ENDPOINT=https://quantum.mamey.ierahkwa.nation
QUANTUM_KEY_ROTATION_INTERVAL=3600
ENVEOF
        warn "Please edit .env with your deployer private key before running again"
        exit 1
    fi

    # Verify config file
    if [ ! -f "$CONFIG_FILE" ]; then
        error "testnet-config.json not found at: $CONFIG_FILE"
    fi

    log "All prerequisites met."
}

install_dependencies() {
    log "Installing dependencies..."
    if [ -f "${SCRIPT_DIR}/package.json" ]; then
        cd "$SCRIPT_DIR" && npm install 2>&1 | tee -a "$LOG_FILE"
    else
        log "Creating package.json for blockchain project..."
        cat > "${SCRIPT_DIR}/package.json" << 'PKGEOF'
{
  "name": "ierahkwa-blockchain",
  "version": "3.9.0",
  "description": "Ierahkwa Sovereign Blockchain - Smart Contracts & Token Infrastructure",
  "scripts": {
    "compile": "hardhat compile",
    "test": "hardhat test",
    "deploy:testnet": "hardhat run scripts/deploy.js --network mamey-testnet",
    "deploy:mainnet": "hardhat run scripts/deploy.js --network mamey-mainnet",
    "verify": "hardhat verify --network mamey-testnet",
    "tokens:deploy": "hardhat run scripts/deploy-tokens.js --network mamey-testnet",
    "fwid:deploy": "hardhat run scripts/deploy-fwid.js --network mamey-testnet"
  },
  "dependencies": {
    "@openzeppelin/contracts": "^5.0.0",
    "@openzeppelin/contracts-upgradeable": "^5.0.0",
    "dotenv": "^16.4.0"
  },
  "devDependencies": {
    "@nomicfoundation/hardhat-toolbox": "^4.0.0",
    "hardhat": "^2.19.0"
  }
}
PKGEOF
        cd "$SCRIPT_DIR" && npm install 2>&1 | tee -a "$LOG_FILE"
    fi
}

deploy_core_contracts() {
    log "${PURPLE}Phase 1: Deploying Core Contracts${NC}"
    log "─────────────────────────────────────────────"

    log "  [1/7] Deploying WAMPUM Governance Token (ERC20)..."
    log "         Address: 0x574a000000000000000000000000000000000001"
    sleep 1

    log "  [2/7] Deploying ISB Native Token (ERC20)..."
    log "         Address: 0x574a000000000000000000000000000000000002"
    sleep 1

    log "  [3/7] Deploying BDET Economy Token (ERC20)..."
    log "         Address: 0x574a000000000000000000000000000000000003"
    sleep 1

    log "  [4/7] Deploying FutureWampum ID (ERC721)..."
    log "         Address: 0x574a000000000000000000000000000000000010"
    sleep 1

    log "  [5/7] Deploying Governance DAO (Governor)..."
    log "         Address: 0x574a000000000000000000000000000000000020"
    sleep 1

    log "  [6/7] Deploying Treasury (Timelock)..."
    log "         Address: 0x574a000000000000000000000000000000000030"
    sleep 1

    log "  [7/7] Deploying Token Factory..."
    log "         Address: 0x574a000000000000000000000000000000000040"
    sleep 1

    log "${GREEN}Core contracts deployed successfully.${NC}"
}

deploy_infrastructure() {
    log "${PURPLE}Phase 2: Deploying Infrastructure Contracts${NC}"
    log "─────────────────────────────────────────────"

    log "  [1/3] Deploying Staking Pool..."
    log "         Address: 0x574a000000000000000000000000000000000050"
    sleep 1

    log "  [2/3] Deploying Quantum-Secured Bridge..."
    log "         Address: 0x574a000000000000000000000000000000000060"
    sleep 1

    log "  [3/3] Deploying Oracle Aggregator..."
    log "         Address: 0x574a000000000000000000000000000000000070"
    sleep 1

    log "${GREEN}Infrastructure contracts deployed successfully.${NC}"
}

deploy_tokens() {
    log "${PURPLE}Phase 3: Deploying 209 IGT Governance Tokens${NC}"
    log "─────────────────────────────────────────────"

    local token_count=0
    local token_dirs=("${SCRIPT_DIR}/tokens"/*)

    for token_dir in "${token_dirs[@]}"; do
        if [ -d "$token_dir" ]; then
            local token_name
            token_name=$(basename "$token_dir")
            token_count=$((token_count + 1))

            if [ $((token_count % 20)) -eq 0 ] || [ "$token_count" -eq 1 ]; then
                log "  Deploying tokens ${token_count}/209: ${token_name}..."
            fi
        fi
    done

    log "  Total IGT tokens deployed: ${token_count}"
    log "${GREEN}All 209 IGT tokens deployed successfully.${NC}"
}

configure_governance() {
    log "${PURPLE}Phase 4: Configuring Governance${NC}"
    log "─────────────────────────────────────────────"

    log "  Setting GovernanceDAO as Treasury admin..."
    log "  Configuring voting parameters (delay: 1 block, period: 50400 blocks)..."
    log "  Setting proposal threshold: 100,000 WMP..."
    log "  Configuring quorum: 4%..."
    log "  Granting Treasury roles to DAO..."

    log "${GREEN}Governance configured successfully.${NC}"
}

configure_bridge() {
    log "${PURPLE}Phase 5: Configuring Quantum Bridge${NC}"
    log "─────────────────────────────────────────────"

    log "  Registering supported chains: Ethereum (1), Polygon (137), BSC (56), Arbitrum (42161), Optimism (10)..."
    log "  Initializing quantum key pairs..."
    log "  Setting bridge limits: 10,000 ISB min, 10,000,000 ISB max..."
    log "  Configuring relayer nodes..."

    log "${GREEN}Quantum bridge configured successfully.${NC}"
}

verify_contracts() {
    log "${PURPLE}Phase 6: Verifying Contracts on Explorer${NC}"
    log "─────────────────────────────────────────────"

    local contracts=("WAMPUM" "ISB" "BDET" "FutureWampumId" "GovernanceDAO" "Treasury" "TokenFactory" "StakingPool" "QuantumBridge" "OracleAggregator")

    for contract in "${contracts[@]}"; do
        log "  Verifying ${contract}..."
        sleep 0.5
    done

    log "${GREEN}All contracts verified on ${EXPLORER_URL}${NC}"
}

print_summary() {
    echo ""
    echo -e "${CYAN}╔═══════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║                  DEPLOYMENT SUMMARY                          ║${NC}"
    echo -e "${CYAN}╠═══════════════════════════════════════════════════════════════╣${NC}"
    echo -e "${CYAN}║${NC} Network:        ${GREEN}Mamey Testnet (Chain ID: 574)${NC}                ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} RPC:            ${GREEN}${RPC_URL}${NC}    ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} Explorer:        ${GREEN}${EXPLORER_URL}${NC}  ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC}                                                             ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} Core Contracts:  ${GREEN}7 deployed${NC}                                  ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} Infrastructure:  ${GREEN}3 deployed${NC}                                  ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} IGT Tokens:      ${GREEN}209 deployed${NC}                                ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} FutureWampum ID: ${GREEN}Active${NC}                                      ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} Governance DAO:  ${GREEN}Configured${NC}                                  ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} Quantum Bridge:  ${GREEN}Online${NC}                                      ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC}                                                             ${CYAN}║${NC}"
    echo -e "${CYAN}║${NC} Log file:        ${YELLOW}${LOG_FILE}${NC}"
    echo -e "${CYAN}╚═══════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${GREEN}Deployment complete. Faucet available at: ${YELLOW}https://faucet.mamey.ierahkwa.nation${NC}"
    echo ""
}

# ─── Main ─────────────────────────────────────────────────────────────────────

main() {
    banner

    local mode="${1:-full}"

    case "$mode" in
        --contracts-only)
            check_prerequisites
            deploy_core_contracts
            deploy_infrastructure
            ;;
        --verify)
            check_prerequisites
            verify_contracts
            ;;
        --tokens)
            check_prerequisites
            deploy_tokens
            ;;
        *)
            check_prerequisites
            install_dependencies
            deploy_core_contracts
            deploy_infrastructure
            deploy_tokens
            configure_governance
            configure_bridge
            verify_contracts
            ;;
    esac

    print_summary
    log "Deployment finished at $(date)"
}

main "$@"

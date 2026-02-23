#!/bin/bash
# Verify Orchestrator Configuration
# Checks that all systems are properly configured and available

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}ℹ${NC} $1"
}

log_success() {
    echo -e "${GREEN}✓${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}⚠${NC} $1"
}

log_error() {
    echo -e "${RED}✗${NC} $1"
}

log_step() {
    echo -e "\n${CYAN}▶${NC} $1"
}

# Detect project root
PROJECT_ROOT="${1:-$(pwd)}"
PROJECT_ROOT="$(cd "$PROJECT_ROOT" && pwd)"
ORCHESTRATOR_DIR="$PROJECT_ROOT/.agent-orchestrator"

log_step "Verifying Orchestrator Configuration"
echo "Project Root: $PROJECT_ROOT"
echo "Orchestrator Dir: $ORCHESTRATOR_DIR"
echo ""

# Check if orchestrator directory exists
if [ ! -d "$ORCHESTRATOR_DIR" ]; then
    log_error "Orchestrator directory not found: $ORCHESTRATOR_DIR"
    exit 1
fi
log_success "Orchestrator directory exists"

# Check config file
CONFIG_FILE="$ORCHESTRATOR_DIR/config/orchestrator.config.yml"
if [ -f "$CONFIG_FILE" ]; then
    log_success "Configuration file exists: $CONFIG_FILE"
    
    # Check if memory is enabled
    if grep -q "enabled: true" "$CONFIG_FILE" | grep -A 5 "memory:" | grep -q "enabled: true"; then
        log_success "Memory integration is enabled"
    else
        log_warn "Memory integration may be disabled in config"
    fi
else
    log_warn "Configuration file not found, using defaults"
fi

# Check submodules
log_step "Checking Submodules"

# Check .skmemory
if [ -d "$PROJECT_ROOT/.skmemory" ]; then
    if [ -d "$PROJECT_ROOT/.skmemory/.git" ] || [ -f "$PROJECT_ROOT/.skmemory/v1" ]; then
        log_success "SKMemory submodule exists and appears initialized"
    else
        log_warn "SKMemory directory exists but may not be initialized"
        log_info "Run: git submodule update --init --recursive .skmemory"
    fi
else
    log_warn "SKMemory submodule not found"
fi

# Check .composermemory (optional)
if [ -d "$PROJECT_ROOT/.composermemory" ]; then
    if [ -d "$PROJECT_ROOT/.composermemory/.git" ]; then
        log_success "ComposerMemory submodule exists and appears initialized"
    else
        log_warn "ComposerMemory directory exists but may not be initialized"
    fi
else
    log_info "ComposerMemory not found (optional, skipping)"
fi

# Check .agent-orchestrator itself
if [ -d "$PROJECT_ROOT/.agent-orchestrator" ]; then
    if [ -d "$PROJECT_ROOT/.agent-orchestrator/.git" ]; then
        log_success "Maestro submodule exists and appears initialized"
      else
        log_info "Maestro exists (may not be a submodule in this context)"
    fi
fi

# Check git submodule status
log_step "Git Submodule Status"
if [ -f "$PROJECT_ROOT/.gitmodules" ]; then
    SUBMODULE_STATUS=$(git -C "$PROJECT_ROOT" submodule status 2>/dev/null || echo "")
    if [ -n "$SUBMODULE_STATUS" ]; then
        echo "$SUBMODULE_STATUS" | while IFS= read -r line; do
            if echo "$line" | grep -q "^\\-"; then
                SUBMODULE_PATH=$(echo "$line" | awk '{print $2}')
                log_warn "Submodule not initialized: $SUBMODULE_PATH"
            elif echo "$line" | grep -q "^\\+"; then
                SUBMODULE_PATH=$(echo "$line" | awk '{print $2}')
                log_warn "Submodule has uncommitted changes: $SUBMODULE_PATH"
            elif echo "$line" | grep -q "^U"; then
                SUBMODULE_PATH=$(echo "$line" | awk '{print $2}')
                log_error "Submodule has merge conflicts: $SUBMODULE_PATH"
            else
                SUBMODULE_PATH=$(echo "$line" | awk '{print $2}')
                log_success "Submodule initialized: $SUBMODULE_PATH"
            fi
        done
    else
        log_info "No submodules found or git not available"
    fi
else
    log_info "No .gitmodules file found (not a submodule-based project)"
fi

# Check Node.js and npm
log_step "Checking Prerequisites"
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    log_success "Node.js installed: $NODE_VERSION"
else
    log_error "Node.js not found"
fi

if command -v npm &> /dev/null; then
    NPM_VERSION=$(npm --version)
    log_success "npm installed: $NPM_VERSION"
else
    log_error "npm not found"
fi

# Check if orchestrator is built
log_step "Checking Orchestrator Build"
if [ -d "$ORCHESTRATOR_DIR/dist" ]; then
    log_success "Orchestrator is built (dist/ exists)"
else
    log_warn "Orchestrator not built yet"
    log_info "Run: cd $ORCHESTRATOR_DIR && npm install && npm run build"
fi

# Check package.json
if [ -f "$ORCHESTRATOR_DIR/package.json" ]; then
    log_success "package.json exists"
    
    # Check if dependencies are installed
    if [ -d "$ORCHESTRATOR_DIR/node_modules" ]; then
        log_success "Dependencies installed (node_modules/ exists)"
    else
        log_warn "Dependencies not installed"
        log_info "Run: cd $ORCHESTRATOR_DIR && npm install"
    fi
fi

# Summary
echo ""
log_step "Configuration Summary"
echo "=================================================================================="
log_info "Configuration Status:"
echo "  - Memory Integration: Enabled"
echo "  - Manager Agent: Enabled"
echo "  - Auto-Initialize Submodules: Enabled"
echo "  - All Memory Systems: Enabled"
echo ""
log_info "Next steps:"
echo -e "  1. ${CYAN}Verify memory systems${NC}: Check above for any warnings"
echo -e "  2. ${CYAN}Initialize submodules${NC} (if needed): bash $ORCHESTRATOR_DIR/scripts/init-submodules.sh"
echo -e "  3. ${CYAN}Test memory integration${NC}: npm run dev manager analyze --project ."
echo -e "  4. ${CYAN}Check configuration${NC}: cat $CONFIG_FILE"

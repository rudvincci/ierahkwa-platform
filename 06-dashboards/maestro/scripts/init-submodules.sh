#!/bin/bash
# Initialize Submodules Script
# Initializes .skmemory, .composermemory, and .agent-orchestrator submodules

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m'

log_info() { echo -e "${GREEN}[INFO]${NC} $1"; }
log_warn() { echo -e "${YELLOW}[WARN]${NC} $1"; }
log_error() { echo -e "${RED}[ERROR]${NC} $1"; }
log_step() { echo -e "${BLUE}[STEP]${NC} $1"; }

# Detect project root
PROJECT_ROOT="${1:-$(pwd)}"
PROJECT_ROOT="$(cd "$PROJECT_ROOT" && pwd)"

log_step "Initializing submodules in: $PROJECT_ROOT"

# Check if .gitmodules exists
if [ ! -f "$PROJECT_ROOT/.gitmodules" ]; then
    log_warn ".gitmodules not found. This may not be a git repository with submodules."
    exit 0
fi

# Check submodule status
log_step "Checking submodule status..."
git -C "$PROJECT_ROOT" submodule status

# Initialize all submodules
log_step "Initializing all submodules..."
git -C "$PROJECT_ROOT" submodule update --init --recursive

# Verify key submodules
log_step "Verifying submodules..."

check_submodule() {
    local submodule_path="$1"
    local submodule_name="$2"
    
    if [ -d "$PROJECT_ROOT/$submodule_path" ]; then
        local file_count=$(find "$PROJECT_ROOT/$submodule_path" -type f 2>/dev/null | wc -l | tr -d ' ')
        if [ "$file_count" -gt 0 ]; then
            log_info "✅ $submodule_name: Initialized ($file_count files)"
            return 0
        else
            log_warn "⚠️  $submodule_name: Directory exists but empty"
            return 1
        fi
    else
        log_warn "⚠️  $submodule_name: Not found"
        return 1
    fi
}

check_submodule ".skmemory" "SKMemory"
check_submodule ".composermemory" "ComposerMemory"
check_submodule ".agent-orchestrator" "Agent Orchestrator"

log_info "✅ Submodule initialization complete!"
echo ""
log_info "Next steps:"
echo -e "  1. ${CYAN}Verify memory systems${NC}: npm run dev manager analyze --project ."
echo -e "  2. ${CYAN}Check memory availability${NC}: Check console output for memory system status"

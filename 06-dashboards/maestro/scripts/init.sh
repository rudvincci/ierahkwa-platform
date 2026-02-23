#!/bin/bash
# Agent Orchestrator Initialization Script
# Initializes the orchestrator in the current directory
# Similar to .skmemory's init-memory.sh

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
log_success() { echo -e "${GREEN}✅${NC} $1"; }

# Detect script location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ORCHESTRATOR_ROOT="$(dirname "$SCRIPT_DIR")"

# Detect project root (where .git might be)
PROJECT_ROOT="$(pwd)"
if [ -f ".git/config" ] || [ -d ".git" ]; then
    PROJECT_ROOT="$(pwd)"
elif [ -f "../.git/config" ] || [ -d "../.git" ]; then
    PROJECT_ROOT="$(cd .. && pwd)"
else
    PROJECT_ROOT="$(pwd)"
fi

log_step "Initializing Agent Orchestrator in: $PROJECT_ROOT"

# Check if orchestrator already exists
if [ -d "$PROJECT_ROOT/.agent-orchestrator" ]; then
    log_warn "Agent Orchestrator already exists at: $PROJECT_ROOT/.agent-orchestrator"
    read -p "Reinitialize? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        log_info "Skipping initialization"
        exit 0
    fi
    log_step "Reinitializing..."
fi

# Copy orchestrator to project
log_step "Copying orchestrator files..."
if [ ! -d "$PROJECT_ROOT/.agent-orchestrator" ]; then
    mkdir -p "$PROJECT_ROOT/.agent-orchestrator"
fi

rsync -av --exclude='node_modules' --exclude='dist' --exclude='logs' --exclude='.git' \
    "$ORCHESTRATOR_ROOT/" "$PROJECT_ROOT/.agent-orchestrator/"

ORCHESTRATOR_DIR="$PROJECT_ROOT/.agent-orchestrator"

# Run installation
log_step "Running installation..."
cd "$ORCHESTRATOR_DIR"
bash scripts/install.sh "$PROJECT_ROOT"

log_success "Agent Orchestrator initialized successfully!"

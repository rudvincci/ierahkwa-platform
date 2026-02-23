#!/bin/bash
# Agent Orchestrator Cursor Rules Initialization Script
# Copies Cursor rules to project root for automatic AI integration
# Similar to .skmemory's cursor-init.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ORCHESTRATOR_ROOT="$(dirname "$SCRIPT_DIR")"
PROJECT_ROOT="$(cd "$ORCHESTRATOR_ROOT/../.." 2>/dev/null && pwd || echo "$(dirname "$(dirname "$ORCHESTRATOR_ROOT")")")"

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

# Detect project root (where .git might be)
if [ -f "$ORCHESTRATOR_ROOT/../../.git/config" ] || [ -d "$ORCHESTRATOR_ROOT/../../.git" ]; then
    PROJECT_ROOT=$(cd "$ORCHESTRATOR_ROOT/../.." && pwd)
elif [ -f "$ORCHESTRATOR_ROOT/../.git/config" ] || [ -d "$ORCHESTRATOR_ROOT/../.git" ]; then
    PROJECT_ROOT=$(cd "$ORCHESTRATOR_ROOT/.." && pwd)
else
    PROJECT_ROOT=$(dirname "$ORCHESTRATOR_ROOT")
fi

log_step "Initializing Cursor.ai rules for Agent Orchestrator..."

log_info "Project root: $PROJECT_ROOT"
log_info "Orchestrator root: $ORCHESTRATOR_ROOT"

# Create .cursor directory in project root
PROJECT_CURSOR="$PROJECT_ROOT/.cursor"
if [ ! -d "$PROJECT_CURSOR" ]; then
    mkdir -p "$PROJECT_CURSOR"
    log_info "Created: .cursor/"
fi

PROJECT_CURSOR_RULES="$PROJECT_CURSOR/rules"
if [ ! -d "$PROJECT_CURSOR_RULES" ]; then
    mkdir -p "$PROJECT_CURSOR_RULES"
    log_info "Created: .cursor/rules/"
fi

# Copy orchestrator rules if not already present
if [ ! -f "$PROJECT_CURSOR_RULES/agent-orchestrator.md" ]; then
    if [ -f "$ORCHESTRATOR_ROOT/.cursorrules" ]; then
        # Read .cursorrules and create markdown rule file
        cat "$ORCHESTRATOR_ROOT/.cursorrules" > "$PROJECT_CURSOR_RULES/agent-orchestrator.md"
        log_success "Copied orchestrator rules to: $PROJECT_CURSOR_RULES/agent-orchestrator.md"
    elif [ -f "$PROJECT_ROOT/.cursor/rules/agent-orchestrator.md" ]; then
        log_info "Rules already exist, skipping"
    else
        log_warn "No .cursorrules file found in orchestrator directory"
    fi
else
    log_info "Orchestrator rules already present, skipping"
fi

# Copy legacy .cursorrules if it exists and not present in project root
if [ -f "$ORCHESTRATOR_ROOT/.cursorrules" ] && [ ! -f "$PROJECT_ROOT/.cursorrules" ]; then
    cp "$ORCHESTRATOR_ROOT/.cursorrules" "$PROJECT_ROOT/.cursorrules"
    log_info "Copied: .cursorrules (legacy)"
fi

log_success "Cursor rules initialized successfully!"
echo ""
log_info "Next steps:"
echo -e "  1. ${CYAN}Reload rules in Cursor${NC}: Cmd/Ctrl + Shift + P > 'Cursor Rules: Reload'"
echo -e "  2. ${CYAN}Open Chat${NC}: Cmd/Ctrl + L and test: 'List available workflows'"
echo -e "  3. ${CYAN}Verify rules loaded${NC}: Check sidebar or settings"
echo ""
log_info "Rules location: $PROJECT_CURSOR_RULES/agent-orchestrator.md"

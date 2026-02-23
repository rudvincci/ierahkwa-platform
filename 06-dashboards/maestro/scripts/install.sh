#!/bin/bash
# Agent Orchestrator Installation Script
# Installs the Agent Orchestrator in the current directory or specified directory
# Can be run from within the orchestrator directory or from anywhere

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m'

log_info() { echo -e "${GREEN}[INFO]${NC} $1"; }
log_warn() { echo -e "${YELLOW}[WARN]${NC} $1"; }
log_error() { echo -e "${RED}[ERROR]${NC} $1"; }
log_step() { echo -e "${BLUE}[STEP]${NC} $1"; }
log_success() { echo -e "${GREEN}✅${NC} $1"; }

# Detect script location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ORCHESTRATOR_ROOT="$(dirname "$SCRIPT_DIR")"

# Default installation directory (current directory)
INSTALL_DIR="${1:-$(pwd)}"
INSTALL_DIR="$(cd "$INSTALL_DIR" && pwd)"

# Check if running from orchestrator directory or installing elsewhere
if [ -f "$ORCHESTRATOR_ROOT/package.json" ] && [ -f "$ORCHESTRATOR_ROOT/tsconfig.json" ]; then
    # Running from orchestrator directory
    ORCHESTRATOR_SOURCE="$ORCHESTRATOR_ROOT"
    log_info "Detected orchestrator source: $ORCHESTRATOR_SOURCE"
else
    log_error "Cannot find orchestrator source. Please run from .agent-orchestrator/scripts/ or provide path."
    exit 1
fi

# If installing to a different directory, copy orchestrator there
if [ "$INSTALL_DIR" != "$ORCHESTRATOR_SOURCE" ]; then
    log_step "Installing Agent Orchestrator to: $INSTALL_DIR"
    
    # Create target directory if it doesn't exist
    if [ ! -d "$INSTALL_DIR" ]; then
        mkdir -p "$INSTALL_DIR"
        log_info "Created directory: $INSTALL_DIR"
    fi
    
    # Copy orchestrator files
    log_step "Copying orchestrator files..."
    rsync -av --exclude='node_modules' --exclude='dist' --exclude='logs' --exclude='.git' \
        "$ORCHESTRATOR_SOURCE/" "$INSTALL_DIR/.agent-orchestrator/"
    
    ORCHESTRATOR_DIR="$INSTALL_DIR/.agent-orchestrator"
else
    # Installing in place
    ORCHESTRATOR_DIR="$ORCHESTRATOR_SOURCE"
    log_step "Installing Agent Orchestrator in place: $ORCHESTRATOR_DIR"
fi

cd "$ORCHESTRATOR_DIR"

# Check for Node.js
log_step "Checking prerequisites..."
if ! command -v node &> /dev/null; then
    log_error "Node.js is not installed. Please install Node.js 18+ first."
    exit 1
fi

NODE_VERSION=$(node -v | cut -d'v' -f2 | cut -d'.' -f1)
if [ "$NODE_VERSION" -lt 18 ]; then
    log_error "Node.js 18+ required. Found: $(node -v)"
    exit 1
fi

log_success "Node.js $(node -v) found"

# Check for npm
if ! command -v npm &> /dev/null; then
    log_error "npm is not installed. Please install npm first."
    exit 1
fi

log_success "npm $(npm -v) found"

# Check for Cursor CLI (optional but recommended)
if command -v cursor-agent &> /dev/null; then
    CURSOR_VERSION=$(cursor-agent --version 2>/dev/null || echo "unknown")
    log_success "Cursor CLI found: $CURSOR_VERSION"
else
    log_warn "Cursor CLI not found. Install it for full functionality:"
    echo -e "  ${CYAN}https://cursor.com/docs/cli/overview${NC}"
fi

# Check for submodules
log_step "Checking submodules..."
if [ -f "$PROJECT_ROOT/.gitmodules" ]; then
    log_info "Git submodules detected"
    
    # Check if submodules are initialized
    SUBMODULE_STATUS=$(git -C "$PROJECT_ROOT" submodule status 2>/dev/null || echo "")
    if echo "$SUBMODULE_STATUS" | grep -q "^\-"; then
        log_warn "Some submodules are not initialized"
        log_info "To initialize submodules, run:"
        echo -e "  ${CYAN}bash $ORCHESTRATOR_DIR/scripts/init-submodules.sh${NC}"
        echo -e "  ${CYAN}or: git submodule update --init --recursive${NC}"
    else
        log_success "Submodules appear to be initialized"
    fi
else
    log_info "No .gitmodules found (not a submodule-based project)"
fi

# Install npm dependencies
log_step "Installing npm dependencies..."
npm install

log_success "Dependencies installed"

# Build TypeScript
log_step "Building TypeScript..."
npm run build

log_success "Build completed"

# Initialize logs directory
if [ ! -d "$ORCHESTRATOR_DIR/logs" ]; then
    mkdir -p "$ORCHESTRATOR_DIR/logs"
    log_info "Created logs directory"
fi

# Copy Cursor rules to project root (if not already present)
PROJECT_ROOT="$INSTALL_DIR"
if [ "$INSTALL_DIR" != "$ORCHESTRATOR_SOURCE" ]; then
    PROJECT_ROOT="$INSTALL_DIR"
else
    # Try to find project root (parent of .agent-orchestrator)
    if [ -d "$(dirname "$ORCHESTRATOR_DIR")/.git" ] || [ -d "$(dirname "$ORCHESTRATOR_DIR")/.cursor" ]; then
        PROJECT_ROOT="$(dirname "$ORCHESTRATOR_DIR")"
    else
        PROJECT_ROOT="$ORCHESTRATOR_DIR"
    fi
fi

log_step "Setting up Cursor rules..."

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
    if [ -f "$ORCHESTRATOR_DIR/.cursorrules" ]; then
        # Create rule file from .cursorrules
        cat > "$PROJECT_CURSOR_RULES/agent-orchestrator.md" << 'EOF'
# Agent Orchestrator Rules

See `.agent-orchestrator/.cursorrules` for complete documentation.
EOF
        log_success "Cursor rules reference added"
    fi
else
    log_info "Cursor rules already present, skipping"
fi

# Create a convenience script in project root
log_step "Creating convenience scripts..."

# Create wrapper script
WRAPPER_SCRIPT="$PROJECT_ROOT/orchestrator"
cat > "$WRAPPER_SCRIPT" << EOF
#!/bin/bash
# Agent Orchestrator Wrapper Script
cd "$ORCHESTRATOR_DIR"
npm run dev "\$@"
EOF

chmod +x "$WRAPPER_SCRIPT"
log_success "Created wrapper script: $WRAPPER_SCRIPT"

# Create npm script in package.json if it exists
if [ -f "$PROJECT_ROOT/package.json" ]; then
    log_info "Project has package.json, you can add this script:"
    echo -e "  ${CYAN}\"orchestrator\": \"cd .agent-orchestrator && npm run dev\"${NC}"
fi

# Summary
echo ""
log_success "Agent Orchestrator installed successfully!"
echo ""
log_info "Installation location: $ORCHESTRATOR_DIR"
log_info "Project root: $PROJECT_ROOT"
echo ""
log_info "Next steps:"
echo -e "  1. ${CYAN}List workflows${NC}: cd $ORCHESTRATOR_DIR && npm run dev flows"
echo -e "  2. ${CYAN}Dry-run workflow${NC}: cd $ORCHESTRATOR_DIR && npm run dev run --flow feature_implementation --dry-run"
echo -e "  3. ${CYAN}Execute workflow${NC}: cd $ORCHESTRATOR_DIR && npm run dev run --flow feature_implementation --runner cursor"
echo -e "  4. ${CYAN}Use wrapper script${NC}: $WRAPPER_SCRIPT flows"
echo ""
log_info "Documentation:"
echo -e "  - ${CYAN}README${NC}: $ORCHESTRATOR_DIR/docs/README.md"
echo -e "  - ${CYAN}Subagent Capabilities${NC}: $ORCHESTRATOR_DIR/docs/SUBAGENT_CAPABILITIES.md"
echo -e "  - ${CYAN}Config${NC}: $ORCHESTRATOR_DIR/config/orchestration.yml"
echo ""

if ! command -v cursor-agent &> /dev/null; then
    log_warn "Cursor CLI not installed. Install it for full functionality:"
    echo -e "  ${CYAN}https://cursor.com/docs/cli/overview${NC}"
    echo ""
fi

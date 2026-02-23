#!/bin/bash

# FutureWampumId TDD Compliance Check Script
# This script helps verify compliance with TDD and plans

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
ORCHESTRATOR_DIR="$PROJECT_ROOT/.agent-orchestrator"

# Microservices location - FutureWampumId services
FWID_DIR="/Volumes/Barracuda/mamey-io/code-final/FutureWampum/FutureWampumId"

# Other paths
TDD_FILE="$PROJECT_ROOT/.designs/TDD/FutureWampum/FutureWampumID TDD.md"
PLANS_DIR="$PROJECT_ROOT/.cursor/plans/FutureWampum/FutureWampumId"
MAMEY_BLOCKCHAIN_DIR="$PROJECT_ROOT/Mamey/src"
MAMEYNODE_DIR="$PROJECT_ROOT/MameyNode"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if file/directory exists
check_exists() {
    if [ ! -e "$1" ]; then
        log_error "$1 does not exist"
        return 1
    fi
    return 0
}

# Main execution
main() {
    log_info "FutureWampumId TDD Compliance Check"
    echo "=========================================="
    echo ""
    
    # Check prerequisites
    log_info "Checking prerequisites..."
    
    check_exists "$TDD_FILE" || exit 1
    check_exists "$PLANS_DIR" || exit 1
    check_exists "$FWID_DIR" || exit 1
    check_exists "$MAMEY_BLOCKCHAIN_DIR" || exit 1
    check_exists "$MAMEYNODE_DIR" || exit 1
    
    log_success "All prerequisites found"
    echo ""
    
    # Service mapping (using arrays for compatibility)
    PLAN_NAMES=("Identity" "DID" "ZKP" "AccessControl" "Credential" "API")
    SERVICE_NAMES=("Mamey.FWID.Identities" "Mamey.FWID.DIDs" "Mamey.FWID.ZKPs" "Mamey.FWID.AccessControls" "Mamey.FWID.Credentials" "Mamey.FWID.ApiGateway")
    
    # Check each service
    log_info "Checking services..."
    echo ""
    
    for i in "${!PLAN_NAMES[@]}"; do
        plan_name="${PLAN_NAMES[$i]}"
        service_name="${SERVICE_NAMES[$i]}"
        service_dir="$FWID_DIR/$service_name"
        plan_file="$PLANS_DIR/${plan_name}.plan.md"
        
        echo "----------------------------------------"
        log_info "Checking: $service_name (Plan: $plan_name)"
        
        # Check if service exists
        if [ ! -d "$service_dir" ]; then
            log_warning "Service directory not found: $service_dir"
            continue
        fi
        
        # Check if plan exists
        if [ ! -f "$plan_file" ]; then
            log_warning "Plan file not found: $plan_file"
        else
            log_success "Plan file found"
        fi
        
        # Check layers
        check_layer "$service_dir" "Domain" "Domain"
        check_layer "$service_dir" "Application" "Application"
        check_layer "$service_dir" "Infrastructure" "Infrastructure"
        check_layer "$service_dir" "Contracts" "Contracts"
        check_layer "$service_dir" "Api" "API"
        
        # Check blockchain references
        check_blockchain_references "$service_dir" "$service_name"
        
        echo ""
    done
    
    # Summary
    echo "=========================================="
    log_info "Compliance check complete"
    log_info "Review the output above for any warnings or errors"
    log_info "For detailed analysis, use the orchestrator workflow:"
    log_info "  cd .agent-orchestrator"
    log_info "  npm run dev execute --workflow config/fwid-compliance-workflow.yml"
}

# Check if a layer exists
check_layer() {
    local service_dir="$1"
    local layer_name="$2"
    local display_name="$3"
    
    local layer_dir="$service_dir/src/${service_dir##*/}.$layer_name"
    
    if [ -d "$layer_dir" ]; then
        log_success "  ✓ $display_name layer exists"
    else
        log_warning "  ✗ $display_name layer missing: $layer_dir"
    fi
}

# Check blockchain references
check_blockchain_references() {
    local service_dir="$1"
    local service_name="$2"
    
    local csproj_files=$(find "$service_dir" -name "*.csproj" -type f)
    local has_blockchain=false
    
    for csproj in $csproj_files; do
        if grep -q "Mamey.Blockchain" "$csproj" 2>/dev/null; then
            has_blockchain=true
            log_success "  ✓ Mamey.Blockchain references found in: $(basename $csproj)"
        fi
    done
    
    if [ "$has_blockchain" = false ]; then
        log_warning "  ✗ No Mamey.Blockchain references found"
    fi
}

# Run main function
main "$@"

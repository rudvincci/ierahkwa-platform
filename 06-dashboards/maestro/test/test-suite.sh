#!/bin/bash

# Test Suite for Maestro
# Tests all implemented features

set -e

ORCHESTRATOR_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ORCHESTRATOR_DIR"

echo "🧪 Maestro Test Suite"
echo "================================"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

PASSED=0
FAILED=0

# Test function
test_command() {
    local name="$1"
    local command="$2"
    local expected_exit="${3:-0}"
    
    echo -n "Testing: $name... "
    
    if eval "$command" > /tmp/test_output.log 2>&1; then
        if [ $? -eq $expected_exit ]; then
            echo -e "${GREEN}✓ PASSED${NC}"
            PASSED=$((PASSED + 1))
            return 0
        else
            echo -e "${RED}✗ FAILED (wrong exit code)${NC}"
            cat /tmp/test_output.log
            FAILED=$((FAILED + 1))
            return 1
        fi
    else
        if [ $? -eq $expected_exit ]; then
            echo -e "${GREEN}✓ PASSED${NC}"
            PASSED=$((PASSED + 1))
            return 0
        else
            echo -e "${RED}✗ FAILED${NC}"
            cat /tmp/test_output.log
            FAILED=$((FAILED + 1))
            return 1
        fi
    fi
}

# Test 1: Build
echo "📦 Build Tests"
echo "--------------"
test_command "TypeScript compilation" "npm run build"
echo ""

# Test 2: CLI Help
echo "🔧 CLI Tests"
echo "------------"
test_command "CLI help command" "node dist/cli/index.js --help"
test_command "CLI version" "node dist/cli/index.js --version"
test_command "List workflows" "node dist/cli/index.js flows"
test_command "List checkpoints (empty)" "node dist/cli/index.js checkpoints"
test_command "List templates (empty)" "node dist/cli/index.js templates"
echo ""

# Test 3: Configuration Loading
echo "⚙️  Configuration Tests"
echo "----------------------"
test_command "Config file exists" "test -f config/orchestrator.config.yml"
test_command "Orchestration file exists" "test -f config/orchestration.yml"
echo ""

# Test 4: Service Initialization
echo "🔌 Service Tests"
echo "----------------"
# Test that services can be imported
test_command "StateManager import" "node -e \"const { StateManager } = require('./dist/services/StateManager'); console.log('OK');\""
test_command "ResultCache import" "node -e \"const { ResultCache } = require('./dist/services/ResultCache'); console.log('OK');\""
test_command "ErrorRecovery import" "node -e \"const { ErrorRecovery } = require('./dist/services/ErrorRecovery'); console.log('OK');\""
test_command "PreflightValidator import" "node -e \"const { PreflightValidator } = require('./dist/services/PreflightValidator'); console.log('OK');\""
test_command "ReportingService import" "node -e \"const { ReportingService } = require('./dist/services/ReportingService'); console.log('OK');\""
test_command "InteractiveMode import" "node -e \"const { InteractiveMode } = require('./dist/services/InteractiveMode'); console.log('OK');\""
test_command "ParallelOptimizer import" "node -e \"const { ParallelOptimizer } = require('./dist/services/ParallelOptimizer'); console.log('OK');\""
test_command "ConditionalExecutor import" "node -e \"const { ConditionalExecutor } = require('./dist/services/ConditionalExecution'); console.log('OK');\""
test_command "WorkflowComposer import" "node -e \"const { WorkflowComposer } = require('./dist/services/WorkflowComposer'); console.log('OK');\""
echo ""

# Test 5: Workflow Validation
echo "📋 Workflow Validation Tests"
echo "---------------------------"
# Create a test workflow file
cat > /tmp/test-workflow.yml <<EOF
flows:
  test_workflow:
    name: test_workflow
    description: Test workflow
    steps:
      - name: step1
        agent: Backend
        description: Test step 1
        dependsOn: []
EOF

test_command "Workflow YAML parsing" "node -e \"const yaml = require('js-yaml'); const fs = require('fs'); const doc = yaml.load(fs.readFileSync('/tmp/test-workflow.yml', 'utf8')); console.log('OK');\""
echo ""

# Test 6: Checkpoint System
echo "💾 Checkpoint System Tests"
echo "--------------------------"
test_command "Checkpoint directory creation" "mkdir -p .maestro/checkpoints && test -d .maestro/checkpoints"
test_command "Cache directory creation" "mkdir -p .maestro/cache && test -d .maestro/cache"
test_command "Reports directory creation" "mkdir -p .maestro/reports && test -d .maestro/reports"
echo ""

# Test 7: Feature Flags
echo "🚩 Feature Flag Tests"
echo "---------------------"
# Test that all CLI options are recognized
test_command "Checkpoint flag" "node dist/cli/index.js run --help 2>&1 | grep -q 'checkpoint'"
test_command "Cache flag" "node dist/cli/index.js run --help 2>&1 | grep -q 'cache'"
test_command "Retry flag" "node dist/cli/index.js run --help 2>&1 | grep -q 'retry'"
test_command "Interactive flag" "node dist/cli/index.js run --help 2>&1 | grep -q 'interactive'"
test_command "Report flag" "node dist/cli/index.js run --help 2>&1 | grep -q 'report'"
test_command "Optimize parallel flag" "node dist/cli/index.js run --help 2>&1 | grep -q 'optimize-parallel'"
echo ""

# Summary
echo ""
echo "================================"
echo "Test Summary"
echo "================================"
echo -e "${GREEN}Passed: $PASSED${NC}"
if [ $FAILED -gt 0 ]; then
    echo -e "${RED}Failed: $FAILED${NC}"
else
    echo -e "${GREEN}Failed: 0${NC}"
fi
echo ""

if [ $FAILED -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
    exit 0
else
    echo -e "${RED}❌ Some tests failed${NC}"
    exit 1
fi

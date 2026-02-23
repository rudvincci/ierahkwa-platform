#!/bin/bash

# Mamey.Auth.Decentralized Test Execution Script
# This script provides various options for running tests with different configurations

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default values
CONFIGURATION="Release"
VERBOSITY="normal"
COLLECT_COVERAGE=false
GENERATE_REPORT=false
CLEANUP_AFTER=false
PARALLEL=true
FILTER=""
CATEGORY=""
DATABASE=""
PERFORMANCE=false
SECURITY=false
W3C_COMPLIANCE=false
INTEGRATION=false
UNIT=true

# Function to print usage
print_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -c, --configuration CONFIG    Build configuration (Debug|Release) [default: Release]"
    echo "  -v, --verbosity LEVEL         Log verbosity (quiet|minimal|normal|detailed|diagnostic) [default: normal]"
    echo "  --coverage                    Collect code coverage"
    echo "  --report                      Generate test report"
    echo "  --cleanup                     Cleanup after tests"
    echo "  --no-parallel                 Disable parallel test execution"
    echo "  -f, --filter FILTER           Test filter expression"
    echo "  --category CATEGORY           Test category (Unit|Integration|Performance|Security|W3CCompliance)"
    echo "  --database DATABASE           Database type (PostgreSQL|MongoDB|Redis|All)"
    echo "  --performance                 Run performance tests only"
    echo "  --security                    Run security tests only"
    echo "  --w3c-compliance              Run W3C compliance tests only"
    echo "  --integration                 Run integration tests only"
    echo "  --unit                        Run unit tests only (default)"
    echo "  --all                         Run all tests"
    echo "  -h, --help                    Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 --unit --coverage"
    echo "  $0 --integration --database PostgreSQL"
    echo "  $0 --performance --report"
    echo "  $0 --all --coverage --report --cleanup"
    echo "  $0 --filter \"ClassName=Mamey.Auth.Decentralized.Tests.Core.DidTests\""
}

# Function to print colored output
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check prerequisites
check_prerequisites() {
    print_info "Checking prerequisites..."
    
    # Check if dotnet is installed
    if ! command -v dotnet &> /dev/null; then
        print_error "dotnet CLI is not installed or not in PATH"
        exit 1
    fi
    
    # Check if docker is installed (for integration tests)
    if ! command -v docker &> /dev/null; then
        print_warning "Docker is not installed. Integration tests will be skipped."
    fi
    
    # Check if reportgenerator is installed (for coverage reports)
    if ! command -v reportgenerator &> /dev/null; then
        print_warning "ReportGenerator is not installed. Coverage reports will not be generated."
    fi
    
    print_success "Prerequisites check completed"
}

# Function to setup test environment
setup_test_environment() {
    print_info "Setting up test environment..."
    
    # Create necessary directories
    mkdir -p coverage
    mkdir -p reports
    mkdir -p logs
    
    # Set environment variables
    export ASPNETCORE_ENVIRONMENT=Testing
    export DOTNET_ENVIRONMENT=Testing
    
    print_success "Test environment setup completed"
}

# Function to start test containers
start_test_containers() {
    if [[ "$INTEGRATION" == true || "$CATEGORY" == "Integration" ]]; then
        print_info "Starting test containers..."
        
        # Start PostgreSQL container
        if [[ "$DATABASE" == "PostgreSQL" || "$DATABASE" == "All" || "$DATABASE" == "" ]]; then
            docker run -d --name mamey-postgres-test \
                -e POSTGRES_DB=mamey_did_test \
                -e POSTGRES_USER=test \
                -e POSTGRES_PASSWORD=test \
                -p 5432:5432 \
                postgres:13 || true
        fi
        
        # Start MongoDB container
        if [[ "$DATABASE" == "MongoDB" || "$DATABASE" == "All" || "$DATABASE" == "" ]]; then
            docker run -d --name mamey-mongo-test \
                -p 27017:27017 \
                mongo:5.0 || true
        fi
        
        # Start Redis container
        if [[ "$DATABASE" == "Redis" || "$DATABASE" == "All" || "$DATABASE" == "" ]]; then
            docker run -d --name mamey-redis-test \
                -p 6379:6379 \
                redis:6.2 || true
        fi
        
        # Wait for containers to be ready
        print_info "Waiting for containers to be ready..."
        sleep 10
        
        print_success "Test containers started"
    fi
}

# Function to stop test containers
stop_test_containers() {
    print_info "Stopping test containers..."
    
    # Stop and remove containers
    docker stop mamey-postgres-test mamey-mongo-test mamey-redis-test 2>/dev/null || true
    docker rm mamey-postgres-test mamey-mongo-test mamey-redis-test 2>/dev/null || true
    
    print_success "Test containers stopped"
}

# Function to build the solution
build_solution() {
    print_info "Building solution..."
    
    dotnet build --configuration $CONFIGURATION --no-restore
    
    if [ $? -eq 0 ]; then
        print_success "Solution built successfully"
    else
        print_error "Solution build failed"
        exit 1
    fi
}

# Function to restore packages
restore_packages() {
    print_info "Restoring packages..."
    
    dotnet restore
    
    if [ $? -eq 0 ]; then
        print_success "Packages restored successfully"
    else
        print_error "Package restoration failed"
        exit 1
    fi
}

# Function to run tests
run_tests() {
    print_info "Running tests..."
    
    # Build test command
    local test_cmd="dotnet test --configuration $CONFIGURATION --verbosity $VERBOSITY --no-build"
    
    # Add parallel execution
    if [[ "$PARALLEL" == true ]]; then
        test_cmd="$test_cmd --parallel"
    fi
    
    # Add filter
    if [[ -n "$FILTER" ]]; then
        test_cmd="$test_cmd --filter \"$FILTER\""
    fi
    
    # Add category filter
    if [[ -n "$CATEGORY" ]]; then
        test_cmd="$test_cmd --filter \"Category=$CATEGORY\""
    fi
    
    # Add coverage collection
    if [[ "$COLLECT_COVERAGE" == true ]]; then
        test_cmd="$test_cmd --collect:\"XPlat Code Coverage\""
    fi
    
    # Add logger
    test_cmd="$test_cmd --logger \"trx;LogFileName=test-results.trx\""
    test_cmd="$test_cmd --logger \"console;verbosity=$VERBOSITY\""
    
    # Execute test command
    eval $test_cmd
    
    if [ $? -eq 0 ]; then
        print_success "Tests completed successfully"
    else
        print_error "Tests failed"
        exit 1
    fi
}

# Function to generate coverage report
generate_coverage_report() {
    if [[ "$COLLECT_COVERAGE" == true && "$GENERATE_REPORT" == true ]]; then
        print_info "Generating coverage report..."
        
        # Find coverage files
        local coverage_files=$(find . -name "coverage.cobertura.xml" -type f)
        
        if [[ -n "$coverage_files" ]]; then
            # Generate HTML report
            reportgenerator -reports:"$coverage_files" -targetdir:"coverage" -reporttypes:"Html;JsonSummary" || true
            
            # Generate JSON summary
            reportgenerator -reports:"$coverage_files" -targetdir:"coverage" -reporttypes:"JsonSummary" || true
            
            print_success "Coverage report generated in coverage/ directory"
        else
            print_warning "No coverage files found"
        fi
    fi
}

# Function to generate test report
generate_test_report() {
    if [[ "$GENERATE_REPORT" == true ]]; then
        print_info "Generating test report..."
        
        # Find test result files
        local test_files=$(find . -name "test-results.trx" -type f)
        
        if [[ -n "$test_files" ]]; then
            # Generate test report (this would require additional tools)
            print_info "Test results available in test-results.trx"
        else
            print_warning "No test result files found"
        fi
    fi
}

# Function to cleanup test environment
cleanup_test_environment() {
    if [[ "$CLEANUP_AFTER" == true ]]; then
        print_info "Cleaning up test environment..."
        
        # Stop test containers
        stop_test_containers
        
        # Clean up test data
        dotnet run --project . -- cleanup-test-data || true
        
        # Clean up temporary files
        rm -rf coverage/*.xml
        rm -rf reports/*.trx
        rm -rf logs/*.log
        
        print_success "Test environment cleaned up"
    fi
}

# Function to run specific test categories
run_test_categories() {
    if [[ "$UNIT" == true ]]; then
        print_info "Running unit tests..."
        CATEGORY="Unit"
        run_tests
    fi
    
    if [[ "$INTEGRATION" == true ]]; then
        print_info "Running integration tests..."
        CATEGORY="Integration"
        run_tests
    fi
    
    if [[ "$PERFORMANCE" == true ]]; then
        print_info "Running performance tests..."
        CATEGORY="Performance"
        run_tests
    fi
    
    if [[ "$SECURITY" == true ]]; then
        print_info "Running security tests..."
        CATEGORY="Security"
        run_tests
    fi
    
    if [[ "$W3C_COMPLIANCE" == true ]]; then
        print_info "Running W3C compliance tests..."
        CATEGORY="W3CCompliance"
        run_tests
    fi
}

# Main execution function
main() {
    print_info "Starting Mamey.Auth.Decentralized test execution"
    
    # Check prerequisites
    check_prerequisites
    
    # Setup test environment
    setup_test_environment
    
    # Start test containers if needed
    start_test_containers
    
    # Restore packages
    restore_packages
    
    # Build solution
    build_solution
    
    # Run tests
    if [[ "$CATEGORY" != "" ]]; then
        run_tests
    else
        run_test_categories
    fi
    
    # Generate reports
    generate_coverage_report
    generate_test_report
    
    # Cleanup if requested
    cleanup_test_environment
    
    print_success "Test execution completed successfully"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        -v|--verbosity)
            VERBOSITY="$2"
            shift 2
            ;;
        --coverage)
            COLLECT_COVERAGE=true
            shift
            ;;
        --report)
            GENERATE_REPORT=true
            shift
            ;;
        --cleanup)
            CLEANUP_AFTER=true
            shift
            ;;
        --no-parallel)
            PARALLEL=false
            shift
            ;;
        -f|--filter)
            FILTER="$2"
            shift 2
            ;;
        --category)
            CATEGORY="$2"
            shift 2
            ;;
        --database)
            DATABASE="$2"
            shift 2
            ;;
        --performance)
            PERFORMANCE=true
            UNIT=false
            shift
            ;;
        --security)
            SECURITY=true
            UNIT=false
            shift
            ;;
        --w3c-compliance)
            W3C_COMPLIANCE=true
            UNIT=false
            shift
            ;;
        --integration)
            INTEGRATION=true
            UNIT=false
            shift
            ;;
        --unit)
            UNIT=true
            shift
            ;;
        --all)
            UNIT=true
            INTEGRATION=true
            PERFORMANCE=true
            SECURITY=true
            W3C_COMPLIANCE=true
            shift
            ;;
        -h|--help)
            print_usage
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            print_usage
            exit 1
            ;;
    esac
done

# Run main function
main

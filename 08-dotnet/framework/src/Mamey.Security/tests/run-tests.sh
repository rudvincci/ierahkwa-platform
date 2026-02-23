#!/bin/bash

# Mamey.Security Test Execution Script
# This script runs all test suites for the Mamey.Security library ecosystem

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
CONFIGURATION="${CONFIGURATION:-Release}"
COVERAGE="${COVERAGE:-false}"
CATEGORY="${CATEGORY:-All}"
VERBOSITY="${VERBOSITY:-normal}"

# Print functions
print_info() {
    echo -e "${YELLOW}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
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

# Function to build solution
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

# Function to run unit tests
run_unit_tests() {
    print_info "Running unit tests..."
    local filter="Category=Unit"
    local args="--configuration $CONFIGURATION --no-build --filter \"$filter\" --verbosity $VERBOSITY"
    
    if [ "$COVERAGE" = "true" ]; then
        args="$args --collect:\"XPlat Code Coverage\""
    fi
    
    dotnet test $args
    if [ $? -eq 0 ]; then
        print_success "Unit tests passed"
    else
        print_error "Unit tests failed"
        exit 1
    fi
}

# Function to run integration tests
run_integration_tests() {
    print_info "Running integration tests..."
    print_info "Note: Integration tests require Docker to be running"
    
    # Check if Docker is running
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
    
    local filter="Category=Integration"
    local args="--configuration $CONFIGURATION --no-build --filter \"$filter\" --verbosity $VERBOSITY"
    
    if [ "$COVERAGE" = "true" ]; then
        args="$args --collect:\"XPlat Code Coverage\""
    fi
    
    dotnet test $args
    if [ $? -eq 0 ]; then
        print_success "Integration tests passed"
    else
        print_error "Integration tests failed"
        exit 1
    fi
}

# Function to run performance tests
run_performance_tests() {
    print_info "Running performance tests..."
    local filter="Category=Performance"
    local args="--configuration $CONFIGURATION --no-build --filter \"$filter\" --verbosity $VERBOSITY"
    
    dotnet test $args
    if [ $? -eq 0 ]; then
        print_success "Performance tests completed"
    else
        print_error "Performance tests failed"
        exit 1
    fi
}

# Function to run security tests
run_security_tests() {
    print_info "Running security tests..."
    local filter="Category=Security"
    local args="--configuration $CONFIGURATION --no-build --filter \"$filter\" --verbosity $VERBOSITY"
    
    if [ "$COVERAGE" = "true" ]; then
        args="$args --collect:\"XPlat Code Coverage\""
    fi
    
    dotnet test $args
    if [ $? -eq 0 ]; then
        print_success "Security tests passed"
    else
        print_error "Security tests failed"
        exit 1
    fi
}

# Function to generate coverage report
generate_coverage_report() {
    if [ "$COVERAGE" = "true" ]; then
        print_info "Generating coverage report..."
        
        # Check if ReportGenerator is installed
        if ! command -v reportgenerator &> /dev/null; then
            print_info "ReportGenerator not found. Installing..."
            dotnet tool install -g dotnet-reportgenerator-globaltool
        fi
        
        reportgenerator \
            -reports:"**/coverage.cobertura.xml" \
            -targetdir:"coverage-report" \
            -reporttypes:"Html;JsonSummary"
        
        if [ $? -eq 0 ]; then
            print_success "Coverage report generated in coverage-report/"
        else
            print_error "Coverage report generation failed"
        fi
    fi
}

# Main execution
main() {
    print_info "Starting Mamey.Security test execution..."
    print_info "Configuration: $CONFIGURATION"
    print_info "Coverage: $COVERAGE"
    print_info "Category: $CATEGORY"
    print_info "Verbosity: $VERBOSITY"
    
    # Restore and build
    restore_packages
    build_solution
    
    # Run tests based on category
    case $CATEGORY in
        Unit)
            run_unit_tests
            ;;
        Integration)
            run_integration_tests
            ;;
        Performance)
            run_performance_tests
            ;;
        Security)
            run_security_tests
            ;;
        All)
            run_unit_tests
            run_integration_tests
            run_performance_tests
            run_security_tests
            ;;
        *)
            print_error "Invalid category: $CATEGORY"
            print_info "Valid categories: Unit, Integration, Performance, Security, All"
            exit 1
            ;;
    esac
    
    # Generate coverage report if requested
    generate_coverage_report
    
    print_success "All tests completed successfully!"
}

# Run main function
main




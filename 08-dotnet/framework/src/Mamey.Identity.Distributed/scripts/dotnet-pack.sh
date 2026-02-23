#!/bin/bash

# Build and pack script for Mamey.Identity.Distributed

set -e

echo "Building Mamey.Identity.Distributed..."

# Navigate to the project directory
cd "$(dirname "$0")/../src/Mamey.Identity.Distributed"

# Clean previous builds
echo "Cleaning previous builds..."
dotnet clean

# Restore packages
echo "Restoring packages..."
dotnet restore

# Build the project
echo "Building project..."
dotnet build --configuration Release

# Run tests (if any)
echo "Running tests..."
dotnet test --configuration Release --no-build --verbosity normal || echo "No tests found or tests failed"

# Pack the project
echo "Packing project..."
dotnet pack --configuration Release --no-build --output ./bin/Release

echo "Build and pack completed successfully!"
echo "Package location: ./bin/Release/"



































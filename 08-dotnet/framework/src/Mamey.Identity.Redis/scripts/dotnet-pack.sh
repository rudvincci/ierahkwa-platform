#!/bin/bash

# Mamey.Identity.Redis Package Script
# This script builds and packages the Mamey.Identity.Redis library

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
PROJECT_NAME="Mamey.Identity.Redis"
PROJECT_PATH="src/$PROJECT_NAME"
PACKAGE_VERSION="1.0.0"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"

echo -e "${GREEN}Starting $PROJECT_NAME package build...${NC}"

# Check if we're in the right directory
if [ ! -f "$PROJECT_PATH/$PROJECT_NAME.csproj" ]; then
    echo -e "${RED}Error: $PROJECT_NAME.csproj not found. Please run this script from the project root.${NC}"
    exit 1
fi

# Clean previous builds
echo -e "${YELLOW}Cleaning previous builds...${NC}"
dotnet clean "$PROJECT_PATH" --configuration Release --verbosity minimal

# Restore dependencies
echo -e "${YELLOW}Restoring dependencies...${NC}"
dotnet restore "$PROJECT_PATH" --verbosity minimal

# Build the project
echo -e "${YELLOW}Building $PROJECT_NAME...${NC}"
dotnet build "$PROJECT_PATH" --configuration Release --no-restore --verbosity minimal

# Run tests if they exist
if [ -d "tests" ]; then
    echo -e "${YELLOW}Running tests...${NC}"
    dotnet test "tests" --configuration Release --no-build --verbosity minimal
fi

# Pack the project
echo -e "${YELLOW}Creating NuGet package...${NC}"
dotnet pack "$PROJECT_PATH" \
    --configuration Release \
    --no-build \
    --output "./packages" \
    --verbosity minimal

# List created packages
echo -e "${GREEN}Created packages:${NC}"
ls -la packages/*.nupkg

echo -e "${GREEN}$PROJECT_NAME package build completed successfully!${NC}"
echo -e "${YELLOW}Packages are available in the ./packages directory${NC}"

# Optional: Push to NuGet (uncomment if needed)
# echo -e "${YELLOW}Pushing to NuGet...${NC}"
# dotnet nuget push "./packages/*.nupkg" \
#     --source "$NUGET_SOURCE" \
#     --api-key "$NUGET_API_KEY" \
#     --skip-duplicate

#!/bin/bash
echo "Executing after success scripts on branch $GITHUB_REF_NAME"
echo "Triggering package build"

# Change to the project directory relative to the script location
cd "$(dirname "$0")/../src/${project_dir##*/}"

# Define your base version here (e.g., 1.1.0)
BASE_VERSION="1.1.0"

# Extract the current date for build metadata
BUILD_DATE=$(date +%Y%m%d)

# Define the full version including the patch version and build metadata
if [ "$GITHUB_REF_NAME" == "develop" ]; then
  FULL_VERSION="$BASE_VERSION.$GITHUB_RUN_NUMBER-preview+$BUILD_DATE"
else
  FULL_VERSION="$BASE_VERSION.$GITHUB_RUN_NUMBER+$BUILD_DATE"
fi

echo "Building package version: $FULL_VERSION"

dotnet pack -c release /p:PackageVersion=$FULL_VERSION --no-restore -o .

echo "Uploading package to Azure Artifacts using branch $GITHUB_REF_NAME"

# Define your feeds and their respective PATs here
FEEDS=(
  "$FEED1_URL"
  "$FEED2_URL"
  # Add more feeds as needed
)

PATS=(
  "$FEED1_PAT"
  "$FEED2_PAT"
  # Add more PATs as needed
)

# Loop through the feeds and push the package
for i in "${!FEEDS[@]}"; do
  FEED=${FEEDS[$i]}
  PAT=${PATS[$i]}
  echo "Pushing package to $FEED"
  case "$GITHUB_REF_NAME" in
    "master"|"develop")
      dotnet nuget push "*.nupkg" --api-key "$PAT" --source "$FEED"
      if [ $? -ne 0 ]; then
        echo "Failed to push package to $FEED"
        exit 1
      else
        echo "Successfully pushed package to $FEED"
      fi
      ;;
    *)
      echo "Branch $GITHUB_REF_NAME does not trigger package push."
      ;;
  esac
done
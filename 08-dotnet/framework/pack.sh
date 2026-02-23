#!/usr/bin/env bash

# Exit immediately if any command fails (-e),
# treat unset variables as an error (-u),
# and propagate errors in pipelines (-o pipefail).
set -euo pipefail

# Arguments:
# 1) Solution Directory (required)
# 2) NuGet Feed URL     (required)
# 3) Feed PAT/API Key   (optional; can be empty)
# 4) Output Folder      (optional; defaults to ./nupkgs)
# 5) Package Version    (optional; e.g. "1.0.3")

solutionDir="${1:-}"
feedUrl="${2:-"http://localhost:4000/v3/index.json"}"
feedPat="${3:-}"
outputFolder="${4:-./nupkgs}"
packageVersion="${5:-}"  # Optional package version

# Validate required arguments.
if [[ -z "$solutionDir" || -z "$feedUrl" ]]; then
  echo "Usage: $0 <SolutionDirectory> <FeedUrl> [<FeedPat>] [<OutputFolder>] [<PackageVersion>]"
  echo "  <SolutionDirectory>: path to the folder containing your .csproj files (required)"
  echo "  <FeedUrl>          : NuGet feed URL (required)"
  echo "  <FeedPat>          : API key or PAT for the feed (optional; can be empty string)"
  echo "  <OutputFolder>     : Where to place .nupkg files (optional; default: ./nupkgs)"
  echo "  <PackageVersion>   : Custom version for the packages (optional)"
  exit 1
fi

# Ensure the solution directory exists.
if [[ ! -d "$solutionDir" ]]; then
  echo "Error: Directory '$solutionDir' does not exist."
  exit 1
fi

# Create the output folder if it doesn't already exist.
mkdir -p "$outputFolder"

# Navigate to the solution directory.
cd "$solutionDir" || exit 1

# Build all packable projects first (ensures all dependencies are built)
echo "Building all packable projects..."
packable_projects=$(find . -type f -name "*.csproj" \
  -not -path "*/tests/*" \
  -not -path "*/test/*" \
  -not -path "*/samples/*" \
  -not -name "*Tests.csproj" \
  -not -name "*Test.csproj")

build_failed=false
failed_projects=()
build_log_file="/tmp/mamey_build_errors.log"

# Clear the build log file
> "$build_log_file"

for csproj in $packable_projects; do
  echo "Building: $csproj"
  
  # Build with detailed output to capture errors
  if ! dotnet build "$csproj" -c Release --verbosity normal 2>&1 | tee -a "$build_log_file"; then
    echo "  ❌ FAILED: $csproj"
    failed_projects+=("$csproj")
    build_failed=true
  else
    echo "  ✅ SUCCESS: $csproj"
  fi
done

echo ""
echo "=========================================="
echo "BUILD SUMMARY"
echo "=========================================="

if [ "$build_failed" = true ]; then
  echo "❌ BUILD FAILURES DETECTED:"
  echo ""
  
  for failed_proj in "${failed_projects[@]}"; do
    echo "Failed project: $failed_proj"
  done
  
  echo ""
  echo "DETAILED ERROR LOG:"
  echo "==================="
  cat "$build_log_file"
  
  echo ""
  echo "=========================================="
  echo "ERRORS BY FILE:"
  echo "=========================================="
  
  # Extract file-specific errors from the build log
  grep -E "\.cs\([0-9]+,[0-9]+\): error" "$build_log_file" | while read -r line; do
    echo "$line"
  done
  
  echo ""
  echo "=========================================="
  echo "WARNINGS BY FILE:"
  echo "=========================================="
  
  # Extract file-specific warnings from the build log
  grep -E "\.cs\([0-9]+,[0-9]+\): warning" "$build_log_file" | while read -r line; do
    echo "$line"
  done
  
  echo ""
  echo "❌ Some projects failed to build. Please fix the errors above before packing."
  echo "Continuing with packing for projects that built successfully..."
else
  echo "✅ All projects built successfully!"
fi

echo ""

# Build the default pack arguments.
packArgs=(-c "Release" -o "$outputFolder" --no-build)

# If a package version was provided, append it to the pack arguments.
if [[ -n "$packageVersion" ]]; then
  echo "Will use custom package version: $packageVersion"
  packArgs+=("/p:PackageVersion=$packageVersion")
fi

# Find packable projects (exclude tests and samples)
echo "Finding packable projects..."
csprojFiles=$(find . -type f -name "*.csproj" \
  -not -path "*/tests/*" \
  -not -path "*/test/*" \
  -not -path "*/samples/*" \
  -not -name "*Tests.csproj" \
  -not -name "*Test.csproj")

echo "Found $(echo "$csprojFiles" | wc -l) packable projects"

# Iterate over each .csproj file found.
for csproj in $csprojFiles; do
  echo "==> Packing project: $csproj"

  # Pack into the specified output folder with the configured arguments.
  if ! dotnet pack "$csproj" "${packArgs[@]}"; then
    echo "    Error: Failed to pack $csproj. Skipping to next project..."
    continue
  fi

  # Grab the base name of the project (without extension).
  baseName=$(basename "$csproj" .csproj)

  # Find the newest matching .nupkg in the output folder (in case multiple versions exist).
  nupkgFile=$(find "$outputFolder" -maxdepth 1 -type f -name "${baseName}.*.nupkg" | sort | tail -n 1)

  if [[ -f "$nupkgFile" ]]; then
    echo "    Generated package: $nupkgFile"

    # Push to the NuGet feed if a feed URL was provided.
    if [[ -n "$feedUrl" ]]; then
      echo "    Attempting push to: $feedUrl"

      # Build arguments for dotnet nuget push.
      dotnetPushArgs=("-s" "$feedUrl")
      # Conditionally add the PAT key if provided.
      if [[ -n "$feedPat" ]]; then
        dotnetPushArgs+=("-k" "$feedPat")
      fi
        echo "    Pushing to: $feedUrl"
      dotnet nuget push "$nupkgFile" "${dotnetPushArgs[@]}" --skip-duplicate
    fi
  else
    echo "    Warning: No .nupkg found for $baseName in '$outputFolder'."
  fi
done

echo ""
echo "All projects have been packed (errors were skipped) into '$outputFolder'."
echo "Done."

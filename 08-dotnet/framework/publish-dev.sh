#!/usr/bin/env bash

set -e

PACKAGE_FOLDER="./nupkgs"
FEED_URL="http://localhost:4000/v3/index.json"
# API_KEY="my-secret-key"

cd "$PACKAGE_FOLDER" || exit 1

for package in *.nupkg; do
  echo "Pushing $package ..."
  # If push fails, print an error but continue.
  if ! dotnet nuget push "$package"  -s "$FEED_URL" --skip-duplicate; then
    echo "Warning: Failed to push $package. Continuing with the next..."
  fi
done

echo "Done pushing packages."

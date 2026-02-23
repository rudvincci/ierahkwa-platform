#!/usr/bin/env bash
set -euo pipefail

# Create a sensible .gitignore if none exists
if [ ! -f .gitignore ]; then
  dotnet new gitignore >/dev/null 2>&1 || true
fi

# Init git only if not already a repo
if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  git init
  git add .
  git commit -m "mamey-micro cli output."
  git checkout -b development
fi

# Initialize user-secrets only if the project exists
if [ -d "src/Pupitre.Progress" ]; then
  pushd src/Pupitre.Progress >/dev/null
  dotnet user-secrets init || true
  popd >/dev/null
fi

# Self-clean
rm -f post-restore.sh || true

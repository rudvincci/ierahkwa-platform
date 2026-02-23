---
on:
  schedule: daily
  permissions:
    contents: read
    issues: read
    pull-requests: read
  safe-outputs:
    create-issue:
      title-prefix: "[docs] "
      labels: [documentation, continuous-ai]
  tools:
    github:
---

# Continuous Documentation — Ierahkwa Platform

Analyze the repository for documentation drift and mismatches.

## Check these areas:

1. **README.md accuracy**:
   - Verify platform count matches actual platforms in `02-plataformas-html/` (currently 49)
   - Verify service count matches actual services in `03-backend/` (currently 20)
   - Verify port assignments match actual server.js configurations
   - Check that all listed platforms actually exist as directories

2. **Service documentation**:
   - For each service in `03-backend/`, check if a README.md exists
   - Flag services missing documentation
   - Check if documented API endpoints match actual route definitions

3. **Architecture alignment**:
   - Verify ARCHITECTURE.md reflects current directory structure
   - Check if docker-compose.sovereign.yml services match actual backend services
   - Verify DEPLOYMENT.md instructions are still valid

4. **CHANGELOG currency**:
   - Check if recent commits are reflected in CHANGELOG.md
   - Suggest entries for undocumented changes

## Output:
Create a single issue summarizing all documentation drift found, with actionable items organized by priority. If no drift is found, create an issue confirming docs are in sync.

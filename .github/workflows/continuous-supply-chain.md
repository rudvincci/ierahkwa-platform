---
on:
  push:
    branches: [main]
    paths:
      - '**/package.json'
      - '**/package-lock.json'
      - '**/Cargo.toml'
      - '**/Cargo.lock'
  schedule: daily
  permissions:
    contents: read
  safe-outputs:
    create-issue:
      title-prefix: "[supply-chain] "
      labels: [security, supply-chain, continuous-ai]
  tools:
    github:
---

# Continuous Supply Chain Security — Ierahkwa Platform

Monitor the supply chain integrity of the sovereign platform ecosystem. Defend against Shai-Hulud-style attacks and dependency confusion.

## Context

The Ierahkwa Platform has 20+ backend services with npm dependencies, 2 Rust blockchain projects with cargo dependencies, and 6+ .NET component groups with NuGet dependencies. As a sovereign platform serving 72 million indigenous people, supply chain integrity is critical — a compromised dependency could affect the entire sovereign infrastructure.

## Analysis Tasks

1. **Dependency change review**:
   - When package.json or lockfiles change, analyze what was added, removed, or updated
   - Flag new dependencies that are: less than 6 months old, have fewer than 100 weekly downloads, or have a single maintainer
   - Check if new dependencies have lifecycle scripts (preinstall, install, postinstall)
   - Verify version changes are intentional (major bumps, unexpected downgrades)

2. **Credential exposure scan**:
   - Scan for accidentally committed tokens (npm, GitHub, AWS, API keys)
   - Check CI workflow files for secrets exposed in logs
   - Verify .env.example files don't contain real values
   - Check GitHub Actions secrets usage for least-privilege

3. **Lifecycle script audit**:
   - Scan all package.json files (project + dependencies) for lifecycle scripts
   - Flag any postinstall scripts that download remote content
   - Flag scripts using eval(), child_process, or network calls
   - Compare lifecycle scripts between versions for unexpected changes

4. **Dependency confusion defense**:
   - Check for internal-looking package names that could be squatted on public registries
   - Verify all scoped packages (@ierahkwa/) are from trusted sources
   - Check for typosquatting attempts on popular dependency names

5. **Build reproducibility**:
   - Verify lockfiles are committed for all services with dependencies
   - Check that CI uses `npm ci` (not `npm install`) for deterministic builds
   - Verify Docker builds use specific base image tags (not :latest)
   - Check for pinned GitHub Actions versions (not @main or @master)

6. **SBOM and attestation**:
   - Verify SBOM can be generated for the project
   - Check if artifact attestations are configured
   - Review dependency license compliance

## Output

Create an issue with findings organized by:
- 🔴 Critical: Immediate action required (compromised packages, exposed credentials)
- 🟠 High: Should fix soon (unpinned versions, lifecycle script risks)
- 🟡 Medium: Improvement opportunities (missing lockfiles, unverified sources)
- 🟢 Informational: Best practice suggestions

Include specific remediation steps for each finding.

---
on:
  schedule: daily
  push:
    branches: [main]
  safe-outputs:
    create-issue:
      title-prefix: "[security] "
      labels: [security, continuous-ai, urgent]
  permissions:
    contents: read
  tools:
    github:
---

# Continuous Security Monitoring — Ierahkwa Platform

Perform daily security analysis of the sovereign platform infrastructure.

## Security checks:

1. **Dependency vulnerability scan**:
   - Check all package.json files for known vulnerable dependencies
   - Flag any dependency with critical or high severity CVEs
   - Check .NET project files for vulnerable NuGet packages

2. **Shared security middleware compliance**:
   - Verify all Node.js services import from `../shared/security.js`
   - Check that `applySecurityMiddleware()` is called in every service
   - Flag services that bypass shared security or implement custom CORS
   - Verify rate limiting is active on all auth endpoints

3. **Configuration security**:
   - Scan for hardcoded secrets, API keys, or credentials
   - Check .env.example files don't contain real values
   - Verify docker-compose files don't expose unnecessary ports
   - Check that no service binds to 0.0.0.0 in production configs

4. **OWASP compliance drift**:
   - Reference SECURITY.md compliance matrices
   - Verify new code additions maintain OWASP Top 10 2025 compliance
   - Check for new injection vectors, broken access control, or cryptographic failures

5. **Sovereign-specific security**:
   - Verify zero-tracking claims (no analytics, no third-party scripts in HTML platforms)
   - Check blockchain API security (transaction signing, key management)
   - Verify voting system (voto-soberano) integrity controls

## Output:
Create an issue with findings organized by severity (Critical > High > Medium > Low). Include remediation steps for each finding.

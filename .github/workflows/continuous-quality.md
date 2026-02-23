---
on:
  pull_request:
    types: [opened, synchronize]
  permissions:
    contents: read
    pull-requests: read
    checks: read
  safe-outputs:
    add-comment:
      max-length: 2000
  tools:
    github:
---

# Continuous Quality Review — Ierahkwa Platform

When a pull request is opened or updated, perform an architectural quality review.

## Review checklist:

1. **Code consistency**:
   - Check if new code follows existing patterns in the codebase
   - Verify Express middleware ordering matches other services
   - Check naming conventions (sovereign naming: *-soberano/a)
   - Verify error handling follows shared/security.js patterns

2. **Security review**:
   - Check for input validation on all new endpoints
   - Verify authentication/authorization on protected routes
   - Check for SQL injection, XSS, or CSRF vulnerabilities
   - Ensure new services use shared security middleware

3. **Performance considerations**:
   - Flag regex compilation inside loops
   - Flag synchronous file I/O in request handlers
   - Flag missing pagination on list endpoints
   - Flag unbounded array operations

4. **Accessibility**:
   - For HTML platform changes: check color contrast ratios
   - Verify keyboard navigability
   - Check for alt text on images
   - Verify ARIA labels on interactive elements
   - Check semantic HTML structure

5. **Documentation impact**:
   - If new services/platforms are added, flag if README.md needs updating
   - If API routes change, flag if documentation needs updating
   - If docker-compose changes, flag if DEPLOYMENT.md needs updating

## Output:
Add a review comment to the PR summarizing findings. Be constructive and specific. Suggest fixes, don't just flag problems.

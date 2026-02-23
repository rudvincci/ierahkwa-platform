---
on:
  schedule: weekly
  permissions:
    contents: read
  safe-outputs:
    create-issue:
      title-prefix: "[testing] "
      labels: [testing, continuous-ai]
  tools:
    github:
---

# Continuous Test Improvement — Ierahkwa Platform

Analyze the test suite across all 19 Node.js backend services and identify gaps.

## Analysis tasks:

1. **Coverage inventory**:
   - List every service in `03-backend/` and whether it has test files
   - Categorize existing tests: health-check only, API tests, auth tests, integration tests
   - Identify services with NO tests at all

2. **Gap analysis for each service**:
   - Compare route definitions in server.js to test coverage
   - Flag untested API endpoints
   - Flag untested middleware (auth, validation, error handling)
   - Flag untested business logic

3. **Test quality assessment**:
   - Check for assertion completeness (not just status codes, but response bodies)
   - Check for edge case coverage (invalid input, auth failures, rate limiting)
   - Check for error path testing
   - Assess if tests are isolated or have shared state issues

4. **Recommendations**:
   - Prioritize by impact: security-critical services first (voto-soberano, blockchain-api, api-gateway)
   - Suggest specific test cases for each service
   - Estimate effort for each recommendation

## Output:
Create a detailed issue with a test coverage matrix and prioritized recommendations. Include code snippets for the top 5 highest-priority missing tests.

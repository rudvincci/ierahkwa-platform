---
on:
  pull_request:
    types: [opened, synchronize]
  schedule: weekly
  permissions:
    contents: read
  safe-outputs:
    create-issue:
      title-prefix: "[perf] "
      labels: [performance, continuous-ai]
    add-comment:
      max-length: 1500
  tools:
    github:
---

# Continuous Performance — Ierahkwa Platform

Detect performance regressions and suggest optimizations across all backend services.

## Analysis Tasks

### Code-Level Performance Checks

1. **Regex patterns**:
   - Flag regex compilation inside loops or frequently-called functions
   - Suggest pre-compiling with `const pattern = /regex/` outside function scope
   - Check for catastrophic backtracking patterns

2. **Synchronous operations**:
   - Flag `fs.readFileSync`, `fs.writeFileSync` in request handlers
   - Flag `JSON.parse()` without try-catch on large payloads
   - Flag blocking operations in Express middleware chains

3. **Memory patterns**:
   - Flag unbounded arrays or Maps without size limits
   - Check for missing cleanup in `setInterval`/`setTimeout`
   - Verify circular buffers have proper eviction (vigilancia-soberana)
   - Check for event listener leaks (Socket.IO services)

4. **Database queries**:
   - Flag N+1 query patterns
   - Check for missing pagination on list endpoints
   - Flag queries without proper indexing hints
   - Check for unparameterized queries (SQL injection + perf)

5. **Response optimization**:
   - Verify `compression()` middleware is active
   - Check for missing `Cache-Control` headers on static content
   - Flag large JSON responses without streaming
   - Check image responses for proper content-type and caching

### Infrastructure Performance

6. **Docker optimization**:
   - Check Dockerfile layer ordering (dependencies before source code)
   - Verify multi-stage builds are used
   - Flag unnecessarily large base images
   - Check for `.dockerignore` to exclude node_modules/tests

7. **Prometheus metrics**:
   - Cross-reference `04-infraestructura/monitoring/prometheus.yml` targets
   - Verify all services are being monitored
   - Check for missing custom metrics in critical paths

### Benchmark Baselines

8. **Response time targets**:
   - API endpoints: < 200ms p95
   - Health checks: < 50ms
   - Static content: < 100ms
   - WebSocket connection: < 500ms

## Output

For PRs: Add a comment highlighting performance concerns in changed files.
For weekly: Create an issue with a full performance audit, prioritized by impact.

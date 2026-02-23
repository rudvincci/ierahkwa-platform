# CI/CD Integration Tests Configuration

## Overview

Integration tests are designed to work with **any Authentik instance** and **skip gracefully** when not configured, making them CI/CD friendly.

## Behavior

### Without Configuration (CI/CD Default)

When `AUTHENTIK_BASE_URL` is **not set**:
- ✅ Tests **skip gracefully** (no failures)
- ✅ CI/CD pipeline **passes** without configuration
- ℹ️ Logs indicate tests were skipped

### With Configuration

When `AUTHENTIK_BASE_URL` is **set**:
- ✅ Tests run against the configured instance
- ✅ Works with any Authentik instance (local, remote, test container)
- ⚠️ Tests skip if instance is not accessible

## CI/CD Configuration

### Option 1: Skip Integration Tests (Default)

No configuration needed - tests skip automatically:

```yaml
- name: Run tests
  run: dotnet test
  # Integration tests skip gracefully if AUTHENTIK_BASE_URL not set
```

### Option 2: Run Against Test Container

Start a test Authentik container in CI:

```yaml
- name: Start Test Authentik Container
  run: |
    docker run -d -p 9000:9000 \
      -e AUTHENTIK_SECRET_KEY=test-secret-key \
      ghcr.io/goauthentik/server:latest

- name: Wait for Authentik
  run: |
    timeout 120 bash -c 'until curl -f http://localhost:9000/api/v3/; do sleep 2; done'

- name: Run Integration Tests
  env:
    AUTHENTIK_BASE_URL: http://localhost:9000
    AUTHENTIK_API_TOKEN: ${{ secrets.AUTHENTIK_API_TOKEN }}
  run: dotnet test
```

### Option 3: Run Against Remote Test Instance

Point tests to a dedicated test Authentik instance:

```yaml
- name: Run Integration Tests
  env:
    AUTHENTIK_BASE_URL: https://test-authentik.example.com
    AUTHENTIK_API_TOKEN: ${{ secrets.AUTHENTIK_API_TOKEN }}
  run: dotnet test
```

## Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `AUTHENTIK_BASE_URL` | No* | URL of Authentik instance (e.g., `http://localhost:9100`) |
| `AUTHENTIK_API_TOKEN` | No | API token for authenticated tests |
| `AUTHENTIK_CHECK_LOCAL_CONTAINER` | No | Set to `"true"` for local container detection (local dev only) |
| `AUTHENTIK_CONTAINER_NAME` | No | Container name to check (default: `mamey-authentik-server`) |

\* Required only if you want to run integration tests. If not set, tests skip gracefully.

## Test Results

### Without AUTHENTIK_BASE_URL

```
Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1
```

Tests skip gracefully - no failures.

### With AUTHENTIK_BASE_URL (Instance Not Accessible)

```
Passed!  - Failed:     0, Passed:     0, Skipped:    20, Total:    20
```

Tests skip with warnings - no failures.

### With AUTHENTIK_BASE_URL (Instance Accessible)

```
Passed!  - Failed:     0, Passed:     3, Skipped:    17, Total:    20
```

Tests run against the configured instance.

## Local Development

For local development, see `tests/Mamey.Authentik.IntegrationTests/LOCAL_DEVELOPMENT.md`.

## Key Features

✅ **CI/CD Friendly**: Tests skip gracefully without configuration  
✅ **Instance Agnostic**: Works with any Authentik instance  
✅ **No Hardcoded Dependencies**: No specific container names or URLs  
✅ **Clear Logging**: Helpful messages when tests are skipped  
✅ **Flexible**: Supports local containers, remote instances, or test containers

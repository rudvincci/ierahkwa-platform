# Authentik Integration Tests

This directory contains integration tests that run against a live Authentik instance.

## Overview

Integration tests are designed to work with **any Authentik instance**, making them suitable for both:
- **Local Development**: Test against your local Docker container
- **CI/CD Pipelines**: Test against a test container or skip gracefully

## Prerequisites

1. **Authentik Instance**: A running Authentik instance (local Docker, remote, or CI test container)
2. **API Token**: An API token with appropriate permissions (optional - some tests will skip)

## Required Environment Variables

### Required
- `AUTHENTIK_BASE_URL` - URL of your Authentik instance (e.g., `http://localhost:9100`)

### Optional
- `AUTHENTIK_API_TOKEN` - API token for authenticated tests (tests will skip if not provided)
- `AUTHENTIK_CHECK_LOCAL_CONTAINER` - Set to `"true"` to check for local Docker container (local dev only)
- `AUTHENTIK_CONTAINER_NAME` - Container name to check (default: `mamey-authentik-server`)

## Setup

### Local Development

#### 1. Start Authentik Container (Optional)

If you have a local Docker container:

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith
docker-compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d
```

#### 2. Get API Token

1. Open Authentik admin interface: http://localhost:9100/if/admin/
2. Navigate to **Applications** → **Tokens**
3. Create a new token with appropriate permissions
4. Copy the token value

#### 3. Configure Environment Variables

```bash
export AUTHENTIK_BASE_URL="http://localhost:9100"
export AUTHENTIK_API_TOKEN="your-api-token-here"
export AUTHENTIK_CHECK_LOCAL_CONTAINER="true"  # Optional: check for local container
```

#### 4. Run Integration Tests

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Authentik
dotnet test --filter "Category=Integration"
```

### CI/CD Pipeline

In CI/CD, tests will automatically skip if no Authentik instance is available:

```yaml
- name: Run Integration Tests
  env:
    AUTHENTIK_BASE_URL: ${{ secrets.AUTHENTIK_BASE_URL }}  # Or use test container URL
    AUTHENTIK_API_TOKEN: ${{ secrets.AUTHENTIK_API_TOKEN }}  # Optional
  run: |
    cd Mamey/src/Mamey.Authentik
    dotnet test
```

**Note**: Tests will skip gracefully if `AUTHENTIK_BASE_URL` is not set or instance is not accessible.

## Test Configuration

The tests work with **any Authentik instance**:
- **Base URL**: Required via `AUTHENTIK_BASE_URL` environment variable
- **API Token**: Optional via `AUTHENTIK_API_TOKEN` environment variable
- **Local Container Check**: Optional, only when `AUTHENTIK_CHECK_LOCAL_CONTAINER=true`

## Test Behavior

| Condition | Behavior |
|-----------|----------|
| `AUTHENTIK_BASE_URL` not set | ❌ Tests fail with clear error message |
| Instance not accessible | ⚠️ Tests are skipped with warning |
| Instance accessible + No token | ⚠️ Auth tests skipped, others run |
| Instance accessible + Token set | ✅ All tests execute |

## Troubleshooting

### Missing AUTHENTIK_BASE_URL

**Error**: "AUTHENTIK_BASE_URL environment variable is required"

**Solution**: Set the environment variable:
```bash
export AUTHENTIK_BASE_URL="http://localhost:9100"
```

### Instance Not Accessible

**Error**: "Authentik instance is not accessible"

**Solutions**:
1. Verify the URL is correct: `curl $AUTHENTIK_BASE_URL/api/v3/`
2. Check if instance is running
3. Verify network connectivity
4. Check firewall/security settings

### Authentication Errors

**Error**: 401/403 responses

**Solutions**:
1. Verify API token is correct: `echo $AUTHENTIK_API_TOKEN`
2. Check token permissions in Authentik admin
3. Ensure token hasn't expired
4. Regenerate token if needed

### Local Container Not Found

**Warning**: "Local Authentik container detected" (when `AUTHENTIK_CHECK_LOCAL_CONTAINER=true`)

**Note**: This is informational only. Tests will work with any Authentik instance, not just local containers.

**Solutions**:
1. Start your local container
2. Or set `AUTHENTIK_CHECK_LOCAL_CONTAINER=false` (or unset it)
3. Or use a remote Authentik instance

## Test Data

Integration tests may create test data in your Authentik instance. Consider:

- Running tests against a dedicated test instance
- Cleaning up test data after tests complete
- Using test-specific prefixes for created resources

## CI/CD Integration

### Option 1: Skip Tests (Recommended for most CI/CD)

Tests will automatically skip if `AUTHENTIK_BASE_URL` is not set or instance is not accessible:

```yaml
- name: Run Tests
  run: dotnet test
  # Tests skip gracefully if AUTHENTIK_BASE_URL not set
```

### Option 2: Use Test Container

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

### Option 3: Use Remote Test Instance

Point tests to a dedicated test Authentik instance:

```yaml
- name: Run Integration Tests
  env:
    AUTHENTIK_BASE_URL: https://test-authentik.example.com
    AUTHENTIK_API_TOKEN: ${{ secrets.AUTHENTIK_API_TOKEN }}
  run: dotnet test
```

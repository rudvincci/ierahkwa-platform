# Integration Testing with Running Authentik Container

## Overview

The integration tests are configured to run against your **existing running Authentik Docker container** instead of starting a new one. This allows you to test against your actual configured instance.

## Quick Start

### 1. Verify Container is Running

```bash
docker ps --filter name=mamey-authentik-server
```

You should see the container with status "Up".

### 2. Get API Token

Run the helper script:

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Authentik
./scripts/get-api-token.sh
```

Or manually:
1. Open http://localhost:9100/if/admin/
2. Navigate to **Applications** → **Tokens**
3. Create a new token
4. Copy the token value

### 3. Set Environment Variables

```bash
export AUTHENTIK_BASE_URL="http://localhost:9100"
export AUTHENTIK_API_TOKEN="your-token-here"
```

Or add to your shell profile:

```bash
echo 'export AUTHENTIK_BASE_URL="http://localhost:9100"' >> ~/.zshrc
echo 'export AUTHENTIK_API_TOKEN="your-token-here"' >> ~/.zshrc
source ~/.zshrc
```

### 4. Run Integration Tests

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Authentik
dotnet test --filter "Category=Integration"
```

Or run all tests:

```bash
dotnet test
```

## How It Works

### Instance Detection

The `AuthentikTestFixture`:

1. **Requires AUTHENTIK_BASE_URL**: Must be set (no defaults for CI/CD compatibility)
2. **Verifies accessibility**: Makes an HTTP request to confirm the instance is reachable
3. **Optional local container check**: Only if `AUTHENTIK_CHECK_LOCAL_CONTAINER=true` (local dev only)
4. **Validates API token**: Checks if `AUTHENTIK_API_TOKEN` is set (optional)

### Test Behavior

- ✅ **Instance accessible + Token set**: All tests execute
- ⚠️ **Instance accessible + No token**: Tests that require auth are skipped
- ⚠️ **Instance not accessible**: All tests are skipped with helpful message
- ❌ **AUTHENTIK_BASE_URL not set**: Tests fail with clear error (prevents silent failures)

### Test Fixture

The `AuthentikTestFixture` provides:

- `BaseUrl`: Authentik instance URL
- `ApiToken`: API token for authentication
- `IsContainerRunning`: Whether container is detected and accessible

## Configuration

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `AUTHENTIK_BASE_URL` | `http://localhost:9100` | Base URL of Authentik instance |
| `AUTHENTIK_API_TOKEN` | (none) | API token for authentication |
| `AUTHENTIK_HTTP_PORT` | `9100` | HTTP port (used if BASE_URL not set) |

### Local Container Configuration (Optional)

For local development, you can optionally check for a specific container:

- `AUTHENTIK_CHECK_LOCAL_CONTAINER=true` - Enable local container detection
- `AUTHENTIK_CONTAINER_NAME=mamey-authentik-server` - Container name to check (optional)

**Note**: This is for local development convenience only. Tests work with any Authentik instance.

## Example Test

```csharp
[Collection("AuthentikIntegration")]
public class CoreServiceIntegrationTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public CoreServiceIntegrationTests(AuthentikTestFixture fixture)
    {
        _fixture = fixture;
        
        if (!_fixture.IsContainerRunning || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return; // Tests will be skipped
        }
        
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = _fixture.BaseUrl;
            options.ApiToken = _fixture.ApiToken;
        });

        var serviceProvider = services.BuildServiceProvider();
        _client = serviceProvider.GetRequiredService<IAuthentikClient>();
    }

    [Fact]
    public async Task ListUsersAsync_ReturnsPaginatedResult()
    {
        if (!_fixture.IsContainerRunning || _client == null)
        {
            return; // Skip if container not running
        }

        var result = await _client.Core.ListUsersAsync(page: 1, pageSize: 10);
        result.Should().NotBeNull();
    }
}
```

## Troubleshooting

### Container Not Found

**Error**: "Authentik container 'mamey-authentik-server' is not running"

**Solution**:
```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith
docker-compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d
```

### Connection Refused

**Error**: "container is running but not accessible"

**Solutions**:
1. Check the port: `docker ps | grep authentik` (should show port 9100)
2. Verify container health: `docker logs mamey-authentik-server`
3. Check if port is correct: Default is `9100` from `AUTHENTIK_HTTP_PORT`

### Authentication Errors

**Error**: 401/403 responses

**Solutions**:
1. Verify token is correct: Check in Authentik admin
2. Check token permissions: Ensure token has required scopes
3. Regenerate token if expired
4. Verify token is set: `echo $AUTHENTIK_API_TOKEN`

## Running Specific Tests

```bash
# Run only integration tests
dotnet test --filter "Category=Integration"

# Run specific test class
dotnet test --filter "FullyQualifiedName~CoreServiceIntegrationTests"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## CI/CD Integration

For CI pipelines, ensure:

1. Authentik container is started before tests
2. Environment variables are set in CI configuration
3. Tests wait for container to be healthy
4. Cleanup containers after tests complete

Example CI step:

```yaml
- name: Start Authentik
  run: |
    cd Mamey.Government/Monolith
    docker-compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d
    # Wait for container to be healthy
    timeout 120 bash -c 'until docker ps --filter name=mamey-authentik-server --format "{{.Status}}" | grep -q "Up"; do sleep 2; done'

- name: Run Integration Tests
  env:
    AUTHENTIK_BASE_URL: http://localhost:9100
    AUTHENTIK_API_TOKEN: ${{ secrets.AUTHENTIK_API_TOKEN }}
  run: |
    cd Mamey/src/Mamey.Authentik
    dotnet test --filter "Category=Integration"
```

## Benefits

✅ **No duplicate containers**: Uses your existing setup  
✅ **Real configuration**: Tests against your actual Authentik instance  
✅ **Fast startup**: No need to wait for container initialization  
✅ **Easy debugging**: Can inspect container logs and state  
✅ **Flexible**: Can test against different environments by changing URL

## Next Steps

1. **Add more integration tests**: Test all 23 services
2. **Test scenarios**: Add end-to-end workflow tests
3. **Performance tests**: Measure response times and throughput
4. **Error handling**: Test error scenarios and edge cases

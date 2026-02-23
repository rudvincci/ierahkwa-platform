# Local Development - Integration Tests

This guide is for **local development only**. For CI/CD, see the main README.

## Quick Start for Local Development

### 1. Start Your Local Authentik Container

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith
docker-compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d
```

### 2. Get API Token

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
export AUTHENTIK_CHECK_LOCAL_CONTAINER="true"  # Optional: enables local container detection
```

### 4. Run Tests

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey/src/Mamey.Authentik
dotnet test
```

## Local-Only Features

When `AUTHENTIK_CHECK_LOCAL_CONTAINER=true`:

- Tests will check if your local Docker container (`mamey-authentik-server`) is running
- Provides helpful messages if container is not found
- This is **purely informational** - tests work with any Authentik instance

## Shell Profile Setup

Add to `~/.zshrc` or `~/.bashrc`:

```bash
# Authentik Integration Tests (Local Development)
export AUTHENTIK_BASE_URL="http://localhost:9100"
export AUTHENTIK_API_TOKEN="your-token-here"
export AUTHENTIK_CHECK_LOCAL_CONTAINER="true"
```

Then reload:
```bash
source ~/.zshrc  # or source ~/.bashrc
```

## Notes

- **Local container check is optional**: Tests work with any Authentik instance
- **CI/CD compatibility**: Tests will work in CI/CD without local container checks
- **No hardcoded dependencies**: Tests are instance-agnostic

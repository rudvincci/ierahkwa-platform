# Maestro Docker Container

Maestro can run as a Docker container, making it easy to deploy and use across different environments.

## Quick Start

### Option 1: Using Docker Compose (Recommended)

1. **Set workspace path** (your repository):
   ```bash
   export WORKSPACE_PATH=/path/to/your/repository
   ```

2. **Start Maestro**:
   ```bash
   docker-compose up -d maestro
   ```

3. **Run commands**:
   ```bash
   docker-compose exec maestro maestro flows
   docker-compose exec maestro maestro run --flow my-workflow
   ```

### Option 2: Using Docker Directly

1. **Build image**:
   ```bash
   docker build -t mamey/maestro:latest .
   ```

2. **Run container**:
   ```bash
   docker run -it --rm \
     -v /path/to/your/repo:/workspace \
     -v maestro-checkpoints:/workspace/.maestro/checkpoints \
     -v maestro-cache:/workspace/.maestro/cache \
     -v maestro-reports:/workspace/.maestro/reports \
     -v maestro-logs:/workspace/.maestro/logs \
     -v maestro-config:/workspace/.maestro/config \
     -p 3000:3000 \
     -p 3001:3001 \
     mamey/maestro:latest \
     maestro flows
   ```

## Configuration

### Environment Variables

Create `.env` file or set environment variables:

```bash
# Your repository path (absolute path)
WORKSPACE_PATH=/path/to/your/repository

# Dashboard port
DASHBOARD_PORT=3000

# MCP server port
MCP_PORT=3001

# Cursor agent path (if installed on host)
CURSOR_AGENT_PATH=/usr/local/bin/cursor-agent
```

### Volume Mounts

The container mounts several volumes:

| Volume | Purpose | Persistence |
|--------|---------|-------------|
| `/workspace` | Your repository | Host filesystem |
| `maestro-checkpoints` | Workflow checkpoints | Docker volume |
| `maestro-cache` | Result cache | Docker volume |
| `maestro-reports` | Execution reports | Docker volume |
| `maestro-logs` | Log files | Docker volume |
| `maestro-config` | Configuration | Docker volume |

## Usage Examples

### List Workflows

```bash
docker-compose exec maestro maestro flows
```

### Run Workflow

```bash
docker-compose exec maestro maestro run \
  --flow my-workflow \
  --runner cursor \
  --model claude-3-5-sonnet-20241022
```

### Start Dashboard

```bash
docker-compose exec maestro maestro dashboard
# Or use the dashboard-only service:
docker-compose --profile dashboard-only up -d maestro-dashboard
```

### Access Dashboard

Open browser: `http://localhost:3000`

### View Logs

```bash
docker-compose logs -f maestro
```

### Stop Container

```bash
docker-compose down
```

## Cursor CLI Integration

### Option 1: Mount cursor-agent from Host

If `cursor-agent` is installed on your host:

```yaml
volumes:
  - ${CURSOR_AGENT_PATH:-/usr/local/bin/cursor-agent}:/usr/local/bin/cursor-agent:ro
```

### Option 2: Install in Container

Add to Dockerfile:

```dockerfile
# Install cursor-agent (adjust based on installation method)
RUN curl -L https://cursor.sh/install.sh | sh
```

### Option 3: Use Network Mode

If cursor-agent runs in another container:

```yaml
network_mode: host
```

## Private Data Sync

For private repository sync, you need:

1. **Git credentials**:
   ```yaml
   volumes:
     - ${HOME}/.gitconfig:/root/.gitconfig:ro
     - ${HOME}/.ssh:/root/.ssh:ro
   ```

2. **Configure sync** in `.maestro/config/orchestrator.config.yml`:
   ```yaml
   privateSync:
     enabled: true
     repositoryUrl: "https://github.com/your-username/your-private-repo.git"
   ```

## Development

### Build for Development

```bash
docker build -t mamey/maestro:dev \
  --target builder \
  -f Dockerfile .
```

### Run with Source Mounted

```bash
docker run -it --rm \
  -v $(pwd):/app \
  -v /path/to/repo:/workspace \
  -p 3000:3000 \
  mamey/maestro:dev \
  npm run dev
```

## Production Deployment

### Multi-stage Build

The Dockerfile uses multi-stage builds:
- **Builder stage**: Installs dependencies and builds TypeScript
- **Runtime stage**: Only production dependencies and built files

### Image Size Optimization

- Uses `node:20-alpine` (smaller base image)
- Only production dependencies in final image
- Multi-stage build reduces layers

### Health Checks

Add to docker-compose.yml:

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:3000/api/health"]
  interval: 30s
  timeout: 10s
  retries: 3
```

## Troubleshooting

### Permission Issues

If you get permission errors:

```bash
# Fix ownership
docker-compose exec maestro chown -R $(id -u):$(id -g) /workspace/.maestro
```

### Cursor Agent Not Found

If `cursor-agent` is not found:

1. Check if it's mounted correctly
2. Verify path in `CURSOR_AGENT_PATH`
3. Install in container or use network mode

### Git Operations Fail

If git operations fail:

1. Ensure `.gitconfig` and `.ssh` are mounted
2. Check SSH key permissions: `chmod 600 ~/.ssh/id_rsa`
3. Verify git credentials are configured

### Port Conflicts

If ports are already in use:

```bash
# Change ports in .env
DASHBOARD_PORT=3002
MCP_PORT=3003
```

## Docker Compose Profiles

### Dashboard Only

Run only the dashboard service:

```bash
docker-compose --profile dashboard-only up -d maestro-dashboard
```

### Full Stack

Run all services:

```bash
docker-compose up -d
```

## Best Practices

1. **Use volumes** for data persistence
2. **Mount workspace** as read-write for workflow execution
3. **Use .env file** for configuration
4. **Keep secrets** in environment variables or mounted files
5. **Use health checks** for production
6. **Monitor logs** for debugging

## Example docker-compose.override.yml

Create `docker-compose.override.yml` for local overrides:

```yaml
version: '3.8'

services:
  maestro:
    environment:
      - NODE_ENV=development
    volumes:
      # Mount source for development
      - ./src:/app/src:ro
      - ./config:/app/config:ro
```

---

**Note**: The container runs as root by default. For production, consider using a non-root user.

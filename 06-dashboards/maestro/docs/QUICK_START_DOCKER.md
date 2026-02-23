# Quick Start - Docker 🐳

## Prerequisites

- Docker installed
- Docker Compose installed
- Your repository path

## 1. Build Image

```bash
npm run docker:build
# Or: docker build -t mamey/maestro:latest .
```

## 2. Configure

Create `.env` file:

```bash
WORKSPACE_PATH=/path/to/your/repository
DASHBOARD_PORT=3000
MCP_PORT=3001
```

## 3. Start Container

```bash
npm run docker:run
# Or: docker-compose up -d maestro
```

## 4. Run Commands

```bash
# List workflows
npm run docker:exec flows

# Run workflow
npm run docker:exec run --flow my-workflow --runner cursor

# Start dashboard
npm run docker:exec dashboard

# Access dashboard
open http://localhost:3000
```

## 5. Stop Container

```bash
npm run docker:stop
# Or: docker-compose down
```

## Common Commands

```bash
# View logs
npm run docker:logs

# Execute any maestro command
npm run docker:exec <command>

# Shell access
npm run docker:shell
```

## Volume Persistence

All data persists in Docker volumes:
- Checkpoints: `maestro-checkpoints`
- Cache: `maestro-cache`
- Reports: `maestro-reports`
- Logs: `maestro-logs`
- Config: `maestro-config`

## See Also

- **[DOCKER.md](./DOCKER.md)** - Complete Docker documentation
- **[README.md](./README.md)** - General Maestro documentation

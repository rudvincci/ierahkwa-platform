# Installation Guide

Complete installation guide for Maestro.

## Prerequisites

- **Node.js** 20+ (for local installation)
- **npm** (comes with Node.js)
- **Docker** 20.10+ (for container installation, optional)
- **Git** (for submodule management)
- **Cursor CLI** (optional, for full functionality)

## Installation Options

Maestro can be installed in two ways:

1. **[Local Installation](#local-installation)** - Direct installation on your system
2. **[Docker Installation](#docker-installation)** - Containerized installation

See [Installation Options](INSTALLATION_OPTIONS.md) for detailed comparison.

## Local Installation

### Step 1: Navigate to Maestro Directory

```bash
cd .maestro
```

### Step 2: Install Dependencies

```bash
npm install
```

### Step 3: Build TypeScript

```bash
npm run build
```

### Step 4: Verify Installation

```bash
maestro --version
# Or: node dist/cli/index.js --version
```

### Step 5: Make Available Globally (Optional)

```bash
npm link
# Now you can use 'maestro' from anywhere
```

## Docker Installation

### Step 1: Build Image

```bash
cd .maestro
npm run docker:build
# Or: docker build -t mamey/maestro:latest .
```

### Step 2: Configure Environment

Create `.env` file:

```bash
WORKSPACE_PATH=/path/to/your/repository
DASHBOARD_PORT=3000
MCP_PORT=3001
```

### Step 3: Start Container

```bash
npm run docker:run
# Or: docker-compose up -d maestro
```

### Step 4: Verify Installation

```bash
npm run docker:exec flows
# Or: docker-compose exec maestro maestro flows
```

## Post-Installation

### Initialize Configuration

```bash
# Copy example config
cp config/orchestrator.config.yml.example config/orchestrator.config.yml

# Edit with your settings
vim config/orchestrator.config.yml
```

### Test Installation

```bash
# List workflows
maestro flows

# Run a test workflow (dry-run)
maestro run --flow my-workflow --dry-run
```

### Start Dashboard

```bash
maestro enable
# Access at http://localhost:3000
```

## Troubleshooting

### Node.js Not Found

Install Node.js 20+:
- **macOS**: `brew install node@20`
- **Linux**: See [NodeSource](https://github.com/nodesource/distributions)
- **Windows**: Download from [nodejs.org](https://nodejs.org/)

### Build Errors

```bash
# Clean and rebuild
rm -rf dist node_modules
npm install
npm run build
```

### Permission Denied

```bash
# Make scripts executable
chmod +x scripts/*.sh
```

### Docker Issues

See [Docker Guide](DOCKER.md) for Docker-specific troubleshooting.

## Next Steps

- Read [Quick Start Guide](QUICK_START.md)
- Explore [Usage Guide](USAGE_GUIDE.md)
- Check [Configuration Guide](CONFIGURATION.md)

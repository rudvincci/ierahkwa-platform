# Quick Start Guide

Get Maestro running in 5 minutes.

## Choose Your Method

- **[Local](#local-installation)** - Fast setup for development
- **[Docker](#docker-installation)** - Isolated setup for production

## Local Installation

### 1. Install & Build

```bash
cd .maestro
npm install
npm run build
```

### 2. List Workflows

```bash
maestro flows
```

### 3. Run a Workflow

```bash
maestro run --flow my-workflow --runner cursor
```

### 4. Start Dashboard

```bash
maestro enable
# Open http://localhost:3000
```

## Docker Installation

### 1. Build Image

```bash
cd .maestro
npm run docker:build
```

### 2. Start Container

```bash
echo "WORKSPACE_PATH=$(pwd)/.." > .env
npm run docker:run
```

### 3. Run Workflows

```bash
npm run docker:exec flows
npm run docker:exec run --flow my-workflow --runner cursor
```

## Next Steps

- Read [Usage Guide](USAGE_GUIDE.md) for detailed usage
- Check [Workflow Guide](WORKFLOWS.md) to create workflows
- Explore [Dashboard Guide](DASHBOARD.md) for monitoring

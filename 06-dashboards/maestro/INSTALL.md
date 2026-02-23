# Agent Orchestrator Installation Guide

## Quick Install

### Option 1: Install in Current Directory

From the orchestrator directory:
```bash
cd .agent-orchestrator
bash scripts/install.sh
```

### Option 2: Install in Specific Directory

```bash
cd .agent-orchestrator
bash scripts/install.sh /path/to/project
```

### Option 3: Initialize in New Project

From your project root:
```bash
# If orchestrator is a submodule or copied
cd /path/to/project
bash .agent-orchestrator/scripts/init.sh
```

## What Gets Installed

1. **Dependencies**: npm packages (commander, js-yaml, uuid, winston, zod)
2. **Build**: TypeScript compiled to JavaScript
3. **Logs Directory**: Created for execution logs
4. **Cursor Rules**: Agent orchestrator rules copied to `.cursor/rules/`
5. **Wrapper Script**: Convenience script in project root

## Prerequisites

- **Node.js 18+**: Required for running the orchestrator
- **npm**: Package manager (comes with Node.js)
- **Git**: Required for submodule management
- **Cursor CLI** (optional): For full functionality with Cursor integration
  - Install: https://cursor.com/docs/cli/overview

## Submodule Setup

The orchestrator integrates with memory systems that are GitHub submodules:

- **`.skmemory`** - SKMemory system (submodule)
- **`.composermemory`** - ComposerMemory system (submodule, optional)
- **`.agent-orchestrator`** - Orchestrator itself (submodule)

### Initialize Submodules

```bash
# Initialize all submodules (recommended)
bash .agent-orchestrator/scripts/init-submodules.sh

# Or manually
git submodule update --init --recursive
```

### Verify Submodules

```bash
# Check status
git submodule status

# Should show:
#  <hash> .skmemory (v1.0.0)
#  <hash> .composermemory (v1.0.0) [optional]
#  <hash> .agent-orchestrator (v1.0.0)
```

## Installation Steps

The installation script automatically:

1. ✅ Checks Node.js and npm versions
2. ✅ Installs npm dependencies
3. ✅ Builds TypeScript code
4. ✅ Creates logs directory
5. ✅ Sets up Cursor rules
6. ✅ Creates wrapper scripts

## Post-Installation

### Verify Installation

```bash
cd .agent-orchestrator
npm run dev flows
```

You should see available workflows and agent roles.

### Initialize Cursor Rules

```bash
cd .agent-orchestrator
bash scripts/cursor-init.sh
```

This copies orchestrator rules to `.cursor/rules/agent-orchestrator.md` for Cursor AI integration.

### Test Execution

```bash
# Dry-run (preview execution plan)
cd .agent-orchestrator
npm run dev run --flow feature_implementation --dry-run

# Execute with dummy runner (testing)
npm run dev run --flow feature_implementation --runner dummy
```

## Usage

### Using the Wrapper Script

After installation, a wrapper script is created in the project root:

```bash
# From project root
./orchestrator flows
./orchestrator run --flow feature_implementation --dry-run
```

### Using npm Scripts

Add to your project's `package.json`:

```json
{
  "scripts": {
    "orchestrator": "cd .agent-orchestrator && npm run dev"
  }
}
```

Then:
```bash
npm run orchestrator flows
npm run orchestrator run --flow feature_implementation --dry-run
```

### Direct Usage

```bash
cd .agent-orchestrator
npm run dev flows
npm run dev run --flow feature_implementation --runner cursor --feature "Description"
```

## Troubleshooting

### Node.js Not Found

```bash
# Install Node.js 18+
# macOS
brew install node@18

# Linux (Ubuntu/Debian)
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# Windows
# Download from https://nodejs.org/
```

### Cursor CLI Not Found

The orchestrator works without Cursor CLI, but you'll need it for actual agent execution:

```bash
# Install Cursor CLI
# See: https://cursor.com/docs/cli/overview
```

### Build Errors

```bash
# Clean and rebuild
cd .agent-orchestrator
rm -rf dist node_modules
npm install
npm run build
```

### Permission Denied

```bash
# Make scripts executable
chmod +x .agent-orchestrator/scripts/*.sh
```

## Uninstallation

To remove the orchestrator:

```bash
# Remove orchestrator directory
rm -rf .agent-orchestrator

# Remove wrapper script (if created)
rm orchestrator

# Remove Cursor rules (optional)
rm .cursor/rules/agent-orchestrator.md
```

## Integration with Existing Projects

The orchestrator can be added to existing projects as:

1. **Git Submodule**:
   ```bash
   git submodule add <repo-url> .agent-orchestrator
   cd .agent-orchestrator
   bash scripts/install.sh
   ```

2. **Copied Directory**:
   ```bash
   cp -r /path/to/orchestrator .agent-orchestrator
   cd .agent-orchestrator
   bash scripts/install.sh
   ```

3. **npm Package** (future):
   ```bash
   npm install -g @mamey/agent-orchestrator
   ```

## Next Steps

After installation:

1. **Read Documentation**: `.agent-orchestrator/docs/README.md`
2. **Review Config**: `.agent-orchestrator/config/orchestration.yml`
3. **List Workflows**: `npm run dev flows`
4. **Create Workflows**: Edit `config/orchestration.yml`
5. **Execute Workflows**: `npm run dev run --flow <name>`

## Support

- **Documentation**: `.agent-orchestrator/docs/`
- **Issues**: Report in repository
- **Examples**: `.agent-orchestrator/config/orchestration-enhanced.yml`

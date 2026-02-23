# Agent Orchestrator Installation Scripts

This directory contains installation and initialization scripts for the Agent Orchestrator, similar to `.skmemory`'s installation pattern.

## Scripts

### `install.sh` - Main Installation Script

Installs the orchestrator in the current directory or specified directory.

**Usage:**
```bash
# Install in current directory (from orchestrator directory)
bash scripts/install.sh

# Install in specific directory
bash scripts/install.sh /path/to/project
```

**What it does:**
1. ✅ Checks Node.js and npm versions
2. ✅ Installs npm dependencies
3. ✅ Builds TypeScript code
4. ✅ Creates logs directory
5. ✅ Sets up Cursor rules
6. ✅ Creates wrapper scripts

### `init.sh` - Initialize in New Project

Initializes the orchestrator in a new project directory.

**Usage:**
```bash
# From project root
bash .agent-orchestrator/scripts/init.sh
```

**What it does:**
1. Copies orchestrator files to project
2. Runs installation script
3. Sets up project structure

### `cursor-init.sh` - Initialize Cursor Rules

Copies Cursor rules to project root for AI integration.

**Usage:**
```bash
cd .agent-orchestrator
bash scripts/cursor-init.sh
```

**What it does:**
1. Creates `.cursor/rules/` directory if needed
2. Copies `agent-orchestrator.md` rule file
3. Copies legacy `.cursorrules` if present

## Installation Flow

```
┌─────────────────────────────────────┐
│  New Project / Directory            │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Run: bash scripts/install.sh      │
│  or: bash scripts/init.sh           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Install Dependencies               │
│  Build TypeScript                    │
│  Create Directories                  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Run: bash scripts/cursor-init.sh   │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Ready to Use!                      │
│  npm run dev flows                  │
└─────────────────────────────────────┘
```

## Comparison with .skmemory

The orchestrator follows the same installation pattern as `.skmemory`:

| Feature | .skmemory | .agent-orchestrator |
|---------|-----------|---------------------|
| Installation Script | `scripts/init-memory.sh` | `scripts/install.sh` |
| Cursor Init | `scripts/cursor-init.sh` | `scripts/cursor-init.sh` |
| Project Integration | Copies rules to `.cursor/rules/` | Copies rules to `.cursor/rules/` |
| Wrapper Scripts | Creates convenience scripts | Creates wrapper script |
| Self-Contained | Yes | Yes |

## Requirements

All scripts require:
- Bash shell
- Node.js 18+
- npm
- Write permissions in target directory

## Error Handling

Scripts use `set -e` to exit on errors and provide colored output:
- 🟢 Green: Success/Info
- 🟡 Yellow: Warnings
- 🔴 Red: Errors
- 🔵 Blue: Steps

## Testing

Test installation:
```bash
# Test install script
cd .agent-orchestrator
bash scripts/install.sh /tmp/test-install

# Verify installation
cd /tmp/test-install/.agent-orchestrator
npm run dev flows
```

## Troubleshooting

### Permission Denied
```bash
chmod +x scripts/*.sh
```

### Node.js Not Found
Install Node.js 18+ from https://nodejs.org/

### Build Errors
```bash
rm -rf node_modules dist
npm install
npm run build
```

# Configuration Setup

## Overview

Maestro uses `orchestrator.config.yml` for user-specific configuration. This file is **excluded from git** to protect your settings (like private repository URLs and encryption keys).

## Essential Configuration: Heap Memory

**Important:** Configure heap memory to prevent crashes during large workflow executions.

### Quick Setup

Add to `.maestro/config/orchestrator.config.yml`:

```yaml
execution:
  heapMemorySize: 8192  # 8GB default (prevents crashes)
```

### When to Increase

- Maestro crashes with "out of memory" errors
- Running very large workflows
- Processing large codebases

### Configuration Options

**Via Config File:**
```yaml
execution:
  heapMemorySize: 16384  # 16GB for large projects
```

**Via Environment Variable:**
```bash
export MAESTRO_HEAP_SIZE=16384
maestro enable
```

**Priority:** Environment variable > Config file > Default (8GB)

See [Configuration Guide](CONFIGURATION.md#execution-configuration) for details.

## Initial Setup

### Step 1: Copy Example Config

```bash
cp .maestro/config/orchestrator.config.yml.example .maestro/config/orchestrator.config.yml
```

### Step 2: Edit Configuration

Edit `.maestro/config/orchestrator.config.yml` and customize:

```yaml
# Private Data Sync (OPT-IN ONLY)
privateSync:
  enabled: false  # Set to true to enable
  repositoryUrl: "https://github.com/your-username/your-private-repo.git"
  encryptionKey: "your-encryption-key-here"
  syncInterval: 60
  includeCheckpoints: true
  includeReports: true
  includeCache: false
  includeLogs: false
```

### Step 3: Or Use Interactive Setup

```bash
maestro sync:configure
```

This will:
- Ask for your repository URL
- Generate an encryption key (or use yours)
- Configure what data to sync
- Save everything to `orchestrator.config.yml`

## Configuration File Location

Maestro looks for configuration in this order:

1. `.maestro/config/orchestrator.config.yml` (recommended)
2. `config/orchestrator.config.yml` (project root)
3. `orchestrator.config.yml` (project root)

## What's Protected

The following are **excluded from git**:

- ✅ `orchestrator.config.yml` - Your user-specific config
- ✅ `.maestro/config/orchestrator.config.yml` - Maestro config directory
- ✅ `config/orchestrator.config.yml` - Alternative location

The following **are committed** (as templates):

- ✅ `orchestrator.config.yml.example` - Example template
- ✅ `.maestro/config/orchestrator.config.yml.example` - Maestro example

## Security Notes

1. **Never commit** `orchestrator.config.yml` - It contains:
   - Private repository URLs
   - Encryption keys
   - User-specific settings

2. **Use the example file** as a template:
   ```bash
   cp orchestrator.config.yml.example orchestrator.config.yml
   ```

3. **Verify gitignore** - Check that your config is excluded:
   ```bash
   git check-ignore orchestrator.config.yml
   ```

## Configuration Options

See `orchestrator.config.yml.example` for all available options:

- Memory integration settings
- Manager agent settings
- Workflow execution settings
- Logging settings
- **Private data sync settings** (OPT-IN ONLY)

## Troubleshooting

### Config Not Found

If you get "Config file not found", create it:

```bash
cp .maestro/config/orchestrator.config.yml.example .maestro/config/orchestrator.config.yml
```

### Config in Git

If your config file is being tracked by git:

```bash
# Remove from git (but keep locally)
git rm --cached orchestrator.config.yml

# Verify it's ignored
git check-ignore orchestrator.config.yml
```

### Multiple Config Files

If you have config files in multiple locations, Maestro uses the first one found (in order above). Remove duplicates to avoid confusion.

---

**Remember**: Your `orchestrator.config.yml` stays local and is never committed! 🔒

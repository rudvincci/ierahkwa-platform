# Orchestrator Configuration

## Overview

The orchestrator uses a configuration file (`config/orchestrator.config.yml`) to control behavior, including memory integration, manager agent settings, execution parameters, and logging.

## Configuration File

Location: `.agent-orchestrator/config/orchestrator.config.yml`

If the configuration file doesn't exist, the orchestrator uses sensible defaults and continues to work.

## Memory Integration Configuration

### Enable/Disable Memory

```yaml
memory:
  enabled: true  # Set to false to disable all memory integration
```

### Individual Memory Systems

```yaml
memory:
  systems:
    skmemory:
      enabled: true
      path: .skmemory  # Custom path if needed
    composermemory:
      enabled: true
      path: .composermemory
    orchestrator:
      enabled: true
      path: .agent-orchestrator
```

### Auto-Initialize Submodules

```yaml
memory:
  autoInitializeSubmodules: false  # Set to true to auto-init if missing
```

### Search Settings

```yaml
memory:
  search:
    maxResults: 10        # Max results per system
    minRelevance: 0.3    # Minimum relevance score (0.0-1.0)
    timeout: 5000       # Timeout in milliseconds
```

## Manager Agent Configuration

### Enable/Disable Manager

```yaml
manager:
  enabled: true
```

### Memory Usage in Manager

```yaml
manager:
  useMemory: true  # Use memory in manager operations
```

### Memory Unavailable Behavior

```yaml
manager:
  memoryUnavailableBehavior: "warn"  # Options: "warn", "skip", "error"
```

**Behavior Options:**
- `warn`: Show warning and continue (default)
- `skip`: Silently skip memory operations
- `error`: Throw error if memory unavailable

## CLI Overrides

You can override configuration via CLI flags:

```bash
# Disable memory for this command
npm run dev manager analyze --no-memory

# Enable memory (overrides config)
npm run dev manager analyze --memory

# Works with all manager commands
npm run dev manager suggest --no-memory
npm run dev manager optimize --memory
```

## Execution Configuration

### Heap Memory Size

Configure Node.js heap memory to prevent crashes during large workflow executions:

```yaml
execution:
  # Node.js heap memory size in MB (default: 8192 = 8GB)
  # Increase this if Maestro crashes due to memory issues
  # Common values: 4096 (4GB), 8192 (8GB), 16384 (16GB)
  heapMemorySize: 8192  # 8GB default to prevent crashes
```

**Priority Order:**
1. Environment variable `MAESTRO_HEAP_SIZE` (highest priority)
2. Config file `execution.heapMemorySize`
3. Default: 8192 MB (8GB)

**Examples:**
```bash
# Via environment variable
export MAESTRO_HEAP_SIZE=16384
maestro enable

# Via config file
# Edit .maestro/config/orchestrator.config.yml
execution:
  heapMemorySize: 16384  # 16GB
```

**Note:** All Node.js processes (dashboard, workflows, MCP server) use this heap memory setting.

### Execution Timeout

```yaml
execution:
  defaultTimeout: 7200  # 2 hours in seconds
```

### Max Concurrency

```yaml
execution:
  maxConcurrency: 10  # Maximum parallel tasks
```

## Working Without Memory

The orchestrator works perfectly fine without memory systems:

1. **Memory Disabled in Config**:
   ```yaml
   memory:
     enabled: false
   ```

2. **Memory Systems Unavailable**:
   - If `.skmemory` or `.composermemory` submodules aren't initialized
   - If submodules don't exist
   - The orchestrator continues with `warn` or `skip` behavior

3. **CLI Override**:
   ```bash
   npm run dev manager analyze --no-memory
   ```

## Default Configuration

If no config file exists, defaults are:

- Memory: **Enabled** (but gracefully handles unavailable systems)
- Manager: **Enabled** with memory
- Behavior: **Warn** when memory unavailable
- Auto-init: **Disabled**

## Examples

### Disable Memory Completely

```yaml
memory:
  enabled: false
```

### Disable Only SKMemory

```yaml
memory:
  enabled: true
  systems:
    skmemory:
      enabled: false
```

### Fail Fast if Memory Unavailable

```yaml
manager:
  memoryUnavailableBehavior: "error"
```

### Auto-Initialize Submodules

```yaml
memory:
  autoInitializeSubmodules: true
```

## Agent Cleanup Configuration

Maestro automatically cleans up old cursor-agent created agents to prevent accumulation. This uses `cursor-agent ls` to list agents and `cursor-agent rm` to delete them.

### Enable/Disable Agent Cleanup

```yaml
agentCleanup:
  enabled: true  # Set to false to disable automatic cleanup
```

### Cleanup Interval

Configure how often cleanup runs:

```yaml
agentCleanup:
  interval:
    value: 24        # Run cleanup every 24 hours
    unit: hours      # Options: hours, days, months
```

**Examples:**
- `value: 12, unit: hours` - Run every 12 hours
- `value: 1, unit: days` - Run once per day
- `value: 1, unit: months` - Run once per month

### Minimum Age for Deletion

Configure the minimum age before agents are deleted:

```yaml
agentCleanup:
  minAge:
    value: 7         # Delete agents older than 7 days
    unit: days        # Options: hours, days, months
```

**Examples:**
- `value: 24, unit: hours` - Delete agents older than 24 hours
- `value: 7, unit: days` - Delete agents older than 7 days (default)
- `value: 1, unit: months` - Delete agents older than 1 month

### Dry Run Mode

Test cleanup without actually deleting agents:

```yaml
agentCleanup:
  dryRun: true  # Only log what would be deleted
```

### Manual Cleanup

You can manually trigger cleanup via CLI:

```bash
# Run cleanup now
maestro agents:cleanup

# Dry run (see what would be deleted)
maestro agents:cleanup --dry-run
```

### How It Works

1. **List Agents**: Uses `cursor-agent ls` to get all created agents
2. **Parse Output**: Handles both JSON and text output formats
3. **Calculate Age**: Determines agent age from creation date or age field
4. **Filter by Age**: Only deletes agents older than `minAge`
5. **Delete Agents**: Uses `cursor-agent rm <agent-id>` to delete each old agent
6. **Log Results**: Reports how many agents were deleted, skipped, or had errors

### Default Configuration

If not specified, defaults are:
- **Enabled**: `true`
- **Interval**: `24 hours`
- **Min Age**: `7 days`
- **Dry Run**: `false`

## See Also

- [Memory Integration](./MEMORY_INTEGRATION.md) - Memory system details
- [Manager Agent](./MANAGER_AGENT.md) - Manager agent usage
- [Installation Guide](../INSTALL.md) - Installation instructions
- [CLI Reference](./CLI_REFERENCE.md) - Command-line interface reference

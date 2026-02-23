# CLI Reference

Complete reference for Maestro CLI commands.

## Global Commands

### `maestro --help`

Show help message.

### `maestro --version`

Show version information.

## Workflow Commands

### `maestro flows`

List all available workflows.

**Options:**
- `--config <path>` - Path to config file

**Example:**
```bash
maestro flows
```

### `maestro run`

Execute a workflow.

**Options:**
- `-f, --flow <name>` - Workflow name (required)
- `-r, --runner <type>` - Runner type: `cursor` or `dummy` (default: `dummy`)
- `-m, --model <model>` - AI model to use
- `-c, --config <path>` - Path to config file
- `--dry-run` - Preview execution plan without running
- `-v, --verbose` - Verbose output
- `--resume <checkpoint>` - Resume from checkpoint

**Examples:**
```bash
# Run workflow
maestro run --flow my-workflow --runner cursor

# Run with specific model
maestro run --flow my-workflow --runner cursor --model claude-3-5-sonnet-20241022

# Dry run
maestro run --flow my-workflow --dry-run

# Resume from checkpoint
maestro run --flow my-workflow --resume checkpoint-123
```

## Dashboard Commands

### `maestro dashboard`

Start the dashboard server.

**Options:**
- `-p, --port <port>` - Port number (default: 3000)
- `--repo-root <path>` - Repository root path

**Example:**
```bash
maestro dashboard
# Access at http://localhost:3000
```

### `maestro enable`

Start dashboard and MCP server.

**Example:**
```bash
maestro enable
```

### `maestro disable`

Stop dashboard and MCP server. Automatically detects running processes even if PID file is missing.

**Features:**
- Finds dashboard process by PID file or port detection
- Gracefully stops processes with SIGTERM
- Force kills if process doesn't respond
- Cleans up PID files automatically

**Example:**
```bash
maestro disable
```

**Output:**
```
🛑 Disabling Maestro Orchestrator...
✅ Stopped dashboard (PID: 12345)
```

**Note:** The command will find and stop processes even if the PID file is missing by checking port 3000.

### `maestro status`

Check status of dashboard and MCP server.

**Example:**
```bash
maestro status
```

## Agent Management Commands

### `maestro agents:cleanup`

Clean up old cursor-agent created agents. Uses `cursor-agent ls` to list agents and `cursor-agent rm` to delete old ones.

**Options:**
- `--dry-run` - Show what would be deleted without actually deleting

**Examples:**
```bash
# Run cleanup now (deletes agents older than configured minAge)
maestro agents:cleanup

# Dry run (see what would be deleted)
maestro agents:cleanup --dry-run
```

**Output:**
```
🧹 Agent Cleanup: Starting cleanup...
📊 Cleanup Results:
   ✅ Deleted: 5
   ⏭️  Skipped: 12
```

**Configuration:**
Cleanup behavior is configured in `.maestro/config/orchestrator.config.yml`:
- `agentCleanup.enabled` - Enable/disable automatic cleanup
- `agentCleanup.interval` - How often to run cleanup (hours/days/months)
- `agentCleanup.minAge` - Minimum age before deletion (hours/days/months)
- `agentCleanup.dryRun` - Test mode (doesn't actually delete)

See [Configuration Guide](./CONFIGURATION.md#agent-cleanup-configuration) for details.

## Private Sync Commands

### `maestro sync:configure`

Configure private data sync.

**Example:**
```bash
maestro sync:configure
```

### `maestro sync:now`

Trigger immediate sync.

**Example:**
```bash
maestro sync:now
```

### `maestro sync:status`

Check sync status.

**Example:**
```bash
maestro sync:status
```

## Common Options

### `--config <path>`

Specify configuration file path.

### `--verbose` or `-v`

Enable verbose output.

### `--help` or `-h`

Show help for command.

## Exit Codes

- `0` - Success
- `1` - General error
- `2` - Invalid arguments
- `3` - Workflow not found
- `4` - Execution failed

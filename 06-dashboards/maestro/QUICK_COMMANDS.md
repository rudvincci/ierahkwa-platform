# Quick Commands - `maestro` 🎼

## Overview

The orchestrator uses the command name **`maestro`** (pronounced "my-stro") - a conductor who orchestrates! Quick commands to enable/disable and check status.

## Installation

After building, link the command:

```bash
cd .agent-orchestrator
npm link
```

This creates a global `maestro` command (and keeps `agent-orchestrator` as an alias).

**Note**: The command name is `maestro` - short, memorable, and perfectly describes what it does! 🎼

## Quick Commands

### Enable Orchestrator

```bash
maestro enable
# or
maestro on
```

**What it does:**
- Starts the dashboard server
- Dashboard available at http://localhost:3000
- Saves PID file for tracking

**Options:**
- `-p, --port <port>` - Specify port (default: 3000)

**Example:**
```bash
maestro enable --port 4000
```

### Disable Orchestrator

```bash
maestro disable
# or
maestro off
```

**What it does:**
- Stops the dashboard server
- Kills the process gracefully (SIGTERM)
- Cleans up PID file
- Falls back to port-based kill if PID file missing

### Check Status

```bash
maestro status
# or
maestro info
```

**What it shows:**
- ✅/❌ Dashboard status (ENABLED/DISABLED)
- Dashboard URL if running
- Process ID (PID) if available
- MCP Server status
- Quick tips for enable/disable

**Example Output:**
```
📊 Maestro Orchestrator Status

✅ Status: ENABLED
📊 Dashboard: http://localhost:3000
🆔 PID: 12345

💡 To disable: maestro disable

🔌 MCP Server: Running on port 3001
```

## All Commands

The `maestro` command supports all orchestrator functionality:

```bash
# Workflow execution
maestro run --flow my-workflow

# Dashboard management
maestro dashboard
maestro dashboard:stop

# MCP server
maestro mcp
maestro mcp:stop

# Quick commands
maestro enable    # Start dashboard
maestro disable   # Stop dashboard
maestro status    # Check status
```

## Why "maestro"?

- **Short**: 6 letters, easy to type
- **Memorable**: Means "conductor/orchestrator" - perfect fit!
- **Pronounceable**: "my-stro"
- **Unique**: Unlikely to conflict with other tools
- **Professional**: Sounds polished and intentional

## Technical Details

### PID File Management

- **Location**: `.agent-orchestrator/dashboard.pid`
- **Purpose**: Track running dashboard process
- **Cleanup**: Automatically removed on exit

### Process Management

- **Graceful Shutdown**: SIGTERM first, then SIGKILL if needed
- **Port Fallback**: If PID file missing, tries to kill by port
- **Health Checks**: HTTP requests to verify status

### Status Checking

- **Dashboard**: Checks `/api/health` endpoint
- **MCP Server**: Checks `/mcp/health` endpoint
- **Timeout**: 1 second per check

## Usage Examples

### Start Development Session

```bash
# Enable orchestrator
maestro enable

# Check it's running
maestro status

# Open dashboard in browser
open http://localhost:3000
```

### Stop Development Session

```bash
# Disable orchestrator
maestro disable

# Verify it's stopped
maestro status
```

### Quick Workflow Run

```bash
# Enable (if not already running)
maestro enable

# Run workflow
maestro run --flow my-workflow --runner cursor

# Check status anytime
maestro status
```

## Benefits

✅ **Memorable**: Easy to remember `maestro`  
✅ **Quick**: Short commands (`enable`, `disable`, `status`)  
✅ **Intuitive**: Clear what each command does  
✅ **Reliable**: Proper process management and cleanup  
✅ **Informative**: Status command shows everything you need  

---

**Type `maestro` and orchestrate your workflows!** 🎼

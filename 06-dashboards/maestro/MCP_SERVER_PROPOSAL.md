# MCP Server Proposal for Maestro

## Overview

Implementing an MCP (Model Context Protocol) server would allow external tools (like Cursor IDE, other AI assistants, or monitoring tools) to query the orchestrator's state and receive real-time updates about what cursor-agent is doing.

## Current State

Currently, we can see:
- ✅ Which step is running
- ✅ Time elapsed
- ✅ Final results
- ❌ **NOT** what cursor-agent is doing internally (files read, edits made, operations performed)

## Proposed Solution

### Phase 1: Enhanced Activity Tracking (Immediate)

1. **Parse cursor-agent output** for activity hints
2. **Track file operations** (reads, writes, edits)
3. **Show current activity** in dashboard
4. **Stream activity updates** via WebSocket

### Phase 2: MCP Server (Future Enhancement)

An MCP server would expose:
- Current workflow state
- Real-time activity feed
- Step progress
- File operations log
- Workflow history

## Benefits

1. **Real-time visibility**: See exactly what cursor-agent is doing
2. **Better debugging**: Know which files are being accessed/modified
3. **Integration**: Other tools can query orchestrator state
4. **Enhanced dashboard**: Show current activity, not just progress

## Implementation Plan

### Step 1: Activity Tracker (Done ✅)
- Created `ActivityTracker` service
- Parses output for file operations
- Tracks current activity per workflow

### Step 2: Dashboard Integration (In Progress)
- Add `currentActivity` to `DashboardMetrics`
- Show activity in dashboard UI
- Stream activity updates via WebSocket

### Step 3: Enhanced Output Parsing
- Parse cursor-agent JSON output for activity hints
- Track file operations from output
- Show in real-time

### Step 4: MCP Server (Future)
- Implement MCP protocol server
- Expose orchestrator state as MCP resources
- Allow external tools to query state

## MCP Server Design

```typescript
// MCP Resources
- orchestrator/workflows/active
- orchestrator/workflows/{name}/status
- orchestrator/workflows/{name}/activity
- orchestrator/workflows/{name}/steps
- orchestrator/workflows/{name}/files

// MCP Tools
- orchestrator/start-workflow
- orchestrator/stop-workflow
- orchestrator/resume-workflow
- orchestrator/get-activity
```

## Usage

### Current (Activity Tracking)
```bash
# Dashboard shows current activity
npm run dev dashboard
# Open http://localhost:3000
# See "Current Activity" field showing what cursor-agent is doing
```

### Future (MCP Server)
```bash
# Start MCP server
npm run dev mcp-server

# Other tools can connect and query:
# - Current workflows
# - Activity feed
# - Step progress
```

## Next Steps

1. ✅ Create ActivityTracker service
2. 🔄 Integrate with dashboard
3. ⏳ Parse cursor-agent output for activity
4. ⏳ Add MCP server implementation
5. ⏳ Document MCP API

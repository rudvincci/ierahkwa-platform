# Unified Dashboard - Single Terminal Experience ✅

## Overview

The dashboard now provides **complete control** from a single interface! No more opening multiple terminals - everything is managed through the web dashboard.

## Features

### 1. Workflow Management
- **Select Workflow**: Dropdown list of all available workflows
- **Start Workflow**: One-click workflow execution
- **Stop Workflow**: Stop running workflows
- **Runner Selection**: Choose between Cursor CLI or Dummy runner
- **Verbose Mode**: Toggle verbose output
- **Active Executions Counter**: See how many workflows are running

### 2. MCP Server Management
- **Status Indicator**: Real-time MCP server status (Running/Stopped)
- **Start MCP Server**: Start the MCP server with one click
- **Stop MCP Server**: Stop the MCP server
- **Port Display**: Shows which port the MCP server is running on

### 3. Real-time Monitoring
- **Workflow Cards**: Visual cards showing workflow progress
- **Activity Tracking**: See what cursor-agent is doing in real-time
- **Activity History**: Timeline of recent activities
- **Metrics**: Duration, success rate, step progress

## Usage

### Start Dashboard (One Terminal!)

```bash
npm run dev dashboard
```

Then open **http://localhost:3000** in your browser.

### From the Dashboard

1. **Start a Workflow**:
   - Select a workflow from the dropdown
   - Choose runner (Cursor CLI recommended)
   - Optionally enable verbose mode
   - Click "Start Workflow"

2. **Start MCP Server** (Optional):
   - Click "Start MCP Server"
   - Status will update to "Running"
   - Port will be displayed

3. **Monitor Progress**:
   - Watch workflow cards update in real-time
   - See current activity and history
   - View metrics and progress

4. **Stop Services**:
   - Click "Stop Workflow" to stop running workflows
   - Click "Stop MCP Server" to stop the MCP server

## Benefits

✅ **Single Terminal**: Only need to run `npm run dev dashboard`  
✅ **Web-Based Control**: Everything managed through browser  
✅ **Real-time Updates**: WebSocket connection for live updates  
✅ **No CLI Needed**: Start/stop workflows without typing commands  
✅ **Visual Feedback**: See everything happening in one place  

## Architecture

```
Dashboard Server (Port 3000)
├── Web UI (HTML/JavaScript)
│   ├── Workflow Management Controls
│   ├── MCP Server Controls
│   └── Real-time Workflow Monitoring
├── API Endpoints
│   ├── /api/workflows/available
│   ├── /api/workflows/start
│   ├── /api/workflows/stop
│   ├── /api/mcp/status
│   ├── /api/mcp/start
│   └── /api/mcp/stop
└── WebSocket Server
    └── Real-time updates for workflow progress
```

## Implementation Details

### WorkflowManager Service
- Manages workflow execution via child processes
- Tracks active executions
- Handles MCP server lifecycle

### Dashboard API
- RESTful endpoints for all operations
- WebSocket for real-time updates
- Integrated with existing workflow execution system

### UI Components
- Workflow selector dropdown
- Start/Stop buttons with state management
- Status indicators
- Real-time metrics display

## Future Enhancements

- [ ] Logs/terminal view in dashboard
- [ ] Multiple workflow execution support
- [ ] Workflow history/archives
- [ ] Configuration management UI
- [ ] Workflow templates browser

---

**No more terminal juggling - everything in one place!** 🎉

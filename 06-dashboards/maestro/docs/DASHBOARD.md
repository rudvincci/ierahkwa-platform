# Dashboard Guide

The Maestro Dashboard provides real-time monitoring and control of workflow executions.

## Features

- **Real-time Updates** - Live workflow status and progress with WebSocket updates
- **Immediate Workflow Display** - Workflows appear instantly when started with "spinning up" status
- **Smart Stop Button** - Automatically enables/disables based on active workflows
- **Custom Dialog System** - Beautiful in-app dialogs replace browser alerts
- **Responsive Design** - Dashboard adapts to different screen sizes (mobile, tablet, desktop)
- **Token Tracking** - Monitor token usage and context window utilization
- **Model Selection** - Choose and change AI models mid-workflow
- **Agent Switching** - Switch agents dynamically during execution
- **Progress Tracking** - Detailed task progress and sub-task monitoring
- **Output Analysis** - AI-powered analysis and re-alignment recommendations
- **Workflow Management** - Create, edit, and manage workflows from the UI
- **System Information** - Real-time process monitoring (PID, RAM, CPU usage)

## Starting the Dashboard

### Local Installation

```bash
cd .maestro
maestro enable
# Or: maestro dashboard
```

Access at: `http://localhost:3000`

### Docker Installation

```bash
npm run docker:exec dashboard
# Or use dashboard-only service:
docker-compose --profile dashboard-only up -d maestro-dashboard
```

## Dashboard Interface

### Workflow List

View all workflows with:
- Current status (running, completed, failed)
- Duration and success rate
- Current step
- Token usage
- Model being used

### Workflow Details

Click on a workflow to see:
- Step-by-step progress
- Token usage per step
- Model history
- Agent switches
- Output analysis
- Recommendations

### Workflow Creation

Create new workflows directly from the dashboard:
1. Click "Create Workflow"
2. Enter workflow name and description
3. Add steps with agent roles
4. Define dependencies
5. Save workflow

### Model Selection

- Select model from dropdown before starting workflow
- Change model mid-workflow via API or dashboard
- View model change history

### Agent Switching

- Switch agents for specific steps
- View agent switch history
- See reasons for switches

## API Endpoints

See [API Reference](API_REFERENCE.md) for complete API documentation.

## Troubleshooting

### Dashboard Not Starting

```bash
# Check if port is in use
lsof -i :3000

# Use different port
DASHBOARD_PORT=3001 maestro dashboard
```

### No Live Updates

- Ensure WebSocket connection is established
- Check browser console for errors
- Verify workflow is sending updates

### Workflow Not Showing

- Workflows now appear immediately when started (with "Spinning up workflow..." status)
- If workflow doesn't appear, check browser console for errors
- Verify workflow is actually running via `maestro status`
- Refresh the page if needed

### Stop Button Disabled

- The stop button automatically enables when workflows are active
- If button remains disabled, check if workflows are actually running
- Use `maestro status` to verify active workflows

### Dashboard Responsiveness

- Dashboard is fully responsive and adapts to screen size
- On mobile: Single column layout, stacked metrics
- On tablet: 2-column workflow grid
- On desktop: Multi-column grid with full details

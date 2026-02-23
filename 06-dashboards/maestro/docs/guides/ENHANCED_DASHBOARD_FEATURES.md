# Enhanced Dashboard Features

## Latest Updates

### Custom Dialog System
Maestro now uses beautiful in-app modal dialogs instead of browser alerts. All notifications, confirmations, and error messages are displayed in a consistent, user-friendly interface.

**Features:**
- Modal dialogs with gradient backgrounds
- Confirmation dialogs with Cancel/OK buttons
- Auto-dismiss on outside click
- Consistent styling across all dialogs

### Immediate Workflow Display
Workflows now appear instantly in the dashboard when started, showing "Spinning up workflow..." status. No more waiting or refreshing to see active workflows.

**Features:**
- Instant workflow card creation
- "Spinning up" status during initialization
- Real-time status updates via WebSocket
- Automatic refresh when workflow metrics become available

### Smart Stop Button
The stop workflow button automatically enables when workflows are active and disables when none are running.

**Features:**
- Auto-enable when workflows start
- Auto-disable when no active workflows
- Visual feedback (opacity and cursor changes)
- Confirmation dialog before stopping

### Responsive Design
The dashboard now adapts to different screen sizes for optimal viewing on any device.

**Breakpoints:**
- **Mobile (< 480px)**: Single column, stacked metrics
- **Tablet (480px - 768px)**: 2-column workflow grid
- **Desktop (> 768px)**: Multi-column grid with full details

**Features:**
- Flexible grid layout
- Responsive workflow cards
- Adaptive metric displays
- Touch-friendly controls on mobile

### System Information Panel
Real-time monitoring of the Maestro process itself.

**Displays:**
- Process ID (PID)
- Memory usage (RAM)
- CPU usage percentage
- Uptime
- Platform information

**Updates:** Live updates every second

# Enhanced Dashboard Features - Implementation Summary

## Overview

This document summarizes the implementation of enhanced dashboard features for Maestro, including:
- Token and context window tracking
- Model selection and switching
- Agent switching mid-workflow
- Intelligent output monitoring and re-alignment
- Dynamic task progress tracking

## New Services Created

### 1. TokenUsageTracker (`src/services/TokenUsageTracker.ts`)
- Parses token usage from cursor-agent JSON responses
- Tracks input/output/total tokens per step
- Calculates context window utilization percentage
- Provides usage summaries per workflow
- Supports multiple models with different context windows

**Key Features:**
- Automatic token extraction from cursor-agent responses
- Context window size detection based on model
- Per-step and per-workflow token aggregation
- Model-specific breakdowns

### 2. ModelManager (`src/services/ModelManager.ts`)
- Manages available models (Claude 3.5 Sonnet, Haiku, etc.)
- Tracks model selection per workflow and step
- Records model change history
- Supports custom model addition

**Key Features:**
- Default Claude models pre-configured
- Per-workflow and per-step model assignment
- Change history tracking
- Model metadata (context window, capabilities, description)

### 3. AgentSwitcher (`src/services/AgentSwitcher.ts`)
- Enables switching agents mid-workflow
- Applies agent switches to workflow definitions
- Tracks switch history with reasons

**Key Features:**
- Dynamic agent assignment per step
- Workflow modification support
- Switch history with timestamps and reasons

### 4. OutputMonitor (`src/services/OutputMonitor.ts`)
- Analyzes cursor-agent output for completion status
- Detects issues and suggests improvements
- Recommends agent/model changes based on output
- Calculates completion percentage

**Key Features:**
- Pattern-based completion detection
- Issue identification
- Re-alignment recommendations
- Confidence scoring

### 5. TaskProgressTracker (`src/services/TaskProgressTracker.ts`)
- Tracks task completion percentage
- Identifies sub-tasks from output
- Monitors completed vs pending sub-tasks
- Generates task update recommendations

**Key Features:**
- Dynamic progress tracking
- Sub-task extraction from markdown/TODO patterns
- Status management (pending/running/completed/needs-update)
- Task update generation

## Dashboard Server Updates

### Enhanced DashboardMetrics Interface
Added fields to `DashboardMetrics`:
- `currentTokenUsage`: Current step token usage
- `totalTokens`: Total tokens used in workflow
- `contextWindowPercent`: Context window utilization
- `currentModel`: Currently active model
- `modelHistory`: History of model changes
- `currentAgent`: Currently active agent
- `agentSwitches`: History of agent switches
- `taskProgress`: Map of step progress
- `latestAnalysis`: Latest output analysis

### New Dashboard Methods
- `recordTokenUsage()`: Records token usage for a step
- `recordModelChange()`: Records and applies model changes
- `recordAgentSwitch()`: Records and applies agent switches
- `recordOutputAnalysis()`: Records output analysis and updates progress
- `getServices()`: Exposes services for external access

### New WebSocket Message Types
- `token-update`: Token usage updates
- `model-change`: Model change notifications
- `agent-switch`: Agent switch notifications
- `progress-update`: Task progress updates

## Integration Points

### CursorCliAgentRunner Integration
- Parse token usage from JSON responses
- Extract usage data from cursor-agent output
- Pass token usage to dashboard via workflow executor

### EnhancedWorkflowExecutor Integration
- Initialize task progress tracking on workflow start
- Monitor output after each step
- Analyze output and update progress
- Apply model/agent switches dynamically
- Track token usage per step

### Dashboard UI Updates Needed
1. **Token Display Section:**
   - Current step tokens (input/output/total)
   - Total workflow tokens
   - Context window utilization bar
   - Per-step token breakdown

2. **Model Display & Control:**
   - Current model indicator
   - Model selector dropdown (per step or workflow)
   - Model change history timeline
   - Model capabilities display

3. **Agent Display & Control:**
   - Current agent indicator
   - Agent switcher dropdown
   - Agent switch history
   - Switch reason display

4. **Progress Display:**
   - Task completion percentage per step
   - Sub-task list with checkboxes
   - Progress bars
   - Status indicators (pending/running/completed/needs-update)

5. **Analysis Display:**
   - Output analysis results
   - Re-alignment recommendations
   - Issue list
   - Suggestions panel

## API Endpoints to Add

### Model Management
- `POST /api/workflows/:workflow/model` - Change model for workflow
- `POST /api/workflows/:workflow/steps/:step/model` - Change model for step
- `GET /api/models` - List available models
- `GET /api/workflows/:workflow/model-history` - Get model change history

### Agent Management
- `POST /api/workflows/:workflow/steps/:step/agent` - Switch agent for step
- `GET /api/workflows/:workflow/agent-history` - Get agent switch history
- `GET /api/agents` - List available agents

### Token Usage
- `GET /api/workflows/:workflow/tokens` - Get token usage summary
- `GET /api/workflows/:workflow/steps/:step/tokens` - Get step token usage

### Progress & Analysis
- `GET /api/workflows/:workflow/progress` - Get task progress
- `GET /api/workflows/:workflow/steps/:step/analysis` - Get output analysis
- `GET /api/workflows/:workflow/recommendations` - Get re-alignment recommendations

## Next Steps

1. **Update Dashboard HTML** (`DashboardServer.ts` - `getDashboardHTML()`)
   - Add token display section
   - Add model selector UI
   - Add agent switcher UI
   - Add progress visualization
   - Add analysis display panel

2. **Add API Endpoints** (`DashboardServer.ts` - `setupApiRoutes()`)
   - Model management endpoints
   - Agent management endpoints
   - Token usage endpoints
   - Progress/analysis endpoints

3. **Update CursorCliAgentRunner**
   - Extract token usage from responses
   - Send to dashboard via callback

4. **Update EnhancedWorkflowExecutor**
   - Initialize progress tracking
   - Call output monitoring after each step
   - Apply model/agent switches
   - Track token usage

5. **Add JavaScript Handlers** (in dashboard HTML)
   - Model change handlers
   - Agent switch handlers
   - Progress update handlers
   - Token display updates

## Usage Examples

### Changing Model Mid-Workflow
```typescript
// Via API
POST /api/workflows/my-workflow/steps/step-1/model
{ "modelId": "claude-3-5-haiku-20241022" }

// Via Dashboard UI
// Select model from dropdown, click "Change Model"
```

### Switching Agent
```typescript
// Via API
POST /api/workflows/my-workflow/steps/step-1/agent
{ "agent": "Architect", "reason": "Output suggests architectural review needed" }

// Via Dashboard UI
// Select agent from dropdown, add reason, click "Switch Agent"
```

### Viewing Token Usage
```typescript
// Via API
GET /api/workflows/my-workflow/tokens
// Returns: { totalTokens: 150000, averageContextUsage: 75, ... }

// Via Dashboard UI
// Token usage displayed in real-time in workflow card
```

## Benefits

1. **Visibility**: Complete insight into token usage, model selection, and agent activity
2. **Control**: Ability to adjust model/agent mid-workflow based on progress
3. **Optimization**: Automatic recommendations for better performance
4. **Completion**: Dynamic task tracking ensures 100% completion
5. **Cost Management**: Token tracking helps manage API costs
6. **Quality**: Output analysis ensures tasks meet requirements

## Status

✅ **Core Services**: Implemented
✅ **Dashboard Server Integration**: Implemented
⏳ **Dashboard UI**: Needs HTML/JavaScript updates
⏳ **API Endpoints**: Need to be added
⏳ **Workflow Executor Integration**: Needs updates
⏳ **CursorCliAgentRunner Integration**: Needs token extraction

---

**Next**: Update dashboard HTML and add API endpoints for full functionality.

# Checkpoint & Resume Guide

## Overview

The orchestrator now supports **checkpoint/resume functionality** using `.skmemory` and `.claudememory` for state persistence. You can stop workflows at any time (Ctrl+C) and resume exactly where you left off.

## Features

### ✅ State Persistence
- **Automatic checkpointing** after each step
- **Auto-save** every 60 seconds (configurable)
- **Graceful shutdown** on Ctrl+C or SIGTERM
- **State stored in** `.skmemory` and `.claudememory` (with local fallback)

### ✅ Resume Capability
- **Resume from checkpoint** - Continue exactly where you left off
- **Skip completed tasks** - Only runs remaining tasks
- **Preserve context** - All previous results maintained

### ✅ Stop/Shutdown
- **Ctrl+C** - Graceful shutdown with checkpoint save
- **SIGTERM** - Graceful shutdown with checkpoint save
- **Programmatic stop** - `--stop` flag

## Usage

### Running a Workflow (Auto-Checkpoint)

```bash
# Run workflow - checkpoints are automatically created
npm run dev run -- --flow fwid-compliance --runner cursor

# Checkpoints are saved:
# - After each step completes
# - Every 60 seconds (auto-save)
# - On graceful shutdown (Ctrl+C)
```

### Stopping a Workflow

**Press Ctrl+C** during execution:

```
⏳ [RUNNING] analyze_tdd_and_plans ... (45s elapsed)
^C

⚠️  SIGINT received. Initiating graceful shutdown...
💾 Saving state and cleaning up...
💾 Checkpoint saved: checkpoint-1234567890-abc123
💡 Use --resume checkpoint-1234567890-abc123 to continue where you left off.

✅ Graceful shutdown complete.
```

### Resuming a Workflow

```bash
# List available checkpoints
npm run dev checkpoints

# Resume from checkpoint
npm run dev run -- --flow fwid-compliance --runner cursor --resume checkpoint-1234567890-abc123

# Output:
# 🔄 Resuming from checkpoint: checkpoint-1234567890-abc123
#    Completed: 3 tasks
#    Failed: 0 tasks
#    Started: 2024-01-15T10:30:00.000Z
# ⏭️  Skipping already completed step: analyze_tdd_and_plans
# ⏭️  Skipping already completed step: inventory_existing_services
# ...
```

### Managing Checkpoints

```bash
# List all checkpoints
npm run dev checkpoints

# Show checkpoint details
npm run dev checkpoint -- --id checkpoint-1234567890-abc123

# Delete a checkpoint
npm run dev delete-checkpoint -- --id checkpoint-1234567890-abc123
```

## Configuration

### Auto-Save Interval

```bash
# Change auto-save interval (default: 60 seconds)
npm run dev run -- --flow fwid-compliance --runner cursor --auto-save-interval 30
```

### Disable Checkpoints

```bash
# Disable checkpoint saving
npm run dev run -- --flow fwid-compliance --runner cursor --enable-checkpoints false
```

## Checkpoint Storage

### Storage Locations (Priority Order)

1. **`.skmemory/v1/memory/public/short-term/orchestrator-checkpoints/`**
   - Primary storage location
   - Git-tracked (if `.skmemory` is a git repo)

2. **`.claudememory/checkpoints/`**
   - Secondary storage location
   - Backup if SKMemory unavailable

3. **`.agent-orchestrator/checkpoints/`**
   - Local fallback
   - Used if neither memory system available

### Checkpoint Format

Each checkpoint is stored as JSON:

```json
{
  "id": "checkpoint-1234567890-abc123",
  "workflowName": "fwid-compliance",
  "startedAt": "2024-01-15T10:30:00.000Z",
  "lastUpdatedAt": "2024-01-15T10:35:00.000Z",
  "completedTasks": ["analyze_tdd_and_plans", "inventory_existing_services"],
  "failedTasks": [],
  "currentStep": "check_blockchain_references",
  "context": {
    "workflow": {...},
    "previousResults": {...},
    "repositoryRoot": "/path/to/repo",
    "featureDescription": "..."
  },
  "results": {...},
  "metadata": {
    "maxConcurrency": 10,
    "continueOnError": true,
    "abortOnError": false
  }
}
```

## Examples

### Example 1: Long-Running Workflow

```bash
# Start workflow
npm run dev run -- --flow fwid-compliance --runner cursor

# After 10 minutes, press Ctrl+C
# Checkpoint saved: checkpoint-1234567890-abc123

# Resume later
npm run dev run -- --flow fwid-compliance --runner cursor --resume checkpoint-1234567890-abc123
```

### Example 2: Checkpoint Management

```bash
# List checkpoints
npm run dev checkpoints

# Output:
# 📋 Available Checkpoints:
# 
# 1. checkpoint-1234567890-abc123
#    Workflow: fwid-compliance
#    Started: 2024-01-15T10:30:00.000Z
#    Last Updated: 2024-01-15T10:35:00.000Z
#    Duration: 300s
#    Progress: 3/11 completed (27%)
#    Current Step: check_blockchain_references

# Show details
npm run dev checkpoint -- --id checkpoint-1234567890-abc123

# Resume
npm run dev run -- --flow fwid-compliance --runner cursor --resume checkpoint-1234567890-abc123
```

### Example 3: Multiple Workflows

```bash
# Run workflow A
npm run dev run -- --flow workflow-a --runner cursor
# Ctrl+C -> checkpoint-a saved

# Run workflow B
npm run dev run -- --flow workflow-b --runner cursor
# Ctrl+C -> checkpoint-b saved

# List all checkpoints
npm run dev checkpoints
# Shows both checkpoint-a and checkpoint-b

# Resume workflow A
npm run dev run -- --flow workflow-a --runner cursor --resume checkpoint-a
```

## Benefits

1. **Resilience** - Workflows survive interruptions
2. **Cost Savings** - Don't re-run completed steps
3. **Debugging** - Inspect state at any point
4. **Flexibility** - Stop/resume as needed
5. **Long Workflows** - Handle multi-hour workflows safely

## Troubleshooting

### Checkpoint Not Found

```bash
# List available checkpoints
npm run dev checkpoints

# Verify checkpoint ID
npm run dev checkpoint -- --id <checkpoint-id>
```

### Checkpoint Corrupted

```bash
# Delete corrupted checkpoint
npm run dev delete-checkpoint -- --id <checkpoint-id>

# Restart workflow from beginning
npm run dev run -- --flow <workflow-name> --runner cursor
```

### Memory Systems Not Available

If `.skmemory` and `.claudememory` are not available, checkpoints are saved to:
- `.agent-orchestrator/checkpoints/`

This ensures checkpoint functionality works even without memory systems.

## Integration with Other Features

### With Verbose Mode

```bash
npm run dev run -- --flow fwid-compliance --runner cursor --verbose --resume checkpoint-123
```

### With Custom Concurrency

```bash
npm run dev run -- --flow fwid-compliance --runner cursor --resume checkpoint-123 --max-concurrency 5
```

### With Error Handling

```bash
npm run dev run -- --flow fwid-compliance --runner cursor --resume checkpoint-123 --continue-on-error
```

## Next Steps

- **Result Caching** - Coming soon (cache task results to avoid re-running)
- **Interactive Mode** - Coming soon (pause/resume interactively)
- **Workflow Templates** - Coming soon (pre-built workflow templates)

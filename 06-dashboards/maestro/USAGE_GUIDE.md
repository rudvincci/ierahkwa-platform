# 🎼 Maestro - Complete Usage Guide

## 📋 Table of Contents

1. [Quick Start](#quick-start)
2. [Basic Commands](#basic-commands)
3. [Running Workflows](#running-workflows)
4. [Real-time Dashboard](#real-time-dashboard)
5. [Checkpoints & Resume](#checkpoints--resume)
6. [Interactive Mode](#interactive-mode)
7. [Reporting & Analytics](#reporting--analytics)
8. [Advanced Features](#advanced-features)
9. [Common Use Cases](#common-use-cases)
10. [Troubleshooting](#troubleshooting)

---

## 🏃 Quick Start

### Installation

```bash
# Navigate to orchestrator directory
cd .agent-orchestrator

# Install dependencies
npm install

# Build the project
npm run build

# Make CLI globally available (optional)
npm link
```

### First Run

```bash
# Check if orchestrator is working
agent-orchestrator --help

# Or if not linked globally
npm run dev -- --help
```

---

## 📝 Basic Commands

### View Help

```bash
agent-orchestrator --help
agent-orchestrator run --help
agent-orchestrator dashboard --help
```

### List Available Workflows

```bash
# View all available workflows in config
agent-orchestrator list
```

### Check Configuration

```bash
# Verify your configuration
agent-orchestrator verify-config
```

---

## 🔄 Running Workflows

### Basic Workflow Execution

```bash
# Run a workflow
agent-orchestrator run --flow <workflow-name> --runner cursor

# Example: Run FWID compliance check
agent-orchestrator run --flow fwid-compliance --runner cursor
```

### With Options

```bash
# Run with verbose output
agent-orchestrator run --flow my-workflow --runner cursor --verbose

# Run with custom config
agent-orchestrator run --flow my-workflow --runner cursor --config custom-config.yml

# Dry run (preview without executing)
agent-orchestrator run --flow my-workflow --runner cursor --dry-run
```

### Parallel Execution

```bash
# Set max concurrency
agent-orchestrator run --flow my-workflow --runner cursor --max-concurrency 5

# Enable parallel optimization
agent-orchestrator run --flow my-workflow --runner cursor --optimize-parallel
```

---

## 📊 Real-time Dashboard

### Start Dashboard

```bash
# Start dashboard (automatically opens browser)
maestro enable

# Or start dashboard only
maestro dashboard

# Stop dashboard
maestro disable
```

**Note:** The dashboard automatically opens in your browser when started.

### Using Dashboard with Workflows

**Start Dashboard:**
```bash
maestro enable
# Dashboard opens at http://localhost:3000
```

**Start Workflow from Dashboard:**
1. Select workflow from dropdown
2. Choose AI model (or use "Auto")
3. Click "Start Workflow"
4. Workflow appears immediately with "spinning up" status

### Dashboard Features

**Latest Updates:**
- **Immediate Display**: Workflows appear instantly when started
- **Custom Dialogs**: Beautiful in-app notifications (no browser alerts)
- **Smart Controls**: Stop button auto-enables/disables
- **Responsive Design**: Works on mobile, tablet, desktop
- **System Monitoring**: Real-time process info (PID, RAM, CPU)

**Core Features:**

- **Real-time Updates**: See workflow progress as it happens
- **Metrics**: Steps completed, success rate, duration
- **Status**: Running/Completed/Failed/Paused indicators
- **Progress Bars**: Visual progress tracking
- **API Endpoints**: 
  - `GET /api/metrics` - All workflows
  - `GET /api/workflows/:name` - Specific workflow
  - `GET /api/checkpoints` - All checkpoints
  - `GET /api/reports` - All reports

---

## 💾 Checkpoints & Resume

### Automatic Checkpoints

Checkpoints are automatically created during workflow execution.

### List Checkpoints

```bash
# List all checkpoints
agent-orchestrator checkpoints

# Show checkpoint details
agent-orchestrator checkpoint --id <checkpoint-id>
```

### Resume from Checkpoint

```bash
# Resume workflow from checkpoint
agent-orchestrator run --flow my-workflow --runner cursor --resume <checkpoint-id>

# Example
agent-orchestrator run --flow fwid-compliance --runner cursor --resume abc123-def456
```

### Delete Checkpoint

```bash
agent-orchestrator delete-checkpoint --id <checkpoint-id>
```

### Checkpoint Storage

Checkpoints are stored in:
- `.skmemory/v1/memory/public/short-term/checkpoints/` (if available)
- `.claudememory/checkpoints/` (if available)
- `.agent-orchestrator/checkpoints/` (fallback)

---

## 🎮 Interactive Mode

### Enable Interactive Mode

```bash
agent-orchestrator run --flow my-workflow --runner cursor --interactive
```

### Interactive Commands

While in interactive mode, you can:

- **`continue`** - Resume execution
- **`pause`** - Pause execution
- **`skip <step-name>`** - Skip a specific step
- **`inspect <step-name>`** - View step details
- **`abort`** - Stop workflow execution
- **`help`** - Show available commands

### Example Session

```
⏸️  Workflow paused. Type 'continue' to resume, 'help' for commands.
> inspect analyze_tdd_and_plans
📋 Step: analyze_tdd_and_plans
   Status: Running
   Role: Architect
   Description: Analyze TDD and plan documents
> continue
▶️  Resuming workflow...
```

---

## 📈 Reporting & Analytics

### Generate Reports

```bash
# Generate markdown report (default)
agent-orchestrator run --flow my-workflow --runner cursor --generate-report

# Generate HTML report
agent-orchestrator run --flow my-workflow --runner cursor --generate-report --report-format html

# Generate JSON report
agent-orchestrator run --flow my-workflow --runner cursor --generate-report --report-format json
```

### Report Location

Reports are saved to:
```
.agent-orchestrator/reports/
```

### View Reports

```bash
# List all reports
ls .agent-orchestrator/reports/

# View markdown report
cat .agent-orchestrator/reports/my-workflow-2024-01-15.md

# Open HTML report in browser
open .agent-orchestrator/reports/my-workflow-2024-01-15.html
```

### Report Contents

- Execution summary
- Performance metrics
- Step-by-step breakdown
- Success/failure rates
- Duration analysis
- Trend comparison (if historical data exists)

---

## ⚙️ Advanced Features

### Error Recovery

```bash
# Enable retry on failure (default: enabled)
agent-orchestrator run --flow my-workflow --runner cursor --enable-retry

# Configure retry attempts
agent-orchestrator run --flow my-workflow --runner cursor --max-retries 5

# Set retry delay
agent-orchestrator run --flow my-workflow --runner cursor --retry-delay 2000
```

### Caching

```bash
# Enable result caching (default: enabled)
agent-orchestrator run --flow my-workflow --runner cursor --enable-cache

# Set cache TTL
agent-orchestrator run --flow my-workflow --runner cursor --cache-ttl 3600

# Disable caching
agent-orchestrator run --flow my-workflow --runner cursor --no-cache
```

### Continue on Error

```bash
# Continue execution even if steps fail (default: true)
agent-orchestrator run --flow my-workflow --runner cursor --continue-on-error

# Abort on first error
agent-orchestrator run --flow my-workflow --runner cursor --abort-on-error
```

### Pre-flight Validation

Pre-flight checks run automatically before workflow execution:
- ✅ Workflow structure validation
- ✅ File existence checks
- ✅ Dependency verification
- ✅ `cursor-agent` availability

---

## 🎯 Common Use Cases

### Use Case 1: Compliance Check with Dashboard

```bash
# Terminal 1: Start dashboard
agent-orchestrator dashboard

# Terminal 2: Run compliance check
agent-orchestrator run --flow fwid-compliance --runner cursor --generate-report

# Browser: Monitor progress at http://localhost:3000
```

### Use Case 2: Long-Running Workflow with Checkpoints

```bash
# Run workflow (checkpoints auto-created)
agent-orchestrator run --flow long-workflow --runner cursor

# If interrupted, resume from checkpoint
agent-orchestrator checkpoints  # Find checkpoint ID
agent-orchestrator run --flow long-workflow --runner cursor --resume <checkpoint-id>
```

### Use Case 3: Interactive Debugging

```bash
# Run with interactive mode
agent-orchestrator run --flow my-workflow --runner cursor --interactive

# Pause, inspect steps, skip problematic steps, then continue
```

### Use Case 4: Parallel Execution Optimization

```bash
# Run with parallel optimization
agent-orchestrator run --flow my-workflow --runner cursor --optimize-parallel --max-concurrency 10
```

### Use Case 5: Generate Report After Execution

```bash
# Run workflow and generate HTML report
agent-orchestrator run --flow my-workflow --runner cursor --generate-report --report-format html

# Open report
open .agent-orchestrator/reports/my-workflow-*.html
```

---

## 🔧 Configuration

### Configuration Files

1. **`orchestrator.config.yml`** - Main orchestrator configuration
2. **`config/orchestration.yml`** - Workflow definitions

### Example Configuration

```yaml
# orchestrator.config.yml
cursor:
  timeout: 1800000  # 30 minutes
  maxRetries: 3

memory:
  enabled: true
  publicMemory: true
  privateMemory: false

dashboard:
  port: 3000
  enabled: true
```

### Environment Variables

```bash
# Override config with environment variables
export AGENT_ORCHESTRATOR_TIMEOUT=3600000
export AGENT_ORCHESTRATOR_PORT=8080
```

---

## 🐛 Troubleshooting

### Issue: "Workflow not found"

**Solution:**
```bash
# List available workflows
agent-orchestrator list

# Check config file
cat config/orchestration.yml
```

### Issue: "Command timed out"

**Solution:**
```bash
# Increase timeout in orchestrator.config.yml
cursor:
  timeout: 3600000  # 1 hour

# Or use environment variable
export AGENT_ORCHESTRATOR_TIMEOUT=3600000
```

### Issue: "Port already in use" (Dashboard)

**Solution:**
```bash
# Use different port
agent-orchestrator dashboard --port 8080

# Or stop existing dashboard
agent-orchestrator dashboard:stop
```

### Issue: "Checkpoint not found"

**Solution:**
```bash
# List all checkpoints
agent-orchestrator checkpoints

# Verify checkpoint ID
agent-orchestrator checkpoint --id <checkpoint-id>
```

### Issue: "cursor-agent not found"

**Solution:**
```bash
# Verify cursor-agent is installed
which cursor-agent

# Check PATH
echo $PATH

# Install cursor-agent if needed
# (Follow Cursor installation instructions)
```

### Issue: Build Errors

**Solution:**
```bash
# Clean and rebuild
rm -rf dist node_modules
npm install
npm run build
```

---

## 📚 Additional Resources

### Documentation Files

- `README.md` - Main documentation
- `DASHBOARD_COMPLETE.md` - Dashboard details
- `FINAL_COMPLETION_STATUS.md` - Feature status
- `FEATURE_IMPLEMENTATION_STATUS.md` - Implementation details

### Example Workflows

Check `config/orchestration.yml` for example workflow definitions.

### CLI Help

```bash
# Get help for any command
agent-orchestrator <command> --help
```

---

## 🎓 Tips & Best Practices

1. **Always start dashboard first** when monitoring workflows
2. **Use checkpoints** for long-running workflows
3. **Enable reporting** for important workflows
4. **Use interactive mode** for debugging
5. **Check pre-flight validation** output before running
6. **Monitor dashboard** for real-time progress
7. **Review reports** after execution for insights
8. **Use parallel optimization** for independent steps
9. **Enable caching** for repeated workflows
10. **Configure timeouts** appropriately for your workflows

---

## 🚀 Quick Reference

```bash
# Dashboard
agent-orchestrator dashboard                    # Start dashboard
agent-orchestrator dashboard:stop              # Stop dashboard

# Workflows
agent-orchestrator run --flow <name>           # Run workflow
agent-orchestrator run --flow <name> --resume <id>  # Resume

# Checkpoints
agent-orchestrator checkpoints                 # List checkpoints
agent-orchestrator checkpoint --id <id>        # Show checkpoint

# Reports
agent-orchestrator run --flow <name> --generate-report  # Generate report

# Interactive
agent-orchestrator run --flow <name> --interactive  # Interactive mode
```

---

**Happy Orchestrating! 🎉**

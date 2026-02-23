# Quick Start - Enhanced Orchestrator

## ✅ What's New

All requested enhancements have been implemented:

1. ✅ **Checkpoint/Resume** - Stop and resume workflows using `.skmemory` and `.claudememory`
2. ✅ **Stop Functionality** - Graceful shutdown with Ctrl+C
3. ✅ **Result Caching** - Avoid re-running expensive tasks
4. ✅ **Pre-flight Validation** - Catch errors before execution
5. ✅ **All Previous Features** - Rules integration, visibility, timeouts

## 🚀 Quick Commands

### Run a Workflow

```bash
# Basic run (all features enabled by default)
npm run dev run -- --flow fwid-compliance --runner cursor

# With verbose output
npm run dev run -- --flow fwid-compliance --runner cursor --verbose

# Custom cache TTL (days)
npm run dev run -- --flow fwid-compliance --runner cursor --cache-ttl 14
```

### Stop & Resume

```bash
# Run workflow
npm run dev run -- --flow fwid-compliance --runner cursor

# Press Ctrl+C to stop gracefully
# Checkpoint automatically saved

# List checkpoints
npm run dev checkpoints

# Resume from checkpoint
npm run dev run -- --flow fwid-compliance --runner cursor --resume <checkpoint-id>
```

### Checkpoint Management

```bash
# List all checkpoints
npm run dev checkpoints

# Show checkpoint details
npm run dev checkpoint -- --id <checkpoint-id>

# Delete checkpoint
npm run dev delete-checkpoint -- --id <checkpoint-id>
```

## 📋 Feature Flags

```bash
# Disable caching
--enable-cache false

# Disable checkpoints
--enable-checkpoints false

# Custom auto-save interval (seconds)
--auto-save-interval 30

# Custom cache TTL (days)
--cache-ttl 7
```

## 🎯 Complete Example

```bash
# Run with all features
npm run dev run \
  --flow fwid-compliance \
  --runner cursor \
  --verbose \
  --enable-cache \
  --cache-ttl 7 \
  --enable-checkpoints \
  --auto-save-interval 60

# Pre-flight validation runs automatically ✅
# Checkpoints saved automatically ✅
# Results cached automatically ✅
# Verbose output shows everything ✅
```

## 📚 Documentation

- **Full Guide**: `CHECKPOINT_RESUME_GUIDE.md`
- **All Features**: `ALL_ENHANCEMENTS_SUMMARY.md`
- **Enhancements**: `ENHANCEMENT_SUGGESTIONS.md`

## ✨ Key Benefits

- **Resilient**: Stop/resume workflows anytime
- **Fast**: Cached results avoid re-runs
- **Safe**: Pre-flight validation catches errors
- **Visible**: Real-time progress and verbose output
- **Persistent**: State saved to `.skmemory` and `.claudememory`

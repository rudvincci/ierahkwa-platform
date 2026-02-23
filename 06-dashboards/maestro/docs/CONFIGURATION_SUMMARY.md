# Configuration Summary

## Quick Reference

### Disable Memory Integration

**Option 1: Config File**
```yaml
# config/orchestrator.config.yml
memory:
  enabled: false
```

**Option 2: CLI Flag**
```bash
npm run dev manager analyze --no-memory
```

**Option 3: Disable Individual Systems**
```yaml
memory:
  enabled: true
  systems:
    skmemory:
      enabled: false  # Disable only SKMemory
```

### Behavior When Memory Unavailable

Configure how the orchestrator behaves when memory systems are unavailable:

```yaml
manager:
  memoryUnavailableBehavior: "warn"  # Options: "warn", "skip", "error"
```

- **`warn`** (default): Show warning, continue without memory
- **`skip`**: Silently skip memory operations
- **`error`**: Throw error if memory unavailable

### Auto-Initialize Submodules

```yaml
memory:
  autoInitializeSubmodules: true  # Auto-init if missing
```

## Working Without Memory

The orchestrator **works perfectly** without memory systems:

1. ✅ All core features work
2. ✅ Workflow execution works
3. ✅ Manager agent works (without memory context)
4. ✅ No errors, just warnings (if configured)

## Default Behavior

- Memory: **Enabled** (but gracefully handles unavailable)
- Behavior: **Warn** when unavailable
- Auto-init: **Disabled**

## See Also

- [Configuration Guide](./CONFIGURATION.md) - Complete configuration reference
- [Memory Integration](./MEMORY_INTEGRATION.md) - Memory system details

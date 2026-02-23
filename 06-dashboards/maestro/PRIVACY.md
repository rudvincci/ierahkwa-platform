# Privacy & Data Security

## User Data Protection

Maestro is designed with privacy and security in mind. **All user data is stored locally and is never committed to the repository.**

## What Data is Stored Locally

Maestro stores the following data locally in your repository:

### 1. **Checkpoints** (`.maestro/checkpoints/`)
- Workflow execution state
- Task results and context
- May contain code snippets, file paths, and execution details
- **Never committed to git** (excluded via `.gitignore`)

### 2. **Result Cache** (`.maestro/cache/`)
- Cached task results to avoid re-execution
- Contains task outputs and metadata
- **Never committed to git** (excluded via `.gitignore`)

### 3. **Execution Reports** (`.maestro/reports/`)
- Workflow execution reports
- Performance metrics and analytics
- May contain workflow names, step details, and execution times
- **Never committed to git** (excluded via `.gitignore`)

### 4. **Logs** (`.maestro/logs/`)
- Execution logs
- Debug information
- Error traces
- **Never committed to git** (excluded via `.gitignore`)

### 5. **Process IDs** (`.maestro/*.pid`)
- Dashboard and MCP server process IDs
- **Never committed to git** (excluded via `.gitignore`)

### 6. **Memory Storage** (if used)
- `.skmemory/` - SKMemory checkpoints (if SKMemory is installed)
- `.claudememory/` - Claude Memory checkpoints (if Claude Memory is installed)
- **Never committed to git** (excluded via `.gitignore`)

## Data Privacy Guarantees

✅ **All user data is stored locally** - No data is sent to external servers by default  
✅ **No telemetry** - Maestro does not collect usage statistics  
✅ **No analytics** - No tracking or monitoring of user behavior  
✅ **No data sharing** - Your data stays on your machine by default  
✅ **Git-safe** - All user data directories are excluded from git via `.gitignore`  
✅ **Opt-in sync** - Optional private repository sync is disabled by default (see [PRIVATE_SYNC.md](./PRIVATE_SYNC.md))

## What IS Committed to Git

Only the following are committed to the repository:

- Source code (`.ts`, `.js`, `.json`, `.yml`, `.md`)
- Configuration templates (not user-specific configs)
- Documentation
- Tests
- Build configuration

## Verifying Privacy

You can verify that user data is excluded by checking `.gitignore`:

```bash
# Check what's ignored
cat .gitignore

# Verify user data directories are ignored
git status --ignored | grep -E "(checkpoints|cache|reports|logs)"
```

## Best Practices

1. **Review `.gitignore`** - Ensure all user data directories are listed
2. **Check before committing** - Use `git status` to verify no user data is staged
3. **Use `.gitignore` patterns** - Don't override ignores with `git add -f`
4. **Clean up sensitive data** - If you accidentally commit user data, remove it immediately

## Reporting Privacy Issues

If you discover any privacy concerns or data leakage issues, please:

1. **Do not commit** the issue publicly
2. **Report privately** via GitHub security advisory or email
3. **Include details** about what data was exposed and how

## Compliance

Maestro follows privacy best practices:

- **GDPR Compliant** - No personal data collection
- **No PII Storage** - No personally identifiable information is stored
- **Local-First** - All data remains on user's machine
- **Transparent** - Clear documentation of what data is stored where

---

**Remember**: Your workflow data, checkpoints, and execution results are yours alone. They never leave your machine and are never committed to the repository.

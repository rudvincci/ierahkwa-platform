# Private Data Sync (OPT-IN ONLY)

## Overview

Maestro includes an **optional** feature to sync your workflow data to a private git repository. This allows AI assistants to access your data for better assistance, debugging, and workflow optimization.

⚠️ **This feature is OPT-IN ONLY** - It is **disabled by default** and requires explicit user consent and configuration.

## Privacy & Security

### Default Behavior
- ✅ **All data stays local** - No syncing by default
- ✅ **No telemetry** - No automatic data collection
- ✅ **User control** - You decide what to sync

### When Enabled
- 🔒 **Encryption** - Sensitive data can be encrypted before syncing
- 🎯 **Selective syncing** - Choose what data to sync (checkpoints, reports, etc.)
- 🔐 **Private repository** - Data goes to YOUR private repo, not ours
- ⏰ **Manual or scheduled** - Sync on-demand or automatically

## What Data Can Be Synced?

| Category | Default | Contains | Encryption Recommended |
|----------|---------|----------|----------------------|
| **Checkpoints** | ✅ Yes | Workflow state, task results, context | ✅ Yes |
| **Reports** | ✅ Yes | Execution reports, metrics | ⚠️ Optional |
| **Cache** | ❌ No | Cached task results | ✅ Yes |
| **Logs** | ❌ No | Execution logs | ⚠️ Optional |

## Setup

### Step 1: Create a Private Repository

Create a private git repository (GitHub, GitLab, etc.) where you want to sync your data.

```bash
# Example: Create on GitHub
gh repo create my-maestro-data --private
```

### Step 2: Configure Sync

Run the interactive configuration:

```bash
maestro sync:configure
```

This will ask you:
- Repository URL or local path
- Branch name (default: `main`)
- Encryption key (optional but recommended)
- Sync interval (minutes)
- What data to sync

### Step 3: Configure in orchestrator.config.yml

The interactive setup (`maestro sync:configure`) will automatically save your configuration to `.maestro/config/orchestrator.config.yml`.

**OR** manually edit `.maestro/config/orchestrator.config.yml`:

```yaml
privateSync:
  enabled: true
  repositoryUrl: "https://github.com/your-username/your-private-repo.git"
  # OR use local path:
  # repositoryPath: "/path/to/local/repo"
  branch: "main"
  encryptionKey: "your-encryption-key-here"  # Optional but recommended
  syncInterval: 60  # Minutes
  includeCheckpoints: true
  includeCache: false
  includeReports: true
  includeLogs: false
  excludePatterns: []  # Patterns to exclude
```

⚠️ **Important**: `orchestrator.config.yml` is **excluded from git** to protect your repository URL and encryption key. See [CONFIG_SETUP.md](./CONFIG_SETUP.md) for details.

### Step 4: Initialize

The sync will initialize automatically on first use, or you can initialize manually:

```bash
maestro sync:now
```

## Usage

### Manual Sync

Sync data immediately:

```bash
maestro sync:now
```

### Check Status

View sync status:

```bash
maestro sync:status
```

### Automatic Sync

If `syncInterval` is configured, sync happens automatically:
- Every N minutes (as configured)
- After workflow completion
- On checkpoint save

## Encryption

### Why Encrypt?

Encryption protects sensitive data like:
- Code snippets in checkpoints
- File paths from your system
- Execution context
- Task results

### Setup Encryption

1. **Generate a key** (or use the one generated during setup):
   ```bash
   openssl rand -hex 32
   ```

2. **Save it securely** - You'll need it to decrypt data

3. **Add to config**:
   ```yaml
   privateSync:
     encryptionKey: "your-key-here"
   ```

### Encrypted Files

Encrypted files have `.encrypted` extension:
- `checkpoint-123.json.encrypted`
- `cache-entry-456.json.encrypted`

## What Gets Synced?

### Included by Default
- ✅ Checkpoints (workflow state)
- ✅ Reports (execution reports)

### Excluded by Default
- ❌ Cache (usually not needed)
- ❌ Logs (can be large)

### Custom Exclusions

Add patterns to exclude specific files:

```yaml
privateSync:
  excludePatterns:
    - "*sensitive*"
    - "*secret*"
    - "checkpoint-*test*"
```

## Repository Structure

Synced data is organized in the repository:

```
your-private-repo/
├── checkpoints/
│   ├── checkpoint-123.json
│   └── checkpoint-456.json.encrypted
├── reports/
│   ├── execution-report-2024-01-15.md
│   └── metrics-2024-01-15.json
├── cache/  # If enabled
└── logs/   # If enabled
```

## Security Best Practices

1. ✅ **Use a private repository** - Never use a public repo
2. ✅ **Enable encryption** - Encrypt sensitive data
3. ✅ **Use strong encryption key** - Generate with `openssl rand -hex 32`
4. ✅ **Store key securely** - Use a password manager
5. ✅ **Review before syncing** - Check what's being synced
6. ✅ **Exclude sensitive patterns** - Use `excludePatterns`
7. ✅ **Regular audits** - Review synced data periodically

## Disabling Sync

To disable sync:

1. **Update config**:
   ```yaml
   privateSync:
     enabled: false
   ```

2. **Or remove the config section entirely**

## Troubleshooting

### Sync Fails

**Error: Repository not found**
- Check repository URL/path
- Ensure repository exists
- Verify git credentials

**Error: Permission denied**
- Check git credentials
- Verify repository access
- Ensure SSH keys are set up (if using SSH URL)

### Encryption Issues

**Can't decrypt files**
- Verify encryption key is correct
- Check key hasn't changed
- Ensure `.encrypted` extension is present

### Large Files

**Sync is slow**
- Exclude large files via `excludePatterns`
- Disable cache/logs syncing
- Increase `syncInterval`

## FAQ

**Q: Is my data safe?**  
A: Yes! Data goes to YOUR private repository. We don't have access unless you grant it.

**Q: Can I encrypt everything?**  
A: Yes, set `encryptionKey` and sensitive data will be encrypted automatically.

**Q: What if I change my mind?**  
A: Just set `enabled: false` in config. Your local data remains untouched.

**Q: Who can access the private repo?**  
A: Only people you grant access to (via GitHub/GitLab permissions).

**Q: Does this affect my local data?**  
A: No! Syncing is one-way (local → remote). Your local data stays local.

## Support

If you have questions or concerns about private data sync:
- Review this documentation
- Check `.maestro/config/orchestrator.config.yml`
- Run `maestro sync:status` to check configuration

---

**Remember**: This feature is **OPT-IN ONLY**. Your data stays local by default. 🔒

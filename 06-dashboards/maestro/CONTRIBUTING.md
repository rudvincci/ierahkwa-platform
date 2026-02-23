# Contributing to Maestro

Thank you for your interest in contributing to Maestro! 🎉

## Privacy & Security Guidelines

### ⚠️ CRITICAL: Never Commit User Data

**Maestro stores all user data locally and it must NEVER be committed to the repository.**

### What NOT to Commit

❌ **Never commit** the following:
- `.maestro/checkpoints/` - Workflow execution state
- `.maestro/cache/` - Cached task results
- `.maestro/reports/` - Execution reports
- `.maestro/logs/` - Log files
- `.maestro/*.pid` - Process ID files
- `.maestro/state/` - State files
- Any files containing:
  - User-specific workflow data
  - Execution results
  - Code snippets from user projects
  - File paths from user systems
  - API keys or secrets
  - Personal information

### What IS Safe to Commit

✅ **Safe to commit**:
- Source code (`.ts`, `.js`)
- Configuration templates (not user-specific)
- Documentation (`.md`)
- Tests
- Build configuration
- Default workflow templates in `config/`

### Before Committing

**Always check what you're committing:**

```bash
# Review staged files
git status

# Check if any user data is staged
git diff --cached --name-only | grep -E "(checkpoints|cache|reports|logs|\.pid)"

# If user data is staged, unstage it:
git reset HEAD <file>
```

### If You Accidentally Commit User Data

1. **Do NOT push** - Stop immediately
2. **Remove from history**:
   ```bash
   git reset --soft HEAD~1  # Undo last commit, keep changes
   git reset HEAD <user-data-file>  # Unstage user data
   git commit  # Re-commit without user data
   ```
3. **If already pushed**, use `git filter-branch` or BFG Repo-Cleaner to remove from history
4. **Rotate any exposed secrets** immediately

## Development Setup

1. **Clone the repository**
2. **Install dependencies**: `npm install`
3. **Build**: `npm run build`
4. **Run tests**: `npm test`

## Code Style

- Follow TypeScript best practices
- Use meaningful variable names
- Add JSDoc comments for public APIs
- Keep functions focused and small
- Write tests for new features

## Pull Request Process

1. **Create a feature branch** from `main`
2. **Make your changes**
3. **Ensure all tests pass**
4. **Verify no user data is included**
5. **Update documentation** if needed
6. **Submit PR** with clear description

## Testing

- Write unit tests for new features
- Test edge cases and error handling
- Ensure tests don't create user data files in the repo
- Clean up test artifacts

## Questions?

If you have questions about what's safe to commit, ask before submitting a PR. It's better to be safe than sorry!

---

**Remember**: User privacy and data security are paramount. When in doubt, don't commit it.

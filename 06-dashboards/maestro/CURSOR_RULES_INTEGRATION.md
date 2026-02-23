# Cursor Rules Integration

## Overview

The orchestrator now automatically loads and includes **all rules from `.cursor/rules/`** directory in every agent prompt. This ensures that all agents follow the workspace-specific rules and guidelines.

## Implementation

### RulesLoader Service

Created `src/services/RulesLoader.ts` that:
- Recursively scans `.cursor/rules/` directory
- Loads all `.md` files
- Formats them for inclusion in prompts
- Caches results for performance

### PromptBuilder Integration

Updated `src/agents/PromptBuilder.ts` to:
- Automatically include all cursor rules in prompts
- Format rules with clear section headers
- Group rules by directory structure
- Mark rules as **CRITICAL** and **MANDATORY**

### Features

1. **Automatic Rule Loading**
   - Scans `.cursor/rules/` on first use
   - Caches results for performance
   - Handles missing directories gracefully

2. **Organized Presentation**
   - Groups rules by directory (root first, then subdirectories)
   - Sorts rules alphabetically within each group
   - Clear section headers and separators

3. **Verbose Mode Support**
   - Shows which rules were loaded
   - Displays rule file paths
   - Helps with debugging

## Rule Files Included

All `.md` files from `.cursor/rules/` are included, including:

- `base.md` - Base rules and memory integration
- `microservice-creation.md` - Microservice creation patterns
- `microservice-*.md` - Various microservice-related rules
- `roles/*.md` - Role-specific rules
- And all other rule files

## Example Prompt Structure

When an agent executes, the prompt includes:

```
## Cursor Rules

**CRITICAL**: You MUST follow all rules defined in .cursor/rules/ directory.

The following rules apply to all work in this workspace:

### base.md

[Content of base.md]

---

### microservice-creation.md

[Content of microservice-creation.md]

---

[... all other rules ...]

**Remember**: These rules are MANDATORY and must be followed for all work.
```

## Usage

Rules are automatically included - no configuration needed!

### Verify Rules Are Loaded

Use verbose mode to see which rules were loaded:

```bash
npm run dev run -- --flow fwid-compliance --runner cursor --verbose
```

Output will show:
```
📋 Loaded 20 rule file(s) from .cursor/rules/
   - add.md
   - agent-orchestrator.md
   - base.md
   - microservice-creation.md
   ...
```

## Benefits

1. **Consistency**: All agents follow the same workspace rules
2. **Compliance**: Ensures adherence to Mamey Framework patterns
3. **No Manual Configuration**: Rules are automatically discovered
4. **Comprehensive**: All rules are included, not just selected ones
5. **Maintainable**: Add new rules by adding files to `.cursor/rules/`

## Technical Details

### Caching

- Rules are loaded once and cached
- Cache persists for the lifetime of the `RulesLoader` instance
- Use `clearCache()` to force reload (useful for testing)

### Error Handling

- Missing `.cursor/rules/` directory: Warning logged, continues without rules
- Failed file reads: Warning logged, skips that file
- Invalid content: File is still included (no validation)

### Performance

- First load: Scans directory and reads all files
- Subsequent loads: Uses cache (instant)
- Prompt building: Includes formatted rules (minimal overhead)

## Files Modified

1. **`src/services/RulesLoader.ts`** (NEW)
   - Service for loading and formatting rules

2. **`src/agents/PromptBuilder.ts`**
   - Added `RulesLoader` integration
   - Made `buildPrompt()` async
   - Added `getRulesSummary()` method

3. **`src/agents/CursorCliAgentRunner.ts`**
   - Updated to handle async `buildPrompt()`
   - Added rules summary to verbose output

4. **`src/cli/commands/run.ts`**
   - Passes repository root to `CursorCliAgentRunner`

## Testing

To verify rules are being included:

1. Run with verbose mode:
   ```bash
   npm run dev run -- --flow fwid-compliance --runner cursor --verbose
   ```

2. Check the prompt output - it should include a "Cursor Rules" section

3. Verify rules are listed in the verbose output

## Future Enhancements

Potential improvements:
- Rule filtering by agent role
- Rule priority/ordering
- Rule validation
- Rule change detection
- Selective rule inclusion based on task type

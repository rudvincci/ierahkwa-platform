# Cursor Agent Integration

## Overview

**Yes, Maestro uses `cursor-agent` CLI.**

The orchestrator integrates with Cursor's CLI tool (`cursor-agent`) to execute AI-powered tasks. Each workflow step is executed by calling `cursor-agent` with a role-specific prompt.

## How It Works

### Architecture

```
Workflow Definition (YAML)
    ↓
WorkflowExecutor
    ↓
CursorCliAgentRunner
    ↓
cursor-agent CLI command
    ↓
Cursor AI (via CLI)
```

### Execution Flow

1. **Workflow Step Defined**: Each step in a workflow YAML specifies an agent role (e.g., `architect`, `backend`, `frontend`)

2. **Task Created**: The `WorkflowExecutor` creates an `AgentTask` from the workflow step

3. **Cursor CLI Called**: The `CursorCliAgentRunner` builds and executes a `cursor-agent` command:
   ```bash
   cursor-agent -p --force --output-format json "<prompt>"
   ```

4. **Result Parsed**: The JSON output from `cursor-agent` is parsed and returned as an `AgentResult`

5. **Context Updated**: Results are stored in `OrchestratorContext` for use in subsequent steps

## CursorCliAgentRunner

The `CursorCliAgentRunner` class (`src/agents/CursorCliAgentRunner.ts`) implements the `IAgentRunner` interface and handles:

- Building `cursor-agent` commands with proper escaping
- Executing commands with timeout handling
- Parsing JSON output from `cursor-agent`
- Error handling and retry logic

### Command Format

```typescript
cursor-agent -p --force --output-format json "<escaped-prompt>"
```

**Options:**
- `-p`: Prompt mode
- `--force`: Force execution
- `--output-format json`: Request JSON output for parsing
- `--model <model>`: Optional model selection
- `--stream-partial-output`: Optional streaming

### Prompt Building

The `PromptBuilder` class (`src/agents/PromptBuilder.ts`) constructs prompts that include:

- Agent role context
- Task description
- Previous step results (from context)
- Domain knowledge
- Memory search results (if enabled)

## Usage in Workflows

### Example Workflow Step

```yaml
steps:
  - name: analyze_project
    agent: architect
    description: Analyze the project structure and identify gaps
    type: sequential
```

This step will:
1. Create an `AgentTask` with role `architect`
2. Build a prompt with architect context
3. Execute: `cursor-agent -p --force --output-format json "As an Architect, analyze the project..."`
4. Parse the JSON response
5. Store results for next steps

### Manager Agent

The `OrchestratorManagerAgent` also uses `cursor-agent` for:
- Project analysis
- Workflow suggestions
- Workflow creation
- Workflow optimization

## Configuration

### Timeout

Default timeout is 5 minutes (300,000ms), configurable:

```typescript
const runner = new CursorCliAgentRunner({
  timeout: 600000 // 10 minutes
});
```

### Model Selection

```typescript
const runner = new CursorCliAgentRunner({
  model: 'claude-3-opus'
});
```

## Requirements

### Prerequisites

1. **Cursor CLI Installed**: `cursor-agent` must be available in PATH
   ```bash
   cursor-agent --version
   ```

2. **Cursor Authentication**: Must be authenticated with Cursor
   ```bash
   cursor-agent auth
   ```

### Verification

Check if `cursor-agent` is available:

```bash
which cursor-agent
cursor-agent --version
```

## Error Handling

The `CursorCliAgentRunner` handles:

- **Command Timeout**: Throws error after timeout period
- **Parse Errors**: Falls back to text parsing if JSON parsing fails
- **Command Errors**: Captures stderr and includes in error result
- **Network Issues**: Retries with exponential backoff (if configured)

## Output Format

### Expected JSON Output

```json
{
  "success": true,
  "summary": "Task completed successfully",
  "details": "..."
}
```

### Fallback Parsing

If JSON parsing fails, the runner:
1. Checks for error keywords in output
2. Returns first 200 characters as summary
3. Marks as failure if errors detected

## Integration Points

### WorkflowExecutor

The `WorkflowExecutor` uses `CursorCliAgentRunner` to execute workflow steps:

```typescript
const runner = new CursorCliAgentRunner();
const executor = new WorkflowExecutor();
await executor.execute(workflow, runner, context);
```

### Manager Agent

The `OrchestratorManagerAgent` uses `CursorCliAgentRunner` for AI-powered management:

```typescript
const manager = new OrchestratorManagerAgent();
const analysis = await manager.analyzeProject(projectPath);
```

## Troubleshooting

### Command Not Found

If `cursor-agent` is not found:

```bash
# Check installation
which cursor-agent

# Install Cursor CLI (if not installed)
# Follow Cursor documentation for CLI installation
```

### Authentication Issues

```bash
# Re-authenticate
cursor-agent auth

# Check authentication status
cursor-agent auth status
```

### Timeout Issues

Increase timeout for long-running tasks:

```typescript
const runner = new CursorCliAgentRunner({
  timeout: 900000 // 15 minutes
});
```

### JSON Parse Errors

If JSON parsing fails:
- Check `cursor-agent` version (should support `--output-format json`)
- Review raw output in `AgentResult.rawOutput`
- Check prompt format (may need escaping)

## Future Enhancements

Potential improvements:
- Streaming support for long-running tasks
- Better error recovery
- Caching of common prompts
- Batch execution for parallel steps
- Progress tracking for long operations

## Related Files

- `src/agents/CursorCliAgentRunner.ts` - Main implementation
- `src/agents/PromptBuilder.ts` - Prompt construction
- `src/workflow/WorkflowExecutor.ts` - Workflow execution
- `src/agents/OrchestratorManagerAgent.ts` - Manager agent
- `.cursorrules` - Cursor rules for orchestrator

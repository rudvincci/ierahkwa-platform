# Automatic Model Selection

Maestro intelligently selects the best AI model for each task based on task characteristics, optimizing for cost, speed, and capability.

## How It Works

When a workflow runs without an explicitly specified model, Maestro analyzes each task and automatically selects the most appropriate model from Cursor's available models.

### Task Analysis

Maestro analyzes tasks based on:

1. **Task Type**:
   - `code-generation` - Creating new code
   - `code-analysis` - Reviewing/analyzing code
   - `documentation` - Writing docs/comments
   - `refactoring` - Restructuring code
   - `testing` - Writing tests
   - `debugging` - Fixing errors
   - `architecture` - Design decisions
   - `general` - Other tasks

2. **Complexity**:
   - `simple` - Quick checks, validations
   - `moderate` - Standard implementations
   - `complex` - Multi-step implementations
   - `very-complex` - Architecture, major refactoring

3. **Requirements**:
   - Code generation needed?
   - Reasoning/explanation required?
   - Long context window needed?
   - Speed priority?
   - Cost priority?

4. **Token Estimation**:
   - Estimates input/output tokens based on task description
   - Accounts for full prompt (rules, context, etc.)

## Model Selection Criteria

### For Code Tasks
- **Code-optimized models** (GPT-5-Codex, Grok Code) get higher scores
- **Models with code capabilities** are preferred

### For Complex Tasks
- **Reasoning models** (o3, Deepseek R1) for very complex tasks
- **Most capable models** (Claude 4 Opus, GPT-5-Pro) for architecture

### For Simple Tasks
- **Fast models** (Claude 4.5 Haiku, GPT-5 Fast, Gemini Flash) are preferred
- **Cost-effective models** (GPT-5 Mini, GPT-5 Nano) for simple tasks

### For Long Context
- **1M+ context models** for tasks requiring entire codebase analysis
- **200k+ context models** for standard long-context needs

### Cost Optimization
- **Cost-effective models** prioritized when cost priority is high
- **Capability prioritized** when cost priority is low

## Examples

### Simple Code Check
```
Task: "Verify all imports are correct"
→ Selected: Claude 4.5 Haiku (fast, cost-effective)
```

### Complex Architecture
```
Task: "Design microservice architecture for payment system"
→ Selected: Claude 4 Opus or GPT-5-Pro (most capable)
```

### Code Generation
```
Task: "Generate REST API endpoints for user management"
→ Selected: GPT-5-Codex or Grok Code (code-optimized)
```

### Long Context Analysis
```
Task: "Analyze entire codebase for security vulnerabilities"
→ Selected: Claude 4.5 Sonnet 1M or Gemini 3 Pro 1M (ultra-long context)
```

### Reasoning Task
```
Task: "Explain why this design pattern was chosen"
→ Selected: o3 or Deepseek R1 (reasoning models)
```

## Override Auto-Selection

You can override auto-selection by:

1. **CLI**: `maestro run --flow my-workflow --model claude-4-5-sonnet`
2. **Dashboard**: Select model from dropdown before starting workflow
3. **Workflow YAML**: Specify model in workflow definition (future feature)

## Cost Tracking

Maestro tracks and displays:
- **Cost per step** - Estimated cost for each task
- **Total workflow cost** - Cumulative cost across all steps
- **Cost breakdown** - By model, by step, by token type

Costs are displayed in the dashboard and logged during execution.

## See Also

- [Models Documentation](MODELS.md) - Complete list of available models
- [Dashboard Guide](DASHBOARD.md) - Viewing model selection and costs
- [CLI Reference](CLI_REFERENCE.md) - Command-line options

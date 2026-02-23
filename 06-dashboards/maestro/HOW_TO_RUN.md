# How to Run and Manage Agents

## Current Status

⚠️ **Note**: There are TypeScript compilation errors that need to be fixed before the orchestrator can run. See "Fixing Compilation Errors" below.

## Once Fixed - Usage Guide

### Prerequisites

1. **Install dependencies**:
   ```bash
   cd .agent-orchestrator
   npm install
   ```

2. **Verify cursor-agent**:
   ```bash
   which cursor-agent
   cursor-agent --version
   ```

### Basic Commands

#### 1. List Available Workflows

```bash
npm run dev flows
```

Shows all workflows defined in `config/orchestration.yml`.

#### 2. Run a Workflow

**Dry Run (Preview Only)**:
```bash
npm run dev run -- --flow <workflow-name> --dry-run
```

**Real Execution (with cursor-agent)**:
```bash
npm run dev run -- --flow <workflow-name> --runner cursor
```

**With Options**:
```bash
npm run dev run -- --flow <workflow-name> \
  --runner cursor \
  --max-concurrency 5 \
  --continue-on-error \
  --feature "Implement user authentication"
```

#### 3. Manager Agent Commands

**Analyze Project**:
```bash
npm run dev manager analyze -- --project <path>
```

**Suggest Workflows**:
```bash
npm run dev manager suggest -- --project <path>
```

**Create Workflow**:
```bash
npm run dev manager create -- --name <name> --suggestion 0
```

**Optimize Workflow**:
```bash
npm run dev manager optimize -- --workflow <name>
```

### Example: FutureWampumId Compliance Workflow

```bash
# 1. Navigate to orchestrator
cd .agent-orchestrator

# 2. List workflows
npm run dev flows

# 3. Preview execution plan
npm run dev run -- --flow fwid-compliance --dry-run

# 4. Execute with cursor-agent
npm run dev run -- --flow fwid-compliance --runner cursor

# 5. Or use the quick compliance script
cd ..
bash .agent-orchestrator/scripts/fwid-compliance-check.sh
```

## Agent Runners

### Dummy Runner (Testing)
```bash
npm run dev run -- --flow <name> --runner dummy
```
- Simulates execution
- No AI calls
- Good for testing workflow structure

### Cursor Runner (Real AI)
```bash
npm run dev run -- --flow <name> --runner cursor
```
- Uses `cursor-agent` CLI
- Real AI execution
- Requires cursor-agent installed and authenticated

## Command Options

### Run Command Options

- `-f, --flow <name>` - **Required**: Workflow name
- `-c, --config <path>` - Custom config file
- `--dry-run` - Preview without executing
- `-r, --runner <type>` - `dummy` or `cursor` (default: `dummy`)
- `--feature <desc>` - Feature description for prompts
- `--log-level <level>` - `info|debug|warn|error`
- `-o, --output <dir>` - Log output directory
- `--max-concurrency <n>` - Max parallel tasks
- `--continue-on-error` - Don't stop on errors
- `--abort-on-error` - Stop on first error

### Manager Command Options

**Analyze**:
- `-p, --project <path>` - Project path
- `-o, --output <file>` - Save analysis JSON
- `--no-memory` - Disable memory
- `--memory` - Enable memory

**Suggest**:
- `-p, --project <path>` - Project path
- `--no-memory` - Disable memory
- `--memory` - Enable memory

**Create**:
- `-n, --name <name>` - **Required**: Workflow name
- `-t, --template <name>` - Use template
- `-s, --suggestion <index>` - Use suggestion index
- `-o, --output <file>` - Output file
- `--no-memory` - Disable memory
- `--memory` - Enable memory

**Optimize**:
- `-w, --workflow <name>` - **Required**: Workflow name
- `-o, --output <file>` - Save report JSON
- `--no-memory` - Disable memory
- `--memory` - Enable memory

## Workflow Configuration

Workflows are defined in `config/orchestration.yml`:

```yaml
flows:
  workflow-name:
    name: Workflow Name
    description: Description
    steps:
      - name: step1
        agent: architect
        description: Step description
        type: sequential
```

## Agent Roles

Available roles in workflows:
- `architect` - System design, DDD patterns
- `backend` - .NET implementation, CQRS
- `frontend` - Blazor/Razor components
- `rust` - Rust code for MameyNode
- `tests` - Unit, integration, E2E tests
- `docs` - Documentation

## Fixing Compilation Errors

Before running, fix these TypeScript errors:

1. **Export StepDefinition**:
   - File: `src/domain/WorkflowDefinition.ts`
   - Export `StepDefinition` interface

2. **Fix Null Checks**:
   - File: `src/agents/OrchestratorManagerAgent.ts`
   - Lines: 229, 230, 368, 369
   - Add null checks for `this.memorySearch`

3. **Install Missing Types**:
   ```bash
   npm install --save-dev @types/glob
   ```

After fixing:
```bash
npm run build
npm run dev flows
```

## Quick Reference

```bash
# List workflows
npm run dev flows

# Dry run
npm run dev run -- --flow <name> --dry-run

# Execute
npm run dev run -- --flow <name> --runner cursor

# Analyze
npm run dev manager analyze

# Suggest
npm run dev manager suggest

# Create
npm run dev manager create -- --name <name> --suggestion 0

# Optimize
npm run dev manager optimize -- --workflow <name>
```

## Documentation

- **Full Usage Guide**: `docs/USAGE_GUIDE.md`
- **Quick Start**: `QUICK_START.md`
- **Cursor Integration**: `docs/CURSOR_AGENT_INTEGRATION.md`
- **Configuration**: `docs/CONFIGURATION.md`

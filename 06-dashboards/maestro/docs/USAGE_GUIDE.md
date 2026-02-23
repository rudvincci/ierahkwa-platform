# Maestro - Usage Guide

## Quick Start

### Prerequisites

1. **Node.js** installed (v18+)
2. **cursor-agent** CLI installed and authenticated
3. **Dependencies** installed:
   ```bash
   cd .agent-orchestrator
   npm install
   ```

### Build (Optional)

```bash
npm run build
```

## Running the Orchestrator

### Basic Commands

The orchestrator provides several commands:

```bash
# Development mode (using ts-node)
npm run dev <command>

# Production mode (after build)
npm start <command>
# or
node dist/cli/index.js <command>
```

### 1. List Available Workflows

```bash
npm run dev flows
# or with custom config
npm run dev flows -- --config config/orchestration.yml
```

**Output**: Lists all workflows defined in the configuration file.

### 2. Run a Workflow

```bash
npm run dev run -- --flow <workflow-name>
```

**Options:**
- `-f, --flow <name>` - **Required**: Workflow name to execute
- `-c, --config <path>` - Path to orchestration config (default: `config/orchestration.yml`)
- `--dry-run` - Show execution plan without executing
- `-r, --runner <type>` - Agent runner type: `dummy` (default) or `cursor`
- `--feature <description>` - Feature description to inject into prompts
- `--log-level <level>` - Logging level: `info|debug|warn|error` (default: `info`)
- `-o, --output <dir>` - Output directory for logs
- `--max-concurrency <number>` - Maximum parallel task executions
- `--continue-on-error` - Continue execution even if a task fails
- `--abort-on-error` - Abort entire workflow on first error

**Examples:**

```bash
# Dry run (preview execution plan)
npm run dev run -- --flow fwid-compliance --dry-run

# Run with cursor-agent (real execution)
npm run dev run -- --flow fwid-compliance --runner cursor

# Run with feature description
npm run dev run -- --flow microservice-implementation \
  --runner cursor \
  --feature "Implement user authentication service"

# Run with custom concurrency
npm run dev run -- --flow fwid-compliance \
  --runner cursor \
  --max-concurrency 5

# Run with error handling
npm run dev run -- --flow fwid-compliance \
  --runner cursor \
  --continue-on-error
```

### 3. Manager Agent Commands

The Manager Agent provides AI-powered workflow management:

#### Analyze Project

```bash
npm run dev manager analyze -- --project <path>
```

**Options:**
- `-p, --project <path>` - Project path to analyze (default: current directory)
- `-o, --output <file>` - Output file for analysis results (JSON)
- `--no-memory` - Disable memory integration for this command
- `--memory` - Enable memory integration (overrides config)

**Example:**

```bash
# Analyze current project
npm run dev manager analyze

# Analyze specific project
npm run dev manager analyze -- --project ../FutureWampum/FutureWampumId

# Save analysis to file
npm run dev manager analyze -- --output analysis.json
```

#### Suggest Workflows

```bash
npm run dev manager suggest -- --project <path>
```

**Options:**
- `-p, --project <path>` - Project path (default: current directory)
- `--no-memory` - Disable memory integration
- `--memory` - Enable memory integration

**Example:**

```bash
# Get workflow suggestions for current project
npm run dev manager suggest

# Get suggestions for specific project
npm run dev manager suggest -- --project ../FutureWampum/FutureWampumId
```

#### Create Workflow

```bash
npm run dev manager create -- --name <name> [--template <template> | --suggestion <index>]
```

**Options:**
- `-n, --name <name>` - **Required**: Workflow name
- `-t, --template <template>` - Template name to use
- `-s, --suggestion <index>` - Use suggestion by index (from `suggest` command)
- `-o, --output <file>` - Output file (default: `config/orchestration.yml`)
- `--no-memory` - Disable memory integration
- `--memory` - Enable memory integration

**Example:**

```bash
# Create workflow from suggestion #0
npm run dev manager suggest
# (note the suggestion index)
npm run dev manager create -- --name my-workflow --suggestion 0

# Create workflow from template
npm run dev manager create -- --name my-workflow --template microservice

# Save to custom location
npm run dev manager create -- --name my-workflow --suggestion 0 \
  --output config/my-workflow.yml
```

#### Optimize Workflow

```bash
npm run dev manager optimize -- --workflow <name>
```

**Options:**
- `-w, --workflow <name>` - **Required**: Workflow name to optimize
- `-o, --output <file>` - Output file for optimization report (JSON)
- `--no-memory` - Disable memory integration
- `--memory` - Enable memory integration

**Example:**

```bash
# Optimize existing workflow
npm run dev manager optimize -- --workflow fwid-compliance

# Save optimization report
npm run dev manager optimize -- --workflow fwid-compliance \
  --output optimization-report.json
```

#### Monitor Workflow

```bash
npm run dev manager monitor -- --workflow <name> [--id <id>]
```

**Options:**
- `-w, --workflow <name>` - Workflow name to monitor
- `-i, --id <id>` - Execution ID to monitor

**Note**: Monitoring feature is coming soon.

## Agent Runners

### Dummy Runner (Default)

The `dummy` runner simulates execution without calling `cursor-agent`. Useful for:
- Testing workflow structure
- Dry-run validation
- Development/testing

```bash
npm run dev run -- --flow <name> --runner dummy
```

### Cursor Runner

The `cursor` runner uses `cursor-agent` CLI for real AI execution. Requires:
- `cursor-agent` installed and in PATH
- Cursor authentication configured

```bash
npm run dev run -- --flow <name> --runner cursor
```

**Verify cursor-agent:**

```bash
which cursor-agent
cursor-agent --version
```

## Workflow Configuration

### Configuration File Location

Default: `config/orchestration.yml`

Custom: Use `--config` option

### Example Workflow

```yaml
flows:
  my-workflow:
    name: My Workflow
    description: Example workflow
    steps:
      - name: step1
        agent: architect
        description: Analyze requirements
        type: sequential
        
      - name: step2
        agent: backend
        description: Implement feature
        dependsOn: [step1]
        type: sequential
```

## Agent Roles

Available agent roles (defined in workflow steps):

1. **architect** - System design, DDD patterns, microservice design
2. **backend** - .NET microservice implementation, CQRS, domain logic
3. **frontend** - Blazor/Razor components, UI/UX implementation
4. **rust** - Rust code for MameyNode
5. **tests** - Unit, integration, E2E tests
6. **docs** - Quick documentation, README updates

## Execution Modes

### Dry Run

Preview execution plan without executing:

```bash
npm run dev run -- --flow <name> --dry-run
```

**Output:**
- Execution plan
- Step order
- Dependencies
- Parallel groups

### Real Execution

Execute workflow with AI agents:

```bash
npm run dev run -- --flow <name> --runner cursor
```

**Output:**
- Real-time task status
- Execution summary
- Success/failure counts
- Error details (if any)

## Error Handling

### Continue on Error

Continue execution even if a task fails:

```bash
npm run dev run -- --flow <name> --continue-on-error
```

### Abort on Error

Abort entire workflow on first error:

```bash
npm run dev run -- --flow <name> --abort-on-error
```

## Concurrency Control

Control parallel execution:

```bash
# Limit to 3 parallel tasks
npm run dev run -- --flow <name> --max-concurrency 3

# Default: Based on workflow configuration
```

## Logging

### Log Levels

```bash
# Info (default)
npm run dev run -- --flow <name> --log-level info

# Debug
npm run dev run -- --flow <name> --log-level debug

# Warn
npm run dev run -- --flow <name> --log-level warn

# Error
npm run dev run -- --flow <name> --log-level error
```

### Log Output

```bash
# Save logs to directory
npm run dev run -- --flow <name> --output logs/
```

## Complete Examples

### Example 1: Analyze and Create Workflow

```bash
# 1. Analyze project
npm run dev manager analyze -- --project ../FutureWampum/FutureWampumId

# 2. Get suggestions
npm run dev manager suggest -- --project ../FutureWampum/FutureWampumId

# 3. Create workflow from suggestion
npm run dev manager create -- --name fwid-compliance --suggestion 0

# 4. Run workflow (dry-run first)
npm run dev run -- --flow fwid-compliance --dry-run

# 5. Execute workflow
npm run dev run -- --flow fwid-compliance --runner cursor
```

### Example 2: Run Existing Workflow

```bash
# List available workflows
npm run dev flows

# Run with cursor-agent
npm run dev run -- --flow fwid-compliance \
  --runner cursor \
  --max-concurrency 5 \
  --continue-on-error
```

### Example 3: Optimize Workflow

```bash
# Optimize existing workflow
npm run dev manager optimize -- --workflow fwid-compliance

# Save report
npm run dev manager optimize -- --workflow fwid-compliance \
  --output optimization-report.json
```

## Troubleshooting

### cursor-agent Not Found

```bash
# Check installation
which cursor-agent

# Check version
cursor-agent --version

# If not found, install Cursor CLI
# Follow Cursor documentation for CLI installation
```

### Authentication Issues

```bash
# Re-authenticate
cursor-agent auth

# Check status
cursor-agent auth status
```

### TypeScript Errors

```bash
# Install missing types
npm install --save-dev @types/glob

# Rebuild
npm run build
```

### Workflow Not Found

```bash
# List available workflows
npm run dev flows

# Check config file
cat config/orchestration.yml
```

### Memory Integration Issues

```bash
# Disable memory for specific command
npm run dev manager analyze -- --no-memory

# Check memory configuration
cat config/orchestrator.config.yml
```

## Next Steps

1. **Explore Workflows**: Run `npm run dev flows` to see available workflows
2. **Try Dry Run**: Test workflows with `--dry-run` first
3. **Use Manager Agent**: Let AI suggest and create workflows
4. **Monitor Execution**: Watch workflow execution in real-time
5. **Optimize**: Use manager agent to optimize workflows

## Related Documentation

- [Cursor Agent Integration](./CURSOR_AGENT_INTEGRATION.md) - How cursor-agent is used
- [Configuration Guide](./CONFIGURATION.md) - Configuration options
- [FWID Compliance Guide](./FWID_COMPLIANCE_GUIDE.md) - FutureWampumId compliance workflow
- [README](../README.md) - General overview

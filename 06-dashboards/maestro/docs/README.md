# Agent Orchestrator

Agent Orchestrator is a production-grade CLI tool for coordinating multiple AI agents through workflow definitions. It integrates with Cursor CLI to execute tasks across different agent roles in a structured, dependency-aware manner.

## Features

- **Workflow Definitions**: Define complex workflows with step dependencies using YAML
- **Agent Roles**: Pre-configured roles tailored to the Mamey Framework workspace
- **Dependency Resolution**: Automatic topological sorting of workflow steps
- **Cursor CLI Integration**: Seamless integration with Cursor CLI for agent execution
- **Comprehensive Logging**: Detailed execution logs for debugging and auditing
- **Dry-Run Mode**: Preview execution plans without running tasks

## Installation

### Quick Install (Recommended)

From the orchestrator directory:
```bash
cd .agent-orchestrator
bash scripts/install.sh
```

This will:
- ✅ Install npm dependencies
- ✅ Build TypeScript code
- ✅ Set up Cursor rules
- ✅ Create convenience scripts

### Install in New Directory

```bash
cd .agent-orchestrator
bash scripts/install.sh /path/to/project
```

### Manual Installation

```bash
cd .agent-orchestrator
npm install
npm run build
```

### Initialize Cursor Rules

After installation, initialize Cursor rules for AI integration:

```bash
cd .agent-orchestrator
bash scripts/cursor-init.sh
```

See [INSTALL.md](../INSTALL.md) for complete installation guide.

## Quick Start

### List Available Workflows

```bash
npm run dev flows list
# or after build:
node dist/cli/index.js flows list
```

### Run a Workflow (Dry-Run)

```bash
npm run dev run --flow feature_implementation --dry-run
```

### Run a Workflow (Dummy Runner)

```bash
npm run dev run --flow feature_implementation --runner dummy
```

### Run a Workflow (Cursor CLI)

```bash
npm run dev run --flow feature_implementation --runner cursor --feature "Implement Patient search"
```

## Configuration

### Config File Discovery

The orchestrator looks for configuration files in this order:

1. Path specified via `--config` flag
2. `orchestration.yml` in repository root
3. `.agent-orchestrator/config/orchestration.yml` (default)

### Workflow Definition

Workflows are defined in YAML format:

```yaml
flows:
  feature_implementation:
    name: feature_implementation
    description: Complete feature implementation workflow
    steps:
      - name: ArchitecturePlan
        agent: Architect
        description: Analyze requirements and propose architecture
        dependsOn: []

      - name: BackendImplementation
        agent: Backend
        description: Implement backend logic & contracts
        dependsOn:
          - ArchitecturePlan
```

### Agent Roles

The default configuration includes 13 agent roles:

1. **Architect**: System design, architecture decisions, DDD patterns
2. **Backend**: .NET microservice implementation, CQRS, domain logic
3. **Frontend**: Blazor/Razor components, UI/UX implementation
4. **Rust**: Rust code implementation for MameyNode
5. **Tests**: Unit, integration, and E2E tests
6. **Docs**: Quick documentation, README updates
7. **TechnicalWriter**: Comprehensive technical documentation
8. **Infrastructure**: Docker, Kubernetes, deployment configs
9. **Contracts**: API contracts, DTOs, shared interfaces
10. **Events**: Event definitions, handlers, saga orchestration
11. **Database**: Database migrations, schema changes
12. **Security**: Authentication, authorization, compliance
13. **Integration**: Service integration, message brokers, external APIs

Each role includes:
- Description and responsibilities
- Mamey Framework-specific prompt hints
- Default file patterns and directories
- Domain knowledge (Government, Banking, FutureWampum, etc.)

## CLI Commands

### `flows list`

List all available workflows and agent roles.

```bash
agent-orchestrator flows list
agent-orchestrator flows list --config ./custom-config.yml
```

### `run`

Execute a workflow.

**Options:**
- `-f, --flow <name>`: Workflow name to execute (required)
- `-c, --config <path>`: Path to orchestration config file
- `--dry-run`: Show execution plan without executing
- `-r, --runner <type>`: Agent runner type (`dummy` or `cursor`, default: `dummy`)
- `--feature <description>`: Feature description to inject into prompts
- `--log-level <level>`: Logging level (`info`, `debug`, `warn`, `error`)
- `-o, --output <dir>`: Output directory for logs

**Examples:**

```bash
# Dry-run to see execution plan
agent-orchestrator run --flow feature_implementation --dry-run

# Execute with dummy runner
agent-orchestrator run --flow feature_implementation --runner dummy

# Execute with Cursor CLI
agent-orchestrator run --flow feature_implementation --runner cursor --feature "Add user authentication"

# Custom config and output directory
agent-orchestrator run --flow feature_implementation --config ./my-config.yml --output ./logs
```

## Cursor CLI Integration

The orchestrator uses Cursor CLI in headless mode:

```bash
cursor-agent -p --force --output-format json "<prompt>"
```

**Requirements:**
- Cursor CLI must be installed and available in PATH
- API key must be configured (see [Cursor CLI docs](https://cursor.com/docs/cli/overview))

**Features:**
- Automatic prompt building with role-specific context
- JSON output parsing
- Error handling and timeouts
- Progress tracking support

## Logging

Execution logs are stored in `.agent-orchestrator/logs/{runId}.json` by default.

Each log includes:
- Run ID and metadata
- Workflow name and timestamp
- Per-step execution details
- Prompts sent to agents
- Results and summaries
- Error information (if any)

## Architecture

### Core Components

- **Domain Models**: AgentRole, AgentTask, WorkflowDefinition, StepDefinition, RunLog
- **Config Loading**: YAML/JSON parsing with validation
- **Dependency Resolution**: Topological sort (Kahn's algorithm)
- **Workflow Execution**: Task orchestration with status tracking
- **Agent Runners**: Dummy (testing) and Cursor CLI implementations
- **Logging**: Structured JSON logging

### Workflow Execution Flow

1. Load and validate configuration
2. Resolve workflow step dependencies
3. Create tasks for each step
4. Execute tasks in dependency order
5. Track status and results
6. Log execution details

## Extending

### Adding New Agent Roles

Edit `.agent-orchestrator/config/orchestration.yml`:

```yaml
roles:
  MyNewRole:
    name: MyNewRole
    description: Description of the role
    promptHints: |
      Specific instructions for this role
    defaultFilePatterns:
      - "**/*.ext"
    defaultDirectories:
      - "src"
```

### Creating Custom Workflows

Add to `flows` section in config:

```yaml
flows:
  my_workflow:
    name: my_workflow
    description: My custom workflow
    steps:
      - name: Step1
        agent: MyNewRole
        description: First step
        dependsOn: []
```

### Custom Agent Runners

Implement `IAgentRunner` interface:

```typescript
import { IAgentRunner } from './agents/AgentRunner';

export class MyCustomRunner implements IAgentRunner {
  async runTask(task: AgentTask, context: OrchestratorContext): Promise<AgentResult> {
    // Your implementation
  }
}
```

## Troubleshooting

### Config Not Found

Ensure `orchestration.yml` exists in one of:
- Repository root
- `.agent-orchestrator/config/`
- Path specified via `--config`

### Cursor CLI Not Found

Verify Cursor CLI is installed:
```bash
cursor-agent --version
```

If not installed:
```bash
curl https://cursor.com/install -fsS | bash
```

### Circular Dependencies

The validator will detect and report circular dependencies in workflow steps. Review your workflow definition.

## License

AGPL-3.0

## References

- [Cursor CLI Documentation](https://cursor.com/docs/cli/overview)
- [Cursor CLI Headless Mode](https://cursor.com/docs/cli/headless)
- Mamey Framework Documentation


# Subagent-like Capabilities

The enhanced orchestrator now supports subagent-like capabilities including nested workflows, parallel execution, and dynamic task spawning.

## Features

### 1. Parallel Execution

Steps can run in parallel when they have no dependencies on each other. Mark steps with `parallel: true` to explicitly enable parallel execution.

**Example:**
```yaml
steps:
  - name: BackendImplementation
    agent: Backend
    description: Implement backend
    dependsOn: [ArchitecturePlan]
    parallel: true  # Can run in parallel

  - name: Contracts
    agent: Contracts
    description: Define contracts
    dependsOn: [ArchitecturePlan]
    parallel: true  # Can run in parallel with BackendImplementation
```

**Benefits:**
- Faster execution for independent tasks
- Better resource utilization
- Configurable concurrency limits

### 2. Nested Workflows

Steps can execute entire workflows as sub-workflows, enabling hierarchical task organization.

**Example:**
```yaml
flows:
  main_workflow:
    steps:
      - name: BackendImplementation
        agent: Backend
        nestedWorkflow: backend_implementation  # Execute nested workflow

  backend_implementation:
    steps:
      - name: DomainLayer
        agent: Backend
        description: Implement domain layer
      - name: InfrastructureLayer
        agent: Backend
        description: Implement infrastructure layer
        dependsOn: [DomainLayer]
```

**Benefits:**
- Reusable workflow components
- Better organization of complex workflows
- Hierarchical task management
- Context inheritance from parent workflow

### 3. Dynamic Task Spawning

Tasks can dynamically create sub-tasks based on conditions and results.

**Example:**
```yaml
steps:
  - name: BackendImplementation
    agent: Backend
    description: Implement backend
    spawnTasks:
      - condition: "previousResult.success === true"
        agent: Tests
        description: Create unit tests
      - condition: "previousResult.success === true"
        agent: Contracts
        description: Verify contracts
```

**Benefits:**
- Adaptive workflows based on results
- Conditional task execution
- Dynamic workflow composition

## Usage

### Basic Parallel Execution

```bash
# Run workflow with parallel execution
npm run dev run --flow parallel_feature_implementation

# Limit concurrency
npm run dev run --flow parallel_feature_implementation --max-concurrency 3
```

### Nested Workflows

```bash
# Run workflow with nested workflows
npm run dev run --flow feature_implementation_enhanced
```

### Dynamic Task Spawning

```bash
# Run workflow with spawned tasks
npm run dev run --flow feature_with_spawned_tasks
```

### Execution Options

```bash
# Continue on error (default: true)
npm run dev run --flow my_workflow --continue-on-error

# Abort on first error
npm run dev run --flow my_workflow --abort-on-error

# Set max concurrency
npm run dev run --flow my_workflow --max-concurrency 5
```

## Configuration Examples

### Parallel Group Example

```yaml
steps:
  # Group 1: Sequential
  - name: ArchitecturePlan
    agent: Architect
    dependsOn: []

  # Group 2: Parallel (3 tasks)
  - name: BackendImplementation
    agent: Backend
    dependsOn: [ArchitecturePlan]
    parallel: true

  - name: Contracts
    agent: Contracts
    dependsOn: [ArchitecturePlan]
    parallel: true

  - name: DatabaseSchema
    agent: Backend
    dependsOn: [ArchitecturePlan]
    parallel: true

  # Group 3: Sequential (waits for Group 2)
  - name: FrontendImplementation
    agent: Frontend
    dependsOn: [BackendImplementation, Contracts, DatabaseSchema]
```

### Nested Workflow Example

```yaml
flows:
  main:
    steps:
      - name: BackendWork
        agent: Backend
        nestedWorkflow: backend_workflow

  backend_workflow:
    steps:
      - name: Domain
        agent: Backend
        parallel: true
      - name: Infrastructure
        agent: Backend
        parallel: true
      - name: Integration
        agent: Backend
        dependsOn: [Domain, Infrastructure]
```

### Spawned Tasks Example

```yaml
steps:
  - name: Implementation
    agent: Backend
    description: Implement feature
    spawnTasks:
      - condition: "previousResult.success === true"
        agent: Tests
        description: Create tests
      - condition: "previousResult.success === true"
        agent: Docs
        description: Update documentation
```

## Execution Flow

1. **Dependency Resolution**: Steps are grouped by execution order
2. **Parallel Detection**: Steps in the same group with `parallel: true` are identified
3. **Concurrency Control**: Parallel steps execute with concurrency limits
4. **Nested Execution**: Nested workflows execute with parent context
5. **Task Spawning**: Conditions are evaluated and sub-tasks are created
6. **Result Aggregation**: Results from all tasks are collected and reported

## Best Practices

1. **Use Parallel Execution** for independent tasks to speed up workflows
2. **Use Nested Workflows** for reusable workflow components
3. **Use Spawned Tasks** for conditional or adaptive workflows
4. **Set Concurrency Limits** to avoid resource exhaustion
5. **Handle Errors** appropriately with `continue-on-error` or `abort-on-error`

## Limitations

- Condition evaluation is currently basic (JavaScript eval) - consider using a safer evaluator
- Nested workflows share context but don't support isolated execution contexts
- Parallel execution is limited by the concurrency setting
- Spawned tasks execute sequentially (not in parallel)

## Future Enhancements

- [ ] Safer condition evaluator (expression parser)
- [ ] Isolated execution contexts for nested workflows
- [ ] Parallel execution of spawned tasks
- [ ] Agent-to-agent communication protocols
- [ ] Workflow templates and composition
- [ ] Real-time progress tracking for parallel tasks

# Workflow Guide

Workflows define multi-step processes executed by AI agents in a structured, dependency-aware manner.

## Workflow Structure

```yaml
flows:
  my-workflow:
    name: my-workflow
    description: Description of what this workflow does
    steps:
      - name: step1
        agent: Architect
        description: First step description
        dependsOn: []
      - name: step2
        agent: Backend
        description: Second step description
        dependsOn: [step1]
```

## Creating Workflows

### Via Dashboard

1. Open dashboard: `maestro enable`
2. Click "Create Workflow"
3. Fill in workflow details
4. Add steps with dependencies
5. Save workflow

### Via YAML File

Edit `config/orchestration.yml`:

```yaml
flows:
  my-workflow:
    name: my-workflow
    description: My custom workflow
    steps:
      - name: analyze
        agent: Architect
        description: Analyze requirements
        dependsOn: []
      - name: implement
        agent: Backend
        description: Implement solution
        dependsOn: [analyze]
      - name: test
        agent: Tests
        description: Write tests
        dependsOn: [implement]
```

## Step Dependencies

Steps can depend on other steps:

```yaml
steps:
  - name: step1
    dependsOn: []  # No dependencies
  
  - name: step2
    dependsOn: [step1]  # Depends on step1
  
  - name: step3
    dependsOn: [step1, step2]  # Depends on both
```

## Parallel Execution

Steps without dependencies run in parallel:

```yaml
steps:
  - name: frontend
    dependsOn: []
  - name: backend
    dependsOn: []
  # Both run simultaneously
```

## Agent Roles

Available agent roles:
- **Architect** - System design and architecture
- **Backend** - .NET microservice implementation
- **Frontend** - Blazor/Razor components
- **Tests** - Unit, integration, E2E tests
- **Docs** - Documentation
- **TechnicalWriter** - Comprehensive documentation
- **Infrastructure** - Docker, Kubernetes
- **Contracts** - API contracts, DTOs
- **Events** - Event definitions, handlers
- **Database** - Database migrations
- **Security** - Authentication, authorization
- **Integration** - Service integration

## Advanced Features

### Nested Workflows

Execute workflows within workflows:

```yaml
steps:
  - name: sub-workflow
    nestedWorkflow: feature-implementation
    dependsOn: []
```

### Dynamic Task Spawning

Create tasks conditionally:

```yaml
steps:
  - name: analyze
    spawnTasks:
      - name: task1
        condition: "needsImplementation"
        agent: Backend
```

### Conditional Execution

Run steps conditionally:

```yaml
steps:
  - name: optional-step
    condition: "${feature.enabled}"
    agent: Backend
```

## Best Practices

1. **Clear Step Names** - Use descriptive, unique names
2. **Minimal Dependencies** - Only add necessary dependencies
3. **Parallel When Possible** - Structure for parallel execution
4. **Descriptive Descriptions** - Help agents understand tasks
5. **Appropriate Agents** - Match agent roles to tasks

## Examples

See [Examples](EXAMPLES.md) for complete workflow examples.

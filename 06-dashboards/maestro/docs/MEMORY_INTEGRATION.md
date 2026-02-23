# Memory Integration for Orchestrator Manager Agent

## Overview

The Orchestrator Manager Agent integrates with memory systems that are **GitHub submodules** (`.skmemory` and `.composermemory`) to leverage historical patterns, workflows, and best practices when analyzing projects, suggesting workflows, and optimizing execution.

**Important**: All memory systems are GitHub submodules and must be initialized before use.

## Supported Memory Systems

All memory systems are **GitHub submodules** that must be initialized before use.

### 1. SKMemory (`.skmemory`)

**Location**: `.skmemory/` (GitHub submodule)

**Submodule Setup**:
```bash
# Initialize submodule
git submodule update --init --recursive .skmemory

# Or initialize all submodules
git submodule update --init --recursive
```

**Structure**:
```
.skmemory/
├── v1/
│   ├── memory/
│   │   ├── public/
│   │   │   ├── short-term/
│   │   │   ├── mid-term/
│   │   │   └── long-term/
│   │   └── private/
│   └── scripts/
│       └── search-memory.sh
```

**Search Methods**:
- Primary: `search-memory.sh` script (if available)
- Fallback: `git grep` or `grep` on memory files

**What It Provides**:
- Project patterns and best practices
- Workflow templates and examples
- Historical decisions and learnings
- Domain-specific knowledge

### 2. ComposerMemory (`.composermemory`)

**Location**: `.composermemory/` (GitHub submodule, optional)

**Submodule Setup**:
```bash
# Initialize submodule
git submodule update --init --recursive .composermemory
```

**Structure** (detected automatically):
```
.composermemory/
├── memory/          # Common structure
├── v1/memory/       # Versioned structure
└── public/          # Alternative structure
```

**Search Methods**:
- `grep` on memory files (`.md`, `.txt`, `.json`)

**What It Provides**:
- Composer-specific patterns
- Cursor AI interaction history
- Code generation patterns
- Project-specific learnings

### 3. Orchestrator History (`.agent-orchestrator`)

**Location**: `.agent-orchestrator/` (GitHub submodule, self)

**Note**: The orchestrator itself is also a submodule, so it's always available.

**Structure**:
```
.agent-orchestrator/
├── logs/            # Execution logs
│   └── *.json
└── config/          # Workflow configurations
    └── *.yml
```

**What It Provides**:
- Previous workflow executions
- Execution patterns and results
- Workflow configurations
- Performance metrics

## Integration Points

### 1. Project Analysis

When analyzing a project, the Manager Agent searches memory for:
- Similar project structures
- Domain-specific patterns
- Technology stack patterns
- Workflow suggestions

**Example**:
```typescript
const memoryResults = await memorySearch.search({
  query: 'project structure microservice workflow',
  type: 'pattern',
  topK: 5,
});
```

### 2. Workflow Suggestions

When suggesting workflows, the Manager Agent searches for:
- Existing workflow patterns
- Best practices for workflow design
- Similar workflows from history
- Domain-specific workflow templates

**Example**:
```typescript
const workflowPatterns = await memorySearch.getWorkflowPatterns();
const bestPractices = await memorySearch.getBestPractices('workflow');
```

### 3. Workflow Optimization

When optimizing workflows, the Manager Agent searches for:
- Optimization patterns
- Similar workflows and their optimizations
- Performance improvement strategies
- Error handling patterns

**Example**:
```typescript
const similarWorkflows = await memorySearch.getSimilarWorkflows(workflowName);
const optimizationPatterns = await memorySearch.search({
  query: 'workflow optimization parallel execution',
  type: 'best-practice',
  topK: 3,
});
```

## Usage

### Automatic Detection

The Manager Agent automatically detects available memory systems:

```bash
npm run dev manager analyze --project .
# Memory systems available: SKMemory=true, ComposerMemory=false, Orchestrator=true
```

### Memory Context in Prompts

Memory search results are automatically included in prompts sent to Cursor CLI:

```
Analyze this project structure...

Relevant patterns from memory:
1. [skmemory] Microservice implementation pattern for Government domain...
2. [orchestrator] Previous workflow execution for similar project...
3. [skmemory] Best practice: Use parallel execution for independent steps...
```

### Disabling Memory Search

To disable memory integration:

```typescript
const agent = new OrchestratorManagerAgent(
  './config/orchestration.yml',
  process.cwd(),
  false  // useMemory = false
);
```

## Memory Search Service API

### Check Availability

```typescript
const availability = await memorySearch.checkAvailability();
// { skmemory: true, composermemory: false, orchestrator: true }
```

### Search Memory

```typescript
const results = await memorySearch.search({
  query: 'microservice implementation',
  topK: 10,
  type: 'pattern',
  tags: ['government', 'backend'],
  since: '2024-01-01',
});
```

### Get Workflow Patterns

```typescript
const patterns = await memorySearch.getWorkflowPatterns();
```

### Get Best Practices

```typescript
const practices = await memorySearch.getBestPractices('workflow');
```

### Get Similar Workflows

```typescript
const similar = await memorySearch.getSimilarWorkflows('feature-implementation');
```

## Memory Search Results

Each result includes:

```typescript
interface MemorySearchResult {
  source: 'skmemory' | 'composermemory' | 'orchestrator';
  content: string;           // Relevant content snippet
  path: string;             // File path or identifier
  relevance: number;        // 0.0 - 1.0 relevance score
  metadata?: {
    tags?: string[];        // Tags if available
    type?: string;         // Type (workflow, pattern, etc.)
    date?: string;         // Date if available
  };
}
```

## Benefits

### 1. Context-Aware Analysis
- Leverages historical patterns
- Considers project-specific learnings
- Uses domain knowledge from memory

### 2. Better Workflow Suggestions
- Based on successful past workflows
- Incorporates best practices
- Learns from execution history

### 3. Smarter Optimization
- References similar optimizations
- Uses proven patterns
- Avoids repeating mistakes

### 4. Continuous Learning
- Builds on past experiences
- Improves over time
- Shares knowledge across projects

## Configuration

### Environment Variables

```bash
# Disable memory search (default: enabled)
ORCHESTRATOR_DISABLE_MEMORY=true

# Custom memory paths
SKMEMORY_PATH=/custom/path/.skmemory
COMPOSERMEMORY_PATH=/custom/path/.composermemory
```

### Project Root Detection

The Manager Agent automatically detects the project root by:
1. Checking current working directory
2. Looking for `.git` directory
3. Finding parent of `.agent-orchestrator`

## Submodule Management

### Checking Submodule Status

```bash
# Check submodule status
git submodule status

# Output shows:
# - (dash) = Not initialized
# - (space) = Initialized and up to date
# - + (plus) = Has changes
# - U (U) = Has merge conflicts
```

### Initializing Submodules

```bash
# Initialize all submodules
git submodule update --init --recursive

# Initialize specific submodule
git submodule update --init --recursive .skmemory
git submodule update --init --recursive .composermemory
```

### Auto-Initialization

The Manager Agent can automatically initialize submodules:

```typescript
const memorySearch = new MemorySearchService(projectRoot);
await memorySearch.initializeSubmodules(); // Auto-initialize if needed
```

## Troubleshooting

### Memory Not Found

If memory systems aren't detected:

```bash
# 1. Check if submodules are configured
cat .gitmodules

# 2. Check submodule status
git submodule status

# 3. Initialize submodules
git submodule update --init --recursive

# 4. Verify paths and content
ls -la .skmemory
ls -la .composermemory
ls -la .agent-orchestrator

# 5. Check if submodules have content
ls -la .skmemory/v1/
```

### Submodule Not Initialized

If you see warnings about uninitialized submodules:

```bash
# Initialize all submodules
git submodule update --init --recursive

# Or initialize individually
git submodule init .skmemory
git submodule update .skmemory
```

### Submodule Update

To update submodules to latest:

```bash
# Update all submodules
git submodule update --remote --recursive

# Update specific submodule
git submodule update --remote .skmemory
```

### Search Script Not Available

If `search-memory.sh` isn't available, the service falls back to:
1. `git grep` (if in git repo)
2. Standard `grep` command

### Permission Issues

Ensure scripts are executable:

```bash
chmod +x .skmemory/v1/scripts/search-memory.sh
```

## Future Enhancements

1. **Qdrant Integration**: Use vector search for semantic similarity
2. **Embedding Search**: Search by meaning, not just keywords
3. **Memory Indexing**: Pre-index memory for faster searches
4. **Memory Updates**: Automatically update memory with new patterns
5. **Cross-Project Memory**: Share patterns across multiple projects

## Examples

### Example 1: Analyzing Project with Memory

```bash
npm run dev manager analyze --project .
```

**Output**:
```
Memory systems available: SKMemory=true, ComposerMemory=true, Orchestrator=true
🔍 Analyzing project structure...
📊 Found 3 relevant patterns from memory:
  1. [skmemory] Government microservice pattern...
  2. [composermemory] Cursor workflow for .NET services...
  3. [orchestrator] Previous execution: feature-implementation...
```

### Example 2: Suggesting Workflows with Memory Context

```bash
npm run dev manager suggest
```

**Memory Context Included**:
- 5 workflow patterns from SKMemory
- 3 best practices from ComposerMemory
- 2 similar workflows from Orchestrator history

### Example 3: Optimizing with Historical Data

```bash
npm run dev manager optimize --workflow feature_implementation
```

**Uses**:
- Execution logs from previous runs
- Optimization patterns from memory
- Similar workflow optimizations

## Best Practices

1. **Keep Memory Updated**: Regularly commit learnings to memory
2. **Tag Appropriately**: Use tags for better searchability
3. **Document Patterns**: Document successful workflow patterns
4. **Share Knowledge**: Use public memory for shareable patterns
5. **Review Results**: Review memory search results for relevance

## See Also

- [Manager Agent Documentation](./MANAGER_AGENT.md)
- [Enhancements](./ENHANCEMENTS.md)
- [SKMemory Documentation](../../.skmemory/v1/docs/)

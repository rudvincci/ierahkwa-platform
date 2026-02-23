# Workflow Creation & Model Selection ✅

## Overview

The dashboard now includes **workflow creation tools** and **model selection** for workflow execution. You can create workflows directly from the dashboard and choose which AI model to use.

## Features

### 1. Workflow Creation Tools

**Create Workflow Modal:**
- **Workflow Name**: Required field for workflow identifier
- **Description**: Optional description of what the workflow does
- **Steps Management**:
  - Add multiple steps dynamically
  - Each step has:
    - **Step Name**: Unique identifier for the step
    - **Agent Role**: Select from available agent roles (Architect, Backend, Frontend, etc.)
    - **Description**: What the step does
    - **Depends On**: Comma-separated list of step names this step depends on
  - Remove steps individually
- **Validation**: 
  - Ensures workflow name is unique
  - Validates step names are unique
  - Validates dependencies reference existing steps
  - Ensures at least one step exists

**Workflow Management:**
- Create new workflows via modal
- Workflows are saved to `orchestration.yml`
- Workflows appear in the workflow selector immediately after creation

### 2. Model Selection

**Model Dropdown:**
- **Default**: Uses Claude 3.5 Sonnet (claude-3-5-sonnet-20241022)
- **Available Models**:
  - Claude 3.5 Sonnet (claude-3-5-sonnet-20241022)
  - Claude 3 Opus (claude-3-opus-20240229)
  - Claude 3 Sonnet (claude-3-sonnet-20240229)
  - Claude 3 Haiku (claude-3-haiku-20240307)
  - Claude 3.5 Haiku (claude-3-5-haiku-20241022)

**How It Works:**
- Model selection is passed to `cursor-agent` via `--model` flag
- If no model is selected, uses default
- Model applies to all steps in the workflow

## Usage

### Creating a Workflow

1. **Open Dashboard**: `npm run dev dashboard` → http://localhost:3000
2. **Click "Create Workflow"** button
3. **Fill in Details**:
   - Enter workflow name (e.g., `my-custom-workflow`)
   - Add description (optional)
   - Add steps:
     - Click "+ Add Step"
     - Fill in step name, select agent role, add description
     - Add dependencies if needed (comma-separated step names)
     - Add more steps as needed
4. **Click "Create Workflow"**
5. **Workflow appears** in the workflow selector immediately

### Running a Workflow with Model Selection

1. **Select Workflow** from dropdown
2. **Select Model** from model dropdown (or leave as default)
3. **Select Runner** (Cursor CLI recommended)
4. **Optionally enable** verbose mode
5. **Click "Start Workflow"**

## API Endpoints

### Workflow Creation

- `GET /api/workflows/roles` - Get available agent roles
- `POST /api/workflows/create` - Create a new workflow
- `PUT /api/workflows/:name` - Update an existing workflow
- `DELETE /api/workflows/:name` - Delete a workflow

### Example: Create Workflow

```bash
curl -X POST http://localhost:3000/api/workflows/create \
  -H "Content-Type: application/json" \
  -d '{
    "name": "my-workflow",
    "description": "My custom workflow",
    "steps": [
      {
        "name": "step1",
        "agent": "Architect",
        "description": "Design the architecture",
        "dependsOn": []
      },
      {
        "name": "step2",
        "agent": "Backend",
        "description": "Implement backend",
        "dependsOn": ["step1"]
      }
    ]
  }'
```

## Implementation Details

### WorkflowCreator Service

- **Location**: `src/services/WorkflowCreator.ts`
- **Features**:
  - Validates workflow structure
  - Checks for duplicate names
  - Validates dependencies
  - Saves to YAML config file
  - Loads available agent roles

### Model Selection

- **CLI Support**: Added `--model` option to `run` command
- **Dashboard Integration**: Model dropdown in workflow management section
- **Pass-through**: Model is passed to `CursorCliAgentRunner` and then to `cursor-agent`

### Workflow Definition Structure

```yaml
flows:
  my-workflow:
    name: my-workflow
    description: My custom workflow
    steps:
      - name: step1
        agent: Architect
        description: Design the architecture
        dependsOn: []
      - name: step2
        agent: Backend
        description: Implement backend
        dependsOn: [step1]
```

## Benefits

✅ **No Manual YAML Editing**: Create workflows through UI  
✅ **Model Flexibility**: Choose the right model for each workflow  
✅ **Validation**: Prevents invalid workflow configurations  
✅ **Immediate Availability**: Created workflows appear instantly  
✅ **Dependency Management**: Visual step dependencies  

## Future Enhancements

- [ ] Edit existing workflows from dashboard
- [ ] Clone/duplicate workflows
- [ ] Workflow templates
- [ ] Visual workflow editor (drag-and-drop)
- [ ] Step reordering
- [ ] Model selection per step (not just per workflow)

---

**Create workflows and select models - all from the dashboard!** 🎉

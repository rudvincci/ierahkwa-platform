import { useState, useEffect, useCallback } from 'react';
import type { WorkflowDefinition } from '../../types/workflow';
import { api } from '../../services/api';
import './WorkflowControls.css';

interface WorkflowControlsProps {
  workflows: WorkflowDefinition[];
  onWorkflowStart: () => void;
}

interface ModelInfo {
  id: string;
  name: string;
  provider: string;
  contextWindow: number;
  description?: string;
  capabilities?: string[];
  pricing?: {
    input: number;
    output: number;
    cacheWrite?: number;
    cacheRead?: number;
  };
}

export default function WorkflowControls({ workflows, onWorkflowStart }: WorkflowControlsProps) {
  const [selectedWorkflow, setSelectedWorkflow] = useState<string>('');
  const [selectedModel, setSelectedModel] = useState<string>('');
  const [models, setModels] = useState<ModelInfo[]>([]);
  const [loading, setLoading] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [generationPrompt, setGenerationPrompt] = useState('');
  const [showGenerator, setShowGenerator] = useState(false);
  const [generatedWorkflow, setGeneratedWorkflow] = useState<WorkflowDefinition | null>(null);

  const loadModels = useCallback(async () => {
    try {
      const m = await api.getModels();
      // Handle both array of strings and array of model objects
      let modelArray: ModelInfo[] = [];
      
      if (Array.isArray(m) && m.length > 0) {
        if (typeof m[0] === 'string') {
          // Array of strings
          modelArray = (m as string[]).map(id => ({ id, name: id, provider: '', contextWindow: 0 }));
        } else if (typeof m[0] === 'object' && m[0] !== null) {
          // Array of objects - ensure they have id property
          modelArray = (m as unknown[]).map((model: unknown) => {
            const modelObj = model as Partial<ModelInfo>;
            return {
              id: modelObj.id || String(model),
              name: modelObj.name || modelObj.id || String(model),
              provider: modelObj.provider || '',
              contextWindow: modelObj.contextWindow || 0,
              description: modelObj.description,
              capabilities: modelObj.capabilities,
              pricing: modelObj.pricing,
            };
          });
        }
      }
      
      setModels(modelArray);
      if (modelArray.length > 0 && !selectedModel) {
        const firstModel = modelArray[0];
        if (firstModel) {
          const firstModelId = typeof firstModel === 'string' 
            ? firstModel 
            : (firstModel?.id ? String(firstModel.id) : '');
          if (firstModelId) {
            setSelectedModel(firstModelId);
          }
        }
      }
    } catch (error) {
      console.error('Failed to load models:', error);
    }
  }, [selectedModel]);

  useEffect(() => {
    loadModels();
  }, [loadModels]);

  const handleStart = async () => {
    if (!selectedWorkflow) return;
    setLoading(true);
    try {
      await api.startWorkflow(selectedWorkflow, selectedModel || undefined);
      onWorkflowStart();
    } catch (error) {
      console.error('Failed to start workflow:', error);
      alert('Failed to start workflow');
    } finally {
      setLoading(false);
    }
  };

  const handleGenerateWorkflow = async () => {
    if (!generationPrompt.trim()) {
      alert('Please enter a description for the workflow');
      return;
    }
    
    setGenerating(true);
    try {
      const workflow = await api.generateWorkflow(generationPrompt);
      setGeneratedWorkflow(workflow);
      setShowGenerator(false);
      
      // Automatically start the generated workflow
      if (selectedModel) {
        try {
          await api.startWorkflow(workflow.name, selectedModel);
          onWorkflowStart();
          alert(`Workflow "${workflow.name}" generated and started! Monitoring in progress...`);
        } catch (error) {
          console.error('Failed to start generated workflow:', error);
          alert(`Workflow "${workflow.name}" generated successfully, but failed to start. You can start it manually.`);
        }
      } else {
        alert(`Workflow "${workflow.name}" generated successfully! Select a model and start it manually.`);
      }
    } catch (error) {
      console.error('Failed to generate workflow:', error);
      alert('Failed to generate workflow. Please try again.');
    } finally {
      setGenerating(false);
      setGenerationPrompt('');
    }
  };

  return (
    <div className="workflow-controls">
      <div className="control-group">
        <label>Workflow:</label>
        <select
          value={selectedWorkflow}
          onChange={(e) => setSelectedWorkflow(e.target.value)}
          disabled={loading}
        >
          <option value="">Select workflow...</option>
          {workflows.map(wf => (
            <option key={wf.name} value={wf.name}>{wf.name}</option>
          ))}
        </select>
        
        <label>Model:</label>
        <select
          value={selectedModel}
          onChange={(e) => setSelectedModel(e.target.value)}
          disabled={loading}
        >
          {models
            .filter((model) => model !== null && model !== undefined)
            .map((model, index) => {
              // Safely extract id and name, ensuring we never render an object
              let modelId = '';
              let modelName = '';
              
              try {
                if (typeof model === 'string') {
                  modelId = model;
                  modelName = model;
                } else if (model && typeof model === 'object' && 'id' in model) {
                  // Ensure id and name are strings, never objects
                  modelId = model.id ? String(model.id) : `model-${index}`;
                  modelName = model.name ? String(model.name) : modelId;
                } else {
                  modelId = `model-${index}`;
                  modelName = `Model ${index + 1}`;
                }
              } catch {
                // Fallback if anything goes wrong
                modelId = `model-${index}`;
                modelName = `Model ${index + 1}`;
              }
              
              // Double-check we have valid strings
              if (!modelId || typeof modelId !== 'string') {
                modelId = `model-${index}`;
              }
              if (!modelName || typeof modelName !== 'string') {
                modelName = modelId;
              }
              
              return (
                <option key={modelId} value={modelId}>{modelName}</option>
              );
            })}
        </select>
        
        <button
          className="btn btn-primary"
          onClick={handleStart}
          disabled={!selectedWorkflow || loading}
        >
          {loading ? 'Starting...' : 'Start Workflow'}
        </button>
      </div>

      <div className="workflow-generator">
        <button
          className="btn btn-secondary"
          onClick={() => setShowGenerator(!showGenerator)}
        >
          {showGenerator ? '▼' : '▶'} {showGenerator ? 'Hide' : 'Show'} AI Workflow Generator
        </button>
        
        {showGenerator && (
          <div className="generator-panel">
            <h3>✨ Generate Workflow with AI</h3>
            <p className="generator-description">
              Describe what you want the workflow to do, and the AI will create a complete workflow with steps, tasks, and agents.
              The workflow will be automatically started and monitored.
            </p>
            
            <textarea
              className="generator-input"
              value={generationPrompt}
              onChange={(e) => setGenerationPrompt(e.target.value)}
              placeholder="Example: Create a workflow that reviews code quality, runs automated tests, and generates a compliance report. The workflow should have 3 steps: code analysis, test execution, and report generation."
              rows={4}
              disabled={generating}
            />
            
            <button
              className="btn btn-primary btn-generate"
              onClick={handleGenerateWorkflow}
              disabled={!generationPrompt.trim() || generating || !selectedModel}
            >
              {generating ? '✨ Generating...' : '✨ Generate & Start Workflow'}
            </button>
            
            {!selectedModel && (
              <p className="generator-warning">
                ⚠️ Please select a model above before generating a workflow.
              </p>
            )}
            
            {generatedWorkflow && (
              <div className="generated-workflow-info">
                <h4>✅ Generated: {generatedWorkflow.name}</h4>
                {generatedWorkflow.description && (
                  <p>{generatedWorkflow.description}</p>
                )}
                <p className="workflow-stats">
                  {generatedWorkflow.steps?.length || 0} steps, {generatedWorkflow.agents?.length || 0} agents
                </p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

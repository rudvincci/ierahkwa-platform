import { useState, useEffect } from 'react';
import type { WorkflowDefinition } from '../../types/workflow';
import type { DashboardMetrics, TaskProgress } from '../../types/metrics';
import { api } from '../../services/api';
import StatusBadge from '../common/StatusBadge';
import { formatDuration, formatPercent, formatCost } from '../../utils/formatters';
import './WorkflowCard.css';

interface WorkflowCardProps {
  workflow: WorkflowDefinition;
  metrics?: DashboardMetrics;
}

type TabType = 'overview' | 'backlog' | 'steps' | 'metrics' | 'agents' | 'details';

export default function WorkflowCard({ workflow, metrics }: WorkflowCardProps) {
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<TabType>('overview');
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [backlog, setBacklog] = useState<Array<{ stepName: string; taskName?: string; priority: number; description?: string; estimatedEffort?: number; dependencies?: string[]; addedAt: string; status?: string }>>([]);
  const [expandedSections, setExpandedSections] = useState<Set<string>>(new Set());

  const status = metrics?.status || 'stopped';
  const isRunning = status === 'running';

  const toggleSection = (section: string) => {
    setExpandedSections(prev => {
      const next = new Set(prev);
      if (next.has(section)) {
        next.delete(section);
      } else {
        next.add(section);
      }
      return next;
    });
  };

  // Load backlog when workflow is running
  useEffect(() => {
    if (isRunning) {
      const loadBacklog = async () => {
        try {
          const response = await api.getWorkflowBacklog(workflow.name);
          const backlogItems = response.backlog || [];
          const enrichedBacklog = backlogItems.map(item => {
            const taskKey = item.taskName ? `${item.stepName}.${item.taskName}` : item.stepName;
            const progress = metrics?.taskProgress?.[taskKey];
            return {
              ...item,
              status: progress?.status || 'pending',
              completionPercent: progress?.completionPercent || 0,
            };
          });
          setBacklog(enrichedBacklog);
        } catch (error) {
          console.error('Failed to load backlog:', error);
        }
      };
      loadBacklog();
      const interval = setInterval(loadBacklog, 5000);
      return () => clearInterval(interval);
    }
  }, [isRunning, workflow.name, metrics?.taskProgress]);

  const handleStart = async () => {
    setLoading(true);
    try {
      await api.startWorkflow(workflow.name);
    } catch (error) {
      console.error('Failed to start workflow:', error);
      alert('Failed to start workflow');
    } finally {
      setLoading(false);
    }
  };

  const handleStop = async () => {
    if (!metrics) return;
    setLoading(true);
    try {
      await api.stopWorkflow(workflow.name, metrics.executionId);
    } catch (error) {
      console.error('Failed to stop workflow:', error);
      alert('Failed to stop workflow');
    } finally {
      setLoading(false);
    }
  };

  const handleReevaluate = async () => {
    if (!isRunning) return;
    setLoading(true);
    try {
      await api.reevaluateWorkflow(workflow.name);
      alert('Workflow reevaluation started. Maestro will reassign steps to agents.');
    } catch (error) {
      console.error('Failed to reevaluate workflow:', error);
      alert('Failed to reevaluate workflow');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (isRunning) {
      alert('Please stop the workflow before deleting it.');
      return;
    }
    if (!showDeleteConfirm) {
      setShowDeleteConfirm(true);
      return;
    }
    setLoading(true);
    try {
      await api.deleteWorkflow(workflow.name);
      alert('Workflow deleted successfully');
    } catch (error) {
      console.error('Failed to delete workflow:', error);
      alert('Failed to delete workflow');
    } finally {
      setLoading(false);
      setShowDeleteConfirm(false);
    }
  };

  const getTaskStatusIcon = (status: string) => {
    switch (status) {
      case 'completed':
        return '✓';
      case 'running':
        return '⟳';
      case 'failed':
        return '✗';
      default:
        return '○';
    }
  };

  const getTaskStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return '#22c55e';
      case 'running':
        return '#f59e0b';
      case 'failed':
        return '#ef4444';
      default:
        return '#888';
    }
  };

  const getTaskProgress = (stepName: string, taskName?: string): TaskProgress | undefined => {
    if (!metrics?.taskProgress) return undefined;
    const key = taskName ? `${stepName}.${taskName}` : stepName;
    return metrics.taskProgress[key];
  };

  const overallProgress = metrics
    ? Math.round((metrics.completedSteps / metrics.totalSteps) * 100)
    : 0;

  return (
    <div className="workflow-card-modern">
      {/* Header */}
      <div className="workflow-header-modern">
        <div className="header-left">
          <h3>{workflow.name}</h3>
          <StatusBadge status={status} />
          {metrics && (
            <div className="progress-indicator">
              <div className="progress-bar">
                <div 
                  className="progress-fill" 
                  style={{ width: `${overallProgress}%` }}
                />
              </div>
              <span className="progress-text">{overallProgress}%</span>
            </div>
          )}
        </div>
        <div className="header-actions">
          {isRunning ? (
            <>
              <button className="btn btn-warning btn-small" onClick={handleStop} disabled={loading}>
                Stop
              </button>
              <button className="btn btn-primary btn-small" onClick={handleReevaluate} disabled={loading}>
                Reevaluate
              </button>
            </>
          ) : (
            <button className="btn btn-success btn-small" onClick={handleStart} disabled={loading}>
              Run
            </button>
          )}
          <button 
            className="btn btn-danger btn-small" 
            onClick={handleDelete} 
            disabled={loading}
          >
            {showDeleteConfirm ? 'Confirm Delete' : 'Delete'}
          </button>
        </div>
      </div>

      {/* Tabs */}
      <div className="workflow-tabs">
        <button 
          className={`tab ${activeTab === 'overview' ? 'active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          Overview
        </button>
        {isRunning && (
          <button 
            className={`tab ${activeTab === 'backlog' ? 'active' : ''}`}
            onClick={() => setActiveTab('backlog')}
          >
            Backlog {backlog.length > 0 && <span className="tab-badge">{backlog.length}</span>}
          </button>
        )}
        <button 
          className={`tab ${activeTab === 'steps' ? 'active' : ''}`}
          onClick={() => setActiveTab('steps')}
        >
          Steps {metrics && <span className="tab-badge">{metrics.completedSteps}/{metrics.totalSteps}</span>}
        </button>
        <button 
          className={`tab ${activeTab === 'metrics' ? 'active' : ''}`}
          onClick={() => setActiveTab('metrics')}
        >
          Metrics
        </button>
        <button 
          className={`tab ${activeTab === 'agents' ? 'active' : ''}`}
          onClick={() => setActiveTab('agents')}
          disabled={!workflow.agents || workflow.agents.length === 0}
        >
          Agents {workflow.agents?.length || 0}
        </button>
        <button 
          className={`tab ${activeTab === 'details' ? 'active' : ''}`}
          onClick={() => setActiveTab('details')}
        >
          Details
        </button>
      </div>

      {/* Tab Content */}
      <div className="tab-content">
        {activeTab === 'overview' && (
          <div className="overview-tab">
            {workflow.description && (
              <CollapsiblePanel title="Description" defaultExpanded={true}>
                <p className="description-text">{workflow.description}</p>
              </CollapsiblePanel>
            )}
            
            {metrics && (
              <>
                <div className="quick-stats">
                  <div className="stat-card">
                    <div className="stat-icon">⏱️</div>
                    <div className="stat-info">
                      <div className="stat-label">Duration</div>
                      <div className="stat-value">{formatDuration(metrics.duration)}</div>
                    </div>
                  </div>
                  <div className="stat-card">
                    <div className="stat-icon">✓</div>
                    <div className="stat-info">
                      <div className="stat-label">Success Rate</div>
                      <div className="stat-value">{formatPercent(metrics.successRate)}</div>
                    </div>
                  </div>
                  <div className="stat-card">
                    <div className="stat-icon">📊</div>
                    <div className="stat-info">
                      <div className="stat-label">Steps</div>
                      <div className="stat-value">{metrics.completedSteps}/{metrics.totalSteps}</div>
                    </div>
                  </div>
                  {metrics.totalCost && (
                    <div className="stat-card">
                      <div className="stat-icon">💰</div>
                      <div className="stat-info">
                        <div className="stat-label">Cost</div>
                        <div className="stat-value">{formatCost(metrics.totalCost)}</div>
                      </div>
                    </div>
                  )}
                </div>

                {metrics.currentActivity && (
                  <CollapsiblePanel title="Current Activity" defaultExpanded={true}>
                    <div className="activity-text">{metrics.currentActivity}</div>
                  </CollapsiblePanel>
                )}

                {metrics.currentStep && (
                  <CollapsiblePanel title="Current Step">
                    <div className="detail-item">
                      <span className="detail-label">Step:</span>
                      <span className="detail-value">{metrics.currentStep}</span>
                    </div>
                  </CollapsiblePanel>
                )}

                {metrics.cursorAgentGuid && (
                  <CollapsiblePanel title="Cursor Agent GUID">
                    <div className="guid-display">{metrics.cursorAgentGuid}</div>
                  </CollapsiblePanel>
                )}
              </>
            )}
          </div>
        )}

        {activeTab === 'backlog' && (
          <div className="backlog-tab">
            <div className="backlog-header">
              <h4>Prioritized Backlog</h4>
              <span className="backlog-count">{backlog.length} items</span>
            </div>
            {backlog.length === 0 ? (
              <div className="empty-state">No items in backlog</div>
            ) : (
              <div className="backlog-list">
                {backlog.map((item, idx) => {
                  const taskStatus = item.status || 'pending';
                  const progress = getTaskProgress(item.stepName, item.taskName);
                  return (
                    <div key={idx} className={`backlog-item backlog-${taskStatus}`}>
                      <div className="backlog-priority">#{item.priority}</div>
                      <div className="backlog-status-indicator" style={{ color: getTaskStatusColor(taskStatus) }}>
                        {getTaskStatusIcon(taskStatus)}
                      </div>
                      <div className="backlog-content">
                        <div className="backlog-title">
                          <span className="backlog-step">{item.stepName}</span>
                          {item.taskName && <span className="backlog-task">.{item.taskName}</span>}
                        </div>
                        {item.description && (
                          <div className="backlog-description">{item.description}</div>
                        )}
                        {item.dependencies && item.dependencies.length > 0 && (
                          <div className="backlog-deps">Depends on: {item.dependencies.join(', ')}</div>
                        )}
                        {progress && progress.completionPercent > 0 && (
                          <div className="backlog-progress">
                            <div className="progress-bar-small">
                              <div 
                                className="progress-fill-small" 
                                style={{ width: `${progress.completionPercent}%` }}
                              />
                            </div>
                            <span>{progress.completionPercent}%</span>
                          </div>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        )}

        {activeTab === 'steps' && (
          <div className="steps-tab">
            {(() => {
              // Ensure steps is always an array
              const steps = Array.isArray(workflow.steps) ? workflow.steps : [];
              
              if (steps.length === 0) {
                return (
                  <div className="empty-state">
                    <p>No steps defined for this workflow</p>
                    <p className="empty-state-hint">Steps will appear here once loaded from the workflow YAML file</p>
                    {workflow.yamlPath && (
                      <p className="empty-state-hint">YAML file: {workflow.yamlPath}</p>
                    )}
                  </div>
                );
              }
              
              return steps
                .filter((step): step is NonNullable<typeof step> => step != null && typeof step === 'object' && 'name' in step && typeof step.name === 'string')
                .map((step, idx) => {
              const stepProgress = getTaskProgress(step.name);
              const stepStatus = stepProgress?.status || 'pending';
              const isExpanded = expandedSections.has(`step-${idx}`);
              return (
                <CollapsiblePanel
                  key={idx}
                  title={
                    <div className="step-header-inline">
                      <span className="step-status-indicator" style={{ color: getTaskStatusColor(stepStatus) }}>
                        {getTaskStatusIcon(stepStatus)}
                      </span>
                      <span className="step-name">{step.name}</span>
                      {stepProgress && (
                        <span className="step-progress-inline">
                          {stepProgress.completionPercent}%
                        </span>
                      )}
                    </div>
                  }
                  defaultExpanded={isExpanded}
                  onToggle={() => toggleSection(`step-${idx}`)}
                >
                  <div className="step-details">
                    {step.description && (
                      <div className="detail-item">
                        <span className="detail-label">Description:</span>
                        <span className="detail-value">{step.description}</span>
                      </div>
                    )}
                    {step.agent && (
                      <div className="detail-item">
                        <span className="detail-label">Agent:</span>
                        <span className="detail-value">{step.agent}</span>
                      </div>
                    )}
                    {step.model && (
                      <div className="detail-item">
                        <span className="detail-label">Model:</span>
                        <span className="detail-value">{step.model}</span>
                      </div>
                    )}
                    {step.type && (
                      <div className="detail-item">
                        <span className="detail-label">Type:</span>
                        <span className="detail-value">{step.type}</span>
                      </div>
                    )}
                    {step.depends_on && (
                      <div className="detail-item">
                        <span className="detail-label">Depends On:</span>
                        <span className="detail-value">
                          {Array.isArray(step.depends_on) ? step.depends_on.join(', ') : step.depends_on}
                        </span>
                      </div>
                    )}
                    {step.dependencies && step.dependencies.length > 0 && (
                      <div className="detail-item">
                        <span className="detail-label">Dependencies:</span>
                        <span className="detail-value">{step.dependencies.join(', ')}</span>
                      </div>
                    )}
                    {step.condition && (
                      <div className="detail-item">
                        <span className="detail-label">Condition:</span>
                        <span className="detail-value code-text">{step.condition}</span>
                      </div>
                    )}
                    {step.expectedOutput && (
                      <div className="detail-item">
                        <span className="detail-label">Expected Output:</span>
                        <span className="detail-value">{step.expectedOutput}</span>
                      </div>
                    )}
                    {step.tasks && Array.isArray(step.tasks) && step.tasks.length > 0 && (
                      <div className="tasks-section">
                        <div className="section-title">Tasks ({step.tasks.length})</div>
                        {step.tasks
                          .filter((task): task is NonNullable<typeof task> => task != null && typeof task === 'object' && 'name' in task && typeof task.name === 'string')
                          .map((task, taskIdx) => {
                          const taskProgress = getTaskProgress(step.name, task.name);
                          const taskStatus = taskProgress?.status || 'pending';
                          return (
                            <CollapsiblePanel
                              key={taskIdx}
                              title={
                                <div className="task-header-inline">
                                  <span className="task-status-indicator" style={{ color: getTaskStatusColor(taskStatus) }}>
                                    {getTaskStatusIcon(taskStatus)}
                                  </span>
                                  <span className="task-name">{task.name}</span>
                                  {taskProgress && taskProgress.completionPercent > 0 && (
                                    <span className="task-progress-inline">
                                      {taskProgress.completionPercent}%
                                    </span>
                                  )}
                                </div>
                              }
                            >
                              <div className="task-details">
                                {task.command && (
                                  <div className="detail-item">
                                    <span className="detail-label">Command:</span>
                                    <pre className="detail-value code-text">{task.command}</pre>
                                  </div>
                                )}
                                {task.output && (
                                  <div className="detail-item">
                                    <span className="detail-label">Output:</span>
                                    <span className="detail-value">{task.output}</span>
                                  </div>
                                )}
                                {task.condition && (
                                  <div className="detail-item">
                                    <span className="detail-label">Condition:</span>
                                    <span className="detail-value code-text">{task.condition}</span>
                                  </div>
                                )}
                                {task.for_each && (
                                  <div className="detail-item">
                                    <span className="detail-label">For Each:</span>
                                    <span className="detail-value code-text">{task.for_each}</span>
                                  </div>
                                )}
                                {taskProgress && (
                                  <>
                                    {taskProgress.issues && taskProgress.issues.length > 0 && (
                                      <div className="detail-item">
                                        <span className="detail-label">Issues:</span>
                                        <ul className="detail-list">
                                          {taskProgress.issues.map((issue, i) => (
                                            <li key={i}>{issue}</li>
                                          ))}
                                        </ul>
                                      </div>
                                    )}
                                    {taskProgress.subTasks && taskProgress.subTasks.length > 0 && (
                                      <div className="detail-item">
                                        <span className="detail-label">Sub Tasks:</span>
                                        <ul className="detail-list">
                                          {taskProgress.subTasks.map((subTask, i) => (
                                            <li key={i}>
                                              {subTask}
                                              {taskProgress.completedSubTasks?.includes(subTask) && (
                                                <span className="completed-badge">✓</span>
                                              )}
                                            </li>
                                          ))}
                                        </ul>
                                      </div>
                                    )}
                                  </>
                                )}
                              </div>
                            </CollapsiblePanel>
                          );
                        })}
                      </div>
                    )}
                  </div>
                </CollapsiblePanel>
              );
            });
            })()}
          </div>
        )}

        {activeTab === 'metrics' && metrics && (
          <div className="metrics-tab">
            <CollapsiblePanel title="Execution Metrics" defaultExpanded={true}>
              <div className="metrics-grid">
                <div className="metric-item">
                  <div className="metric-label">Duration</div>
                  <div className="metric-value">{formatDuration(metrics.duration)}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Success Rate</div>
                  <div className="metric-value">{formatPercent(metrics.successRate)}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Completed Steps</div>
                  <div className="metric-value">{metrics.completedSteps}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Failed Steps</div>
                  <div className="metric-value">{metrics.failedSteps}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Total Steps</div>
                  <div className="metric-value">{metrics.totalSteps}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Cache Hits</div>
                  <div className="metric-value">{metrics.cacheHits}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Cache Misses</div>
                  <div className="metric-value">{metrics.cacheMisses}</div>
                </div>
                <div className="metric-item">
                  <div className="metric-label">Retry Attempts</div>
                  <div className="metric-value">{metrics.retryAttempts}</div>
                </div>
              </div>
            </CollapsiblePanel>

            {metrics.totalTokens && (
              <CollapsiblePanel title="Token Usage">
                <div className="metrics-grid">
                  <div className="metric-item">
                    <div className="metric-label">Total Tokens</div>
                    <div className="metric-value">{metrics.totalTokens.toLocaleString()}</div>
                  </div>
                  {metrics.contextWindowPercent && (
                    <div className="metric-item">
                      <div className="metric-label">Context Window</div>
                      <div className="metric-value">{formatPercent(metrics.contextWindowPercent)}</div>
                    </div>
                  )}
                  {metrics.currentTokenUsage && (
                    <>
                      <div className="metric-item">
                        <div className="metric-label">Input Tokens</div>
                        <div className="metric-value">{metrics.currentTokenUsage.inputTokens.toLocaleString()}</div>
                      </div>
                      <div className="metric-item">
                        <div className="metric-label">Output Tokens</div>
                        <div className="metric-value">{metrics.currentTokenUsage.outputTokens.toLocaleString()}</div>
                      </div>
                    </>
                  )}
                </div>
              </CollapsiblePanel>
            )}

            {metrics.totalCost && (
              <CollapsiblePanel title="Cost Analysis">
                <div className="metrics-grid">
                  <div className="metric-item">
                    <div className="metric-label">Total Cost</div>
                    <div className="metric-value">{formatCost(metrics.totalCost)}</div>
                  </div>
                  {metrics.costPerStep && typeof metrics.costPerStep === 'object' && !Array.isArray(metrics.costPerStep) && Object.keys(metrics.costPerStep).length > 0 && (
                    <div className="cost-per-step">
                      <div className="section-title">Cost Per Step</div>
                      {Object.entries(metrics.costPerStep).map(([step, cost]) => (
                        <div key={step} className="detail-item">
                          <span className="detail-label">{step}:</span>
                          <span className="detail-value">{formatCost(typeof cost === 'number' ? cost : 0)}</span>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </CollapsiblePanel>
            )}

            {metrics.modelHistory && Array.isArray(metrics.modelHistory) && metrics.modelHistory.length > 0 && (
              <CollapsiblePanel title="Model History">
                <div className="model-history-list">
                  {metrics.modelHistory
                    .filter((entry): entry is NonNullable<typeof entry> => entry != null && typeof entry === 'object' && 'timestamp' in entry && 'model' in entry)
                    .map((entry, idx) => (
                    <div key={idx} className="history-item">
                      <span className="history-time">{new Date(entry.timestamp).toLocaleString()}</span>
                      <span className="history-model">{String(entry.model)}</span>
                      {entry.stepName && <span className="history-step">{String(entry.stepName)}</span>}
                    </div>
                  ))}
                </div>
              </CollapsiblePanel>
            )}

            {metrics.agentSwitches && Array.isArray(metrics.agentSwitches) && metrics.agentSwitches.length > 0 && (
              <CollapsiblePanel title="Agent Switches">
                <div className="agent-switches-list">
                  {metrics.agentSwitches
                    .filter((switch_): switch_ is NonNullable<typeof switch_> => switch_ != null && typeof switch_ === 'object' && 'timestamp' in switch_ && 'fromAgent' in switch_ && 'toAgent' in switch_)
                    .map((switch_, idx) => (
                    <div key={idx} className="switch-item">
                      <span className="switch-time">{new Date(switch_.timestamp).toLocaleString()}</span>
                      <span className="switch-detail">
                        {String(switch_.fromAgent)} → {String(switch_.toAgent)}
                      </span>
                      {switch_.stepName && <span className="switch-step">{String(switch_.stepName)}</span>}
                    </div>
                  ))}
                </div>
              </CollapsiblePanel>
            )}

            {metrics.currentModel && (
              <CollapsiblePanel title="Current Model">
                <div className="detail-item">
                  <span className="detail-label">Model:</span>
                  <span className="detail-value">{metrics.currentModel}</span>
                </div>
              </CollapsiblePanel>
            )}

            {metrics.currentAgent && (
              <CollapsiblePanel title="Current Agent">
                <div className="detail-item">
                  <span className="detail-label">Agent:</span>
                  <span className="detail-value">{metrics.currentAgent}</span>
                </div>
              </CollapsiblePanel>
            )}

            {metrics.processStatus && (
              <CollapsiblePanel title="Process Status">
                <div className="metrics-grid">
                  <div className="metric-item">
                    <div className="metric-label">Running</div>
                    <div className="metric-value">{metrics.processStatus.running ? 'Yes' : 'No'}</div>
                  </div>
                  {metrics.processStatus.pid && (
                    <div className="metric-item">
                      <div className="metric-label">PID</div>
                      <div className="metric-value">{metrics.processStatus.pid}</div>
                    </div>
                  )}
                  {metrics.processStatus.cursorAgentRunning !== undefined && (
                    <div className="metric-item">
                      <div className="metric-label">Cursor Agent</div>
                      <div className="metric-value">{metrics.processStatus.cursorAgentRunning ? 'Running' : 'Stopped'}</div>
                    </div>
                  )}
                </div>
              </CollapsiblePanel>
            )}
          </div>
        )}

        {activeTab === 'agents' && (
          <div className="agents-tab">
            {workflow.agents && Array.isArray(workflow.agents) && workflow.agents.length > 0 ? (
              <>
                <div className="agents-header">
                  <h4>Workflow Agents ({workflow.agents.length})</h4>
                  <p className="agents-description">Agents assigned to this workflow</p>
                </div>
                {workflow.agents
                  .filter((agent): agent is NonNullable<typeof agent> => agent != null && typeof agent === 'object' && 'name' in agent)
                  .map((agent, idx) => (
                  <CollapsiblePanel
                    key={idx}
                    title={
                      <div className="agent-header-inline">
                        <span className="agent-name">{agent.name}</span>
                        {agent.role && <span className="agent-role-badge">{String(agent.role)}</span>}
                        {metrics?.agentGuids?.[agent.name] && typeof metrics.agentGuids[agent.name] === 'string' && (
                          <span className="agent-guid-badge">GUID: {metrics.agentGuids[agent.name].substring(0, 8)}...</span>
                        )}
                      </div>
                    }
                    defaultExpanded={idx === 0}
                  >
                    <div className="agent-details">
                      {agent.role && (
                        <div className="detail-item">
                          <span className="detail-label">Role:</span>
                          <span className="detail-value">{String(agent.role)}</span>
                        </div>
                      )}
                      {metrics?.agentGuids?.[agent.name] && typeof metrics.agentGuids[agent.name] === 'string' && (
                        <div className="detail-item">
                          <span className="detail-label">Cursor Agent GUID:</span>
                          <div className="guid-display">{metrics.agentGuids[agent.name]}</div>
                        </div>
                      )}
                      {Object.entries(agent)
                        .filter(([key]) => !['name', 'role'].includes(key))
                        .map(([key, value]) => (
                        <div key={key} className="detail-item">
                          <span className="detail-label">{key.replace(/_/g, ' ')}:</span>
                          <span className="detail-value">{String(value ?? '')}</span>
                        </div>
                      ))}
                    </div>
                  </CollapsiblePanel>
                ))}
              </>
            ) : (
              <div className="empty-state">
                <p>No agents defined for this workflow</p>
                <p className="empty-state-hint">Add agents to the workflow YAML file to see them here</p>
              </div>
            )}
          </div>
        )}

        {activeTab === 'details' && (
          <div className="details-tab">
            {workflow.metadata && typeof workflow.metadata === 'object' && !Array.isArray(workflow.metadata) && Object.keys(workflow.metadata).length > 0 && (
              <CollapsiblePanel title="Metadata" defaultExpanded={true}>
                <div className="metadata-grid">
                  {Object.entries(workflow.metadata).map(([key, value]) => (
                    <div key={key} className="detail-item">
                      <span className="detail-label">{key.replace(/_/g, ' ')}:</span>
                      <span className="detail-value">{String(value ?? '')}</span>
                    </div>
                  ))}
                </div>
              </CollapsiblePanel>
            )}

            {workflow.yamlPath && (
              <CollapsiblePanel title="YAML Path">
                <div className="detail-item">
                  <span className="detail-value code-text">{workflow.yamlPath}</span>
                </div>
              </CollapsiblePanel>
            )}

            {workflow.yamlContent && (
              <CollapsiblePanel title="YAML Content">
                <pre className="yaml-content">{workflow.yamlContent}</pre>
              </CollapsiblePanel>
            )}

            {metrics && (
              <>
                <CollapsiblePanel title="Execution Info">
                  <div className="metrics-grid">
                    {metrics.executionId && (
                      <div className="detail-item">
                        <span className="detail-label">Execution ID:</span>
                        <span className="detail-value code-text">{metrics.executionId}</span>
                      </div>
                    )}
                    {metrics.startTime && (
                      <div className="detail-item">
                        <span className="detail-label">Start Time:</span>
                        <span className="detail-value">
                          {(() => {
                            try {
                              const date = new Date(metrics.startTime);
                              return isNaN(date.getTime()) ? String(metrics.startTime) : date.toLocaleString();
                            } catch {
                              return String(metrics.startTime);
                            }
                          })()}
                        </span>
                      </div>
                    )}
                    {metrics.endTime && (
                      <div className="detail-item">
                        <span className="detail-label">End Time:</span>
                        <span className="detail-value">
                          {(() => {
                            try {
                              const date = new Date(metrics.endTime);
                              return isNaN(date.getTime()) ? String(metrics.endTime) : date.toLocaleString();
                            } catch {
                              return String(metrics.endTime);
                            }
                          })()}
                        </span>
                      </div>
                    )}
                  </div>
                </CollapsiblePanel>

                {metrics.latestAnalysis && (
                  <CollapsiblePanel title="Latest Analysis">
                    <div className="analysis-details">
                      <div className="detail-item">
                        <span className="detail-label">Completion:</span>
                        <span className="detail-value">{formatPercent(metrics.latestAnalysis.completionPercent)}</span>
                      </div>
                      <div className="detail-item">
                        <span className="detail-label">Confidence:</span>
                        <span className="detail-value">{formatPercent(metrics.latestAnalysis.confidence)}</span>
                      </div>
                      <div className="detail-item">
                        <span className="detail-label">Needs Re-alignment:</span>
                        <span className="detail-value">{metrics.latestAnalysis.needsReAlignment ? 'Yes' : 'No'}</span>
                      </div>
                      {metrics.latestAnalysis.recommendedAgent && (
                        <div className="detail-item">
                          <span className="detail-label">Recommended Agent:</span>
                          <span className="detail-value">{metrics.latestAnalysis.recommendedAgent}</span>
                        </div>
                      )}
                      {metrics.latestAnalysis.recommendedModel && (
                        <div className="detail-item">
                          <span className="detail-label">Recommended Model:</span>
                          <span className="detail-value">{metrics.latestAnalysis.recommendedModel}</span>
                        </div>
                      )}
                      {metrics.latestAnalysis.issues && Array.isArray(metrics.latestAnalysis.issues) && metrics.latestAnalysis.issues.length > 0 && (
                        <div className="detail-item">
                          <span className="detail-label">Issues:</span>
                          <ul className="detail-list">
                            {metrics.latestAnalysis.issues
                              .filter((issue): issue is string => typeof issue === 'string')
                              .map((issue, i) => (
                              <li key={i}>{issue}</li>
                            ))}
                          </ul>
                        </div>
                      )}
                      {metrics.latestAnalysis.suggestions && Array.isArray(metrics.latestAnalysis.suggestions) && metrics.latestAnalysis.suggestions.length > 0 && (
                        <div className="detail-item">
                          <span className="detail-label">Suggestions:</span>
                          <ul className="detail-list">
                            {metrics.latestAnalysis.suggestions
                              .filter((suggestion): suggestion is string => typeof suggestion === 'string')
                              .map((suggestion, i) => (
                              <li key={i}>{suggestion}</li>
                            ))}
                          </ul>
                        </div>
                      )}
                    </div>
                  </CollapsiblePanel>
                )}
              </>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

// Collapsible Panel Component
interface CollapsiblePanelProps {
  title: string | React.ReactNode;
  children: React.ReactNode;
  defaultExpanded?: boolean;
  onToggle?: () => void;
}

function CollapsiblePanel({ title, children, defaultExpanded = false, onToggle }: CollapsiblePanelProps) {
  const [isExpanded, setIsExpanded] = useState(defaultExpanded);

  const handleToggle = () => {
    setIsExpanded(!isExpanded);
    onToggle?.();
  };

  return (
    <div className="collapsible-panel">
      <button className="panel-header-btn" onClick={handleToggle}>
        <span className="panel-chevron">{isExpanded ? '▼' : '▶'}</span>
        <span className="panel-title">{title}</span>
      </button>
      {isExpanded && (
        <div className="panel-content">
          {children}
        </div>
      )}
    </div>
  );
}

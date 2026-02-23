import type { WorkflowDefinition } from '../../types/workflow';
import type { DashboardMetrics } from '../../types/metrics';
import WorkflowCard from './WorkflowCard';
import './WorkflowList.css';

interface WorkflowListProps {
  workflows: WorkflowDefinition[];
  metrics: Map<string, DashboardMetrics>;
}

export default function WorkflowList({ workflows, metrics }: WorkflowListProps) {
  return (
    <div className="workflow-list">
      <h2>Active Workflows</h2>
      {workflows.length === 0 ? (
        <div className="no-workflows">No workflows available</div>
      ) : (
        <div className="workflow-grid">
          {workflows.map(workflow => (
            <WorkflowCard
              key={workflow.name}
              workflow={workflow}
              metrics={metrics.get(workflow.name)}
            />
          ))}
        </div>
      )}
    </div>
  );
}

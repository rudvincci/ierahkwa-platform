/**
 * Activity Tracker
 * 
 * Tracks real-time activity of cursor-agent and workflow execution
 */

export interface ActivityEvent {
  timestamp: Date;
  type: 'file_read' | 'file_write' | 'file_edit' | 'command' | 'thinking' | 'analysis' | 'completion';
  stepName: string;
  workflowName: string;
  details: string;
  metadata?: Record<string, any>;
}

export class ActivityTracker {
  private activities: Map<string, ActivityEvent[]> = new Map(); // workflowName -> activities
  private currentActivity: Map<string, ActivityEvent> = new Map(); // workflowName -> current activity
  private maxActivitiesPerWorkflow: number = 50; // Reduced from 200 to prevent memory issues

  /**
   * Record an activity event
   */
  recordActivity(
    workflowName: string,
    stepName: string,
    type: ActivityEvent['type'],
    details: string,
    metadata?: Record<string, any>
  ): void {
    const event: ActivityEvent = {
      timestamp: new Date(),
      type,
      stepName,
      workflowName,
      details,
      metadata,
    };

    // Get or create activities list for this workflow
    if (!this.activities.has(workflowName)) {
      this.activities.set(workflowName, []);
    }
    const workflowActivities = this.activities.get(workflowName)!;

    // Add activity
    workflowActivities.push(event);

    // Limit activities per workflow
    if (workflowActivities.length > this.maxActivitiesPerWorkflow) {
      workflowActivities.shift(); // Remove oldest
    }

    // Update current activity
    this.currentActivity.set(workflowName, event);
  }

  /**
   * Get current activity for a workflow
   */
  getCurrentActivity(workflowName: string): ActivityEvent | null {
    return this.currentActivity.get(workflowName) || null;
  }

  /**
   * Get recent activities for a workflow
   */
  getRecentActivities(workflowName: string, limit: number = 50): ActivityEvent[] {
    const activities = this.activities.get(workflowName) || [];
    return activities.slice(-limit);
  }

  /**
   * Clear activities for a workflow
   */
  clearWorkflow(workflowName: string): void {
    this.activities.delete(workflowName);
    this.currentActivity.delete(workflowName);
  }

  /**
   * Parse cursor-agent output for activity hints
   */
  parseOutputForActivity(output: string, workflowName: string, stepName: string): void {
    // Look for file operations
    const fileReadPattern = /(?:reading|reading file|opening|accessing|checking)\s+['"]?([^\s'"]+\.(?:cs|ts|js|tsx|jsx|md|json|yml|yaml))/gi;
    const fileWritePattern = /(?:writing|writing to|creating|updating|editing|modifying)\s+['"]?([^\s'"]+\.(?:cs|ts|js|tsx|jsx|md|json|yml|yaml))/gi;
    const commandPattern = /(?:running|executing|command):\s*(.+)/gi;
    const thinkingPattern = /(?:thinking|analyzing|considering|evaluating):\s*(.+)/gi;

    // Extract file reads
    let match;
    while ((match = fileReadPattern.exec(output)) !== null) {
      this.recordActivity(workflowName, stepName, 'file_read', `Reading file: ${match[1]}`, {
        file: match[1],
      });
    }

    // Extract file writes/edits
    while ((match = fileWritePattern.exec(output)) !== null) {
      this.recordActivity(workflowName, stepName, 'file_write', `Writing to file: ${match[1]}`, {
        file: match[1],
      });
    }

    // Extract commands
    while ((match = commandPattern.exec(output)) !== null) {
      this.recordActivity(workflowName, stepName, 'command', `Running: ${match[1]}`, {
        command: match[1],
      });
    }

    // Extract thinking/analysis
    while ((match = thinkingPattern.exec(output)) !== null) {
      this.recordActivity(workflowName, stepName, 'thinking', match[1]);
    }
  }

  /**
   * Get all workflows with activities
   */
  getActiveWorkflows(): string[] {
    return Array.from(this.currentActivity.keys());
  }
}

/**
 * Output Monitor
 * 
 * Monitors cursor-agent output and re-aligns agents based on progress
 */

import { AgentResult } from '../workflow/OrchestratorContext';
import { AgentTask } from '../domain/AgentTask';
import { AgentRole } from '../domain/AgentRole';

export interface OutputAnalysis {
  stepName: string;
  workflowName: string;
  completionPercent: number; // Estimated completion percentage
  issues: string[];
  suggestions: string[];
  needsReAlignment: boolean;
  recommendedAgent?: string | AgentRole;
  recommendedModel?: string;
  confidence: number; // 0-1 confidence in analysis
}

export interface ReAlignmentRecommendation {
  stepName: string;
  currentAgent: string | AgentRole;
  recommendedAgent: string | AgentRole;
  reason: string;
  confidence: number;
  estimatedImprovement: string; // e.g., "30% faster", "better accuracy"
}

export class OutputMonitor {
  private analyses: Map<string, OutputAnalysis[]> = new Map(); // workflowName -> analyses
  private maxAnalysesPerWorkflow: number = 20; // Reduced from 100 to prevent memory issues

  /**
   * Analyze output and determine if re-alignment is needed
   */
  analyzeOutput(
    workflowName: string,
    stepName: string,
    result: AgentResult,
    task: AgentTask
  ): OutputAnalysis {
    const analysis: OutputAnalysis = {
      stepName,
      workflowName,
      completionPercent: 0,
      issues: [],
      suggestions: [],
      needsReAlignment: false,
      confidence: 0.5,
    };

    // Analyze result content
    const content = result.rawOutput || result.summary || '';
    const summary = result.summary || '';

    // Check for completion indicators
    const completionIndicators = [
      /completed successfully/i,
      /finished/i,
      /done/i,
      /all requirements met/i,
      /task complete/i,
      /100%/i,
    ];

    const incompleteIndicators = [
      /incomplete/i,
      /not finished/i,
      /still working/i,
      /needs more/i,
      /partial/i,
      /error/i,
      /failed/i,
      /issue/i,
      /problem/i,
    ];

    let completionScore = 0;
    for (const indicator of completionIndicators) {
      if (indicator.test(content) || indicator.test(summary)) {
        completionScore += 0.2;
      }
    }

    for (const indicator of incompleteIndicators) {
      if (indicator.test(content) || indicator.test(summary)) {
        completionScore -= 0.1;
      }
    }

    analysis.completionPercent = Math.max(0, Math.min(100, completionScore * 100));

    // Detect issues
    if (!result.success) {
      analysis.issues.push('Task resulted in an error');
      analysis.completionPercent = Math.min(analysis.completionPercent, 50);
    }

    if (content.length < 100 && result.success) {
      analysis.issues.push('Output seems incomplete or minimal');
    }

    // Check for specific patterns that suggest re-alignment
    const needsDifferentAgentPatterns = [
      /need.*architect/i,
      /should.*backend/i,
      /requires.*frontend/i,
      /needs.*devops/i,
      /should.*security/i,
    ];

    const currentAgentName = typeof task.role === 'string' ? task.role : task.role.name || '';
    
    for (const pattern of needsDifferentAgentPatterns) {
      if (pattern.test(content) || pattern.test(summary)) {
        analysis.needsReAlignment = true;
        analysis.suggestions.push('Output suggests a different agent role might be more appropriate');
        
        // Try to infer recommended agent
        if (/architect/i.test(content)) {
          analysis.recommendedAgent = 'Architect';
        } else if (/backend/i.test(content)) {
          analysis.recommendedAgent = 'Backend';
        } else if (/frontend/i.test(content)) {
          analysis.recommendedAgent = 'Frontend';
        }
        break;
      }
    }

    // Check if output suggests model change
    if (content.length > 50000) {
      analysis.suggestions.push('Large output detected, consider using a model with larger context window');
      analysis.recommendedModel = 'claude-4-5-sonnet';
    }

    // Calculate confidence
    if (analysis.issues.length > 0 || analysis.needsReAlignment) {
      analysis.confidence = 0.7;
    } else if (analysis.completionPercent > 80) {
      analysis.confidence = 0.9;
    } else {
      analysis.confidence = 0.5;
    }

    // Record analysis
    this.recordAnalysis(workflowName, analysis);

    return analysis;
  }

  /**
   * Get re-alignment recommendations
   */
  getReAlignmentRecommendations(workflowName: string): ReAlignmentRecommendation[] {
    const analyses = this.analyses.get(workflowName) || [];
    const recommendations: ReAlignmentRecommendation[] = [];

    for (const analysis of analyses) {
      if (analysis.needsReAlignment && analysis.recommendedAgent) {
        recommendations.push({
          stepName: analysis.stepName,
          currentAgent: 'current', // Would need actual current agent
          recommendedAgent: analysis.recommendedAgent,
          reason: analysis.suggestions.join('; '),
          confidence: analysis.confidence,
          estimatedImprovement: 'Improved task completion',
        });
      }
    }

    return recommendations;
  }

  /**
   * Record analysis
   */
  private recordAnalysis(workflowName: string, analysis: OutputAnalysis): void {
    if (!this.analyses.has(workflowName)) {
      this.analyses.set(workflowName, []);
    }
    const workflowAnalyses = this.analyses.get(workflowName)!;
    workflowAnalyses.push(analysis);

    // Limit analyses per workflow
    if (workflowAnalyses.length > this.maxAnalysesPerWorkflow) {
      workflowAnalyses.shift();
    }
  }

  /**
   * Get latest analysis for a step
   */
  getLatestAnalysis(workflowName: string, stepName: string): OutputAnalysis | null {
    const analyses = this.analyses.get(workflowName) || [];
    const stepAnalyses = analyses.filter(a => a.stepName === stepName);
    return stepAnalyses.length > 0 ? stepAnalyses[stepAnalyses.length - 1] : null;
  }

  /**
   * Get all analyses for a workflow
   */
  getAnalyses(workflowName: string): OutputAnalysis[] {
    return this.analyses.get(workflowName) || [];
  }

  /**
   * Clear analyses for a workflow
   */
  clearWorkflow(workflowName: string): void {
    this.analyses.delete(workflowName);
  }
}

/**
 * Type definitions for Orchestrator Manager Agent
 */

import { WorkflowDefinition, StepDefinition } from './WorkflowDefinition';

export interface ProjectAnalysis {
  structure: {
    services: string[];
    domains: string[];
    technologies: string[];
    patterns: string[];
  };
  workflows: {
    existing: string[];
    suggested: WorkflowSuggestion[];
  };
  complexity: 'low' | 'medium' | 'high';
}

export interface WorkflowSuggestion {
  name: string;
  description: string;
  confidence: 'low' | 'medium' | 'high';
  template?: string;
  steps: StepSuggestion[];
}

export interface StepSuggestion {
  name: string;
  agent: string;
  description: string;
  dependsOn?: string[];
  parallel?: boolean;
}

export interface OptimizationReport {
  workflowName: string;
  optimizations: Optimization[];
  metrics: {
    avgExecutionTime: number;
    successRate: number;
    failureRate: number;
  };
}

export interface Optimization {
  type: 'parallel' | 'dependency' | 'retry' | 'skip';
  description: string;
  step?: string;
  impact: 'low' | 'medium' | 'high';
  suggestedChange: any;
}

export interface LearnedPattern {
  pattern: string;
  successRate: number;
  avgExecutionTime: number;
  commonIssues: string[];
  bestPractices: string[];
}

export interface WorkflowTemplate {
  name: string;
  description: string;
  category: string;
  workflow: WorkflowDefinition;
  tags: string[];
}

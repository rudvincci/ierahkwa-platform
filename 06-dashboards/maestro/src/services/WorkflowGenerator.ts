/**
 * Workflow Generator Service
 * 
 * Uses AI to generate workflow definitions from natural language prompts
 */

import { spawn } from 'child_process';
import * as path from 'path';
import * as fs from 'fs';
import { findRepoRoot } from '../cli/utils';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';
import * as yaml from 'js-yaml';

export interface GenerateWorkflowRequest {
  prompt: string;
  context?: {
    existingWorkflows?: string[];
    availableRoles?: string[];
  };
}

export interface GeneratedWorkflow {
  name: string;
  description: string;
  steps: Array<{
    name: string;
    agent: string;
    description: string;
    dependsOn?: string[];
  }>;
  reasoning?: string; // AI's reasoning for the workflow structure
}

export class WorkflowGenerator {
  private repoRoot: string;

  constructor() {
    this.repoRoot = findRepoRoot();
  }

  /**
   * Generate workflow from natural language prompt using AI
   */
  async generateWorkflow(request: GenerateWorkflowRequest): Promise<{ success: boolean; workflow?: GeneratedWorkflow; error?: string }> {
    try {
      // Build the prompt for the AI
      const systemPrompt = this.buildSystemPrompt(request.context);
      const userPrompt = request.prompt;

      // Use cursor-agent to generate the workflow
      const workflowYaml = await this.callCursorAgent(systemPrompt, userPrompt);

      if (!workflowYaml) {
        return { success: false, error: 'Failed to generate workflow from AI' };
      }

      // Parse the generated YAML
      const parsed = this.parseGeneratedWorkflow(workflowYaml);

      if (!parsed) {
        return { success: false, error: 'Failed to parse generated workflow. Please try rephrasing your prompt.' };
      }

      return { success: true, workflow: parsed };
    } catch (error: any) {
      console.error('Error generating workflow:', error);
      return { success: false, error: error.message || 'Unknown error generating workflow' };
    }
  }

  /**
   * Build system prompt for AI
   */
  private buildSystemPrompt(context?: GenerateWorkflowRequest['context']): string {
    const availableRoles = context?.availableRoles || [
      'Architect', 'Backend', 'Frontend', 'DevOps', 'Security', 'QA', 'Documentation'
    ];

    return `You are a workflow generation assistant. Your task is to create a workflow definition in YAML format based on a user's natural language description.

A workflow consists of:
1. A name (lowercase with hyphens, e.g., "code-review-workflow")
2. A description (what the workflow accomplishes)
3. Steps (array of tasks, each with):
   - name: unique identifier (lowercase with underscores)
   - agent: one of the available roles: ${availableRoles.join(', ')}
   - description: detailed task description
   - dependsOn: optional array of step names that must complete first

Available agent roles:
${availableRoles.map(r => `- ${r}: Specialized in ${this.getRoleDescription(r)}`).join('\n')}

Output format (YAML):
\`\`\`yaml
name: workflow-name
description: Workflow description
steps:
  - name: step_name
    agent: RoleName
    description: Detailed step description
    dependsOn: []  # Optional: list of step names
  - name: another_step
    agent: RoleName
    description: Another step description
    dependsOn: [step_name]  # This step depends on step_name completing first
\`\`\`

Guidelines:
- Break down complex tasks into logical steps
- Use appropriate agent roles for each step
- Define dependencies when steps must run in sequence
- Make step descriptions clear and actionable
- Ensure step names are unique and descriptive

${context?.existingWorkflows && context.existingWorkflows.length > 0 
  ? `\nExisting workflows for reference: ${context.existingWorkflows.join(', ')}`
  : ''}

Respond ONLY with valid YAML. Do not include markdown code fences or explanations outside the YAML.`;
  }

  /**
   * Get role description
   */
  private getRoleDescription(role: string): string {
    const descriptions: Record<string, string> = {
      'Architect': 'system design, architecture decisions, technical planning',
      'Backend': 'server-side code, APIs, databases, business logic',
      'Frontend': 'user interfaces, client-side code, UX/UI',
      'DevOps': 'infrastructure, deployment, CI/CD, monitoring',
      'Security': 'security analysis, vulnerability assessment, compliance',
      'QA': 'testing, quality assurance, test automation',
      'Documentation': 'documentation, technical writing, guides',
    };
    return descriptions[role] || 'general tasks';
  }

  /**
   * Call cursor-agent to generate workflow
   */
  private async callCursorAgent(systemPrompt: string, userPrompt: string): Promise<string | null> {
    return new Promise((resolve, reject) => {
      const fullPrompt = `${systemPrompt}\n\nUser Request:\n${userPrompt}\n\nGenerate the workflow YAML:`;

      // Find cursor-agent command
      const cursorAgentCmd = this.findCursorAgent();

      if (!cursorAgentCmd) {
        reject(new Error('cursor-agent command not found. Please ensure Cursor CLI is installed.'));
        return;
      }

      const child = spawn(cursorAgentCmd, ['-p', '--output-format', 'json'], {
        cwd: this.repoRoot,
        env: { ...process.env },
      });

      let stdout = '';
      let stderr = '';

      // Send prompt via stdin
      child.stdin?.write(fullPrompt);
      child.stdin?.end();

      child.stdout?.on('data', (data) => {
        stdout += data.toString();
      });

      child.stderr?.on('data', (data) => {
        stderr += data.toString();
      });

      child.on('exit', (code) => {
        if (code !== 0) {
          console.error('cursor-agent error:', stderr);
          reject(new Error(`cursor-agent exited with code ${code}: ${stderr}`));
          return;
        }

        try {
          // Parse JSON output from cursor-agent
          const result = JSON.parse(stdout);
          
          // Extract YAML from the response
          // cursor-agent may return the YAML in different formats
          let yamlContent = '';
          
          if (result.content) {
            yamlContent = result.content;
          } else if (result.text) {
            yamlContent = result.text;
          } else if (typeof result === 'string') {
            yamlContent = result;
          } else {
            // Try to find YAML in the response
            const responseStr = JSON.stringify(result);
            const yamlMatch = responseStr.match(/```yaml\n([\s\S]*?)\n```/) || 
                             responseStr.match(/```\n([\s\S]*?)\n```/);
            if (yamlMatch) {
              yamlContent = yamlMatch[1];
            } else {
              yamlContent = responseStr;
            }
          }

          // Clean up the YAML (remove markdown code fences if present)
          yamlContent = yamlContent
            .replace(/```yaml\n?/g, '')
            .replace(/```\n?/g, '')
            .trim();

          resolve(yamlContent);
        } catch (error) {
          // If JSON parsing fails, try to extract YAML directly
          const yamlMatch = stdout.match(/```yaml\n([\s\S]*?)\n```/) || 
                           stdout.match(/```\n([\s\S]*?)\n```/) ||
                           stdout.match(/^name:/m);
          
          if (yamlMatch) {
            let yamlContent = yamlMatch[1] || stdout;
            yamlContent = yamlContent
              .replace(/```yaml\n?/g, '')
              .replace(/```\n?/g, '')
              .trim();
            resolve(yamlContent);
          } else {
            reject(new Error('Could not extract YAML from AI response'));
          }
        }
      });

      child.on('error', (error) => {
        reject(new Error(`Failed to execute cursor-agent: ${error.message}`));
      });

      // Timeout after 60 seconds
      setTimeout(() => {
        child.kill();
        reject(new Error('Workflow generation timed out'));
      }, 60000);
    });
  }

  /**
   * Find cursor-agent command
   */
  private findCursorAgent(): string | null {
    // Try common locations
    const possibleCommands = [
      'cursor-agent',
      'cursor',
      path.join(process.env.HOME || '', '.cursor', 'bin', 'cursor-agent'),
    ];

    for (const cmd of possibleCommands) {
      try {
        // Check if command exists (simple check)
        if (fs.existsSync(cmd) || this.commandExists(cmd)) {
          return cmd;
        }
      } catch {
        // Continue to next
      }
    }

    return null;
  }

  /**
   * Check if command exists (simple check)
   */
  private commandExists(cmd: string): boolean {
    // Simple heuristic - if it's in PATH, assume it exists
    // In production, you might want to use `which` command
    return !cmd.includes('/') || fs.existsSync(cmd);
  }

  /**
   * Parse generated YAML into workflow structure
   */
  private parseGeneratedWorkflow(yamlContent: string): GeneratedWorkflow | null {
    try {
      const parsed = yaml.load(yamlContent) as any;

      if (!parsed || typeof parsed !== 'object') {
        return null;
      }

      // Validate and extract workflow structure
      const workflow: GeneratedWorkflow = {
        name: parsed.name || 'generated-workflow',
        description: parsed.description || '',
        steps: [],
      };

      if (parsed.steps && Array.isArray(parsed.steps)) {
        workflow.steps = parsed.steps.map((step: any) => ({
          name: step.name || `step_${workflow.steps.length + 1}`,
          agent: step.agent || 'Backend',
          description: step.description || '',
          dependsOn: step.dependsOn || [],
        }));
      }

      if (parsed.reasoning) {
        workflow.reasoning = parsed.reasoning;
      }

      return workflow;
    } catch (error) {
      console.error('Error parsing generated workflow:', error);
      return null;
    }
  }
}

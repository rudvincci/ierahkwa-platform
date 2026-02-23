import { AgentTask } from '../domain/AgentTask';
import { OrchestratorContext } from '../workflow/OrchestratorContext';
import { AgentRole } from '../domain/AgentRole';
import { RulesLoader } from '../services/RulesLoader';

export class PromptBuilder {
  private rulesLoader: RulesLoader;

  constructor(repositoryRoot: string = process.cwd()) {
    this.rulesLoader = new RulesLoader(repositoryRoot);
  }

  async buildPrompt(task: AgentTask, context: OrchestratorContext): Promise<string> {
    const role = typeof task.role === 'string' ? null : task.role;
    const roleName = typeof task.role === 'string' ? task.role : task.role.name;

    let prompt = `You are acting as the ${roleName} agent in the Mamey Framework workspace.\n\n`;

    // Add role description
    if (role?.description) {
      prompt += `## Role Description\n${role.description}\n\n`;
    }

    // Add prompt hints
    if (role?.promptHints) {
      prompt += `## Specific Instructions\n${role.promptHints}\n\n`;
    }

    // Add feature context
    if (context.featureDescription) {
      prompt += `## Feature Context\n${context.featureDescription}\n\n`;
    }

    // Add task description
    prompt += `## Task\n${task.description}\n\n`;

    // Add input files/context
    if (task.inputs && task.inputs.length > 0) {
      prompt += `## Input Files/Context\n`;
      for (const input of task.inputs) {
        prompt += `- ${input}\n`;
      }
      prompt += '\n';
    }

    // Add default file patterns if available
    if (role?.defaultFilePatterns && role.defaultFilePatterns.length > 0) {
      prompt += `## Relevant File Patterns\n`;
      for (const pattern of role.defaultFilePatterns) {
        prompt += `- ${pattern}\n`;
      }
      prompt += '\n';
    }

    // Add default directories if available
    if (role?.defaultDirectories && role.defaultDirectories.length > 0) {
      prompt += `## Relevant Directories\n`;
      for (const dir of role.defaultDirectories) {
        prompt += `- ${dir}\n`;
      }
      prompt += '\n';
    }

    // Include previous step results
    if (context.previousResults.size > 0) {
      prompt += `## Previous Step Results\n`;
      for (const [stepName, result] of context.previousResults.entries()) {
        prompt += `### ${stepName}\n`;
        prompt += `${result.summary}\n\n`;
        if (result.rawOutput) {
          prompt += `Details: ${result.rawOutput.substring(0, 500)}...\n\n`;
        }
      }
    }

    // Add Cursor Rules (CRITICAL - must be included)
    try {
      const rulesContent = await this.rulesLoader.getFormattedRules();
      if (rulesContent) {
        prompt += rulesContent;
      }
    } catch (error) {
      console.warn('Failed to load cursor rules:', error);
      prompt += `## Cursor Rules\n\n`;
      prompt += `**Note**: Rules from .cursor/rules/ directory should be followed.\n`;
      prompt += `Please refer to .cursor/rules/ directory for workspace-specific rules.\n\n`;
    }

    // Add Mamey Framework context
    prompt += `## Mamey Framework Context\n`;
    prompt += `This workspace uses the Mamey Framework with:\n`;
    prompt += `- .NET microservices with CQRS pattern\n`;
    prompt += `- Event-driven architecture\n`;
    prompt += `- Domain-Driven Design (DDD)\n`;
    prompt += `- Multiple domains: Government, Banking, FutureWampum, etc.\n`;
    prompt += `- Both .NET and Rust codebases\n\n`;

    // Add domain knowledge if available
    if (role?.domainKnowledge && role.domainKnowledge.length > 0) {
      prompt += `## Domain Knowledge\n`;
      prompt += `Relevant domains: ${role.domainKnowledge.join(', ')}\n\n`;
    }

    // Request JSON response
    prompt += `## Response Format\n`;
    prompt += `Please respond with a JSON object containing:\n`;
    prompt += `{\n`;
    prompt += `  "success": boolean,\n`;
    prompt += `  "summary": "Brief summary of what was accomplished",\n`;
    prompt += `  "details": "Optional detailed information about changes made"\n`;
    prompt += `}\n`;

    return prompt;
  }

  /**
   * Get rules summary (for verbose output)
   */
  async getRulesSummary(): Promise<string[]> {
    return await this.rulesLoader.getRulesSummary();
  }
}


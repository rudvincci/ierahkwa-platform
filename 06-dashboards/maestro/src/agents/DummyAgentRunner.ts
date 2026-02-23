import { IAgentRunner } from './AgentRunner';
import { AgentTask } from '../domain/AgentTask';
import { OrchestratorContext, AgentResult } from '../workflow/OrchestratorContext';

export class DummyAgentRunner implements IAgentRunner {
  async runTask(task: AgentTask, context: OrchestratorContext): Promise<AgentResult> {
    const roleName = typeof task.role === 'string' ? task.role : task.role.name;
    const prompt = this.buildPrompt(task, context);

    console.log(`\n[DUMMY RUNNER] Would execute task: ${task.stepName}`);
    console.log(`Role: ${roleName}`);
    console.log(`Description: ${task.description}`);
    console.log(`\nPrompt that would be sent:\n${prompt}\n`);

    // Simulate some processing time
    await new Promise((resolve) => setTimeout(resolve, 100));

    return {
      success: true,
      summary: `[DUMMY] Successfully completed ${task.stepName}`,
      rawOutput: `Dummy execution result for ${task.stepName}`,
    };
  }

  private buildPrompt(task: AgentTask, context: OrchestratorContext): string {
    const roleName = typeof task.role === 'string' ? task.role : task.role.name;
    const roleDescription = typeof task.role === 'string' ? '' : task.role.description;
    const promptHints = typeof task.role === 'string' ? '' : task.role.promptHints || '';

    let prompt = `You are acting as the ${roleName} agent.\n\n`;

    if (roleDescription) {
      prompt += `Role Description: ${roleDescription}\n\n`;
    }

    if (promptHints) {
      prompt += `Specific Instructions:\n${promptHints}\n\n`;
    }

    if (context.featureDescription) {
      prompt += `Feature Context: ${context.featureDescription}\n\n`;
    }

    prompt += `Task: ${task.description}\n\n`;

    if (task.inputs && task.inputs.length > 0) {
      prompt += `Input Files/Context:\n${task.inputs.map((i) => `- ${i}`).join('\n')}\n\n`;
    }

    // Include previous step results
    if (context.previousResults.size > 0) {
      prompt += `Previous Step Results:\n`;
      for (const [stepName, result] of context.previousResults.entries()) {
        prompt += `- ${stepName}: ${result.summary}\n`;
      }
      prompt += '\n';
    }

    prompt += `Please respond with a JSON object containing:\n`;
    prompt += `{\n`;
    prompt += `  "success": boolean,\n`;
    prompt += `  "summary": "Brief summary of what was accomplished",\n`;
    prompt += `  "details": "Optional detailed information"\n`;
    prompt += `}\n`;

    return prompt;
  }
}


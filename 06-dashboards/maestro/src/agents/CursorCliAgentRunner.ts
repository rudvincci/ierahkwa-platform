import { exec, spawn } from 'child_process';
import { promisify } from 'util';
import { IAgentRunner } from './AgentRunner';
import { AgentTask } from '../domain/AgentTask';
import { OrchestratorContext, AgentResult } from '../workflow/OrchestratorContext';
import { PromptBuilder } from './PromptBuilder';
import { ActivityTracker } from '../services/ActivityTracker';

const execAsync = promisify(exec);

export interface CursorCliOptions {
  model?: string;
  streamPartialOutput?: boolean;
  timeout?: number;
  verbose?: boolean;
  activityTracker?: ActivityTracker; // Optional activity tracker
  workflowName?: string; // Workflow name for activity tracking
  cursorAgentGuid?: string; // GUID for cursor-agent --resume to maintain context
  onTokenUsage?: (usage: { inputTokens: number; outputTokens: number; totalTokens: number; contextWindowSize: number; contextWindowUsed: number; contextWindowPercent: number; model: string }) => void; // Token usage callback
}

export class CursorCliAgentRunner implements IAgentRunner {
  private promptBuilder: PromptBuilder;
  private options: CursorCliOptions;
  private activityTracker?: ActivityTracker;
  private workflowName?: string;
  private onTokenUsage?: CursorCliOptions['onTokenUsage'];

  constructor(options: CursorCliOptions = {}, repositoryRoot?: string) {
    this.promptBuilder = new PromptBuilder(repositoryRoot);
    this.options = {
      timeout: 1800000, // 30 minutes default (increased for complex analysis tasks)
      verbose: false,
      ...options,
    };
    this.activityTracker = options.activityTracker;
    this.workflowName = options.workflowName;
    this.onTokenUsage = options.onTokenUsage;
  }

  async runTask(task: AgentTask, context: OrchestratorContext): Promise<AgentResult> {
    const prompt = await this.promptBuilder.buildPrompt(task, context);
    const startTime = Date.now();

    // Build command (returns command, prompt, and temp file path)
    const { command, prompt: promptForStdin, tempFile } = this.buildCommand(prompt);

    console.log(`\n${'='.repeat(80)}`);
    console.log(`🔄 [TASK] ${task.stepName}`);
    console.log(`📋 Role: ${typeof task.role === 'string' ? task.role : task.role.name || 'Unknown'}`);
    console.log(`⏱️  Timeout: ${Math.round(this.options.timeout! / 60000)} minutes`);
    console.log(`${'='.repeat(80)}`);

    if (this.options.verbose) {
      console.log(`\n${'─'.repeat(80)}`);
      console.log(`📝 PROMPT DETAILS`);
      console.log(`${'─'.repeat(80)}`);
      console.log(`\n💻 Command: ${command}`);
      console.log(`📄 Temp file: ${tempFile || 'N/A'}`);
      console.log(`\n📝 Full Prompt (${prompt.length} chars):`);
      console.log(`${'─'.repeat(80)}`);
      console.log(prompt);
      console.log(`${'─'.repeat(80)}`);
      
      // Show rules summary
      try {
        const rulesSummary = await this.promptBuilder.getRulesSummary();
        if (rulesSummary.length > 0) {
          console.log(`\n📋 Loaded ${rulesSummary.length} rule file(s) from .cursor/rules/`);
          if (rulesSummary.length <= 10) {
            rulesSummary.forEach(rule => console.log(`   - ${rule}`));
          } else {
            rulesSummary.slice(0, 10).forEach(rule => console.log(`   - ${rule}`));
            console.log(`   ... and ${rulesSummary.length - 10} more`);
          }
        }
      } catch (error) {
        // Ignore errors in verbose output
      }
      console.log(`\n${'─'.repeat(80)}\n`);
    }

    // Show progress indicator
    const progressInterval = this.showProgress(task.stepName, startTime);
    let intervalCleared = false;

    try {
      // Use streaming execution for better visibility
      const output = await this.executeWithStreaming(command, tempFile || '', context.repositoryRoot, task);

      if (!intervalCleared) {
        clearInterval(progressInterval);
        intervalCleared = true;
      }

      // Clear progress line and show completion
      process.stdout.write('\r' + ' '.repeat(80) + '\r'); // Clear line
      const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
      console.log(`✅ ${task.stepName} completed (${elapsed}s)`);

      // Parse JSON output
      const result = this.parseOutput(output);

      // Extract and report token usage
      this.extractAndReportTokenUsage(output, task.stepName);

      if (this.options.verbose) {
        console.log(`\n${'─'.repeat(80)}`);
        console.log(`📊 TASK RESULT`);
        console.log(`${'─'.repeat(80)}`);
        console.log(`Success: ${result.success}`);
        console.log(`Summary: ${result.summary || 'N/A'}`);
        
        if (result.details) {
          console.log(`\n📄 Details:`);
          console.log(result.details);
        }
        
        // Show full raw output in verbose mode
        if (output) {
          console.log(`\n📄 Raw Output (${output.length} chars):`);
          console.log(`${'─'.repeat(80)}`);
          // Show first 2000 chars, or full output if less
          if (output.length > 2000) {
            console.log(output.substring(0, 2000));
            console.log(`\n... (truncated, ${output.length - 2000} more chars)`);
          } else {
            console.log(output);
          }
          console.log(`${'─'.repeat(80)}`);
        }
      }

      return {
        success: result.success,
        summary: result.summary || 'Task completed',
        rawOutput: output,
      };
    } catch (error: any) {
      if (!intervalCleared) {
        clearInterval(progressInterval);
        intervalCleared = true;
      }
      // Clear progress line and show failure
      process.stdout.write('\r' + ' '.repeat(80) + '\r'); // Clear line
      const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
      const errorMessage = error.message || String(error);
      
      console.log(`❌ ${task.stepName} failed (${elapsed}s)`);
      console.error(`   Error: ${errorMessage}`);

      if (this.options.verbose) {
        console.log(`\n${'─'.repeat(80)}`);
        console.log(`❌ ERROR DETAILS`);
        console.log(`${'─'.repeat(80)}`);
        console.log(`Error Message: ${errorMessage}`);
        
        if (error.stdout) {
          console.log(`\n📄 Stdout (${error.stdout.length} chars):`);
          console.log(`${'─'.repeat(80)}`);
          if (error.stdout.length > 2000) {
            console.log(error.stdout.slice(-2000));
            console.log(`\n... (showing last 2000 chars of ${error.stdout.length} total)`);
          } else {
            console.log(error.stdout);
          }
          console.log(`${'─'.repeat(80)}`);
        }
        
        if (error.stderr) {
          console.log(`\n⚠️  Stderr (${error.stderr.length} chars):`);
          console.log(`${'─'.repeat(80)}`);
          if (error.stderr.length > 2000) {
            console.log(error.stderr.slice(-2000));
            console.log(`\n... (showing last 2000 chars of ${error.stderr.length} total)`);
          } else {
            console.log(error.stderr);
          }
          console.log(`${'─'.repeat(80)}`);
        }
      }

      return {
        success: false,
        summary: `Task failed: ${errorMessage}`,
        error: errorMessage,
        rawOutput: error.stdout || error.stderr || '',
      };
    }
  }

  private showProgress(taskName: string, startTime: number): NodeJS.Timeout {
    const dots = ['.', '..', '...'];
    let dotIndex = 0;
    let lastUpdate = 0;

    return setInterval(() => {
      const now = Date.now();
      const elapsed = Math.floor((now - startTime) / 1000);
      
      // Only update every second to avoid flickering
      if (now - lastUpdate >= 1000) {
        const minutes = Math.floor(elapsed / 60);
        const seconds = elapsed % 60;
        const timeStr = minutes > 0 ? `${minutes}m ${seconds}s` : `${seconds}s`;
        
        // Use single-line format that overwrites
        process.stdout.write(`\r🔄 ${taskName} ${dots[dotIndex].padEnd(3)} [${timeStr.padStart(8)}]`);
        dotIndex = (dotIndex + 1) % dots.length;
        lastUpdate = now;
      }
    }, 1000);
  }

  private async executeWithStreaming(command: string, tempFile: string | undefined, cwd: string, task: AgentTask): Promise<string> {
    return new Promise((resolve, reject) => {
      const fs = require('fs');
      
      // Parse command into parts
      const parts = this.parseCommand(command);
      const [cmd, ...args] = parts;

      const child = spawn(cmd, args, {
        cwd,
        shell: true,
        stdio: ['ignore', 'pipe', 'pipe'],
      });

      // Store PID for graceful termination
      const pid = child.pid;
      if (pid && this.workflowName) {
        // Store PID in a way that can be retrieved later
        // This will be used by the workflow manager for graceful termination
        (child as any).__maestro_pid = pid;
        (child as any).__maestro_workflow = this.workflowName;
        (child as any).__maestro_guid = this.options.cursorAgentGuid;
      }

      let stdout = '';
      let stderr = '';

      // Stream stdout
      child.stdout?.on('data', (data: Buffer) => {
        const chunk = data.toString();
        stdout += chunk;
        
        // Parse activity from output if activity tracker is available
        if (this.activityTracker && this.workflowName) {
          try {
            // Try to parse JSON output for structured activity hints
            if (chunk.trim().startsWith('{') && chunk.includes('"type"')) {
              try {
                const json = JSON.parse(chunk.trim());
                // Extract activity from JSON response
                if (json.type === 'result' && json.summary) {
                  this.activityTracker.recordActivity(
                    this.workflowName,
                    task.stepName,
                    'completion',
                    json.summary.substring(0, 200)
                  );
                } else if (json.details) {
                  this.activityTracker.recordActivity(
                    this.workflowName,
                    task.stepName,
                    'analysis',
                    json.details.substring(0, 200)
                  );
                }
              } catch {
                // Not valid JSON, continue with text parsing
              }
            }
            
            // Parse text output for file operations and activity hints
            this.activityTracker.parseOutputForActivity(chunk, this.workflowName, task.stepName);
          } catch (error) {
            // Silently ignore parsing errors
          }
        }
        
        // In verbose mode, show all output including JSON
        if (this.options.verbose) {
          const isJsonOutput = chunk.trim().startsWith('{') && chunk.includes('"type"');
          
          if (isJsonOutput) {
            // Format JSON output nicely
            try {
              const json = JSON.parse(chunk.trim());
              console.log(`\n📥 [STDOUT] JSON Response:`);
              console.log(JSON.stringify(json, null, 2));
            } catch {
              // If not valid JSON, show raw
              console.log(`\n📥 [STDOUT] Raw JSON:`);
              console.log(chunk);
            }
          } else {
            // Show non-JSON output
            const lines = chunk.split('\n').filter(l => l.trim());
            if (lines.length > 0) {
              console.log(`\n📥 [STDOUT] Output:`);
              lines.forEach(line => {
                console.log(`   ${line}`);
              });
            }
          }
        }
      });

      // Stream stderr
      child.stderr?.on('data', (data: Buffer) => {
        const chunk = data.toString();
        stderr += chunk;
        
        if (this.options.verbose) {
          console.error(`\n⚠️  [STDERR] ${chunk}`);
        }
      });

      // Handle completion
      child.on('close', (code) => {
        // Clean up temp file
        if (tempFile && fs.existsSync(tempFile)) {
          try {
            fs.unlinkSync(tempFile);
          } catch (err) {
            // Ignore cleanup errors
          }
        }
        
        if (code === 0) {
          resolve(stdout);
        } else {
          const error: any = new Error(`Command exited with code ${code}`);
          error.stdout = stdout;
          error.stderr = stderr;
          reject(error);
        }
      });

      // Handle errors
      child.on('error', (error) => {
        // Clean up temp file on error
        if (tempFile && fs.existsSync(tempFile)) {
          try {
            fs.unlinkSync(tempFile);
          } catch (err) {
            // Ignore cleanup errors
          }
        }
        reject(error);
      });

      // Set timeout
      const timeout = setTimeout(() => {
        child.kill();
        reject(new Error(`Command timed out after ${this.options.timeout}ms`));
      }, this.options.timeout);

      // Clear timeout on completion
      child.on('close', () => clearTimeout(timeout));
    });
  }

  private parseCommand(command: string): string[] {
    // Simple command parsing - handles quoted strings
    const parts: string[] = [];
    let current = '';
    let inQuotes = false;
    let quoteChar = '';

    for (let i = 0; i < command.length; i++) {
      const char = command[i];
      
      if ((char === '"' || char === "'") && (i === 0 || command[i - 1] !== '\\')) {
        if (!inQuotes) {
          inQuotes = true;
          quoteChar = char;
        } else if (char === quoteChar) {
          inQuotes = false;
          quoteChar = '';
        } else {
          current += char;
        }
      } else if (char === ' ' && !inQuotes) {
        if (current) {
          parts.push(current);
          current = '';
        }
      } else {
        current += char;
      }
    }

    if (current) {
      parts.push(current);
    }

    return parts;
  }

  private buildCommand(prompt: string): { command: string; prompt: string; tempFile?: string } {
    // Use temp file approach to avoid shell escaping issues completely
    const fs = require('fs');
    const path = require('path');
    const os = require('os');
    
    // Create temp file for prompt
    const tempDir = os.tmpdir();
    const tempFile = path.join(tempDir, `cursor-prompt-${Date.now()}-${Math.random().toString(36).substring(7)}.txt`);
    
    // Write prompt to temp file
    fs.writeFileSync(tempFile, prompt, 'utf8');
    
    // Build command - cursor-agent reads from stdin or we can use cat to pipe file content
    // Since cursor-agent accepts prompt as args, we'll use cat to read file and pipe
    let command = `cat "${tempFile}" | cursor-agent -p --force --output-format json`;

    // Add --resume GUID if provided to maintain context across workflow runs
    if (this.options.cursorAgentGuid) {
      command += ` --resume "${this.options.cursorAgentGuid}"`;
    }

    if (this.options.model) {
      command += ` --model "${this.options.model}"`;
    }

    if (this.options.streamPartialOutput) {
      command += ' --stream-partial-output';
    }

    // Return command, prompt (for reference), and temp file path (for cleanup)
    return { command, prompt, tempFile };
  }

  private parseOutput(output: string): { success: boolean; summary?: string; details?: string } {
    try {
      // Try to extract JSON from output
      // Cursor CLI might wrap JSON in other text
      const jsonMatch = output.match(/\{[\s\S]*\}/);
      if (jsonMatch) {
        const json = JSON.parse(jsonMatch[0]);
        return {
          success: json.success !== false,
          summary: json.summary,
          details: json.details,
        };
      }

      // Fallback: if no JSON found, check for success indicators
      const lowerOutput = output.toLowerCase();
      const hasError = lowerOutput.includes('error') || lowerOutput.includes('failed');
      
      return {
        success: !hasError,
        summary: output.substring(0, 200),
      };
    } catch (error) {
      // If parsing fails, assume failure
      return {
        success: false,
        summary: 'Failed to parse output',
        details: output.substring(0, 500),
      };
    }
  }

  /**
   * Extract token usage from output and report via callback
   */
  private extractAndReportTokenUsage(output: string, stepName: string): void {
    if (!this.onTokenUsage) {
      return;
    }

    try {
      // Try to extract JSON from output
      const jsonMatch = output.match(/\{[\s\S]*\}/);
      if (!jsonMatch) {
        return;
      }

      const json = JSON.parse(jsonMatch[0]);
      
      // Extract token usage from various possible structures
      let inputTokens = 0;
      let outputTokens = 0;
      let totalTokens = 0;

      if (json.usage) {
        inputTokens = json.usage.input_tokens || json.usage.inputTokens || 0;
        outputTokens = json.usage.output_tokens || json.usage.outputTokens || 0;
        totalTokens = json.usage.total_tokens || json.usage.totalTokens || (inputTokens + outputTokens);
      } else if (json.input_tokens !== undefined) {
        inputTokens = json.input_tokens;
        outputTokens = json.output_tokens || 0;
        totalTokens = json.total_tokens || (inputTokens + outputTokens);
      } else if (json.tokens) {
        inputTokens = json.tokens.input || 0;
        outputTokens = json.tokens.output || 0;
        totalTokens = json.tokens.total || (inputTokens + outputTokens);
      }

      // Only report if we found token data
      if (inputTokens > 0 || outputTokens > 0 || totalTokens > 0) {
        const model = this.options.model || json.model || 'claude-4-5-sonnet';
        const contextWindowSize = this.getContextWindowSize(model);
        const contextWindowUsed = inputTokens + outputTokens;
        const contextWindowPercent = contextWindowSize > 0 
          ? (contextWindowUsed / contextWindowSize) * 100 
          : 0;

        this.onTokenUsage({
          inputTokens,
          outputTokens,
          totalTokens,
          contextWindowSize,
          contextWindowUsed,
          contextWindowPercent,
          model,
        });
      }
    } catch (error) {
      // Silently ignore token extraction errors
    }
  }

  /**
   * Get context window size for a model
   */
  private getContextWindowSize(model: string): number {
    const contextWindows: Record<string, number> = {
      // Anthropic Claude models
      'claude-4-5-sonnet': 200000,
      'claude-4-5-sonnet-1m': 1000000,
      'claude-4-5-haiku': 200000,
      'claude-4-1-opus': 200000,
      'claude-4-opus': 200000,
      'claude-4-sonnet': 200000,
      'claude-4-sonnet-1m': 1000000,
      // OpenAI GPT models
      'gpt-5': 272000,
      'gpt-5-pro': 272000,
      'gpt-5-fast': 272000,
      'gpt-5-codex': 272000,
      'gpt-5-1': 272000,
      'gpt-5-1-codex': 272000,
      'gpt-5-mini': 272000,
      'gpt-5-nano': 272000,
      'gpt-5-1-codex-mini': 272000,
      'gpt-4-1': 200000,
      'gpt-4-1-1m': 1000000,
      'o3': 200000,
      // Google Gemini models
      'gemini-3-pro': 200000,
      'gemini-3-pro-1m': 1000000,
      'gemini-2-5-pro': 200000,
      'gemini-2-5-pro-1m': 1000000,
      'gemini-2-5-flash': 1000000,
      // xAI Grok models
      'grok-4': 256000,
      'grok-4-fast': 200000,
      'grok-4-fast-2m': 2000000,
      'grok-code': 256000,
      // DeepSeek models
      'deepseek-v3-1': 60000,
      'deepseek-r1-20250528': 60000,
      // Cursor models
      'composer-1': 200000,
      'claude-3-5-sonnet-20240620': 200000,
      'claude-3-opus-20240229': 200000,
      'claude-3-sonnet-20240229': 200000,
      'claude-3-haiku-20240307': 200000,
      'claude-3-5-haiku-20241022': 200000,
      'claude-3-5-haiku-20240620': 200000,
      'gpt-4': 8192,
      'gpt-4-turbo': 128000,
      'gpt-4o': 128000,
      'gpt-3.5-turbo': 16385,
    };

    if (contextWindows[model]) {
      return contextWindows[model];
    }

    // Try partial match
    for (const [key, value] of Object.entries(contextWindows)) {
      if (model.includes(key) || key.includes(model)) {
        return value;
      }
    }

    // Default to Claude 3.5 Sonnet context window
    return 200000;
  }

  private createTimeout(ms: number): Promise<never> {
    return new Promise((_, reject) => {
      setTimeout(() => {
        reject(new Error(`Command timed out after ${ms}ms`));
      }, ms);
    });
  }
}


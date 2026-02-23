/**
 * Interactive Mode Service
 * 
 * Provides interactive pause/resume, step-by-step execution, and result inspection.
 */

import * as readline from 'readline';
import { AgentTask, TaskStatus } from '../domain/AgentTask';
import { AgentResult } from '../workflow/OrchestratorContext';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface InteractiveCommand {
  command: 'continue' | 'pause' | 'skip' | 'inspect' | 'modify' | 'abort' | 'help';
  stepName?: string;
  modification?: any;
}

export interface InteractiveState {
  paused: boolean;
  currentStep?: string;
  completedSteps: string[];
  pendingSteps: string[];
  results: Map<string, AgentResult>;
}

/**
 * Interactive Mode Manager
 */
export class InteractiveMode {
  private rl: readline.Interface;
  private state: InteractiveState;
  private workflow: WorkflowDefinition;
  private onCommand?: (command: InteractiveCommand) => Promise<void>;

  constructor(workflow: WorkflowDefinition) {
    this.workflow = workflow;
    this.rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout,
    });

    this.state = {
      paused: false,
      completedSteps: [],
      pendingSteps: workflow.steps.map(s => s.name),
      results: new Map(),
    };
  }

  /**
   * Start interactive mode
   */
  async start(): Promise<void> {
    console.log('\n🎮 Interactive Mode Enabled');
    console.log('Commands: continue, pause, skip <step>, inspect <step>, modify <step>, abort, help\n');
    
    this.setupCommandHandler();
  }

  /**
   * Setup command handler
   */
  private setupCommandHandler(): void {
    this.rl.on('line', async (input: string) => {
      const command = this.parseCommand(input.trim());
      if (command) {
        await this.handleCommand(command);
      }
    });

    // Handle Ctrl+C gracefully
    this.rl.on('SIGINT', () => {
      console.log('\n\n⚠️  Interactive mode interrupted. Use "abort" to exit.\n');
    });
  }

  /**
   * Parse user input into command
   */
  private parseCommand(input: string): InteractiveCommand | null {
    const parts = input.toLowerCase().split(' ');
    const cmd = parts[0];

    switch (cmd) {
      case 'continue':
      case 'c':
        return { command: 'continue' };
      
      case 'pause':
      case 'p':
        return { command: 'pause' };
      
      case 'skip':
      case 's':
        if (parts.length < 2) {
          console.log('❌ Usage: skip <step-name>');
          return null;
        }
        return { command: 'skip', stepName: parts.slice(1).join(' ') };
      
      case 'inspect':
      case 'i':
        if (parts.length < 2) {
          console.log('❌ Usage: inspect <step-name>');
          return null;
        }
        return { command: 'inspect', stepName: parts.slice(1).join(' ') };
      
      case 'modify':
      case 'm':
        if (parts.length < 2) {
          console.log('❌ Usage: modify <step-name>');
          return null;
        }
        return { command: 'modify', stepName: parts.slice(1).join(' ') };
      
      case 'abort':
      case 'a':
        return { command: 'abort' };
      
      case 'help':
      case 'h':
        return { command: 'help' };
      
      default:
        console.log(`❌ Unknown command: ${cmd}. Type "help" for available commands.`);
        return null;
    }
  }

  /**
   * Handle interactive command
   */
  private async handleCommand(command: InteractiveCommand): Promise<void> {
    switch (command.command) {
      case 'continue':
        this.state.paused = false;
        console.log('▶️  Continuing execution...\n');
        if (this.onCommand) {
          await this.onCommand(command);
        }
        break;

      case 'pause':
        this.state.paused = true;
        console.log('⏸️  Execution paused. Type "continue" to resume.\n');
        break;

      case 'skip':
        if (command.stepName) {
          console.log(`⏭️  Skipping step: ${command.stepName}\n`);
          if (this.onCommand) {
            await this.onCommand(command);
          }
        }
        break;

      case 'inspect':
        if (command.stepName) {
          await this.inspectStep(command.stepName);
        }
        break;

      case 'modify':
        if (command.stepName) {
          await this.modifyStep(command.stepName);
        }
        break;

      case 'abort':
        console.log('🛑 Aborting workflow...\n');
        if (this.onCommand) {
          await this.onCommand(command);
        }
        this.close();
        break;

      case 'help':
        this.showHelp();
        break;
    }
  }

  /**
   * Inspect step results
   */
  private async inspectStep(stepName: string): Promise<void> {
    const result = this.state.results.get(stepName);
    if (!result) {
      console.log(`❌ No results found for step: ${stepName}`);
      console.log(`Available steps: ${Array.from(this.state.results.keys()).join(', ')}\n`);
      return;
    }

    console.log(`\n📋 Step: ${stepName}`);
    console.log(`Status: ${result.success ? '✅ Success' : '❌ Failed'}`);
    console.log(`Summary: ${result.summary}`);
    
    if (result.rawOutput) {
      console.log(`\nRaw Output (first 500 chars):`);
      console.log(result.rawOutput.substring(0, 500));
      if (result.rawOutput.length > 500) {
        console.log('... (truncated)');
      }
    }

    if (result.error) {
      console.log(`\nError: ${result.error}`);
    }

    console.log('');
  }

  /**
   * Modify step (placeholder for future implementation)
   */
  private async modifyStep(stepName: string): Promise<void> {
    console.log(`🔧 Step modification not yet implemented for: ${stepName}`);
    console.log('💡 You can skip this step and run it manually if needed.\n');
  }

  /**
   * Show help
   */
  private showHelp(): void {
    console.log('\n📖 Interactive Mode Commands:\n');
    console.log('  continue, c     - Continue execution');
    console.log('  pause, p        - Pause execution');
    console.log('  skip <step>, s  - Skip a specific step');
    console.log('  inspect <step>  - Inspect step results');
    console.log('  modify <step>   - Modify step (coming soon)');
    console.log('  abort, a        - Abort workflow');
    console.log('  help, h         - Show this help\n');
  }

  /**
   * Check if execution should pause
   */
  isPaused(): boolean {
    return this.state.paused;
  }

  /**
   * Register command handler
   */
  onCommandReceived(handler: (command: InteractiveCommand) => Promise<void>): void {
    this.onCommand = handler;
  }

  /**
   * Update state
   */
  updateState(updates: Partial<InteractiveState>): void {
    this.state = { ...this.state, ...updates };
  }

  /**
   * Get current state
   */
  getState(): InteractiveState {
    return { ...this.state };
  }

  /**
   * Close interactive mode
   */
  close(): void {
    this.rl.close();
  }

  /**
   * Prompt for confirmation
   */
  async prompt(question: string): Promise<string> {
    return new Promise((resolve) => {
      this.rl.question(question, (answer) => {
        resolve(answer.trim());
      });
    });
  }

  /**
   * Show current status
   */
  showStatus(): void {
    console.log('\n📊 Current Status:');
    console.log(`  Completed: ${this.state.completedSteps.length}/${this.workflow.steps.length}`);
    console.log(`  Pending: ${this.state.pendingSteps.length}`);
    if (this.state.currentStep) {
      console.log(`  Current: ${this.state.currentStep}`);
    }
    console.log('');
  }
}

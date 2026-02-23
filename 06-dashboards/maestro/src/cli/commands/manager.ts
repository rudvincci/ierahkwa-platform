/**
 * Manager Agent CLI Commands
 * 
 * Commands for the Orchestrator Manager Agent
 */

import { Command } from 'commander';
import { OrchestratorManagerAgent } from '../../agents/OrchestratorManagerAgent';
import { ConfigLoader } from '../../config/OrchestratorConfig';
import * as path from 'path';

export function createManagerCommand(): Command {
  const manager = new Command('manager');

  manager
    .command('analyze')
    .description('Analyze project structure and suggest workflows')
    .option('-p, --project <path>', 'Project path to analyze', process.cwd())
    .option('-o, --output <file>', 'Output file for analysis results')
    .option('--no-memory', 'Disable memory integration for this command')
    .option('--memory', 'Enable memory integration (overrides config)')
    .action(async (options) => {
      // Load config to check memory settings
      const config = await ConfigLoader.load();
      const useMemory = options.memory !== undefined 
        ? options.memory 
        : (options.noMemory ? false : config.manager.useMemory && config.memory.enabled);
      
      const agent = new OrchestratorManagerAgent(undefined, options.project, useMemory);
      console.log('🔍 Analyzing project structure...');
      
      try {
        const analysis = await agent.analyzeProject(options.project);
        console.log('\n📊 Project Analysis:');
        console.log(`  Complexity: ${analysis.complexity}`);
        console.log(`  Services: ${analysis.structure.services.length}`);
        console.log(`  Domains: ${analysis.structure.domains.length}`);
        console.log(`  Technologies: ${analysis.structure.technologies.join(', ')}`);
        
        if (options.output) {
          const fs = await import('fs/promises');
          await fs.writeFile(options.output, JSON.stringify(analysis, null, 2));
          console.log(`\n✅ Analysis saved to: ${options.output}`);
        }
      } catch (error) {
        console.error('❌ Failed to analyze project:', error);
        process.exit(1);
      }
    });

  manager
    .command('suggest')
    .description('Suggest workflows based on project analysis')
    .option('-p, --project <path>', 'Project path', process.cwd())
    .option('--no-memory', 'Disable memory integration for this command')
    .option('--memory', 'Enable memory integration (overrides config)')
    .action(async (options) => {
      // Load config to check memory settings
      const config = await ConfigLoader.load();
      const useMemory = options.memory !== undefined 
        ? options.memory 
        : (options.noMemory ? false : config.manager.useMemory && config.memory.enabled);
      
      const agent = new OrchestratorManagerAgent(undefined, options.project, useMemory);
      console.log('💡 Generating workflow suggestions...');
      
      try {
        const analysis = await agent.analyzeProject(options.project);
        const suggestions = await agent.suggestWorkflows(analysis);
        
        console.log('\n📋 Suggested Workflows:');
        suggestions.forEach((suggestion, index) => {
          console.log(`\n${index + 1}. ${suggestion.name} (${suggestion.confidence} confidence)`);
          console.log(`   ${suggestion.description}`);
          console.log(`   Steps: ${suggestion.steps.length}`);
        });
      } catch (error) {
        console.error('❌ Failed to suggest workflows:', error);
        process.exit(1);
      }
    });

  manager
    .command('create')
    .description('Create a workflow from suggestion or template')
    .requiredOption('-n, --name <name>', 'Workflow name')
    .option('-t, --template <template>', 'Template name')
    .option('-s, --suggestion <index>', 'Use suggestion by index')
    .option('-o, --output <file>', 'Output file (default: config/orchestration.yml)')
    .option('--no-memory', 'Disable memory integration for this command')
    .option('--memory', 'Enable memory integration (overrides config)')
    .action(async (options) => {
      // Load config to check memory settings
      const config = await ConfigLoader.load();
      const useMemory = options.memory !== undefined 
        ? options.memory 
        : (options.noMemory ? false : config.manager.useMemory && config.memory.enabled);
      
      const agent = new OrchestratorManagerAgent(undefined, process.cwd(), useMemory);
      console.log(`🚀 Creating workflow: ${options.name}...`);
      
      try {
        let workflow;
        
        if (options.template) {
          workflow = await agent.createWorkflow(options.template, options.name);
        } else if (options.suggestion) {
          const analysis = await agent.analyzeProject(process.cwd());
          const suggestions = await agent.suggestWorkflows(analysis);
          const suggestion = suggestions[parseInt(options.suggestion)];
          workflow = await agent.createWorkflow(suggestion, options.name);
        } else {
          console.error('❌ Must provide either --template or --suggestion');
          process.exit(1);
        }
        
        const outputFile = options.output || './config/orchestration.yml';
        console.log(`\n✅ Workflow created: ${outputFile}`);
        console.log(`   Steps: ${workflow.steps?.length || 0}`);
      } catch (error) {
        console.error('❌ Failed to create workflow:', error);
        process.exit(1);
      }
    });

  manager
    .command('optimize')
    .description('Optimize an existing workflow')
    .requiredOption('-w, --workflow <name>', 'Workflow name to optimize')
    .option('-o, --output <file>', 'Output file for optimization report')
    .option('--no-memory', 'Disable memory integration for this command')
    .option('--memory', 'Enable memory integration (overrides config)')
    .action(async (options) => {
      // Load config to check memory settings
      const config = await ConfigLoader.load();
      const useMemory = options.memory !== undefined 
        ? options.memory 
        : (options.noMemory ? false : config.manager.useMemory && config.memory.enabled);
      
      const agent = new OrchestratorManagerAgent(undefined, process.cwd(), useMemory);
      console.log(`⚡ Optimizing workflow: ${options.workflow}...`);
      
      try {
        const report = await agent.optimizeWorkflow(options.workflow);
        
        console.log('\n📈 Optimization Report:');
        console.log(`  Workflow: ${report.workflowName}`);
        console.log(`  Success Rate: ${(report.metrics.successRate * 100).toFixed(1)}%`);
        console.log(`  Avg Execution Time: ${report.metrics.avgExecutionTime.toFixed(2)}s`);
        console.log(`  Optimizations Found: ${report.optimizations.length}`);
        
        report.optimizations.forEach((opt, index) => {
          console.log(`\n  ${index + 1}. ${opt.type.toUpperCase()} (${opt.impact} impact)`);
          console.log(`     ${opt.description}`);
        });
        
        if (options.output) {
          const fs = await import('fs/promises');
          await fs.writeFile(options.output, JSON.stringify(report, null, 2));
          console.log(`\n✅ Report saved to: ${options.output}`);
        }
      } catch (error) {
        console.error('❌ Failed to optimize workflow:', error);
        process.exit(1);
      }
    });

  manager
    .command('monitor')
    .description('Monitor workflow execution')
    .option('-w, --workflow <name>', 'Workflow name to monitor')
    .option('-i, --id <id>', 'Execution ID to monitor')
    .action(async (options) => {
      console.log('👁️  Monitoring workflow execution...');
      // Implementation for real-time monitoring
      console.log('(Monitoring feature coming soon)');
    });

  return manager;
}

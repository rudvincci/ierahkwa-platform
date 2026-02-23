#!/usr/bin/env node

import { Command } from 'commander';
import { listFlows } from './commands/flows';
import { runWorkflow } from './commands/run';
import { createManagerCommand } from './commands/manager';
import { listCheckpoints, showCheckpoint, deleteCheckpoint } from './commands/checkpoints';
import { configureSync, syncNow, syncStatus } from './commands/sync';
import { listTemplates, showTemplate, instantiateTemplate } from './commands/templates';

const program = new Command();

program
  .name('maestro')
  .description('Maestro - Orchestrate AI agents and workflows')
  .version('1.0.0');

program
  .command('flows')
  .description('List available workflows')
  .option('-c, --config <path>', 'Path to orchestration config file')
  .action(async (options) => {
    await listFlows(options.config);
  });

program
  .command('run')
  .description('Run a workflow')
  .requiredOption('-f, --flow <name>', 'Workflow name to execute')
  .option('-c, --config <path>', 'Path to orchestration config file')
  .option('--dry-run', 'Show execution plan without executing', false)
  .option('-r, --runner <type>', 'Agent runner type (dummy|cursor)', 'dummy')
  .option('--model <model>', 'Model to use for cursor-agent (e.g., claude-4-5-sonnet, gpt-5, gemini-3-pro)')
  .option('--feature <description>', 'Feature description to inject into prompts')
  .option('--log-level <level>', 'Logging level (info|debug|warn|error)', 'info')
  .option('-o, --output <dir>', 'Output directory for logs')
  .option('--max-concurrency <number>', 'Maximum parallel task executions', parseInt)
  .option('--continue-on-error', 'Continue execution even if a task fails', false)
  .option('--abort-on-error', 'Abort entire workflow on first error', false)
  .option('-v, --verbose', 'Show verbose output including prompts and partial results', false)
  .option('--resume <checkpoint-id>', 'Resume workflow from checkpoint')
  .option('--enable-checkpoints', 'Enable checkpoint saving (default: true)', true)
  .option('--auto-save-interval <seconds>', 'Auto-save interval in seconds (default: 60)', parseInt)
  .option('--stop', 'Request graceful shutdown of running workflow', false)
  .option('--enable-cache', 'Enable result caching (default: true)', true)
  .option('--cache-ttl <days>', 'Cache time-to-live in days (default: 7)', parseInt)
  .option('--enable-retry', 'Enable error retry with exponential backoff (default: true)', true)
  .option('--max-retries <number>', 'Maximum retry attempts per task (default: 3)', parseInt)
  .option('--retry-initial-delay <seconds>', 'Initial retry delay in seconds (default: 1)', parseInt)
  .option('--interactive', 'Enable interactive mode (pause/resume/inspect)', false)
  .option('--generate-report', 'Generate execution report (default: false)', false)
  .option('--report-format <format>', 'Report format: json, html, markdown (default: markdown)', 'markdown')
  .option('--optimize-parallel', 'Enable parallel execution optimization', false)
  .action(async (options) => {
    await runWorkflow({
      flow: options.flow,
      config: options.config,
      dryRun: options.dryRun,
      runner: options.runner,
      model: options.model,
      feature: options.feature,
      logLevel: options.logLevel,
      output: options.output,
      maxConcurrency: options.maxConcurrency,
      continueOnError: options.continueOnError,
      abortOnError: options.abortOnError,
      verbose: options.verbose,
      resume: options.resume,
      enableCheckpoints: options.enableCheckpoints,
      autoSaveInterval: options.autoSaveInterval,
      stop: options.stop,
      enableCache: options.enableCache,
      cacheTtl: options.cacheTtl,
      enableRetry: options.enableRetry,
      maxRetries: options.maxRetries,
      retryInitialDelay: options.retryInitialDelay,
      interactive: options.interactive,
      generateReport: options.generateReport,
      reportFormat: options.reportFormat,
      optimizeParallel: options.optimizeParallel,
    });
  });

// Add manager command
program.addCommand(createManagerCommand());

// Dashboard command
import { startDashboard, stopDashboard } from './commands/dashboard';
import { startMcpServer, stopMcpServer } from './commands/mcp';
import { enableOrchestrator } from './commands/enable';
import { disableOrchestrator } from './commands/disable';
import { checkStatus } from './commands/status';
import { cleanupAgents } from './commands/cleanup';
import { killAllMaestroProcesses } from './commands/killall';

program
  .command('dashboard')
  .description('Start real-time monitoring dashboard')
  .option('-p, --port <port>', 'Port number (default: 3000)', parseInt)
  .action(async (options) => {
    await startDashboard({ port: options.port });
  });

program
  .command('dashboard:stop')
  .description('Stop the dashboard server')
  .action(async () => {
    await stopDashboard();
  });

program
  .command('mcp')
  .description('Start the MCP (Model Context Protocol) server')
  .option('-p, --port <port>', 'Port to run MCP server on (HTTP mode)', '3001')
  .option('--stdio', 'Use stdio transport (for Cursor MCP protocol)', false)
  .action(async (options) => {
    await startMcpServer(parseInt(options.port), options.stdio);
  });

program
  .command('mcp:stop')
  .description('Stop the MCP server')
  .action(async () => {
    await stopMcpServer();
  });

// Quick enable/disable commands
program
  .command('enable')
  .alias('on')
  .description('Enable the orchestrator (start dashboard)')
  .option('-p, --port <port>', 'Port to run dashboard on', '3000')
  .action(async (options) => {
    await enableOrchestrator({ port: parseInt(options.port) });
  });

program
  .command('disable')
  .alias('off')
  .description('Disable the orchestrator (stop dashboard)')
  .action(async () => {
    await disableOrchestrator();
  });

program
  .command('status')
  .alias('info')
  .description('Check orchestrator status')
  .action(async () => {
    await checkStatus();
  });

// Agent cleanup commands
program
  .command('agents:cleanup')
  .description('Clean up old cursor-agent created agents')
  .option('--dry-run', 'Show what would be deleted without actually deleting', false)
  .action(async (options) => {
    await cleanupAgents({ dryRun: options.dryRun });
  });

// Kill all Maestro processes (emergency cleanup)
program
  .command('killall')
  .alias('kill-all')
  .description('Forcefully kill all Maestro-related processes (use if processes are stuck)')
  .action(async () => {
    await killAllMaestroProcesses();
  });

// Checkpoint management commands
program
  .command('checkpoints')
  .description('List all available checkpoints')
  .action(async () => {
    await listCheckpoints();
  });

program
  .command('checkpoint')
  .description('Show checkpoint details')
  .requiredOption('-i, --id <checkpoint-id>', 'Checkpoint ID')
  .action(async (options) => {
    await showCheckpoint(options.id);
  });

program
  .command('delete-checkpoint')
  .description('Delete a checkpoint')
  .requiredOption('-i, --id <checkpoint-id>', 'Checkpoint ID')
  .action(async (options) => {
    await deleteCheckpoint(options.id);
  });

// Private data sync commands (OPT-IN ONLY)
program
  .command('sync:configure')
  .description('Configure private data sync to a git repository (OPT-IN ONLY)')
  .action(async () => {
    await configureSync();
  });

program
  .command('sync:now')
  .description('Sync data to private repository now')
  .action(async () => {
    await syncNow();
  });

program
  .command('sync:status')
  .description('Show private data sync status')
  .action(async () => {
    await syncStatus();
  });

// Template management commands
program
  .command('templates')
  .description('List all available workflow templates')
  .option('-c, --category <category>', 'Filter by category')
  .action(async (options) => {
    await listTemplates(options.category);
  });

program
  .command('template')
  .description('Show template details')
  .requiredOption('-n, --name <name>', 'Template name')
  .action(async (options) => {
    await showTemplate(options.name);
  });

program
  .command('instantiate')
  .description('Instantiate a template with parameters')
  .requiredOption('-n, --name <name>', 'Template name')
  .option('-p, --param <key=value>', 'Parameter (can be used multiple times)', (value, prev) => {
    const [key, val] = value.split('=');
    return { ...prev, [key]: val };
  }, {})
  .action(async (options) => {
    await instantiateTemplate(options.name, options.param || {});
  });

// Parse command line arguments once at the end
program.parse(process.argv);


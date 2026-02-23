/**
 * Private Data Sync Commands
 * 
 * Commands to configure and manage private data sync to a git repository.
 * This is OPT-IN ONLY and requires explicit user consent.
 */

import { PrivateDataSync, PrivateSyncConfig } from '../../services/PrivateDataSync';
import { ConfigLoader } from '../../config/OrchestratorConfig';
import { findRepoRoot } from '../utils';
import * as readline from 'readline';
import * as fs from 'fs';
import * as path from 'path';

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
});

function question(prompt: string): Promise<string> {
  return new Promise((resolve) => {
    rl.question(prompt, resolve);
  });
}

/**
 * Configure private data sync
 */
export async function configureSync(): Promise<void> {
  console.log('\n🔒 Private Data Sync Configuration');
  console.log('=====================================\n');
  console.log('⚠️  IMPORTANT: This will sync your workflow data to a private repository.');
  console.log('   This is OPT-IN ONLY and requires explicit consent.\n');

  const enabled = await question('Enable private data sync? (yes/no): ');
  if (enabled.toLowerCase() !== 'yes') {
    console.log('\n✅ Private data sync disabled. Your data stays local only.');
    rl.close();
    return;
  }

  console.log('\n📋 Configuration Options:\n');

  const repoUrl = await question('Private repository URL (or press Enter to skip): ');
  const repoPath = await question('Local repository path (or press Enter to use default): ');

  if (!repoUrl && !repoPath) {
    console.log('\n❌ Either repository URL or path must be provided.');
    rl.close();
    return;
  }

  const branch = await question('Branch name (default: main): ') || 'main';
  
  const encrypt = await question('Encrypt sensitive data? (yes/no, default: yes): ') || 'yes';
  let encryptionKey: string | undefined;
  if (encrypt.toLowerCase() === 'yes') {
    encryptionKey = await question('Encryption key (or press Enter to generate): ');
    if (!encryptionKey) {
      // Generate a random key
      const crypto = require('crypto');
      encryptionKey = crypto.randomBytes(32).toString('hex');
      console.log(`\n🔑 Generated encryption key: ${encryptionKey}`);
      console.log('⚠️  Save this key securely! You will need it to decrypt data.\n');
    }
  }

  const syncInterval = await question('Sync interval in minutes (default: 60): ') || '60';

  console.log('\n📦 What data to sync:');
  const checkpointsAnswer = await question('  Include checkpoints? (yes/no, default: yes): ');
  const includeCheckpoints = (checkpointsAnswer || 'yes').toLowerCase() === 'yes';
  
  const cacheAnswer = await question('  Include cache? (yes/no, default: no): ');
  const includeCache = (cacheAnswer || 'no').toLowerCase() === 'yes';
  
  const reportsAnswer = await question('  Include reports? (yes/no, default: yes): ');
  const includeReports = (reportsAnswer || 'yes').toLowerCase() === 'yes';
  
  const logsAnswer = await question('  Include logs? (yes/no, default: no): ');
  const includeLogs = (logsAnswer || 'no').toLowerCase() === 'yes';

  const config: PrivateSyncConfig = {
    enabled: true,
    repositoryUrl: repoUrl || undefined,
    repositoryPath: repoPath || undefined,
    branch,
    encryptionKey,
    syncInterval: parseInt(syncInterval, 10),
    includeCheckpoints,
    includeCache,
    includeReports,
    includeLogs,
  };

  // Save configuration to orchestrator.config.yml
  const repoRoot = findRepoRoot();
  const configPath = path.join(repoRoot, '.maestro', 'config', 'orchestrator.config.yml');
  const examplePath = path.join(repoRoot, '.maestro', 'config', 'orchestrator.config.yml.example');
  
  // Load existing config or create from example
  let orchestratorConfig: any;
  try {
    orchestratorConfig = await ConfigLoader.load();
  } catch (error) {
    // Config doesn't exist, try to load from example
    if (fs.existsSync(examplePath)) {
      const yaml = require('js-yaml');
      const exampleContent = fs.readFileSync(examplePath, 'utf-8');
      orchestratorConfig = yaml.load(exampleContent);
    } else {
      orchestratorConfig = {};
    }
  }

  // Update privateSync config
  orchestratorConfig.privateSync = config;

  // Ensure config directory exists
  const configDir = path.dirname(configPath);
  if (!fs.existsSync(configDir)) {
    fs.mkdirSync(configDir, { recursive: true });
  }

  // Write updated config
  const yaml = require('js-yaml');
  const yamlContent = yaml.dump(orchestratorConfig, {
    indent: 2,
    lineWidth: 120,
    noRefs: true,
  });
  
  fs.writeFileSync(configPath, yamlContent, 'utf-8');

  console.log('\n✅ Configuration saved to orchestrator.config.yml!');
  console.log(`   Location: ${configPath}`);
  console.log('\n⚠️  Note: This file is excluded from git to protect your settings.');
  console.log('   Your repository URL and encryption key are kept private.');

  rl.close();
}

/**
 * Sync data now
 */
export async function syncNow(): Promise<void> {
  const repoRoot = findRepoRoot();
  const config = await ConfigLoader.load();

  if (!config.privateSync?.enabled) {
    console.log('❌ Private data sync is not enabled.');
    console.log('   Run "maestro sync configure" to set it up.');
    return;
  }

  console.log('🔄 Syncing data to private repository...\n');

  const sync = new PrivateDataSync(config.privateSync, repoRoot);
  await sync.initialize();
  
  const result = await sync.sync(repoRoot);

  if (result.success) {
    console.log(`✅ Sync completed successfully!`);
    console.log(`   Synced: ${result.syncedFiles.length} file(s)`);
    if (result.skippedFiles.length > 0) {
      console.log(`   Skipped: ${result.skippedFiles.length} file(s)`);
    }
    if (result.errors.length > 0) {
      console.log(`   Errors: ${result.errors.length}`);
      result.errors.forEach(err => console.log(`     - ${err}`));
    }
  } else {
    console.log('❌ Sync failed:');
    result.errors.forEach(err => console.log(`   - ${err}`));
  }
}

/**
 * Show sync status
 */
export async function syncStatus(): Promise<void> {
  const config = await ConfigLoader.load();

  if (!config.privateSync?.enabled) {
    console.log('📊 Private Data Sync: Disabled');
    console.log('   Your data stays local only.');
    return;
  }

  const repoRoot = findRepoRoot();
  const sync = new PrivateDataSync(config.privateSync, repoRoot);
  
  const status = await sync.getStatus();

  console.log('📊 Private Data Sync Status');
  console.log('============================\n');
  console.log(`Enabled: ${status.enabled ? '✅ Yes' : '❌ No'}`);
  console.log(`Repository: ${status.repositoryPath}`);
  console.log(`Pending Changes: ${status.pendingChanges}`);
  if (status.lastSync) {
    console.log(`Last Sync: ${status.lastSync.toISOString()}`);
  }
}

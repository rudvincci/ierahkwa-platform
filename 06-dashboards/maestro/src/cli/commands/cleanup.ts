/**
 * Cleanup command - Clean up cursor-agent created agents
 */

import { AgentCleanupService } from '../../services/AgentCleanupService';
import { ConfigLoader } from '../../config/OrchestratorConfig';

export async function cleanupAgents(options: { dryRun?: boolean; force?: boolean }): Promise<void> {
  try {
    const config = await ConfigLoader.load();
    const cleanupConfig = config.agentCleanup || {
      enabled: true,
      interval: { value: 24, unit: 'hours' },
      minAge: { value: 7, unit: 'days' },
      dryRun: false,
    };

    // Override dryRun if specified
    if (options.dryRun !== undefined) {
      cleanupConfig.dryRun = options.dryRun;
    }

    const service = new AgentCleanupService(cleanupConfig);

    console.log('🧹 Agent Cleanup: Starting cleanup...');
    if (cleanupConfig.dryRun) {
      console.log('   Mode: DRY RUN (no agents will be deleted)');
    }

    const result = await service.cleanup();

    console.log('\n📊 Cleanup Results:');
    console.log(`   ✅ Deleted: ${result.deleted}`);
    console.log(`   ⏭️  Skipped: ${result.skipped}`);
    if (result.errors > 0) {
      console.log(`   ❌ Errors: ${result.errors}`);
    }

    if (cleanupConfig.dryRun) {
      console.log('\n💡 This was a dry run. Run without --dry-run to actually delete agents.');
    }
  } catch (error: any) {
    console.error('❌ Cleanup failed:', error.message);
    process.exit(1);
  }
}

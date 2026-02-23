/**
 * Checkpoint Management Commands
 * 
 * List, view, and manage workflow checkpoints
 */

import { StateManager, MemoryStateStorage } from '../../services/StateManager';
import { findRepoRoot } from '../utils';

export async function listCheckpoints(): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const stateStorage = new MemoryStateStorage(repoRoot);
    const stateManager = new StateManager(stateStorage);

    const checkpoints = await stateManager.list();

    if (checkpoints.length === 0) {
      console.log('\n📋 No checkpoints found.\n');
      return;
    }

    console.log('\n📋 Available Checkpoints:\n');
    
    checkpoints.forEach((checkpoint, index) => {
      const duration = Math.round((Date.now() - checkpoint.startedAt.getTime()) / 1000);
      const completed = checkpoint.completedTasks.length;
      const failed = checkpoint.failedTasks.length;
      const total = completed + failed;
      const progress = total > 0 ? Math.round((completed / total) * 100) : 0;

      console.log(`${index + 1}. ${checkpoint.id}`);
      console.log(`   Workflow: ${checkpoint.workflowName}`);
      console.log(`   Started: ${checkpoint.startedAt.toISOString()}`);
      console.log(`   Last Updated: ${checkpoint.lastUpdatedAt.toISOString()}`);
      console.log(`   Duration: ${duration}s`);
      console.log(`   Progress: ${completed}/${total} completed (${progress}%)`);
      if (failed > 0) {
        console.log(`   Failed: ${failed} tasks`);
      }
      if (checkpoint.currentStep) {
        console.log(`   Current Step: ${checkpoint.currentStep}`);
      }
      console.log('');
    });

    console.log('💡 Resume a checkpoint with: --resume <checkpoint-id>\n');
  } catch (error: any) {
    console.error('❌ Failed to list checkpoints:', error.message);
    process.exit(1);
  }
}

export async function showCheckpoint(checkpointId: string): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const stateStorage = new MemoryStateStorage(repoRoot);
    const stateManager = new StateManager(stateStorage);

    const checkpoint = await stateManager.load(checkpointId);

    if (!checkpoint) {
      console.error(`❌ Checkpoint not found: ${checkpointId}`);
      process.exit(1);
    }

    console.log(`\n📋 Checkpoint Details: ${checkpointId}\n`);
    console.log(`Workflow: ${checkpoint.workflowName}`);
    console.log(`Started: ${checkpoint.startedAt.toISOString()}`);
    console.log(`Last Updated: ${checkpoint.lastUpdatedAt.toISOString()}`);
    console.log(`Repository: ${checkpoint.metadata.repositoryRoot}`);
    
    if (checkpoint.metadata.featureDescription) {
      console.log(`Feature: ${checkpoint.metadata.featureDescription}`);
    }

    console.log(`\nCompleted Tasks (${checkpoint.completedTasks.length}):`);
    checkpoint.completedTasks.forEach((task, index) => {
      console.log(`  ${index + 1}. ${task}`);
    });

    if (checkpoint.failedTasks.length > 0) {
      console.log(`\nFailed Tasks (${checkpoint.failedTasks.length}):`);
      checkpoint.failedTasks.forEach((task, index) => {
        console.log(`  ${index + 1}. ${task}`);
      });
    }

    if (checkpoint.currentStep) {
      console.log(`\nCurrent Step: ${checkpoint.currentStep}`);
    }

    console.log(`\nResults: ${checkpoint.results.size} stored`);
    console.log(`\n💡 Resume with: --resume ${checkpointId}\n`);
  } catch (error: any) {
    console.error('❌ Failed to show checkpoint:', error.message);
    process.exit(1);
  }
}

export async function deleteCheckpoint(checkpointId: string): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const stateStorage = new MemoryStateStorage(repoRoot);
    const stateManager = new StateManager(stateStorage);

    await stateManager.delete(checkpointId);
    console.log(`✅ Checkpoint deleted: ${checkpointId}\n`);
  } catch (error: any) {
    console.error('❌ Failed to delete checkpoint:', error.message);
    process.exit(1);
  }
}

import { ConfigLoader } from '../../config/ConfigLoader';
import { OrchestrationConfig } from '../../domain/WorkflowDefinition';

export async function listFlows(configPath?: string): Promise<void> {
  try {
    const loader = new ConfigLoader();
    const config = loader.loadConfig(configPath);

    console.log('\n📋 Available Workflows:\n');

    if (Object.keys(config.flows).length === 0) {
      console.log('  No workflows defined.');
      return;
    }

    for (const [flowName, flow] of Object.entries(config.flows)) {
      console.log(`  ${flowName}`);
      if (flow.description) {
        console.log(`    ${flow.description}`);
      }
      console.log(`    Steps: ${flow.steps.length}`);
      console.log('');
    }

    if (config.roles) {
      console.log('\n👥 Available Agent Roles:\n');
      for (const [roleName, role] of Object.entries(config.roles)) {
        console.log(`  ${roleName}`);
        console.log(`    ${role.description}`);
        console.log('');
      }
    }
  } catch (error: any) {
    console.error('❌ Error loading flows:', error.message);
    process.exit(1);
  }
}


/**
 * Template Management Commands
 * 
 * List, create, and manage workflow templates
 */

import { WorkflowTemplateManager } from '../../services/WorkflowTemplate';
import { findRepoRoot } from '../utils';

export async function listTemplates(category?: string): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const templateManager = new WorkflowTemplateManager(repoRoot);

    const templates = templateManager.listTemplates(category);

    if (templates.length === 0) {
      console.log('\n📋 No templates found.\n');
      if (category) {
        console.log(`💡 Try without category filter or create a template.\n`);
      }
      return;
    }

    console.log(`\n📋 Available Templates${category ? ` (Category: ${category})` : ''}:\n`);

    // Group by category
    const byCategory = new Map<string, typeof templates>();
    for (const template of templates) {
      const cat = template.metadata.category || 'uncategorized';
      if (!byCategory.has(cat)) {
        byCategory.set(cat, []);
      }
      byCategory.get(cat)!.push(template);
    }

    for (const [cat, catTemplates] of byCategory.entries()) {
      console.log(`📁 ${cat.toUpperCase()}`);
      for (const template of catTemplates) {
        console.log(`   • ${template.metadata.name}`);
        console.log(`     ${template.metadata.description}`);
        if (template.metadata.tags && template.metadata.tags.length > 0) {
          console.log(`     Tags: ${template.metadata.tags.join(', ')}`);
        }
        if (template.metadata.parameters && template.metadata.parameters.length > 0) {
          console.log(`     Parameters: ${template.metadata.parameters.map(p => p.name).join(', ')}`);
        }
        console.log('');
      }
    }

    const categories = templateManager.getCategories();
    if (categories.length > 0) {
      console.log(`\n💡 Categories: ${categories.join(', ')}\n`);
    }
  } catch (error: any) {
    console.error('❌ Failed to list templates:', error.message);
    process.exit(1);
  }
}

export async function showTemplate(name: string): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const templateManager = new WorkflowTemplateManager(repoRoot);

    const template = templateManager.getTemplate(name);

    if (!template) {
      console.error(`❌ Template not found: ${name}`);
      console.log('\nAvailable templates:');
      const templates = templateManager.listTemplates();
      templates.forEach(t => console.log(`  - ${t.metadata.name}`));
      process.exit(1);
    }

    console.log(`\n📋 Template: ${template.metadata.name}\n`);
    console.log(`Description: ${template.metadata.description}`);
    
    if (template.metadata.author) {
      console.log(`Author: ${template.metadata.author}`);
    }
    if (template.metadata.version) {
      console.log(`Version: ${template.metadata.version}`);
    }
    if (template.metadata.category) {
      console.log(`Category: ${template.metadata.category}`);
    }
    if (template.metadata.tags && template.metadata.tags.length > 0) {
      console.log(`Tags: ${template.metadata.tags.join(', ')}`);
    }

    if (template.metadata.parameters && template.metadata.parameters.length > 0) {
      console.log(`\nParameters:`);
      template.metadata.parameters.forEach(param => {
        console.log(`  • ${param.name} (${param.type})`);
        console.log(`    ${param.description}`);
        if (param.required) {
          console.log(`    Required: Yes`);
        }
        if (param.default !== undefined) {
          console.log(`    Default: ${param.default}`);
        }
        if (param.options) {
          console.log(`    Options: ${param.options.join(', ')}`);
        }
      });
    }

    console.log(`\nWorkflow Steps: ${template.workflow.steps.length}`);
    template.workflow.steps.forEach((step, index) => {
      console.log(`  ${index + 1}. ${step.name} (${step.agent || 'default'})`);
    });

    if (template.examples && template.examples.length > 0) {
      console.log(`\nExamples:`);
      template.examples.forEach((example, index) => {
        console.log(`  ${index + 1}. ${example}`);
      });
    }

    console.log('');
  } catch (error: any) {
    console.error('❌ Failed to show template:', error.message);
    process.exit(1);
  }
}

export async function instantiateTemplate(
  name: string,
  parameters: Record<string, any>
): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const templateManager = new WorkflowTemplateManager(repoRoot);

    const workflow = templateManager.instantiateTemplate(name, parameters);

    if (!workflow) {
      console.error(`❌ Template not found: ${name}`);
      process.exit(1);
    }

    console.log(`\n✅ Template instantiated: ${name}`);
    console.log(`   Workflow: ${workflow.name}`);
    console.log(`   Steps: ${workflow.steps.length}\n`);

    // Output workflow as YAML
    const yaml = require('js-yaml');
    console.log(yaml.dump(workflow, { indent: 2 }));
  } catch (error: any) {
    console.error(`❌ Failed to instantiate template: ${error.message}`);
    process.exit(1);
  }
}

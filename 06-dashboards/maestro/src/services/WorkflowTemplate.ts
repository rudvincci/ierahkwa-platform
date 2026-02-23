/**
 * Workflow Template Service
 * 
 * Manages workflow templates for common patterns and reusable workflows.
 */

import * as fs from 'fs';
import * as path from 'path';
import * as yaml from 'js-yaml';
import { WorkflowDefinition } from '../domain/WorkflowDefinition';

export interface TemplateMetadata {
  name: string;
  description: string;
  author?: string;
  version?: string;
  tags?: string[];
  parameters?: TemplateParameter[];
  category?: string;
}

export interface TemplateParameter {
  name: string;
  description: string;
  type: 'string' | 'number' | 'boolean' | 'array';
  required?: boolean;
  default?: any;
  options?: any[]; // For enum-like parameters
}

export interface WorkflowTemplate {
  metadata: TemplateMetadata;
  workflow: WorkflowDefinition;
  examples?: string[];
}

/**
 * Workflow Template Manager
 */
export class WorkflowTemplateManager {
  private templatesDir: string;
  private templates: Map<string, WorkflowTemplate> = new Map();

  constructor(repositoryRoot: string = process.cwd()) {
    this.templatesDir = path.join(repositoryRoot, '.maestro', 'templates');
    this.loadTemplates();
  }

  /**
   * Load all templates from templates directory
   */
  private loadTemplates(): void {
    if (!fs.existsSync(this.templatesDir)) {
      fs.mkdirSync(this.templatesDir, { recursive: true });
      return;
    }

    const files = fs.readdirSync(this.templatesDir);
    for (const file of files) {
      if (file.endsWith('.yml') || file.endsWith('.yaml')) {
        try {
          const filePath = path.join(this.templatesDir, file);
          const content = fs.readFileSync(filePath, 'utf-8');
          const template = this.parseTemplate(content);
          if (template) {
            this.templates.set(template.metadata.name, template);
          }
        } catch (error) {
          console.warn(`Failed to load template ${file}:`, error);
        }
      }
    }
  }

  /**
   * Parse template from YAML content
   */
  private parseTemplate(content: string): WorkflowTemplate | null {
    try {
      const doc = yaml.load(content) as any;
      
      if (!doc.metadata || !doc.workflow) {
        console.warn('Template missing metadata or workflow');
        return null;
      }

      return {
        metadata: doc.metadata,
        workflow: doc.workflow,
        examples: doc.examples || [],
      };
    } catch (error) {
      console.warn('Failed to parse template:', error);
      return null;
    }
  }

  /**
   * Get template by name
   */
  getTemplate(name: string): WorkflowTemplate | null {
    return this.templates.get(name) || null;
  }

  /**
   * List all templates
   */
  listTemplates(category?: string): WorkflowTemplate[] {
    const templates = Array.from(this.templates.values());
    if (category) {
      return templates.filter(t => t.metadata.category === category);
    }
    return templates;
  }

  /**
   * Instantiate template with parameters
   */
  instantiateTemplate(
    name: string,
    parameters: Record<string, any> = {}
  ): WorkflowDefinition | null {
    const template = this.getTemplate(name);
    if (!template) {
      return null;
    }

    // Validate parameters
    if (template.metadata.parameters) {
      for (const param of template.metadata.parameters) {
        if (param.required && !(param.name in parameters)) {
          if (param.default === undefined) {
            throw new Error(`Required parameter missing: ${param.name}`);
          }
          parameters[param.name] = param.default;
        } else if (!(param.name in parameters) && param.default !== undefined) {
          parameters[param.name] = param.default;
        }
      }
    }

    // Clone workflow and substitute parameters
    const workflow = JSON.parse(JSON.stringify(template.workflow));
    this.substituteParameters(workflow, parameters);

    return workflow;
  }

  /**
   * Substitute parameters in workflow definition
   */
  private substituteParameters(obj: any, parameters: Record<string, any>): any {
    if (typeof obj === 'string') {
      // Replace {{param}} placeholders
      return obj.replace(/\{\{(\w+)\}\}/g, (match, paramName) => {
        return parameters[paramName] !== undefined ? String(parameters[paramName]) : match;
      });
    }

    if (Array.isArray(obj)) {
      for (let i = 0; i < obj.length; i++) {
        obj[i] = this.substituteParameters(obj[i], parameters);
      }
    } else if (obj && typeof obj === 'object') {
      for (const key in obj) {
        obj[key] = this.substituteParameters(obj[key], parameters);
      }
    }
  }

  /**
   * Create template from workflow
   */
  async createTemplate(
    name: string,
    workflow: WorkflowDefinition,
    metadata: Partial<TemplateMetadata> & { examples?: string[] } = {}
  ): Promise<void> {
    const template: WorkflowTemplate = {
      metadata: {
        name,
        description: metadata.description || workflow.description || '',
        author: metadata.author,
        version: metadata.version || '1.0.0',
        tags: metadata.tags || [],
        parameters: metadata.parameters || [],
        category: metadata.category,
      },
      workflow,
      examples: metadata.examples || [],
    };

    const filePath = path.join(this.templatesDir, `${name}.yml`);
    const content = yaml.dump(template, { indent: 2 });
    await fs.promises.writeFile(filePath, content, 'utf-8');

    // Reload templates
    this.templates.set(name, template);
  }

  /**
   * Delete template
   */
  async deleteTemplate(name: string): Promise<void> {
    const filePath = path.join(this.templatesDir, `${name}.yml`);
    if (fs.existsSync(filePath)) {
      await fs.promises.unlink(filePath);
    }
    this.templates.delete(name);
  }

  /**
   * Get template categories
   */
  getCategories(): string[] {
    const categories = new Set<string>();
    for (const template of this.templates.values()) {
      if (template.metadata.category) {
        categories.add(template.metadata.category);
      }
    }
    return Array.from(categories);
  }
}

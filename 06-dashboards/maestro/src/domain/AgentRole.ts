export interface AgentRole {
  name: string;
  description: string;
  promptHints?: string;
  defaultFilePatterns?: string[];
  defaultDirectories?: string[];
  domainKnowledge?: string[];
}

export const createAgentRole = (
  name: string,
  description: string,
  options?: {
    promptHints?: string;
    defaultFilePatterns?: string[];
    defaultDirectories?: string[];
    domainKnowledge?: string[];
  }
): AgentRole => ({
  name,
  description,
  promptHints: options?.promptHints,
  defaultFilePatterns: options?.defaultFilePatterns,
  defaultDirectories: options?.defaultDirectories,
  domainKnowledge: options?.domainKnowledge,
});


/**
 * LLM-Based Reranking
 * Reranks search results using LLM to improve relevance
 * Ported from .skmemory/v1/api/reranking.py
 */

import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';
import * as yaml from 'js-yaml';

export interface RerankResult {
  file_path: string;
  score: number;
  hybrid_score?: number;
  semantic_score?: number;
  keyword_score?: number;
  normalized_keyword_score?: number;
  source?: 'semantic' | 'keyword' | 'hybrid';
  matched_terms?: string[];
  metadata?: Record<string, any>;
}

export interface RerankerConfig {
  provider?: 'openai' | 'anthropic' | 'ollama' | 'none';
  enabled?: boolean;
  model?: string;
  cache_enabled?: boolean;
  api_key?: string;
  base_url?: string;
}

/**
 * Reranks search results using LLM to determine relevance.
 * 
 * Supports multiple providers:
 * - OpenAI (GPT models)
 * - Anthropic (Claude models)
 * - Ollama (local models)
 */
export class Reranker {
  private config: RerankerConfig;
  private cacheDir: string;
  private provider: string;
  private enabled: boolean;
  private model: string;
  private cacheEnabled: boolean;
  private apiKey?: string;
  private baseUrl?: string;

  constructor(configPath?: string, cacheDir?: string) {
    this.config = this.loadConfig(configPath);
    this.cacheDir = cacheDir || path.join(process.cwd(), '.maestro', 'cache', 'reranking');
    if (!fs.existsSync(this.cacheDir)) {
      fs.mkdirSync(this.cacheDir, { recursive: true });
    }

    this.provider = this.config.provider || 'none';
    this.enabled = this.config.enabled || false;
    this.model = this.config.model || 'gpt-4o-mini';
    this.cacheEnabled = this.config.cache_enabled !== false;
    this.apiKey = this.config.api_key;
    this.baseUrl = this.config.base_url;
  }

  private loadConfig(configPath?: string): RerankerConfig {
    if (!configPath) {
      configPath = path.join(process.cwd(), '.maestro', 'config', 'llm-config.yml');
    }

    if (fs.existsSync(configPath)) {
      try {
        const data = fs.readFileSync(configPath, 'utf-8');
        const config = yaml.load(data) as any;
        return config?.llm || {};
      } catch (error) {
        // Ignore errors
      }
    }

    return {};
  }

  /**
   * Rerank search results using LLM.
   */
  async rerank(query: string, results: RerankResult[]): Promise<RerankResult[]> {
    if (!this.enabled || this.provider === 'none' || results.length === 0) {
      return results;
    }

    // Check cache
    const cacheKey = this.getCacheKey(query, results);
    const cached = this.loadFromCache(cacheKey);
    if (cached) {
      return cached;
    }

    // Rerank with LLM
    let reranked: RerankResult[];
    try {
      switch (this.provider) {
        case 'openai':
          reranked = await this.rerankWithOpenAI(query, results);
          break;
        case 'anthropic':
          reranked = await this.rerankWithAnthropic(query, results);
          break;
        case 'ollama':
          reranked = await this.rerankWithOllama(query, results);
          break;
        default:
          reranked = results;
      }
    } catch (error) {
      // If reranking fails, return original results
      reranked = results;
    }

    // Save to cache
    if (this.cacheEnabled) {
      this.saveToCache(cacheKey, reranked);
    }

    return reranked;
  }

  private getCacheKey(query: string, results: RerankResult[]): string {
    const cacheData = {
      query,
      results: results.map(r => r.file_path),
    };
    const cacheStr = JSON.stringify(cacheData);
    return crypto.createHash('md5').update(cacheStr).digest('hex');
  }

  private loadFromCache(cacheKey: string): RerankResult[] | null {
    if (!this.cacheEnabled) {
      return null;
    }

    const cacheFile = path.join(this.cacheDir, `${cacheKey}.json`);
    if (fs.existsSync(cacheFile)) {
      try {
        const data = fs.readFileSync(cacheFile, 'utf-8');
        return JSON.parse(data);
      } catch (error) {
        // Ignore errors
      }
    }

    return null;
  }

  private saveToCache(cacheKey: string, results: RerankResult[]): void {
    if (!this.cacheEnabled) {
      return;
    }

    const cacheFile = path.join(this.cacheDir, `${cacheKey}.json`);
    try {
      fs.writeFileSync(cacheFile, JSON.stringify(results, null, 2));
    } catch (error) {
      // Ignore errors
    }
  }

  private createRerankingPrompt(query: string, results: RerankResult[]): string {
    const resultsText = results.map((result, i) => {
      const filePath = result.file_path || 'Unknown';
      const score = result.hybrid_score || result.score || 0.0;
      return `${i + 1}. ${filePath} (score: ${score.toFixed(3)})`;
    }).join('\n');

    return `You are helping to rerank search results for better relevance.

Query: "${query}"

Current search results (in order):
${resultsText}

Please rerank these results by relevance to the query. Return a JSON array with the new order (1-based indices).

Example response format:
[3, 1, 5, 2, 4]

This means: result 3 is most relevant, then 1, then 5, etc.

Return only the JSON array, nothing else.`;
  }

  private async rerankWithOpenAI(query: string, results: RerankResult[]): Promise<RerankResult[]> {
    try {
      // Dynamic import to avoid requiring OpenAI SDK if not used
      // Use require for optional dependency
      let OpenAI: any;
      try {
        OpenAI = require('openai').OpenAI;
      } catch {
        return results; // SDK not available
      }
      
      const apiKey = this.apiKey || process.env.OPENAI_API_KEY;
      if (!apiKey) {
        return results;
      }

      const client = new OpenAI({ apiKey });
      const prompt = this.createRerankingPrompt(query, results);

      const response = await client.chat.completions.create({
        model: this.model,
        messages: [
          { role: 'system', content: 'You are a helpful assistant that reranks search results by relevance.' },
          { role: 'user', content: prompt },
        ],
        temperature: 0.1,
        max_tokens: 200,
      });

      const content = response.choices[0]?.message?.content?.trim() || '';
      return this.parseRerankingResponse(content, results);
    } catch (error) {
      // If OpenAI SDK not available or request fails, return original results
      return results;
    }
  }

  private async rerankWithAnthropic(query: string, results: RerankResult[]): Promise<RerankResult[]> {
    try {
      // Dynamic import to avoid requiring Anthropic SDK if not used
      // Use require for optional dependency
      let Anthropic: any;
      try {
        Anthropic = require('@anthropic-ai/sdk').Anthropic;
      } catch {
        return results; // SDK not available
      }
      
      const apiKey = this.apiKey || process.env.ANTHROPIC_API_KEY;
      if (!apiKey) {
        return results;
      }

      const client = new Anthropic({ apiKey });
      const prompt = this.createRerankingPrompt(query, results);

      const message = await client.messages.create({
        model: this.model,
        max_tokens: 200,
        temperature: 0.1,
        messages: [
          { role: 'user', content: prompt },
        ],
      });

      const content = message.content[0]?.type === 'text' ? message.content[0].text.trim() : '';
      return this.parseRerankingResponse(content, results);
    } catch (error) {
      // If Anthropic SDK not available or request fails, return original results
      return results;
    }
  }

  private async rerankWithOllama(query: string, results: RerankResult[]): Promise<RerankResult[]> {
    try {
      const baseUrl = this.baseUrl || 'http://localhost:11434';
      const prompt = this.createRerankingPrompt(query, results);

      const response = await fetch(`${baseUrl}/api/generate`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          model: this.model,
          prompt,
          options: {
            temperature: 0.1,
            num_predict: 200,
          },
        }),
      });

      if (!response.ok) {
        return results;
      }

      const data = await response.json() as any;
      const content = data.response?.trim() || '';
      return this.parseRerankingResponse(content, results);
    } catch (error) {
      // If Ollama not available or request fails, return original results
      return results;
    }
  }

  private parseRerankingResponse(content: string, results: RerankResult[]): RerankResult[] {
    try {
      // Extract JSON array from response
      const jsonMatch = content.match(/\[[\d\s,]+\]/);
      if (!jsonMatch) {
        return results;
      }

      const indices = JSON.parse(jsonMatch[0]) as number[];
      if (!Array.isArray(indices)) {
        return results;
      }

      // Convert to 0-based and rerank
      const reranked: RerankResult[] = [];
      const usedIndices = new Set<number>();

      for (const index of indices) {
        const zeroBasedIndex = index - 1;
        if (zeroBasedIndex >= 0 && zeroBasedIndex < results.length) {
          reranked.push(results[zeroBasedIndex]);
          usedIndices.add(zeroBasedIndex);
        }
      }

      // Add any missing results
      for (let i = 0; i < results.length; i++) {
        if (!usedIndices.has(i)) {
          reranked.push(results[i]);
        }
      }

      return reranked;
    } catch (error) {
      // If parsing fails, return original results
      return results;
    }
  }
}


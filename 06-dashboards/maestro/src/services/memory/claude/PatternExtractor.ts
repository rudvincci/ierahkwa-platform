/**
 * Pattern Extractor
 * Extracts patterns from sessions using frequency-based recognition
 * Ported from .claudememory/scripts/extract-patterns.sh
 */

import * as fs from 'fs';
import * as path from 'path';

export interface PatternMetadata {
  pattern_id: string;
  category: string;
  frequency: number;
  confidence: number;
  status: 'established' | 'emerging' | 'deprecated';
}

export interface ExtractedPattern {
  id: string;
  title: string;
  metadata: PatternMetadata;
  content: string;
  filePath: string;
}

export interface PatternExtractorConfig {
  min_frequency?: number;
  categories?: string[];
  patterns_dir?: string;
  sessions_dir?: string;
}

/**
 * Pattern extractor for frequency-based pattern recognition
 */
export class PatternExtractor {
  private root: string;
  private config: PatternExtractorConfig;
  private patternsDir: string;
  private sessionsDir: string;

  constructor(root: string = '.maestro', config?: PatternExtractorConfig) {
    this.root = root;
    this.config = config || {};
    this.patternsDir = this.config.patterns_dir || path.join(this.root, 'memory', 'patterns');
    this.sessionsDir = this.config.sessions_dir || path.join(this.root, 'memory', 'sessions');

    // Ensure directories exist
    if (!fs.existsSync(this.patternsDir)) {
      fs.mkdirSync(this.patternsDir, { recursive: true });
    }
  }

  /**
   * Extract patterns from sessions
   */
  async extractPatterns(): Promise<ExtractedPattern[]> {
    const minFrequency = this.config.min_frequency || 3;
    const categories = this.config.categories || [
      'authentication',
      'service-registration',
      'repository-patterns',
      'error-resolutions',
      'build-fixes',
      'test-patterns',
    ];

    const patterns: ExtractedPattern[] = [];

    // Extract patterns by category
    for (const category of categories) {
      const categoryPatterns = await this.extractCategoryPatterns(category, minFrequency);
      patterns.push(...categoryPatterns);
    }

    return patterns;
  }

  private async extractCategoryPatterns(
    category: string,
    minFrequency: number
  ): Promise<ExtractedPattern[]> {
    const patterns: ExtractedPattern[] = [];

    // Analyze sessions for this category
    const sessionFiles = this.getSessionFiles();
    const categoryOccurrences = new Map<string, number>();

    for (const sessionFile of sessionFiles) {
      try {
        const content = fs.readFileSync(sessionFile, 'utf-8');
        const lowerContent = content.toLowerCase();

        // Check if session mentions this category
        if (lowerContent.includes(category)) {
          // Extract key phrases/patterns
          const phrases = this.extractPhrases(content, category);
          for (const phrase of phrases) {
            categoryOccurrences.set(phrase, (categoryOccurrences.get(phrase) || 0) + 1);
          }
        }
      } catch (error) {
        // Ignore errors
      }
    }

    // Create patterns for frequent occurrences
    for (const [phrase, frequency] of categoryOccurrences.entries()) {
      if (frequency >= minFrequency) {
        const pattern = await this.createPattern(category, phrase, frequency);
        if (pattern) {
          patterns.push(pattern);
        }
      }
    }

    return patterns;
  }

  private getSessionFiles(): string[] {
    if (!fs.existsSync(this.sessionsDir)) {
      return [];
    }

    return fs.readdirSync(this.sessionsDir)
      .filter(f => f.endsWith('.md'))
      .map(f => path.join(this.sessionsDir, f));
  }

  private extractPhrases(content: string, category: string): string[] {
    const phrases: string[] = [];
    const lines = content.split('\n');

    for (const line of lines) {
      const lowerLine = line.toLowerCase();
      if (lowerLine.includes(category)) {
        // Extract meaningful phrases (code blocks, error messages, etc.)
        const codeBlockMatch = line.match(/```[\s\S]*?```/);
        if (codeBlockMatch) {
          phrases.push(codeBlockMatch[0].substring(0, 100));
        }

        const errorMatch = line.match(/error|exception|failed/i);
        if (errorMatch) {
          phrases.push(line.substring(0, 200));
        }
      }
    }

    return phrases;
  }

  private async createPattern(
    category: string,
    phrase: string,
    frequency: number
  ): Promise<ExtractedPattern | null> {
    const patternId = this.generatePatternId(category, phrase);
    const fileName = `${category}-${patternId}.md`;
    const filePath = path.join(this.patternsDir, fileName);

    // Check if pattern already exists
    if (fs.existsSync(filePath)) {
      // Update existing pattern
      try {
        const existing = fs.readFileSync(filePath, 'utf-8');
        const updated = this.updatePatternFrequency(existing, frequency);
        fs.writeFileSync(filePath, updated, 'utf-8');
        
        return {
          id: patternId,
          title: this.extractTitle(existing),
          metadata: this.extractMetadata(existing),
          content: updated,
          filePath,
        };
      } catch (error) {
        // Ignore errors
      }
    }

    // Create new pattern
    const patternContent = this.generatePatternContent(category, patternId, phrase, frequency);
    fs.writeFileSync(filePath, patternContent, 'utf-8');

    return {
      id: patternId,
      title: this.extractTitle(patternContent),
      metadata: {
        pattern_id: patternId,
        category,
        frequency,
        confidence: Math.min(95, frequency * 10),
        status: frequency >= 5 ? 'established' : 'emerging',
      },
      content: patternContent,
      filePath,
    };
  }

  private generatePatternId(category: string, phrase: string): string {
    const hash = phrase.substring(0, 8).replace(/[^a-z0-9]/gi, '').toLowerCase();
    return `${category}-${hash}`;
  }

  private generatePatternContent(
    category: string,
    patternId: string,
    phrase: string,
    frequency: number
  ): string {
    return `# Pattern: ${category}

<!-- CLAUDE_CONTEXT: Pattern extracted from sessions -->

## Metadata
- **Pattern ID**: ${patternId}
- **Category**: ${category}
- **Frequency**: ${frequency} occurrences
- **Confidence**: ${Math.min(95, frequency * 10)}%
- **Status**: ${frequency >= 5 ? 'Established' : 'Emerging'}

## Summary (for Claude)

Pattern extracted from ${frequency} session(s) related to ${category}.

## Pattern Details

${phrase.substring(0, 500)}

## Tags

#${category} #pattern #extracted
`;
  }

  private updatePatternFrequency(content: string, newFrequency: number): string {
    // Update frequency in metadata
    return content.replace(
      /- \*\*Frequency\*\*: \d+ occurrences/,
      `- **Frequency**: ${newFrequency} occurrences`
    ).replace(
      /- \*\*Confidence\*\*: \d+%/,
      `- **Confidence**: ${Math.min(95, newFrequency * 10)}%`
    );
  }

  private extractTitle(content: string): string {
    const match = content.match(/^# Pattern: (.+)$/m);
    return match ? match[1] : 'Unknown Pattern';
  }

  private extractMetadata(content: string): PatternMetadata {
    const metadata: Partial<PatternMetadata> = {
      pattern_id: '',
      category: '',
      frequency: 0,
      confidence: 0,
      status: 'emerging',
    };

    const idMatch = content.match(/- \*\*Pattern ID\*\*: (.+)/);
    if (idMatch) metadata.pattern_id = idMatch[1];

    const categoryMatch = content.match(/- \*\*Category\*\*: (.+)/);
    if (categoryMatch) metadata.category = categoryMatch[1];

    const freqMatch = content.match(/- \*\*Frequency\*\*: (\d+)/);
    if (freqMatch) metadata.frequency = parseInt(freqMatch[1], 10);

    const confMatch = content.match(/- \*\*Confidence\*\*: (\d+)%/);
    if (confMatch) metadata.confidence = parseInt(confMatch[1], 10);

    const statusMatch = content.match(/- \*\*Status\*\*: (.+)/);
    if (statusMatch) {
      const status = statusMatch[1].toLowerCase();
      if (status.includes('established')) metadata.status = 'established';
      else if (status.includes('deprecated')) metadata.status = 'deprecated';
    }

    return metadata as PatternMetadata;
  }

  /**
   * Get all extracted patterns
   */
  getPatterns(): ExtractedPattern[] {
    if (!fs.existsSync(this.patternsDir)) {
      return [];
    }

    const patterns: ExtractedPattern[] = [];
    const files = fs.readdirSync(this.patternsDir).filter(f => f.endsWith('.md'));

    for (const file of files) {
      const filePath = path.join(this.patternsDir, file);
      try {
        const content = fs.readFileSync(filePath, 'utf-8');
        const id = file.replace('.md', '');
        patterns.push({
          id,
          title: this.extractTitle(content),
          metadata: this.extractMetadata(content),
          content,
          filePath,
        });
      } catch (error) {
        // Ignore errors
      }
    }

    return patterns;
  }

  /**
   * Search patterns
   */
  searchPatterns(query: string): ExtractedPattern[] {
    const patterns = this.getPatterns();
    const lowerQuery = query.toLowerCase();

    return patterns.filter(pattern => {
      const searchable = [
        pattern.title,
        pattern.metadata.category,
        pattern.content,
      ].join(' ').toLowerCase();

      return searchable.includes(lowerQuery);
    });
  }
}


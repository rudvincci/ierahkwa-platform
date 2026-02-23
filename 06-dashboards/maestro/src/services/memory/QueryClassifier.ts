/**
 * Query Classifier
 * Classifies queries to optimize hybrid search weights
 * Ported from .skmemory/v1/api/query_classifier.py
 */

export enum QueryType {
  EXACT_MATCH = 'exact_match',
  CONCEPTUAL = 'conceptual',
  MIXED = 'mixed',
}

export interface QueryClassification {
  type: QueryType;
  confidence: number;
}

/**
 * Classifies queries to determine optimal hybrid search weights.
 * 
 * Uses heuristics to detect:
 * - Exact match queries: Specific terms, file names, quoted strings
 * - Conceptual queries: Natural language questions, descriptive queries
 * - Mixed queries: Combination of both
 */
export class QueryClassifier {
  // Patterns for exact match detection
  private static EXACT_MATCH_PATTERNS = [
    /"[^"]+"/,  // Quoted strings
    /'[^']+'/,  // Single-quoted strings
    /\b[A-Z][a-zA-Z0-9_]+\.(md|py|js|ts|cs|java|go|rs)\b/,  // File names
    /\b[A-Z][a-zA-Z0-9_]+\(\)/,  // Function names
    /^[A-Z_][A-Z0-9_]+$/,  // Constants/ENUMS
    /\b\d+\.\d+\.\d+/,  // Version numbers
    /#[a-zA-Z0-9_-]+/,  // Hashtags/IDs
  ];

  // Patterns for conceptual detection
  private static CONCEPTUAL_PATTERNS = [
    /\b(how|what|why|when|where|which|who)\s+(to|is|are|do|does|did|can|could|should|will|would)\b/i,
    /\b(explain|describe|understand|learn|tutorial|guide|example|help)\b/i,
    /\b(show|find|get|retrieve|search|look\s+for)\s+(me|all|some|any)\b/i,
    /\b(best\s+practice|pattern|approach|method|way|solution)\b/i,
    /\b(error|issue|problem|bug|fix|troubleshoot|debug)\b/i,
    /\b(configure|setup|install|deploy|run|start|stop)\b/i,
  ];

  // Technical terms that suggest exact match
  private static TECHNICAL_TERMS = [
    /\b(API|URL|URI|HTTP|HTTPS|JSON|XML|YAML|SQL|NoSQL)\b/i,
    /\b(class|function|method|variable|constant|enum|interface|type)\b/i,
    /\b(config|config\.|settings|options|parameters|args|kwargs)\b/i,
  ];

  private exactMatchRegex: RegExp[];
  private conceptualRegex: RegExp[];
  private technicalRegex: RegExp[];

  constructor() {
    this.exactMatchRegex = QueryClassifier.EXACT_MATCH_PATTERNS;
    this.conceptualRegex = QueryClassifier.CONCEPTUAL_PATTERNS;
    this.technicalRegex = QueryClassifier.TECHNICAL_TERMS;
  }

  classifyQuery(query: string): QueryClassification {
    if (!query || !query.trim()) {
      return { type: QueryType.MIXED, confidence: 0.5 };
    }

    const queryLower = query.toLowerCase().trim();
    let exactScore = 0.0;
    let conceptualScore = 0.0;
    let technicalScore = 0.0;

    // Check for exact match patterns
    for (const pattern of this.exactMatchRegex) {
      const matches = query.match(pattern);
      if (matches) {
        exactScore += matches.length * 0.3;
        // Quoted strings are strong indicators
        if (query.includes('"') || query.includes("'")) {
          exactScore += 0.4;
        }
      }
    }

    // Check for conceptual patterns
    for (const pattern of this.conceptualRegex) {
      if (pattern.test(query)) {
        conceptualScore += 0.3;
      }
    }

    // Check for technical terms (can go either way, but lean toward exact)
    for (const pattern of this.technicalRegex) {
      if (pattern.test(query)) {
        technicalScore += 0.2;
      }
    }

    // Normalize scores
    exactScore = Math.min(exactScore, 1.0);
    conceptualScore = Math.min(conceptualScore, 1.0);
    technicalScore = Math.min(technicalScore, 1.0);

    // Add technical score to exact (technical terms suggest exact match)
    exactScore = Math.min(exactScore + technicalScore * 0.5, 1.0);

    // Determine query type
    const scoreDiff = Math.abs(exactScore - conceptualScore);
    const confidence = Math.max(exactScore, conceptualScore);

    if (scoreDiff < 0.2) {
      // Scores are close, it's mixed
      return { type: QueryType.MIXED, confidence: Math.max(confidence, 0.5) };
    } else if (exactScore > conceptualScore) {
      return { type: QueryType.EXACT_MATCH, confidence };
    } else {
      return { type: QueryType.CONCEPTUAL, confidence };
    }
  }

  /**
   * Get hybrid search weights based on query classification.
   * Returns weights for [keyword_search, semantic_search]
   */
  getHybridWeights(query: string): [number, number] {
    const classification = this.classifyQuery(query);

    switch (classification.type) {
      case QueryType.EXACT_MATCH:
        return [0.8, 0.2]; // Favor keyword search
      case QueryType.CONCEPTUAL:
        return [0.2, 0.8]; // Favor semantic search
      case QueryType.MIXED:
      default:
        return [0.5, 0.5]; // Balanced
    }
  }
}


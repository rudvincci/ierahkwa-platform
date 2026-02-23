/**
 * Centralized Error Handler
 * Provides comprehensive error handling, classification, and recovery
 */

export enum ErrorType {
  TRANSIENT = 'transient',
  PERMANENT = 'permanent',
  RECOVERABLE = 'recoverable',
  FATAL = 'fatal',
}

export enum ErrorCategory {
  NETWORK = 'network',
  DATABASE = 'database',
  FILE_SYSTEM = 'file_system',
  PERMISSION = 'permission',
  VALIDATION = 'validation',
  BUSINESS_LOGIC = 'business_logic',
  EXTERNAL_SERVICE = 'external_service',
  UNKNOWN = 'unknown',
}

export interface ErrorContext {
  error: Error;
  type: ErrorType;
  category: ErrorCategory;
  timestamp: Date;
  context?: Record<string, any>;
  retryable: boolean;
  retryCount?: number;
  maxRetries?: number;
}

export interface ErrorReport {
  id: string;
  error: ErrorContext;
  resolved: boolean;
  resolution?: string;
  reportedAt: Date;
  resolvedAt?: Date;
}

/**
 * Centralized error handler
 */
export class ErrorHandler {
  private errorReports: Map<string, ErrorReport> = new Map();
  private errorAggregation: Map<string, number> = new Map();

  /**
   * Handle and classify error
   */
  handleError(error: Error, context?: Record<string, any>): ErrorContext {
    const errorContext: ErrorContext = {
      error,
      type: this.classifyErrorType(error),
      category: this.classifyErrorCategory(error),
      timestamp: new Date(),
      context,
      retryable: this.isRetryable(error),
      maxRetries: this.getMaxRetries(error),
    };

    // Aggregate errors
    const errorKey = `${errorContext.category}:${errorContext.type}`;
    this.errorAggregation.set(errorKey, (this.errorAggregation.get(errorKey) || 0) + 1);

    // Create error report
    const reportId = this.generateReportId();
    const report: ErrorReport = {
      id: reportId,
      error: errorContext,
      resolved: false,
      reportedAt: new Date(),
    };
    this.errorReports.set(reportId, report);

    return errorContext;
  }

  /**
   * Classify error type
   */
  private classifyErrorType(error: Error): ErrorType {
    const message = error.message.toLowerCase();
    const name = error.name.toLowerCase();

    // Network errors are usually transient
    if (name.includes('network') || name.includes('timeout') || name.includes('econnrefused')) {
      return ErrorType.TRANSIENT;
    }

    // Permission errors are usually permanent
    if (name.includes('permission') || name.includes('eacces') || name.includes('eperm')) {
      return ErrorType.PERMANENT;
    }

    // Validation errors are usually recoverable
    if (name.includes('validation') || name.includes('invalid') || name.includes('typeerror')) {
      return ErrorType.RECOVERABLE;
    }

    // Fatal errors
    if (name.includes('fatal') || name.includes('outofmemory') || name.includes('stackoverflow')) {
      return ErrorType.FATAL;
    }

    // Default to recoverable
    return ErrorType.RECOVERABLE;
  }

  /**
   * Classify error category
   */
  private classifyErrorCategory(error: Error): ErrorCategory {
    const message = error.message.toLowerCase();
    const name = error.name.toLowerCase();

    if (name.includes('network') || message.includes('connection') || message.includes('timeout')) {
      return ErrorCategory.NETWORK;
    }

    if (name.includes('database') || message.includes('sql') || message.includes('mongodb') || message.includes('postgres')) {
      return ErrorCategory.DATABASE;
    }

    if (name.includes('file') || message.includes('enoent') || message.includes('eisdir') || message.includes('enotdir')) {
      return ErrorCategory.FILE_SYSTEM;
    }

    if (name.includes('permission') || message.includes('eacces') || message.includes('eperm')) {
      return ErrorCategory.PERMISSION;
    }

    if (name.includes('validation') || message.includes('invalid')) {
      return ErrorCategory.VALIDATION;
    }

    if (message.includes('api') || message.includes('service') || message.includes('http')) {
      return ErrorCategory.EXTERNAL_SERVICE;
    }

    return ErrorCategory.UNKNOWN;
  }

  /**
   * Determine if error is retryable
   */
  private isRetryable(error: Error): boolean {
    const type = this.classifyErrorType(error);
    return type === ErrorType.TRANSIENT || type === ErrorType.RECOVERABLE;
  }

  /**
   * Get max retries for error
   */
  private getMaxRetries(error: Error): number {
    const type = this.classifyErrorType(error);
    const category = this.classifyErrorCategory(error);

    if (type === ErrorType.TRANSIENT && category === ErrorCategory.NETWORK) {
      return 5; // Network errors: 5 retries
    }

    if (type === ErrorType.RECOVERABLE) {
      return 3; // Recoverable errors: 3 retries
    }

    return 0; // No retries for permanent/fatal errors
  }

  /**
   * Generate unique report ID
   */
  private generateReportId(): string {
    return `err-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
  }

  /**
   * Get error reports
   */
  getErrorReports(limit?: number): ErrorReport[] {
    const reports = Array.from(this.errorReports.values())
      .sort((a, b) => b.reportedAt.getTime() - a.reportedAt.getTime());
    return limit ? reports.slice(0, limit) : reports;
  }

  /**
   * Get error aggregation
   */
  getErrorAggregation(): Record<string, number> {
    const result: Record<string, number> = {};
    for (const [key, count] of this.errorAggregation.entries()) {
      result[key] = count;
    }
    return result;
  }

  /**
   * Resolve error report
   */
  resolveError(reportId: string, resolution?: string): void {
    const report = this.errorReports.get(reportId);
    if (report) {
      report.resolved = true;
      report.resolution = resolution;
      report.resolvedAt = new Date();
    }
  }

  /**
   * Clear old error reports
   */
  clearOldReports(maxAge: number = 7 * 24 * 60 * 60 * 1000): number {
    const cutoff = Date.now() - maxAge;
    let cleared = 0;

    for (const [id, report] of this.errorReports.entries()) {
      if (report.reportedAt.getTime() < cutoff) {
        this.errorReports.delete(id);
        cleared++;
      }
    }

    return cleared;
  }
}


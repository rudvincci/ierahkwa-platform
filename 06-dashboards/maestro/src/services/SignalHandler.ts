/**
 * Signal Handler Service
 * 
 * Handles graceful shutdown signals (SIGINT, SIGTERM) to allow
 * stopping workflows and saving state.
 */

import { StateManager } from './StateManager';

export interface ShutdownHandler {
  onShutdown(callback: () => Promise<void>): void;
  isShuttingDown(): boolean;
  requestShutdown(): void;
}

export class SignalHandler implements ShutdownHandler {
  private shutdownCallbacks: Array<() => Promise<void>> = [];
  private isShuttingDownFlag: boolean = false;
  private shutdownRequested: boolean = false;

  constructor() {
    this.setupSignalHandlers();
  }

  /**
   * Setup signal handlers for graceful shutdown
   */
  private setupSignalHandlers(): void {
    // Handle SIGINT (Ctrl+C)
    process.on('SIGINT', () => {
      console.log('\n\n⚠️  SIGINT received. Initiating graceful shutdown...');
      this.handleShutdown();
    });

    // Handle SIGTERM
    process.on('SIGTERM', () => {
      console.log('\n\n⚠️  SIGTERM received. Initiating graceful shutdown...');
      this.handleShutdown();
    });

    // Handle uncaught exceptions
    process.on('uncaughtException', (error) => {
      console.error('\n\n❌ Uncaught exception:', error);
      this.handleShutdown();
    });

    // Handle unhandled promise rejections
    process.on('unhandledRejection', (reason, promise) => {
      console.error('\n\n❌ Unhandled rejection:', reason);
      this.handleShutdown();
    });
  }

  /**
   * Register a shutdown callback
   */
  onShutdown(callback: () => Promise<void>): void {
    this.shutdownCallbacks.push(callback);
  }

  /**
   * Check if shutdown is in progress
   */
  isShuttingDown(): boolean {
    return this.isShuttingDownFlag;
  }

  /**
   * Request shutdown programmatically
   */
  requestShutdown(): void {
    if (!this.isShuttingDownFlag) {
      console.log('\n\n⚠️  Shutdown requested. Initiating graceful shutdown...');
      this.handleShutdown();
    }
  }

  /**
   * Handle shutdown process
   */
  private async handleShutdown(): Promise<void> {
    if (this.isShuttingDownFlag) {
      return; // Already shutting down
    }

    this.isShuttingDownFlag = true;
    this.shutdownRequested = true;

    console.log('\n💾 Saving state and cleaning up...\n');

    try {
      // Execute all shutdown callbacks
      const promises = this.shutdownCallbacks.map(async (callback) => {
        try {
          await callback();
        } catch (error) {
          console.error('Error in shutdown callback:', error);
        }
      });

      await Promise.all(promises);

      console.log('✅ Graceful shutdown complete.');
      console.log('💡 Use --resume <checkpoint-id> to continue where you left off.\n');
    } catch (error) {
      console.error('❌ Error during shutdown:', error);
    } finally {
      process.exit(0);
    }
  }

  /**
   * Check if shutdown was requested
   */
  wasShutdownRequested(): boolean {
    return this.shutdownRequested;
  }
}

// Singleton instance
let signalHandlerInstance: SignalHandler | null = null;

export function getSignalHandler(): SignalHandler {
  if (!signalHandlerInstance) {
    signalHandlerInstance = new SignalHandler();
  }
  return signalHandlerInstance;
}

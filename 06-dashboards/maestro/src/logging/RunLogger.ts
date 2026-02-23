import * as fs from 'fs';
import * as path from 'path';
import { RunLog } from '../domain/RunLog';

export class RunLogger {
  private logDir: string;

  constructor(logDir?: string) {
    // Use process.cwd() relative path for logs
    this.logDir = logDir || path.join(process.cwd(), '.maestro/logs');
    this.ensureLogDirectory();
  }

  async logRun(runLog: RunLog): Promise<void> {
    const logPath = this.getLogPath(runLog.runId);
    const logData = JSON.stringify(runLog, null, 2);
    
    fs.writeFileSync(logPath, logData, 'utf-8');
  }

  getLogPath(runId: string): string {
    return path.join(this.logDir, `${runId}.json`);
  }

  private ensureLogDirectory(): void {
    if (!fs.existsSync(this.logDir)) {
      fs.mkdirSync(this.logDir, { recursive: true });
    }
  }
}


/**
 * System Information Service
 * 
 * Tracks and provides system information about the Maestro process
 */

import * as os from 'os';
import * as process from 'process';
import * as fs from 'fs';
import { getHeapMemorySize } from '../cli/utils';

export interface SystemInfo {
  processId: number;
  uptime: number; // seconds
  memory: {
    used: number; // bytes
    total: number; // bytes
    percentage: number; // 0-100
    heapUsed: number; // bytes (Node.js heap)
    heapTotal: number; // bytes (Node.js heap)
    heapLimit: number; // bytes (configured max heap size)
    external: number; // bytes (external memory)
    rss: number; // Resident Set Size
    arrayBuffers: number; // Array buffers memory
  };
  cpu: {
    usage: number; // percentage (0-100)
    count: number; // number of CPU cores
    model: string; // CPU model
    speed: number; // CPU speed in MHz
  };
  network: {
    bytesSent: number;
    bytesReceived: number;
    connections: number;
  };
  disk: {
    readOps: number;
    writeOps: number;
    spaceUsed: number; // bytes
    spaceTotal: number; // bytes
    spaceFree: number; // bytes
  };
  processTree: Array<{
    pid: number;
    name: string;
    ppid: number;
    cpu: number;
    memory: number;
    command: string;
  }>;
  eventLoop: {
    lag: number; // milliseconds
    utilization: number; // 0-1
  };
  activeHandles: number;
  activeRequests: number;
  platform: {
    type: string; // 'darwin', 'linux', 'win32', etc.
    arch: string; // 'x64', 'arm64', etc.
    hostname: string;
    nodeVersion: string;
    platform: string;
    release: string;
  };
  environment: Record<string, string>; // Filtered, non-sensitive env vars
  timestamp: Date;
}

export class SystemInfoService {
  private startTime: number;
  private lastCpuUsage: NodeJS.CpuUsage | null = null;
  private lastCpuTime: number = 0;
  private lastNetworkBytes: { sent: number; received: number } = { sent: 0, received: 0 };
  private lastDiskStats: { readOps: number; writeOps: number } = { readOps: 0, writeOps: 0 };
  private eventLoopLag: number = 0;
  private eventLoopUtilization: number = 0;

  constructor() {
    this.startTime = Date.now();
    this.startEventLoopMonitoring();
  }

  private startEventLoopMonitoring(): void {
    // Monitor event loop lag
    setInterval(() => {
      const start = process.hrtime.bigint();
      setImmediate(() => {
        const delta = process.hrtime.bigint() - start;
        this.eventLoopLag = Number(delta) / 1_000_000; // Convert to milliseconds
      });
    }, 1000);

    // Monitor event loop utilization (if available)
    if (typeof (process as any).resourceUsage === 'function') {
      setInterval(() => {
        try {
          const usage = (process as any).resourceUsage();
          // Calculate utilization (simplified)
          this.eventLoopUtilization = Math.min(1, (usage.userCPUTime + usage.systemCPUTime) / 1000000);
        } catch (error) {
          // Ignore errors
        }
      }, 1000);
    }
  }

  /**
   * Get current system information
   */
  getSystemInfo(): SystemInfo {
    const memUsage = process.memoryUsage();
    const totalMem = os.totalmem();
    const freeMem = os.freemem();
    const usedMem = totalMem - freeMem;
    
    // Get configured heap memory limit (in MB, convert to bytes)
    const heapLimitMB = getHeapMemorySize();
    const heapLimitBytes = heapLimitMB * 1024 * 1024;
    
    // Calculate CPU usage
    const cpuUsage = this.calculateCpuUsage();
    const cpus = os.cpus();
    
    // Get network stats (simplified - would need actual network monitoring)
    const networkStats = this.getNetworkStats();
    
    // Get disk stats
    const diskStats = this.getDiskStats();
    
    // Get process tree
    const processTree = this.getProcessTree();
    
    // Get environment variables (filtered)
    const environment = this.getFilteredEnvironment();
    
    return {
      processId: process.pid,
      uptime: Math.floor((Date.now() - this.startTime) / 1000),
      memory: {
        used: usedMem,
        total: totalMem,
        percentage: (usedMem / totalMem) * 100,
        heapUsed: memUsage.heapUsed,
        heapTotal: memUsage.heapTotal,
        heapLimit: heapLimitBytes,
        external: memUsage.external,
        rss: memUsage.rss,
        arrayBuffers: (memUsage as any).arrayBuffers || 0,
      },
      cpu: {
        usage: cpuUsage,
        count: cpus.length,
        model: cpus[0]?.model || 'Unknown',
        speed: cpus[0]?.speed || 0,
      },
      network: networkStats,
      disk: diskStats,
      processTree,
      eventLoop: {
        lag: this.eventLoopLag,
        utilization: this.eventLoopUtilization,
      },
      activeHandles: (process as any)._getActiveHandles?.()?.length || 0,
      activeRequests: (process as any)._getActiveRequests?.()?.length || 0,
      platform: {
        type: os.type(),
        arch: os.arch(),
        hostname: os.hostname(),
        nodeVersion: process.version,
        platform: os.platform(),
        release: os.release(),
      },
      environment,
      timestamp: new Date(),
    };
  }

  private getNetworkStats(): { bytesSent: number; bytesReceived: number; connections: number } {
    // Network stats would require actual network monitoring
    // For now, return placeholder values
    // In production, you'd use a library like `systeminformation` or monitor network interfaces
    return {
      bytesSent: 0,
      bytesReceived: 0,
      connections: 0,
    };
  }

  private getDiskStats(): { readOps: number; writeOps: number; spaceUsed: number; spaceTotal: number; spaceFree: number } {
    try {
      const stats = fs.statSync(process.cwd());
      // Get disk space (simplified - would need actual disk monitoring)
      // In production, use a library like `systeminformation`
      return {
        readOps: 0,
        writeOps: 0,
        spaceUsed: 0,
        spaceTotal: 0,
        spaceFree: 0,
      };
    } catch (error) {
      return {
        readOps: 0,
        writeOps: 0,
        spaceUsed: 0,
        spaceTotal: 0,
        spaceFree: 0,
      };
    }
  }

  private getProcessTree(): Array<{ pid: number; name: string; ppid: number; cpu: number; memory: number; command: string }> {
    try {
      const { execSync } = require('child_process');
      const tree: Array<{ pid: number; name: string; ppid: number; cpu: number; memory: number; command: string }> = [];
      
      // Get current process
      tree.push({
        pid: process.pid,
        name: 'node',
        ppid: process.ppid || 0,
        cpu: 0,
        memory: process.memoryUsage().rss,
        command: process.argv.join(' '),
      });

      // Try to get child processes (platform-specific)
      if (process.platform === 'darwin' || process.platform === 'linux') {
        try {
          const output = execSync(`pgrep -P ${process.pid}`, { encoding: 'utf-8', maxBuffer: 1024 * 1024 });
          const childPids = output.trim().split('\n').filter((p: string) => p);
          
          for (const pidStr of childPids) {
            const pid = parseInt(pidStr, 10);
            if (!isNaN(pid)) {
              try {
                const psOutput = execSync(`ps -p ${pid} -o comm=,pid=,ppid=,pcpu=,rss=,command=`, {
                  encoding: 'utf-8',
                  maxBuffer: 1024 * 1024,
                });
                const parts = psOutput.trim().split(/\s+/);
                if (parts.length >= 6) {
                  tree.push({
                    pid,
                    name: parts[0],
                    ppid: parseInt(parts[2], 10) || 0,
                    cpu: parseFloat(parts[3]) || 0,
                    memory: parseInt(parts[4], 10) * 1024 || 0, // Convert KB to bytes
                    command: parts.slice(5).join(' '),
                  });
                }
              } catch (error) {
                // Ignore errors for individual processes
              }
            }
          }
        } catch (error) {
          // Ignore errors
        }
      }

      return tree;
    } catch (error) {
      return [];
    }
  }

  private getFilteredEnvironment(): Record<string, string> {
    const env: Record<string, string> = {};
    const sensitiveKeys = ['PASSWORD', 'SECRET', 'KEY', 'TOKEN', 'API_KEY', 'AUTH', 'CREDENTIAL'];
    
    for (const [key, value] of Object.entries(process.env)) {
      // Filter out sensitive environment variables
      const upperKey = key.toUpperCase();
      if (!sensitiveKeys.some(sk => upperKey.includes(sk))) {
        env[key] = value || '';
      }
    }
    
    return env;
  }

  /**
   * Calculate CPU usage percentage
   * Note: This is a simplified calculation. For more accurate results,
   * you'd need to measure over time intervals.
   */
  private calculateCpuUsage(): number {
    const currentUsage = process.cpuUsage();
    const currentTime = Date.now();

    if (this.lastCpuUsage === null) {
      this.lastCpuUsage = currentUsage;
      this.lastCpuTime = currentTime;
      return 0;
    }

    const elapsedTime = (currentTime - this.lastCpuTime) / 1000; // seconds
    const userDiff = currentUsage.user - this.lastCpuUsage.user;
    const systemDiff = currentUsage.system - this.lastCpuUsage.system;
    const totalDiff = (userDiff + systemDiff) / 1000000; // Convert to seconds

    this.lastCpuUsage = currentUsage;
    this.lastCpuTime = currentTime;

    // CPU usage as percentage (simplified - assumes single core equivalent)
    // For multi-core, this would need more complex calculation
    const cpuPercent = (totalDiff / elapsedTime) * 100;
    
    return Math.min(100, Math.max(0, cpuPercent));
  }

  /**
   * Format bytes to human-readable string
   */
  formatBytes(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  }

  /**
   * Format seconds to human-readable duration
   */
  formatUptime(seconds: number): string {
    const days = Math.floor(seconds / 86400);
    const hours = Math.floor((seconds % 86400) / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;

    if (days > 0) {
      return `${days}d ${hours}h ${minutes}m`;
    } else if (hours > 0) {
      return `${hours}h ${minutes}m ${secs}s`;
    } else if (minutes > 0) {
      return `${minutes}m ${secs}s`;
    } else {
      return `${secs}s`;
    }
  }
}

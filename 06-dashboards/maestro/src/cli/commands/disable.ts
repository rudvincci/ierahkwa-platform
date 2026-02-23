/**
 * Disable command - Stop the orchestrator dashboard
 */

import * as http from 'http';
import * as fs from 'fs';
import * as path from 'path';
import { findRepoRoot } from '../utils';

const PID_FILE = '.maestro/dashboard.pid';
const DASHBOARD_PORT = 3000;
const MCP_PORT = 3001;

export async function disableOrchestrator(): Promise<void> {
  const repoRoot = findRepoRoot();
  const pidFile = path.join(repoRoot, PID_FILE);

  console.log('🛑 Disabling Maestro Orchestrator...');

  let pid: number | null = null;
  let foundViaPidFile = false;

  // First, try to read PID from file
  if (fs.existsSync(pidFile)) {
    try {
      const pidFromFile = parseInt(fs.readFileSync(pidFile, 'utf-8').trim(), 10);
      if (!isNaN(pidFromFile)) {
        // Check if process exists
        try {
          process.kill(pidFromFile, 0); // Signal 0 checks if process exists
          pid = pidFromFile;
          foundViaPidFile = true;
        } catch {
          // Process is dead, remove stale PID file
          fs.unlinkSync(pidFile);
        }
      }
    } catch (error) {
      // Invalid PID file, continue to port check
    }
  }

  // If no PID from file, try to find by port
  if (!pid) {
    try {
      pid = await findPidByPort(DASHBOARD_PORT);
      if (pid) {
        console.log(`ℹ️  Found dashboard process on port ${DASHBOARD_PORT} (PID: ${pid})`);
      }
    } catch (error) {
      // Port not in use or can't find PID
    }
  }

  // If still no PID, check if port is in use at all
  if (!pid) {
    const portInUse = await checkPortInUse(DASHBOARD_PORT);
    if (!portInUse) {
      console.log('ℹ️  Dashboard is not running (no PID file and port 3000 is not in use)');
      // Still check for MCP server
      await stopMcpServer();
      process.exit(0);
      return;
    }
    // Port is in use but we couldn't find PID - try killByPort anyway
    try {
      await killByPort(DASHBOARD_PORT);
      console.log(`✅ Stopped process on port ${DASHBOARD_PORT}`);
      // Clean up PID file if it exists
      if (fs.existsSync(pidFile)) {
        fs.unlinkSync(pidFile);
      }
      // Also stop MCP server
      await stopMcpServer();
      process.exit(0);
      return;
    } catch (error) {
      console.log('ℹ️  Could not stop dashboard process');
      // Still try to stop MCP server
      await stopMcpServer();
      process.exit(0);
      return;
    }
  }

  // We have a PID, kill it
  if (!pid) {
    console.log('ℹ️  No dashboard process found');
    if (fs.existsSync(pidFile)) {
      fs.unlinkSync(pidFile);
    }
    // Still check for MCP server
    await stopMcpServer();
    process.exit(0);
    return;
  }

  // Kill the dashboard process
  try {
    process.kill(pid, 'SIGTERM');
    if (foundViaPidFile) {
      console.log(`✅ Stopped dashboard (PID: ${pid})`);
    } else {
      console.log(`✅ Stopped dashboard process (PID: ${pid})`);
    }
    
    // Wait a bit, then force kill if still running
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Check if still running and force kill if needed
    try {
      process.kill(pid, 0); // Check if still exists
      process.kill(pid, 'SIGKILL');
      console.log(`⚠️  Force killed dashboard (PID: ${pid})`);
    } catch {
      // Process already dead, good
    }
  } catch (error: any) {
    if (error.code === 'ESRCH') {
      console.log('ℹ️  Process not found (may have already stopped)');
    } else {
      console.error('❌ Error stopping process:', error.message);
    }
  } finally {
    // Clean up PID file
    if (fs.existsSync(pidFile)) {
      try {
        fs.unlinkSync(pidFile);
      } catch {
        // Ignore cleanup errors
      }
    }
    
    // Stop MCP server
    await stopMcpServer();
    
    process.exit(0);
  }
}

/**
 * Stop MCP server by finding and killing process on port 3001
 */
async function stopMcpServer(): Promise<void> {
  console.log('🔌 Stopping MCP Server...');
  
  try {
    // Check if port 3001 is in use
    const portInUse = await checkPortInUse(MCP_PORT);
    if (!portInUse) {
      console.log('ℹ️  MCP Server is not running (port 3001 is not in use)');
      return;
    }
    
    // Find PID by port
    const mcpPid = await findPidByPort(MCP_PORT);
    if (!mcpPid) {
      // Port is in use but can't find PID - try killByPort
      try {
        await killByPort(MCP_PORT);
        console.log(`✅ Stopped MCP Server on port ${MCP_PORT}`);
      } catch (error) {
        console.log('ℹ️  Could not stop MCP Server process');
      }
      return;
    }
    
    // Kill the MCP server process
    try {
      process.kill(mcpPid, 'SIGTERM');
      console.log(`✅ Stopped MCP Server (PID: ${mcpPid})`);
      
      // Wait a bit, then force kill if still running
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      // Check if still running and force kill if needed
      try {
        process.kill(mcpPid, 0); // Check if still exists
        process.kill(mcpPid, 'SIGKILL');
        console.log(`⚠️  Force killed MCP Server (PID: ${mcpPid})`);
      } catch {
        // Process already dead, good
      }
    } catch (error: any) {
      if (error.code === 'ESRCH') {
        console.log('ℹ️  MCP Server process not found (may have already stopped)');
      } else {
        console.error('❌ Error stopping MCP Server:', error.message);
      }
    }
  } catch (error: any) {
    console.log('ℹ️  Could not stop MCP Server:', error.message);
  }
}

async function findPidByPort(port: number): Promise<number | null> {
  return new Promise((resolve) => {
    const { exec } = require('child_process');
    const platform = process.platform;
    
    let command: string;
    if (platform === 'win32') {
      command = `netstat -ano | findstr :${port}`;
    } else {
      command = `lsof -ti:${port}`;
    }
    
    exec(command, (error: any, stdout: string) => {
      if (error || !stdout.trim()) {
        resolve(null);
        return;
      }
      
      const pids = stdout.trim().split('\n').map(line => {
        if (platform === 'win32') {
          const parts = line.trim().split(/\s+/);
          return parts[parts.length - 1];
        }
        return line.trim();
      }).filter(pid => pid && !isNaN(parseInt(pid, 10)));
      
      if (pids.length === 0) {
        resolve(null);
        return;
      }
      
      // Return the first valid PID
      resolve(parseInt(pids[0], 10));
    });
  });
}

async function checkPortInUse(port: number): Promise<boolean> {
  return new Promise((resolve) => {
    const testReq = http.request(
      { host: 'localhost', port, method: 'HEAD', timeout: 1000 },
      () => {
        testReq.destroy();
        resolve(true);
      }
    );
    
    testReq.on('error', () => {
      resolve(false);
    });
    
    testReq.on('timeout', () => {
      testReq.destroy();
      resolve(false);
    });
    
    testReq.end();
  });
}

async function killByPort(port: number): Promise<void> {
  return new Promise((resolve, reject) => {
    // Try to connect to see if port is in use
    const testReq = http.request(
      { host: 'localhost', port, method: 'HEAD', timeout: 1000 },
      () => {
        // Port is in use - try to find PID using lsof (macOS/Linux) or netstat (Windows)
        const { exec } = require('child_process');
        const platform = process.platform;
        
        let command: string;
        if (platform === 'win32') {
          command = `netstat -ano | findstr :${port}`;
        } else {
          command = `lsof -ti:${port}`;
        }
        
        exec(command, (error: any, stdout: string) => {
          if (error || !stdout.trim()) {
            reject(new Error('Port in use but cannot determine PID'));
            return;
          }
          
          const pids = stdout.trim().split('\n').map(line => {
            if (platform === 'win32') {
              const parts = line.trim().split(/\s+/);
              return parts[parts.length - 1];
            }
            return line.trim();
          }).filter(pid => pid && !isNaN(parseInt(pid, 10)));
          
          if (pids.length === 0) {
            reject(new Error('No valid PID found'));
            return;
          }
          
          // Kill all processes on this port
          pids.forEach(pid => {
            try {
              if (platform === 'win32') {
                exec(`taskkill /PID ${pid} /F`);
              } else {
                process.kill(parseInt(pid, 10), 'SIGTERM');
              }
            } catch (e) {
              // Ignore errors
            }
          });
          
          resolve();
        });
      }
    );
    
    testReq.on('error', () => {
      // Port not in use
      reject(new Error('Port not in use'));
    });
    
    testReq.on('timeout', () => {
      testReq.destroy();
      reject(new Error('Timeout checking port'));
    });
    
    testReq.end();
  });
}

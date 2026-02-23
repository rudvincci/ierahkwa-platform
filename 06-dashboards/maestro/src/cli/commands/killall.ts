/**
 * Kill All Command - Forcefully kill all Maestro-related processes
 */

import { execSync } from 'child_process';
import { findRepoRoot } from '../utils';
import * as path from 'path';
import * as fs from 'fs';

const PID_FILE = '.maestro/dashboard.pid';

export async function killAllMaestroProcesses(): Promise<void> {
  const repoRoot = findRepoRoot();
  const pidFile = path.join(repoRoot, PID_FILE);
  
  console.log('🔍 Searching for Maestro processes...\n');

  const killedPids: number[] = [];
  const platform = process.platform;

  try {
    // Method 1: Kill by PID file
    if (fs.existsSync(pidFile)) {
      try {
        const pid = parseInt(fs.readFileSync(pidFile, 'utf-8').trim(), 10);
        if (!isNaN(pid)) {
          try {
            process.kill(pid, 0); // Check if process exists
            console.log(`📌 Found process from PID file: ${pid}`);
            try {
              process.kill(pid, 'SIGTERM');
              console.log(`   ✅ Sent SIGTERM to PID ${pid}`);
              killedPids.push(pid);
              // Wait a moment
              await new Promise(resolve => setTimeout(resolve, 1000));
              // Force kill if still alive
              try {
                process.kill(pid, 0);
                process.kill(pid, 'SIGKILL');
                console.log(`   🔪 Force killed PID ${pid}`);
              } catch {
                // Already dead
              }
            } catch (error: any) {
              if (error.code !== 'ESRCH') {
                console.log(`   ⚠️  Could not kill PID ${pid}: ${error.message}`);
              }
            }
          } catch {
            console.log(`   ℹ️  PID ${pid} from file is not running`);
          }
        }
      } catch (error) {
        console.log(`   ⚠️  Could not read PID file: ${error}`);
      }
    }

    // Method 2: Find processes by port
    const ports = [3000, 3001]; // Dashboard and MCP ports
    
    for (const port of ports) {
      try {
        let pid: number | null = null;
        
        if (platform === 'darwin' || platform === 'linux') {
          // Use lsof on macOS/Linux
          try {
            const result = execSync(`lsof -ti:${port}`, { encoding: 'utf-8', timeout: 2000 }).trim();
            if (result) {
              pid = parseInt(result.split('\n')[0], 10);
            }
          } catch {
            // Port not in use or lsof not available
          }
        } else if (platform === 'win32') {
          // Use netstat on Windows
          try {
            const result = execSync(`netstat -ano | findstr :${port}`, { encoding: 'utf-8', timeout: 2000 });
            const lines = result.split('\n');
            for (const line of lines) {
              const match = line.match(/\s+(\d+)\s*$/);
              if (match) {
                pid = parseInt(match[1], 10);
                break;
              }
            }
          } catch {
            // Port not in use
          }
        }

        if (pid && !isNaN(pid) && !killedPids.includes(pid)) {
          console.log(`📌 Found process on port ${port}: PID ${pid}`);
          try {
            process.kill(pid, 'SIGTERM');
            console.log(`   ✅ Sent SIGTERM to PID ${pid}`);
            killedPids.push(pid);
            await new Promise(resolve => setTimeout(resolve, 1000));
            try {
              process.kill(pid, 0);
              process.kill(pid, 'SIGKILL');
              console.log(`   🔪 Force killed PID ${pid}`);
            } catch {
              // Already dead
            }
          } catch (error: any) {
            if (error.code !== 'ESRCH') {
              console.log(`   ⚠️  Could not kill PID ${pid}: ${error.message}`);
            }
          }
        }
      } catch (error) {
        // Ignore errors
      }
    }

    // Method 3: Find by process name/command
    try {
      let pids: number[] = [];
      
      if (platform === 'darwin' || platform === 'linux') {
        // Find node processes that might be Maestro
        // Look for node processes with .maestro in the path or running dashboard/mcp commands
        try {
          // Use ps with full command line (auxww) to see full paths
          const result = execSync('ps auxww | grep -E "node.*\.maestro|node.*maestro.*(dashboard|mcp)|node.*dist.*index\.js.*(dashboard|mcp)" | grep -v grep', { 
            encoding: 'utf-8', 
            timeout: 2000 
          });
          
          const lines = result.split('\n').filter(line => line.trim());
          for (const line of lines) {
            const parts = line.trim().split(/\s+/);
            if (parts.length > 1) {
              const pid = parseInt(parts[1], 10);
              if (!isNaN(pid) && pid !== process.pid && !killedPids.includes(pid)) {
                // Check if it's actually a Maestro process (not cursor-agent)
                const cmd = line.toLowerCase();
                // Match if it contains .maestro path in the command
                // Exclude cursor-agent processes
                if (!cmd.includes('cursor-agent') && 
                    (cmd.includes('.maestro/') || 
                     cmd.includes('/.maestro/') ||
                     (cmd.includes('dist/cli/index.js') && (cmd.includes('dashboard') || cmd.includes('mcp'))))) {
                  console.log(`📌 Found Maestro node process: PID ${pid}`);
                  console.log(`   Command: ${line.substring(line.indexOf('node'))}`);
                  pids.push(pid);
                }
              }
            }
          }
        } catch {
          // ps command failed or no matches - this is OK
        }
      } else if (platform === 'win32') {
        // Windows: use tasklist
        try {
          const result = execSync('tasklist /FI "IMAGENAME eq node.exe" /FO CSV', { 
            encoding: 'utf-8', 
            timeout: 2000 
          });
          // Parse CSV and check command line (would need wmic for full command)
          // For now, just note that we'd need to check each node.exe process
        } catch {
          // tasklist failed
        }
      }

      for (const pid of pids) {
        if (!killedPids.includes(pid)) {
          console.log(`📌 Found Maestro-related process: PID ${pid}`);
          try {
            process.kill(pid, 'SIGTERM');
            console.log(`   ✅ Sent SIGTERM to PID ${pid}`);
            killedPids.push(pid);
            await new Promise(resolve => setTimeout(resolve, 1000));
            try {
              process.kill(pid, 0);
              process.kill(pid, 'SIGKILL');
              console.log(`   🔪 Force killed PID ${pid}`);
            } catch {
              // Already dead
            }
          } catch (error: any) {
            if (error.code !== 'ESRCH') {
              console.log(`   ⚠️  Could not kill PID ${pid}: ${error.message}`);
            }
          }
        }
      }
    } catch (error) {
      // Ignore errors
    }

    // Clean up PID file
    if (fs.existsSync(pidFile)) {
      try {
        fs.unlinkSync(pidFile);
        console.log(`\n🧹 Cleaned up PID file`);
      } catch {
        // Ignore
      }
    }

    if (killedPids.length > 0) {
      console.log(`\n✅ Killed ${killedPids.length} process(es): ${killedPids.join(', ')}`);
    } else {
      console.log(`\nℹ️  No Maestro processes found running`);
    }

  } catch (error: any) {
    console.error(`❌ Error killing processes: ${error.message}`);
    process.exit(1);
  }
}

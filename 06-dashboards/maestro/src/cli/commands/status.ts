/**
 * Status command - Check orchestrator status
 */

import * as http from 'http';
import * as fs from 'fs';
import * as path from 'path';
import { findRepoRoot } from '../utils';

const PID_FILE = '.maestro/dashboard.pid';
const API_PORT = 3000; // Express API server (serves React dashboard)
const MCP_PORT = 3001;

async function checkPortInUse(port: number): Promise<boolean> {
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: 'localhost',
        port,
        method: 'HEAD',
        timeout: 1000,
      },
      () => {
        resolve(true);
      }
    );

    req.on('error', () => resolve(false));
    req.on('timeout', () => {
      req.destroy();
      resolve(false);
    });

    req.end();
  });
}

function formatUptime(seconds: number): string {
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

export async function checkStatus(): Promise<void> {
  const repoRoot = findRepoRoot();
  const pidFile = path.join(repoRoot, PID_FILE);

  console.log('📊 Maestro Orchestrator Status\n');
  console.log('═'.repeat(60));

  // Check Express API server PID file
  let apiPid: number | null = null;
  let apiPidFileExists = false;
  let apiPidValid = false;
  
  if (fs.existsSync(pidFile)) {
    apiPidFileExists = true;
    try {
      apiPid = parseInt(fs.readFileSync(pidFile, 'utf-8').trim(), 10);
      if (!isNaN(apiPid)) {
        apiPidValid = true;
        try {
          process.kill(apiPid, 0);
        } catch {
          apiPidValid = false;
        }
      }
    } catch {
      // Ignore
    }
  }

  // Check if API server is responding (serves React dashboard)
  const apiRunning = await checkDashboardHealth(API_PORT);
  const apiDetails = apiRunning ? await getDashboardDetails(API_PORT) : null;
  const activeWorkflows = apiRunning ? await getActiveWorkflowsCount(API_PORT) : 0;
  const apiPortInUse = await checkPortInUse(API_PORT);

  // Check MCP server
  const mcpRunning = await checkMcpHealth(MCP_PORT);
  const mcpDetails = mcpRunning ? await getMcpDetails(MCP_PORT) : null;
  const mcpPortInUse = await checkPortInUse(MCP_PORT);

  // Display status
  console.log('\n📊 Dashboard Server:');
  if (apiRunning) {
    console.log('   ✅ Status: RUNNING');
    console.log(`   🌐 URL: http://localhost:${API_PORT}`);
    if (apiPidValid && apiPid) {
      console.log(`   🆔 PID: ${apiPid}`);
    } else if (apiPortInUse) {
      console.log(`   ⚠️  Process running on port ${API_PORT} (PID file missing or invalid)`);
    }
    if (apiDetails) {
      if (apiDetails.uptime !== undefined) {
        console.log(`   ⏱️  Uptime: ${formatUptime(apiDetails.uptime)}`);
      }
      if (apiDetails.memory) {
        const memUsedMB = (apiDetails.memory.used / 1024 / 1024).toFixed(1);
        const memTotalMB = (apiDetails.memory.total / 1024 / 1024).toFixed(1);
        console.log(`   💾 Memory: ${memUsedMB}MB / ${memTotalMB}MB (${apiDetails.memory.percentage.toFixed(1)}%)`);
      }
    }
    if (activeWorkflows > 0) {
      console.log(`   🔄 Active Workflows: ${activeWorkflows}`);
    }
  } else {
    console.log('   ❌ Status: NOT RUNNING');
    if (apiPidFileExists && !apiPidValid) {
      console.log('   ⚠️  Stale PID file detected (process may have crashed)');
    } else if (apiPortInUse) {
      console.log(`   ⚠️  Port ${API_PORT} is in use (may be another process)`);
    } else if (apiPidFileExists) {
      console.log('   ⚠️  PID file exists but server is not responding');
    }
  }

  console.log('\n🔌 MCP Server:');
  if (mcpRunning) {
    console.log('   ✅ Status: RUNNING');
    console.log(`   🌐 URL: http://localhost:${MCP_PORT}`);
    if (mcpDetails) {
      if (mcpDetails.health) {
        const healthEmoji = mcpDetails.health === 'healthy' ? '✅' : '⚠️';
        console.log(`   ${healthEmoji} Health: ${mcpDetails.health}`);
      }
      if (mcpDetails.resourceCount !== undefined) {
        console.log(`   📋 Resources: ${mcpDetails.resourceCount}`);
      }
      if (mcpDetails.toolCount !== undefined) {
        console.log(`   🛠️  Tools: ${mcpDetails.toolCount}`);
      }
    }
  } else {
    console.log('   ❌ Status: NOT RUNNING');
    if (mcpPortInUse) {
      console.log(`   ⚠️  Port ${MCP_PORT} is in use (may be another process)`);
    }
  }

  // Overall status
  console.log('\n' + '═'.repeat(60));
  
  let overallStatus: string;
  let statusEmoji: string;
  let statusMessage: string;
  
  if (apiRunning && mcpRunning) {
    overallStatus = 'FULLY OPERATIONAL';
    statusEmoji = '✅';
    statusMessage = 'All services are running';
  } else if (apiRunning) {
    overallStatus = 'PARTIALLY OPERATIONAL';
    statusEmoji = '⚠️';
    statusMessage = 'Dashboard is running but MCP server is not';
  } else if (mcpRunning) {
    overallStatus = 'PARTIALLY OPERATIONAL';
    statusEmoji = '⚠️';
    statusMessage = 'MCP server is running but Dashboard is not';
  } else {
    overallStatus = 'NOT RUNNING';
    statusEmoji = '❌';
    statusMessage = 'No services are running';
  }
  
  console.log(`\n${statusEmoji} Overall Status: ${overallStatus}`);
  console.log(`   ${statusMessage}`);
  
  console.log(`\n💡 Commands:`);
  if (apiRunning || mcpRunning) {
    console.log(`   • Stop all: maestro disable`);
    if (apiRunning) {
      console.log(`   • View dashboard: http://localhost:${API_PORT}`);
    }
    if (!mcpRunning) {
      console.log(`   • Start MCP: Use dashboard UI or restart with "maestro enable"`);
    }
  } else {
    console.log(`   • Start: maestro enable`);
  }
  
  // Explicitly exit to ensure command terminates
  process.exit(0);
}

async function checkDashboardHealth(port: number): Promise<boolean> {
  // Try multiple endpoints to confirm server is actually responding
  const endpoints = ['/api/system/info', '/api/health', '/'];
  
  for (const endpoint of endpoints) {
    const isUp = await new Promise<boolean>((resolve) => {
      const req = http.request(
        {
          host: 'localhost',
          port,
          path: endpoint,
          method: 'GET',
          timeout: 2000,
        },
        (res) => {
          // Any response means server is up
          resolve(res.statusCode !== undefined);
        }
      );

      req.on('error', () => resolve(false));
      req.on('timeout', () => {
        req.destroy();
        resolve(false);
      });

      req.end();
    });
    
    if (isUp) return true;
  }
  
  return false;
}

async function getDashboardDetails(port: number): Promise<{ 
  uptime?: number; 
  memory?: { used: number; total: number; percentage: number };
  activeWorkflows?: number;
} | null> {
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: 'localhost',
        port,
        path: '/api/system/info',
        method: 'GET',
        timeout: 3000,
      },
      (res) => {
        let data = '';
        res.on('data', (chunk) => {
          data += chunk;
        });
        res.on('end', () => {
          try {
            const info = JSON.parse(data);
            resolve({ 
              uptime: info.uptime,
              memory: info.memory ? {
                used: info.memory.used,
                total: info.memory.total,
                percentage: info.memory.percentage
              } : undefined
            });
          } catch {
            resolve(null);
          }
        });
      }
    );

    req.on('error', () => resolve(null));
    req.on('timeout', () => {
      req.destroy();
      resolve(null);
    });

    req.end();
  });
}

async function getActiveWorkflowsCount(port: number): Promise<number> {
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: 'localhost',
        port,
        path: '/api/workflows/active',
        method: 'GET',
        timeout: 2000,
      },
      (res) => {
        let data = '';
        res.on('data', (chunk) => {
          data += chunk;
        });
        res.on('end', () => {
          try {
            const result = JSON.parse(data);
            resolve(result.workflows?.length || 0);
          } catch {
            resolve(0);
          }
        });
      }
    );

    req.on('error', () => resolve(0));
    req.on('timeout', () => {
      req.destroy();
      resolve(0);
    });

    req.end();
  });
}

async function checkMcpHealth(port: number): Promise<boolean> {
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: 'localhost',
        port,
        path: '/mcp/health',
        method: 'GET',
        timeout: 2000,
      },
      (res) => {
        resolve(res.statusCode === 200);
      }
    );

    req.on('error', () => resolve(false));
    req.on('timeout', () => {
      req.destroy();
      resolve(false);
    });

    req.end();
  });
}

async function getMcpDetails(port: number): Promise<{ health?: string; resourceCount?: number; toolCount?: number } | null> {
  // First try the info endpoint which has more details
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: 'localhost',
        port,
        path: '/mcp/info',
        method: 'GET',
        timeout: 2000,
      },
      (res) => {
        if (res.statusCode === 200) {
          let data = '';
          res.on('data', (chunk) => {
            data += chunk;
          });
          res.on('end', () => {
            try {
              const info = JSON.parse(data);
              resolve({
                health: info.health || 'healthy',
                resourceCount: info.resourceCount,
                toolCount: info.toolCount,
              });
            } catch {
              // Fallback to health endpoint
              getMcpHealthDetails(port).then(resolve).catch(() => resolve(null));
            }
          });
        } else {
          // Fallback to health endpoint
          getMcpHealthDetails(port).then(resolve).catch(() => resolve(null));
        }
      }
    );

    req.on('error', () => {
      // Fallback to health endpoint
      getMcpHealthDetails(port).then(resolve).catch(() => resolve(null));
    });
    req.on('timeout', () => {
      req.destroy();
      // Fallback to health endpoint
      getMcpHealthDetails(port).then(resolve).catch(() => resolve(null));
    });

    req.end();
  });
}

async function getMcpHealthDetails(port: number): Promise<{ health?: string } | null> {
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: 'localhost',
        port,
        path: '/mcp/health',
        method: 'GET',
        timeout: 2000,
      },
      (res) => {
        if (res.statusCode === 200) {
          let data = '';
          res.on('data', (chunk) => {
            data += chunk;
          });
          res.on('end', () => {
            try {
              const info = JSON.parse(data);
              resolve({
                health: info.status || 'healthy',
              });
            } catch {
              resolve({ health: 'healthy' });
            }
          });
        } else {
          resolve(null);
        }
      }
    );

    req.on('error', () => resolve(null));
    req.on('timeout', () => {
      req.destroy();
      resolve(null);
    });

    req.end();
  });
}


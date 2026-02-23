/**
 * Dashboard Command
 * 
 * Start/stop the real-time monitoring dashboard server.
 * Can be run directly (foreground) or spawned (background).
 */

import { DashboardServer } from '../../server/DashboardServer';
import { findRepoRoot, openBrowser } from '../utils';
import * as fs from 'fs';
import * as path from 'path';

const PID_FILE = '.maestro/dashboard.pid';

let dashboardServer: DashboardServer | null = null;

export interface DashboardOptions {
  port?: number;
  start?: boolean;
  stop?: boolean;
}

// Check if running as standalone script (spawned from enable command)
// This allows the dashboard to run in a separate process
if (require.main === module) {
  (async () => {
    try {
      const args = process.argv.slice(2);
      const portIndex = args.indexOf('--port');
      const port = portIndex >= 0 && args[portIndex + 1] 
        ? parseInt(args[portIndex + 1], 10) 
        : 3000;
      
      await startDashboard({ port });
    } catch (error: any) {
      console.error('❌ Failed to start dashboard:', error.message);
      process.exit(1);
    }
  })();
}

export async function startDashboard(options: DashboardOptions): Promise<void> {
  try {
    const repoRoot = findRepoRoot();
    const port = options.port || 3000;

    if (dashboardServer) {
      console.log('⚠️  Dashboard server is already running in this process');
      return;
    }

    // Build React dashboard if it doesn't exist
    const dashboardDir = path.join(repoRoot, '.maestro', 'dashboard');
    const dashboardDist = path.join(dashboardDir, 'dist');
    const dashboardIndexPath = path.join(dashboardDist, 'index.html');
    
    if (!fs.existsSync(dashboardIndexPath)) {
      console.log('📦 Building React dashboard...');
      try {
        const { execSync } = require('child_process');
        
        // Check if dashboard directory exists
        if (fs.existsSync(dashboardDir)) {
          // Install dependencies if needed
          const packageJsonPath = path.join(dashboardDir, 'package.json');
          const nodeModulesPath = path.join(dashboardDir, 'node_modules');
          if (fs.existsSync(packageJsonPath) && !fs.existsSync(nodeModulesPath)) {
            console.log('📦 Installing dashboard dependencies...');
            execSync('npm install', { 
              cwd: dashboardDir, 
              stdio: 'inherit'
            });
          }
          
          // Build the dashboard
          console.log('🔨 Building dashboard...');
          execSync('npm run build', { 
            cwd: dashboardDir, 
            stdio: 'inherit'
          });
          console.log('✅ Dashboard build complete');
        }
      } catch (error: any) {
        console.error('⚠️  Failed to build React dashboard:', error.message);
        console.log('📊 Dashboard may not be available');
      }
    }

    dashboardServer = new DashboardServer(port, repoRoot);
    
    // Save PID file
    const pidFile = path.join(repoRoot, PID_FILE);
    const pidDir = path.dirname(pidFile);
    if (!fs.existsSync(pidDir)) {
      fs.mkdirSync(pidDir, { recursive: true });
    }
    fs.writeFileSync(pidFile, String(process.pid), 'utf-8');
    
    // Clean up PID file on exit
    const cleanup = () => {
      if (fs.existsSync(pidFile)) {
        try {
          fs.unlinkSync(pidFile);
        } catch {
          // Ignore errors during cleanup
        }
      }
    };
    process.on('SIGINT', cleanup);
    process.on('SIGTERM', cleanup);
    process.on('exit', cleanup);
    
    await dashboardServer.start();

    console.log('\n💡 Press Ctrl+C to stop the dashboard server\n');
    
    // Note: The dashboard command starts the Express server which serves the React dashboard
    // The React dashboard is built and served from the dist/ directory
    // Don't open browser here - the Express server is API-only
    console.log(`\n📊 Express API server running on http://localhost:${port}`);
    console.log(`💡 Dashboard is available at: http://localhost:${port}`);

    // Handle graceful shutdown
    process.on('SIGINT', async () => {
      await stopDashboard();
      process.exit(0);
    });

    process.on('SIGTERM', async () => {
      await stopDashboard();
      process.exit(0);
    });

    // Keep process alive
    await new Promise(() => {}); // Never resolves, keeps process running

  } catch (error: any) {
    const port = options.port || 3000;
    if (error.code === 'EADDRINUSE') {
      console.error(`❌ Port ${port} is already in use.`);
      console.error(`   Try using a different port: npm run dev dashboard -- --port 3001`);
      console.error(`   Or stop the existing server on port ${port}`);
    } else {
      console.error('❌ Failed to start dashboard:', error.message);
    }
    process.exit(1);
  }
}

export async function stopDashboard(): Promise<void> {
  if (dashboardServer) {
    await dashboardServer.stop();
    dashboardServer = null;
  }
}

export function getDashboardServer(): DashboardServer | null {
  return dashboardServer;
}

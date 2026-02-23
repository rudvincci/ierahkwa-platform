/**
 * Enable command - Start the orchestrator dashboard in background
 */

import { spawn } from 'child_process';
import * as fs from 'fs';
import * as path from 'path';
import { findRepoRoot, openBrowser, getNodeArgs } from '../utils';

const PID_FILE = '.maestro/dashboard.pid';

export async function enableOrchestrator(options: { port?: number }): Promise<void> {
  const port = options.port || 3000;
  const repoRoot = findRepoRoot();
  const pidFile = path.join(repoRoot, PID_FILE);

  // Check if already running
  const expressRunning = fs.existsSync(pidFile) && (() => {
    try {
      const pid = parseInt(fs.readFileSync(pidFile, 'utf-8').trim(), 10);
      if (!isNaN(pid)) {
        try {
          process.kill(pid, 0);
          return true;
        } catch {
          fs.unlinkSync(pidFile);
          return false;
        }
      }
    } catch {
      return false;
    }
    return false;
  })();

  if (expressRunning) {
    console.log(`✅ Dashboard is already running`);
    console.log(`📊 Dashboard: http://localhost:${port}`);
    
    // Open browser to dashboard
    setTimeout(() => {
      console.log(`🌐 Opening dashboard in browser...`);
      openBrowser(`http://localhost:${port}`);
    }, 500);
    
    setTimeout(() => {
      process.exit(0);
    }, 1000);
    return;
  }

  console.log('🚀 Enabling Maestro Orchestrator...');
  console.log(`📊 Dashboard will be available at http://localhost:${port}`);

  // Find the .maestro directory that contains the dashboard
  // Check current repo root first, then parent directory
  // Use relative path from repoRoot
  let dashboardDirRel = '.maestro/dashboard';
  let dashboardDir = path.join(repoRoot, dashboardDirRel);
  let effectiveRepoRoot = repoRoot;
  
  // If dashboard doesn't exist in current .maestro, check parent
  if (!fs.existsSync(dashboardDir)) {
    const parentRepoRoot = path.dirname(repoRoot);
    const parentDashboardDir = path.join(parentRepoRoot, '.maestro', 'dashboard');
    if (fs.existsSync(parentDashboardDir)) {
      // Use relative path from parent repo root
      dashboardDirRel = path.relative(parentRepoRoot, parentDashboardDir);
      dashboardDir = parentDashboardDir;
      effectiveRepoRoot = parentRepoRoot;
    }
  }

  // Build React dashboard if it doesn't exist
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
        
                // Build the dashboard in production mode
                console.log('🔨 Building dashboard (production mode)...');
                execSync('npm run build', { 
                  cwd: dashboardDir, 
                  stdio: 'inherit',
                  env: { ...process.env, NODE_ENV: 'production' }
                });
        console.log('✅ Dashboard build complete');
      } else {
        console.log('⚠️  Dashboard directory not found, will use fallback HTML dashboard');
      }
    } catch (error: any) {
      console.error('⚠️  Failed to build React dashboard:', error.message);
      console.log('📊 Will use fallback HTML dashboard');
    }
  }

  // Get the path to the maestro CLI executable
  // When running from dist/, use the compiled CLI
  // When running from src/, use ts-node
  const isDev = __filename.includes('src');
  
  // Find the main CLI entry point
  const mainCliPath = isDev
    ? path.join(__dirname, '../index.ts')
    : path.join(__dirname, '../index.js');
  
  // Use ts-node in dev, node in production
  const nodePath = process.execPath;
  const scriptArgs = isDev
    ? [require.resolve('ts-node/dist/bin'), mainCliPath, 'dashboard', '--port', String(port)]
    : [mainCliPath, 'dashboard', '--port', String(port)];

  // Spawn dashboard in background - use a wrapper script that saves its own PID
  // This ensures proper daemonization
  const wrapperScript = path.join(repoRoot, '.maestro', 'dashboard-wrapper.js');
  const wrapperDir = path.dirname(wrapperScript);
  
  // Ensure directory exists
  if (!fs.existsSync(wrapperDir)) {
    fs.mkdirSync(wrapperDir, { recursive: true });
  }
  
  // Get heap memory size for Node.js
  const heapSize = require('../utils').getHeapMemorySize();
  // Add --expose-gc to enable garbage collection, and heap memory limit
  const nodeArgs = ['--expose-gc', '--max-old-space-size=' + heapSize, ...scriptArgs];

  // Create wrapper script that spawns dashboard and saves PID
  const wrapperContent = `#!/usr/bin/env node
const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

const pidFile = ${JSON.stringify(pidFile)};
const nodePath = ${JSON.stringify(nodePath)};
const nodeArgs = ${JSON.stringify(nodeArgs)};
const repoRoot = ${JSON.stringify(repoRoot)};

// Spawn the actual dashboard process with heap memory configuration
const child = spawn(nodePath, nodeArgs, {
  detached: true,
  stdio: 'ignore',
  cwd: repoRoot,
});

// Save the actual dashboard process PID
const pidDir = path.dirname(pidFile);
if (!fs.existsSync(pidDir)) {
  fs.mkdirSync(pidDir, { recursive: true });
}
fs.writeFileSync(pidFile, String(child.pid), 'utf-8');

// Unref so this wrapper can exit immediately
child.unref();
process.exit(0);
`;

  // Write wrapper script
  fs.writeFileSync(wrapperScript, wrapperContent, 'utf-8');
  fs.chmodSync(wrapperScript, 0o755);

  // Spawn the wrapper script in background
  const spawner = spawn(process.execPath, [wrapperScript], {
    detached: true,
    stdio: 'ignore',
  });

  // Unref immediately so parent can exit
  spawner.unref();

  // Don't wait - return immediately
  // The wrapper script will handle PID file creation
  console.log(`✅ Dashboard server starting in background...`);
  
  console.log(`📊 Dashboard: http://localhost:${port}`);
  console.log(`💡 Check status: maestro status`);
  console.log(`💡 Stop with: maestro disable`);
  
  // Wait for server to be ready, then open browser
  const checkAndOpen = async () => {
    const maxAttempts = 20; // Increased attempts
    const delay = 500; // 500ms between attempts
    let opened = false;
    
    for (let i = 0; i < maxAttempts; i++) {
      await new Promise(resolve => setTimeout(resolve, delay));
      
      // Check if server is running
      const expressReady = fs.existsSync(pidFile) && (() => {
        try {
          const pid = parseInt(fs.readFileSync(pidFile, 'utf-8').trim(), 10);
          if (!isNaN(pid)) {
            try {
              process.kill(pid, 0);
              return true;
            } catch {
              return false;
            }
          }
        } catch {
          return false;
        }
        return false;
      })();

      if (expressReady && !opened) {
        // Try to connect to the dashboard to verify it's ready
        const http = require('http');
        
        try {
          await new Promise<void>((resolve, reject) => {
            const checkRequest = http.get(`http://localhost:${port}`, (res: any) => {
              if (res.statusCode === 200 || res.statusCode === 304) {
                console.log(`\n🌐 Opening dashboard in browser...`);
                openBrowser(`http://localhost:${port}`);
                opened = true;
                res.resume(); // Consume response to free up memory
                resolve();
              } else {
                res.resume();
                reject(new Error(`Server returned ${res.statusCode}`));
              }
            });
            
            checkRequest.on('error', (err: any) => {
              // Server not ready yet, will retry
              reject(err);
            });
            
            checkRequest.setTimeout(2000, () => {
              checkRequest.destroy();
              reject(new Error('Request timeout'));
            });
          });
          
          // Successfully opened browser, break out of loop
          if (opened) {
            break;
          }
        } catch (error) {
          // Server not ready yet, continue trying
          if (i === maxAttempts - 1) {
            // Last attempt failed, try opening anyway
            console.log(`\n🌐 Opening dashboard in browser (server may still be starting)...`);
            openBrowser(`http://localhost:${port}`);
          }
        }
      }
    }
  };
  
  // Start checking in background (non-blocking)
  checkAndOpen().catch((err) => {
    // If all attempts fail, try opening anyway
    console.log(`\n🌐 Opening dashboard in browser...`);
    openBrowser(`http://localhost:${port}`);
  });
  
  // Don't exit immediately - give the checkAndOpen function time to run
  // Exit after a reasonable delay to ensure process doesn't hang
  setTimeout(() => {
    process.exit(0);
  }, 2000).unref();
}

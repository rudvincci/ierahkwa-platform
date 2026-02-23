#!/usr/bin/env node
const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');
const os = require('os');

const pidFile = "/Volumes/Barracuda/mamey-io/code-final/.maestro/blazor-dashboard.pid";
const dashboardDir = "/Volumes/Barracuda/mamey-io/code-final/.maestro/dashboard";

// Ensure PID directory exists
const pidDir = path.dirname(pidFile);
if (!fs.existsSync(pidDir)) {
  fs.mkdirSync(pidDir, { recursive: true });
}

// Spawn the Blazor dev server
const isWindows = os.platform() === 'win32';
const child = spawn('dotnet', ['run', '--launch-profile', 'https'], {
  cwd: dashboardDir,
  detached: true,
  stdio: 'ignore',
  env: { ...process.env, ASPNETCORE_ENVIRONMENT: 'Development' }
});

// Save the initial PID (dotnet process)
fs.writeFileSync(pidFile, String(child.pid), 'utf-8');

// Unref so parent can exit
child.unref();

// On Unix, periodically check for the actual process listening on ports 5298/7159
if (!isWindows) {
  const checkInterval = setInterval(() => {
    try {
      const { exec } = require('child_process');
      // Find process listening on port 5298 or 7159
      exec('lsof -ti:5298,7159 2>/dev/null | head -1', (error, stdout) => {
        if (!error && stdout.trim()) {
          const actualPid = stdout.trim();
          if (actualPid && !isNaN(parseInt(actualPid, 10))) {
            fs.writeFileSync(pidFile, actualPid, 'utf-8');
            clearInterval(checkInterval);
          }
        }
      });
    } catch (e) {
      // Ignore errors
    }
  }, 1000);
  
  // Stop checking after 10 seconds
  setTimeout(() => clearInterval(checkInterval), 10000);
}

process.exit(0);

#!/usr/bin/env node
const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

const pidFile = "/Volumes/Barracuda/mamey-io/code-final/.maestro/dashboard.pid";
const nodePath = "/Users/manolo/.nvm/versions/node/v18.17.0/bin/node";
const nodeArgs = ["--expose-gc","--max-old-space-size=4096","/Volumes/Barracuda/mamey-io/code-final/.maestro/dist/cli/index.js","dashboard","--port","3000"];
const repoRoot = "/Volumes/Barracuda/mamey-io/code-final";

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

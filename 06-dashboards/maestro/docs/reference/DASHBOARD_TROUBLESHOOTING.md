# 🔧 Dashboard Troubleshooting Guide

## Issue: Port Already in Use

### Error Message
```
Error: listen EADDRINUSE: address already in use :::3000
```

### Solutions

#### Option 1: Use a Different Port (Quick Fix)
```bash
npm run dev dashboard -- --port 3001
```

Then access dashboard at: `http://localhost:3001`

#### Option 2: Stop Process Using Port 3000

**Find the process:**
```bash
# macOS/Linux
lsof -ti:3000

# Or check what's using the port
lsof -i:3000
```

**Kill the process:**
```bash
# Replace <PID> with the process ID from above
kill <PID>

# Or force kill
kill -9 <PID>
```

**Then start dashboard:**
```bash
npm run dev dashboard
```

#### Option 3: Check for Existing Dashboard

If you have a dashboard running in another terminal:
1. Find the terminal where dashboard is running
2. Press `Ctrl+C` to stop it
3. Or run: `npm run dev dashboard:stop`

---

## Issue: Dashboard Starts But Doesn't Show Workflows

### Check:
1. **Is a workflow running?** - Dashboard only shows active workflows
2. **Check browser console** - Open browser dev tools (F12) and check for errors
3. **WebSocket connection** - Check connection status indicator in dashboard

### Solution:
```bash
# Terminal 1: Start dashboard
npm run dev dashboard

# Terminal 2: Run a workflow (dashboard will show it)
npm run dev run -- --flow fwid-compliance --runner cursor
```

---

## Issue: Dashboard Command Not Found

### Error:
```
command not found: dashboard
```

### Solution:
Make sure you're using the correct syntax:
```bash
# Correct
npm run dev dashboard

# Wrong
npm run dev -- dashboard  # Don't use -- here
```

---

## Issue: Dashboard Crashes Immediately

### Check Logs:
```bash
npm run dev dashboard 2>&1 | tee dashboard.log
```

### Common Causes:
1. **Port conflict** - Use different port: `--port 3001`
2. **Missing dependencies** - Run `npm install`
3. **Build issues** - Run `npm run build`

---

## Quick Diagnostic Commands

```bash
# Check if port 3000 is in use
lsof -i:3000

# Check if dashboard process is running
ps aux | grep dashboard

# Test dashboard on different port
npm run dev dashboard -- --port 3001

# Rebuild if needed
npm run build

# Reinstall dependencies
npm install
```

---

## Working Example

**Terminal 1:**
```bash
cd .agent-orchestrator
npm run dev dashboard -- --port 3001
```

**Terminal 2:**
```bash
cd .agent-orchestrator
npm run dev run -- --flow fwid-compliance --runner cursor
```

**Browser:**
```
http://localhost:3001
```

---

## Still Having Issues?

1. Check the error message carefully
2. Try a different port (3001, 3002, etc.)
3. Make sure you're in the `.agent-orchestrator` directory
4. Verify build: `npm run build`
5. Check Node.js version: `node --version` (should be 18+)

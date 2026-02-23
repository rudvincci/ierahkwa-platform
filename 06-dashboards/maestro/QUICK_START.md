# 🎼 Quick Start - How to Run Maestro

## Choose Your Installation Method

Maestro can run **locally** or in a **Docker container**. Choose what works best for you:

- **[Local Installation](#local-installation)** - Fast, direct execution (recommended for development)
- **[Docker Container](#docker-container)** - Isolated, portable (recommended for production)

---

## Local Installation

## Option 1: Using npm scripts (Recommended)

```bash
# Navigate to orchestrator directory
cd .agent-orchestrator

# Run workflow using npm dev script
npm run dev run -- --flow fwid-compliance --runner cursor -v

# Or using npm start (after build)
npm run build
npm start run -- --flow fwid-compliance --runner cursor -v
```

## Option 2: Direct Node execution

```bash
# Navigate to orchestrator directory
cd .agent-orchestrator

# Run directly with node
node dist/cli/index.js run --flow fwid-compliance --runner cursor -v

# Or if not built yet
npm run build
node dist/cli/index.js run --flow fwid-compliance --runner cursor -v
```

## Option 3: Global installation (Optional)

```bash
# Navigate to orchestrator directory
cd .agent-orchestrator

# Link globally (requires sudo on some systems)
npm link

# Now you can use from anywhere
agent-orchestrator run --flow fwid-compliance --runner cursor -v
```

## Option 4: Create alias (Quick fix)

Add to your `~/.zshrc` or `~/.bashrc`:

```bash
# Add alias for agent-orchestrator
alias agent-orchestrator='node /Volumes/Barracuda/mamey-io/code-final/.agent-orchestrator/dist/cli/index.js'

# Or if using npm dev
alias agent-orchestrator='cd /Volumes/Barracuda/mamey-io/code-final/.agent-orchestrator && npm run dev'
```

Then reload:
```bash
source ~/.zshrc  # or source ~/.bashrc
```

---

## ✅ Recommended Workflow

### Step 1: Build the project
```bash
cd .agent-orchestrator
npm install
npm run build
```

### Step 2: Run using npm dev (easiest)
```bash
npm run dev run -- --flow fwid-compliance --runner cursor -v
```

### Step 3: Start dashboard (in another terminal)
```bash
cd .agent-orchestrator
npm run dev dashboard
```

---

## 📝 Common Commands

```bash
# From .agent-orchestrator directory:

# Run workflow
npm run dev run -- --flow fwid-compliance --runner cursor

# Start dashboard
npm run dev dashboard

# List workflows
npm run dev flows

# Show help
npm run dev -- --help
```

---

## 🔧 Troubleshooting

### "command not found"
- Make sure you're in the `.agent-orchestrator` directory
- Use `npm run dev` instead of `agent-orchestrator`
- Or build first: `npm run build`

### "Cannot find module"
- Run `npm install` first
- Then `npm run build`

### "Workflow not found"
- Check available workflows: `npm run dev flows`
- Verify workflow name in `config/orchestration.yml`

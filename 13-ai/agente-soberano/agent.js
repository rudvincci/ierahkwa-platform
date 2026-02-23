#!/usr/bin/env node
'use strict';

/**
 * Agente Soberano — Sovereign AI Coding Agent CLI
 * Terminal-first agentic coding assistant
 * Replaces: OpenCode.ai, Qwen Code, Claude Code CLI
 *
 * Usage:
 *   npx agente-soberano "refactor the auth module"
 *   npx agente-soberano --interactive
 *   npx agente-soberano --headless "add tests for server.js"
 */

const fs = require('fs');
const path = require('path');
const readline = require('readline');

const VERSION = '1.0.0';
const CONFIG_DIR = path.join(process.env.HOME || process.env.USERPROFILE || '.', '.agente-soberano');
const CONFIG_FILE = path.join(CONFIG_DIR, 'config.json');

// ── Default Configuration ──
const DEFAULT_CONFIG = {
  model: 'sovereign-default',
  provider: 'local',
  temperature: 0.7,
  maxTokens: 4096,
  telemetry: false,
  privacy: {
    storeCode: false,
    storeConversations: false,
    sendAnalytics: false
  },
  providers: {
    local: { url: 'http://localhost:11434/api/generate', type: 'ollama' },
    openai: { url: 'https://api.openai.com/v1', type: 'openai', key: '' },
    anthropic: { url: 'https://api.anthropic.com/v1', type: 'anthropic', key: '' },
    sovereign: { url: 'https://ai.ierahkwa.sovereign/v1', type: 'openai', key: '' }
  },
  skills: ['code', 'test', 'security', 'docs', 'refactor', 'debug'],
  mcpServers: {}
};

// ── Skills System ──
const SKILLS = {
  code: {
    name: 'Code Generation',
    description: 'Generate code from natural language descriptions',
    systemPrompt: 'You are a sovereign coding agent. Generate clean, secure, well-tested code following OWASP best practices. Use strict mode. Never hardcode secrets.'
  },
  test: {
    name: 'Test Generation',
    description: 'Generate comprehensive test suites',
    systemPrompt: 'Generate Jest test suites covering: unit tests, integration tests, edge cases, error paths. Use describe/test blocks. Aim for >80% coverage.'
  },
  security: {
    name: 'Security Analysis',
    description: 'Analyze code for security vulnerabilities',
    systemPrompt: 'Analyze code for OWASP Top 10 vulnerabilities. Check for injection, XSS, CSRF, broken auth, crypto failures, misconfigurations. Provide fixes.'
  },
  docs: {
    name: 'Documentation',
    description: 'Generate and update documentation',
    systemPrompt: 'Generate clear documentation: JSDoc comments, README sections, API documentation. Follow the Ierahkwa Platform documentation style.'
  },
  refactor: {
    name: 'Code Refactoring',
    description: 'Suggest and apply refactoring improvements',
    systemPrompt: 'Analyze code for refactoring opportunities: extract functions, reduce complexity, improve naming, apply SOLID principles, remove duplication.'
  },
  debug: {
    name: 'Debugging Assistant',
    description: 'Help diagnose and fix bugs',
    systemPrompt: 'Analyze the code and error information to identify root causes. Suggest targeted fixes. Check for common Node.js issues: async/await errors, memory leaks, race conditions.'
  },
  a11y: {
    name: 'Accessibility',
    description: 'Check and fix accessibility issues',
    systemPrompt: 'Analyze HTML/CSS for WCAG 2.2 AA compliance. Check: lang attribute, alt text, keyboard navigation, color contrast, ARIA labels, semantic HTML, focus management.'
  },
  sovereign: {
    name: 'Sovereign Compliance',
    description: 'Verify sovereign platform standards',
    systemPrompt: 'Verify code follows sovereign standards: shared security middleware imported, /health endpoint exists, no tracking scripts, 0% tax rate, WAMPUM token support, privacy-first design.'
  }
};

// ── Context Gathering ──
function gatherContext(workDir) {
  const context = {
    cwd: workDir,
    files: [],
    packageJson: null,
    gitBranch: null,
    structure: []
  };

  // Read package.json
  const pkgPath = path.join(workDir, 'package.json');
  if (fs.existsSync(pkgPath)) {
    try { context.packageJson = JSON.parse(fs.readFileSync(pkgPath, 'utf-8')); } catch {}
  }

  // List files (max 2 levels)
  const listFiles = (dir, depth = 0) => {
    if (depth > 2) return;
    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      for (const entry of entries) {
        if (['node_modules', '.git', 'dist', 'build', 'coverage'].includes(entry.name)) continue;
        const rel = path.relative(workDir, path.join(dir, entry.name));
        if (entry.isDirectory()) {
          context.structure.push({ type: 'dir', path: rel });
          listFiles(path.join(dir, entry.name), depth + 1);
        } else {
          context.structure.push({ type: 'file', path: rel });
        }
      }
    } catch {}
  };
  listFiles(workDir);

  return context;
}

// ── File Reference System (@file syntax) ──
function resolveFileRefs(input, workDir) {
  const refs = input.match(/@[\w./\-]+/g) || [];
  const fileContents = {};

  for (const ref of refs) {
    const filePath = path.resolve(workDir, ref.substring(1));
    if (fs.existsSync(filePath)) {
      try {
        fileContents[ref] = fs.readFileSync(filePath, 'utf-8');
      } catch {
        fileContents[ref] = '[Error: Could not read file]';
      }
    }
  }

  return fileContents;
}

// ── Session Management ──
class Session {
  constructor(id) {
    this.id = id || Date.now().toString(36);
    this.messages = [];
    this.skill = 'code';
    this.startTime = new Date();
    this.filesModified = [];
  }

  addMessage(role, content) {
    this.messages.push({ role, content, timestamp: new Date().toISOString() });
  }

  toJSON() {
    return {
      id: this.id,
      skill: this.skill,
      messages: this.messages.length,
      startTime: this.startTime.toISOString(),
      filesModified: this.filesModified
    };
  }
}

// ── Interactive REPL ──
async function interactiveMode(config) {
  const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
  const session = new Session();
  const workDir = process.cwd();
  const context = gatherContext(workDir);

  console.log('');
  console.log('🤖 Agente Soberano v' + VERSION);
  console.log('━'.repeat(50));
  console.log(`Model: ${config.model} | Provider: ${config.provider}`);
  console.log(`Skills: ${config.skills.join(', ')}`);
  console.log(`Privacy: telemetry=${config.telemetry}, storeCode=${config.privacy.storeCode}`);
  console.log(`Project: ${context.packageJson?.name || path.basename(workDir)}`);
  console.log(`Files: ${context.structure.filter(s => s.type === 'file').length} | Dirs: ${context.structure.filter(s => s.type === 'dir').length}`);
  console.log('');
  console.log('Commands: /skill <name>, /context, /files, /session, /exit');
  console.log('File refs: @path/to/file.js');
  console.log('━'.repeat(50));

  const prompt = () => {
    const skillIcon = { code: '💻', test: '🧪', security: '🛡️', docs: '📝', refactor: '🔧', debug: '🐛', a11y: '♿', sovereign: '🏛️' };
    rl.question(`\n${skillIcon[session.skill] || '🤖'} [${session.skill}] > `, async (input) => {
      if (!input || input.trim() === '') return prompt();

      const cmd = input.trim();

      if (cmd === '/exit' || cmd === '/quit') {
        console.log(`\nSession ${session.id}: ${session.messages.length} messages, ${session.filesModified.length} files modified`);
        rl.close();
        return;
      }

      if (cmd === '/context') {
        console.log(JSON.stringify(context, null, 2));
        return prompt();
      }

      if (cmd === '/files') {
        context.structure.forEach(s => console.log(`  ${s.type === 'dir' ? '📁' : '📄'} ${s.path}`));
        return prompt();
      }

      if (cmd === '/session') {
        console.log(JSON.stringify(session.toJSON(), null, 2));
        return prompt();
      }

      if (cmd.startsWith('/skill ')) {
        const newSkill = cmd.split(' ')[1];
        if (SKILLS[newSkill]) {
          session.skill = newSkill;
          console.log(`Switched to skill: ${SKILLS[newSkill].name}`);
        } else {
          console.log(`Unknown skill. Available: ${Object.keys(SKILLS).join(', ')}`);
        }
        return prompt();
      }

      // Resolve file references
      const fileRefs = resolveFileRefs(cmd, workDir);
      if (Object.keys(fileRefs).length > 0) {
        console.log(`📎 Referenced ${Object.keys(fileRefs).length} file(s)`);
      }

      // Build prompt with context
      const systemPrompt = SKILLS[session.skill]?.systemPrompt || SKILLS.code.systemPrompt;
      session.addMessage('user', cmd);

      console.log('\n🔄 Processing with sovereign AI...');
      console.log(`   Model: ${config.model}`);
      console.log(`   Skill: ${SKILLS[session.skill]?.name}`);
      console.log('   [Connect an LLM provider to get AI responses]');
      console.log('   Configure: ~/.agente-soberano/config.json');

      session.addMessage('assistant', '[Awaiting LLM provider configuration]');
      prompt();
    });
  };

  prompt();
}

// ── Headless Mode ──
function headlessMode(task, config) {
  console.log('🤖 Agente Soberano — Headless Mode');
  console.log(`Task: ${task}`);
  console.log(`Model: ${config.model}`);
  console.log('Status: Ready for LLM provider connection');
  console.log('Configure: ~/.agente-soberano/config.json');
}

// ── Load/Save Config ──
function loadConfig() {
  if (fs.existsSync(CONFIG_FILE)) {
    try {
      return { ...DEFAULT_CONFIG, ...JSON.parse(fs.readFileSync(CONFIG_FILE, 'utf-8')) };
    } catch { return DEFAULT_CONFIG; }
  }
  return DEFAULT_CONFIG;
}

function initConfig() {
  if (!fs.existsSync(CONFIG_DIR)) fs.mkdirSync(CONFIG_DIR, { recursive: true });
  if (!fs.existsSync(CONFIG_FILE)) {
    fs.writeFileSync(CONFIG_FILE, JSON.stringify(DEFAULT_CONFIG, null, 2));
    console.log(`Config initialized: ${CONFIG_FILE}`);
  }
}

// ── Main CLI ──
function main() {
  const args = process.argv.slice(2);
  const config = loadConfig();

  if (args.includes('--version') || args.includes('-v')) {
    console.log(`Agente Soberano v${VERSION}`);
    return;
  }

  if (args.includes('--help') || args.includes('-h')) {
    console.log(`
🤖 Agente Soberano v${VERSION} — Sovereign AI Coding Agent

Usage:
  agente-soberano                    Interactive mode
  agente-soberano "task description" Execute task
  agente-soberano --headless "task"  Headless/CI mode
  agente-soberano --init             Initialize config
  agente-soberano --skills           List available skills

Options:
  --model <name>     Override model selection
  --provider <name>  Override LLM provider
  --skill <name>     Start with specific skill
  --headless         Run without interactive UI
  --init             Create default config
  --version, -v      Show version
  --help, -h         Show this help

Skills: ${Object.keys(SKILLS).join(', ')}

Privacy: Zero telemetry. No code storage. All sovereign.
    `);
    return;
  }

  if (args.includes('--init')) {
    initConfig();
    return;
  }

  if (args.includes('--skills')) {
    console.log('\n🤖 Available Skills:\n');
    for (const [key, skill] of Object.entries(SKILLS)) {
      console.log(`  ${key.padEnd(12)} ${skill.name} — ${skill.description}`);
    }
    return;
  }

  if (args.includes('--headless')) {
    const taskIdx = args.indexOf('--headless') + 1;
    const task = args[taskIdx] || 'No task specified';
    headlessMode(task, config);
    return;
  }

  // Task mode or interactive
  const task = args.filter(a => !a.startsWith('--')).join(' ');
  if (task) {
    headlessMode(task, config);
  } else {
    interactiveMode(config);
  }
}

if (require.main === module) {
  main();
}

module.exports = { SKILLS, Session, gatherContext, resolveFileRefs, loadConfig, DEFAULT_CONFIG };

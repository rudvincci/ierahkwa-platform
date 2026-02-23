#!/usr/bin/env node
'use strict';

/**
 * Ierahkwa Platform — SAST AutoFix Engine
 * Generates code fix suggestions for security findings
 * Replaces: Aikido AI AutoFix, Snyk Fix, SonarQube Quick Fixes
 */

const fs = require('fs');
const path = require('path');

// ── AutoFix Patterns ──
// Maps SAST rule IDs to their automated fix templates
const FIXES = {
  'SAST-003': {
    name: 'Replace hardcoded credentials with env vars',
    detect: /(const|let|var)\s+(\w*(?:password|secret|api_?key|token|credential)\w*)\s*=\s*['"]([^'"]+)['"]/i,
    fix: (match, decl, name, _value) => `${decl} ${name} = process.env.${name.toUpperCase()} || ''`,
    confidence: 'HIGH'
  },
  'SAST-005': {
    name: 'Parameterize SQL query',
    detect: /(query|execute)\s*\(\s*['"`](SELECT|INSERT|UPDATE|DELETE)\s+.*\$\{(\w+)\}/i,
    fix: (match) => match.replace(/\$\{(\w+)\}/g, '?').replace(/`/, "'") + ' /* TODO: pass params array */',
    confidence: 'MEDIUM'
  },
  'SAST-006': {
    name: 'Sanitize command execution input',
    detect: /(exec|execSync)\s*\(\s*(`[^`]*\$\{|.*\+)/,
    fix: () => '// SECURITY: Use execFile() with argument array instead of exec()\n// execFile(command, [arg1, arg2], callback)',
    confidence: 'LOW'
  },
  'SAST-011': {
    name: 'Replace wildcard CORS with explicit origins',
    detect: /(origin|Access-Control-Allow-Origin)\s*[:=]\s*['"]\*['"]/,
    fix: (match) => match.replace("'*'", "process.env.ALLOWED_ORIGINS?.split(',') || ['http://localhost:3000']").replace('"*"', "process.env.ALLOWED_ORIGINS?.split(',') || ['http://localhost:3000']"),
    confidence: 'HIGH'
  },
  'SAST-016': {
    name: 'Replace eval with safe alternative',
    detect: /\beval\s*\(\s*(\w+)\s*\)/,
    fix: (match, varName) => `JSON.parse(${varName}) /* SECURITY: eval() replaced with JSON.parse() */`,
    confidence: 'MEDIUM'
  },
  'SAST-018': {
    name: 'Redact sensitive data in logs',
    detect: /console\.(log|info|warn|error)\s*\(([^)]*(?:password|token|secret|credit|ssn|apiKey)[^)]*)\)/i,
    fix: (match) => match.replace(/(password|token|secret|credit|ssn|apiKey)/gi, "'[REDACTED]'"),
    confidence: 'HIGH'
  },
  'SAST-022': {
    name: 'Add security flags to cookies',
    detect: /cookie\s*\(\s*['"](\w+)['"]\s*,\s*([^,)]+)/,
    fix: (match, name, value) => `cookie('${name}', ${value}, { secure: true, httpOnly: true, sameSite: 'strict', maxAge: 3600000 })`,
    confidence: 'HIGH'
  },
  'SAST-024': {
    name: 'Replace Math.random with crypto.randomUUID',
    detect: /Math\.random\s*\(\)/,
    fix: () => "require('crypto').randomUUID()",
    confidence: 'HIGH'
  }
};

// ── Code Quality Rules (like Aikido code quality) ──
const CODE_QUALITY_RULES = [
  {
    id: 'CQ-001',
    name: 'Console.log in production code',
    pattern: /\bconsole\.log\s*\(/,
    suggestion: 'Use structured logging (e.g., securityLogger from shared/security.js)',
    severity: 'INFO'
  },
  {
    id: 'CQ-002',
    name: 'TODO/FIXME/HACK comment',
    pattern: /\/\/\s*(TODO|FIXME|HACK|XXX)\b/i,
    suggestion: 'Create a GitHub issue to track this technical debt',
    severity: 'INFO'
  },
  {
    id: 'CQ-003',
    name: 'Magic number',
    pattern: /(?:===?|!==?|[<>]=?)\s+\d{2,}(?!\d*\.)\b(?!px|em|rem|vh|vw|%|ms|s\b)/,
    suggestion: 'Extract magic numbers into named constants',
    severity: 'INFO'
  },
  {
    id: 'CQ-004',
    name: 'Deeply nested callbacks',
    pattern: /\)\s*=>\s*\{[\s\S]*\)\s*=>\s*\{[\s\S]*\)\s*=>\s*\{/,
    suggestion: 'Refactor nested callbacks using async/await',
    severity: 'WARNING'
  },
  {
    id: 'CQ-005',
    name: 'Function exceeds 50 lines',
    pattern: null, // Checked programmatically
    suggestion: 'Break into smaller, focused functions',
    severity: 'WARNING'
  },
  {
    id: 'CQ-006',
    name: 'Missing error handling in async',
    pattern: /async\s+(?:function|\w+\s*=\s*async)(?![^{]*\.catch|[^{]*try\s*\{)/,
    suggestion: 'Wrap async operations in try-catch blocks',
    severity: 'WARNING'
  },
  {
    id: 'CQ-007',
    name: 'var usage (use const/let)',
    pattern: /\bvar\s+\w/,
    suggestion: 'Use const for immutable bindings, let for mutable',
    severity: 'INFO'
  },
  {
    id: 'CQ-008',
    name: 'Empty catch block',
    pattern: /catch\s*\([^)]*\)\s*\{\s*\}/,
    suggestion: 'Log or handle the error, do not silently swallow',
    severity: 'WARNING'
  }
];

// ── Generate fix for a finding ──
function generateFix(finding, fileContent) {
  const fix = FIXES[finding.id];
  if (!fix) return null;

  const lines = fileContent.split('\n');
  const lineIdx = finding.line - 1;
  if (lineIdx >= lines.length) return null;

  const originalLine = lines[lineIdx];
  const match = fix.detect.exec(originalLine);
  if (!match) return null;

  let fixedLine;
  try {
    fixedLine = fix.fix(...match);
  } catch {
    return null;
  }

  return {
    ruleId: finding.id,
    ruleName: fix.name,
    file: finding.file,
    line: finding.line,
    original: originalLine.trim(),
    fixed: fixedLine.trim(),
    confidence: fix.confidence,
    diff: `- ${originalLine.trim()}\n+ ${fixedLine.trim()}`
  };
}

// ── Scan for code quality issues ──
function scanCodeQuality(filePath) {
  const findings = [];
  try {
    const content = fs.readFileSync(filePath, 'utf-8');
    const lines = content.split('\n');

    for (const rule of CODE_QUALITY_RULES) {
      if (!rule.pattern) continue;
      for (let i = 0; i < lines.length; i++) {
        if (rule.pattern.test(lines[i])) {
          const trimmed = lines[i].trim();
          if (trimmed.startsWith('//') && rule.id !== 'CQ-002') continue;
          findings.push({
            id: rule.id,
            name: rule.name,
            severity: rule.severity,
            file: filePath,
            line: i + 1,
            code: trimmed.substring(0, 120),
            suggestion: rule.suggestion
          });
        }
      }
    }
  } catch { /* skip */ }
  return findings;
}

// ── Main ──
if (require.main === module) {
  const { scanDirectory, RULES } = require('./sast-scanner');

  console.log('🔧 Ierahkwa SAST AutoFix Engine');
  console.log('━'.repeat(60));

  const ROOT = path.resolve(__dirname, '..');
  const target = process.argv[2] || path.join(ROOT, '03-backend');
  const findings = scanDirectory(target);

  let fixCount = 0;
  for (const finding of findings) {
    try {
      const content = fs.readFileSync(finding.file, 'utf-8');
      const fix = generateFix(finding, content);
      if (fix) {
        fixCount++;
        const icon = fix.confidence === 'HIGH' ? '🟢' : fix.confidence === 'MEDIUM' ? '🟡' : '🔴';
        const rel = path.relative(ROOT, fix.file);
        console.log(`\n${icon} [${fix.confidence}] ${fix.ruleName}`);
        console.log(`   ${rel}:${fix.line}`);
        console.log(`   ${fix.diff}`);
      }
    } catch { /* skip */ }
  }

  // Code quality scan
  console.log('\n\n📋 Code Quality Analysis');
  console.log('━'.repeat(60));

  const walkDir = (dir) => {
    const results = [];
    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      for (const entry of entries) {
        if (['node_modules', '.git', 'dist'].includes(entry.name)) continue;
        const full = path.join(dir, entry.name);
        if (entry.isDirectory()) results.push(...walkDir(full));
        else if (entry.name.endsWith('.js')) results.push(...scanCodeQuality(full));
      }
    } catch { /* skip */ }
    return results;
  };

  const qualityFindings = walkDir(target);
  const grouped = {};
  for (const f of qualityFindings) {
    grouped[f.id] = grouped[f.id] || [];
    grouped[f.id].push(f);
  }

  for (const [id, items] of Object.entries(grouped)) {
    const rule = CODE_QUALITY_RULES.find(r => r.id === id);
    console.log(`\n${rule.severity === 'WARNING' ? '⚠️' : 'ℹ️'} ${rule.name}: ${items.length} occurrence(s)`);
    for (const item of items.slice(0, 3)) {
      console.log(`   ${path.relative(ROOT, item.file)}:${item.line}`);
    }
    if (items.length > 3) console.log(`   ... and ${items.length - 3} more`);
  }

  console.log(`\n${'━'.repeat(60)}`);
  console.log(`Security fixes available: ${fixCount}/${findings.length}`);
  console.log(`Code quality issues: ${qualityFindings.length}`);
}

module.exports = { generateFix, scanCodeQuality, FIXES, CODE_QUALITY_RULES };

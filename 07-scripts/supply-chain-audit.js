#!/usr/bin/env node
'use strict';

/**
 * Ierahkwa Platform — Supply Chain Audit Tool
 * Defends against Shai-Hulud-style supply chain attacks
 *
 * Checks for:
 * 1. Malicious lifecycle scripts in dependencies
 * 2. Dependency confusion (private package names on public registry)
 * 3. Suspicious package patterns
 * 4. Lockfile integrity
 * 5. Known malicious packages
 */

const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');

// ── Known Suspicious Lifecycle Script Patterns ──
const SUSPICIOUS_PATTERNS = [
  /eval\s*\(/,
  /Buffer\.from\s*\([^)]*,\s*['"]base64['"]\)/,
  /child_process/,
  /require\s*\(\s*['"]https?['"]\s*\)/,
  /\.exec\s*\(/,
  /process\.env\.(npm_|NPM_)/,
  /curl\s+/,
  /wget\s+/,
  /powershell/i,
  /nc\s+-/,
  /\/dev\/tcp/,
  /socket\.connect/,
  /dns\.resolve/,
  /crypto\.createCipher/,
  /\.npmrc/,
  /\.netrc/,
  /ssh.*id_rsa/,
  /aws.*credentials/,
];

// ── Known Malicious Package Name Patterns ──
const MALICIOUS_PATTERNS = [
  /^@[a-z]+-internal\//,  // Dependency confusion targeting internal packages
  /test-malware/i,
  /reverse-shell/i,
  /crypto-?miner/i,
  /keylogger/i,
];

// ── Scan a package.json for risks ──
function auditPackageJson(pkgPath) {
  const issues = [];

  try {
    const pkg = JSON.parse(fs.readFileSync(pkgPath, 'utf-8'));
    const dir = path.dirname(pkgPath);
    const name = pkg.name || path.basename(dir);

    // Check lifecycle scripts
    const dangerousScripts = ['preinstall', 'install', 'postinstall', 'preuninstall', 'postuninstall'];
    for (const script of dangerousScripts) {
      if (pkg.scripts && pkg.scripts[script]) {
        const scriptContent = pkg.scripts[script];
        for (const pattern of SUSPICIOUS_PATTERNS) {
          if (pattern.test(scriptContent)) {
            issues.push({
              severity: 'CRITICAL',
              file: pkgPath,
              issue: `Suspicious ${script} script matches pattern: ${pattern}`,
              script: scriptContent
            });
          }
        }
        // Flag any postinstall that runs arbitrary commands
        if (script === 'postinstall' && /node\s+[a-z]/.test(scriptContent) && !scriptContent.includes('husky')) {
          issues.push({
            severity: 'HIGH',
            file: pkgPath,
            issue: `postinstall runs a node script: "${scriptContent}"`,
            recommendation: 'Review script contents for malicious behavior'
          });
        }
      }
    }

    // Check for dependency confusion risk
    const allDeps = { ...pkg.dependencies, ...pkg.devDependencies };
    for (const [dep, version] of Object.entries(allDeps || {})) {
      // Check for malicious package name patterns
      for (const pattern of MALICIOUS_PATTERNS) {
        if (pattern.test(dep)) {
          issues.push({
            severity: 'CRITICAL',
            file: pkgPath,
            issue: `Suspicious dependency name: "${dep}" matches known malicious pattern`,
            recommendation: 'Remove immediately and audit system'
          });
        }
      }

      // Check for git:// protocol (can be MITM'd)
      if (typeof version === 'string' && version.startsWith('git://')) {
        issues.push({
          severity: 'HIGH',
          file: pkgPath,
          issue: `Dependency "${dep}" uses insecure git:// protocol`,
          recommendation: 'Use https:// or git+ssh:// instead'
        });
      }

      // Check for URL dependencies (bypass registry)
      if (typeof version === 'string' && /^https?:\/\//.test(version)) {
        issues.push({
          severity: 'MEDIUM',
          file: pkgPath,
          issue: `Dependency "${dep}" installed from URL: ${version}`,
          recommendation: 'Prefer registry packages with integrity hashes'
        });
      }

      // Check for wildcard versions
      if (version === '*' || version === 'latest') {
        issues.push({
          severity: 'HIGH',
          file: pkgPath,
          issue: `Dependency "${dep}" uses wildcard version: ${version}`,
          recommendation: 'Pin to specific version range'
        });
      }
    }

    // Check for missing lockfile
    const lockPath = path.join(dir, 'package-lock.json');
    if (!fs.existsSync(lockPath) && Object.keys(allDeps || {}).length > 0) {
      issues.push({
        severity: 'MEDIUM',
        file: pkgPath,
        issue: 'No package-lock.json found — builds are not reproducible',
        recommendation: 'Run npm install to generate lockfile and commit it'
      });
    }

  } catch (err) {
    issues.push({
      severity: 'LOW',
      file: pkgPath,
      issue: `Failed to parse: ${err.message}`
    });
  }

  return issues;
}

// ── Scan node_modules for suspicious packages ──
function scanNodeModules(rootDir) {
  const issues = [];
  const nmPath = path.join(rootDir, 'node_modules');

  if (!fs.existsSync(nmPath)) return issues;

  try {
    const packages = fs.readdirSync(nmPath);
    for (const pkg of packages) {
      if (pkg.startsWith('.')) continue;

      const pkgJsonPath = path.join(nmPath, pkg, 'package.json');
      if (!fs.existsSync(pkgJsonPath)) continue;

      try {
        const pkgJson = JSON.parse(fs.readFileSync(pkgJsonPath, 'utf-8'));

        // Check for lifecycle scripts in installed packages
        const lifecycleScripts = ['preinstall', 'install', 'postinstall'];
        for (const script of lifecycleScripts) {
          if (pkgJson.scripts && pkgJson.scripts[script]) {
            for (const pattern of SUSPICIOUS_PATTERNS) {
              if (pattern.test(pkgJson.scripts[script])) {
                issues.push({
                  severity: 'CRITICAL',
                  file: pkgJsonPath,
                  issue: `Installed package "${pkg}" has suspicious ${script}: matches ${pattern}`,
                  script: pkgJson.scripts[script]
                });
              }
            }
          }
        }
      } catch { /* skip unparseable */ }
    }
  } catch { /* no node_modules */ }

  return issues;
}

// ── Main ──
function runAudit() {
  console.log('🛡️  Ierahkwa Supply Chain Security Audit');
  console.log('━'.repeat(50));
  console.log('Defending against: Shai-Hulud, dependency confusion,');
  console.log('credential harvesting, malicious lifecycle scripts\n');

  const allIssues = [];

  // Find all package.json files
  const findPackageJsons = (dir, depth = 0) => {
    if (depth > 4) return [];
    const results = [];

    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      for (const entry of entries) {
        if (entry.name === 'node_modules' || entry.name === '.git') continue;

        const fullPath = path.join(dir, entry.name);
        if (entry.isFile() && entry.name === 'package.json') {
          results.push(fullPath);
        } else if (entry.isDirectory()) {
          results.push(...findPackageJsons(fullPath, depth + 1));
        }
      }
    } catch { /* permission denied */ }

    return results;
  };

  const packageJsonFiles = findPackageJsons(ROOT);
  console.log(`📦 Found ${packageJsonFiles.length} package.json files\n`);

  // Audit each package.json
  for (const pkgPath of packageJsonFiles) {
    const relative = path.relative(ROOT, pkgPath);
    const issues = auditPackageJson(pkgPath);

    if (issues.length > 0) {
      console.log(`❌ ${relative} — ${issues.length} issue(s)`);
      issues.forEach(i => {
        const icon = i.severity === 'CRITICAL' ? '🔴' : i.severity === 'HIGH' ? '🟠' : i.severity === 'MEDIUM' ? '🟡' : '🔵';
        console.log(`   ${icon} [${i.severity}] ${i.issue}`);
      });
      allIssues.push(...issues);
    } else {
      console.log(`✅ ${relative}`);
    }
  }

  // Scan node_modules if present
  const nmIssues = scanNodeModules(ROOT);
  if (nmIssues.length > 0) {
    console.log(`\n⚠️  node_modules issues: ${nmIssues.length}`);
    nmIssues.forEach(i => console.log(`   🔴 ${i.issue}`));
    allIssues.push(...nmIssues);
  }

  // Summary
  console.log(`\n${'━'.repeat(50)}`);
  const critical = allIssues.filter(i => i.severity === 'CRITICAL').length;
  const high = allIssues.filter(i => i.severity === 'HIGH').length;
  const medium = allIssues.filter(i => i.severity === 'MEDIUM').length;
  const low = allIssues.filter(i => i.severity === 'LOW').length;

  console.log(`Total: ${allIssues.length} issues`);
  console.log(`  🔴 Critical: ${critical}`);
  console.log(`  🟠 High: ${high}`);
  console.log(`  🟡 Medium: ${medium}`);
  console.log(`  🔵 Low: ${low}`);

  if (critical > 0) {
    console.log('\n🚨 CRITICAL issues found — immediate action required!');
    process.exit(2);
  } else if (high > 0) {
    console.log('\n⚠️  High-severity issues found — review recommended');
    process.exit(1);
  } else {
    console.log('\n✅ Supply chain audit passed');
    process.exit(0);
  }
}

if (require.main === module) {
  runAudit();
}

module.exports = { auditPackageJson, scanNodeModules, runAudit, SUSPICIOUS_PATTERNS, MALICIOUS_PATTERNS };

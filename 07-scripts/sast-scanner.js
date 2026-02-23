#!/usr/bin/env node
'use strict';

/**
 * Ierahkwa Platform — Sovereign SAST Scanner
 * Static Application Security Testing
 * Replaces: Aikido.dev, SonarQube, Checkmarx, Snyk Code
 *
 * Scans for:
 * - OWASP Top 10 2025 vulnerabilities
 * - CWE patterns in JavaScript/Node.js code
 * - Injection vulnerabilities (SQL, NoSQL, Command, XSS)
 * - Authentication & session management issues
 * - Cryptographic failures
 * - Security misconfigurations
 * - Hardcoded secrets and credentials
 */

const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');

// ── SAST Rules Engine ──
const RULES = [
  // A01:2021 — Broken Access Control
  {
    id: 'SAST-001',
    cwe: 'CWE-284',
    owasp: 'A01',
    severity: 'HIGH',
    name: 'Missing Access Control',
    pattern: /app\.(get|post|put|delete|patch)\s*\([^)]*(?!auth|verify|protect|guard|middleware)/,
    description: 'Route handler may lack authentication middleware',
    recommendation: 'Add authentication middleware to all protected routes'
  },
  // A02:2021 — Cryptographic Failures
  {
    id: 'SAST-002',
    cwe: 'CWE-327',
    owasp: 'A02',
    severity: 'CRITICAL',
    name: 'Weak Cryptography',
    pattern: /crypto\.create(Cipher|Decipher)\s*\(\s*['"](?:des|rc4|md5|sha1)['"]/i,
    description: 'Use of weak or deprecated cryptographic algorithm',
    recommendation: 'Use AES-256-GCM or ChaCha20-Poly1305'
  },
  {
    id: 'SAST-003',
    cwe: 'CWE-798',
    owasp: 'A02',
    severity: 'CRITICAL',
    name: 'Hardcoded Credentials',
    pattern: /(password|secret|api_?key|token|credential)\s*[:=]\s*['"][^'"]{8,}['"]/i,
    description: 'Hardcoded credential or secret detected',
    recommendation: 'Use environment variables or a secrets manager'
  },
  {
    id: 'SAST-004',
    cwe: 'CWE-326',
    owasp: 'A02',
    severity: 'HIGH',
    name: 'Insufficient Key Length',
    pattern: /createHmac\s*\(\s*['"](?:md5|sha1)['"]/,
    description: 'HMAC using weak hash algorithm',
    recommendation: 'Use SHA-256 or SHA-512 for HMAC'
  },
  // A03:2021 — Injection
  {
    id: 'SAST-005',
    cwe: 'CWE-89',
    owasp: 'A03',
    severity: 'CRITICAL',
    name: 'SQL Injection',
    pattern: /(?:query|execute)\s*\(\s*['"`](?:SELECT|INSERT|UPDATE|DELETE|DROP).*\$\{|(?:query|execute)\s*\(\s*['"`].*\+\s*(?:req\.|user|input|param|body)/i,
    description: 'Possible SQL injection via string concatenation',
    recommendation: 'Use parameterized queries or prepared statements'
  },
  {
    id: 'SAST-006',
    cwe: 'CWE-78',
    owasp: 'A03',
    severity: 'CRITICAL',
    name: 'Command Injection',
    pattern: /(?:exec|execSync|spawn|execFile)\s*\(\s*(?:req\.|`.*\$\{|.*\+\s*(?:req\.|user|input))/,
    description: 'Possible OS command injection',
    recommendation: 'Validate and sanitize all input before shell execution'
  },
  {
    id: 'SAST-007',
    cwe: 'CWE-79',
    owasp: 'A03',
    severity: 'HIGH',
    name: 'Cross-Site Scripting (XSS)',
    pattern: /res\.(?:send|write)\s*\(\s*(?:req\.|`.*\$\{.*req\.|.*\+\s*req\.)/,
    description: 'User input reflected in response without sanitization',
    recommendation: 'Sanitize output and use Content-Security-Policy headers'
  },
  {
    id: 'SAST-008',
    cwe: 'CWE-943',
    owasp: 'A03',
    severity: 'HIGH',
    name: 'NoSQL Injection',
    pattern: /\.find\s*\(\s*\{[^}]*req\.(?:body|query|params)/,
    description: 'User input directly used in NoSQL query',
    recommendation: 'Validate input types and use query builders'
  },
  // A04:2021 — Insecure Design
  {
    id: 'SAST-009',
    cwe: 'CWE-352',
    owasp: 'A04',
    severity: 'MEDIUM',
    name: 'Missing CSRF Protection',
    pattern: /app\.post\s*\([^)]*(?!csrf|csrfProtection|csurf)/,
    description: 'POST endpoint may lack CSRF token validation',
    recommendation: 'Implement CSRF tokens for state-changing operations'
  },
  // A05:2021 — Security Misconfiguration
  {
    id: 'SAST-010',
    cwe: 'CWE-16',
    owasp: 'A05',
    severity: 'HIGH',
    name: 'Debug Mode in Production',
    pattern: /(?:DEBUG|NODE_ENV)\s*[:=]\s*['"](?:true|development)['"]/,
    description: 'Debug mode or development configuration in source code',
    recommendation: 'Use environment variables for configuration'
  },
  {
    id: 'SAST-011',
    cwe: 'CWE-942',
    owasp: 'A05',
    severity: 'HIGH',
    name: 'Permissive CORS',
    pattern: /(?:origin|Access-Control-Allow-Origin)\s*[:=]\s*['"]\*['"]/,
    description: 'CORS configured with wildcard origin',
    recommendation: 'Specify allowed origins explicitly'
  },
  {
    id: 'SAST-012',
    cwe: 'CWE-209',
    owasp: 'A05',
    severity: 'MEDIUM',
    name: 'Error Information Exposure',
    pattern: /res\.\w+\(\s*\{[^}]*(?:stack|trace|err\.message)[^}]*\}/,
    description: 'Stack trace or detailed error exposed to client',
    recommendation: 'Return generic error messages in production'
  },
  // A06:2021 — Vulnerable Components (handled by SCA)
  // A07:2021 — Auth Failures
  {
    id: 'SAST-013',
    cwe: 'CWE-307',
    owasp: 'A07',
    severity: 'HIGH',
    name: 'Missing Rate Limiting on Auth',
    pattern: /(?:login|signin|authenticate|register).*(?!rateLimit|limiter)/i,
    description: 'Authentication endpoint may lack rate limiting',
    recommendation: 'Apply rate limiting to all auth endpoints'
  },
  {
    id: 'SAST-014',
    cwe: 'CWE-521',
    owasp: 'A07',
    severity: 'MEDIUM',
    name: 'Weak Password Requirements',
    pattern: /password.*\.length\s*[<>=]+\s*[1-5]\b/,
    description: 'Password minimum length appears too short',
    recommendation: 'Require minimum 12 characters with complexity rules'
  },
  // A08:2021 — Software & Data Integrity
  {
    id: 'SAST-015',
    cwe: 'CWE-502',
    owasp: 'A08',
    severity: 'CRITICAL',
    name: 'Unsafe Deserialization',
    pattern: /JSON\.parse\s*\(\s*(?:req\.body|req\.query|req\.params|Buffer|readFile)/,
    description: 'Parsing untrusted data without validation',
    recommendation: 'Validate and sanitize before parsing, use schemas'
  },
  {
    id: 'SAST-016',
    cwe: 'CWE-94',
    owasp: 'A08',
    severity: 'CRITICAL',
    name: 'Code Injection via eval',
    pattern: /\beval\s*\(\s*(?!['"])/,
    description: 'Dynamic code execution via eval()',
    recommendation: 'Never use eval() with user input'
  },
  {
    id: 'SAST-017',
    cwe: 'CWE-1321',
    owasp: 'A08',
    severity: 'HIGH',
    name: 'Prototype Pollution',
    pattern: /(?:__proto__|constructor\.prototype|Object\.assign\s*\(\s*\{\},\s*(?:req|user|input))/,
    description: 'Possible prototype pollution vector',
    recommendation: 'Use Object.create(null) or validate keys'
  },
  // A09:2021 — Security Logging Failures
  {
    id: 'SAST-018',
    cwe: 'CWE-532',
    owasp: 'A09',
    severity: 'MEDIUM',
    name: 'Sensitive Data in Logs',
    pattern: /console\.(?:log|info|warn|error)\s*\([^)]*(?:password|token|secret|credit|ssn|apiKey)/i,
    description: 'Potentially logging sensitive information',
    recommendation: 'Redact sensitive data before logging'
  },
  // A10:2021 — Server-Side Request Forgery
  {
    id: 'SAST-019',
    cwe: 'CWE-918',
    owasp: 'A10',
    severity: 'HIGH',
    name: 'Server-Side Request Forgery (SSRF)',
    pattern: /(?:fetch|axios|http\.get|request)\s*\(\s*(?:req\.|`.*\$\{|.*\+\s*(?:req\.|user|input))/,
    description: 'URL from user input used in server-side request',
    recommendation: 'Validate and whitelist URLs before server-side requests'
  },
  // Additional patterns
  {
    id: 'SAST-020',
    cwe: 'CWE-22',
    owasp: 'A01',
    severity: 'HIGH',
    name: 'Path Traversal',
    pattern: /(?:readFile|createReadStream|access|stat|unlink|rmdir)\s*\(\s*(?:req\.|`.*\$\{|.*\+\s*(?:req\.|user|input))/,
    description: 'File path derived from user input without validation',
    recommendation: 'Use path.resolve() and validate against a base directory'
  },
  {
    id: 'SAST-021',
    cwe: 'CWE-601',
    owasp: 'A01',
    severity: 'MEDIUM',
    name: 'Open Redirect',
    pattern: /res\.redirect\s*\(\s*(?:req\.|.*\+\s*req\.)/,
    description: 'Redirect URL from user input',
    recommendation: 'Validate redirect URLs against whitelist'
  },
  {
    id: 'SAST-022',
    cwe: 'CWE-614',
    owasp: 'A02',
    severity: 'MEDIUM',
    name: 'Insecure Cookie',
    pattern: /cookie\s*\([^)]*(?!secure|httpOnly|sameSite)/i,
    description: 'Cookie may be missing security flags',
    recommendation: 'Set Secure, HttpOnly, and SameSite flags on all cookies'
  },
  {
    id: 'SAST-023',
    cwe: 'CWE-400',
    owasp: 'A05',
    severity: 'MEDIUM',
    name: 'Uncontrolled Resource Consumption',
    pattern: /JSON\.parse\s*\([^)]*\)\s*(?!.*limit)/,
    description: 'JSON parsing without size limit checking',
    recommendation: 'Limit request body size with express.json({ limit })'
  },
  {
    id: 'SAST-024',
    cwe: 'CWE-330',
    owasp: 'A02',
    severity: 'MEDIUM',
    name: 'Insecure Randomness',
    pattern: /Math\.random\s*\(\)/,
    description: 'Math.random() used — not cryptographically secure',
    recommendation: 'Use crypto.randomBytes() or crypto.randomUUID()'
  }
];

// ── File Scanner ──
function scanFile(filePath) {
  const findings = [];

  try {
    const content = fs.readFileSync(filePath, 'utf-8');
    const lines = content.split('\n');

    for (const rule of RULES) {
      for (let i = 0; i < lines.length; i++) {
        if (rule.pattern.test(lines[i])) {
          // Check if line is a comment
          const trimmed = lines[i].trim();
          if (trimmed.startsWith('//') || trimmed.startsWith('*') || trimmed.startsWith('/*')) continue;

          findings.push({
            ...rule,
            file: filePath,
            line: i + 1,
            code: lines[i].trim().substring(0, 120),
            pattern: undefined // Don't serialize regex
          });
        }
      }
    }
  } catch (err) {
    // Skip unreadable files
  }

  return findings;
}

// ── Directory Scanner ──
function scanDirectory(dir, extensions = ['.js', '.ts', '.mjs', '.cjs']) {
  const findings = [];

  const walk = (d, depth = 0) => {
    if (depth > 6) return;
    try {
      const entries = fs.readdirSync(d, { withFileTypes: true });
      for (const entry of entries) {
        if (['node_modules', '.git', 'dist', 'build', '.next', 'coverage'].includes(entry.name)) continue;

        const fullPath = path.join(d, entry.name);
        if (entry.isDirectory()) {
          walk(fullPath, depth + 1);
        } else if (extensions.some(ext => entry.name.endsWith(ext))) {
          findings.push(...scanFile(fullPath));
        }
      }
    } catch { /* skip */ }
  };

  walk(dir);
  return findings;
}

// ── Generate SARIF Report ──
function generateSARIF(findings) {
  return {
    $schema: 'https://raw.githubusercontent.com/oasis-tcs/sarif-spec/master/Schemata/sarif-schema-2.1.0.json',
    version: '2.1.0',
    runs: [{
      tool: {
        driver: {
          name: 'Ierahkwa SAST Scanner',
          version: '1.0.0',
          informationUri: 'https://github.com/rudvincci/ierahkwa-platform',
          rules: RULES.map(r => ({
            id: r.id,
            shortDescription: { text: r.name },
            fullDescription: { text: r.description },
            help: { text: r.recommendation },
            properties: { severity: r.severity, cwe: r.cwe, owasp: r.owasp }
          }))
        }
      },
      results: findings.map(f => ({
        ruleId: f.id,
        level: f.severity === 'CRITICAL' ? 'error' : f.severity === 'HIGH' ? 'warning' : 'note',
        message: { text: `${f.name}: ${f.description}` },
        locations: [{
          physicalLocation: {
            artifactLocation: { uri: path.relative(ROOT, f.file) },
            region: { startLine: f.line }
          }
        }]
      }))
    }]
  };
}

// ── Main ──
function runSAST(targetDir) {
  const scanDir = targetDir || ROOT;

  console.log('🔍 Ierahkwa Sovereign SAST Scanner');
  console.log('━'.repeat(60));
  console.log(`Target: ${scanDir}`);
  console.log(`Rules: ${RULES.length} (OWASP Top 10 2025 + CWE)`);
  console.log('');

  const findings = scanDirectory(scanDir);

  // Group by severity
  const critical = findings.filter(f => f.severity === 'CRITICAL');
  const high = findings.filter(f => f.severity === 'HIGH');
  const medium = findings.filter(f => f.severity === 'MEDIUM');

  // Display findings
  const display = (list, icon) => {
    for (const f of list) {
      const rel = path.relative(ROOT, f.file);
      console.log(`${icon} [${f.id}] ${f.name}`);
      console.log(`   ${rel}:${f.line}`);
      console.log(`   ${f.code}`);
      console.log(`   💡 ${f.recommendation}`);
      console.log('');
    }
  };

  if (critical.length) { console.log('🔴 CRITICAL\n'); display(critical, '🔴'); }
  if (high.length) { console.log('🟠 HIGH\n'); display(high, '🟠'); }
  if (medium.length) { console.log('🟡 MEDIUM\n'); display(medium, '🟡'); }

  // Summary
  console.log('━'.repeat(60));
  console.log(`Total findings: ${findings.length}`);
  console.log(`  🔴 Critical: ${critical.length}`);
  console.log(`  🟠 High: ${high.length}`);
  console.log(`  🟡 Medium: ${medium.length}`);
  console.log(`  Files scanned: ${new Set(findings.map(f => f.file)).size}`);

  // SARIF output
  const args = process.argv.slice(2);
  if (args.includes('--sarif')) {
    const sarif = generateSARIF(findings);
    const sarifPath = path.join(ROOT, 'sast-results.sarif');
    fs.writeFileSync(sarifPath, JSON.stringify(sarif, null, 2));
    console.log(`\n📄 SARIF report: ${sarifPath}`);
  }

  // Exit code
  if (critical.length > 0) process.exit(2);
  if (high.length > 0) process.exit(1);
  process.exit(0);
}

if (require.main === module) {
  const target = process.argv[2] && !process.argv[2].startsWith('--') ? process.argv[2] : undefined;
  runSAST(target);
}

module.exports = { scanFile, scanDirectory, generateSARIF, runSAST, RULES };

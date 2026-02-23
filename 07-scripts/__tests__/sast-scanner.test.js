'use strict';

const path = require('path');
const fs = require('fs');
const { scanFile, RULES } = require('../sast-scanner');

describe('SAST Scanner', () => {
  const tmpDir = path.join(__dirname, 'tmp-sast');

  beforeAll(() => { if (!fs.existsSync(tmpDir)) fs.mkdirSync(tmpDir); });
  afterAll(() => { fs.rmSync(tmpDir, { recursive: true, force: true }); });

  test('has 24 security rules', () => {
    expect(RULES.length).toBe(24);
  });

  test('all rules have required fields', () => {
    for (const rule of RULES) {
      expect(rule.id).toMatch(/^SAST-\d{3}$/);
      expect(rule.cwe).toMatch(/^CWE-\d+$/);
      expect(rule.owasp).toMatch(/^A\d{2}$/);
      expect(['CRITICAL', 'HIGH', 'MEDIUM']).toContain(rule.severity);
      expect(rule.name).toBeTruthy();
      expect(rule.description).toBeTruthy();
      expect(rule.recommendation).toBeTruthy();
    }
  });

  test('detects SQL injection', () => {
    const file = path.join(tmpDir, 'sqli.js');
    fs.writeFileSync(file, 'db.query("SELECT * FROM users WHERE id=" + req.params.id);');
    const findings = scanFile(file);
    expect(findings.some(f => f.id === 'SAST-005')).toBe(true);
  });

  test('detects eval usage', () => {
    const file = path.join(tmpDir, 'eval.js');
    fs.writeFileSync(file, 'const result = eval(userInput);');
    const findings = scanFile(file);
    expect(findings.some(f => f.id === 'SAST-016')).toBe(true);
  });

  test('detects hardcoded secrets', () => {
    const file = path.join(tmpDir, 'secret.js');
    fs.writeFileSync(file, 'const apiKey = "sk_live_1234567890abcdef";');
    const findings = scanFile(file);
    expect(findings.some(f => f.id === 'SAST-003')).toBe(true);
  });

  test('detects wildcard CORS', () => {
    const file = path.join(tmpDir, 'cors.js');
    fs.writeFileSync(file, 'res.setHeader("Access-Control-Allow-Origin", "*");');
    const findings = scanFile(file);
    expect(findings.some(f => f.id === 'SAST-011')).toBe(true);
  });

  test('detects Math.random for security', () => {
    const file = path.join(tmpDir, 'random.js');
    fs.writeFileSync(file, 'const token = Math.random().toString(36);');
    const findings = scanFile(file);
    expect(findings.some(f => f.id === 'SAST-024')).toBe(true);
  });

  test('skips comments', () => {
    const file = path.join(tmpDir, 'comment.js');
    fs.writeFileSync(file, '// eval(userInput)\nconst safe = true;');
    const findings = scanFile(file);
    expect(findings.filter(f => f.id === 'SAST-016')).toHaveLength(0);
  });

  test('returns line numbers', () => {
    const file = path.join(tmpDir, 'lines.js');
    fs.writeFileSync(file, 'line1\nline2\nconst x = eval(input);\nline4');
    const findings = scanFile(file);
    const evalFinding = findings.find(f => f.id === 'SAST-016');
    expect(evalFinding).toBeDefined();
    expect(evalFinding.line).toBe(3);
  });
});

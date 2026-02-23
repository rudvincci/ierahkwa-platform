'use strict';

const { generateFix, scanCodeQuality, FIXES, CODE_QUALITY_RULES } = require('../sast-autofix');

describe('SAST AutoFix Engine', () => {
  test('has fixes for key rules', () => {
    expect(Object.keys(FIXES).length).toBeGreaterThanOrEqual(8);
    expect(FIXES['SAST-003']).toBeDefined();
    expect(FIXES['SAST-011']).toBeDefined();
    expect(FIXES['SAST-016']).toBeDefined();
    expect(FIXES['SAST-024']).toBeDefined();
  });

  test('fixes hardcoded credentials', () => {
    const finding = { id: 'SAST-003', file: 'test.js', line: 1 };
    const content = 'const apiKey = "sk_live_1234567890abcdef";';
    const fix = generateFix(finding, content);
    expect(fix).toBeDefined();
    expect(fix.fixed).toContain('process.env');
    expect(fix.confidence).toBe('HIGH');
  });

  test('fixes Math.random', () => {
    const finding = { id: 'SAST-024', file: 'test.js', line: 1 };
    const content = 'const id = Math.random().toString(36);';
    const fix = generateFix(finding, content);
    expect(fix).toBeDefined();
    expect(fix.fixed).toContain('randomUUID');
  });

  test('fixes wildcard CORS', () => {
    const finding = { id: 'SAST-011', file: 'test.js', line: 1 };
    const content = "res.setHeader('Access-Control-Allow-Origin', '*');";
    const fix = generateFix(finding, content);
    expect(fix).toBeDefined();
    expect(fix.fixed).toContain('ALLOWED_ORIGINS');
  });

  test('returns null for unknown rules', () => {
    const finding = { id: 'SAST-999', file: 'test.js', line: 1 };
    const fix = generateFix(finding, 'some code');
    expect(fix).toBeNull();
  });
});

describe('Code Quality Rules', () => {
  test('has 8 quality rules', () => {
    expect(CODE_QUALITY_RULES.length).toBe(8);
  });

  test('detects console.log', () => {
    const fs = require('fs');
    const path = require('path');
    const tmp = path.join(__dirname, 'tmp-cq.js');
    fs.writeFileSync(tmp, 'console.log("debug info");');
    const findings = scanCodeQuality(tmp);
    expect(findings.some(f => f.id === 'CQ-001')).toBe(true);
    fs.unlinkSync(tmp);
  });

  test('detects var usage', () => {
    const fs = require('fs');
    const path = require('path');
    const tmp = path.join(__dirname, 'tmp-var.js');
    fs.writeFileSync(tmp, 'var x = 5;');
    const findings = scanCodeQuality(tmp);
    expect(findings.some(f => f.id === 'CQ-007')).toBe(true);
    fs.unlinkSync(tmp);
  });

  test('detects empty catch', () => {
    const fs = require('fs');
    const path = require('path');
    const tmp = path.join(__dirname, 'tmp-catch.js');
    fs.writeFileSync(tmp, 'try { x(); } catch (e) {}');
    const findings = scanCodeQuality(tmp);
    expect(findings.some(f => f.id === 'CQ-008')).toBe(true);
    fs.unlinkSync(tmp);
  });
});

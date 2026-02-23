'use strict';

const { SKILLS, Session, gatherContext, resolveFileRefs, DEFAULT_CONFIG } = require('../agent');
const path = require('path');
const fs = require('fs');

describe('Agente Soberano', () => {
  describe('Skills', () => {
    test('has 8 skills', () => {
      expect(Object.keys(SKILLS).length).toBe(8);
    });

    test('all skills have required fields', () => {
      for (const [key, skill] of Object.entries(SKILLS)) {
        expect(skill.name).toBeTruthy();
        expect(skill.description).toBeTruthy();
        expect(skill.systemPrompt).toBeTruthy();
        expect(skill.systemPrompt.length).toBeGreaterThan(20);
      }
    });

    test('includes sovereign-specific skills', () => {
      expect(SKILLS.sovereign).toBeDefined();
      expect(SKILLS.a11y).toBeDefined();
      expect(SKILLS.security).toBeDefined();
    });
  });

  describe('Session', () => {
    test('creates with unique ID', () => {
      const s1 = new Session();
      const s2 = new Session();
      expect(s1.id).toBeTruthy();
      expect(s1.id).not.toBe(s2.id);
    });

    test('tracks messages', () => {
      const session = new Session('test-1');
      session.addMessage('user', 'hello');
      session.addMessage('assistant', 'hi');
      expect(session.messages.length).toBe(2);
      expect(session.messages[0].role).toBe('user');
      expect(session.messages[0].timestamp).toBeDefined();
    });

    test('serializes to JSON', () => {
      const session = new Session('json-test');
      session.skill = 'security';
      const json = session.toJSON();
      expect(json.id).toBe('json-test');
      expect(json.skill).toBe('security');
      expect(json.startTime).toBeDefined();
    });
  });

  describe('Context Gathering', () => {
    test('gathers project context', () => {
      const ctx = gatherContext(process.cwd());
      expect(ctx.cwd).toBeTruthy();
      expect(ctx.structure).toBeInstanceOf(Array);
    });
  });

  describe('File References', () => {
    test('resolves @file references', () => {
      const tmpFile = path.join(__dirname, 'tmp-ref.js');
      fs.writeFileSync(tmpFile, 'const x = 1;');
      const refs = resolveFileRefs('@__tests__/tmp-ref.js', path.join(__dirname, '..'));
      expect(refs['@__tests__/tmp-ref.js']).toContain('const x = 1');
      fs.unlinkSync(tmpFile);
    });
  });

  describe('Config', () => {
    test('default config has privacy settings', () => {
      expect(DEFAULT_CONFIG.telemetry).toBe(false);
      expect(DEFAULT_CONFIG.privacy.storeCode).toBe(false);
      expect(DEFAULT_CONFIG.privacy.sendAnalytics).toBe(false);
    });

    test('supports multiple providers', () => {
      expect(DEFAULT_CONFIG.providers.local).toBeDefined();
      expect(DEFAULT_CONFIG.providers.openai).toBeDefined();
      expect(DEFAULT_CONFIG.providers.anthropic).toBeDefined();
      expect(DEFAULT_CONFIG.providers.sovereign).toBeDefined();
    });
  });
});

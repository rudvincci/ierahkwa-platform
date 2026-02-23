'use strict';

const {
  htmlAccessibilityCheck,
  accessibleErrorHandler
} = require('../accessibility');

describe('Accessibility Middleware', () => {

  describe('htmlAccessibilityCheck', () => {
    test('passes for fully accessible HTML', () => {
      const html = `
        <html lang="en">
        <head>
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>Ierahkwa Platform</title>
        </head>
        <body>
          <a href="#main">Skip to content</a>
          <header><nav><h1>Platform</h1></nav></header>
          <main id="main">
            <h2>Welcome</h2>
            <img src="logo.png" alt="Ierahkwa Logo">
            <form>
              <label for="name">Name</label>
              <input id="name" type="text" aria-label="Your name">
            </form>
          </main>
          <footer></footer>
        </body>
        </html>
      `;
      const result = htmlAccessibilityCheck(html);
      expect(result.passed).toBe(true);
      expect(result.score).toBeGreaterThanOrEqual(80);
    });

    test('fails for HTML missing lang attribute', () => {
      const html = '<html><head><title>Test</title></head><body><main></main></body></html>';
      const result = htmlAccessibilityCheck(html);
      const langIssue = result.issues.find(i => i.rule === 'html-has-lang');
      expect(langIssue).toBeDefined();
      expect(langIssue.severity).toBe('critical');
    });

    test('detects images without alt text', () => {
      const html = '<html lang="en"><head><title>Test</title></head><body><main><img src="photo.jpg"></main></body></html>';
      const result = htmlAccessibilityCheck(html);
      const altIssue = result.issues.find(i => i.rule === 'image-alt');
      expect(altIssue).toBeDefined();
      expect(altIssue.severity).toBe('critical');
    });

    test('detects missing skip navigation', () => {
      const html = '<html lang="en"><head><title>Test</title><meta name="viewport" content="width=device-width"></head><body><main><h1>Content</h1></main></body></html>';
      const result = htmlAccessibilityCheck(html);
      const skipIssue = result.issues.find(i => i.rule === 'skip-nav');
      expect(skipIssue).toBeDefined();
    });

    test('detects positive tabindex', () => {
      const html = '<html lang="en"><head><title>Test</title></head><body><main><button tabindex="5">Click</button></main></body></html>';
      const result = htmlAccessibilityCheck(html);
      const tabIssue = result.issues.find(i => i.rule === 'tabindex');
      expect(tabIssue).toBeDefined();
      expect(tabIssue.severity).toBe('serious');
    });

    test('returns score and timestamp', () => {
      const html = '<html lang="en"><head><title>Test</title></head><body><main></main></body></html>';
      const result = htmlAccessibilityCheck(html);
      expect(typeof result.score).toBe('number');
      expect(result.score).toBeLessThanOrEqual(100);
      expect(result.checkedAt).toBeDefined();
    });
  });

  describe('accessibleErrorHandler', () => {
    test('returns structured error for client errors', () => {
      const err = { status: 404, message: 'Not found', code: 'NOT_FOUND' };
      const req = {};
      const res = {
        statusCode: null,
        body: null,
        status(code) { this.statusCode = code; return this; },
        json(data) { this.body = data; }
      };
      const next = jest.fn();

      accessibleErrorHandler(err, req, res, next);

      expect(res.statusCode).toBe(404);
      expect(res.body.error.message).toBe('Not found');
      expect(res.body.error.code).toBe('NOT_FOUND');
      expect(res.body.error.action).toBeDefined();
      expect(res.body.error.timestamp).toBeDefined();
    });

    test('hides internal details for server errors', () => {
      const err = { status: 500, message: 'Database connection pool exhausted' };
      const req = {};
      const res = {
        status(code) { this.statusCode = code; return this; },
        json(data) { this.body = data; }
      };

      accessibleErrorHandler(err, req, res, () => {});

      expect(res.body.error.message).toBe('An unexpected error occurred');
    });
  });
});

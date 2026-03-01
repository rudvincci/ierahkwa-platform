const http = require('http');

let app;
let server;
let baseUrl;

/**
 * Helper: make an HTTP request and return { statusCode, headers, body }
 */
function request(method, path) {
  return new Promise((resolve, reject) => {
    const url = new URL(path, baseUrl);
    const req = http.request(url, { method }, (res) => {
      let data = '';
      res.on('data', (chunk) => { data += chunk; });
      res.on('end', () => {
        let body = data;
        try { body = JSON.parse(data); } catch (_) { /* keep as string */ }
        resolve({ statusCode: res.statusCode, headers: res.headers, body });
      });
    });
    req.on('error', reject);
    req.end();
  });
}

beforeAll((done) => {
  // Import the Express app from server.js
  app = require('../server');

  // If server.js exports a listening server, use its address;
  // otherwise create our own server from the express app.
  if (app && typeof app.listen === 'function' && !app.listening) {
    server = app.listen(0, () => {
      const { port } = server.address();
      baseUrl = `http://127.0.0.1:${port}`;
      done();
    });
  } else if (app && app.address && typeof app.address === 'function') {
    // Already listening
    server = app;
    const addr = server.address();
    baseUrl = `http://127.0.0.1:${addr.port}`;
    done();
  } else {
    // app is an Express instance, spin up a server
    server = app.listen(0, () => {
      const { port } = server.address();
      baseUrl = `http://127.0.0.1:${port}`;
      done();
    });
  }
});

afterAll((done) => {
  if (server && typeof server.close === 'function') {
    server.close(done);
  } else {
    done();
  }
});

// ─── Tests ───────────────────────────────────────────────────────────

describe('GET /health', () => {
  test('returns 200 with status ok', async () => {
    const res = await request('GET', '/health');
    expect(res.statusCode).toBe(200);
    expect(res.body).toBeDefined();
    expect(res.body.status).toBe('ok');
  });
});

describe('POST /v1/auth/register — validation', () => {
  test('returns 400 when required fields are missing', async () => {
    const res = await request('POST', '/v1/auth/register');
    expect(res.statusCode).toBe(400);
  });
});

describe('GET /v1/nonexistent', () => {
  test('returns 404 for unknown routes', async () => {
    const res = await request('GET', '/v1/nonexistent');
    expect(res.statusCode).toBe(404);
  });
});

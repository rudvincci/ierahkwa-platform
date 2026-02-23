'use strict';

const { test, expect } = require('@playwright/test');

/**
 * API Health Check E2E Tests
 * Verifies all backend services are responding correctly
 * Run with: npx playwright test api-health.spec.js
 * Requires services to be running (docker-compose up)
 */

const SERVICES = [
  { name: 'API Gateway', port: 3000, path: '/health' },
  { name: 'Plataforma Principal', port: 3001, path: '/health' },
  { name: 'Blockchain API', port: 3010, path: '/health' },
  { name: 'POS System', port: 3020, path: '/health' },
  { name: 'Ierahkwa Shop', port: 3030, path: '/health' },
  { name: 'Inventory System', port: 3040, path: '/health' },
  { name: 'Image Upload', port: 3050, path: '/health' },
  { name: 'Forex Trading', port: 3060, path: '/health' },
  { name: 'Voto Soberano', port: 3070, path: '/health' },
  { name: 'Conferencia Soberana', port: 3090, path: '/health' },
  { name: 'Vigilancia Soberana', port: 3091, path: '/health' },
  { name: 'Empresa Soberana', port: 3092, path: '/health' },
];

test.describe('Backend Service Health Checks', () => {
  for (const service of SERVICES) {
    test(`${service.name} (port ${service.port}) — responds healthy`, async ({ request }) => {
      try {
        const response = await request.get(`http://localhost:${service.port}${service.path}`);
        expect(response.status()).toBe(200);

        const body = await response.json();
        expect(body.status).toBeTruthy();
      } catch (err) {
        // Service not running — skip gracefully in non-CI environments
        test.skip(!process.env.CI, `${service.name} not running locally`);
        throw err;
      }
    });
  }
});

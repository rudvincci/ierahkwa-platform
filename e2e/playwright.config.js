'use strict';

/**
 * Ierahkwa Platform — E2E Testing Configuration
 * Playwright-based end-to-end testing for sovereign platforms
 */

const { defineConfig } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './tests',
  timeout: 30000,
  retries: 1,
  workers: 4,

  reporter: [
    ['html', { outputFolder: '../ACCESSIBILITY-REPORT-E2E' }],
    ['json', { outputFile: 'results.json' }],
    ['list']
  ],

  use: {
    baseURL: 'http://localhost:3000',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'retain-on-failure',

    // Accessibility testing defaults
    colorScheme: 'dark',
    locale: 'en-US',
    timezoneId: 'America/New_York',
  },

  projects: [
    {
      name: 'Desktop Chrome',
      use: {
        browserName: 'chromium',
        viewport: { width: 1280, height: 720 },
      },
    },
    {
      name: 'Mobile Safari',
      use: {
        browserName: 'webkit',
        viewport: { width: 375, height: 667 },
        isMobile: true,
      },
    },
    {
      name: 'Accessibility',
      use: {
        browserName: 'chromium',
        viewport: { width: 1280, height: 720 },
      },
      testMatch: '**/*.a11y.spec.js',
    },
  ],
});

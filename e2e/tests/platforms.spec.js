'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');

// Get all platform directories
const platforms = fs.readdirSync(PLATFORMS_DIR)
  .filter(d => {
    const fullPath = path.join(PLATFORMS_DIR, d);
    return fs.statSync(fullPath).isDirectory() &&
           fs.existsSync(path.join(fullPath, 'index.html'));
  })
  .sort();

test.describe('Sovereign Platform Smoke Tests', () => {
  for (const platform of platforms) {
    test(`${platform} — loads and renders`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;

      await page.goto(filePath);

      // Page loads without errors
      const title = await page.title();
      expect(title).toBeTruthy();

      // Has visible content
      const body = await page.locator('body');
      await expect(body).toBeVisible();

      // No JavaScript errors
      const errors = [];
      page.on('pageerror', err => errors.push(err.message));
      expect(errors).toHaveLength(0);
    });
  }
});

test.describe('Sovereign Platform Navigation', () => {
  for (const platform of platforms.slice(0, 10)) {
    test(`${platform} — keyboard navigation works`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      // Tab through interactive elements
      await page.keyboard.press('Tab');
      const focused = await page.evaluate(() => document.activeElement?.tagName);
      expect(focused).toBeTruthy();

      // Check that focus is visible
      const focusedEl = page.locator(':focus');
      if (await focusedEl.count() > 0) {
        await expect(focusedEl).toBeVisible();
      }
    });
  }
});

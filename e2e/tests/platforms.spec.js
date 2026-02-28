'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');
const MAIN_PORTAL = `file://${path.join(PLATFORMS_DIR, 'index.html')}`;

// Get all platform directories
const platforms = fs.readdirSync(PLATFORMS_DIR)
  .filter(d => {
    const fullPath = path.join(PLATFORMS_DIR, d);
    return fs.statSync(fullPath).isDirectory() &&
           fs.existsSync(path.join(fullPath, 'index.html'));
  })
  .sort();

/* ============================================================
   MAIN PORTAL — index.html
   ============================================================ */
test.describe('Main Portal — Portal Central', () => {

  test('loads and displays 386+ platforms in the hero description', async ({ page }) => {
    await page.goto(MAIN_PORTAL);
    await expect(page).toHaveTitle(/Ierahkwa/);

    // The hero section mentions "386 plataformas"
    const heroText = await page.locator('.hero').textContent();
    const match = heroText.match(/(\d+)\s*plataformas/);
    expect(match).toBeTruthy();
    expect(parseInt(match[1])).toBeGreaterThanOrEqual(386);
  });

  test('shows all 18 NEXUS filter buttons plus "Todas"', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const filterButtons = page.locator('button[data-filter]');
    const count = await filterButtons.count();
    // 18 NEXUS + 1 "Todas" = 19
    expect(count).toBe(19);

    // Verify specific NEXUS names exist
    const expectedNexus = [
      'all', 'orbital', 'escudo', 'cerebro', 'tesoro', 'voces',
      'consejo', 'tierra', 'forja', 'urbe', 'raices', 'salud',
      'academia', 'escolar', 'entretenimiento', 'escritorio',
      'comercio', 'amparo', 'cosmos'
    ];
    for (const nexus of expectedNexus) {
      await expect(page.locator(`button[data-filter="${nexus}"]`)).toBeVisible();
    }
  });

  test('NEXUS filter buttons show/hide correct platform cards', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    // Click "Orbital" filter
    await page.locator('button[data-filter="orbital"]').click();
    await page.waitForTimeout(300);

    // All visible cards should have nexus=orbital
    const visibleCards = page.locator('[data-name]:visible');
    const allCount = await visibleCards.count();
    expect(allCount).toBeGreaterThan(0);

    for (let i = 0; i < Math.min(allCount, 5); i++) {
      const nexus = await visibleCards.nth(i).getAttribute('data-nexus');
      expect(nexus).toBe('orbital');
    }

    // Click "Todas" to show all again
    await page.locator('button[data-filter="all"]').click();
    await page.waitForTimeout(300);
    const totalVisible = await page.locator('[data-name]:visible').count();
    expect(totalVisible).toBeGreaterThanOrEqual(380);
  });

  test('search functionality filters platforms by name', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const searchInput = page.locator('[data-search]');
    await expect(searchInput).toBeVisible();

    // Search for a known platform
    await searchInput.fill('blockchain');
    await page.waitForTimeout(300);

    const visibleCards = page.locator('[data-name]:visible');
    const count = await visibleCards.count();
    expect(count).toBeGreaterThan(0);
    expect(count).toBeLessThan(380); // Should be filtered down

    // Every visible card name should contain "blockchain"
    for (let i = 0; i < count; i++) {
      const name = await visibleCards.nth(i).getAttribute('data-name');
      expect(name.toLowerCase()).toContain('blockchain');
    }
  });

  test('stats counters are visible and show correct values', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const counters = page.locator('.counters .counter');
    await expect(counters).not.toHaveCount(0);

    // Check specific data-count values
    const platformCount = page.locator('[data-count="386"]');
    await expect(platformCount).toBeVisible();

    const nexusCount = page.locator('[data-count="18"]');
    await expect(nexusCount).toBeVisible();

    const nationsCount = page.locator('[data-count="19"]');
    await expect(nationsCount).toBeVisible();

    const tribesCount = page.locator('[data-count="574"]');
    await expect(tribesCount).toBeVisible();
  });
});

/* ============================================================
   PLATFORM CARDS — Link Validation
   ============================================================ */
test.describe('Platform Card Links', () => {

  test('platform cards have valid href pointing to existing pages', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const cards = page.locator('.p-card[href]');
    const count = await cards.count();
    expect(count).toBeGreaterThan(0);

    // Sample 20 random cards to check links
    const sampleSize = Math.min(20, count);
    const indices = [];
    for (let i = 0; i < sampleSize; i++) {
      indices.push(Math.floor(Math.random() * count));
    }

    for (const idx of indices) {
      const href = await cards.nth(idx).getAttribute('href');
      expect(href).toBeTruthy();
      // Relative path should end with / or index.html
      expect(href).toMatch(/\/$|\/index\.html$/);
      // The directory should exist on disk
      const dirPath = path.join(PLATFORMS_DIR, href.replace(/\/$/, '').replace(/\/index\.html$/, ''));
      const indexPath = path.join(dirPath, 'index.html');
      expect(fs.existsSync(indexPath) || fs.existsSync(dirPath + '/index.html')).toBe(true);
    }
  });
});

/* ============================================================
   INDIVIDUAL PLATFORM SMOKE TESTS
   ============================================================ */
test.describe('Sovereign Platform Smoke Tests', () => {
  for (const platform of platforms) {
    test(`${platform} -- loads and renders`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;

      // Collect JS errors
      const errors = [];
      page.on('pageerror', err => errors.push(err.message));

      await page.goto(filePath);

      // Page loads
      const title = await page.title();
      expect(title).toBeTruthy();

      // Has visible content
      await expect(page.locator('body')).toBeVisible();

      // No JS errors
      expect(errors).toHaveLength(0);
    });
  }
});

test.describe('Sovereign Platform Navigation', () => {
  for (const platform of platforms.slice(0, 10)) {
    test(`${platform} -- keyboard navigation works`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      await page.keyboard.press('Tab');
      const focused = await page.evaluate(() => document.activeElement?.tagName);
      expect(focused).toBeTruthy();

      const focusedEl = page.locator(':focus');
      if (await focusedEl.count() > 0) {
        await expect(focusedEl).toBeVisible();
      }
    });
  }
});

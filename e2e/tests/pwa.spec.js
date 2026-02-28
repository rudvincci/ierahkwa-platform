'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');
const SHARED_DIR = path.join(PLATFORMS_DIR, 'shared');
const MAIN_PORTAL = `file://${path.join(PLATFORMS_DIR, 'index.html')}`;

/* ============================================================
   PWA — manifest.json Validation
   ============================================================ */
test.describe('PWA — Manifest', () => {

  test('manifest.json exists and is valid JSON', async () => {
    const manifestPath = path.join(SHARED_DIR, 'manifest.json');
    expect(fs.existsSync(manifestPath)).toBe(true);

    const content = fs.readFileSync(manifestPath, 'utf-8');
    const manifest = JSON.parse(content); // will throw if invalid JSON
    expect(manifest).toBeTruthy();
  });

  test('manifest.json has required PWA fields', async () => {
    const manifestPath = path.join(SHARED_DIR, 'manifest.json');
    const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf-8'));

    // Required fields for installable PWA
    expect(manifest.name, 'missing name').toBeTruthy();
    expect(manifest.short_name, 'missing short_name').toBeTruthy();
    expect(manifest.start_url, 'missing start_url').toBeTruthy();
    expect(manifest.display, 'missing display').toBeTruthy();
    expect(manifest.background_color, 'missing background_color').toBeTruthy();
    expect(manifest.theme_color, 'missing theme_color').toBeTruthy();
    expect(manifest.icons, 'missing icons').toBeTruthy();
    expect(Array.isArray(manifest.icons)).toBe(true);

    // Display should be standalone, fullscreen, or minimal-ui
    expect(['standalone', 'fullscreen', 'minimal-ui']).toContain(manifest.display);
  });

  test('manifest.json icons are defined with required sizes', async () => {
    const manifestPath = path.join(SHARED_DIR, 'manifest.json');
    const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf-8'));

    expect(manifest.icons.length).toBeGreaterThanOrEqual(2);

    // Must have at least 192x192 and 512x512
    const sizes = manifest.icons.map(i => i.sizes);
    expect(sizes.some(s => s.includes('192'))).toBe(true);
    expect(sizes.some(s => s.includes('512'))).toBe(true);

    // Each icon must have src, sizes, and type
    for (const icon of manifest.icons) {
      expect(icon.src, 'icon missing src').toBeTruthy();
      expect(icon.sizes, 'icon missing sizes').toBeTruthy();
      expect(icon.type, 'icon missing type').toBeTruthy();
    }
  });

  test('manifest.json has NEXUS shortcuts defined', async () => {
    const manifestPath = path.join(SHARED_DIR, 'manifest.json');
    const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf-8'));

    expect(manifest.shortcuts, 'no shortcuts defined').toBeTruthy();
    expect(Array.isArray(manifest.shortcuts)).toBe(true);
    expect(manifest.shortcuts.length).toBeGreaterThanOrEqual(10);

    for (const shortcut of manifest.shortcuts) {
      expect(shortcut.name).toBeTruthy();
      expect(shortcut.url).toBeTruthy();
    }
  });

  test('main portal references manifest.json', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const manifestLink = await page.locator('link[rel="manifest"]').getAttribute('href');
    expect(manifestLink).toBeTruthy();
    expect(manifestLink).toContain('manifest.json');
  });
});

/* ============================================================
   PWA — Service Worker
   ============================================================ */
test.describe('PWA — Service Worker', () => {

  test('sw.js file exists on disk', async () => {
    const swPath = path.join(SHARED_DIR, 'sw.js');
    expect(fs.existsSync(swPath)).toBe(true);
  });

  test('sw.js defines cache names and static assets', async () => {
    const swPath = path.join(SHARED_DIR, 'sw.js');
    const content = fs.readFileSync(swPath, 'utf-8');

    // Should define cache names
    expect(content).toContain('CACHE_NAME');
    expect(content).toContain('STATIC_ASSETS');

    // Should handle install, activate, and fetch events
    expect(content).toContain('install');
    expect(content).toContain('activate');
    expect(content).toContain('fetch');
  });

  test('sw.js caches NEXUS routes for offline', async () => {
    const swPath = path.join(SHARED_DIR, 'sw.js');
    const content = fs.readFileSync(swPath, 'utf-8');

    expect(content).toContain('NEXUS_ROUTES');
    expect(content).toContain('nexus-orbital');
    expect(content).toContain('nexus-escudo');
    expect(content).toContain('nexus-cerebro');
  });

  test('service worker registers on main portal (http context)', async ({ page, context }) => {
    // Note: Service workers only work over HTTPS or localhost
    // When running against localhost:3000, this test validates registration
    test.skip(!page.url().startsWith('http'), 'Service workers require HTTP/HTTPS context');

    await page.goto('http://localhost:3000');
    await page.waitForTimeout(2000);

    const swRegistered = await page.evaluate(async () => {
      if (!('serviceWorker' in navigator)) return false;
      const registrations = await navigator.serviceWorker.getRegistrations();
      return registrations.length > 0;
    });

    expect(swRegistered).toBe(true);
  });
});

/* ============================================================
   PWA — Offline Capability
   ============================================================ */
test.describe('PWA — Offline Mode', () => {

  test('sw.js includes offline fallback strategy', async () => {
    const swPath = path.join(SHARED_DIR, 'sw.js');
    const content = fs.readFileSync(swPath, 'utf-8');

    // Should have cache-first or network-first strategy
    const hasCacheStrategy = content.includes('caches.match') || content.includes('cache.match');
    expect(hasCacheStrategy, 'No cache strategy found in sw.js').toBe(true);
  });

  test('sw.js defines priority offline platforms', async () => {
    const swPath = path.join(SHARED_DIR, 'sw.js');
    const content = fs.readFileSync(swPath, 'utf-8');

    expect(content).toContain('OFFLINE_PRIORITY_PLATFORMS');
  });

  test('offline mode works with cached content (http context)', async ({ page, context }) => {
    test.skip(!page.url().startsWith('http'), 'Offline test requires HTTP context');

    // Load the page online first to populate cache
    await page.goto('http://localhost:3000');
    await page.waitForTimeout(3000);

    // Go offline
    await context.setOffline(true);

    // Reload
    await page.reload();
    await page.waitForTimeout(1000);

    // Page should still render from cache
    const title = await page.title();
    expect(title).toBeTruthy();

    const bodyVisible = await page.locator('body').isVisible();
    expect(bodyVisible).toBe(true);

    // Go back online
    await context.setOffline(false);
  });
});

/* ============================================================
   PWA — Icons Validation
   ============================================================ */
test.describe('PWA — Icons', () => {

  test('icon files referenced in manifest exist on disk', async () => {
    const manifestPath = path.join(SHARED_DIR, 'manifest.json');
    const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf-8'));

    for (const icon of manifest.icons) {
      // Convert absolute paths to relative paths from PLATFORMS_DIR
      const iconSrc = icon.src.startsWith('/') ? icon.src.slice(1) : icon.src;
      const iconPath = path.join(PLATFORMS_DIR, iconSrc);
      expect(fs.existsSync(iconPath), `Icon not found: ${icon.src} -> ${iconPath}`).toBe(true);
    }
  });

  test('apple-touch-icon is defined in main portal', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const appleTouchIcon = await page.locator('link[rel="apple-touch-icon"]').count();
    expect(appleTouchIcon).toBeGreaterThan(0);
  });

  test('theme-color meta tag is defined', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const themeColor = await page.locator('meta[name="theme-color"]').getAttribute('content');
    expect(themeColor).toBeTruthy();
    // Should be the Ierahkwa green
    expect(themeColor.toLowerCase()).toBe('#00ff41');
  });
});

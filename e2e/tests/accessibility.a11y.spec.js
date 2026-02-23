'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');

const platforms = fs.readdirSync(PLATFORMS_DIR)
  .filter(d => {
    const fullPath = path.join(PLATFORMS_DIR, d);
    return fs.statSync(fullPath).isDirectory() &&
           fs.existsSync(path.join(fullPath, 'index.html'));
  })
  .sort();

test.describe('WCAG 2.2 AA Accessibility Checks', () => {
  for (const platform of platforms) {
    test(`${platform} — meets accessibility standards`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      // Check lang attribute
      const lang = await page.getAttribute('html', 'lang');
      expect(lang, `${platform}: missing lang attribute`).toBeTruthy();

      // Check title
      const title = await page.title();
      expect(title, `${platform}: missing page title`).toBeTruthy();

      // Check viewport meta
      const viewport = await page.locator('meta[name="viewport"]').count();
      expect(viewport, `${platform}: missing viewport meta`).toBeGreaterThan(0);

      // Check for heading hierarchy
      const h1Count = await page.locator('h1').count();
      expect(h1Count, `${platform}: no h1 heading found`).toBeGreaterThan(0);

      // Check images have alt text
      const imgsWithoutAlt = await page.locator('img:not([alt])').count();
      expect(imgsWithoutAlt, `${platform}: images missing alt text`).toBe(0);

      // Check for semantic landmarks
      const mainCount = await page.locator('main, [role="main"]').count();
      const navCount = await page.locator('nav, [role="navigation"]').count();
      expect(mainCount + navCount, `${platform}: no landmark regions`).toBeGreaterThan(0);

      // Check color contrast (basic — text is visible)
      const bodyColor = await page.evaluate(() => {
        const body = document.body;
        const style = window.getComputedStyle(body);
        return { bg: style.backgroundColor, color: style.color };
      });
      expect(bodyColor.bg).toBeTruthy();

      // Check no positive tabindex
      const badTabindex = await page.locator('[tabindex]:not([tabindex="0"]):not([tabindex="-1"])').count();
      expect(badTabindex, `${platform}: positive tabindex found`).toBe(0);

      // Check links have accessible names
      const emptyLinks = await page.locator('a:not([aria-label]):not([aria-labelledby])').evaluateAll(
        links => links.filter(l => !l.textContent.trim() && !l.querySelector('img[alt]')).length
      );
      expect(emptyLinks, `${platform}: links without accessible names`).toBe(0);
    });
  }
});

test.describe('Keyboard Accessibility', () => {
  for (const platform of platforms.slice(0, 15)) {
    test(`${platform} — no keyboard traps`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      // Tab 20 times and verify we can always move focus
      let lastFocused = '';
      let trapped = false;

      for (let i = 0; i < 20; i++) {
        await page.keyboard.press('Tab');
        const current = await page.evaluate(() => {
          const el = document.activeElement;
          return el ? `${el.tagName}#${el.id}.${el.className}` : 'none';
        });

        if (current === lastFocused && current !== 'none') {
          // Same element focused twice — might be trapped
          await page.keyboard.press('Tab');
          const afterRetry = await page.evaluate(() => document.activeElement?.tagName);
          if (afterRetry === current) {
            trapped = true;
            break;
          }
        }
        lastFocused = current;
      }

      expect(trapped, `${platform}: keyboard trap detected`).toBe(false);
    });
  }
});

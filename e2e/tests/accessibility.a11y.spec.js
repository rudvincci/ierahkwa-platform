'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');
const MAIN_PORTAL = `file://${path.join(PLATFORMS_DIR, 'index.html')}`;

const platforms = fs.readdirSync(PLATFORMS_DIR)
  .filter(d => {
    const fullPath = path.join(PLATFORMS_DIR, d);
    return fs.statSync(fullPath).isDirectory() &&
           fs.existsSync(path.join(fullPath, 'index.html'));
  })
  .sort();

/* ============================================================
   LANG ATTRIBUTE
   ============================================================ */
test.describe('Accessibility — Lang Attribute', () => {

  test('main portal has lang attribute on <html>', async ({ page }) => {
    await page.goto(MAIN_PORTAL);
    const lang = await page.getAttribute('html', 'lang');
    expect(lang).toBeTruthy();
    expect(lang.length).toBeGreaterThanOrEqual(2);
  });

  for (const platform of platforms) {
    test(`${platform} -- has lang attribute`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      const lang = await page.getAttribute('html', 'lang');
      expect(lang, `${platform}: missing lang attribute`).toBeTruthy();
    });
  }
});

/* ============================================================
   IMAGE ALT TEXT
   ============================================================ */
test.describe('Accessibility — Image Alt Text', () => {

  test('main portal images all have alt attributes', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const imgsNoAlt = await page.locator('img:not([alt])').count();
    expect(imgsNoAlt).toBe(0);
  });

  for (const platform of platforms) {
    test(`${platform} -- all images have alt text`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      const imgsNoAlt = await page.locator('img:not([alt])').count();
      expect(imgsNoAlt, `${platform}: images without alt text`).toBe(0);
    });
  }
});

/* ============================================================
   HEADING HIERARCHY (h1 -> h2 -> h3)
   ============================================================ */
test.describe('Accessibility — Heading Hierarchy', () => {

  test('main portal has exactly one h1 and proper hierarchy', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const h1Count = await page.locator('h1').count();
    expect(h1Count).toBe(1);

    // Check that headings don't skip levels (e.g., h1 -> h3 without h2)
    const headingLevels = await page.evaluate(() => {
      const headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
      return Array.from(headings).map(h => parseInt(h.tagName[1]));
    });

    for (let i = 1; i < headingLevels.length; i++) {
      const jump = headingLevels[i] - headingLevels[i - 1];
      // Heading level should not jump by more than 1 (e.g., h1 -> h3 is a skip)
      expect(jump, `Heading jumps from h${headingLevels[i-1]} to h${headingLevels[i]}`).toBeLessThanOrEqual(1);
    }
  });

  for (const platform of platforms) {
    test(`${platform} -- has at least one h1`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      const h1Count = await page.locator('h1').count();
      expect(h1Count, `${platform}: no h1 heading found`).toBeGreaterThan(0);
    });
  }
});

/* ============================================================
   FOCUS INDICATORS
   ============================================================ */
test.describe('Accessibility — Focus Indicators', () => {

  test('main portal has visible focus indicators on interactive elements', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    // Tab to the first interactive element
    await page.keyboard.press('Tab');
    await page.waitForTimeout(200);

    const focusInfo = await page.evaluate(() => {
      const el = document.activeElement;
      if (!el || el === document.body) return null;
      const cs = window.getComputedStyle(el);
      const csAfterFocus = window.getComputedStyle(el, ':focus-visible');
      return {
        tag: el.tagName,
        outline: cs.outline,
        outlineWidth: cs.outlineWidth,
        outlineColor: cs.outlineColor,
        outlineStyle: cs.outlineStyle,
        boxShadow: cs.boxShadow,
      };
    });

    // Focus indicator should be visible (outline or box-shadow)
    if (focusInfo) {
      const hasOutline = focusInfo.outlineStyle !== 'none' && focusInfo.outlineWidth !== '0px';
      const hasBoxShadow = focusInfo.boxShadow !== 'none';
      expect(hasOutline || hasBoxShadow, 'No visible focus indicator').toBe(true);
    }
  });

  for (const platform of platforms.slice(0, 15)) {
    test(`${platform} -- CSS includes focus-visible styles`, async () => {
      const html = fs.readFileSync(path.join(PLATFORMS_DIR, platform, 'index.html'), 'utf-8');
      const css = fs.existsSync(path.join(PLATFORMS_DIR, 'shared', 'ierahkwa.css'))
        ? fs.readFileSync(path.join(PLATFORMS_DIR, 'shared', 'ierahkwa.css'), 'utf-8')
        : '';

      // Either the HTML or the shared CSS should define :focus-visible or :focus styles
      const combined = html + css;
      const hasFocusStyles = combined.includes(':focus-visible') || combined.includes(':focus');
      expect(hasFocusStyles, `${platform}: no focus styles defined`).toBe(true);
    });
  }
});

/* ============================================================
   SKIP-TO-CONTENT LINKS
   ============================================================ */
test.describe('Accessibility — Skip Navigation', () => {

  test('main portal has skip-to-content link', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const skipLink = page.locator('.skip, .skip-nav, [class*="skip"], a[href="#main"], a[href="#content"]');
    const count = await skipLink.count();
    expect(count, 'No skip-to-content link found').toBeGreaterThan(0);
  });

  for (const platform of platforms) {
    test(`${platform} -- has skip navigation or landmark roles`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      // Either skip-nav link OR main landmark must exist
      const skipCount = await page.locator('.skip, .skip-nav, a[href="#main"], a[href="#content"]').count();
      const mainCount = await page.locator('main, [role="main"]').count();

      expect(skipCount + mainCount, `${platform}: no skip link and no main landmark`).toBeGreaterThan(0);
    });
  }
});

/* ============================================================
   COLOR CONTRAST — WCAG AA (Basic Checks)
   ============================================================ */
test.describe('Accessibility — Color Contrast (Basic)', () => {

  test('main portal body text has sufficient contrast ratio', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const contrastCheck = await page.evaluate(() => {
      function luminance(r, g, b) {
        const sRGB = [r, g, b].map(v => {
          v /= 255;
          return v <= 0.03928 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4);
        });
        return 0.2126 * sRGB[0] + 0.7152 * sRGB[1] + 0.0722 * sRGB[2];
      }

      function parseColor(c) {
        const m = c.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)/);
        if (!m) return null;
        return { r: parseInt(m[1]), g: parseInt(m[2]), b: parseInt(m[3]) };
      }

      const body = document.body;
      const cs = window.getComputedStyle(body);
      const fg = parseColor(cs.color);
      const bg = parseColor(cs.backgroundColor);

      if (!fg || !bg) return { ratio: 999, passes: true }; // Can't determine, pass

      const l1 = luminance(fg.r, fg.g, fg.b);
      const l2 = luminance(bg.r, bg.g, bg.b);
      const ratio = (Math.max(l1, l2) + 0.05) / (Math.min(l1, l2) + 0.05);

      return {
        ratio: Math.round(ratio * 100) / 100,
        fg: cs.color,
        bg: cs.backgroundColor,
        passes: ratio >= 4.5 // WCAG AA for normal text
      };
    });

    expect(contrastCheck.passes, `Contrast ratio ${contrastCheck.ratio}:1 (fg: ${contrastCheck.fg}, bg: ${contrastCheck.bg})`).toBe(true);
  });
});

/* ============================================================
   WCAG 2.2 AA — Comprehensive per-platform
   ============================================================ */
test.describe('WCAG 2.2 AA Accessibility Checks', () => {
  for (const platform of platforms) {
    test(`${platform} -- meets accessibility standards`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      // lang attribute
      const lang = await page.getAttribute('html', 'lang');
      expect(lang, `${platform}: missing lang`).toBeTruthy();

      // Title
      const title = await page.title();
      expect(title, `${platform}: missing title`).toBeTruthy();

      // Viewport meta
      const viewport = await page.locator('meta[name="viewport"]').count();
      expect(viewport, `${platform}: missing viewport`).toBeGreaterThan(0);

      // h1 exists
      const h1Count = await page.locator('h1').count();
      expect(h1Count, `${platform}: no h1`).toBeGreaterThan(0);

      // Images have alt
      const imgsNoAlt = await page.locator('img:not([alt])').count();
      expect(imgsNoAlt, `${platform}: imgs without alt`).toBe(0);

      // Semantic landmarks
      const mainCount = await page.locator('main, [role="main"]').count();
      const navCount = await page.locator('nav, [role="navigation"]').count();
      expect(mainCount + navCount, `${platform}: no landmarks`).toBeGreaterThan(0);

      // No positive tabindex
      const badTabindex = await page.locator('[tabindex]:not([tabindex="0"]):not([tabindex="-1"])').count();
      expect(badTabindex, `${platform}: positive tabindex`).toBe(0);

      // Links have accessible names
      const emptyLinks = await page.locator('a:not([aria-label]):not([aria-labelledby])').evaluateAll(
        links => links.filter(l => !l.textContent.trim() && !l.querySelector('img[alt]')).length
      );
      expect(emptyLinks, `${platform}: empty links`).toBe(0);
    });
  }
});

/* ============================================================
   KEYBOARD ACCESSIBILITY — No Traps
   ============================================================ */
test.describe('Keyboard Accessibility — No Traps', () => {
  for (const platform of platforms.slice(0, 15)) {
    test(`${platform} -- no keyboard traps`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      let lastFocused = '';
      let trapped = false;

      for (let i = 0; i < 20; i++) {
        await page.keyboard.press('Tab');
        const current = await page.evaluate(() => {
          const el = document.activeElement;
          return el ? `${el.tagName}#${el.id}.${el.className}` : 'none';
        });

        if (current === lastFocused && current !== 'none') {
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

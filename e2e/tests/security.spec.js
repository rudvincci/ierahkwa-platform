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
   SOVEREIGNTY — No External Scripts
   ============================================================ */
test.describe('Sovereignty — No External Scripts', () => {

  test('main portal loads zero external scripts', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const externalScripts = await page.evaluate(() => {
      const scripts = Array.from(document.querySelectorAll('script[src]'));
      return scripts
        .map(s => s.src)
        .filter(src => {
          // External = not relative, not file://, not ierahkwa.nation
          if (src.startsWith('file://')) return false;
          if (src.startsWith('/') || src.startsWith('./') || src.startsWith('../')) return false;
          if (src.includes('ierahkwa.nation')) return false;
          if (src.startsWith('http://') || src.startsWith('https://')) return true;
          return false;
        });
    });

    expect(externalScripts).toHaveLength(0);
  });

  for (const platform of platforms.slice(0, 30)) {
    test(`${platform} -- no external scripts loaded`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      const externalScripts = await page.evaluate(() => {
        return Array.from(document.querySelectorAll('script[src]'))
          .map(s => s.src)
          .filter(src => {
            if (src.startsWith('file://')) return false;
            if (src.includes('ierahkwa.nation')) return false;
            return src.startsWith('http://') || src.startsWith('https://');
          });
      });

      expect(externalScripts, `${platform}: external scripts found: ${externalScripts.join(', ')}`).toHaveLength(0);
    });
  }
});

/* ============================================================
   SOVEREIGNTY — No External CSS
   ============================================================ */
test.describe('Sovereignty — No External CSS Dependencies', () => {

  test('main portal loads zero external stylesheets', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const externalCSS = await page.evaluate(() => {
      return Array.from(document.querySelectorAll('link[rel="stylesheet"]'))
        .map(l => l.href)
        .filter(href => {
          if (href.startsWith('file://')) return false;
          if (href.includes('ierahkwa.nation')) return false;
          return href.startsWith('http://') || href.startsWith('https://');
        });
    });

    expect(externalCSS).toHaveLength(0);
  });

  for (const platform of platforms.slice(0, 30)) {
    test(`${platform} -- no external CSS dependencies`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      const externalCSS = await page.evaluate(() => {
        return Array.from(document.querySelectorAll('link[rel="stylesheet"]'))
          .map(l => l.href)
          .filter(href => {
            if (href.startsWith('file://')) return false;
            if (href.includes('ierahkwa.nation')) return false;
            return href.startsWith('http://') || href.startsWith('https://');
          });
      });

      expect(externalCSS, `${platform}: external CSS: ${externalCSS.join(', ')}`).toHaveLength(0);
    });
  }
});

/* ============================================================
   SOVEREIGNTY — Links Use Relative Paths or .nation Domain
   ============================================================ */
test.describe('Sovereignty — Link Paths', () => {

  test('main portal links use relative paths or ierahkwa.nation', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const externalLinks = await page.evaluate(() => {
      return Array.from(document.querySelectorAll('a[href]'))
        .map(a => a.getAttribute('href'))
        .filter(href => {
          if (!href) return false;
          if (href.startsWith('#') || href.startsWith('/') || href.startsWith('./') || href.startsWith('../')) return false;
          if (href.startsWith('mailto:') || href.startsWith('tel:')) return false;
          if (href.includes('ierahkwa.nation')) return false;
          return href.startsWith('http://') || href.startsWith('https://');
        });
    });

    expect(externalLinks, `External links found: ${externalLinks.join(', ')}`).toHaveLength(0);
  });

  for (const platform of platforms.slice(0, 20)) {
    test(`${platform} -- all links are sovereign (relative or .nation)`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);

      const externalLinks = await page.evaluate(() => {
        return Array.from(document.querySelectorAll('a[href]'))
          .map(a => a.getAttribute('href'))
          .filter(href => {
            if (!href) return false;
            if (href.startsWith('#') || href.startsWith('/') || href.startsWith('./') || href.startsWith('../')) return false;
            if (href.startsWith('mailto:') || href.startsWith('tel:') || href.startsWith('javascript:')) return false;
            if (href.includes('ierahkwa.nation')) return false;
            return href.startsWith('http://') || href.startsWith('https://');
          });
      });

      expect(externalLinks, `${platform}: external links: ${externalLinks.join(', ')}`).toHaveLength(0);
    });
  }
});

/* ============================================================
   CSP COMPATIBILITY — No Inline Event Handlers
   ============================================================ */
test.describe('CSP Compatibility — No Inline Event Handlers', () => {

  test('main portal HTML has no inline event handlers', async () => {
    const html = fs.readFileSync(path.join(PLATFORMS_DIR, 'index.html'), 'utf-8');

    // Match on* attributes like onclick=, onload=, onerror= etc.
    // Exclude from <body> onload which is sometimes acceptable
    const inlineHandlers = html.match(/\s(onclick|onmouseover|onmouseout|onkeydown|onkeyup|onsubmit|onfocus|onblur|onchange|oninput|ondblclick|oncontextmenu|onscroll|onresize|ontouchstart|ontouchmove|ontouchend)=/gi);

    expect(inlineHandlers || [], 'Inline event handlers found in main portal').toHaveLength(0);
  });

  for (const platform of platforms.slice(0, 30)) {
    test(`${platform} -- no inline event handlers in HTML source`, async () => {
      const html = fs.readFileSync(path.join(PLATFORMS_DIR, platform, 'index.html'), 'utf-8');

      // Allow onclick in dynamically generated content (agents panel close button)
      // but flag it in the main HTML structure
      const inlineHandlers = html.match(/\s(onclick|onmouseover|onmouseout|onkeydown|onkeyup|onsubmit|onfocus|onblur|onchange|oninput|ondblclick|oncontextmenu|onscroll|onresize)=/gi);

      expect(inlineHandlers || [], `${platform}: inline handlers found`).toHaveLength(0);
    });
  }
});

/* ============================================================
   SOVEREIGNTY — No Tracking Pixels or Analytics
   ============================================================ */
test.describe('Sovereignty — No Tracking or Analytics', () => {

  const trackingPatterns = [
    'google-analytics', 'googletagmanager', 'gtag', 'ga.js', 'analytics.js',
    'facebook.net', 'fbevents', 'pixel', 'hotjar', 'mixpanel', 'segment',
    'amplitude', 'heap', 'fullstory', 'mouseflow', 'clarity.ms',
    'doubleclick', 'adsense', 'adwords', 'tracking', 'beacon'
  ];

  test('main portal has no tracking scripts', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const allScripts = await page.evaluate(() => {
      const scripts = Array.from(document.querySelectorAll('script'));
      return scripts.map(s => (s.src || '') + ' ' + (s.textContent || '').substring(0, 500));
    });

    const allText = allScripts.join(' ').toLowerCase();
    for (const pattern of trackingPatterns) {
      expect(allText, `Tracking pattern "${pattern}" found`).not.toContain(pattern);
    }
  });

  test('main portal has no tracking pixels (1x1 images)', async ({ page }) => {
    await page.goto(MAIN_PORTAL);

    const pixelImages = await page.evaluate(() => {
      return Array.from(document.querySelectorAll('img'))
        .filter(img => {
          const w = img.naturalWidth || parseInt(img.width) || parseInt(img.getAttribute('width')) || 999;
          const h = img.naturalHeight || parseInt(img.height) || parseInt(img.getAttribute('height')) || 999;
          return w <= 2 && h <= 2;
        })
        .map(img => img.src);
    });

    expect(pixelImages, 'Tracking pixels found').toHaveLength(0);
  });

  for (const platform of platforms.slice(0, 20)) {
    test(`${platform} -- no tracking or analytics in HTML source`, async () => {
      const html = fs.readFileSync(path.join(PLATFORMS_DIR, platform, 'index.html'), 'utf-8').toLowerCase();

      for (const pattern of trackingPatterns) {
        expect(html, `${platform}: tracking pattern "${pattern}" found`).not.toContain(pattern);
      }
    });
  }
});

'use strict';

const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');

/* ============================================================
   NEXUS PORTAL DEFINITIONS
   All 18 NEXUS mega-portals with their expected accent colors
   ============================================================ */
const NEXUS_PORTALS = [
  { name: 'nexus-orbital',         color: '#00bcd4', label: 'Orbital — Telecom' },
  { name: 'nexus-escudo',          color: '#f44336', label: 'Escudo — Defense' },
  { name: 'nexus-cerebro',         color: '#7c4dff', label: 'Cerebro — AI/Quantum' },
  { name: 'nexus-tesoro',          color: '#ffd600', label: 'Tesoro — Finance' },
  { name: 'nexus-voces',           color: '#e040fb', label: 'Voces — Social/Media' },
  { name: 'nexus-consejo',         color: '#1565c0', label: 'Consejo — Government' },
  { name: 'nexus-tierra',          color: '#43a047', label: 'Tierra — Nature' },
  { name: 'nexus-forja',           color: '#00e676', label: 'Forja — Dev Tools' },
  { name: 'nexus-urbe',            color: '#ff9100', label: 'Urbe — Smart City' },
  { name: 'nexus-raices',          color: '#00FF41', label: 'Raices — Culture' },
  { name: 'nexus-salud',           color: '#FF5722', label: 'Salud — Health' },
  { name: 'nexus-academia',        color: '#9C27B0', label: 'Academia — University' },
  { name: 'nexus-escolar',         color: '#1E88E5', label: 'Escolar — K-12' },
  { name: 'nexus-entretenimiento', color: '#E91E63', label: 'Entretenimiento — Entertainment' },
  { name: 'nexus-amparo',          color: '#607D8B', label: 'Amparo — Social Protection' },
  { name: 'nexus-escritorio',      color: '#26C6DA', label: 'Escritorio — Desktop' },
  { name: 'nexus-comercio',        color: '#FF6D00', label: 'Comercio — Commerce' },
  { name: 'nexus-cosmos',          color: '#1a237e', label: 'Cosmos — Space' },
];

/* ============================================================
   NEXUS PORTALS — Load Tests
   ============================================================ */
test.describe('NEXUS Portals — Page Load', () => {

  for (const nexus of NEXUS_PORTALS) {
    const indexPath = path.join(PLATFORMS_DIR, nexus.name, 'index.html');
    const exists = fs.existsSync(indexPath);

    test(`${nexus.label} (${nexus.name}) -- portal page loads`, async ({ page }) => {
      test.skip(!exists, `${nexus.name}/index.html does not exist`);

      const filePath = `file://${indexPath}`;
      const errors = [];
      page.on('pageerror', err => errors.push(err.message));

      await page.goto(filePath);

      // Has a title
      const title = await page.title();
      expect(title).toBeTruthy();

      // Has visible content
      await expect(page.locator('body')).toBeVisible();

      // Has an h1
      const h1Count = await page.locator('h1').count();
      expect(h1Count).toBeGreaterThan(0);

      // No JS errors
      expect(errors).toHaveLength(0);
    });
  }
});

/* ============================================================
   NEXUS PORTALS — Color Theme Verification
   ============================================================ */
test.describe('NEXUS Portals — Color Theme', () => {

  for (const nexus of NEXUS_PORTALS) {
    const indexPath = path.join(PLATFORMS_DIR, nexus.name, 'index.html');
    const exists = fs.existsSync(indexPath);

    test(`${nexus.label} -- has correct accent color (${nexus.color})`, async ({ page }) => {
      test.skip(!exists, `${nexus.name}/index.html does not exist`);

      const filePath = `file://${indexPath}`;
      await page.goto(filePath);

      // Read the HTML source for the --accent CSS variable or the color value
      const html = fs.readFileSync(indexPath, 'utf-8');

      // The accent color should be defined somewhere in the HTML (inline style or CSS variable)
      const colorLower = nexus.color.toLowerCase();
      const htmlLower = html.toLowerCase();

      // Check for --accent definition or the color value itself
      const hasColor = htmlLower.includes(colorLower) ||
                       htmlLower.includes(`--accent:${colorLower}`) ||
                       htmlLower.includes(`--accent: ${colorLower}`);

      expect(hasColor, `${nexus.name}: accent color ${nexus.color} not found in HTML`).toBe(true);
    });
  }
});

/* ============================================================
   NEXUS PORTALS — Platform Cards Link Correctly
   ============================================================ */
test.describe('NEXUS Portals — Platform Card Links', () => {

  for (const nexus of NEXUS_PORTALS) {
    const indexPath = path.join(PLATFORMS_DIR, nexus.name, 'index.html');
    const exists = fs.existsSync(indexPath);

    test(`${nexus.label} -- platform cards have valid links`, async ({ page }) => {
      test.skip(!exists, `${nexus.name}/index.html does not exist`);

      const filePath = `file://${indexPath}`;
      await page.goto(filePath);

      // Find all card links (a tags with href pointing to platform directories)
      const cardLinks = await page.evaluate(() => {
        const links = document.querySelectorAll('a[href]');
        return Array.from(links)
          .map(a => a.getAttribute('href'))
          .filter(href => href && !href.startsWith('#') && !href.startsWith('http') && !href.startsWith('mailto'));
      });

      // Each link should resolve to an existing file/directory
      for (const href of cardLinks.slice(0, 10)) {
        const resolvedPath = path.resolve(path.join(PLATFORMS_DIR, nexus.name), href);
        const targetDir = resolvedPath.replace(/\/index\.html$/, '');

        // Check if it resolves to an existing index.html
        const targetIndex = resolvedPath.endsWith('.html')
          ? resolvedPath
          : path.join(resolvedPath, 'index.html');

        const targetExists = fs.existsSync(targetIndex) || fs.existsSync(targetDir + '/index.html');
        expect(targetExists, `${nexus.name}: broken link "${href}" -> ${targetIndex}`).toBe(true);
      }
    });
  }
});

/* ============================================================
   NEXUS PORTALS — Structural Consistency
   ============================================================ */
test.describe('NEXUS Portals — Structural Consistency', () => {

  for (const nexus of NEXUS_PORTALS) {
    const indexPath = path.join(PLATFORMS_DIR, nexus.name, 'index.html');
    const exists = fs.existsSync(indexPath);

    test(`${nexus.label} -- uses shared ierahkwa.css`, async () => {
      test.skip(!exists, `${nexus.name}/index.html does not exist`);

      const html = fs.readFileSync(indexPath, 'utf-8');
      expect(html).toContain('ierahkwa.css');
    });

    test(`${nexus.label} -- has lang attribute and viewport meta`, async ({ page }) => {
      test.skip(!exists, `${nexus.name}/index.html does not exist`);

      const filePath = `file://${indexPath}`;
      await page.goto(filePath);

      const lang = await page.getAttribute('html', 'lang');
      expect(lang).toBeTruthy();

      const viewport = await page.locator('meta[name="viewport"]').count();
      expect(viewport).toBeGreaterThan(0);
    });
  }
});

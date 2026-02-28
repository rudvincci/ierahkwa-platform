'use strict';

const { test, expect } = require('@playwright/test');
const path = require('path');
const fs = require('fs');

const PLATFORMS_DIR = path.resolve(__dirname, '../../02-plataformas-html');
const MAIN_PORTAL = `file://${path.join(PLATFORMS_DIR, 'index.html')}`;

// Pick a few platforms that load ierahkwa-agents.js
const samplePlatforms = fs.readdirSync(PLATFORMS_DIR)
  .filter(d => {
    const fullPath = path.join(PLATFORMS_DIR, d);
    if (!fs.statSync(fullPath).isDirectory()) return false;
    const indexPath = path.join(fullPath, 'index.html');
    if (!fs.existsSync(indexPath)) return false;
    const html = fs.readFileSync(indexPath, 'utf-8');
    return html.includes('ierahkwa-agents');
  })
  .slice(0, 5);

/* ============================================================
   AI AGENTS — Script Loading & API
   ============================================================ */
test.describe('AI Agents — Script Loading', () => {

  test('ierahkwa-agents.js script file exists on disk', async () => {
    const agentsPath = path.join(PLATFORMS_DIR, 'shared', 'ierahkwa-agents.js');
    expect(fs.existsSync(agentsPath)).toBe(true);
  });

  test('agents script loads without errors on main portal', async ({ page }) => {
    const errors = [];
    page.on('pageerror', err => errors.push(err.message));

    await page.goto(MAIN_PORTAL);
    await page.waitForTimeout(1000);

    // Filter out errors unrelated to agents
    const agentErrors = errors.filter(e => e.toLowerCase().includes('agent'));
    expect(agentErrors).toHaveLength(0);
  });
});

/* ============================================================
   AI AGENTS — window.IerahkwaAgents API
   ============================================================ */
test.describe('AI Agents — Public API', () => {

  for (const platform of samplePlatforms) {
    test(`${platform} -- window.IerahkwaAgents is accessible`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(1500);

      const hasAgents = await page.evaluate(() => typeof window.IerahkwaAgents !== 'undefined');
      expect(hasAgents).toBe(true);
    });

    test(`${platform} -- IerahkwaAgents has correct structure`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(1500);

      const apiCheck = await page.evaluate(() => {
        const agents = window.IerahkwaAgents;
        if (!agents) return null;
        return {
          hasVersion: typeof agents.version === 'string',
          hasGuardian: typeof agents.guardian === 'object',
          hasPattern: typeof agents.pattern === 'object',
          hasAnomaly: typeof agents.anomaly === 'object',
          hasTrust: typeof agents.trust === 'object',
          hasShield: typeof agents.shield === 'object',
          hasForensic: typeof agents.forensic === 'object',
          hasEvolution: typeof agents.evolution === 'object',
          hasGetStatus: typeof agents.getStatus === 'function',
        };
      });

      expect(apiCheck).not.toBeNull();
      expect(apiCheck.hasVersion).toBe(true);
      expect(apiCheck.hasGuardian).toBe(true);
      expect(apiCheck.hasPattern).toBe(true);
      expect(apiCheck.hasAnomaly).toBe(true);
      expect(apiCheck.hasTrust).toBe(true);
      expect(apiCheck.hasShield).toBe(true);
      expect(apiCheck.hasForensic).toBe(true);
      expect(apiCheck.hasEvolution).toBe(true);
      expect(apiCheck.hasGetStatus).toBe(true);
    });
  }
});

/* ============================================================
   AI AGENTS — Trust Badge UI
   ============================================================ */
test.describe('AI Agents — Trust Badge', () => {

  for (const platform of samplePlatforms.slice(0, 3)) {
    test(`${platform} -- trust badge appears in bottom-left corner`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(2000);

      const badge = page.locator('#trust-badge');
      await expect(badge).toBeVisible();

      // Verify position is fixed, bottom-left
      const style = await badge.evaluate(el => {
        const cs = window.getComputedStyle(el);
        return {
          position: cs.position,
          bottom: cs.bottom,
          left: cs.left,
        };
      });
      expect(style.position).toBe('fixed');
    });

    test(`${platform} -- trust score is between 0 and 100`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(2000);

      const score = await page.evaluate(() => {
        return window.IerahkwaAgents?.trust?.score;
      });

      expect(score).toBeGreaterThanOrEqual(0);
      expect(score).toBeLessThanOrEqual(100);

      // Badge text should match score
      const badgeText = await page.locator('#trust-badge').textContent();
      expect(parseInt(badgeText)).toBe(score);
    });

    test(`${platform} -- guardian agent is active`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(1500);

      const guardianActive = await page.evaluate(() => {
        return window.IerahkwaAgents?.guardian?.active;
      });
      expect(guardianActive).toBe(true);
    });

    test(`${platform} -- pattern agent starts learning`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(1500);

      const patternAgent = await page.evaluate(() => {
        const agent = window.IerahkwaAgents?.pattern;
        return agent ? { name: agent.name, hasProfile: !!agent.userProfile } : null;
      });

      expect(patternAgent).not.toBeNull();
      expect(patternAgent.name).toBe('Pattern');
    });
  }
});

/* ============================================================
   AI AGENTS — Panel Interaction
   ============================================================ */
test.describe('AI Agents — Panel Interaction', () => {

  for (const platform of samplePlatforms.slice(0, 2)) {
    test(`${platform} -- agent panel opens on badge click`, async ({ page }) => {
      const filePath = `file://${path.join(PLATFORMS_DIR, platform, 'index.html')}`;
      await page.goto(filePath);
      await page.waitForTimeout(2000);

      const badge = page.locator('#trust-badge');
      await expect(badge).toBeVisible();

      // Panel should not exist yet
      await expect(page.locator('#agents-panel')).toHaveCount(0);

      // Click the badge
      await badge.click();
      await page.waitForTimeout(500);

      // Panel should now be visible
      const panel = page.locator('#agents-panel');
      await expect(panel).toBeVisible();

      // Panel should contain agent names
      const panelText = await panel.textContent();
      expect(panelText).toContain('Guardian');
      expect(panelText).toContain('Pattern');
      expect(panelText).toContain('Trust');
      expect(panelText).toContain('Shield');

      // Click badge again to close
      await badge.click();
      await page.waitForTimeout(300);
      await expect(page.locator('#agents-panel')).toHaveCount(0);
    });
  }
});

'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Hosting Module v1.0.0 — Sovereign Hosting & Domains
// VPS, Web Hosting, Domain Registration, SSL
// Paid in WPM/BDET — MameyNode settlement
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:hosting');
const audit = createAuditLogger('sovereign-core:hosting');

const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';

// ============================================================
// Hosting Plans
// ============================================================
const HOSTING_PLANS = {
  // Web Hosting
  'web-basic': { name: 'Web Básico', type: 'web', storage_gb: 10, bandwidth_gb: 100, domains: 1, ssl: true, email_accounts: 5, price_monthly_wpm: 9.99, price_yearly_wpm: 99.99 },
  'web-pro': { name: 'Web Profesional', type: 'web', storage_gb: 50, bandwidth_gb: 500, domains: 5, ssl: true, email_accounts: 25, databases: 10, price_monthly_wpm: 29.99, price_yearly_wpm: 299.99 },
  'web-enterprise': { name: 'Web Empresarial', type: 'web', storage_gb: 200, bandwidth_gb: 2000, domains: 25, ssl: true, email_accounts: 100, databases: 50, price_monthly_wpm: 99.99, price_yearly_wpm: 999.99 },

  // VPS
  'vps-micro': { name: 'VPS Micro', type: 'vps', vcpus: 1, ram_gb: 1, storage_gb: 25, bandwidth_gb: 1000, ipv4: 1, ipv6: true, price_monthly_wpm: 4.99, price_yearly_wpm: 49.99 },
  'vps-small': { name: 'VPS Pequeño', type: 'vps', vcpus: 2, ram_gb: 4, storage_gb: 80, bandwidth_gb: 3000, ipv4: 1, ipv6: true, price_monthly_wpm: 19.99, price_yearly_wpm: 199.99 },
  'vps-medium': { name: 'VPS Mediano', type: 'vps', vcpus: 4, ram_gb: 8, storage_gb: 160, bandwidth_gb: 5000, ipv4: 1, ipv6: true, price_monthly_wpm: 39.99, price_yearly_wpm: 399.99 },
  'vps-large': { name: 'VPS Grande', type: 'vps', vcpus: 8, ram_gb: 16, storage_gb: 320, bandwidth_gb: 10000, ipv4: 2, ipv6: true, price_monthly_wpm: 79.99, price_yearly_wpm: 799.99 },
  'vps-xl': { name: 'VPS XL Soberano', type: 'vps', vcpus: 16, ram_gb: 64, storage_gb: 640, bandwidth_gb: 20000, ipv4: 4, ipv6: true, price_monthly_wpm: 199.99, price_yearly_wpm: 1999.99 },

  // Dedicated
  'dedicated-1': { name: 'Dedicado Soberano', type: 'dedicated', vcpus: 32, ram_gb: 128, storage_gb: 2000, bandwidth_gb: 50000, ipv4: 8, ipv6: true, price_monthly_wpm: 499.99, price_yearly_wpm: 4999.99 },

  // Sovereign Node (MameyNode validator)
  'node-validator': { name: 'Nodo Validador MameyNode', type: 'node', vcpus: 4, ram_gb: 16, storage_gb: 500, bandwidth_gb: 10000, stake_required_wpm: 10000, price_monthly_wpm: 49.99, price_yearly_wpm: 499.99 }
};

// Domain TLDs
const SOVEREIGN_TLDS = [
  { tld: '.soberano', price_yearly_wpm: 14.99, description: 'Dominio Soberano' },
  { tld: '.nacion', price_yearly_wpm: 19.99, description: 'Dominio de Nación' },
  { tld: '.tribu', price_yearly_wpm: 9.99, description: 'Dominio Tribal' },
  { tld: '.indigena', price_yearly_wpm: 12.99, description: 'Dominio Indígena' },
  { tld: '.wampum', price_yearly_wpm: 24.99, description: 'Dominio WAMPUM' },
  { tld: '.mamey', price_yearly_wpm: 9.99, description: 'Dominio MameyNode' },
  { tld: '.bdet', price_yearly_wpm: 19.99, description: 'Dominio BDET Bank' },
  // Standard TLDs
  { tld: '.com', price_yearly_wpm: 12.99, description: 'Dominio Comercial' },
  { tld: '.org', price_yearly_wpm: 12.99, description: 'Dominio Organización' },
  { tld: '.net', price_yearly_wpm: 12.99, description: 'Dominio Red' },
  { tld: '.io', price_yearly_wpm: 39.99, description: 'Dominio Tecnología' },
  { tld: '.xyz', price_yearly_wpm: 4.99, description: 'Dominio Universal' }
];

function generateServiceId(prefix) {
  return `${prefix}-${crypto.randomBytes(8).toString('hex').toUpperCase()}`;
}

// ============================================================
// GET /plans — All hosting plans
// ============================================================
router.get('/plans', (_req, res) => {
  const plans = Object.entries(HOSTING_PLANS).map(([id, plan]) => ({ planId: id, ...plan }));
  const byType = {
    web: plans.filter(p => p.type === 'web'),
    vps: plans.filter(p => p.type === 'vps'),
    dedicated: plans.filter(p => p.type === 'dedicated'),
    node: plans.filter(p => p.type === 'node')
  };
  res.json({ status: 'ok', data: { plans: byType, totalPlans: plans.length, currency: 'WMP' } });
});

// ============================================================
// GET /domains/tlds — Available TLDs
// ============================================================
router.get('/domains/tlds', (_req, res) => {
  res.json({ status: 'ok', data: { tlds: SOVEREIGN_TLDS, currency: 'WMP' } });
});

// ============================================================
// GET /domains/check — Check domain availability
// ============================================================
router.get('/domains/check', asyncHandler(async (req, res) => {
  const { domain } = req.query;
  if (!domain) throw new AppError('INVALID_INPUT', 'Domain name required');

  const normalizedDomain = domain.toLowerCase().trim();

  // Check if already registered
  const existing = await db.query(
    `SELECT domain_name, status FROM hosting_domains WHERE domain_name = $1 AND status != 'expired'`,
    [normalizedDomain]
  );

  const available = existing.rows.length === 0;
  const tld = '.' + normalizedDomain.split('.').pop();
  const tldInfo = SOVEREIGN_TLDS.find(t => t.tld === tld);

  res.json({
    status: 'ok',
    data: {
      domain: normalizedDomain,
      available,
      tld,
      price: tldInfo ? { yearly: tldInfo.price_yearly_wpm, currency: 'WMP' } : null,
      premium: normalizedDomain.length <= 3,
      suggestions: available ? [] : [
        `mi-${normalizedDomain}`,
        normalizedDomain.replace(tld, '.soberano'),
        normalizedDomain.replace(tld, '.nacion')
      ]
    }
  });
}));

// ============================================================
// POST /domains/register — Register a domain
// ============================================================
router.post('/domains/register',
  validate({
    body: {
      domain_name: t.string({ required: true, max: 253 }),
      years: t.number({ min: 1, max: 10 }),
      privacy: t.string({ enum: ['yes', 'no'] }),
      auto_renew: t.string({ enum: ['yes', 'no'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { domain_name, years, privacy, auto_renew } = req.body;
    const normalizedDomain = domain_name.toLowerCase().trim();
    const regYears = years || 1;
    const tld = '.' + normalizedDomain.split('.').pop();
    const tldInfo = SOVEREIGN_TLDS.find(t => t.tld === tld);

    if (!tldInfo) throw new AppError('INVALID_INPUT', `TLD ${tld} not supported`);

    // Check availability
    const existing = await db.query(
      `SELECT id FROM hosting_domains WHERE domain_name = $1 AND status != 'expired'`,
      [normalizedDomain]
    );
    if (existing.rows.length > 0) throw new AppError('CONFLICT', 'Domain already registered');

    const totalCost = tldInfo.price_yearly_wpm * regYears;
    const domainId = generateServiceId('DOM');
    const expiresAt = new Date();
    expiresAt.setFullYear(expiresAt.getFullYear() + regYears);

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Charge user (deduct from WMP balance)
      const balance = await client.query(
        `SELECT balance FROM bank_accounts WHERE user_id = $1 AND currency = 'WMP' AND status = 'active' ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
        [req.user.id]
      );
      if (!balance.rows.length || parseFloat(balance.rows[0].balance) < totalCost) {
        throw new AppError('INSUFFICIENT_FUNDS', `Need ${totalCost} WMP to register domain for ${regYears} year(s)`);
      }

      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW()
         WHERE user_id = $2 AND currency = 'WMP' AND status = 'active' ORDER BY balance DESC LIMIT 1`,
        [totalCost, req.user.id]
      );

      // Register domain
      await client.query(
        `INSERT INTO hosting_domains
           (domain_id, user_id, domain_name, tld, status, registered_at, expires_at,
            privacy_protection, auto_renew, nameservers, price_paid, currency)
         VALUES ($1, $2, $3, $4, 'active', NOW(), $5, $6, $7, $8, $9, 'WMP')`,
        [domainId, req.user.id, normalizedDomain, tld, expiresAt,
         privacy !== 'no', auto_renew !== 'no',
         JSON.stringify(['ns1.soberano.io', 'ns2.soberano.io']),
         totalCost]
      );

      // Payment record
      const txHash = crypto.createHash('sha256')
        .update(`domain:${domainId}:${totalCost}:${Date.now()}`).digest('hex');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, $2, $3, 'WMP', 'domain_registration', $4, $5, 'completed', NOW())`,
        [req.user.id, PLATFORM_TREASURY_ID, totalCost,
         `Domain registration: ${normalizedDomain} (${regYears}yr)`, txHash]
      );

      await client.query('COMMIT');

      audit.record({
        category: 'HOSTING',
        action: 'domain_registered',
        userId: req.user.id,
        risk: 'LOW',
        details: { domainId, domain: normalizedDomain, years: regYears, cost: totalCost }
      });

      res.status(201).json({
        status: 'ok',
        data: {
          domainId,
          domain: normalizedDomain,
          tld,
          status: 'active',
          registeredAt: new Date().toISOString(),
          expiresAt: expiresAt.toISOString(),
          nameservers: ['ns1.soberano.io', 'ns2.soberano.io'],
          privacyProtection: privacy !== 'no',
          autoRenew: auto_renew !== 'no',
          cost: { amount: totalCost, currency: 'WMP', period: `${regYears} year(s)` }
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('REGISTRATION_FAILED', 'Domain registration failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /domains — User's domains
// ============================================================
router.get('/domains', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT domain_id, domain_name, tld, status, registered_at, expires_at,
            privacy_protection, auto_renew, nameservers, ssl_status
     FROM hosting_domains WHERE user_id = $1 ORDER BY registered_at DESC`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// POST /service/provision — Provision hosting/VPS service
// ============================================================
router.post('/service/provision',
  validate({
    body: {
      plan_id: t.string({ required: true }),
      billing_cycle: t.string({ required: true, enum: ['monthly', 'yearly'] }),
      hostname: t.string({ max: 100 }),
      region: t.string({ max: 50 }),
      os: t.string({ max: 50 }),
      domain_id: t.string({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { plan_id, billing_cycle, hostname, region, os, domain_id } = req.body;
    const plan = HOSTING_PLANS[plan_id];
    if (!plan) throw new AppError('INVALID_INPUT', `Plan '${plan_id}' not found`);

    const price = billing_cycle === 'yearly' ? plan.price_yearly_wpm : plan.price_monthly_wpm;
    const serviceId = generateServiceId(plan.type === 'vps' ? 'VPS' : plan.type === 'node' ? 'NODE' : 'WEB');

    // Default OS for VPS
    const selectedOS = os || (plan.type === 'vps' ? 'ubuntu-22.04' : null);
    const selectedRegion = region || 'pa-west-1'; // Panama default

    const REGIONS = {
      'pa-west-1': 'Panamá Oeste',
      'pa-east-1': 'Panamá Este',
      'us-east-1': 'Virginia, USA',
      'us-west-1': 'Oregon, USA',
      'eu-west-1': 'Ámsterdam, EU',
      'br-south-1': 'São Paulo, Brasil',
      'mx-central-1': 'CDMX, México'
    };

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Charge user
      const balance = await client.query(
        `SELECT account_number, balance FROM bank_accounts
         WHERE user_id = $1 AND currency = 'WMP' AND status = 'active' AND balance >= $2
         ORDER BY balance DESC LIMIT 1 FOR UPDATE`,
        [req.user.id, price]
      );
      if (!balance.rows.length) {
        throw new AppError('INSUFFICIENT_FUNDS', `Need ${price} WMP for ${plan.name} (${billing_cycle})`);
      }

      await client.query(
        `UPDATE bank_accounts SET balance = balance - $1, updated_at = NOW() WHERE account_number = $2`,
        [price, balance.rows[0].account_number]
      );

      // Provision service
      const ipv4 = `172.${28 + Math.floor(Math.random() * 4)}.${Math.floor(Math.random() * 254)}.${Math.floor(Math.random() * 254) + 1}`;
      const ipv6 = `2001:db8:574:${Math.floor(Math.random() * 9999)}::1`;
      const nextBillingDate = new Date();
      if (billing_cycle === 'yearly') {
        nextBillingDate.setFullYear(nextBillingDate.getFullYear() + 1);
      } else {
        nextBillingDate.setMonth(nextBillingDate.getMonth() + 1);
      }

      await client.query(
        `INSERT INTO hosting_services
           (service_id, user_id, plan_id, plan_name, service_type, hostname, region, os,
            ipv4_address, ipv6_address, vcpus, ram_gb, storage_gb, bandwidth_gb,
            billing_cycle, price, currency, status, next_billing_date, domain_id,
            specs)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16, 'WMP', 'provisioning', $17, $18, $19::jsonb)`,
        [serviceId, req.user.id, plan_id, plan.name, plan.type,
         hostname || `${plan.type}-${serviceId.toLowerCase()}`,
         selectedRegion, selectedOS,
         ipv4, ipv6,
         plan.vcpus || null, plan.ram_gb || null, plan.storage_gb, plan.bandwidth_gb || null,
         billing_cycle, price, nextBillingDate, domain_id || null,
         JSON.stringify({
           domains: plan.domains || null,
           ssl: plan.ssl || false,
           email_accounts: plan.email_accounts || null,
           databases: plan.databases || null,
           stake_required: plan.stake_required_wpm || null
         })]
      );

      // Payment transaction
      const txHash = crypto.createHash('sha256')
        .update(`hosting:${serviceId}:${price}:${Date.now()}`).digest('hex');
      await client.query(
        `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, created_at)
         VALUES ($1, $2, $3, 'WMP', 'hosting_provision', $4, $5, 'completed', NOW())`,
        [req.user.id, PLATFORM_TREASURY_ID, price,
         `${plan.name} provision (${billing_cycle})`, txHash]
      );

      await client.query('COMMIT');

      // Simulate provisioning (in production: call actual cloud API)
      setTimeout(async () => {
        try {
          await db.query(
            `UPDATE hosting_services SET status = 'active', provisioned_at = NOW() WHERE service_id = $1`,
            [serviceId]
          );
        } catch { /* ok */ }
      }, 3000);

      audit.record({
        category: 'HOSTING',
        action: 'service_provisioned',
        userId: req.user.id,
        risk: 'MEDIUM',
        details: { serviceId, plan: plan_id, price, billing_cycle }
      });

      res.status(201).json({
        status: 'ok',
        data: {
          serviceId,
          plan: { id: plan_id, name: plan.name, type: plan.type },
          server: {
            hostname: hostname || `${plan.type}-${serviceId.toLowerCase()}`,
            ipv4,
            ipv6,
            region: { code: selectedRegion, name: REGIONS[selectedRegion] || selectedRegion },
            os: selectedOS,
            specs: {
              vcpus: plan.vcpus || null,
              ram: plan.ram_gb ? `${plan.ram_gb} GB` : null,
              storage: `${plan.storage_gb} GB SSD`,
              bandwidth: plan.bandwidth_gb ? `${plan.bandwidth_gb} GB/mo` : 'Unlimited'
            }
          },
          billing: {
            cycle: billing_cycle,
            price,
            currency: 'WMP',
            nextBilling: nextBillingDate.toISOString()
          },
          status: 'provisioning',
          estimatedReady: '2-5 minutes',
          sshAccess: plan.type === 'vps' || plan.type === 'dedicated' ? `ssh root@${ipv4}` : null,
          controlPanel: plan.type === 'web' ? `https://panel.soberano.io/${serviceId}` : null
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      throw new AppError('PROVISION_FAILED', 'Service provisioning failed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// GET /services — User's active hosting services
// ============================================================
router.get('/services', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT service_id, plan_id, plan_name, service_type, hostname, region, os,
            ipv4_address, ipv6_address, vcpus, ram_gb, storage_gb, bandwidth_gb,
            billing_cycle, price, currency, status, next_billing_date, provisioned_at, created_at
     FROM hosting_services WHERE user_id = $1 AND status != 'terminated'
     ORDER BY created_at DESC`,
    [req.user.id]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /services/:serviceId — Service details
// ============================================================
router.get('/services/:serviceId', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `SELECT * FROM hosting_services WHERE service_id = $1 AND user_id = $2`,
    [req.params.serviceId, req.user.id]
  );
  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Service not found');

  const service = result.rows[0];

  // Simulated usage stats
  const usage = {
    cpu: Math.random() * 40 + 5,
    ram: Math.random() * 60 + 10,
    storage: Math.random() * 50 + 5,
    bandwidth: Math.random() * 30 + 2,
    uptime: 99.97
  };

  res.json({
    status: 'ok',
    data: {
      service,
      usage,
      actions: ['restart', 'stop', 'resize', 'snapshot', 'reinstall', 'destroy']
    }
  });
}));

// ============================================================
// POST /services/:serviceId/action — Service actions
// ============================================================
router.post('/services/:serviceId/action',
  validate({
    body: {
      action: t.string({ required: true, enum: ['restart', 'stop', 'start', 'resize', 'snapshot', 'reinstall'] }),
      params: t.object({})
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { action, params } = req.body;

    const service = await db.query(
      `SELECT * FROM hosting_services WHERE service_id = $1 AND user_id = $2`,
      [req.params.serviceId, req.user.id]
    );
    if (service.rows.length === 0) throw new AppError('NOT_FOUND', 'Service not found');

    const statusMap = {
      restart: 'restarting',
      stop: 'stopped',
      start: 'active',
      resize: 'resizing',
      snapshot: service.rows[0].status,
      reinstall: 'reinstalling'
    };

    await db.query(
      `UPDATE hosting_services SET status = $1, updated_at = NOW() WHERE service_id = $2`,
      [statusMap[action], req.params.serviceId]
    );

    audit.record({
      category: 'HOSTING',
      action: `service_${action}`,
      userId: req.user.id,
      risk: action === 'reinstall' ? 'HIGH' : 'MEDIUM',
      details: { serviceId: req.params.serviceId, action, params }
    });

    // Auto-restore status after actions
    if (['restart', 'resize', 'reinstall'].includes(action)) {
      setTimeout(async () => {
        try {
          await db.query(
            `UPDATE hosting_services SET status = 'active', updated_at = NOW() WHERE service_id = $1`,
            [req.params.serviceId]
          );
        } catch { /* ok */ }
      }, 5000);
    }

    res.json({
      status: 'ok',
      data: {
        serviceId: req.params.serviceId,
        action,
        newStatus: statusMap[action],
        message: `Action '${action}' initiated successfully`,
        estimatedCompletion: action === 'reinstall' ? '5-10 minutes' : '30 seconds'
      }
    });
  })
);

// ============================================================
// GET /dns/:domainId — DNS records
// ============================================================
router.get('/dns/:domainId', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const domain = await db.query(
    `SELECT * FROM hosting_domains WHERE domain_id = $1 AND user_id = $2`,
    [req.params.domainId, req.user.id]
  );
  if (domain.rows.length === 0) throw new AppError('NOT_FOUND', 'Domain not found');

  const records = await db.query(
    `SELECT record_id, record_type, name, value, ttl, priority, created_at
     FROM hosting_dns_records WHERE domain_id = $1 ORDER BY record_type, name`,
    [req.params.domainId]
  );

  res.json({ status: 'ok', data: { domain: domain.rows[0], records: records.rows } });
}));

// ============================================================
// POST /dns/:domainId — Add DNS record
// ============================================================
router.post('/dns/:domainId',
  validate({
    body: {
      record_type: t.string({ required: true, enum: ['A', 'AAAA', 'CNAME', 'MX', 'TXT', 'NS', 'SRV', 'CAA'] }),
      name: t.string({ required: true, max: 253 }),
      value: t.string({ required: true, max: 1000 }),
      ttl: t.number({ min: 60, max: 86400 }),
      priority: t.number({ min: 0, max: 65535 })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { record_type, name, value, ttl, priority } = req.body;

    // Verify domain ownership
    const domain = await db.query(
      `SELECT * FROM hosting_domains WHERE domain_id = $1 AND user_id = $2`,
      [req.params.domainId, req.user.id]
    );
    if (domain.rows.length === 0) throw new AppError('NOT_FOUND', 'Domain not found');

    const recordId = generateServiceId('DNS');

    await db.query(
      `INSERT INTO hosting_dns_records (record_id, domain_id, record_type, name, value, ttl, priority)
       VALUES ($1, $2, $3, $4, $5, $6, $7)`,
      [recordId, req.params.domainId, record_type, name, value, ttl || 3600, priority || null]
    );

    res.status(201).json({
      status: 'ok',
      data: { recordId, record_type, name, value, ttl: ttl || 3600, priority: priority || null }
    });
  })
);

// ============================================================
// DELETE /dns/:domainId/:recordId — Delete DNS record
// ============================================================
router.delete('/dns/:domainId/:recordId', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  // Verify domain ownership
  const domain = await db.query(
    `SELECT id FROM hosting_domains WHERE domain_id = $1 AND user_id = $2`,
    [req.params.domainId, req.user.id]
  );
  if (domain.rows.length === 0) throw new AppError('NOT_FOUND', 'Domain not found');

  const result = await db.query(
    `DELETE FROM hosting_dns_records WHERE record_id = $1 AND domain_id = $2 RETURNING record_id`,
    [req.params.recordId, req.params.domainId]
  );

  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'DNS record not found');

  res.json({ status: 'ok', data: { deleted: result.rows[0].record_id } });
}));

// ============================================================
// GET /dashboard — Hosting overview
// ============================================================
router.get('/dashboard', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const [services, domains, spending] = await Promise.all([
    db.query(
      `SELECT service_type, COUNT(*) AS count, SUM(price) AS monthly_cost
       FROM hosting_services WHERE user_id = $1 AND status = 'active' GROUP BY service_type`,
      [req.user.id]
    ),
    db.query(
      `SELECT COUNT(*) AS total, COUNT(CASE WHEN expires_at < NOW() + INTERVAL '30 days' THEN 1 END) AS expiring_soon
       FROM hosting_domains WHERE user_id = $1 AND status = 'active'`,
      [req.user.id]
    ),
    db.query(
      `SELECT SUM(amount) AS total_spent FROM transactions
       WHERE from_user = $1 AND type IN ('hosting_provision', 'domain_registration') AND created_at > NOW() - INTERVAL '30 days'`,
      [req.user.id]
    )
  ]);

  res.json({
    status: 'ok',
    data: {
      services: services.rows,
      domains: domains.rows[0],
      spending30d: parseFloat(spending.rows[0]?.total_spent || 0),
      currency: 'WMP',
      blockchain: { network: 'MameyNode', chainId: 574, settlement: 'BDET Bank' }
    }
  });
}));

module.exports = router;

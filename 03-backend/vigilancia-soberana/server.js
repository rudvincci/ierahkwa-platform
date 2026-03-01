'use strict';

// ============================================================================
// VIGILANCIA SOBERANA — Sovereign SIEM / Security Monitoring API
// Security Information and Event Management System
// Ierahkwa Ne Kanienke / MameyNode Platform
// ============================================================================

const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const { v4: uuidv4 } = require('uuid');
const { corsConfig, applySecurityMiddleware, errorHandler } = require('../shared/security');
const db = require('./db');

const app = express();

// ============================================================================
// MIDDLEWARE
// ============================================================================

app.use(helmet());
app.use(compression());
app.use(cors(corsConfig()));
app.use(express.json({ limit: '5mb' }));

// Apply shared Ierahkwa security middleware
const logger = applySecurityMiddleware(app, 'vigilancia-soberana');

// ============================================================================
// CONFIGURATION
// ============================================================================

const MAX_EVENTS = 10000;

// ============================================================================
// HELPER — Row to camelCase event object (preserves original JSON shape)
// ============================================================================

function rowToEvent(row) {
  return {
    id: row.id,
    source: row.source,
    severity: row.severity,
    message: row.message,
    category: row.category,
    metadata: row.metadata,
    ip: row.ip,
    userAgent: row.user_agent,
    timestamp: row.timestamp instanceof Date ? row.timestamp.toISOString() : row.timestamp
  };
}

function rowToRule(row) {
  return {
    id: row.id,
    name: row.name,
    condition: row.condition,
    value: row.value,
    threshold: row.threshold,
    action: row.action,
    alertSeverity: row.alert_severity,
    enabled: row.enabled,
    triggerCount: row.trigger_count,
    lastTriggered: row.last_triggered ? (row.last_triggered instanceof Date ? row.last_triggered.toISOString() : row.last_triggered) : null,
    createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at
  };
}

function rowToTriggeredAlert(row) {
  return {
    id: row.id,
    ruleId: row.rule_id,
    ruleName: row.rule_name,
    severity: row.severity,
    message: row.message,
    event: {
      id: row.event_id,
      source: row.event_source,
      severity: row.event_severity
    },
    action: row.action,
    triggeredAt: row.triggered_at instanceof Date ? row.triggered_at.toISOString() : row.triggered_at
  };
}

// ============================================================================
// ALERT RULE EVALUATION (PostgreSQL-backed)
// ============================================================================

async function evaluateAlertRules(event) {
  const { rows: rules } = await db.query(
    'SELECT * FROM alert_rules WHERE enabled = TRUE'
  );

  for (const rule of rules) {
    let matches = false;

    switch (rule.condition) {
      case 'severity_equals':
        matches = event.severity === rule.value;
        break;
      case 'source_contains':
        matches = event.source.toLowerCase().includes(rule.value.toLowerCase());
        break;
      case 'message_contains':
        matches = event.message.toLowerCase().includes(rule.value.toLowerCase());
        break;
      case 'category_equals':
        matches = event.category === rule.value;
        break;
      default:
        break;
    }

    if (matches) {
      const newTriggerCount = (rule.trigger_count || 0) + 1;
      const now = new Date().toISOString();

      if (newTriggerCount >= (rule.threshold || 1)) {
        // Fire the alert
        const alertId = uuidv4();
        const alertMessage = `Alert rule "${rule.name}" triggered: ${event.message}`;

        await db.query(
          `INSERT INTO triggered_alerts (id, rule_id, rule_name, severity, message, event_id, event_source, event_severity, action, triggered_at)
           VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)`,
          [alertId, rule.id, rule.name, rule.alert_severity || event.severity, alertMessage, event.id, event.source, event.severity, rule.action, now]
        );

        // Keep only the last 1000 triggered alerts
        await db.query(`
          DELETE FROM triggered_alerts
          WHERE id IN (
            SELECT id FROM triggered_alerts
            ORDER BY triggered_at DESC
            OFFSET 1000
          )
        `);

        // Reset counter after alert fires
        await db.query(
          'UPDATE alert_rules SET trigger_count = 0, last_triggered = $1 WHERE id = $2',
          [now, rule.id]
        );
      } else {
        // Increment trigger count
        await db.query(
          'UPDATE alert_rules SET trigger_count = $1, last_triggered = $2 WHERE id = $3',
          [newTriggerCount, now, rule.id]
        );
      }
    }
  }
}

// ============================================================================
// COMPLIANCE HELPER (PostgreSQL-backed)
// ============================================================================

async function getComplianceScore(framework) {
  const { rows } = await db.query(
    'SELECT ROUND(AVG(score)) AS avg_score FROM compliance_controls WHERE framework_key = $1',
    [framework]
  );
  if (rows.length === 0 || rows[0].avg_score === null) return null;
  return Number(rows[0].avg_score);
}

// ============================================================================
// REST API ENDPOINTS
// ============================================================================

// Health check
app.get('/health', async (req, res) => {
  try {
    const { rows: countRows } = await db.query('SELECT COUNT(*) AS cnt FROM events');
    const eventsStored = Number(countRows[0].cnt);

    const { rows: counterRows } = await db.query("SELECT value FROM counters WHERE key = 'totalIngested'");
    const totalIngested = counterRows.length > 0 ? Number(counterRows[0].value) : 0;

    const { rows: rulesRows } = await db.query('SELECT COUNT(*) AS cnt FROM alert_rules');
    const alertRulesCount = Number(rulesRows[0].cnt);

    res.json({
      status: 'ok',
      service: 'vigilancia-soberana',
      version: '1.0.0',
      uptime: process.uptime(),
      timestamp: new Date().toISOString(),
      eventsStored,
      totalIngested,
      alertRules: alertRulesCount,
      poweredBy: 'MameyNode'
    });
  } catch (error) {
    res.json({
      status: 'ok',
      service: 'vigilancia-soberana',
      version: '1.0.0',
      uptime: process.uptime(),
      timestamp: new Date().toISOString(),
      eventsStored: 0,
      totalIngested: 0,
      alertRules: 0,
      poweredBy: 'MameyNode'
    });
  }
});

// Ingest security events
app.post('/api/events', async (req, res) => {
  try {
    const events = Array.isArray(req.body) ? req.body : [req.body];
    const ingested = [];

    for (const eventData of events) {
      const { source, severity, message, category, metadata } = eventData;

      if (!source || !severity || !message) {
        return res.status(400).json({
          success: false,
          error: 'Campos requeridos: source, severity, message'
        });
      }

      const validSeverities = ['critical', 'high', 'medium', 'low', 'info'];
      if (!validSeverities.includes(severity)) {
        return res.status(400).json({
          success: false,
          error: `Severidad invalida. Valores validos: ${validSeverities.join(', ')}`
        });
      }

      const event = {
        id: uuidv4(),
        source,
        severity,
        message,
        category: category || 'general',
        metadata: metadata || {},
        ip: req.ip || req.connection.remoteAddress,
        userAgent: req.headers['user-agent'],
        timestamp: new Date().toISOString()
      };

      await db.query(
        `INSERT INTO events (id, source, severity, message, category, metadata, ip, user_agent, timestamp)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)`,
        [event.id, event.source, event.severity, event.message, event.category, JSON.stringify(event.metadata), event.ip, event.userAgent, event.timestamp]
      );

      // Increment totalIngested counter
      await db.query("UPDATE counters SET value = value + 1 WHERE key = 'totalIngested'");

      // Enforce max events cap (remove oldest beyond MAX_EVENTS)
      await db.query(`
        DELETE FROM events
        WHERE id IN (
          SELECT id FROM events
          ORDER BY timestamp DESC
          OFFSET $1
        )
      `, [MAX_EVENTS]);

      await evaluateAlertRules(event);
      ingested.push(event);
    }

    logger.dataAccess(req, 'events', 'ingest');

    const { rows: countRows } = await db.query('SELECT COUNT(*) AS cnt FROM events');
    const bufferSize = Number(countRows[0].cnt);

    res.status(201).json({
      success: true,
      ingested: ingested.length,
      events: ingested,
      bufferSize,
      message: `${ingested.length} evento(s) registrado(s) exitosamente`
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al registrar eventos' });
  }
});

// Query events with filters
app.get('/api/events', async (req, res) => {
  try {
    const { severity, source, timeFrom, timeTo, category, search, limit = 100, offset = 0 } = req.query;

    const conditions = [];
    const params = [];
    let paramIndex = 1;

    if (severity) {
      conditions.push(`severity = $${paramIndex++}`);
      params.push(severity);
    }

    if (source) {
      conditions.push(`LOWER(source) LIKE $${paramIndex++}`);
      params.push(`%${source.toLowerCase()}%`);
    }

    if (timeFrom) {
      conditions.push(`timestamp >= $${paramIndex++}`);
      params.push(new Date(timeFrom).toISOString());
    }

    if (timeTo) {
      conditions.push(`timestamp <= $${paramIndex++}`);
      params.push(new Date(timeTo).toISOString());
    }

    if (category) {
      conditions.push(`category = $${paramIndex++}`);
      params.push(category);
    }

    if (search) {
      conditions.push(`(LOWER(message) LIKE $${paramIndex} OR LOWER(source) LIKE $${paramIndex})`);
      params.push(`%${search.toLowerCase()}%`);
      paramIndex++;
    }

    const whereClause = conditions.length > 0 ? `WHERE ${conditions.join(' AND ')}` : '';

    // Get total count
    const { rows: countRows } = await db.query(
      `SELECT COUNT(*) AS cnt FROM events ${whereClause}`,
      params
    );
    const total = Number(countRows[0].cnt);

    // Get paginated results
    const paginatedParams = [...params, Number(limit), Number(offset)];
    const { rows } = await db.query(
      `SELECT * FROM events ${whereClause} ORDER BY timestamp DESC LIMIT $${paramIndex++} OFFSET $${paramIndex++}`,
      paginatedParams
    );

    const events = rows.map(rowToEvent);

    // Get buffer size (total events stored)
    const { rows: bufferRows } = await db.query('SELECT COUNT(*) AS cnt FROM events');
    const bufferSize = Number(bufferRows[0].cnt);

    res.json({
      success: true,
      events,
      total,
      limit: Number(limit),
      offset: Number(offset),
      bufferSize
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al consultar eventos' });
  }
});

// Dashboard summary
app.get('/api/dashboard', async (req, res) => {
  try {
    // Severity counts
    const { rows: sevRows } = await db.query(
      `SELECT severity, COUNT(*) AS cnt FROM events GROUP BY severity`
    );
    const severityCounts = { critical: 0, high: 0, medium: 0, low: 0, info: 0 };
    for (const row of sevRows) {
      if (severityCounts[row.severity] !== undefined) {
        severityCounts[row.severity] = Number(row.cnt);
      }
    }

    // Top sources (top 10)
    const { rows: sourceRows } = await db.query(
      `SELECT source, COUNT(*) AS cnt FROM events GROUP BY source ORDER BY cnt DESC LIMIT 10`
    );
    const topSources = sourceRows.map(r => ({ source: r.source, count: Number(r.cnt) }));

    // Recent alerts (last 20)
    const { rows: alertRows } = await db.query(
      `SELECT * FROM triggered_alerts ORDER BY triggered_at DESC LIMIT 20`
    );
    const recentAlerts = alertRows.map(rowToTriggeredAlert);

    // Events per hour (last 24 hours)
    const now = Date.now();
    const eventsPerHour = [];
    for (let i = 23; i >= 0; i--) {
      const hourStart = new Date(now - (i + 1) * 60 * 60 * 1000).toISOString();
      const hourEnd = new Date(now - i * 60 * 60 * 1000).toISOString();
      const { rows: hourRows } = await db.query(
        `SELECT COUNT(*) AS cnt FROM events WHERE timestamp >= $1 AND timestamp < $2`,
        [hourStart, hourEnd]
      );
      eventsPerHour.push({
        hour: hourEnd,
        count: Number(hourRows[0].cnt)
      });
    }

    // Category breakdown
    const { rows: catRows } = await db.query(
      `SELECT category, COUNT(*) AS cnt FROM events GROUP BY category ORDER BY cnt DESC`
    );
    const categoryBreakdown = catRows.map(r => ({ category: r.category, count: Number(r.cnt) }));

    // Overview stats
    const { rows: countRows } = await db.query('SELECT COUNT(*) AS cnt FROM events');
    const totalEvents = Number(countRows[0].cnt);

    const { rows: counterRows } = await db.query("SELECT value FROM counters WHERE key = 'totalIngested'");
    const totalIngested = counterRows.length > 0 ? Number(counterRows[0].value) : 0;

    const { rows: rulesCountRows } = await db.query('SELECT COUNT(*) AS cnt FROM alert_rules');
    const activeAlertRules = Number(rulesCountRows[0].cnt);

    const { rows: triggeredCountRows } = await db.query('SELECT COUNT(*) AS cnt FROM triggered_alerts');
    const triggeredAlertsCount = Number(triggeredCountRows[0].cnt);

    // Compliance overview
    const owaspScore = await getComplianceScore('OWASP-TOP-10');
    const pciScore = await getComplianceScore('PCI-DSS');
    const hipaaScore = await getComplianceScore('HIPAA');

    res.json({
      success: true,
      dashboard: {
        overview: {
          totalEvents,
          totalIngested,
          activeAlertRules,
          triggeredAlertsCount,
          bufferCapacity: `${totalEvents}/${MAX_EVENTS}`
        },
        severityCounts,
        topSources,
        recentAlerts,
        eventsPerHour,
        categoryBreakdown,
        complianceOverview: {
          'OWASP-TOP-10': owaspScore,
          'PCI-DSS': pciScore,
          'HIPAA': hipaaScore
        },
        lastUpdated: new Date().toISOString()
      }
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al generar dashboard' });
  }
});

// Create alert rule
app.post('/api/alerts', async (req, res) => {
  try {
    const { name, condition, value, threshold, action, alertSeverity, enabled } = req.body;

    if (!name || !condition || !value) {
      return res.status(400).json({
        success: false,
        error: 'Campos requeridos: name, condition, value'
      });
    }

    const validConditions = ['severity_equals', 'source_contains', 'message_contains', 'category_equals'];
    if (!validConditions.includes(condition)) {
      return res.status(400).json({
        success: false,
        error: `Condicion invalida. Valores validos: ${validConditions.join(', ')}`
      });
    }

    const rule = {
      id: uuidv4(),
      name,
      condition,
      value,
      threshold: threshold || 1,
      action: action || 'log',
      alertSeverity: alertSeverity || 'high',
      enabled: enabled !== false,
      triggerCount: 0,
      lastTriggered: null,
      createdAt: new Date().toISOString()
    };

    await db.query(
      `INSERT INTO alert_rules (id, name, condition, value, threshold, action, alert_severity, enabled, trigger_count, last_triggered, created_at)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11)`,
      [rule.id, rule.name, rule.condition, rule.value, rule.threshold, rule.action, rule.alertSeverity, rule.enabled, rule.triggerCount, rule.lastTriggered, rule.createdAt]
    );

    logger.dataAccess(req, 'alert-rules', 'create');

    res.status(201).json({
      success: true,
      rule,
      message: `Regla de alerta "${name}" creada exitosamente`
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al crear regla de alerta' });
  }
});

// List alert rules
app.get('/api/alerts', async (req, res) => {
  try {
    const { enabled } = req.query;

    let rulesQuery = 'SELECT * FROM alert_rules';
    const params = [];

    if (enabled !== undefined) {
      rulesQuery += ' WHERE enabled = $1';
      params.push(enabled === 'true');
    }

    const { rows } = await db.query(rulesQuery, params);
    const rules = rows.map(rowToRule);

    // Recent triggered alerts (last 10)
    const { rows: recentRows } = await db.query(
      'SELECT * FROM triggered_alerts ORDER BY triggered_at DESC LIMIT 10'
    );
    const recentlyTriggered = recentRows.map(rowToTriggeredAlert);

    res.json({
      success: true,
      rules,
      total: rules.length,
      recentlyTriggered
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al listar reglas de alerta' });
  }
});

// Compliance status
app.get('/api/compliance', async (req, res) => {
  try {
    const { framework } = req.query;

    if (framework) {
      // Check if framework exists
      const { rows: fwRows } = await db.query(
        'SELECT * FROM compliance_frameworks WHERE key = $1',
        [framework]
      );

      if (fwRows.length > 0) {
        const fw = fwRows[0];
        const { rows: controlRows } = await db.query(
          'SELECT control_id AS id, name, status, score FROM compliance_controls WHERE framework_key = $1',
          [framework]
        );

        const overallScore = await getComplianceScore(framework);

        return res.json({
          success: true,
          framework: framework,
          name: fw.name,
          overallScore,
          controls: controlRows,
          assessedAt: new Date().toISOString()
        });
      }
    }

    // Return all frameworks
    const { rows: allFwRows } = await db.query('SELECT * FROM compliance_frameworks');
    const allFrameworks = {};

    for (const fw of allFwRows) {
      const { rows: controlRows } = await db.query(
        'SELECT control_id AS id, name, status, score FROM compliance_controls WHERE framework_key = $1',
        [fw.key]
      );

      const overallScore = await getComplianceScore(fw.key);

      allFrameworks[fw.key] = {
        name: fw.name,
        overallScore,
        totalControls: controlRows.length,
        passing: controlRows.filter(c => c.status === 'pass').length,
        warnings: controlRows.filter(c => c.status === 'warning').length,
        failing: controlRows.filter(c => c.status === 'fail').length
      };
    }

    // Calculate overall compliance
    const scores = Object.values(allFrameworks).map(f => f.overallScore).filter(s => s !== null);
    const overallCompliance = scores.length > 0
      ? Math.round(scores.reduce((sum, s) => sum + s, 0) / scores.length)
      : 0;

    res.json({
      success: true,
      frameworks: allFrameworks,
      overallCompliance,
      assessedAt: new Date().toISOString()
    });
  } catch (error) {
    res.status(500).json({ success: false, error: 'Error al obtener estado de cumplimiento' });
  }
});

// ============================================================================
// ERROR HANDLING
// ============================================================================

// 404 handler
app.use((req, res) => {
  res.status(404).json({
    error: 'Endpoint no encontrado',
    path: req.path,
    method: req.method
  });
});

// Global error handler
app.use(errorHandler('vigilancia-soberana'));

// ============================================================================
// START SERVER
// ============================================================================

const PORT = process.env.PORT || 3091;

async function start() {
  try {
    // Initialize database schema and seed data
    await db.initialize();

    const server = app.listen(PORT, () => {
      console.log('');
      console.log('  ============================================================');
      console.log('  ||                                                        ||');
      console.log('  ||     VIGILANCIA SOBERANA                                ||');
      console.log('  ||     Sovereign SIEM / Security Monitoring               ||');
      console.log('  ||                                                        ||');
      console.log('  ||     Real-time Event Ingestion & Analysis               ||');
      console.log('  ||     Alert Rules Engine + Compliance Tracking           ||');
      console.log('  ||                                                        ||');
      console.log(`  ||     Port: ${PORT}                                         ||`);
      console.log('  ||     Status: OPERATIONAL                                ||');
      console.log('  ||     Storage: PostgreSQL                                ||');
      console.log('  ||                                                        ||');
      console.log('  ||     Powered by MameyNode                               ||');
      console.log('  ||     Ierahkwa Ne Kanienke Sovereign Platform            ||');
      console.log('  ||                                                        ||');
      console.log('  ============================================================');
      console.log('');
      console.log(`  [INFO] SIEM API ready on http://localhost:${PORT}`);
      console.log(`  [INFO] Event buffer capacity: ${MAX_EVENTS}`);
      console.log(`  [INFO] Compliance frameworks: OWASP, PCI-DSS, HIPAA`);
      console.log(`  [INFO] Database: ${process.env.DATABASE_URL || 'postgresql://localhost:5432/vigilancia_soberana'}`);
      console.log('');
    });

    // Graceful shutdown
    const shutdown = async (signal) => {
      console.log(`\n  [INFO] ${signal} received — shutting down gracefully`);
      server.close(async () => {
        await db.end();
        console.log('  [INFO] Database pool closed');
        process.exit(0);
      });

      // Force shutdown after 10s
      setTimeout(() => {
        console.error('  [ERROR] Forced shutdown after timeout');
        process.exit(1);
      }, 10000);
    };

    process.on('SIGTERM', () => shutdown('SIGTERM'));
    process.on('SIGINT', () => shutdown('SIGINT'));

  } catch (err) {
    console.error('  [FATAL] Failed to start server:', err.message);
    process.exit(1);
  }
}

start();

module.exports = app;

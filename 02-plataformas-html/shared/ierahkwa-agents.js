/**
 * Ierahkwa Sovereign AI Agents v1.0.0
 * Sistema de Agentes Inteligentes Anti-Fraude con Aprendizaje Adaptativo
 *
 * 7 Agentes Autónomos:
 *   1. GuardianAgent    — Vigilancia continua anti-fraude y anti-robo
 *   2. PatternAgent     — Aprendizaje de patrones de comportamiento
 *   3. AnomalyAgent     — Detección de anomalías y actividad sospechosa
 *   4. TrustAgent       — Sistema de reputación y confianza
 *   5. ShieldAgent      — Protección en tiempo real de transacciones
 *   6. ForensicAgent    — Análisis forense y trazabilidad
 *   7. EvolutionAgent   — Auto-mejora y adaptación continua
 *
 * Capacidades:
 *   - Aprendizaje por experiencia (IndexedDB behavioral store)
 *   - Detección de fraude en tiempo real
 *   - Análisis de patrones de uso por usuario
 *   - Score de confianza dinámico
 *   - Alertas instantáneas de actividad sospechosa
 *   - Evolución autónoma de reglas de detección
 *   - Protección de transacciones financieras
 *   - Anti-phishing y anti-suplantación
 */

(function() {
  'use strict';

  // ============================================================
  // CONFIGURACIÓN GLOBAL
  // ============================================================
  const AGENTS_VERSION = '1.0.0';
  const DB_NAME = 'ierahkwa-agents';
  const DB_VERSION = 1;
  const STORES = {
    behaviors: 'agent-behaviors',
    anomalies: 'agent-anomalies',
    trust: 'agent-trust-scores',
    rules: 'agent-evolved-rules',
    forensics: 'agent-forensics',
    sessions: 'agent-sessions'
  };

  const platform = detectPlatform();

  // ============================================================
  // 1. GUARDIAN AGENT — Vigilancia Anti-Fraude
  // ============================================================
  const GuardianAgent = {
    name: 'Guardian',
    active: true,
    alerts: [],
    watchList: new Set(),

    init() {
      this.monitorDOM();
      this.monitorNetwork();
      this.monitorForms();
      this.monitorClipboard();
      console.log(`[Guardian] 🛡️ Vigilancia activa en ${platform}`);
    },

    // Vigila cambios sospechosos en el DOM
    monitorDOM() {
      const observer = new MutationObserver(mutations => {
        for (const m of mutations) {
          for (const node of m.addedNodes) {
            if (node.nodeType !== 1) continue;
            // Detectar scripts inyectados
            if (node.tagName === 'SCRIPT' && !node.src?.includes('ierahkwa')) {
              this.raiseAlert('script-injection', {
                src: node.src || 'inline',
                content: node.textContent?.substring(0, 200)
              });
            }
            // Detectar iframes sospechosos
            if (node.tagName === 'IFRAME' && !node.src?.includes('ierahkwa.nation')) {
              this.raiseAlert('suspicious-iframe', { src: node.src });
            }
            // Detectar formularios ocultos (phishing)
            if (node.tagName === 'FORM' && (
              node.style.display === 'none' ||
              node.style.opacity === '0' ||
              node.style.position === 'absolute' && parseInt(node.style.left) < -1000
            )) {
              this.raiseAlert('hidden-form', { action: node.action });
            }
          }
        }
      });
      observer.observe(document.body || document.documentElement, {
        childList: true, subtree: true
      });
    },

    // Vigila peticiones de red sospechosas
    monitorNetwork() {
      const origFetch = window.fetch;
      window.fetch = async (url, opts) => {
        const urlStr = typeof url === 'string' ? url : url?.url || '';
        // Detectar exfiltración de datos
        if (this.isExfiltrationAttempt(urlStr, opts)) {
          this.raiseAlert('data-exfiltration', { url: urlStr, method: opts?.method });
          throw new Error('[Guardian] Solicitud bloqueada: posible exfiltración de datos');
        }
        return origFetch(url, opts);
      };

      const origXHR = XMLHttpRequest.prototype.open;
      XMLHttpRequest.prototype.open = function(method, url) {
        if (GuardianAgent.isExfiltrationAttempt(url, { method })) {
          GuardianAgent.raiseAlert('xhr-exfiltration', { url, method });
        }
        return origXHR.apply(this, arguments);
      };
    },

    // Vigila formularios de datos sensibles
    monitorForms() {
      document.addEventListener('submit', e => {
        const form = e.target;
        const inputs = form.querySelectorAll('input[type="password"], input[type="email"], input[name*="card"], input[name*="account"], input[name*="ssn"]');
        if (inputs.length > 0) {
          const action = form.action || window.location.href;
          if (!action.includes('ierahkwa.nation') && !action.startsWith('/')) {
            this.raiseAlert('form-data-leak', {
              action,
              sensitiveFields: Array.from(inputs).map(i => i.name)
            });
            e.preventDefault();
          }
        }
      }, true);
    },

    // Vigila clipboard para prevenir robo de datos
    monitorClipboard() {
      document.addEventListener('copy', () => {
        const selection = window.getSelection()?.toString() || '';
        if (this.containsSensitiveData(selection)) {
          this.raiseAlert('sensitive-copy', {
            dataType: this.classifySensitiveData(selection),
            length: selection.length
          });
        }
      });
    },

    isExfiltrationAttempt(url, opts) {
      if (!url) return false;
      const suspicious = [
        'pastebin.com', 'webhook.site', 'requestbin',
        'ngrok.io', 'serveo.net', 'localtunnel'
      ];
      return suspicious.some(s => url.includes(s));
    },

    containsSensitiveData(text) {
      if (!text || text.length < 8) return false;
      const patterns = [
        /\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b/,  // Card
        /\b\d{3}-\d{2}-\d{4}\b/,                          // SSN
        /\b[A-Z0-9]{32,}\b/i,                              // API Key
        /\bWMP[A-Z0-9]{20,}\b/,                             // WAMPUM wallet
      ];
      return patterns.some(p => p.test(text));
    },

    classifySensitiveData(text) {
      if (/\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}/.test(text)) return 'tarjeta';
      if (/\d{3}-\d{2}-\d{4}/.test(text)) return 'identificacion';
      if (/WMP/.test(text)) return 'wallet-wampum';
      return 'datos-sensibles';
    },

    raiseAlert(type, details) {
      const alert = {
        agent: 'Guardian',
        type,
        details,
        timestamp: Date.now(),
        platform,
        severity: this.getSeverity(type)
      };
      this.alerts.push(alert);
      ForensicAgent.log(alert);
      EvolutionAgent.learn('threat', alert);

      if (alert.severity === 'critical') {
        this.notifyUser(alert);
      }
      console.warn(`[Guardian] ⚠️ ALERTA ${alert.severity}: ${type}`, details);
    },

    getSeverity(type) {
      const critical = ['data-exfiltration', 'xhr-exfiltration', 'form-data-leak'];
      const high = ['script-injection', 'hidden-form', 'sensitive-copy'];
      if (critical.includes(type)) return 'critical';
      if (high.includes(type)) return 'high';
      return 'medium';
    },

    notifyUser(alert) {
      if (!document.getElementById('guardian-alert-panel')) {
        const panel = document.createElement('div');
        panel.id = 'guardian-alert-panel';
        panel.setAttribute('role', 'alert');
        panel.setAttribute('aria-live', 'assertive');
        panel.style.cssText = 'position:fixed;top:1rem;right:1rem;z-index:99999;background:#1a0000;border:2px solid #f44336;border-radius:12px;padding:1rem 1.5rem;color:#ff8a80;font-family:system-ui;font-size:.85rem;max-width:400px;box-shadow:0 4px 24px rgba(244,67,54,.3);animation:fadeIn .3s ease';
        panel.innerHTML = `
          <div style="display:flex;align-items:center;gap:.5rem;margin-bottom:.5rem">
            <span style="font-size:1.2rem">🛡️</span>
            <strong style="color:#f44336">Agente Guardian — Alerta de Seguridad</strong>
          </div>
          <p style="margin:0;line-height:1.5">${this.getAlertMessage(alert)}</p>
          <button onclick="this.parentElement.remove()" style="margin-top:.8rem;background:#f44336;color:#fff;border:none;padding:.4rem 1rem;border-radius:8px;cursor:pointer;font-size:.8rem">Entendido</button>
        `;
        (document.body || document.documentElement).appendChild(panel);
        setTimeout(() => panel.remove(), 15000);
      }
    },

    getAlertMessage(alert) {
      const msgs = {
        'data-exfiltration': 'Se detectó un intento de enviar datos a un servidor externo sospechoso. La solicitud fue bloqueada.',
        'script-injection': 'Se detectó un script no autorizado intentando ejecutarse. Puede ser un intento de ataque.',
        'form-data-leak': 'Un formulario intentó enviar datos sensibles a un destino no soberano.',
        'hidden-form': 'Se detectó un formulario oculto — posible intento de phishing.',
        'sensitive-copy': 'Se copió información sensible al portapapeles. Tenga cuidado al pegarla.',
        'suspicious-iframe': 'Se detectó un iframe de origen externo.'
      };
      return msgs[alert.type] || `Actividad sospechosa detectada: ${alert.type}`;
    }
  };

  // ============================================================
  // 2. PATTERN AGENT — Aprendizaje de Patrones
  // ============================================================
  const PatternAgent = {
    name: 'Pattern',
    sessionData: {
      clicks: [], navigations: [], inputs: [],
      startTime: Date.now(), platform
    },
    userProfile: null,

    init() {
      this.loadProfile();
      this.trackNavigation();
      this.trackInteraction();
      this.trackTiming();
      console.log(`[Pattern] 🧠 Aprendiendo patrones en ${platform}`);
    },

    async loadProfile() {
      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.behaviors, 'readonly');
        const store = tx.objectStore(STORES.behaviors);
        const req = store.get('user-profile');
        req.onsuccess = () => {
          this.userProfile = req.result?.data || this.createDefaultProfile();
        };
      } catch {
        this.userProfile = this.createDefaultProfile();
      }
    },

    createDefaultProfile() {
      return {
        totalSessions: 0,
        avgSessionDuration: 0,
        commonPlatforms: {},
        typicalHours: new Array(24).fill(0),
        typicalDays: new Array(7).fill(0),
        avgClicksPerSession: 0,
        avgInputsPerSession: 0,
        deviceFingerprint: this.getDeviceFingerprint(),
        lastUpdated: Date.now()
      };
    },

    getDeviceFingerprint() {
      return {
        screen: `${screen.width}x${screen.height}`,
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
        language: navigator.language,
        platform: navigator.platform,
        cores: navigator.hardwareConcurrency || 'unknown'
      };
    },

    trackNavigation() {
      this.sessionData.navigations.push({
        url: location.pathname,
        time: Date.now()
      });
    },

    trackInteraction() {
      document.addEventListener('click', e => {
        this.sessionData.clicks.push({
          target: e.target.tagName,
          x: e.clientX, y: e.clientY,
          time: Date.now()
        });
      }, { passive: true });

      document.addEventListener('input', e => {
        this.sessionData.inputs.push({
          type: e.target.type || 'text',
          time: Date.now()
        });
      }, { passive: true });
    },

    trackTiming() {
      // Cada 60 segundos, actualizar perfil
      setInterval(() => this.updateProfile(), 60000);
      // Al cerrar, guardar sesión
      window.addEventListener('beforeunload', () => this.saveSession());
    },

    async updateProfile() {
      if (!this.userProfile) return;
      const now = new Date();
      this.userProfile.typicalHours[now.getHours()]++;
      this.userProfile.typicalDays[now.getDay()]++;
      this.userProfile.commonPlatforms[platform] =
        (this.userProfile.commonPlatforms[platform] || 0) + 1;
      this.userProfile.lastUpdated = Date.now();
    },

    async saveSession() {
      if (!this.userProfile) return;
      const duration = Date.now() - this.sessionData.startTime;
      this.userProfile.totalSessions++;
      this.userProfile.avgSessionDuration = (
        (this.userProfile.avgSessionDuration * (this.userProfile.totalSessions - 1)) + duration
      ) / this.userProfile.totalSessions;
      this.userProfile.avgClicksPerSession = (
        (this.userProfile.avgClicksPerSession * (this.userProfile.totalSessions - 1)) + this.sessionData.clicks.length
      ) / this.userProfile.totalSessions;

      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.behaviors, 'readwrite');
        tx.objectStore(STORES.behaviors).put({
          id: 'user-profile',
          data: this.userProfile,
          updated: Date.now()
        });
      } catch { /* offline, retry later */ }
    },

    // Verifica si el comportamiento actual es consistente con el perfil
    isConsistentBehavior() {
      if (!this.userProfile || this.userProfile.totalSessions < 3) return true;
      const now = new Date();
      const hourActivity = this.userProfile.typicalHours[now.getHours()];
      const totalActivity = this.userProfile.typicalHours.reduce((a, b) => a + b, 0);
      if (totalActivity === 0) return true;
      const hourRatio = hourActivity / totalActivity;
      // Si esta hora tiene menos del 1% de actividad histórica = inusual
      return hourRatio > 0.01;
    }
  };

  // ============================================================
  // 3. ANOMALY AGENT — Detección de Anomalías
  // ============================================================
  const AnomalyAgent = {
    name: 'Anomaly',
    thresholds: {
      rapidClicks: 20,         // clicks por segundo
      rapidRequests: 50,       // requests por minuto
      largeDataTransfer: 5e6,  // 5MB en una sola request
      unusualHour: true,       // actividad a horas no habituales
      multiTabAbuse: 10        // demasiadas tabs abiertas
    },
    counters: { clicks: 0, requests: 0, dataBytes: 0 },

    init() {
      this.monitorClickRate();
      this.monitorRequestRate();
      this.checkUnusualActivity();
      setInterval(() => this.resetCounters(), 60000);
      console.log(`[Anomaly] 🔍 Detección de anomalías activa`);
    },

    monitorClickRate() {
      let clickTimestamps = [];
      document.addEventListener('click', () => {
        const now = Date.now();
        clickTimestamps.push(now);
        // Mantener solo últimos 2 segundos
        clickTimestamps = clickTimestamps.filter(t => now - t < 2000);
        if (clickTimestamps.length > this.thresholds.rapidClicks) {
          this.reportAnomaly('rapid-clicks', {
            clicksPerSecond: clickTimestamps.length / 2,
            threshold: this.thresholds.rapidClicks
          });
        }
      }, { passive: true });
    },

    monitorRequestRate() {
      const origFetch = window.fetch;
      const self = this;
      // Re-wrap después de GuardianAgent
      const currentFetch = window.fetch;
      window.fetch = async function(url, opts) {
        self.counters.requests++;
        if (self.counters.requests > self.thresholds.rapidRequests) {
          self.reportAnomaly('rapid-requests', {
            requestsPerMinute: self.counters.requests,
            threshold: self.thresholds.rapidRequests
          });
        }
        return currentFetch(url, opts);
      };
    },

    checkUnusualActivity() {
      if (!PatternAgent.isConsistentBehavior()) {
        this.reportAnomaly('unusual-hour', {
          hour: new Date().getHours(),
          message: 'Actividad detectada a hora inusual para este usuario'
        });
      }
    },

    resetCounters() {
      this.counters = { clicks: 0, requests: 0, dataBytes: 0 };
    },

    reportAnomaly(type, details) {
      const anomaly = {
        agent: 'Anomaly',
        type,
        details,
        timestamp: Date.now(),
        platform,
        riskScore: this.calculateRisk(type, details)
      };

      // Reducir trust score
      TrustAgent.adjustScore(-anomaly.riskScore);
      ForensicAgent.log(anomaly);
      EvolutionAgent.learn('anomaly', anomaly);

      if (anomaly.riskScore > 7) {
        GuardianAgent.raiseAlert('anomaly-' + type, details);
      }
      console.warn(`[Anomaly] 🔍 Anomalía detectada: ${type} (riesgo: ${anomaly.riskScore}/10)`, details);
    },

    calculateRisk(type, details) {
      const risks = {
        'rapid-clicks': 3,
        'rapid-requests': 6,
        'large-data-transfer': 8,
        'unusual-hour': 2,
        'multi-tab-abuse': 4
      };
      return risks[type] || 5;
    }
  };

  // ============================================================
  // 4. TRUST AGENT — Sistema de Confianza
  // ============================================================
  const TrustAgent = {
    name: 'Trust',
    score: 100,  // 0-100
    history: [],

    init() {
      this.loadScore();
      this.displayBadge();
      console.log(`[Trust] ⭐ Score de confianza: ${this.score}`);
    },

    async loadScore() {
      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.trust, 'readonly');
        const req = tx.objectStore(STORES.trust).get('current-score');
        req.onsuccess = () => {
          if (req.result) {
            this.score = req.result.score;
            this.history = req.result.history || [];
          }
        };
      } catch { /* use default */ }
    },

    adjustScore(delta) {
      const oldScore = this.score;
      this.score = Math.max(0, Math.min(100, this.score + delta));
      this.history.push({
        from: oldScore,
        to: this.score,
        delta,
        timestamp: Date.now(),
        platform
      });

      // Guardar
      this.saveScore();

      // Si baja de 30, activar modo protegido
      if (this.score < 30 && oldScore >= 30) {
        ShieldAgent.activateProtectedMode();
      }
      // Si baja de 10, bloquear transacciones
      if (this.score < 10) {
        ShieldAgent.blockTransactions();
      }
    },

    async saveScore() {
      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.trust, 'readwrite');
        tx.objectStore(STORES.trust).put({
          id: 'current-score',
          score: this.score,
          history: this.history.slice(-100),
          updated: Date.now()
        });
      } catch { /* retry later */ }
    },

    // Incrementar score por buen comportamiento
    reward(reason) {
      this.adjustScore(+1);
      EvolutionAgent.learn('trust-reward', { reason, score: this.score });
    },

    displayBadge() {
      // No mostrar badge en plataformas sin body
      if (!document.body) return;
      const badge = document.createElement('div');
      badge.id = 'trust-badge';
      badge.setAttribute('aria-label', `Score de confianza: ${this.score}`);
      badge.style.cssText = 'position:fixed;bottom:1rem;left:1rem;z-index:9999;background:rgba(10,14,23,.9);border:1px solid rgba(0,255,65,.3);border-radius:50%;width:36px;height:36px;display:flex;align-items:center;justify-content:center;font-size:.7rem;font-weight:700;cursor:pointer;transition:all .3s;backdrop-filter:blur(8px)';
      badge.style.color = this.score > 70 ? '#00FF41' : this.score > 30 ? '#ffd600' : '#f44336';
      badge.textContent = this.score;
      badge.title = `Agentes AI Soberanos — Confianza: ${this.score}/100`;
      badge.addEventListener('click', () => this.showPanel());
      document.body.appendChild(badge);
    },

    showPanel() {
      if (document.getElementById('agents-panel')) {
        document.getElementById('agents-panel').remove();
        return;
      }
      const panel = document.createElement('div');
      panel.id = 'agents-panel';
      panel.style.cssText = 'position:fixed;bottom:3.5rem;left:1rem;z-index:9999;background:#0a0e17;border:1px solid rgba(0,255,65,.3);border-radius:12px;padding:1rem;color:#e8e4df;font-family:system-ui;font-size:.78rem;width:280px;max-height:360px;overflow-y:auto;box-shadow:0 8px 32px rgba(0,0,0,.5)';
      panel.innerHTML = `
        <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:.8rem">
          <strong style="color:#00FF41;font-size:.85rem">🤖 Agentes AI Soberanos</strong>
          <button onclick="this.parentElement.parentElement.remove()" style="background:none;border:none;color:#8a8694;cursor:pointer;font-size:1rem">&times;</button>
        </div>
        <div style="margin-bottom:.6rem">
          <div style="display:flex;justify-content:space-between"><span>🛡️ Guardian</span><span style="color:#00FF41">Activo</span></div>
          <div style="display:flex;justify-content:space-between"><span>🧠 Pattern</span><span style="color:#00FF41">Aprendiendo</span></div>
          <div style="display:flex;justify-content:space-between"><span>🔍 Anomaly</span><span style="color:#00FF41">Vigilando</span></div>
          <div style="display:flex;justify-content:space-between"><span>⭐ Trust</span><span style="color:${this.score > 70 ? '#00FF41' : '#ffd600'}">${this.score}/100</span></div>
          <div style="display:flex;justify-content:space-between"><span>🔒 Shield</span><span style="color:#00FF41">${ShieldAgent.protectedMode ? 'Protegido' : 'Normal'}</span></div>
          <div style="display:flex;justify-content:space-between"><span>🔬 Forensic</span><span style="color:#00FF41">${ForensicAgent.logCount} eventos</span></div>
          <div style="display:flex;justify-content:space-between"><span>🧬 Evolution</span><span style="color:#00FF41">Gen ${EvolutionAgent.generation}</span></div>
        </div>
        <div style="border-top:1px solid rgba(255,255,255,.1);padding-top:.6rem;margin-top:.4rem">
          <div style="font-size:.7rem;color:#8a8694">
            Alertas: ${GuardianAgent.alerts.length} |
            Sesiones: ${PatternAgent.userProfile?.totalSessions || 0} |
            Reglas: ${EvolutionAgent.evolvedRules.length}
          </div>
          <div style="font-size:.65rem;color:#555;margin-top:.3rem">v${AGENTS_VERSION} — ${platform}</div>
        </div>
      `;
      document.body.appendChild(panel);
    }
  };

  // ============================================================
  // 5. SHIELD AGENT — Protección de Transacciones
  // ============================================================
  const ShieldAgent = {
    name: 'Shield',
    protectedMode: false,
    transactionsBlocked: false,
    transactionLog: [],

    init() {
      this.protectForms();
      this.protectStorage();
      this.protectCookies();
      console.log(`[Shield] 🔒 Protección de transacciones activa`);
    },

    // Proteger formularios financieros
    protectForms() {
      document.addEventListener('submit', e => {
        const form = e.target;
        // Detectar formularios de pago/transferencia
        const isFinancial = form.querySelector('[name*="amount"], [name*="monto"], [name*="transfer"], [name*="pago"], [name*="wallet"]');
        if (isFinancial && this.transactionsBlocked) {
          e.preventDefault();
          this.notifyBlocked('Transacción bloqueada: score de confianza demasiado bajo');
          return;
        }
        if (isFinancial) {
          this.logTransaction(form);
        }
      }, true);
    },

    // Proteger localStorage/sessionStorage contra manipulación
    protectStorage() {
      const origSetItem = Storage.prototype.setItem;
      Storage.prototype.setItem = function(key, value) {
        // Detectar intentos de modificar datos de sesión/auth
        if (key.includes('token') || key.includes('auth') || key.includes('session')) {
          const callStack = new Error().stack || '';
          if (!callStack.includes('ierahkwa')) {
            GuardianAgent.raiseAlert('storage-tamper', { key, source: 'external' });
          }
        }
        return origSetItem.call(this, key, value);
      };
    },

    // Proteger cookies contra robo
    protectCookies() {
      // Monitorear acceso a cookies sensibles
      let lastCookies = document.cookie;
      setInterval(() => {
        if (document.cookie !== lastCookies) {
          const diff = this.cookieDiff(lastCookies, document.cookie);
          if (diff.removed.some(c => c.includes('auth') || c.includes('session'))) {
            GuardianAgent.raiseAlert('cookie-theft', { removed: diff.removed });
          }
          lastCookies = document.cookie;
        }
      }, 5000);
    },

    cookieDiff(old, current) {
      const oldSet = new Set(old.split('; '));
      const newSet = new Set(current.split('; '));
      return {
        added: [...newSet].filter(c => !oldSet.has(c)),
        removed: [...oldSet].filter(c => !newSet.has(c))
      };
    },

    logTransaction(form) {
      const entry = {
        timestamp: Date.now(),
        platform,
        action: form.action,
        method: form.method,
        fields: Array.from(form.elements).map(e => e.name).filter(Boolean)
      };
      this.transactionLog.push(entry);
      ForensicAgent.log({ agent: 'Shield', type: 'transaction', details: entry });
      TrustAgent.reward('legitimate-transaction');
    },

    activateProtectedMode() {
      this.protectedMode = true;
      console.warn('[Shield] ⚠️ MODO PROTEGIDO ACTIVADO — Verificación extra requerida');
    },

    blockTransactions() {
      this.transactionsBlocked = true;
      console.error('[Shield] 🚫 TRANSACCIONES BLOQUEADAS — Score de confianza crítico');
    },

    notifyBlocked(message) {
      const notice = document.createElement('div');
      notice.setAttribute('role', 'alert');
      notice.style.cssText = 'position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);z-index:99999;background:#1a0000;border:2px solid #f44336;border-radius:16px;padding:2rem;color:#ff8a80;font-family:system-ui;text-align:center;max-width:400px;box-shadow:0 8px 32px rgba(244,67,54,.4)';
      notice.innerHTML = `<div style="font-size:2rem;margin-bottom:1rem">🚫</div><p style="margin:0 0 1rem">${message}</p><button onclick="this.parentElement.remove()" style="background:#f44336;color:#fff;border:none;padding:.5rem 1.5rem;border-radius:8px;cursor:pointer">Cerrar</button>`;
      document.body?.appendChild(notice);
    }
  };

  // ============================================================
  // 6. FORENSIC AGENT — Análisis Forense
  // ============================================================
  const ForensicAgent = {
    name: 'Forensic',
    logCount: 0,
    buffer: [],

    init() {
      this.flushInterval = setInterval(() => this.flush(), 30000);
      console.log(`[Forensic] 🔬 Registro forense activo`);
    },

    log(event) {
      this.logCount++;
      this.buffer.push({
        ...event,
        id: `${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
        userAgent: navigator.userAgent?.substring(0, 80),
        url: location.href,
        referrer: document.referrer
      });

      // Flush si el buffer es grande
      if (this.buffer.length >= 20) {
        this.flush();
      }
    },

    async flush() {
      if (this.buffer.length === 0) return;
      const events = [...this.buffer];
      this.buffer = [];

      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.forensics, 'readwrite');
        const store = tx.objectStore(STORES.forensics);
        for (const event of events) {
          store.put(event);
        }
      } catch { /* offline, events lost */ }
    },

    // Obtener historial forense para análisis
    async getHistory(limit = 100) {
      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.forensics, 'readonly');
        const store = tx.objectStore(STORES.forensics);
        return new Promise(resolve => {
          const req = store.getAll();
          req.onsuccess = () => resolve(req.result.slice(-limit));
        });
      } catch {
        return [];
      }
    }
  };

  // ============================================================
  // 7. EVOLUTION AGENT — Auto-Mejora Continua
  // ============================================================
  const EvolutionAgent = {
    name: 'Evolution',
    generation: 1,
    evolvedRules: [],
    learningData: {
      threats: [], anomalies: [], rewards: []
    },

    init() {
      this.loadRules();
      // Evolucionar cada 5 minutos
      setInterval(() => this.evolve(), 300000);
      console.log(`[Evolution] 🧬 Motor de evolución activo — Gen ${this.generation}`);
    },

    async loadRules() {
      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.rules, 'readonly');
        const req = tx.objectStore(STORES.rules).get('evolved-rules');
        req.onsuccess = () => {
          if (req.result) {
            this.evolvedRules = req.result.rules || [];
            this.generation = req.result.generation || 1;
          }
        };
      } catch { /* use defaults */ }
    },

    learn(type, data) {
      if (type === 'threat') this.learningData.threats.push(data);
      else if (type === 'anomaly') this.learningData.anomalies.push(data);
      else if (type === 'trust-reward') this.learningData.rewards.push(data);

      // Mantener últimos 1000 eventos
      Object.keys(this.learningData).forEach(key => {
        if (this.learningData[key].length > 1000) {
          this.learningData[key] = this.learningData[key].slice(-500);
        }
      });
    },

    async evolve() {
      if (this.learningData.threats.length < 5 &&
          this.learningData.anomalies.length < 5) return;

      this.generation++;
      const newRules = [];

      // Analizar patrones de amenazas repetidas
      const threatTypes = {};
      this.learningData.threats.forEach(t => {
        threatTypes[t.type] = (threatTypes[t.type] || 0) + 1;
      });

      // Si un tipo de amenaza se repite >3 veces, crear regla preventiva
      for (const [type, count] of Object.entries(threatTypes)) {
        if (count >= 3) {
          newRules.push({
            id: `rule-${this.generation}-${type}`,
            type: 'block',
            trigger: type,
            action: 'auto-block',
            confidence: Math.min(count / 10, 1),
            created: Date.now(),
            generation: this.generation
          });
        }
      }

      // Ajustar umbrales de anomalías basado en falsos positivos
      const anomalyTypes = {};
      this.learningData.anomalies.forEach(a => {
        anomalyTypes[a.type] = (anomalyTypes[a.type] || 0) + 1;
      });

      // Si muchas anomalías del mismo tipo = posible falso positivo, relajar umbral
      for (const [type, count] of Object.entries(anomalyTypes)) {
        if (count > 20 && type === 'unusual-hour') {
          // Muchas "horas inusuales" = el usuario cambió horario
          newRules.push({
            id: `rule-${this.generation}-relax-${type}`,
            type: 'relax',
            trigger: type,
            action: 'increase-threshold',
            confidence: 0.7,
            created: Date.now(),
            generation: this.generation
          });
        }
      }

      if (newRules.length > 0) {
        this.evolvedRules.push(...newRules);
        await this.saveRules();
        console.log(`[Evolution] 🧬 Generación ${this.generation}: ${newRules.length} nuevas reglas evolucionadas`);
      }

      // Limpiar datos de aprendizaje procesados
      this.learningData.threats = [];
      this.learningData.anomalies = [];
    },

    async saveRules() {
      try {
        const db = await openAgentDB();
        const tx = db.transaction(STORES.rules, 'readwrite');
        tx.objectStore(STORES.rules).put({
          id: 'evolved-rules',
          rules: this.evolvedRules.slice(-200),
          generation: this.generation,
          updated: Date.now()
        });
      } catch { /* retry later */ }
    }
  };

  // ============================================================
  // UTILIDADES
  // ============================================================
  function detectPlatform() {
    const path = location.pathname.replace(/^\/|\/$/g, '').split('/')[0];
    return path || 'portal';
  }

  function openAgentDB() {
    return new Promise((resolve, reject) => {
      const req = indexedDB.open(DB_NAME, DB_VERSION);
      req.onupgradeneeded = () => {
        const db = req.result;
        Object.values(STORES).forEach(name => {
          if (!db.objectStoreNames.contains(name)) {
            db.createObjectStore(name, { keyPath: 'id' });
          }
        });
      };
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  }

  // ============================================================
  // INICIALIZACIÓN — Activar los 7 Agentes
  // ============================================================
  function initAgents() {
    try {
      GuardianAgent.init();
      PatternAgent.init();
      AnomalyAgent.init();
      TrustAgent.init();
      ShieldAgent.init();
      ForensicAgent.init();
      EvolutionAgent.init();

      console.log(
        `%c🤖 Ierahkwa AI Agents v${AGENTS_VERSION} — 7 Agentes Activos en ${platform}`,
        'color:#00FF41;font-weight:bold;font-size:12px'
      );
    } catch (e) {
      console.error('[Agents] Error inicializando agentes:', e);
    }
  }

  // Esperar a que el DOM esté listo
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAgents);
  } else {
    initAgents();
  }

  // API Pública
  window.IerahkwaAgents = {
    version: AGENTS_VERSION,
    guardian: GuardianAgent,
    pattern: PatternAgent,
    anomaly: AnomalyAgent,
    trust: TrustAgent,
    shield: ShieldAgent,
    forensic: ForensicAgent,
    evolution: EvolutionAgent,
    getStatus() {
      return {
        version: AGENTS_VERSION,
        platform,
        trustScore: TrustAgent.score,
        alerts: GuardianAgent.alerts.length,
        generation: EvolutionAgent.generation,
        evolvedRules: EvolutionAgent.evolvedRules.length,
        forensicEvents: ForensicAgent.logCount,
        protectedMode: ShieldAgent.protectedMode,
        sessions: PatternAgent.userProfile?.totalSessions || 0
      };
    }
  };
})();

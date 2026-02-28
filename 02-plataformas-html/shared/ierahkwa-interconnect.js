/**
 * Ierahkwa Interconnect System v1.0.0
 * Connects all 400+ sovereign platforms into a unified mesh
 * Each platform discovers and displays its connections automatically
 */
(function() {
  'use strict';

  // ═══════════════════════════════════════════════════
  // SOVEREIGN INTERCONNECTION MAP
  // Maps each platform to its connected platforms by data flow
  // ═══════════════════════════════════════════════════

  const NEXUS = {
    orbital:         { color: '#00bcd4', icon: '🛰️', label: 'NEXUS Orbital',         path: '/nexus-orbital/' },
    escudo:          { color: '#f44336', icon: '🛡️', label: 'NEXUS Escudo',          path: '/nexus-escudo/' },
    cerebro:         { color: '#7c4dff', icon: '🧠', label: 'NEXUS Cerebro',         path: '/nexus-cerebro/' },
    tesoro:          { color: '#ffd600', icon: '💰', label: 'NEXUS Tesoro',          path: '/nexus-tesoro/' },
    voces:           { color: '#e040fb', icon: '📢', label: 'NEXUS Voces',           path: '/nexus-voces/' },
    consejo:         { color: '#1565c0', icon: '🏛️', label: 'NEXUS Consejo',         path: '/nexus-consejo/' },
    tierra:          { color: '#43a047', icon: '🌿', label: 'NEXUS Tierra',          path: '/nexus-tierra/' },
    forja:           { color: '#00e676', icon: '⚒️', label: 'NEXUS Forja',           path: '/nexus-forja/' },
    urbe:            { color: '#ff9100', icon: '🏙️', label: 'NEXUS Urbe',            path: '/nexus-urbe/' },
    raices:          { color: '#00FF41', icon: '🌱', label: 'NEXUS Raíces',          path: '/nexus-raices/' },
    salud:           { color: '#FF5722', icon: '❤️', label: 'NEXUS Salud',           path: '/nexus-salud/' },
    academia:        { color: '#9C27B0', icon: '🎓', label: 'NEXUS Academia',        path: '/nexus-academia/' },
    escolar:         { color: '#1E88E5', icon: '📚', label: 'NEXUS Escolar',         path: '/nexus-escolar/' },
    entretenimiento: { color: '#E91E63', icon: '🎮', label: 'NEXUS Entretenimiento', path: '/nexus-entretenimiento/' },
    amparo:          { color: '#607D8B', icon: '🤲', label: 'NEXUS Amparo',          path: '/nexus-amparo/' },
    escritorio:      { color: '#00e676', icon: '💻', label: 'NEXUS Escritorio',      path: '/nexus-escritorio/' },
    comercio:        { color: '#ff9100', icon: '🛒', label: 'NEXUS Comercio',        path: '/nexus-comercio/' }
  };

  // Core backbone systems that everything connects to
  const BACKBONE = [
    { key: 'blockchain-soberana',        icon: '⛓️', label: 'Blockchain MameyNode', nexus: 'tesoro' },
    { key: 'identidad-digital-soberana', icon: '🪪', label: 'Identidad Digital',    nexus: 'consejo' },
    { key: 'seguridad-soberana',         icon: '🔒', label: 'Seguridad Soberana',   nexus: 'escudo' },
    { key: 'ierahkwa-ai',               icon: '🤖', label: 'IA Soberana',          nexus: 'cerebro' },
    { key: 'gateway-soberano',           icon: '🚪', label: 'API Gateway',          nexus: 'forja' },
    { key: 'base-datos-soberana',        icon: '🗄️', label: 'Base de Datos',        nexus: 'forja' }
  ];

  // Interconnection flows: grouped by ecosystem
  const FLOWS = {
    // ══ FINANCIAL FLOW ══
    finanzas: {
      label: '💰 Flujo Financiero',
      color: '#ffd600',
      platforms: [
        'bdet-bank', 'bdet-wallet', 'fintech-soberano', 'crypto-exchange',
        'defi-soberano', 'dex-soberano', 'staking-soberano', 'pagos-soberano',
        'payment-gateway-soberano', 'contabilidad-soberana', 'nomina-soberana',
        'sovereign-payments', 'trading-bot-soberano', 'trading-dashboard',
        'portfolio-soberano', 'tax-crypto-soberano', 'crowdfunding-soberano',
        'seguros-soberano', 'erp-soberano', 'tokenizacion-soberana',
        'launchpad-soberano', 'layer2-soberano'
      ]
    },
    // ══ BLOCKCHAIN FLOW ══
    blockchain: {
      label: '⛓️ Flujo Blockchain',
      color: '#7c4dff',
      platforms: [
        'blockchain-soberana', 'blockchain-explorer', 'smart-contracts-soberano',
        'defi-soberano', 'dex-soberano', 'dao-soberano', 'bridge-soberano',
        'oracle-soberano', 'storage-descentralizado-soberano', 'layer2-soberano',
        'staking-soberano', 'dapps-soberano', 'nft-marketplace-soberano',
        'indexador-soberano', 'onchain-analytics-soberano', 'nodo-soberano',
        'tokenizacion-soberana', 'launchpad-soberano', 'web-descentralizada-soberana',
        'zk-identidad-soberana', 'cripto-soberana', 'certificaciones-nft'
      ]
    },
    // ══ GOVERNMENT FLOW ══
    gobierno: {
      label: '🏛️ Flujo Gobierno',
      color: '#1565c0',
      platforms: [
        'gobierno-digital-soberano', 'democracia-liquida-soberana',
        'comision-electoral-soberana', 'justicia-digital-soberana',
        'justicia-restaurativa-soberana', 'registro-civil-soberano',
        'catastro-soberano', 'diplomacia-soberana', 'licencias-permisos-soberano',
        'notificaciones-gobierno', 'impuestos-soberano', 'transparencia-soberana',
        'inmigracion-soberana', 'auditoria-soberana', 'datos-abiertos'
      ]
    },
    // ══ DEFENSE FLOW ══
    defensa: {
      label: '🛡️ Flujo Defensa',
      color: '#f44336',
      platforms: [
        'ciberdefensa-soberana', 'ciberseguridad-soberana', 'inteligencia-soberana',
        'seguridad-soberana', 'vpn-soberana', 'vigilancia-soberana',
        'ejercito-soberano', 'criptografia-militar-soberana',
        'logistica-militar-soberana', 'drones-soberanos',
        'centro-operaciones-soberano', 'radar-soberano'
      ]
    },
    // ══ TELECOM FLOW ══
    telecom: {
      label: '📡 Flujo Telecomunicaciones',
      color: '#00bcd4',
      platforms: [
        'telecom-soberano', 'voip-soberano', 'callcenter-soberano',
        'conferencia-soberana', 'mensajeria-soberana', 'correo-soberano',
        'correo-postal-soberano', 'streaming-estudio-soberano',
        'radio-soberana', 'mesh-soberana', 'satelite-comunicaciones-soberano'
      ]
    },
    // ══ HEALTH FLOW ══
    salud: {
      label: '❤️ Flujo Salud',
      color: '#FF5722',
      platforms: [
        'telemedicina-soberana', 'healthcare-dashboard', 'farmacia-soberana',
        'hospital-soberano', 'laboratorio-soberano', 'salud-mental-soberana',
        'nutricion-soberana', 'emergencias-soberano', 'ambulancia-soberana'
      ]
    },
    // ══ EDUCATION FLOW ══
    educacion: {
      label: '🎓 Flujo Educación',
      color: '#9C27B0',
      platforms: [
        'education-dashboard', 'universidad-soberana', 'escuela-soberana',
        'capacitacion-soberana', 'biblioteca-soberana', 'biblioteca-ancestral',
        'calificaciones-soberana', 'becas-soberana', 'investigacion-soberana',
        'archivo-linguistico-soberano', 'sabiduria-soberana'
      ]
    },
    // ══ TECH/DEV FLOW ══
    tecnologia: {
      label: '⚒️ Flujo Tecnología',
      color: '#00e676',
      platforms: [
        'dev-soberano', 'dev-platform', 'devops-soberano', 'devops-pipeline',
        'runtime-soberano', 'sdk-soberano', 'gateway-soberano',
        'microservicios-soberano', 'orm-soberano', 'ui-framework-soberano',
        'testing-soberano', 'bigdata-soberano', 'buscador-soberano',
        'streaming-datos-soberano', 'cms-soberano', 'base-datos-soberana',
        'analitica-soberana', 'automatizacion-soberana', 'virtualizacion-soberana'
      ]
    },
    // ══ COMMERCE FLOW ══
    comercio: {
      label: '🛒 Flujo Comercio',
      color: '#ff9100',
      platforms: [
        'comercio-soberano', 'sovereign-marketplace', 'artisan-market',
        'supply-chain-soberano', 'sovereign-logistics', 'marketing-soberano',
        'payment-gateway-soberano', 'nft-marketplace-soberano',
        'mercado-datos-soberano', 'erp-soberano', 'crm-soberano'
      ]
    },
    // ══ CULTURE FLOW ══
    cultura: {
      label: '🌱 Flujo Cultura',
      color: '#00FF41',
      platforms: [
        'ceremonia-virtual-soberana', 'sabiduria-soberana', 'archivo-linguistico-soberano',
        'biblioteca-ancestral', 'artisan-market', 'traduccion-soberana',
        'adn-ancestral-soberano', 'museo-soberano', 'patrimonio-soberano',
        'musica-soberana', 'cine-soberano', 'podcast-soberano'
      ]
    },
    // ══ AI/QUANTUM FLOW ══
    inteligencia: {
      label: '🧠 Flujo IA/Quantum',
      color: '#7c4dff',
      platforms: [
        'ai-agent-dev', 'bci-soberano', 'robotica-soberana',
        'automatizacion-soberana', 'oraculo-climatico-soberano',
        'analitica-soberana', 'onchain-analytics-soberano',
        'bigdata-soberano', 'buscador-soberano'
      ]
    },
    // ══ SPACE FLOW ══
    espacio: {
      label: '🛰️ Flujo Espacial',
      color: '#00bcd4',
      platforms: [
        'programa-espacial-soberano', 'estacion-terrena-soberana',
        'observacion-terrestre-soberana', 'satelite-comunicaciones-soberano',
        'aviacion-soberana'
      ]
    },
    // ══ SOCIAL/WELFARE FLOW ══
    social: {
      label: '🤲 Flujo Social',
      color: '#607D8B',
      platforms: [
        'social-welfare', 'sovereign-social', 'discapacidad-soberana',
        'alimentacion-soberana', 'pension-soberana', 'correccional-soberano',
        'vivienda-soberana', 'refugio-soberano', 'asistencia-soberana'
      ]
    }
  };

  // ═══════════════════════════════════════════════════
  // DETECT CURRENT PLATFORM
  // ═══════════════════════════════════════════════════
  function getCurrentPlatform() {
    var path = window.location.pathname;
    var parts = path.split('/').filter(function(p) { return p.length > 0; });
    // Find the platform directory name
    for (var i = parts.length - 1; i >= 0; i--) {
      if (parts[i] !== 'index.html' && parts[i] !== '02-plataformas-html') {
        return parts[i];
      }
    }
    return null;
  }

  // ═══════════════════════════════════════════════════
  // FIND CONNECTIONS FOR CURRENT PLATFORM
  // ═══════════════════════════════════════════════════
  function findConnections(platformKey) {
    var connections = { flows: [], backbone: BACKBONE, nexus: [] };
    if (!platformKey) return connections;

    var seenNexus = {};
    Object.keys(FLOWS).forEach(function(flowKey) {
      var flow = FLOWS[flowKey];
      if (flow.platforms.indexOf(platformKey) !== -1) {
        // This platform belongs to this flow
        var related = flow.platforms.filter(function(p) { return p !== platformKey; });
        connections.flows.push({
          key: flowKey,
          label: flow.label,
          color: flow.color,
          platforms: related.slice(0, 8) // Show max 8 related per flow
        });
      }
    });

    return connections;
  }

  // ═══════════════════════════════════════════════════
  // RENDER INTERCONNECTION PANEL
  // ═══════════════════════════════════════════════════
  function renderPanel(connections, currentPlatform) {
    if (connections.flows.length === 0) return;

    var main = document.querySelector('main') || document.body;
    var footer = document.querySelector('footer');

    var panel = document.createElement('div');
    panel.className = 'section';
    panel.id = 'interconnections';
    panel.setAttribute('style', 'margin-top:2rem');

    var html = '<h2 style="text-align:center;margin-bottom:.5rem"><span aria-hidden="true">🔗</span> Interconexiones del Ecosistema</h2>';
    html += '<div style="text-align:center;color:var(--txt2);font-size:.8rem;margin-bottom:1.5rem">Conexiones automáticas con las 400+ plataformas soberanas</div>';

    // Backbone
    html += '<div style="display:flex;flex-wrap:wrap;gap:.5rem;justify-content:center;margin-bottom:1.5rem">';
    connections.backbone.forEach(function(b) {
      html += '<a href="../' + b.key + '/" style="background:rgba(255,255,255,.05);border:1px solid rgba(255,255,255,.1);border-radius:8px;padding:.4rem .8rem;text-decoration:none;color:var(--txt1);font-size:.72rem;display:flex;align-items:center;gap:.3rem;transition:all .2s" onmouseover="this.style.borderColor=\'var(--accent)\'" onmouseout="this.style.borderColor=\'rgba(255,255,255,.1)\'">';
      html += '<span aria-hidden="true">' + b.icon + '</span> ' + b.label + '</a>';
    });
    html += '</div>';

    // Flow connections
    connections.flows.forEach(function(flow) {
      html += '<div style="margin-bottom:1.2rem">';
      html += '<h4 style="font-size:.82rem;margin-bottom:.5rem;color:' + flow.color + '">' + flow.label + '</h4>';
      html += '<div style="display:flex;flex-wrap:wrap;gap:.4rem">';
      flow.platforms.forEach(function(p) {
        var name = p.replace(/-/g, ' ').replace(/soberan[oa]s?/gi, '').replace(/sovereign/gi, '').trim();
        name = name.charAt(0).toUpperCase() + name.slice(1);
        html += '<a href="../' + p + '/" style="background:rgba(255,255,255,.04);border:1px solid ' + flow.color + '33;border-radius:6px;padding:.3rem .6rem;text-decoration:none;color:var(--txt2);font-size:.68rem;transition:all .2s" onmouseover="this.style.background=\'' + flow.color + '22\';this.style.color=\'#fff\'" onmouseout="this.style.background=\'rgba(255,255,255,.04)\';this.style.color=\'var(--txt2)\'">';
        html += name + '</a>';
      });
      html += '</div></div>';
    });

    // Stats
    html += '<div style="text-align:center;margin-top:1.5rem;padding-top:1rem;border-top:1px solid rgba(255,255,255,.08)">';
    html += '<span style="font-size:.7rem;color:var(--txt2)">🏛️ Ecosistema Ierahkwa Ne Kanienke — ';
    html += '400+ plataformas · 17 NEXUS · 13 flujos de datos · 72M personas · 19 naciones</span>';
    html += '</div>';

    panel.innerHTML = html;

    if (footer) {
      main.insertBefore(panel, footer);
    } else {
      main.appendChild(panel);
    }
  }

  // ═══════════════════════════════════════════════════
  // INIT
  // ═══════════════════════════════════════════════════
  function init() {
    var platform = getCurrentPlatform();
    if (!platform || platform === 'shared' || platform === 'icons') return;

    var connections = findConnections(platform);
    if (connections.flows.length > 0) {
      if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() { renderPanel(connections, platform); });
      } else {
        renderPanel(connections, platform);
      }
    }
    console.log('[ierahkwa-interconnect] ' + platform + ' → ' + connections.flows.length + ' flujos conectados');
  }

  // Export
  window.IerahkwaInterconnect = {
    flows: FLOWS,
    nexus: NEXUS,
    backbone: BACKBONE,
    getCurrentPlatform: getCurrentPlatform,
    findConnections: findConnections
  };

  init();
})();

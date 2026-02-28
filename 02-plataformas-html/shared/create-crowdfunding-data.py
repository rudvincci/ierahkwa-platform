#!/usr/bin/env python3
"""
create-crowdfunding-data.py
Genera 2 plataformas Pattern B:
  1. crowdfunding-soberano (Tesoro #ffd600)
  2. mercado-datos-soberano (Cerebro #7c4dff)
"""

import os, pathlib

BASE = pathlib.Path(__file__).resolve().parent.parent  # 02-plataformas-html/

PLATFORMS = [
    # 1. Crowdfunding Soberano
    {
        "slug": "crowdfunding-soberano",
        "title": "Crowdfunding Soberano",
        "subtitle": "Financiamiento Colectivo Descentralizado con Gobernanza",
        "icon": "\U0001f91d",
        "accent": "#ffd600",
        "description": (
            "Plataforma de crowdfunding descentralizado que reemplaza GoFundMe, "
            "Kickstarter e Indiegogo. Financiamiento colectivo con smart contracts "
            "que garantizan transparencia total, milestone-based funding con liberaci\u00f3n "
            "escalonada, 4 modalidades (donaci\u00f3n, recompensa, equity, deuda), regulaci\u00f3n "
            "soberana integrada con marco normativo de las 19 naciones, auditor\u00eda "
            "autom\u00e1tica, KYC descentralizado y protecci\u00f3n al inversionista."
        ),
        "metrics": [
            ("4", "Modalidades"),
            ("Smart", "Contracts"),
            ("Milestone", "Based"),
            ("Soberana", "Regulaci\u00f3n"),
            ("Auto", "Auditor\u00eda"),
            ("Desc.", "KYC"),
        ],
        "arch": [
            ('var(--accent)', 'CREADOR', 'Proyecto \u00b7 Hitos \u00b7 Presupuesto \u00b7 Pitch', 'KYC verificado \u00b7 Equity \u00b7 Deuda \u00b7 Donaci\u00f3n \u00b7 Recompensa'),
            ('#00bcd4',       'SMART CONTRACT (ESCROW)', 'Multi-sig \u00b7 Milestone gates \u00b7 Refund rules', 'Fondos bloqueados \u00b7 Votaci\u00f3n backers \u00b7 Release parcial'),
            ('#7c4dff',       'REGULACI\u00d3N SOBERANA', 'Compliance \u00b7 L\u00edmites \u00b7 Normativa 19 naciones', 'Auditor\u00eda IA \u00b7 Alertas \u00b7 Dashboard regulador'),
            ('var(--accent)', 'LIBERACI\u00d3N POR MILESTONES', 'Verificaci\u00f3n \u00b7 Aprobaci\u00f3n \u00b7 Distribuci\u00f3n', 'Escalonada \u00b7 Transparente \u00b7 Reembolso auto'),
        ],
        "cards": [
            ("\U0001f4dc", "Crowdfunding con Smart Contracts",
             "Fondos bloqueados en contratos inteligentes con liberaci\u00f3n por milestones. Transparencia total: cada WAMPUM rastreable en blockchain. Sin intermediarios."),
            ("\U0001f3af", "Milestone-Based Release",
             "Liberaci\u00f3n escalonada de fondos seg\u00fan hitos verificables. Votaci\u00f3n de backers para aprobar cada milestone. Reembolso autom\u00e1tico si no se cumple."),
            ("\u2696\ufe0f", "Marco Normativo Soberano",
             "Regulaci\u00f3n integrada con normativas de las 19 naciones. Compliance autom\u00e1tico, l\u00edmites de inversi\u00f3n configurables por jurisdicci\u00f3n, reportes regulatorios generados autom\u00e1ticamente."),
            ("\U0001f500", "4 Modalidades de Campa\u00f1a",
             "Donaci\u00f3n pura, recompensa (perks), equity tokenizado, y deuda (pr\u00e9stamos P2P). Cada modalidad con su propio framework legal soberano."),
            ("\U0001f916", "Auditor\u00eda Autom\u00e1tica con IA",
             "IA que audita cada campa\u00f1a: verifica identidad del creador, analiza viabilidad del proyecto, detecta fraude, genera score de confianza. Publicada en blockchain."),
            ("\U0001f6e1\ufe0f", "Protecci\u00f3n al Inversionista",
             "Seguro descentralizado que protege hasta el 80% de la inversi\u00f3n. Escrow multi-sig, dispute resolution DAO, blacklist de actores maliciosos."),
            ("\U0001f510", "KYC Descentralizado",
             "Verificaci\u00f3n de identidad sin enviar documentos a terceros. Zero-knowledge proofs para cumplir regulaci\u00f3n sin sacrificar privacidad."),
            ("\U0001fa99", "Equity Tokenizado",
             "Inversi\u00f3n en equity representada como tokens ERC-20 soberanos. Dividendos autom\u00e1ticos, derechos de voto proporcionales, mercado secundario en DEX."),
            ("\U0001f4ca", "Dashboard de Regulador",
             "Panel para reguladores de las 19 naciones: estad\u00edsticas, alertas de riesgo, capacidad de pausar campa\u00f1as sospechosas, reportes de compliance."),
            ("\U0001f3ea", "Marketplace de Proyectos",
             "Cat\u00e1logo de proyectos por categor\u00eda, regi\u00f3n y naci\u00f3n. Filtros por etapa, rendimiento esperado, sector (agricultura, tecnolog\u00eda, cultura, salud)."),
        ],
        "apis": [
            ("POST", "/api/v1/crowdfunding/campaign",  "Crear campa\u00f1a de crowdfunding. Params: title, description, goal, modality, milestones."),
            ("POST", "/api/v1/crowdfunding/invest",     "Invertir en campa\u00f1a. Params: campaign_id, amount, modality."),
            ("GET",  "/api/v1/crowdfunding/campaigns",  "Listar campa\u00f1as activas con filtros por naci\u00f3n, categor\u00eda y modalidad."),
            ("POST", "/api/v1/crowdfunding/milestone",  "Verificar y aprobar milestone. Params: campaign_id, milestone_id, vote."),
            ("GET",  "/api/v1/crowdfunding/regulate",   "Panel regulador con estad\u00edsticas, alertas y acciones de compliance."),
            ("GET",  "/api/v1/crowdfunding/audit",      "Auditor\u00eda IA de campa\u00f1a con score de confianza y an\u00e1lisis de riesgo."),
        ],
        "db_stores": ["crowdfunding-campaigns", "investor-positions", "regulation-records"],
    },
    # 2. Mercado de Datos Soberano
    {
        "slug": "mercado-datos-soberano",
        "title": "Mercado de Datos Soberano",
        "subtitle": "Marketplace de Datos Descentralizado con Regulaci\u00f3n",
        "icon": "\U0001f4ca",
        "accent": "#7c4dff",
        "description": (
            "Marketplace de datos descentralizado que reemplaza AWS Data Exchange, "
            "Snowflake Marketplace y Databricks. Compra-venta de datasets con soberan\u00eda "
            "total de datos, smart contracts de licencia, privacidad diferencial, "
            "anonimizaci\u00f3n autom\u00e1tica, compliance con marco normativo soberano de "
            "protecci\u00f3n de datos, auditor\u00eda de uso, monetizaci\u00f3n de datos comunitarios "
            "para las 19 naciones."
        ),
        "metrics": [
            ("19", "Naciones"),
            ("Diferencial", "Privacidad"),
            ("Smart", "Contracts"),
            ("Soberana", "Regulaci\u00f3n"),
            ("Uso", "Auditor\u00eda"),
            ("Auto", "Anonimizaci\u00f3n"),
        ],
        "arch": [
            ('var(--accent)', 'PROVEEDOR DE DATOS', 'Upload \u00b7 Schema \u00b7 Metadata \u00b7 Pricing', 'Datasets clim\u00e1ticos \u00b7 Agr\u00edcolas \u00b7 Ling\u00fc\u00edsticos \u00b7 Econ\u00f3micos'),
            ('#00bcd4',       'ANONIMIZACI\u00d3N + QUALITY', 'PII removal \u00b7 K-anonymity \u00b7 Quality score', 'Privacidad diferencial \u00b7 l-diversity \u00b7 t-closeness'),
            ('#ffd600',       'MARKETPLACE', 'Cat\u00e1logo \u00b7 B\u00fasqueda \u00b7 Preview \u00b7 Compra', 'Smart contracts licencia \u00b7 Royalties \u00b7 DEX secundario'),
            ('var(--accent)', 'REGULACI\u00d3N SOBERANA', 'Compliance \u00b7 Auditor\u00eda uso \u00b7 Alertas', 'Marco normativo 19 naciones \u00b7 GDPR+ \u00b7 Bloqueo datasets'),
        ],
        "cards": [
            ("\U0001f5c2\ufe0f", "Marketplace de Datasets",
             "Cat\u00e1logo de datos: clim\u00e1ticos, agr\u00edcolas, econ\u00f3micos, demogr\u00e1ficos, culturales, ling\u00fc\u00edsticos. Cada dataset con preview, schema, calidad score y precio en WAMPUM."),
            ("\U0001f4dc", "Smart Contracts de Licencia",
             "Licencias de uso definidas en smart contracts: tiempo limitado, uso espec\u00edfico, sublicencia prohibida. Revocaci\u00f3n autom\u00e1tica al expirar. Royalties perpetuos para el productor."),
            ("\u2696\ufe0f", "Marco Normativo de Datos",
             "Regulaci\u00f3n soberana de datos integrada: qu\u00e9 datos se pueden vender, l\u00edmites de uso, protecci\u00f3n de datos sensibles ind\u00edgenas, consentimiento informado obligatorio. Cumple GDPR + leyes soberanas."),
            ("\U0001f512", "Privacidad Diferencial Nativa",
             "Datos sensibles protegidos con privacidad diferencial autom\u00e1tica. Ruido calibrado que preserva utilidad estad\u00edstica sin exponer individuos. Configurable por regulador."),
            ("\U0001fae5", "Anonimizaci\u00f3n Autom\u00e1tica",
             "Motor de anonimizaci\u00f3n que detecta y remueve PII autom\u00e1ticamente: nombres, ubicaciones, identificadores. K-anonymity, l-diversity, t-closeness integrados."),
            ("\U0001f50d", "Auditor\u00eda de Uso",
             "Cada acceso a datos registrado en blockchain: qui\u00e9n, cu\u00e1ndo, qu\u00e9, para qu\u00e9. Reguladores pueden auditar uso en tiempo real. Alertas de uso indebido."),
            ("\U0001f33e", "Monetizaci\u00f3n Comunitaria",
             "Las comunidades ind\u00edgenas monetizan sus datos colectivamente: datos agr\u00edcolas, clim\u00e1ticos, ling\u00fc\u00edsticos. Ingresos distribuidos equitativamente via DAO comunitario."),
            ("\u2b50", "Data Quality Scoring",
             "IA que eval\u00faa calidad de datos: completitud, frescura, consistencia, precisi\u00f3n. Score publicado, datasets de baja calidad marcados. Incentivos para mejorar calidad."),
            ("\U0001f4ca", "Panel de Regulador",
             "Dashboard para reguladores de datos de las 19 naciones: estad\u00edsticas de mercado, alertas de violaci\u00f3n, capacidad de bloquear datasets, reportes de compliance autom\u00e1ticos."),
            ("\U0001f517", "API Federation",
             "APIs federadas para que investigadores, universidades y gobiernos consulten datos sin descargarlos. Computation-to-data: el algoritmo va a los datos, no los datos al algoritmo."),
        ],
        "apis": [
            ("GET",  "/api/v1/data-market/datasets",  "Listar datasets disponibles con filtros por categor\u00eda, naci\u00f3n y calidad."),
            ("POST", "/api/v1/data-market/purchase",   "Comprar acceso a dataset. Params: dataset_id, license_type, duration."),
            ("GET",  "/api/v1/data-market/license",    "Verificar licencia activa. Params: license_id, dataset_id."),
            ("POST", "/api/v1/data-market/publish",    "Publicar dataset. Params: schema, metadata, pricing, privacy_level."),
            ("GET",  "/api/v1/data-market/regulate",   "Panel regulador con estad\u00edsticas y acciones de compliance."),
            ("GET",  "/api/v1/data-market/audit",      "Auditor\u00eda de uso de datos con trazabilidad completa en blockchain."),
        ],
        "db_stores": ["data-listings", "purchase-licenses", "audit-trail"],
    },
]


def build_html(p):
    slug      = p["slug"]
    title     = p["title"]
    subtitle  = p["subtitle"]
    icon      = p["icon"]
    accent    = p["accent"]
    desc      = p["description"]
    metrics   = p["metrics"]
    arch      = p["arch"]
    cards     = p["cards"]
    apis      = p["apis"]
    db_stores = p["db_stores"]

    seo_desc = f"{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total."
    tw_desc  = f"{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle}."

    # metrics HTML
    stats_html = ""
    for val, lbl in metrics:
        stats_html += f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>\n'

    # architecture diagram
    arch_lines = []
    for i, (color, layer_name, line1, line2) in enumerate(arch):
        pad = max(1, 44 - len(layer_name))
        arch_lines.append(f'<span style="color:{color}">\u250c\u2500 {layer_name} {"\u2500" * pad}\u2510</span>')
        arch_lines.append(f'\u2502  {line1:<53}\u2502')
        arch_lines.append(f'\u2502  {line2:<53}\u2502')
        if i < len(arch) - 1:
            arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2534\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
            arch_lines.append('                   \u2502')
        else:
            arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
    arch_html = "\n".join(arch_lines)

    # feature cards
    cards_html = ""
    for c_icon, c_title, c_desc in cards:
        cards_html += f'<article class="card">\n<div class="card-icon" aria-hidden="true">{c_icon}</div>\n<h4>{c_title}</h4>\n<p>{c_desc}</p>\n</article>\n'

    # API endpoints
    api_html = ""
    for method, path, desc_api in apis:
        color = accent if method == "POST" else "#00FF41"
        api_html += f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{path}</code></div>\n'
        api_html += f'<p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{desc_api}</p>\n'

    # db store names for JS
    stores_js = ", ".join(f'"{s}"' for s in db_stores)

    html = f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{seo_desc}">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta property="og:description" content="{seo_desc}">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta name="twitter:description" content="{tw_desc}">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} \u2014 Ierahkwa Ne Kanienke</title>
<style>:root{{--accent:{accent}}}</style>
</head>
<body role="document">
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header>
<div class="logo"><div class="logo-icon" aria-hidden="true">{icon}</div><h1>{title}</h1></div>
<nav aria-label="Navegacion principal">
<a href="#dashboard" aria-current="page">Dashboard</a>
<a href="#features">Modulos</a>
<a href="#api">API</a>
<a href="#pricing">Precios</a>
</nav>
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">\u269b\ufe0f</span> Quantum-Safe</span>
</header>

<main id="main">

<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar M\u00f3dulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<div class="stats" role="list" aria-label="Metricas del sistema">
{stats_html}</div>

<div class="section" id="architecture">
<h2><span aria-hidden="true">\U0001f3d7\ufe0f</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}
</div>
</div>

<div class="section-title" id="features">
<h3>M\u00f3dulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>

<div class="grid">
{cards_html}</div>

<div class="section" id="api">
<h2><span aria-hidden="true">\U0001f50c</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">
{api_html}</div>
</div>

<div class="section-title" id="pricing">
<h3>Planes Soberanos</h3>
<p>Empieza gratis. Escala soberanamente.</p>
</div>

<div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(220px,1fr))">
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Guerrero</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">0 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 100 operaciones/mes</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Dashboard b\u00e1sico</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 1 proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte comunidad</li>
</ul>
</div>
<div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Ilimitado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Analytics avanzados</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 API completa</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte prioritario</li>
</ul>
</div>
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Naci\u00f3n</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-naci\u00f3n</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 SLA 99.99%</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Auditor dedicado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte 24/7</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Custom integrations</li>
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">200+ plataformas soberanas &middot; 15 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">\U0001f6e1\ufe0f</span> Seguro</span></div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script>
(function(){{
  var DB_NAME='ierahkwa-{slug}';
  var DB_VER=1;
  var STORES=[{stores_js}];
  var db=null;
  function openDB(){{
    return new Promise(function(resolve,reject){{
      var req=indexedDB.open(DB_NAME,DB_VER);
      req.onupgradeneeded=function(){{
        var d=req.result;
        STORES.forEach(function(s){{
          if(!d.objectStoreNames.contains(s))d.createObjectStore(s,{{keyPath:'id'}})
        }});
      }};
      req.onsuccess=function(){{db=req.result;resolve(db)}};
      req.onerror=function(){{reject(req.error)}}
    }})
  }}
  function showOfflineBanner(show){{
    var b=document.getElementById('offline-banner');
    if(!b){{
      b=document.createElement('div');
      b.id='offline-banner';
      b.style.cssText='position:fixed;bottom:0;left:0;right:0;background:var(--accent);color:#09090d;text-align:center;padding:8px;font-size:13px;font-weight:700;z-index:9999;transform:translateY(100%);transition:transform .3s';
      b.textContent='Modo Offline \u2014 Datos y operaciones pendientes disponibles offline.';
      document.body.appendChild(b)
    }}
    b.style.transform=show?'translateY(0)':'translateY(100%)'
  }}
  function init(){{
    openDB().then(function(){{
      window.addEventListener('online',function(){{showOfflineBanner(false)}});
      window.addEventListener('offline',function(){{showOfflineBanner(true)}});
      if(!navigator.onLine)showOfflineBanner(true);
      console.log('[{slug}] Offline module ready')
    }})
  }}
  init()
}})();
</script>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body>
</html>'''
    return html


def main():
    created = []
    for p in PLATFORMS:
        out_dir = BASE / p["slug"]
        out_dir.mkdir(parents=True, exist_ok=True)
        html = build_html(p)
        out_file = out_dir / "index.html"
        out_file.write_text(html, encoding="utf-8")
        lines = html.count("\n") + 1
        created.append((p["slug"], lines))
        print(f"  OK  {p['slug']}/index.html  ({lines} lineas)")

    print(f"\n  Total: {len(created)} plataformas generadas.")
    for slug, lines in created:
        status = "OK" if lines >= 180 else "WARN < 180 lineas"
        print(f"    {slug}: {lines} lineas -- {status}")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""
create-final-crypto.py — Genera 3 plataformas crypto finales para Ierahkwa Ne Kanienke.
Plataformas: launchpad-soberano, onchain-analytics-soberano, nft-marketplace-soberano
Pattern B: ~200-260 líneas, SEO, header+nav+quantum, hero, stats, architecture, 10 cards,
6 APIs, 3 pricing plans, footer, 4 scripts, offline IndexedDB, SW registration.
"""

import os, textwrap

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

PLATFORMS = [
    {
        "slug": "launchpad-soberano",
        "title": "Launchpad Soberano",
        "subtitle": "Plataforma de Lanzamiento de Tokens y Proyectos",
        "icon": "\U0001F680",  # 🚀
        "accent": "#ffd600",
        "description": "Launchpad soberano que reemplaza Binance Launchpad, Pump.fun y PinkSale. Lanzamiento de tokens WAMPUM con vesting schedules, IDO/IEO/ILO soberanos, KYC descentralizado, whitelist automatizada, liquidity lock, anti-bot, auditoría de smart contracts integrada. Gobernado por las 19 naciones.",
        "metrics": [
            ("100+", "Proyectos"),
            ("IDO/IEO", "ILO"),
            ("Vesting", "Schedules"),
            ("Anti-Bot", "Protección"),
            ("Auditoría", "Contratos"),
            ("Governance", "19 Naciones"),
        ],
        "cards": [
            ("\U0001F3AF", "IDO Launchpad Soberano", "Plataforma de oferta inicial descentralizada para proyectos del ecosistema WAMPUM. Fair launch con distribución equitativa, sin whales dominando, transparencia total on-chain."),
            ("\U0001F4C5", "Vesting Schedules Personalizables", "Configuración flexible de calendarios de vesting: cliff periods, linear unlock, milestone-based release. Contratos inmutables que garantizan cumplimiento automático."),
            ("\U0001F464", "KYC Descentralizado", "Verificación de identidad soberana sin terceros centralizados. Zero-knowledge proofs para privacidad, cumplimiento regulatorio y protección de datos ancestrales."),
            ("\U0001F916", "Whitelist Automatizada con IA", "Sistema de whitelisting inteligente que evalúa participantes con machine learning. Score de reputación soberana, detección de sybils, fairness garantizado."),
            ("\U0001F512", "Liquidity Lock Automático", "Bloqueo automático de liquidez post-launch en contratos auditados. Timelock configurable, multi-sig para unlock de emergencia, transparencia total."),
            ("\U0001F6E1\uFE0F", "Anti-Bot Protection", "Protección avanzada contra bots de sniper y front-running. Cooldown periods, gas limit, captcha soberano y transaction ordering justo."),
            ("\U0001F50D", "Auditoría Smart Contracts", "Motor de auditoría integrado que verifica seguridad de contratos antes del lanzamiento. Análisis estático, fuzzing, verificación formal y reporte público."),
            ("\U0001F527", "Token Generator sin Código", "Crea tokens WAMPUM sin escribir código. Wizard visual con templates para utility tokens, governance tokens, NFTs y tokens de seguridad."),
            ("\U0001F4B0", "Staking para Allocation", "Sistema de staking donde holders de WAMPUM obtienen allocations proporcionales. Más stake = mayor allocation en IDOs/IEOs soberanos."),
            ("\U0001F4CA", "Dashboard de Proyectos", "Panel centralizado con todos los launches activos, métricas de participación, countdown timers, historial de proyectos y rendimiento post-launch."),
        ],
        "apis": [
            ("POST", "/api/v1/launch/create", "Crear nuevo proyecto de lanzamiento. Params: name, token, supply, vesting."),
            ("POST", "/api/v1/launch/participate", "Participar en un IDO/IEO activo. Params: project_id, amount, wallet."),
            ("GET", "/api/v1/launch/projects", "Lista de proyectos activos, próximos y completados con métricas."),
            ("GET", "/api/v1/launch/schedule", "Calendario de lanzamientos con fechas, allocations y requisitos."),
            ("POST", "/api/v1/launch/audit", "Solicitar auditoría de smart contract. Params: contract_address, chain."),
            ("GET", "/api/v1/launch/allocations", "Consultar allocations del usuario por proyecto y tier de staking."),
        ],
        "db_name": "ierahkwa-launchpad-soberano",
        "db_stores": ["launch-projects", "participant-allocations", "vesting-schedules"],
        "arch": {
            "blocks": [
                ("#ffd600", "PROYECTO", [
                    "Propuesta de token · Tokenomics · Roadmap",
                    "Smart contract · Documentación · Equipo"
                ]),
                ("#7c4dff", "AUDITORÍA + KYC", [
                    "Análisis estático · Fuzzing · Formal verification",
                    "ZK-KYC · Sybil detection · Reputation score"
                ]),
                ("#00bcd4", "IDO/IEO ENGINE", [
                    "Whitelist · Allocation · Fair distribution",
                    "Multi-round · Overflow · Lottery system"
                ]),
                ("#ffd600", "TOKEN DISTRIBUTION", [
                    "Vesting schedules · Cliff · Linear unlock",
                    "Liquidity lock · DEX listing · Price discovery"
                ]),
            ]
        }
    },
    {
        "slug": "onchain-analytics-soberano",
        "title": "On-Chain Analytics Soberano",
        "subtitle": "Análisis Profundo de Blockchain en Tiempo Real",
        "icon": "\U0001F4E1",  # 📡
        "accent": "#7c4dff",
        "description": "Plataforma de analytics on-chain que reemplaza Glassnode, Arkham Intelligence y Nansen. Análisis de flujos de WAMPUM, detección de ballenas, métricas DeFi en tiempo real, wallet labeling, entity clustering, alertas inteligentes, dashboards personalizables, API para investigadores.",
        "metrics": [
            ("Real-Time", "Análisis"),
            ("Whale", "Detection"),
            ("DeFi", "Métricas"),
            ("Wallet", "Labels"),
            ("Entity", "Cluster"),
            ("Custom", "Dashboards"),
        ],
        "cards": [
            ("\U0001F40B", "Whale Tracking en Tiempo Real", "Monitoreo continuo de wallets con grandes holdings de WAMPUM. Alertas instantáneas de movimientos significativos, patrones de acumulación y distribución."),
            ("\U0001F9E9", "Entity Clustering Soberano", "Algoritmos de clustering que agrupan wallets pertenecientes a la misma entidad. Heurísticas on-chain, análisis de patrones temporales y graph analysis."),
            ("\U0001F3F7\uFE0F", "Wallet Labeling Automático", "Clasificación automática de wallets: exchanges, DeFi protocols, DAOs, whales, market makers. Labels verificados por la comunidad soberana."),
            ("\U0001F4C8", "Métricas DeFi Live", "TVL, APY, volumen, impermanent loss, utilización de pools en tiempo real. Comparativas entre protocolos del ecosistema WAMPUM soberano."),
            ("\U0001F30A", "Flow Analysis Multi-Chain", "Seguimiento de flujos de tokens entre chains, bridges y protocolos. Visualización Sankey de movimientos de capital entre ecosistemas soberanos."),
            ("\U0001F514", "Alertas Inteligentes de Mercado", "Sistema de alertas configurables: movimientos de whales, cambios de TVL, liquidaciones, anomalías de precio y volumen inusual."),
            ("\U0001F4CA", "Dashboards Personalizables", "Constructor drag-and-drop de dashboards con widgets de métricas, gráficos, tablas y mapas de calor. Compartibles entre analistas soberanos."),
            ("\U0001F504", "Token Flow Visualization", "Visualización interactiva de flujos de tokens con grafos dirigidos. Zoom temporal, filtros por monto y tipo de transacción."),
            ("\U0001F4B0", "Smart Money Tracking", "Identificación y seguimiento de wallets con historial de rendimiento superior. Copia de estrategias de smart money del ecosistema soberano."),
            ("\U0001F50C", "API para Investigadores", "API REST y WebSocket con datos on-chain en tiempo real. Rate limits generosos para investigadores académicos y analistas soberanos."),
        ],
        "apis": [
            ("GET", "/api/v1/onchain/whales", "Top wallets por balance y actividad reciente. Filtros por token y período."),
            ("GET", "/api/v1/onchain/flows", "Flujos de tokens entre entidades. Params: token, period, min_amount."),
            ("GET", "/api/v1/onchain/metrics", "Métricas agregadas: TVL, volumen, transacciones, fees, active addresses."),
            ("GET", "/api/v1/onchain/wallets", "Información detallada de wallet: balance, historial, labels, score."),
            ("POST", "/api/v1/onchain/alert", "Crear alerta personalizada. Params: condition, threshold, webhook_url."),
            ("GET", "/api/v1/onchain/entities", "Clusters de entidades con wallets asociadas y clasificación."),
        ],
        "db_name": "ierahkwa-onchain-analytics-soberano",
        "db_stores": ["onchain-events", "wallet-labels", "alert-rules"],
        "arch": {
            "blocks": [
                ("#7c4dff", "MAMEYNODE BLOCKCHAIN", [
                    "Bloques en tiempo real · Transacciones · Eventos",
                    "Multi-chain sync · Indexación · Normalización"
                ]),
                ("#00bcd4", "EVENT PARSER", [
                    "Decodificación de logs · ABI registry · Token transfers",
                    "DEX swaps · Lending events · Bridge transfers"
                ]),
                ("#ffd600", "ANALYTICS ENGINE (ML)", [
                    "Entity clustering · Whale detection · Anomaly detection",
                    "Time series · Flow analysis · Predictive models"
                ]),
                ("#7c4dff", "DASHBOARD VISUAL", [
                    "Gráficos interactivos · Mapas de calor · Sankey flows",
                    "Alertas · Reportes · Exportación · API WebSocket"
                ]),
            ]
        }
    },
    {
        "slug": "nft-marketplace-soberano",
        "title": "NFT Marketplace Soberano",
        "subtitle": "Mercado de Arte Digital y Coleccionables Indígenas",
        "icon": "\U0001F3A8",  # 🎨
        "accent": "#e040fb",
        "description": "Marketplace NFT soberano que reemplaza OpenSea, Rarible y Foundation. Mercado de arte digital indígena, coleccionables culturales, música NFT, videos NFT, tierras virtuales, royalties automáticos para artistas, curación por comunidad, minting gasless en WAMPUM, metaverso integration.",
        "metrics": [
            ("10K+", "Artistas"),
            ("0 Gas", "Mint"),
            ("Royalties", "Auto"),
            ("Curación", "Comunidad"),
            ("14", "Idiomas"),
            ("Metaverso", "Gallery"),
        ],
        "cards": [
            ("\u2728", "Minting Gasless en WAMPUM", "Creación de NFTs sin costo de gas para artistas. Meta-transacciones subsidiadas por el protocolo soberano, eliminando barreras de entrada para creadores indígenas."),
            ("\U0001F4B8", "Royalties Automáticos Perpetuos", "Smart contracts que garantizan royalties en cada reventa, para siempre. Porcentaje configurable por el artista, enforcement on-chain sin intermediarios."),
            ("\U0001F3AD", "Arte Digital Indígena", "Galería curada de arte digital de las 19 naciones. Categorías por tradición cultural, técnica artística y nación de origen. Certificado de autenticidad soberano."),
            ("\U0001F48E", "Coleccionables Culturales", "NFTs que representan artefactos culturales, símbolos sagrados y patrimonio digital. Colecciones oficiales de cada nación con metadatos culturales enriquecidos."),
            ("\U0001F3B5", "Música NFT Streaming", "Distribución de música como NFTs con streaming integrado. Los holders pueden escuchar, los artistas reciben royalties directos sin intermediarios discográficos."),
            ("\U0001F3AC", "Video NFT 4K", "Marketplace de video arte y cortometrajes como NFTs. Streaming en 4K, almacenamiento descentralizado en IPFS soberano, certificación de originalidad."),
            ("\U0001F465", "Curación por Comunidad", "Sistema de curación donde la comunidad vota y destaca las mejores obras. Curadores ganan rewards, artistas ganan visibilidad, compradores descubren talento."),
            ("\U0001F525", "Subastas en Tiempo Real", "Motor de subastas con countdown, pujas incrementales, reserve price y buy-now. Subastas inglesas y holandesas con settlement automático on-chain."),
            ("\U0001F4A4", "Lazy Minting", "NFTs que se mintean solo cuando se compran. El artista lista sin costo, el comprador paga el mint, optimizando costos para creadores emergentes."),
            ("\U0001F30D", "Metaverso Gallery Integration", "Galerías virtuales 3D donde exhibir NFTs. Integración con el metaverso soberano, recorridos virtuales y eventos de inauguración en vivo."),
        ],
        "apis": [
            ("POST", "/api/v1/nft/mint", "Mintear nuevo NFT. Params: metadata, media_url, royalty_pct, collection."),
            ("GET", "/api/v1/nft/collection", "Obtener colección con NFTs, floor price, volumen y holders."),
            ("POST", "/api/v1/nft/list", "Listar NFT en venta. Params: token_id, price, auction_type."),
            ("POST", "/api/v1/nft/bid", "Realizar puja en subasta. Params: token_id, bid_amount, wallet."),
            ("GET", "/api/v1/nft/trending", "NFTs y colecciones trending por volumen, ventas y actividad."),
            ("GET", "/api/v1/nft/artist", "Perfil de artista con obras, ventas, royalties acumulados y rating."),
        ],
        "db_name": "ierahkwa-nft-marketplace-soberano",
        "db_stores": ["nft-metadata", "collection-cache", "bid-history"],
        "arch": {
            "blocks": [
                ("#e040fb", "ARTISTA", [
                    "Upload media · Metadata · Royalty config",
                    "Colección · Categoría · Tags culturales"
                ]),
                ("#7c4dff", "MINT (GASLESS)", [
                    "Meta-transaction · IPFS upload · Token URI",
                    "Lazy mint · Batch mint · ERC-721/1155"
                ]),
                ("#00bcd4", "MARKETPLACE LISTING", [
                    "Fixed price · Auction · Bundle · Offer",
                    "Search · Filter · Curación · Featured"
                ]),
                ("#e040fb", "BUYER + ROYALTY DISTRIBUTION", [
                    "Purchase · Settlement · Transfer · Royalty split",
                    "Secondary sales · Perpetual royalties · Analytics"
                ]),
            ]
        }
    },
]


def gen_arch(arch, accent):
    """Generate the architecture ASCII art section."""
    lines = []
    for i, block in enumerate(arch["blocks"]):
        color, name, desc_lines = block
        lines.append(f'<span style="color:{color}">┌─ {name} {"─" * max(1, 45 - len(name))}┐</span>')
        for dl in desc_lines:
            lines.append(f'│  {dl:<48}│')
        lines.append(f'<span style="color:{color}">└{"─" * 18}┬{"─" * 28}┘</span>')
        if i < len(arch["blocks"]) - 1:
            lines.append('                   │')
    # Remove the connector from last block
    lines[-1] = lines[-1].replace('┬', '─')
    return "\n".join(lines)


def gen_html(p):
    slug = p["slug"]
    title = p["title"]
    subtitle = p["subtitle"]
    icon = p["icon"]
    accent = p["accent"]
    desc = p["description"]
    metrics = p["metrics"]
    cards = p["cards"]
    apis = p["apis"]
    db_name = p["db_name"]
    db_stores = p["db_stores"]
    arch_html = gen_arch(p["arch"], accent)

    # Build stats
    stats_html = ""
    for val, lbl in metrics:
        stats_html += f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>\n'

    # Build cards
    cards_html = ""
    for c_icon, c_title, c_desc in cards:
        cards_html += f"""<article class="card">
<div class="card-icon" aria-hidden="true">{c_icon}</div>
<h4>{c_title}</h4>
<p>{c_desc}</p>
</article>
"""

    # Build API endpoints
    api_html = ""
    for method, path, desc_api in apis:
        method_color = "#00FF41" if method == "GET" else accent
        api_html += f"""<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{method_color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{path}</code></div>
<p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{desc_api}</p>
"""

    # Build DB stores JS array
    stores_js = ", ".join(f'"{s}"' for s in db_stores)

    html = f"""<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{title} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cuántico Kyber-768, blockchain MameyNode y soberanía digital total.">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} — Ierahkwa Ne Kanienke">
<meta property="og:description" content="{title} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cuántico Kyber-768, blockchain MameyNode y soberanía digital total.">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} — Ierahkwa Ne Kanienke">
<meta name="twitter:description" content="{title} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle}.">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} — Ierahkwa Ne Kanienke</title>
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
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">⚛️</span> Quantum-Safe</span>
</header>

<main id="main">

<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar Módulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<div class="stats" role="list" aria-label="Metricas del sistema">
{stats_html}</div>

<div class="section" id="architecture">
<h2><span aria-hidden="true">🏗️</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}
</div>
</div>

<div class="section-title" id="features">
<h3>Módulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>

<div class="grid">
{cards_html}</div>

<div class="section" id="api">
<h2><span aria-hidden="true">🔌</span> API Endpoints</h2>
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
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ 100 operaciones/mes</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Dashboard básico</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ 1 proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte comunidad</li>
</ul>
</div>
<div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Ilimitado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Analytics avanzados</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Multi-proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ API completa</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte prioritario</li>
</ul>
</div>
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Nación</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Multi-nación</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ SLA 99.99%</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Auditor dedicado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte 24/7</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Custom integrations</li>
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">200+ plataformas soberanas &middot; 15 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">🛡️</span> Seguro</span></div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script>
(function(){{
  var DB_NAME='{db_name}';
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
      b.textContent='Modo Offline — Datos y operaciones pendientes disponibles offline.';
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
</html>"""
    return html


def main():
    created = []
    for p in PLATFORMS:
        dirpath = os.path.join(BASE, p["slug"])
        os.makedirs(dirpath, exist_ok=True)
        filepath = os.path.join(dirpath, "index.html")
        html = gen_html(p)
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(html)
        line_count = html.count("\n") + 1
        created.append((p["slug"], line_count, len(html)))
        print(f"  [OK] {p['slug']}/index.html — {line_count} líneas, {len(html):,} bytes")

    print(f"\n  {len(created)} plataformas creadas exitosamente.")
    for slug, lines, size in created:
        print(f"    - {slug}: {lines} líneas, {size:,} bytes")


if __name__ == "__main__":
    print("=" * 60)
    print("  create-final-crypto.py — Ierahkwa Ne Kanienke")
    print("  Generando 3 plataformas crypto finales...")
    print("=" * 60)
    main()
    print("=" * 60)
    print("  ¡Completado!")
    print("=" * 60)

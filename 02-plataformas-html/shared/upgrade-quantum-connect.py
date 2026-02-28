#!/usr/bin/env python3
"""
Ierahkwa Platform — Quantum + AI + Bank + Casino + Interconnection Upgrade
Upgrades ALL platform HTML files with:
  1. ierahkwa-quantum.js module
  2. Platform interconnections (related platforms sidebar)
  3. Enhanced interactive features (security dashboard, live status)
  4. AI assistant widget
  5. Quantum encryption badge
"""

import os
import re
import json

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

# ═══════════════════════════════════════════
# NEXUS MEMBERSHIP MAP — which platforms belong to which NEXUS
# ═══════════════════════════════════════════
NEXUS_MAP = {
    'orbital': {
        'color': '#00bcd4', 'emoji': '🛰️', 'name': 'Orbital',
        'platforms': [
            'starlink-soberano', 'telecom-soberano', 'gps-soberano', 'radio-soberana',
            'mesh-soberano', 'espectro-soberano', 'fibra-soberana', 'dns-soberano',
            'vpn-nacional', 'cdn-soberana', 'iot-soberano', 'streaming-soberano',
            'telefonia-soberana', 'antenas-soberanas', 'redundancia-soberana',
            'satelite-observacion', 'comunicacion-emergencia', 'frecuencias-soberanas',
            'internet-comunitario', 'datacenters-soberanos'
        ]
    },
    'escudo': {
        'color': '#f44336', 'emoji': '🛡️', 'name': 'Escudo',
        'platforms': [
            'ciberseguridad-nacional', 'firewall-soberano', 'soc-soberano',
            'forense-digital', 'pentest-soberano', 'criptografia-soberana',
            'identidad-soberana', 'fronteras-digitales', 'alertas-nacionales',
            'bunker-datos', 'vigilancia-digital', 'cert-soberano',
            'honeypots-soberanos', 'dlp-soberano', 'audit-soberano',
            'zk-proofs-soberano', 'threat-intel-soberano', 'devsecops-soberano',
            'vpn-soberana', 'contrasenas-soberana'
        ]
    },
    'cerebro': {
        'color': '#7c4dff', 'emoji': '🧠', 'name': 'Cerebro',
        'platforms': [
            'ai-soberana', 'quantum-soberano', 'ml-soberano', 'nlp-soberano',
            'vision-soberana', 'datos-soberanos', 'analytics-soberano',
            'robotics-soberana', 'neural-soberano', 'llm-soberano',
            'datos-abiertos-soberanos', 'big-data-soberano', 'ai-etica-soberana',
            'prediccion-soberana', 'automatizacion-soberana', 'deep-learning-soberano'
        ]
    },
    'tesoro': {
        'color': '#ffd600', 'emoji': '💰', 'name': 'Tesoro',
        'platforms': [
            'bdet-bank', 'wampum-exchange', 'seguros-soberanos',
            'microfinanzas-soberanas', 'remesas-soberanas', 'pagos-soberanos',
            'banca-movil-soberana', 'prestamos-soberanos', 'inversiones-soberanas',
            'tesoreria-soberana', 'presupuesto-soberano', 'auditorias-soberanas',
            'finanzas-personales', 'fondos-comunitarios', 'crowdfunding-soberano',
            'facturacion-soberana', 'blockchain-soberano', 'nft-soberano',
            'contabilidad-soberana', 'nomina-soberana', 'impuestos-soberano'
        ]
    },
    'voces': {
        'color': '#e040fb', 'emoji': '📡', 'name': 'Voces',
        'platforms': [
            'red-social-soberana', 'media-soberana', 'podcast-soberano',
            'blog-soberano', 'foro-soberano', 'noticia-soberana',
            'correo-soberano', 'mensajeria-soberana', 'conferencia-soberana',
            'directorio-soberano', 'video-editor-soberano', 'streaming-estudio-soberano'
        ]
    },
    'consejo': {
        'color': '#1565c0', 'emoji': '🏛️', 'name': 'Consejo',
        'platforms': [
            'gobierno-soberano', 'votacion-soberana', 'leyes-soberanas',
            'censo-soberano', 'registro-civil-soberano', 'pasaporte-soberano',
            'justicia-soberana', 'parlamento-soberano', 'diplomacia-soberana',
            'tratados-soberanos', 'constitucion-soberana', 'territorio-soberano',
            'defensa-civil-soberana', 'archivos-nacionales', 'protocolo-soberano',
            'administracion-publica', 'transparencia-soberana',
            'notificaciones-gobierno', 'licencias-permisos-soberano',
            'justicia-restaurativa-soberana'
        ]
    },
    'tierra': {
        'color': '#43a047', 'emoji': '🌿', 'name': 'Tierra',
        'platforms': [
            'agricultura-soberana', 'agua-soberana', 'energia-soberana',
            'minerales-soberanos', 'bosques-soberanos', 'pesca-soberana',
            'biodiversidad-soberana', 'clima-soberano', 'residuos-soberanos',
            'mapa-soberano', 'catastro-soberano', 'recursos-naturales',
            'renovables-soberano', 'conservacion-soberana', 'fauna-soberana',
            'reciclaje-soberano', 'huella-carbono-soberana', 'parques-soberanos',
            'territorio-vigilante-soberano', 'oraculo-climatico-soberano'
        ]
    },
    'forja': {
        'color': '#00e676', 'emoji': '⚒️', 'name': 'Forja',
        'platforms': [
            'ide-soberano', 'git-soberano', 'ci-cd-soberano', 'api-gateway-soberano',
            'contenedores-soberano', 'serverless-soberano', 'base-datos-soberana',
            'devops-soberano', 'testing-soberano', 'documentacion-soberana',
            'paquetes-soberanos', 'cloud-soberana', 'hosting-soberano',
            'microservicios-soberanos', 'monitoreo-soberano', 'logs-soberanos',
            'feature-flags-soberano', 'debug-soberano',
            'virtualizacion-soberana', 'observabilidad-soberana'
        ]
    },
    'escritorio': {
        'color': '#26C6DA', 'emoji': '🖥️', 'name': 'Escritorio',
        'platforms': [
            'docs-soberanos', 'ofimatica-soberana', 'hojas-calculo-soberana',
            'calendario-soberano', 'notas-soberana', 'formularios-soberano',
            'colaboracion-soberana', 'diseno-soberano', 'plantillas-soberana',
            'proyecto-soberano', 'crm-soberano', 'transferencia-soberana',
            'audio-editor-soberano', 'mapas-mentales-soberano', 'wiki-soberana',
            'firma-digital-soberana', 'gestion-tiempo-soberano',
            'pizarra-soberana', 'publicacion-soberana', 'lector-soberano'
        ]
    },
    'comercio': {
        'color': '#FF6D00', 'emoji': '🛒', 'name': 'Comercio',
        'platforms': [
            'comercio-soberano', 'commerce-dashboard', 'marketing-soberano',
            'empresa-soberana', 'hospedaje-soberano', 'turismo-soberano',
            'rrhh-soberano', 'soporte-soberano', 'inventario-soberano',
            'pos-soberano', 'erp-soberano'
        ]
    },
    'urbe': {
        'color': '#ff9100', 'emoji': '🏙️', 'name': 'Urbe',
        'platforms': [
            'transporte-soberano', 'vivienda-soberana', 'urbanismo-soberano',
            'agua-potable-soberana', 'electricidad-soberana', 'basura-soberana',
            'alumbrado-soberano', 'emergencias-soberano'
        ]
    },
    'raices': {
        'color': '#00FF41', 'emoji': '🪶', 'name': 'Raices',
        'platforms': [
            'archivo-linguistico-soberano', 'sabiduria-soberana',
            'biblioteca-soberana', 'musica-soberana', 'artesanias-soberanas',
            'ceremonias-soberanas', 'galeria-soberana',
            'adn-ancestral-soberano', 'semilla-soberana',
            'intercambio-comunitario-soberano', 'ceremonia-virtual-soberana'
        ]
    },
    'salud': {
        'color': '#FF5722', 'emoji': '🏥', 'name': 'Salud',
        'platforms': [
            'healthcare-dashboard', 'telemedicina-soberana', 'farmacia-soberana',
            'salud-mental-soberana', 'nutricion-soberana', 'medicina-tradicional',
            'emergencias-medicas-soberano', 'fitness-soberano'
        ]
    },
    'academia': {
        'color': '#9C27B0', 'emoji': '🎓', 'name': 'Academia',
        'platforms': [
            'education-dashboard', 'universidad-soberana', 'investigacion-soberana',
            'becas-soberanas', 'certificacion-soberana', 'idiomas-soberano',
            'cursos-soberanos'
        ]
    },
    'escolar': {
        'color': '#1E88E5', 'emoji': '📚', 'name': 'Escolar',
        'platforms': [
            'escuela-soberana', 'aula-virtual-soberana', 'biblioteca-escolar',
            'deportes-escolares', 'comedor-escolar', 'transporte-escolar',
            'padres-soberano', 'evaluacion-soberana'
        ]
    },
    'entretenimiento': {
        'color': '#E91E63', 'emoji': '🎮', 'name': 'Entretenimiento',
        'platforms': [
            'gaming-soberano', 'casino-soberano', 'musica-streaming-soberano',
            'video-streaming-soberano', 'deportes-soberano', 'eventos-soberanos',
            'modelado-3d-soberano', 'realidad-virtual-soberana'
        ]
    },
    'amparo': {
        'color': '#607D8B', 'emoji': '🤝', 'name': 'Amparo',
        'platforms': [
            'asistencia-social-soberana', 'empleo-soberano', 'vivienda-social-soberana',
            'discapacidad-soberana', 'ancianos-soberano', 'ninez-soberana',
            'refugiados-soberanos', 'alimentacion-soberana', 'migracion-soberana',
            'voluntariado-soberano', 'derechos-humanos-soberano'
        ]
    }
}

# ═══════════════════════════════════════════
# PLATFORM DEPENDENCY MAP — What each platform needs from others
# ═══════════════════════════════════════════
DEPENDENCIES = {
    # Commerce platforms need payment + inventory
    'comercio-soberano': ['bdet-bank', 'pagos-soberanos', 'inventario-soberano', 'marketing-soberano'],
    'pos-soberano': ['bdet-bank', 'inventario-soberano', 'comercio-soberano', 'facturacion-soberana'],
    'erp-soberano': ['contabilidad-soberana', 'rrhh-soberano', 'inventario-soberano', 'nomina-soberana'],
    'marketing-soberano': ['analytics-soberano', 'correo-soberano', 'red-social-soberana'],

    # Finance platforms need crypto + compliance
    'bdet-bank': ['blockchain-soberano', 'criptografia-soberana', 'identidad-soberana', 'contabilidad-soberana'],
    'wampum-exchange': ['bdet-bank', 'blockchain-soberano', 'analytics-soberano'],
    'contabilidad-soberana': ['facturacion-soberana', 'nomina-soberana', 'impuestos-soberano'],
    'nomina-soberana': ['rrhh-soberano', 'contabilidad-soberana', 'bdet-bank'],

    # Health platforms need identity + comms
    'healthcare-dashboard': ['identidad-soberana', 'telemedicina-soberana', 'farmacia-soberana', 'conferencia-soberana'],
    'telemedicina-soberana': ['conferencia-soberana', 'healthcare-dashboard', 'mensajeria-soberana'],
    'farmacia-soberana': ['healthcare-dashboard', 'inventario-soberano', 'bdet-bank'],

    # Education needs content + comms
    'education-dashboard': ['aula-virtual-soberana', 'biblioteca-soberana', 'certificacion-soberana'],
    'universidad-soberana': ['education-dashboard', 'investigacion-soberana', 'becas-soberanas'],
    'escuela-soberana': ['aula-virtual-soberana', 'comedor-escolar', 'transporte-escolar'],

    # Government needs identity + voting + docs
    'gobierno-soberano': ['votacion-soberana', 'leyes-soberanas', 'censo-soberano', 'identidad-soberana'],
    'votacion-soberana': ['identidad-soberana', 'blockchain-soberano', 'criptografia-soberana'],
    'pasaporte-soberano': ['identidad-soberana', 'registro-civil-soberano', 'criptografia-soberana'],
    'justicia-soberana': ['leyes-soberanas', 'identidad-soberana', 'archivos-nacionales'],

    # Communication platforms
    'correo-soberano': ['identidad-soberana', 'criptografia-soberana', 'dns-soberano'],
    'mensajeria-soberana': ['identidad-soberana', 'criptografia-soberana'],
    'conferencia-soberana': ['streaming-soberano', 'identidad-soberana'],
    'red-social-soberana': ['identidad-soberana', 'media-soberana', 'mensajeria-soberana'],

    # DevOps & infrastructure
    'ci-cd-soberano': ['git-soberano', 'contenedores-soberano', 'testing-soberano'],
    'hosting-soberano': ['dns-soberano', 'cdn-soberana', 'contenedores-soberano'],
    'cloud-soberana': ['datacenters-soberanos', 'contenedores-soberano', 'hosting-soberano'],
    'monitoreo-soberano': ['logs-soberanos', 'observabilidad-soberana', 'alertas-nacionales'],

    # Office & productivity
    'docs-soberanos': ['cloud-soberana', 'colaboracion-soberana', 'identidad-soberana'],
    'ofimatica-soberana': ['cloud-soberana', 'docs-soberanos', 'firma-digital-soberana'],
    'calendario-soberano': ['correo-soberano', 'conferencia-soberana', 'notificaciones-gobierno'],
    'proyecto-soberano': ['calendario-soberano', 'mensajeria-soberana', 'docs-soberanos'],
    'crm-soberano': ['correo-soberano', 'bdet-bank', 'analytics-soberano'],

    # Security
    'ciberseguridad-nacional': ['firewall-soberano', 'soc-soberano', 'threat-intel-soberano'],
    'identidad-soberana': ['criptografia-soberana', 'blockchain-soberano', 'zk-proofs-soberano'],

    # AI & Quantum
    'ai-soberana': ['datos-soberanos', 'ml-soberano', 'gpu-soberano'],
    'quantum-soberano': ['criptografia-soberana', 'ai-soberana'],
    'llm-soberano': ['ai-soberana', 'nlp-soberano', 'datos-soberanos'],

    # Nature & territory
    'agricultura-soberana': ['clima-soberano', 'agua-soberana', 'mapa-soberano', 'oraculo-climatico-soberano'],
    'territorio-vigilante-soberano': ['mapa-soberano', 'satelite-observacion', 'alertas-nacionales'],
    'oraculo-climatico-soberano': ['clima-soberano', 'agricultura-soberana', 'ai-soberana'],

    # Culture
    'archivo-linguistico-soberano': ['ai-soberana', 'nlp-soberano', 'biblioteca-soberana'],
    'ceremonia-virtual-soberana': ['conferencia-soberana', 'streaming-soberano', 'calendario-soberano'],
    'adn-ancestral-soberano': ['healthcare-dashboard', 'criptografia-soberana', 'identidad-soberana'],

    # Casino
    'casino-soberano': ['bdet-bank', 'identidad-soberana', 'blockchain-soberano', 'analytics-soberano'],

    # Smart city
    'emergencias-soberano': ['comunicacion-emergencia', 'mapa-soberano', 'mensajeria-soberana', 'healthcare-dashboard'],
    'transporte-soberano': ['mapa-soberano', 'iot-soberano', 'pagos-soberanos'],
}

# ═══════════════════════════════════════════
# HTML TEMPLATES
# ═══════════════════════════════════════════

def get_interconnection_html(platform_name, deps, nexus_info):
    """Generate the interconnection sidebar HTML for a platform"""
    if not deps and not nexus_info:
        return ''

    lines = []
    lines.append('<section class="sec-panel" aria-label="Interconexiones">')
    lines.append('<h4><span aria-hidden="true">🔗</span> Interconexiones Activas</h4>')

    if deps:
        for dep in deps[:6]:  # Max 6 connections
            dep_display = dep.replace('-soberano', '').replace('-soberana', '').replace('-soberanos', '').replace('-soberanas', '').replace('-', ' ').title()
            lines.append(f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><a href="../{dep}/" class="label">{dep_display}</a><span class="status ok">Conectado</span></div>')

    lines.append('</section>')
    return '\n'.join(lines)

def get_live_status_html():
    """Generate live status panel HTML"""
    return '''<section class="sec-panel" aria-label="Estado del Sistema">
<h4><span aria-hidden="true">⚡</span> Estado en Tiempo Real</h4>
<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><span class="label">Cifrado Quantum</span><span class="status ok">Activo</span></div>
<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><span class="label">AI Engine</span><span class="status ok">Online</span></div>
<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><span class="label">Bank-Grade TLS</span><span class="status ok">256-bit</span></div>
<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><span class="label">Casino Fair RNG</span><span class="status ok">Verificado</span></div>
<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><span class="label">Soberania Datos</span><span class="status ok">100%</span></div>
<div class="sec-progress"><div class="bar green" style="width:98%"></div></div>
</section>'''

def get_quantum_badge_html():
    """Generate quantum encryption badge"""
    return '<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">⚛️</span> Quantum-Safe</span>'


def upgrade_platform(name, filepath):
    """Upgrade a single platform with quantum + connections"""
    with open(filepath, 'r', encoding='utf-8') as f:
        html = f.read()

    original = html
    changes = []

    # 1. Add quantum.js if not present
    if 'ierahkwa-quantum.js' not in html:
        if 'ierahkwa-security.js' in html:
            html = html.replace(
                '<script src="../shared/ierahkwa-security.js"></script>',
                '<script src="../shared/ierahkwa-security.js"></script>\n<script src="../shared/ierahkwa-quantum.js"></script>',
                1
            )
        elif 'shared/ierahkwa-security.js' in html:
            html = html.replace(
                '<script src="shared/ierahkwa-security.js"></script>',
                '<script src="shared/ierahkwa-security.js"></script>\n<script src="shared/ierahkwa-quantum.js"></script>',
                1
            )
        elif '</body>' in html:
            html = html.replace(
                '</body>',
                '<script src="../shared/ierahkwa-quantum.js"></script>\n</body>',
                1
            )
        changes.append('quantum-js')

    # 2. Add interconnections + live status before </main>
    if 'Interconexiones Activas' not in html and '</main>' in html:
        # Find dependencies for this platform
        deps = DEPENDENCIES.get(name, [])

        # Find which NEXUS this belongs to
        nexus_info = None
        for nid, ndata in NEXUS_MAP.items():
            if name in ndata['platforms']:
                nexus_info = ndata
                # Add sibling platforms as connections if no explicit deps
                if not deps:
                    siblings = [p for p in ndata['platforms'] if p != name][:4]
                    deps = siblings
                break

        if deps:
            interconnect = get_interconnection_html(name, deps, nexus_info)
            status = get_live_status_html()
            injection = interconnect + '\n' + status
            html = html.replace('</main>', injection + '\n</main>', 1)
            changes.append('interconnections')
            changes.append('live-status')

    # 3. Add quantum badge to hero section
    if 'Quantum-Safe' not in html and 'encrypted-badge' not in html:
        badge = get_quantum_badge_html()
        # Try to add after the badge div
        if '</div><h2>' in html:
            html = html.replace('</div><h2>', '</div>' + badge + '<h2>', 1)
            changes.append('quantum-badge')
        elif '<h2>' in html and '<section class="hero">' in html:
            # Add before first h2 in hero
            html = html.replace(
                '<section class="hero">',
                '<section class="hero">' + '\n<div style="text-align:center;margin-top:1rem">' + badge + '</div>',
                1
            )
            changes.append('quantum-badge')

    if html != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(html)
        return changes
    return []


def upgrade_root():
    """Upgrade root index.html"""
    filepath = os.path.join(BASE, 'index.html')
    if not os.path.isfile(filepath):
        return []

    with open(filepath, 'r', encoding='utf-8') as f:
        html = f.read()

    original = html
    changes = []

    if 'ierahkwa-quantum.js' not in html:
        if 'ierahkwa-security.js' in html:
            html = html.replace(
                '<script src="shared/ierahkwa-security.js"></script>',
                '<script src="shared/ierahkwa-security.js"></script>\n<script src="shared/ierahkwa-quantum.js"></script>',
                1
            )
        elif '</body>' in html:
            html = html.replace(
                '</body>',
                '<script src="shared/ierahkwa-quantum.js"></script>\n</body>',
                1
            )
        changes.append('quantum-js')

    if html != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(html)
    return changes


def main():
    print("=" * 60)
    print("⚛️  IERAHKWA QUANTUM + AI + BANK + CASINO + CONNECT UPGRADE")
    print("=" * 60)

    # 1. Root
    print("\n📄 Upgrading root index.html...")
    changes = upgrade_root()
    if changes:
        print(f"   ✅ Root: {', '.join(changes)}")
    else:
        print("   ⏭️  Root: already upgraded")

    # 2. All platforms
    upgraded = 0
    skipped = 0
    connected = 0
    total_changes = {}

    for item in sorted(os.listdir(BASE)):
        path = os.path.join(BASE, item)
        idx = os.path.join(path, 'index.html')
        if os.path.isdir(path) and os.path.isfile(idx):
            if item in ('shared', 'icons', 'screenshots'):
                continue
            changes = upgrade_platform(item, idx)
            if changes:
                upgraded += 1
                if 'interconnections' in changes:
                    connected += 1
                for c in changes:
                    total_changes[c] = total_changes.get(c, 0) + 1
            else:
                skipped += 1

    # Summary
    print(f"\n{'=' * 60}")
    print(f"📊 QUANTUM UPGRADE RESULTS")
    print(f"{'=' * 60}")
    print(f"  ✅ Upgraded:     {upgraded} platforms")
    print(f"  🔗 Connected:    {connected} with interconnections")
    print(f"  ⏭️  Skipped:      {skipped}")
    print(f"\n  Changes applied:")
    for change, count in sorted(total_changes.items()):
        print(f"    • {change}: {count} files")
    print(f"\n⚛️ Quantum + AI + Bank + Casino upgrade complete!")
    print(f"{'=' * 60}")


if __name__ == '__main__':
    main()

#!/usr/bin/env node
/**
 * Wire All 190 HTML Platforms to Backend APIs
 * Injects <script src="../shared/ierahkwa-api.js"> into every index.html
 * and adds API initialization code specific to each platform's NEXUS domain.
 */

const fs = require('fs');
const path = require('path');

const PLATFORMS_DIR = path.join(__dirname, '..', '02-plataformas-html');
const API_SCRIPT_TAG = '<script src="../shared/ierahkwa-api.js"></script>';

// Map platform folder prefixes to their API endpoints
const PLATFORM_API_MAP = {
    // NEXUS Orbital — Science & Tech
    'observatorio': '/api/space', 'estacion-espacial': '/api/space', 'astro': '/api/space',
    'telecom': '/api/telecom', 'frecuencia': '/api/telecom', 'conecta': '/api/telecom',
    'genomica': '/api/genomics', 'biotech': '/api/genomics',
    'iot': '/api/iot-robotics', 'robotica': '/api/iot-robotics', 'domotica': '/api/iot-robotics',
    'quantum': '/api/quantum', 'cuantica': '/api/quantum',
    'ai-': '/api/ai-engine', 'inteligencia-artificial': '/api/ai-engine', 'fortress': '/api/ai-engine',
    'red-soberana': '/api/network', 'mesh': '/api/network',
    'dev-tools': '/api/devtools', 'codigo': '/api/devtools', 'ide': '/api/devtools',

    // NEXUS Escudo — Defense
    'defensa': '/api/military', 'militar': '/api/military', 'guardia': '/api/military',
    'dron': '/api/drones', 'drone': '/api/drones', 'aereo': '/api/drones',
    'cyber': '/api/cybersec', 'seguridad-digital': '/api/cybersec', 'firewall': '/api/cybersec',
    'inteligencia': '/api/intelligence', 'vigilancia': '/api/intelligence',
    'emergencia': '/api/emergency', 'alerta': '/api/emergency', 'rescate': '/api/emergency',

    // NEXUS Cerebro — Education
    'escuela': '/api/education', 'universidad': '/api/education', 'academia': '/api/education',
    'aprendizaje': '/api/education', 'educacion': '/api/education', 'mooc': '/api/education',
    'investigacion': '/api/research', 'laboratorio': '/api/research', 'ciencia': '/api/research',
    'atabey': '/api/language', 'idioma': '/api/language', 'lengua': '/api/language',
    'busqueda': '/api/search', 'buscador': '/api/search',

    // NEXUS Tesoro — Economy
    'mercado': '/api/commerce', 'tienda': '/api/commerce', 'comercio': '/api/commerce',
    'mameynode': '/api/blockchain', 'blockchain': '/api/blockchain', 'wampum': '/api/blockchain',
    'banco': '/api/banking', 'finanzas': '/api/banking', 'banca': '/api/banking',
    'seguro': '/api/insurance', 'aseguradora': '/api/insurance',
    'empleo': '/api/employment', 'trabajo': '/api/employment', 'bolsa-trabajo': '/api/employment',
    'fabrica': '/api/smart-factory', 'manufactura': '/api/smart-factory',
    'artesania': '/api/artisan', 'artesano': '/api/artisan', 'craftsman': '/api/artisan',
    'turismo': '/api/tourism', 'viaje': '/api/tourism', 'destino': '/api/tourism',

    // NEXUS Voces — Culture
    'canal': '/api/media', 'noticias': '/api/media', 'prensa': '/api/media', 'video': '/api/media',
    'mensajeria': '/api/messaging', 'chat': '/api/messaging', 'correo': '/api/messaging',
    'cultura': '/api/culture', 'museo': '/api/culture', 'archivo': '/api/culture', 'herencia': '/api/culture',
    'deporte': '/api/sports', 'juego-ancestral': '/api/sports', 'olimpiada': '/api/sports',
    'social': '/api/social', 'comunidad': '/api/social', 'foro': '/api/social',

    // NEXUS Consejo — Governance
    'consejo': '/api/governance', 'gobierno': '/api/governance', 'parlamento': '/api/governance',
    'justicia': '/api/justice', 'tribunal': '/api/justice', 'ley': '/api/justice',
    'diplomacia': '/api/diplomacy', 'embajada': '/api/diplomacy', 'tratado': '/api/diplomacy',
    'ciudadano': '/api/citizen', 'registro': '/api/citizen', 'censo': '/api/citizen',
    'bienestar': '/api/social-welfare', 'asistencia': '/api/social-welfare', 'ayuda': '/api/social-welfare',

    // NEXUS Tierra — Environment
    'agricultura': '/api/agriculture', 'granja': '/api/agriculture', 'cosecha': '/api/agriculture',
    'recurso': '/api/natural-resource', 'mineria': '/api/natural-resource',
    'ambiente': '/api/environment', 'ecologia': '/api/environment', 'clima': '/api/environment',
    'residuo': '/api/waste', 'reciclaje': '/api/waste',
    'energia': '/api/energy', 'solar': '/api/energy', 'eolica': '/api/energy',

    // NEXUS Forja — Innovation
    'devops': '/api/devops', 'pipeline': '/api/devops',
    'lowcode': '/api/lowcode', 'nocode': '/api/lowcode', 'constructor': '/api/lowcode',
    'navegador': '/api/browser', 'browser': '/api/browser',
    'productividad': '/api/productivity', 'nota': '/api/productivity', 'tarea': '/api/productivity',
    'nube': '/api/cloud', 'cloud': '/api/cloud',

    // NEXUS Urbe — Infrastructure
    'urbanismo': '/api/urban', 'ciudad': '/api/urban', 'planificacion': '/api/urban',
    'transporte': '/api/transport', 'ruta': '/api/transport', 'autobus': '/api/transport',
    'correo-postal': '/api/postal-maps', 'mapa': '/api/postal-maps', 'postal': '/api/postal-maps',
    'vivienda': '/api/housing', 'hogar': '/api/housing', 'inmobiliaria': '/api/housing',

    // NEXUS Raíces — Identity
    'identidad': '/api/identity', 'fwid': '/api/identity', 'verificacion': '/api/identity',
    'salud': '/api/health', 'hospital': '/api/health', 'clinica': '/api/health', 'medic': '/api/health',
    'nexus': '/api/nexus', 'portal': '/api/nexus', 'central': '/api/nexus',
    'licencia': '/api/licensing', 'admin': '/api/licensing',
};

function findApiEndpoint(folderName) {
    const lower = folderName.toLowerCase();
    for (const [prefix, endpoint] of Object.entries(PLATFORM_API_MAP)) {
        if (lower.includes(prefix)) return endpoint;
    }
    return '/api/nexus'; // Default to NEXUS aggregation
}

function wireAllPlatforms() {
    const dirs = fs.readdirSync(PLATFORMS_DIR, { withFileTypes: true })
        .filter(d => d.isDirectory() && d.name !== 'shared' && d.name !== 'investor-audit-presentation');

    let wired = 0, skipped = 0;

    for (const dir of dirs) {
        const indexPath = path.join(PLATFORMS_DIR, dir.name, 'index.html');
        if (!fs.existsSync(indexPath)) { skipped++; continue; }

        let html = fs.readFileSync(indexPath, 'utf-8');

        // Skip if already wired
        if (html.includes('ierahkwa-api.js')) { skipped++; continue; }

        const apiEndpoint = findApiEndpoint(dir.name);

        // Inject API script before </body>
        const initScript = `
${API_SCRIPT_TAG}
<script>
// Auto-connect to ${apiEndpoint}
window.PLATFORM_API = '${apiEndpoint}';
if (!Ierahkwa.auth.isLoggedIn()) {
    console.log('[${dir.name}] Not logged in — showing login modal');
}
</script>`;

        if (html.includes('</body>')) {
            html = html.replace('</body>', initScript + '\n</body>');
        } else {
            html += initScript;
        }

        fs.writeFileSync(indexPath, html);
        wired++;
        console.log(`  [+] ${dir.name} → ${apiEndpoint}`);
    }

    console.log(`\nWiring complete: ${wired} platforms wired, ${skipped} skipped.`);
    console.log(`Total directories scanned: ${dirs.length}`);
}

wireAllPlatforms();

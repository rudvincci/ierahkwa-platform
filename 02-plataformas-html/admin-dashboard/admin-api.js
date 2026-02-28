// Ierahkwa Admin Panel — API Client
// Connects to the Gateway API for all admin operations

const API = {
    baseUrl: window.IERAHKWA_API || 'https://api.ierahkwa.org',
    token: localStorage.getItem('ierahkwa_admin_token'),

    async request(method, path, body = null) {
        const headers = { 'Content-Type': 'application/json' };
        if (this.token) headers['Authorization'] = `Bearer ${this.token}`;
        const tenantId = localStorage.getItem('ierahkwa_tenant');
        if (tenantId) headers['X-Tenant-Id'] = tenantId;

        const res = await fetch(`${this.baseUrl}${path}`, {
            method, headers,
            body: body ? JSON.stringify(body) : null
        });

        if (res.status === 401) {
            localStorage.removeItem('ierahkwa_admin_token');
            window.location.href = '#login';
            throw new Error('Unauthorized');
        }
        if (!res.ok) throw new Error(`API Error: ${res.status}`);
        return res.json();
    },

    get: (path) => API.request('GET', path),
    post: (path, body) => API.request('POST', path, body),
    put: (path, body) => API.request('PUT', path, body),
    del: (path) => API.request('DELETE', path),

    // Auth
    async login(userId, tenantId, tier, roles) {
        const data = await this.post('/auth/login', { userId, tenantId, tier, roles });
        this.token = data.token;
        localStorage.setItem('ierahkwa_admin_token', data.token);
        localStorage.setItem('ierahkwa_tenant', tenantId);
        return data;
    },
    async me() { return this.get('/auth/me'); },

    // Service catalog
    async services() { return this.get('/services'); },
    async health() { return this.get('/health'); },

    // Licensing
    async tenants() { return this.get('/api/licensing/tenants'); },
    async plans() { return this.get('/api/licensing/plans'); },
    async licenses() { return this.get('/api/licensing/licenses'); },
    async createLicense(data) { return this.post('/api/licensing/licenses', data); },

    // Citizens
    async citizens(tenantId) { return this.get(`/api/citizen?tenantId=${tenantId}`); },
    async citizenCount() { return this.get('/api/citizen/count'); },

    // Governance
    async councils() { return this.get('/api/governance/councils'); },
    async resolutions() { return this.get('/api/governance/resolutions'); },

    // NEXUS Aggregation
    async dashboards() { return this.get('/api/nexus/dashboards'); },
    async metrics() { return this.get('/api/nexus/metrics'); },

    // NEXUS Escolar
    async escuelas() { return this.get('/api/escolar/escuelas'); },
    async escolarStats() { return this.get('/api/escolar/stats'); },

    // NEXUS Entretenimiento
    async entretenimiento() { return this.get('/api/entretenimiento/platforms'); },
    async entertainmentStats() { return this.get('/api/entretenimiento/stats'); },

    // Gaming & Entertainment
    async casinoStats() { return this.get('/api/gaming/casino/stats'); },
    async casinoGames() { return this.get('/api/gaming/casino/games'); },
    async bettingEvents() { return this.get('/api/gaming/betting/events'); },
    async bettingLive() { return this.get('/api/gaming/betting/live'); },
    async lotteryDraws() { return this.get('/api/gaming/lottery/draws'); },
    async lotteryResults() { return this.get('/api/gaming/lottery/results'); },
    async raffleEvents() { return this.get('/api/gaming/raffle/events'); },
    async raffleResults() { return this.get('/api/gaming/raffle/results'); },
};

// Admin Panel State
const AdminState = {
    currentPage: 'dashboard',
    tenant: localStorage.getItem('ierahkwa_tenant') || 'ierahkwa-global',
    user: null,
    platforms: 220,
    microservices: 87,
    nexusDomains: [
        { name: 'Orbital',  color: '#00bcd4', icon: '🛰', services: 8,  platforms: 29 },
        { name: 'Escudo',   color: '#f44336', icon: '🛡', services: 5,  platforms: 16 },
        { name: 'Cerebro',  color: '#7c4dff', icon: '🧠', services: 4,  platforms: 8  },
        { name: 'Tesoro',   color: '#ffd600', icon: '💰', services: 11, platforms: 25 },
        { name: 'Voces',    color: '#e040fb', icon: '📡', services: 5,  platforms: 15 },
        { name: 'Consejo',  color: '#1565c0', icon: '⚖', services: 5,  platforms: 14 },
        { name: 'Tierra',   color: '#43a047', icon: '🌿', services: 5,  platforms: 17 },
        { name: 'Forja',    color: '#00e676', icon: '🔧', services: 5,  platforms: 7  },
        { name: 'Urbe',     color: '#ff9100', icon: '🏙', services: 4,  platforms: 11 },
        { name: 'Raíces',   color: '#00FF41', icon: '🏺', services: 4,  platforms: 8  },
        { name: 'Salud',    color: '#FF5722', icon: '🏥', services: 4,  platforms: 7  },
        { name: 'Academia', color: '#9C27B0', icon: '🎓', services: 3,  platforms: 5  },
        { name: 'Amparo',   color: '#607D8B', icon: '🤝', services: 3,  platforms: 10 },
        { name: 'Entretenimiento', color: '#E91E63', icon: '🎰', services: 8, platforms: 8 },
    ],
    gamingPlatforms: [
        { name: 'Casino Soberano',   type: 'casino',  games: 6,   status: 'active' },
        { name: 'Apuestas Soberano', type: 'betting', sports: 6,  status: 'active' },
        { name: 'Loto Soberano',     type: 'lottery', drawTypes: 3, status: 'active' },
        { name: 'Rifa Soberana',     type: 'raffle',  raffleTypes: 5, status: 'active' },
    ],
    nexusPlatforms: [
        { name: 'NEXUS Escolar', type: 'education-k12', services: 8, status: 'active' },
        { name: 'NEXUS Entretenimiento', type: 'entertainment-gaming', services: 8, status: 'active' },
    ],
};

// White-label configuration
const WhiteLabelConfig = {
    tiers: [
        { id: 'wl-starter', name: 'Starter', price: '$25K/yr', platforms: 10, features: ['Basic customization', 'Email support', 'Standard SLA'] },
        { id: 'wl-professional', name: 'Professional', price: '$100K/yr', platforms: 50, features: ['Full customization', 'Priority support', 'Custom domain', '99.9% SLA'] },
        { id: 'wl-sovereign', name: 'Sovereign', price: '$500K/yr', platforms: 196, features: ['Sovereign branding', 'Dedicated success manager', 'Custom integrations', '99.99% SLA'] },
        { id: 'wl-enterprise', name: 'Enterprise', price: '$1M+/yr', platforms: 196, features: ['Dedicated infrastructure', 'Custom development', 'On-premise option', 'Unlimited SLA'] },
    ],
    userTiers: [
        { id: 'member', name: 'Member', price: 'Free', access: 'Basic' },
        { id: 'resident', name: 'Resident', price: '$2.99/mo', access: 'Enhanced' },
        { id: 'citizen', name: 'Citizen', price: '$9.99/mo', access: 'Full' },
    ],
};

// Render helper
function renderAdminTable(headers, rows) {
    return `<table class="tbl">
        <thead><tr>${headers.map(h => `<th>${h}</th>`).join('')}</tr></thead>
        <tbody>${rows.map(r => `<tr>${r.map(c => `<td>${c}</td>`).join('')}</tr>`).join('')}</tbody>
    </table>`;
}

function renderNexusGrid() {
    return AdminState.nexusDomains.map(d => `
        <div class="card" style="border-left:3px solid ${d.color}">
            <div class="l">${d.icon} NEXUS ${d.name}</div>
            <div class="n">${d.services}</div>
            <div class="d">microservices</div>
        </div>
    `).join('');
}

console.log('[Ierahkwa Admin] v3.9.0 loaded — 87 microservices, 220 platforms, 14 NEXUS');

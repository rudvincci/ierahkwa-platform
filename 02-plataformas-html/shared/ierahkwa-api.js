/**
 * IERAHKWA SOVEREIGN PLATFORM — Shared API Client v3.0.0
 * Include this in every HTML platform to connect to the backend.
 *
 * Usage:
 *   <script src="../shared/ierahkwa-api.js"></script>
 *   <script>
 *     // Login
 *     await Ierahkwa.auth.login('user-id', 'navajo', 'citizen');
 *
 *     // API calls
 *     const patients = await Ierahkwa.api.get('/api/health/patients');
 *     const newPatient = await Ierahkwa.api.post('/api/health/patients', { name: 'John' });
 *   </script>
 */

window.Ierahkwa = (function() {
    'use strict';

    const BASE_URL = window.IERAHKWA_API_URL || 'https://api.ierahkwa.org';
    const STORAGE_KEY = 'ierahkwa_session';

    // Session management
    function getSession() {
        try { return JSON.parse(localStorage.getItem(STORAGE_KEY) || 'null'); }
        catch { return null; }
    }
    function setSession(session) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    }
    function clearSession() {
        localStorage.removeItem(STORAGE_KEY);
    }

    // Core HTTP client
    async function request(method, path, body) {
        const session = getSession();
        const headers = { 'Content-Type': 'application/json' };
        if (session?.token) headers['Authorization'] = 'Bearer ' + session.token;
        if (session?.tenantId) headers['X-Tenant-Id'] = session.tenantId;

        const opts = { method, headers };
        if (body && method !== 'GET') opts.body = JSON.stringify(body);

        const res = await fetch(BASE_URL + path, opts);

        if (res.status === 401) {
            clearSession();
            showLoginModal();
            throw new Error('Session expired. Please login again.');
        }
        if (res.status === 403) throw new Error('Access denied. Upgrade your tier for this feature.');
        if (res.status === 429) throw new Error('Rate limit exceeded. Please wait.');
        if (!res.ok) throw new Error('API Error: ' + res.status);
        return res.json();
    }

    // Auth module
    const auth = {
        async login(userId, tenantId, tier, roles) {
            const data = await request('POST', '/auth/login', {
                userId, tenantId, tier: tier || 'member', roles: roles || ['user']
            });
            setSession({ token: data.token, userId, tenantId, tier: data.tier });
            hideLoginModal();
            return data;
        },
        async refresh() {
            const session = getSession();
            if (!session?.token) return null;
            const data = await request('POST', '/auth/refresh', { token: session.token });
            session.token = data.token;
            setSession(session);
            return data;
        },
        async me() { return request('GET', '/auth/me'); },
        logout() { clearSession(); showLoginModal(); },
        isLoggedIn() { return !!getSession()?.token; },
        getSession,
        getTier() { return getSession()?.tier || 'anonymous'; },
        getTenantId() { return getSession()?.tenantId || ''; },
    };

    // API module
    const api = {
        get: (path) => request('GET', path),
        post: (path, body) => request('POST', path, body),
        put: (path, body) => request('PUT', path, body),
        delete: (path) => request('DELETE', path),
    };

    // Tier-based feature flags
    const tiers = {
        isMember() { return ['member','resident','citizen'].includes(auth.getTier()); },
        isResident() { return ['resident','citizen'].includes(auth.getTier()); },
        isCitizen() { return auth.getTier() === 'citizen'; },
        canAccess(requiredTier) {
            const order = { anonymous: 0, member: 1, resident: 2, citizen: 3 };
            return (order[auth.getTier()] || 0) >= (order[requiredTier] || 0);
        },
    };

    // Login modal (injected into page)
    function showLoginModal() {
        if (document.getElementById('ierahkwa-login-modal')) return;
        const modal = document.createElement('div');
        modal.id = 'ierahkwa-login-modal';
        modal.innerHTML = `
        <div style="position:fixed;inset:0;background:rgba(0,0,0,.85);z-index:99999;display:flex;align-items:center;justify-content:center">
            <div style="background:#111116;border:1px solid #2a2a36;border-radius:16px;padding:40px;width:360px;max-width:90vw">
                <h2 style="color:#d4a853;font-size:20px;margin-bottom:4px;text-align:center">IERAHKWA</h2>
                <p style="color:#8a8694;font-size:12px;text-align:center;margin-bottom:24px">Sign in with your FWID</p>
                <label style="color:#8a8694;font-size:10px;text-transform:uppercase;letter-spacing:1px">FWID</label>
                <input id="ierahkwa-login-uid" style="width:100%;background:#1a1a20;border:1px solid #2a2a36;border-radius:8px;padding:10px;color:#e8e4df;margin:6px 0 12px;font-size:14px" placeholder="Your FWID">
                <label style="color:#8a8694;font-size:10px;text-transform:uppercase;letter-spacing:1px">Tenant</label>
                <input id="ierahkwa-login-tenant" style="width:100%;background:#1a1a20;border:1px solid #2a2a36;border-radius:8px;padding:10px;color:#e8e4df;margin:6px 0 12px;font-size:14px" placeholder="e.g. navajo">
                <select id="ierahkwa-login-tier" style="width:100%;background:#1a1a20;border:1px solid #2a2a36;border-radius:8px;padding:10px;color:#e8e4df;margin:6px 0 16px;font-size:14px">
                    <option value="member">Member (Free)</option>
                    <option value="resident">Resident ($2.99/mo)</option>
                    <option value="citizen">Citizen ($9.99/mo)</option>
                </select>
                <button id="ierahkwa-login-btn" style="width:100%;background:#d4a853;color:#09090d;border:none;border-radius:8px;padding:12px;font-weight:700;cursor:pointer;font-size:14px">Sign In</button>
                <p id="ierahkwa-login-err" style="color:#ff4d6a;font-size:11px;text-align:center;margin-top:8px;display:none"></p>
            </div>
        </div>`;
        document.body.appendChild(modal);
        document.getElementById('ierahkwa-login-btn').onclick = async () => {
            const uid = document.getElementById('ierahkwa-login-uid').value;
            const tenant = document.getElementById('ierahkwa-login-tenant').value;
            const tier = document.getElementById('ierahkwa-login-tier').value;
            try {
                await auth.login(uid || 'demo', tenant || 'ierahkwa-global', tier);
            } catch(e) {
                const err = document.getElementById('ierahkwa-login-err');
                err.textContent = e.message;
                err.style.display = 'block';
            }
        };
    }

    function hideLoginModal() {
        const m = document.getElementById('ierahkwa-login-modal');
        if (m) m.remove();
    }

    // Auto-refresh token every 20 hours
    if (auth.isLoggedIn()) {
        setInterval(() => auth.refresh().catch(() => {}), 72000000);
    }

    // Health check on load
    request('GET', '/health').catch(() => {
        console.warn('[Ierahkwa] Gateway unreachable — running in offline mode');
    });

    return { auth, api, tiers, BASE_URL, version: '3.0.0' };
})();

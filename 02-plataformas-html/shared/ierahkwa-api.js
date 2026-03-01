/**
 * IERAHKWA SOVEREIGN PLATFORM — Shared API Client v4.0.0
 * Include this in every HTML platform to connect to the backend.
 * Backward-compatible with v3.5.0 (window.Ierahkwa) + new sovereign-core endpoints.
 *
 * Usage:
 *   <script src="../shared/ierahkwa-api.js"></script>
 *   <script>
 *     // v3.5 API (still works):
 *     await Ierahkwa.auth.login('user-id', 'navajo', 'citizen');
 *     const data = await Ierahkwa.api.get('/api/health/patients');
 *
 *     // v4.0 sovereign-core API:
 *     await Ierahkwa.content.list('voz-soberana');
 *     await Ierahkwa.payments.balance();
 *     await Ierahkwa.votes.active();
 *     await Ierahkwa.messages.inbox();
 *   </script>
 */

window.Ierahkwa = (function() {
    'use strict';

    const BASE_URL = window.IERAHKWA_API_URL || 'https://api.ierahkwa.org';
    const CORE_URL = window.IERAHKWA_CORE_URL || BASE_URL;
    const STORAGE_KEY = 'ierahkwa_session';

    // ── Session Management ─────────────────────────────────────
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

    // ── Core HTTP Client ───────────────────────────────────────
    async function request(method, path, body, baseUrl) {
        const session = getSession();
        const headers = { 'Content-Type': 'application/json' };
        if (session?.token) headers['Authorization'] = 'Bearer ' + session.token;
        if (session?.tenantId) headers['X-Tenant-Id'] = session.tenantId;

        const opts = { method, headers };
        if (body && method !== 'GET') opts.body = JSON.stringify(body);

        const url = (baseUrl || BASE_URL) + path;
        const res = await fetch(url, opts);

        if (res.status === 401) {
            clearSession();
            window.dispatchEvent(new CustomEvent('ierahkwa:auth:expired'));
            showLoginModal();
            throw new Error('Session expired. Please login again.');
        }
        if (res.status === 403) throw new Error('Access denied. Upgrade your tier for this feature.');
        if (res.status === 429) throw new Error('Rate limit exceeded. Please wait.');
        if (!res.ok) {
            let errBody;
            try { errBody = await res.json(); } catch { errBody = {}; }
            const err = new Error(errBody.detail || errBody.message || 'API Error: ' + res.status);
            err.status = res.status;
            err.code = errBody.code || 'UNKNOWN';
            throw err;
        }
        return res.json();
    }

    // Sovereign-core requests go to CORE_URL
    function coreReq(method, path, body) {
        return request(method, path, body, CORE_URL);
    }

    // File upload (multipart)
    async function uploadFile(path, file, extraFields) {
        const session = getSession();
        const headers = {};
        if (session?.token) headers['Authorization'] = 'Bearer ' + session.token;

        const formData = new FormData();
        formData.append('file', file);
        if (extraFields) {
            Object.entries(extraFields).forEach(([k, v]) => formData.append(k, v));
        }

        const res = await fetch(CORE_URL + path, { method: 'POST', headers, body: formData });
        if (!res.ok) throw new Error('Upload failed: ' + res.status);
        return res.json();
    }

    // ── Auto-detect platform name from URL path ────────────────
    function detectPlatform() {
        const match = window.location.pathname.match(/\/([a-z0-9-]+)\/(?:index\.html)?$/);
        return match ? match[1] : 'unknown';
    }
    const currentPlatform = detectPlatform();

    // ── Auth Module (v3.5 compatible) ──────────────────────────
    const auth = {
        async login(userId, tenantId, tier, roles) {
            const data = await request('POST', '/v1/auth/login', {
                userId, tenantId, tier: tier || 'member', roles: roles || ['user']
            });
            setSession({ token: data.token, userId, tenantId, tier: data.tier || tier });
            hideLoginModal();
            window.dispatchEvent(new CustomEvent('ierahkwa:auth:login', { detail: data }));
            return data;
        },
        async register(data) {
            const result = await coreReq('POST', '/v1/auth/register', data);
            if (result.token) {
                setSession({ token: result.token, userId: result.user?.id, tier: result.user?.tier || 'member' });
                hideLoginModal();
            }
            return result;
        },
        async refresh() {
            const session = getSession();
            if (!session?.token) return null;
            const data = await request('POST', '/v1/auth/refresh', { token: session.token });
            session.token = data.token;
            setSession(session);
            return data;
        },
        async me() { return request('GET', '/v1/auth/me'); },
        logout() {
            clearSession();
            window.dispatchEvent(new CustomEvent('ierahkwa:auth:logout'));
            showLoginModal();
        },
        isLoggedIn() { return !!getSession()?.token; },
        getSession,
        getTier() { return getSession()?.tier || 'anonymous'; },
        getTenantId() { return getSession()?.tenantId || ''; },
    };

    // ── API Module (v3.5 compatible — raw HTTP) ────────────────
    const api = {
        get: (path) => request('GET', path),
        post: (path, body) => request('POST', path, body),
        put: (path, body) => request('PUT', path, body),
        delete: (path) => request('DELETE', path),
    };

    // ── Tier-Based Feature Flags ───────────────────────────────
    const tiers = {
        isMember() { return ['member','resident','citizen'].includes(auth.getTier()); },
        isResident() { return ['resident','citizen'].includes(auth.getTier()); },
        isCitizen() { return auth.getTier() === 'citizen'; },
        canAccess(requiredTier) {
            const order = { anonymous: 0, member: 1, resident: 2, citizen: 3 };
            return (order[auth.getTier()] || 0) >= (order[requiredTier] || 0);
        },
    };

    // ══════════════════════════════════════════════════════════
    // v4.0 — Sovereign Core Modules
    // ══════════════════════════════════════════════════════════

    // ── Content CRUD (generic, per-platform) ───────────────────
    const content = {
        list(platform, opts) {
            platform = platform || currentPlatform;
            const q = new URLSearchParams();
            if (opts?.page) q.set('page', opts.page);
            if (opts?.limit) q.set('limit', opts.limit);
            if (opts?.type) q.set('type', opts.type);
            if (opts?.sort) q.set('sort', opts.sort);
            return coreReq('GET', `/v1/${platform}/items?${q}`);
        },
        create(platform, data) {
            return coreReq('POST', `/v1/${platform || currentPlatform}/items`, data);
        },
        get(platform, id) {
            return coreReq('GET', `/v1/${platform || currentPlatform}/items/${id}`);
        },
        update(platform, id, data) {
            return coreReq('PUT', `/v1/${platform || currentPlatform}/items/${id}`, data);
        },
        delete(platform, id) {
            return coreReq('DELETE', `/v1/${platform || currentPlatform}/items/${id}`);
        },
        stats(platform) {
            return coreReq('GET', `/v1/${platform || currentPlatform}/items/stats`);
        },
        categories(platform) {
            return coreReq('GET', `/v1/${platform || currentPlatform}/items/categories`);
        },
    };

    // ── Users ──────────────────────────────────────────────────
    const users = {
        profile() { return coreReq('GET', '/v1/users/profile'); },
        updateProfile(data) { return coreReq('PUT', '/v1/users/profile', data); },
        publicProfile(userId) { return coreReq('GET', `/v1/users/${userId}/public`); },
    };

    // ── Payments (BDET / WMP) ──────────────────────────────────
    const payments = {
        send(toUserId, amount, currency) {
            return coreReq('POST', '/v1/payments/send', { to_user_id: toUserId, amount, currency: currency || 'WMP' });
        },
        history(opts) {
            const q = new URLSearchParams();
            if (opts?.page) q.set('page', opts.page);
            if (opts?.limit) q.set('limit', opts.limit);
            if (opts?.direction) q.set('direction', opts.direction);
            return coreReq('GET', `/v1/payments/history?${q}`);
        },
        balance() { return coreReq('GET', '/v1/payments/balance'); },
        tip(toUserId, amount, platform) {
            return coreReq('POST', '/v1/payments/tip', { to_user_id: toUserId, amount, platform: platform || currentPlatform });
        },
    };

    // ── Messages ───────────────────────────────────────────────
    const messages = {
        send(toUserId, subject, body) {
            return coreReq('POST', '/v1/messages/send', { to_user_id: toUserId, subject, body });
        },
        inbox(opts) {
            const q = new URLSearchParams();
            if (opts?.page) q.set('page', opts.page);
            if (opts?.limit) q.set('limit', opts.limit);
            if (opts?.unread) q.set('unread', 'true');
            return coreReq('GET', `/v1/messages/inbox?${q}`);
        },
        thread(threadId) { return coreReq('GET', `/v1/messages/thread/${threadId}`); },
        markRead(messageId) { return coreReq('PUT', `/v1/messages/${messageId}/read`); },
    };

    // ── Votes ──────────────────────────────────────────────────
    const votes = {
        active() { return coreReq('GET', '/v1/votes/active'); },
        cast(electionId, choice) {
            return coreReq('POST', '/v1/votes/cast', { election_id: electionId, choice });
        },
        results(electionId) { return coreReq('GET', `/v1/votes/results/${electionId}`); },
        createElection(data) { return coreReq('POST', '/v1/votes/create-election', data); },
    };

    // ── Storage ────────────────────────────────────────────────
    const storage = {
        upload(file, platform) {
            return uploadFile('/v1/storage/upload', file, { platform: platform || currentPlatform });
        },
        get(fileId) { return coreReq('GET', `/v1/storage/${fileId}?meta=true`); },
        delete(fileId) { return coreReq('DELETE', `/v1/storage/${fileId}`); },
    };

    // ── Analytics ──────────────────────────────────────────────
    const analytics = {
        summary(platform, days) {
            const q = days ? `?days=${days}` : '';
            return coreReq('GET', `/v1/analytics/${platform || currentPlatform}/summary${q}`);
        },
        users(platform) {
            return coreReq('GET', `/v1/analytics/${platform || currentPlatform}/users`);
        },
    };

    // ── WebSocket Chat ─────────────────────────────────────────
    const chat = (function() {
        let ws = null;
        let reconnectAttempts = 0;
        const maxReconnect = 10;
        const listeners = [];

        function connect(platform) {
            const session = getSession();
            const userId = session?.userId || 'anon-' + Date.now();
            const wsUrl = CORE_URL.replace(/^http/, 'ws') + `/ws/chat?platform=${platform || currentPlatform}&userId=${userId}`;

            ws = new WebSocket(wsUrl);
            ws.onopen = () => {
                reconnectAttempts = 0;
                window.dispatchEvent(new CustomEvent('ierahkwa:ws:connected'));
            };
            ws.onmessage = (e) => {
                try {
                    const msg = JSON.parse(e.data);
                    listeners.forEach(fn => fn(msg));
                    window.dispatchEvent(new CustomEvent('ierahkwa:ws:message', { detail: msg }));
                } catch {}
            };
            ws.onclose = () => {
                window.dispatchEvent(new CustomEvent('ierahkwa:ws:disconnected'));
                if (reconnectAttempts < maxReconnect) {
                    const delay = Math.min(1000 * Math.pow(2, reconnectAttempts), 30000);
                    reconnectAttempts++;
                    setTimeout(() => connect(platform), delay);
                }
            };
            ws.onerror = () => {};
        }

        return {
            connect,
            send(body) {
                if (ws?.readyState === 1) ws.send(JSON.stringify({ type: 'chat', body }));
            },
            dm(toUserId, body) {
                if (ws?.readyState === 1) ws.send(JSON.stringify({ type: 'dm', to: toUserId, body }));
            },
            onMessage(fn) { listeners.push(fn); },
            disconnect() { if (ws) { reconnectAttempts = maxReconnect; ws.close(); ws = null; } },
            isConnected() { return ws?.readyState === 1; },
        };
    })();

    // ── Login Modal ────────────────────────────────────────────
    function showLoginModal() {
        if (document.getElementById('ierahkwa-login-modal')) return;
        const modal = document.createElement('div');
        modal.id = 'ierahkwa-login-modal';
        modal.innerHTML = `
        <div style="position:fixed;inset:0;background:rgba(0,0,0,.85);z-index:99999;display:flex;align-items:center;justify-content:center">
            <div style="background:#111116;border:1px solid #2a2a36;border-radius:16px;padding:40px;width:360px;max-width:90vw">
                <h2 style="color:#00FF41;font-size:20px;margin-bottom:4px;text-align:center">IERAHKWA</h2>
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
                <button id="ierahkwa-login-btn" style="width:100%;background:#00FF41;color:#09090d;border:none;border-radius:8px;padding:12px;font-weight:700;cursor:pointer;font-size:14px">Sign In</button>
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

    // ── Auto-refresh token every 20 hours ──────────────────────
    if (auth.isLoggedIn()) {
        setInterval(() => auth.refresh().catch(() => {}), 72000000);
    }

    // ── Health check on load ───────────────────────────────────
    request('GET', '/health').catch(() => {
        console.warn('[Ierahkwa] Gateway unreachable — running in offline mode');
    });

    // ── Public API ─────────────────────────────────────────────
    return {
        // v3.5 backward-compatible
        auth, api, tiers, BASE_URL,
        // v4.0 sovereign-core modules
        content, users, payments, messages, votes, storage, analytics, chat,
        // Metadata
        platform: currentPlatform,
        version: '4.0.0'
    };
})();

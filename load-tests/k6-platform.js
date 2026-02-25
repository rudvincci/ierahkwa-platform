import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const BASE_URL = __ENV.API_URL || 'https://api.ierahkwa.org';
const errorRate = new Rate('errors');
const loginDuration = new Trend('login_duration');

export const options = {
    stages: [
        { duration: '1m', target: 50 },    // Ramp up
        { duration: '3m', target: 200 },   // Steady state
        { duration: '2m', target: 500 },   // Peak load
        { duration: '1m', target: 1000 },  // Stress test
        { duration: '1m', target: 0 },     // Ramp down
    ],
    thresholds: {
        http_req_duration: ['p(95)<2000', 'p(99)<5000'],
        errors: ['rate<0.05'],
        login_duration: ['p(95)<1000'],
    },
};

const TENANTS = ['navajo', 'cherokee', 'maya', 'quechua', 'cree', 'maori', 'guarani'];
const TIERS = ['member', 'resident', 'citizen'];
const API_ENDPOINTS = [
    '/api/space', '/api/telecom', '/api/education', '/api/commerce',
    '/api/governance', '/api/health', '/api/culture', '/api/transport',
    '/api/messaging', '/api/sports', '/api/environment', '/api/employment',
];

function getRandomItem(arr) { return arr[Math.floor(Math.random() * arr.length)]; }

export default function () {
    const tenant = getRandomItem(TENANTS);
    const tier = getRandomItem(TIERS);
    const userId = `loadtest-${__VU}-${__ITER}`;

    group('Health Check', function () {
        const res = http.get(`${BASE_URL}/health`);
        check(res, {
            'health status 200': (r) => r.status === 200,
            'health reports healthy': (r) => r.json('status') === 'healthy',
            'health shows 83 services': (r) => r.json('microservices') === 83,
        });
        errorRate.add(res.status !== 200);
    });

    group('Authentication', function () {
        const loginStart = Date.now();
        const loginRes = http.post(`${BASE_URL}/auth/login`, JSON.stringify({
            userId, tenantId: tenant, tier, roles: ['user']
        }), { headers: { 'Content-Type': 'application/json' } });

        loginDuration.add(Date.now() - loginStart);
        check(loginRes, {
            'login status 200': (r) => r.status === 200,
            'login returns token': (r) => r.json('token') !== undefined,
        });
        errorRate.add(loginRes.status !== 200);

        if (loginRes.status === 200) {
            const token = loginRes.json('token');
            const authHeaders = {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'X-Tenant-Id': tenant,
                    'Content-Type': 'application/json',
                },
            };

            // Profile check
            const meRes = http.get(`${BASE_URL}/auth/me`, authHeaders);
            check(meRes, {
                'me status 200': (r) => r.status === 200,
                'me returns userId': (r) => r.json('userId') === userId,
                'me returns tenant': (r) => r.json('tenantId') === tenant,
            });

            // Token refresh
            const refreshRes = http.post(`${BASE_URL}/auth/refresh`,
                JSON.stringify({ token }), authHeaders);
            check(refreshRes, {
                'refresh status 200': (r) => r.status === 200,
                'refresh returns new token': (r) => r.json('token') !== token,
            });
        }
    });

    group('Service Discovery', function () {
        const res = http.get(`${BASE_URL}/services`);
        check(res, {
            'services status 200': (r) => r.status === 200,
            'services has gateway': (r) => r.json('gateway') !== undefined,
        });
    });

    group('API Endpoints', function () {
        const endpoint = getRandomItem(API_ENDPOINTS);

        // Login for API access
        const loginRes = http.post(`${BASE_URL}/auth/login`, JSON.stringify({
            userId, tenantId: tenant, tier: 'citizen', roles: ['user', 'admin']
        }), { headers: { 'Content-Type': 'application/json' } });

        if (loginRes.status === 200) {
            const token = loginRes.json('token');
            const res = http.get(`${BASE_URL}${endpoint}`, {
                headers: { Authorization: `Bearer ${token}`, 'X-Tenant-Id': tenant },
            });
            check(res, {
                'api responds (not 404)': (r) => r.status !== 404,
                'api latency < 3s': (r) => r.timings.duration < 3000,
            });
            errorRate.add(res.status >= 500);
        }
    });

    sleep(Math.random() * 2 + 0.5);
}

export function handleSummary(data) {
    return {
        'load-test-results.json': JSON.stringify(data, null, 2),
        stdout: textSummary(data, { indent: '  ', enableColors: true }),
    };
}

function textSummary(data, opts) {
    const metrics = data.metrics;
    return `
=== IERAHKWA LOAD TEST RESULTS ===
Duration:         ${data.state.testRunDurationMs}ms
VUs Peak:         ${data.metrics.vus_max?.values?.max || 'N/A'}
Total Requests:   ${metrics.http_reqs?.values?.count || 0}
Requests/sec:     ${metrics.http_reqs?.values?.rate?.toFixed(1) || 0}
Avg Duration:     ${metrics.http_req_duration?.values?.avg?.toFixed(0) || 0}ms
P95 Duration:     ${metrics.http_req_duration?.values?.['p(95)']?.toFixed(0) || 0}ms
P99 Duration:     ${metrics.http_req_duration?.values?.['p(99)']?.toFixed(0) || 0}ms
Error Rate:       ${((metrics.errors?.values?.rate || 0) * 100).toFixed(2)}%
Login P95:        ${metrics.login_duration?.values?.['p(95)']?.toFixed(0) || 0}ms
===================================
`;
}

import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const apiLatency = new Trend('api_latency');

// Test configuration
export const options = {
    stages: [
        { duration: '1m', target: 10 },   // Ramp up to 10 users
        { duration: '3m', target: 50 },   // Ramp up to 50 users
        { duration: '5m', target: 100 },  // Stay at 100 users
        { duration: '2m', target: 50 },   // Ramp down to 50
        { duration: '1m', target: 0 },    // Ramp down to 0
    ],
    thresholds: {
        http_req_duration: ['p(95)<500', 'p(99)<1000'],
        errors: ['rate<0.1'],
        api_latency: ['p(95)<400'],
    },
};

const BASE_URL = __ENV.API_URL || 'http://localhost:60000';

// Helper function to make requests and record metrics
function makeRequest(name, method, url, body = null) {
    const params = {
        headers: { 'Content-Type': 'application/json' },
        tags: { name: name },
    };

    const start = Date.now();
    let response;
    
    if (method === 'GET') {
        response = http.get(url, params);
    } else if (method === 'POST') {
        response = http.post(url, JSON.stringify(body), params);
    } else if (method === 'PUT') {
        response = http.put(url, JSON.stringify(body), params);
    } else if (method === 'DELETE') {
        response = http.del(url, null, params);
    }

    const duration = Date.now() - start;
    apiLatency.add(duration);

    const success = check(response, {
        [`${name} status is 2xx`]: (r) => r.status >= 200 && r.status < 300,
    });

    errorRate.add(!success);
    return response;
}

export default function () {
    // Test Foundation Services
    group('Foundation Services', function () {
        group('Users API', function () {
            makeRequest('List Users', 'GET', `${BASE_URL}/api/users`);
            sleep(0.5);
        });

        group('Lessons API', function () {
            makeRequest('List Lessons', 'GET', `${BASE_URL}/api/lessons`);
            sleep(0.5);
        });

        group('Assessments API', function () {
            makeRequest('List Assessments', 'GET', `${BASE_URL}/api/assessments`);
            sleep(0.5);
        });

        group('Progress API', function () {
            makeRequest('List Progress', 'GET', `${BASE_URL}/api/progress`);
            sleep(0.5);
        });
    });

    // Test AI Services
    group('AI Services', function () {
        group('AI Tutors API', function () {
            makeRequest('List Tutors', 'GET', `${BASE_URL}/api/ai/tutors`);
            sleep(0.5);
        });

        group('AI Recommendations API', function () {
            makeRequest('List Recommendations', 'GET', `${BASE_URL}/api/ai/recommendations`);
            sleep(0.5);
        });
    });

    // Test Support Services
    group('Support Services', function () {
        group('Analytics API', function () {
            makeRequest('List Analytics', 'GET', `${BASE_URL}/api/analytics`);
            sleep(0.5);
        });

        group('Notifications API', function () {
            makeRequest('List Notifications', 'GET', `${BASE_URL}/api/notifications`);
            sleep(0.5);
        });
    });

    sleep(1);
}

export function handleSummary(data) {
    return {
        'stdout': textSummary(data, { indent: ' ', enableColors: true }),
        'summary.json': JSON.stringify(data),
    };
}

function textSummary(data, options) {
    const indent = options.indent || '';
    let summary = '\n=== PUPITRE LOAD TEST SUMMARY ===\n\n';
    
    summary += `${indent}Total Requests: ${data.metrics.http_reqs.values.count}\n`;
    summary += `${indent}Failed Requests: ${data.metrics.http_req_failed.values.passes}\n`;
    summary += `${indent}Avg Response Time: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms\n`;
    summary += `${indent}P95 Response Time: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms\n`;
    summary += `${indent}P99 Response Time: ${data.metrics.http_req_duration.values['p(99)'].toFixed(2)}ms\n`;
    summary += `${indent}Error Rate: ${(data.metrics.errors.values.rate * 100).toFixed(2)}%\n`;
    
    return summary;
}

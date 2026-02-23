// k6 Load Test for Pupitre Users API
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const getUsersTrend = new Trend('get_users_duration');
const createUserTrend = new Trend('create_user_duration');

// Test configuration
export const options = {
  stages: [
    { duration: '30s', target: 10 },   // Warm-up
    { duration: '1m', target: 50 },    // Ramp-up
    { duration: '2m', target: 100 },   // Peak load
    { duration: '1m', target: 50 },    // Ramp-down
    { duration: '30s', target: 0 },    // Cool-down
  ],
  thresholds: {
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
    errors: ['rate<0.01'],
    get_users_duration: ['p(95)<300'],
    create_user_duration: ['p(95)<500'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:60000';
const AUTH_TOKEN = __ENV.AUTH_TOKEN || '';

const headers = {
  'Content-Type': 'application/json',
  'Authorization': `Bearer ${AUTH_TOKEN}`,
};

export default function () {
  // Test: Get Users (list)
  const listResponse = http.get(`${BASE_URL}/api/users?page=1&pageSize=10`, { headers });
  getUsersTrend.add(listResponse.timings.duration);
  
  check(listResponse, {
    'GET /users status is 200': (r) => r.status === 200,
    'GET /users has items': (r) => {
      const body = JSON.parse(r.body);
      return body.items !== undefined;
    },
  }) || errorRate.add(1);

  sleep(1);

  // Test: Get User by ID (if we have items)
  if (listResponse.status === 200) {
    try {
      const users = JSON.parse(listResponse.body);
      if (users.items && users.items.length > 0) {
        const userId = users.items[0].id;
        const getResponse = http.get(`${BASE_URL}/api/users/${userId}`, { headers });
        
        check(getResponse, {
          'GET /users/{id} status is 200': (r) => r.status === 200,
        }) || errorRate.add(1);
      }
    } catch (e) {
      // Ignore parse errors during load test
    }
  }

  sleep(1);

  // Test: Create User (use unique email per VU iteration)
  const uniqueId = `${__VU}-${__ITER}-${Date.now()}`;
  const createPayload = JSON.stringify({
    email: `loadtest-${uniqueId}@pupitre.test`,
    firstName: 'LoadTest',
    lastName: `User${uniqueId}`,
    role: 'Student',
  });

  const createResponse = http.post(`${BASE_URL}/api/users`, createPayload, { headers });
  createUserTrend.add(createResponse.timings.duration);
  
  check(createResponse, {
    'POST /users status is 201 or 200': (r) => r.status === 201 || r.status === 200,
  }) || errorRate.add(1);

  sleep(2);
}

export function handleSummary(data) {
  return {
    'stdout': textSummary(data, { indent: '  ', enableColors: true }),
    'summary.json': JSON.stringify(data),
  };
}

function textSummary(data, options) {
  return `
=== Pupitre Users API Load Test Summary ===

Duration: ${data.state.testRunDurationMs}ms
VUs: ${data.metrics.vus.values.max}
Iterations: ${data.metrics.iterations.values.count}

HTTP Requests:
  Total: ${data.metrics.http_reqs.values.count}
  Rate: ${data.metrics.http_reqs.values.rate.toFixed(2)}/s

Response Times (p95):
  Overall: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms
  Get Users: ${data.metrics.get_users_duration?.values['p(95)']?.toFixed(2) || 'N/A'}ms
  Create User: ${data.metrics.create_user_duration?.values['p(95)']?.toFixed(2) || 'N/A'}ms

Error Rate: ${(data.metrics.errors?.values?.rate * 100 || 0).toFixed(2)}%
`;
}

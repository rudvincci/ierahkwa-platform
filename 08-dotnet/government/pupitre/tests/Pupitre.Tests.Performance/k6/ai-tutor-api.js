// k6 Load Test for Pupitre AI Tutor API
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const chatTrend = new Trend('chat_duration');
const sessionTrend = new Trend('session_duration');

// AI endpoints need lower concurrency and longer timeouts
export const options = {
  stages: [
    { duration: '30s', target: 5 },    // Warm-up
    { duration: '1m', target: 20 },    // Ramp-up
    { duration: '2m', target: 50 },    // Peak load (lower for AI)
    { duration: '1m', target: 20 },    // Ramp-down
    { duration: '30s', target: 0 },    // Cool-down
  ],
  thresholds: {
    http_req_duration: ['p(95)<5000', 'p(99)<10000'], // AI can be slow
    errors: ['rate<0.05'],
    chat_duration: ['p(95)<3000'],
    session_duration: ['p(95)<1000'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:60000';
const AUTH_TOKEN = __ENV.AUTH_TOKEN || '';

const headers = {
  'Content-Type': 'application/json',
  'Authorization': `Bearer ${AUTH_TOKEN}`,
};

// Sample questions for testing
const sampleQuestions = [
  "What is 2 + 2?",
  "Explain photosynthesis in simple terms.",
  "What is the capital of France?",
  "How do fractions work?",
  "Why is the sky blue?",
  "What is a noun?",
  "Explain the water cycle.",
  "What is gravity?",
];

export default function () {
  // Test: Create Tutor Session
  const sessionPayload = JSON.stringify({
    studentId: `student-${__VU}`,
    subject: 'Mathematics',
    gradeLevel: 5,
  });

  const sessionResponse = http.post(
    `${BASE_URL}/api/ai/tutors/sessions`,
    sessionPayload,
    { headers, timeout: '10s' }
  );
  sessionTrend.add(sessionResponse.timings.duration);

  check(sessionResponse, {
    'POST /sessions status is 200/201': (r) => r.status === 200 || r.status === 201,
  }) || errorRate.add(1);

  let sessionId = null;
  if (sessionResponse.status === 200 || sessionResponse.status === 201) {
    try {
      const session = JSON.parse(sessionResponse.body);
      sessionId = session.id || session.sessionId;
    } catch (e) {
      // Continue without session
    }
  }

  sleep(1);

  // Test: Chat with Tutor
  if (sessionId) {
    const question = sampleQuestions[Math.floor(Math.random() * sampleQuestions.length)];
    const chatPayload = JSON.stringify({
      sessionId: sessionId,
      message: question,
    });

    const chatResponse = http.post(
      `${BASE_URL}/api/ai/tutors/chat`,
      chatPayload,
      { headers, timeout: '30s' }  // AI responses can take time
    );
    chatTrend.add(chatResponse.timings.duration);

    check(chatResponse, {
      'POST /chat status is 200': (r) => r.status === 200,
      'POST /chat has response': (r) => {
        if (r.status !== 200) return false;
        try {
          const body = JSON.parse(r.body);
          return body.response !== undefined || body.message !== undefined;
        } catch (e) {
          return false;
        }
      },
    }) || errorRate.add(1);
  }

  sleep(3);  // Longer wait between AI requests
}

export function handleSummary(data) {
  return {
    'ai-tutor-summary.json': JSON.stringify(data),
  };
}

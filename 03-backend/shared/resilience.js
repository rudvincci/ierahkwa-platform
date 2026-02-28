'use strict';

// ============================================================
// Ierahkwa Platform — Resilience Patterns v1.0.0
// Circuit Breaker, Retry with Backoff, Timeout, Bulkhead
// Zero external dependencies
// ============================================================

/**
 * Circuit Breaker — prevents cascading failures
 *
 * States:
 *   CLOSED  → Normal operation, requests pass through
 *   OPEN    → Requests fail immediately (downstream is down)
 *   HALF_OPEN → Test request to check if downstream recovered
 *
 * Usage:
 *   const breaker = createCircuitBreaker('bdet-bank-api', { threshold: 5 });
 *   const result = await breaker.execute(() => fetch('/api/bdet/balance'));
 */
function createCircuitBreaker(name, options = {}) {
  const {
    threshold = 5,          // failures before opening
    resetTimeout = 30000,   // ms before trying half-open (30s)
    monitorInterval = 60000, // ms between stat resets (1min)
    onStateChange = null     // callback(name, oldState, newState)
  } = options;

  let state = 'CLOSED';
  let failures = 0;
  let successes = 0;
  let lastFailureTime = null;
  let halfOpenAttempts = 0;

  // Stats
  const stats = { total: 0, success: 0, failure: 0, rejected: 0, timeouts: 0 };

  // Reset stats periodically
  const statsInterval = setInterval(() => {
    stats.total = 0;
    stats.success = 0;
    stats.failure = 0;
    stats.rejected = 0;
    stats.timeouts = 0;
  }, monitorInterval);
  if (statsInterval.unref) statsInterval.unref();

  function changeState(newState) {
    const oldState = state;
    if (oldState !== newState) {
      state = newState;
      if (onStateChange) onStateChange(name, oldState, newState);
    }
  }

  async function execute(fn) {
    stats.total++;

    // OPEN — fail fast
    if (state === 'OPEN') {
      const elapsed = Date.now() - lastFailureTime;
      if (elapsed >= resetTimeout) {
        changeState('HALF_OPEN');
        halfOpenAttempts = 0;
      } else {
        stats.rejected++;
        throw new CircuitBreakerError(name, state, `Circuit open — retry in ${Math.ceil((resetTimeout - elapsed) / 1000)}s`);
      }
    }

    // HALF_OPEN — limit concurrent test requests
    if (state === 'HALF_OPEN' && halfOpenAttempts >= 1) {
      stats.rejected++;
      throw new CircuitBreakerError(name, state, 'Circuit half-open — test request in progress');
    }

    try {
      if (state === 'HALF_OPEN') halfOpenAttempts++;
      const result = await fn();

      // Success
      stats.success++;
      failures = 0;
      successes++;

      if (state === 'HALF_OPEN') {
        changeState('CLOSED');
        successes = 0;
      }

      return result;
    } catch (err) {
      stats.failure++;
      failures++;
      lastFailureTime = Date.now();

      if (state === 'HALF_OPEN') {
        changeState('OPEN');
      } else if (failures >= threshold) {
        changeState('OPEN');
      }

      throw err;
    }
  }

  return {
    execute,
    getState: () => state,
    getStats: () => ({ ...stats, state, name, failures, threshold }),
    reset: () => { changeState('CLOSED'); failures = 0; successes = 0; }
  };
}

class CircuitBreakerError extends Error {
  constructor(breakerName, state, message) {
    super(message);
    this.name = 'CircuitBreakerError';
    this.breakerName = breakerName;
    this.circuitState = state;
    this.statusCode = 503;
  }
}

/**
 * Retry with Exponential Backoff
 *
 * Usage:
 *   const result = await retry(() => fetch('/api/data'), {
 *     retries: 3,
 *     baseDelay: 1000,
 *     onRetry: (err, attempt) => console.log(`Retry ${attempt}`)
 *   });
 */
async function retry(fn, options = {}) {
  const {
    retries = 3,
    baseDelay = 1000,
    maxDelay = 30000,
    factor = 2,
    jitter = true,
    retryOn = null,  // function(err) => boolean — should retry on this error?
    onRetry = null   // callback(err, attempt, delay)
  } = options;

  let lastError;

  for (let attempt = 0; attempt <= retries; attempt++) {
    try {
      return await fn(attempt);
    } catch (err) {
      lastError = err;

      // Don't retry on last attempt
      if (attempt >= retries) break;

      // Check if this error is retryable
      if (retryOn && !retryOn(err)) break;

      // Don't retry on client errors (4xx)
      if (err.statusCode && err.statusCode >= 400 && err.statusCode < 500) break;

      // Calculate delay with exponential backoff + jitter
      let delay = Math.min(baseDelay * Math.pow(factor, attempt), maxDelay);
      if (jitter) {
        delay = delay * (0.5 + Math.random() * 0.5); // 50-100% of calculated delay
      }

      if (onRetry) onRetry(err, attempt + 1, Math.round(delay));

      await new Promise(resolve => setTimeout(resolve, delay));
    }
  }

  throw lastError;
}

/**
 * Timeout wrapper — fails if operation exceeds time limit
 *
 * Usage:
 *   const result = await withTimeout(() => fetch('/api/data'), 5000);
 */
async function withTimeout(fn, ms, message) {
  return new Promise((resolve, reject) => {
    const timer = setTimeout(() => {
      reject(new TimeoutError(message || `Operation timed out after ${ms}ms`, ms));
    }, ms);

    Promise.resolve(fn())
      .then(result => {
        clearTimeout(timer);
        resolve(result);
      })
      .catch(err => {
        clearTimeout(timer);
        reject(err);
      });
  });
}

class TimeoutError extends Error {
  constructor(message, timeout) {
    super(message);
    this.name = 'TimeoutError';
    this.timeout = timeout;
    this.statusCode = 504;
  }
}

/**
 * Bulkhead — limits concurrent operations to prevent resource exhaustion
 *
 * Usage:
 *   const pool = createBulkhead('db-queries', { maxConcurrent: 10, maxQueued: 50 });
 *   const result = await pool.execute(() => db.query('SELECT...'));
 */
function createBulkhead(name, options = {}) {
  const {
    maxConcurrent = 10,
    maxQueued = 50,
    queueTimeout = 30000
  } = options;

  let active = 0;
  const queue = [];

  function tryNext() {
    if (active >= maxConcurrent || queue.length === 0) return;

    const { fn, resolve, reject, timer } = queue.shift();
    active++;

    if (timer) clearTimeout(timer);

    Promise.resolve(fn())
      .then(resolve)
      .catch(reject)
      .finally(() => {
        active--;
        tryNext();
      });
  }

  function execute(fn) {
    return new Promise((resolve, reject) => {
      if (active < maxConcurrent) {
        active++;
        Promise.resolve(fn())
          .then(resolve)
          .catch(reject)
          .finally(() => {
            active--;
            tryNext();
          });
      } else if (queue.length < maxQueued) {
        const timer = setTimeout(() => {
          const idx = queue.findIndex(q => q.timer === timer);
          if (idx >= 0) queue.splice(idx, 1);
          reject(new Error(`Bulkhead ${name}: queue timeout after ${queueTimeout}ms`));
        }, queueTimeout);

        queue.push({ fn, resolve, reject, timer });
      } else {
        reject(new Error(`Bulkhead ${name}: queue full (${maxQueued})`));
      }
    });
  }

  return {
    execute,
    getStats: () => ({ name, active, queued: queue.length, maxConcurrent, maxQueued })
  };
}

/**
 * Resilient HTTP client — combines circuit breaker + retry + timeout
 *
 * Usage:
 *   const client = createResilientClient('bdet-api', {
 *     baseUrl: 'http://bdet-service:8080',
 *     timeout: 5000,
 *     retries: 3
 *   });
 *   const data = await client.get('/v1/balance/0x123');
 */
function createResilientClient(name, options = {}) {
  const {
    baseUrl = '',
    timeout = 10000,
    retries: retryCount = 3,
    circuitThreshold = 5,
    circuitReset = 30000,
    headers = {},
    onCircuitChange = null
  } = options;

  const breaker = createCircuitBreaker(name, {
    threshold: circuitThreshold,
    resetTimeout: circuitReset,
    onStateChange: onCircuitChange
  });

  async function request(method, path, body) {
    return breaker.execute(() =>
      retry(() =>
        withTimeout(async () => {
          const url = `${baseUrl}${path}`;
          const opts = {
            method,
            headers: { 'Content-Type': 'application/json', ...headers }
          };

          if (body && (method === 'POST' || method === 'PUT' || method === 'PATCH')) {
            opts.body = JSON.stringify(body);
          }

          const res = await fetch(url, opts);

          if (!res.ok) {
            const err = new Error(`${name}: ${method} ${path} returned ${res.status}`);
            err.statusCode = res.status;
            try { err.body = await res.json(); } catch {}
            throw err;
          }

          return res.json();
        }, timeout),
        { retries: retryCount, baseDelay: 500 }
      )
    );
  }

  return {
    get:    (path) => request('GET', path),
    post:   (path, body) => request('POST', path, body),
    put:    (path, body) => request('PUT', path, body),
    patch:  (path, body) => request('PATCH', path, body),
    delete: (path) => request('DELETE', path),
    getCircuit: () => breaker.getStats()
  };
}

// ============================================================
// Exports
// ============================================================
module.exports = {
  createCircuitBreaker,
  CircuitBreakerError,
  retry,
  withTimeout,
  TimeoutError,
  createBulkhead,
  createResilientClient
};

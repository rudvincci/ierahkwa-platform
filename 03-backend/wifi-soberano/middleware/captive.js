'use strict';

/**
 * Captive Portal Middleware
 * Redirects unauthenticated users to the WiFi portal login page.
 * Checks Redis for active session by IP address.
 */
module.exports = function captiveMiddleware(redis) {
  return async (req, res, next) => {
    // Skip API routes and health checks
    if (req.path.startsWith('/api/') || req.path === '/health') return next();

    const ip = req.headers['x-forwarded-for']?.split(',')[0] || req.socket.remoteAddress;

    try {
      const session = await redis.get(`wifi:session:${ip}`);
      if (session) {
        const parsed = JSON.parse(session);
        const remaining = new Date(parsed.expires) - Date.now();
        if (remaining > 0) {
          // Active session — allow through
          req.wifiSession = parsed;
          return next();
        }
      }
    } catch (err) {
      // Redis error — allow through to prevent lockout
      return next();
    }

    // No active session — redirect to captive portal
    res.redirect(302, '/wifi/portal');
  };
};

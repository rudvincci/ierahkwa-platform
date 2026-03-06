'use strict';

/**
 * Bandwidth Middleware
 * Enforces bandwidth limits based on user's WiFi plan.
 * Sets response headers for downstream proxy (nginx) to enforce.
 */
module.exports = function bandwidthMiddleware(redis) {
  return async (req, res, next) => {
    const ip = req.headers['x-forwarded-for']?.split(',')[0] || req.socket.remoteAddress;

    try {
      const session = await redis.get(`wifi:session:${ip}`);
      if (session) {
        const parsed = JSON.parse(session);
        // Set bandwidth limit header for nginx proxy
        res.setHeader('X-WiFi-Bandwidth', parsed.bandwidth || 50);
        res.setHeader('X-WiFi-Plan', parsed.plan || 'basic');
        res.setHeader('X-WiFi-Session', parsed.id || '');
      }
    } catch (err) {
      // Continue without bandwidth headers
    }

    next();
  };
};

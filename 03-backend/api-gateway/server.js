const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const morgan = require('morgan');
const { RateLimiterMemory } = require('rate-limiter-flexible');

const app = express();
const PORT = process.env.PORT || 3000;

// Security & Middleware
app.use(helmet());
const corsOptions = require('../shared/security').corsConfig();
app.use(cors(corsOptions));
app.use(compression());
app.use(morgan('combined'));
app.use(express.json({ limit: '10mb' }));

// Rate Limiting
const rateLimiter = new RateLimiterMemory({ points: 100, duration: 60 });
app.use(async (req, res, next) => {
  try { await rateLimiter.consume(req.ip); next(); }
  catch { res.status(429).json({ error: 'Too many requests' }); }
});

// Routes
app.use('/v1/auth', require('./routes/auth'));
app.use('/v1/bdet', require('./routes/bdet'));
app.use('/v1/mail', require('./routes/mail'));
app.use('/v1/social', require('./routes/social'));
app.use('/v1/search', require('./routes/search'));
app.use('/v1/video', require('./routes/video'));
app.use('/v1/music', require('./routes/music'));
app.use('/v1/lodging', require('./routes/lodging'));
app.use('/v1/artisan', require('./routes/artisan'));
app.use('/v1/commerce', require('./routes/commerce'));
app.use('/v1/invest', require('./routes/invest'));
app.use('/v1/docs', require('./routes/docs'));
app.use('/v1/map', require('./routes/map'));
app.use('/v1/voice', require('./routes/voice'));
app.use('/v1/jobs', require('./routes/jobs'));
app.use('/v1/renta', require('./routes/renta'));
app.use('/v1/wiki', require('./routes/wiki'));
app.use('/v1/edu', require('./routes/edu'));
app.use('/v1/news', require('./routes/news'));
app.use('/v1/atabey', require('./routes/atabey'));
app.use('/v1/ai', require('./routes/ai'));
app.use('/v1/chain', require('./routes/chain'));

// Health
app.get('/health', (_, res) => res.json({ status: 'ok', version: '1.0.0', node: 'MameyNode v4.2' }));
app.get('/ready', (_, res) => res.json({ ready: true, platforms: 98, engines: 42 }));

// Error handler
app.use((err, req, res, next) => {
  console.error(err.stack);
  res.status(500).json({ error: 'Internal server error', id: require('uuid').v4() });
});

app.listen(PORT, () => console.log(`🌐 Red Soberana API v1.0 running on port ${PORT} · 98 platforms · MameyNode v4.2`));
module.exports = app;

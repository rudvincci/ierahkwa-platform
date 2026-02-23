const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const http = require('http');
const { WebSocketServer } = require('ws');

const app = express();
const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

app.use(helmet());
app.use(cors());
app.use(express.json({ limit: '50mb' }));

// Social Media Routes
app.use('/v1/feed', require('./routes/feed'));
app.use('/v1/posts', require('./routes/posts'));
app.use('/v1/stories', require('./routes/stories'));
app.use('/v1/comments', require('./routes/comments'));
app.use('/v1/likes', require('./routes/likes'));
app.use('/v1/follow', require('./routes/follow'));
app.use('/v1/profiles', require('./routes/profiles'));
app.use('/v1/groups', require('./routes/groups'));
app.use('/v1/chat', require('./routes/chat'));
app.use('/v1/notifications', require('./routes/notifications'));
app.use('/v1/media', require('./routes/media'));
app.use('/v1/live', require('./routes/live'));
app.use('/v1/hashtags', require('./routes/hashtags'));
app.use('/v1/search', require('./routes/search'));

app.get('/health', (req, res) => res.json({
  service: 'RedSoberana Social Media', version: '4.2.0', status: 'operational',
  features: ['posts','stories','feed','chat','groups','live','video','photos'],
  creatorRevenue: '92%', ads: 'ZERO forever', tracking: 'NONE',
  encryption: 'Post-quantum E2E', languages: 14, taxRate: '0%'
}));

// WebSocket for real-time chat, notifications, live
require('./services/realtime')(wss);

const PORT = process.env.SOCIAL_PORT || 4001;
server.listen(PORT, () => console.log('👥 RedSoberana Social live on :' + PORT + ' — 0 ads — 0 tracking — 92% to creators'));

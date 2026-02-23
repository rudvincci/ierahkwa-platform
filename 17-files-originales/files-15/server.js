const express = require('express');
const cors = require('cors');
const http = require('http');
const { WebSocketServer } = require('ws');
const app = express();
const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

app.use(cors());
app.use(express.json());

app.use('/v1/providers', require('./routes/providers'));
app.use('/v1/services', require('./routes/services'));
app.use('/v1/bookings', require('./routes/bookings'));
app.use('/v1/reviews', require('./routes/reviews'));
app.use('/v1/categories', require('./routes/categories'));
app.use('/v1/availability', require('./routes/availability'));
app.use('/v1/locations', require('./routes/locations'));

app.get('/health', (req, res) => res.json({
  service: 'SoberanoServicios',
  version: '4.2.0',
  status: 'operational',
  categories: 30,
  description: 'Book ANY service — barbers, tattoo, massage, delivery, plumber, electrician, chef, tutor, photographer, and more',
  providerPercent: '92%',
  taxRate: '0%',
  languages: 43
}));

// WebSocket — real-time booking updates, provider location
require('./services/realtime')(wss);

const PORT = process.env.SERVICIOS_PORT || 4010;
server.listen(PORT, () => console.log('🔧 SoberanoServicios on :' + PORT + ' — 92% to providers — 0% taxes'));

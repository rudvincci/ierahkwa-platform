const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');
const cors = require('cors');
const helmet = require('helmet');
const app = express();

const { corsConfig } = require('../shared/security');
app.use(helmet());
app.use(cors(corsConfig()));

// Route to microservices
const SERVICES = {
  '/v1/wallet': 'http://bdet-bank:4000',
  '/v1/payments': 'http://bdet-bank:4000',
  '/v1/exchange': 'http://bdet-bank:4000',
  '/v1/trading': 'http://bdet-bank:4000',
  '/v1/remittance': 'http://bdet-bank:4000',
  '/v1/escrow': 'http://bdet-bank:4000',
  '/v1/loans': 'http://bdet-bank:4000',
  '/v1/insurance': 'http://bdet-bank:4000',
  '/v1/staking': 'http://bdet-bank:4000',
  '/v1/treasury': 'http://bdet-bank:4000',
  '/v1/fiscal': 'http://bdet-bank:4000',
  '/v1/feed': 'http://social-media:4001',
  '/v1/posts': 'http://social-media:4001',
  '/v1/stories': 'http://social-media:4001',
  '/v1/comments': 'http://social-media:4001',
  '/v1/likes': 'http://social-media:4001',
  '/v1/follow': 'http://social-media:4001',
  '/v1/profiles': 'http://social-media:4001',
  '/v1/groups': 'http://social-media:4001',
  '/v1/chat': 'http://social-media:4001',
  '/v1/notifications': 'http://social-media:4001',
  '/v1/live': 'http://social-media:4001',
  '/v1/doctor': 'http://soberano-doctor:4002',
  '/v1/education': 'http://pupitresoberano:4003',
  '/v1/rides': 'http://soberano-uber:4004',
  '/v1/food': 'http://soberano-eats:4005',
  '/v1/vote': 'http://voto-soberano:4006',
  '/v1/disputes': 'http://justicia-soberano:4007',
  '/v1/census': 'http://censo-soberano:4008',
  '/v1/identity': 'http://soberano-id:4009',
  '/v1/services': 'http://soberano-servicios:4010',
  '/v1/bookings': 'http://soberano-servicios:4010',
  '/v1/mail': 'http://correo-soberano:4011',
  '/v1/search': 'http://busqueda-soberana:4012',
  '/v1/maps': 'http://mapa-soberano:4013',
  '/v1/cloud': 'http://nube-soberana:4014',
  '/v1/farm': 'http://soberano-farm:4015',
  '/v1/radio': 'http://radio-soberana:4016',
  '/v1/cooperatives': 'http://cooperativa-soberana:4017',
  '/v1/tourism': 'http://turismo-soberano:4018',
  '/v1/freelance': 'http://soberano-freelance:4019',
  '/v1/pos': 'http://soberano-pos:4020',
};

for (const [path, target] of Object.entries(SERVICES)) {
  app.use(path, createProxyMiddleware({ target, changeOrigin: true }));
}

app.get('/health', async (req, res) => {
  const checks = {};
  for (const [path, target] of Object.entries(SERVICES)) {
    try { await fetch(target + '/health'); checks[path] = 'healthy'; }
    catch (e) { checks[path] = 'down'; }
  }
  const healthy = Object.values(checks).filter(v => v === 'healthy').length;
  res.json({ gateway: 'Red Soberana API Gateway', services: Object.keys(SERVICES).length, healthy, checks, taxRate: '0%' });
});

app.listen(3000, () => console.log('🌐 Gateway on :3000 — ' + Object.keys(SERVICES).length + ' routes → 19 microservices'));

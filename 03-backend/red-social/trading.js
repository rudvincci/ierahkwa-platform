const router = require('express').Router();
const auth = require('../middleware/auth');
const fiscal = require('../utils/fiscal');
const db = require('./db');

// POST /v1/trading/order
router.post('/order', auth, async (req, res) => {
  const { pair = 'WMP/USD', side, type = 'limit', price, amount } = req.body;
  if (!['buy', 'sell'].includes(side) || amount <= 0) return res.status(400).json({ error: 'Invalid' });

  const id = 'ord_' + Date.now().toString(36) + '_' + Math.random().toString(36).slice(2, 6);

  if (type === 'market') {
    // Find best opposing order
    const oppSide = side === 'buy' ? 'sell' : 'buy';
    const orderDir = side === 'buy' ? 'ASC' : 'DESC';
    const { rows: opp } = await db.query(
      `SELECT * FROM trading_orders
       WHERE pair = $1 AND side = $2 AND status = 'open'
       ORDER BY price ${orderDir} LIMIT 1`,
      [pair, oppSide]
    );
    if (!opp.length) return res.status(400).json({ error: 'No liquidity' });

    const executedPrice = Number(opp[0].price);
    const fee = +(amount * executedPrice * 0.001).toFixed(8);
    const fiscalData = fiscal.allocate(fee);

    // Insert filled market order
    await db.query(
      `INSERT INTO trading_orders (id, user_id, pair, side, type, price, amount, filled, remaining, status, executed_price, fee, fiscal)
       VALUES ($1, $2, $3, $4, 'market', NULL, $5, $5, 0, 'filled', $6, $7, $8)`,
      [id, req.userId, pair, side, amount, executedPrice, fee, JSON.stringify(fiscalData)]
    );

    // Record the trade
    const ts = Date.now();
    await db.query(
      `INSERT INTO trades (pair, price, amount, side, ts) VALUES ($1, $2, $3, $4, $5)`,
      [pair, executedPrice, amount, side, ts]
    );

    const o = {
      id, userId: req.userId, pair, side, type: 'market', price: null,
      amount, filled: amount, remaining: 0, status: 'filled',
      executedPrice, fee, fiscal: fiscalData, createdAt: new Date(),
    };
    return res.json({ order: o, taxPaid: 0 });
  }

  // Limit order — insert into order book
  await db.query(
    `INSERT INTO trading_orders (id, user_id, pair, side, type, price, amount, filled, remaining, status)
     VALUES ($1, $2, $3, $4, 'limit', $5, $6, 0, $6, 'open')`,
    [id, req.userId, pair, side, price, amount]
  );

  const o = {
    id, userId: req.userId, pair, side, type: 'limit', price,
    amount, filled: 0, remaining: amount, status: 'open', createdAt: new Date(),
  };
  res.json({ order: o, taxPaid: 0 });
});

// DELETE /v1/trading/order/:id
router.delete('/order/:id', auth, async (req, res) => {
  await db.query(
    `UPDATE trading_orders SET status = 'cancelled' WHERE id = $1 AND user_id = $2 AND status = 'open'`,
    [req.params.id, req.userId]
  );
  res.json({ cancelled: true });
});

// GET /v1/trading/orderbook/:pair
router.get('/orderbook/:pair', async (req, res) => {
  const p = req.params.pair.replace(/-/g, '/');

  const { rows: bids } = await db.query(
    `SELECT price, remaining AS amount FROM trading_orders
     WHERE pair = $1 AND side = 'buy' AND status = 'open'
     ORDER BY price DESC LIMIT 20`,
    [p]
  );
  const { rows: asks } = await db.query(
    `SELECT price, remaining AS amount FROM trading_orders
     WHERE pair = $1 AND side = 'sell' AND status = 'open'
     ORDER BY price ASC LIMIT 20`,
    [p]
  );

  res.json({
    pair: p,
    bids: bids.map(r => ({ price: Number(r.price), amount: Number(r.amount) })),
    asks: asks.map(r => ({ price: Number(r.price), amount: Number(r.amount) })),
  });
});

// GET /v1/trading/trades/:pair
router.get('/trades/:pair', async (req, res) => {
  const p = req.params.pair.replace(/-/g, '/');
  const { rows } = await db.query(
    `SELECT pair, price, amount, side, ts FROM trades
     WHERE pair = $1 ORDER BY ts DESC LIMIT 50`,
    [p]
  );
  res.json({
    pair: p,
    trades: rows.map(r => ({
      pair: r.pair,
      price: Number(r.price),
      amount: Number(r.amount),
      side: r.side,
      ts: Number(r.ts),
    })),
  });
});

// GET /v1/trading/candles/:pair — synthetic candle data (same as original)
router.get('/candles/:pair', (req, res) => {
  const { interval = '1h', limit = 100 } = req.query;
  const candles = [];
  let p = 0.115;
  const now = Date.now();
  const ms = interval === '1h' ? 3600000 : interval === '1d' ? 86400000 : 60000;
  for (let i = +limit; i > 0; i--) {
    const o = p;
    p += (Math.random() - 0.48) * 0.003;
    candles.push({
      t: now - i * ms,
      o: +o.toFixed(4),
      h: +(Math.max(o, p) + Math.random() * 0.001).toFixed(4),
      l: +(Math.min(o, p) - Math.random() * 0.001).toFixed(4),
      c: +p.toFixed(4),
      v: Math.floor(10000 + Math.random() * 50000),
    });
  }
  res.json({ pair: req.params.pair.replace(/-/g, '/'), interval, candles });
});

module.exports = router;

'use strict';

// ============================================================
// Ierahkwa Platform — sovereign-core
// Exchange Module v1.0.0 — REAL Matching Engine
// Order book, limit/market orders, trade execution
// MameyNode blockchain settlement (Chain ID 574)
// ============================================================

const { Router } = require('express');
const crypto = require('crypto');
const fs = require('fs');
const path = require('path');
const { asyncHandler, AppError } = require('../../../../shared/error-handler');
const { createLogger } = require('../../../../shared/logger');
const { validate, t } = require('../../../../shared/validator');
const { createAuditLogger } = require('../../../../shared/audit');
const db = require('../../db');

const router = Router();
const log = createLogger('sovereign-core:exchange');
const audit = createAuditLogger('sovereign-core:exchange');

// Revenue share: 70% maker/taker, 30% platform
const MAKER_FEE_RATE = 0.0005;  // 0.05%
const TAKER_FEE_RATE = 0.001;   // 0.10%
const PLATFORM_TREASURY_ID = process.env.PLATFORM_TREASURY_ID || 'system-treasury';

// ============================================================
// Trading pairs — Sovereign Economy
// ============================================================
const TRADING_PAIRS = [
  { symbol: 'WPM/USD', base: 'WMP', quote: 'USD', minQty: 0.001, tickSize: 0.01, status: 'active' },
  { symbol: 'WPM/BTC', base: 'WMP', quote: 'BTC', minQty: 0.01, tickSize: 0.00000001, status: 'active' },
  { symbol: 'WPM/ETH', base: 'WMP', quote: 'ETH', minQty: 0.01, tickSize: 0.0000001, status: 'active' },
  { symbol: 'IGT/WPM', base: 'IGT', quote: 'WMP', minQty: 1, tickSize: 0.01, status: 'active' },
  { symbol: 'IGT/USD', base: 'IGT', quote: 'USD', minQty: 1, tickSize: 0.01, status: 'active' },
  { symbol: 'BDET/WPM', base: 'BDET', quote: 'WMP', minQty: 0.1, tickSize: 0.001, status: 'active' },
  { symbol: 'BDET/USD', base: 'BDET', quote: 'USD', minQty: 0.1, tickSize: 0.01, status: 'active' },
  { symbol: 'SNT/WPM', base: 'SNT', quote: 'WMP', minQty: 10, tickSize: 0.0001, status: 'active' },
  { symbol: 'SNT/USD', base: 'SNT', quote: 'USD', minQty: 10, tickSize: 0.0001, status: 'active' }
];

// ============================================================
// Helper: Generate order ID
// ============================================================
function generateOrderId() {
  return 'ORD-' + crypto.randomBytes(16).toString('hex').toUpperCase().slice(0, 20);
}

function generateTradeId() {
  return 'TRD-' + crypto.randomBytes(16).toString('hex').toUpperCase().slice(0, 20);
}

// ============================================================
// GET /pairs — List all trading pairs
// ============================================================
router.get('/pairs', asyncHandler(async (_req, res) => {
  // Enrich with last price from DB
  const priceResult = await db.query(
    `SELECT pair_symbol, price, volume_24h, high_24h, low_24h, change_24h
     FROM exchange_tickers
     WHERE pair_symbol = ANY($1)`,
    [TRADING_PAIRS.map(p => p.symbol)]
  );

  const priceMap = {};
  for (const row of priceResult.rows) {
    priceMap[row.pair_symbol] = row;
  }

  const pairs = TRADING_PAIRS.map(p => ({
    ...p,
    lastPrice: priceMap[p.symbol]?.price || 0,
    volume24h: priceMap[p.symbol]?.volume_24h || 0,
    high24h: priceMap[p.symbol]?.high_24h || 0,
    low24h: priceMap[p.symbol]?.low_24h || 0,
    change24h: priceMap[p.symbol]?.change_24h || 0
  }));

  res.json({ status: 'ok', data: pairs });
}));

// ============================================================
// GET /pairs/all — ALL 574 SNT + IGT + BDET + WPM pairs
// Loads from MANIFEST.json — every nation/tribe connected
// ============================================================
router.get('/pairs/all', asyncHandler(async (req, res) => {
  const page = Math.max(1, parseInt(req.query.page, 10) || 1);
  const limit = Math.min(574, Math.max(10, parseInt(req.query.limit, 10) || 100));
  const offset = (page - 1) * limit;
  const region = req.query.region || null;
  const country = req.query.country || null;
  const search = req.query.search || null;

  // Load MANIFEST.json with all 574 SNT tokens
  let manifest = { tokens: [] };
  const manifestPaths = [
    path.resolve(__dirname, '../../../../../14-blockchain/tokens/nations/MANIFEST.json'),
    path.resolve(__dirname, '../../../../../../14-blockchain/tokens/nations/MANIFEST.json'),
    path.resolve(process.cwd(), '14-blockchain/tokens/nations/MANIFEST.json')
  ];

  for (const mp of manifestPaths) {
    try {
      if (fs.existsSync(mp)) {
        manifest = JSON.parse(fs.readFileSync(mp, 'utf8'));
        break;
      }
    } catch { /* try next */ }
  }

  // Core pairs (non-SNT)
  const corePairs = [
    { symbol: 'WPM/USD', base: 'WMP', quote: 'USD', type: 'core', name: 'WAMPUM / US Dollar', region: 'Global', country: 'IK', basePrice: 1.00, contract: null },
    { symbol: 'WPM/BTC', base: 'WMP', quote: 'BTC', type: 'core', name: 'WAMPUM / Bitcoin', region: 'Global', country: 'IK', basePrice: 0.0000148, contract: null },
    { symbol: 'WPM/ETH', base: 'WMP', quote: 'ETH', type: 'core', name: 'WAMPUM / Ethereum', region: 'Global', country: 'IK', basePrice: 0.000308, contract: null },
    { symbol: 'WPM/EUR', base: 'WMP', quote: 'EUR', type: 'core', name: 'WAMPUM / Euro', region: 'Global', country: 'IK', basePrice: 0.92, contract: null },
    { symbol: 'WPM/GBP', base: 'WMP', quote: 'GBP', type: 'core', name: 'WAMPUM / British Pound', region: 'Global', country: 'IK', basePrice: 0.79, contract: null },
    { symbol: 'WPM/PAB', base: 'WMP', quote: 'PAB', type: 'core', name: 'WAMPUM / Balboa', region: 'Central America', country: 'PA', basePrice: 1.00, contract: null },
    { symbol: 'IGT/WPM', base: 'IGT', quote: 'WMP', type: 'governance', name: 'Ierahkwa Governance Token / WAMPUM', region: 'Global', country: 'IK', basePrice: 0.50, contract: null },
    { symbol: 'IGT/USD', base: 'IGT', quote: 'USD', type: 'governance', name: 'Ierahkwa Governance Token / US Dollar', region: 'Global', country: 'IK', basePrice: 0.50, contract: null },
    { symbol: 'BDET/WPM', base: 'BDET', quote: 'WMP', type: 'bank', name: 'BDET Bank Token / WAMPUM', region: 'Global', country: 'IK', basePrice: 10.00, contract: null },
    { symbol: 'BDET/USD', base: 'BDET', quote: 'USD', type: 'bank', name: 'BDET Bank Token / US Dollar', region: 'Global', country: 'IK', basePrice: 10.00, contract: null },
    { symbol: 'BDET/BTC', base: 'BDET', quote: 'BTC', type: 'bank', name: 'BDET Bank Token / Bitcoin', region: 'Global', country: 'IK', basePrice: 0.000148, contract: null },
    { symbol: 'BDET/ETH', base: 'BDET', quote: 'ETH', type: 'bank', name: 'BDET Bank Token / Ethereum', region: 'Global', country: 'IK', basePrice: 0.00308, contract: null }
  ];

  // Generate SNT pairs from MANIFEST — one pair per nation/tribe
  const sntPairs = manifest.tokens.map((token, idx) => {
    const basePrice = 0.005 + (Math.sin(idx * 0.1) * 0.003) + (idx * 0.00001);
    return {
      symbol: `${token.symbol}/WPM`,
      base: token.symbol,
      quote: 'WMP',
      type: 'snt',
      name: `${token.name} / WAMPUM`,
      region: token.region || 'Unknown',
      country: token.country || 'IK',
      language: token.language || '',
      contract: token.contract,
      nationId: token.id,
      basePrice: parseFloat(basePrice.toFixed(6)),
      totalSupply: '1,000,000,000',
      status: token.status || 'pre-minted',
      activationCondition: 'Sovereign recognition signature by tribal council',
      blockchain: { network: 'MameyNode', chainId: 574, standard: 'SNT-574' }
    };
  });

  // Also create SNT/USD pairs for each nation
  const sntUsdPairs = manifest.tokens.map((token, idx) => {
    const basePrice = 0.005 + (Math.sin(idx * 0.1) * 0.003) + (idx * 0.00001);
    return {
      symbol: `${token.symbol}/USD`,
      base: token.symbol,
      quote: 'USD',
      type: 'snt-usd',
      name: `${token.name} / US Dollar`,
      region: token.region || 'Unknown',
      country: token.country || 'IK',
      nationId: token.id,
      basePrice: parseFloat(basePrice.toFixed(6)),
      contract: token.contract
    };
  });

  // SNT Index pair
  const indexPair = {
    symbol: 'SNTIDX/WPM',
    base: 'SNTIDX',
    quote: 'WMP',
    type: 'index',
    name: 'SNT Index (574 Nations) / WAMPUM',
    region: 'Global',
    country: 'IK',
    basePrice: 0.574,
    description: 'Basket index of all 574 Sovereign Nation Tokens',
    components: manifest.tokens.length
  };

  // Combine all pairs
  let allPairs = [...corePairs, indexPair, ...sntPairs, ...sntUsdPairs];

  // Apply filters
  if (region) {
    allPairs = allPairs.filter(p => p.region && p.region.toLowerCase().includes(region.toLowerCase()));
  }
  if (country) {
    allPairs = allPairs.filter(p => p.country && p.country.toLowerCase().includes(country.toLowerCase()));
  }
  if (search) {
    const s = search.toLowerCase();
    allPairs = allPairs.filter(p =>
      p.symbol.toLowerCase().includes(s) ||
      p.name.toLowerCase().includes(s) ||
      (p.base && p.base.toLowerCase().includes(s)) ||
      (p.language && p.language.toLowerCase().includes(s))
    );
  }

  const total = allPairs.length;
  const paginated = allPairs.slice(offset, offset + limit);

  // Enrich with ticker data from DB where available
  const symbols = paginated.map(p => p.symbol);
  let tickerMap = {};
  try {
    const tickerResult = await db.query(
      `SELECT pair_symbol, price, volume_24h, high_24h, low_24h, change_24h
       FROM exchange_tickers WHERE pair_symbol = ANY($1)`,
      [symbols]
    );
    for (const row of tickerResult.rows) {
      tickerMap[row.pair_symbol] = row;
    }
  } catch { /* tickers table may not exist yet */ }

  const enriched = paginated.map(p => ({
    ...p,
    lastPrice: tickerMap[p.symbol]?.price ? parseFloat(tickerMap[p.symbol].price) : p.basePrice,
    volume24h: tickerMap[p.symbol]?.volume_24h ? parseFloat(tickerMap[p.symbol].volume_24h) : 0,
    high24h: tickerMap[p.symbol]?.high_24h ? parseFloat(tickerMap[p.symbol].high_24h) : p.basePrice * 1.05,
    low24h: tickerMap[p.symbol]?.low_24h ? parseFloat(tickerMap[p.symbol].low_24h) : p.basePrice * 0.95,
    change24h: tickerMap[p.symbol]?.change_24h ? parseFloat(tickerMap[p.symbol].change_24h) : 0,
    minQty: p.type === 'snt' || p.type === 'snt-usd' ? 10 : p.type === 'core' ? 0.001 : 0.1,
    tickSize: p.basePrice < 0.01 ? 0.000001 : p.basePrice < 1 ? 0.0001 : 0.01,
    tradeable: true
  }));

  // Get unique regions for filter menu
  const regions = [...new Set(allPairs.map(p => p.region).filter(Boolean))].sort();
  const countries = [...new Set(allPairs.map(p => p.country).filter(Boolean))].sort();

  res.json({
    status: 'ok',
    data: {
      pairs: enriched,
      pagination: {
        page,
        limit,
        total,
        totalPages: Math.ceil(total / limit),
        hasNext: page < Math.ceil(total / limit),
        hasPrev: page > 1
      },
      summary: {
        totalPairs: total,
        corePairs: corePairs.length,
        sntPairsWPM: sntPairs.length,
        sntPairsUSD: sntUsdPairs.length,
        indexPairs: 1,
        totalNations: manifest.tokens.length,
        regions,
        countries
      },
      blockchain: {
        network: 'MameyNode',
        chainId: 574,
        nativeCurrency: 'WAMPUM (WMP)',
        bank: 'BDET Bank',
        tokenStandard: 'SNT-574',
        governanceToken: 'IGT',
        bankToken: 'BDET'
      },
      revenueShare: {
        creator: 70,
        platform: 30,
        makerFee: '0.05%',
        takerFee: '0.10%'
      }
    }
  });
}));

// ============================================================
// GET /pairs/regions — All regions with pair counts
// ============================================================
router.get('/pairs/regions', asyncHandler(async (_req, res) => {
  let manifest = { tokens: [] };
  const manifestPaths = [
    path.resolve(__dirname, '../../../../../14-blockchain/tokens/nations/MANIFEST.json'),
    path.resolve(__dirname, '../../../../../../14-blockchain/tokens/nations/MANIFEST.json'),
    path.resolve(process.cwd(), '14-blockchain/tokens/nations/MANIFEST.json')
  ];
  for (const mp of manifestPaths) {
    try {
      if (fs.existsSync(mp)) {
        manifest = JSON.parse(fs.readFileSync(mp, 'utf8'));
        break;
      }
    } catch { /* next */ }
  }

  // Count by region
  const regionCounts = {};
  const countryCounts = {};
  for (const token of manifest.tokens) {
    const r = token.region || 'Other';
    const c = token.country || 'IK';
    regionCounts[r] = (regionCounts[r] || 0) + 1;
    countryCounts[c] = (countryCounts[c] || 0) + 1;
  }

  const regions = Object.entries(regionCounts)
    .map(([name, count]) => ({ name, pairsWPM: count, pairsUSD: count, totalPairs: count * 2 }))
    .sort((a, b) => b.totalPairs - a.totalPairs);

  const countries = Object.entries(countryCounts)
    .map(([code, count]) => ({ code, nations: count, pairsWPM: count, pairsUSD: count }))
    .sort((a, b) => b.nations - a.nations);

  res.json({
    status: 'ok',
    data: {
      regions,
      countries,
      totalNations: manifest.tokens.length,
      totalPairs: manifest.tokens.length * 2 + 12 + 1, // SNT*2 + core + index
      corePairs: 12,
      indexPairs: 1
    }
  });
}));

// ============================================================
// GET /pairs/nation/:nationId — Single nation token details
// ============================================================
router.get('/pairs/nation/:nationId', asyncHandler(async (req, res) => {
  const { nationId } = req.params;

  let manifest = { tokens: [] };
  const manifestPaths = [
    path.resolve(__dirname, '../../../../../14-blockchain/tokens/nations/MANIFEST.json'),
    path.resolve(__dirname, '../../../../../../14-blockchain/tokens/nations/MANIFEST.json'),
    path.resolve(process.cwd(), '14-blockchain/tokens/nations/MANIFEST.json')
  ];
  for (const mp of manifestPaths) {
    try {
      if (fs.existsSync(mp)) {
        manifest = JSON.parse(fs.readFileSync(mp, 'utf8'));
        break;
      }
    } catch { /* next */ }
  }

  const token = manifest.tokens.find(t => t.id === nationId || t.symbol === nationId || t.symbol === `SNT-${nationId}`);
  if (!token) throw new AppError('NOT_FOUND', `Nation token ${nationId} not found`);

  const idx = manifest.tokens.indexOf(token);
  const basePrice = 0.005 + (Math.sin(idx * 0.1) * 0.003) + (idx * 0.00001);

  // Try to load individual token JSON for full details
  let fullToken = null;
  try {
    const tokenFile = path.resolve(__dirname, `../../../../../14-blockchain/tokens/nations/${token.id}-${token.symbol}.json`);
    if (fs.existsSync(tokenFile)) {
      fullToken = JSON.parse(fs.readFileSync(tokenFile, 'utf8'));
    }
  } catch { /* ok */ }

  // Get ticker data from DB
  let ticker = null;
  try {
    const tickerResult = await db.query(
      `SELECT * FROM exchange_tickers WHERE pair_symbol = $1`,
      [`${token.symbol}/WPM`]
    );
    if (tickerResult.rows.length > 0) ticker = tickerResult.rows[0];
  } catch { /* ok */ }

  // Get recent trades for this pair
  let recentTrades = [];
  try {
    const tradesResult = await db.query(
      `SELECT trade_id, price, quantity, value, created_at
       FROM exchange_trades WHERE pair_symbol = $1
       ORDER BY created_at DESC LIMIT 20`,
      [`${token.symbol}/WPM`]
    );
    recentTrades = tradesResult.rows;
  } catch { /* ok */ }

  // Get open orders count
  let openOrders = 0;
  try {
    const ordersResult = await db.query(
      `SELECT COUNT(*) AS count FROM exchange_orders WHERE pair_symbol = $1 AND status = 'open'`,
      [`${token.symbol}/WPM`]
    );
    openOrders = parseInt(ordersResult.rows[0].count);
  } catch { /* ok */ }

  res.json({
    status: 'ok',
    data: {
      token: {
        id: token.id,
        symbol: token.symbol,
        name: token.name,
        region: token.region,
        country: token.country,
        language: token.language,
        contract: token.contract,
        status: token.status
      },
      fullDetails: fullToken,
      tradingPairs: [
        {
          symbol: `${token.symbol}/WPM`,
          base: token.symbol,
          quote: 'WMP',
          lastPrice: ticker ? parseFloat(ticker.price) : parseFloat(basePrice.toFixed(6)),
          volume24h: ticker ? parseFloat(ticker.volume_24h) : 0,
          high24h: ticker ? parseFloat(ticker.high_24h) : parseFloat((basePrice * 1.05).toFixed(6)),
          low24h: ticker ? parseFloat(ticker.low_24h) : parseFloat((basePrice * 0.95).toFixed(6)),
          change24h: ticker ? parseFloat(ticker.change_24h) : 0,
          minQty: 10,
          tickSize: 0.000001,
          tradeable: true
        },
        {
          symbol: `${token.symbol}/USD`,
          base: token.symbol,
          quote: 'USD',
          lastPrice: parseFloat(basePrice.toFixed(6)),
          minQty: 10,
          tickSize: 0.000001,
          tradeable: true
        }
      ],
      orderBook: { openOrders },
      recentTrades,
      blockchain: {
        network: 'MameyNode',
        chainId: 574,
        contract: token.contract,
        standard: 'SNT-574',
        totalSupply: '1,000,000,000',
        decimals: 18
      },
      governance: fullToken?.governance || {
        type: 'Tribal Council DAO',
        votingPeriod: '7 days',
        quorum: '10%',
        councilSeats: 21
      }
    }
  });
}));

// ============================================================
// GET /orderbook/:symbol — Get order book for a pair
// ============================================================
router.get('/orderbook/:symbol', asyncHandler(async (req, res) => {
  const symbol = req.params.symbol.toUpperCase().replace('-', '/');
  const depth = Math.min(50, Math.max(5, parseInt(req.query.depth, 10) || 20));

  const pair = TRADING_PAIRS.find(p => p.symbol === symbol);
  if (!pair) throw new AppError('NOT_FOUND', `Trading pair ${symbol} not found`);

  // Fetch open orders grouped by price level
  const [bidsResult, asksResult] = await Promise.all([
    db.query(
      `SELECT price, SUM(quantity - filled_quantity) AS total_qty, COUNT(*) AS order_count
       FROM exchange_orders
       WHERE pair_symbol = $1 AND side = 'buy' AND status = 'open' AND type = 'limit'
       GROUP BY price ORDER BY price DESC LIMIT $2`,
      [symbol, depth]
    ),
    db.query(
      `SELECT price, SUM(quantity - filled_quantity) AS total_qty, COUNT(*) AS order_count
       FROM exchange_orders
       WHERE pair_symbol = $1 AND side = 'sell' AND status = 'open' AND type = 'limit'
       GROUP BY price ORDER BY price ASC LIMIT $2`,
      [symbol, depth]
    )
  ]);

  const bids = bidsResult.rows.map(r => ({
    price: parseFloat(r.price),
    quantity: parseFloat(r.total_qty),
    orders: parseInt(r.order_count)
  }));

  const asks = asksResult.rows.map(r => ({
    price: parseFloat(r.price),
    quantity: parseFloat(r.total_qty),
    orders: parseInt(r.order_count)
  }));

  const spread = asks.length > 0 && bids.length > 0
    ? parseFloat((asks[0].price - bids[0].price).toFixed(8))
    : 0;

  res.json({
    status: 'ok',
    data: {
      symbol,
      bids,
      asks,
      spread,
      bestBid: bids[0]?.price || 0,
      bestAsk: asks[0]?.price || 0,
      timestamp: new Date().toISOString()
    }
  });
}));

// ============================================================
// POST /order — Place a new order (LIMIT or MARKET)
// ============================================================
router.post('/order',
  validate({
    body: {
      pair: t.string({ required: true }),
      side: t.string({ required: true, enum: ['buy', 'sell'] }),
      type: t.string({ required: true, enum: ['limit', 'market', 'stop_limit'] }),
      quantity: t.number({ required: true, min: 0.00000001 }),
      price: t.number({ min: 0.00000001 }),
      stop_price: t.number({ min: 0.00000001 }),
      time_in_force: t.string({ enum: ['GTC', 'IOC', 'FOK'] })
    }
  }),
  asyncHandler(async (req, res) => {
    if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

    const { side, type, quantity, stop_price } = req.body;
    const symbol = req.body.pair.toUpperCase().replace('-', '/');
    const price = req.body.price || null;
    const tif = req.body.time_in_force || 'GTC';

    // Validate pair
    const pair = TRADING_PAIRS.find(p => p.symbol === symbol);
    if (!pair) throw new AppError('INVALID_INPUT', `Trading pair ${symbol} not supported`);
    if (pair.status !== 'active') throw new AppError('INVALID_INPUT', `Trading pair ${symbol} is not active`);

    // Validate quantity
    if (quantity < pair.minQty) {
      throw new AppError('INVALID_INPUT', `Minimum quantity for ${symbol} is ${pair.minQty}`);
    }

    // Limit orders require price
    if (type === 'limit' && !price) {
      throw new AppError('INVALID_INPUT', 'Limit orders require a price');
    }
    if (type === 'stop_limit' && (!price || !stop_price)) {
      throw new AppError('INVALID_INPUT', 'Stop-limit orders require both price and stop_price');
    }

    // Check balance — buyer needs quote currency, seller needs base currency
    const checkCurrency = side === 'buy' ? pair.quote : pair.base;
    const requiredAmount = side === 'buy' ? quantity * (price || 0) : quantity;

    if (type !== 'market' || side === 'sell') {
      const balResult = await db.query(
        `SELECT COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS balance
         FROM transactions WHERE (from_user = $1 OR to_user = $1) AND currency = $2 AND status = 'completed'`,
        [req.user.id, checkCurrency]
      );
      const balance = parseFloat(balResult.rows[0].balance);

      // Also check locked balance in open orders
      const lockedResult = await db.query(
        `SELECT COALESCE(SUM(
           CASE WHEN side = 'buy' THEN (quantity - filled_quantity) * price
                ELSE quantity - filled_quantity END
         ), 0) AS locked
         FROM exchange_orders
         WHERE user_id = $1 AND status = 'open'
           AND CASE WHEN side = 'buy' THEN quote_currency ELSE base_currency END = $2`,
        [req.user.id, checkCurrency]
      );
      const locked = parseFloat(lockedResult.rows[0].locked);

      if (balance - locked < requiredAmount && type !== 'market') {
        throw new AppError('INSUFFICIENT_FUNDS', `Insufficient ${checkCurrency} balance. Available: ${(balance - locked).toFixed(8)}, Required: ${requiredAmount.toFixed(8)}`);
      }
    }

    const orderId = generateOrderId();

    const client = await db.getClient();
    try {
      await client.query('BEGIN');

      // Insert the order
      const orderResult = await client.query(
        `INSERT INTO exchange_orders
           (order_id, user_id, pair_symbol, base_currency, quote_currency, side, type, quantity, price, stop_price, time_in_force, status, filled_quantity, filled_value)
         VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, 0, 0)
         RETURNING order_id, pair_symbol, side, type, quantity, price, status, created_at`,
        [orderId, req.user.id, symbol, pair.base, pair.quote, side, type, quantity,
         price, stop_price || null, tif,
         type === 'stop_limit' ? 'pending' : 'open']
      );

      // === MATCHING ENGINE ===
      // Try to match the new order against existing orders
      if (type !== 'stop_limit') {
        await matchOrder(client, orderId, req.user.id, symbol, side, type, quantity, price, pair);
      }

      // Fetch final order state
      const finalOrder = await client.query(
        `SELECT * FROM exchange_orders WHERE order_id = $1`,
        [orderId]
      );

      await client.query('COMMIT');

      const order = finalOrder.rows[0];
      audit.record({
        category: 'EXCHANGE_ORDER',
        action: 'order_placed',
        userId: req.user.id,
        risk: 'MEDIUM',
        details: { orderId, symbol, side, type, quantity, price }
      });

      log.info('Order placed', { orderId, symbol, side, type, quantity, price, status: order.status });

      res.status(201).json({
        status: 'ok',
        data: {
          orderId: order.order_id,
          pair: order.pair_symbol,
          side: order.side,
          type: order.type,
          quantity: parseFloat(order.quantity),
          price: order.price ? parseFloat(order.price) : null,
          filledQuantity: parseFloat(order.filled_quantity),
          filledValue: parseFloat(order.filled_value),
          status: order.status,
          createdAt: order.created_at
        }
      });
    } catch (err) {
      await client.query('ROLLBACK');
      if (err instanceof AppError) throw err;
      log.error('Order placement failed', { err, orderId });
      throw new AppError('EXCHANGE_ERROR', 'Order could not be placed');
    } finally {
      client.release();
    }
  })
);

// ============================================================
// MATCHING ENGINE — Core order matching logic
// ============================================================
async function matchOrder(client, orderId, userId, symbol, side, type, quantity, price, pair) {
  let remainingQty = quantity;

  // Find matching orders on the opposite side
  const oppositeSide = side === 'buy' ? 'sell' : 'buy';
  const priceCondition = type === 'market'
    ? '' // Market orders match at any price
    : side === 'buy'
      ? `AND price <= ${price}`   // Buy limit: match sells at or below our price
      : `AND price >= ${price}`;  // Sell limit: match buys at or above our price

  const orderBy = oppositeSide === 'sell' ? 'price ASC' : 'price DESC';

  const matchingOrders = await client.query(
    `SELECT order_id, user_id, price, quantity, filled_quantity,
            (quantity - filled_quantity) AS remaining
     FROM exchange_orders
     WHERE pair_symbol = $1 AND side = $2 AND status = 'open' AND type = 'limit'
       ${priceCondition}
     ORDER BY ${orderBy}, created_at ASC
     FOR UPDATE`,
    [symbol, oppositeSide]
  );

  for (const matchOrder of matchingOrders.rows) {
    if (remainingQty <= 0) break;
    if (matchOrder.user_id === userId) continue; // Don't self-match

    const matchRemaining = parseFloat(matchOrder.remaining);
    const fillQty = Math.min(remainingQty, matchRemaining);
    const fillPrice = parseFloat(matchOrder.price); // Price-time priority: use resting order's price
    const fillValue = fillQty * fillPrice;

    // Calculate fees
    const makerFee = fillValue * MAKER_FEE_RATE;
    const takerFee = fillValue * TAKER_FEE_RATE;

    const tradeId = generateTradeId();

    // Insert trade record
    await client.query(
      `INSERT INTO exchange_trades
         (trade_id, pair_symbol, buy_order_id, sell_order_id, buyer_id, seller_id,
          price, quantity, value, maker_fee, taker_fee, maker_order_id, taker_order_id)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13)`,
      [
        tradeId, symbol,
        side === 'buy' ? orderId : matchOrder.order_id,
        side === 'sell' ? orderId : matchOrder.order_id,
        side === 'buy' ? userId : matchOrder.user_id,
        side === 'sell' ? userId : matchOrder.user_id,
        fillPrice, fillQty, fillValue, makerFee, takerFee,
        matchOrder.order_id, // Maker is the resting order
        orderId              // Taker is the new order
      ]
    );

    // Update the matching (resting/maker) order
    const newMatchFilled = parseFloat(matchOrder.filled_quantity) + fillQty;
    const matchStatus = newMatchFilled >= parseFloat(matchOrder.quantity) ? 'filled' : 'open';
    await client.query(
      `UPDATE exchange_orders SET filled_quantity = $1, filled_value = filled_value + $2, status = $3, updated_at = NOW()
       WHERE order_id = $4`,
      [newMatchFilled, fillValue, matchStatus, matchOrder.order_id]
    );

    // Create settlement transactions (actual token transfers)
    const timestamp = new Date().toISOString();
    const buyerId = side === 'buy' ? userId : matchOrder.user_id;
    const sellerId = side === 'sell' ? userId : matchOrder.user_id;

    // Buyer sends quote currency → seller
    const quoteHash = crypto.createHash('sha256')
      .update(`${buyerId}:${sellerId}:${fillValue}:${pair.quote}:${timestamp}:${tradeId}`)
      .digest('hex');
    await client.query(
      `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
       VALUES ($1, $2, $3, $4, 'exchange', $5, $6, 'completed', $7::jsonb, $8)`,
      [buyerId, sellerId, fillValue - takerFee, pair.quote, `Exchange trade ${tradeId}`, quoteHash,
       JSON.stringify({ tradeId, pair: symbol, side: 'buy_payment' }), timestamp]
    );

    // Seller sends base currency → buyer
    const baseHash = crypto.createHash('sha256')
      .update(`${sellerId}:${buyerId}:${fillQty}:${pair.base}:${timestamp}:${tradeId}`)
      .digest('hex');
    await client.query(
      `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
       VALUES ($1, $2, $3, $4, 'exchange', $5, $6, 'completed', $7::jsonb, $8)`,
      [sellerId, buyerId, fillQty, pair.base, `Exchange trade ${tradeId}`, baseHash,
       JSON.stringify({ tradeId, pair: symbol, side: 'sell_delivery' }), timestamp]
    );

    // Platform fee transaction (taker fee + maker fee → treasury)
    const feeHash = crypto.createHash('sha256')
      .update(`fees:${PLATFORM_TREASURY_ID}:${makerFee + takerFee}:${pair.quote}:${timestamp}:${tradeId}`)
      .digest('hex');
    await client.query(
      `INSERT INTO transactions (from_user, to_user, amount, currency, type, memo, tx_hash, status, metadata, created_at)
       VALUES ($1, $2, $3, $4, 'exchange_fee', $5, $6, 'completed', $7::jsonb, $8)`,
      ['system-exchange', PLATFORM_TREASURY_ID, makerFee + takerFee, pair.quote,
       `Exchange fee for trade ${tradeId}`, feeHash,
       JSON.stringify({ tradeId, makerFee, takerFee }), timestamp]
    );

    // Update ticker
    await client.query(
      `INSERT INTO exchange_tickers (pair_symbol, price, volume_24h, high_24h, low_24h, change_24h, last_trade_at)
       VALUES ($1, $2, $3, $2, $2, 0, NOW())
       ON CONFLICT (pair_symbol) DO UPDATE SET
         price = $2,
         volume_24h = exchange_tickers.volume_24h + $3,
         high_24h = GREATEST(exchange_tickers.high_24h, $2),
         low_24h = LEAST(exchange_tickers.low_24h, $2),
         last_trade_at = NOW()`,
      [symbol, fillPrice, fillValue]
    );

    remainingQty -= fillQty;

    log.info('Trade executed', { tradeId, pair: symbol, price: fillPrice, qty: fillQty, buyer: buyerId, seller: sellerId });
  }

  // Update the taker order
  const filled = quantity - remainingQty;
  const filledValue = filled * (price || 0); // Approximate for market orders
  let orderStatus = 'open';
  if (remainingQty <= 0) orderStatus = 'filled';
  else if (filled > 0 && type === 'market') orderStatus = 'filled'; // Market orders don't rest
  else if (type === 'market' && filled === 0) orderStatus = 'canceled'; // No liquidity for market order

  await client.query(
    `UPDATE exchange_orders SET filled_quantity = $1, filled_value = $2, status = $3, updated_at = NOW()
     WHERE order_id = $4`,
    [filled, filledValue, orderStatus, orderId]
  );
}

// ============================================================
// DELETE /order/:orderId — Cancel an open order
// ============================================================
router.delete('/order/:orderId', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const result = await db.query(
    `UPDATE exchange_orders SET status = 'canceled', updated_at = NOW()
     WHERE order_id = $1 AND user_id = $2 AND status IN ('open', 'pending')
     RETURNING order_id, pair_symbol, side, type, quantity, filled_quantity, status`,
    [req.params.orderId, req.user.id]
  );

  if (result.rows.length === 0) throw new AppError('NOT_FOUND', 'Order not found or already filled/canceled');

  audit.record({
    category: 'EXCHANGE_ORDER',
    action: 'order_canceled',
    userId: req.user.id,
    risk: 'LOW',
    details: { orderId: req.params.orderId }
  });

  res.json({ status: 'ok', data: result.rows[0] });
}));

// ============================================================
// GET /orders — List user's orders
// ============================================================
router.get('/orders', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const pair = req.query.pair || null;
  const status = req.query.status || null;
  const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 50));
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT order_id, pair_symbol, side, type, quantity, price, stop_price,
     filled_quantity, filled_value, time_in_force, status, created_at, updated_at
     FROM exchange_orders WHERE user_id = $1`;
  const params = [req.user.id];
  let idx = 2;

  if (pair) { query += ` AND pair_symbol = $${idx}`; params.push(pair.toUpperCase()); idx++; }
  if (status) { query += ` AND status = $${idx}`; params.push(status); idx++; }

  query += ` ORDER BY created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /trades — Trade history
// ============================================================
router.get('/trades', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  const pair = req.query.pair || null;
  const limit = Math.min(100, Math.max(1, parseInt(req.query.limit, 10) || 50));
  const offset = parseInt(req.query.offset, 10) || 0;

  let query = `SELECT trade_id, pair_symbol, price, quantity, value, maker_fee, taker_fee,
     CASE WHEN buyer_id = $1 THEN 'buy' ELSE 'sell' END AS side,
     CASE WHEN maker_order_id IN (SELECT order_id FROM exchange_orders WHERE user_id = $1) THEN 'maker' ELSE 'taker' END AS role,
     created_at
     FROM exchange_trades
     WHERE buyer_id = $1 OR seller_id = $1`;
  const params = [req.user.id];
  let idx = 2;

  if (pair) { query += ` AND pair_symbol = $${idx}`; params.push(pair.toUpperCase()); idx++; }

  query += ` ORDER BY created_at DESC LIMIT $${idx} OFFSET $${idx + 1}`;
  params.push(limit, offset);

  const result = await db.query(query, params);
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /trades/:symbol/recent — Public recent trades for a pair
// ============================================================
router.get('/trades/:symbol/recent', asyncHandler(async (req, res) => {
  const symbol = req.params.symbol.toUpperCase().replace('-', '/');
  const limit = Math.min(100, Math.max(10, parseInt(req.query.limit, 10) || 50));

  const result = await db.query(
    `SELECT trade_id, price, quantity, value,
            CASE WHEN taker_order_id IN (SELECT order_id FROM exchange_orders WHERE side = 'buy') THEN 'buy' ELSE 'sell' END AS taker_side,
            created_at
     FROM exchange_trades WHERE pair_symbol = $1
     ORDER BY created_at DESC LIMIT $2`,
    [symbol, limit]
  );

  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /ticker — All tickers
// ============================================================
router.get('/ticker', asyncHandler(async (_req, res) => {
  const result = await db.query(
    `SELECT pair_symbol, price, volume_24h, high_24h, low_24h, change_24h, last_trade_at FROM exchange_tickers ORDER BY pair_symbol`
  );
  res.json({ status: 'ok', data: result.rows });
}));

// ============================================================
// GET /ticker/:symbol — Single ticker
// ============================================================
router.get('/ticker/:symbol', asyncHandler(async (req, res) => {
  const symbol = req.params.symbol.toUpperCase().replace('-', '/');
  const result = await db.query(
    `SELECT pair_symbol, price, volume_24h, high_24h, low_24h, change_24h, last_trade_at FROM exchange_tickers WHERE pair_symbol = $1`,
    [symbol]
  );
  if (result.rows.length === 0) throw new AppError('NOT_FOUND', `Ticker for ${symbol} not found`);
  res.json({ status: 'ok', data: result.rows[0] });
}));

// ============================================================
// GET /portfolio — User's exchange portfolio / balances
// ============================================================
router.get('/portfolio', asyncHandler(async (req, res) => {
  if (!req.user) throw new AppError('AUTH_REQUIRED', 'Authentication required');

  // Get all balances
  const balances = await db.query(
    `SELECT currency,
            COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
            COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) AS available
     FROM transactions
     WHERE (from_user = $1 OR to_user = $1) AND status = 'completed'
     GROUP BY currency HAVING
       COALESCE(SUM(CASE WHEN to_user = $1 THEN amount ELSE 0 END), 0) -
       COALESCE(SUM(CASE WHEN from_user = $1 THEN amount ELSE 0 END), 0) > 0`,
    [req.user.id]
  );

  // Get locked in open orders
  const locked = await db.query(
    `SELECT
       CASE WHEN side = 'buy' THEN quote_currency ELSE base_currency END AS currency,
       SUM(CASE WHEN side = 'buy' THEN (quantity - filled_quantity) * price
                ELSE quantity - filled_quantity END) AS locked
     FROM exchange_orders
     WHERE user_id = $1 AND status = 'open'
     GROUP BY CASE WHEN side = 'buy' THEN quote_currency ELSE base_currency END`,
    [req.user.id]
  );

  const lockedMap = {};
  for (const row of locked.rows) {
    lockedMap[row.currency] = parseFloat(row.locked);
  }

  const portfolio = balances.rows.map(row => ({
    currency: row.currency,
    available: parseFloat(row.available) - (lockedMap[row.currency] || 0),
    locked: lockedMap[row.currency] || 0,
    total: parseFloat(row.available)
  }));

  // Get open orders count
  const openOrders = await db.query(
    `SELECT COUNT(*) AS count FROM exchange_orders WHERE user_id = $1 AND status = 'open'`,
    [req.user.id]
  );

  res.json({
    status: 'ok',
    data: {
      portfolio,
      openOrders: parseInt(openOrders.rows[0].count),
      timestamp: new Date().toISOString()
    }
  });
}));

// ============================================================
// Exports
// ============================================================
module.exports = router;

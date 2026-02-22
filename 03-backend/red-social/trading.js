const router = require('express').Router();
const auth = require('../middleware/auth');
const fiscal = require('../utils/fiscal');
const books = {}, trades = [];
function getBook(p) { if (!books[p]) books[p] = { bids: [], asks: [] }; return books[p]; }
router.post('/order', auth, (req, res) => {
  const { pair='WMP/USD', side, type='limit', price, amount } = req.body;
  if (!['buy','sell'].includes(side) || amount <= 0) return res.status(400).json({ error: 'Invalid' });
  const o = { id:'ord_'+Date.now().toString(36)+'_'+Math.random().toString(36).slice(2,6), userId:req.userId, pair, side, type, price: type==='market'?null:price, amount, filled:0, remaining:amount, status:'open', createdAt:new Date() };
  if (type === 'market') {
    const book = getBook(pair), opp = side==='buy'?book.asks:book.bids;
    if (!opp.length) return res.status(400).json({ error: 'No liquidity' });
    o.executedPrice = opp[0].price; o.filled = amount; o.remaining = 0; o.status = 'filled';
    const f = amount*o.executedPrice*0.001; o.fee = +f.toFixed(8); o.fiscal = fiscal.allocate(f);
    trades.push({ pair, price:o.executedPrice, amount, side, ts:Date.now() });
  } else {
    const book = getBook(pair);
    if (side==='buy') { book.bids.push(o); book.bids.sort((a,b)=>b.price-a.price); }
    else { book.asks.push(o); book.asks.sort((a,b)=>a.price-b.price); }
  }
  res.json({ order: o, taxPaid: 0 });
});
router.delete('/order/:id', auth, (req, res) => {
  for (const b of Object.values(books)) { b.bids=b.bids.filter(o=>o.id!==req.params.id); b.asks=b.asks.filter(o=>o.id!==req.params.id); }
  res.json({ cancelled: true });
});
router.get('/orderbook/:pair', (req, res) => {
  const p = req.params.pair.replace(/-/g,'/'), b = getBook(p);
  res.json({ pair:p, bids:b.bids.slice(0,20).map(o=>({price:o.price,amount:o.remaining})), asks:b.asks.slice(0,20).map(o=>({price:o.price,amount:o.remaining})) });
});
router.get('/trades/:pair', (req, res) => {
  res.json({ pair:req.params.pair.replace(/-/g,'/'), trades:trades.filter(t=>t.pair===req.params.pair.replace(/-/g,'/')).slice(-50).reverse() });
});
router.get('/candles/:pair', (req, res) => {
  const { interval='1h', limit=100 } = req.query;
  const candles = []; let p = 0.1150; const now = Date.now(), ms = interval==='1h'?3600000:interval==='1d'?86400000:60000;
  for (let i=+limit;i>0;i--) { const o=p; p+=(Math.random()-0.48)*0.003; candles.push({t:now-i*ms,o:+o.toFixed(4),h:+(Math.max(o,p)+Math.random()*0.001).toFixed(4),l:+(Math.min(o,p)-Math.random()*0.001).toFixed(4),c:+p.toFixed(4),v:Math.floor(10000+Math.random()*50000)}); }
  res.json({ pair:req.params.pair.replace(/-/g,'/'), interval, candles });
});
module.exports = router;
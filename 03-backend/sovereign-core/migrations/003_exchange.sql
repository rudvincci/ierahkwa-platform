-- ============================================================
-- Ierahkwa Sovereign Core — Migration 003
-- Exchange: Orders, Trades, Tickers, Order Book
-- MameyNode blockchain (Chain ID 574) settlement
-- ============================================================

-- Exchange Orders
CREATE TABLE IF NOT EXISTS exchange_orders (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  order_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  pair_symbol VARCHAR(20) NOT NULL,
  base_currency VARCHAR(10) NOT NULL,
  quote_currency VARCHAR(10) NOT NULL,
  side VARCHAR(4) NOT NULL CHECK (side IN ('buy', 'sell')),
  type VARCHAR(20) NOT NULL CHECK (type IN ('limit', 'market', 'stop_limit')),
  quantity DECIMAL(18,8) NOT NULL CHECK (quantity > 0),
  price DECIMAL(18,8),
  stop_price DECIMAL(18,8),
  filled_quantity DECIMAL(18,8) NOT NULL DEFAULT 0,
  filled_value DECIMAL(18,8) NOT NULL DEFAULT 0,
  time_in_force VARCHAR(3) DEFAULT 'GTC' CHECK (time_in_force IN ('GTC', 'IOC', 'FOK')),
  status VARCHAR(20) NOT NULL DEFAULT 'open' CHECK (status IN ('open', 'filled', 'partially_filled', 'canceled', 'pending', 'expired')),
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_order_id ON exchange_orders(order_id);
CREATE INDEX IF NOT EXISTS idx_order_user ON exchange_orders(user_id);
CREATE INDEX IF NOT EXISTS idx_order_pair ON exchange_orders(pair_symbol);
CREATE INDEX IF NOT EXISTS idx_order_side ON exchange_orders(side);
CREATE INDEX IF NOT EXISTS idx_order_status ON exchange_orders(status);
CREATE INDEX IF NOT EXISTS idx_order_type ON exchange_orders(type);
CREATE INDEX IF NOT EXISTS idx_order_price ON exchange_orders(price);
CREATE INDEX IF NOT EXISTS idx_order_created ON exchange_orders(created_at);
-- Composite index for order book queries
CREATE INDEX IF NOT EXISTS idx_orderbook_buy ON exchange_orders(pair_symbol, side, status, price DESC) WHERE status = 'open' AND type = 'limit';
CREATE INDEX IF NOT EXISTS idx_orderbook_sell ON exchange_orders(pair_symbol, side, status, price ASC) WHERE status = 'open' AND type = 'limit';

-- Exchange Trades (executed fills)
CREATE TABLE IF NOT EXISTS exchange_trades (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  trade_id VARCHAR(40) UNIQUE NOT NULL,
  pair_symbol VARCHAR(20) NOT NULL,
  buy_order_id VARCHAR(40) REFERENCES exchange_orders(order_id),
  sell_order_id VARCHAR(40) REFERENCES exchange_orders(order_id),
  buyer_id UUID REFERENCES users(id),
  seller_id UUID REFERENCES users(id),
  price DECIMAL(18,8) NOT NULL,
  quantity DECIMAL(18,8) NOT NULL,
  value DECIMAL(18,8) NOT NULL,
  maker_fee DECIMAL(18,8) NOT NULL DEFAULT 0,
  taker_fee DECIMAL(18,8) NOT NULL DEFAULT 0,
  maker_order_id VARCHAR(40),
  taker_order_id VARCHAR(40),
  settlement_status VARCHAR(20) DEFAULT 'settled',
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_trade_id ON exchange_trades(trade_id);
CREATE INDEX IF NOT EXISTS idx_trade_pair ON exchange_trades(pair_symbol);
CREATE INDEX IF NOT EXISTS idx_trade_buyer ON exchange_trades(buyer_id);
CREATE INDEX IF NOT EXISTS idx_trade_seller ON exchange_trades(seller_id);
CREATE INDEX IF NOT EXISTS idx_trade_created ON exchange_trades(created_at);

-- Exchange Tickers (last price per pair)
CREATE TABLE IF NOT EXISTS exchange_tickers (
  pair_symbol VARCHAR(20) PRIMARY KEY,
  price DECIMAL(18,8) NOT NULL DEFAULT 0,
  volume_24h DECIMAL(18,8) NOT NULL DEFAULT 0,
  high_24h DECIMAL(18,8) NOT NULL DEFAULT 0,
  low_24h DECIMAL(18,8) NOT NULL DEFAULT 0,
  change_24h DECIMAL(10,4) NOT NULL DEFAULT 0,
  last_trade_at TIMESTAMPTZ,
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Initialize tickers for sovereign trading pairs
INSERT INTO exchange_tickers (pair_symbol, price, volume_24h, high_24h, low_24h)
VALUES
  ('WPM/USD', 1.00, 0, 1.05, 0.95),
  ('WPM/BTC', 0.0000148, 0, 0.000016, 0.000014),
  ('WPM/ETH', 0.000308, 0, 0.00033, 0.00029),
  ('IGT/WPM', 0.50, 0, 0.55, 0.45),
  ('IGT/USD', 0.50, 0, 0.55, 0.45),
  ('BDET/WPM', 10.00, 0, 10.50, 9.50),
  ('BDET/USD', 10.00, 0, 10.50, 9.50),
  ('SNT/WPM', 0.01, 0, 0.012, 0.009),
  ('SNT/USD', 0.01, 0, 0.012, 0.009)
ON CONFLICT (pair_symbol) DO NOTHING;

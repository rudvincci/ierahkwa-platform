CREATE TABLE IF NOT EXISTS transactions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  from_user UUID REFERENCES users(id),
  to_user UUID REFERENCES users(id),
  amount DECIMAL(18,8) NOT NULL CHECK (amount > 0),
  currency TEXT DEFAULT 'WMP',
  type TEXT NOT NULL CHECK (type IN ('transfer', 'tip', 'payment', 'refund', 'fee')),
  platform TEXT,
  reference TEXT,
  status TEXT DEFAULT 'completed' CHECK (status IN ('pending', 'completed', 'failed', 'reversed')),
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_tx_from ON transactions(from_user);
CREATE INDEX IF NOT EXISTS idx_tx_to ON transactions(to_user);
CREATE INDEX IF NOT EXISTS idx_tx_platform ON transactions(platform);
CREATE INDEX IF NOT EXISTS idx_tx_created ON transactions(created_at DESC);

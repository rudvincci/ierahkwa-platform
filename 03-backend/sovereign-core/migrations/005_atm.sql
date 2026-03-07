-- ============================================================
-- Ierahkwa Sovereign Core — Migration 005
-- ATM Network: Locations, Transactions, Cash Management
-- BDET Bank — MameyNode blockchain (Chain ID 574)
-- ============================================================

-- ATM Locations
CREATE TABLE IF NOT EXISTS atm_locations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  atm_id VARCHAR(40) UNIQUE NOT NULL,
  atm_type VARCHAR(20) NOT NULL CHECK (atm_type IN ('full', 'deposit', 'standard', 'mini', 'kiosk')),
  name VARCHAR(200) NOT NULL,
  address VARCHAR(500) NOT NULL,
  city VARCHAR(100) NOT NULL,
  country VARCHAR(3) NOT NULL,
  latitude DECIMAL(10,7),
  longitude DECIMAL(10,7),
  nation_id VARCHAR(10),
  territory VARCHAR(200),
  operating_hours VARCHAR(100) DEFAULT '24/7',
  currencies_accepted VARCHAR(200) DEFAULT 'USD,PAB,WMP',
  capabilities JSONB NOT NULL DEFAULT '[]',
  max_deposit DECIMAL(12,2) NOT NULL DEFAULT 0,
  max_withdrawal DECIMAL(12,2) NOT NULL DEFAULT 0,
  denominations JSONB NOT NULL DEFAULT '[]',
  owner_id UUID REFERENCES users(id),
  status VARCHAR(20) NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'maintenance', 'offline', 'decommissioned')),
  cash_level INT NOT NULL DEFAULT 100 CHECK (cash_level >= 0 AND cash_level <= 100),
  last_transaction_at TIMESTAMPTZ,
  firmware_version VARCHAR(20) DEFAULT '1.0.0',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_atm_id ON atm_locations(atm_id);
CREATE INDEX IF NOT EXISTS idx_atm_type ON atm_locations(atm_type);
CREATE INDEX IF NOT EXISTS idx_atm_country ON atm_locations(country);
CREATE INDEX IF NOT EXISTS idx_atm_city ON atm_locations(city);
CREATE INDEX IF NOT EXISTS idx_atm_nation ON atm_locations(nation_id);
CREATE INDEX IF NOT EXISTS idx_atm_status ON atm_locations(status);
CREATE INDEX IF NOT EXISTS idx_atm_owner ON atm_locations(owner_id);
-- Geo-spatial index for location searches
CREATE INDEX IF NOT EXISTS idx_atm_geo ON atm_locations(latitude, longitude) WHERE status = 'active';

-- ATM Transactions
CREATE TABLE IF NOT EXISTS atm_transactions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tx_id VARCHAR(40) UNIQUE NOT NULL,
  atm_id VARCHAR(40) REFERENCES atm_locations(atm_id) NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  account_number VARCHAR(40) NOT NULL,
  operation VARCHAR(30) NOT NULL CHECK (operation IN (
    'cash_deposit', 'cash_withdrawal', 'wpm_purchase', 'check_deposit',
    'transfer', 'bill_payment', 'balance_inquiry', 'card_issuance'
  )),
  amount DECIMAL(18,8) NOT NULL CHECK (amount >= 0),
  currency VARCHAR(10) NOT NULL,
  fee DECIMAL(18,8) NOT NULL DEFAULT 0,
  net_amount DECIMAL(18,8),
  converted_amount DECIMAL(18,8),
  converted_currency VARCHAR(10),
  exchange_rate DECIMAL(18,8),
  denominations JSONB DEFAULT '{}',
  tx_hash VARCHAR(64),
  status VARCHAR(20) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'completed', 'failed', 'reversed')),
  error_message TEXT,
  receipt_data JSONB,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_atm_tx_id ON atm_transactions(tx_id);
CREATE INDEX IF NOT EXISTS idx_atm_tx_atm ON atm_transactions(atm_id);
CREATE INDEX IF NOT EXISTS idx_atm_tx_user ON atm_transactions(user_id);
CREATE INDEX IF NOT EXISTS idx_atm_tx_op ON atm_transactions(operation);
CREATE INDEX IF NOT EXISTS idx_atm_tx_status ON atm_transactions(status);
CREATE INDEX IF NOT EXISTS idx_atm_tx_created ON atm_transactions(created_at);
-- Composite for daily stats queries
CREATE INDEX IF NOT EXISTS idx_atm_tx_daily ON atm_transactions(atm_id, created_at, status) WHERE status = 'completed';

-- ATM Cash Replenishment Log
CREATE TABLE IF NOT EXISTS atm_cash_replenishments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  atm_id VARCHAR(40) REFERENCES atm_locations(atm_id) NOT NULL,
  technician_id UUID REFERENCES users(id),
  previous_level INT NOT NULL,
  new_level INT NOT NULL,
  amount_loaded DECIMAL(12,2),
  currency VARCHAR(10) DEFAULT 'USD',
  denominations_loaded JSONB DEFAULT '{}',
  notes TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_atm_replenish_atm ON atm_cash_replenishments(atm_id);
CREATE INDEX IF NOT EXISTS idx_atm_replenish_date ON atm_cash_replenishments(created_at);

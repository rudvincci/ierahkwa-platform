-- ============================================================
-- Ierahkwa Sovereign Core — Migration 004
-- Payment Cards & Payment Intents
-- PCI-safe tokenized card storage
-- ============================================================

-- Payment Cards (tokenized — NEVER stores full card numbers)
CREATE TABLE IF NOT EXISTS payment_cards (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id) NOT NULL,
  card_token VARCHAR(40) UNIQUE NOT NULL,
  fingerprint VARCHAR(24) NOT NULL,
  brand VARCHAR(20) NOT NULL CHECK (brand IN ('visa', 'mastercard', 'amex', 'discover', 'diners', 'unionpay')),
  last4 CHAR(4) NOT NULL,
  exp_month SMALLINT NOT NULL CHECK (exp_month BETWEEN 1 AND 12),
  exp_year SMALLINT NOT NULL,
  cardholder_name VARCHAR(100) NOT NULL,
  billing_country CHAR(2) DEFAULT 'PA',
  is_default BOOLEAN DEFAULT false,
  status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'expired', 'deleted', 'blocked')),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_card_user ON payment_cards(user_id);
CREATE INDEX IF NOT EXISTS idx_card_token ON payment_cards(card_token);
CREATE INDEX IF NOT EXISTS idx_card_status ON payment_cards(status);
CREATE INDEX IF NOT EXISTS idx_card_fingerprint ON payment_cards(fingerprint);
-- Prevent duplicate cards per user
CREATE UNIQUE INDEX IF NOT EXISTS idx_card_unique ON payment_cards(user_id, fingerprint) WHERE status = 'active';

-- Payment Intents (Stripe-like payment flow)
CREATE TABLE IF NOT EXISTS payment_intents (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  intent_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  amount DECIMAL(18,8) NOT NULL CHECK (amount > 0),
  currency VARCHAR(10) NOT NULL DEFAULT 'USD',
  card_token VARCHAR(40) REFERENCES payment_cards(card_token),
  description TEXT,
  metadata JSONB DEFAULT '{}',
  tx_hash VARCHAR(64),
  status VARCHAR(30) NOT NULL DEFAULT 'requires_confirmation'
    CHECK (status IN ('requires_confirmation', 'processing', 'succeeded', 'failed', 'canceled', 'refunded')),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  confirmed_at TIMESTAMPTZ,
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_intent_id ON payment_intents(intent_id);
CREATE INDEX IF NOT EXISTS idx_intent_user ON payment_intents(user_id);
CREATE INDEX IF NOT EXISTS idx_intent_status ON payment_intents(status);
CREATE INDEX IF NOT EXISTS idx_intent_created ON payment_intents(created_at);

-- WiFi Sessions (for wifi-soberano integration)
CREATE TABLE IF NOT EXISTS wifi_sessions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  mac_address VARCHAR(17),
  ip_address INET,
  plan_id UUID,
  hotspot_id UUID,
  started_at TIMESTAMPTZ DEFAULT NOW(),
  expires_at TIMESTAMPTZ NOT NULL,
  data_used_mb DECIMAL(10,2) DEFAULT 0,
  status VARCHAR(20) DEFAULT 'active',
  payment_id UUID
);

CREATE INDEX IF NOT EXISTS idx_wifi_mac ON wifi_sessions(mac_address);
CREATE INDEX IF NOT EXISTS idx_wifi_status ON wifi_sessions(status);
CREATE INDEX IF NOT EXISTS idx_wifi_hotspot ON wifi_sessions(hotspot_id);

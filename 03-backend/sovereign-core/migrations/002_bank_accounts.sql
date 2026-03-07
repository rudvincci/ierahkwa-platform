-- ============================================================
-- Ierahkwa Sovereign Core — Migration 002
-- Bank Accounts, Transfers, VIP Banking
-- BDET Bank — MameyNode blockchain (Chain ID 574)
-- ============================================================

-- Bank Accounts
CREATE TABLE IF NOT EXISTS bank_accounts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id),
  account_number VARCHAR(30) UNIQUE NOT NULL,
  iban VARCHAR(40) UNIQUE NOT NULL,
  account_type VARCHAR(30) NOT NULL DEFAULT 'checking',
  currency VARCHAR(10) NOT NULL DEFAULT 'WMP',
  display_name VARCHAR(200),
  balance DECIMAL(18,8) NOT NULL DEFAULT 0 CHECK (balance >= 0),
  status VARCHAR(20) DEFAULT 'active',
  vip_tier VARCHAR(20) DEFAULT 'standard',
  is_default BOOLEAN DEFAULT false,
  daily_limit DECIMAL(18,8) DEFAULT 100000,
  monthly_limit DECIMAL(18,8) DEFAULT 1000000,
  interest_rate DECIMAL(5,4) DEFAULT 0,
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_bank_acc_user ON bank_accounts(user_id);
CREATE INDEX IF NOT EXISTS idx_bank_acc_number ON bank_accounts(account_number);
CREATE INDEX IF NOT EXISTS idx_bank_acc_type ON bank_accounts(account_type);
CREATE INDEX IF NOT EXISTS idx_bank_acc_currency ON bank_accounts(currency);
CREATE INDEX IF NOT EXISTS idx_bank_acc_status ON bank_accounts(status);
CREATE INDEX IF NOT EXISTS idx_bank_acc_vip ON bank_accounts(vip_tier);

-- Bank Transfers
CREATE TABLE IF NOT EXISTS bank_transfers (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  transfer_id VARCHAR(40) UNIQUE NOT NULL,
  from_account VARCHAR(30) NOT NULL,
  to_account VARCHAR(30) NOT NULL,
  from_user_id UUID REFERENCES users(id),
  to_user_id UUID REFERENCES users(id),
  amount DECIMAL(18,8) NOT NULL CHECK (amount > 0),
  fee DECIMAL(18,8) NOT NULL DEFAULT 0,
  net_amount DECIMAL(18,8) NOT NULL,
  currency VARCHAR(10) NOT NULL,
  transfer_type VARCHAR(30) NOT NULL DEFAULT 'domestic',
  swift_code VARCHAR(11),
  beneficiary_name VARCHAR(200),
  memo TEXT,
  tx_hash VARCHAR(64),
  status VARCHAR(20) DEFAULT 'pending',
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  completed_at TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_transfer_id ON bank_transfers(transfer_id);
CREATE INDEX IF NOT EXISTS idx_transfer_from ON bank_transfers(from_account);
CREATE INDEX IF NOT EXISTS idx_transfer_to ON bank_transfers(to_account);
CREATE INDEX IF NOT EXISTS idx_transfer_from_user ON bank_transfers(from_user_id);
CREATE INDEX IF NOT EXISTS idx_transfer_type ON bank_transfers(transfer_type);
CREATE INDEX IF NOT EXISTS idx_transfer_status ON bank_transfers(status);
CREATE INDEX IF NOT EXISTS idx_transfer_created ON bank_transfers(created_at);

-- Nation Bank Accounts (one per sovereign nation/tribe)
CREATE TABLE IF NOT EXISTS nation_bank_accounts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  nation_id VARCHAR(10) NOT NULL,
  nation_name VARCHAR(200) NOT NULL,
  snt_symbol VARCHAR(30) NOT NULL,
  account_number VARCHAR(30) UNIQUE NOT NULL,
  iban VARCHAR(40) UNIQUE NOT NULL,
  currency VARCHAR(10) NOT NULL DEFAULT 'WMP',
  snt_balance DECIMAL(18,8) NOT NULL DEFAULT 0,
  wmp_balance DECIMAL(18,8) NOT NULL DEFAULT 0,
  usd_balance DECIMAL(18,8) NOT NULL DEFAULT 0,
  status VARCHAR(20) DEFAULT 'pre-activated',
  activation_condition TEXT DEFAULT 'Sovereign recognition signature by tribal council',
  region VARCHAR(100),
  country VARCHAR(10),
  language VARCHAR(100),
  council_members JSONB DEFAULT '[]',
  governance_type VARCHAR(50) DEFAULT 'Tribal Council DAO',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  activated_at TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_nation_bank_id ON nation_bank_accounts(nation_id);
CREATE INDEX IF NOT EXISTS idx_nation_bank_snt ON nation_bank_accounts(snt_symbol);
CREATE INDEX IF NOT EXISTS idx_nation_bank_region ON nation_bank_accounts(region);
CREATE INDEX IF NOT EXISTS idx_nation_bank_status ON nation_bank_accounts(status);

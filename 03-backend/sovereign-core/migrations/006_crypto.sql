-- ============================================================
-- Ierahkwa Sovereign Core — Migration 006
-- Crypto: Wallets, Transactions, Token Balances, Bridge
-- MameyNode blockchain (Chain ID 574)
-- ============================================================

-- Crypto Wallets
CREATE TABLE IF NOT EXISTS crypto_wallets (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  wallet_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  address VARCHAR(42) NOT NULL,
  chain VARCHAR(20) NOT NULL,
  chain_id INT NOT NULL,
  label VARCHAR(100),
  encrypted_private_key TEXT,
  balance DECIMAL(28,18) NOT NULL DEFAULT 0,
  status VARCHAR(20) NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'frozen', 'deleted')),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_cw_wallet_id ON crypto_wallets(wallet_id);
CREATE INDEX IF NOT EXISTS idx_cw_user ON crypto_wallets(user_id);
CREATE INDEX IF NOT EXISTS idx_cw_address ON crypto_wallets(address);
CREATE INDEX IF NOT EXISTS idx_cw_chain ON crypto_wallets(chain);
CREATE UNIQUE INDEX IF NOT EXISTS idx_cw_user_address ON crypto_wallets(user_id, address, chain);

-- Token Balances per Wallet
CREATE TABLE IF NOT EXISTS crypto_token_balances (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  wallet_id VARCHAR(40) REFERENCES crypto_wallets(wallet_id) NOT NULL,
  token_symbol VARCHAR(20) NOT NULL,
  token_name VARCHAR(100),
  balance DECIMAL(28,18) NOT NULL DEFAULT 0,
  contract_address VARCHAR(42),
  last_updated TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE(wallet_id, token_symbol)
);

CREATE INDEX IF NOT EXISTS idx_ctb_wallet ON crypto_token_balances(wallet_id);
CREATE INDEX IF NOT EXISTS idx_ctb_token ON crypto_token_balances(token_symbol);

-- Crypto Transactions
CREATE TABLE IF NOT EXISTS crypto_transactions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tx_hash VARCHAR(66) UNIQUE NOT NULL,
  from_wallet_id VARCHAR(40) REFERENCES crypto_wallets(wallet_id),
  from_address VARCHAR(42),
  to_address VARCHAR(42) NOT NULL,
  amount DECIMAL(28,18) NOT NULL,
  token_symbol VARCHAR(20) NOT NULL,
  fee DECIMAL(28,18) NOT NULL DEFAULT 0,
  fee_token VARCHAR(20),
  chain VARCHAR(20) NOT NULL,
  chain_id INT NOT NULL,
  block_number BIGINT,
  gas_used BIGINT,
  gas_price VARCHAR(50),
  swap_output_amount DECIMAL(28,18),
  swap_output_token VARCHAR(20),
  swap_rate DECIMAL(28,18),
  bridge_dest_chain VARCHAR(20),
  bridge_dest_chain_id INT,
  bridge_dest_tx_hash VARCHAR(66),
  memo TEXT,
  tx_type VARCHAR(20) NOT NULL CHECK (tx_type IN ('internal', 'external', 'swap', 'bridge', 'staking', 'reward')),
  status VARCHAR(20) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'failed', 'dropped')),
  is_internal BOOLEAN DEFAULT false,
  confirmed_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_ctx_hash ON crypto_transactions(tx_hash);
CREATE INDEX IF NOT EXISTS idx_ctx_from ON crypto_transactions(from_wallet_id);
CREATE INDEX IF NOT EXISTS idx_ctx_to ON crypto_transactions(to_address);
CREATE INDEX IF NOT EXISTS idx_ctx_token ON crypto_transactions(token_symbol);
CREATE INDEX IF NOT EXISTS idx_ctx_type ON crypto_transactions(tx_type);
CREATE INDEX IF NOT EXISTS idx_ctx_status ON crypto_transactions(status);
CREATE INDEX IF NOT EXISTS idx_ctx_chain ON crypto_transactions(chain);
CREATE INDEX IF NOT EXISTS idx_ctx_created ON crypto_transactions(created_at);

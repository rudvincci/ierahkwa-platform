-- ============================================================
-- Ierahkwa Sovereign Core — Migration 007
-- Hosting, Marketplace, Loans, Staking, Notifications
-- Complete sovereign economy infrastructure
-- ============================================================

-- ===== HOSTING =====

CREATE TABLE IF NOT EXISTS hosting_domains (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  domain_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  domain_name VARCHAR(253) UNIQUE NOT NULL,
  tld VARCHAR(20) NOT NULL,
  status VARCHAR(20) NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'expired', 'suspended', 'transferring')),
  registered_at TIMESTAMPTZ DEFAULT NOW(),
  expires_at TIMESTAMPTZ NOT NULL,
  privacy_protection BOOLEAN DEFAULT true,
  auto_renew BOOLEAN DEFAULT true,
  nameservers JSONB DEFAULT '["ns1.soberano.io", "ns2.soberano.io"]',
  ssl_status VARCHAR(20) DEFAULT 'none',
  price_paid DECIMAL(12,2),
  currency VARCHAR(10) DEFAULT 'WMP',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_hd_domain ON hosting_domains(domain_name);
CREATE INDEX IF NOT EXISTS idx_hd_user ON hosting_domains(user_id);
CREATE INDEX IF NOT EXISTS idx_hd_status ON hosting_domains(status);
CREATE INDEX IF NOT EXISTS idx_hd_expires ON hosting_domains(expires_at);

CREATE TABLE IF NOT EXISTS hosting_dns_records (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  record_id VARCHAR(40) UNIQUE NOT NULL,
  domain_id VARCHAR(40) REFERENCES hosting_domains(domain_id) NOT NULL,
  record_type VARCHAR(10) NOT NULL CHECK (record_type IN ('A', 'AAAA', 'CNAME', 'MX', 'TXT', 'NS', 'SRV', 'CAA')),
  name VARCHAR(253) NOT NULL,
  value VARCHAR(1000) NOT NULL,
  ttl INT DEFAULT 3600,
  priority INT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_dns_domain ON hosting_dns_records(domain_id);

CREATE TABLE IF NOT EXISTS hosting_services (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  service_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  plan_id VARCHAR(50) NOT NULL,
  plan_name VARCHAR(100),
  service_type VARCHAR(20) NOT NULL CHECK (service_type IN ('web', 'vps', 'dedicated', 'node')),
  hostname VARCHAR(100),
  region VARCHAR(50),
  os VARCHAR(50),
  ipv4_address VARCHAR(15),
  ipv6_address VARCHAR(45),
  vcpus INT,
  ram_gb INT,
  storage_gb INT,
  bandwidth_gb INT,
  billing_cycle VARCHAR(10) NOT NULL CHECK (billing_cycle IN ('monthly', 'yearly')),
  price DECIMAL(12,2) NOT NULL,
  currency VARCHAR(10) DEFAULT 'WMP',
  status VARCHAR(20) NOT NULL DEFAULT 'provisioning' CHECK (status IN ('provisioning', 'active', 'stopped', 'restarting', 'resizing', 'reinstalling', 'suspended', 'terminated')),
  next_billing_date TIMESTAMPTZ,
  domain_id VARCHAR(40),
  specs JSONB DEFAULT '{}',
  provisioned_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_hs_user ON hosting_services(user_id);
CREATE INDEX IF NOT EXISTS idx_hs_status ON hosting_services(status);
CREATE INDEX IF NOT EXISTS idx_hs_type ON hosting_services(service_type);

-- ===== MARKETPLACE =====

CREATE TABLE IF NOT EXISTS marketplace_listings (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  listing_id VARCHAR(40) UNIQUE NOT NULL,
  seller_id UUID REFERENCES users(id) NOT NULL,
  title VARCHAR(200) NOT NULL,
  description TEXT,
  category VARCHAR(50) NOT NULL,
  price DECIMAL(18,8) NOT NULL CHECK (price > 0),
  currency VARCHAR(10) NOT NULL DEFAULT 'WMP',
  quantity INT NOT NULL DEFAULT 1 CHECK (quantity >= 0),
  listing_type VARCHAR(20) NOT NULL DEFAULT 'fixed' CHECK (listing_type IN ('fixed', 'auction', 'service', 'barter')),
  condition VARCHAR(20) DEFAULT 'new' CHECK (condition IN ('new', 'like_new', 'good', 'fair', 'parts')),
  location VARCHAR(200),
  nation_id VARCHAR(10),
  shipping VARCHAR(20) DEFAULT 'local' CHECK (shipping IN ('local', 'national', 'international', 'digital', 'pickup')),
  images TEXT,
  tags TEXT,
  views INT NOT NULL DEFAULT 0,
  favorites INT NOT NULL DEFAULT 0,
  status VARCHAR(20) NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'sold', 'paused', 'deleted', 'expired')),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_ml_seller ON marketplace_listings(seller_id);
CREATE INDEX IF NOT EXISTS idx_ml_category ON marketplace_listings(category);
CREATE INDEX IF NOT EXISTS idx_ml_status ON marketplace_listings(status);
CREATE INDEX IF NOT EXISTS idx_ml_price ON marketplace_listings(price);
CREATE INDEX IF NOT EXISTS idx_ml_nation ON marketplace_listings(nation_id);
CREATE INDEX IF NOT EXISTS idx_ml_created ON marketplace_listings(created_at);
CREATE INDEX IF NOT EXISTS idx_ml_search ON marketplace_listings USING gin(to_tsvector('spanish', title || ' ' || COALESCE(description, '') || ' ' || COALESCE(tags, '')));

CREATE TABLE IF NOT EXISTS marketplace_orders (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  order_id VARCHAR(40) UNIQUE NOT NULL,
  listing_id VARCHAR(40) REFERENCES marketplace_listings(listing_id) NOT NULL,
  buyer_id UUID REFERENCES users(id) NOT NULL,
  seller_id UUID REFERENCES users(id) NOT NULL,
  quantity INT NOT NULL DEFAULT 1,
  unit_price DECIMAL(18,8) NOT NULL,
  subtotal DECIMAL(18,8) NOT NULL,
  commission DECIMAL(18,8) NOT NULL DEFAULT 0,
  total DECIMAL(18,8) NOT NULL,
  currency VARCHAR(10) NOT NULL DEFAULT 'WMP',
  shipping_address TEXT,
  note TEXT,
  tx_hash VARCHAR(64),
  dispute_reason TEXT,
  disputed_at TIMESTAMPTZ,
  status VARCHAR(20) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'paid', 'shipped', 'completed', 'disputed', 'refunded', 'canceled')),
  completed_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_mo_buyer ON marketplace_orders(buyer_id);
CREATE INDEX IF NOT EXISTS idx_mo_seller ON marketplace_orders(seller_id);
CREATE INDEX IF NOT EXISTS idx_mo_status ON marketplace_orders(status);
CREATE INDEX IF NOT EXISTS idx_mo_created ON marketplace_orders(created_at);

-- ===== LOANS =====

CREATE TABLE IF NOT EXISTS loans (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  loan_id VARCHAR(40) UNIQUE NOT NULL,
  borrower_id UUID REFERENCES users(id) NOT NULL,
  product VARCHAR(30) NOT NULL,
  amount DECIMAL(18,8) NOT NULL CHECK (amount > 0),
  currency VARCHAR(10) NOT NULL DEFAULT 'WMP',
  apr DECIMAL(6,3) NOT NULL,
  term_months INT NOT NULL,
  monthly_payment DECIMAL(18,8) NOT NULL,
  total_interest DECIMAL(18,8) NOT NULL,
  total_repayment DECIMAL(18,8) NOT NULL,
  amount_repaid DECIMAL(18,8) NOT NULL DEFAULT 0,
  payments_made INT NOT NULL DEFAULT 0,
  purpose TEXT,
  collateral_amount DECIMAL(18,8) NOT NULL DEFAULT 0,
  collateral_wallet_id VARCHAR(40),
  guarantor_id UUID,
  credit_score INT,
  credit_tier VARCHAR(20),
  status VARCHAR(20) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'active', 'repaid', 'defaulted', 'restructured')),
  disbursed_at TIMESTAMPTZ,
  due_date TIMESTAMPTZ,
  last_payment_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_loan_borrower ON loans(borrower_id);
CREATE INDEX IF NOT EXISTS idx_loan_product ON loans(product);
CREATE INDEX IF NOT EXISTS idx_loan_status ON loans(status);
CREATE INDEX IF NOT EXISTS idx_loan_due ON loans(due_date);

-- ===== STAKING =====

CREATE TABLE IF NOT EXISTS staking_positions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  stake_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  pool_id VARCHAR(30) NOT NULL,
  token VARCHAR(20) NOT NULL,
  amount DECIMAL(28,18) NOT NULL CHECK (amount > 0),
  apy DECIMAL(6,3) NOT NULL,
  lock_period_days INT NOT NULL DEFAULT 0,
  lock_until TIMESTAMPTZ,
  auto_compound BOOLEAN DEFAULT true,
  rewards_earned DECIMAL(28,18) NOT NULL DEFAULT 0,
  status VARCHAR(20) NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'unstaked', 'slashed')),
  unstaked_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_sp_user ON staking_positions(user_id);
CREATE INDEX IF NOT EXISTS idx_sp_pool ON staking_positions(pool_id);
CREATE INDEX IF NOT EXISTS idx_sp_token ON staking_positions(token);
CREATE INDEX IF NOT EXISTS idx_sp_status ON staking_positions(status);

-- ===== NOTIFICATIONS =====

CREATE TABLE IF NOT EXISTS notifications (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  notification_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  type VARCHAR(30) NOT NULL,
  title VARCHAR(200) NOT NULL,
  body TEXT,
  icon VARCHAR(10),
  priority VARCHAR(10) NOT NULL DEFAULT 'medium' CHECK (priority IN ('low', 'medium', 'high', 'critical')),
  channel VARCHAR(10) DEFAULT 'inapp',
  action_url TEXT,
  metadata JSONB DEFAULT '{}',
  is_read BOOLEAN DEFAULT false,
  read_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_notif_user ON notifications(user_id);
CREATE INDEX IF NOT EXISTS idx_notif_type ON notifications(type);
CREATE INDEX IF NOT EXISTS idx_notif_read ON notifications(user_id, is_read) WHERE is_read = false;
CREATE INDEX IF NOT EXISTS idx_notif_created ON notifications(created_at);

CREATE TABLE IF NOT EXISTS notification_preferences (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID UNIQUE REFERENCES users(id) NOT NULL,
  preferences JSONB NOT NULL DEFAULT '{}',
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS price_alerts (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  alert_id VARCHAR(40) UNIQUE NOT NULL,
  user_id UUID REFERENCES users(id) NOT NULL,
  token VARCHAR(20) NOT NULL,
  target_price DECIMAL(18,8) NOT NULL,
  direction VARCHAR(10) NOT NULL CHECK (direction IN ('above', 'below')),
  status VARCHAR(20) NOT NULL DEFAULT 'active' CHECK (status IN ('active', 'triggered', 'canceled')),
  triggered_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_pa_user ON price_alerts(user_id);
CREATE INDEX IF NOT EXISTS idx_pa_token ON price_alerts(token);
CREATE INDEX IF NOT EXISTS idx_pa_status ON price_alerts(status);

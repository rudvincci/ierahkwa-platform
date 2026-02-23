-- ============================================================================
-- IERAHKWA SOVEREIGN BANKING - PostgreSQL Schema
-- Complete database schema for production
-- ============================================================================

-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================================
-- USERS & AUTHENTICATION
-- ============================================================================

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    password_hash TEXT NOT NULL,
    roles TEXT[] DEFAULT ARRAY['User'],
    permissions TEXT[] DEFAULT ARRAY[]::TEXT[],
    is_active BOOLEAN DEFAULT true,
    is_verified BOOLEAN DEFAULT false,
    verification_token VARCHAR(255),
    reset_token VARCHAR(255),
    reset_token_expires TIMESTAMP,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    last_login_at TIMESTAMP,
    failed_login_attempts INT DEFAULT 0,
    locked_until TIMESTAMP
);

CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    token TEXT UNIQUE NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    revoked_at TIMESTAMP,
    replaced_by_token TEXT,
    is_revoked BOOLEAN DEFAULT false
);

CREATE TABLE user_sessions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    last_activity TIMESTAMP DEFAULT NOW(),
    expires_at TIMESTAMP NOT NULL
);

-- ============================================================================
-- BANKS & ACCOUNTS
-- ============================================================================

CREATE TABLE banks (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    code VARCHAR(20) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    swift_code VARCHAR(11),
    country VARCHAR(100),
    currency VARCHAR(3) DEFAULT 'USD',
    is_central_bank BOOLEAN DEFAULT false,
    is_active BOOLEAN DEFAULT true,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE accounts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    bank_id UUID REFERENCES banks(id),
    account_number VARCHAR(50) UNIQUE NOT NULL,
    account_type VARCHAR(50) NOT NULL,
    balance DECIMAL(20, 2) DEFAULT 0.00,
    available_balance DECIMAL(20, 2) DEFAULT 0.00,
    currency VARCHAR(3) DEFAULT 'USD',
    status VARCHAR(20) DEFAULT 'active',
    is_vip BOOLEAN DEFAULT false,
    credit_limit DECIMAL(20, 2) DEFAULT 0.00,
    interest_rate DECIMAL(5, 4) DEFAULT 0.0000,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================================
-- TRANSACTIONS
-- ============================================================================

CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    transaction_code VARCHAR(50) UNIQUE NOT NULL,
    from_account_id UUID REFERENCES accounts(id),
    to_account_id UUID REFERENCES accounts(id),
    amount DECIMAL(20, 2) NOT NULL,
    fee DECIMAL(20, 2) DEFAULT 0.00,
    currency VARCHAR(3) DEFAULT 'USD',
    type VARCHAR(50) NOT NULL,
    status VARCHAR(20) DEFAULT 'pending',
    description TEXT,
    reference VARCHAR(255),
    external_reference VARCHAR(255),
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT NOW(),
    processed_at TIMESTAMP,
    completed_at TIMESTAMP,
    failed_at TIMESTAMP,
    failure_reason TEXT
);

CREATE TABLE transaction_documents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    transaction_id UUID REFERENCES transactions(id) ON DELETE CASCADE,
    document_type VARCHAR(100) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    file_path TEXT NOT NULL,
    file_size BIGINT,
    mime_type VARCHAR(100),
    checksum VARCHAR(64),
    is_verified BOOLEAN DEFAULT false,
    uploaded_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================================
-- TRADING & FOREX
-- ============================================================================

CREATE TABLE trading_pairs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    base_currency VARCHAR(10) NOT NULL,
    quote_currency VARCHAR(10) NOT NULL,
    symbol VARCHAR(20) UNIQUE NOT NULL,
    bid_price DECIMAL(20, 8),
    ask_price DECIMAL(20, 8),
    last_price DECIMAL(20, 8),
    volume_24h DECIMAL(20, 2),
    change_24h DECIMAL(10, 4),
    is_active BOOLEAN DEFAULT true,
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(base_currency, quote_currency)
);

CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    account_id UUID REFERENCES accounts(id),
    pair_id UUID REFERENCES trading_pairs(id),
    order_type VARCHAR(20) NOT NULL,
    side VARCHAR(10) NOT NULL,
    price DECIMAL(20, 8),
    quantity DECIMAL(20, 8) NOT NULL,
    filled_quantity DECIMAL(20, 8) DEFAULT 0,
    status VARCHAR(20) DEFAULT 'pending',
    time_in_force VARCHAR(10) DEFAULT 'GTC',
    stop_price DECIMAL(20, 8),
    take_profit DECIMAL(20, 8),
    stop_loss DECIMAL(20, 8),
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    filled_at TIMESTAMP,
    cancelled_at TIMESTAMP
);

CREATE TABLE trades (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    order_id UUID REFERENCES orders(id),
    pair_id UUID REFERENCES trading_pairs(id),
    buyer_id UUID REFERENCES users(id),
    seller_id UUID REFERENCES users(id),
    price DECIMAL(20, 8) NOT NULL,
    quantity DECIMAL(20, 8) NOT NULL,
    fee DECIMAL(20, 8) DEFAULT 0,
    fee_currency VARCHAR(10),
    executed_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================================
-- BIOMETRICS
-- ============================================================================

CREATE TABLE biometric_data (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    type VARCHAR(50) NOT NULL,
    template_hash TEXT NOT NULL,
    device_id VARCHAR(255),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT NOW(),
    last_used_at TIMESTAMP,
    UNIQUE(user_id, type)
);

CREATE TABLE biometric_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    biometric_id UUID REFERENCES biometric_data(id),
    action VARCHAR(50) NOT NULL,
    success BOOLEAN NOT NULL,
    ip_address INET,
    device_info JSONB,
    created_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================================
-- AUDIT TRAIL
-- ============================================================================

CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(100),
    entity_id UUID,
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================================
-- INDEXES
-- ============================================================================

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_accounts_user ON accounts(user_id);
CREATE INDEX idx_accounts_bank ON accounts(bank_id);
CREATE INDEX idx_accounts_number ON accounts(account_number);
CREATE INDEX idx_transactions_code ON transactions(transaction_code);
CREATE INDEX idx_transactions_from ON transactions(from_account_id);
CREATE INDEX idx_transactions_to ON transactions(to_account_id);
CREATE INDEX idx_transactions_status ON transactions(status);
CREATE INDEX idx_transactions_date ON transactions(created_at DESC);
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_trades_pair ON trades(pair_id);
CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_entity ON audit_logs(entity_type, entity_id);

-- ============================================================================
-- SEED DATA
-- ============================================================================

-- Central Banks
INSERT INTO banks (code, name, swift_code, country, currency, is_central_bank) VALUES
('FED', 'Federal Reserve System', 'FRNYUS33', 'United States', 'USD', true),
('ECB', 'European Central Bank', 'ECBFDEFF', 'European Union', 'EUR', true),
('BOJ', 'Bank of Japan', 'BOJPJPJT', 'Japan', 'JPY', true),
('PBOC', 'People''s Bank of China', 'PBOCBEJS', 'China', 'CNY', true),
('BDET', 'BDET Central Reserve', 'BDETIEXX', 'IERAHKWA', 'USD', true);

-- Admin user (password: Admin123!)
INSERT INTO users (id, email, full_name, password_hash, roles, permissions, is_active, is_verified) VALUES
(uuid_generate_v4(), 'admin@ierahkwa.gov', 'System Administrator', 
 '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4UQ1EpL8AqJqxkWe', 
 ARRAY['Admin', 'User'], ARRAY['all'], true, true);

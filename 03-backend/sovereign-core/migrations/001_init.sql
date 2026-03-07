-- ============================================================
-- Ierahkwa Sovereign Core — Migration 001
-- Base tables: users, transactions, content, messages, elections
-- MameyNode blockchain (Chain ID 574) integration
-- ============================================================

-- Users
CREATE TABLE IF NOT EXISTS users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  email VARCHAR(255) UNIQUE,
  password_hash TEXT,
  display_name VARCHAR(200),
  role VARCHAR(50) DEFAULT 'citizen',
  tier VARCHAR(50) DEFAULT 'free',
  nation_code VARCHAR(10) DEFAULT 'IK',
  tribe_id VARCHAR(20),
  status VARCHAR(20) DEFAULT 'active',
  metadata JSONB DEFAULT '{}',
  last_login TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_status ON users(status);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);
CREATE INDEX IF NOT EXISTS idx_users_nation ON users(nation_code);
CREATE INDEX IF NOT EXISTS idx_users_tribe ON users(tribe_id);

-- Transactions (universal ledger)
CREATE TABLE IF NOT EXISTS transactions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  from_user VARCHAR(100) NOT NULL,
  to_user VARCHAR(100) NOT NULL,
  amount DECIMAL(18,8) NOT NULL CHECK (amount > 0),
  currency VARCHAR(20) NOT NULL DEFAULT 'WMP',
  type VARCHAR(50) NOT NULL DEFAULT 'transfer',
  memo TEXT,
  tx_hash VARCHAR(64) UNIQUE,
  status VARCHAR(20) DEFAULT 'completed',
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_tx_from ON transactions(from_user);
CREATE INDEX IF NOT EXISTS idx_tx_to ON transactions(to_user);
CREATE INDEX IF NOT EXISTS idx_tx_currency ON transactions(currency);
CREATE INDEX IF NOT EXISTS idx_tx_type ON transactions(type);
CREATE INDEX IF NOT EXISTS idx_tx_status ON transactions(status);
CREATE INDEX IF NOT EXISTS idx_tx_created ON transactions(created_at);
CREATE INDEX IF NOT EXISTS idx_tx_hash ON transactions(tx_hash);

-- Content (per-platform items)
CREATE TABLE IF NOT EXISTS content (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  platform VARCHAR(100) NOT NULL,
  author_id UUID REFERENCES users(id),
  title VARCHAR(500),
  body JSONB DEFAULT '{}',
  category VARCHAR(100),
  status VARCHAR(20) DEFAULT 'published',
  view_count INT DEFAULT 0,
  like_count INT DEFAULT 0,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_content_platform ON content(platform);
CREATE INDEX IF NOT EXISTS idx_content_author ON content(author_id);
CREATE INDEX IF NOT EXISTS idx_content_status ON content(status);

-- Messages (internal messaging)
CREATE TABLE IF NOT EXISTS messages (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  thread_id UUID,
  from_user UUID REFERENCES users(id),
  to_user UUID REFERENCES users(id),
  subject VARCHAR(500),
  body TEXT NOT NULL,
  is_read BOOLEAN DEFAULT false,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_messages_thread ON messages(thread_id);
CREATE INDEX IF NOT EXISTS idx_messages_to ON messages(to_user);
CREATE INDEX IF NOT EXISTS idx_messages_read ON messages(to_user, is_read);

-- Elections (governance voting)
CREATE TABLE IF NOT EXISTS elections (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  title VARCHAR(500) NOT NULL,
  description TEXT,
  options JSONB NOT NULL DEFAULT '[]',
  created_by UUID REFERENCES users(id),
  status VARCHAR(20) DEFAULT 'active',
  starts_at TIMESTAMPTZ DEFAULT NOW(),
  ends_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Ballots
CREATE TABLE IF NOT EXISTS ballots (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  election_id UUID REFERENCES elections(id),
  voter_id UUID REFERENCES users(id),
  vote_hash VARCHAR(64) NOT NULL,
  option_index INT NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE(election_id, voter_id)
);

-- Files (storage)
CREATE TABLE IF NOT EXISTS files (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id),
  filename VARCHAR(500) NOT NULL,
  original_name VARCHAR(500),
  mime_type VARCHAR(100),
  size_bytes BIGINT,
  file_hash VARCHAR(64),
  storage_path TEXT,
  status VARCHAR(20) DEFAULT 'active',
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  deleted_at TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_files_user ON files(user_id);
CREATE INDEX IF NOT EXISTS idx_files_hash ON files(file_hash);

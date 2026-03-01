CREATE TABLE IF NOT EXISTS messages (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  from_user UUID REFERENCES users(id),
  to_user UUID REFERENCES users(id),
  thread_id UUID,
  subject TEXT,
  body TEXT NOT NULL,
  platform TEXT DEFAULT 'correo-soberano',
  read BOOLEAN DEFAULT FALSE,
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_msg_to ON messages(to_user);
CREATE INDEX IF NOT EXISTS idx_msg_from ON messages(from_user);
CREATE INDEX IF NOT EXISTS idx_msg_thread ON messages(thread_id);
CREATE INDEX IF NOT EXISTS idx_msg_read ON messages(to_user, read);

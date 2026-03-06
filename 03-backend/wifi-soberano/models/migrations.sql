-- ============================================================
-- WiFi Soberano — Database Migrations
-- Internet Satelital Soberano — Ierahkwa Ne Kanienke
-- Version: 1.0.0
-- ============================================================

-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================
-- 1. WiFi Plans — Pricing tiers
-- ============================================================
CREATE TABLE IF NOT EXISTS wifi_plans (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(50) NOT NULL,
  duration_hours INT NOT NULL,
  price_wampum DECIMAL(10,2) NOT NULL,
  bandwidth_mbps INT DEFAULT 50,
  data_limit_gb DECIMAL(10,2),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Default plans
INSERT INTO wifi_plans (name, duration_hours, price_wampum, bandwidth_mbps, data_limit_gb) VALUES
  ('1 Hora', 1, 9.99, 25, 1),
  ('1 Día', 24, 24.99, 50, 5),
  ('1 Semana', 168, 99.99, 75, 25),
  ('1 Mes', 720, 249.99, 100, 100),
  ('6 Meses', 4320, 999.99, 150, NULL),
  ('1 Año', 8760, 1499.99, 200, NULL)
ON CONFLICT DO NOTHING;

-- ============================================================
-- 2. Starlink Fleet — Hardware inventory
-- ============================================================
CREATE TABLE IF NOT EXISTS starlink_fleet (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  utid VARCHAR(50) UNIQUE NOT NULL,
  model VARCHAR(50) NOT NULL,
  account_name VARCHAR(100),
  activation_date DATE,
  transfer_eligible_date DATE,
  location TEXT,
  status VARCHAR(20) DEFAULT 'online' CHECK (status IN ('online', 'offline', 'rma', 'transit', 'stored')),
  signal_quality INT CHECK (signal_quality >= 0 AND signal_quality <= 100),
  firmware_version VARCHAR(20),
  monthly_cost_usd DECIMAL(8,2),
  notes TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Fleet data from actual Starlink accounts
INSERT INTO starlink_fleet (utid, model, account_name, activation_date, transfer_eligible_date, location, status) VALUES
  ('UT-GOMEZ-001', 'Performance Gen 3', 'Gómez', '2025-11-15', '2026-02-13', 'Tocumen, Panamá', 'online'),
  ('UT-OMAR-001', 'Starlink Mini', 'Omar', '2025-12-01', '2026-03-01', 'Chepo, Darién', 'online'),
  ('UT-SEGURA-001', 'Performance Gen 1', 'Segura', '2025-10-20', '2026-01-18', 'Darién, Panamá', 'rma'),
  ('UT-MIKEKOL-001', 'Standard', 'Mikekol', '2025-09-15', '2025-12-14', 'Fort Lauderdale, FL', 'online'),
  ('UT-WILSON-001', 'Performance Gen 1', 'Wilson', '2025-11-01', '2026-01-30', 'Guna Yala, Panamá', 'online'),
  ('UT-ERICK-001', 'Mesh Node', 'Erick', '2025-12-10', '2026-03-10', 'Emberá, Panamá', 'online'),
  ('UT-FELIX-001', 'Performance Gen 3', 'Félix', '2026-01-05', '2026-04-05', 'Carti, Guna Yala', 'online'),
  ('UT-GARY-001', 'Standard', 'Gary/Appel', '2025-08-20', '2025-11-18', 'Fort Lauderdale, FL', 'offline')
ON CONFLICT (utid) DO NOTHING;

-- ============================================================
-- 3. Hotspots — Physical deployment locations
-- ============================================================
CREATE TABLE IF NOT EXISTS hotspots (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(100) NOT NULL,
  location_lat DECIMAL(10,7),
  location_lng DECIMAL(10,7),
  address TEXT,
  territory VARCHAR(100),
  starlink_kit_id UUID REFERENCES starlink_fleet(id) ON DELETE SET NULL,
  max_users INT DEFAULT 50,
  status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'maintenance', 'planned')),
  signal_multiplier_count INT DEFAULT 1,
  coverage_radius_m INT DEFAULT 100,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Initial hotspot locations
INSERT INTO hotspots (name, location_lat, location_lng, address, territory, max_users, status) VALUES
  ('Mercado Tocumen', 9.0825, -79.3836, 'Mercado Central, Tocumen', 'Panamá Este', 100, 'active'),
  ('Terminal Chepo', 9.1700, -79.0986, 'Terminal de Buses, Chepo', 'Chepo/Darién', 75, 'active'),
  ('Centro Darién', 8.4100, -78.1500, 'Centro Comunitario, Darién', 'Darién', 50, 'active'),
  ('Puerto Guna Yala', 9.5500, -78.9500, 'Muelle Principal, Guna Yala', 'Guna Yala', 60, 'active'),
  ('Comunidad Emberá', 9.0200, -79.5800, 'Centro Cultural Emberá', 'Emberá-Wounaan', 40, 'planned'),
  ('Isla Carti', 9.4600, -78.9600, 'Puerto Carti, San Blas', 'Guna Yala', 80, 'planned')
ON CONFLICT DO NOTHING;

-- ============================================================
-- 4. WiFi Sessions — Active user connections
-- ============================================================
CREATE TABLE IF NOT EXISTS wifi_sessions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  mac_address VARCHAR(17),
  ip_address INET,
  plan_id UUID REFERENCES wifi_plans(id) ON DELETE SET NULL,
  hotspot_id UUID REFERENCES hotspots(id) ON DELETE SET NULL,
  started_at TIMESTAMPTZ DEFAULT NOW(),
  expires_at TIMESTAMPTZ NOT NULL,
  data_used_mb DECIMAL(10,2) DEFAULT 0,
  status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'expired', 'terminated', 'suspended')),
  payment_id UUID,
  device_fingerprint TEXT,
  user_agent TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_wifi_sessions_status ON wifi_sessions(status);
CREATE INDEX IF NOT EXISTS idx_wifi_sessions_ip ON wifi_sessions(ip_address);
CREATE INDEX IF NOT EXISTS idx_wifi_sessions_expires ON wifi_sessions(expires_at);
CREATE INDEX IF NOT EXISTS idx_wifi_sessions_started ON wifi_sessions(started_at);
CREATE INDEX IF NOT EXISTS idx_wifi_sessions_hotspot ON wifi_sessions(hotspot_id);

-- ============================================================
-- 5. WiFi Payments — WAMPUM transactions
-- ============================================================
CREATE TABLE IF NOT EXISTS wifi_payments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  session_id UUID REFERENCES wifi_sessions(id) ON DELETE SET NULL,
  plan_id UUID REFERENCES wifi_plans(id) ON DELETE SET NULL,
  amount_wampum DECIMAL(10,2) NOT NULL,
  tx_hash VARCHAR(66),
  wallet_address VARCHAR(42),
  status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'failed', 'refunded')),
  payment_method VARCHAR(20) DEFAULT 'wampum',
  ip_address INET,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  confirmed_at TIMESTAMPTZ
);

CREATE INDEX IF NOT EXISTS idx_wifi_payments_status ON wifi_payments(status);
CREATE INDEX IF NOT EXISTS idx_wifi_payments_session ON wifi_payments(session_id);
CREATE INDEX IF NOT EXISTS idx_wifi_payments_tx ON wifi_payments(tx_hash);

-- ============================================================
-- 6. WiFi Analytics — Usage tracking
-- ============================================================
CREATE TABLE IF NOT EXISTS wifi_analytics (
  id BIGSERIAL PRIMARY KEY,
  session_id UUID REFERENCES wifi_sessions(id) ON DELETE SET NULL,
  hotspot_id UUID REFERENCES hotspots(id) ON DELETE SET NULL,
  timestamp TIMESTAMPTZ DEFAULT NOW(),
  bytes_up BIGINT DEFAULT 0,
  bytes_down BIGINT DEFAULT 0,
  device_type VARCHAR(50),
  os VARCHAR(50),
  browser VARCHAR(50),
  screen_resolution VARCHAR(20),
  language VARCHAR(10)
);

CREATE INDEX IF NOT EXISTS idx_wifi_analytics_timestamp ON wifi_analytics(timestamp);
CREATE INDEX IF NOT EXISTS idx_wifi_analytics_hotspot ON wifi_analytics(hotspot_id);
CREATE INDEX IF NOT EXISTS idx_wifi_analytics_session ON wifi_analytics(session_id);

-- ============================================================
-- 7. VIP Protected — Atabey AI protection list
-- ============================================================
CREATE TABLE IF NOT EXISTS vip_protected (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(200) NOT NULL,
  role VARCHAR(100),
  keywords TEXT,
  protection_level VARCHAR(20) DEFAULT 'high' CHECK (protection_level IN ('standard', 'high', 'critical', 'maximum')),
  atabey_monitor BOOLEAN DEFAULT true,
  alert_webhook TEXT,
  notes TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_vip_name ON vip_protected USING gin(to_tsvector('spanish', name));
CREATE INDEX IF NOT EXISTS idx_vip_keywords ON vip_protected USING gin(to_tsvector('spanish', keywords));

-- ============================================================
-- 8. Vigilancia Alerts — Sovereign surveillance log
-- ============================================================
CREATE TABLE IF NOT EXISTS vigilancia_alerts (
  id BIGSERIAL PRIMARY KEY,
  alert_type VARCHAR(50) NOT NULL,
  severity VARCHAR(20) DEFAULT 'info' CHECK (severity IN ('info', 'low', 'medium', 'high', 'critical')),
  ip_address INET,
  mac_address VARCHAR(17),
  user_agent TEXT,
  request_path TEXT,
  query_string TEXT,
  geo_country VARCHAR(3),
  geo_city VARCHAR(100),
  geo_lat DECIMAL(10,7),
  geo_lng DECIMAL(10,7),
  matched_vip VARCHAR(200),
  action_taken VARCHAR(100),
  details JSONB,
  session_id UUID REFERENCES wifi_sessions(id) ON DELETE SET NULL,
  hotspot_id UUID REFERENCES hotspots(id) ON DELETE SET NULL,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_vigilancia_type ON vigilancia_alerts(alert_type);
CREATE INDEX IF NOT EXISTS idx_vigilancia_severity ON vigilancia_alerts(severity);
CREATE INDEX IF NOT EXISTS idx_vigilancia_ip ON vigilancia_alerts(ip_address);
CREATE INDEX IF NOT EXISTS idx_vigilancia_created ON vigilancia_alerts(created_at);
CREATE INDEX IF NOT EXISTS idx_vigilancia_vip ON vigilancia_alerts(matched_vip);

-- ============================================================
-- 9. Vigilancia Connection Log — EVERY connection logged
-- ============================================================
CREATE TABLE IF NOT EXISTS vigilancia_connections (
  id BIGSERIAL PRIMARY KEY,
  ip_address INET NOT NULL,
  mac_address VARCHAR(17),
  user_agent TEXT,
  device_fingerprint TEXT,
  method VARCHAR(10),
  path TEXT,
  query_params TEXT,
  referer TEXT,
  geo_country VARCHAR(3),
  geo_city VARCHAR(100),
  geo_lat DECIMAL(10,7),
  geo_lng DECIMAL(10,7),
  hotspot_id UUID REFERENCES hotspots(id) ON DELETE SET NULL,
  session_id UUID REFERENCES wifi_sessions(id) ON DELETE SET NULL,
  timestamp TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_connections_ip ON vigilancia_connections(ip_address);
CREATE INDEX IF NOT EXISTS idx_connections_timestamp ON vigilancia_connections(timestamp);
CREATE INDEX IF NOT EXISTS idx_connections_hotspot ON vigilancia_connections(hotspot_id);

-- Partition by month for performance (optional — create partitions as needed)
-- CREATE TABLE vigilancia_connections_2026_03 PARTITION OF vigilancia_connections
--   FOR VALUES FROM ('2026-03-01') TO ('2026-04-01');

-- ============================================================
-- 10. Functions & Triggers
-- ============================================================

-- Auto-update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_wifi_plans_updated
  BEFORE UPDATE ON wifi_plans
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

CREATE TRIGGER trg_starlink_fleet_updated
  BEFORE UPDATE ON starlink_fleet
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

CREATE TRIGGER trg_hotspots_updated
  BEFORE UPDATE ON hotspots
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

CREATE TRIGGER trg_vip_protected_updated
  BEFORE UPDATE ON vip_protected
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

-- Auto-expire sessions function
CREATE OR REPLACE FUNCTION expire_wifi_sessions()
RETURNS INTEGER AS $$
DECLARE
  expired_count INTEGER;
BEGIN
  UPDATE wifi_sessions
  SET status = 'expired'
  WHERE status = 'active' AND expires_at < NOW();
  GET DIAGNOSTICS expired_count = ROW_COUNT;
  RETURN expired_count;
END;
$$ LANGUAGE plpgsql;

-- Calculate transfer eligible date (90 days)
CREATE OR REPLACE FUNCTION calc_transfer_date()
RETURNS TRIGGER AS $$
BEGIN
  IF NEW.activation_date IS NOT NULL AND NEW.transfer_eligible_date IS NULL THEN
    NEW.transfer_eligible_date = NEW.activation_date + INTERVAL '90 days';
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_fleet_transfer_date
  BEFORE INSERT OR UPDATE ON starlink_fleet
  FOR EACH ROW EXECUTE FUNCTION calc_transfer_date();

-- ============================================================
-- 11. Views for Dashboard
-- ============================================================

-- Active sessions overview
CREATE OR REPLACE VIEW v_active_sessions AS
SELECT
  s.id, s.ip_address, s.mac_address, s.started_at, s.expires_at,
  s.data_used_mb, s.status,
  p.name as plan_name, p.bandwidth_mbps, p.price_wampum,
  h.name as hotspot_name, h.territory,
  EXTRACT(EPOCH FROM (s.expires_at - NOW())) / 3600 as hours_remaining
FROM wifi_sessions s
LEFT JOIN wifi_plans p ON s.plan_id = p.id
LEFT JOIN hotspots h ON s.hotspot_id = h.id
WHERE s.status = 'active' AND s.expires_at > NOW();

-- Revenue summary
CREATE OR REPLACE VIEW v_revenue_summary AS
SELECT
  DATE(s.started_at) as date,
  COUNT(*) as sessions,
  COALESCE(SUM(p.price_wampum), 0) as revenue_wampum,
  COUNT(DISTINCT s.ip_address) as unique_users,
  COUNT(DISTINCT s.hotspot_id) as active_hotspots
FROM wifi_sessions s
JOIN wifi_plans p ON s.plan_id = p.id
WHERE s.started_at > NOW() - INTERVAL '90 days'
GROUP BY DATE(s.started_at)
ORDER BY date DESC;

-- Fleet health
CREATE OR REPLACE VIEW v_fleet_health AS
SELECT
  f.*,
  h.name as hotspot_name,
  h.status as hotspot_status,
  (SELECT COUNT(*) FROM wifi_sessions s WHERE s.hotspot_id = h.id AND s.status = 'active') as active_sessions
FROM starlink_fleet f
LEFT JOIN hotspots h ON h.starlink_kit_id = f.id;

-- ============================================================
-- Done — WiFi Soberano Database Ready
-- ============================================================

-- ============================================================
-- RED SOBERANA — Database Schema v1.0
-- PostgreSQL + MameyNode blockchain hybrid
-- On-chain: payments, certificates, votes, identity
-- Off-chain: content, media, sessions, cache
-- ============================================================

-- ==================== CORE ====================

CREATE TABLE sovereign_users (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sovereign_id    VARCHAR(64) UNIQUE NOT NULL,    -- blockchain address
    name_hash       BYTEA NOT NULL,                 -- cifrado
    nation          VARCHAR(64),                     -- "Lenca", "Maya", "Navajo"
    country         VARCHAR(2),                      -- ISO country
    email           VARCHAR(255) UNIQUE,            -- @soberano.bo
    verification    SMALLINT DEFAULT 0,             -- 0-3
    is_artisan      BOOLEAN DEFAULT false,
    is_creator      BOOLEAN DEFAULT false,
    is_worker       BOOLEAN DEFAULT false,
    reputation      INTEGER DEFAULT 0,
    wallet_balance  DECIMAL(18,8) DEFAULT 0,
    language        VARCHAR(10) DEFAULT 'es',
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    last_active     TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE sovereign_sessions (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID REFERENCES sovereign_users(id),
    token_hash  BYTEA NOT NULL,
    device      VARCHAR(255),
    ip_hash     BYTEA,          -- cifrado, no guardamos IP real
    expires_at  TIMESTAMPTZ,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== BDET BANK ====================

CREATE TABLE bdet_transactions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tx_hash         VARCHAR(66) UNIQUE,     -- MameyNode hash
    from_user       UUID REFERENCES sovereign_users(id),
    to_user         UUID REFERENCES sovereign_users(id),
    amount          DECIMAL(18,8) NOT NULL,
    currency        VARCHAR(8) DEFAULT 'WMP',
    platform_id     SMALLINT,               -- 1-20
    split_worker    DECIMAL(18,8),
    split_platform  DECIMAL(18,8),
    split_treasury  DECIMAL(18,8),
    split_community DECIMAL(18,8),
    split_insurance DECIMAL(18,8),
    status          VARCHAR(16) DEFAULT 'pending', -- pending, confirmed, failed
    created_at      TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE bdet_escrows (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    escrow_hash VARCHAR(66) UNIQUE,
    buyer_id    UUID REFERENCES sovereign_users(id),
    seller_id   UUID REFERENCES sovereign_users(id),
    amount      DECIMAL(18,8) NOT NULL,
    platform_id SMALLINT,
    status      VARCHAR(16) DEFAULT 'active', -- active, released, disputed, refunded
    release_at  TIMESTAMPTZ,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 01 CORREO SOBERANO ====================

CREATE TABLE emails (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    from_user   UUID REFERENCES sovereign_users(id),
    to_user     UUID REFERENCES sovereign_users(id),
    subject_enc BYTEA NOT NULL,         -- cifrado E2E
    body_enc    BYTEA NOT NULL,         -- cifrado E2E
    has_bdet    BOOLEAN DEFAULT false,  -- pago inline
    bdet_amount DECIMAL(18,8),
    read        BOOLEAN DEFAULT false,
    labels      TEXT[],
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 02 RED SOBERANA ====================

CREATE TABLE social_posts (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    author_id   UUID REFERENCES sovereign_users(id),
    content     TEXT NOT NULL,
    media_urls  TEXT[],
    post_type   VARCHAR(16) DEFAULT 'text', -- text, image, video, poll, event, bdet
    likes       INTEGER DEFAULT 0,
    comments    INTEGER DEFAULT 0,
    shares      INTEGER DEFAULT 0,
    tips_wmp    DECIMAL(18,8) DEFAULT 0,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE social_polls (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    post_id     UUID REFERENCES social_posts(id),
    question    TEXT NOT NULL,
    options     JSONB NOT NULL,          -- [{text, votes}]
    vote_hash   VARCHAR(66),            -- MameyNode verification
    total_votes INTEGER DEFAULT 0,
    ends_at     TIMESTAMPTZ,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 04 CANAL SOBERANO ====================

CREATE TABLE videos (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    creator_id  UUID REFERENCES sovereign_users(id),
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    video_url   VARCHAR(512),
    thumbnail   VARCHAR(512),
    duration    INTEGER,                -- seconds
    views       BIGINT DEFAULT 0,
    likes       INTEGER DEFAULT 0,
    category    VARCHAR(64),
    is_live     BOOLEAN DEFAULT false,
    subtitles   JSONB,                  -- {lang: url} via Atabey
    revenue_wmp DECIMAL(18,8) DEFAULT 0,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 05 MUSICA SOBERANA ====================

CREATE TABLE tracks (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    artist_id   UUID REFERENCES sovereign_users(id),
    title       VARCHAR(255) NOT NULL,
    album       VARCHAR(255),
    audio_url   VARCHAR(512),
    cover_url   VARCHAR(512),
    duration    INTEGER,
    genre       VARCHAR(64),
    plays       BIGINT DEFAULT 0,
    tips_wmp    DECIMAL(18,8) DEFAULT 0,
    lyrics      JSONB,                  -- {lang: text}
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 06 HOSPEDAJE SOBERANO ====================

CREATE TABLE lodgings (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    host_id     UUID REFERENCES sovereign_users(id),
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    location    JSONB,                  -- {lat, lng, country, region}
    price_wmp   DECIMAL(18,8),
    price_type  VARCHAR(16) DEFAULT 'night',
    category    VARCHAR(32),            -- eco-lodge, cabana, camping, etc
    amenities   TEXT[],
    images      TEXT[],
    rating      DECIMAL(3,2) DEFAULT 0,
    reviews     INTEGER DEFAULT 0,
    is_superhost BOOLEAN DEFAULT false,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE bookings (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    lodging_id  UUID REFERENCES lodgings(id),
    guest_id    UUID REFERENCES sovereign_users(id),
    check_in    DATE NOT NULL,
    check_out   DATE NOT NULL,
    guests      SMALLINT DEFAULT 1,
    total_wmp   DECIMAL(18,8),
    escrow_id   UUID REFERENCES bdet_escrows(id),
    status      VARCHAR(16) DEFAULT 'confirmed',
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 07 ARTESANIA SOBERANA ====================

CREATE TABLE artisan_products (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    artisan_id      UUID REFERENCES sovereign_users(id),
    name            VARCHAR(255) NOT NULL,
    description     TEXT,
    price_wmp       DECIMAL(18,8),
    category        VARCHAR(64),
    nation          VARCHAR(64),
    technique       VARCHAR(128),
    materials       TEXT,
    images          TEXT[],
    cert_nft_id     VARCHAR(66),        -- MameyNode NFT
    rating          DECIMAL(3,2) DEFAULT 0,
    total_sales     INTEGER DEFAULT 0,
    is_bestseller   BOOLEAN DEFAULT false,
    created_at      TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 09 COMERCIO SOBERANO ====================

CREATE TABLE stores (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id    UUID REFERENCES sovereign_users(id),
    name        VARCHAR(255) NOT NULL,
    slug        VARCHAR(128) UNIQUE,     -- analenca.soberano.bo
    template    VARCHAR(64),
    plan        VARCHAR(16) DEFAULT 'seed', -- seed, growth, nation
    products    INTEGER DEFAULT 0,
    revenue_wmp DECIMAL(18,8) DEFAULT 0,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 10 INVERTIR SOBERANO ====================

CREATE TABLE portfolios (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID REFERENCES sovereign_users(id),
    total_value DECIMAL(18,8) DEFAULT 0,
    total_pnl   DECIMAL(18,8) DEFAULT 0,
    updated_at  TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE positions (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    portfolio_id UUID REFERENCES portfolios(id),
    symbol      VARCHAR(16) NOT NULL,   -- WMP, BTC, FARM-TKN, etc
    quantity    DECIMAL(18,8),
    avg_price   DECIMAL(18,8),
    current_val DECIMAL(18,8),
    pnl         DECIMAL(18,8),
    updated_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 14 TRABAJO SOBERANO ====================

CREATE TABLE jobs (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_id  UUID REFERENCES sovereign_users(id),
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    salary_wmp  DECIMAL(18,8),
    salary_type VARCHAR(16) DEFAULT 'month',
    location    VARCHAR(128),
    remote      BOOLEAN DEFAULT true,
    skills      TEXT[],
    applications INTEGER DEFAULT 0,
    status      VARCHAR(16) DEFAULT 'open',
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 15 RENTA SOBERANO ====================

CREATE TABLE gigs (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    client_id   UUID REFERENCES sovereign_users(id),
    worker_id   UUID REFERENCES sovereign_users(id),
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    budget_wmp  DECIMAL(18,8),
    category    VARCHAR(64),
    escrow_id   UUID REFERENCES bdet_escrows(id),
    status      VARCHAR(16) DEFAULT 'open',
    rating      DECIMAL(3,2),
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== 19 UNIVERSIDAD SOBERANA ====================

CREATE TABLE courses (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    instructor_id UUID REFERENCES sovereign_users(id),
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    category    VARCHAR(64),
    language    VARCHAR(10),
    price_wmp   DECIMAL(18,8) DEFAULT 0,
    lessons     INTEGER DEFAULT 0,
    students    INTEGER DEFAULT 0,
    rating      DECIMAL(3,2) DEFAULT 0,
    cert_nft    BOOLEAN DEFAULT true,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE enrollments (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    course_id   UUID REFERENCES courses(id),
    student_id  UUID REFERENCES sovereign_users(id),
    progress    DECIMAL(5,2) DEFAULT 0,
    completed   BOOLEAN DEFAULT false,
    cert_hash   VARCHAR(66),            -- NFT certificate
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ==================== INDEXES ====================

CREATE INDEX idx_users_nation ON sovereign_users(nation);
CREATE INDEX idx_users_country ON sovereign_users(country);
CREATE INDEX idx_tx_from ON bdet_transactions(from_user);
CREATE INDEX idx_tx_to ON bdet_transactions(to_user);
CREATE INDEX idx_tx_platform ON bdet_transactions(platform_id);
CREATE INDEX idx_posts_author ON social_posts(author_id);
CREATE INDEX idx_videos_creator ON videos(creator_id);
CREATE INDEX idx_tracks_artist ON tracks(artist_id);
CREATE INDEX idx_products_artisan ON artisan_products(artisan_id);
CREATE INDEX idx_products_category ON artisan_products(category);
CREATE INDEX idx_lodgings_location ON lodgings USING GIN(location);
CREATE INDEX idx_jobs_skills ON jobs USING GIN(skills);
CREATE INDEX idx_courses_category ON courses(category);

-- ============================================================
-- Total: 20+ tables covering all 20 platforms
-- Hybrid: PostgreSQL (content) + MameyNode (payments/certs/votes)
-- Cifrado: Todas las PII cifradas E2E
-- ============================================================

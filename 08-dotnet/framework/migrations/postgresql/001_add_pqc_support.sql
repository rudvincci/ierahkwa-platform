-- Migration: 001_add_pqc_support.sql
-- Description: Create wallet_keys_v2 table with PQC key support and key migration logging.
-- This script is intended for PostgreSQL.

BEGIN;

-- Create new wallet_keys_v2 table with PQC-friendly schema
CREATE TABLE IF NOT EXISTS wallet_keys_v2 (
    key_id              UUID        PRIMARY KEY,
    user_id             UUID        NOT NULL,
    public_key          BYTEA       NOT NULL,
    encrypted_private_key BYTEA     NOT NULL,
    algorithm           VARCHAR(50) NOT NULL,
    is_primary          BOOLEAN     DEFAULT FALSE NOT NULL,
    is_active           BOOLEAN     DEFAULT TRUE  NOT NULL,
    is_quantum_resistant BOOLEAN    DEFAULT FALSE NOT NULL,
    security_level      INTEGER     DEFAULT 0     NOT NULL,
    nist_status         VARCHAR(50),
    linked_classical_key_id UUID,
    linked_pq_key_id    UUID,
    migrated_from_key_id UUID,
    migration_date      TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_wallet_keys_v2_linked_classical
        FOREIGN KEY (linked_classical_key_id) REFERENCES wallet_keys_v2(key_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_wallet_keys_v2_linked_pq
        FOREIGN KEY (linked_pq_key_id) REFERENCES wallet_keys_v2(key_id)
        ON DELETE SET NULL
);

-- Enforce maximum sizes for key material to support larger PQC keys
ALTER TABLE wallet_keys_v2
    ADD CONSTRAINT ck_wallet_keys_v2_public_key_size
        CHECK (octet_length(public_key) <= 4096), -- up to 4KB for public keys
    ADD CONSTRAINT ck_wallet_keys_v2_encrypted_private_key_size
        CHECK (octet_length(encrypted_private_key) <= 8192); -- up to 8KB for private keys

-- Indexes for common query patterns
CREATE INDEX IF NOT EXISTS idx_wallet_keys_user_id
    ON wallet_keys_v2 (user_id);

CREATE INDEX IF NOT EXISTS idx_wallet_keys_algorithm
    ON wallet_keys_v2 (algorithm);

CREATE INDEX IF NOT EXISTS idx_wallet_keys_quantum_resistant
    ON wallet_keys_v2 (is_quantum_resistant);

-- Optional: additional index to speed up lookups by linked key IDs
CREATE INDEX IF NOT EXISTS idx_wallet_keys_linked_classical
    ON wallet_keys_v2 (linked_classical_key_id);

CREATE INDEX IF NOT EXISTS idx_wallet_keys_linked_pq
    ON wallet_keys_v2 (linked_pq_key_id);

-- Create key_migration_log table to track key migration events
CREATE TABLE IF NOT EXISTS key_migration_log (
    id                  BIGSERIAL   PRIMARY KEY,
    classical_key_id    UUID        NOT NULL,
    pq_key_id           UUID        NOT NULL,
    classical_algorithm VARCHAR(50) NOT NULL,
    pq_algorithm        VARCHAR(50) NOT NULL,
    requested_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at        TIMESTAMPTZ,
    status              VARCHAR(50) NOT NULL,
    error_message       TEXT
);

-- Indexes for migration lookups
CREATE INDEX IF NOT EXISTS idx_key_migration_classical_key
    ON key_migration_log (classical_key_id);

CREATE INDEX IF NOT EXISTS idx_key_migration_pq_key
    ON key_migration_log (pq_key_id);

COMMIT;



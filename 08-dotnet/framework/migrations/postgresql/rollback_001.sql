-- Rollback wrapper for migration 001 (PQC support)
-- Delegates to 001_add_pqc_support_rollback.sql for backwards compatibility.

BEGIN;

-- Drop PQC-related key migration structures
DROP INDEX IF EXISTS idx_key_migration_pq_key;
DROP INDEX IF EXISTS idx_key_migration_classical_key;
DROP TABLE IF EXISTS key_migration_log;

DROP INDEX IF EXISTS idx_wallet_keys_linked_pq;
DROP INDEX IF EXISTS idx_wallet_keys_linked_classical;
DROP INDEX IF EXISTS idx_wallet_keys_quantum_resistant;
DROP INDEX IF EXISTS idx_wallet_keys_algorithm;
DROP INDEX IF EXISTS idx_wallet_keys_user_id;

ALTER TABLE IF EXISTS wallet_keys_v2
    DROP CONSTRAINT IF EXISTS ck_wallet_keys_v2_encrypted_private_key_size,
    DROP CONSTRAINT IF EXISTS ck_wallet_keys_v2_public_key_size,
    DROP CONSTRAINT IF EXISTS fk_wallet_keys_v2_linked_pq,
    DROP CONSTRAINT IF EXISTS fk_wallet_keys_v2_linked_classical;

DROP TABLE IF EXISTS wallet_keys_v2;

COMMIT;



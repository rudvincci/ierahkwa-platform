#!/usr/bin/env bash
set -euo pipefail

# Simple validation script for PQC database migrations.
#
# Usage:
#   PGDATABASE=mydb PGUSER=myuser PGPASSWORD=mypassword \
#   MONGO_URI="mongodb://localhost:27017/wallet" \
#   ./validate.sh
#
# Exit codes:
#   0 - All checks passed
#   1 - PostgreSQL validation failed
#   2 - MongoDB validation failed

PG_STATUS=0
MONGO_STATUS=0

validate_postgres() {
  echo "[validate] Validating PostgreSQL schema..."

  # Ensure wallet_keys_v2 table exists and required columns are present
  psql -XAt <<'SQL' || return 1
SELECT 'wallet_keys_v2_table_ok'
WHERE EXISTS (
  SELECT 1
  FROM information_schema.columns
  WHERE table_name = 'wallet_keys_v2'
    AND column_name IN (
      'public_key',
      'encrypted_private_key',
      'algorithm',
      'is_quantum_resistant',
      'security_level',
      'nist_status',
      'linked_classical_key_id',
      'linked_pq_key_id',
      'migrated_from_key_id',
      'migration_date'
    )
);

-- Check indexes
SELECT 'wallet_keys_v2_indexes_ok'
WHERE EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_wallet_keys_user_id')
  AND EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_wallet_keys_algorithm')
  AND EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_wallet_keys_quantum_resistant');

-- Optional data loss check: if legacy wallet_keys exists, compare row counts.
DO $$
BEGIN
  IF to_regclass('public.wallet_keys') IS NOT NULL THEN
    IF (SELECT count(*) FROM wallet_keys) <> (SELECT count(*) FROM wallet_keys_v2) THEN
      RAISE EXCEPTION 'Row count mismatch between wallet_keys and wallet_keys_v2';
    END IF;
  END IF;
END$$;
SQL
}

validate_mongo() {
  echo "[validate] Validating MongoDB schema..."

  if [[ -z "${MONGO_URI:-}" ]]; then
    echo "[validate] MONGO_URI not set, skipping MongoDB checks." >&2
    return 0
  fi

  mongo "$MONGO_URI" --quiet <<'JS' || exit 2
(function() {
  var collName = 'wallet_keys_v2';
  if (!db.getCollectionNames().includes(collName)) {
    print("wallet_keys_v2 collection does not exist");
    quit(1);
  }

  var coll = db.getCollection(collName);
  var idx = coll.getIndexes().map(function(i) { return i.name; });
  var required = ['idx_wallet_keys_user_id', 'idx_wallet_keys_algorithm', 'idx_wallet_keys_quantum_resistant'];
  for (var i = 0; i < required.length; i++) {
    if (idx.indexOf(required[i]) < 0) {
      print("Missing index: " + required[i]);
      quit(1);
    }
  }

  print("MongoDB validation OK");
})();
JS
}

if ! validate_postgres; then
  echo "[validate] PostgreSQL validation failed" >&2
  PG_STATUS=1
else
  echo "[validate] PostgreSQL validation OK"
fi

if ! validate_mongo; then
  echo "[validate] MongoDB validation failed" >&2
  MONGO_STATUS=2
else
  echo "[validate] MongoDB validation OK"
fi

if [[ $PG_STATUS -ne 0 ]]; then
  exit 1
fi

if [[ $MONGO_STATUS -ne 0 ]]; then
  exit 2
fi

exit 0



// Migration: 001_add_pqc_support.js
// Description: Create/update wallet key storage schema for PQC support in MongoDB.

(function() {
  // Adjust this if your wallet database has a different name
  var dbName = db.getName();
  var walletCollectionName = 'wallet_keys_v2';

  // Ensure wallet_keys_v2 collection exists
  if (!db.getCollectionNames().includes(walletCollectionName)) {
    db.createCollection(walletCollectionName);
  }

  var coll = db.getCollection(walletCollectionName);

  // Create indexes for common query patterns
  coll.createIndex({ user_id: 1 }, { name: 'idx_wallet_keys_user_id' });
  coll.createIndex({ algorithm: 1 }, { name: 'idx_wallet_keys_algorithm' });
  coll.createIndex({ quantum_resistant: 1 }, { name: 'idx_wallet_keys_quantum_resistant' });

  // Optionally migrate existing keys from legacy collection if present
  var legacyCollectionName = 'wallet_keys';
  if (db.getCollectionNames().includes(legacyCollectionName)) {
    var legacy = db.getCollection(legacyCollectionName);
    legacy.find({}).forEach(function(doc) {
      var newDoc = {
        key_id: doc.key_id,
        user_id: doc.user_id,
        public_key: doc.public_key,
        encrypted_private_key: doc.encrypted_private_key,
        algorithm: doc.algorithm,
        is_primary: doc.is_primary || false,
        is_active: doc.is_active !== false,
        quantum_resistant: false,
        security_level: 0,
        nist_status: 'classical',
        linked_classical_key_id: null,
        linked_pq_key_id: null,
        migrated_from_key_id: null,
        migration_date: null,
        created_at: doc.created_at || new Date(),
        updated_at: doc.updated_at || new Date()
      };

      coll.update({ key_id: newDoc.key_id }, newDoc, { upsert: true });
    });
  }
})();



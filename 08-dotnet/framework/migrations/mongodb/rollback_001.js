// Rollback wrapper for migration 001 (PQC support)
// Reverts changes applied by 001_add_pqc_support.js.

(function() {
  var walletCollectionName = 'wallet_keys_v2';
  if (db.getCollectionNames().includes(walletCollectionName)) {
    var coll = db.getCollection(walletCollectionName);

    // Drop indexes if they exist
    try { coll.dropIndex('idx_wallet_keys_user_id'); } catch (e) {}
    try { coll.dropIndex('idx_wallet_keys_algorithm'); } catch (e) {}
    try { coll.dropIndex('idx_wallet_keys_quantum_resistant'); } catch (e) {}

    // Drop collection
    coll.drop();
  }
})();



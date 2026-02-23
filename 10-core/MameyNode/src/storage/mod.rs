//! Storage layer for MameyNode

use std::path::PathBuf;
use serde::{de::DeserializeOwned, Serialize};

/// Storage backend trait
pub trait StorageBackend {
    fn get(&self, key: &[u8]) -> Option<Vec<u8>>;
    fn put(&self, key: &[u8], value: &[u8]) -> Result<(), String>;
    fn delete(&self, key: &[u8]) -> Result<(), String>;
    fn exists(&self, key: &[u8]) -> bool;
}

/// In-memory storage (for development)
pub struct MemoryStorage {
    data: std::sync::RwLock<std::collections::HashMap<Vec<u8>, Vec<u8>>>,
}

impl MemoryStorage {
    pub fn new() -> Self {
        Self {
            data: std::sync::RwLock::new(std::collections::HashMap::new()),
        }
    }
}

impl Default for MemoryStorage {
    fn default() -> Self {
        Self::new()
    }
}

impl StorageBackend for MemoryStorage {
    fn get(&self, key: &[u8]) -> Option<Vec<u8>> {
        self.data.read().ok()?.get(key).cloned()
    }
    
    fn put(&self, key: &[u8], value: &[u8]) -> Result<(), String> {
        self.data
            .write()
            .map_err(|e| e.to_string())?
            .insert(key.to_vec(), value.to_vec());
        Ok(())
    }
    
    fn delete(&self, key: &[u8]) -> Result<(), String> {
        self.data
            .write()
            .map_err(|e| e.to_string())?
            .remove(key);
        Ok(())
    }
    
    fn exists(&self, key: &[u8]) -> bool {
        self.data.read().map(|d| d.contains_key(key)).unwrap_or(false)
    }
}

/// Database keys
pub mod keys {
    pub const BLOCK_PREFIX: &[u8] = b"block:";
    pub const BLOCK_HASH_PREFIX: &[u8] = b"blockhash:";
    pub const TX_PREFIX: &[u8] = b"tx:";
    pub const ACCOUNT_PREFIX: &[u8] = b"account:";
    pub const TOKEN_PREFIX: &[u8] = b"token:";
    pub const STATE_PREFIX: &[u8] = b"state:";
    pub const META_PREFIX: &[u8] = b"meta:";
    
    pub fn block_key(number: u64) -> Vec<u8> {
        let mut key = BLOCK_PREFIX.to_vec();
        key.extend_from_slice(&number.to_be_bytes());
        key
    }
    
    pub fn block_hash_key(hash: &str) -> Vec<u8> {
        let mut key = BLOCK_HASH_PREFIX.to_vec();
        key.extend_from_slice(hash.as_bytes());
        key
    }
    
    pub fn tx_key(hash: &str) -> Vec<u8> {
        let mut key = TX_PREFIX.to_vec();
        key.extend_from_slice(hash.as_bytes());
        key
    }
    
    pub fn account_key(address: &str) -> Vec<u8> {
        let mut key = ACCOUNT_PREFIX.to_vec();
        key.extend_from_slice(address.as_bytes());
        key
    }
    
    pub fn token_key(symbol: &str) -> Vec<u8> {
        let mut key = TOKEN_PREFIX.to_vec();
        key.extend_from_slice(symbol.as_bytes());
        key
    }
}

/// Storage manager
pub struct StorageManager<S: StorageBackend> {
    backend: S,
}

impl<S: StorageBackend> StorageManager<S> {
    pub fn new(backend: S) -> Self {
        Self { backend }
    }
    
    pub fn get<T: DeserializeOwned>(&self, key: &[u8]) -> Option<T> {
        self.backend
            .get(key)
            .and_then(|data| serde_json::from_slice(&data).ok())
    }
    
    pub fn put<T: Serialize>(&self, key: &[u8], value: &T) -> Result<(), String> {
        let data = serde_json::to_vec(value).map_err(|e| e.to_string())?;
        self.backend.put(key, &data)
    }
    
    pub fn delete(&self, key: &[u8]) -> Result<(), String> {
        self.backend.delete(key)
    }
    
    pub fn exists(&self, key: &[u8]) -> bool {
        self.backend.exists(key)
    }
}

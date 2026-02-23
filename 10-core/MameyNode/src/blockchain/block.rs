//! Block structure

use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use sha2::{Sha256, Digest};

use super::Transaction;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Block {
    pub number: u64,
    pub hash: String,
    pub parent_hash: String,
    pub timestamp: DateTime<Utc>,
    pub transactions: Vec<Transaction>,
    pub transactions_root: String,
    pub state_root: String,
    pub receipts_root: String,
    pub miner: String,
    pub gas_used: u64,
    pub gas_limit: u64,
    pub base_fee: u64,
    pub extra_data: String,
}

impl Block {
    pub fn compute_hash(&self) -> String {
        let mut hasher = Sha256::new();
        hasher.update(self.number.to_le_bytes());
        hasher.update(self.parent_hash.as_bytes());
        hasher.update(self.timestamp.timestamp().to_le_bytes());
        hasher.update(self.transactions_root.as_bytes());
        hasher.update(self.state_root.as_bytes());
        hasher.update(self.miner.as_bytes());
        hasher.update(self.extra_data.as_bytes());
        
        format!("0x{}", hex::encode(hasher.finalize()))
    }
    
    pub fn is_valid(&self) -> bool {
        self.hash == self.compute_hash()
    }
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct BlockHeader {
    pub number: u64,
    pub hash: String,
    pub parent_hash: String,
    pub timestamp: DateTime<Utc>,
    pub transactions_count: usize,
    pub miner: String,
    pub gas_used: u64,
}

impl From<&Block> for BlockHeader {
    fn from(block: &Block) -> Self {
        Self {
            number: block.number,
            hash: block.hash.clone(),
            parent_hash: block.parent_hash.clone(),
            timestamp: block.timestamp,
            transactions_count: block.transactions.len(),
            miner: block.miner.clone(),
            gas_used: block.gas_used,
        }
    }
}

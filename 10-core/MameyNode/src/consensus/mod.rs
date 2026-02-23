//! Consensus mechanism for MameyNode
//! Uses Proof of Authority (PoA) for sovereign governance

use std::collections::HashSet;
use std::sync::Arc;
use tokio::sync::RwLock;
use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};

use crate::blockchain::{Block, Blockchain};

/// Proof of Authority consensus
pub struct PoAConsensus {
    validators: HashSet<String>,
    current_validator_index: usize,
    block_time_ms: u64,
    last_block_time: DateTime<Utc>,
}

impl PoAConsensus {
    pub fn new(validators: Vec<String>, block_time_ms: u64) -> Self {
        Self {
            validators: validators.into_iter().collect(),
            current_validator_index: 0,
            block_time_ms,
            last_block_time: Utc::now(),
        }
    }
    
    /// Check if an address is a validator
    pub fn is_validator(&self, address: &str) -> bool {
        self.validators.contains(address)
    }
    
    /// Get the current block producer
    pub fn get_current_producer(&self) -> Option<&String> {
        let validators: Vec<_> = self.validators.iter().collect();
        validators.get(self.current_validator_index).copied()
    }
    
    /// Rotate to next validator
    pub fn rotate_validator(&mut self) {
        self.current_validator_index = (self.current_validator_index + 1) % self.validators.len().max(1);
    }
    
    /// Check if it's time to produce a new block
    pub fn should_produce_block(&self) -> bool {
        let elapsed = Utc::now().signed_duration_since(self.last_block_time);
        elapsed.num_milliseconds() as u64 >= self.block_time_ms
    }
    
    /// Validate a block
    pub fn validate_block(&self, block: &Block, parent: &Block) -> Result<(), String> {
        // Check block number
        if block.number != parent.number + 1 {
            return Err("Invalid block number".to_string());
        }
        
        // Check parent hash
        if block.parent_hash != parent.hash {
            return Err("Invalid parent hash".to_string());
        }
        
        // Check timestamp
        if block.timestamp <= parent.timestamp {
            return Err("Invalid timestamp".to_string());
        }
        
        // Check miner is validator
        if !self.is_validator(&block.miner) {
            return Err("Miner is not a validator".to_string());
        }
        
        // Check block hash
        if block.hash != block.compute_hash() {
            return Err("Invalid block hash".to_string());
        }
        
        Ok(())
    }
    
    /// Add a new validator
    pub fn add_validator(&mut self, address: String) -> bool {
        self.validators.insert(address)
    }
    
    /// Remove a validator
    pub fn remove_validator(&mut self, address: &str) -> bool {
        self.validators.remove(address)
    }
    
    /// Get all validators
    pub fn get_validators(&self) -> Vec<&String> {
        self.validators.iter().collect()
    }
}

/// Governance proposal for validator changes
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ValidatorProposal {
    pub id: String,
    pub proposal_type: ValidatorProposalType,
    pub address: String,
    pub proposer: String,
    pub votes_for: u64,
    pub votes_against: u64,
    pub created_at: DateTime<Utc>,
    pub expires_at: DateTime<Utc>,
    pub executed: bool,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum ValidatorProposalType {
    Add,
    Remove,
}

/// Start the consensus loop
pub async fn run_consensus(
    blockchain: Arc<RwLock<Blockchain>>,
    consensus: Arc<RwLock<PoAConsensus>>,
    validator_address: String,
) {
    loop {
        {
            let mut consensus = consensus.write().await;
            
            if consensus.should_produce_block() && consensus.is_validator(&validator_address) {
                let mut blockchain = blockchain.write().await;
                let block = blockchain.mine_block(&validator_address);
                
                tracing::info!(
                    "Produced block #{} with {} transactions",
                    block.number,
                    block.transactions.len()
                );
                
                consensus.rotate_validator();
                consensus.last_block_time = Utc::now();
            }
        }
        
        tokio::time::sleep(tokio::time::Duration::from_millis(100)).await;
    }
}

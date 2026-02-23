//! Blockchain core implementation

use std::collections::HashMap;
use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use sha2::{Sha256, Digest};

pub mod block;
pub mod transaction;
pub mod account;
pub mod token;
pub mod state;

pub use block::Block;
pub use transaction::Transaction;
pub use account::Account;
pub use token::Token;

/// Main blockchain structure
#[derive(Debug)]
pub struct Blockchain {
    pub chain_id: u64,
    pub blocks: Vec<Block>,
    pub pending_transactions: Vec<Transaction>,
    pub accounts: HashMap<String, Account>,
    pub tokens: HashMap<String, Token>,
    pub state_root: String,
}

impl Blockchain {
    pub fn new(chain_id: u64) -> Self {
        let mut blockchain = Self {
            chain_id,
            blocks: Vec::new(),
            pending_transactions: Vec::new(),
            accounts: HashMap::new(),
            tokens: HashMap::new(),
            state_root: String::new(),
        };
        
        // Create genesis block
        blockchain.create_genesis_block();
        
        // Initialize native tokens
        blockchain.initialize_native_tokens();
        
        blockchain
    }
    
    fn create_genesis_block(&mut self) {
        let genesis = Block {
            number: 0,
            hash: "0x0000000000000000000000000000000000000000000000000000000000000000".to_string(),
            parent_hash: "0x0000000000000000000000000000000000000000000000000000000000000000".to_string(),
            timestamp: Utc::now(),
            transactions: Vec::new(),
            transactions_root: self.compute_merkle_root(&[]),
            state_root: String::new(),
            receipts_root: String::new(),
            miner: "0x0000000000000000000000000000000000000000".to_string(),
            gas_used: 0,
            gas_limit: 30_000_000,
            base_fee: 0, // Zero gas for sovereign chain
            extra_data: "Ierahkwa Sovereign Genesis - Chain ID 777777".to_string(),
        };
        
        self.blocks.push(genesis);
    }
    
    fn initialize_native_tokens(&mut self) {
        // WAMPUM - Primary sovereign currency
        self.tokens.insert("WAMPUM".to_string(), Token {
            symbol: "WAMPUM".to_string(),
            name: "Sovereign Wampum".to_string(),
            decimals: 18,
            total_supply: 1_000_000_000_000_000_000_000_000_000u128, // 1 billion
            owner: "0x0000000000000000000000000000000000000001".to_string(),
            mintable: true,
            burnable: true,
            pausable: true,
            is_native: true,
        });
        
        // SICBDC - Central Bank Digital Currency
        self.tokens.insert("SICBDC".to_string(), Token {
            symbol: "SICBDC".to_string(),
            name: "Sovereign Indigenous Central Bank Digital Currency".to_string(),
            decimals: 18,
            total_supply: 0, // Minted on demand by treasury
            owner: "0x0000000000000000000000000000000000000002".to_string(),
            mintable: true,
            burnable: true,
            pausable: true,
            is_native: true,
        });
        
        // IGT - Ierahkwa Governance Token
        self.tokens.insert("IGT".to_string(), Token {
            symbol: "IGT".to_string(),
            name: "Ierahkwa Governance Token".to_string(),
            decimals: 18,
            total_supply: 103_000_000_000_000_000_000_000_000u128, // 103 million (for 103 departments)
            owner: "0x0000000000000000000000000000000000000003".to_string(),
            mintable: false,
            burnable: true,
            pausable: false,
            is_native: true,
        });
    }
    
    pub fn add_transaction(&mut self, tx: Transaction) -> Result<String, String> {
        // Validate transaction
        if !self.validate_transaction(&tx) {
            return Err("Invalid transaction".to_string());
        }
        
        let tx_hash = tx.hash.clone();
        self.pending_transactions.push(tx);
        
        Ok(tx_hash)
    }
    
    pub fn mine_block(&mut self, miner: &str) -> Block {
        let parent = self.blocks.last().unwrap();
        let transactions = std::mem::take(&mut self.pending_transactions);
        
        let block = Block {
            number: parent.number + 1,
            hash: String::new(), // Will be computed
            parent_hash: parent.hash.clone(),
            timestamp: Utc::now(),
            transactions_root: self.compute_merkle_root(&transactions.iter().map(|t| t.hash.as_str()).collect::<Vec<_>>()),
            state_root: self.state_root.clone(),
            receipts_root: String::new(),
            transactions,
            miner: miner.to_string(),
            gas_used: 0,
            gas_limit: 30_000_000,
            base_fee: 0,
            extra_data: String::new(),
        };
        
        // Compute block hash
        let mut block = block;
        block.hash = block.compute_hash();
        
        self.blocks.push(block.clone());
        block
    }
    
    fn validate_transaction(&self, tx: &Transaction) -> bool {
        // Basic validation
        !tx.hash.is_empty() && !tx.from.is_empty()
    }
    
    fn compute_merkle_root(&self, items: &[&str]) -> String {
        if items.is_empty() {
            return "0x0000000000000000000000000000000000000000000000000000000000000000".to_string();
        }
        
        let mut hasher = Sha256::new();
        for item in items {
            hasher.update(item.as_bytes());
        }
        format!("0x{}", hex::encode(hasher.finalize()))
    }
    
    pub fn get_balance(&self, address: &str, token: &str) -> u128 {
        self.accounts
            .get(address)
            .and_then(|acc| acc.balances.get(token))
            .copied()
            .unwrap_or(0)
    }
    
    pub fn get_block(&self, number: u64) -> Option<&Block> {
        self.blocks.get(number as usize)
    }
    
    pub fn get_latest_block(&self) -> Option<&Block> {
        self.blocks.last()
    }
    
    pub fn get_block_count(&self) -> u64 {
        self.blocks.len() as u64
    }
    
    pub fn get_transaction_count(&self) -> u64 {
        self.blocks.iter().map(|b| b.transactions.len() as u64).sum()
    }
}

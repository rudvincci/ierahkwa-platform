//! Transaction structure

use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use sha2::{Sha256, Digest};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Transaction {
    pub hash: String,
    pub from: String,
    pub to: String,
    pub value: u128,
    pub data: Option<String>,
    pub nonce: u64,
    pub gas_price: u64,
    pub gas_limit: u64,
    pub signature: Option<TransactionSignature>,
    pub timestamp: DateTime<Utc>,
    pub tx_type: TransactionType,
    pub status: TransactionStatus,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TransactionSignature {
    pub v: u8,
    pub r: String,
    pub s: String,
}

#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
pub enum TransactionType {
    Transfer,
    TokenTransfer,
    TokenCreate,
    TokenMint,
    TokenBurn,
    ContractCall,
    ContractDeploy,
    GovernanceVote,
    GovernanceProposal,
    IdentityRegister,
    IdentityVerify,
    BridgeDeposit,
    BridgeWithdraw,
    TreasuryDisbursement,
    TreasuryIssuance,
}

#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
pub enum TransactionStatus {
    Pending,
    Confirmed,
    Failed,
    Reverted,
}

impl Transaction {
    pub fn new(from: String, to: String, value: u128, tx_type: TransactionType) -> Self {
        let mut tx = Self {
            hash: String::new(),
            from,
            to,
            value,
            data: None,
            nonce: 0,
            gas_price: 0,
            gas_limit: 21000,
            signature: None,
            timestamp: Utc::now(),
            tx_type,
            status: TransactionStatus::Pending,
        };
        tx.hash = tx.compute_hash();
        tx
    }
    
    pub fn compute_hash(&self) -> String {
        let mut hasher = Sha256::new();
        hasher.update(self.from.as_bytes());
        hasher.update(self.to.as_bytes());
        hasher.update(self.value.to_le_bytes());
        hasher.update(self.nonce.to_le_bytes());
        hasher.update(self.timestamp.timestamp().to_le_bytes());
        
        if let Some(data) = &self.data {
            hasher.update(data.as_bytes());
        }
        
        format!("0x{}", hex::encode(hasher.finalize()))
    }
    
    pub fn with_data(mut self, data: String) -> Self {
        self.data = Some(data);
        self.hash = self.compute_hash();
        self
    }
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TransactionReceipt {
    pub transaction_hash: String,
    pub block_number: u64,
    pub block_hash: String,
    pub from: String,
    pub to: String,
    pub status: TransactionStatus,
    pub gas_used: u64,
    pub logs: Vec<TransactionLog>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TransactionLog {
    pub address: String,
    pub topics: Vec<String>,
    pub data: String,
    pub log_index: u32,
}

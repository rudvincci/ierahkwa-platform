//! Account structure

use std::collections::HashMap;
use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Account {
    pub address: String,
    pub nonce: u64,
    pub balances: HashMap<String, u128>,
    pub code_hash: Option<String>,
    pub storage_root: Option<String>,
    pub is_contract: bool,
    pub created_at: i64,
    
    // FutureWampumID integration
    pub fwid: Option<String>,
    pub kyc_level: KycLevel,
    pub identity_verified: bool,
}

#[derive(Debug, Clone, Serialize, Deserialize, PartialEq)]
pub enum KycLevel {
    None,
    Basic,      // Email verified
    Standard,   // Phone + Email
    Enhanced,   // Document verified
    Full,       // Biometric verified
}

impl Default for KycLevel {
    fn default() -> Self {
        Self::None
    }
}

impl Account {
    pub fn new(address: String) -> Self {
        let mut balances = HashMap::new();
        balances.insert("WAMPUM".to_string(), 0);
        
        Self {
            address,
            nonce: 0,
            balances,
            code_hash: None,
            storage_root: None,
            is_contract: false,
            created_at: chrono::Utc::now().timestamp(),
            fwid: None,
            kyc_level: KycLevel::None,
            identity_verified: false,
        }
    }
    
    pub fn get_balance(&self, token: &str) -> u128 {
        self.balances.get(token).copied().unwrap_or(0)
    }
    
    pub fn add_balance(&mut self, token: &str, amount: u128) {
        *self.balances.entry(token.to_string()).or_insert(0) += amount;
    }
    
    pub fn subtract_balance(&mut self, token: &str, amount: u128) -> bool {
        if let Some(balance) = self.balances.get_mut(token) {
            if *balance >= amount {
                *balance -= amount;
                return true;
            }
        }
        false
    }
    
    pub fn set_fwid(&mut self, fwid: String) {
        self.fwid = Some(fwid);
        self.identity_verified = true;
    }
    
    pub fn increment_nonce(&mut self) {
        self.nonce += 1;
    }
}

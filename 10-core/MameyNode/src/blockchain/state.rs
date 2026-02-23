//! State management

use std::collections::HashMap;
use serde::{Deserialize, Serialize};
use sha2::{Sha256, Digest};

use super::{Account, Token};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct WorldState {
    pub accounts: HashMap<String, Account>,
    pub tokens: HashMap<String, Token>,
    pub token_balances: HashMap<String, HashMap<String, u128>>, // token -> address -> balance
    pub contracts: HashMap<String, ContractState>,
    pub root_hash: String,
}

impl WorldState {
    pub fn new() -> Self {
        Self {
            accounts: HashMap::new(),
            tokens: HashMap::new(),
            token_balances: HashMap::new(),
            contracts: HashMap::new(),
            root_hash: String::new(),
        }
    }
    
    pub fn compute_root(&mut self) -> String {
        let mut hasher = Sha256::new();
        
        // Hash accounts
        let mut account_hashes: Vec<String> = self.accounts
            .iter()
            .map(|(addr, acc)| {
                let mut h = Sha256::new();
                h.update(addr.as_bytes());
                h.update(acc.nonce.to_le_bytes());
                format!("{:x}", h.finalize())
            })
            .collect();
        account_hashes.sort();
        
        for hash in &account_hashes {
            hasher.update(hash.as_bytes());
        }
        
        self.root_hash = format!("0x{:x}", hasher.finalize());
        self.root_hash.clone()
    }
    
    pub fn get_account(&self, address: &str) -> Option<&Account> {
        self.accounts.get(address)
    }
    
    pub fn get_or_create_account(&mut self, address: &str) -> &mut Account {
        self.accounts
            .entry(address.to_string())
            .or_insert_with(|| Account::new(address.to_string()))
    }
    
    pub fn transfer(
        &mut self,
        from: &str,
        to: &str,
        token: &str,
        amount: u128,
    ) -> Result<(), String> {
        // Get sender balance
        let sender_balance = self.get_balance(from, token);
        if sender_balance < amount {
            return Err("Insufficient balance".to_string());
        }
        
        // Update balances
        self.subtract_balance(from, token, amount);
        self.add_balance(to, token, amount);
        
        Ok(())
    }
    
    pub fn get_balance(&self, address: &str, token: &str) -> u128 {
        self.token_balances
            .get(token)
            .and_then(|balances| balances.get(address))
            .copied()
            .unwrap_or(0)
    }
    
    pub fn add_balance(&mut self, address: &str, token: &str, amount: u128) {
        self.token_balances
            .entry(token.to_string())
            .or_insert_with(HashMap::new)
            .entry(address.to_string())
            .and_modify(|b| *b += amount)
            .or_insert(amount);
    }
    
    pub fn subtract_balance(&mut self, address: &str, token: &str, amount: u128) {
        if let Some(balances) = self.token_balances.get_mut(token) {
            if let Some(balance) = balances.get_mut(address) {
                *balance = balance.saturating_sub(amount);
            }
        }
    }
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ContractState {
    pub address: String,
    pub code_hash: String,
    pub storage: HashMap<String, String>,
    pub owner: String,
    pub created_at: i64,
}

impl Default for WorldState {
    fn default() -> Self {
        Self::new()
    }
}

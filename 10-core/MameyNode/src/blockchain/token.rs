//! Token structure

use std::collections::HashMap;
use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Token {
    pub symbol: String,
    pub name: String,
    pub decimals: u8,
    pub total_supply: u128,
    pub owner: String,
    pub mintable: bool,
    pub burnable: bool,
    pub pausable: bool,
    pub is_native: bool,
}

impl Token {
    pub fn new(
        symbol: String,
        name: String,
        decimals: u8,
        initial_supply: u128,
        owner: String,
    ) -> Self {
        Self {
            symbol,
            name,
            decimals,
            total_supply: initial_supply,
            owner,
            mintable: true,
            burnable: true,
            pausable: false,
            is_native: false,
        }
    }
    
    pub fn format_amount(&self, amount: u128) -> String {
        let divisor = 10u128.pow(self.decimals as u32);
        let whole = amount / divisor;
        let fraction = amount % divisor;
        
        if fraction == 0 {
            format!("{} {}", whole, self.symbol)
        } else {
            format!("{}.{:0width$} {}", whole, fraction, self.symbol, width = self.decimals as usize)
        }
    }
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TokenMetadata {
    pub symbol: String,
    pub name: String,
    pub decimals: u8,
    pub total_supply: String,
    pub holders: u64,
    pub transfers: u64,
    pub created_at: i64,
    pub logo_url: Option<String>,
    pub website: Option<String>,
    pub description: Option<String>,
}

impl From<&Token> for TokenMetadata {
    fn from(token: &Token) -> Self {
        Self {
            symbol: token.symbol.clone(),
            name: token.name.clone(),
            decimals: token.decimals,
            total_supply: token.total_supply.to_string(),
            holders: 0,
            transfers: 0,
            created_at: chrono::Utc::now().timestamp(),
            logo_url: None,
            website: None,
            description: None,
        }
    }
}

/// IGT Token variants (103 department tokens)
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct IgtToken {
    pub id: u8,
    pub symbol: String,
    pub name: String,
    pub department: String,
    pub allocation: u128,
}

pub fn get_igt_tokens() -> Vec<IgtToken> {
    vec![
        IgtToken { id: 1, symbol: "IGT-PM".to_string(), name: "Prime Minister Token".to_string(), department: "Office of Prime Minister".to_string(), allocation: 1_000_000 },
        IgtToken { id: 2, symbol: "IGT-TRES".to_string(), name: "Treasury Token".to_string(), department: "Treasury".to_string(), allocation: 5_000_000 },
        IgtToken { id: 3, symbol: "IGT-FIN".to_string(), name: "Finance Token".to_string(), department: "Finance".to_string(), allocation: 3_000_000 },
        IgtToken { id: 4, symbol: "IGT-JUST".to_string(), name: "Justice Token".to_string(), department: "Justice".to_string(), allocation: 2_000_000 },
        IgtToken { id: 5, symbol: "IGT-EDU".to_string(), name: "Education Token".to_string(), department: "Education".to_string(), allocation: 4_000_000 },
        IgtToken { id: 6, symbol: "IGT-HEALTH".to_string(), name: "Health Token".to_string(), department: "Health".to_string(), allocation: 4_000_000 },
        IgtToken { id: 7, symbol: "IGT-DEF".to_string(), name: "Defense Token".to_string(), department: "Defense".to_string(), allocation: 2_000_000 },
        IgtToken { id: 8, symbol: "IGT-TRADE".to_string(), name: "Trade Token".to_string(), department: "Trade & Commerce".to_string(), allocation: 3_000_000 },
        IgtToken { id: 9, symbol: "IGT-TECH".to_string(), name: "Technology Token".to_string(), department: "Technology".to_string(), allocation: 5_000_000 },
        IgtToken { id: 10, symbol: "IGT-ENV".to_string(), name: "Environment Token".to_string(), department: "Environment".to_string(), allocation: 2_000_000 },
        // ... 93 more department tokens defined similarly
    ]
}

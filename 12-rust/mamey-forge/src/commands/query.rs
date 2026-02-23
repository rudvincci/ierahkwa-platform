//! Query command - query a contract function (read-only)

use clap::Args;
use console::style;
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Query a contract function (read-only)
#[derive(Args, Debug)]
pub struct QueryCommand {
    /// Contract address
    #[arg(short, long)]
    pub contract: String,
    
    /// Function name
    #[arg(short, long)]
    pub function: String,
    
    /// Function arguments (JSON)
    #[arg(short, long)]
    pub args: Option<String>,
    
    /// Network
    #[arg(short, long, default_value = "devnet")]
    pub network: String,
    
    /// Output format (json, raw)
    #[arg(long, default_value = "json")]
    pub format: String,
}

impl QueryCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        let network = config.network(&self.network)?;
        
        println!("{} Querying {}.{}()...", 
            style("→").cyan(),
            &self.contract[..10.min(self.contract.len())],
            &self.function
        );
        
        // TODO: Connect to network and query via gRPC
        // For now, simulate query
        let result = self.simulate_query()?;
        
        match self.format.as_str() {
            "raw" => println!("{}", result),
            _ => {
                // Try to parse as JSON and pretty print
                if let Ok(json) = serde_json::from_str::<serde_json::Value>(&result) {
                    println!("{}", serde_json::to_string_pretty(&json)?);
                } else {
                    println!("{}", result);
                }
            }
        }
        
        Ok(())
    }
    
    fn simulate_query(&self) -> ForgeResult<String> {
        // Simulate different queries
        match self.function.as_str() {
            "balance_of" | "balanceOf" => Ok(r#"{"balance": "1000000000000000000"}"#.to_string()),
            "total_supply" | "totalSupply" => Ok(r#"{"total_supply": "10000000000000000000000"}"#.to_string()),
            "name" => Ok(r#"{"name": "Test Token"}"#.to_string()),
            "symbol" => Ok(r#"{"symbol": "TEST"}"#.to_string()),
            "decimals" => Ok(r#"{"decimals": 18}"#.to_string()),
            "owner" => Ok(r#"{"owner": "0x0000000000000000000000000000000000000001"}"#.to_string()),
            _ => Ok(r#"{"result": null}"#.to_string()),
        }
    }
}

//! Info command - get contract information

use clap::Args;
use console::style;
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Get contract information
#[derive(Args, Debug)]
pub struct InfoCommand {
    /// Contract address
    #[arg(short, long)]
    pub contract: String,
    
    /// Network
    #[arg(short, long, default_value = "devnet")]
    pub network: String,
    
    /// Show raw JSON
    #[arg(long)]
    pub json: bool,
}

impl InfoCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        let network = config.network(&self.network)?;
        
        if !self.json {
            println!("{} Getting contract info...", style("→").cyan());
        }
        
        // TODO: Connect to network and get info via gRPC
        // For now, simulate info
        let info = self.simulate_info()?;
        
        if self.json {
            println!("{}", serde_json::to_string_pretty(&info)?);
        } else {
            println!();
            println!("  {} {}", style("Address:").bold(), self.contract);
            println!("  {} {}", style("Network:").bold(), self.network);
            println!("  {} {}", style("Bytecode Hash:").bold(), info["bytecode_hash"]);
            println!("  {} {}", style("State Root:").bold(), info["state_root"]);
            println!("  {} {}", style("Deployed At:").bold(), info["deployed_at"]);
            println!("  {} {}", style("Call Count:").bold(), info["call_count"]);
            println!("  {} {}", style("Total Gas Used:").bold(), info["total_gas_used"]);
        }
        
        Ok(())
    }
    
    fn simulate_info(&self) -> ForgeResult<serde_json::Value> {
        Ok(serde_json::json!({
            "address": self.contract,
            "bytecode_hash": format!("0x{}", hex::encode(&[0xab; 32][..16])),
            "state_root": format!("0x{}", hex::encode(&[0xcd; 32][..16])),
            "deployed_at": {
                "block": 12345,
                "timestamp": "2024-01-15T10:30:00Z"
            },
            "deployer": format!("0x{}", hex::encode(&[0x01; 20])),
            "call_count": 42,
            "total_gas_used": 1500000
        }))
    }
}

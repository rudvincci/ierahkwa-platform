//! Call command - call a contract function (state-changing)

use clap::Args;
use console::style;
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Call a contract function
#[derive(Args, Debug)]
pub struct CallCommand {
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
    
    /// Gas limit
    #[arg(long)]
    pub gas_limit: Option<u64>,
    
    /// Dry run
    #[arg(long)]
    pub dry_run: bool,
}

impl CallCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        let network = config.network(&self.network)?;
        
        println!("{} Calling {}.{}()...", 
            style("→").cyan(),
            &self.contract[..10],
            &self.function
        );
        
        println!("  Contract: {}", style(&self.contract).dim());
        println!("  Function: {}", &self.function);
        if let Some(ref args) = self.args {
            println!("  Args: {}", args);
        }
        println!("  Network: {} ({})", self.network, network.url);
        
        if self.dry_run {
            println!("{} Dry run - not calling", style("⚠").yellow());
            return Ok(());
        }
        
        // TODO: Connect to network and call via gRPC
        // For now, simulate call
        let result = self.simulate_call()?;
        
        println!("{} Call successful!", style("✓").green());
        println!("  Transaction: {}", style(&result.tx_hash).cyan());
        println!("  Gas used: {}", result.gas_used);
        
        if !result.return_data.is_empty() {
            println!("  Return: {}", result.return_data);
        }
        
        Ok(())
    }
    
    fn simulate_call(&self) -> ForgeResult<CallResult> {
        Ok(CallResult {
            tx_hash: format!("0x{}", hex::encode(&[0u8; 32][..16])),
            gas_used: 50000,
            return_data: "".to_string(),
            success: true,
        })
    }
}

struct CallResult {
    tx_hash: String,
    gas_used: u64,
    return_data: String,
    success: bool,
}

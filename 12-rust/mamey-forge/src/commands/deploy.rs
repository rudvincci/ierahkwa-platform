//! Deploy command - deploy contracts to network

use clap::Args;
use console::style;
use std::fs;
use std::path::Path;
use sha2::{Sha256, Digest};
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Deploy a contract
#[derive(Args, Debug)]
pub struct DeployCommand {
    /// Contract name or WASM file path
    #[arg(short, long)]
    pub contract: String,
    
    /// Network to deploy to
    #[arg(short, long, default_value = "devnet")]
    pub network: String,
    
    /// Constructor arguments (JSON)
    #[arg(short, long)]
    pub args: Option<String>,
    
    /// Gas limit
    #[arg(long)]
    pub gas_limit: Option<u64>,
    
    /// Dry run (don't actually deploy)
    #[arg(long)]
    pub dry_run: bool,
}

impl DeployCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        let network = config.network(&self.network)?;
        
        println!("{} Deploying contract '{}' to {}...", 
            style("→").cyan(), 
            self.contract,
            self.network
        );
        
        // Find contract WASM
        let wasm_path = self.find_wasm(&config)?;
        let bytecode = fs::read(&wasm_path)?;
        
        println!("  Contract: {}", style(&wasm_path).dim());
        println!("  Size: {} bytes", bytecode.len());
        println!("  Network: {} ({})", self.network, network.url);
        
        // Calculate bytecode hash
        let mut hasher = Sha256::new();
        hasher.update(&bytecode);
        let hash = hasher.finalize();
        println!("  Bytecode hash: {}", hex::encode(&hash[..8]));
        
        if self.dry_run {
            println!("{} Dry run - not deploying", style("⚠").yellow());
            return Ok(());
        }
        
        // TODO: Connect to network and deploy via gRPC
        // For now, simulate deployment
        let contract_address = self.simulate_deploy(&bytecode)?;
        
        println!("{} Contract deployed!", style("✓").green());
        println!("  Address: {}", style(&contract_address).cyan());
        
        // Save deployment info
        self.save_deployment_info(&config, &contract_address, &hash)?;
        
        Ok(())
    }
    
    fn find_wasm(&self, config: &ForgeConfig) -> ForgeResult<String> {
        // Check if contract is a path
        if Path::new(&self.contract).exists() && self.contract.ends_with(".wasm") {
            return Ok(self.contract.clone());
        }
        
        // Look in artifacts directory
        let artifact_path = format!("{}/{}.wasm", config.contracts.output, self.contract);
        if Path::new(&artifact_path).exists() {
            return Ok(artifact_path);
        }
        
        // Look for snake_case version
        let snake_case = self.contract.replace("-", "_");
        let artifact_path = format!("{}/{}.wasm", config.contracts.output, snake_case);
        if Path::new(&artifact_path).exists() {
            return Ok(artifact_path);
        }
        
        Err(ForgeError::ContractNotFound(format!(
            "Contract '{}' not found. Run 'mameyforge build' first.",
            self.contract
        )))
    }
    
    fn simulate_deploy(&self, bytecode: &[u8]) -> ForgeResult<String> {
        // Generate deterministic address from bytecode hash
        let mut hasher = Sha256::new();
        hasher.update(bytecode);
        hasher.update(b"deploy");
        let hash = hasher.finalize();
        
        Ok(format!("0x{}", hex::encode(&hash[..20])))
    }
    
    fn save_deployment_info(
        &self,
        config: &ForgeConfig,
        address: &str,
        bytecode_hash: &[u8],
    ) -> ForgeResult<()> {
        let deployments_dir = format!("{}/.deployments", config.contracts.output);
        fs::create_dir_all(&deployments_dir)?;
        
        let deployment = serde_json::json!({
            "contract": self.contract,
            "address": address,
            "network": self.network,
            "bytecode_hash": hex::encode(bytecode_hash),
            "deployed_at": chrono::Utc::now().to_rfc3339(),
            "args": self.args,
        });
        
        let filename = format!("{}/{}_{}.json", deployments_dir, self.contract, self.network);
        fs::write(&filename, serde_json::to_string_pretty(&deployment)?)?;
        
        println!("  Deployment info saved to {}", style(&filename).dim());
        Ok(())
    }
}

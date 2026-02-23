//! Clean command - remove build artifacts

use clap::Args;
use console::style;
use std::fs;
use std::path::Path;
use crate::config::ForgeConfig;
use crate::error::ForgeResult;

/// Clean build artifacts
#[derive(Args, Debug)]
pub struct CleanCommand {
    /// Also clean devnet data
    #[arg(long)]
    pub all: bool,
}

impl CleanCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        
        println!("{} Cleaning build artifacts...", style("→").cyan());
        
        // Clean artifacts directory
        let artifacts_path = Path::new(&config.contracts.output);
        if artifacts_path.exists() {
            fs::remove_dir_all(artifacts_path)?;
            println!("  Removed {}", style(artifacts_path.display()).dim());
        }
        
        // Clean cargo target
        let target_path = Path::new("target");
        if target_path.exists() {
            println!("  Cleaning cargo target...");
            std::process::Command::new("cargo")
                .arg("clean")
                .status()?;
        }
        
        // Clean devnet data if requested
        if self.all {
            let devnet_path = Path::new(&config.devnet.data_dir);
            if devnet_path.exists() {
                fs::remove_dir_all(devnet_path)?;
                println!("  Removed {}", style(devnet_path.display()).dim());
            }
            
            // Clean .mameyforge directory
            let forge_dir = Path::new(".mameyforge");
            if forge_dir.exists() {
                fs::remove_dir_all(forge_dir)?;
                println!("  Removed {}", style(forge_dir.display()).dim());
            }
        }
        
        println!("{} Clean complete!", style("✓").green());
        Ok(())
    }
}

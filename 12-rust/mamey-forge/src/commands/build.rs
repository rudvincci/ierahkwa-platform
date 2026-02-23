//! Build command - compile contracts to WASM

use clap::Args;
use console::style;
use std::process::Command;
use std::path::Path;
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Build contracts to WASM
#[derive(Args, Debug)]
pub struct BuildCommand {
    /// Build in release mode
    #[arg(short, long)]
    pub release: bool,
    
    /// Specific contract to build
    #[arg(short, long)]
    pub contract: Option<String>,
    
    /// Skip optimization
    #[arg(long)]
    pub no_optimize: bool,
}

impl BuildCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        
        println!("{} Building contracts...", style("→").cyan());
        
        // Check for Rust and WASM target
        self.check_prerequisites()?;
        
        // Build contracts
        let contracts_path = Path::new(&config.contracts.path);
        if !contracts_path.exists() {
            return Err(ForgeError::ProjectNotFound(format!(
                "Contracts directory not found: {}",
                config.contracts.path
            )));
        }
        
        // Run cargo build
        let mut cmd = Command::new("cargo");
        cmd.arg("build")
            .arg("--target")
            .arg("wasm32-unknown-unknown");
        
        if self.release || !self.no_optimize {
            cmd.arg("--release");
        }
        
        if let Some(ref contract) = self.contract {
            cmd.arg("-p").arg(contract);
        }
        
        println!("{} Running: {:?}", style("  ").dim(), cmd);
        
        let status = cmd.status()
            .map_err(|e| ForgeError::Build(format!("Failed to run cargo: {}", e)))?;
        
        if !status.success() {
            return Err(ForgeError::Build("Cargo build failed".to_string()));
        }
        
        // Copy artifacts
        self.copy_artifacts(&config)?;
        
        // Optimize WASM (if not skipped)
        if self.release && !self.no_optimize {
            self.optimize_wasm(&config)?;
        }
        
        println!("{} Build complete!", style("✓").green());
        Ok(())
    }
    
    fn check_prerequisites(&self) -> ForgeResult<()> {
        // Check rustc
        let rustc = Command::new("rustc")
            .arg("--version")
            .output()
            .map_err(|_| ForgeError::Build("Rust is not installed".to_string()))?;
        
        if !rustc.status.success() {
            return Err(ForgeError::Build("Rust is not installed".to_string()));
        }
        
        // Check WASM target
        let targets = Command::new("rustup")
            .args(["target", "list", "--installed"])
            .output()
            .map_err(|_| ForgeError::Build("rustup not found".to_string()))?;
        
        let targets_str = String::from_utf8_lossy(&targets.stdout);
        if !targets_str.contains("wasm32-unknown-unknown") {
            println!("{} Installing wasm32-unknown-unknown target...", style("→").yellow());
            let install = Command::new("rustup")
                .args(["target", "add", "wasm32-unknown-unknown"])
                .status()
                .map_err(|e| ForgeError::Build(format!("Failed to install WASM target: {}", e)))?;
            
            if !install.success() {
                return Err(ForgeError::Build("Failed to install WASM target".to_string()));
            }
        }
        
        Ok(())
    }
    
    fn copy_artifacts(&self, config: &ForgeConfig) -> ForgeResult<()> {
        let profile = if self.release { "release" } else { "debug" };
        let source = format!("target/wasm32-unknown-unknown/{}", profile);
        let dest = &config.contracts.output;
        
        std::fs::create_dir_all(dest)?;
        
        // Find and copy WASM files
        if let Ok(entries) = std::fs::read_dir(&source) {
            for entry in entries.flatten() {
                let path = entry.path();
                if path.extension().map(|e| e == "wasm").unwrap_or(false) {
                    let dest_path = Path::new(dest).join(path.file_name().unwrap());
                    std::fs::copy(&path, &dest_path)?;
                    println!("  {} -> {}", 
                        style(path.display()).dim(),
                        style(dest_path.display()).green()
                    );
                }
            }
        }
        
        Ok(())
    }
    
    fn optimize_wasm(&self, config: &ForgeConfig) -> ForgeResult<()> {
        // Check if wasm-opt is available
        if which::which("wasm-opt").is_err() {
            println!("{} wasm-opt not found, skipping optimization", style("⚠").yellow());
            return Ok(());
        }
        
        println!("{} Optimizing WASM...", style("→").cyan());
        
        let dest = &config.contracts.output;
        if let Ok(entries) = std::fs::read_dir(dest) {
            for entry in entries.flatten() {
                let path = entry.path();
                if path.extension().map(|e| e == "wasm").unwrap_or(false) {
                    let opt_level = format!("-O{}", config.build.optimization_level);
                    let status = Command::new("wasm-opt")
                        .arg(&opt_level)
                        .arg(&path)
                        .arg("-o")
                        .arg(&path)
                        .status();
                    
                    if let Ok(s) = status {
                        if s.success() {
                            println!("  {} optimized", style(path.display()).dim());
                        }
                    }
                }
            }
        }
        
        Ok(())
    }
}

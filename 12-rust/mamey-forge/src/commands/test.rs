//! Test command - run contract tests

use clap::Args;
use console::style;
use std::process::Command;
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Run tests
#[derive(Args, Debug)]
pub struct TestCommand {
    /// Test filter pattern
    #[arg(short, long)]
    pub filter: Option<String>,
    
    /// Run in verbose mode
    #[arg(short, long)]
    pub verbose: bool,
    
    /// Run only unit tests
    #[arg(long)]
    pub unit: bool,
    
    /// Run only integration tests
    #[arg(long)]
    pub integration: bool,
}

impl TestCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let _config = ForgeConfig::load(config_path)?;
        
        println!("{} Running tests...", style("→").cyan());
        
        let mut cmd = Command::new("cargo");
        cmd.arg("test");
        
        if let Some(ref filter) = self.filter {
            cmd.arg(filter);
        }
        
        if self.verbose {
            cmd.arg("--").arg("--nocapture");
        }
        
        if self.unit {
            cmd.arg("--lib");
        }
        
        if self.integration {
            cmd.arg("--test").arg("*");
        }
        
        let status = cmd.status()
            .map_err(|e| ForgeError::Test(format!("Failed to run tests: {}", e)))?;
        
        if status.success() {
            println!("{} All tests passed!", style("✓").green());
        } else {
            return Err(ForgeError::Test("Some tests failed".to_string()));
        }
        
        Ok(())
    }
}

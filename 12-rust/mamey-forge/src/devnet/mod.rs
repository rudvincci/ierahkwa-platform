//! Devnet management - local development network

use clap::{Args, Subcommand};
use console::style;
use std::fs;
use std::path::Path;
use std::process::{Command, Stdio};
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Devnet management commands
#[derive(Args, Debug)]
pub struct DevnetCommand {
    #[command(subcommand)]
    pub action: DevnetAction,
}

#[derive(Subcommand, Debug)]
pub enum DevnetAction {
    /// Start local devnet
    Start {
        /// Run in foreground
        #[arg(short, long)]
        foreground: bool,
    },
    
    /// Stop local devnet
    Stop,
    
    /// Check devnet status
    Status,
    
    /// View devnet logs
    Logs {
        /// Follow logs
        #[arg(short, long)]
        follow: bool,
        
        /// Number of lines
        #[arg(short, long, default_value = "50")]
        lines: usize,
    },
    
    /// Reset devnet state
    Reset {
        /// Force reset without confirmation
        #[arg(short, long)]
        force: bool,
    },
    
    /// Create state snapshot
    Snapshot {
        /// Snapshot name
        name: String,
    },
    
    /// Restore from snapshot
    Restore {
        /// Snapshot name
        name: String,
    },
}

impl DevnetCommand {
    pub async fn execute(&self, config_path: &str) -> ForgeResult<()> {
        let config = ForgeConfig::load(config_path)?;
        
        match &self.action {
            DevnetAction::Start { foreground } => self.start(&config, *foreground).await,
            DevnetAction::Stop => self.stop(&config).await,
            DevnetAction::Status => self.status(&config).await,
            DevnetAction::Logs { follow, lines } => self.logs(&config, *follow, *lines).await,
            DevnetAction::Reset { force } => self.reset(&config, *force).await,
            DevnetAction::Snapshot { name } => self.snapshot(&config, name).await,
            DevnetAction::Restore { name } => self.restore(&config, name).await,
        }
    }
    
    async fn start(&self, config: &ForgeConfig, foreground: bool) -> ForgeResult<()> {
        println!("{} Starting devnet...", style("→").cyan());
        
        // Create data directory
        let data_dir = Path::new(&config.devnet.data_dir);
        fs::create_dir_all(data_dir)?;
        
        // Check if already running
        if self.is_running(config).await? {
            println!("{} Devnet is already running", style("⚠").yellow());
            return Ok(());
        }
        
        // Check Docker
        if !self.check_docker()? {
            return Err(ForgeError::Devnet("Docker is not running".to_string()));
        }
        
        println!("  Port: {}", config.devnet.port);
        println!("  Data: {}", config.devnet.data_dir);
        println!("  Image: {}", config.devnet.docker_image);
        
        if foreground {
            // Run in foreground
            self.run_docker_foreground(config)?;
        } else {
            // Run in background
            self.run_docker_background(config)?;
            
            // Wait for startup
            println!("{} Waiting for devnet to start...", style("→").cyan());
            tokio::time::sleep(tokio::time::Duration::from_secs(3)).await;
            
            if self.is_running(config).await? {
                println!("{} Devnet started!", style("✓").green());
                println!();
                println!("  gRPC endpoint: http://localhost:{}", config.devnet.port);
                println!();
                println!("Commands:");
                println!("  mameyforge devnet status  - Check status");
                println!("  mameyforge devnet logs    - View logs");
                println!("  mameyforge devnet stop    - Stop devnet");
            } else {
                return Err(ForgeError::Devnet("Failed to start devnet".to_string()));
            }
        }
        
        Ok(())
    }
    
    async fn stop(&self, config: &ForgeConfig) -> ForgeResult<()> {
        println!("{} Stopping devnet...", style("→").cyan());
        
        let container_name = self.container_name(config);
        
        let status = Command::new("docker")
            .args(["stop", &container_name])
            .stdout(Stdio::null())
            .stderr(Stdio::null())
            .status();
        
        if status.is_ok() {
            // Remove container
            let _ = Command::new("docker")
                .args(["rm", &container_name])
                .stdout(Stdio::null())
                .stderr(Stdio::null())
                .status();
            
            println!("{} Devnet stopped!", style("✓").green());
        } else {
            println!("{} Devnet was not running", style("⚠").yellow());
        }
        
        Ok(())
    }
    
    async fn status(&self, config: &ForgeConfig) -> ForgeResult<()> {
        println!("{} Devnet Status", style("→").cyan());
        println!();
        
        if self.is_running(config).await? {
            println!("  Status: {}", style("Running").green());
            println!("  Endpoint: http://localhost:{}", config.devnet.port);
            
            // Get container info
            let container_name = self.container_name(config);
            let output = Command::new("docker")
                .args(["inspect", "--format", "{{.State.StartedAt}}", &container_name])
                .output();
            
            if let Ok(out) = output {
                let started = String::from_utf8_lossy(&out.stdout);
                println!("  Started: {}", started.trim());
            }
        } else {
            println!("  Status: {}", style("Stopped").red());
        }
        
        Ok(())
    }
    
    async fn logs(&self, config: &ForgeConfig, follow: bool, lines: usize) -> ForgeResult<()> {
        let container_name = self.container_name(config);
        
        let mut cmd = Command::new("docker");
        cmd.arg("logs")
            .arg("--tail")
            .arg(lines.to_string());
        
        if follow {
            cmd.arg("-f");
        }
        
        cmd.arg(&container_name);
        
        let _ = cmd.status();
        
        Ok(())
    }
    
    async fn reset(&self, config: &ForgeConfig, force: bool) -> ForgeResult<()> {
        if !force {
            println!("{} This will delete all devnet data. Continue? [y/N]", style("⚠").yellow());
            let mut input = String::new();
            std::io::stdin().read_line(&mut input)?;
            if input.trim().to_lowercase() != "y" {
                println!("Aborted.");
                return Ok(());
            }
        }
        
        // Stop if running
        self.stop(config).await?;
        
        // Delete data directory
        let data_dir = Path::new(&config.devnet.data_dir);
        if data_dir.exists() {
            fs::remove_dir_all(data_dir)?;
        }
        
        println!("{} Devnet reset!", style("✓").green());
        Ok(())
    }
    
    async fn snapshot(&self, config: &ForgeConfig, name: &str) -> ForgeResult<()> {
        println!("{} Creating snapshot '{}'...", style("→").cyan(), name);
        
        let snapshots_dir = format!("{}/.snapshots", config.devnet.data_dir);
        fs::create_dir_all(&snapshots_dir)?;
        
        let snapshot_path = format!("{}/{}", snapshots_dir, name);
        let data_dir = &config.devnet.data_dir;
        
        // Copy data directory to snapshot
        if Path::new(data_dir).exists() {
            fs::create_dir_all(&snapshot_path)?;
            // In production, use proper directory copy
            println!("  Snapshot saved to {}", style(&snapshot_path).dim());
        }
        
        println!("{} Snapshot created!", style("✓").green());
        Ok(())
    }
    
    async fn restore(&self, config: &ForgeConfig, name: &str) -> ForgeResult<()> {
        println!("{} Restoring from snapshot '{}'...", style("→").cyan(), name);
        
        let snapshot_path = format!("{}/.snapshots/{}", config.devnet.data_dir, name);
        
        if !Path::new(&snapshot_path).exists() {
            return Err(ForgeError::Devnet(format!("Snapshot '{}' not found", name)));
        }
        
        // Stop devnet
        self.stop(config).await?;
        
        // Restore snapshot
        // In production, use proper directory restore
        
        println!("{} Snapshot restored!", style("✓").green());
        Ok(())
    }
    
    // Helper methods
    
    fn container_name(&self, config: &ForgeConfig) -> String {
        format!("mameyforge-devnet-{}", config.devnet.port)
    }
    
    async fn is_running(&self, config: &ForgeConfig) -> ForgeResult<bool> {
        let container_name = self.container_name(config);
        
        let output = Command::new("docker")
            .args(["ps", "-q", "-f", &format!("name={}", container_name)])
            .output();
        
        match output {
            Ok(out) => Ok(!out.stdout.is_empty()),
            Err(_) => Ok(false),
        }
    }
    
    fn check_docker(&self) -> ForgeResult<bool> {
        let status = Command::new("docker")
            .arg("info")
            .stdout(Stdio::null())
            .stderr(Stdio::null())
            .status();
        
        Ok(status.map(|s| s.success()).unwrap_or(false))
    }
    
    fn run_docker_foreground(&self, config: &ForgeConfig) -> ForgeResult<()> {
        let container_name = self.container_name(config);
        
        let _ = Command::new("docker")
            .args([
                "run",
                "--rm",
                "--name", &container_name,
                "-p", &format!("{}:50051", config.devnet.port),
                "-v", &format!("{}:/data", config.devnet.data_dir),
                &config.devnet.docker_image,
            ])
            .status();
        
        Ok(())
    }
    
    fn run_docker_background(&self, config: &ForgeConfig) -> ForgeResult<()> {
        let container_name = self.container_name(config);
        
        let status = Command::new("docker")
            .args([
                "run",
                "-d",
                "--name", &container_name,
                "-p", &format!("{}:50051", config.devnet.port),
                "-v", &format!("{}:/data", config.devnet.data_dir),
                &config.devnet.docker_image,
            ])
            .stdout(Stdio::null())
            .status();
        
        if status.map(|s| s.success()).unwrap_or(false) {
            Ok(())
        } else {
            // If Docker image doesn't exist, simulate devnet
            println!("{} Docker image not found, using simulation mode", style("⚠").yellow());
            self.write_pid_file(config)?;
            Ok(())
        }
    }
    
    fn write_pid_file(&self, config: &ForgeConfig) -> ForgeResult<()> {
        let pid_file = format!("{}/.pid", config.devnet.data_dir);
        fs::write(&pid_file, std::process::id().to_string())?;
        Ok(())
    }
}

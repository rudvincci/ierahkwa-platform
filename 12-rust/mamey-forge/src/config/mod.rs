//! Configuration management for MameyForge

use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::path::Path;
use crate::error::{ForgeError, ForgeResult};

/// Main configuration structure
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ForgeConfig {
    /// Project configuration
    pub project: ProjectConfig,
    
    /// Contract configuration
    #[serde(default)]
    pub contracts: ContractsConfig,
    
    /// Build configuration
    #[serde(default)]
    pub build: BuildConfig,
    
    /// Network configurations
    #[serde(default)]
    pub networks: HashMap<String, NetworkConfig>,
    
    /// Devnet configuration
    #[serde(default)]
    pub devnet: DevnetConfig,
}

impl ForgeConfig {
    /// Load configuration from file
    pub fn load<P: AsRef<Path>>(path: P) -> ForgeResult<Self> {
        let path = path.as_ref();
        
        if !path.exists() {
            return Err(ForgeError::Config(format!(
                "Configuration file not found: {}",
                path.display()
            )));
        }
        
        let content = std::fs::read_to_string(path)?;
        let config: ForgeConfig = toml::from_str(&content)?;
        Ok(config)
    }
    
    /// Save configuration to file
    pub fn save<P: AsRef<Path>>(&self, path: P) -> ForgeResult<()> {
        let content = toml::to_string_pretty(self)
            .map_err(|e| ForgeError::Serialization(e.to_string()))?;
        std::fs::write(path, content)?;
        Ok(())
    }
    
    /// Get network configuration
    pub fn network(&self, name: &str) -> ForgeResult<&NetworkConfig> {
        self.networks.get(name)
            .ok_or_else(|| ForgeError::Config(format!("Network not found: {}", name)))
    }
    
    /// Create default configuration
    pub fn default_with_name(name: &str) -> Self {
        Self {
            project: ProjectConfig {
                name: name.to_string(),
                version: "0.1.0".to_string(),
                authors: vec![],
            },
            contracts: ContractsConfig::default(),
            build: BuildConfig::default(),
            networks: default_networks(),
            devnet: DevnetConfig::default(),
        }
    }
}

/// Project configuration
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ProjectConfig {
    /// Project name
    pub name: String,
    
    /// Project version
    pub version: String,
    
    /// Authors
    #[serde(default)]
    pub authors: Vec<String>,
}

/// Contracts configuration
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ContractsConfig {
    /// Path to contracts directory
    #[serde(default = "default_contracts_path")]
    pub path: String,
    
    /// Output directory for artifacts
    #[serde(default = "default_output_path")]
    pub output: String,
}

impl Default for ContractsConfig {
    fn default() -> Self {
        Self {
            path: default_contracts_path(),
            output: default_output_path(),
        }
    }
}

fn default_contracts_path() -> String {
    "contracts".to_string()
}

fn default_output_path() -> String {
    "artifacts".to_string()
}

/// Build configuration
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct BuildConfig {
    /// Optimization level (s, z, or 3)
    #[serde(default = "default_optimization_level")]
    pub optimization_level: String,
    
    /// Strip symbols
    #[serde(default = "default_true")]
    pub strip: bool,
    
    /// Link-time optimization
    #[serde(default = "default_true")]
    pub lto: bool,
}

impl Default for BuildConfig {
    fn default() -> Self {
        Self {
            optimization_level: default_optimization_level(),
            strip: true,
            lto: true,
        }
    }
}

fn default_optimization_level() -> String {
    "z".to_string()
}

fn default_true() -> bool {
    true
}

/// Network configuration
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NetworkConfig {
    /// RPC URL
    pub url: String,
    
    /// Chain ID
    pub chain_id: String,
    
    /// Gas price (optional)
    #[serde(default)]
    pub gas_price: Option<u64>,
    
    /// Gas limit (optional)
    #[serde(default)]
    pub gas_limit: Option<u64>,
}

/// Devnet configuration
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct DevnetConfig {
    /// gRPC port
    #[serde(default = "default_port")]
    pub port: u16,
    
    /// Data directory
    #[serde(default = "default_data_dir")]
    pub data_dir: String,
    
    /// Auto-mine blocks
    #[serde(default = "default_true")]
    pub auto_mine: bool,
    
    /// Block time in seconds
    #[serde(default = "default_block_time")]
    pub block_time: u64,
    
    /// Docker image
    #[serde(default = "default_docker_image")]
    pub docker_image: String,
}

impl Default for DevnetConfig {
    fn default() -> Self {
        Self {
            port: default_port(),
            data_dir: default_data_dir(),
            auto_mine: true,
            block_time: default_block_time(),
            docker_image: default_docker_image(),
        }
    }
}

fn default_port() -> u16 {
    50051
}

fn default_data_dir() -> String {
    ".mameyforge/devnet".to_string()
}

fn default_block_time() -> u64 {
    1
}

fn default_docker_image() -> String {
    "mamey/general-node:latest".to_string()
}

/// Create default network configurations
fn default_networks() -> HashMap<String, NetworkConfig> {
    let mut networks = HashMap::new();
    
    networks.insert("devnet".to_string(), NetworkConfig {
        url: "http://localhost:50051".to_string(),
        chain_id: "mamey-general-devnet".to_string(),
        gas_price: None,
        gas_limit: Some(10_000_000),
    });
    
    networks.insert("testnet".to_string(), NetworkConfig {
        url: "https://testnet.mamey.io:50051".to_string(),
        chain_id: "mamey-general-testnet".to_string(),
        gas_price: None,
        gas_limit: Some(10_000_000),
    });
    
    networks.insert("mainnet".to_string(), NetworkConfig {
        url: "https://mainnet.mamey.io:50051".to_string(),
        chain_id: "mamey-general".to_string(),
        gas_price: None,
        gas_limit: Some(10_000_000),
    });
    
    networks
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_default_config() {
        let config = ForgeConfig::default_with_name("test-project");
        assert_eq!(config.project.name, "test-project");
        assert!(config.networks.contains_key("devnet"));
    }
}

//! Node configuration

use serde::{Deserialize, Serialize};
use std::path::PathBuf;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NodeConfig {
    pub chain_id: u64,
    pub host: String,
    pub port: u16,
    pub data_dir: PathBuf,
    pub log_level: String,
    pub network: NetworkConfig,
    pub consensus: ConsensusConfig,
    pub rpc: RpcConfig,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct NetworkConfig {
    pub listen_addr: String,
    pub bootstrap_nodes: Vec<String>,
    pub max_peers: usize,
    pub enable_mdns: bool,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ConsensusConfig {
    pub block_time_ms: u64,
    pub validators: Vec<String>,
    pub min_validators: usize,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct RpcConfig {
    pub enable_http: bool,
    pub enable_ws: bool,
    pub cors_origins: Vec<String>,
    pub max_connections: usize,
}

impl Default for NodeConfig {
    fn default() -> Self {
        Self {
            chain_id: 777777,
            host: "0.0.0.0".to_string(),
            port: 8545,
            data_dir: PathBuf::from("./data"),
            log_level: "info".to_string(),
            network: NetworkConfig {
                listen_addr: "/ip4/0.0.0.0/tcp/30303".to_string(),
                bootstrap_nodes: vec![],
                max_peers: 50,
                enable_mdns: true,
            },
            consensus: ConsensusConfig {
                block_time_ms: 3000,
                validators: vec![],
                min_validators: 1,
            },
            rpc: RpcConfig {
                enable_http: true,
                enable_ws: true,
                cors_origins: vec!["*".to_string()],
                max_connections: 100,
            },
        }
    }
}

impl NodeConfig {
    pub fn load() -> anyhow::Result<Self> {
        // Try to load from file, otherwise use defaults
        let config_path = std::env::var("MAMEY_CONFIG")
            .map(PathBuf::from)
            .unwrap_or_else(|_| PathBuf::from("mamey.toml"));
        
        if config_path.exists() {
            let content = std::fs::read_to_string(&config_path)?;
            let config: NodeConfig = toml::from_str(&content)?;
            Ok(config)
        } else {
            Ok(Self::default())
        }
    }
    
    pub fn save(&self, path: &PathBuf) -> anyhow::Result<()> {
        let content = toml::to_string_pretty(self)?;
        std::fs::write(path, content)?;
        Ok(())
    }
}

//! Initialize command - create a new contract project
//!
//! Supports file-based templates from the `templates/` directory with
//! `{{placeholder}}` substitution, falling back to built-in inline templates
//! when file-based ones are not available.

use clap::Args;
use console::style;
use std::fs;
use std::path::{Path, PathBuf};
use walkdir::WalkDir;
use crate::config::ForgeConfig;
use crate::error::{ForgeError, ForgeResult};

/// Initialize a new contract project
#[derive(Args, Debug)]
pub struct InitCommand {
    /// Project name
    pub name: String,
    
    /// Template to use (basic, token, nft, governance, multi-token)
    #[arg(short, long, default_value = "basic")]
    pub template: String,
    
    /// Force overwrite if directory exists
    #[arg(short, long)]
    pub force: bool,
}

impl InitCommand {
    pub async fn execute(&self) -> ForgeResult<()> {
        println!("{} Initializing project '{}'...", style("→").cyan(), self.name);
        
        let project_dir = Path::new(&self.name);
        
        // Check if directory exists
        if project_dir.exists() && !self.force {
            return Err(ForgeError::Config(format!(
                "Directory '{}' already exists. Use --force to overwrite.",
                self.name
            )));
        }
        
        // Create directory structure
        self.create_directories(project_dir)?;
        
        // Try file-based template first, fall back to inline
        let used_file_template = self.try_file_based_template(project_dir)?;

        if !used_file_template {
            // Fall back to inline templates
            self.create_config(project_dir)?;
            self.create_template(project_dir)?;
            self.create_cargo_toml(project_dir)?;
        }
        
        // Always create .gitignore and common scaffolding files
        self.create_gitignore(project_dir)?;

        // Create tests and scripts if they weren't provided by file template
        if !project_dir.join("tests/integration.rs").exists() {
            fs::write(project_dir.join("tests/integration.rs"), INTEGRATION_TEST_TEMPLATE)?;
        }
        if !project_dir.join("scripts/deploy.rs").exists() {
            fs::write(project_dir.join("scripts/deploy.rs"), DEPLOY_SCRIPT_TEMPLATE)?;
        }
        
        println!("{} Project initialized successfully!", style("✓").green());
        if used_file_template {
            println!("  (used file-based template '{}')", self.template);
        }
        println!();
        println!("Next steps:");
        println!("  cd {}", self.name);
        println!("  mameyforge build");
        println!("  mameyforge test");
        
        Ok(())
    }

    // ── File-based template support ─────────────────────────────────────

    /// Attempt to scaffold from `templates/<name>/`. Returns `true` if the
    /// template directory was found and processed.
    fn try_file_based_template(&self, project_dir: &Path) -> ForgeResult<bool> {
        let template_dir = self.find_template_dir();
        let template_dir = match template_dir {
            Some(d) if d.is_dir() => d,
            _ => return Ok(false),
        };

        println!(
            "  {} Using file-based template from {}",
            style("•").dim(),
            template_dir.display()
        );

        let author = Self::detect_author();
        let replacements = [
            ("{{project_name}}", self.name.as_str()),
            ("{{version}}", "0.1.0"),
            ("{{author}}", &author),
        ];

        // Walk the template directory and copy / transform each file
        for entry in WalkDir::new(&template_dir).into_iter().filter_map(|e| e.ok()) {
            let src_path = entry.path();
            if !src_path.is_file() {
                continue;
            }

            // Compute relative path within the template dir
            let rel = src_path.strip_prefix(&template_dir)
                .map_err(|e| ForgeError::Config(format!("Path error: {}", e)))?;

            // Determine destination: strip `.template` extension if present
            let dest_rel = if rel.to_string_lossy().ends_with(".template") {
                let s = rel.to_string_lossy();
                PathBuf::from(s.trim_end_matches(".template"))
            } else {
                rel.to_path_buf()
            };

            // For template files: lib.rs.template → contracts/src/lib.rs
            // For Cargo.toml.template → Cargo.toml (project root)
            // For mameyforge.toml.template → mameyforge.toml (project root)
            // For README.md.template → README.md (project root)
            let dest_path = project_dir.join(&dest_rel);

            // Ensure parent directory exists
            if let Some(parent) = dest_path.parent() {
                fs::create_dir_all(parent)?;
            }

            // Read, replace placeholders, write
            let content = fs::read_to_string(src_path)?;
            let mut processed = content;
            for (placeholder, value) in &replacements {
                processed = processed.replace(placeholder, value);
            }

            fs::write(&dest_path, processed)?;
        }

        Ok(true)
    }

    /// Search for the template directory. Looks relative to the executable
    /// location and a few well-known paths.
    fn find_template_dir(&self) -> Option<PathBuf> {
        let candidates = [
            // Relative to current working directory (development)
            PathBuf::from(format!("templates/{}", self.template)),
            // Relative to the executable
            std::env::current_exe().ok()
                .and_then(|p| p.parent().map(|d| d.join(format!("../templates/{}", self.template))))
                .unwrap_or_default(),
            // Absolute from repo root (development fallback)
            std::env::current_exe().ok()
                .and_then(|p| p.parent().map(|d| d.join(format!("../../templates/{}", self.template))))
                .unwrap_or_default(),
        ];

        candidates.into_iter().find(|p| p.is_dir())
    }

    /// Try to detect the git user name for the `{{author}}` placeholder.
    fn detect_author() -> String {
        std::process::Command::new("git")
            .args(["config", "user.name"])
            .output()
            .ok()
            .and_then(|out| {
                if out.status.success() {
                    String::from_utf8(out.stdout).ok().map(|s| s.trim().to_string())
                } else {
                    None
                }
            })
            .unwrap_or_default()
    }

    // ── Inline fallback helpers ─────────────────────────────────────────
    
    fn create_directories(&self, root: &Path) -> ForgeResult<()> {
        let dirs = [
            "contracts/src",
            "tests",
            "scripts",
            "artifacts",
        ];
        
        for dir in &dirs {
            fs::create_dir_all(root.join(dir))?;
        }
        
        Ok(())
    }
    
    fn create_config(&self, root: &Path) -> ForgeResult<()> {
        let config = ForgeConfig::default_with_name(&self.name);
        config.save(root.join("mameyforge.toml"))?;
        Ok(())
    }
    
    fn create_template(&self, root: &Path) -> ForgeResult<()> {
        let contract_code = match self.template.as_str() {
            "basic" => BASIC_TEMPLATE,
            "token" => TOKEN_TEMPLATE,
            "nft" => NFT_TEMPLATE,
            "governance" => GOVERNANCE_TEMPLATE,
            "multi-token" => MULTI_TOKEN_TEMPLATE,
            _ => return Err(ForgeError::InvalidArgument(format!(
                "Unknown template: {}. Available: basic, token, nft, governance, multi-token",
                self.template
            ))),
        };
        
        fs::write(root.join("contracts/src/lib.rs"), contract_code)?;
        fs::write(root.join("tests/integration.rs"), INTEGRATION_TEST_TEMPLATE)?;
        fs::write(root.join("scripts/deploy.rs"), DEPLOY_SCRIPT_TEMPLATE)?;
        
        Ok(())
    }
    
    fn create_cargo_toml(&self, root: &Path) -> ForgeResult<()> {
        let cargo_toml = format!(r#"[package]
name = "{}"
version = "0.1.0"
edition = "2021"
authors = []

[lib]
crate-type = ["cdylib", "rlib"]

[dependencies]
mamey-contracts-base = {{ git = "https://github.com/Mamey-io/mamey-contracts.git", package = "mamey-contracts-base" }}
mamey-contracts-shared = {{ git = "https://github.com/Mamey-io/mamey-contracts.git", package = "mamey-contracts-shared" }}

[dev-dependencies]
tokio = {{ version = "1", features = ["full"] }}

[profile.release]
opt-level = "z"
lto = true
strip = true
panic = "abort"

[profile.release.package."*"]
opt-level = "z"
"#, self.name);
        
        fs::write(root.join("Cargo.toml"), cargo_toml)?;
        Ok(())
    }
    
    fn create_gitignore(&self, root: &Path) -> ForgeResult<()> {
        let gitignore = r#"# Build artifacts
target/
artifacts/
*.wasm

# IDE
.idea/
.vscode/
*.swp

# MameyForge
.mameyforge/

# Environment
.env
.env.local
"#;
        
        fs::write(root.join(".gitignore"), gitignore)?;
        Ok(())
    }
}

// Contract templates

const BASIC_TEMPLATE: &str = r#"//! Basic contract template

#![no_std]

use mamey_contracts_base::prelude::*;

/// Basic contract
pub struct MyContract {
    storage: Storage,
}

impl MyContract {
    /// Create a new contract instance
    pub fn new() -> Self {
        Self {
            storage: Storage::new(),
        }
    }
    
    /// Initialize the contract
    pub fn initialize(&mut self) -> Result<(), ContractError> {
        // Initialization logic here
        Ok(())
    }
    
    /// Example function
    pub fn get_value(&self, key: &str) -> Option<Vec<u8>> {
        self.storage.get(key)
    }
    
    /// Example setter
    pub fn set_value(&mut self, key: &str, value: Vec<u8>) -> Result<(), ContractError> {
        self.storage.set(key, value);
        Ok(())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_initialize() {
        let mut contract = MyContract::new();
        assert!(contract.initialize().is_ok());
    }
}
"#;

const TOKEN_TEMPLATE: &str = r#"//! Fungible token contract template

#![no_std]

use mamey_contracts_base::prelude::*;

/// Token contract implementing fungible token standard
pub struct Token {
    name: String,
    symbol: String,
    decimals: u8,
    total_supply: u128,
    balances: Storage,
    allowances: Storage,
}

impl Token {
    /// Create a new token
    pub fn new(name: String, symbol: String, decimals: u8) -> Self {
        Self {
            name,
            symbol,
            decimals,
            total_supply: 0,
            balances: Storage::new(),
            allowances: Storage::new(),
        }
    }
    
    /// Get token name
    pub fn name(&self) -> &str {
        &self.name
    }
    
    /// Get token symbol
    pub fn symbol(&self) -> &str {
        &self.symbol
    }
    
    /// Get decimals
    pub fn decimals(&self) -> u8 {
        self.decimals
    }
    
    /// Get total supply
    pub fn total_supply(&self) -> u128 {
        self.total_supply
    }
    
    /// Get balance of account
    pub fn balance_of(&self, account: &[u8; 32]) -> u128 {
        self.balances.get_u128(account).unwrap_or(0)
    }
    
    /// Transfer tokens
    pub fn transfer(&mut self, from: &[u8; 32], to: &[u8; 32], amount: u128) -> Result<(), ContractError> {
        let from_balance = self.balance_of(from);
        if from_balance < amount {
            return Err(ContractError::InsufficientBalance);
        }
        
        self.balances.set_u128(from, from_balance - amount);
        let to_balance = self.balance_of(to);
        self.balances.set_u128(to, to_balance + amount);
        
        Ok(())
    }
    
    /// Mint tokens (owner only)
    pub fn mint(&mut self, to: &[u8; 32], amount: u128) -> Result<(), ContractError> {
        let balance = self.balance_of(to);
        self.balances.set_u128(to, balance + amount);
        self.total_supply += amount;
        Ok(())
    }
    
    /// Burn tokens
    pub fn burn(&mut self, from: &[u8; 32], amount: u128) -> Result<(), ContractError> {
        let balance = self.balance_of(from);
        if balance < amount {
            return Err(ContractError::InsufficientBalance);
        }
        
        self.balances.set_u128(from, balance - amount);
        self.total_supply -= amount;
        Ok(())
    }
}
"#;

const NFT_TEMPLATE: &str = r#"//! NFT contract template

#![no_std]

use mamey_contracts_base::prelude::*;

/// NFT collection contract
pub struct NftCollection {
    name: String,
    symbol: String,
    owners: Storage,
    token_uris: Storage,
    next_token_id: u64,
}

impl NftCollection {
    /// Create a new NFT collection
    pub fn new(name: String, symbol: String) -> Self {
        Self {
            name,
            symbol,
            owners: Storage::new(),
            token_uris: Storage::new(),
            next_token_id: 1,
        }
    }
    
    /// Get collection name
    pub fn name(&self) -> &str {
        &self.name
    }
    
    /// Get collection symbol
    pub fn symbol(&self) -> &str {
        &self.symbol
    }
    
    /// Get owner of token
    pub fn owner_of(&self, token_id: u64) -> Option<[u8; 32]> {
        self.owners.get_bytes32(&token_id.to_le_bytes())
    }
    
    /// Get token URI
    pub fn token_uri(&self, token_id: u64) -> Option<String> {
        self.token_uris.get_string(&token_id.to_le_bytes())
    }
    
    /// Mint a new NFT
    pub fn mint(&mut self, to: &[u8; 32], uri: String) -> Result<u64, ContractError> {
        let token_id = self.next_token_id;
        self.next_token_id += 1;
        
        self.owners.set_bytes32(&token_id.to_le_bytes(), to);
        self.token_uris.set_string(&token_id.to_le_bytes(), &uri);
        
        Ok(token_id)
    }
    
    /// Transfer NFT
    pub fn transfer(&mut self, from: &[u8; 32], to: &[u8; 32], token_id: u64) -> Result<(), ContractError> {
        let owner = self.owner_of(token_id)
            .ok_or(ContractError::TokenNotFound)?;
            
        if &owner != from {
            return Err(ContractError::NotAuthorized);
        }
        
        self.owners.set_bytes32(&token_id.to_le_bytes(), to);
        Ok(())
    }
}
"#;

const GOVERNANCE_TEMPLATE: &str = r#"//! Governance contract template

#![no_std]

use mamey_contracts_base::prelude::*;

/// Proposal status
#[derive(Clone, Copy, PartialEq)]
pub enum ProposalStatus {
    Pending,
    Active,
    Passed,
    Rejected,
    Executed,
}

/// Governance contract for DAO
pub struct Governance {
    proposals: Storage,
    votes: Storage,
    voting_period: u64,
    quorum: u128,
    next_proposal_id: u64,
}

impl Governance {
    /// Create new governance contract
    pub fn new(voting_period: u64, quorum: u128) -> Self {
        Self {
            proposals: Storage::new(),
            votes: Storage::new(),
            voting_period,
            quorum,
            next_proposal_id: 1,
        }
    }
    
    /// Create a new proposal
    pub fn propose(&mut self, proposer: &[u8; 32], description: &str) -> Result<u64, ContractError> {
        let id = self.next_proposal_id;
        self.next_proposal_id += 1;
        
        // Store proposal
        // In real implementation, serialize proposal struct
        
        Ok(id)
    }
    
    /// Vote on a proposal
    pub fn vote(&mut self, voter: &[u8; 32], proposal_id: u64, support: bool, weight: u128) -> Result<(), ContractError> {
        // Record vote
        Ok(())
    }
    
    /// Execute a passed proposal
    pub fn execute(&mut self, proposal_id: u64) -> Result<(), ContractError> {
        // Execute proposal
        Ok(())
    }
}
"#;

const MULTI_TOKEN_TEMPLATE: &str = r#"//! Multi-token contract template (ERC-1155 style)

#![no_std]

use mamey_contracts_base::prelude::*;

/// Multi-token contract supporting multiple token types
pub struct MultiToken {
    balances: Storage,      // (account, token_id) -> balance
    token_uris: Storage,    // token_id -> uri
    operators: Storage,     // (owner, operator) -> approved
}

impl MultiToken {
    /// Create new multi-token contract
    pub fn new() -> Self {
        Self {
            balances: Storage::new(),
            token_uris: Storage::new(),
            operators: Storage::new(),
        }
    }
    
    /// Get balance of token for account
    pub fn balance_of(&self, account: &[u8; 32], token_id: u64) -> u128 {
        let key = Self::balance_key(account, token_id);
        self.balances.get_u128(&key).unwrap_or(0)
    }
    
    /// Get token URI
    pub fn uri(&self, token_id: u64) -> Option<String> {
        self.token_uris.get_string(&token_id.to_le_bytes())
    }
    
    /// Transfer tokens
    pub fn transfer(
        &mut self,
        from: &[u8; 32],
        to: &[u8; 32],
        token_id: u64,
        amount: u128,
    ) -> Result<(), ContractError> {
        let from_balance = self.balance_of(from, token_id);
        if from_balance < amount {
            return Err(ContractError::InsufficientBalance);
        }
        
        let from_key = Self::balance_key(from, token_id);
        let to_key = Self::balance_key(to, token_id);
        
        self.balances.set_u128(&from_key, from_balance - amount);
        let to_balance = self.balance_of(to, token_id);
        self.balances.set_u128(&to_key, to_balance + amount);
        
        Ok(())
    }
    
    /// Mint tokens
    pub fn mint(&mut self, to: &[u8; 32], token_id: u64, amount: u128) -> Result<(), ContractError> {
        let key = Self::balance_key(to, token_id);
        let balance = self.balance_of(to, token_id);
        self.balances.set_u128(&key, balance + amount);
        Ok(())
    }
    
    fn balance_key(account: &[u8; 32], token_id: u64) -> Vec<u8> {
        let mut key = account.to_vec();
        key.extend_from_slice(&token_id.to_le_bytes());
        key
    }
}
"#;

const INTEGRATION_TEST_TEMPLATE: &str = r#"//! Integration tests

use std::path::Path;

#[tokio::test]
async fn test_contract_deployment() {
    // TODO: Add deployment test
}

#[tokio::test]
async fn test_contract_interaction() {
    // TODO: Add interaction test
}
"#;

const DEPLOY_SCRIPT_TEMPLATE: &str = r#"//! Deployment script

use std::env;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let network = env::var("NETWORK").unwrap_or_else(|_| "devnet".to_string());
    
    println!("Deploying to {}...", network);
    
    // TODO: Add deployment logic
    
    println!("Deployment complete!");
    Ok(())
}
"#;

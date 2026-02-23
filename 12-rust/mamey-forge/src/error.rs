//! Error types for MameyForge

use thiserror::Error;

/// MameyForge error types
#[derive(Error, Debug)]
pub enum ForgeError {
    /// Configuration error
    #[error("Configuration error: {0}")]
    Config(String),

    /// Build error
    #[error("Build error: {0}")]
    Build(String),

    /// Test error
    #[error("Test error: {0}")]
    Test(String),

    /// Deployment error
    #[error("Deployment error: {0}")]
    Deploy(String),

    /// Network error
    #[error("Network error: {0}")]
    Network(String),

    /// Contract error
    #[error("Contract error: {0}")]
    Contract(String),

    /// Devnet error
    #[error("Devnet error: {0}")]
    Devnet(String),

    /// IO error
    #[error("IO error: {0}")]
    Io(#[from] std::io::Error),

    /// Serialization error
    #[error("Serialization error: {0}")]
    Serialization(String),

    /// Project not found
    #[error("Project not found: {0}")]
    ProjectNotFound(String),

    /// Contract not found
    #[error("Contract not found: {0}")]
    ContractNotFound(String),

    /// Invalid argument
    #[error("Invalid argument: {0}")]
    InvalidArgument(String),

    /// Process error
    #[error("Process error: {0}")]
    Process(String),

    /// gRPC error
    #[error("gRPC error: {0}")]
    Grpc(String),
}

/// Result type for MameyForge
pub type ForgeResult<T> = Result<T, ForgeError>;

impl From<toml::de::Error> for ForgeError {
    fn from(err: toml::de::Error) -> Self {
        ForgeError::Config(err.to_string())
    }
}

impl From<serde_json::Error> for ForgeError {
    fn from(err: serde_json::Error) -> Self {
        ForgeError::Serialization(err.to_string())
    }
}

impl From<tonic::transport::Error> for ForgeError {
    fn from(err: tonic::transport::Error) -> Self {
        ForgeError::Grpc(err.to_string())
    }
}

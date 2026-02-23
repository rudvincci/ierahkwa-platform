pub mod error;
pub mod metadata;
pub mod services;

// Generated gRPC client types (from MameyNode/proto via build.rs).
// We only `include_proto!` for the packages required by the minimal staging smoke surface.
pub mod proto {
    pub mod mamey {
        pub mod node {
            tonic::include_proto!("mamey.node");
        }

        pub mod rpc {
            tonic::include_proto!("mamey.rpc");
        }

        pub mod banking {
            tonic::include_proto!("mamey.banking");
        }
    }
}

use error::SdkError;
use metadata::ClientMetadata;
use services::{BankingApi, NodeApi, RpcApi};
use std::time::Duration;
use tonic::transport::{Channel, Endpoint};

/// Main MameyNode SDK client
#[derive(Clone)]
pub struct MameyNodeClient {
    channel: Channel,
    metadata: ClientMetadata,
}

impl MameyNodeClient {
    /// Create a new MameyNode client
    ///
    /// # Arguments
    /// * `endpoint` - gRPC endpoint URL (e.g., "http://localhost:50051")
    /// * `metadata` - Client metadata (correlation ID, bearer token, etc.)
    ///
    /// # Example
    /// ```no_run
    /// use mamey_node_sdk::{MameyNodeClient, ClientMetadata};
    ///
    /// let metadata = ClientMetadata {
    ///     correlation_id: Some("test-correlation-id".to_string()),
    ///     bearer_token: None,
    /// };
    ///
    /// let client = MameyNodeClient::connect("http://localhost:50051", metadata)
    ///     .await
    ///     .unwrap();
    /// ```
    pub async fn connect(
        endpoint: impl Into<String>,
        metadata: ClientMetadata,
    ) -> Result<Self, SdkError> {
        let endpoint = Endpoint::from_shared(endpoint.into().to_string())?
            .timeout(Duration::from_secs(30))
            .connect_timeout(Duration::from_secs(10));

        let channel = endpoint.connect().await?;

        Ok(Self { channel, metadata })
    }

    /// Create a new MameyNode client with custom timeout
    pub async fn connect_with_timeout(
        endpoint: impl Into<String>,
        metadata: ClientMetadata,
        timeout: Duration,
        connect_timeout: Duration,
    ) -> Result<Self, SdkError> {
        let endpoint = Endpoint::from_shared(endpoint.into().to_string())?
            .timeout(timeout)
            .connect_timeout(connect_timeout);

        let channel = endpoint.connect().await?;

        Ok(Self { channel, metadata })
    }

    /// Get the Node API client
    pub fn node(&self) -> NodeApi {
        NodeApi::new(self.channel.clone(), self.metadata.clone())
    }

    /// Get the RPC API client
    pub fn rpc(&self) -> RpcApi {
        RpcApi::new(self.channel.clone(), self.metadata.clone())
    }

    /// Get the Banking API client
    pub fn banking(&self) -> BankingApi {
        BankingApi::new(self.channel.clone(), self.metadata.clone())
    }

    /// Update client metadata
    pub fn with_metadata(mut self, metadata: ClientMetadata) -> Self {
        self.metadata = metadata;
        self
    }
}





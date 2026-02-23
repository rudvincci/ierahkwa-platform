use crate::error::SdkError;
use crate::metadata::ClientMetadata;
use crate::proto::mamey::rpc::{
    rpc_service_client::RpcServiceClient, VersionRequest, VersionResponse,
};
use tonic::transport::Channel;
use tonic::Request;

#[derive(Clone)]
pub struct RpcApi {
    client: RpcServiceClient<Channel>,
    metadata: ClientMetadata,
}

impl RpcApi {
    pub fn new(channel: Channel, metadata: ClientMetadata) -> Self {
        let client = RpcServiceClient::new(channel);
        Self { client, metadata }
    }

    /// Get RPC version information
    ///
    /// Returns RPC version, store version, protocol version, and build information.
    ///
    /// # Example
    /// ```no_run
    /// use mamey_node_sdk::{MameyNodeClient, ClientMetadata};
    ///
    /// # async fn example() -> Result<(), Box<dyn std::error::Error>> {
    /// let metadata = ClientMetadata::default();
    /// let client = MameyNodeClient::connect("http://localhost:50051", metadata).await?;
    /// let version = client.rpc().version().await?;
    /// println!("RPC version: {}", version.rpc_version);
    /// # Ok(())
    /// # }
    /// ```
    pub async fn version(&self) -> Result<VersionResponse, SdkError> {
        let mut request = Request::new(VersionRequest {});
        self.metadata.apply_to(request.metadata_mut());

        let response = self.client.version(request).await?;
        Ok(response.into_inner())
    }
}
}





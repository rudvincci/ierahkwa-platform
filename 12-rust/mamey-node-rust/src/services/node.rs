use crate::error::SdkError;
use crate::metadata::ClientMetadata;
use crate::proto::mamey::node::{
    node_service_client::NodeServiceClient, GetNodeInfoRequest, GetNodeInfoResponse,
};
use tonic::transport::Channel;
use tonic::Request;

#[derive(Clone)]
pub struct NodeApi {
    client: NodeServiceClient<Channel>,
    metadata: ClientMetadata,
}

impl NodeApi {
    pub fn new(channel: Channel, metadata: ClientMetadata) -> Self {
        let client = NodeServiceClient::new(channel);
        Self { client, metadata }
    }

    /// Get node information
    ///
    /// Returns node version, ID, block count, account count, peer count, and confirmation height.
    ///
    /// # Example
    /// ```no_run
    /// use mamey_node_sdk::{MameyNodeClient, ClientMetadata};
    ///
    /// # async fn example() -> Result<(), Box<dyn std::error::Error>> {
    /// let metadata = ClientMetadata::default();
    /// let client = MameyNodeClient::connect("http://localhost:50051", metadata).await?;
    /// let node_info = client.node().get_node_info().await?;
    /// println!("Node version: {}", node_info.version);
    /// # Ok(())
    /// # }
    /// ```
    pub async fn get_node_info(&self) -> Result<GetNodeInfoResponse, SdkError> {
        let mut request = Request::new(GetNodeInfoRequest {});
        self.metadata.apply_to(request.metadata_mut());

        let response = self.client.get_node_info(request).await?;
        Ok(response.into_inner())
    }
}
}





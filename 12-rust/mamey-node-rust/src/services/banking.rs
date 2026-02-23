use crate::error::SdkError;
use crate::metadata::ClientMetadata;
use crate::proto::mamey::banking::{
    banking_service_client::BankingServiceClient, CreatePaymentRequestRequest,
    CreatePaymentRequestResponse, GetPaymentRequestRequest, GetPaymentRequestResponse,
};
use tonic::transport::Channel;
use tonic::Request;

#[derive(Clone)]
pub struct BankingApi {
    client: BankingServiceClient<Channel>,
    metadata: ClientMetadata,
}

impl BankingApi {
    pub fn new(channel: Channel, metadata: ClientMetadata) -> Self {
        let client = BankingServiceClient::new(channel);
        Self { client, metadata }
    }

    /// Create a payment request
    ///
    /// Creates a new payment request that can be approved by the payer.
    ///
    /// # Arguments
    /// * `requester` - Account ID of the requester
    /// * `payer` - Account ID of the payer
    /// * `amount` - Amount as a string (e.g., "100.00")
    /// * `currency` - Currency code (e.g., "USD")
    /// * `description` - Optional description
    /// * `expires_at` - Optional expiration timestamp (0 for no expiration)
    ///
    /// # Example
    /// ```no_run
    /// use mamey_node_sdk::{MameyNodeClient, ClientMetadata};
    ///
    /// # async fn example() -> Result<(), Box<dyn std::error::Error>> {
    /// let metadata = ClientMetadata::default();
    /// let client = MameyNodeClient::connect("http://localhost:50051", metadata).await?;
    /// let response = client.banking().create_payment_request(
    ///     "ACC-REQ",
    ///     "ACC-PAY",
    ///     "100.00",
    ///     "USD",
    ///     Some("Payment for services"),
    ///     0,
    /// ).await?;
    /// println!("Payment request ID: {}", response.request_id);
    /// # Ok(())
    /// # }
    /// ```
    pub async fn create_payment_request(
        &self,
        requester: impl Into<String>,
        payer: impl Into<String>,
        amount: impl Into<String>,
        currency: impl Into<String>,
        description: Option<String>,
        expires_at: u64,
    ) -> Result<CreatePaymentRequestResponse, SdkError> {
        let mut request = Request::new(CreatePaymentRequestRequest {
            requester: requester.into(),
            payer: payer.into(),
            amount: amount.into(),
            currency: currency.into(),
            description: description.unwrap_or_default(),
            expires_at,
        });
        self.metadata.apply_to(request.metadata_mut());

        let response = self.client.create_payment_request(request).await?;
        Ok(response.into_inner())
    }

    /// Get a payment request by ID
    ///
    /// Retrieves details of a payment request.
    ///
    /// # Arguments
    /// * `request_id` - Payment request ID
    ///
    /// # Example
    /// ```no_run
    /// use mamey_node_sdk::{MameyNodeClient, ClientMetadata};
    ///
    /// # async fn example() -> Result<(), Box<dyn std::error::Error>> {
    /// let metadata = ClientMetadata::default();
    /// let client = MameyNodeClient::connect("http://localhost:50051", metadata).await?;
    /// let payment_request = client.banking().get_payment_request("REQ-123").await?;
    /// if payment_request.exists {
    ///     println!("Amount: {} {}", payment_request.amount, payment_request.currency);
    /// }
    /// # Ok(())
    /// # }
    /// ```
    pub async fn get_payment_request(
        &self,
        request_id: impl Into<String>,
    ) -> Result<GetPaymentRequestResponse, SdkError> {
        let mut request = Request::new(GetPaymentRequestRequest {
            request_id: request_id.into(),
        });
        self.metadata.apply_to(request.metadata_mut());

        let response = self.client.get_payment_request(request).await?;
        Ok(response.into_inner())
    }
}
}





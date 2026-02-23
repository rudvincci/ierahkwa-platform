use tonic::metadata::{MetadataMap, MetadataValue};
use uuid::Uuid;

#[derive(Clone, Debug, Default)]
pub struct ClientMetadata {
    pub correlation_id: Option<String>,
    pub bearer_token: Option<String>,
    // Future: credential_proofs: Option<CredentialProofs>
}

impl ClientMetadata {
    /// Create new metadata with auto-generated correlation ID
    pub fn new() -> Self {
        Self {
            correlation_id: Some(Uuid::new_v4().to_string()),
            bearer_token: None,
        }
    }

    /// Create new metadata with bearer token
    pub fn with_token(token: impl Into<String>) -> Self {
        Self {
            correlation_id: Some(Uuid::new_v4().to_string()),
            bearer_token: Some(token.into()),
        }
    }

    /// Create new metadata with correlation ID and bearer token
    pub fn with_correlation_and_token(
        correlation_id: impl Into<String>,
        token: impl Into<String>,
    ) -> Self {
        Self {
            correlation_id: Some(correlation_id.into()),
            bearer_token: Some(token.into()),
        }
    }

    /// Apply metadata to gRPC request headers
    pub fn apply_to(&self, headers: &mut MetadataMap) {
        if let Some(cid) = &self.correlation_id {
            if let Ok(val) = MetadataValue::try_from(cid.as_str()) {
                headers.insert("x-mamey-correlation-id", val);
            }
        }

        if let Some(token) = &self.bearer_token {
            let auth = format!("Bearer {}", token);
            if let Ok(val) = MetadataValue::try_from(auth.as_str()) {
                headers.insert("authorization", val);
            }
        }
    }
}





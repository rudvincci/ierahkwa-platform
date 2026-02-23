use thiserror::Error;

#[derive(Debug, Error)]
pub enum SdkError {
    #[error("invalid config: {0}")]
    InvalidConfig(String),

    #[error(transparent)]
    Transport(#[from] tonic::transport::Error),

    #[error(transparent)]
    GrpcStatus(#[from] tonic::Status),
}





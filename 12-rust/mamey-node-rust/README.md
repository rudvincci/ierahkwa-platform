# MameyNode.Rust

Rust client SDK for MameyNode blockchain.

**Location:** `MameyNode.Rust/` (root level, consistent with other SDKs: `MameyNode.Go/`, `MameyNode.JavaScript/`, etc.)

## Features

- **gRPC-first** client implementation
- **Credential/policy metadata** support (correlation IDs, bearer tokens)
- **Minimal staging gRPC surface** support:
  - NodeService (GetNodeInfo)
  - RpcService (Version)
  - BankingService (CreatePaymentRequest, GetPaymentRequest)
- **Type-safe** API with generated proto types
- **Async/await** support with Tokio

## Quick Start

Add to your `Cargo.toml`:

```toml
[dependencies]
mamey-node-sdk = { path = "../MameyNode.Rust" }
# Or from git:
# mamey-node-sdk = { git = "https://github.com/Mamey-io/MameyNode.Rust" }
tokio = { version = "1.35", features = ["full"] }
```

## Usage

```rust
use mamey_node_sdk::{ClientMetadata, MameyNodeClient};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Create client with metadata
    let metadata = ClientMetadata::new(); // Auto-generates correlation ID
    let client = MameyNodeClient::connect("http://localhost:50051", metadata).await?;

    // Get node information
    let node_info = client.node().get_node_info().await?;
    println!("Node version: {}", node_info.version);

    // Get RPC version
    let version = client.rpc().version().await?;
    println!("RPC version: {}", version.rpc_version);

    // Create payment request
    let payment_request = client
        .banking()
        .create_payment_request(
            "ACC-REQ",
            "ACC-PAY",
            "100.00",
            "USD",
            Some("Payment description"),
            0,
        )
        .await?;
    println!("Payment request ID: {}", payment_request.request_id);

    // Get payment request
    let request = client
        .banking()
        .get_payment_request(&payment_request.request_id)
        .await?;
    println!("Amount: {} {}", request.amount, request.currency);

    Ok(())
}
```

## Client Metadata

The SDK supports credential/policy metadata for authenticated and regulated operations:

```rust
use mamey_node_sdk::ClientMetadata;

// Auto-generate correlation ID
let metadata = ClientMetadata::new();

// With bearer token
let metadata = ClientMetadata::with_token("your-token-here");

// With custom correlation ID and token
let metadata = ClientMetadata::with_correlation_and_token(
    "custom-correlation-id",
    "your-token-here",
);
```

## Examples

Run the smoke test example:

```bash
MAMEYNODE_ENDPOINT=http://localhost:50051 cargo run --example smoke_test
```

## Proto / Codegen Strategy

**SDK-local codegen** using `tonic-build`, mirroring `crates/integration/mamey-rpc/build.rs`:

- Compiles **all** protos under `MameyNode/proto/` in a single invocation
- Uses `tonic::include_proto!(...)` for packages we need for staging smoke:
  - `mamey.node`
  - `mamey.rpc`
  - `mamey.banking`

## Architecture

- **Main Client** (`MameyNodeClient`): Entry point, manages gRPC channel
- **Service APIs**: Domain-specific clients (NodeApi, RpcApi, BankingApi)
- **Metadata**: Credential/policy metadata handling
- **Error Handling**: Comprehensive error types with `thiserror`

## Status

✅ **Implemented:**
- Client connection and channel management
- NodeService/GetNodeInfo
- RpcService/Version
- BankingService/CreatePaymentRequest
- BankingService/GetPaymentRequest
- Metadata support (correlation ID, bearer token)

🚧 **Planned:**
- Additional BankingService methods
- Streaming support
- Retry policies
- Connection pooling
- Credential proof support

## Documentation

See [SDK Specification](../../docs/sdk/SDK_SPECIFICATION.md) for complete SDK strategy and versioning policy.





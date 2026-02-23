//! Smoke test example for MameyNode SDK
//!
//! This example demonstrates the minimal staging gRPC surface:
//! 1. NodeService/GetNodeInfo
//! 2. RpcService/Version
//! 3. BankingService/CreatePaymentRequest
//! 4. BankingService/GetPaymentRequest

use mamey_node_sdk::{ClientMetadata, MameyNodeClient};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Initialize client with metadata
    let metadata = ClientMetadata::new();
    let endpoint = std::env::var("MAMEYNODE_ENDPOINT")
        .unwrap_or_else(|_| "http://localhost:50051".to_string());

    println!("Connecting to MameyNode at: {}", endpoint);
    let client = MameyNodeClient::connect(endpoint, metadata).await?;

    // 1. Node health check
    println!("\n=== Node Health Check ===");
    match client.node().get_node_info().await {
        Ok(info) => {
            println!("✓ Node version: {}", info.version);
            println!("✓ Node ID: {}", info.node_id);
            println!("✓ Block count: {}", info.block_count);
            println!("✓ Account count: {}", info.account_count);
            println!("✓ Peer count: {}", info.peer_count);
            println!("✓ Confirmation height: {}", info.confirmation_height);
        }
        Err(e) => {
            eprintln!("✗ Failed to get node info: {}", e);
            return Err(e.into());
        }
    }

    // 2. RPC version check
    println!("\n=== RPC Version Check ===");
    match client.rpc().version().await {
        Ok(version) => {
            println!("✓ RPC version: {}", version.rpc_version);
            println!("✓ Store version: {}", version.store_version);
            println!("✓ Protocol version: {}", version.protocol_version);
            println!("✓ Node vendor: {}", version.node_vendor);
            println!("✓ Network: {}", version.network);
            if version.success {
                println!("✓ RPC service is operational");
            } else {
                eprintln!("✗ RPC service returned error: {}", version.error);
            }
        }
        Err(e) => {
            eprintln!("✗ Failed to get RPC version: {}", e);
            return Err(e.into());
        }
    }

    // 3. Banking smoke test - Create payment request
    println!("\n=== Banking Smoke Test ===");
    let create_result = client
        .banking()
        .create_payment_request(
            "ACC-REQ",
            "ACC-PAY",
            "100.00",
            "USD",
            Some("staging-smoke-test"),
            0,
        )
        .await;

    let request_id = match create_result {
        Ok(response) => {
            if response.success {
                println!("✓ Payment request created");
                println!("  Request ID: {}", response.request_id);
                response.request_id
            } else {
                eprintln!("✗ Failed to create payment request: {}", response.error_message);
                return Err(format!("Create payment request failed: {}", response.error_message).into());
            }
        }
        Err(e) => {
            eprintln!("✗ Error creating payment request: {}", e);
            return Err(e.into());
        }
    };

    // 4. Get payment request
    println!("\n=== Get Payment Request ===");
    match client.banking().get_payment_request(&request_id).await {
        Ok(response) => {
            if response.exists {
                println!("✓ Payment request found");
                println!("  Request ID: {}", response.request_id);
                println!("  Requester: {}", response.requester);
                println!("  Payer: {}", response.payer);
                println!("  Amount: {} {}", response.amount, response.currency);
                println!("  Status: {:?}", response.status);
            } else {
                eprintln!("✗ Payment request not found");
            }
        }
        Err(e) => {
            eprintln!("✗ Error getting payment request: {}", e);
            return Err(e.into());
        }
    }

    println!("\n✓ All smoke tests passed!");
    Ok(())
}

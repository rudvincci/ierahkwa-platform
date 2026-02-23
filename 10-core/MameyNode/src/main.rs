//! MameyNode - High-performance Sovereign Blockchain Node
//! 
//! This is the core blockchain node for the Ierahkwa Sovereign Government platform.
//! Chain ID: 777777

use std::sync::Arc;
use tokio::sync::RwLock;
use axum::{Router, routing::{get, post}, Json, Extension};
use tower_http::cors::{CorsLayer, Any};
use tracing::{info, Level};
use tracing_subscriber::FmtSubscriber;

mod blockchain;
mod consensus;
mod crypto;
mod network;
mod storage;
mod api;
mod config;

use blockchain::{Blockchain, Block, Transaction};
use config::NodeConfig;

#[tokio::main]
async fn main() -> anyhow::Result<()> {
    // Initialize logging
    let subscriber = FmtSubscriber::builder()
        .with_max_level(Level::INFO)
        .with_target(false)
        .json()
        .init();

    info!("╔══════════════════════════════════════════════════════════════╗");
    info!("║           MAMEYNODE v1.0.0 - SOVEREIGN BLOCKCHAIN            ║");
    info!("║              Chain ID: 777777 | Ierahkwa Network             ║");
    info!("╚══════════════════════════════════════════════════════════════╝");

    // Load configuration
    let config = NodeConfig::load()?;
    
    // Initialize blockchain
    let blockchain = Arc::new(RwLock::new(Blockchain::new(config.chain_id)));
    
    // Build API router
    let app = Router::new()
        // Health & Info
        .route("/health", get(api::health))
        .route("/info", get(api::node_info))
        
        // JSON-RPC compatible
        .route("/rpc", post(api::json_rpc))
        
        // REST API v1
        .route("/api/v1/stats", get(api::stats))
        .route("/api/v1/blocks", get(api::get_blocks))
        .route("/api/v1/blocks/:hash", get(api::get_block))
        .route("/api/v1/blocks/latest", get(api::get_latest_block))
        .route("/api/v1/transactions", post(api::submit_transaction))
        .route("/api/v1/transactions/:hash", get(api::get_transaction))
        .route("/api/v1/accounts/:address", get(api::get_account))
        .route("/api/v1/accounts/:address/balance", get(api::get_balance))
        .route("/api/v1/accounts/:address/nonce", get(api::get_nonce))
        
        // Token operations
        .route("/api/v1/tokens", get(api::list_tokens))
        .route("/api/v1/tokens", post(api::create_token))
        .route("/api/v1/tokens/:symbol", get(api::get_token))
        .route("/api/v1/tokens/:symbol/holders", get(api::get_token_holders))
        
        // Bridge operations
        .route("/api/v1/bridge/chains", get(api::bridge_chains))
        .route("/api/v1/bridge/deposit", post(api::bridge_deposit))
        .route("/api/v1/bridge/withdraw", post(api::bridge_withdraw))
        .route("/api/v1/bridge/status/:id", get(api::bridge_status))
        
        // Governance
        .route("/api/v1/governance/proposals", get(api::list_proposals))
        .route("/api/v1/governance/proposals", post(api::create_proposal))
        .route("/api/v1/governance/vote", post(api::cast_vote))
        
        // Identity (FutureWampumID)
        .route("/api/v1/identity/register", post(api::register_identity))
        .route("/api/v1/identity/verify", post(api::verify_identity))
        .route("/api/v1/identity/:fwid", get(api::get_identity))
        
        // Middleware
        .layer(CorsLayer::new()
            .allow_origin(Any)
            .allow_methods(Any)
            .allow_headers(Any))
        .layer(Extension(blockchain));

    // Start server
    let addr = format!("{}:{}", config.host, config.port);
    info!("🚀 MameyNode listening on {}", addr);
    info!("📡 RPC endpoint: http://{}/rpc", addr);
    info!("📊 API endpoint: http://{}/api/v1", addr);
    
    let listener = tokio::net::TcpListener::bind(&addr).await?;
    axum::serve(listener, app).await?;

    Ok(())
}

//! API endpoints for MameyNode

use std::sync::Arc;
use tokio::sync::RwLock;
use axum::{
    extract::{Extension, Path, Json},
    http::StatusCode,
    response::IntoResponse,
};
use serde::{Deserialize, Serialize};
use serde_json::{json, Value};

use crate::blockchain::{Blockchain, Transaction, TransactionType};

type SharedState = Arc<RwLock<Blockchain>>;

// ============ Health & Info ============

pub async fn health() -> impl IntoResponse {
    Json(json!({
        "status": "healthy",
        "service": "MameyNode",
        "version": "1.0.0"
    }))
}

pub async fn node_info(Extension(state): Extension<SharedState>) -> impl IntoResponse {
    let blockchain = state.read().await;
    Json(json!({
        "name": "MameyNode",
        "version": "1.0.0",
        "chain_id": blockchain.chain_id,
        "network": "Ierahkwa Sovereign Network",
        "block_height": blockchain.get_block_count(),
        "pending_transactions": blockchain.pending_transactions.len(),
        "tokens": blockchain.tokens.len(),
        "accounts": blockchain.accounts.len()
    }))
}

// ============ JSON-RPC ============

#[derive(Debug, Deserialize)]
pub struct JsonRpcRequest {
    pub jsonrpc: String,
    pub method: String,
    pub params: Option<Value>,
    pub id: Value,
}

#[derive(Debug, Serialize)]
pub struct JsonRpcResponse {
    pub jsonrpc: String,
    pub result: Option<Value>,
    pub error: Option<JsonRpcError>,
    pub id: Value,
}

#[derive(Debug, Serialize)]
pub struct JsonRpcError {
    pub code: i32,
    pub message: String,
}

pub async fn json_rpc(
    Extension(state): Extension<SharedState>,
    Json(req): Json<JsonRpcRequest>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    let result = match req.method.as_str() {
        "eth_chainId" => Ok(json!(format!("0x{:x}", blockchain.chain_id))),
        "eth_blockNumber" => Ok(json!(format!("0x{:x}", blockchain.get_block_count().saturating_sub(1)))),
        "net_version" => Ok(json!(blockchain.chain_id.to_string())),
        "eth_gasPrice" => Ok(json!("0x0")), // Zero gas
        "eth_getBalance" => {
            if let Some(params) = &req.params {
                if let Some(address) = params.get(0).and_then(|v| v.as_str()) {
                    let balance = blockchain.get_balance(address, "WAMPUM");
                    Ok(json!(format!("0x{:x}", balance)))
                } else {
                    Err(JsonRpcError { code: -32602, message: "Invalid params".to_string() })
                }
            } else {
                Err(JsonRpcError { code: -32602, message: "Missing params".to_string() })
            }
        },
        "eth_getBlockByNumber" => {
            if let Some(params) = &req.params {
                if let Some(block_num) = params.get(0).and_then(|v| v.as_str()) {
                    let num = if block_num == "latest" {
                        blockchain.get_block_count().saturating_sub(1)
                    } else {
                        u64::from_str_radix(block_num.trim_start_matches("0x"), 16).unwrap_or(0)
                    };
                    if let Some(block) = blockchain.get_block(num) {
                        Ok(json!({
                            "number": format!("0x{:x}", block.number),
                            "hash": block.hash,
                            "parentHash": block.parent_hash,
                            "timestamp": format!("0x{:x}", block.timestamp.timestamp()),
                            "miner": block.miner,
                            "gasUsed": format!("0x{:x}", block.gas_used),
                            "gasLimit": format!("0x{:x}", block.gas_limit),
                            "transactions": block.transactions.iter().map(|t| t.hash.clone()).collect::<Vec<_>>()
                        }))
                    } else {
                        Ok(json!(null))
                    }
                } else {
                    Err(JsonRpcError { code: -32602, message: "Invalid params".to_string() })
                }
            } else {
                Err(JsonRpcError { code: -32602, message: "Missing params".to_string() })
            }
        },
        _ => Err(JsonRpcError { code: -32601, message: format!("Method not found: {}", req.method) })
    };
    
    let response = match result {
        Ok(value) => JsonRpcResponse {
            jsonrpc: "2.0".to_string(),
            result: Some(value),
            error: None,
            id: req.id,
        },
        Err(error) => JsonRpcResponse {
            jsonrpc: "2.0".to_string(),
            result: None,
            error: Some(error),
            id: req.id,
        },
    };
    
    Json(response)
}

// ============ REST API ============

pub async fn stats(Extension(state): Extension<SharedState>) -> impl IntoResponse {
    let blockchain = state.read().await;
    Json(json!({
        "chain_id": blockchain.chain_id,
        "block_height": blockchain.get_block_count(),
        "total_transactions": blockchain.get_transaction_count(),
        "pending_transactions": blockchain.pending_transactions.len(),
        "total_tokens": blockchain.tokens.len(),
        "total_accounts": blockchain.accounts.len(),
        "tps": 0, // Calculate based on recent blocks
        "gas_price": "0"
    }))
}

pub async fn get_blocks(Extension(state): Extension<SharedState>) -> impl IntoResponse {
    let blockchain = state.read().await;
    let blocks: Vec<_> = blockchain.blocks.iter().rev().take(10).collect();
    Json(json!({ "blocks": blocks }))
}

pub async fn get_block(
    Extension(state): Extension<SharedState>,
    Path(hash): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    if let Some(block) = blockchain.blocks.iter().find(|b| b.hash == hash) {
        Json(json!({ "block": block }))
    } else {
        Json(json!({ "error": "Block not found" }))
    }
}

pub async fn get_latest_block(Extension(state): Extension<SharedState>) -> impl IntoResponse {
    let blockchain = state.read().await;
    if let Some(block) = blockchain.get_latest_block() {
        Json(json!({ "block": block }))
    } else {
        Json(json!({ "error": "No blocks" }))
    }
}

#[derive(Debug, Deserialize)]
pub struct SubmitTxRequest {
    pub from: String,
    pub to: String,
    pub value: String,
    pub data: Option<String>,
}

pub async fn submit_transaction(
    Extension(state): Extension<SharedState>,
    Json(req): Json<SubmitTxRequest>,
) -> impl IntoResponse {
    let mut blockchain = state.write().await;
    
    let value: u128 = req.value.parse().unwrap_or(0);
    let tx = Transaction::new(req.from, req.to, value, TransactionType::Transfer);
    
    match blockchain.add_transaction(tx.clone()) {
        Ok(hash) => Json(json!({
            "success": true,
            "transaction_hash": hash
        })),
        Err(e) => Json(json!({
            "success": false,
            "error": e
        }))
    }
}

pub async fn get_transaction(
    Extension(state): Extension<SharedState>,
    Path(hash): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    // Search in blocks
    for block in &blockchain.blocks {
        if let Some(tx) = block.transactions.iter().find(|t| t.hash == hash) {
            return Json(json!({
                "transaction": tx,
                "block_number": block.number,
                "block_hash": block.hash
            }));
        }
    }
    
    // Search in pending
    if let Some(tx) = blockchain.pending_transactions.iter().find(|t| t.hash == hash) {
        return Json(json!({
            "transaction": tx,
            "status": "pending"
        }));
    }
    
    Json(json!({ "error": "Transaction not found" }))
}

pub async fn get_account(
    Extension(state): Extension<SharedState>,
    Path(address): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    if let Some(account) = blockchain.accounts.get(&address) {
        Json(json!({ "account": account }))
    } else {
        Json(json!({
            "account": {
                "address": address,
                "balances": {},
                "nonce": 0
            }
        }))
    }
}

pub async fn get_balance(
    Extension(state): Extension<SharedState>,
    Path(address): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    let wampum = blockchain.get_balance(&address, "WAMPUM");
    let sicbdc = blockchain.get_balance(&address, "SICBDC");
    let igt = blockchain.get_balance(&address, "IGT");
    
    Json(json!({
        "address": address,
        "balances": {
            "WAMPUM": wampum.to_string(),
            "SICBDC": sicbdc.to_string(),
            "IGT": igt.to_string()
        }
    }))
}

pub async fn get_nonce(
    Extension(state): Extension<SharedState>,
    Path(address): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    let nonce = blockchain.accounts
        .get(&address)
        .map(|a| a.nonce)
        .unwrap_or(0);
    
    Json(json!({
        "address": address,
        "nonce": nonce
    }))
}

// ============ Tokens ============

pub async fn list_tokens(Extension(state): Extension<SharedState>) -> impl IntoResponse {
    let blockchain = state.read().await;
    Json(json!({ "tokens": blockchain.tokens.values().collect::<Vec<_>>() }))
}

#[derive(Debug, Deserialize)]
pub struct CreateTokenRequest {
    pub symbol: String,
    pub name: String,
    pub decimals: u8,
    pub initial_supply: String,
    pub owner: String,
}

pub async fn create_token(
    Extension(state): Extension<SharedState>,
    Json(req): Json<CreateTokenRequest>,
) -> impl IntoResponse {
    let mut blockchain = state.write().await;
    
    if blockchain.tokens.contains_key(&req.symbol) {
        return Json(json!({ "error": "Token already exists" }));
    }
    
    let supply: u128 = req.initial_supply.parse().unwrap_or(0);
    let token = crate::blockchain::Token::new(
        req.symbol.clone(),
        req.name,
        req.decimals,
        supply,
        req.owner,
    );
    
    blockchain.tokens.insert(req.symbol.clone(), token);
    
    Json(json!({
        "success": true,
        "symbol": req.symbol
    }))
}

pub async fn get_token(
    Extension(state): Extension<SharedState>,
    Path(symbol): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    if let Some(token) = blockchain.tokens.get(&symbol) {
        Json(json!({ "token": token }))
    } else {
        Json(json!({ "error": "Token not found" }))
    }
}

pub async fn get_token_holders(
    Extension(state): Extension<SharedState>,
    Path(symbol): Path<String>,
) -> impl IntoResponse {
    let blockchain = state.read().await;
    
    let holders: Vec<_> = blockchain.accounts
        .iter()
        .filter_map(|(addr, acc)| {
            acc.balances.get(&symbol).map(|&bal| {
                json!({
                    "address": addr,
                    "balance": bal.to_string()
                })
            })
        })
        .collect();
    
    Json(json!({ "holders": holders }))
}

// ============ Bridge ============

pub async fn bridge_chains() -> impl IntoResponse {
    Json(json!({
        "chains": [
            { "id": 1, "name": "Ethereum", "status": "active" },
            { "id": 56, "name": "BSC", "status": "active" },
            { "id": 137, "name": "Polygon", "status": "active" },
            { "id": 43114, "name": "Avalanche", "status": "active" },
            { "id": 42161, "name": "Arbitrum", "status": "active" },
            { "id": 10, "name": "Optimism", "status": "active" },
            { "id": 777777, "name": "Ierahkwa", "status": "native" }
        ]
    }))
}

pub async fn bridge_deposit(Json(req): Json<Value>) -> impl IntoResponse {
    Json(json!({
        "success": true,
        "bridge_id": format!("BR{}", chrono::Utc::now().timestamp()),
        "status": "pending"
    }))
}

pub async fn bridge_withdraw(Json(req): Json<Value>) -> impl IntoResponse {
    Json(json!({
        "success": true,
        "bridge_id": format!("BR{}", chrono::Utc::now().timestamp()),
        "status": "pending"
    }))
}

pub async fn bridge_status(Path(id): Path<String>) -> impl IntoResponse {
    Json(json!({
        "bridge_id": id,
        "status": "completed"
    }))
}

// ============ Governance ============

pub async fn list_proposals() -> impl IntoResponse {
    Json(json!({ "proposals": [] }))
}

pub async fn create_proposal(Json(req): Json<Value>) -> impl IntoResponse {
    Json(json!({
        "success": true,
        "proposal_id": format!("PROP{}", chrono::Utc::now().timestamp())
    }))
}

pub async fn cast_vote(Json(req): Json<Value>) -> impl IntoResponse {
    Json(json!({
        "success": true,
        "vote_id": format!("VOTE{}", chrono::Utc::now().timestamp())
    }))
}

// ============ Identity ============

pub async fn register_identity(Json(req): Json<Value>) -> impl IntoResponse {
    Json(json!({
        "success": true,
        "fwid": format!("FWID{}", chrono::Utc::now().timestamp())
    }))
}

pub async fn verify_identity(Json(req): Json<Value>) -> impl IntoResponse {
    Json(json!({
        "success": true,
        "verified": true
    }))
}

pub async fn get_identity(Path(fwid): Path<String>) -> impl IntoResponse {
    Json(json!({
        "fwid": fwid,
        "status": "verified"
    }))
}

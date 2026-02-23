//! Escrow Contract Example
//!
//! A two-party escrow for MameyFutureNode. A buyer deposits funds, an arbiter
//! can release them to the seller or refund them to the buyer.
//!
//! Demonstrates:
//!   - Multi-role access control (buyer, seller, arbiter)
//!   - State machine pattern (Awaiting → Funded → Released | Refunded)
//!   - Balance tracking and safe math
//!   - Event emission
//!   - Validation utilities

use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage, math, validation};

// ─── Storage Keys ───────────────────────────────────────────────────────────

const KEY_BUYER: &str = "escrow:buyer";
const KEY_SELLER: &str = "escrow:seller";
const KEY_ARBITER: &str = "escrow:arbiter";
const KEY_AMOUNT: &str = "escrow:amount";
const KEY_STATUS: &str = "escrow:status";
const KEY_INITIALIZED: &str = "escrow:initialized";

// ─── Status Codes ───────────────────────────────────────────────────────────
// Stored as u8: 0 = Awaiting, 1 = Funded, 2 = Released, 3 = Refunded

const STATUS_AWAITING: u64 = 0;
const STATUS_FUNDED: u64 = 1;
const STATUS_RELEASED: u64 = 2;
const STATUS_REFUNDED: u64 = 3;

fn status_name(code: u64) -> &'static str {
    match code {
        STATUS_AWAITING => "Awaiting",
        STATUS_FUNDED => "Funded",
        STATUS_RELEASED => "Released",
        STATUS_REFUNDED => "Refunded",
        _ => "Unknown",
    }
}

// ─── Initialization ─────────────────────────────────────────────────────────

/// Create a new escrow between buyer, seller, and arbiter.
/// The caller is set as the arbiter.
#[wasm_bindgen]
pub fn init(buyer: String, seller: String) -> Result<String, String> {
    if storage::get_bool(KEY_INITIALIZED) {
        return Err("Escrow already initialized".to_string());
    }

    validation::validate_address(&buyer).map_err(|e| e.to_string())?;
    validation::validate_address(&seller).map_err(|e| e.to_string())?;
    validation::validate_different_addresses(&buyer, &seller)
        .map_err(|e| e.to_string())?;

    let arbiter = host_functions::get_caller();

    storage::set_string(KEY_BUYER, &buyer).map_err(|e| e.to_string())?;
    storage::set_string(KEY_SELLER, &seller).map_err(|e| e.to_string())?;
    storage::set_string(KEY_ARBITER, &arbiter).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_AMOUNT, 0).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_STATUS, STATUS_AWAITING).map_err(|e| e.to_string())?;
    storage::set_bool(KEY_INITIALIZED, true).map_err(|e| e.to_string())?;

    host_functions::emit_event("EscrowCreated", &serde_json::json!({
        "buyer": buyer,
        "seller": seller,
        "arbiter": arbiter,
    }));

    Ok("Escrow initialized".to_string())
}

// ─── Execute ────────────────────────────────────────────────────────────────

/// Deposit funds into the escrow (buyer only).
#[wasm_bindgen]
pub fn deposit(amount: u64) -> Result<String, String> {
    require_initialized()?;
    require_status(STATUS_AWAITING)?;
    require_buyer()?;

    validation::validate_amount(amount).map_err(|e| e.to_string())?;

    let current = storage::get_u64(KEY_AMOUNT).unwrap_or(0);
    let new_amount = math::safe_add(current, amount).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_AMOUNT, new_amount).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_STATUS, STATUS_FUNDED).map_err(|e| e.to_string())?;

    host_functions::emit_event("Deposited", &serde_json::json!({
        "amount": amount,
        "total": new_amount,
    }));

    Ok(format!("Deposited {}. Escrow funded.", amount))
}

/// Release funds to the seller (arbiter only).
#[wasm_bindgen]
pub fn release() -> Result<String, String> {
    require_initialized()?;
    require_status(STATUS_FUNDED)?;
    require_arbiter()?;

    let amount = storage::get_u64(KEY_AMOUNT).unwrap_or(0);
    let seller = storage::get_string(KEY_SELLER).unwrap_or_default();

    storage::set_u64(KEY_STATUS, STATUS_RELEASED).map_err(|e| e.to_string())?;

    host_functions::emit_event("Released", &serde_json::json!({
        "to": seller,
        "amount": amount,
    }));

    Ok(format!("Released {} to seller.", amount))
}

/// Refund funds to the buyer (arbiter only).
#[wasm_bindgen]
pub fn refund() -> Result<String, String> {
    require_initialized()?;
    require_status(STATUS_FUNDED)?;
    require_arbiter()?;

    let amount = storage::get_u64(KEY_AMOUNT).unwrap_or(0);
    let buyer = storage::get_string(KEY_BUYER).unwrap_or_default();

    storage::set_u64(KEY_STATUS, STATUS_REFUNDED).map_err(|e| e.to_string())?;

    host_functions::emit_event("Refunded", &serde_json::json!({
        "to": buyer,
        "amount": amount,
    }));

    Ok(format!("Refunded {} to buyer.", amount))
}

// ─── Queries ────────────────────────────────────────────────────────────────

/// Get the current escrow status.
#[wasm_bindgen]
pub fn status() -> String {
    let code = storage::get_u64(KEY_STATUS).unwrap_or(0);
    status_name(code).to_string()
}

/// Get the escrowed amount.
#[wasm_bindgen]
pub fn amount() -> u64 {
    storage::get_u64(KEY_AMOUNT).unwrap_or(0)
}

/// Get the buyer address.
#[wasm_bindgen]
pub fn buyer() -> Option<String> {
    storage::get_string(KEY_BUYER)
}

/// Get the seller address.
#[wasm_bindgen]
pub fn seller() -> Option<String> {
    storage::get_string(KEY_SELLER)
}

/// Get the arbiter address.
#[wasm_bindgen]
pub fn arbiter() -> Option<String> {
    storage::get_string(KEY_ARBITER)
}

// ─── Internal Helpers ───────────────────────────────────────────────────────

fn require_initialized() -> Result<(), String> {
    if !storage::get_bool(KEY_INITIALIZED) {
        return Err("Escrow not initialized".to_string());
    }
    Ok(())
}

fn require_status(expected: u64) -> Result<(), String> {
    let current = storage::get_u64(KEY_STATUS).unwrap_or(0);
    if current != expected {
        return Err(format!(
            "Invalid status: expected {}, got {}",
            status_name(expected),
            status_name(current),
        ));
    }
    Ok(())
}

fn require_buyer() -> Result<(), String> {
    let caller = host_functions::get_caller();
    let expected = storage::get_string(KEY_BUYER)
        .ok_or("No buyer set".to_string())?;
    if caller != expected {
        return Err("Unauthorized: caller is not the buyer".to_string());
    }
    Ok(())
}

fn require_arbiter() -> Result<(), String> {
    let caller = host_functions::get_caller();
    let expected = storage::get_string(KEY_ARBITER)
        .ok_or("No arbiter set".to_string())?;
    if caller != expected {
        return Err("Unauthorized: caller is not the arbiter".to_string());
    }
    Ok(())
}

// ─── Tests ──────────────────────────────────────────────────────────────────

#[cfg(test)]
mod tests {
    use super::*;

    // Addresses must be 64 hex characters for validation
    const BUYER: &str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    const SELLER: &str = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    #[test]
    fn test_init_escrow() {
        let result = init(BUYER.to_string(), SELLER.to_string());
        assert!(result.is_ok());
        assert_eq!(status(), "Awaiting");
        assert_eq!(amount(), 0);
    }

    #[test]
    fn test_same_buyer_seller_rejected() {
        let result = init(BUYER.to_string(), BUYER.to_string());
        assert!(result.is_err());
    }
}

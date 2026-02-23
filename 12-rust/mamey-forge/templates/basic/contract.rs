//! Basic MameyFutureNode Smart Contract
//!
//! A minimal contract demonstrating init, execute, and query entry points
//! using the MameyFutureNode host functions and shared utilities.
//!
//! This is a standalone reference file. The scaffolded project uses
//! `src/lib.rs` as the crate root (see `lib.rs.template`).

use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage, validation};

// ─── Storage Keys ───────────────────────────────────────────────────────────

const KEY_OWNER: &str = "owner";
const KEY_INITIALIZED: &str = "initialized";

// ─── Entry Point: Init ──────────────────────────────────────────────────────

/// Initialize the contract. Called once at deployment.
///
/// Sets the deployer as the contract owner and marks the contract as
/// initialized to prevent re-initialization.
#[wasm_bindgen]
pub fn init() -> Result<String, String> {
    if storage::get_bool(KEY_INITIALIZED) {
        return Err("Contract already initialized".to_string());
    }

    let caller = host_functions::get_caller();
    storage::set_string(KEY_OWNER, &caller)
        .map_err(|e| format!("Failed to set owner: {}", e))?;
    storage::set_bool(KEY_INITIALIZED, true)
        .map_err(|e| format!("Failed to mark initialized: {}", e))?;

    host_functions::emit_event("Initialized", &serde_json::json!({
        "owner": caller,
    }));
    host_functions::log("Contract initialized");

    Ok("Contract initialized successfully".to_string())
}

// ─── Entry Point: Execute (state-changing) ──────────────────────────────────

/// Store a value under the given key. Only the owner may call this.
#[wasm_bindgen]
pub fn execute_set(key: String, value: String) -> Result<String, String> {
    require_initialized()?;
    require_owner()?;

    validation::validate_string_length(&key, 1, 256)
        .map_err(|e| e.to_string())?;
    validation::validate_string_length(&value, 0, 4096)
        .map_err(|e| e.to_string())?;

    storage::set_string(&format!("data:{}", key), &value)
        .map_err(|e| format!("Storage error: {}", e))?;

    host_functions::emit_event("ValueSet", &serde_json::json!({
        "key": key,
        "value": value,
    }));

    Ok("Value set successfully".to_string())
}

/// Delete a value. Only the owner may call this.
#[wasm_bindgen]
pub fn execute_delete(key: String) -> Result<String, String> {
    require_initialized()?;
    require_owner()?;

    host_functions::storage_del(&format!("data:{}", key))
        .map_err(|e| format!("Delete error: {}", e))?;

    host_functions::emit_event("ValueDeleted", &serde_json::json!({ "key": key }));

    Ok("Value deleted successfully".to_string())
}

// ─── Entry Point: Query (read-only) ─────────────────────────────────────────

/// Retrieve a value by key.
#[wasm_bindgen]
pub fn query_get(key: String) -> Option<String> {
    storage::get_string(&format!("data:{}", key))
}

/// Return the contract owner address.
#[wasm_bindgen]
pub fn query_owner() -> Option<String> {
    storage::get_string(KEY_OWNER)
}

/// Return whether the contract has been initialized.
#[wasm_bindgen]
pub fn query_initialized() -> bool {
    storage::get_bool(KEY_INITIALIZED)
}

// ─── State Helpers ──────────────────────────────────────────────────────────

fn require_initialized() -> Result<(), String> {
    if !storage::get_bool(KEY_INITIALIZED) {
        return Err("Contract not initialized".to_string());
    }
    Ok(())
}

fn require_owner() -> Result<(), String> {
    let caller = host_functions::get_caller();
    let owner = storage::get_string(KEY_OWNER)
        .ok_or_else(|| "No owner set".to_string())?;
    if caller != owner {
        return Err("Unauthorized: caller is not the owner".to_string());
    }
    Ok(())
}

// ─── Tests ──────────────────────────────────────────────────────────────────

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_init() {
        let result = init();
        assert!(result.is_ok());
    }

    #[test]
    fn test_double_init_rejected() {
        let _ = init();
        let result = init();
        assert!(result.is_err());
    }

    #[test]
    fn test_query_initialized() {
        let _ = init();
        assert!(query_initialized());
    }
}

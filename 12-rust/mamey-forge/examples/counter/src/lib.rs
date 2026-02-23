//! Counter Contract Example
//!
//! A minimal MameyFutureNode smart contract that maintains a counter.
//! Demonstrates:
//!   - Initialization and ownership
//!   - State-changing operations (increment, decrement, reset)
//!   - Read-only queries
//!   - Event emission
//!   - Safe math (overflow/underflow protection)

use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage, math};

// ─── Storage Keys ───────────────────────────────────────────────────────────

const KEY_COUNTER: &str = "counter:value";
const KEY_OWNER: &str = "counter:owner";
const KEY_INITIALIZED: &str = "counter:initialized";

// ─── Initialization ─────────────────────────────────────────────────────────

/// Initialize the counter. Sets the caller as owner and value to 0.
#[wasm_bindgen]
pub fn init() -> Result<String, String> {
    if storage::get_bool(KEY_INITIALIZED) {
        return Err("Already initialized".to_string());
    }

    let caller = host_functions::get_caller();

    storage::set_string(KEY_OWNER, &caller).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_COUNTER, 0).map_err(|e| e.to_string())?;
    storage::set_bool(KEY_INITIALIZED, true).map_err(|e| e.to_string())?;

    host_functions::emit_event("CounterInitialized", &serde_json::json!({
        "owner": caller,
    }));

    Ok("Counter initialized".to_string())
}

// ─── Execute ────────────────────────────────────────────────────────────────

/// Increment the counter by 1.
#[wasm_bindgen]
pub fn increment() -> Result<String, String> {
    require_initialized()?;

    let current = storage::get_u64(KEY_COUNTER).unwrap_or(0);
    let new_value = math::safe_add(current, 1).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_COUNTER, new_value).map_err(|e| e.to_string())?;

    host_functions::emit_event("Incremented", &serde_json::json!({
        "old_value": current,
        "new_value": new_value,
    }));

    Ok(format!("Counter: {}", new_value))
}

/// Decrement the counter by 1. Fails if counter is already 0.
#[wasm_bindgen]
pub fn decrement() -> Result<String, String> {
    require_initialized()?;

    let current = storage::get_u64(KEY_COUNTER).unwrap_or(0);
    let new_value = math::safe_sub(current, 1)
        .map_err(|_| "Counter is already at zero".to_string())?;
    storage::set_u64(KEY_COUNTER, new_value).map_err(|e| e.to_string())?;

    host_functions::emit_event("Decremented", &serde_json::json!({
        "old_value": current,
        "new_value": new_value,
    }));

    Ok(format!("Counter: {}", new_value))
}

/// Reset the counter to 0 (owner only).
#[wasm_bindgen]
pub fn reset() -> Result<String, String> {
    require_initialized()?;
    require_owner()?;

    let old_value = storage::get_u64(KEY_COUNTER).unwrap_or(0);
    storage::set_u64(KEY_COUNTER, 0).map_err(|e| e.to_string())?;

    host_functions::emit_event("Reset", &serde_json::json!({
        "old_value": old_value,
    }));

    Ok("Counter reset to 0".to_string())
}

/// Set the counter to a specific value (owner only).
#[wasm_bindgen]
pub fn set(value: u64) -> Result<String, String> {
    require_initialized()?;
    require_owner()?;

    storage::set_u64(KEY_COUNTER, value).map_err(|e| e.to_string())?;

    host_functions::emit_event("ValueSet", &serde_json::json!({
        "value": value,
    }));

    Ok(format!("Counter set to {}", value))
}

// ─── Queries ────────────────────────────────────────────────────────────────

/// Get the current counter value.
#[wasm_bindgen]
pub fn get() -> u64 {
    storage::get_u64(KEY_COUNTER).unwrap_or(0)
}

/// Get the contract owner.
#[wasm_bindgen]
pub fn owner() -> Option<String> {
    storage::get_string(KEY_OWNER)
}

// ─── Internal Helpers ───────────────────────────────────────────────────────

fn require_initialized() -> Result<(), String> {
    if !storage::get_bool(KEY_INITIALIZED) {
        return Err("Contract not initialized".to_string());
    }
    Ok(())
}

fn require_owner() -> Result<(), String> {
    let caller = host_functions::get_caller();
    let contract_owner = storage::get_string(KEY_OWNER)
        .ok_or("No owner set".to_string())?;
    if caller != contract_owner {
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
        assert_eq!(get(), 0);
    }

    #[test]
    fn test_increment() {
        let _ = init();
        let result = increment();
        assert!(result.is_ok());
        assert_eq!(get(), 1);
    }

    #[test]
    fn test_decrement_at_zero_fails() {
        let _ = init();
        let result = decrement();
        assert!(result.is_err());
    }

    #[test]
    fn test_increment_then_decrement() {
        let _ = init();
        let _ = increment();
        let _ = increment();
        let result = decrement();
        assert!(result.is_ok());
        assert_eq!(get(), 1);
    }
}

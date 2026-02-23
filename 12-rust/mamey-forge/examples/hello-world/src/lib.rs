//! Hello World — The simplest possible MameyFutureNode contract.
//!
//! Stores a greeting message and lets anyone read it.

use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage};

const KEY_GREETING: &str = "greeting";
const KEY_INITIALIZED: &str = "initialized";

/// Initialize with a greeting.
#[wasm_bindgen]
pub fn init() -> Result<String, String> {
    if storage::get_bool(KEY_INITIALIZED) {
        return Err("Already initialized".to_string());
    }

    storage::set_string(KEY_GREETING, "Hello, MameyFutureNode!")
        .map_err(|e| e.to_string())?;
    storage::set_bool(KEY_INITIALIZED, true)
        .map_err(|e| e.to_string())?;

    host_functions::emit_event("HelloInitialized", &serde_json::json!({
        "greeting": "Hello, MameyFutureNode!",
    }));

    Ok("Hello World contract initialized".to_string())
}

/// Update the greeting.
#[wasm_bindgen]
pub fn execute_set_greeting(message: String) -> Result<String, String> {
    storage::set_string(KEY_GREETING, &message)
        .map_err(|e| e.to_string())?;

    host_functions::emit_event("GreetingUpdated", &serde_json::json!({
        "message": message,
    }));

    Ok("Greeting updated".to_string())
}

/// Read the current greeting.
#[wasm_bindgen]
pub fn query_greeting() -> String {
    storage::get_string(KEY_GREETING).unwrap_or_else(|| "No greeting set".to_string())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_hello_world() {
        let result = init();
        assert!(result.is_ok());
        assert_eq!(query_greeting(), "Hello, MameyFutureNode!");
    }

    #[test]
    fn test_update_greeting() {
        let _ = init();
        let _ = execute_set_greeting("Hola, Mamey!".into());
        assert_eq!(query_greeting(), "Hola, Mamey!");
    }
}

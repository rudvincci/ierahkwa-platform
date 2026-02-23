# Contract API Reference

This document covers the MameyFutureNode smart contract API — entry points, state management, host functions, and cross-contract calls.

## Entry Point Pattern

Every MameyFutureNode contract is a Rust library compiled to WASM. The runtime recognises three categories of exported functions:

| Category | Convention | Mutates State? | Example |
|----------|-----------|----------------|---------|
| **Init** | `init(...)` | Yes (once) | `pub fn init() -> Result<String, String>` |
| **Execute** | `execute_*(...)` | Yes | `pub fn execute_set(key, value)` |
| **Query** | `query_*(...)` | No | `pub fn query_get(key) -> Option<String>` |

All public functions are annotated with `#[wasm_bindgen]` so the host runtime can call them by name.

### Init

```rust
#[wasm_bindgen]
pub fn init(/* constructor args */) -> Result<String, String> {
    // Guard against re-initialization
    if storage::get_bool("initialized") {
        return Err("Already initialized".into());
    }
    // Set up initial state ...
    storage::set_bool("initialized", true)?;
    Ok("OK".into())
}
```

### Execute (state-changing)

```rust
#[wasm_bindgen]
pub fn execute_transfer(to: String, amount: u64) -> Result<String, String> {
    let caller = host_functions::get_caller();
    // Validate, mutate storage, emit events
    host_functions::emit_event("Transfer", &serde_json::json!({
        "from": caller, "to": to, "amount": amount
    }));
    Ok("Transfer successful".into())
}
```

### Query (read-only)

```rust
#[wasm_bindgen]
pub fn query_balance(account: String) -> u64 {
    storage::get_u64(&format!("bal:{}", account)).unwrap_or(0)
}
```

## State Management

### Storage API

The `mamey_contracts_shared::storage` module provides typed key-value storage:

```rust
use mamey_contracts_shared::storage;

// Strings
storage::set_string("key", "value")?;
let val: Option<String> = storage::get_string("key");

// Integers
storage::set_u64("counter", 42)?;
let n: u64 = storage::get_u64("counter").unwrap_or(0);

// Booleans
storage::set_bool("active", true)?;
let active: bool = storage::get_bool("active");

// Bytes (u8)
storage::set_u8("decimals", 18)?;
```

### Key Conventions

| Pattern | Usage |
|---------|-------|
| `owner` | Contract owner address |
| `initialized` | Init guard flag |
| `bal:<address>` | Balance for an account |
| `allow:<owner>:<spender>` | Allowance |
| `data:<key>` | Generic user data |
| `token:name`, `token:symbol` | Token metadata |

## Host Functions

The `mamey_contracts_shared::host_functions` module provides runtime services:

```rust
use mamey_contracts_shared::host_functions;

// Identity
let caller: String = host_functions::get_caller();

// Events (indexed by the node/indexer)
host_functions::emit_event("EventName", &serde_json::json!({
    "field": "value"
}));

// Logging (dev/debug)
host_functions::log("Debug message");

// Storage deletion
host_functions::storage_del("key")?;
```

## Validation Utilities

```rust
use mamey_contracts_shared::validation;

// String length bounds
validation::validate_string_length(&name, 1, 100)?;

// Address format
validation::validate_address(&addr)?;

// Amount > 0
validation::validate_amount(amount)?;

// Decimals range
validation::validate_decimals(decimals)?;

// Distinct addresses (e.g., no self-approve)
validation::validate_different_addresses(&a, &b)?;
```

## Math Utilities

Safe arithmetic that returns `Result` on overflow/underflow:

```rust
use mamey_contracts_shared::math;

let sum = math::safe_add(a, b)?;      // u64 + u64
let diff = math::safe_sub(a, b)?;     // u64 - u64 (errors if b > a)
```

## Cross-Contract Calls

> **Status:** Planned for a future release. Currently, contracts are isolated.

When available, the API will look like:

```rust
// Invoke another contract's execute function
let result = host_functions::call_contract(
    "0xOtherContractAddress",
    "execute_transfer",
    &serde_json::json!({"to": "0x...", "amount": 100}),
)?;

// Query another contract (read-only)
let balance = host_functions::query_contract(
    "0xOtherContractAddress",
    "query_balance",
    &serde_json::json!({"account": "0x..."}),
)?;
```

## Error Handling

All execute/init functions return `Result<String, String>`:
- `Ok(message)` — success, included in transaction receipt
- `Err(message)` — failure, transaction is reverted, error surfaced to caller

## Events

Events emitted via `host_functions::emit_event()` are:
1. Recorded in the transaction receipt
2. Indexed by the read-model Postgres indexer
3. Available via the Portal API for filtering/search/reporting

Standard event names: `Initialized`, `Transfer`, `Approval`, `Mint`, `Burn`, `Paused`, `Unpaused`.

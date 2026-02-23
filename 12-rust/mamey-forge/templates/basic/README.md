# Basic Contract Template

A minimal MameyFutureNode WASM smart contract demonstrating the three core entry points: **init**, **execute**, and **query**.

## What's Included

| File | Purpose |
|------|---------|
| `contract.rs` | Standalone reference contract source |
| `src/lib.rs.template` | Scaffolded template (used by `mameyforge init`) |
| `Cargo.toml` / `Cargo.toml.template` | Rust package manifest |
| `mameyforge.toml.template` | MameyForge project config |

## Entry Points

### `init()`
Called once at deployment. Sets the caller as owner and marks the contract as initialized.

### `execute_set(key, value)` / `execute_delete(key)`
State-changing operations. Only the owner can call these.

### `query_get(key)` / `query_owner()` / `query_initialized()`
Read-only queries. Anyone can call these.

## Quick Start

```bash
# Scaffold a new project from this template
mameyforge init my-contract --template basic

cd my-contract

# Build to WASM
mameyforge build

# Run tests
mameyforge test

# Start local devnet and deploy
mameyforge devnet start
mameyforge deploy --network devnet
```

## Customising

1. Edit `src/lib.rs` to add your business logic
2. Add new storage keys for your state
3. Add new `execute_*` functions for state changes
4. Add new `query_*` functions for reads
5. Emit events with `host_functions::emit_event()`

## Dependencies

- `mamey-contracts-base` — Core runtime interfaces
- `mamey-contracts-shared` — Host functions, storage, validation utilities
- `wasm-bindgen` — WASM interop
- `serde` / `serde_json` — Serialization

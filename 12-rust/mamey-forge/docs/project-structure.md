# Project Structure

Every MameyForge project follows a consistent layout that separates contract source, tests, deployment scripts, and build artifacts.

## Directory Layout

```
my-project/
├── contracts/
│   └── src/
│       └── lib.rs              # Contract entry points
├── tests/
│   └── integration.rs          # Integration tests
├── scripts/
│   └── deploy.rs               # Deployment / migration script
├── artifacts/                  # Build output (gitignored)
│   └── my_project.wasm         # Compiled WASM binary
├── .mameyforge/                # Local devnet data (gitignored)
│   └── devnet/
├── Cargo.toml                  # Rust package manifest
├── mameyforge.toml             # MameyForge configuration
└── .gitignore
```

## Key Files

### `mameyforge.toml`

The project configuration file. Controls project metadata, build options, network endpoints, and devnet settings.

```toml
[project]
name = "my-project"
version = "0.1.0"
authors = ["You <you@example.com>"]

[contracts]
path = "contracts"          # Where contract source lives
output = "artifacts"        # Where WASM artifacts go

[build]
optimization_level = "z"    # WASM optimisation (s, z, or 3)
strip = true                # Strip debug symbols
lto = true                  # Link-time optimisation

[networks.devnet]
url = "http://localhost:50051"
chain_id = "mamey-general-devnet"

[devnet]
port = 50051
auto_mine = true
block_time = 1
```

See [Configuration Reference](configuration.md) for full details.

### `Cargo.toml`

Standard Rust manifest. Generated projects depend on:

- **`mamey-contracts-base`** — Security patterns, validation, typed storage, safe math, error types, ownership/roles
- **`mamey-contracts-shared`** — Host function bindings (`storage_set`, `storage_get`, `emit_event`, `get_caller`, etc.), storage helpers, math utilities, validation helpers

```toml
[dependencies]
wasm-bindgen = "0.2"
mamey-contracts-base = { git = "https://github.com/Mamey-io/mamey-contracts.git", package = "mamey-contracts-base" }
mamey-contracts-shared = { git = "https://github.com/Mamey-io/mamey-contracts.git", package = "mamey-contracts-shared" }
```

### `contracts/src/lib.rs`

The contract entry point. Exports public functions via `#[wasm_bindgen]` that become callable on-chain. A typical contract has:

- **`init()`** — Called once at deployment to set up initial state
- **`execute_*()` / `call_*()`** — State-changing functions
- **`query_*()` / read functions** — Read-only functions

```rust
use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage, validation};

#[wasm_bindgen]
pub fn init() -> Result<String, String> { /* ... */ }

#[wasm_bindgen]
pub fn transfer(to: String, amount: u64) -> Result<String, String> { /* ... */ }

#[wasm_bindgen]
pub fn balance_of(account: String) -> u64 { /* ... */ }
```

## The Shared Library (`mamey-contracts-shared`)

The shared library provides the runtime interface:

| Module | Purpose |
|--------|---------|
| `host_functions` | Bindings to MameyNode runtime: `storage_set`, `storage_get`, `storage_del`, `emit_event`, `log`, `get_caller`, `get_timestamp`, `get_block_number` |
| `storage` | Type-safe helpers: `set_string`, `get_string`, `set_u64`, `get_u64`, `set_bool`, `get_bool`, `inc_u64`, `dec_u64` |
| `math` | Safe arithmetic: `safe_add`, `safe_sub`, `safe_mul`, `safe_div`, `percentage`, `basis_points` |
| `validation` | Input checks: `validate_address`, `validate_amount`, `validate_string_length`, `validate_decimals`, `validate_different_addresses` |

## The Base Library (`mamey-contracts-base`)

Higher-level patterns for production contracts:

| Module | Purpose |
|--------|---------|
| `security` | Reentrancy guards, rate limiting, transfer limits, time locks, cooldowns |
| `validation` | Address and amount validation with typed errors |
| `storage` | Typed storage (same helpers, wired through base error types) |
| `math` | Safe arithmetic with base error types |
| `errors` | `ContractError` enum: `Overflow`, `Underflow`, `InvalidState`, etc. |
| `patterns` | Ownable, Roles — common access-control patterns |

## Build Output

After `mameyforge build --release`, the `artifacts/` directory contains:

```
artifacts/
├── my_project.wasm       # Optimised WASM binary
└── my_project.json       # ABI / metadata (when available)
```

The WASM binary is what gets deployed to MameyFutureNode.

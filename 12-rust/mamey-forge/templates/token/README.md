# Token Contract Template

A fungible token (ERC-20-like) contract for MameyFutureNode with **mint**, **transfer**, **burn**, and **balance_of** support, plus allowances and governance controls.

## What's Included

| File | Purpose |
|------|---------|
| `contract.rs` | Standalone reference contract source |
| `src/lib.rs.template` | Scaffolded template (used by `mameyforge init`) |
| `Cargo.toml` / `Cargo.toml.template` | Rust package manifest (includes `mamey-token-std`) |
| `mameyforge.toml.template` | MameyForge project config |

## Token Functions

### Initialization
- `init(name, symbol, decimals, initial_supply)` — One-time setup; mints initial supply to deployer.

### Queries (read-only)
- `name()` / `symbol()` / `decimals()` — Token metadata
- `total_supply()` — Current circulating supply
- `balance_of(account)` — Balance of a specific account
- `allowance(owner, spender)` — Approved spend amount

### State-changing
- `transfer(to, amount)` — Send tokens from caller
- `transfer_from(from, to, amount)` — Delegated transfer (requires allowance)
- `approve(spender, amount)` — Approve a spender
- `mint(to, amount)` — Mint new tokens (minter only)
- `burn(amount)` — Burn tokens from caller's balance

### Governance (owner only)
- `pause()` / `unpause()` — Emergency circuit breaker
- `set_minter(address)` — Change the minter address

## Quick Start

```bash
mameyforge init my-token --template token
cd my-token

mameyforge build
mameyforge test

mameyforge devnet start
mameyforge deploy --network devnet \
  --args '{"name":"MyToken","symbol":"MTK","decimals":18,"initial_supply":1000000}'
```

## Dependencies

- `mamey-contracts-base` — Core runtime interfaces
- `mamey-contracts-shared` — Host functions, storage, math, validation
- `mamey-token-std` — Token standard library (ERC-20 patterns)
- `wasm-bindgen` — WASM interop
- `serde` / `serde_json` — Serialization

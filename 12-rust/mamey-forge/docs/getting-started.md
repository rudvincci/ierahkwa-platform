# Getting Started with MameyForge

MameyForge is the smart contract development framework for **MameyFutureNode** — think Hardhat, but for the Mamey ecosystem. It handles project scaffolding, building Rust contracts to WASM, testing, local devnet, deployment, and on-chain interaction.

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| Rust | 1.75+ | Contract compilation |
| `wasm32-unknown-unknown` target | — | WASM output |
| Docker | 20+ | Local devnet |
| MameyForge | latest | This CLI |

```bash
# Install Rust (if needed)
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# Add the WASM target
rustup target add wasm32-unknown-unknown
```

## Install MameyForge

### From Source

```bash
git clone https://github.com/Mamey-io/MameyForge.git
cd MameyForge
cargo install --path .
```

### Verify

```bash
mameyforge --version
```

## Create Your First Project

```bash
# Scaffold a basic contract
mameyforge init my-first-contract

# Or pick a template
mameyforge init my-token --template token
mameyforge init my-nft   --template nft
```

This creates:

```
my-first-contract/
├── contracts/
│   └── src/
│       └── lib.rs            # Your contract
├── tests/
│   └── integration.rs        # Integration tests
├── scripts/
│   └── deploy.rs             # Deploy script
├── artifacts/                # Build output (gitignored)
├── Cargo.toml                # Rust package
├── mameyforge.toml           # MameyForge config
└── .gitignore
```

## Build

```bash
cd my-first-contract

# Debug build
mameyforge build

# Release build (optimised WASM for deployment)
mameyforge build --release
```

Compiled WASM artifacts land in `artifacts/`.

## Test

```bash
# Run all tests
mameyforge test

# Filter by test name
mameyforge test --filter "test_transfer"

# Verbose output
mameyforge test -v
```

## Start a Local Devnet

A local MameyFutureNode General chain for development:

```bash
# Start
mameyforge devnet start

# Check status
mameyforge devnet status

# Tail logs
mameyforge devnet logs

# Stop
mameyforge devnet stop
```

## Deploy

```bash
# Deploy to local devnet
mameyforge deploy --contract my_first_contract --network devnet

# Deploy to testnet
mameyforge deploy --contract my_first_contract --network testnet

# With constructor arguments
mameyforge deploy --contract my_first_contract --network devnet \
  --args '{"name": "MyToken", "symbol": "MTK", "decimals": 18, "initial_supply": 1000000}'
```

## Interact

```bash
# State-changing call
mameyforge call \
  --contract 0x1234...abcd \
  --function transfer \
  --args '{"to": "0x5678...ef01", "amount": 100}'

# Read-only query
mameyforge query \
  --contract 0x1234...abcd \
  --function balance_of \
  --args '{"account": "0x5678...ef01"}'

# Contract info
mameyforge info --contract 0x1234...abcd
```

## Next Steps

- [Project Structure](project-structure.md) — understand the layout
- [Templates](templates.md) — available templates and how to make your own
- [Commands Reference](commands.md) — full CLI reference
- [Configuration](configuration.md) — `mameyforge.toml` deep-dive

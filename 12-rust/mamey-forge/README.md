# MameyForge

Smart contract development framework for MameyFutureNode — the Hardhat of the Mamey ecosystem.

## Overview

MameyForge provides a complete development environment for building, testing, and deploying WASM smart contracts on MameyFutureNode's General chain.

## Features

- **Project Initialization** — Scaffold new contract projects with [file-based templates](#contract-templates)
- **Contract Building** — Compile Rust contracts to WASM with optimization
- **Testing Framework** — Run unit and integration tests
- **Local Devnet** — Start a local General node for development
- **Deployment** — Deploy contracts to devnet, testnet, or mainnet
- **Interaction** — Call and query deployed contracts

## 📖 Documentation

| Document | Description |
|----------|-------------|
| [Getting Started](docs/getting-started.md) | Install, create, build, test, deploy |
| [Project Structure](docs/project-structure.md) | Layout, mameyforge.toml, shared libs |
| [Templates](docs/templates.md) | Available templates & custom templates |
| [Commands Reference](docs/commands.md) | Full CLI reference |
| [Configuration](docs/configuration.md) | mameyforge.toml deep-dive |

## Installation

### From Source

```bash
# Clone the repository
git clone https://github.com/Mamey-io/MameyForge.git
cd MameyForge

# Build and install
cargo install --path .
```

### Prerequisites

- Rust 1.75+ with `wasm32-unknown-unknown` target
- Docker (for devnet)

```bash
# Add WASM target
rustup target add wasm32-unknown-unknown

# Verify installation
mameyforge --version
```

## Quick Start

### Initialize a New Project

```bash
# Create a new contract project
mameyforge init my-contract

cd my-contract

# Project structure:
# my-contract/
# ├── contracts/
# │   └── src/
# │       └── lib.rs
# ├── tests/
# │   └── integration.rs
# ├── scripts/
# │   └── deploy.rs
# ├── Cargo.toml
# └── mameyforge.toml
```

### Build Contracts

```bash
# Build all contracts
mameyforge build

# Build with optimization (for production)
mameyforge build --release

# Build specific contract
mameyforge build --contract my_contract
```

### Run Tests

```bash
# Run all tests
mameyforge test

# Run specific test
mameyforge test --filter "test_transfer"

# Run with verbose output
mameyforge test -v
```

### Start Local Devnet

```bash
# Start devnet
mameyforge devnet start

# Check status
mameyforge devnet status

# View logs
mameyforge devnet logs

# Stop devnet
mameyforge devnet stop
```

### Deploy Contracts

```bash
# Deploy to devnet
mameyforge deploy --contract MyContract --network devnet

# Deploy to testnet
mameyforge deploy --contract MyContract --network testnet

# Deploy with constructor args
mameyforge deploy --contract MyContract --args '{"name": "Test", "supply": 1000000}'
```

### Interact with Contracts

```bash
# Call a function (state-changing)
mameyforge call --contract 0x1234... --function transfer --args '{"to": "0x5678...", "amount": 100}'

# Query a function (read-only)
mameyforge query --contract 0x1234... --function balance_of --args '{"account": "0xabcd..."}'

# Get contract info
mameyforge info --contract 0x1234...
```

## Configuration

### mameyforge.toml

```toml
[project]
name = "my-project"
version = "0.1.0"
authors = ["Your Name <you@example.com>"]

[contracts]
path = "contracts"
output = "artifacts"

[build]
optimization_level = "z"  # s, z, or 3
strip = true
lto = true

[networks.devnet]
url = "http://localhost:50051"
chain_id = "mamey-general-devnet"

[networks.testnet]
url = "https://testnet.mamey.io:50051"
chain_id = "mamey-general-testnet"

[networks.mainnet]
url = "https://mainnet.mamey.io:50051"
chain_id = "mamey-general"

[devnet]
port = 50051
data_dir = ".mameyforge/devnet"
auto_mine = true
block_time = 1
```

## Commands

| Command | Description |
|---------|-------------|
| `init <name>` | Initialize a new project |
| `build` | Build contracts to WASM |
| `test` | Run tests |
| `devnet start` | Start local devnet |
| `devnet stop` | Stop local devnet |
| `devnet status` | Check devnet status |
| `devnet logs` | View devnet logs |
| `deploy` | Deploy a contract |
| `call` | Call a contract function |
| `query` | Query a contract (read-only) |
| `info` | Get contract information |
| `clean` | Clean build artifacts |
| `verify` | Verify contract source |

## Contract Templates

MameyForge ships with file-based templates (`templates/`) and built-in inline fallbacks:

| Template | Description | Command |
|----------|-------------|---------|
| **basic** (default) | Minimal contract with key-value storage | `mameyforge init my-app` |
| **token** | ERC-20-like fungible token | `mameyforge init my-token --template token` |
| **nft** | ERC-721-like non-fungible token | `mameyforge init my-nft --template nft` |
| **governance** | DAO governance (inline) | `mameyforge init my-dao --template governance` |
| **multi-token** | Multi-token standard (inline) | `mameyforge init my-mt --template multi-token` |

Templates use `{{project_name}}`, `{{version}}`, and `{{author}}` placeholders that are replaced during `init`. See [Templates docs](docs/templates.md) for details on creating custom templates.

## Examples

Working contract examples in [`examples/`](examples/):

| Example | Description |
|---------|-------------|
| [counter](examples/counter/) | Simple counter — increment, decrement, reset |
| [escrow](examples/escrow/) | Two-party escrow — deposit, release, refund |

## Devnet Features

- **Auto-mining** - Automatic block production
- **Time manipulation** - Fast-forward time for testing
- **State snapshots** - Save and restore devnet state
- **Rich logging** - Detailed execution traces

## Integration with mamey-contracts

MameyForge integrates with the `mamey-contracts` library:

```rust
use mamey_contracts_base::{Storage, Security, Validation};
use mamey_contracts_token::Token;

#[contract]
pub struct MyToken {
    token: Token,
}
```

## CI/CD Integration

### GitHub Actions

```yaml
name: Contract CI

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: mamey-io/setup-mameyforge@v1
      - run: mameyforge build
      - run: mameyforge test
```

## Troubleshooting

### Build Errors

```bash
# Clean and rebuild
mameyforge clean
mameyforge build

# Check Rust version
rustc --version

# Ensure WASM target is installed
rustup target list --installed | grep wasm
```

### Devnet Issues

```bash
# Reset devnet
mameyforge devnet stop
rm -rf .mameyforge/devnet
mameyforge devnet start

# Check port availability
lsof -i :50051
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

AGPL-3.0 - See [LICENSE](LICENSE) for details.

## Related

- [MameyFutureNode](https://github.com/Mamey-io/MameyFutureNode) - Multi-chain node
- [mamey-contracts](https://github.com/Mamey-io/mamey-contracts) - Contract libraries
- [MameyFutureNode.Protos](https://github.com/Mamey-io/MameyFutureNode.Protos) - gRPC definitions

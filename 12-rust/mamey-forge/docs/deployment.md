# Deployment Guide

How to deploy MameyFutureNode smart contracts to testnet and mainnet.

## Overview

```
Source (.rs) → Build (WASM) → Deploy (network) → Verify → Interact
```

MameyForge handles the entire pipeline. Under the hood it:
1. Compiles your Rust contract to optimised WASM (`wasm32-unknown-unknown`)
2. Strips & optimises the binary (`wasm-opt`)
3. Submits a deploy transaction to the target network
4. Returns the contract address on success

## Networks

| Network | Endpoint | Purpose |
|---------|----------|---------|
| `devnet` | `http://localhost:26657` | Local development (Docker) |
| `testnet` | `https://testnet.mamey.io` | Public test network |
| `mainnet` | `https://mainnet.mamey.io` | Production |

Configure endpoints in `mameyforge.toml`:

```toml
[networks.devnet]
rpc = "http://localhost:26657"
chain_id = "mamey-devnet-1"

[networks.testnet]
rpc = "https://testnet-rpc.mamey.io"
chain_id = "mamey-testnet-1"

[networks.mainnet]
rpc = "https://mainnet-rpc.mamey.io"
chain_id = "mamey-mainnet-1"
```

## Step-by-Step

### 1. Build for Release

```bash
mameyforge build --release
```

This produces an optimised WASM artifact in `artifacts/`:

```
artifacts/
└── my_contract.wasm    # ~50-200 KB typical
```

### 2. Deploy to Devnet (Local)

```bash
# Start devnet if not running
mameyforge devnet start

# Deploy
mameyforge deploy --contract my_contract --network devnet

# With constructor arguments
mameyforge deploy --contract my_contract --network devnet \
  --args '{"name":"MyToken","symbol":"MTK","decimals":18,"initial_supply":1000000}'
```

Output:
```
✓ Contract deployed successfully
  Address: 0xabc123...def456
  Tx Hash: 0x789...
  Gas Used: 142,580
```

### 3. Deploy to Testnet

```bash
# Set your wallet key (one-time)
mameyforge config set wallet.key <your-private-key>
# Or use environment variable:
export MAMEY_WALLET_KEY=<your-private-key>

# Deploy
mameyforge deploy --contract my_contract --network testnet \
  --args '{"name":"MyToken","symbol":"MTK","decimals":18,"initial_supply":1000000}'
```

### 4. Deploy to Mainnet

```bash
# Double-check: audit your contract, run all tests
mameyforge test
mameyforge build --release

# Deploy with explicit confirmation
mameyforge deploy --contract my_contract --network mainnet \
  --args '...' \
  --confirm
```

> ⚠️ Mainnet deployments are **irreversible**. The `--confirm` flag is required.

## Verification

After deployment, verify your contract source matches the on-chain bytecode:

```bash
mameyforge verify --contract <ADDRESS> --network testnet
```

This compares your local WASM artifact hash against the deployed bytecode hash.

## Contract Info

```bash
# View deployed contract details
mameyforge info --contract <ADDRESS> --network testnet
```

Output:
```
Contract: 0xabc123...def456
  Network:    testnet
  Creator:    0x111...222
  Block:      1,234,567
  Code Hash:  sha256:abcd1234...
  Initialized: true
```

## Upgradeability

> **Status:** Proxy-based upgrades planned for a future release. Currently, deployed contracts are immutable.

When available:
```bash
mameyforge upgrade --proxy <PROXY_ADDRESS> --new-impl <NEW_ADDRESS> --network testnet
```

## Cost Estimation

Before deploying, estimate gas costs:

```bash
mameyforge estimate --contract my_contract --network testnet \
  --args '{"name":"MyToken","symbol":"MTK","decimals":18,"initial_supply":1000000}'
```

## Troubleshooting

| Issue | Fix |
|-------|-----|
| "Insufficient funds" | Top up your wallet on the faucet (testnet) |
| "WASM too large" | Use `--release` build; check `opt-level = "z"` in Cargo.toml |
| "Init failed" | Check constructor args match the `init()` signature |
| "Network unreachable" | Verify RPC endpoint in `mameyforge.toml` |
| "Nonce mismatch" | Wait for pending tx to confirm, or use `--nonce <N>` |

## Testnet Faucet

Get free testnet tokens for deployment:

```bash
mameyforge faucet --network testnet --address <YOUR_ADDRESS>
```

Or visit: `https://faucet.testnet.mamey.io`

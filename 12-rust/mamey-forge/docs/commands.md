# CLI Commands Reference

MameyForge provides a set of subcommands for the full smart-contract development lifecycle.

## Global Options

| Flag | Description |
|------|-------------|
| `-v, --verbose` | Enable debug-level output |
| `-c, --config <PATH>` | Path to `mameyforge.toml` (default: `mameyforge.toml`) |
| `--version` | Print version |
| `--help` | Print help |

---

## `init`

Scaffold a new contract project.

```bash
mameyforge init <NAME> [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `<NAME>` | *required* | Project name (also the directory name) |
| `-t, --template <TPL>` | `basic` | Template: `basic`, `token`, `nft`, `governance`, `multi-token` |
| `-f, --force` | — | Overwrite if directory exists |

**Examples:**

```bash
mameyforge init my-contract
mameyforge init my-token --template token
mameyforge init my-nft --template nft --force
```

---

## `build`

Compile contracts to optimised WASM.

```bash
mameyforge build [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--release` | — | Production build with full optimisation |
| `--contract <NAME>` | all | Build a specific contract |

Reads `[build]` section from `mameyforge.toml` for optimisation level, LTO, and stripping.

---

## `test`

Run unit and integration tests.

```bash
mameyforge test [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--filter <PATTERN>` | — | Run only tests matching the pattern |
| `-v` | — | Verbose test output |

---

## `deploy`

Deploy a compiled contract to a network.

```bash
mameyforge deploy [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--contract <NAME>` | *required* | Contract to deploy |
| `--network <NET>` | `devnet` | Target network (`devnet`, `testnet`, `mainnet`) |
| `--args <JSON>` | — | Constructor arguments as JSON |

**Example:**

```bash
mameyforge deploy \
  --contract my_token \
  --network testnet \
  --args '{"name":"MyCoin","symbol":"MC","decimals":18,"initial_supply":1000000}'
```

---

## `call`

Execute a state-changing function on a deployed contract.

```bash
mameyforge call [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--contract <ADDR>` | *required* | Contract address |
| `--function <NAME>` | *required* | Function to call |
| `--args <JSON>` | — | Function arguments as JSON |
| `--network <NET>` | `devnet` | Target network |

**Example:**

```bash
mameyforge call \
  --contract 0xabcd...1234 \
  --function transfer \
  --args '{"to":"0x5678...","amount":100}' \
  --network devnet
```

---

## `query`

Read-only query against a deployed contract (no state change).

```bash
mameyforge query [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--contract <ADDR>` | *required* | Contract address |
| `--function <NAME>` | *required* | Function to query |
| `--args <JSON>` | — | Function arguments as JSON |
| `--network <NET>` | `devnet` | Target network |

**Example:**

```bash
mameyforge query \
  --contract 0xabcd...1234 \
  --function balance_of \
  --args '{"account":"0x5678..."}'
```

---

## `info`

Retrieve metadata about a deployed contract.

```bash
mameyforge info [OPTIONS]
```

| Option | Default | Description |
|--------|---------|-------------|
| `--contract <ADDR>` | *required* | Contract address |
| `--network <NET>` | `devnet` | Target network |

---

## `clean`

Remove build artifacts.

```bash
mameyforge clean
```

Deletes the `artifacts/` directory and Rust `target/` build cache.

---

## `devnet`

Manage a local MameyFutureNode General node (Docker-based).

### `devnet start`

```bash
mameyforge devnet start
```

Starts a local devnet using the Docker image configured in `mameyforge.toml`.

### `devnet stop`

```bash
mameyforge devnet stop
```

### `devnet status`

```bash
mameyforge devnet status
```

### `devnet logs`

```bash
mameyforge devnet logs
```

Tail-follows the devnet container logs.

---

## Exit Codes

| Code | Meaning |
|------|---------|
| `0` | Success |
| `1` | General error |
| `2` | Configuration error |
| `3` | Build error |
| `4` | Test failure |
| `5` | Deployment error |
| `6` | Network error |

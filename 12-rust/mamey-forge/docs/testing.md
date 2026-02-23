# Testing Contracts

MameyForge supports unit tests, integration tests, and local devnet testing for MameyFutureNode smart contracts.

## Unit Tests

Standard Rust `#[cfg(test)]` modules work out of the box:

```rust
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_init() {
        let result = init();
        assert!(result.is_ok());
    }

    #[test]
    fn test_double_init_rejected() {
        let _ = init();
        let result = init();
        assert!(result.is_err());
    }

    #[test]
    fn test_query_after_set() {
        let _ = init();
        let _ = execute_set("greeting".into(), "hello".into());
        assert_eq!(query_get("greeting".into()), Some("hello".to_string()));
    }
}
```

Run with:

```bash
mameyforge test

# Or vanilla cargo (useful for IDE integration):
cargo test
```

### Filtering Tests

```bash
# Run only tests matching a pattern
mameyforge test --filter "test_transfer"

# Verbose output
mameyforge test -v
```

## Integration Tests

Place integration tests in `tests/`:

```
my-contract/
├── contracts/
│   └── src/
│       └── lib.rs
├── tests/
│   └── integration.rs      ← integration tests
└── Cargo.toml
```

Example `tests/integration.rs`:

```rust
use my_contract::*;

#[test]
fn test_full_token_lifecycle() {
    // Init
    let result = init("MyToken".into(), "MTK".into(), 18, 1_000_000);
    assert!(result.is_ok());

    // Check initial balance
    assert_eq!(balance_of("deployer".into()), 1_000_000);
    assert_eq!(total_supply(), 1_000_000);

    // Transfer
    let result = transfer("alice".into(), 500);
    assert!(result.is_ok());
    assert_eq!(balance_of("alice".into()), 500);

    // Burn
    let result = burn(100);
    assert!(result.is_ok());
    assert_eq!(total_supply(), 999_900);
}
```

## Local Devnet Testing

For end-to-end testing against a real node:

```bash
# 1. Start local devnet
mameyforge devnet start

# 2. Build optimized WASM
mameyforge build --release

# 3. Deploy
mameyforge deploy --contract my_contract --network devnet \
  --args '{"name":"TestToken","symbol":"TST","decimals":18,"initial_supply":1000000}'

# 4. Interact
mameyforge call --contract <ADDRESS> --function transfer \
  --args '{"to":"0x1234...","amount":100}'

mameyforge query --contract <ADDRESS> --function balance_of \
  --args '{"account":"0x1234..."}'

# 5. Check devnet logs
mameyforge devnet logs

# 6. Stop when done
mameyforge devnet stop
```

## Test Helpers

### Mock Host Functions

For unit tests, `mamey-contracts-shared` provides mock implementations of host functions:

```rust
#[cfg(test)]
mod tests {
    use mamey_contracts_shared::test_helpers::{MockContext, set_caller};

    #[test]
    fn test_owner_only() {
        set_caller("alice");
        let _ = init();

        // Try calling as different user
        set_caller("bob");
        let result = execute_set("key".into(), "value".into());
        assert!(result.is_err());
        assert!(result.unwrap_err().contains("Unauthorized"));
    }
}
```

### Storage Reset

Between tests, storage is automatically isolated. Each `#[test]` function runs in its own context.

## CI Integration

Add to your CI pipeline:

```yaml
# GitHub Actions
- name: Run contract tests
  run: |
    mameyforge test
    mameyforge build --release
```

Or use the MameyForge CI check script:

```bash
./ci/check.sh
```

## Troubleshooting

| Issue | Fix |
|-------|-----|
| `wasm32-unknown-unknown` target missing | `rustup target add wasm32-unknown-unknown` |
| Tests pass but devnet deploy fails | Ensure `--release` build; debug WASM may exceed size limits |
| Storage not persisting between calls | Each unit test is isolated; use integration tests for multi-step flows |
| Host function not found in tests | Ensure `mamey-contracts-shared` test features are enabled |

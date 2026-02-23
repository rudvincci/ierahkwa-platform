# Counter Example

A simple counter smart contract for MameyFutureNode demonstrating the basics of contract development with MameyForge.

## What It Does

- **Increment** — Add 1 to the counter (anyone)
- **Decrement** — Subtract 1 from the counter (anyone, fails at zero)
- **Reset** — Set counter to 0 (owner only)
- **Set** — Set counter to a specific value (owner only)
- **Get** — Read the current counter value

## Concepts Demonstrated

- Contract initialization with ownership
- State-changing vs read-only functions
- Owner-gated access control
- Safe math (overflow/underflow protection)
- Event emission for off-chain indexing
- `#[wasm_bindgen]` for WASM exports

## Build & Test

```bash
mameyforge build
mameyforge test
```

## Deploy & Interact

```bash
# Start local devnet
mameyforge devnet start

# Deploy
mameyforge deploy --contract counter-example --network devnet

# Increment
mameyforge call --contract <ADDR> --function increment

# Get current value
mameyforge query --contract <ADDR> --function get

# Reset (owner only)
mameyforge call --contract <ADDR> --function reset
```

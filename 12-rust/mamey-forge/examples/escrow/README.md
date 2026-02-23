# Escrow Example

A two-party escrow contract for MameyFutureNode demonstrating multi-role access control and state-machine patterns.

## What It Does

Three roles:
- **Buyer** — Deposits funds
- **Seller** — Receives funds on release
- **Arbiter** — Decides whether to release to seller or refund to buyer

State machine: `Awaiting → Funded → Released | Refunded`

## Concepts Demonstrated

- Multi-role access control
- State machine pattern with explicit status codes
- Input validation (addresses, amounts)
- Safe math (overflow protection on deposits)
- Event emission for off-chain tracking
- Query functions for reading contract state

## Build & Test

```bash
mameyforge build
mameyforge test
```

## Deploy & Interact

```bash
# Start local devnet
mameyforge devnet start

# Deploy (arbiter is the deployer)
mameyforge deploy --contract escrow-example --network devnet \
  --args '{"buyer":"<BUYER_ADDR>","seller":"<SELLER_ADDR>"}'

# Buyer deposits
mameyforge call --contract <ADDR> --function deposit --args '{"amount":1000}'

# Check status
mameyforge query --contract <ADDR> --function status

# Arbiter releases to seller
mameyforge call --contract <ADDR> --function release

# — or — Arbiter refunds to buyer
mameyforge call --contract <ADDR> --function refund
```

## Flow

```
┌──────────┐   deposit()   ┌──────────┐
│ Awaiting │──────────────→│  Funded  │
└──────────┘  (buyer only) └────┬─────┘
                                │
                    ┌───────────┴───────────┐
                    │                       │
              release()                refund()
           (arbiter only)           (arbiter only)
                    │                       │
                    ▼                       ▼
             ┌──────────┐           ┌──────────┐
             │ Released │           │ Refunded │
             └──────────┘           └──────────┘
```

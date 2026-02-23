# ERC-20 Token Example

A standard fungible token for MameyFutureNode implementing the ERC-20 pattern.

## Features

- **Mint / Burn** — Supply management (minter role)
- **Transfer / TransferFrom / Approve** — Standard token operations
- **Pause / Unpause** — Emergency governance controls
- **balance_of / allowance / total_supply** — Read-only queries

## Deploy

```bash
mameyforge build --release
mameyforge deploy --network devnet \
  --args '{"name":"MameyUSD","symbol":"MUSD","decimals":6,"initial_supply":1000000}'
```

## Interact

```bash
mameyforge query --contract <ADDR> --function balance_of --args '{"account":"0x..."}'
mameyforge call  --contract <ADDR> --function transfer   --args '{"to":"0x...","amount":100}'
mameyforge call  --contract <ADDR> --function mint       --args '{"to":"0x...","amount":500}'
```

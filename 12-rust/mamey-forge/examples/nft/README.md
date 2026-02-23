# NFT Example

A basic non-fungible token (NFT) contract for MameyFutureNode.

## Features

- **Mint** — Create unique tokens with metadata URIs (minter role)
- **Transfer / Approve** — Standard ownership transfers
- **Burn** — Destroy tokens
- **owner_of / token_uri / balance_of** — Read-only queries

## Deploy

```bash
mameyforge build --release
mameyforge deploy --network devnet \
  --args '{"name":"MameyNFT","symbol":"MNFT"}'
```

## Interact

```bash
# Mint a new NFT
mameyforge call --contract <ADDR> --function mint \
  --args '{"to":"0x...","uri":"ipfs://Qm..."}'

# Check ownership
mameyforge query --contract <ADDR> --function owner_of --args '{"token_id":1}'

# Transfer
mameyforge call --contract <ADDR> --function transfer \
  --args '{"to":"0x...","token_id":1}'
```

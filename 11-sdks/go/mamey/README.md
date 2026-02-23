# Mamey Go SDK

Go SDK for interacting with the MameyNode blockchain and the Ierahkwa Sovereign Platform services.

## Installation

```bash
go get github.com/rudvincci/ierahkwa-platform/11-sdks/go/mamey
```

## Quick Start

```go
package main

import (
    "fmt"
    "log"

    "github.com/rudvincci/ierahkwa-platform/11-sdks/go/mamey"
)

func main() {
    client := mamey.NewClient("http://localhost:8545")

    // Check node health
    healthy, err := client.Health()
    if err != nil {
        log.Fatal(err)
    }
    fmt.Println("Node healthy:", healthy)

    // Get chain info
    info, err := client.GetChainInfo()
    if err != nil {
        log.Fatal(err)
    }
    fmt.Printf("Chain: %s, Block Height: %d\n", info.Name, info.BlockHeight)
}
```

## API Reference

### Client

Create a new client connected to a MameyNode instance:

```go
client := mamey.NewClient("http://localhost:8545")
```

The client provides default URLs for Identity (`localhost:5001`), Treasury (`localhost:5003`), and ZKP (`localhost:5002`) services. These can be overridden by setting the corresponding fields directly:

```go
client.IdentityURL = "http://identity.example.com:5001"
client.TreasuryURL = "http://treasury.example.com:5003"
client.ZKPURL = "http://zkp.example.com:5002"
```

### Chain Operations

```go
// Get blockchain information
info, err := client.GetChainInfo()

// Get latest block number
blockNum, err := client.GetBlockNumber()

// Get block by number or hash
block, err := client.GetBlock(42)
block, err := client.GetBlock("0xabc123...")

// Get latest block
block, err := client.GetLatestBlock()

// Check node health
healthy, err := client.Health()
```

### Wallet Operations

```go
// Create a new wallet
wallet, err := client.CreateWallet()
fmt.Println("Address:", wallet.Address)
fmt.Println("Mnemonic:", wallet.Mnemonic)

// Get wallet details
w, err := client.GetWallet("0x1234...")
fmt.Println("Balance:", w.Balance)
fmt.Println("Nonce:", w.Nonce)

// Get wallet balance
balance, err := client.GetWalletBalance("0x1234...")
```

### Token Operations

```go
// List all tokens
tokens, err := client.GetTokens()

// Create a new token
symbol, err := client.CreateToken("TAINO", "Taino Token", 18, "1000000", "0xOwner...")

// Get token info by contract address
info, err := client.GetTokenInfo("0xContract...")

// Get token balance for a wallet
balance, err := client.GetTokenBalance("0xContract...", "0xWallet...")

// Transfer tokens
transfer, err := client.TransferToken(
    "0xFrom...",
    "0xTo...",
    "0xContract...",
    big.NewInt(1000),
    "privateKey",
)
```

### Transaction Operations

```go
// Send a simple transaction
txHash, err := client.SendTransaction("0xFrom...", "0xTo...", "1000", nil)

// Send a detailed transaction with receipt
receipt, err := client.SendDetailedTransaction(&mamey.TransactionRequest{
    From:       "0xFrom...",
    To:         "0xTo...",
    Value:      big.NewInt(1000),
    PrivateKey: "privateKey",
})

// Get transaction details
tx, err := client.GetDetailedTransaction("0xTxHash...")

// Get transaction receipt
receipt, err := client.GetTransactionReceipt("0xTxHash...")

// Get pending transactions
pending, err := client.GetPendingTransactions()
```

### Error Handling

The SDK provides typed errors for common failure scenarios:

```go
wallet, err := client.GetWallet("0xinvalid")
if mamey.IsNotFound(err) {
    fmt.Println("Wallet not found")
}
if mamey.IsUnauthorized(err) {
    fmt.Println("Authentication required")
}
```

Predefined error types:

| Error | Code | Description |
|-------|------|-------------|
| `ErrNotFound` | 404 | Resource not found |
| `ErrUnauthorized` | 401 | Authentication required |
| `ErrInsufficientFunds` | 400 | Insufficient balance |
| `ErrInvalidAddress` | 400 | Invalid address format |
| `ErrTransactionFailed` | 500 | Transaction execution failed |
| `ErrConnectionFailed` | 503 | Service unavailable |
| `ErrRateLimited` | 429 | Rate limit exceeded |

## Testing

```bash
cd 11-sdks/go/mamey
go test ./...
```

## Requirements

- Go 1.22 or later
- Running MameyNode instance (default: `http://localhost:8545`)

## License

MIT - See [LICENSE](../../../LICENSE) for details.

---

*Skennen -- Peace*

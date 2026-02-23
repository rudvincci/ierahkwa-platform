# Mamey.Blockchain.Banking

.NET client library for interacting with MameyNode banking operations via gRPC.

## Features

- Account management (create, get info, get balance)
- Transaction operations (send, get status, list)
- Multi-currency support
- Transaction status tracking
- Integration with blockchain ledger

## Usage

```csharp
using Mamey.Blockchain.Banking;

// Create client
var options = new MameyBankingClientOptions
{
    Host = "localhost",
    Port = 50052
};
var client = new MameyBankingClient(options);

// Create account
var createResult = await client.CreateAccountAsync("account123", "USD");
if (createResult.Success)
{
    Console.WriteLine($"Account created: {createResult.AccountId}");
    Console.WriteLine($"Blockchain account: {createResult.BlockchainAccount}");
}

// Get account balance
var balance = await client.GetBalanceAsync("account123");
if (balance != null)
{
    Console.WriteLine($"Balance: {balance.Balance} {balance.Currency}");
}

// Send transaction
var sendResult = await client.SendTransactionAsync(
    fromAccount: "account123",
    toAccount: "account456",
    amount: "1000",
    currency: "USD"
);
if (sendResult.Status == TransactionStatus.Confirmed)
{
    Console.WriteLine($"Transaction confirmed: {sendResult.TransactionId}");
    Console.WriteLine($"Block hash: {sendResult.BlockHash}");
}

// Get transaction status
var status = await client.GetTransactionStatusAsync(sendResult.TransactionId);
if (status != null)
{
    Console.WriteLine($"Status: {status.Status}");
    Console.WriteLine($"Amount: {status.Amount} {status.Currency}");
}

// List transactions
var transactions = await client.ListTransactionsAsync(
    accountId: "account123",
    limit: 50,
    offset: 0
);
foreach (var tx in transactions.Transactions)
{
    Console.WriteLine($"{tx.TransactionId}: {tx.Amount} {tx.Currency} - {tx.Status}");
}
```

## Dependency Injection

```csharp
services.AddMameyBankingClient(options =>
{
    options.Host = "localhost";
    options.Port = 50052;
});

// Inject in your service
public class MyService
{
    private readonly MameyBankingClient _client;
    
    public MyService(MameyBankingClient client)
    {
        _client = client;
    }
}
```

## Transaction Status

- `Pending` - Transaction is queued
- `Processing` - Transaction is being processed
- `Confirmed` - Transaction is confirmed on the blockchain
- `Failed` - Transaction failed

## Account Status

- `Active` - Account is active and can transact
- `Suspended` - Account is temporarily suspended
- `Closed` - Account is closed



























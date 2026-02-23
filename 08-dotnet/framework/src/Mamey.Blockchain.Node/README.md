# Mamey.Blockchain.Node

.NET client library for interacting with MameyNode blockchain via gRPC.

## Features

- Node information retrieval
- Block operations (get, publish)
- Account operations (get, history)
- Peer management
- Confirmation height tracking

## Usage

```csharp
using Mamey.Blockchain.Node;

// Create client
var options = new MameyNodeClientOptions
{
    Host = "localhost",
    Port = 50051
};
var client = new MameyNodeClient(options);

// Get node info
var nodeInfo = await client.GetNodeInfoAsync();
Console.WriteLine($"Node Version: {nodeInfo.Version}");

// Get account
var account = await client.GetAccountAsync("account_hex_string");
if (account != null)
{
    Console.WriteLine($"Balance: {account.Balance}");
}

// Publish block
var blockData = /* your block data */;
var result = await client.PublishBlockAsync(blockData);
if (result.Accepted)
{
    Console.WriteLine($"Block published: {result.BlockHash}");
}
```

## Dependency Injection

```csharp
services.AddMameyNodeClient(options =>
{
    options.Host = "localhost";
    options.Port = 50051;
});

// Inject in your service
public class MyService
{
    private readonly MameyNodeClient _client;
    
    public MyService(MameyNodeClient client)
    {
        _client = client;
    }
}
```



























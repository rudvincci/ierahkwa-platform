# Mamey.Blockchain.Swap

.NET library for DEX swap operations, AMM calculations, and swap routing.

## Features

- Swap operations (quote, execute)
- Constant Product AMM calculations
- Swap routing (direct and multi-hop)
- Liquidity pool operations
- Price impact calculations

## Usage

```csharp
using Mamey.Blockchain.Swap;
using Mamey.Blockchain.Node;

// Create node client
var nodeClient = new MameyNodeClient(new MameyNodeClientOptions
{
    Host = "localhost",
    Port = 50051
});

// Create swap client
var swapClient = new SwapClient(nodeClient);

// Get swap quote
var quote = await swapClient.GetSwapQuoteAsync(
    "tokenA",
    "tokenB",
    1000m
);
Console.WriteLine($"Expected output: {quote.AmountOut}");

// Execute swap
var request = new SwapRequest
{
    TokenIn = "tokenA",
    TokenOut = "tokenB",
    AmountIn = 1000m,
    MinAmountOut = 990m,
    SlippageTolerance = 0.01m
};
var result = await swapClient.ExecuteSwapAsync(request);
if (result.Success)
{
    Console.WriteLine($"Swap completed: {result.TransactionHash}");
}

// AMM calculations
var amm = new ConstantProductAMM(feeRate: 0.003m);
var output = amm.CalculateOutput(
    amountIn: 1000m,
    reserveIn: 1000000m,
    reserveOut: 1000000m
);
Console.WriteLine($"Output: {output}");
```

## Dependency Injection

```csharp
services.AddMameyNodeClient(options =>
{
    options.Host = "localhost";
    options.Port = 50051;
});
services.AddMameyBlockchainSwap();

// Inject in your service
public class MyService
{
    private readonly SwapClient _swapClient;
    
    public MyService(SwapClient swapClient)
    {
        _swapClient = swapClient;
    }
}
```



























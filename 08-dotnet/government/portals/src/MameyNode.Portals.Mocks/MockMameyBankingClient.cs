using Grpc.Core;
using Mamey.Blockchain.Banking;
using Mamey.Banking;
using Bogus;

namespace MameyNode.Portals.Mocks;

public class MockMameyBankingClient : IMameyBankingClient
{
    public BankingService.BankingServiceClient Client { get; }

    public MockMameyBankingClient()
    {
        Client = new MockBankingServiceClient();
    }
}

public class MockBankingServiceClient : BankingService.BankingServiceClient
{
    private readonly Faker _faker = new();

    // Add overrides for banking methods as needed
    // For now, we'll just provide the mock structure
    
    private AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response)
    {
        return new AsyncUnaryCall<T>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { }
        );
    }
}






using Grpc.Core;
using Mamey.Blockchain.Government;
using Mamey.Government;
using Bogus;

namespace MameyNode.Portals.Mocks;

public class MockMameyGovernmentClient : IMameyGovernmentClient
{
    public GovernmentService.GovernmentServiceClient Client { get; }

    public MockMameyGovernmentClient()
    {
        Client = new MockGovernmentServiceClient();
    }
}

public class MockGovernmentServiceClient : GovernmentService.GovernmentServiceClient
{
    private readonly Faker _faker = new();

    // Add overrides for government methods as needed
    
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






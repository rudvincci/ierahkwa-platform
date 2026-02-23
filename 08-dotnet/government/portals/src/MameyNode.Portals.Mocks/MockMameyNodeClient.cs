using Grpc.Core;
using Mamey.Blockchain.Node;
using Mamey.Node;
using Bogus;

namespace MameyNode.Portals.Mocks;

public class MockMameyNodeClient : IMameyNodeClient
{
    public NodeService.NodeServiceClient Client { get; }

    public MockMameyNodeClient()
    {
        Client = new MockNodeServiceClient();
    }
}

public class MockNodeServiceClient : NodeService.NodeServiceClient
{
    private readonly Faker _faker = new();

    // NodeInfoRequest and NodeInfoResponse are generated in Mamey.Node namespace
    // from node.proto in Mamey.Blockchain.Node project
    
    public override AsyncUnaryCall<Mamey.Node.GetNodeInfoResponse> GetNodeInfoAsync(Mamey.Node.GetNodeInfoRequest request, CallOptions options)
    {
        var response = new Mamey.Node.GetNodeInfoResponse
        {
            Version = "1.0.0-mock",
            NodeId = Guid.NewGuid().ToString(),
            BlockCount = (ulong)_faker.Random.Long(1000, 1000000),
            AccountCount = (ulong)_faker.Random.Long(100, 10000),
            PeerCount = (uint)_faker.Random.Int(5, 50)
        };
        
        return CreateAsyncUnaryCall(response);
    }

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

using Grpc.Core;
using Mamey.Blockchain.Node;

namespace MameyNode.UI.Services;

/// <summary>
/// Implementation of IMetadataService for Blazor Server
/// Uses session/request-scoped storage for metadata
/// </summary>
public class MetadataService : IMetadataService
{
    private readonly IConfiguration _configuration;
    private string? _correlationId;
    private string? _bearerToken;
    private string? _institutionId;
    private List<Credential>? _credentialChain;

    public MetadataService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetCorrelationId()
    {
        if (string.IsNullOrEmpty(_correlationId))
        {
            _correlationId = Guid.NewGuid().ToString();
        }
        return _correlationId;
    }

    public string? GetBearerToken()
    {
        if (string.IsNullOrEmpty(_bearerToken))
        {
            var authConfig = _configuration.GetSection("MameyNode:Authentication");
            var authType = authConfig["Type"] ?? "None";
            
            if (authType == "ApiKey" || authType == "Jwt")
            {
                _bearerToken = authConfig["ApiKey"] ?? authConfig["Token"];
            }
        }
        return _bearerToken;
    }

    public string? GetInstitutionId()
    {
        if (string.IsNullOrEmpty(_institutionId))
        {
            _institutionId = _configuration["MameyNode:DefaultInstitutionId"];
        }
        return _institutionId;
    }

    public List<Credential>? GetCredentialChain()
    {
        return _credentialChain;
    }

    public Metadata GetMetadata(string? correlationId = null)
    {
        var provider = new CredentialProvider
        {
            InstitutionId = GetInstitutionId(),
            CredentialChain = GetCredentialChain(),
            CorrelationIdGenerator = () => GetCorrelationId()
        };

        var metadata = provider.GetMetadata(correlationId);

        // Add bearer token if available
        var bearerToken = GetBearerToken();
        if (!string.IsNullOrEmpty(bearerToken))
        {
            metadata.Add("authorization", $"Bearer {bearerToken}");
        }

        return metadata;
    }

    public void SetInstitutionId(string? institutionId)
    {
        _institutionId = institutionId;
    }

    public void SetCredentialChain(List<Credential>? credentialChain)
    {
        _credentialChain = credentialChain;
    }

    public void SetBearerToken(string? bearerToken)
    {
        _bearerToken = bearerToken;
    }
}

namespace Mamey.Auth.Decentralized.Persistence.Read;

/// <summary>
/// MongoDB options for DID persistence
/// </summary>
public class MongoDidOptions
{
    /// <summary>
    /// The connection string for MongoDB
    /// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// The database name
    /// </summary>
    public string DatabaseName { get; set; } = "mamey_did";

    /// <summary>
    /// The collection name for DID Documents
    /// </summary>
    public string DidDocumentsCollectionName { get; set; } = "did_documents";

    /// <summary>
    /// The collection name for Verification Methods
    /// </summary>
    public string VerificationMethodsCollectionName { get; set; } = "verification_methods";

    /// <summary>
    /// The collection name for Service Endpoints
    /// </summary>
    public string ServiceEndpointsCollectionName { get; set; } = "service_endpoints";

    /// <summary>
    /// The collection name for Proofs
    /// </summary>
    public string ProofsCollectionName { get; set; } = "proofs";

    /// <summary>
    /// Whether to create indexes automatically
    /// </summary>
    public bool CreateIndexes { get; set; } = true;

    /// <summary>
    /// The timeout for operations in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

using MongoDB.Driver;
using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read;

/// <summary>
/// MongoDB context for DID read operations
/// </summary>
public class MongoDidDbContext
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDidDbContext> _logger;

    public MongoDidDbContext(IMongoDatabase database, ILogger<MongoDidDbContext> logger)
    {
        _database = database;
        _logger = logger;
    }

    /// <summary>
    /// DID Documents collection
    /// </summary>
    public IMongoCollection<DidDocumentReadModel> DidDocuments => 
        _database.GetCollection<DidDocumentReadModel>("did_documents");

    /// <summary>
    /// Verification Methods collection
    /// </summary>
    public IMongoCollection<VerificationMethodReadModel> VerificationMethods => 
        _database.GetCollection<VerificationMethodReadModel>("verification_methods");

    /// <summary>
    /// Service Endpoints collection
    /// </summary>
    public IMongoCollection<ServiceEndpointReadModel> ServiceEndpoints => 
        _database.GetCollection<ServiceEndpointReadModel>("service_endpoints");

    /// <summary>
    /// Proofs collection
    /// </summary>
    public IMongoCollection<ProofReadModel> Proofs => 
        _database.GetCollection<ProofReadModel>("proofs");

    /// <summary>
    /// Creates indexes for optimal query performance
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    public async Task CreateIndexesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating MongoDB indexes for DID collections");

            // DID Documents indexes
            var didDocumentsCollection = DidDocuments;
            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.Did),
                    new CreateIndexOptions { Unique = true, Name = "idx_did_unique" }
                ),
                cancellationToken: cancellationToken
            );

            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.Method),
                    new CreateIndexOptions { Name = "idx_did_method" }
                ),
                cancellationToken: cancellationToken
            );

            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.Controller),
                    new CreateIndexOptions { Name = "idx_did_controller" }
                ),
                cancellationToken: cancellationToken
            );

            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.IsActive),
                    new CreateIndexOptions { Name = "idx_did_active" }
                ),
                cancellationToken: cancellationToken
            );

            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.CreatedAt),
                    new CreateIndexOptions { Name = "idx_did_created_at" }
                ),
                cancellationToken: cancellationToken
            );

            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.UpdatedAt),
                    new CreateIndexOptions { Name = "idx_did_updated_at" }
                ),
                cancellationToken: cancellationToken
            );

            await didDocumentsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DidDocumentReadModel>(
                    Builders<DidDocumentReadModel>.IndexKeys.Ascending(d => d.Tags),
                    new CreateIndexOptions { Name = "idx_did_tags" }
                ),
                cancellationToken: cancellationToken
            );

            // Verification Methods indexes
            var verificationMethodsCollection = VerificationMethods;
            await verificationMethodsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<VerificationMethodReadModel>(
                    Builders<VerificationMethodReadModel>.IndexKeys.Ascending(v => v.VerificationMethodId),
                    new CreateIndexOptions { Unique = true, Name = "idx_vm_id_unique" }
                ),
                cancellationToken: cancellationToken
            );

            await verificationMethodsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<VerificationMethodReadModel>(
                    Builders<VerificationMethodReadModel>.IndexKeys.Ascending(v => v.Type),
                    new CreateIndexOptions { Name = "idx_vm_type" }
                ),
                cancellationToken: cancellationToken
            );

            await verificationMethodsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<VerificationMethodReadModel>(
                    Builders<VerificationMethodReadModel>.IndexKeys.Ascending(v => v.Algorithm),
                    new CreateIndexOptions { Name = "idx_vm_algorithm" }
                ),
                cancellationToken: cancellationToken
            );

            await verificationMethodsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<VerificationMethodReadModel>(
                    Builders<VerificationMethodReadModel>.IndexKeys.Ascending(v => v.Curve),
                    new CreateIndexOptions { Name = "idx_vm_curve" }
                ),
                cancellationToken: cancellationToken
            );

            await verificationMethodsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<VerificationMethodReadModel>(
                    Builders<VerificationMethodReadModel>.IndexKeys.Ascending(v => v.IsActive),
                    new CreateIndexOptions { Name = "idx_vm_active" }
                ),
                cancellationToken: cancellationToken
            );

            // Service Endpoints indexes
            var serviceEndpointsCollection = ServiceEndpoints;
            await serviceEndpointsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<ServiceEndpointReadModel>(
                    Builders<ServiceEndpointReadModel>.IndexKeys.Ascending(s => s.ServiceEndpointId),
                    new CreateIndexOptions { Unique = true, Name = "idx_se_id_unique" }
                ),
                cancellationToken: cancellationToken
            );

            await serviceEndpointsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<ServiceEndpointReadModel>(
                    Builders<ServiceEndpointReadModel>.IndexKeys.Ascending(s => s.Type),
                    new CreateIndexOptions { Name = "idx_se_type" }
                ),
                cancellationToken: cancellationToken
            );

            await serviceEndpointsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<ServiceEndpointReadModel>(
                    Builders<ServiceEndpointReadModel>.IndexKeys.Ascending(s => s.ServiceEndpoint),
                    new CreateIndexOptions { Name = "idx_se_endpoint" }
                ),
                cancellationToken: cancellationToken
            );

            await serviceEndpointsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<ServiceEndpointReadModel>(
                    Builders<ServiceEndpointReadModel>.IndexKeys.Ascending(s => s.IsActive),
                    new CreateIndexOptions { Name = "idx_se_active" }
                ),
                cancellationToken: cancellationToken
            );

            await serviceEndpointsCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<ServiceEndpointReadModel>(
                    Builders<ServiceEndpointReadModel>.IndexKeys.Ascending(s => s.Priority),
                    new CreateIndexOptions { Name = "idx_se_priority" }
                ),
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Successfully created MongoDB indexes for DID collections");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MongoDB indexes for DID collections");
            throw;
        }
    }

    /// <summary>
    /// Drops all indexes (use with caution)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    public async Task DropIndexesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("Dropping all MongoDB indexes for DID collections");

            await DidDocuments.Indexes.DropAllAsync(cancellationToken);
            await VerificationMethods.Indexes.DropAllAsync(cancellationToken);
            await ServiceEndpoints.Indexes.DropAllAsync(cancellationToken);
            await Proofs.Indexes.DropAllAsync(cancellationToken);

            _logger.LogWarning("Successfully dropped all MongoDB indexes for DID collections");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dropping MongoDB indexes for DID collections");
            throw;
        }
    }
}

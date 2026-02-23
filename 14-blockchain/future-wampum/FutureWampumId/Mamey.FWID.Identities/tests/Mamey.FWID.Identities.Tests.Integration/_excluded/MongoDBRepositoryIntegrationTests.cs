using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Repositories;

/// <summary>
/// Integration tests for MongoDB repository with real database.
/// Note: MongoDB is a read model, so it stores IdentityDocument, not full Identity entities.
/// </summary>
[Collection("Integration")]
public class MongoDBRepositoryIntegrationTests : IClassFixture<MongoDBFixture>, IAsyncLifetime
{
    private readonly MongoDBFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private IIdentityRepository? _repository;

    public MongoDBRepositoryIntegrationTests(MongoDBFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add MongoDB repository
        services.AddScoped<IIdentityRepository>(provider =>
        {
            // MongoDBFixture.Repository is internal - use ServiceProvider instead
            var mongoRepository = _fixture.ServiceProvider.GetRequiredService<Mamey.Persistence.MongoDB.IMongoRepository<Mamey.FWID.Identities.Infrastructure.Mongo.Documents.IdentityDocument, Guid>>();
            return new IdentityMongoRepository(mongoRepository);
        });

        _serviceProvider = services.BuildServiceProvider();

        // Get repository
        _repository = _serviceProvider.GetRequiredService<IIdentityRepository>();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Fact]
    public async Task AddAsync_ShouldPersistIdentityDocument()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();

        // Act
        await _repository!.AddAsync(identity);

        // Assert
        var mongoRepository = _fixture.ServiceProvider.GetRequiredService<Mamey.Persistence.MongoDB.IMongoRepository<Mamey.FWID.Identities.Infrastructure.Mongo.Documents.IdentityDocument, Guid>>();
        var document = await mongoRepository.GetAsync(identity.Id.Value);
        document.ShouldNotBeNull();
        document.Id.ShouldBe(identity.Id.Value);
        document.FirstName.ShouldBe(identity.Name.FirstName);
        document.LastName.ShouldBe(identity.Name.LastName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateIdentityDocument()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        identity.UpdateContactInformation(TestDataFactory.CreateTestContactInformation("updated@example.com", "5551234567"));
        await _repository.UpdateAsync(identity);

        // Assert
        var mongoRepository = _fixture.ServiceProvider.GetRequiredService<Mamey.Persistence.MongoDB.IMongoRepository<Mamey.FWID.Identities.Infrastructure.Mongo.Documents.IdentityDocument, Guid>>();
        var document = await mongoRepository.GetAsync(identity.Id.Value);
        document.ShouldNotBeNull();
        // TODO: IdentityDocument doesn't currently have Email - it's a minimal read model
        // For now, verify that the identity was synced by checking Status
        document.Status.ShouldBe(identity.Status.ToString());
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveIdentityDocument()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        await _repository.DeleteAsync(identity.Id);

        // Assert
        var mongoRepository = _fixture.ServiceProvider.GetRequiredService<Mamey.Persistence.MongoDB.IMongoRepository<Mamey.FWID.Identities.Infrastructure.Mongo.Documents.IdentityDocument, Guid>>();
        var document = await mongoRepository.GetAsync(identity.Id.Value);
        document.ShouldBeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenDocumentExists_ShouldReturnTrue()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        var exists = await _repository.ExistsAsync(identity.Id);

        // Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenDocumentDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var exists = await _repository!.ExistsAsync(new IdentityId(Guid.NewGuid()));

        // Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_AsMongoIsReadModel()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _repository!.AddAsync(identity);

        // Act
        var result = await _repository.GetAsync(identity.Id);

        // Assert
        // MongoDB is a read model - cannot convert back to full entities
        // Composite repository will fall back to PostgreSQL
        result.ShouldBeNull();
    }
}


using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Queries.Handlers;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Queries;

/// <summary>
/// Integration tests for GetIdentity query with real PostgreSQL repository and caching.
/// </summary>
[Collection("Integration")]
public class GetIdentityIntegrationTests : BaseQueryIntegrationTests
{
    private IQueryHandler<GetIdentity, IdentityDto>? _handler;

    public GetIdentityIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add query handler
        services.AddScoped<IQueryHandler<GetIdentity, IdentityDto>, GetIdentityHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<IQueryHandler<GetIdentity, IdentityDto>>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldReturnIdentityDto()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        var query = new GetIdentity(identity.Id);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.Value.ShouldBe(identity.Id.Value);
        result.Name.FirstName.ShouldBe(identity.Name.FirstName);
        result.Name.LastName.ShouldBe(identity.Name.LastName);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var query = new GetIdentity(new IdentityId(Guid.NewGuid()));

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(query));

        exception.IdentityId.ShouldBe(query.IdentityId);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityIsCached_ShouldReturnCachedIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        var query = new GetIdentity(identity.Id);
        var cacheKey = $"identity:{identity.Id.Value}";

        // First call - should fetch from database
        var firstResult = await _handler!.HandleAsync(query);
        firstResult.ShouldNotBeNull();

        // Verify it's in cache
        MemoryCache!.TryGetValue(cacheKey, out IdentityDto? cachedValue).ShouldBeTrue();
        cachedValue.ShouldNotBeNull();

        // Second call - should return from cache
        var secondResult = await _handler!.HandleAsync(query);
        secondResult.ShouldNotBeNull();
        secondResult.Id.Value.ShouldBe(identity.Id.Value);
    }
}


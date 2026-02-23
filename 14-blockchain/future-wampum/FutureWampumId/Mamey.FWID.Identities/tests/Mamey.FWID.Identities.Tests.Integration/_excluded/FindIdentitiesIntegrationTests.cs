using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Queries.Handlers;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Queries;

/// <summary>
/// Integration tests for FindIdentities query with real PostgreSQL repository and filtering.
/// </summary>
[Collection("Integration")]
public class FindIdentitiesIntegrationTests : BaseQueryIntegrationTests
{
    private IQueryHandler<FindIdentities, List<IdentityDto>>? _handler;

    public FindIdentitiesIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add query handler
        services.AddScoped<IQueryHandler<FindIdentities, List<IdentityDto>>, FindIdentitiesHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<IQueryHandler<FindIdentities, List<IdentityDto>>>();
    }

    [Fact]
    public async Task HandleAsync_WhenNoFilters_ShouldReturnAllIdentities()
    {
        // Arrange
        var identities = TestDataFactory.CreateTestIdentities(5);
        foreach (var identity in identities)
        {
            await Repository!.AddAsync(identity);
        }

        var query = new FindIdentities(Zone: null, Status: null);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task HandleAsync_WhenFilteredByZone_ShouldReturnOnlyMatchingIdentities()
    {
        // Arrange
        var identity1 = TestDataFactory.CreateTestIdentity(zone: "zone-001");
        var identity2 = TestDataFactory.CreateTestIdentity(zone: "zone-002");
        var identity3 = TestDataFactory.CreateTestIdentity(zone: "zone-001");

        await Repository!.AddAsync(identity1);
        await Repository!.AddAsync(identity2);
        await Repository!.AddAsync(identity3);

        var query = new FindIdentities(Zone: "zone-001", Status: null);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.All(i => i.Zone == "zone-001").ShouldBeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenFilteredByStatus_ShouldReturnOnlyMatchingIdentities()
    {
        // Arrange
        var identity1 = TestDataFactory.CreateTestIdentity();
        var identity2 = TestDataFactory.CreateTestIdentity();
        var identity3 = TestDataFactory.CreateTestIdentity();

        await Repository!.AddAsync(identity1);
        await Repository!.AddAsync(identity2);
        await Repository!.AddAsync(identity3);

        // Verify all identities
        identity1.VerifyBiometric(identity1.BiometricData, 0.95);
        identity2.VerifyBiometric(identity2.BiometricData, 0.95);
        identity3.VerifyBiometric(identity3.BiometricData, 0.95);

        await Repository.UpdateAsync(identity1);
        await Repository.UpdateAsync(identity2);
        await Repository.UpdateAsync(identity3);

        var query = new FindIdentities(Zone: null, Status: IdentityStatus.Verified);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(3);
        result.All(i => i.Status == IdentityStatus.Verified).ShouldBeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenFilteredByZoneAndStatus_ShouldReturnOnlyMatchingIdentities()
    {
        // Arrange
        var identity1 = TestDataFactory.CreateTestIdentity(zone: "zone-001");
        var identity2 = TestDataFactory.CreateTestIdentity(zone: "zone-002");
        var identity3 = TestDataFactory.CreateTestIdentity(zone: "zone-001");

        await Repository!.AddAsync(identity1);
        await Repository!.AddAsync(identity2);
        await Repository!.AddAsync(identity3);

        // Verify identities in zone-001
        identity1.VerifyBiometric(identity1.BiometricData, 0.95);
        identity3.VerifyBiometric(identity3.BiometricData, 0.95);

        await Repository.UpdateAsync(identity1);
        await Repository.UpdateAsync(identity2);
        await Repository.UpdateAsync(identity3);

        var query = new FindIdentities(Zone: "zone-001", Status: IdentityStatus.Verified);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.All(i => i.Zone == "zone-001" && i.Status == IdentityStatus.Verified).ShouldBeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenNoMatchingIdentities_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new FindIdentities(Zone: "non-existent-zone", Status: null);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }
}


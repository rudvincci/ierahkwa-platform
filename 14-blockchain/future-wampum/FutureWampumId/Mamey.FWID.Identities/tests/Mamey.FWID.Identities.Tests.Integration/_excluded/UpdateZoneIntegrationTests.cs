using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Commands;

/// <summary>
/// Integration tests for UpdateZone command with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class UpdateZoneIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<UpdateZone>? _handler;

    public UpdateZoneIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add command handler
        services.AddScoped<ICommandHandler<UpdateZone>, UpdateZoneHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<UpdateZone>>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldUpdateZone()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity(zone: "zone-001");
        await Repository!.AddAsync(identity);

        var command = new UpdateZone
        {
            IdentityId = identity.Id,
            Zone = "zone-002"
        };

        // Act
        await _handler!.HandleAsync(command);

        // Assert
        var updatedIdentity = await Repository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.Zone.ShouldBe("zone-002");
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateZone
        {
            IdentityId = new IdentityId(Guid.NewGuid()),
            Zone = "zone-002"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.ShouldBe(command.IdentityId);
    }
}


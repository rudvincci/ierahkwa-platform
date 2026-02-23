using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Commands;

/// <summary>
/// Integration tests for RevokeIdentity command with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class RevokeIdentityIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<RevokeIdentity>? _handler;

    public RevokeIdentityIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add command handler
        services.AddScoped<ICommandHandler<RevokeIdentity>, RevokeIdentityHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<RevokeIdentity>>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldRevokeIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        var command = new RevokeIdentity
        {
            IdentityId = identity.Id,
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        // Act
        await _handler!.HandleAsync(command);

        // Assert
        var revokedIdentity = await Repository.GetAsync(identity.Id);
        revokedIdentity.ShouldNotBeNull();
        revokedIdentity.Status.ShouldBe(IdentityStatus.Revoked);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new RevokeIdentity
        {
            IdentityId = new IdentityId(Guid.NewGuid()),
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.ShouldBe(command.IdentityId);
    }
}


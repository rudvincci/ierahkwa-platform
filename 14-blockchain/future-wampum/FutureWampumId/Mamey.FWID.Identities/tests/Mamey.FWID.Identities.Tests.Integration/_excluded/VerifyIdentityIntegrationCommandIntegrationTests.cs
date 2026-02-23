using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Commands.Handlers.Integration;
using Mamey.FWID.Identities.Application.Commands.Integration.Identities;
using Mamey.FWID.Identities.Application.Exceptions;
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
/// Integration tests for VerifyIdentityIntegrationCommand with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class VerifyIdentityIntegrationCommandIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<VerifyIdentityIntegrationCommand>? _handler;

    public VerifyIdentityIntegrationCommandIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add command handler
        services.AddScoped<ICommandHandler<VerifyIdentityIntegrationCommand>, VerifyIdentityIntegrationCommandHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<VerifyIdentityIntegrationCommand>>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExistsAndBiometricMatches_ShouldVerifyIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        // Use the same biometric data for verification (should match)
        var providedBiometric = identity.BiometricData;

        var command = new VerifyIdentityIntegrationCommand
        {
            IdentityId = identity.Id.Value,
            ProvidedBiometric = providedBiometric,
            Threshold = 0.95
        };

        // Act
        await _handler!.HandleAsync(command);

        // Assert
        var verifiedIdentity = await Repository.GetAsync(identity.Id);
        verifiedIdentity.ShouldNotBeNull();
        verifiedIdentity.Status.ShouldBe(IdentityStatus.Verified);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var providedBiometric = TestDataFactory.CreateTestBiometricData();

        var command = new VerifyIdentityIntegrationCommand
        {
            IdentityId = Guid.NewGuid(),
            ProvidedBiometric = providedBiometric,
            Threshold = 0.95
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.Value.ShouldBe(command.IdentityId);
    }
}


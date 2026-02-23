using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Commands.Handlers.Integration;
using Mamey.FWID.Identities.Application.Commands.Integration.Identities;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Commands;

/// <summary>
/// Integration tests for CreateIdentityIntegrationCommand with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class CreateIdentityIntegrationCommandIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<CreateIdentityIntegrationCommand>? _handler;

    public CreateIdentityIntegrationCommandIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add command handler
        services.AddScoped<ICommandHandler<CreateIdentityIntegrationCommand>, CreateIdentityIntegrationCommandHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<CreateIdentityIntegrationCommand>>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldCreateIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        var command = new CreateIdentityIntegrationCommand
        {
            Id = identity.Id.Value,
            Name = identity.Name,
            PersonalDetails = identity.PersonalDetails,
            ContactInformation = identity.ContactInformation,
            BiometricData = identity.BiometricData,
            Zone = identity.Zone,
            ClanRegistrarId = identity.ClanRegistrarId
        };

        // Act
        await _handler!.HandleAsync(command);

        // Assert
        var createdIdentity = await Repository!.GetAsync(identity.Id);
        createdIdentity.ShouldNotBeNull();
        createdIdentity.Id.ShouldBe(identity.Id);
        createdIdentity.Name.ShouldBe(identity.Name);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        var command = new CreateIdentityIntegrationCommand
        {
            Id = identity.Id.Value,
            Name = identity.Name,
            PersonalDetails = identity.PersonalDetails,
            ContactInformation = identity.ContactInformation,
            BiometricData = identity.BiometricData,
            Zone = identity.Zone,
            ClanRegistrarId = identity.ClanRegistrarId
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityAlreadyExistsException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.Value.ShouldBe(command.Id);
    }
}


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
/// Integration tests for UpdateContactInformation command with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class UpdateContactInformationIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<UpdateContactInformation>? _handler;

    public UpdateContactInformationIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Add command handler
        services.AddScoped<ICommandHandler<UpdateContactInformation>, UpdateContactInformationHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<UpdateContactInformation>>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldUpdateContactInformation()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        var newContactInfo = TestDataFactory.CreateTestContactInformation("newemail@example.com", "5559876543");

        var command = new UpdateContactInformation
        {
            IdentityId = identity.Id,
            ContactInformation = newContactInfo
        };

        // Act
        await _handler!.HandleAsync(command);

        // Assert
        var updatedIdentity = await Repository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.ContactInformation.Email.Value.ShouldBe("newemail@example.com");
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var newContactInfo = TestDataFactory.CreateTestContactInformation();

        var command = new UpdateContactInformation
        {
            IdentityId = new IdentityId(Guid.NewGuid()),
            ContactInformation = newContactInfo
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.ShouldBe(command.IdentityId);
    }
}


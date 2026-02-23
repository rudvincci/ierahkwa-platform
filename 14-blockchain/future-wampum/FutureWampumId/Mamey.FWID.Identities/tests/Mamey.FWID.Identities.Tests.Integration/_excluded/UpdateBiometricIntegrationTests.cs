using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Commands;

/// <summary>
/// Integration tests for UpdateBiometric command with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class UpdateBiometricIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<UpdateBiometric>? _handler;
    private IBiometricStorageService? _storageService;

    public UpdateBiometricIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Mock biometric storage service
        var mockStorageService = Substitute.For<IBiometricStorageService>();
        mockStorageService.UploadBiometricAsync(
            Arg.Any<IdentityId>(),
            Arg.Any<BiometricType>(),
            Arg.Any<byte[]>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("biometric-data/test-id/fingerprint.bin"));
        
        services.AddSingleton(mockStorageService);

        // Add command handler
        services.AddScoped<ICommandHandler<UpdateBiometric>, UpdateBiometricHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<UpdateBiometric>>();
        _storageService = ServiceProvider!.GetRequiredService<IBiometricStorageService>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldUpdateBiometric()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        var newBiometric = TestDataFactory.CreateTestBiometricData(BiometricType.Fingerprint, 2048);
        var verificationBiometric = identity.BiometricData;

        var command = new UpdateBiometric
        {
            IdentityId = identity.Id,
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
        };

        // Act
        await _handler!.HandleAsync(command);

        // Assert
        var updatedIdentity = await Repository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.BiometricData.Type.ShouldBe(newBiometric.Type);
        updatedIdentity.BiometricData.EncryptedTemplate.ShouldBe(newBiometric.EncryptedTemplate);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var newBiometric = TestDataFactory.CreateTestBiometricData();
        var verificationBiometric = TestDataFactory.CreateTestBiometricData();

        var command = new UpdateBiometric
        {
            IdentityId = new IdentityId(Guid.NewGuid()),
            NewBiometric = newBiometric,
            VerificationBiometric = verificationBiometric
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.ShouldBe(command.IdentityId);
    }
}


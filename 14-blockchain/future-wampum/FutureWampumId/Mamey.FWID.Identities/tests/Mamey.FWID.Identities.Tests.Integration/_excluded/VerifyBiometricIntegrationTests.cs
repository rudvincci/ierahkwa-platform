using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Commands;

/// <summary>
/// Integration tests for VerifyBiometric command with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class VerifyBiometricIntegrationTests : BaseCommandIntegrationTests
{
    private ICommandHandler<VerifyBiometric>? _handler;
    private IBiometricStorageService? _storageService;

    public VerifyBiometricIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
    {
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Mock biometric storage service
        var mockStorageService = Substitute.For<IBiometricStorageService>();
        // Return the same biometric data for verification (simulating a match)
        mockStorageService.DownloadBiometricAsync(
            Arg.Any<IdentityId>(),
            Arg.Any<BiometricType>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var identityId = callInfo.Arg<IdentityId>();
                // Return test biometric data
                var testData = TestDataFactory.CreateTestBiometricData(BiometricType.Fingerprint, 1024);
                return Task.FromResult(testData.EncryptedTemplate);
            });
        
        services.AddSingleton(mockStorageService);

        // Add command handler
        services.AddScoped<ICommandHandler<VerifyBiometric>, VerifyBiometricHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<ICommandHandler<VerifyBiometric>>();
        _storageService = ServiceProvider!.GetRequiredService<IBiometricStorageService>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExistsAndBiometricMatches_ShouldVerifyIdentity()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        // Use the same biometric data for verification (should match)
        var providedBiometric = identity.BiometricData;

        var command = new VerifyBiometric
        {
            IdentityId = identity.Id,
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

        var command = new VerifyBiometric
        {
            IdentityId = new IdentityId(Guid.NewGuid()),
            ProvidedBiometric = providedBiometric,
            Threshold = 0.95
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(command));

        exception.IdentityId.ShouldBe(command.IdentityId);
    }
}


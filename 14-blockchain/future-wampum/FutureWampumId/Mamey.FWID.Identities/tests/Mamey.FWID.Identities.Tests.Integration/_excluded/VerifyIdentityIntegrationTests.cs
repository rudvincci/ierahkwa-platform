using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Queries.Handlers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Queries;

/// <summary>
/// Integration tests for VerifyIdentity query with real PostgreSQL repository.
/// </summary>
[Collection("Integration")]
public class VerifyIdentityIntegrationTests : BaseQueryIntegrationTests
{
    private IQueryHandler<VerifyIdentity, BiometricVerificationResultDto>? _handler;
    private IBiometricStorageService? _storageService;

    public VerifyIdentityIntegrationTests(PostgreSQLFixture fixture) : base(fixture)
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

        // Add query handler
        services.AddScoped<IQueryHandler<VerifyIdentity, BiometricVerificationResultDto>, VerifyIdentityHandler>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _handler = ServiceProvider!.GetRequiredService<IQueryHandler<VerifyIdentity, BiometricVerificationResultDto>>();
        _storageService = ServiceProvider!.GetRequiredService<IBiometricStorageService>();
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExistsAndBiometricMatches_ShouldReturnVerifiedResult()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        // Use the same biometric data for verification (should match)
        var providedBiometric = identity.BiometricData;

        var query = new VerifyIdentity(identity.Id, providedBiometric);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.IsVerified.ShouldBeTrue();
        result.MatchScore.ShouldBeGreaterThan(0.95);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var providedBiometric = TestDataFactory.CreateTestBiometricData();
        var query = new VerifyIdentity(new IdentityId(Guid.NewGuid()), providedBiometric);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler!.HandleAsync(query));

        exception.IdentityId.ShouldBe(query.IdentityId);
    }

    [Fact]
    public async Task HandleAsync_WhenBiometricDoesNotMatch_ShouldReturnNotVerifiedResult()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await Repository!.AddAsync(identity);

        // Use different biometric data (should not match)
        var differentBiometric = TestDataFactory.CreateTestBiometricData(BiometricType.Facial, 2048);

        var query = new VerifyIdentity(identity.Id, differentBiometric);

        // Act
        var result = await _handler!.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.IsVerified.ShouldBeFalse();
        result.MatchScore.ShouldBeLessThan(0.95);
    }
}


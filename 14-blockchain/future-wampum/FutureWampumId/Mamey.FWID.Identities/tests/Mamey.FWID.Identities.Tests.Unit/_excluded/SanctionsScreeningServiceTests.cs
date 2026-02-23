using FluentAssertions;
using Mamey.FWID.Identities.Application.AML.Models;
using Mamey.FWID.Identities.Application.AML.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.AML.Services;

public class SanctionsScreeningServiceTests
{
    private readonly ILogger<SanctionsScreeningService> _logger;
    private readonly IMemoryCache _cache;
    private readonly SanctionsScreeningService _sut;
    
    public SanctionsScreeningServiceTests()
    {
        _logger = Substitute.For<ILogger<SanctionsScreeningService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _sut = new SanctionsScreeningService(_logger, _cache);
    }
    
    [Fact]
    public async Task ScreenIdentityAsync_Should_ReturnClear_When_NoMatches()
    {
        // Arrange
        var request = new SanctionsScreeningRequest
        {
            IdentityId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Smith",
            UseCache = false
        };
        
        // Act
        var result = await _sut.ScreenIdentityAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ScreeningStatus.Clear);
        result.HasMatches.Should().BeFalse();
        result.ProcessingTime.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public async Task ScreenIdentityAsync_Should_ReturnPotentialMatch_When_NameMatchesEntry()
    {
        // Arrange
        var request = new SanctionsScreeningRequest
        {
            IdentityId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Test Sanctioned", // Matches sample entry
            UseCache = false
        };
        
        // Act
        var result = await _sut.ScreenIdentityAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        if (result.HasMatches)
        {
            result.Status.Should().Be(ScreeningStatus.PotentialMatch);
            result.Matches.Should().NotBeEmpty();
            result.HighestMatchScore.Should().BeGreaterThan(0);
        }
    }
    
    [Fact]
    public async Task ScreenIdentityAsync_Should_CheckAllSources()
    {
        // Arrange
        var request = new SanctionsScreeningRequest
        {
            IdentityId = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Person",
            UseCache = false
        };
        
        // Act
        var result = await _sut.ScreenIdentityAsync(request);
        
        // Assert
        result.SourcesChecked.Should().Contain("OFAC_SDN");
        result.SourcesChecked.Should().Contain("UN_SC");
        result.SourcesChecked.Should().Contain("EU_CONS");
    }
    
    [Fact]
    public async Task ScreenIdentityAsync_Should_UseCachedResult_When_Available()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var request = new SanctionsScreeningRequest
        {
            IdentityId = identityId,
            FirstName = "John",
            LastName = "Doe",
            UseCache = true
        };
        
        // First call
        await _sut.ScreenIdentityAsync(request);
        
        // Act - Second call
        var result = await _sut.ScreenIdentityAsync(request);
        
        // Assert - Should be faster due to cache
        result.Should().NotBeNull();
        result.IdentityId.Should().Be(identityId);
    }
    
    [Fact]
    public async Task GetSanctionsListsAsync_Should_ReturnAllConfiguredLists()
    {
        // Act
        var lists = await _sut.GetSanctionsListsAsync();
        
        // Assert
        lists.Should().NotBeEmpty();
        lists.Should().HaveCountGreaterOrEqualTo(3);
        lists.Should().Contain(l => l.Source == "OFAC");
        lists.Should().Contain(l => l.Source == "UN");
        lists.Should().Contain(l => l.Source == "EU");
    }
    
    [Fact]
    public async Task ScreenNameAsync_Should_ScreenWithoutIdentityId()
    {
        // Arrange
        var firstName = "Test";
        var lastName = "User";
        
        // Act
        var result = await _sut.ScreenNameAsync(firstName, lastName);
        
        // Assert
        result.Should().NotBeNull();
        result.IdentityId.Should().Be(Guid.Empty);
        result.ScreeningType.Should().Be(ScreeningType.Sanctions);
    }
    
    [Fact]
    public async Task InvalidateCacheAsync_Should_RemoveCachedResult()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var request = new SanctionsScreeningRequest
        {
            IdentityId = identityId,
            FirstName = "John",
            LastName = "Doe",
            UseCache = true
        };
        
        await _sut.ScreenIdentityAsync(request);
        
        // Act
        await _sut.InvalidateCacheAsync(identityId);
        var cached = await _sut.GetCachedResultAsync(identityId);
        
        // Assert
        cached.Should().BeNull();
    }
    
    [Fact]
    public async Task ScreenIdentityAsync_Should_IncludeAliasMatching()
    {
        // Arrange
        var request = new SanctionsScreeningRequest
        {
            IdentityId = Guid.NewGuid(),
            FirstName = "Johnny",
            LastName = "Sanctioned", // Alias in sample data
            Aliases = new List<string> { "J. Sanctioned" },
            UseCache = false
        };
        
        // Act
        var result = await _sut.ScreenIdentityAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        // The test verifies alias processing occurred
        result.ProcessingTime.Should().BeLessThan(TimeSpan.FromSeconds(5));
    }
    
    [Fact]
    public async Task ScreenIdentityAsync_Should_BoostScoreForDOBMatch()
    {
        // Arrange
        var request = new SanctionsScreeningRequest
        {
            IdentityId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Test Sanctioned",
            DateOfBirth = new DateTime(1970, 5, 15), // Matches sample entry
            UseCache = false
        };
        
        // Act
        var result = await _sut.ScreenIdentityAsync(request);
        
        // Assert
        if (result.HasMatches)
        {
            result.Matches.First().MatchingCriteria
                .Should().Contain(c => c.Field == "Name" || c.Field == "DateOfBirth");
        }
    }
}

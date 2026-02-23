using FluentAssertions;
using Mamey.FWID.Identities.Application.AI.Models;
using Mamey.FWID.Identities.Application.AI.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.AI.Services;

public class BehavioralAnalyticsServiceTests
{
    private readonly ILogger<BehavioralAnalyticsService> _logger;
    private readonly BehavioralAnalyticsService _sut;
    
    public BehavioralAnalyticsServiceTests()
    {
        _logger = Substitute.For<ILogger<BehavioralAnalyticsService>>();
        _sut = new BehavioralAnalyticsService(_logger);
    }
    
    [Fact]
    public async Task GetOrCreateProfileAsync_Should_CreateNewProfile_When_NotExists()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        
        // Act
        var profile = await _sut.GetOrCreateProfileAsync(identityId);
        
        // Assert
        profile.Should().NotBeNull();
        profile.IdentityId.Should().Be(identityId);
        profile.DataPointCount.Should().Be(0);
        profile.IsEstablished.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetOrCreateProfileAsync_Should_ReturnExistingProfile_When_Exists()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var profile1 = await _sut.GetOrCreateProfileAsync(identityId);
        
        // Act
        var profile2 = await _sut.GetOrCreateProfileAsync(identityId);
        
        // Assert
        profile2.ProfileId.Should().Be(profile1.ProfileId);
    }
    
    [Fact]
    public async Task RecordBehaviorAsync_Should_IncrementDataPointCount()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        await _sut.GetOrCreateProfileAsync(identityId);
        
        var behaviorEvent = new BehavioralEvent
        {
            IdentityId = identityId,
            EventType = BehavioralEventType.Login,
            IPAddress = "192.168.1.1",
            Location = "New York, US"
        };
        
        // Act
        await _sut.RecordBehaviorAsync(behaviorEvent);
        
        // Assert
        var profile = await _sut.GetOrCreateProfileAsync(identityId);
        profile.DataPointCount.Should().Be(1);
    }
    
    [Fact]
    public async Task RecordBehaviorAsync_Should_UpdateLoginPatterns()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        
        // Record multiple logins
        for (int i = 0; i < 5; i++)
        {
            await _sut.RecordBehaviorAsync(new BehavioralEvent
            {
                IdentityId = identityId,
                EventType = BehavioralEventType.Login,
                Timestamp = DateTime.UtcNow.AddHours(-i),
                Location = "New York, US"
            });
        }
        
        // Act
        var profile = await _sut.GetOrCreateProfileAsync(identityId);
        
        // Assert
        profile.LoginPattern.PreferredHoursOfDay.Should().NotBeEmpty();
        profile.LoginPattern.CommonLocations.Should().Contain("New York, US");
    }
    
    [Fact]
    public async Task AnalyzeBehaviorAsync_Should_ReturnLowConfidence_When_BaselineNotEstablished()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var context = new AuthenticationContext
        {
            IdentityId = identityId,
            Country = "US"
        };
        
        // Act
        var result = await _sut.AnalyzeBehaviorAsync(identityId, context);
        
        // Assert
        result.Confidence.Should().BeLessThan(0.5);
        result.IsAnomalous.Should().BeFalse();
    }
    
    [Fact]
    public async Task AnalyzeBehaviorAsync_Should_DetectLocationDeviation_When_FromNewLocation()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        
        // Establish baseline with US locations
        for (int i = 0; i < 15; i++)
        {
            await _sut.RecordBehaviorAsync(new BehavioralEvent
            {
                IdentityId = identityId,
                EventType = BehavioralEventType.Login,
                Location = "New York, US"
            });
        }
        
        var context = new AuthenticationContext
        {
            IdentityId = identityId,
            Country = "Russia",
            City = "Moscow"
        };
        
        // Act
        var result = await _sut.AnalyzeBehaviorAsync(identityId, context);
        
        // Assert
        result.Deviations.Should().Contain(d => d.DeviationType == "Location");
    }
    
    [Fact]
    public async Task EvaluateForAdaptiveAuthAsync_Should_AllowAccess_When_NoDeviations()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var context = new AuthenticationContext
        {
            IdentityId = identityId
        };
        
        // Act
        var result = await _sut.EvaluateForAdaptiveAuthAsync(identityId, context);
        
        // Assert
        result.RequiredAction.Should().Be(AdaptiveAuthAction.Allow);
        result.RiskScore.Should().BeLessThan(30);
    }
    
    [Fact]
    public async Task DetectAnomaliesAsync_Should_DetectRapidLoginAttempts()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        
        // Simulate rapid logins
        for (int i = 0; i < 15; i++)
        {
            await _sut.RecordBehaviorAsync(new BehavioralEvent
            {
                IdentityId = identityId,
                EventType = BehavioralEventType.Login,
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        
        // Act
        var anomalies = await _sut.DetectAnomaliesAsync(identityId);
        
        // Assert
        anomalies.Should().Contain(a => a.AnomalyType == "RapidLoginAttempts");
    }
    
    [Fact]
    public async Task DetectAnomaliesAsync_Should_DetectMultipleLocations()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var locations = new[] { "New York, US", "London, UK", "Tokyo, JP", "Sydney, AU" };
        
        foreach (var location in locations)
        {
            await _sut.RecordBehaviorAsync(new BehavioralEvent
            {
                IdentityId = identityId,
                EventType = BehavioralEventType.Login,
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                Location = location
            });
        }
        
        // Act
        var anomalies = await _sut.DetectAnomaliesAsync(identityId);
        
        // Assert
        anomalies.Should().Contain(a => a.AnomalyType == "MultipleLocations");
    }
}

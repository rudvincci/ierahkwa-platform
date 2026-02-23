using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Audit;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Audit;

public class DidAuditServiceTests
{
    private readonly Mock<IAuditRepository> _auditRepositoryMock;
    private readonly Mock<ILogger<DidAuditService>> _loggerMock;
    private readonly DidAuditService _auditService;

    public DidAuditServiceTests()
    {
        _auditRepositoryMock = new Mock<IAuditRepository>();
        _loggerMock = new Mock<ILogger<DidAuditService>>();
        _auditService = new DidAuditService(_loggerMock.Object, _auditRepositoryMock.Object);
    }

    [Fact]
    public async Task LogAuthenticationEventAsync_WithValidParameters_ShouldLogEvent()
    {
        // Arrange
        var eventType = DidAuditEventTypes.DID_AUTHENTICATION;
        var category = DidAuditCategories.AUTHENTICATION;
        var status = DidAuditStatus.SUCCESS;
        var userId = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var correlationId = "test-correlation-id";
        var metadata = new Dictionary<string, object>
        {
            ["did"] = userId,
            ["role"] = "user"
        };

        _auditRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _auditService.LogAuthenticationEventAsync(
            eventType,
            category,
            status,
            userId,
            correlationId,
            metadata: metadata);

        // Assert
        _auditRepositoryMock.Verify(x => x.SaveAsync(
            It.Is<AuditLog>(e => 
                e.ActivityType == eventType &&
                e.Category == category &&
                e.Status == status &&
                e.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogDidOperationAsync_WithValidParameters_ShouldLogEvent()
    {
        // Arrange
        var eventType = DidAuditEventTypes.DID_CREATION;
        var category = DidAuditCategories.DID_OPERATIONS;
        var status = DidAuditStatus.SUCCESS;
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var correlationId = "test-correlation-id";
        var metadata = new Dictionary<string, object>
        {
            ["did"] = did,
            ["method"] = "key"
        };

        _auditRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _auditService.LogDidOperationAsync(
            eventType,
            category,
            status,
            did,
            correlationId,
            metadata: metadata);

        // Assert
        _auditRepositoryMock.Verify(x => x.SaveAsync(
            It.Is<AuditLog>(e => 
                e.ActivityType == eventType &&
                e.Category == category &&
                e.Status == status &&
                e.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogCredentialOperationAsync_WithValidParameters_ShouldLogEvent()
    {
        // Arrange
        var eventType = DidAuditEventTypes.VC_ISSUANCE;
        var category = DidAuditCategories.CREDENTIAL_OPERATIONS;
        var status = DidAuditStatus.SUCCESS;
        var credentialId = "test-credential-id";
        var correlationId = "test-correlation-id";
        var metadata = new Dictionary<string, object>
        {
            ["credentialId"] = credentialId,
            ["issuerDid"] = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"
        };

        _auditRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _auditService.LogCredentialOperationAsync(
            eventType,
            category,
            status,
            credentialId,
            correlationId,
            metadata: metadata);

        // Assert
        _auditRepositoryMock.Verify(x => x.SaveAsync(
            It.Is<AuditLog>(e => 
                e.ActivityType == eventType &&
                e.Category == category &&
                e.Status == status &&
                e.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogEventAsync_WithValidParameters_ShouldLogEvent()
    {
        // Arrange
        var eventType = DidAuditEventTypes.KEY_MANAGEMENT;
        var category = DidAuditCategories.KEY_MANAGEMENT;
        var status = DidAuditStatus.SUCCESS;
        var keyId = "test-key-id";
        var correlationId = "test-correlation-id";
        var metadata = new Dictionary<string, object>
        {
            ["keyId"] = keyId,
            ["keyType"] = "Ed25519"
        };

        _auditRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _auditService.LogEventAsync(
            eventType,
            category,
            status,
            keyId,
            correlationId,
            metadata: metadata);

        // Assert
        _auditRepositoryMock.Verify(x => x.SaveAsync(
            It.Is<AuditLog>(e => 
                e.ActivityType == eventType &&
                e.Category == category &&
                e.Status == status &&
                e.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogEventAsync_WithSystemEvent_ShouldLogEvent()
    {
        // Arrange
        var eventType = DidAuditEventTypes.DID_RESOLUTION;
        var category = DidAuditCategories.DID_OPERATIONS;
        var status = DidAuditStatus.SUCCESS;
        var correlationId = "test-correlation-id";
        var metadata = new Dictionary<string, object>
        {
            ["did"] = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            ["cacheKey"] = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"
        };

        _auditRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _auditService.LogEventAsync(
            eventType,
            category,
            status,
            "System",
            correlationId,
            metadata: metadata);

        // Assert
        _auditRepositoryMock.Verify(x => x.SaveAsync(
            It.Is<AuditLog>(e => 
                e.ActivityType == eventType &&
                e.Category == category &&
                e.Status == status &&
                e.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAuthenticationEventAsync_WithNullMetadata_ShouldUseEmptyDictionary()
    {
        // Arrange
        var eventType = DidAuditEventTypes.DID_AUTHENTICATION;
        var category = DidAuditCategories.AUTHENTICATION;
        var status = DidAuditStatus.SUCCESS;
        var userId = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var correlationId = "test-correlation-id";
        Dictionary<string, object> metadata = null;

        _auditRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _auditService.LogAuthenticationEventAsync(
            eventType,
            category,
            status,
            userId,
            correlationId,
            metadata: metadata);

        // Assert
        _auditRepositoryMock.Verify(x => x.SaveAsync(
            It.IsAny<AuditLog>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

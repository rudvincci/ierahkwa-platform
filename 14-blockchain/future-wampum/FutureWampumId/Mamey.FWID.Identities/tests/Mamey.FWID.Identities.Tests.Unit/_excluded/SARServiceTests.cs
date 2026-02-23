using FluentAssertions;
using Mamey.FWID.Identities.Application.AML.Models;
using Mamey.FWID.Identities.Application.AML.Services;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.AML.Services;

public class SARServiceTests
{
    private readonly ILogger<SARService> _logger;
    private readonly IBusPublisher _publisher;
    private readonly SARService _sut;
    
    public SARServiceTests()
    {
        _logger = Substitute.For<ILogger<SARService>>();
        _publisher = Substitute.For<IBusPublisher>();
        _sut = new SARService(_logger, _publisher);
    }
    
    [Fact]
    public async Task CreateSARAsync_Should_CreateDraftSAR()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test Subject",
            Trigger = SARTrigger.RiskScoreThreshold,
            Type = SARType.SuspiciousActivity,
            Priority = SARPriority.High,
            Narrative = "Suspicious activity detected",
            ActivityDetectedAt = DateTime.UtcNow.AddHours(-2),
            CreatedBy = Guid.NewGuid()
        };
        
        // Act
        var sar = await _sut.CreateSARAsync(request);
        
        // Assert
        sar.Should().NotBeNull();
        sar.Status.Should().Be(SARStatus.Draft);
        sar.ReferenceNumber.Should().StartWith("SAR-");
        sar.DueDate.Should().BeAfter(DateTime.UtcNow);
    }
    
    [Fact]
    public async Task CreateSARAsync_Should_PublishEvent()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test Subject",
            Trigger = SARTrigger.SanctionsMatch,
            Type = SARType.SanctionsViolation,
            Priority = SARPriority.Critical,
            Narrative = "Sanctions match detected",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        
        // Act
        await _sut.CreateSARAsync(request);
        
        // Assert
        await _publisher.Received(1).PublishAsync(
            Arg.Any<SARCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task UpdateStatusAsync_Should_AddWorkflowHistory()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test",
            Trigger = SARTrigger.ManualFlag,
            Type = SARType.Other,
            Priority = SARPriority.Medium,
            Narrative = "Manual flag",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        var sar = await _sut.CreateSARAsync(request);
        var reviewerId = Guid.NewGuid();
        
        // Act
        var updatedSar = await _sut.UpdateStatusAsync(
            sar.Id,
            SARStatus.PendingReview,
            reviewerId,
            "Submitted for review");
        
        // Assert
        updatedSar.Status.Should().Be(SARStatus.PendingReview);
        updatedSar.WorkflowHistory.Should().HaveCountGreaterThan(1);
        updatedSar.WorkflowHistory.Last().ToStatus.Should().Be(SARStatus.PendingReview);
    }
    
    [Fact]
    public async Task FileSARAsync_Should_RequireApproval()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test",
            Trigger = SARTrigger.RiskScoreThreshold,
            Type = SARType.MoneyLaundering,
            Priority = SARPriority.High,
            Narrative = "Test narrative",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        var sar = await _sut.CreateSARAsync(request);
        
        // Act - Try to file without approval
        var result = await _sut.FileSARAsync(sar.Id, Guid.NewGuid());
        
        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("approved");
    }
    
    [Fact]
    public async Task FileSARAsync_Should_Succeed_WhenApproved()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test",
            Trigger = SARTrigger.TransactionPatternAnomaly,
            Type = SARType.StructuringActivity,
            Priority = SARPriority.High,
            Narrative = "Structuring detected",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        var sar = await _sut.CreateSARAsync(request);
        var approverId = Guid.NewGuid();
        
        // Approve first
        await _sut.ApproveSARAsync(sar.Id, approverId);
        
        // Act
        var result = await _sut.FileSARAsync(sar.Id, Guid.NewGuid());
        
        // Assert
        result.Success.Should().BeTrue();
        result.ConfirmationNumber.Should().StartWith("SCOB-");
        result.RegulatoryBody.Should().Contain("SCOB");
    }
    
    [Fact]
    public async Task GetSARByReferenceAsync_Should_FindSAR()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test",
            Trigger = SARTrigger.BehavioralAnomaly,
            Type = SARType.IdentityFraud,
            Priority = SARPriority.Medium,
            Narrative = "Identity fraud suspected",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        var sar = await _sut.CreateSARAsync(request);
        
        // Act
        var found = await _sut.GetSARByReferenceAsync(sar.ReferenceNumber);
        
        // Assert
        found.Should().NotBeNull();
        found!.Id.Should().Be(sar.Id);
    }
    
    [Fact]
    public async Task GetPendingSARsAsync_Should_ReturnOnlyPending()
    {
        // Arrange
        var request1 = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test 1",
            Trigger = SARTrigger.ManualFlag,
            Type = SARType.Other,
            Priority = SARPriority.Low,
            Narrative = "Test",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        await _sut.CreateSARAsync(request1);
        
        // Act
        var pending = await _sut.GetPendingSARsAsync();
        
        // Assert
        pending.Should().AllSatisfy(sar => 
            sar.Status.Should().BeOneOf(
                SARStatus.Draft, 
                SARStatus.PendingReview, 
                SARStatus.UnderReview,
                SARStatus.ApprovalRequired));
    }
    
    [Fact]
    public async Task AddEvidenceAsync_Should_AttachEvidence()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test",
            Trigger = SARTrigger.ThirdPartyAlert,
            Type = SARType.BriberyCorruption,
            Priority = SARPriority.Critical,
            Narrative = "Third party alert",
            ActivityDetectedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        var sar = await _sut.CreateSARAsync(request);
        
        var evidence = new SAREvidence
        {
            Type = "Document",
            Description = "Transaction records",
            FileReference = "files/tx-records.pdf",
            AddedBy = Guid.NewGuid()
        };
        
        // Act
        var result = await _sut.AddEvidenceAsync(sar.Id, evidence);
        
        // Assert
        result.Should().BeTrue();
        var updated = await _sut.GetSARAsync(sar.Id);
        updated!.Evidence.Should().Contain(e => e.Type == "Document");
    }
    
    [Fact]
    public async Task GenerateSARReportAsync_Should_ReturnDocument()
    {
        // Arrange
        var request = new CreateSARRequest
        {
            SubjectIdentityId = Guid.NewGuid(),
            SubjectName = "Test Subject",
            SubjectDID = "did:futurewampum:test:123",
            Trigger = SARTrigger.RiskScoreThreshold,
            Type = SARType.UnusualTransaction,
            Priority = SARPriority.High,
            Narrative = "Unusual transaction patterns detected over the past 30 days.",
            ActivityDetectedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid()
        };
        var sar = await _sut.CreateSARAsync(request);
        
        // Act
        var report = await _sut.GenerateSARReportAsync(sar.Id);
        
        // Assert
        report.Should().NotBeEmpty();
        var reportText = System.Text.Encoding.UTF8.GetString(report);
        reportText.Should().Contain("SUSPICIOUS ACTIVITY REPORT");
        reportText.Should().Contain(sar.ReferenceNumber);
        reportText.Should().Contain("Test Subject");
    }
    
    [Fact]
    public async Task GetSARsApproachingDeadlineAsync_Should_FilterByDeadline()
    {
        // Act
        var approaching = await _sut.GetSARsApproachingDeadlineAsync(3);
        
        // Assert
        approaching.Should().AllSatisfy(sar =>
            sar.DueDate.Should().BeBefore(DateTime.UtcNow.AddDays(4)));
    }
}

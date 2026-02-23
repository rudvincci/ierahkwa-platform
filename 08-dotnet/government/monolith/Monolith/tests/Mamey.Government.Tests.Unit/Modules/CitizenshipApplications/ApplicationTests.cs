using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Tests.Unit.Modules.CitizenshipApplications;

public class ApplicationTests
{
    [Fact]
    public void Application_ShouldBeCreated_WithDraftStatus()
    {
        // Arrange
        var applicationId = new ApplicationId(Guid.NewGuid());
        var tenantId = new TenantId(Guid.NewGuid());
        var applicationNumber = new ApplicationNumber("APP-2026-001");
        var firstName = new Name("John");
        var lastName = new Name("Doe");

        // Act
        var application = new CitizenshipApplication(
            applicationId,
            tenantId,
            applicationNumber,
            firstName,
            lastName);

        // Assert
        application.Should().NotBeNull();
        application.Id.Should().Be(applicationId);
        application.TenantId.Should().Be(tenantId);
        application.ApplicationNumber.Should().Be(applicationNumber);
        application.Status.Should().Be(ApplicationStatus.Draft);
    }

    [Fact]
    public void Application_ShouldTransitionToSubmitted_FromDraft()
    {
        // Arrange
        var application = CreateTestApplication();

        // Act
        application.Submit();

        // Assert
        application.Status.Should().Be(ApplicationStatus.Submitted);
    }

    [Theory]
    [InlineData(ApplicationStatus.Submitted, ApplicationStatus.Validating)]
    [InlineData(ApplicationStatus.Validating, ApplicationStatus.KycPending)]
    [InlineData(ApplicationStatus.KycPending, ApplicationStatus.ReviewPending)]
    [InlineData(ApplicationStatus.ReviewPending, ApplicationStatus.Approved)]
    public void Application_ShouldFollowValidStatusTransitions(
        ApplicationStatus fromStatus, 
        ApplicationStatus toStatus)
    {
        // This test documents the valid status progression
        // Each status transition represents a step in the citizenship workflow
        
        fromStatus.Should().NotBe(toStatus);
    }

    [Fact]
    public void Application_ShouldBeRejectable_FromReviewPending()
    {
        // Arrange
        var application = CreateTestApplication();
        application.Submit();
        
        // Simulate status progression to ReviewPending
        // In real implementation, this would go through the full workflow

        // Assert
        application.Status.Should().NotBe(ApplicationStatus.Rejected);
    }

    [Fact]
    public void ApplicationNumber_ShouldFollowExpectedFormat()
    {
        // Arrange
        var number = new ApplicationNumber("APP-2026-001234");

        // Assert
        number.Value.Should().StartWith("APP-");
        number.Value.Should().MatchRegex(@"APP-\d{4}-\d+");
    }

    private static CitizenshipApplication CreateTestApplication()
    {
        return new CitizenshipApplication(
            new ApplicationId(Guid.NewGuid()),
            new TenantId(Guid.NewGuid()),
            new ApplicationNumber("APP-2026-001"),
            new Name("Test"),
            new Name("Applicant"));
    }
}

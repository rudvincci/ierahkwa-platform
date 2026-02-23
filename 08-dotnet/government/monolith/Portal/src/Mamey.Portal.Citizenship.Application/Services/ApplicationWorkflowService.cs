using Microsoft.Extensions.Logging;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class ApplicationWorkflowService : IApplicationWorkflowService
{
    private readonly ITenantContext _tenant;
    private readonly ICitizenshipBackofficeService _backofficeService;
    private readonly ILogger<ApplicationWorkflowService> _logger;
    private readonly IApplicationWorkflowStore _store;

    public ApplicationWorkflowService(
        ITenantContext tenant,
        ICitizenshipBackofficeService backofficeService,
        ILogger<ApplicationWorkflowService> logger,
        IApplicationWorkflowStore store)
    {
        _tenant = tenant;
        _backofficeService = backofficeService;
        _logger = logger;
        _store = store;
    }

    public async Task<bool> ProcessNextWorkflowStepAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null)
        {
            return false;
        }

        var processed = app.Status switch
        {
            "Submitted" => await ValidateApplicationAsync(applicationId, ct),
            "Validating" => await ProcessKycAsync(applicationId, ct),
            "KycPending" => await AutoApproveIfReadyAsync(applicationId, ct),
            "Approved" => await CreatePaymentPlanAsync(applicationId, ct),
            "CitizenCreating" => await ProcessCitizenCreationAsync(applicationId, ct),
            _ => false
        };

        if (processed)
        {
            await Task.Delay(100, ct);
            return await ProcessNextWorkflowStepAsync(applicationId, ct);
        }

        return processed;
    }

    public async Task<bool> ValidateApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null || app.Status != "Submitted")
        {
            return false;
        }

        var (isValid, rejectionReason) = await PerformBasicValidationAsync(app, ct);
        var now = DateTimeOffset.UtcNow;

        await _store.UpdateStatusAsync(
            tenantId,
            applicationId,
            isValid ? "Validating" : "Rejected",
            isValid ? null : rejectionReason,
            now,
            ct);

        if (!isValid)
        {
            _logger.LogWarning("Application {ApplicationId} rejected during validation: {Reason}", applicationId, rejectionReason);
        }

        return isValid;
    }

    public async Task<bool> ProcessKycAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null || app.Status != "Validating")
        {
            return false;
        }

        var (kycPassed, rejectionReason) = await PerformBasicKycAsync(app, ct);
        var now = DateTimeOffset.UtcNow;

        await _store.UpdateStatusAsync(
            tenantId,
            applicationId,
            kycPassed ? "KycPending" : "Rejected",
            kycPassed ? null : rejectionReason,
            now,
            ct);

        if (!kycPassed)
        {
            _logger.LogWarning("Application {ApplicationId} rejected during KYC: {Reason}", applicationId, rejectionReason);
        }

        return kycPassed;
    }

    private async Task<bool> AutoApproveIfReadyAsync(Guid applicationId, CancellationToken ct)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null || app.Status != "KycPending")
        {
            return false;
        }

        var intakeReview = await _store.GetLatestIntakeReviewAsync(tenantId, applicationId, ct);

        if (intakeReview != null && intakeReview.Recommendation == "Approve")
        {
            await _store.UpdateStatusAsync(
                tenantId,
                applicationId,
                "Approved",
                null,
                DateTimeOffset.UtcNow,
                ct);
            return true;
        }

        return false;
    }

    private Task<(bool IsValid, string? RejectionReason)> PerformBasicValidationAsync(WorkflowApplicationSnapshot app, CancellationToken ct)
    {
        var missingFields = new List<string>();
        if (string.IsNullOrWhiteSpace(app.FirstName))
            missingFields.Add("First name");
        if (string.IsNullOrWhiteSpace(app.LastName))
            missingFields.Add("Last name");
        if (string.IsNullOrWhiteSpace(app.Email))
            missingFields.Add("Email address");
        if (app.DateOfBirth == default)
            missingFields.Add("Date of birth");

        var missingConsents = new List<string>();
        if (!app.AcknowledgeTreaty)
            missingConsents.Add("Treaty acknowledgment");
        if (!app.SwearAllegiance)
            missingConsents.Add("Affidavit of allegiance");
        if (!app.ConsentToVerification)
            missingConsents.Add("Consent to verification");
        if (!app.ConsentToDataProcessing)
            missingConsents.Add("Consent to data processing");

        var missingDocuments = new List<string>();
        if (!app.UploadKinds.Contains("PersonalDocument"))
            missingDocuments.Add("Personal documents");
        if (!app.UploadKinds.Contains("PassportPhoto"))
            missingDocuments.Add("Passport photo");
        if (!app.UploadKinds.Contains("Signature"))
            missingDocuments.Add("Signature");

        var allValid = missingFields.Count == 0 && missingConsents.Count == 0 && missingDocuments.Count == 0;

        string? rejectionReason = null;
        if (!allValid)
        {
            var reasons = new List<string>();
            if (missingFields.Count > 0)
                reasons.Add($"Missing required fields: {string.Join(", ", missingFields)}");
            if (missingConsents.Count > 0)
                reasons.Add($"Missing required consents: {string.Join(", ", missingConsents)}");
            if (missingDocuments.Count > 0)
                reasons.Add($"Missing required documents: {string.Join(", ", missingDocuments)}");
            rejectionReason = string.Join("; ", reasons);
        }

        return Task.FromResult((allValid, rejectionReason));
    }

    private Task<(bool IsValid, string? RejectionReason)> PerformBasicKycAsync(WorkflowApplicationSnapshot app, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.DateTime);
        var age = today.Year - app.DateOfBirth.Year;
        if (app.DateOfBirth > today.AddYears(-age)) age--;

        var ageValid = age >= 18 && age <= 120;
        string? rejectionReason = null;

        if (!ageValid)
        {
            if (age < 18)
                rejectionReason = $"Applicant must be at least 18 years old. Current age: {age}";
            else if (age > 120)
                rejectionReason = $"Date of birth appears invalid. Calculated age: {age} years";
            else
                rejectionReason = $"Age validation failed. Calculated age: {age} years";
        }

        return Task.FromResult((ageValid, rejectionReason));
    }

    public async Task<bool> CreatePaymentPlanAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null || app.Status != "Approved")
        {
            return false;
        }

        var now = DateTimeOffset.UtcNow;
        return await _store.CreatePaymentPlanIfMissingAsync(
            tenantId,
            applicationId,
            app.ApplicationNumber,
            150.00m,
            "USD",
            now,
            ct);
    }

    public async Task<bool> ProcessCitizenCreationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null || app.Status != "CitizenCreating")
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(app.Email))
        {
            return false;
        }

        try
        {
            await _backofficeService.ApproveApplicationAsync(applicationId, ct);
            await _backofficeService.IssuePassportAsync(applicationId, ct);
            await _backofficeService.IssueTravelIdAsync(applicationId, ct);
            await _backofficeService.IssueIdCardAsync(applicationId, "IdentificationCard", ct);
            await _backofficeService.GenerateCitizenshipCertificateAsync(applicationId, ct);

            await _store.UpdateStatusAsync(
                tenantId,
                applicationId,
                "Completed",
                null,
                DateTimeOffset.UtcNow,
                ct);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing citizen creation for application {ApplicationId}", applicationId);
            return false;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

/// <summary>
/// Background service that periodically processes application workflow steps
/// </summary>
public sealed class ApplicationWorkflowBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ApplicationWorkflowBackgroundService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5); // Process every 5 minutes

    public ApplicationWorkflowBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ApplicationWorkflowBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Application Workflow Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingApplicationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing application workflow");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Application Workflow Background Service stopped");
    }

    private async Task ProcessPendingApplicationsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CitizenshipDbContext>();

        // Get applications that are in states that can be automatically processed
        var pendingApps = await db.Applications
            .AsNoTracking()
            .Where(a => a.Status == "Submitted" || a.Status == "Validating" || a.Status == "KycPending" || a.Status == "Approved" || a.Status == "CitizenCreating")
            .OrderBy(a => a.CreatedAt)
            .Take(10) // Process up to 10 applications per cycle
            .Select(a => new { a.Id, a.TenantId, a.Status })
            .ToListAsync(ct);

        if (pendingApps.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Processing {Count} pending applications", pendingApps.Count);

        foreach (var app in pendingApps)
        {
            try
            {
                // Process workflow step directly using the workflow service
                // We'll create a tenant-aware workflow processor
                await ProcessApplicationWorkflowStepAsync(app.Id, app.TenantId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing application {ApplicationId}", app.Id);
            }
        }
    }

    private async Task ProcessApplicationWorkflowStepAsync(Guid applicationId, string tenantId, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CitizenshipDbContext>();

        var app = await db.Applications
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (app == null)
        {
            return;
        }

        // Process workflow steps based on current status
        var processed = false;
        switch (app.Status)
        {
            case "Submitted":
                processed = await ValidateApplicationDirectAsync(db, app, ct);
                break;
            case "Validating":
                processed = await ProcessKycDirectAsync(db, app, ct);
                break;
            case "KycPending":
                processed = await AutoApproveIfReadyDirectAsync(db, app, ct);
                break;
            case "Approved":
                processed = await CreatePaymentPlanDirectAsync(db, app, ct);
                break;
        }

        // If processed successfully, try to process the next step
        if (processed)
        {
            await Task.Delay(100, ct);
            await ProcessApplicationWorkflowStepAsync(applicationId, tenantId, ct);
        }
    }

    private async Task<bool> ValidateApplicationDirectAsync(CitizenshipDbContext db, CitizenshipApplicationRow app, CancellationToken ct)
    {
        if (app.Status != "Submitted")
        {
            return false;
        }

        // Basic validation checks
        var hasRequiredFields = !string.IsNullOrWhiteSpace(app.FirstName) &&
                                 !string.IsNullOrWhiteSpace(app.LastName) &&
                                 !string.IsNullOrWhiteSpace(app.Email) &&
                                 app.DateOfBirth != default;

        var hasRequiredConsents = app.AcknowledgeTreaty &&
                                  app.SwearAllegiance &&
                                  app.ConsentToVerification &&
                                  app.ConsentToDataProcessing;

        var hasDocuments = app.Uploads.Any(u => u.Kind == "PersonalDocument") &&
                           app.Uploads.Any(u => u.Kind == "PassportPhoto") &&
                           app.Uploads.Any(u => u.Kind == "Signature");

        var isValid = hasRequiredFields && hasRequiredConsents && hasDocuments;

        app.Status = isValid ? "Validating" : "Rejected";
        app.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return isValid;
    }

    private async Task<bool> ProcessKycDirectAsync(CitizenshipDbContext db, CitizenshipApplicationRow app, CancellationToken ct)
    {
        if (app.Status != "Validating")
        {
            return false;
        }

        // Basic KYC checks
        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.DateTime);
        var age = today.Year - app.DateOfBirth.Year;
        if (app.DateOfBirth > today.AddYears(-age)) age--;

        var ageValid = age >= 18 && age <= 120;
        var kycPassed = ageValid;

        app.Status = kycPassed ? "KycPending" : "Rejected";
        app.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return kycPassed;
    }

    private async Task<bool> AutoApproveIfReadyDirectAsync(CitizenshipDbContext db, CitizenshipApplicationRow app, CancellationToken ct)
    {
        if (app.Status != "KycPending")
        {
            return false;
        }

        // Check if intake review exists and is positive
        var intakeReview = await db.IntakeReviews
            .Where(r => r.ApplicationId == app.Id && r.TenantId == app.TenantId)
            .OrderByDescending(r => r.ReviewDate)
            .FirstOrDefaultAsync(ct);

        // If intake review exists and recommends approval, auto-approve
        if (intakeReview != null && intakeReview.Recommendation == "Approve")
        {
            app.Status = "Approved";
            app.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            return true;
        }

        return false;
    }

    private async Task<bool> CreatePaymentPlanDirectAsync(CitizenshipDbContext db, CitizenshipApplicationRow app, CancellationToken ct)
    {
        if (app.Status != "Approved")
        {
            return false;
        }

        // Check if payment plan already exists
        var existingPlan = await db.PaymentPlans
            .Where(p => p.ApplicationId == app.Id && p.TenantId == app.TenantId)
            .FirstOrDefaultAsync(ct);

        if (existingPlan != null)
        {
            return true; // Payment plan already exists
        }

        // Create payment plan
        var now = DateTimeOffset.UtcNow;
        var plan = new PaymentPlanRow
        {
            Id = Guid.NewGuid(),
            TenantId = app.TenantId,
            ApplicationId = app.Id,
            ApplicationNumber = app.ApplicationNumber,
            Amount = 150.00m, // Placeholder - should be tenant-configurable
            Currency = "USD",
            Status = "Pending",
            CreatedAt = now,
            UpdatedAt = now
        };

        db.PaymentPlans.Add(plan);

        // Update application status to PaymentPending
        app.Status = "PaymentPending";
        app.UpdatedAt = now;

        await db.SaveChangesAsync(ct);
        return true;
    }
}


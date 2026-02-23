using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class PaymentPlanService : IPaymentPlanService
{
    private const decimal DefaultApplicationFee = 150.00m; // Placeholder - should be tenant-configurable
    private readonly ITenantContext _tenant;
    private readonly IPaymentPlanStore _store;

    public PaymentPlanService(
        ITenantContext tenant,
        IPaymentPlanStore store)
    {
        _tenant = tenant;
        _store = store;
    }

    public async Task<string> CreatePaymentPlanAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);

        if (app == null)
        {
            throw new InvalidOperationException($"Application {applicationId} not found.");
        }

        if (app.Status != "Approved")
        {
            throw new InvalidOperationException($"Application must be Approved to create payment plan. Current status: {app.Status}");
        }

        var existing = await _store.GetPaymentPlanSnapshotAsync(tenantId, applicationId, ct);
        if (existing != null)
        {
            return existing.Id.ToString();
        }

        var now = DateTimeOffset.UtcNow;
        return await _store.CreatePaymentPlanAsync(
            tenantId,
            applicationId,
            app.ApplicationNumber,
            DefaultApplicationFee,
            "USD",
            now,
            ct);
    }

    public Task<PaymentPlanDto?> GetPaymentPlanAsync(Guid applicationId, CancellationToken ct = default)
    {
        return _store.GetPaymentPlanAsync(_tenant.TenantId, applicationId, ct);
    }

    public async Task<bool> ConfirmPaymentAsync(Guid applicationId, string paymentReference, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var plan = await _store.GetPaymentPlanSnapshotAsync(tenantId, applicationId, ct);
        if (plan == null || plan.Status != "Pending")
        {
            return false;
        }

        var app = await _store.GetApplicationAsync(tenantId, applicationId, ct);
        if (app == null || app.Status != "PaymentPending")
        {
            return false;
        }

        return await _store.MarkPaymentPaidAsync(tenantId, applicationId, paymentReference, DateTimeOffset.UtcNow, ct);
    }
}

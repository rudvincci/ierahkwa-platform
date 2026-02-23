using Mamey.Portal.Citizenship.Domain.Entities;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IPaymentPlanRepository
{
    Task<PaymentPlan?> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default);
    Task SaveAsync(PaymentPlan plan, CancellationToken ct = default);
}

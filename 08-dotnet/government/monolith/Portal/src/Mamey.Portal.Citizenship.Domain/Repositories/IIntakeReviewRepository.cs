using Mamey.Portal.Citizenship.Domain.Entities;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IIntakeReviewRepository
{
    Task<IntakeReview?> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default);
    Task SaveAsync(IntakeReview review, CancellationToken ct = default);
}

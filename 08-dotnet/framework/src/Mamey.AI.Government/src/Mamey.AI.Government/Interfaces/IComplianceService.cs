namespace Mamey.AI.Government.Interfaces;

public interface IComplianceService
{
    Task<bool> CheckComplianceAsync(object entityData, string regulationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetViolationsAsync(object entityData, CancellationToken cancellationToken = default);
}

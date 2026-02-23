namespace Mamey.Policies;

public interface IPolicyHandler<TPolicy> where TPolicy : IPolicy
{
    Task<bool> EvaluateAsync(TPolicy policy, CancellationToken cancellationToken);
}

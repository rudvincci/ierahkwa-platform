namespace Mamey.Http;

internal class EmptyCorrelationContextFactory : ICorrelationContextFactory
{
    public string Create() => default;
}
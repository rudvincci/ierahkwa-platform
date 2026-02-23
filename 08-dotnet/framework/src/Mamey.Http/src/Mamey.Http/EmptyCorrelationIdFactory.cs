namespace Mamey.Http;

internal class EmptyCorrelationIdFactory : ICorrelationIdFactory
{
    public string Create() => default;
}
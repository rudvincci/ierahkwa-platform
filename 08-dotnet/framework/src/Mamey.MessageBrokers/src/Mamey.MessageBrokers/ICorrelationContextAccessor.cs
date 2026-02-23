namespace Mamey.MessageBrokers;

public interface ICorrelationContextAccessor
{
    object CorrelationContext { get; set; }
}
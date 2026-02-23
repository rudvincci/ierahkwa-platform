using Mamey.CQRS;

namespace Mamey.Types;

public interface IAggregateRoot<T>
{
    T Id { get; protected set; }
    int Version { get; protected set; }
    IEnumerable<IDomainEvent> Events { get; }

    void ClearEvents();
    (bool, List<ValidationResult>?) Validate(bool throwException = true);
}
public interface IAggregateRoot : IAggregateRoot<AggregateId>
{

}
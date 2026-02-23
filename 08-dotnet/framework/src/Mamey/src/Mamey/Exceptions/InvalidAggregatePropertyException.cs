namespace Mamey.Exceptions;

public class InvalidAggregatePropertyException<TEntity> : DomainException
{
    public InvalidAggregatePropertyException(string propertyName, string message)
        : base($"{propertyName}:{message}")
    {
    }

    public override string Code => "validation_property";
}


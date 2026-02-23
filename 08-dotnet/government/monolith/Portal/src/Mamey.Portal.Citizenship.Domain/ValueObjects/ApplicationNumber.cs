namespace Mamey.Portal.Citizenship.Domain.ValueObjects;

public readonly record struct ApplicationNumber(string Value)
{
    public override string ToString() => Value;
}

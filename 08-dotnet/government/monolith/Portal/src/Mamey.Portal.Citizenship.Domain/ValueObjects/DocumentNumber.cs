namespace Mamey.Portal.Citizenship.Domain.ValueObjects;

public readonly record struct DocumentNumber(string Value)
{
    public override string ToString() => Value;
}

namespace Mamey.Portal.Citizenship.Domain.ValueObjects;

public readonly record struct DocumentKind(string Value)
{
    public override string ToString() => Value;
}

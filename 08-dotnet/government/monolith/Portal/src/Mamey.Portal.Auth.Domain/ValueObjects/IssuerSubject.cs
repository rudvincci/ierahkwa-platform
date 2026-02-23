namespace Mamey.Portal.Auth.Domain.ValueObjects;

public readonly record struct IssuerSubject(string Issuer, string Subject)
{
    public override string ToString() => $"{Issuer}:{Subject}";
}

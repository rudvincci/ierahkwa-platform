namespace Mamey.Identity.AspNetCore.ValueObjects;

/// <summary>
/// Represents the scope of a claim (e.g., "Global", "Tenant", "Resource").
/// </summary>
public readonly record struct ClaimScope(string Value)
{
    /// <summary>
    /// Global scope.
    /// </summary>
    public static ClaimScope Global   => new("Global");

    /// <summary>
    /// Tenant‑wide scope.
    /// </summary>
    public static ClaimScope Tenant   => new("Tenant");

    /// <summary>
    /// Resource‑specific scope.
    /// </summary>
    public static ClaimScope Resource => new("Resource");

    public override string ToString() => Value;
    public static implicit operator string(ClaimScope cs) => cs.Value;
}


namespace Mamey.Auth.Identity.ValueObjects;

/// <summary>
/// Defines the level of a permission (e.g., View=1, Edit=2, Admin=3).
/// </summary>
public readonly record struct PermissionLevel(int Value)
{
    public static PermissionLevel View  => new(1);
    public static PermissionLevel Edit  => new(2);
    public static PermissionLevel Admin => new(3);

    public override string ToString() => Value.ToString();
    public static implicit operator int(PermissionLevel pl) => pl.Value;
}
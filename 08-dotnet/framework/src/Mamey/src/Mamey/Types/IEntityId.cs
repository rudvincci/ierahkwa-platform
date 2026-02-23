namespace Mamey.Types;

/// <summary>
/// Marker interface for strongly-typed entity identifiers.
/// Implement on value objects (typically <c>readonly record struct</c>) that serve as entity IDs.
/// </summary>
/// <typeparam name="T">The underlying primitive type of the identifier (usually <see cref="Guid"/>).</typeparam>
public interface IEntityId<out T>
{
    T Value { get; }
}

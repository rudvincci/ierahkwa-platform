namespace Mamey.Types;

/// <summary>
/// Marker interface for strongly-typed aggregate root identifiers.
/// </summary>
/// <typeparam name="T">The underlying primitive type (usually <see cref="Guid"/>).</typeparam>
public interface IAggregateId<out T>
{
    T Value { get; }
}

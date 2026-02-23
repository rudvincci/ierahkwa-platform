namespace Mamey.Types;

/// <summary>
/// Base initializer contract for modules.
/// Note: Mamey.MicroMonolith.Infrastructure also defines IInitializer.
/// They are intentionally kept in sync. If both namespaces are imported,
/// use a fully-qualified name or using alias to disambiguate.
/// </summary>
public interface IInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Represents a controller, which may be one or more DIDs.
/// </summary>
public class DidController
{
    public IReadOnlyList<string> Controllers { get; }

    public DidController(IEnumerable<string> controllers)
    {
        Controllers = new List<string>(controllers);
    }
}
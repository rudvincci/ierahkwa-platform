namespace Mamey.Auth.DecentralizedIdentifiers;

/// <summary>
/// Allows defining custom verification relationships beyond the standard set.
/// </summary>
public static class CustomVerificationRelationships
{
    private static readonly Dictionary<string, string> _customRelationships = new();

    /// <summary>
    /// Registers a custom verification relationship.
    /// </summary>
    public static void Register(string relationshipName, string description)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentNullException(nameof(relationshipName));
        _customRelationships[relationshipName] = description;
    }

    /// <summary>
    /// Looks up a custom relationship's description.
    /// </summary>
    public static string GetDescription(string relationshipName)
    {
        _customRelationships.TryGetValue(relationshipName, out var desc);
        return desc;
    }

    /// <summary>
    /// Enumerates all registered custom relationships.
    /// </summary>
    public static IReadOnlyDictionary<string, string> GetAll() => _customRelationships;
}
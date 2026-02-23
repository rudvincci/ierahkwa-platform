namespace Mamey.Identity.AspNetCore.Configuration;

/// <summary>
/// Static definitions of all supported feature flags.
/// </summary>
public sealed class FeatureDefinition
{
    public string Name { get; }
    public string Description { get; }

    private FeatureDefinition(string name, string description)
    {
        Name        = name;
        Description = description;
    }

    public static IReadOnlyList<FeatureDefinition> All { get; } = new[]
    {
        new FeatureDefinition("BetaDashboard", "Enables the beta UI dashboard"),
        new FeatureDefinition("AdvancedSecurity", "Activates extra security checks"),
        new FeatureDefinition("UserImpersonation", "Allows administrators to impersonate users"),
        // add other features here…
    };
}

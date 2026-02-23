namespace Mamey.Government.Shared.Abstractions;

/// <summary>
/// Shared seed data generator for consistent IDs across all modules.
/// Uses deterministic GUID generation based on seeds to ensure relationships work.
/// </summary>
public static class SeedData
{
    private static readonly Guid TenantIdValue = Guid.Parse("6d1c92c5-eb7c-4bf1-ac35-eb4e3db9c115");
    
    public static Guid TenantId => TenantIdValue;
    
    /// <summary>
    /// Generates a deterministic GUID from a seed string.
    /// Same seed always produces the same GUID.
    /// </summary>
    public static Guid GenerateDeterministicGuid(string seed)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(seed);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        var guidBytes = new byte[16];
        Array.Copy(hash, 0, guidBytes, 0, 16);
        return new Guid(guidBytes);
    }
    
    /// <summary>
    /// Generates a deterministic GUID from an integer index.
    /// </summary>
    public static Guid GenerateDeterministicGuid(int index, string prefix = "entity")
    {
        return GenerateDeterministicGuid($"{prefix}_{index}");
    }
    
    // Citizen IDs (used by Certificates, Passports, TravelIdentities)
    public static Guid GetCitizenId(int index) => GenerateDeterministicGuid(index, "citizen");
    
    // User IDs (used by Notifications)
    public static Guid GetUserId(int index) => GenerateDeterministicGuid(index, "user");
    
    // Application IDs (used by Citizens)
    public static Guid GetApplicationId(int index) => GenerateDeterministicGuid(index, "application");
}

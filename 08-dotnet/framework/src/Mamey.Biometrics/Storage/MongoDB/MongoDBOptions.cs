using System.ComponentModel.DataAnnotations;

namespace Mamey.Biometrics.Storage.MongoDB;

/// <summary>
/// MongoDB configuration options.
/// </summary>
public class MongoDBOptions
{
    /// <summary>
    /// Configuration section name for appsettings.json
    /// </summary>
    public const string SectionName = "MongoDB";

    /// <summary>
    /// MongoDB connection string
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database name
    /// </summary>
    [Required]
    public string DatabaseName { get; set; } = "fwid_biometrics";

    /// <summary>
    /// Collection name for templates
    /// </summary>
    public string TemplatesCollectionName { get; set; } = "biometric_templates";

    /// <summary>
    /// Enable SSL
    /// </summary>
    public bool UseSSL { get; set; } = false;

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Server selection timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int ServerSelectionTimeoutSeconds { get; set; } = 30;
}

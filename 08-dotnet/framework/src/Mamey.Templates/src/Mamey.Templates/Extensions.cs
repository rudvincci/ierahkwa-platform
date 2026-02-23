namespace Mamey.Templates;

public static class Extensions
{
    private const string SectionName = "temlates";
    private const string RegistryName = "temlates";
    public static IMameyBuilder AddMameyTemplates(this IMameyBuilder builder)
    {
        return builder;
    }
    public static IMameyBuilder AddTemplates(this IMameyBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var mongoOptions = builder.GetOptions<MameyTemplatesOptions>(sectionName);
        return builder;
    }
}

/// <summary>Runtime options for template fetching and policy.</summary>
public sealed class MameyTemplatesOptions
{
    /// <summary>Logical name of the calling service (used in default access context).</summary>
    public string ServiceName { get; set; } = "doc-render";

    /// <summary>Cache TTL for template binaries in Redis.</summary>
    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromHours(24);

    /// <summary>Whether to default-allow when no policy rows exist. Default=false (secure by default).</summary>
    public bool DefaultAllowWhenNoPolicy { get; set; } = false;

    /// <summary>MongoDB GridFS bucket name for template blobs.</summary>
    public string GridFsBucketName { get; set; } = "templates";

    /// <summary>Maximum upload size in bytes for an individual template (defensive limit).</summary>
    public long MaxUploadBytes { get; set; } = 20 * 1024 * 1024; // 20MB

    /// <summary>Optional: allowed content types (empty = allow any).</summary>
    public string[] AllowedContentTypes { get; set; } = new[]
    {
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/pdf"
    };
}
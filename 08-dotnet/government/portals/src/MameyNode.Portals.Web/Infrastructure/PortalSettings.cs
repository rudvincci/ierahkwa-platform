namespace MameyNode.Portals.Web.Infrastructure;

public class PortalSettings
{
    public string[] AuthMethods { get; set; } = Array.Empty<string>();
    public string Policy { get; set; } = "EitherOr";
    public string[] Routes { get; set; } = Array.Empty<string>();
}




namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal sealed class GeoLocationOptions
{
    public int? CacheTtlMinutes { get; set; }
    public string? LookupUrlTemplate { get; set; }
}

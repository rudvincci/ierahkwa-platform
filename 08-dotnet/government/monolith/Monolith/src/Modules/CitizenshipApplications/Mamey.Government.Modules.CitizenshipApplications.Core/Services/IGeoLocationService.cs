namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal interface IGeoLocationService
{
    Task<GeoLocationResult?> LookupAsync(
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);
}

internal sealed record GeoLocationResult(
    string? Country,
    string? CountryCode,
    string? Region,
    string? City,
    string? PostalCode,
    string? Timezone,
    double? Latitude,
    double? Longitude);

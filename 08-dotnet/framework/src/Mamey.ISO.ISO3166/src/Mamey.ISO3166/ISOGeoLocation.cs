using System.Text.Json.Serialization;

namespace Mamey.ISO3166;

public sealed class ISOGeoLocation 
{
    [JsonPropertyName("latitude")]
    public decimal? Latitude { get; set; }
    [JsonPropertyName("longitude")]
    public decimal? Longitude { get; set; }
    [JsonPropertyName("max_latitude")]
    public decimal? MaxLatitude { get; set; }
    [JsonPropertyName("max_longitude")]
    public decimal? MaxLongitude { get; set; }
    [JsonPropertyName("min_latitude")]
    public decimal? MinLatitude { get; set; }
    [JsonPropertyName("min_longitude")]
    public decimal? MinLongitude { get; set; }
}




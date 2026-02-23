using System.Text.Json.Serialization;

namespace Mamey.ISO.Abstractions;

public interface IISOGeoLocation
{
    [JsonPropertyName("latitude")]
    decimal Latitude { get; set; }
    [JsonPropertyName("longitude")]
    decimal Longitude { get; set; }
    [JsonPropertyName("max_latitude")]
    decimal MaxLatitude { get; set; }
    [JsonPropertyName("max_longitude")]
    decimal MaxLongitude { get; set; }
    [JsonPropertyName("min_latitude")]
    decimal MinLatitude { get; set; }
    [JsonPropertyName("min_longitude")]
    decimal MinLongitude { get; set; }
}


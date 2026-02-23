using System.Text.Json.Serialization;

namespace Mamey.ISO3166;

public sealed class ISOSubdivision 
{
    [JsonConstructor]
    public ISOSubdivision(string? name, string? code,// IEnumerable<string>? unofficialNames,
        ISOGeoLocation? geoLocation, Dictionary<string, string>? translations, string? comments)
    {
        Name = name;
        Code = code;
        //UnofficialNames = unofficialNames;
        GeoLocation = geoLocation;
        Translations = translations;
        Comments = comments;
    }



    public string? Name { get; set; }
    public string? Code { get; set; }
    //[JsonPropertyName("unofficial_names")]
    //public IEnumerable<string>? UnofficialNames { get; set; }
    [JsonPropertyName("geo")]
    public ISOGeoLocation? GeoLocation { get; set; }
    public Dictionary<string, string>? Translations { get; set; }
    public string? Comments { get; set; }
}




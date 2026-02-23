using System.Collections.Generic;

namespace Mamey.ISO.Abstractions;

public interface IISOSubdivision
{
    string Name { get; set; }
    string Code { get; set; }
    string UnofficialNames { get; set; }
    IISOGeoLocation GeoLocation { get; set; }
    Dictionary<string, string> Translations { get; set; }
    string Comments { get; set; }
}


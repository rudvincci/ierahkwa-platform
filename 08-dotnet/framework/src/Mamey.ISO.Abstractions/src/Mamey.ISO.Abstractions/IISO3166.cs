using System.Collections.Generic;

namespace Mamey.ISO.Abstractions;

public interface IISO3166
{
    Dictionary<string, IISOCountry> Countries { get; set; }
    Dictionary<string, IISOSubdivision> Subdivision { get; set; }

}


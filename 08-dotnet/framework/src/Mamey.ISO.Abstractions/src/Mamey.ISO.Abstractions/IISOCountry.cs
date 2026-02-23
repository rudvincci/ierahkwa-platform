using System.Collections.Generic;

namespace Mamey.ISO.Abstractions;

public interface IISOCountry
{
    string Alpha2 { get; set; }
    string Alpha3 { get; set; }
    string Continent { get; set; }
    string CountryCode { get; set; }
    string CurrencyCode { get; set; }
    string Gec { get; set; }
    IISOGeoLocation GeoLocation { get; set; }
    string InternationalPrefix { get; set; }
    string InternationalOlympicCode { get; set; }
    string ISOLongName { get; set; }
    string ISOShortName { get; set; }
    IEnumerable<string> OfficialLanguages { get; set; }
    IEnumerable<string> SpokenLanguages { get; set; }
    IEnumerable<int> NationalDestinationCodeLengths { get; set; }
    IEnumerable<int> NationalNumberLengths { get; set; }
    string NationalPrefix { get; set; }
    string Nationality { get; set; }
    string Number { get; set; }
    bool PostalCode { get; set; }
    string PostalCodeFormatRegex { get; set; }
    string Region { get; set; }
    string StartOfWeek { get; set; }
    string Subregion { get; set; }
    IEnumerable<string> UnofficialNames { get; set; }
    public string UnLocode { get; set; }
    string WorldRegion { get; set; }
}


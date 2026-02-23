using System.Text.Json.Serialization;

namespace Mamey.ISO3166;

public sealed class ISOCountry 
{
    public ISOCountry(string alpha2, string alpha3, string continent,
        string countryCode, string currencyCode, string gec, ISOGeoLocation geoLocation,
        string internationalPrefix, string internationalOlympicCode, string iSOLongName,
        string iSOShortName, IEnumerable<string> officialLanguages, IEnumerable<string> spokenLanguages,
        IEnumerable<int> nationalDestinationCodeLengths, IEnumerable<int> nationalNumberLengths, string nationalPrefix,
        string nationality, string number, bool postalCode, string postalCodeFormatRegex, string region, string startOfWeek,
        string subregion, string unLocode, IEnumerable<string> unofficialNames, string worldRegion)
    {
        Alpha2 = alpha2;
        Alpha3 = alpha3;
        Continent = continent;
        CountryCode = countryCode;
        CurrencyCode = currencyCode;
        Gec = gec;
        GeoLocation = geoLocation;
        InternationalPrefix = internationalPrefix;
        InternationalOlympicCode = internationalOlympicCode;
        ISOLongName = iSOLongName;
        ISOShortName = iSOShortName;
        OfficialLanguages = officialLanguages;
        SpokenLanguages = spokenLanguages;
        NationalDestinationCodeLengths = nationalDestinationCodeLengths;
        NationalNumberLengths = nationalNumberLengths;
        NationalPrefix = nationalPrefix;
        Nationality = nationality;
        Number = number;
        PostalCode = postalCode;
        PostalCodeFormatRegex = postalCodeFormatRegex;
        Region = region;
        StartOfWeek = startOfWeek;
        Subregion = subregion;
        UnLocode = unLocode;
        UnofficialNames = unofficialNames;
        WorldRegion = worldRegion;
    }


    /// <summary>
    /// Alpha-2 codes from ISO 3166-1
    /// </summary>
    [JsonPropertyName("alpha2")]
    public string Alpha2 { get; set; }
    /// <summary>
    /// Alpha-3 codes from ISO 3166-1 (synonymous with World Bank Codes)
    /// </summary>
    [JsonPropertyName("alpha3")]
    public string Alpha3 { get; set; }
    [JsonPropertyName("continent")]
    public string Continent { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Codes from the U.S. standard GEC
    /// </summary>
    [JsonPropertyName("gec")]
    public string Gec { get; set; }

    [JsonPropertyName("geo")]
    public ISOGeoLocation GeoLocation { get; set; }

    [JsonPropertyName("international_prefix")]
    public string InternationalPrefix { get; set; }
    /// <summary>
    /// Codes assigned by the International Olympics Committee . These codes identify the nationality of athletes and teams during Olympic events.
    /// </summary>
    [JsonPropertyName("ioc")]
    public string InternationalOlympicCode { get; set; }
    [JsonPropertyName("iso_long_name")]
    public string ISOLongName { get; set; }
    [JsonPropertyName("iso_short_name")]
    public string ISOShortName { get; set; }
    [JsonPropertyName("languages_official")]
    public IEnumerable<string> OfficialLanguages { get; set; }
    [JsonPropertyName("languages_spoken")]
    public IEnumerable<string> SpokenLanguages { get; set; }
    [JsonPropertyName("national_destination_code_lengths")]
    public IEnumerable<int> NationalDestinationCodeLengths { get; set; }
    [JsonPropertyName("national_number_lengths")]
    public IEnumerable<int> NationalNumberLengths { get; set; }
    [JsonPropertyName("national_prefix")]
    public string NationalPrefix { get; set; }
    [JsonPropertyName("nationality")]
    public string Nationality { get; set; }
    /// <summary>
    /// Numeric codes from ISO 3166-1
    /// </summary>
    [JsonPropertyName("number")]
    public string Number { get; set; }
    [JsonPropertyName("postal_code")]
    public bool PostalCode { get; set; }
    [JsonPropertyName("postal_code_format")]
    public string PostalCodeFormatRegex { get; set; }
    public string Region { get; set; }
    [JsonPropertyName("start_of_week")]
    public string StartOfWeek { get; set; }
    [JsonPropertyName("subregion")]
    public string Subregion { get; set; }
    [JsonPropertyName("un_locode")]
    public string UnLocode { get; set; }
    [JsonPropertyName("unofficial_names")]
    public IEnumerable<string> UnofficialNames { get; set; }
    [JsonPropertyName("world_region")]
    public string WorldRegion { get; set; }


}




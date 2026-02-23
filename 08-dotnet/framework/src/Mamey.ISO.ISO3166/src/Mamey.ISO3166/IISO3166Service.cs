using System.Linq.Expressions;

namespace Mamey.ISO3166;

public interface IISO3166Service
{
    Task InitializeAsync();
    Task<ISOCountry?> GetCountryAsync(string countryCode);
    Task<ISOSubdivision?> GetSubDivisionAsync(string countryCode);
    Task<IEnumerable<CountryPhoneCode>> GetContriesAreaCodesAsync();
    
    Task<Dictionary<string, ISOCountry>> GetAllCountriesAsync();
    Task<Dictionary<string, ISOSubdivision>> GetAllSubdivisionsAsync();

    Task<TProperty?> GetCountryPropertyAsync<TProperty>(string countryCode,
        Expression<Func<ISOCountry, TProperty>> propertyExpression);

    Task<TProperty?> GetSubdivisionPropertyAsync<TProperty>(string subdivisionCode,
        Expression<Func<ISOSubdivision, TProperty>> propertyExpression);
    List<KeyValuePair<string, ISOCountry>>? Countries { get; }
    /// <summary>UI-friendly list of country phone codes. Alphabetized by name.</summary>
    Task<IEnumerable<CountryPhoneCode>> GetCountriesAreaCodesAsync();
}


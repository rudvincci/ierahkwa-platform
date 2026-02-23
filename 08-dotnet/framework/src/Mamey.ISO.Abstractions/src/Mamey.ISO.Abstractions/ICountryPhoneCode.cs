namespace Mamey.ISO.Abstractions;

public interface ICountryPhoneCode
{
    string Name { get; set; }
    string CountryCode { get; set; }
    string PhoneCode { get; set; }
    string Flag { get; }

    bool Contains(string value);
    string ToString();
}
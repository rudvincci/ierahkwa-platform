namespace Mamey.ISO3166;

public sealed class CountryPhoneCode
{
    public CountryPhoneCode(string name, string alpha2, string phoneCode)
    {
        Name = name;
        Alpha2 = alpha2;
        PhoneCode = phoneCode;
    }

    public string Name { get; set; }
    public string Alpha2 { get; set; }
    public string PhoneCode { get; set; }
    public string Flag => $"https://countryflagsapi.com/svg/{Alpha2.ToLower()}";
    public override string ToString() => $"{Alpha2} - {Name}, (+{PhoneCode})";

    public bool Contains(string value)
        => ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase);
}


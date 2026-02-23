namespace Mamey.Mifos.Entities
{
    public record Address(IEnumerable<Option> CountryIdOptions, IEnumerable<Option> StateProvinceIdOptions, IEnumerable<Option> AddressTypeOptions);
}


// using Mamey.ISO.Abstractions;
// using Mamey.Types;
// using Microsoft.AspNetCore.Components;
//
//
// namespace Mamey.BlazorWasm.Components;
//
// public partial class PhoneTextField : ComponentBase
// {
//     [Inject]
//     public IISO3166Service _isoCountryService { get; set; }
//
//     [Parameter]
//     public Phone? Phone { get; set; }
//
//     ICountryPhoneCode _selectedCountryCode;
//
//     IEnumerable<ICountryPhoneCode> _phoneCodes = Enumerable.Empty<ICountryPhoneCode>();
//
//     protected override async Task OnInitializedAsync()
//     {
//         //_phoneCodes = (await _isoCountryService.GetContriesAreaCodesAsync()).OrderBy(c=> c.CountryCode);
//     }
//     protected override void OnParametersSet()
//     {
//         if(Phone is null)
//         {
//             throw new ArgumentException("Phone parameter is not set to an instance of an object.");
//         }
//     }
//     private async Task<IEnumerable<ICountryPhoneCode>> Search(string value)
//     {
//         // if text is null or empty, don't return values (drop-down will not open)
//         if (string.IsNullOrEmpty(value))
//             return _phoneCodes;
//         var filtered = _phoneCodes.Where(x => x.Contains(value));
//         return filtered;
//     }
//     private void CountryCodeChanged(ICountryPhoneCode countryPhoneCode)
//     {
//         Phone.CountryCode = countryPhoneCode.PhoneCode;
//     }
// }
//

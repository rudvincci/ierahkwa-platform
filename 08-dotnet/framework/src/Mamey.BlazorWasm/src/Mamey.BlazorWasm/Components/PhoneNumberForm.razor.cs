// using Mamey.ISO.Abstractions;
// using Mamey.Types;
// using Microsoft.AspNetCore.Components;
// using Mamey.ISO.ISO3166;

// namespace Mamey.BlazorWasm.Components;

// public partial class PhoneNumberForm : ComponentBase
// {

//     [Inject]
//     public ISO3166Service isoCountryService { get; set; }
//     [Parameter] public string? Title { get; set; } = "Phone Numbers";
//     [Parameter] public int Elevation { get; set; }
//     [Parameter] public List<Phone>? Model { get; set; }
//     [Parameter]
//     public EventCallback<Phone> OnPhoneChanged { get; set; }
    
//     private string _selectedCountryCode { get; set; } = "+1";
//     private string? _phone = string.Empty;
    
//     private IReadOnlyList<KeyValuePair<string, IISOCountry>>? _countries;

//     protected override async Task OnInitializedAsync()
//     {
//         _countries = await isoCountryService.Countries;
//     }

//     private async Task OnCountryCodeChanged(string value)
//     {
//         _selectedCountryCode = value;
//         await UpdatePhoneAsync();
//     }

//     private async Task OnPhoneNumberChanged(string value)
//     {
//         _phone = value;
//         await UpdatePhoneAsync();
//     }
//     private async Task UpdatePhoneAsync()
//     {
//         var phoneUpdate = new Phone(_selectedCountryCode, _phone);
//         await OnPhoneChanged.InvokeAsync(phoneUpdate);
//         StateHasChanged();
//     }
// }


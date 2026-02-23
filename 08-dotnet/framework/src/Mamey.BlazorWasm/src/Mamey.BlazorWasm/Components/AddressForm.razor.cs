using Mamey.Types;
using Microsoft.AspNetCore.Components;

namespace Mamey.BlazorWasm.Components;

public partial class AddressForm : ComponentBase
{
    private Address? _address = null;
    [Parameter] public Address? Address { get; set; } = null;
    [Parameter] public int Elevation { get; set; } = 1;
    [Parameter] public string? Title { get; set; } = "Address";
    [Parameter] public string? Class { get; set; }

    [Parameter] public EventCallback<Address> OnAddressChanged { get; set; }
    private string firmName = string.Empty;
    private string line1 = string.Empty;
    private string? line2 = string.Empty;
    private string? line3 = string.Empty;
    private string? urbanization = string.Empty;
    private string city = string.Empty;
    private string state = string.Empty;
    private string? province = string.Empty;
    private string zip5 = string.Empty;
    private string country = string.Empty;

    private void UpdateAddress()
    {
        _address = new Address(

            firmName: firmName,
            line: line1,
            line2: line2,
            line3: line3,
            city: city,
            urbanization: urbanization,
            country: country,
            zip5: zip5,
            zip4: null,
            state: state,
            postalCode: null,
            province: null
        );
        OnAddressChanged.InvokeAsync(_address);
        StateHasChanged();
    }
}


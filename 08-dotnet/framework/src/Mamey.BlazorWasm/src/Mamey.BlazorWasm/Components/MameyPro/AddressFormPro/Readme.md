# AddressFormPro

A comprehensive address input component for Blazor built on **MudBlazor** with ISO3166 integration. Automatically adapts the form layout based on country selection (US vs non-US addresses) and uses the `Mamey.Types.Address` value object for type-safe address handling.

---

## Why this component exists

* **International support**: Full ISO3166 country and subdivision integration
* **Smart layout**: Automatically switches between US (ZIP) and non-US (Postal Code) layouts
* **Type-safe**: Uses `Mamey.Types.Address` value object for guaranteed valid addresses
* **Comprehensive**: Supports firm name, multiple address lines, urbanization, and address types
* **User-friendly**: Dropdown selectors for states/provinces when available

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<AddressFormPro @bind-Value="_address" />

@code {
    private Address? _address;
}
```

### With helper text

```razor
<AddressFormPro 
    HelperText="Enter your complete mailing address"
    @bind-Value="_address" />
```

---

## Feature tour

### Country-based layout

* **US addresses**: Shows State dropdown and ZIP5/ZIP+4 fields
* **Non-US addresses**: Shows Province/Region dropdown and Postal Code field
* **Automatic switching**: Layout changes when country selection changes
* **ISO3166 integration**: Uses `IISO3166Service` for country and subdivision data

### Address fields

* **Firm/Name**: Optional firm or recipient name
* **Address Line 1**: Required street address
* **Address Line 2**: Optional additional address line
* **City**: Required city name
* **State/Province**: Required state or province (dropdown when available)
* **ZIP/Postal Code**: Required ZIP5 (US) or Postal Code (non-US)
* **ZIP+4**: Optional 4-digit ZIP extension (US only)
* **Urbanization**: Optional urbanization field (US only)

### Validation

* Validates required fields based on country
* US addresses require State and ZIP5
* Non-US addresses require Province and Postal Code
* Shows validation errors on Save
* Emits `null` when invalid, valid `Address` object when valid

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `Address?` | `null` | Two-way bindable address value object |
| `ValueChanged` | `EventCallback<Address?>` | - | Event fired when address changes |
| `HelperText` | `string` | `"US vs non-US automatically toggles ZIP vs Postal and uses ISO-3166 subdivisions when available."` | Helper text below form |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |

---

## Usage examples

### Basic address input

```razor
<AddressFormPro @bind-Value="_address" />

@code {
    private Address? _address;
    
    private void OnSubmit()
    {
        if (_address is not null)
        {
            // Address is guaranteed to be valid
            Console.WriteLine($"Address: {_address.Line}, {_address.City}, {_address.State}");
        }
    }
}
```

### US address

The component automatically shows US-specific fields when "United States" is selected:

* State dropdown (with all 50 states + territories)
* ZIP5 field (required, 5 digits)
* ZIP+4 field (optional, 4 digits)
* Urbanization field (optional)

### Non-US address

When a non-US country is selected, the component shows:

* Province/Region dropdown (when available from ISO3166)
* Postal Code field (required)
* State field (optional)

### With EditForm

```razor
<EditForm Model="@_contact" OnValidSubmit="SaveContact">
    <DataAnnotationsValidator />
    
    <AddressFormPro 
        HelperText="Enter your mailing address"
        @bind-Value="_contact.Address" />
    
    <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
</EditForm>
```

---

## Address types

The component supports address types from `Address.AddressType` enum:

* `Main` - Primary address
* `Home` - Home address
* `Work` - Work address
* `Billing` - Billing address
* `Shipping` - Shipping address
* `Other` - Other address type

---

## Country and subdivision data

The component uses `IISO3166Service` to load:

* **Countries**: All ISO3166 countries with Alpha2 codes
* **Subdivisions**: States, provinces, regions for selected country
* **Fallback**: US states list if ISO service unavailable

### Required service registration

```csharp
// In Program.cs or Startup.cs
builder.Services.AddScoped<IISO3166Service, ISO3166Service>();
```

---

## Form workflow

1. **User enters address fields**: All fields are editable
2. **User selects country**: Form layout changes based on country
3. **User clicks Save**: Component validates and creates `Address` object
4. **Validation passes**: `ValueChanged` fires with valid `Address`
5. **Validation fails**: `ValueChanged` fires with `null` and error is shown

### Reset functionality

The component includes a Reset button that:
* Clears all address fields
* Resets country to "US"
* Clears the `Value` parameter

---

## Integration with forms

Works seamlessly with Blazor's `EditForm`:

```razor
<EditForm Model="@_user" OnValidSubmit="SaveUser">
    <DataAnnotationsValidator />
    
    <AddressFormPro 
        HelperText="Enter your mailing address"
        @bind-Value="_user.Address" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class User
    {
        public Address? Address { get; set; }
    }
    
    private User _user = new();
}
```

---

## Troubleshooting

### Address is always null

**Problem**: The `Value` parameter is always `null` even after filling all fields.

**Solution**: The component validates on Save button click. Make sure the user clicks the Save button after filling the form. The component emits `null` during editing and only emits a valid `Address` object after Save.

### Country dropdown is empty

**Problem**: Country dropdown shows "United States" only or is empty.

**Solution**: Ensure `IISO3166Service` is registered in your DI container:

```csharp
builder.Services.AddScoped<IISO3166Service, ISO3166Service>();
```

### State/Province dropdown is empty

**Problem**: State or Province dropdown is empty even after selecting a country.

**Solution**: The component loads subdivisions from ISO3166 service. If the service is unavailable or doesn't have data for the selected country, the component falls back to a text input. For US addresses, there's a hardcoded fallback list of states.

### Validation errors not showing

**Problem**: Invalid addresses don't show error messages.

**Solution**: The component validates on Save button click. Make sure the user clicks Save. Validation errors are shown via console output (check browser console) and the component emits `null` when validation fails.

### ZIP code validation

**Problem**: ZIP code accepts invalid formats.

**Solution**: The `Address` value object enforces ZIP5 format (exactly 5 digits for US addresses). The component validates this on Save. Ensure users enter exactly 5 digits for US ZIP codes.

---

## Accessibility

* Proper label association via MudBlazor
* Keyboard navigation support for all inputs
* Screen reader friendly field labels
* ARIA attributes handled by MudBlazor
* Clear visual distinction between required and optional fields

---

## Styling & theming

Customize appearance using MudBlazor theme:

```razor
<AddressFormPro 
    Class="custom-address-form"
    Style="max-width: 800px;"
    @bind-Value="_address" />
```

The component uses `MudPaper` with padding. You can override styles:

```css
.custom-address-form {
    background-color: var(--mud-palette-background);
}
```

---

## Performance

* Country data loaded once on initialization
* Subdivision data loaded when country changes
* Efficient re-renders with proper state management
* Snapshot-based change detection to prevent unnecessary updates

---

## Security

* Address validation follows standard address format rules
* No client-side data storage
* Safe for use in forms with sensitive data
* ISO3166 data loaded from trusted service

---

## Related components

* `EmailInputPro` - Email input component
* `PhoneNumberInputPro` - Phone number input component
* `NameInputPro` - Name input component

---

## Advanced usage

### Programmatic address creation

```razor
<AddressFormPro @bind-Value="_address" />

@code {
    private Address? _address;
    
    private void SetDefaultAddress()
    {
        _address = new Address(
            firmName: "Acme Corp",
            line: "123 Main St",
            line2: "Suite 100",
            line3: null,
            urbanization: null,
            city: "New York",
            state: "NY",
            zip5: "10001",
            zip4: "1234",
            postalCode: null,
            country: "US",
            province: null,
            isDefault: true,
            type: Address.AddressType.Work
        );
    }
}
```

### Address type selection

The component currently uses `Address.AddressType.Main` as default. To support address type selection, you can extend the component or handle it in your parent component.






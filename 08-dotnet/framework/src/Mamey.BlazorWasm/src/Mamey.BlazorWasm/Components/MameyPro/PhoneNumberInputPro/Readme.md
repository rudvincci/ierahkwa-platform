# PhoneNumberInputPro

A comprehensive phone number input component for Blazor built on **MudBlazor** that supports country codes, extensions, phone types, and ISO3166 integration. Uses the `Mamey.Types.Phone` value object for type-safe phone number handling.

---

## Why this component exists

* **International support**: Full country code selection with ISO3166 integration
* **Flexible input**: Supports phone type, extension, and country code selection
* **Type-safe**: Uses `Mamey.Types.Phone` value object for guaranteed valid phone numbers
* **User-friendly**: Autocomplete or dropdown for country selection
* **Feature-rich**: Optional phone type and extension fields

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<PhoneNumberInputPro @bind-Value="_phone" />

@code {
    private Phone? _phone;
}
```

### With all features enabled

```razor
<PhoneNumberInputPro 
    Label="Contact Phone"
    ShowType="true"
    ShowExtension="true"
    RequireExtension="false"
    @bind-Value="_phone" />
```

---

## Feature tour

### Country selection

* **Dropdown mode** (default): Standard select dropdown with country list
* **Autocomplete mode**: Searchable autocomplete for faster country selection
* **ISO3166 integration**: Uses `IISO3166Service` for country data
* **Smart grouping**: Groups countries with same dial code (e.g., all +1 countries)

### Phone type

* Optional phone type selector (Main, Home, Mobile, Fax, Other)
* Configurable allowed types
* Default type selection

### Extension

* Optional extension field
* Required extension option
* Custom extension validation

### Validation

* Validates phone number format using `Mamey.Types.Phone` constructor
* Validates extension if required
* Shows clear error messages
* Emits `null` when invalid, valid `Phone` object when valid

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `Phone?` | `null` | Two-way bindable phone value object |
| `ValueChanged` | `EventCallback<Phone?>` | - | Event fired when phone changes |
| `ValueExpression` | `Expression<Func<Phone?>>?` | `null` | Expression for EditForm validation |
| `Label` | `string` | `"Phone number"` | Label text for the phone number input |
| `Placeholder` | `string` | `"5551234567"` | Placeholder text for phone number |
| `HelperText` | `string` | `"Stored as +<country> <number>"` | Helper text below input |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |
| `ShowType` | `bool` | `false` | Show phone type selector |
| `AllowedTypes` | `IEnumerable<Phone.PhoneType>?` | `null` | Allowed phone types (null = all) |
| `DefaultType` | `Phone.PhoneType` | `Phone.PhoneType.Main` | Default phone type |
| `ShowExtension` | `bool` | `false` | Show extension field |
| `RequireExtension` | `bool` | `false` | Require extension when shown |
| `ValidateExtension` | `Func<string, bool>?` | `null` | Custom extension validation |
| `ShowAllCountriesEvenIfSameCode` | `bool` | `false` | Show all countries even with same dial code |
| `UseAutocompleteForCountry` | `bool` | `false` | Use autocomplete instead of dropdown |
| `DefaultCallingCode` | `string` | `"1"` | Default country calling code (NANP) |
| `EmitDefaultOnInit` | `bool` | `false` | Emit default phone on initialization |

---

## Usage examples

### Basic phone input

```razor
<PhoneNumberInputPro @bind-Value="_phone" />

@code {
    private Phone? _phone;
}
```

### Mobile phone with type

```razor
<PhoneNumberInputPro 
    Label="Mobile Phone"
    ShowType="true"
    AllowedTypes="@(new[] { Phone.PhoneType.Mobile, Phone.PhoneType.Main })"
    DefaultType="Phone.PhoneType.Mobile"
    @bind-Value="_mobilePhone" />
```

### Work phone with extension

```razor
<PhoneNumberInputPro 
    Label="Work Phone"
    ShowType="true"
    ShowExtension="true"
    RequireExtension="true"
    DefaultType="Phone.PhoneType.Main"
    @bind-Value="_workPhone" />
```

### Autocomplete country selection

```razor
<PhoneNumberInputPro 
    UseAutocompleteForCountry="true"
    ShowAllCountriesEvenIfSameCode="true"
    @bind-Value="_phone" />
```

### Custom extension validation

```razor
<PhoneNumberInputPro 
    ShowExtension="true"
    RequireExtension="true"
    ValidateExtension="@(ext => ext.Length >= 3 && ext.Length <= 6)"
    @bind-Value="_phone" />
```

### With EditForm

```razor
<EditForm Model="@_contact" OnValidSubmit="SaveContact">
    <DataAnnotationsValidator />
    
    <PhoneNumberInputPro 
        Label="Primary Phone"
        ShowType="true"
        Required="true"
        @bind-Value="_contact.Phone" />
    
    <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
</EditForm>
```

---

## Phone type options

The component supports the following phone types (from `Phone.PhoneType` enum):

* `Main` - Primary phone number
* `Home` - Home phone number
* `Mobile` - Mobile/cell phone number
* `Fax` - Fax number
* `Other` - Other type

---

## Country selection modes

### Dropdown mode (default)

Standard select dropdown with all countries. Countries with the same dial code are grouped (e.g., all +1 countries show as "United States (+1)").

```razor
<PhoneNumberInputPro 
    UseAutocompleteForCountry="false"
    @bind-Value="_phone" />
```

### Autocomplete mode

Searchable autocomplete that opens on focus and shows all countries. Useful for faster selection when you know the country name.

```razor
<PhoneNumberInputPro 
    UseAutocompleteForCountry="true"
    @bind-Value="_phone" />
```

### Show all countries

By default, countries with the same dial code are grouped. Set `ShowAllCountriesEvenIfSameCode="true"` to show all ~249 countries individually.

```razor
<PhoneNumberInputPro 
    ShowAllCountriesEvenIfSameCode="true"
    @bind-Value="_phone" />
```

---

## Integration with forms

Works seamlessly with Blazor's `EditForm`:

```razor
<EditForm Model="@_user" OnValidSubmit="SaveUser">
    <DataAnnotationsValidator />
    
    <PhoneNumberInputPro 
        Label="Phone Number"
        ShowType="true"
        Required="true"
        @bind-Value="_user.Phone" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class User
    {
        public Phone? Phone { get; set; }
    }
    
    private User _user = new();
}
```

---

## Troubleshooting

### Phone is always null

**Problem**: The `Value` parameter is always `null` even after entering a valid phone number.

**Solution**: The component validates on blur. Make sure the user has focused and then blurred the phone number input. The component emits `null` during typing and only emits a valid `Phone` object after validation.

### Country dropdown is empty

**Problem**: Country dropdown shows "Loading..." and never populates.

**Solution**: Ensure `IISO3166Service` is registered in your DI container. The component requires this service to load country data.

```csharp
// In Program.cs or Startup.cs
builder.Services.AddScoped<IISO3166Service, ISO3166Service>();
```

### Extension validation not working

**Problem**: Invalid extensions are accepted.

**Solution**: Ensure `RequireExtension="true"` is set if extension is required, and provide a `ValidateExtension` function for custom validation rules.

### Phone type not showing

**Problem**: Phone type selector is not visible.

**Solution**: Set `ShowType="true"` to enable the phone type selector.

---

## Accessibility

* Proper label association via MudBlazor
* Keyboard navigation support for all inputs
* Screen reader friendly error messages
* ARIA attributes handled by MudBlazor
* Autocomplete supports keyboard navigation

---

## Styling & theming

Customize appearance using MudBlazor theme:

```razor
<PhoneNumberInputPro 
    Class="custom-phone-input"
    Style="max-width: 600px;"
    @bind-Value="_phone" />
```

---

## Performance

* Country data loaded once on initialization
* Validation only occurs on blur
* Efficient re-renders with proper state management
* Autocomplete limits results to 200 items for performance

---

## Security

* Phone validation follows standard phone number format rules
* No client-side data storage
* Safe for use in forms with sensitive data

---

## Related components

* `EmailInputPro` - Email input component
* `AddressFormPro` - Full address form
* `NameInputPro` - Name input component






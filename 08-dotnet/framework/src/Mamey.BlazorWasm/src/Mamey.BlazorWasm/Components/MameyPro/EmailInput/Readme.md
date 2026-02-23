# EmailInputPro

A production-grade email input component for Blazor built on **MudBlazor** that validates and stores email addresses using the `Mamey.Types.Email` value object.

---

## Why this component exists

* **Type-safe validation**: Uses `Mamey.Types.Email` value object for guaranteed valid email addresses
* **Immediate feedback**: Validates on blur with clear error messages
* **Consistent API**: Follows MameyPro component patterns with two-way binding
* **Accessibility**: Built on MudBlazor with proper ARIA labels and keyboard navigation

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<EmailInputPro @bind-Value="_email" />

@code {
    private Email? _email;
}
```

### With validation and helper text

```razor
<EmailInputPro 
    Label="Email Address"
    Placeholder="name@example.com"
    HelperText="We'll send a confirmation email to this address"
    Required="true"
    @bind-Value="_email" />
```

---

## Feature tour

### Validation

* Validates email format using `Mamey.Types.Email` constructor
* Shows error message on blur if invalid
* Clears error when valid email is entered
* Emits `null` when invalid, valid `Email` object when valid

### Styling

* Supports all MudBlazor `Variant` options (Text, Outlined, Filled)
* Customizable with `Class` and `Style` parameters
* Icon adornment (default: email icon)
* Responsive design

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `Email?` | `null` | Two-way bindable email value object |
| `ValueChanged` | `EventCallback<Email?>` | - | Event fired when email changes |
| `Label` | `string` | `"Email"` | Label text for the input |
| `Placeholder` | `string` | `"name@example.com"` | Placeholder text |
| `HelperText` | `string` | `"We'll validate and store a normalized Email VO"` | Helper text below input |
| `Required` | `bool` | `false` | Whether the field is required |
| `Disabled` | `bool` | `false` | Whether the input is disabled |
| `Variant` | `Variant` | `Variant.Text` | MudBlazor input variant |
| `Adornment` | `Adornment` | `Adornment.End` | Icon adornment position |
| `AdornmentIcon` | `string?` | `Icons.Material.Filled.Email` | Icon to display |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |

---

## Usage examples

### Basic email input

```razor
<EmailInputPro @bind-Value="_email" />

@code {
    private Email? _email;
    
    private void OnSubmit()
    {
        if (_email is not null)
        {
            // Email is guaranteed to be valid
            Console.WriteLine($"Valid email: {_email.Value}");
        }
    }
}
```

### Required email with custom styling

```razor
<EmailInputPro 
    Label="Work Email"
    Placeholder="work@company.com"
    HelperText="Your work email address"
    Required="true"
    Variant="Variant.Outlined"
    Class="my-4"
    @bind-Value="_workEmail" />
```

### Disabled state

```razor
<EmailInputPro 
    Label="Email (Read-only)"
    Disabled="true"
    Value="@_existingEmail" />
```

### With EditForm validation

```razor
<EditForm Model="@_model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    
    <EmailInputPro 
        Label="Email"
        Required="true"
        @bind-Value="_model.Email" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class Model
    {
        public Email? Email { get; set; }
    }
    
    private Model _model = new();
}
```

---

## Integration with forms

The component works seamlessly with Blazor's `EditForm` and validation:

```razor
<EditForm Model="@_user" OnValidSubmit="SaveUser">
    <DataAnnotationsValidator />
    
    <EmailInputPro 
        Label="Email Address"
        Required="true"
        @bind-Value="_user.Email" />
    
    <MudButton ButtonType="ButtonType.Submit" Variant="Variant.Filled">
        Save
    </MudButton>
</EditForm>
```

---

## Troubleshooting

### Email is always null

**Problem**: The `Value` parameter is always `null` even after entering a valid email.

**Solution**: Ensure you're using two-way binding with `@bind-Value`. The component emits `null` during typing and only emits a valid `Email` object on blur after validation.

```razor
<!-- Correct -->
<EmailInputPro @bind-Value="_email" />

<!-- Incorrect - won't update -->
<EmailInputPro Value="@_email" />
```

### Validation error not showing

**Problem**: Invalid emails don't show error messages.

**Solution**: The component validates on blur. Make sure the user has focused and then blurred the input field. The `Immediate="true"` parameter ensures validation happens on blur.

### Email value object throws exception

**Problem**: Getting exceptions when creating `Email` value object.

**Solution**: The component handles exceptions internally and displays them as error messages. If you're creating `Email` objects manually, ensure the email string is valid:

```csharp
try
{
    var email = new Email("invalid-email"); // Will throw
}
catch (Exception ex)
{
    // Handle validation error
}
```

---

## Accessibility

* Proper `label` association via MudBlazor
* Keyboard navigation support
* Screen reader friendly error messages
* ARIA attributes handled by MudBlazor

---

## Styling & theming

Customize appearance using MudBlazor theme variables or component parameters:

```razor
<EmailInputPro 
    Variant="Variant.Outlined"
    Adornment="Adornment.Start"
    AdornmentIcon="@Icons.Material.Filled.Mail"
    Class="custom-email-input"
    Style="max-width: 400px;" />
```

---

## Performance

* Validation only occurs on blur (not on every keystroke)
* Uses compiled value accessors for efficient updates
* Minimal re-renders with proper state management

---

## Security

* Email validation follows standard email format rules
* No client-side data storage
* Safe for use in forms with sensitive data

---

## Related components

* `PhoneNumberInputPro` - Phone number input with country codes
* `AddressFormPro` - Full address form
* `NameInputPro` - Name input component






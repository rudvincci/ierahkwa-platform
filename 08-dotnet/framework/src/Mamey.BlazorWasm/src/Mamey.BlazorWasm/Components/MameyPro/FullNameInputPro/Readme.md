# FullNameInputPro

A full name input component for Blazor built on **MudBlazor** that validates and stores full names using the `Mamey.Types.FullName` value object. Supports configurable maximum length validation.

---

## Why this component exists

* **Single-field input**: Simple single-field input for full names
* **Type-safe validation**: Uses `Mamey.Types.FullName` value object for guaranteed valid names
* **Length validation**: Configurable maximum length (default: 200 characters)
* **Immediate feedback**: Validates on blur with clear error messages
* **Consistent API**: Follows MameyPro component patterns

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<FullNameInputPro @bind-Value="_fullName" />

@code {
    private FullName? _fullName;
}
```

### With validation and helper text

```razor
<FullNameInputPro 
    Label="Full Name"
    Placeholder="John Doe"
    HelperText="Enter your complete full name"
    Required="true"
    @bind-Value="_fullName" />
```

---

## Feature tour

### Validation

* Validates name length (minimum 2 characters, configurable maximum)
* Validates name format using `Mamey.Types.FullName` constructor
* Shows error message on blur if invalid
* Clears error when valid name is entered
* Emits `null` when invalid, valid `FullName` object when valid

### Length control

* Default maximum length: 200 characters
* Configurable via `MaxLength` parameter
* Minimum length: 2 characters (enforced by value object)

### Styling

* Supports all MudBlazor `Variant` options (Text, Outlined, Filled)
* Customizable with `Class` and `Style` parameters
* Optional icon adornment
* Responsive design

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `FullName?` | `null` | Two-way bindable full name value object |
| `ValueChanged` | `EventCallback<FullName?>` | - | Event fired when full name changes |
| `Label` | `string` | `"Full Name"` | Label text for the input |
| `HelperText` | `string` | `"Enter your full name (2-200 characters)"` | Helper text below input |
| `Placeholder` | `string` | `"John Doe"` | Placeholder text |
| `Required` | `bool` | `false` | Whether the field is required |
| `Disabled` | `bool` | `false` | Whether the input is disabled |
| `Variant` | `Variant` | `Variant.Text` | MudBlazor input variant |
| `Adornment` | `Adornment` | `Adornment.None` | Icon adornment position |
| `AdornmentIcon` | `string?` | `null` | Icon to display |
| `MaxLength` | `int` | `200` | Maximum allowed length |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |

---

## Usage examples

### Basic full name input

```razor
<FullNameInputPro @bind-Value="_fullName" />

@code {
    private FullName? _fullName;
    
    private void OnSubmit()
    {
        if (_fullName is not null)
        {
            // Full name is guaranteed to be valid
            Console.WriteLine($"Full name: {_fullName.Value}");
        }
    }
}
```

### Required full name with custom styling

```razor
<FullNameInputPro 
    Label="Complete Name"
    Placeholder="Enter your full name"
    HelperText="This will appear on official documents"
    Required="true"
    Variant="Variant.Outlined"
    Class="my-4"
    @bind-Value="_fullName" />
```

### Custom maximum length

```razor
<FullNameInputPro 
    Label="Short Name"
    MaxLength="50"
    HelperText="Maximum 50 characters"
    @bind-Value="_shortName" />
```

### Disabled state

```razor
<FullNameInputPro 
    Label="Full Name (Read-only)"
    Disabled="true"
    Value="@_existingFullName" />
```

### With EditForm validation

```razor
<EditForm Model="@_model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    
    <FullNameInputPro 
        Label="Full Name"
        Required="true"
        @bind-Value="_model.FullName" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class Model
    {
        public FullName? FullName { get; set; }
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
    
    <FullNameInputPro 
        Label="Full Name"
        Required="true"
        @bind-Value="_user.FullName" />
    
    <MudButton ButtonType="ButtonType.Submit" Variant="Variant.Filled">
        Save
    </MudButton>
</EditForm>
```

---

## Troubleshooting

### Full name is always null

**Problem**: The `Value` parameter is always `null` even after entering a valid name.

**Solution**: Ensure you're using two-way binding with `@bind-Value`. The component emits `null` during typing and only emits a valid `FullName` object on blur after validation.

```razor
<!-- Correct -->
<FullNameInputPro @bind-Value="_fullName" />

<!-- Incorrect - won't update -->
<FullNameInputPro Value="@_fullName" />
```

### Validation error not showing

**Problem**: Invalid names don't show error messages.

**Solution**: The component validates on blur. Make sure the user has focused and then blurred the input field. The `Immediate="true"` parameter ensures validation happens on blur.

### Name too short error

**Problem**: Getting "Full name must be at least 2 characters" error.

**Solution**: The `FullName` value object requires a minimum of 2 characters. Ensure the name is at least 2 characters long.

### Name too long error

**Problem**: Getting "Full name exceeds the maximum allowed length" error.

**Solution**: The default maximum is 200 characters. If you need longer names, set `MaxLength` to a higher value:

```razor
<FullNameInputPro 
    MaxLength="500"
    @bind-Value="_fullName" />
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
<FullNameInputPro 
    Variant="Variant.Outlined"
    Adornment="Adornment.Start"
    AdornmentIcon="@Icons.Material.Filled.Person"
    Class="custom-full-name-input"
    Style="max-width: 400px;" />
```

---

## Performance

* Validation only occurs on blur (not on every keystroke)
* Uses compiled value accessors for efficient updates
* Minimal re-renders with proper state management

---

## Security

* Name validation follows standard name format rules
* No client-side data storage
* Safe for use in forms with sensitive data

---

## Related components

* `NameInputPro` - Multi-field name input (first, middle, last, nickname)
* `EmailInputPro` - Email input component
* `PhoneNumberInputPro` - Phone number input component

---

## Advanced usage

### Programmatic full name creation

```razor
<FullNameInputPro @bind-Value="_fullName" />

@code {
    private FullName? _fullName;
    
    private void SetDefaultName()
    {
        _fullName = new FullName("John Michael Doe", maxLength: 200);
    }
}
```

### Implicit conversion

The `FullName` value object supports implicit conversion to/from string:

```razor
<FullNameInputPro @bind-Value="_fullName" />

@code {
    private FullName? _fullName;
    
    private void DisplayName()
    {
        if (_fullName is not null)
        {
            // Implicit conversion to string
            string nameString = _fullName;
            Console.WriteLine(nameString);
            
            // Implicit conversion from string
            _fullName = "Jane Smith";
        }
    }
}
```






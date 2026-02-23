# NameInputPro

A comprehensive name input component for Blazor built on **MudBlazor** that handles first name, middle name, last name, and nickname. Uses the `Mamey.Types.Name` value object for type-safe name handling.

---

## Why this component exists

* **Multi-field input**: Handles all name components (first, middle, last, nickname)
* **Flexible display**: Optional middle name and nickname fields
* **Type-safe**: Uses `Mamey.Types.Name` value object for guaranteed valid names
* **Validation**: Validates required fields and name format
* **User-friendly**: Clear labels and placeholders for each field

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<NameInputPro @bind-Value="_name" />

@code {
    private Name? _name;
}
```

### Minimal (first and last name only)

```razor
<NameInputPro 
    ShowMiddleName="false"
    ShowNickname="false"
    @bind-Value="_name" />
```

---

## Feature tour

### Name fields

* **First Name**: Required field (if `Required="true"`)
* **Last Name**: Required field (if `Required="true"`)
* **Middle Name**: Optional field (shown if `ShowMiddleName="true"`)
* **Nickname**: Optional field (shown if `ShowNickname="true"`)

### Validation

* Validates required fields when `Required="true"`
* Validates name format using `Mamey.Types.Name` constructor
* Shows error messages on validation failure
* Emits `null` when invalid, valid `Name` object when valid

### Layout

* Responsive grid layout
* First and last name side-by-side on larger screens
* Middle name and nickname side-by-side when both shown
* Stacks vertically on mobile

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `Name?` | `null` | Two-way bindable name value object |
| `ValueChanged` | `EventCallback<Name?>` | - | Event fired when name changes |
| `Label` | `string` | `"Name"` | Label text (not currently displayed) |
| `HelperText` | `string` | `"Enter your full name"` | Helper text below input |
| `FirstNamePlaceholder` | `string` | `"First name"` | Placeholder for first name |
| `LastNamePlaceholder` | `string` | `"Last name"` | Placeholder for last name |
| `MiddleNamePlaceholder` | `string` | `"Middle name (optional)"` | Placeholder for middle name |
| `NicknamePlaceholder` | `string` | `"Nickname (optional)"` | Placeholder for nickname |
| `Required` | `bool` | `false` | Whether first and last name are required |
| `Disabled` | `bool` | `false` | Whether all inputs are disabled |
| `Variant` | `Variant` | `Variant.Text` | MudBlazor input variant |
| `ShowMiddleName` | `bool` | `true` | Show middle name field |
| `ShowNickname` | `bool` | `true` | Show nickname field |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |

---

## Usage examples

### Basic name input

```razor
<NameInputPro @bind-Value="_name" />

@code {
    private Name? _name;
    
    private void OnSubmit()
    {
        if (_name is not null)
        {
            // Name is guaranteed to be valid
            Console.WriteLine($"Full name: {_name.FullName}");
        }
    }
}
```

### Required name

```razor
<NameInputPro 
    Required="true"
    HelperText="First and last name are required"
    @bind-Value="_name" />
```

### Minimal name (first and last only)

```razor
<NameInputPro 
    ShowMiddleName="false"
    ShowNickname="false"
    @bind-Value="_name" />
```

### With custom placeholders

```razor
<NameInputPro 
    FirstNamePlaceholder="Given name"
    LastNamePlaceholder="Family name"
    MiddleNamePlaceholder="Middle initial or name"
    NicknamePlaceholder="Preferred name"
    @bind-Value="_name" />
```

### With EditForm

```razor
<EditForm Model="@_person" OnValidSubmit="SavePerson">
    <DataAnnotationsValidator />
    
    <NameInputPro 
        Required="true"
        @bind-Value="_person.Name" />
    
    <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
</EditForm>
```

---

## Name value object

The component uses `Mamey.Types.Name` which provides:

* `FirstName` - First name (required)
* `LastName` - Last name (required)
* `MiddleName` - Middle name (optional)
* `Nickname` - Nickname (optional)
* `FullName` - Full name in "First Middle Last" format
* `ShortFullName` - Short form "First Last"
* `GivenNames` - First and middle names
* `Initials` - Initials in "F.M.L." format

---

## Integration with forms

Works seamlessly with Blazor's `EditForm`:

```razor
<EditForm Model="@_user" OnValidSubmit="SaveUser">
    <DataAnnotationsValidator />
    
    <NameInputPro 
        Required="true"
        @bind-Value="_user.Name" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class User
    {
        public Name? Name { get; set; }
    }
    
    private User _user = new();
}
```

---

## Troubleshooting

### Name is always null

**Problem**: The `Value` parameter is always `null` even after entering names.

**Solution**: The component validates on blur. Make sure the user has focused and then blurred the first name or last name field. The component emits `null` during typing and only emits a valid `Name` object after validation.

### Validation error not showing

**Problem**: Invalid names don't show error messages.

**Solution**: The component validates on blur. Make sure the user has focused and then blurred a name field. The `Immediate="true"` parameter ensures validation happens on blur.

### Name value object throws exception

**Problem**: Getting exceptions when creating `Name` value object.

**Solution**: The component handles exceptions internally and displays them as error messages. The `Name` constructor requires non-empty first and last names. Ensure both are provided.

---

## Accessibility

* Proper label association via MudBlazor
* Keyboard navigation support
* Screen reader friendly field labels
* ARIA attributes handled by MudBlazor

---

## Styling & theming

Customize appearance using MudBlazor theme:

```razor
<NameInputPro 
    Variant="Variant.Outlined"
    Class="custom-name-input"
    Style="max-width: 600px;"
    @bind-Value="_name" />
```

---

## Performance

* Validation only occurs on blur (not on every keystroke)
* Efficient re-renders with proper state management
* Minimal re-renders with proper state management

---

## Security

* Name validation follows standard name format rules
* No client-side data storage
* Safe for use in forms with personal data

---

## Related components

* `FullNameInputPro` - Single-field full name input
* `EmailInputPro` - Email input component
* `PhoneNumberInputPro` - Phone number input component

---

## Advanced usage

### Programmatic name creation

```razor
<NameInputPro @bind-Value="_name" />

@code {
    private Name? _name;
    
    private void SetDefaultName()
    {
        _name = new Name("John", "Doe", "Michael", "Johnny");
    }
}
```

### Accessing name properties

```razor
<NameInputPro @bind-Value="_name" />

@if (_name is not null)
{
    <p>Full Name: @_name.FullName</p>
    <p>Short Name: @_name.ShortFullName</p>
    <p>Initials: @_name.Initials</p>
}
```






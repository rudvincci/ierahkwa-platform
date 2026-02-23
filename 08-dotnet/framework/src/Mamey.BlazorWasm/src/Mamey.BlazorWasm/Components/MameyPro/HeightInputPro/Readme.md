# HeightInputPro

A height input component for Blazor built on **MudBlazor** that supports multiple unit systems (feet/inches and centimeters) with automatic conversion. Stores height canonically in inches.

---

## Why this component exists

* **Multiple units**: Supports both imperial (feet/inches) and metric (centimeters) units
* **User-friendly**: Users can input in their preferred unit system
* **Automatic conversion**: Converts between units automatically
* **Canonical storage**: Stores height in inches for consistency
* **Type-safe**: Uses double for height values

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<HeightInputPro @bind-HeightInInches="_height" />

@code {
    private double _height;
}
```

### With custom label and helper text

```razor
<HeightInputPro 
    Label="Height"
    HelperText="Enter your height in your preferred unit"
    @bind-HeightInInches="_height" />
```

---

## Feature tour

### Unit selection

* **Feet/Inches mode**: Two separate inputs for feet and inches
* **Centimeters mode**: Single input for centimeters
* **Unit switching**: Radio buttons to switch between units
* **Automatic conversion**: Values convert automatically when unit changes

### Input modes

* **Feet/Inches**: Two numeric fields (feet and inches)
* **Centimeters**: Single numeric field
* **Real-time updates**: Height updates as user types

### Storage

* **Canonical format**: Height stored in inches (double)
* **Automatic conversion**: Input in any unit, stored in inches
* **Precise calculations**: Accurate conversion between units

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `HeightInInches` | `double` | `0` | Two-way bindable height in inches |
| `HeightInInchesChanged` | `EventCallback<double>` | - | Event fired when height changes |
| `Label` | `string` | `"Height"` | Label text for the input |
| `HelperText` | `string` | `"Stored canonically in inches"` | Helper text below input |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |

---

## Usage examples

### Basic height input

```razor
<HeightInputPro @bind-HeightInInches="_height" />

@code {
    private double _height;
    
    private void OnSubmit()
    {
        // Height is in inches
        Console.WriteLine($"Height: {_height} inches");
    }
}
```

### With custom styling

```razor
<HeightInputPro 
    Label="Your Height"
    HelperText="We'll use this for BMI calculations"
    Class="my-4"
    Style="max-width: 400px;"
    @bind-HeightInInches="_height" />
```

### With EditForm

```razor
<EditForm Model="@_person" OnValidSubmit="SavePerson">
    <DataAnnotationsValidator />
    
    <HeightInputPro 
        Label="Height"
        @bind-HeightInInches="_person.HeightInInches" />
    
    <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
</EditForm>
```

---

## Unit conversion

### Feet/Inches to Inches

Formula: `inches = (feet * 12) + inches`

Example:
* 5 feet 10 inches = 70 inches
* 6 feet 0 inches = 72 inches

### Centimeters to Inches

Formula: `inches = centimeters / 2.54`

Example:
* 175 cm = 68.9 inches
* 180 cm = 70.9 inches

### Inches to Feet/Inches

Formula:
* `feet = floor(inches / 12)`
* `remainingInches = inches - (feet * 12)`

Example:
* 70 inches = 5 feet 10 inches
* 72 inches = 6 feet 0 inches

### Inches to Centimeters

Formula: `centimeters = inches * 2.54`

Example:
* 70 inches = 177.8 cm
* 72 inches = 182.9 cm

---

## Integration with forms

Works seamlessly with Blazor's `EditForm`:

```razor
<EditForm Model="@_user" OnValidSubmit="SaveUser">
    <DataAnnotationsValidator />
    
    <HeightInputPro 
        Label="Height"
        @bind-HeightInInches="_user.HeightInInches" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class User
    {
        public double HeightInInches { get; set; }
    }
    
    private User _user = new();
}
```

---

## Troubleshooting

### Height not updating

**Problem**: Height value doesn't update when switching units.

**Solution**: The component updates on input change. Make sure you're using two-way binding with `@bind-HeightInInches`. The component converts values automatically when units change.

### Conversion accuracy

**Problem**: Converted values don't match expected results.

**Solution**: The component uses standard conversion formulas. Feet/inches conversion is exact. Centimeters conversion uses 2.54 cm per inch. Rounding may occur in display, but stored value is precise.

### Feet/inches input not working

**Problem**: Feet or inches input doesn't update height.

**Solution**: Both feet and inches must be entered for the height to update. The component calculates total inches from both values.

---

## Accessibility

* Proper label association via MudBlazor
* Keyboard navigation support
* Screen reader friendly unit labels
* ARIA attributes handled by MudBlazor

---

## Styling & theming

Customize appearance using MudBlazor theme:

```razor
<HeightInputPro 
    Class="custom-height-input"
    Style="max-width: 400px;"
    @bind-HeightInInches="_height" />
```

---

## Performance

* Real-time conversion on input change
* Efficient re-renders with proper state management
* No external API calls

---

## Security

* No client-side data storage
* Safe for use in forms with personal data
* Input validation handled by MudBlazor

---

## Related components

* `WeightInputPro` - Weight input component
* `EmailInputPro` - Email input component
* `PhoneNumberInputPro` - Phone number input component

---

## Advanced usage

### Programmatic height setting

```razor
<HeightInputPro @bind-HeightInInches="_height" />

@code {
    private double _height;
    
    private void SetDefaultHeight()
    {
        // Set height in inches (e.g., 70 inches = 5'10")
        _height = 70;
    }
}
```

### Height validation

```razor
<HeightInputPro @bind-HeightInInches="_height" />

@code {
    private double _height;
    
    private bool IsValidHeight()
    {
        // Validate height range (e.g., 24-96 inches = 2-8 feet)
        return _height >= 24 && _height <= 96;
    }
}
```

### Display height in different units

```razor
<HeightInputPro @bind-HeightInInches="_height" />

<p>Height: @GetHeightDisplay()</p>

@code {
    private double _height;
    
    private string GetHeightDisplay()
    {
        var feet = (int)Math.Floor(_height / 12);
        var inches = (int)Math.Round(_height - (feet * 12));
        return $"{feet}'{inches}\" ({_height * 2.54:F1} cm)";
    }
}
```






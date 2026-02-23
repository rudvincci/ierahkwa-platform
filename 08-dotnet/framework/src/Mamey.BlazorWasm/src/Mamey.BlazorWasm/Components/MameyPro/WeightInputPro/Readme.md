# WeightInputPro

A weight input component for Blazor built on **MudBlazor** that supports multiple unit systems (pounds and kilograms) with automatic conversion. Stores weight canonically in pounds.

---

## Why this component exists

* **Multiple units**: Supports both imperial (pounds) and metric (kilograms) units
* **User-friendly**: Users can input in their preferred unit system
* **Automatic conversion**: Converts between units automatically
* **Canonical storage**: Stores weight in pounds for consistency
* **Type-safe**: Uses double for weight values

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro

<WeightInputPro @bind-WeightInLbs="_weight" />

@code {
    private double _weight;
}
```

### With custom label and helper text

```razor
<WeightInputPro 
    Label="Weight"
    HelperText="Enter your weight in your preferred unit"
    @bind-WeightInLbs="_weight" />
```

---

## Feature tour

### Unit selection

* **Pounds mode**: Input in pounds (lbs)
* **Kilograms mode**: Input in kilograms (kg)
* **Unit switching**: Radio buttons to switch between units
* **Automatic conversion**: Values convert automatically when unit changes

### Input modes

* **Pounds**: Single numeric field for pounds
* **Kilograms**: Single numeric field for kilograms
* **Real-time updates**: Weight updates as user types

### Storage

* **Canonical format**: Weight stored in pounds (double)
* **Automatic conversion**: Input in any unit, stored in pounds
* **Precise calculations**: Accurate conversion between units

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `WeightInLbs` | `double` | `0` | Two-way bindable weight in pounds |
| `WeightInLbsChanged` | `EventCallback<double>` | - | Event fired when weight changes |
| `Label` | `string` | `"Weight"` | Label text for the input |
| `HelperText` | `string` | `"Stored canonically in pounds"` | Helper text below input |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |

---

## Usage examples

### Basic weight input

```razor
<WeightInputPro @bind-WeightInLbs="_weight" />

@code {
    private double _weight;
    
    private void OnSubmit()
    {
        // Weight is in pounds
        Console.WriteLine($"Weight: {_weight} lbs");
    }
}
```

### With custom styling

```razor
<WeightInputPro 
    Label="Your Weight"
    HelperText="We'll use this for BMI calculations"
    Class="my-4"
    Style="max-width: 400px;"
    @bind-WeightInLbs="_weight" />
```

### With EditForm

```razor
<EditForm Model="@_person" OnValidSubmit="SavePerson">
    <DataAnnotationsValidator />
    
    <WeightInputPro 
        Label="Weight"
        @bind-WeightInLbs="_person.WeightInLbs" />
    
    <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
</EditForm>
```

---

## Unit conversion

### Pounds to Kilograms

Formula: `kilograms = pounds / 2.20462`

Example:
* 150 lbs = 68.0 kg
* 200 lbs = 90.7 kg

### Kilograms to Pounds

Formula: `pounds = kilograms * 2.20462`

Example:
* 70 kg = 154.3 lbs
* 80 kg = 176.4 lbs

---

## Integration with forms

Works seamlessly with Blazor's `EditForm`:

```razor
<EditForm Model="@_user" OnValidSubmit="SaveUser">
    <DataAnnotationsValidator />
    
    <WeightInputPro 
        Label="Weight"
        @bind-WeightInLbs="_user.WeightInLbs" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class User
    {
        public double WeightInLbs { get; set; }
    }
    
    private User _user = new();
}
```

---

## Troubleshooting

### Weight not updating

**Problem**: Weight value doesn't update when switching units.

**Solution**: The component updates on input change. Make sure you're using two-way binding with `@bind-WeightInLbs`. The component converts values automatically when units change.

### Conversion accuracy

**Problem**: Converted values don't match expected results.

**Solution**: The component uses standard conversion formula (1 kg = 2.20462 lbs). Rounding may occur in display, but stored value is precise.

### Input not working

**Problem**: Weight input doesn't update.

**Solution**: Ensure you're using two-way binding. The component updates on every input change.

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
<WeightInputPro 
    Class="custom-weight-input"
    Style="max-width: 400px;"
    @bind-WeightInLbs="_weight" />
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

* `HeightInputPro` - Height input component
* `EmailInputPro` - Email input component
* `PhoneNumberInputPro` - Phone number input component

---

## Advanced usage

### Programmatic weight setting

```razor
<WeightInputPro @bind-WeightInLbs="_weight" />

@code {
    private double _weight;
    
    private void SetDefaultWeight()
    {
        // Set weight in pounds (e.g., 150 lbs)
        _weight = 150;
    }
}
```

### Weight validation

```razor
<WeightInputPro @bind-WeightInLbs="_weight" />

@code {
    private double _weight;
    
    private bool IsValidWeight()
    {
        // Validate weight range (e.g., 50-500 lbs)
        return _weight >= 50 && _weight <= 500;
    }
}
```

### Display weight in different units

```razor
<WeightInputPro @bind-WeightInLbs="_weight" />

<p>Weight: @GetWeightDisplay()</p>

@code {
    private double _weight;
    
    private string GetWeightDisplay()
    {
        var kg = _weight / 2.20462;
        return $"{_weight:F1} lbs ({kg:F1} kg)";
    }
}
```

### BMI calculation

```razor
<HeightInputPro @bind-HeightInInches="_height" />
<WeightInputPro @bind-WeightInLbs="_weight" />

<p>BMI: @CalculateBMI()</p>

@code {
    private double _height;
    private double _weight;
    
    private string CalculateBMI()
    {
        if (_height <= 0 || _weight <= 0) return "N/A";
        
        // BMI = (weight in lbs / (height in inches)^2) * 703
        var bmi = (_weight / (_height * _height)) * 703;
        return $"{bmi:F1}";
    }
}
```






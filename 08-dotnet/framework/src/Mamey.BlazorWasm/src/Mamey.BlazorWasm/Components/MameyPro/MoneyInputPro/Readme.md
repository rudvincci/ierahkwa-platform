# MoneyInputPro

A production-grade money input component for Blazor built on **MudBlazor** that handles amount and currency selection. Uses `Mamey.Types.Money`, `Mamey.Types.Amount`, and `Mamey.Types.Currency` value objects for type-safe monetary values.

---

## Why this component exists

* **Type-safe amounts**: Uses `Mamey.Types.Amount` with configurable decimal precision
* **Currency support**: Full currency selection with ISO currency codes
* **Precision control**: Configurable decimal places (default: 2)
* **Validation**: Enforces amount precision and currency allowlist
* **Consistent API**: Follows MameyPro component patterns

---

## Quick start

### Basic usage

```razor
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.Types

<MoneyInputPro @bind-Value="_money" />

@code {
    private Money? _money;
}
```

### With custom currency list

```razor
<MoneyInputPro 
    AllowedCurrencies="@(new[] { "USD", "EUR", "GBP" })"
    DecimalPlaces="2"
    @bind-Value="_money" />
```

---

## Feature tour

### Amount input

* Numeric field with configurable decimal places
* Minimum value: 0
* Step size based on decimal places (e.g., 0.01 for 2 decimal places)
* Real-time validation

### Currency selection

* Dropdown selector for currency codes
* Configurable allowed currencies
* Default currencies: USD, EUR, JPY, GBP, PLN
* Currency allowlist enforced via `Currency.ConfigureAllowedValues()`

### Validation

* Validates amount precision (throws if exceeds decimal places)
* Validates currency against allowlist
* Shows error messages on validation failure
* Emits `null` when invalid, valid `Money` object when valid

---

## API Reference

### Parameters

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `Value` | `Money?` | `null` | Two-way bindable money value object |
| `ValueChanged` | `EventCallback<Money?>` | - | Event fired when money changes |
| `HelperText` | `string` | `"Validated Amount (precision) + Currency"` | Helper text below input |
| `Class` | `string` | `""` | Additional CSS classes |
| `Style` | `string` | `""` | Additional inline styles |
| `AllowedCurrencies` | `IReadOnlyList<string>` | `["USD", "EUR", "JPY", "GBP", "PLN"]` | Allowed currency codes |
| `DecimalPlaces` | `int` | `2` | Number of decimal places for amount |

---

## Usage examples

### Basic money input

```razor
<MoneyInputPro @bind-Value="_price" />

@code {
    private Money? _price;
    
    private void OnSubmit()
    {
        if (_price is not null)
        {
            // Money is guaranteed to be valid
            Console.WriteLine($"Amount: {_price.Amount.Value} {_price.Currency.Code}");
        }
    }
}
```

### Custom currency list

```razor
<MoneyInputPro 
    AllowedCurrencies="@(new[] { "USD", "CAD", "MXN" })"
    HelperText="North American currencies only"
    @bind-Value="_amount" />
```

### High precision amounts

```razor
<MoneyInputPro 
    DecimalPlaces="4"
    HelperText="Four decimal places for precise calculations"
    @bind-Value="_preciseAmount" />
```

### Integer amounts (no decimals)

```razor
<MoneyInputPro 
    DecimalPlaces="0"
    HelperText="Whole dollar amounts only"
    @bind-Value="_wholeAmount" />
```

### With EditForm

```razor
<EditForm Model="@_product" OnValidSubmit="SaveProduct">
    <DataAnnotationsValidator />
    
    <MoneyInputPro 
        Label="Price"
        AllowedCurrencies="@(new[] { "USD", "EUR" })"
        @bind-Value="_product.Price" />
    
    <MudButton ButtonType="ButtonType.Submit">Save</MudButton>
</EditForm>
```

---

## Currency configuration

### Default currencies

The component defaults to these currencies:
* USD - US Dollar
* EUR - Euro
* JPY - Japanese Yen
* GBP - British Pound
* PLN - Polish Zloty

### Custom currency list

Provide your own list of allowed currencies:

```razor
<MoneyInputPro 
    AllowedCurrencies="@(new[] { "USD", "CAD", "MXN", "AUD", "NZD" })"
    @bind-Value="_money" />
```

**Important**: The currency allowlist must be configured via `Currency.ConfigureAllowedValues()` before creating `Currency` objects. The component does this automatically in `OnInitialized()`.

---

## Decimal precision

### Standard precision (2 decimal places)

Default for most monetary values:

```razor
<MoneyInputPro 
    DecimalPlaces="2"
    @bind-Value="_money" />
```

Accepts: `100.00`, `99.99`, `0.01`
Rejects: `100.001` (exceeds precision)

### High precision (4 decimal places)

For calculations requiring more precision:

```razor
<MoneyInputPro 
    DecimalPlaces="4"
    @bind-Value="_money" />
```

Accepts: `100.0000`, `99.9999`, `0.0001`
Rejects: `100.00001` (exceeds precision)

### Integer amounts (0 decimal places)

For whole dollar amounts:

```razor
<MoneyInputPro 
    DecimalPlaces="0"
    @bind-Value="_money" />
```

Accepts: `100`, `99`, `1`
Rejects: `100.50` (exceeds precision)

---

## Integration with forms

Works seamlessly with Blazor's `EditForm`:

```razor
<EditForm Model="@_invoice" OnValidSubmit="SaveInvoice">
    <DataAnnotationsValidator />
    
    <MoneyInputPro 
        HelperText="Enter invoice amount"
        AllowedCurrencies="@(new[] { "USD", "EUR" })"
        @bind-Value="_invoice.Amount" />
    
    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private class Invoice
    {
        public Money? Amount { get; set; }
    }
    
    private Invoice _invoice = new();
}
```

---

## Troubleshooting

### Money is always null

**Problem**: The `Value` parameter is always `null` even after entering a valid amount and currency.

**Solution**: The component validates on change. Make sure both amount and currency are set. The component emits `null` during editing and only emits a valid `Money` object after validation passes.

### Currency not in allowlist error

**Problem**: Getting error "Currency not in allowlist" even with valid currency code.

**Solution**: Ensure the currency code is in the `AllowedCurrencies` list. The component configures the allowlist in `OnInitialized()`, so make sure `AllowedCurrencies` is set before the component initializes.

### Amount precision error

**Problem**: Getting error "Amount exceeds allowed precision" even with valid decimal places.

**Solution**: The `Amount` value object strictly enforces decimal precision. For example, with `DecimalPlaces="2"`, entering `100.001` will fail. Ensure the amount matches the configured precision exactly.

### Currency allowlist not working

**Problem**: Any currency code is accepted, not just those in `AllowedCurrencies`.

**Solution**: The component calls `Currency.ConfigureAllowedValues()` in `OnInitialized()`. Make sure this runs before any `Currency` objects are created. If you're creating `Currency` objects elsewhere, ensure the allowlist is configured there too.

---

## Accessibility

* Proper label association via MudBlazor
* Keyboard navigation support
* Screen reader friendly error messages
* ARIA attributes handled by MudBlazor

---

## Styling & theming

Customize appearance using MudBlazor theme:

```razor
<MoneyInputPro 
    Class="custom-money-input"
    Style="max-width: 400px;"
    @bind-Value="_money" />
```

---

## Performance

* Validation occurs on change (not on every keystroke for amount)
* Efficient re-renders with proper state management
* Currency allowlist configured once on initialization

---

## Security

* Amount validation enforces precision rules
* Currency allowlist prevents invalid currency codes
* No client-side data storage
* Safe for use in forms with sensitive financial data

---

## Related components

* `EmailInputPro` - Email input component
* `PhoneNumberInputPro` - Phone number input component
* `AddressFormPro` - Full address form

---

## Advanced usage

### Programmatic money creation

```razor
<MoneyInputPro @bind-Value="_money" />

@code {
    private Money? _money;
    
    private void SetDefaultPrice()
    {
        var amount = new Amount(99.99m, decimalPlaces: 2);
        var currency = new Currency("USD");
        _money = new Money(amount, currency);
    }
}
```

### Currency conversion

The component handles currency selection but doesn't perform conversion. For currency conversion, handle it in your parent component:

```razor
<MoneyInputPro @bind-Value="_money" />

@code {
    private Money? _money;
    
    private async Task ConvertCurrency(string targetCurrency)
    {
        if (_money is null) return;
        
        // Perform conversion logic here
        var convertedAmount = await ConvertAsync(_money, targetCurrency);
        _money = new Money(convertedAmount, new Currency(targetCurrency));
    }
}
```






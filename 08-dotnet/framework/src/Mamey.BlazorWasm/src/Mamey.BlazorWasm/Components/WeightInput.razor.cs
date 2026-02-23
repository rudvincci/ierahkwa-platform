using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Mamey.BlazorWasm.Components;
public partial class WeightInput : ComponentBase
{
    private double _weightInLbs;
    private string _selectedUnit = "lbs";
    private double _inputWeight;
    public string _helperText = string.Empty;
    private MudRadioGroup<string> radioGroup = new();

    [Parameter]
    public double WeightInLbs
    {
        get => _weightInLbs;
        set
        {
            if (_weightInLbs != value)
            {
                _weightInLbs = value;
                WeightInLbsChanged.InvokeAsync(value);
            }
        }
    }

    [Parameter]
    public EventCallback<double> WeightInLbsChanged { get; set; }

    protected override void OnInitialized()
    {
        _helperText = $"Enter your weight in {_selectedUnit}";
        _selectedUnit = "lbs";
        base.OnInitialized();
    }
    private void UpdateWeight(double value)
    {
        _inputWeight = value;
        if (_selectedUnit == "lbs")
        {
            WeightInLbs = value;
        }
        else if (_selectedUnit == "kg")
        {
            WeightInLbs = UnitsOfMeasurementExtensions.ConvertKgToLbs(value);
        }
    }

    private void OnUnitChanged(string unit)
    {
        Console.WriteLine($"radio changex to {unit}");
        _selectedUnit = unit;
        _helperText = $"Enter your weight in {_selectedUnit}";
        // Perform conversion based on the newly selected unit
        UpdateWeight(_inputWeight);
        StateHasChanged();
    }
}
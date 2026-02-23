using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Mamey.BlazorWasm.Components;
public partial class HeightInput : ComponentBase
{
    private double _heightInInches;
    private string _selectedUnit = "ft-in";
    private double _heightValueCm; // For cm input, to be converted to inches for internal storage
    private int _feet; // For ft-in input
    private int _inches; // For ft-in input
    private MudRadioGroup<string> radioGroup = new();

    [Parameter]
    public double HeightInInches
    {
        get => _heightInInches;
        set
        {
            if (_heightInInches != value)
            {
                _heightInInches = value;
                HeightInInchesChanged.InvokeAsync(value);
            }
        }
    }

    [Parameter]
    public EventCallback<double> HeightInInchesChanged { get; set; }

    private void UpdateHeight()
    {
        if (_selectedUnit == "cm")
        {
            HeightInInches = _heightValueCm.ConvertCmToInches();
        }
        else if (_selectedUnit == "ft-in")
        {
            HeightInInches = UnitsOfMeasurementExtensions.ConvertFtInToInches(_feet, _inches);
        }
    }

    private void OnUnitChanged(string unit)
    {
        _selectedUnit = unit;
        // Reset inputs when changing units
        UpdateHeight();
        StateHasChanged();
    }

    
}

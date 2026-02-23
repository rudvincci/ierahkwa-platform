namespace Mamey.BlazorWasm.Components.Table;

public class MameyTableViewModel<TModel>
{
    public TModel Data { get; }
    public string DisplayName { get; set; }
    public string AdditionalInfo { get; set; }

    public MameyTableViewModel(TModel data)
    {
        Data = data;
        DisplayName = data.ToString(); // Example transformation
        AdditionalInfo = ""; // Additional UI-specific logic
    }
}
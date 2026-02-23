using Microsoft.AspNetCore.Components;

namespace Mamey.BlazorWasm.Components.Table;

public partial class MameyTableColumn<TModel> : ComponentBase
{
    public MameyTableColumn(PropertyMetadata<TModel> metadata)
    {
        Metadata = metadata;
    }

    public MameyTableColumn(Func<object, object> metadata, string name, bool b, bool b1)
    {
        throw new NotImplementedException();
    }

    public PropertyMetadata<TModel> Metadata { get; }
}
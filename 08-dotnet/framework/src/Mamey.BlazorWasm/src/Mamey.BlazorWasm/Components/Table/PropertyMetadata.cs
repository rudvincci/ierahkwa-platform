using System.Linq.Expressions;
using MudBlazor;

namespace Mamey.BlazorWasm.Components.Table;

public class PropertyMetadata<TModel>
{
    public string Header { get; set; }
    public Expression<Func<TModel, object>> Property { get; set; }
    public Func<TModel, object> ValueAccessor => Property.Compile();
    public bool IsSortable { get; set; }
    public bool IsFilterable { get; set; }
    public SortDirection SortDirection { get; set; }
}
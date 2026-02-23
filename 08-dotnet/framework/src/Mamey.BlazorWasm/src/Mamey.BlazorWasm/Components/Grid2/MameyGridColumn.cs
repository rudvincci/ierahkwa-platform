// using System.Linq.Expressions;
// using Microsoft.AspNetCore.Components;
//
// namespace Mamey.BlazorWasm.Components.Grid2;
//
// public class MameyGridColumn<TEntity>
// {
//     public Expression<Func<TEntity, object>> Property { get; }
//     public string Header { get; }
//     public string? Tooltip { get; }
//     public bool Sortable { get; }
//     public bool Filterable { get; }
//     public bool Visible { get; set; }
//     public RenderFragment<TEntity>? ColumnTemplate { get; }
//
//     public MameyGridColumn(
//         Expression<Func<TEntity, object>> property,
//         string header,
//         bool sortable = false,
//         bool filterable = false,
//         string? tooltip = null,
//         bool visible = true,
//         RenderFragment<TEntity>? columnTemplate = null)
//     {
//         Property = property;
//         Header = header;
//         Sortable = sortable;
//         Filterable = filterable;
//         Tooltip = tooltip;
//         Visible = visible;
//         ColumnTemplate = columnTemplate;
//     }
// }
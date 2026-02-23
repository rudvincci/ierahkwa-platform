using System.ComponentModel;
using System.Linq.Expressions;
using Mamey.Blazor.Abstractions.Types;
using Microsoft.AspNetCore.Components;

namespace Mamey.BlazorWasm.Components.Grid;

public static class Extensions
{
    // public static MameyGridColumn<TModel> ToMameyGridColumn<TModel>(
    //     this TModel model, Expression<Func<TModel, object>> property, string header, bool sortable = false,
    //     bool filterable = false, SortDirection sortDirection = SortDirection.None)
    //     where TModel : IUIModel, new()
    // {
    //     return new MameyGridColumn<TModel>(property, header, sortable, filterable, sortDirection);
    // }
    //
    // public static List<MameyGridColumn<TModel>> ToMameyGridColumnConfigs<TModel>(
    //     this IEnumerable<Expression<Func<TModel, object>>> properties,
    //     string header, bool sortable = false, bool filterable = false,
    //     SortDirection sortDirection = SortDirection.None)
    //     where TModel : IUIModel, new()
    // {
    //     var columns = new List<MameyGridColumn<TModel>>();
    //
    //     foreach (var propExpression in properties)
    //     {
    //         columns.Add(new MameyGridColumn<TModel>(propExpression, header, sortable, filterable, sortDirection));
    //     }
    //
    //     return columns;
    // }
    public static MameyGridBuilder<TModel> ConfigureGrid<TModel>(
        this IEnumerable<Expression<Func<TModel, object>>> properties,
        Action<MameyGridBuilder<TModel>> configure)
        where TModel : IUIModel, INotifyPropertyChanged
    {
        var builder = new MameyGridBuilder<TModel>();
        foreach (var property in properties)
        {
            builder.AddColumn(property, property.ToString(), sortable: true, filterable: true);
        }
        configure?.Invoke(builder);
        return builder;
    }

    public static MameyGridBuilder<TModel> AddColumn<TModel>(
        this MameyGridBuilder<TModel> builder,
        Expression<Func<TModel, object>> property,
        string header,
        bool sortable = false,
        bool filterable = false,
        string? tooltip = null,
        bool visible = true,
        RenderFragment<TModel>? columnTemplate = null)
        where TModel : IUIModel, INotifyPropertyChanged
    {
        return builder.AddColumn(property, header, sortable, filterable, tooltip, visible, columnTemplate);
    }
}

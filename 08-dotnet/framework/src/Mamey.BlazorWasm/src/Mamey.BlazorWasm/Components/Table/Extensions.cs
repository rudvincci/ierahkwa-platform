using System.Linq.Expressions;
using Mamey.Blazor.Abstractions.Types;
using MudBlazor;

namespace Mamey.BlazorWasm.Components.Table;

public static class Extensions
{
    public static MameyTableColumn<TModel> ToMameyTableColumn<TModel>(
        this TModel model, Expression<Func<TModel, object>> property, string header, bool sortable = false,
        bool filterable = false, SortDirection sortDirection = SortDirection.None)
        where TModel : IUIModel, new()
    {
        return new MameyTableColumn<TModel>(new PropertyMetadata<TModel>()
        {
            Property = property,
            Header = header,
            IsSortable = sortable,
            IsFilterable = filterable,
            SortDirection = sortDirection
        });
    }

    public static List<MameyTableColumn<TModel>> ToMameyGridColumnConfigs<TModel>(
        this IEnumerable<Expression<Func<TModel, object>>> properties,
        string header, bool sortable = false, bool filterable = false,
        SortDirection sortDirection = SortDirection.None)
        where TModel : IUIModel, new()
    {
        var columns = new List<MameyTableColumn<TModel>>();

        foreach (var propExpression in properties)
        {
            columns.Add(new MameyTableColumn<TModel>(new PropertyMetadata<TModel>()
            {
                Property = propExpression,
                Header = header,
                IsSortable = sortable,
                IsFilterable = filterable,
                SortDirection = sortDirection
            }));
        }

        return columns;
    }
    public static PropertyMetadata<TModel> ToPropertyMetadata<TModel>(
        this Expression<Func<TModel, object>> property, 
        string header, 
        bool sortable = false, 
        bool filterable = false)
        where TModel : IUIModel
    {
        return new PropertyMetadata<TModel>
        {
            Property = property,
            Header = header,
            IsSortable = sortable,
            IsFilterable = filterable
        };
    }
}
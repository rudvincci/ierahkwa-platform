// using System.ComponentModel;
// using System.Linq.Expressions;
// using Mamey.BlazorWasm.Models;
// using Microsoft.AspNetCore.Components;
//
// namespace Mamey.BlazorWasm.Components.Grid2;
//
// public static class Extensions
// {
//     public static MameyGridBuilder<TModel> ConfigureGrid<TModel>(
//         this IEnumerable<Expression<Func<TModel, object>>> properties,
//         Action<MameyGridBuilder<TModel>> configure)
//         where TModel : IUIModel, INotifyPropertyChanged
//     {
//         var builder = new MameyGridBuilder<TModel>();
//         foreach (var property in properties)
//         {
//             builder.AddColumn(property, property.ToString(), sortable: true, filterable: true);
//         }
//         configure?.Invoke(builder);
//         return builder;
//     }
//
//     public static MameyGridBuilder<TModel> AddColumn<TModel>(
//         this MameyGridBuilder<TModel> builder,
//         Expression<Func<TModel, object>> property,
//         string header,
//         bool sortable = false,
//         bool filterable = false,
//         string? tooltip = null,
//         bool visible = true,
//         RenderFragment<TModel>? columnTemplate = null)
//         where TModel : IUIModel, INotifyPropertyChanged
//     {
//         return builder.AddColumn(property, header, sortable, filterable, tooltip, visible, columnTemplate);
//     }
// }
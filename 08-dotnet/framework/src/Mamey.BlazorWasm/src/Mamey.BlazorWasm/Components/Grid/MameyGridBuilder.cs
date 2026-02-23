using System.ComponentModel;
using System.Linq.Expressions;
using Mamey.Blazor.Abstractions.Types;
using Mamey.Http;

namespace Mamey.BlazorWasm.Components.Grid;

using Microsoft.AspNetCore.Components;

public class MameyGridBuilder<TEntity> 
    where TEntity : IUIModel, INotifyPropertyChanged
{
    private readonly List<MameyGridColumn<TEntity>> _columns = new();

    public MameyGridBuilder<TEntity> AddColumn(
        Expression<Func<TEntity, object>> property,
        string header,
        bool sortable = false,
        bool filterable = false,
        string? tooltip = null,
        bool visible = true,
        RenderFragment<TEntity>? columnTemplate = null)
    {
        _columns.Add(new MameyGridColumn<TEntity>(property, header, sortable, filterable, tooltip, visible, columnTemplate));
        return this;
    }

    public MameyGridBuilder<TEntity> ToggleColumnVisibility(string header, bool visible)
    {
        var column = _columns.FirstOrDefault(c => c.Header == header);
        if (column != null)
        {
            column.Visible = visible;
        }
        return this;
    }

    public MameyGridViewModel<TEntity> Build(IReactiveService<TEntity, MameyHttpClient> service)
    {
        var viewModel = new MameyGridViewModel<TEntity>(service);
        viewModel.ConfigureColumns(cols => cols.AddRange(_columns));
        return viewModel;
    }
}

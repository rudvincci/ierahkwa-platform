using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Threading.Tasks;
using Mamey.Http;
using MudBlazor;
using DynamicData;
using ReactiveUI;
using System.Reactive.Linq;
using Mamey.Blazor.Abstractions.Types;

namespace Mamey.BlazorWasm.Components.Grid;


public class MameyGridViewModel<TEntity> : ReactiveObject
    where TEntity : IUIModel, INotifyPropertyChanged
{
    private readonly IReactiveService<TEntity, MameyHttpClient> _service;

    private int _currentPage = 0;
    private int _pageSize = 20;

    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => this.RaiseAndSetIfChanged(ref _pageSize, value);
    }
    private readonly ReadOnlyObservableCollection<TEntity> _observableList;
    private SourceCache<TEntity, Guid> _items = new(x => x.Id);
    public SourceCache<TEntity, Guid> Items => _items;

    public MameyGridViewModel(IReactiveService<TEntity, MameyHttpClient> service)
    {
        _service = service;

        // Bind the SourceCache to an IObservableList
        Items.Connect()
            .AutoRefresh()
            .Bind(out _observableList)
            .Subscribe();

        // Refresh trigger subscription
        _service.RefreshTrigger
            .Select(_ => LoadPageAsync().ToObservable())
            .Concat()
            .Subscribe();
    }

    public async Task<TableData<TEntity>> FetchGridItemsAsync(TableState tableState, CancellationToken cancellationToken)
    {
        try
        {
            // Fetch paginated, sorted, and filtered data from the service
            var tableData = await _service.FetchItemsAsync();

            // Update the SourceCache with the fetched items
            Items.Edit(updater =>
            {
                updater.Clear();
                updater.AddOrUpdate(tableData);
            });

            return new TableData<TEntity>()
            {
                Items = tableData,
                TotalItems = tableData.Count
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching grid items: {ex.Message}");

            // Return empty data if there's an error
            return new TableData<TEntity>
            {
                Items = new List<TEntity>(),
                TotalItems = 0
            };
        }
    }

    public async Task LoadPageAsync(CancellationToken cancellationToken = default)
    {
        var tableState = new TableState()
        {
            PageSize = PageSize,
            Page = CurrentPage,
        };
        await FetchGridItemsAsync(tableState, cancellationToken);
    }

    public IEnumerable<TEntity>? FilteredAndSortedItems
    {
        get
        {
            return _observableList; // Use the collection directly
        }
    }


    public List<MameyGridColumn<TEntity>> Columns { get; private set; } = new();

    public IEnumerable<MameyGridColumn<TEntity>> VisibleColumns => Columns.Where(c => c.Visible);

    public void ToggleColumnVisibility(string header, bool visible)
    {
        var column = Columns.FirstOrDefault(c => c.Header == header);
        if (column != null)
        {
            column.Visible = visible;
            this.RaisePropertyChanged(nameof(VisibleColumns));
        }
    }

    public void ConfigureColumns(Action<List<MameyGridColumn<TEntity>>> configure)
    {
        configure?.Invoke(Columns);
    }
}

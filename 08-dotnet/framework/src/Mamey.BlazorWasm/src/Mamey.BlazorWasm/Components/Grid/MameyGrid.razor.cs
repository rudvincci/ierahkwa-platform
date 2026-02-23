using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Mamey.Blazor.Abstractions.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Pluralize.NET.Core;

namespace Mamey.BlazorWasm.Components.Grid;
using Mamey.Http;

public partial class MameyGrid<TModel, TApiClient> : ComponentBase
    where TModel : IUIModel, INotifyPropertyChanged
    where TApiClient : MameyHttpClient
{
    [Inject] private ILogger<MameyGrid<TModel, TApiClient>> _Logger { get; set; }
    [Inject] private MameyHttpClient _client { get; set; } 

    [Parameter] public Breakpoint Breakpoint { get; set; } = Breakpoint.Sm;
    [Parameter] public Typo ToolbarTextTypo { get; set; } = Typo.h6;
    [Parameter] public bool DisableToolbar { get; set; } = false;
    [Parameter] public bool DisableSearch { get; set; } = false;
    [Parameter] public TModel? SelectedItem { get; set; }
    [Parameter] public bool Dense { get; set; } = true;
    [Parameter] public bool Hover { get; set; } = false;
    [Parameter] public bool Bordered { get; set; } = false;
    [Parameter] public bool Striped { get; set; } = false;
    [Parameter] public string ToolbarText { get; set; } = $"{new Pluralizer().Pluralize(typeof(TModel).Name)}";
    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public int Elevation { get; set; } = 0;
    [Parameter] public EventCallback<TableRowClickEventArgs<TModel>> OnRowClicked { get; set; }
    [Parameter] public Func<TModel, string>? DetailPageLinkGenerator { get; set; }
    [Parameter] public Func<TModel, bool>? Filter { get; set; }
    [Parameter] public List<TModel>? Items { get; set; }
    [Parameter] public Func<TableState, CancellationToken, Task<TableData<TModel>>>? ServerReload { get; set; }
    [Parameter] public List<MameyGridColumn<TModel>> Columns { get; set; } = new();
    [Parameter] public RenderFragment<TModel> RowTemplate { get; set; } = default!;
    [Parameter] public Color LoadingProgressColor { get; set; } = Color.Info;
    [Parameter] public string SortLabel { get; set; } = "Sort By";
    [Parameter] public bool AllowUnsorted { get; set; } = false;

    private MudTable<TModel> mudTable;
    private int selectedRowNumber = -1;


    private static readonly ConcurrentDictionary<Expression<Func<TModel, object>>, Func<TModel, object>> _columnValueCache = new();
    private HashSet<TModel> selectedItems = new();
    bool _loading = false;

    
    private string currentSortColumn = string.Empty;
    private bool isSortAscending = true;
    MudTextField<string> _searchTextField = new();

    private string _searchString = string.Empty;
    [Parameter] public string Placeholder { get; set; } = "Search";
    private List<TModel>? FullList { get; set; }
    [Parameter]
    public MameyGridViewModel<TModel> ViewModel { get; set; } = default!;
    protected override void OnInitialized()
    {
        if (DetailPageLinkGenerator == null)
        {
            DetailPageLinkGenerator = item => "#";
            _Logger.LogWarning("DetailPageLinkGenerator not provided. Defaulting to '#' for detail page links.");
        }
    }

    protected override void OnParametersSet()
    {
        FullList = Items ?? new List<TModel>();
    }

    private static object GetColumnValue(TModel item, Expression<Func<TModel, object>> column)
    {
        if (!_columnValueCache.TryGetValue(column, out var valueGetter))
        {
            valueGetter = CreateValueGetter(column);
            _columnValueCache[column] = valueGetter;
        }
        return valueGetter(item);
    }

    private static Func<TModel, object> CreateValueGetter(Expression<Func<TModel, object>> columnExpression)
    {
        var member = (columnExpression.Body as MemberExpression ?? ((UnaryExpression)columnExpression.Body).Operand as MemberExpression)?.Member as PropertyInfo;
        if (member == null) throw new InvalidOperationException("The provided expression does not access a property.");
        return (Func<TModel, object>)Delegate.CreateDelegate(typeof(Func<TModel, object>), null, member.GetGetMethod());
    }

    static string GetFullPropertyName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        var memberNames = new List<string>();
        var memberExpression = expression.Body as MemberExpression;

        while (memberExpression != null)
        {
            memberNames.Add(memberExpression.Member.Name);
            memberExpression = memberExpression.Expression as MemberExpression;
        }

        memberNames.Reverse();
        return string.Join(".", memberNames);
    }

    private async Task OnSearchTextChanged(string searchString)
    {
        _searchString = searchString;
        // Debounce to prevent unnecessary filtering
        await Task.Delay(300);
        if (searchString != _searchString) return;

        TextChanged(searchString);
    }

    private void TextChanged(string searchString)
    {
        _searchString = searchString;
        if (string.IsNullOrEmpty(_searchString))
        {
            Items = FullList;
        }
        else if (Columns.Any(column => column.Filterable))
        {
            Items = FullList?.Where(item => Columns.Where(column => column.Filterable)
                .Any(column => GetColumnValue(item, column.Property)
                ?.ToString()?
                .Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true))
                .ToList();
        }

        StateHasChanged();
    }

    private void ApplySort(Expression<Func<TModel, object>> columnExpression)
    {
        var columnName = GetFullPropertyName(columnExpression);
        if (columnName == currentSortColumn)
        {
            isSortAscending = !isSortAscending;
        }
        else
        {
            currentSortColumn = columnName;
            isSortAscending = true;
        }

        SortItems();
    }

    private SortDirection GetSortDirection(Expression<Func<TModel, object>> columnExpression)
    {
        var columnName = GetFullPropertyName(columnExpression);
        if (columnName != currentSortColumn) return SortDirection.Ascending;
        return isSortAscending ? SortDirection.Ascending : SortDirection.Descending;
    }

    private void SortItems()
    {
        if (string.IsNullOrEmpty(currentSortColumn)) return;

        var propInfo = typeof(TModel).GetProperty(currentSortColumn);
        if (propInfo == null) return;

        Items = isSortAscending
            ? Items.OrderBy(x => propInfo.GetValue(x, null)).ToList()
            : Items.OrderByDescending(x => propInfo.GetValue(x, null)).ToList();

        StateHasChanged();
    }

    private void OnKeyDown(KeyboardEventArgs args)
    {
        TextChanged(_searchTextField.Value);
    }

    private string SelectedRowClassFunc(TModel model, int rowNumber)
    {
        if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            Console.WriteLine("Selected Row: None");
            return string.Empty;
        }
        else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(model))
        {
            selectedRowNumber = rowNumber;
            Console.WriteLine($"Selected Row: {rowNumber}");
            return "selected";
        }
        else
        {
            return string.Empty;
        }
    }
}


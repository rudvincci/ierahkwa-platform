# **MameyGrid Component Documentation**

## **Introduction**

The `MameyGrid` component is a flexible and powerful Blazor grid system designed for applications requiring dynamic column visibility, server-side pagination, sorting, filtering, and custom cell templates. It is built with **MudBlazor**, **ReactiveUI**, and **DynamicData**, enabling real-time data updates and rich UI experiences.

---

## **Features**
- Dynamic column visibility toggling.
- Server-side pagination with caching.
- Custom column headers, tooltips, and templates.
- Reactive and real-time state updates.
- Fluent configuration using `MameyGridBuilder`.
- Lightweight, extensible design.

---

## **Installation**
Ensure the following NuGet packages are installed in your project:
1. [MudBlazor](https://github.com/MudBlazor/MudBlazor)
2. [ReactiveUI](https://reactiveui.net/)
3. [DynamicData](https://github.com/reactivemarbles/DynamicData)

Install them via the Package Manager Console:

```bash
dotnet add package MudBlazor
dotnet add package ReactiveUI
dotnet add package DynamicData
```

---

## **Getting Started**

### **Step 1: Define Your Entity**
Define a class that implements the `IUIModel` interface. This entity will represent the rows in your grid.

```csharp
public class MyEntity : IUIModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
}
```

---

### **Step 2: Implement `IReactiveService`**
Create a service that implements `IReactiveService<TEntity, TApiClient>`. This service fetches paginated data from your backend and manages caching.

```csharp
public class MyEntityService : ReactiveService<MyEntity, MameyHttpClient>
{
    public MyEntityService(MameyHttpClient client) : base(client) { }
}
```

---

### **Step 3: Register Services**
In your `Program.cs` file, register your service in the DI container.

```csharp
builder.Services.AddScoped<IReactiveService<MyEntity, MameyHttpClient>, MyEntityService>();
```

---

### **Step 4: Configure the Grid**
In your Razor page/component, configure the grid using the `MameyGridBuilder`.

```csharp
@inject IReactiveService<MyEntity, MameyHttpClient> EntityService

@code {
    private MameyGridViewModel<MyEntity> ViewModel;

    protected override void OnInitialized()
    {
        var builder = new MameyGridBuilder<MyEntity>()
            .AddColumn(e => e.Name, "Name", sortable: true, filterable: true, tooltip: "The entity's name")
            .AddColumn(e => e.Age, "Age", sortable: true, filterable: false, columnTemplate: entity => @<MudTd>@entity.Age years</MudTd>)
            .AddColumn(e => e.Address, "Address", visible: false); // Hidden by default

        ViewModel = builder.Build(EntityService);
    }
}
```

---

### **Step 5: Add the Grid Component**
Render the grid component in your Razor page.

```razor
<MameyGrid ViewModel="ViewModel" />
```

---

## **Dynamic Column Visibility**
To allow users to toggle column visibility dynamically, the `MameyGrid` includes a built-in `ToggleVisibility` component.

```razor
<ToggleVisibility ViewModel="ViewModel" />
```

---

## **Custom Row Templates**
Custom row templates are defined in the `AddColumn` method using a `RenderFragment<TEntity>`.

Example:
```csharp
.AddColumn(e => e.Age, "Age", columnTemplate: entity => @<MudTd>@entity.Age years</MudTd>)
```

---

## **Server-Side Pagination**
The `MameyGridViewModel` handles pagination. Users interact with pagination via a `MudPagination` component built into the grid.

Pagination properties:
- `CurrentPage`: Current page number (0-based).
- `PageSize`: Number of items per page.

Example:
```razor
<MudPagination @bind-Page="ViewModel.CurrentPage" PageSize="ViewModel.PageSize" TotalItems="@ViewModel.Items.Count()" />
```

---

## **Full Example**

### Razor Page: `MyEntityGrid.razor`

```razor
@inject IReactiveService<MyEntity, MameyHttpClient> EntityService

<MameyGrid ViewModel="ViewModel" />

@code {
    private MameyGridViewModel<MyEntity> ViewModel;

    protected override void OnInitialized()
    {
        var builder = new MameyGridBuilder<MyEntity>()
            .AddColumn(e => e.Name, "Name", sortable: true, filterable: true, tooltip: "The entity's name")
            .AddColumn(e => e.Age, "Age", sortable: true, filterable: false, columnTemplate: entity => @<MudTd>@entity.Age years</MudTd>)
            .AddColumn(e => e.Address, "Address", visible: false); // Hidden by default

        ViewModel = builder.Build(EntityService);
    }
}
```

### Backend Service: `MyEntityService.cs`

```csharp
public class MyEntityService : ReactiveService<MyEntity, MameyHttpClient>
{
    public MyEntityService(MameyHttpClient client) : base(client) { }
}
```

---


### **Additional Examples**

#### **1. Advanced Features Examples**
Provide examples for more advanced use cases, such as:

##### **a. Filtering with a Search Bar**
Integrate a search bar that filters the grid dynamically:

```razor
<MudTextField @bind-Value="SearchQuery" Label="Search" Placeholder="Enter a keyword..." Immediate="true" OnInput="OnSearch" />

<MameyGrid ViewModel="ViewModel" />

@code {
    private string SearchQuery = string.Empty;

    private void OnSearch(ChangeEventArgs e)
    {
        ViewModel.Items.Connect()
            .Filter(x => string.IsNullOrEmpty(SearchQuery) || x.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .Subscribe();
    }
}
```

---

##### **b. Multi-Column Sorting**
Illustrate how to sort multiple columns.

```razor
<MudTable Items="@ViewModel.FilteredAndSortedItems().ToEnumerable()">
    <Columns>
        @foreach (var column in ViewModel.VisibleColumns)
        {
            <MudTh OnClick="@(() => ViewModel.SortColumn(column.Property))">
                @column.Header
            </MudTh>
        }
    </Columns>
</MudTable>

@code {
    @inject ILogger<MameyGridViewModel<MyEntity>> Logger;

    private void SortColumn(Expression<Func<MyEntity, object>> columnExpression)
    {
        ViewModel.SortBy(columnExpression);
        Logger.LogInformation($"Sorted by {columnExpression}");
    }
}
```

---

##### **c. Real-Time Updates**
Explain how to handle live data updates with Reactive Streams:

```csharp
public class MyEntityService : ReactiveService<MyEntity, MameyHttpClient>
{
    private readonly Subject<MyEntity> _realTimeUpdates = new();

    public MyEntityService(MameyHttpClient client) : base(client) { }

    public IObservable<MyEntity> RealTimeUpdates => _realTimeUpdates.AsObservable();

    public void PushUpdate(MyEntity entity)
    {
        _realTimeUpdates.OnNext(entity);
    }
}

// In ViewModel
public void SubscribeToRealTimeUpdates()
{
    Service.RealTimeUpdates
        .Subscribe(entity =>
        {
            Items.Edit(updater => updater.AddOrUpdate(entity));
            RaisePropertyChanged(nameof(FilteredAndSortedItems));
        });
}
```

---

#### **2. Common Scenarios**

##### **Exporting Data**
Add functionality to export the grid data to CSV or Excel:

```csharp
public async Task ExportToCsvAsync()
{
    var items = ViewModel.Items.Items.ToList();
    var csv = new StringBuilder();
    csv.AppendLine("Id,Name,Age,Address");
    foreach (var item in items)
    {
        csv.AppendLine($"{item.Id},{item.Name},{item.Age},{item.Address}");
    }
    var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
    await FileUtil.SaveAsAsync("export.csv", byteArray);
}
```

---

##### **Using Templates for Rich Content**
Example: Customizing a column to display badges for a status field:

```csharp
.AddColumn(
    e => e.Status,
    "Status",
    columnTemplate: entity => @<MudTd>
        <MudBadge Color="@(entity.Status == "Active" ? Color.Success : Color.Warning)">
            @entity.Status
        </MudBadge>
    </MudTd>)
```

---

#### **3. Edge Cases and Solutions**

| **Scenario**                          | **Solution**                                                                                      |
|---------------------------------------|--------------------------------------------------------------------------------------------------|
| No data is loaded in the grid         | Check the service’s `FetchItemsAsync` implementation and ensure the backend endpoint is correct. |
| Sorting/Filtering doesn't work        | Verify the `FilteredAndSortedItems` method is called properly, and columns have `Sortable`/`Filterable` enabled. |
| Column toggles not updating instantly | Ensure `RaisePropertyChanged(nameof(VisibleColumns))` is called when toggling visibility.         |
| Grid is slow with large datasets      | Use virtualization (e.g., `MudVirtualize`) for efficient rendering of large collections.          |

---

#### **4. Performance Optimization Tips**
Include suggestions to improve performance:
- Use **MudVirtualize** for large datasets:
```razor
<MudVirtualize Items="@ViewModel.FilteredAndSortedItems().ToEnumerable()" ItemsProvider="LoadItems">
</MudVirtualize>

@code {
    private async Task<IEnumerable<MyEntity>> LoadItems(VirtualizeRequest request)
    {
        return await ViewModel.LoadPageAsync(request.StartIndex, request.Count);
    }
}
```

- Implement caching on the client-side to reduce network requests.
- Fetch only visible columns’ data if data retrieval is column-specific.

---

#### **5. Integration with Other Libraries**
Provide examples of integrating with other popular Blazor libraries:
- **BlazorFluentUI**: Custom styles and icons.
- **Syncfusion**: Exporting directly to Excel or PDF.
- **Blazored.LocalStorage**: Persist column visibility preferences.

Example:
```csharp
await LocalStorage.SetItemAsync("visibleColumns", ViewModel.VisibleColumns.Select(c => c.Header).ToList());
```

---

#### **6. API Compatibility**
If the grid interacts with an API, provide guidelines for implementing a compatible backend. Example:
- Expected request parameters: `page`, `pageSize`, `sortBy`, `filter`.
- Example response:
```json
{
    "items": [
        { "id": "1", "name": "John", "age": 30, "address": "123 Street" },
        { "id": "2", "name": "Jane", "age": 25, "address": "456 Avenue" }
    ],
    "totalItems": 50
}
```

---

#### **7. Debugging and Troubleshooting**
Add a section for debugging common issues:
- **Issue:** Grid data is not refreshing.
    - **Solution:** Call `RaisePropertyChanged(nameof(Items))` in `MameyGridViewModel`.
- **Issue:** Visibility toggles not applying.
    - **Solution:** Ensure `Visible` property is used in the `MudTable` binding.

---

### Suggested Additions

1. **Flow Diagrams**:
    - Include a diagram showing the interaction between `MameyGrid`, `MameyGridViewModel`, and `ReactiveService`.

2. **FAQs**:
    - Address common questions like:
        - How to reset the grid state?
        - How to add additional column types dynamically?

3. **Test Scenarios**:
    - Example unit tests for sorting, filtering, and pagination.

4. **Customization Options**:
    - Document all CSS classes and properties that can be used to style the grid.

---

## **API Reference**

### **MameyGridViewModel<TEntity>**
| Property        | Description                                   |
|-----------------|-----------------------------------------------|
| `Columns`       | List of all columns in the grid.              |
| `VisibleColumns`| List of currently visible columns.            |
| `CurrentPage`   | The current page (0-based).                   |
| `PageSize`      | Number of items per page.                     |
| `FilteredAndSortedItems()` | Filtered and sorted item collection.|

### **MameyGridBuilder<TEntity>**
| Method          | Description                                   |
|-----------------|-----------------------------------------------|
| `AddColumn`     | Adds a column to the grid.                    |
| `ToggleColumnVisibility` | Sets visibility for a specific column.|
| `Build`         | Builds the grid ViewModel.                    |

---

## **Customization Tips**
- Use `columnTemplate` for advanced cell rendering (e.g., icons, badges).
- Add CSS classes to `MudTh` or `MudTd` for custom styling.
- Integrate filtering by binding search inputs to `MameyGridViewModel`.

---

## **Contributing**
Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a feature branch.
3. Submit a pull request with a detailed description.

---

## **License**
This component is licensed under the MIT License. See `LICENSE` for details.

--- 

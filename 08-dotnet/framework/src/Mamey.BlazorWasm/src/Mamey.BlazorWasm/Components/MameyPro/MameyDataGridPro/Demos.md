Nice—this `MameyDataGridPro<TModel>` is solid. To make sure you can test every path (client, server, observable, custom header/rows, virtualization, selection), here’s a **set of drop-in demos** you can paste into your app and run immediately.

> ⚠️ One tiny pager tweak (recommended)
> To see paging in **server mode**, show the pager in both modes. In your component, change:
>
> ```razor
> <PagerContent>
>     @if (!HidePager && !UseServerData)
>     {
>         <MudTablePager />
>     }
> </PagerContent>
> ```
>
> to:
>
> ```razor
> <PagerContent>
>     @if (!HidePager)
>     {
>         <MudTablePager />
>     }
> </PagerContent>
> ```
>
> That’s API-neutral and makes server paging visible.

---

# Shared model + columns (used by all demos)

**File:** `Models/LawDoc.cs`

```csharp
public record LawDoc(Guid Id, string Number, string Title, DateTime Updated);
```

**File:** `Models/LawDocColumns.cs`

```csharp
using Mamey.BlazorWasm.Components.Table;

public static class LawDocColumns
{
    public static List<MameyTableColumn<LawDoc>> Build() => new()
    {
        new(new PropertyMetadata<LawDoc>
        {
            Header        = "Number",
            Property      = x => x.Number,
            ValueAccessor = x => x.Number,
            IsSortable    = true,
            IsFilterable  = true
        }),
        new(new PropertyMetadata<LawDoc>
        {
            Header        = "Title",
            Property      = x => x.Title,
            ValueAccessor = x => x.Title,
            IsSortable    = true,
            IsFilterable  = true
        }),
        new(new PropertyMetadata<LawDoc>
        {
            Header        = "Updated",
            Property      = x => x.Updated,
            ValueAccessor = x => x.Updated,
            IsSortable    = true
        })
    };
}
```

**Optional CSS (row highlight):** add to your site css

```css
.mud-table-row.selected { background-color: rgba(25,118,210,.12); }
```

---

# Demo 1 — Client mode (local filter + sort + pager)

**File:** `Pages/Demos/GridClient.razor`

```razor
@page "/demos/grid-client"
@using MudBlazor

<PageTitle>Grid (Client)</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-6">
    <MameyDataGridPro TModel="LawDoc"
                      ToolbarText="Laws (Client)"
                      Columns="@_columns"
                      Items="@_items"
                      Dense="true" Hover="true" Striped="true" Bordered="false"
                      HidePager="false">

        <RowTemplate>
            <MudTd></MudTd>
            <MudTd>@context.Number</MudTd>
            <MudTd>@context.Title</MudTd>
            <MudTd>@context.Updated.ToShortDateString()</MudTd>
        </RowTemplate>
    </MameyDataGridPro>
</MudContainer>

@code {
    private List<MameyTableColumn<LawDoc>> _columns = LawDocColumns.Build();
    private List<LawDoc> _items = new();

    protected override void OnInitialized()
    {
        var rng = new Random(42);
        var titles = new[]
        {
            "National Governance Structure Act","SICB Charter & Governance Act",
            "Digital Signatures & Trust Act","Open Data Transparency Act",
            "Public Procurement Reform","Cybersecurity Standards Bill",
            "Sustainable Infrastructure Act","Indigenous Land Stewardship Act",
            "Financial Stability & Reserves Act","Future Wampum and Payments Act"
        };

        // ~2k rows is snappy for client mode
        for (int i=1; i<=2000; i++)
            _items.Add(new LawDoc(Guid.NewGuid(), $"LAW-{i:00000}", titles[rng.Next(titles.Length)],
                                  DateTime.UtcNow.AddDays(-rng.Next(0,365*10))));
    }
}
```

What to verify:

* Type in search → instant client filtering.
* Click column headers → client sorting toggles.
* Pager shows and works (because `HidePager="false"`).

---

# Demo 2 — Server mode (25k rows, true paging + SortLabel + search)

**File:** `Pages/Demos/GridServer.razor`

```razor
@page "/demos/grid-server"
@using MudBlazor

<PageTitle>Grid (Server)</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-6">
    <MameyDataGridPro TModel="LawDoc"
                      ToolbarText="Laws (Server)"
                      Columns="@_columns"
                      ServerReloadWithQuery="LoadServerAsync"
                      Dense="true" Hover="true" Striped="true"
                      HidePager="false">
        <RowTemplate>
            <MudTd></MudTd>
            <MudTd>@context.Number</MudTd>
            <MudTd>@context.Title</MudTd>
            <MudTd>@context.Updated.ToShortDateString()</MudTd>
        </RowTemplate>
    </MameyDataGridPro>
</MudContainer>

@code {
    private readonly List<LawDoc> _all = new();
    private List<MameyTableColumn<LawDoc>> _columns = LawDocColumns.Build();

    protected override void OnInitialized()
    {
        var rng = new Random(42);
        var titles = new[]
        {
            "National Governance Structure Act","SICB Charter & Governance Act",
            "Digital Signatures & Trust Act","Open Data Transparency Act",
            "Public Procurement Reform","Cybersecurity Standards Bill",
            "Sustainable Infrastructure Act","Indigenous Land Stewardship Act",
            "Financial Stability & Reserves Act","Future Wampum and Payments Act"
        };

        // 25,000 rows: use server paging/sort/filter
        for (int i=1; i<=25_000; i++)
            _all.Add(new LawDoc(Guid.NewGuid(), $"LAW-{i:00000}", titles[rng.Next(titles.Length)],
                                DateTime.UtcNow.AddDays(-rng.Next(0,365*15))));
    }

    private Task<TableData<LawDoc>> LoadServerAsync(TableState state, string? query, CancellationToken ct)
    {
        IEnumerable<LawDoc> q = _all;

        // filter
        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim();
            q = q.Where(d => (d.Number?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false)
                             || (d.Title?.Contains(s,  StringComparison.OrdinalIgnoreCase) ?? false));
        }

        // sort (SortLabel + Direction)
        var sortMap = new Dictionary<string, Func<LawDoc, object>> {
            [nameof(LawDoc.Number)]  = d => d.Number,
            [nameof(LawDoc.Title)]   = d => d.Title,
            [nameof(LawDoc.Updated)] = d => d.Updated
        };

        if (!string.IsNullOrWhiteSpace(state.SortLabel) && sortMap.TryGetValue(state.SortLabel, out var key))
        {
            q = state.SortDirection == SortDirection.Descending ? q.OrderByDescending(key) : q.OrderBy(key);
        }
        else
        {
            q = q.OrderBy(d => d.Number); // default
        }

        var total = q.Count();

        // page
        var items = q.Skip(state.Page * state.PageSize)
                     .Take(state.PageSize)
                     .ToList();

        return Task.FromResult(new TableData<LawDoc> { Items = items, TotalItems = total });
    }
}
```

What to verify:

* Search triggers server reload (because your component calls `_table.ReloadServerData()`).
* Click header to set `SortLabel` and direction → server sorts accordingly.
* Pager changes pages across 25k rows.

---

# Demo 3 — ObservableCollection (reactive add/remove in place)

**File:** `Pages/Demos/GridObservable.razor`

```razor
@page "/demos/grid-observable"
@using System.Collections.ObjectModel
@using MudBlazor

<PageTitle>Grid (Observable)</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-6">
    <MameyDataGridPro TModel="LawDoc"
                      ToolbarText="Laws (Observable)"
                      Columns="@_columns"
                      ObservableItems="@_items"
                      Dense="true" Hover="true" Striped="true" Bordered="true"
                      HidePager="false"
                      >
        <RowTemplate>
            <MudTd></MudTd>
            <MudTd>@context.Number</MudTd>
            <MudTd>@context.Title</MudTd>
            <MudTd>@context.Updated.ToShortDateString()</MudTd>
        </RowTemplate>

        <ToolbarContent>
            <MudButton Variant="Variant.Filled" StartIcon="@Icons.Material.Filled.Add" OnClick="AddOne">Add</MudButton>
            <MudButton Variant="Variant.Outlined" StartIcon="@Icons.Material.Filled.Remove" OnClick="RemoveOne">Remove</MudButton>
        </ToolbarContent>
    </MameyDataGridPro>
</MudContainer>

@code {
    private List<MameyTableColumn<LawDoc>> _columns = LawDocColumns.Build();
    private ObservableCollection<LawDoc> _items = new();

    protected override void OnInitialized()
    {
        for (int i=1; i<=25; i++)
            _items.Add(new LawDoc(Guid.NewGuid(), $"LAW-{i:00000}", "Reactive Row", DateTime.UtcNow.AddDays(-i)));
    }

    private void AddOne()
        => _items.Insert(0, new LawDoc(Guid.NewGuid(), $"LAW-{_items.Count+1:00000}", "Added", DateTime.UtcNow));

    private void RemoveOne()
    {
        if (_items.Count > 0) _items.RemoveAt(0);
    }
}
```

What to verify:

* Clicking **Add** injects a row immediately.
* Clicking **Remove** updates the grid without a full reload.

---

# Demo 4 — Custom header + action column + multi-select

**File:** `Pages/Demos/GridCustom.razor`

```razor
@page "/demos/grid-custom"
@using MudBlazor

<PageTitle>Grid (Custom Header/Rows)</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="mt-6">
    <MameyDataGridPro TModel="LawDoc"
                      Columns="@_columns"
                      Items="@_items"
                      MultiSelection="true"
                      ToolbarText="Laws (Custom)"
                      HidePager="false">

        <Header>
            <MudTh></MudTh>
            <MudTh>Number</MudTh>
            <MudTh>Title</MudTh>
            <MudTh>Updated</MudTh>
            <MudTh>Actions</MudTh>
        </Header>

        <RowTemplate>
            <MudTd></MudTd>
            <MudTd>@context.Number</MudTd>
            <MudTd>
                @context.Title
                @if (context.Title.Contains("Security", StringComparison.OrdinalIgnoreCase))
                {
                    <MudChip Color="Color.Warning" Variant="Variant.Outlined" Size="Size.Small" Class="ml-2">Security</MudChip>
                }
            </MudTd>
            <MudTd>@context.Updated.ToShortDateString()</MudTd>
            <MudTd>
                <MudIconButton Icon="@Icons.Material.Filled.Visibility" Title="View" />
                <MudIconButton Icon="@Icons.Material.Filled.Edit" Title="Edit" />
            </MudTd>
        </RowTemplate>

        <ToolbarContent>
            <MudText Class="mr-4">@($"{SelectedCount} selected")</MudText>
        </ToolbarContent>
    </MameyDataGridPro>
</MudContainer>

@code {
    private List<MameyTableColumn<LawDoc>> _columns = LawDocColumns.Build();
    private List<LawDoc> _items = new();
    [CascadingParameter] public MudTable<LawDoc>? MudTable { get; set; }
    private int SelectedCount => MudTable?.SelectedItems?.Count ?? 0;

    protected override void OnInitialized()
    {
        for (int i=1; i<=150; i++)
            _items.Add(new LawDoc(Guid.NewGuid(), $"LAW-{i:00000}",
                i % 5 == 0 ? "Cybersecurity Standards Bill" : "Public Procurement Reform",
                DateTime.UtcNow.AddDays(-i)));
    }
}
```

What to verify:

* Custom header renders.
* “Security” rows show a chip.
* Multi-selection count updates in the toolbar.

---

# Demo 5 — Server + Virtualize (infinite-scroll style)

**File:** `Pages/Demos/GridVirtualized.razor`

```razor
@page "/demos/grid-virtual"
@using MudBlazor

<PageTitle>Grid (Server + Virtualize)</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-6">
    <MameyDataGridPro TModel="LawDoc"
                      ToolbarText="Laws (Server + Virtualize)"
                      Columns="@_columns"
                      ServerReloadWithQuery="LoadServerAsync"
                      Virtualize="true"
                      Dense="true" Hover="true"
                      HidePager="true">
        <RowTemplate>
            <MudTd></MudTd>
            <MudTd>@context.Number</MudTd>
            <MudTd>@context.Title</MudTd>
            <MudTd>@context.Updated</MudTd>
        </RowTemplate>
    </MameyDataGridPro>
</MudContainer>

@code {
    private readonly List<LawDoc> _all = new();
    private List<MameyTableColumn<LawDoc>> _columns = LawDocColumns.Build();

    protected override void OnInitialized()
    {
        var rng = new Random(7);
        var titles = new[]{ "Open Data Transparency Act","Future Wampum and Payments Act","Sustainable Infrastructure Act" };
        for (int i=1; i<=50_000; i++)
            _all.Add(new LawDoc(Guid.NewGuid(), $"LAW-{i:000000}", titles[rng.Next(titles.Length)],
                                DateTime.UtcNow.AddDays(-rng.Next(0,365*10))));
    }

    // For virtualized tables, Mud still calls ServerData with Page/PageSize based on viewport.
    private Task<TableData<LawDoc>> LoadServerAsync(TableState state, string? query, CancellationToken ct)
    {
        IEnumerable<LawDoc> q = _all;

        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim();
            q = q.Where(d => d.Title.Contains(s, StringComparison.OrdinalIgnoreCase) || d.Number.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        // default sort to Updated desc for infinite-feel
        q = q.OrderByDescending(d => d.Updated);

        var total = q.Count();
        var items = q.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList();

        return Task.FromResult(new TableData<LawDoc> { Items = items, TotalItems = total });
    }
}
```

What to verify:

* Scrolling triggers server fetches (Mud sets `Page`/`PageSize` based on viewport).
* Search still reloads server with query.
* Pager is hidden; scroll is the UX.

---

## Where this takes us next

With these demos validated, we can build the **other “Pro” components** (FileUploadEx, PhoneNumberFormPro, Height/Weight Pro, CardsPro, etc.) to match this pattern:

* Take **developer-friendly parameters** (slots for header/toolbar/actions).
* Provide **client and server hooks** where it makes sense.
* Keep **non-breaking** names (suffix `Pro`/`Ex`).
* Document the **SortLabel** mapping (you already did with `GetSortKey`).

If you want, I’ll now adapt your existing **MameyGrid / MameyTable** usages to `MameyDataGridPro` with minimal diffs (basically columns + row templates + optional server loaders).

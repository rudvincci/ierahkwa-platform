# MameyDataGridPro\<TModel>

A flexible, production-grade data grid for Blazor built on **MudBlazor** that unifies the strengths of your previous `MameyTable` and `MameyGrid`. It supports **client-side** and **server-side** data, **search with debounce**, **sorting**, **paging**, **virtualization**, **selection**, and fully **templated headers/rows**—all with ergonomics that make developers happy.

---

## Why this component exists

* **One API, two modes:** bind `Items` for client-side behavior, or provide a server delegate via `ServerReload`/`ServerReloadWithQuery` for large datasets and backend paging/sorting.
* **First-class search:** debounced search that either filters locally or triggers a server reload with the current query.
* **Straightforward sorting:** client mode uses compiled value accessors; server mode integrates with `MudTableSortLabel` so you receive `SortLabel` + `SortDirection`.
* **Templating without tears:** slot your own header/row UI or let it auto-render from your column metadata.
* **No breaking mental model:** columns use your existing `MameyTableColumn<TModel>` + `PropertyMetadata<TModel>`.

---

## Quick start

### 1) Add the component to your page

```razor
@using Mamey.BlazorWasm.Components.Table

<MameyDataGridPro TModel="LawDoc"
                  ToolbarText="Laws"
                  Columns="@_columns"
                  Items="@_clientItems"   @* Client mode *@
                  HidePager="false" />

@code {
    public record LawDoc(Guid Id, string Number, string Title, DateTime Updated);

    private List<LawDoc> _clientItems = new() {
        new(Guid.NewGuid(),"LAW-00001","Open Data Transparency Act", DateTime.UtcNow),
        new(Guid.NewGuid(),"LAW-00002","Cybersecurity Standards Bill", DateTime.UtcNow.AddDays(-7)),
    };

    private List<MameyTableColumn<LawDoc>> _columns = new()
    {
        new(new PropertyMetadata<LawDoc>{ Header="Number",  Property=x=>x.Number,  ValueAccessor=x=>x.Number,  IsSortable=true, IsFilterable=true }),
        new(new PropertyMetadata<LawDoc>{ Header="Title",   Property=x=>x.Title,   ValueAccessor=x=>x.Title,   IsSortable=true, IsFilterable=true }),
        new(new PropertyMetadata<LawDoc>{ Header="Updated", Property=x=>x.Updated, ValueAccessor=x=>x.Updated, IsSortable=true })
    };
}
```

### 2) Server mode: pass a loader (with query + cancellation)

```razor
<MameyDataGridPro TModel="LawDoc"
                  ToolbarText="Laws (Server)"
                  Columns="@_columns"
                  ServerReloadWithQuery="LoadServerAsync"
                  HidePager="false" />

@code {
    private readonly List<LawDoc> _all = /* …seeded big dataset… */;

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

        // sort via SortLabel + SortDirection
        var sortMap = new Dictionary<string, Func<LawDoc, object>> {
            [nameof(LawDoc.Number)]  = d => d.Number,
            [nameof(LawDoc.Title)]   = d => d.Title,
            [nameof(LawDoc.Updated)] = d => d.Updated
        };
        if (!string.IsNullOrWhiteSpace(state.SortLabel) && sortMap.TryGetValue(state.SortLabel, out var key))
            q = state.SortDirection == SortDirection.Descending ? q.OrderByDescending(key) : q.OrderBy(key);
        else
            q = q.OrderBy(d => d.Number);

        // page
        var total = q.Count();
        var items = q.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList();

        // optional: honor cancellation
        ct.ThrowIfCancellationRequested();

        return Task.FromResult(new TableData<LawDoc>{ Items = items, TotalItems = total });
    }
}
```

---

## Feature tour

### Modes

* **Client mode:** set `Items` (or `ObservableItems`) and the grid performs local filtering/sorting/paging.
* **Server mode:** provide `ServerReload` or `ServerReloadWithQuery`. The grid forwards a `TableState` (page, size, sort label, direction) and optional `query` string, and **expects** a `TableData<TModel>` result.

### Search (debounced)

* The built-in search box debounces input (300 ms), then:

  * **Client mode:** filters over `IsFilterable` columns using their compiled `ValueAccessor`.
  * **Server mode:** calls `_table.ReloadServerData()` and passes the latest query into your server delegate.

### Sorting

* **Client mode:** clicking a header toggles ascending/descending for that column; sorting uses your column’s `ValueAccessor`.
* **Server mode:** headers render `<MudTableSortLabel SortLabel="PropertyName">` and `TableState.SortLabel` + `SortDirection` are passed to your server delegate.

### Paging

* Uses Mud’s pager. By default (your final code), pager is **rendered for client mode only** (`!HidePager && !UseServerData`).
  **Recommendation:** render pager for both modes so server paging is visible:

  ```razor
  <PagerContent>
      @if (!HidePager) { <MudTablePager /> }
  </PagerContent>
  ```

### Virtualization

* Set `Virtualize="true"` to enable MudBlazor’s virtualization (great for large sets).
* In server mode, `TableState.Page/PageSize` reflect the visible window.

### Selection

* Single or multi-select (`MultiSelection=true`). Selected row gets `selected` CSS class (you can style `.mud-table-row.selected`).

### Templating

* **Header:** provide a full custom `<Header>…</Header>` or let the grid auto-render.
* **RowTemplate:** supply `<RowTemplate>` for complete control, or let the grid print cells from your `Columns`.

---

## API Reference

### Parameters

| Name                    | Type                                                                     |                                    Default | Applies To | Description                                                        |
| ----------------------- | ------------------------------------------------------------------------ | -----------------------------------------: | ---------- | ------------------------------------------------------------------ |
| `Columns`               | `List<MameyTableColumn<TModel>>`                                         |                                        new | All        | Column metadata (header text, `Property`, `ValueAccessor`, flags). |
| `Items`                 | `List<TModel>?`                                                          |                                       null | Client     | Static list for client mode.                                       |
| `ObservableItems`       | `ObservableCollection<TModel>?`                                          |                                       null | Client     | Reactive collection; grid recomputes on change.                    |
| `ServerReload`          | `Func<TableState, CancellationToken, Task<TableData<TModel>>>?`          |                                       null | Server     | Backend loader (no query).                                         |
| `ServerReloadWithQuery` | `Func<TableState, string?, CancellationToken, Task<TableData<TModel>>>?` |                                       null | Server     | Backend loader (receives search text).                             |
| `ToolbarText`           | `string`                                                                 |                                       `""` | All        | Title text in toolbar.                                             |
| `DisableToolbar`        | `bool`                                                                   |                                    `false` | All        | Hide the toolbar area.                                             |
| `DisableSearch`         | `bool`                                                                   |                                    `false` | All        | Hide the search box.                                               |
| `Placeholder`           | `string`                                                                 |                                 `"Search"` | All        | Search box placeholder.                                            |
| `ToolbarContent`        | `RenderFragment?`                                                        |                                       null | All        | Inject custom actions into toolbar (e.g., buttons).                |
| `Header`                | `RenderFragment?`                                                        |                                       null | All        | Custom header content; overrides auto header.                      |
| `RowTemplate`           | `RenderFragment<TModel>?`                                                |                                       null | All        | Custom row layout; overrides auto row.                             |
| `Dense`                 | `bool`                                                                   |                                     `true` | All        | Compact row height.                                                |
| `Hover`                 | `bool`                                                                   |                                     `true` | All        | Row hover effect.                                                  |
| `Bordered`              | `bool`                                                                   |                                    `false` | All        | Table borders.                                                     |
| `Striped`               | `bool`                                                                   |                                    `false` | All        | Zebra striping.                                                    |
| `Virtualize`            | `bool`                                                                   |                                    `false` | All        | Enable virtualization.                                             |
| `Breakpoint`            | `Breakpoint`                                                             |                                       `Sm` | All        | Responsive breakpoint for stacked cells.                           |
| `HidePager`             | `bool`                                                                   |                                    `false` | All        | Hide the pager area.                                               |
| `MultiSelection`        | `bool`                                                                   |                                    `false` | All        | Allow multiple rows to be selected.                                |
| `SelectedItem`          | `TModel?` (two-way)                                                      |                                       null | All        | Bind the active row.                                               |
| `SelectedItems`         | `HashSet<TModel>` (two-way)                                              |                                        new | All        | Bind selected rows when multi-select.                              |
| `Class`                 | `string`                                                                 |                                       `""` | All        | CSS class on outer paper.                                          |
| `Elevation`             | `int`                                                                    |                                        `0` | All        | Paper elevation.                                                   |
| `EmptyTitle`            | `string`                                                                 |                             `"No results"` | Client     | Empty state title (client mode).                                   |
| `EmptySubtitle`         | `string`                                                                 | `"Try adjusting filters or search terms."` | Client     | Empty state subtitle (client mode).                                |
| `AllowUnsorted`         | `bool`                                                                   |                                    `false` | Client     | If you want a third “unsorted” state for client sort.              |

### Slots (top-level MudTable regions you can override)

* `HeaderContent` (managed internally; provide `Header` to replace)
* `RowTemplate`
* `PagerContent`
* `ToolbarContent` (in the component’s outer toolbar, not a MudTable slot)

---

## Columns & metadata

`MameyTableColumn<TModel>` wraps your `PropertyMetadata<TModel>`:

```csharp
new(new PropertyMetadata<MyModel>
{
    Header        = "Amount",
    Property      = x => x.Amount,          // used for sort label (name)
    ValueAccessor = x => x.Amount,          // compiled getter for sorting/filtering locally
    IsSortable    = true,
    IsFilterable  = true
});
```

**Guidelines**

* Always set **both** `Property` and `ValueAccessor`.

  * `Property` → the grid extracts a *sort label* (property name) for server mode.
  * `ValueAccessor` → used for client filtering/sorting.
* Mark `IsFilterable=true` only on columns that should participate in text search.

---

## End-to-end scenarios

### A) Client mode (2,000 rows)

* Bind `Items` (or `ObservableItems`).
* Enable/disable `Dense`, `Hover`, `Striped`, etc.
* The pager shows when `HidePager=false`.

### B) Server mode (25,000+ rows)

* Bind `ServerReloadWithQuery` (preferred) or `ServerReload`.
* In header cells, the component renders `MudTableSortLabel` with `SortLabel` taken from the column’s `Property` member name.
* In your loader, read `state.SortLabel` and `state.SortDirection` to sort; `state.Page` and `state.PageSize` to page.
* The component triggers reloads when:

  * the pager changes page,
  * a sort label/direction changes,
  * the debounced search text changes.

### C) Virtualized server grid (50,000+ rows)

* Set `Virtualize=true` and usually `HidePager=true`.
* Your loader still receives `Page`/`PageSize` based on viewport.

### D) Custom header and row actions

* Provide `<Header>` and `<RowTemplate>` to insert chips, icons, menus, etc.
* You can still get sorting in server mode by placing a `MudTableSortLabel` in your custom header yourself:

  ```razor
  <MudTh><MudTableSortLabel SortLabel="Number">Number</MudTableSortLabel></MudTh>
  ```

---

## Troubleshooting & gotchas

* **RZ9996: Unrecognized child content inside MudTable**
  Make sure top-level MudTable slots (`HeaderContent`, `RowTemplate`, `PagerContent`, …) are **direct children**. Put conditionals **inside** the slot:

  ```razor
  <PagerContent>
      @if (!HidePager) { <MudTablePager /> }
  </PagerContent>
  ```

* **CS1503: cannot convert method group to EventCallback**
  For `MudTextField`, specify `T="string"` and use a lambda:

  ```razor
  <MudTextField T="string" Value="_searchString" ValueChanged="@(v => OnSearchTextChanged(v))" />
  ```

* **Sorting on server: TableState has no SortBy**
  Correct. Use `state.SortLabel` (string) + `state.SortDirection`. Make sure the header uses `<MudTableSortLabel SortLabel="…">`.

* **No paging visible in server mode**
  Your final code only shows pager in client mode. Render the pager regardless of mode:

  ```razor
  <PagerContent>@if (!HidePager) { <MudTablePager /> }</PagerContent>
  ```

* **Null `_table` when reloading**
  The component already checks `if (_table is not null) await _table.ReloadServerData();`. Ensure the table is rendered before your first reload.

* **Slow client filtering on huge lists**
  Switch to server mode, or set `Virtualize=true`, or reduce `Items` size. Client search walks only columns flagged `IsFilterable`.

---

## Performance tips

* **Prefer server mode** beyond \~5–10k rows; use proper DB-side paging and sorting.
* **Honor cancellation** in loaders (`ct.ThrowIfCancellationRequested()`), especially with EF Core/long queries.
* **Precompute accessors** (you already do via `ValueAccessor`) to avoid reflection in hot paths.
* **Debounce search** (already 300 ms). You can expose this as a parameter if you want a different cadence per page.

---

## Accessibility & i18n

* **Labels & placeholders:** set `ToolbarText` and `Placeholder` with localized strings.
* **Keyboard & focus:** MudBlazor controls are keyboard-friendly; don’t remove default focus outlines.
* **Row highlight:** ensure `.selected` color passes contrast (AA). You can override with a theme variable or CSS.

---

## Styling & theming

* Use your MudBlazor theme to tweak typography, density, and table colors globally.
* Local tweaks:

  ```css
  .mud-table-row.selected { background-color: rgba(25,118,210,.12); }
  .cursor-pointer { cursor: pointer; }
  .opacity-60 { opacity: .6; }
  ```

---

## Extensibility patterns

### 1) Auto-build a server sort map from `Columns`

Avoid hand-maintaining the `sortMap`:

```csharp
private Dictionary<string, Func<LawDoc, object>> BuildSortMap(List<MameyTableColumn<LawDoc>> cols)
    => cols.Where(c => c.Metadata.IsSortable)
           .ToDictionary(c => GetSortKey(c.Metadata.Property), c => c.Metadata.ValueAccessor);
```

Then in your loader:

```csharp
var map = BuildSortMap(_columns);
if (!string.IsNullOrWhiteSpace(state.SortLabel) && map.TryGetValue(state.SortLabel, out var key))
    q = state.SortDirection == SortDirection.Descending ? q.OrderByDescending(key) : q.OrderBy(key);
```

### 2) Pluggable filter descriptors (future)

Evolve the loader to accept a richer `GridQuery`:

```csharp
public sealed record GridQuery(int Page, int PageSize, string? Search, string? SortLabel, SortDirection SortDirection);
```

You can build this inside `MameyDataGridPro` and pass it to your API.

---

## Migration guide

### From `MameyTable`

* Keep your **columns** as-is.
* Replace `<MameyTable …>` with `<MameyDataGridPro …>` and move `ServerData` → `ServerReload` or `ServerReloadWithQuery`.

### From `MameyGrid`

* Move custom header/rows into `<Header>` / `<RowTemplate>`.
* Keep your toolbar buttons via `ToolbarContent`.

### From raw `MudTable`

* Supply `Columns` metadata so client filtering/sorting work automatically.
* In server mode, replace your `ServerData` with the component’s `ServerReload*` parameters.

---

## Full scenario samples (copy/paste)

* **Client mode (2k rows):** `/demos/grid-client`
* **Server mode (25k rows):** `/demos/grid-server`
* **Observable (reactive add/remove):** `/demos/grid-observable`
* **Custom header + actions + multi-select:** `/demos/grid-custom`
* **Server + Virtualize (50k rows):** `/demos/grid-virtual`

(You already have the full code for these in the previous message; drop them into your `Pages/Demos` folder.)

---

## FAQ

**How do I change debounce time?**
Wrap `Debouncer` in a parameter (e.g., `SearchDebounceMs`) and pass the value into its constructor.

**Can I add per-column filter components?**
Yes—add a `FilterContent` slot in your toolbar and surface filter state to the loader as part of a `GridQuery`.

**How do I do multi-column sorting on the server?**
Extend the header to allow modifier-click collecting labels and send a list of `(Label, Direction)` pairs to your API. Apply with chained `ThenBy`/`ThenByDescending`.

---

## Security & correctness

* Always **sanitize** search strings if you pass them to raw SQL. Prefer EF Core parameterization.
* Always honor the **`CancellationToken`** to avoid runaway queries.
* Validate `state.Page` and `state.PageSize` to keep reasonable limits.

---

## Minimal checklist (before using in production)

* [ ] Pager visible in server mode (`HidePager=false` and render pager regardless of mode).
* [ ] `Columns` have **both** `Property` and `ValueAccessor`.
* [ ] Server loader **sorts by `SortLabel`** and **pages** using `Page`/`PageSize`.
* [ ] Search path tested (client filter vs server reload).
* [ ] Row selection styling verified against your theme.
* [ ] Large dataset perf validated (try `Virtualize=true` if needed).

---

When you’re ready, we can use this pattern to build **Pro** versions of your other components (FileUpload, PhoneNumberForm, Height/Weight inputs, Cards) with the same developer ergonomics: clear parameters, server/client hooks, and thoughtful defaults.

// using System.Reactive;
// using System.Reactive.Linq;
// using System.Reactive.Subjects;
// using Mamey.BlazorWasm.Api;
// using Mamey.BlazorWasm.Models;
// using Mamey.Http;
// using MudBlazor;
//
// namespace Mamey.BlazorWasm.Components.Grid2;
//
// public class ReactiveService<TEntity, TApiClient> : IReactiveService<TEntity, TApiClient>
//     where TEntity : IUIModel
//     where TApiClient : MameyHttpClient
// {
//     private readonly string _endpoint;
//     protected readonly TApiClient _client;
//     protected readonly IObservableApiResponseHandler _apiResponseHandler;
//     
//     private readonly Dictionary<(int Page, int PageSize), List<TEntity>> _cache = new();
//     private readonly SemaphoreSlim _semaphore = new(1, 1);
//     
//     protected ReactiveService(TApiClient client, IObservableApiResponseHandler apiResponseHandler, string endpoint)
//     {
//         _client = client;
//         _apiResponseHandler = apiResponseHandler;
//         _endpoint = endpoint;
//     }
//     public async Task<List<TEntity>> FetchItemsAsync(int page, int pageSize)
//     {
//         if (_cache.TryGetValue((page, pageSize), out var cachedItems))
//         {
//             return cachedItems;
//         }
//
//         await _semaphore.WaitAsync();
//         try
//         {
//             if (_cache.TryGetValue((page, pageSize), out cachedItems))
//             {
//                 return cachedItems;
//             }
//
//             var response = await _client.GetApiResponseAsync<List<TEntity>>($"{_endpoint}?page={page}&pageSize={pageSize}");
//             _cache[(page, pageSize)] = response.Value;
//             return response.Value;
//         }
//         finally
//         {
//             _semaphore.Release();
//         }
//     }
//     public async Task<TableData<TEntity>> FetchItemsAsync(
//         TableState tableState,
//         CancellationToken cancellationToken)
//     {
//         try
//         {
//             // Extract paging information
//             var page = tableState.Page;
//             var pageSize = tableState.PageSize;
//
//             // Extract sorting information
//             var sortBy = tableState.SortLabel;
//             var isDescending = tableState.SortDirection == SortDirection.Descending;
//
//             // Fetch data from the backend
//             var response = await _client.GetApiResponseAsync<List<TEntity>>(
//                 $"{_endpoint}?page={page}&pageSize={pageSize}"
//             );
//
//             var data = response.Value ?? new List<TEntity>();
//
//             // Perform sorting if applicable
//             if (!string.IsNullOrEmpty(sortBy))
//             {
//                 var propertyInfo = typeof(TEntity).GetProperty(sortBy);
//                 if (propertyInfo != null)
//                 {
//                     data = isDescending
//                         ? data.OrderByDescending(e => propertyInfo.GetValue(e)).ToList()
//                         : data.OrderBy(e => propertyInfo.GetValue(e)).ToList();
//                 }
//             }
//
//             // Return data in the required format
//             return new TableData<TEntity>
//             {
//                 Items = data,
//                 TotalItems = data.Count // Replace this with total count from backend if available
//             };
//         }
//         catch (Exception ex)
//         {
//             Console.Error.WriteLine($"Error fetching items: {ex.Message}");
//             return new TableData<TEntity>
//             {
//                 Items = new List<TEntity>(),
//                 TotalItems = 0
//             };
//         }
//     }
//
//
//     public void InvalidateCache() => _cache.Clear();
//
//     public IObservable<Unit> RefreshTrigger => _refreshTrigger.AsObservable();
//
//     private readonly Subject<Unit> _refreshTrigger = new();
// }
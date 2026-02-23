// using System.Reactive;
// using MudBlazor;
//
// namespace Mamey.BlazorWasm.Components.Grid2;
//
// public interface IReactiveService<TEntity, TApiClient>
// {
//     Task<TableData<TEntity>> FetchItemsAsync(
//         TableState tableState,
//         CancellationToken cancellationToken);
//     IObservable<Unit> RefreshTrigger { get; }
//     void InvalidateCache();
// }
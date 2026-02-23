using System.Reactive;
using Mamey.Http;
using MudBlazor;
using Mamey.Blazor.Abstractions.Types;

namespace Mamey.BlazorWasm.Components.Grid;

public interface IReactiveService<TModel, TApiClient>
    where TModel : IUIModel
    where TApiClient : MameyHttpClient
{

    List<TModel> Items { get; }
    IObservable<Unit> RefreshTrigger { get; }
    Task<TableData<TModel>> FetchGridItemsAsync(int page, int pageSize, string sortBy = null, bool descending = false, string filter = null);
    Task<List<TModel>> FetchItemsAsync();
    Task<IObservable<List<TModel>>> GetAsync();
    IObservable<TModel?> GetById(Guid id);
    Task<IObservable<TModel>> CreateAsync<TRequest>(TRequest request, IEnumerable<KeyValuePair<string, string>>? headers) where TRequest : IRequest;
    Task<IObservable<bool>> Delete(Guid id, string endpoint, IEnumerable<KeyValuePair<string, string>>? headers);
    Task<IObservable<bool>> Update<TRequest>(Guid id, TRequest request);
    Task<IObservable<TModel>> CreateOrUpdateAsync<TRequest>(Guid? id, TRequest request, IEnumerable<KeyValuePair<string, string>>? headers) where TRequest : IRequest;
}


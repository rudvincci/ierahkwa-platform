using System.Reactive;
using Mamey.Blazor.Abstractions.Types;
using Mamey.Http;
using MudBlazor;

namespace Mamey.BlazorWasm.Components.Table;

public interface IMameyTableReactiveService<TModel, TApiClient>
    where TModel : IUIModel
    where TApiClient : MameyHttpClient
{
    List<TModel> Items { get; }
    
    IObservable<Unit> RefreshTrigger { get; }
    Task<IObservable<List<TModel>>> GetAsync();
    IObservable<TModel?> GetById(Guid id);
    
    Task<TableData<TModel>> FetchTableItemsAsync(int page, int pageSize, string sortBy = null, 
        bool descending = false, string filter = null);
    Task<List<TModel>> FetchItemsAsync();

    Task<IObservable<TModel>> CreateAsync<TRequest>(TRequest request, 
        IEnumerable<KeyValuePair<string, string>>? headers) where TRequest : IRequest;
    
    Task<IObservable<bool>> Update<TRequest>(Guid id, TRequest request);
    
    Task<IObservable<TModel>> CreateOrUpdateAsync<TRequest>(Guid? id, TRequest request, 
        IEnumerable<KeyValuePair<string, string>>? headers) where TRequest : IRequest;
    Task<IObservable<bool>> Delete(Guid id, string endpoint, 
        IEnumerable<KeyValuePair<string, string>>? headers);
}
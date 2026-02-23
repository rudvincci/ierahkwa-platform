using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Mamey.Blazor.Abstractions.Api;
using Mamey.Blazor.Abstractions.Types;
using Mamey.Http;
using MudBlazor;
using ReactiveUI;

namespace Mamey.BlazorWasm.Components.Table;

public class MameyTableReactiveService<TModel, TApiClient> : ReactiveObject, IMameyTableReactiveService<TModel, TApiClient>
    where TModel : IUIModel
    where TApiClient : MameyHttpClient
{
    protected readonly TApiClient _client;
    protected readonly IObservableApiResponseHandler _apiResponseHandler;
    private readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);
    private readonly string _endpoint;
    private List<TModel> _items = new();
    private readonly Subject<Unit> _refreshTrigger = new();

    public List<TModel> Items => _items;
    public IObservable<Unit> RefreshTrigger => _refreshTrigger.AsObservable();
    public bool _initialized = false;
    
    protected MameyTableReactiveService(TApiClient client, IObservableApiResponseHandler apiResponseHandler, string endpoint)
    {
        _client = client;
        _apiResponseHandler = apiResponseHandler;
        _endpoint = endpoint;
    }

    public async Task InitializeAsync()
    {
        if (!_initialized)
        {
            await _initSemaphore.WaitAsync();
            try
            {
                if (!_initialized)
                {
                    await FetchItemsAsync();
                    _initialized = true;
                    _refreshTrigger.OnNext(Unit.Default);
                }
            }
            finally
            {
                _initSemaphore.Release();
            }
        }
    }
    public async Task<IObservable<List<TModel>>> GetAsync()
    {
        if (!_initialized)
        {
            await InitializeAsync();
            _initialized = true;
        }

        return Observable.Return(_items);
    }
    public virtual IObservable<TModel?> GetById(Guid id)
    {
        return Observable.Return(_items.FirstOrDefault(item => item.Id == id));
    }

    public Task<TableData<TModel>> FetchTableItemsAsync(int page, int pageSize, string sortBy = null, bool descending = false, string filter = null)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<List<TModel>> FetchItemsAsync()
    {
        try
        {
            _items = await _apiResponseHandler.HandleAsync(_client.GetApiResponseAsync<List<TModel>>(_endpoint));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching items: {ex.Message}");
            _items = new List<TModel>();
        }
        return _items;
    }

    public virtual async Task<IObservable<TModel>> CreateAsync<TRequest>(TRequest request, IEnumerable<KeyValuePair<string, string>>? headers)
        where TRequest : IRequest
        => await _apiResponseHandler.HandleAsObservableAsync(_client.PostApiResponseAsync<TRequest, TModel>($"{_endpoint}/{request.Id}", request, headers));

    public virtual async Task<IObservable<bool>> Delete(Guid id, string endpoint, IEnumerable<KeyValuePair<string, string>>? headers)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            var response = await _apiResponseHandler.HandleAsync(_client.DeleteApiResponseAsync(endpoint));
            if (response != null && response.Succeeded)
            {
                _items.Remove(item);
                return Observable.Return(true);
            }
            return Observable.Return(false);
        }

        return Observable.Return(false);
    }

    public virtual async Task<IObservable<bool>> Update<TRequest>(Guid id, TRequest request)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        var response = await _apiResponseHandler.HandleAsync(_client.PutApiResponseAsync($"{_endpoint}/{id}", request));

        if (response != null && response.Succeeded)
        {
            return Observable.Return(true);
        }
        else if (response != null && response.StatusCode == HttpStatusCode.NotFound)
        {
            return Observable.Throw<bool>(new Exception("Item not found"));
        }
        else
        {
            return Observable.Throw<bool>(new Exception("Sorry. Something happened on our end."));
        }
    }

    public Task<IObservable<TModel>> CreateOrUpdateAsync<TRequest>(Guid? id, TRequest request, IEnumerable<KeyValuePair<string, string>>? headers) where TRequest : IRequest
    {
        throw new NotImplementedException();
    }
}
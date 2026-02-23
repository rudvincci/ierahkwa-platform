using System.Net.Http.Json;

namespace Pupitre.UI.Shared.Services;

public abstract class BaseApiClient<TDto, TDetailsDto, TCreateCommand, TUpdateCommand> 
    : IApiClient<TDto, TDetailsDto, TCreateCommand, TUpdateCommand>, IPagedApiClient<TDto>
    where TDto : class
    where TDetailsDto : class
    where TCreateCommand : class
    where TUpdateCommand : class
{
    protected readonly HttpClient Http;
    protected abstract string BasePath { get; }

    protected BaseApiClient(HttpClient http)
    {
        Http = http;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await Http.GetFromJsonAsync<IEnumerable<TDto>>(BasePath, ct);
        return result ?? Enumerable.Empty<TDto>();
    }

    public virtual async Task<PagedResult<TDto>> BrowseAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var result = await Http.GetFromJsonAsync<PagedResult<TDto>>($"{BasePath}?page={page}&results={pageSize}", ct);
        return result ?? new PagedResult<TDto>(Enumerable.Empty<TDto>(), 1, 1, 0, false, false);
    }

    public virtual async Task<TDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Http.GetFromJsonAsync<TDetailsDto>($"{BasePath}/{id}", ct);
    }

    public virtual async Task<Guid> CreateAsync(TCreateCommand command, CancellationToken ct = default)
    {
        var response = await Http.PostAsJsonAsync(BasePath, command, ct);
        response.EnsureSuccessStatusCode();
        
        if (response.Headers.Location != null)
        {
            var segments = response.Headers.Location.Segments;
            if (segments.Length > 0 && Guid.TryParse(segments[^1].TrimEnd('/'), out var id))
            {
                return id;
            }
        }
        
        return Guid.Empty;
    }

    public virtual async Task UpdateAsync(Guid id, TUpdateCommand command, CancellationToken ct = default)
    {
        var response = await Http.PutAsJsonAsync($"{BasePath}/{id}", command, ct);
        response.EnsureSuccessStatusCode();
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await Http.DeleteAsync($"{BasePath}/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}

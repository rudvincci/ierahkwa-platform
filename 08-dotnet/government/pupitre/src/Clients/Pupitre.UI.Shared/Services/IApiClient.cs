namespace Pupitre.UI.Shared.Services;

public interface IApiClient<TDto, TDetailsDto, TCreateCommand, TUpdateCommand>
    where TDto : class
    where TDetailsDto : class
    where TCreateCommand : class
    where TUpdateCommand : class
{
    Task<IEnumerable<TDto>> GetAllAsync(CancellationToken ct = default);
    Task<TDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(TCreateCommand command, CancellationToken ct = default);
    Task UpdateAsync(Guid id, TUpdateCommand command, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IPagedApiClient<TDto> where TDto : class
{
    Task<PagedResult<TDto>> BrowseAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
}

public record PagedResult<T>(IEnumerable<T> Items, int CurrentPage, int TotalPages, int TotalCount, bool HasPrevious, bool HasNext);

using Mamey.Domain.Service.BlazorWasm.ApiClients;
using Mamey.Domain.Service.Contracts;

namespace Mamey.Domain.Service.BlazorWasm.Services;

public interface IServiceService
{
    Task<Guid> CreateAsync(CreateEntityCommand command);
    Task<EntityDto> GetByIdAsync(Guid id);
    Task<PagedResult<EntitySummaryDto>> GetAsync(GetEntitiesQuery query);
    Task UpdateAsync(Guid id, UpdateEntityCommand command);
    Task DeleteAsync(Guid id);
}

public class ServiceService : IServiceService
{
    private readonly IServiceApiClient _apiClient;

    public ServiceService(IServiceApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Guid> CreateAsync(CreateEntityCommand command)
    {
        return await _apiClient.CreateAsync(command);
    }

    public async Task<EntityDto> GetByIdAsync(Guid id)
    {
        return await _apiClient.GetByIdAsync(id);
    }

    public async Task<PagedResult<EntitySummaryDto>> GetAsync(GetEntitiesQuery query)
    {
        return await _apiClient.GetAsync(query);
    }

    public async Task UpdateAsync(Guid id, UpdateEntityCommand command)
    {
        await _apiClient.UpdateAsync(id, command);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _apiClient.DeleteAsync(id);
    }
}












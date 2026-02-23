using Mamey.Domain.Service.Contracts;
using Refit;

namespace Mamey.Domain.Service.BlazorWasm.ApiClients;

public interface IServiceApiClient
{
    [Post("/api/service")]
    Task<Guid> CreateAsync([Body] CreateEntityCommand command);
    
    [Get("/api/service/{id}")]
    Task<EntityDto> GetByIdAsync(Guid id);
    
    [Get("/api/service")]
    Task<PagedResult<EntitySummaryDto>> GetAsync([Query] GetEntitiesQuery query);
    
    [Put("/api/service/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateEntityCommand command);
    
    [Delete("/api/service/{id}")]
    Task DeleteAsync(Guid id);
}












namespace INKG.CitizenPortal.ApiGataway.Infrastructure;

internal interface IPayloadBuilder
{
    Task<T> BuildFromJsonAsync<T>(HttpRequest request) where T : class, new();
}

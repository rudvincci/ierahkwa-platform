namespace Mamey.Government.Identity.BlazorWasm.Storage;

public interface ITokenStore
{
    Task SaveAsync(string token);
    Task<string?> GetAsync();
    Task ClearAsync();
}
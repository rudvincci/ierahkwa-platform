namespace Mamey.BlazorWasm.Http;

public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}
public interface ISessionStorageService
{
    Task SetSessionStorageAsync<T>(string key, T value);
}

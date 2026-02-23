using System.Text.Json;
using Mamey.BlazorWasm.Http;
using Microsoft.JSInterop;

namespace Mamey.BlazorWasm.Api;

public class LocalStorageService : ILocalStorageService
{
    private IJSRuntime _jsRuntime;


    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            return json is { }
            ? JsonSerializer.Deserialize<T>(json, JsonExtensions.SerializerOptions)
            : default;
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }
    

    public async Task SetItemAsync<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value, JsonExtensions.SerializerOptions));
    }

    public async Task RemoveItemAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}

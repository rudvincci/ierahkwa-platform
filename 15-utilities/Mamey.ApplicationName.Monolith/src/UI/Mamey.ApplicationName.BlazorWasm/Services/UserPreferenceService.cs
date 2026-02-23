using Blazored.LocalStorage;
using Mamey.ApplicationName.BlazorWasm.Configuration;

namespace Mamey.ApplicationName.BlazorWasm.Services
{
    public class UserPreferenceService
    {
        private readonly ILocalStorageService _localStorage;

        public event Action? OnChange; // Marking the event as nullable

        public UserPreferences UserPreferences { get; private set; } = new UserPreferences(); // Initialize

        public UserPreferenceService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public async Task LoadUserPreferencesAsync(Guid userId)
        {
            var storedPreferences = await _localStorage.GetItemAsync<UserPreferences>("userPreferences");
            UserPreferences = storedPreferences ?? new UserPreferences();
            NotifyStateChanged();
        }

        public async Task SaveUserPreferencesAsync()
        {
            await _localStorage.SetItemAsync("userPreferences", UserPreferences);
            NotifyStateChanged();
        }

        public async Task ResetPreferencesAsync()
        {
            UserPreferences = new UserPreferences();
            await _localStorage.RemoveItemAsync("userPreferences");
            NotifyStateChanged();
        }
    }
}


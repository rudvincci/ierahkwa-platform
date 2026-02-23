using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.Configuration
{
    public class ThemePreferences : ReactiveObject
    {
        private string _theme = "light";
        private bool _useSystemTheme = false;
        private bool _isCondensedLayout = false;

        public string Theme
        {
            get => _theme;
            set => this.RaiseAndSetIfChanged(ref _theme, value);
        }

        public bool UseSystemTheme
        {
            get => _useSystemTheme;
            set => this.RaiseAndSetIfChanged(ref _useSystemTheme, value);
        }

        public bool IsCondensedLayout
        {
            get => _isCondensedLayout;
            set => this.RaiseAndSetIfChanged(ref _isCondensedLayout, value);
        }
    }
}
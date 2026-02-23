using MudBlazor;
using ReactiveUI;
using Blazored.LocalStorage;
using MudBlazor.Utilities;

namespace Mamey.ApplicationName.BlazorWasm.Configuration
{
    public class GlobalSettings : ReactiveObject
    {
        private readonly ILocalStorageService _localStorage;

        public GlobalSettings(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private string _siteTitle = "Futurehead Group";
        private bool _isDarkMode = true;
        private MudTheme? _theme = null;

        public MudTheme? Theme
        {
            get => _theme;
            set
            {
                this.RaiseAndSetIfChanged(ref _theme, value);
                SaveSettingAsync(nameof(SiteTitle), value);
            }
        }
        public string SiteTitle
        {
            get => _siteTitle;
            set
            {
                this.RaiseAndSetIfChanged(ref _siteTitle, value);
                SaveSettingAsync(nameof(SiteTitle), value);
            }
        }

        public string AppName { get; } = "Futurehead Group";

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDarkMode, value);
                SaveSettingAsync(nameof(IsDarkMode), value);
                
            }
        }
        
        public Palette Palette { get; set; } 

        public PaletteLight LandingPalette { get; set; } = new()
        {
            Black = "#110e2d",
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,0.8)",
            DrawerBackground = "#ffffff",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
            Background = Colors.BlueGray.Default,
        };
        public PaletteLight LightPalette { get; set; } = new()
        {
            Primary = "#2C74B3", // Sky Blue
            Secondary = "#205295", // Deeper Blue
            AppbarBackground = "rgba(255,255,255,0.9)", // Translucent white
            AppbarText = "#0A2647", // Deep Navy
            Background = "#F5F7FA", // Light gray background
            BackgroundGray = "#E6ECF2", // Subtle contrast
            Surface = "#FFFFFF", // Standard white surface
            DrawerBackground = "#FFFFFF",
            DrawerText = "#0A2647",
            DrawerIcon = "#0A2647",
            LinesDefault = "#D3DDE8", // Light dividers
            TableLines = "#D3DDE8",
            Divider = "#D3DDE8",
            TextPrimary = "#0A2647", // Strong dark navy
            TextSecondary = "#144272", // Royal Blue for subtext
            TextDisabled = "#14427266", // Transparent blue subtext
            ActionDefault = "#205295",
            ActionDisabled = "#2052954D",
            ActionDisabledBackground = "#2C74B333",
            GrayLight = "#F0F4FA", // Pale blue-tinted gray
            GrayLighter = "#FAFCFF", // Almost white with a tint
            OverlayLight = "#0A264720", // Very light navy overlay
            Success = "#3DCA6C",
            Info = "#2C74B3",
            Warning = "#FFC107",
            Error = "#F44336"
        };
        
        public PaletteDark DarkPalette { get; set; } = new()
        {
            Primary = "#2C74B3", // Brightest blue
            Secondary = "#205295", // Base secondary
            AppbarBackground = "rgba(10,38,71,0.8)", // Slightly transparent deep navy
            AppbarText = "#B0BEDF", // Muted light blue-gray text for contrast
            Background = "#0A2647", // Deep navy as the base
            BackgroundGray = "#144272", // Royal blue for gray contrast
            Surface = "#144272", // Same as background gray
            DrawerBackground = "#0A2647", // Match base background for immersive effect
            DrawerText = "#DCE3F3", // Light text for contrast
            DrawerIcon = "#DCE3F3",
            LinesDefault = "#205295", // Slight blue line color
            TableLines = "#205295",
            Divider = "#144272",
            TextPrimary = "#E0ECFF", // Light blue-white for visibility
            TextSecondary = "#B0BEDF", // Subdued secondary text
            TextDisabled = "#B0BEDF66", // Semi-transparent secondary text
            ActionDefault = "#B0BEDF",
            ActionDisabled = "#B0BEDF4D",
            ActionDisabledBackground = "#20529533",
            GrayLight = "#1B3B66", // Lighter blue shade
            GrayLighter = "#2C74B3",
            OverlayLight = "#0A264799", // Semi-transparent overlay
            Success = "#3DCA6C",
            Info = "#2C74B3",
            Warning = "#FFC107",
            Error = "#F44336"
        };
        
        public PaletteDark AdminDarkPalette { get; set; } = new()
        {
            Primary = "#697565", // Moss Green
            Secondary = "#3C3D37", // Charcoal Gray
            AppbarBackground = "rgba(24,28,20,0.9)", // Translucent deep green-black
            AppbarText = "#ECDFCC", // Soft cream
            Background = "#181C14", // Very dark forest green
            BackgroundGray = "#3C3D37", // Charcoal gray
            Surface = "#2B2C26", // Slightly lighter than background
            DrawerBackground = "#181C14",
            DrawerText = "#ECDFCC",
            DrawerIcon = "#ECDFCC",
            LinesDefault = "#3C3D37",
            TableLines = "#3C3D37",
            Divider = "#2E302A",
            TextPrimary = "#ECDFCC",
            TextSecondary = "#B8B2A3", // Aged paper tint
            TextDisabled = "#ECDFCC66",
            ActionDefault = "#B8B2A3",
            ActionDisabled = "#B8B2A34D",
            ActionDisabledBackground = "#69756533",
            GrayLight = "#3C3D37",
            GrayLighter = "#2B2C26",
            OverlayLight = "#181C1499",
            Success = "#3DCA6C",
            Info = "#697565",
            Warning = "#E6B400",
            Error = "#D9534F"
        };
        public PaletteLight AdminLightPalette { get; set; } = new()
        {
            Primary = "#697565", // Moss Green
            Secondary = "#3C3D37", // Charcoal Gray
            AppbarBackground = "rgba(255,255,255,0.95)",
            AppbarText = "#3C3D37",
            Background = "#FAF8F3", // Slightly tinted off-white
            BackgroundGray = "#ECDFCC", // Cream from the palette
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#3C3D37",
            DrawerIcon = "#3C3D37",
            LinesDefault = "#D3CEC3",
            TableLines = "#D3CEC3",
            Divider = "#D3CEC3",
            TextPrimary = "#181C14", // Deep forest green
            TextSecondary = "#3C3D37", // Charcoal
            TextDisabled = "#3C3D3766",
            ActionDefault = "#3C3D37",
            ActionDisabled = "#3C3D374D",
            ActionDisabledBackground = "#69756533",
            GrayLight = "#F0EFE9",
            GrayLighter = "#FAF8F3",
            OverlayLight = "#181C1420",
            Success = "#3DCA6C",
            Info = "#697565",
            Warning = "#E6B400",
            Error = "#D9534F"
        };


        public async Task InitializeAsync()
        {
            SiteTitle = await LoadSettingAsync(nameof(SiteTitle), "Futurehead Group");
            IsDarkMode = await LoadSettingAsync(nameof(IsDarkMode), _isDarkMode);
        }
        public MudTheme GetLandingTheme()
        {
            return new MudTheme()
            {
                PaletteDark = DarkPalette,
                PaletteLight = LightPalette,
                
                Typography = new Typography()
                {
                    Default = new Body1Typography()
                    {
                        FontFamily = new[] { "Inter", "Roboto", "Helvetica", "Arial", "sans-serif" },
                    },
                    H1 = new H1Typography
                    {
                        FontSize = "3rem",
                        FontWeight = "600",
                    },
                    H2 = new H2Typography
                    {
                        FontSize = "2.25rem",
                        FontWeight = "600"
                    },
                    // You can define more typography settings here...
                },
                LayoutProperties = new LayoutProperties
                {
                    DefaultBorderRadius = "6px"
                },
                ZIndex = new ZIndex()
                {
                    Drawer = 1300
                }
            };
        }
        public MudTheme GetUserTheme()
        {
            return new MudTheme()
            {
                PaletteDark = AdminDarkPalette,
                PaletteLight = LightPalette,
                
                Typography = new Typography()
                {
                    Default = new Body1Typography()
                    {
                        FontFamily = new[] { "Inter", "Roboto", "Helvetica", "Arial", "sans-serif" },
                    },
                    H1 = new H1Typography
                    {
                        FontSize = "3rem",
                        FontWeight = "600",
                    },
                    H2 = new H2Typography
                    {
                        FontSize = "2.25rem",
                        FontWeight = "600"
                    },
                    // You can define more typography settings here...
                },
                LayoutProperties = new LayoutProperties
                {
                    DefaultBorderRadius = "6px"
                },
                ZIndex = new ZIndex()
                {
                    Drawer = 1300
                }
            };
        }
        public MudTheme GetAdminTheme()
        {
            return new MudTheme()
            {
                PaletteDark = AdminDarkPalette,
                PaletteLight = LightPalette,
                
                Typography = new Typography()
                {
                    Default = new Body1Typography()
                    {
                        FontFamily = new[] { "Inter", "Roboto", "Helvetica", "Arial", "sans-serif" },
                    },
                    H1 = new H1Typography
                    {
                        FontSize = "3rem",
                        FontWeight = "600",
                    },
                    H2 = new H2Typography
                    {
                        FontSize = "2.25rem",
                        FontWeight = "600"
                    },
                    // You can define more typography settings here...
                },
                LayoutProperties = new LayoutProperties
                {
                    DefaultBorderRadius = "6px"
                },
                ZIndex = new ZIndex()
                {
                    Drawer = 1300
                }
            };
        }
        private async Task SaveSettingAsync<T>(string key, T value)
        {
            await _localStorage.SetItemAsync(key, value);
        }

        private async Task<T> LoadSettingAsync<T>(string key, T defaultValue)
        {
            var localSettings = await _localStorage.GetItemAsync<T>(key) ?? defaultValue;
            return localSettings;
        }
    }
}

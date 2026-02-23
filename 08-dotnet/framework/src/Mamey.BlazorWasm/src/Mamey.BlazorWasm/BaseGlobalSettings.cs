using Mamey.BlazorWasm.Themes;
using Mamey.Persistence.Redis;
using Mamey.Types;
using MudBlazor;
using ReactiveUI;

namespace Mamey.BlazorWasm
{
    /// <summary>
    /// Blazor Base Global Settings
    /// </summary>
    public abstract class BaseGlobalSettings : ReactiveObject
    {
        protected readonly ICache _cache;
        protected string _siteTitle ;
        protected bool _isDarkMode = false;
        protected MudTheme? _theme = null;

        public BaseGlobalSettings(ICache cache, AppOptions appOptions)
        {
            _cache = cache;
            AppName = appOptions.Name ?? "Mamey Technologies";
            _siteTitle = appOptions.SiteTitle ?? "Mamey Technologies";
        }


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

        public string AppName { get; }
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDarkMode, value);
                SaveSettingAsync(nameof(IsDarkMode), value);
                
            }
        }
        
        public Palette Palette { get; protected set; } 

        public PaletteLight LandingPalette { get; protected set; } = new()
        {
            Black = "#110e2d",
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,0.8)",
            DrawerBackground = "#ffffff",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
            Background = Colors.BlueGray.Default,
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
            await Task.CompletedTask;
            // SiteTitle = await LoadSettingAsync(nameof(SiteTitle), "Futurehead Group");
            // IsDarkMode = await LoadSettingAsync(nameof(IsDarkMode), _isDarkMode);
        }
        public MudTheme GetLandingTheme()
        {
            // return new MudTheme()
            // {
            //     PaletteDark = MameyThemes.DefaultDarkPalette,
            //     PaletteLight = MameyThemes.DefaultLightPalette,
            //     
            //     Typography = new Typography()
            //     {
            //         Default = new Body1Typography()
            //         {
            //             FontFamily = new[] { "Inter", "Roboto", "Helvetica", "Arial", "sans-serif" },
            //         },
            //         H1 = new H1Typography
            //         {
            //             FontSize = "3rem",
            //             FontWeight = "600",
            //         },
            //         H2 = new H2Typography
            //         {
            //             FontSize = "2.25rem",
            //             FontWeight = "600"
            //         },
            //         // You can define more typography settings here...
            //     },
            //     LayoutProperties = new LayoutProperties
            //     {
            //         DefaultBorderRadius = "6px"
            //     },
            //     ZIndex = new ZIndex()
            //     {
            //         Drawer = 1300
            //     }
            // };
            return MameyThemes.MameyDefaultTheme;
        }
        public MudTheme GetUserTheme()
        {
            return new MudTheme()
            {
                PaletteDark = AdminDarkPalette,
                PaletteLight = MameyThemes.DefaultLightPalette,
                
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
                PaletteLight = MameyThemes.DefaultLightPalette,
                
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
        protected async Task SaveSettingAsync<T>(string key, T value)
        {
            await _cache.AddToSetAsync(key, value);
        }
        
    }
}

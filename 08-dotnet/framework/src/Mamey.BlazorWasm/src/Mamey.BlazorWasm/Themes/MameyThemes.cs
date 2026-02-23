using MudBlazor;

namespace Mamey.BlazorWasm.Themes;

public static class MameyThemes
{
    public static PaletteDark DefaultDarkPalette = new()
    {
        // Branding
        Primary = "#2C74B3",
        Secondary = "#205295",

        // Core theme colors
        Black = "#27272f",
        Dark = "#27272f",

        // Typography
        TextPrimary = "rgba(255,255,255,0.87)", // Clear white text
        TextSecondary = "rgba(255,255,255,0.60)", // Softer white
        TextDisabled = "rgba(255,255,255,0.38)", // For disabled content

        // Action states
        ActionDefault = "#B0BEDF", // Matches your light blue-gray
        ActionDisabled = "rgba(255,255,255,0.26)",
        ActionDisabledBackground = "rgba(255,255,255,0.12)",

        // Backgrounds & surfaces
        Background = "#121212", // True dark for contrast
        BackgroundGray = "#1C1C1E",
        Surface = "#1E1E20",
        AppbarBackground = "#1A1F2B",
        AppbarText = "rgba(255,255,255,0.87)",
        DrawerBackground = "#121212",
        DrawerText = "rgba(255,255,255,0.70)",
        DrawerIcon = "rgba(255,255,255,0.70)",

        // Line work and dividers
        LinesDefault = "rgba(255,255,255,0.12)",
        LinesInputs = "rgba(255,255,255,0.30)",
        TableLines = "rgba(255,255,255,0.12)",
        TableStriped = "rgba(255,255,255,0.06)",
        Divider = "rgba(255,255,255,0.12)",
        DividerLight = "rgba(255,255,255,0.06)",

        // Visual skeleton loaders
        Skeleton = "rgba(255,255,255,0.11)",

        // Overlays
        OverlayLight = "rgba(0,0,0,0.5)",

        // Feedback colors (standard)
        Success = "#3DCA6C", // Green
        Info = "#2C74B3", // Reuse primary
        Warning = "#FFC107", // Amber
        Error = "#F44336", // Red

        // Optional branding tints
        GrayLight = "#2A2A2E",
        GrayLighter = "#3A3A3E"
    };

    public static PaletteLight DefaultLightPalette = new()
    {
        // Branding
        Primary = "#2C74B3",
        Secondary = "#205295",

        // Core colors
        Black = "#000000",
        Dark = "#1A2B3C",

        // Typography
        TextPrimary = "rgba(0,0,0,0.87)",
        TextSecondary = "rgba(0,0,0,0.60)",
        TextDisabled = "rgba(0,0,0,0.38)",

        // Actions
        ActionDefault = "#4C6E91",
        ActionDisabled = "rgba(0,0,0,0.26)",
        ActionDisabledBackground = "rgba(0,0,0,0.12)",

        // Backgrounds
        Background = "#F4F8FB", // Light blue background
        BackgroundGray = "#E9F0F7",
        Surface = "#EDF3FA", // << Blue-tinted surface (was #FFFFFF)
        AppbarBackground = "#E3EDF7",
        AppbarText = "rgba(0,0,0,0.87)",
        DrawerBackground = "#F0F4FA",
        DrawerText = "rgba(0,0,0,0.70)",
        DrawerIcon = "rgba(0,0,0,0.70)",

        // Dividers and lines
        LinesDefault = "rgba(0,0,0,0.12)",
        LinesInputs = "rgba(0,0,0,0.30)",
        TableLines = "rgba(0,0,0,0.12)",
        TableStriped = "rgba(44,116,179,0.03)",
        Divider = "rgba(0,0,0,0.12)",
        DividerLight = "rgba(0,0,0,0.06)",

        // Effects
        Skeleton = "rgba(0,0,0,0.08)",
        OverlayLight = "rgba(255,255,255,0.5)",

        // Feedback
        Success = "#2E7D32",
        Info = "#2C74B3",
        Warning = "#FFA000",
        Error = "#D32F2F",

        // Blue-tinted grays
        GrayLight = "#DCE6F0",
        GrayLighter = "#EDF3FA"
    };

    public static Typography DefaultTypography = new Typography
    {
        Default = new DefaultTypography
        {
            FontFamily = new[]
            {
                
                "Segoe UI",
                "Roboto",
                "Helvetica Neue",
                "sans-serif"
            },
            FontSize = "0.95rem",
            FontWeight = "400"
        },
        // Subtitle1 = new DefaultTypography()
        // {
        //     FontFamily = new[] { "Segoe UI", "Roboto", "sans-serif" },
        //     FontSize = "1.60rem",
        //     FontWeight = "300",
        //     LetterSpacing = "0.01rem"
        // },
        
        H1 = new H1Typography()
        {
            FontFamily = new[] { "Orbitron", "Segoe UI", "sans-serif" },
            FontSize = "2.8rem",
            FontWeight = "700",
            LetterSpacing = "0.02rem"
        },
        H2 = new H2Typography()
        {
            FontFamily = new[] { "Orbitron", "Segoe UI", "sans-serif" },
            FontSize = "2.2rem",
            FontWeight = "600",
            LetterSpacing = "0.015rem"
        },
        H3 = new H3Typography()
        {
            FontFamily = new[] { "Orbitron", "Segoe UI", "sans-serif" },
            FontSize = "1.8rem",
            FontWeight = "600"
        },
        H4 = new H4Typography()
        {
            FontFamily = new[] { "Orbitron", "Segoe UI", "sans-serif" },
            FontSize = "1.4rem",
            FontWeight = "600"
        },
        H5 = new H5Typography()
        {
            FontFamily = new[] { "Orbitron", "Segoe UI", "sans-serif" },
            FontSize = "1rem",
            FontWeight = "600"
        },
        
        H6 = new H6Typography
        {
            FontFamily = new[]
            {
                "Orbitron",
                "Segoe UI",
                "sans-serif"
            },
            FontSize = "0.95rem",
            FontWeight = "600"
        },

        Button = new DefaultTypography()
        {
            FontFamily = new[] { "Orbitron", "Segoe UI", "sans-serif" },
            FontSize = "0.75rem",
            FontWeight = "600",
            TextTransform = "uppercase"
        },
        Caption = new DefaultTypography
        {
            FontFamily = new[] { "Segoe UI", "Roboto", "sans-serif" },
            FontSize = "0.75rem",
            FontWeight = "300",
            LetterSpacing = "0.01rem"
        },
        Overline = new DefaultTypography
        {
            FontFamily = new[] { "Segoe UI", "Roboto", "sans-serif" },
            FontSize = "0.7rem",
            LetterSpacing = "0.12em",
            TextTransform = "uppercase"
        }
    };

    public static Shadow DefaultShadows = new Shadow()
    {
        Elevation = new[]
        {
            "none",
            "0px 1px 3px rgba(0,0,0,0.2)",
            "0px 4px 8px rgba(0,255,255,0.1)",
            "0px 10px 20px rgba(0,229,255,0.2)"
        }
    };

    public static LayoutProperties DefaultLayoutProperties = new LayoutProperties
    {
        DefaultBorderRadius = "8px",
        AppbarHeight = "64px",
        DrawerWidthLeft = "300px",
        DrawerWidthRight = "300px",
        // DrawerMiniWidthLeft = "",
        // DrawerMiniWidthRight = "",
    };

    public static ZIndex DefaultZIndex = new ZIndex()
    {
        Drawer = 1300,
        AppBar = 1400,
        Dialog = 1500,
        Snackbar = 1600,
        Tooltip = 1700,
        // Popover = 1800,
    };

    public static MudTheme MameyDefaultTheme = new MudTheme()
    {
        PaletteDark = DefaultDarkPalette,
        PaletteLight = DefaultLightPalette,
        Typography = DefaultTypography,
        LayoutProperties = DefaultLayoutProperties,
        // ZIndex = DefaultZIndex,
        // Shadows = DefaultShadows,
    };
}


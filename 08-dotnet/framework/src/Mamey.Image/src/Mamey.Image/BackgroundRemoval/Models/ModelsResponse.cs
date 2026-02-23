namespace Mamey.Image.BackgroundRemoval.Models;

/// <summary>
/// Response model for available models.
/// </summary>
public class ModelsResponse
{
    /// <summary>
    /// List of available background removal models.
    /// </summary>
    public string[] Models { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Currently active model.
    /// </summary>
    public string CurrentModel { get; set; } = string.Empty;
}


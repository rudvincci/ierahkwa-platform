using System.ComponentModel.DataAnnotations;

namespace Mamey.Image.BackgroundRemoval.Models;

/// <summary>
/// Request model for background removal.
/// </summary>
public class RemoveBackgroundRequest
{
    /// <summary>
    /// Image file to process.
    /// </summary>
    [Required]
    public Stream ImageStream { get; set; } = Stream.Null;

    /// <summary>
    /// Original filename of the image.
    /// </summary>
    public string? Filename { get; set; }

    /// <summary>
    /// Content type of the image.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Output format (PNG, JPEG).
    /// </summary>
    public string OutputFormat { get; set; } = "PNG";

    /// <summary>
    /// Background removal model to use.
    /// </summary>
    public string? Model { get; set; }
}


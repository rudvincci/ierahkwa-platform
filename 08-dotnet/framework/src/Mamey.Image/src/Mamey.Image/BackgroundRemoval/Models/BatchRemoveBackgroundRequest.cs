using System.ComponentModel.DataAnnotations;

namespace Mamey.Image.BackgroundRemoval.Models;

/// <summary>
/// Request model for batch background removal.
/// </summary>
public class BatchRemoveBackgroundRequest
{
    /// <summary>
    /// Image files to process.
    /// </summary>
    [Required]
    public List<ImageFile> ImageFiles { get; set; } = new();

    /// <summary>
    /// Output format (PNG, JPEG).
    /// </summary>
    public string OutputFormat { get; set; } = "PNG";

    /// <summary>
    /// Background removal model to use.
    /// </summary>
    public string? Model { get; set; }
}

/// <summary>
/// Image file information for batch processing.
/// </summary>
public class ImageFile
{
    /// <summary>
    /// Image file stream.
    /// </summary>
    [Required]
    public Stream ImageStream { get; set; } = Stream.Null;

    /// <summary>
    /// Original filename.
    /// </summary>
    public string? Filename { get; set; }

    /// <summary>
    /// Content type of the image.
    /// </summary>
    public string? ContentType { get; set; }
}


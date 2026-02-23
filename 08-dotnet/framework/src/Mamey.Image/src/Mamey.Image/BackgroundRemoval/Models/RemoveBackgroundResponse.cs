namespace Mamey.Image.BackgroundRemoval.Models;

/// <summary>
/// Response model for background removal.
/// </summary>
public class RemoveBackgroundResponse
{
    /// <summary>
    /// Processed image stream with transparent background.
    /// </summary>
    public Stream ProcessedImageStream { get; set; } = Stream.Null;

    /// <summary>
    /// Content type of the processed image.
    /// </summary>
    public string ContentType { get; set; } = "image/png";

    /// <summary>
    /// Suggested filename for the processed image.
    /// </summary>
    public string? SuggestedFilename { get; set; }

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Model used for processing.
    /// </summary>
    public string? ModelUsed { get; set; }

    /// <summary>
    /// Whether the processing was successful.
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}


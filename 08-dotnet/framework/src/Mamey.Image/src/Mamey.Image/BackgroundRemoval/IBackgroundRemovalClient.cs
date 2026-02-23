using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mamey.Image.BackgroundRemoval;

/// <summary>
/// Interface for background removal client.
/// </summary>
public interface IBackgroundRemovalClient
{
    /// <summary>
    /// Remove background from raw image bytes.
    /// </summary>
    /// <param name="imageBytes">The raw image bytes to process</param>
    /// <param name="outputFormat">Output format (PNG, JPEG)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raw bytes of processed image with transparent background</returns>
    Task<byte[]> RemoveBackgroundFromBytesAsync(byte[] imageBytes, string outputFormat = "PNG", CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove background from an image stream.
    /// </summary>
    /// <param name="imageStream">The image stream to process</param>
    /// <param name="outputFormat">Output format (PNG, JPEG)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processed image stream with transparent background</returns>
    Task<Stream> RemoveBackgroundAsync(Stream imageStream, string outputFormat = "PNG", CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove background from an image file.
    /// </summary>
    /// <param name="filePath">Path to the input image file</param>
    /// <param name="outputPath">Path for the output file (optional)</param>
    /// <param name="outputFormat">Output format (PNG, JPEG)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to the processed image file</returns>
    Task<string> RemoveBackgroundFromFileAsync(string filePath, string? outputPath = null, string outputFormat = "PNG", CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove background from multiple images in batch.
    /// </summary>
    /// <param name="filePaths">Paths to the input image files</param>
    /// <param name="outputFormat">Output format (PNG, JPEG)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stream containing ZIP file with processed images</returns>
    Task<Stream> RemoveBackgroundBatchAsync(string[] filePaths, string outputFormat = "PNG", CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available background removal models.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available models</returns>
    Task<string[]> GetAvailableModelsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the background removal service is healthy.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if service is healthy</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}


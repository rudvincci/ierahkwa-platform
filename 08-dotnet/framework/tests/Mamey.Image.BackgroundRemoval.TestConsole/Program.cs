using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mamey.Image.BackgroundRemoval;
using Mamey.Image;

// Create host builder
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add background removal services
        services.AddMameyImageBackgroundRemoval(context.Configuration);
    })
    .Build();

// Get the background removal client
var backgroundRemovalClient = host.Services.GetRequiredService<IBackgroundRemovalClient>();

Console.WriteLine("=== Mamey Image Background Removal Test ===");
Console.WriteLine();

try
{
    // Check service health
    Console.WriteLine("1. Checking service health...");
    var isHealthy = await backgroundRemovalClient.IsHealthyAsync();
    Console.WriteLine($"   Service is {(isHealthy ? "healthy" : "unhealthy")}");
    Console.WriteLine();

    // Get available models
    Console.WriteLine("2. Getting available models...");
    var models = await backgroundRemovalClient.GetAvailableModelsAsync();
    Console.WriteLine($"   Available models: {string.Join(", ", models)}");
    Console.WriteLine();

    // Test directory
    var testDir = "/Volumes/Barracuda/mamey-io/code-cursor/Mamey.Image/testImages";
    var outputDir = Path.Combine(testDir, "output");
    
    // Ensure output directory exists
    Directory.CreateDirectory(outputDir);
    
    // Get all image files from test directory
    var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };
    var imageFiles = Directory.GetFiles(testDir)
        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
        .ToArray();

    Console.WriteLine($"3. Found {imageFiles.Length} image files to process:");
    foreach (var file in imageFiles)
    {
        Console.WriteLine($"   - {Path.GetFileName(file)}");
    }
    Console.WriteLine();

    if (imageFiles.Length == 0)
    {
        Console.WriteLine("No image files found in test directory. Creating a test image...");
        
        // Create a simple test image using System.Drawing
        var testImagePath = Path.Combine(testDir, "test_image.png");
        CreateTestImage(testImagePath);
        imageFiles = new[] { testImagePath };
    }

    // Process each image
    Console.WriteLine("4. Processing images...");
    var processedCount = 0;
    
    foreach (var imageFile in imageFiles)
    {
        try
        {
            Console.WriteLine($"   Processing: {Path.GetFileName(imageFile)}");
            
            // Generate output filename
            var fileName = Path.GetFileNameWithoutExtension(imageFile);
            var outputFile = Path.Combine(outputDir, $"{fileName}_no_bg.png");
            
            // Process using bytes method
            var imageBytes = await File.ReadAllBytesAsync(imageFile);
            var processedBytes = await backgroundRemovalClient.RemoveBackgroundFromBytesAsync(imageBytes, "PNG");
            
            // Save result
            await File.WriteAllBytesAsync(outputFile, processedBytes);
            
            Console.WriteLine($"   ✓ Saved: {Path.GetFileName(outputFile)}");
            processedCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ✗ Error processing {Path.GetFileName(imageFile)}: {ex.Message}");
        }
    }
    
    Console.WriteLine();
    Console.WriteLine($"5. Processing complete! {processedCount}/{imageFiles.Length} images processed successfully.");
    Console.WriteLine($"   Output directory: {outputDir}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

// Helper method to create a test image
static void CreateTestImage(string filePath)
{
    // Create a simple test image with a person-like figure
    using var bitmap = new System.Drawing.Bitmap(200, 300);
    using var graphics = System.Drawing.Graphics.FromImage(bitmap);
    
    // Fill background
    graphics.Clear(System.Drawing.Color.LightBlue);
    
    // Draw a simple person
    using var brush = new System.Drawing.SolidBrush(System.Drawing.Color.PeachPuff);
    using var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 2);
    
    // Head
    graphics.FillEllipse(brush, 80, 20, 40, 40);
    graphics.DrawEllipse(pen, 80, 20, 40, 40);
    
    // Body
    graphics.FillRectangle(brush, 90, 60, 20, 90);
    graphics.DrawRectangle(pen, 90, 60, 20, 90);
    
    // Arms
    graphics.FillRectangle(brush, 70, 70, 20, 50);
    graphics.FillRectangle(brush, 110, 70, 20, 50);
    
    // Legs
    graphics.FillRectangle(brush, 95, 150, 10, 100);
    graphics.FillRectangle(brush, 105, 150, 10, 100);
    
    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
}

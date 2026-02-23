using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.PixelFormats;
namespace Mamey.Image;

public static class ImageProcessorExtensions
{
    public static byte[] ProcessImage(this string path, ImageType? type = null)
    {
        try
        {
            using (var image = LoadImage(path))
            {
                var imageType = type ?? DetectImageType(path);
                return ToByteArray(image, imageType);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing image: {ex.Message}");
            return null;
        }
    }

    public static string ProcessImageAsBase64(this string path, ImageType? type = null)
    {
        try
        {
            using (var image = LoadImage(path))
            {
                var imageType = type ?? DetectImageType(path);
                return ToBase64String(image, imageType);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing image: {ex.Message}");
            return null;
        }
    }

    private static Image<Rgba32> LoadImage(string path)
    {
        try
        {
            return SixLabors.ImageSharp.Image.Load<Rgba32>(path);
        }
        catch (Exception ex)
        {
            throw new IOException($"Could not load image: {ex.Message}");
        }
    }

    private static byte[] ToByteArray(Image<Rgba32> image, ImageType type)
    {
        using (var ms = new MemoryStream())
        {
            IImageEncoder encoder = GetEncoder(type);
            image.Save(ms, encoder);
            return ms.ToArray();
        }
    }

    private static string ToBase64String(Image<Rgba32> image, ImageType type)
    {
        var byteArray = ToByteArray(image, type);
        return Convert.ToBase64String(byteArray);
    }

    private static IImageEncoder GetEncoder(ImageType type)
    {
        return type switch
        {
            ImageType.Jpeg => new JpegEncoder(),
            ImageType.Png => new PngEncoder(),
            ImageType.Gif => new GifEncoder(),
            ImageType.Bmp => new BmpEncoder(),
            ImageType.Tiff => new TiffEncoder(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    private static ImageType DetectImageType(string path)
    {
        var extension = Path.GetExtension(path).ToLower();
        return extension switch
        {
            ".jpg" => ImageType.Jpeg,
            ".jpeg" => ImageType.Jpeg,
            ".png" => ImageType.Png,
            ".gif" => ImageType.Gif,
            ".bmp" => ImageType.Bmp,
            ".tiff" => ImageType.Tiff,
            ".tif" => ImageType.Tiff,
            _ => throw new NotSupportedException($"Unsupported image format: {extension}"),
        };
    }
}

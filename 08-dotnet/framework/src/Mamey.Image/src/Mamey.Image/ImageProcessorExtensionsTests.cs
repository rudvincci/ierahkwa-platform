using Xunit;
namespace Mamey.Image;

public class ImageProcessorExtensionsTests
{
    private const string JpegImagePath = "path/to/valid/jpeg/image.jpg";
    private const string PngImagePath = "path/to/valid/png/image.png";
    private const string GifImagePath = "path/to/valid/gif/image.gif";
    private const string BmpImagePath = "path/to/valid/bmp/image.bmp";
    private const string TiffImagePath = "path/to/valid/tiff/image.tiff";
    private const string UnsupportedImagePath = "path/to/unsupported/image.webp";

    [Fact]
    public void ProcessJpegImageToByteArray_ReturnsNonNullByteArray()
    {
        var result = JpegImagePath.ProcessImage(ImageType.Jpeg);
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);
    }

    [Fact]
    public void ProcessPngImageToByteArray_ReturnsNonNullByteArray()
    {
        var result = PngImagePath.ProcessImage(ImageType.Png);
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);
    }

    [Fact]
    public void ProcessGifImageToByteArray_ReturnsNonNullByteArray()
    {
        var result = GifImagePath.ProcessImage(ImageType.Gif);
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);
    }

    [Fact]
    public void ProcessBmpImageToByteArray_ReturnsNonNullByteArray()
    {
        var result = BmpImagePath.ProcessImage(ImageType.Bmp);
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);
    }

    [Fact]
    public void ProcessTiffImageToByteArray_ReturnsNonNullByteArray()
    {
        var result = TiffImagePath.ProcessImage(ImageType.Tiff);
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);
    }

    [Fact]
    public void ProcessJpegImageToBase64String_ReturnsNonNullBase64String()
    {
        var result = JpegImagePath.ProcessImageAsBase64(ImageType.Jpeg);
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void ProcessPngImageToBase64String_ReturnsNonNullBase64String()
    {
        var result = PngImagePath.ProcessImageAsBase64(ImageType.Png);
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void ProcessGifImageToBase64String_ReturnsNonNullBase64String()
    {
        var result = GifImagePath.ProcessImageAsBase64(ImageType.Gif);
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void ProcessBmpImageToBase64String_ReturnsNonNullBase64String()
    {
        var result = BmpImagePath.ProcessImageAsBase64(ImageType.Bmp);
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void ProcessUnsupportedImageFormat_ThrowsException()
    {
        var exception = Assert.Throws<NotSupportedException>(() => UnsupportedImagePath.ProcessImage());
        Assert.Equal("Unsupported image format: .webp", exception.Message);
    }
}

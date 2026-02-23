namespace Mamey.Templates.Abstractions;

internal interface IQrGen
{
    byte[] MakePng(string data, int pixelsPerModule = 8, int border = 1);
}
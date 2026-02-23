namespace Common.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<Stream?> GetFileAsync(string filePath);
    string GetFileUrl(string filePath);
    bool IsValidImage(string fileName);
    bool IsValidDocument(string fileName);
    bool IsValidVideo(string fileName);
}

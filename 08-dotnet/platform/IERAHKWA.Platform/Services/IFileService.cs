namespace IERAHKWA.Platform.Services;

public interface IFileService
{
    Task<string> ReadFileAsync(string path);
    Task WriteFileAsync(string path, string content);
    Task<List<FileNode>> GetFileTreeAsync(string rootPath);
    Task<bool> CreateDirectoryAsync(string path);
    Task<bool> DeleteFileAsync(string path);
}

public class FileNode
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public List<FileNode>? Children { get; set; }
}

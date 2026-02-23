using IERAHKWA.Platform.Services;

namespace IERAHKWA.Platform.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly string _workspacePath;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
        _workspacePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "platform", "workspace");
        
        if (!Directory.Exists(_workspacePath))
        {
            Directory.CreateDirectory(_workspacePath);
        }
    }

    public async Task<string> ReadFileAsync(string path)
    {
        try
        {
            var fullPath = Path.Combine(_workspacePath, path);
            if (File.Exists(fullPath))
            {
                return await File.ReadAllTextAsync(fullPath);
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {Path}", path);
            throw;
        }
    }

    public async Task WriteFileAsync(string path, string content)
    {
        try
        {
            var fullPath = Path.Combine(_workspacePath, path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await File.WriteAllTextAsync(fullPath, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing file: {Path}", path);
            throw;
        }
    }

    public async Task<List<FileNode>> GetFileTreeAsync(string rootPath)
    {
        return await Task.Run(() =>
        {
            var nodes = new List<FileNode>();
            var fullPath = Path.Combine(_workspacePath, rootPath);
            
            if (!Directory.Exists(fullPath))
                return nodes;

            try
            {
                foreach (var item in Directory.GetFileSystemEntries(fullPath))
                {
                    var name = Path.GetFileName(item);
                    var isDirectory = Directory.Exists(item);
                    
                    var node = new FileNode
                    {
                        Name = name,
                        Path = Path.GetRelativePath(_workspacePath, item),
                        IsDirectory = isDirectory
                    };

                    if (isDirectory)
                    {
                        node.Children = GetFileTreeAsync(Path.GetRelativePath(_workspacePath, item)).Result;
                    }

                    nodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building file tree: {Path}", rootPath);
            }

            return nodes;
        });
    }

    public async Task<bool> CreateDirectoryAsync(string path)
    {
        try
        {
            var fullPath = Path.Combine(_workspacePath, path);
            Directory.CreateDirectory(fullPath);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating directory: {Path}", path);
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync(string path)
    {
        try
        {
            var fullPath = Path.Combine(_workspacePath, path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", path);
            return false;
        }
    }
}

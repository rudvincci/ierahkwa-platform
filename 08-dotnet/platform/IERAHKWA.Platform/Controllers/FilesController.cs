using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;
using IERAHKWA.Platform.Services;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileService fileService, ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<FileNode>>>> GetFileTree([FromQuery] string? path = "")
    {
        try
        {
            var tree = await _fileService.GetFileTreeAsync(path ?? "");
            return Ok(new ApiResponse<List<FileNode>> { Success = true, Data = tree });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file tree");
            return StatusCode(500, new ApiResponse<List<FileNode>> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("read")]
    public async Task<ActionResult<ApiResponse<object>>> ReadFile([FromQuery] string path)
    {
        try
        {
            var content = await _fileService.ReadFileAsync(path);
            return Ok(new ApiResponse<object> { Success = true, Data = new { content = content, path = path } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpPost("save")]
    public async Task<ActionResult<ApiResponse<object>>> SaveFile([FromBody] SaveFileRequest request)
    {
        try
        {
            await _fileService.WriteFileAsync(request.Path ?? "", request.Content ?? "");
            return Ok(new ApiResponse<object> { Success = true, Data = new { message = "File saved successfully" } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<object>>> CreateFile([FromBody] CreateFileRequest request)
    {
        try
        {
            await _fileService.WriteFileAsync(request.Path ?? "", request.Content ?? "");
            return Ok(new ApiResponse<object> { Success = true, Data = new { message = "File created successfully" } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating file");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpPost("mkdir")]
    public async Task<ActionResult<ApiResponse<object>>> CreateDirectory([FromBody] CreateDirectoryRequest request)
    {
        try
        {
            var result = await _fileService.CreateDirectoryAsync(request.Path ?? "");
            return Ok(new ApiResponse<object> { Success = result, Data = new { message = result ? "Directory created" : "Failed to create directory" } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating directory");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFile([FromQuery] string path)
    {
        try
        {
            var result = await _fileService.DeleteFileAsync(path);
            return Ok(new ApiResponse<object> { Success = result, Data = new { message = result ? "File deleted" : "File not found" } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }
}

public class SaveFileRequest
{
    public string? Path { get; set; }
    public string? Content { get; set; }
}

public class CreateFileRequest
{
    public string? Path { get; set; }
    public string? Content { get; set; }
}

public class CreateDirectoryRequest
{
    public string? Path { get; set; }
}

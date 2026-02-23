using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;
using IERAHKWA.Platform.Services;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService _platformService;
    private readonly ILogger<PlatformController> _logger;

    public PlatformController(IPlatformService platformService, ILogger<PlatformController> logger)
    {
        _platformService = platformService;
        _logger = logger;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<PlatformOverview>>> GetOverview()
    {
        try
        {
            var overview = await _platformService.GetOverviewAsync();
            return Ok(new ApiResponse<PlatformOverview> { Success = true, Data = overview });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting platform overview");
            return StatusCode(500, new ApiResponse<PlatformOverview> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("services")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, ServiceInfo>>>> GetServices()
    {
        try
        {
            var services = await _platformService.GetServicesStatusAsync();
            return Ok(new ApiResponse<Dictionary<string, ServiceInfo>> { Success = true, Data = services });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services");
            return StatusCode(500, new ApiResponse<Dictionary<string, ServiceInfo>> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("health/{serviceId}")]
    public async Task<ActionResult<ApiResponse<ServiceInfo>>> GetServiceHealth(string serviceId)
    {
        try
        {
            var service = await _platformService.GetServiceHealthAsync(serviceId);
            if (service == null)
            {
                return NotFound(new ApiResponse<ServiceInfo> { Success = false, Error = "Service not found" });
            }
            return Ok(new ApiResponse<ServiceInfo> { Success = true, Data = service });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service health");
            return StatusCode(500, new ApiResponse<ServiceInfo> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("modules")]
    public async Task<ActionResult<ApiResponse<object>>> GetModules()
    {
        try
        {
            var services = await _platformService.GetAllServicesAsync();
            var departments = await _platformService.GetAllDepartmentsAsync();
            
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new
                {
                    services = services,
                    departments = departments,
                    all = services.Cast<object>().Concat(departments.Cast<object>()).ToList(),
                    total = services.Count + departments.Count
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting modules");
            return StatusCode(500, new ApiResponse<object> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("departments")]
    public async Task<ActionResult<ApiResponse<List<DepartmentInfo>>>> GetDepartments()
    {
        try
        {
            var departments = await _platformService.GetAllDepartmentsAsync();
            return Ok(new ApiResponse<List<DepartmentInfo>> { Success = true, Data = departments });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return StatusCode(500, new ApiResponse<List<DepartmentInfo>> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("tokens")]
    public async Task<ActionResult<ApiResponse<List<TokenInfo>>>> GetTokens()
    {
        try
        {
            var tokens = await _platformService.GetAllTokensAsync();
            return Ok(new ApiResponse<List<TokenInfo>> { Success = true, Data = tokens });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tokens");
            return StatusCode(500, new ApiResponse<List<TokenInfo>> { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("config")]
    public async Task<ActionResult<ApiResponse<PlatformConfig>>> GetConfig()
    {
        try
        {
            var config = await _platformService.GetConfigAsync();
            return Ok(new ApiResponse<PlatformConfig> { Success = true, Data = config });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting config");
            return StatusCode(500, new ApiResponse<PlatformConfig> { Success = false, Error = ex.Message });
        }
    }
}

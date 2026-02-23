using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<List<MemberInfo>>> GetMembers()
    {
        var members = new List<MemberInfo>
        {
            new() { Name = "Admin User", Email = "admin@ierahkwa.gov", Role = "admin", Status = "Active", LastActive = DateTime.UtcNow.AddMinutes(-2), Avatar = "AD" },
            new() { Name = "Developer", Email = "dev@ierahkwa.gov", Role = "developer", Status = "Active", LastActive = DateTime.UtcNow.AddHours(-1), Avatar = "DV" }
        };

        return Ok(new ApiResponse<List<MemberInfo>> { Success = true, Data = members });
    }
}

public class MemberInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastActive { get; set; }
    public string Avatar { get; set; } = string.Empty;
}

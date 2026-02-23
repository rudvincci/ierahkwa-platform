using Microsoft.AspNetCore.Mvc;
using NET10.Core.Architecture;
using NET10.Core.ERP;

namespace NET10.API.Controllers;

// ═══════════════════════════════════════════════════════════════════════════════
// WEB ERP API CONTROLLER - PRESENTATION LAYER (LAYER 1)
// Three-Tier Architecture Implementation
// ═══════════════════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/web-erp")]
public class WebERPController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly IERPUserService _userService;
    private readonly ISalesService _salesService;
    
    public WebERPController(
        IOrganizationService organizationService,
        IERPUserService userService,
        ISalesService salesService)
    {
        _organizationService = organizationService;
        _userService = userService;
        _salesService = salesService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // AUTHENTICATION
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("auth/login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] NET10.Core.ERP.LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("auth/refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _userService.RefreshTokenAsync(request.RefreshToken);
        return result.Success ? Ok(result.Data) : Unauthorized(result);
    }
    
    /// <summary>
    /// User logout
    /// </summary>
    [HttpPost("auth/logout/{userId}")]
    public async Task<ActionResult> Logout(Guid userId)
    {
        var result = await _userService.LogoutAsync(userId);
        return result.Success ? Ok() : BadRequest(result);
    }
    
    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("auth/change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpPost("auth/forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _userService.ForgotPasswordAsync(request.Email);
        return Ok(result);
    }
    
    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("auth/reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _userService.ResetPasswordAsync(request.Token, request.NewPassword);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ORGANIZATIONS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("organizations/{id}")]
    public async Task<ActionResult<Organization>> GetOrganization(Guid id)
    {
        var result = await _organizationService.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpPost("organizations")]
    public async Task<ActionResult<Organization>> CreateOrganization([FromBody] Organization organization)
    {
        var result = await _organizationService.CreateAsync(organization);
        return result.Success ? CreatedAtAction(nameof(GetOrganization), new { id = result.Data!.Id }, result.Data) : BadRequest(result);
    }
    
    [HttpPut("organizations/{id}")]
    public async Task<ActionResult<Organization>> UpdateOrganization(Guid id, [FromBody] Organization organization)
    {
        organization.Id = id;
        var result = await _organizationService.UpdateAsync(organization);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("organizations/{id}")]
    public async Task<ActionResult> DeleteOrganization(Guid id)
    {
        var result = await _organizationService.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(result);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // USERS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("users/{id}")]
    public async Task<ActionResult<ERPUser>> GetUser(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet("organizations/{orgId}/users")]
    public async Task<ActionResult<List<ERPUser>>> GetUsersByOrganization(Guid orgId)
    {
        var result = await _userService.GetByOrganizationAsync(orgId);
        return Ok(result.Data);
    }
    
    [HttpPost("users")]
    public async Task<ActionResult<ERPUser>> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result.Data) : BadRequest(result);
    }
    
    [HttpPut("users/{id}")]
    public async Task<ActionResult<ERPUser>> UpdateUser(Guid id, [FromBody] ERPUser user)
    {
        user.Id = id;
        var result = await _userService.UpdateAsync(user);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("users/{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result.Success ? NoContent() : NotFound(result);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // SALES ORDERS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("organizations/{orgId}/sales-orders")]
    public async Task<ActionResult<PagedResult<SalesOrder>>> GetSalesOrders(Guid orgId, [FromQuery] PaginationParams pagination)
    {
        var result = await _salesService.GetOrdersAsync(orgId, pagination);
        return Ok(result.Data);
    }
    
    [HttpGet("sales-orders/{id}")]
    public async Task<ActionResult<SalesOrder>> GetSalesOrder(Guid id)
    {
        var result = await _salesService.GetOrderByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet("sales-orders/number/{number}")]
    public async Task<ActionResult<SalesOrder>> GetSalesOrderByNumber(string number)
    {
        var result = await _salesService.GetOrderByNumberAsync(number);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpPost("sales-orders")]
    public async Task<ActionResult<SalesOrder>> CreateSalesOrder([FromBody] SalesOrder order)
    {
        var result = await _salesService.CreateOrderAsync(order);
        return result.Success ? CreatedAtAction(nameof(GetSalesOrder), new { id = result.Data!.Id }, result.Data) : BadRequest(result);
    }
    
    [HttpPut("sales-orders/{id}")]
    public async Task<ActionResult<SalesOrder>> UpdateSalesOrder(Guid id, [FromBody] SalesOrder order)
    {
        order.Id = id;
        var result = await _salesService.UpdateOrderAsync(order);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("sales-orders/{id}/submit")]
    public async Task<ActionResult<SalesOrder>> SubmitSalesOrder(Guid id)
    {
        var result = await _salesService.SubmitOrderAsync(id);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("sales-orders/{id}/approve")]
    public async Task<ActionResult<SalesOrder>> ApproveSalesOrder(Guid id, [FromQuery] Guid approvedById)
    {
        var result = await _salesService.ApproveOrderAsync(id, approvedById);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("sales-orders/{id}/ship")]
    public async Task<ActionResult<SalesOrder>> ShipSalesOrder(Guid id, [FromQuery] string trackingNumber)
    {
        var result = await _salesService.ShipOrderAsync(id, trackingNumber);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("sales-orders/{id}/cancel")]
    public async Task<ActionResult> CancelSalesOrder(Guid id, [FromQuery] string reason)
    {
        var result = await _salesService.CancelOrderAsync(id, reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet("customers/{customerId}/sales-orders")]
    public async Task<ActionResult<List<SalesOrder>>> GetOrdersByCustomer(Guid customerId)
    {
        var result = await _salesService.GetOrdersByCustomerAsync(customerId);
        return Ok(result.Data);
    }
    
    [HttpGet("organizations/{orgId}/sales-dashboard")]
    public async Task<ActionResult<SalesDashboard>> GetSalesDashboard(Guid orgId)
    {
        var result = await _salesService.GetDashboardAsync(orgId);
        return Ok(result.Data);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// REQUEST DTOs
// ═══════════════════════════════════════════════════════════════════════════════

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

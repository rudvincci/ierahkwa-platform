using System.Security.Cryptography;
using System.Text;
using NET10.Core.Architecture;
using NET10.Core.ERP;

namespace NET10.Infrastructure.Services.WebERP;

// ═══════════════════════════════════════════════════════════════════════════════
// BUSINESS LOGIC LAYER (BLL) - LAYER 2
// Three-Tier Architecture Implementation
// Contains business rules, validation, and orchestration
// ═══════════════════════════════════════════════════════════════════════════════

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;
    
    public OrganizationService(IOrganizationRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<Organization>> GetByIdAsync(Guid id)
    {
        var org = await _repository.GetByIdAsync(id);
        return org != null 
            ? Result<Organization>.Ok(org) 
            : Result<Organization>.Fail("Organization not found", 404);
    }
    
    public async Task<Result<Organization>> CreateAsync(Organization org)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(org.Name))
            return Result<Organization>.Fail("Organization name is required");
        
        if (string.IsNullOrWhiteSpace(org.Code))
            return Result<Organization>.Fail("Organization code is required");
        
        // Check for duplicates
        var existing = await _repository.GetByCodeAsync(org.Code);
        if (existing != null)
            return Result<Organization>.Fail("Organization code already exists");
        
        var created = await _repository.AddAsync(org);
        return Result<Organization>.Ok(created, "Organization created successfully");
    }
    
    public async Task<Result<Organization>> UpdateAsync(Organization org)
    {
        var existing = await _repository.GetByIdAsync(org.Id);
        if (existing == null)
            return Result<Organization>.Fail("Organization not found", 404);
        
        var updated = await _repository.UpdateAsync(org);
        return Result<Organization>.Ok(updated, "Organization updated successfully");
    }
    
    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        return result 
            ? Result<bool>.Ok(true, "Organization deleted") 
            : Result<bool>.Fail("Organization not found", 404);
    }
    
    public async Task<Result<Organization>> GetByCodeAsync(string code)
    {
        var org = await _repository.GetByCodeAsync(code);
        return org != null 
            ? Result<Organization>.Ok(org) 
            : Result<Organization>.Fail("Organization not found", 404);
    }
}

public class ERPUserService : IERPUserService
{
    private readonly IERPUserRepository _userRepository;
    private readonly IOrganizationRepository _orgRepository;
    
    public ERPUserService(IERPUserRepository userRepository, IOrganizationRepository orgRepository)
    {
        _userRepository = userRepository;
        _orgRepository = orgRepository;
    }
    
    public async Task<Result<ERPUser>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null 
            ? Result<ERPUser>.Ok(user) 
            : Result<ERPUser>.Fail("User not found", 404);
    }
    
    public async Task<Result<ERPUser>> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null 
            ? Result<ERPUser>.Ok(user) 
            : Result<ERPUser>.Fail("User not found", 404);
    }
    
    public async Task<Result<ERPUser>> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null 
            ? Result<ERPUser>.Ok(user) 
            : Result<ERPUser>.Fail("User not found", 404);
    }
    
    public async Task<Result<List<ERPUser>>> GetByOrganizationAsync(Guid organizationId)
    {
        var users = await _userRepository.GetByOrganizationAsync(organizationId);
        return Result<List<ERPUser>>.Ok(users);
    }
    
    public async Task<Result<ERPUser>> CreateAsync(CreateUserRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Username))
            return Result<ERPUser>.Fail("Username is required");
        
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<ERPUser>.Fail("Email is required");
        
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return Result<ERPUser>.Fail("Password must be at least 8 characters");
        
        // Check for duplicates
        var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUsername != null)
            return Result<ERPUser>.Fail("Username already exists");
        
        var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (existingEmail != null)
            return Result<ERPUser>.Fail("Email already registered");
        
        // Hash password
        var salt = GenerateSalt();
        var passwordHash = HashPassword(request.Password, salt);
        
        var user = new ERPUser
        {
            OrganizationId = request.OrganizationId,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Salt = salt,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleId = request.RoleId,
            IsActive = true
        };
        
        var created = await _userRepository.AddAsync(user);
        return Result<ERPUser>.Ok(created, "User created successfully");
    }
    
    public async Task<Result<ERPUser>> UpdateAsync(ERPUser user)
    {
        var existing = await _userRepository.GetByIdAsync(user.Id);
        if (existing == null)
            return Result<ERPUser>.Fail("User not found", 404);
        
        var updated = await _userRepository.UpdateAsync(user);
        return Result<ERPUser>.Ok(updated, "User updated successfully");
    }
    
    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var result = await _userRepository.DeleteAsync(id);
        return result 
            ? Result<bool>.Ok(true, "User deleted") 
            : Result<bool>.Fail("User not found", 404);
    }
    
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
            return Result<AuthResponse>.Fail("Invalid username or password");
        
        // Check lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            return Result<AuthResponse>.Fail("Account is locked. Please try again later.");
        
        // Verify password
        var passwordHash = HashPassword(request.Password, user.Salt);
        if (passwordHash != user.PasswordHash)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            }
            await _userRepository.UpdateAsync(user);
            return Result<AuthResponse>.Fail("Invalid username or password");
        }
        
        // Check 2FA
        if (user.TwoFactorEnabled && string.IsNullOrEmpty(request.TwoFactorCode))
        {
            return Result<AuthResponse>.Ok(new AuthResponse { RequiresTwoFactor = true });
        }
        
        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt = DateTime.UtcNow;
        user.FailedLoginAttempts = 0;
        await _userRepository.UpdateAsync(user);
        
        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiry = DateTime.UtcNow.AddHours(1),
            User = user
        });
    }
    
    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<AuthResponse>.Fail("Invalid or expired refresh token");
        
        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);
        
        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Expiry = DateTime.UtcNow.AddHours(1),
            User = user
        });
    }
    
    public async Task<Result<bool>> LogoutAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<bool>.Fail("User not found", 404);
        
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userRepository.UpdateAsync(user);
        
        return Result<bool>.Ok(true, "Logged out successfully");
    }
    
    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<bool>.Fail("User not found", 404);
        
        var currentHash = HashPassword(currentPassword, user.Salt);
        if (currentHash != user.PasswordHash)
            return Result<bool>.Fail("Current password is incorrect");
        
        if (newPassword.Length < 8)
            return Result<bool>.Fail("New password must be at least 8 characters");
        
        var newSalt = GenerateSalt();
        user.Salt = newSalt;
        user.PasswordHash = HashPassword(newPassword, newSalt);
        user.MustChangePassword = false;
        await _userRepository.UpdateAsync(user);
        
        return Result<bool>.Ok(true, "Password changed successfully");
    }
    
    public async Task<Result<string>> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return Result<string>.Ok("If email exists, reset link has been sent"); // Security: don't reveal if email exists
        
        user.PasswordResetToken = GenerateRefreshToken();
        user.PasswordResetExpiry = DateTime.UtcNow.AddHours(24);
        await _userRepository.UpdateAsync(user);
        
        // TODO: Send email with reset link
        return Result<string>.Ok("Password reset link sent to email");
    }
    
    public async Task<Result<bool>> ResetPasswordAsync(string token, string newPassword)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.PasswordResetToken == token);
        
        if (user == null || user.PasswordResetExpiry < DateTime.UtcNow)
            return Result<bool>.Fail("Invalid or expired reset token");
        
        if (newPassword.Length < 8)
            return Result<bool>.Fail("Password must be at least 8 characters");
        
        var newSalt = GenerateSalt();
        user.Salt = newSalt;
        user.PasswordHash = HashPassword(newPassword, newSalt);
        user.PasswordResetToken = null;
        user.PasswordResetExpiry = null;
        await _userRepository.UpdateAsync(user);
        
        return Result<bool>.Ok(true, "Password reset successfully");
    }
    
    public Task<Result<bool>> Enable2FAAsync(Guid userId)
    {
        // TODO: Implement 2FA setup
        return Task.FromResult(Result<bool>.Ok(true));
    }
    
    public Task<Result<bool>> Verify2FAAsync(Guid userId, string code)
    {
        // TODO: Implement 2FA verification
        return Task.FromResult(Result<bool>.Ok(true));
    }
    
    // Helper methods
    private static string GenerateSalt()
    {
        var buffer = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        return Convert.ToBase64String(buffer);
    }
    
    private static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }
    
    private static string GenerateAccessToken(ERPUser user)
    {
        // In production, use JWT
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "." + 
               Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Id}|{user.Username}|{DateTime.UtcNow.AddHours(1):O}"));
    }
    
    private static string GenerateRefreshToken()
    {
        var buffer = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        return Convert.ToBase64String(buffer);
    }
}

public class SalesService : ISalesService
{
    private readonly ISalesOrderRepository _repository;
    private static int _orderCounter = 10000;
    
    public SalesService(ISalesOrderRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<PagedResult<SalesOrder>>> GetOrdersAsync(Guid organizationId, PaginationParams pagination)
    {
        var orders = await _repository.GetByOrganizationAsync(organizationId);
        
        // Apply search
        if (!string.IsNullOrEmpty(pagination.SearchTerm))
        {
            orders = orders.Where(o => 
                o.OrderNumber.Contains(pagination.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                o.CustomerName.Contains(pagination.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        // Apply sorting
        orders = pagination.SortDescending
            ? orders.OrderByDescending(o => o.OrderDate).ToList()
            : orders.OrderBy(o => o.OrderDate).ToList();
        
        var totalCount = orders.Count;
        var pagedOrders = orders
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();
        
        return Result<PagedResult<SalesOrder>>.Ok(new PagedResult<SalesOrder>
        {
            Items = pagedOrders,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        });
    }
    
    public async Task<Result<SalesOrder>> GetOrderByIdAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order != null 
            ? Result<SalesOrder>.Ok(order) 
            : Result<SalesOrder>.Fail("Order not found", 404);
    }
    
    public async Task<Result<SalesOrder>> GetOrderByNumberAsync(string orderNumber)
    {
        var order = await _repository.GetByNumberAsync(orderNumber);
        return order != null 
            ? Result<SalesOrder>.Ok(order) 
            : Result<SalesOrder>.Fail("Order not found", 404);
    }
    
    public async Task<Result<SalesOrder>> CreateOrderAsync(SalesOrder order)
    {
        // Validation
        if (order.Items.Count == 0)
            return Result<SalesOrder>.Fail("Order must have at least one item");
        
        order.OrderNumber = await GenerateOrderNumberAsync(order.OrganizationId).ContinueWith(t => t.Result.Data!);
        order.Status = SalesOrderStatus.Draft;
        
        var created = await _repository.AddAsync(order);
        return Result<SalesOrder>.Ok(created, "Order created successfully");
    }
    
    public async Task<Result<SalesOrder>> UpdateOrderAsync(SalesOrder order)
    {
        var existing = await _repository.GetByIdAsync(order.Id);
        if (existing == null)
            return Result<SalesOrder>.Fail("Order not found", 404);
        
        if (existing.Status != SalesOrderStatus.Draft)
            return Result<SalesOrder>.Fail("Cannot modify order that is not in draft status");
        
        var updated = await _repository.UpdateAsync(order);
        return Result<SalesOrder>.Ok(updated, "Order updated successfully");
    }
    
    public async Task<Result<bool>> CancelOrderAsync(Guid id, string reason)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null)
            return Result<bool>.Fail("Order not found", 404);
        
        if (order.Status == SalesOrderStatus.Shipped || order.Status == SalesOrderStatus.Delivered)
            return Result<bool>.Fail("Cannot cancel shipped or delivered order");
        
        order.Status = SalesOrderStatus.Cancelled;
        await _repository.UpdateAsync(order);
        
        return Result<bool>.Ok(true, "Order cancelled");
    }
    
    public async Task<Result<SalesOrder>> SubmitOrderAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null)
            return Result<SalesOrder>.Fail("Order not found", 404);
        
        if (order.Status != SalesOrderStatus.Draft)
            return Result<SalesOrder>.Fail("Only draft orders can be submitted");
        
        order.Status = SalesOrderStatus.Submitted;
        await _repository.UpdateAsync(order);
        
        return Result<SalesOrder>.Ok(order, "Order submitted for approval");
    }
    
    public async Task<Result<SalesOrder>> ApproveOrderAsync(Guid id, Guid approvedById)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null)
            return Result<SalesOrder>.Fail("Order not found", 404);
        
        if (order.Status != SalesOrderStatus.Submitted)
            return Result<SalesOrder>.Fail("Only submitted orders can be approved");
        
        order.Status = SalesOrderStatus.Approved;
        await _repository.UpdateAsync(order);
        
        return Result<SalesOrder>.Ok(order, "Order approved");
    }
    
    public async Task<Result<SalesOrder>> ShipOrderAsync(Guid id, string trackingNumber)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null)
            return Result<SalesOrder>.Fail("Order not found", 404);
        
        order.Status = SalesOrderStatus.Shipped;
        order.TrackingNumber = trackingNumber;
        order.ShippedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(order);
        
        return Result<SalesOrder>.Ok(order, "Order shipped");
    }
    
    public Task<Result<string>> GenerateOrderNumberAsync(Guid organizationId)
    {
        _orderCounter++;
        return Task.FromResult(Result<string>.Ok($"SO-{_orderCounter:D6}"));
    }
    
    public async Task<Result<List<SalesOrder>>> GetOrdersByCustomerAsync(Guid customerId)
    {
        var orders = await _repository.GetByCustomerAsync(customerId);
        return Result<List<SalesOrder>>.Ok(orders);
    }
    
    public async Task<Result<SalesDashboard>> GetDashboardAsync(Guid organizationId)
    {
        var orders = await _repository.GetByOrganizationAsync(organizationId);
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var yearStart = new DateTime(today.Year, 1, 1);
        
        var dashboard = new SalesDashboard
        {
            TodaySales = orders.Where(o => o.OrderDate.Date == today).Sum(o => o.Total),
            WeekSales = orders.Where(o => o.OrderDate >= weekStart).Sum(o => o.Total),
            MonthSales = orders.Where(o => o.OrderDate >= monthStart).Sum(o => o.Total),
            YearSales = orders.Where(o => o.OrderDate >= yearStart).Sum(o => o.Total),
            TotalOrders = orders.Count,
            PendingOrders = orders.Count(o => o.Status == SalesOrderStatus.Submitted || o.Status == SalesOrderStatus.Approved),
            ShippedOrders = orders.Count(o => o.Status == SalesOrderStatus.Shipped)
        };
        
        return Result<SalesDashboard>.Ok(dashboard);
    }
}

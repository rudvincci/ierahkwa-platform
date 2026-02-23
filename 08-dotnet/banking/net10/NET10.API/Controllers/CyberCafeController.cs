using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Cyber Cafe Controller - Ierahkwa Internet Cafe Time Management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CyberCafeController : ControllerBase
{
    private readonly ICyberCafeSessionService _sessionService;
    private readonly ICafeStationService _stationService;
    private readonly ICafeCustomerService _customerService;
    private readonly ICafePricingService _pricingService;
    private readonly ICafeReportService _reportService;
    
    public CyberCafeController(
        ICyberCafeSessionService sessionService,
        ICafeStationService stationService,
        ICafeCustomerService customerService,
        ICafePricingService pricingService,
        ICafeReportService reportService)
    {
        _sessionService = sessionService;
        _stationService = stationService;
        _customerService = customerService;
        _pricingService = pricingService;
        _reportService = reportService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // OVERVIEW
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get cafe overview
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CafeOverview>> GetOverview()
    {
        var stations = await _stationService.GetAllAsync();
        var activeSessions = await _sessionService.GetActiveSessionsAsync();
        var todaySessions = await _sessionService.GetTodaySessionsAsync();
        
        return Ok(new CafeOverview
        {
            TotalStations = stations.Count,
            AvailableStations = stations.Count(s => s.Status == StationStatus.Available),
            InUseStations = stations.Count(s => s.Status == StationStatus.InUse),
            ActiveSessions = activeSessions.Count,
            TodaySessions = todaySessions.Count,
            TodayRevenue = todaySessions.Sum(s => s.TotalCost),
            Stations = stations.Select(s => new StationInfo
            {
                Id = s.Id,
                Number = s.Number,
                Name = s.Name,
                Type = s.Type.ToString(),
                Status = s.Status.ToString(),
                CurrentSessionId = s.CurrentSessionId
            }).ToList()
        });
    }
    
    // ═══════════════════════════════════════════════════════════════
    // SESSIONS (TIMER)
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get active sessions
    /// </summary>
    [HttpGet("sessions/active")]
    public async Task<ActionResult<List<CafeSession>>> GetActiveSessions()
    {
        var sessions = await _sessionService.GetActiveSessionsAsync();
        return Ok(sessions);
    }
    
    /// <summary>
    /// Get today's sessions
    /// </summary>
    [HttpGet("sessions/today")]
    public async Task<ActionResult<List<CafeSession>>> GetTodaySessions()
    {
        var sessions = await _sessionService.GetTodaySessionsAsync();
        return Ok(sessions);
    }
    
    /// <summary>
    /// Get session by ID
    /// </summary>
    [HttpGet("sessions/{id}")]
    public async Task<ActionResult<CafeSession>> GetSession(Guid id)
    {
        var session = await _sessionService.GetSessionAsync(id);
        if (session == null) return NotFound();
        return Ok(session);
    }
    
    /// <summary>
    /// Start session (Login Customer)
    /// </summary>
    [HttpPost("sessions/start")]
    public async Task<ActionResult<CafeSession>> StartSession([FromBody] StartSessionRequest request)
    {
        try
        {
            var session = await _sessionService.StartSessionAsync(request);
            return Ok(session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// End session (Logout Customer)
    /// </summary>
    [HttpPost("sessions/{id}/end")]
    public async Task<ActionResult<CafeSession>> EndSession(Guid id)
    {
        try
        {
            var session = await _sessionService.EndSessionAsync(id);
            return Ok(session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Pause session
    /// </summary>
    [HttpPost("sessions/{id}/pause")]
    public async Task<ActionResult<CafeSession>> PauseSession(Guid id)
    {
        try
        {
            var session = await _sessionService.PauseSessionAsync(id);
            return Ok(session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Resume session
    /// </summary>
    [HttpPost("sessions/{id}/resume")]
    public async Task<ActionResult<CafeSession>> ResumeSession(Guid id)
    {
        try
        {
            var session = await _sessionService.ResumeSessionAsync(id);
            return Ok(session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get current cost
    /// </summary>
    [HttpGet("sessions/{id}/cost")]
    public async Task<ActionResult<SessionCost>> GetSessionCost(Guid id)
    {
        try
        {
            var cost = await _sessionService.GetCurrentCostAsync(id);
            return Ok(cost);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // STATIONS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all stations
    /// </summary>
    [HttpGet("stations")]
    public async Task<ActionResult<List<CafeStation>>> GetStations()
    {
        var stations = await _stationService.GetAllAsync();
        return Ok(stations);
    }
    
    /// <summary>
    /// Get available stations
    /// </summary>
    [HttpGet("stations/available")]
    public async Task<ActionResult<List<CafeStation>>> GetAvailableStations()
    {
        var stations = await _stationService.GetAvailableAsync();
        return Ok(stations);
    }
    
    /// <summary>
    /// Get station by ID
    /// </summary>
    [HttpGet("stations/{id}")]
    public async Task<ActionResult<CafeStation>> GetStation(Guid id)
    {
        var station = await _stationService.GetByIdAsync(id);
        if (station == null) return NotFound();
        return Ok(station);
    }
    
    /// <summary>
    /// Get stations by type
    /// </summary>
    [HttpGet("stations/type/{type}")]
    public async Task<ActionResult<List<CafeStation>>> GetStationsByType(StationType type)
    {
        var stations = await _stationService.GetByTypeAsync(type);
        return Ok(stations);
    }
    
    /// <summary>
    /// Create station
    /// </summary>
    [HttpPost("stations")]
    public async Task<ActionResult<CafeStation>> CreateStation([FromBody] CafeStation station)
    {
        var created = await _stationService.CreateAsync(station);
        return CreatedAtAction(nameof(GetStation), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update station
    /// </summary>
    [HttpPut("stations/{id}")]
    public async Task<ActionResult<CafeStation>> UpdateStation(Guid id, [FromBody] CafeStation station)
    {
        station.Id = id;
        var updated = await _stationService.UpdateAsync(station);
        return Ok(updated);
    }
    
    /// <summary>
    /// Set station status
    /// </summary>
    [HttpPost("stations/{id}/status")]
    public async Task<ActionResult> SetStationStatus(Guid id, [FromQuery] StationStatus status)
    {
        var result = await _stationService.SetStatusAsync(id, status);
        if (!result) return NotFound();
        return Ok(new { id, status = status.ToString() });
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CUSTOMERS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet("customers")]
    public async Task<ActionResult<List<CafeCustomer>>> GetCustomers()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }
    
    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("customers/{id}")]
    public async Task<ActionResult<CafeCustomer>> GetCustomer(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }
    
    /// <summary>
    /// Get customer by card
    /// </summary>
    [HttpGet("customers/card/{cardNumber}")]
    public async Task<ActionResult<CafeCustomer>> GetCustomerByCard(string cardNumber)
    {
        var customer = await _customerService.GetByCardNumberAsync(cardNumber);
        if (customer == null) return NotFound();
        return Ok(customer);
    }
    
    /// <summary>
    /// Get members
    /// </summary>
    [HttpGet("customers/members")]
    public async Task<ActionResult<List<CafeCustomer>>> GetMembers()
    {
        var members = await _customerService.GetMembersAsync();
        return Ok(members);
    }
    
    /// <summary>
    /// Register customer
    /// </summary>
    [HttpPost("customers")]
    public async Task<ActionResult<CafeCustomer>> RegisterCustomer([FromBody] CafeCustomer customer)
    {
        var created = await _customerService.RegisterAsync(customer);
        return CreatedAtAction(nameof(GetCustomer), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Add credit
    /// </summary>
    [HttpPost("customers/{id}/credit")]
    public async Task<ActionResult<CafeCustomer>> AddCredit(Guid id, [FromBody] AddCreditRequest request)
    {
        var customer = await _customerService.AddCreditAsync(id, request.Amount);
        return Ok(customer);
    }
    
    /// <summary>
    /// Get customer history
    /// </summary>
    [HttpGet("customers/{id}/history")]
    public async Task<ActionResult<List<CafeSession>>> GetCustomerHistory(Guid id)
    {
        var history = await _sessionService.GetCustomerHistoryAsync(id);
        return Ok(history);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PRICING
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get pricing plans
    /// </summary>
    [HttpGet("pricing")]
    public async Task<ActionResult<List<PricingPlan>>> GetPricingPlans()
    {
        var plans = await _pricingService.GetAllPlansAsync();
        return Ok(plans);
    }
    
    /// <summary>
    /// Get packages
    /// </summary>
    [HttpGet("packages")]
    public async Task<ActionResult<List<TimePackage>>> GetPackages()
    {
        var packages = await _pricingService.GetPackagesAsync();
        return Ok(packages);
    }
    
    /// <summary>
    /// Calculate cost
    /// </summary>
    [HttpGet("pricing/calculate")]
    public async Task<ActionResult<decimal>> CalculateCost([FromQuery] StationType type, [FromQuery] int minutes, [FromQuery] bool isMember = false)
    {
        var cost = await _pricingService.CalculateCostAsync(type, TimeSpan.FromMinutes(minutes), isMember);
        return Ok(new { type, minutes, isMember, cost });
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get daily report
    /// </summary>
    [HttpGet("reports/daily")]
    public async Task<ActionResult<DailyReport>> GetDailyReport([FromQuery] DateTime? date = null)
    {
        var report = await _reportService.GetDailyReportAsync(date ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get revenue report
    /// </summary>
    [HttpGet("reports/revenue")]
    public async Task<ActionResult<CafeRevenueReport>> GetRevenueReport([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetRevenueReportAsync(from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get station usage report
    /// </summary>
    [HttpGet("reports/stations")]
    public async Task<ActionResult<StationUsageReport>> GetStationUsage([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetStationUsageAsync(from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get customer report
    /// </summary>
    [HttpGet("reports/customers")]
    public async Task<ActionResult<CustomerReport>> GetCustomerReport([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetCustomerReportAsync(from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get peak hours report
    /// </summary>
    [HttpGet("reports/peak-hours")]
    public async Task<ActionResult<PeakHoursReport>> GetPeakHours([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetPeakHoursAsync(from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
}

// ═══════════════════════════════════════════════════════════════
// VIEW MODELS
// ═══════════════════════════════════════════════════════════════

public class CafeOverview
{
    public int TotalStations { get; set; }
    public int AvailableStations { get; set; }
    public int InUseStations { get; set; }
    public int ActiveSessions { get; set; }
    public int TodaySessions { get; set; }
    public decimal TodayRevenue { get; set; }
    public List<StationInfo> Stations { get; set; } = new();
}

public class StationInfo
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CurrentSessionId { get; set; }
}

public class AddCreditRequest
{
    public decimal Amount { get; set; }
}

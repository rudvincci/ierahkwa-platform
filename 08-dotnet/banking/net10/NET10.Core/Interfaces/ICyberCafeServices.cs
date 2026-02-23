namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA CYBER CAFE MANAGEMENT SYSTEM
// Complete Time-Based Billing & Station Management
// ═══════════════════════════════════════════════════════════════════════════════

public interface ICafeStationService
{
    Task<List<CafeStation>> GetAllAsync();
    Task<CafeStation?> GetByIdAsync(Guid id);
    Task<List<CafeStation>> GetAvailableAsync();
    Task<List<CafeStation>> GetByTypeAsync(StationType type);
    Task<CafeStation> CreateAsync(CafeStation station);
    Task<CafeStation> UpdateAsync(CafeStation station);
    Task<bool> SetStatusAsync(Guid id, StationStatus status);
    Task<bool> DeleteAsync(Guid id);
}

public interface ICafeCustomerService
{
    Task<List<CafeCustomer>> GetAllAsync();
    Task<CafeCustomer?> GetByIdAsync(Guid id);
    Task<CafeCustomer?> GetByCardNumberAsync(string cardNumber);
    Task<CafeCustomer?> GetByPhoneAsync(string phone);
    Task<CafeCustomer> RegisterAsync(CafeCustomer customer);
    Task<CafeCustomer> UpdateAsync(CafeCustomer customer);
    Task<decimal> GetBalanceAsync(Guid customerId);
    Task<CafeCustomer> AddCreditAsync(Guid customerId, decimal amount);
    Task<List<CafeCustomer>> GetMembersAsync();
}

public interface ICafePricingService
{
    Task<List<PricingPlan>> GetAllPlansAsync();
    Task<PricingPlan?> GetPlanAsync(Guid id);
    Task<PricingPlan?> GetPlanByTypeAsync(StationType stationType);
    Task<PricingPlan> CreatePlanAsync(PricingPlan plan);
    Task<PricingPlan> UpdatePlanAsync(PricingPlan plan);
    Task<decimal> CalculateCostAsync(StationType type, TimeSpan duration, bool isMember);
    Task<List<TimePackage>> GetPackagesAsync();
    Task<TimePackage> CreatePackageAsync(TimePackage package);
}

public interface ICyberCafeSessionService
{
    Task<CafeSession> StartSessionAsync(StartSessionRequest request);
    Task<CafeSession> EndSessionAsync(Guid sessionId);
    Task<CafeSession?> GetSessionAsync(Guid id);
    Task<CafeSession?> GetActiveSessionAsync(Guid stationId);
    Task<List<CafeSession>> GetActiveSessionsAsync();
    Task<List<CafeSession>> GetCustomerHistoryAsync(Guid customerId);
    Task<List<CafeSession>> GetTodaySessionsAsync();
    Task<CafeSession> ExtendSessionAsync(Guid sessionId, TimeSpan additionalTime);
    Task<CafeSession> PauseSessionAsync(Guid sessionId);
    Task<CafeSession> ResumeSessionAsync(Guid sessionId);
    Task<SessionCost> GetCurrentCostAsync(Guid sessionId);
}

public interface ICafeReportService
{
    Task<DailyReport> GetDailyReportAsync(DateTime date);
    Task<CafeRevenueReport> GetRevenueReportAsync(DateTime from, DateTime to);
    Task<StationUsageReport> GetStationUsageAsync(DateTime from, DateTime to);
    Task<CustomerReport> GetCustomerReportAsync(DateTime from, DateTime to);
    Task<PeakHoursReport> GetPeakHoursAsync(DateTime from, DateTime to);
}

// ═══════════════════════════════════════════════════════════════════════════════
// CYBER CAFE MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class CafeStation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public StationType Type { get; set; } = StationType.Standard;
    public StationStatus Status { get; set; } = StationStatus.Available;
    public string? IPAddress { get; set; }
    public string? MACAddress { get; set; }
    public string? Specs { get; set; }
    public string Location { get; set; } = string.Empty;
    public Guid? CurrentSessionId { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum StationType
{
    Standard,
    Gaming,
    VIP,
    Printing,
    Private
}

public enum StationStatus
{
    Available,
    InUse,
    Reserved,
    Maintenance,
    OutOfOrder
}

public class CafeCustomer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CardNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? IdNumber { get; set; }
    public CustomerType Type { get; set; } = CustomerType.WalkIn;
    public decimal CreditBalance { get; set; }
    public decimal TotalSpent { get; set; }
    public TimeSpan TotalTimeUsed { get; set; }
    public int VisitCount { get; set; }
    public DateTime? MembershipExpiry { get; set; }
    public bool IsMember => Type == CustomerType.Member && MembershipExpiry > DateTime.UtcNow;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastVisit { get; set; }
}

public enum CustomerType
{
    WalkIn,
    Registered,
    Member,
    Student,
    VIP
}

public class CafeSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StationId { get; set; }
    public string StationNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public SessionType Type { get; set; } = SessionType.TimeBased;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? PausedAt { get; set; }
    public TimeSpan TotalPausedTime { get; set; }
    public TimeSpan? PrepaidTime { get; set; }
    public TimeSpan ActiveDuration => CalculateActiveDuration();
    public decimal HourlyRate { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance => TotalCost - AmountPaid;
    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public string? Notes { get; set; }
    
    private TimeSpan CalculateActiveDuration()
    {
        var end = EndTime ?? DateTime.UtcNow;
        var total = end - StartTime;
        return total - TotalPausedTime;
    }
}

public enum SessionType
{
    TimeBased,
    Prepaid,
    Package,
    Membership,
    Free
}

public enum SessionStatus
{
    Active,
    Paused,
    Completed,
    Cancelled,
    Expired
}

public class StartSessionRequest
{
    public Guid StationId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public SessionType Type { get; set; } = SessionType.TimeBased;
    public TimeSpan? PrepaidTime { get; set; }
    public Guid? PackageId { get; set; }
}

public class SessionCost
{
    public Guid SessionId { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal BaseCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string? PrepaidTimeRemaining { get; set; }
}

public class PricingPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public StationType StationType { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal HalfHourRate { get; set; }
    public decimal MinimumCharge { get; set; }
    public int MinimumMinutes { get; set; } = 30;
    public decimal MemberDiscount { get; set; } = 10; // Percentage
    public decimal StudentDiscount { get; set; } = 15;
    public bool IsActive { get; set; } = true;
}

public class TimePackage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Hours { get; set; }
    public decimal Price { get; set; }
    public decimal SavesPercent { get; set; }
    public int ValidDays { get; set; } = 30;
    public StationType? StationType { get; set; }
    public bool IsActive { get; set; } = true;
}

// Report Models
public class DailyReport
{
    public DateTime Date { get; set; }
    public int TotalSessions { get; set; }
    public int UniqueCustomers { get; set; }
    public TimeSpan TotalUsageTime { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal CashRevenue { get; set; }
    public decimal CreditRevenue { get; set; }
    public Dictionary<StationType, int> SessionsByType { get; set; } = new();
    public Dictionary<int, int> SessionsByHour { get; set; } = new();
    public List<CafeSession> Sessions { get; set; } = new();
}

public class CafeRevenueReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageDaily { get; set; }
    public decimal HighestDay { get; set; }
    public decimal LowestDay { get; set; }
    public Dictionary<string, decimal> ByDay { get; set; } = new();
    public Dictionary<StationType, decimal> ByStationType { get; set; } = new();
    public Dictionary<string, decimal> ByPaymentMethod { get; set; } = new();
}

public class StationUsageReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<StationUsage> Stations { get; set; } = new();
    public StationType MostUsedType { get; set; }
    public decimal AverageUtilization { get; set; }
}

public class StationUsage
{
    public Guid StationId { get; set; }
    public string StationNumber { get; set; } = string.Empty;
    public StationType Type { get; set; }
    public int TotalSessions { get; set; }
    public TimeSpan TotalUsageTime { get; set; }
    public decimal Revenue { get; set; }
    public decimal UtilizationPercent { get; set; }
}

public class CustomerReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int ReturningCustomers { get; set; }
    public int ActiveMembers { get; set; }
    public List<TopCustomer> TopCustomers { get; set; } = new();
}

public class TopCustomer
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Visits { get; set; }
    public TimeSpan TotalTime { get; set; }
    public decimal TotalSpent { get; set; }
}

public class PeakHoursReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Dictionary<int, int> SessionsByHour { get; set; } = new();
    public Dictionary<DayOfWeek, int> SessionsByDay { get; set; } = new();
    public int PeakHour { get; set; }
    public DayOfWeek PeakDay { get; set; }
    public decimal AverageSessionsPerHour { get; set; }
}

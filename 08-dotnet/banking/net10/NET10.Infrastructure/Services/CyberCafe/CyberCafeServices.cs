using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.CyberCafe;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA CYBER CAFE MANAGEMENT SYSTEM
// Complete Time-Based Billing & Station Management
// ═══════════════════════════════════════════════════════════════════════════════

public class CafeStationService : ICafeStationService
{
    private static readonly List<CafeStation> _stations = InitializeDemoStations();
    
    private static List<CafeStation> InitializeDemoStations()
    {
        return new List<CafeStation>
        {
            new() { Number = "PC-01", Name = "Station 1", Type = StationType.Standard, Location = "Main Hall", Specs = "i5, 16GB RAM, GTX 1060" },
            new() { Number = "PC-02", Name = "Station 2", Type = StationType.Standard, Location = "Main Hall", Specs = "i5, 16GB RAM, GTX 1060" },
            new() { Number = "PC-03", Name = "Station 3", Type = StationType.Standard, Location = "Main Hall", Specs = "i5, 16GB RAM, GTX 1060" },
            new() { Number = "PC-04", Name = "Station 4", Type = StationType.Standard, Location = "Main Hall", Specs = "i5, 16GB RAM, GTX 1060" },
            new() { Number = "PC-05", Name = "Station 5", Type = StationType.Standard, Location = "Main Hall", Specs = "i5, 16GB RAM, GTX 1060" },
            new() { Number = "GAM-01", Name = "Gaming 1", Type = StationType.Gaming, Location = "Gaming Zone", Specs = "i9, 32GB RAM, RTX 4080" },
            new() { Number = "GAM-02", Name = "Gaming 2", Type = StationType.Gaming, Location = "Gaming Zone", Specs = "i9, 32GB RAM, RTX 4080" },
            new() { Number = "GAM-03", Name = "Gaming 3", Type = StationType.Gaming, Location = "Gaming Zone", Specs = "i9, 32GB RAM, RTX 4080" },
            new() { Number = "VIP-01", Name = "VIP Room 1", Type = StationType.VIP, Location = "VIP Area", Specs = "i9, 64GB RAM, RTX 4090" },
            new() { Number = "VIP-02", Name = "VIP Room 2", Type = StationType.VIP, Location = "VIP Area", Specs = "i9, 64GB RAM, RTX 4090" },
            new() { Number = "PRN-01", Name = "Print Station", Type = StationType.Printing, Location = "Print Center", Specs = "i5, 8GB RAM, Laser Printer" }
        };
    }
    
    public Task<List<CafeStation>> GetAllAsync() => Task.FromResult(_stations.Where(s => s.IsActive).ToList());
    
    public Task<CafeStation?> GetByIdAsync(Guid id) => Task.FromResult(_stations.FirstOrDefault(s => s.Id == id));
    
    public Task<List<CafeStation>> GetAvailableAsync() => 
        Task.FromResult(_stations.Where(s => s.Status == StationStatus.Available && s.IsActive).ToList());
    
    public Task<List<CafeStation>> GetByTypeAsync(StationType type) =>
        Task.FromResult(_stations.Where(s => s.Type == type && s.IsActive).ToList());
    
    public Task<CafeStation> CreateAsync(CafeStation station)
    {
        station.Id = Guid.NewGuid();
        station.CreatedAt = DateTime.UtcNow;
        _stations.Add(station);
        return Task.FromResult(station);
    }
    
    public Task<CafeStation> UpdateAsync(CafeStation station)
    {
        var index = _stations.FindIndex(s => s.Id == station.Id);
        if (index >= 0) _stations[index] = station;
        return Task.FromResult(station);
    }
    
    public Task<bool> SetStatusAsync(Guid id, StationStatus status)
    {
        var station = _stations.FirstOrDefault(s => s.Id == id);
        if (station != null) { station.Status = status; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var station = _stations.FirstOrDefault(s => s.Id == id);
        if (station != null) { station.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
}

public class CafeCustomerService : ICafeCustomerService
{
    private static readonly List<CafeCustomer> _customers = InitializeDemoCustomers();
    private static int _cardCounter = 10001;
    
    private static List<CafeCustomer> InitializeDemoCustomers()
    {
        return new List<CafeCustomer>
        {
            new() { CardNumber = "CAFE-10001", FirstName = "Kawennahere", LastName = "Thompson", Phone = "+1-777-555-4001", Type = CustomerType.Member, CreditBalance = 50, TotalSpent = 250, VisitCount = 45, MembershipExpiry = DateTime.UtcNow.AddMonths(6) },
            new() { CardNumber = "CAFE-10002", FirstName = "Tehonikonrathe", LastName = "White", Phone = "+1-777-555-4002", Type = CustomerType.VIP, CreditBalance = 200, TotalSpent = 850, VisitCount = 120, MembershipExpiry = DateTime.UtcNow.AddYears(1) },
            new() { CardNumber = "CAFE-10003", FirstName = "Maria", LastName = "Garcia", Phone = "+1-777-555-4003", Type = CustomerType.Student, CreditBalance = 25, TotalSpent = 75, VisitCount = 18, MembershipExpiry = DateTime.UtcNow.AddMonths(3) },
            new() { CardNumber = "CAFE-10004", FirstName = "James", LastName = "Running Deer", Phone = "+1-777-555-4004", Type = CustomerType.Registered, CreditBalance = 0, TotalSpent = 150, VisitCount = 32 }
        };
    }
    
    public Task<List<CafeCustomer>> GetAllAsync() => Task.FromResult(_customers.ToList());
    
    public Task<CafeCustomer?> GetByIdAsync(Guid id) => Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
    
    public Task<CafeCustomer?> GetByCardNumberAsync(string cardNumber) =>
        Task.FromResult(_customers.FirstOrDefault(c => c.CardNumber == cardNumber));
    
    public Task<CafeCustomer?> GetByPhoneAsync(string phone) =>
        Task.FromResult(_customers.FirstOrDefault(c => c.Phone == phone));
    
    public Task<CafeCustomer> RegisterAsync(CafeCustomer customer)
    {
        customer.Id = Guid.NewGuid();
        customer.CardNumber = $"CAFE-{_cardCounter++:D5}";
        customer.RegisteredAt = DateTime.UtcNow;
        _customers.Add(customer);
        return Task.FromResult(customer);
    }
    
    public Task<CafeCustomer> UpdateAsync(CafeCustomer customer)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0) _customers[index] = customer;
        return Task.FromResult(customer);
    }
    
    public Task<decimal> GetBalanceAsync(Guid customerId)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        return Task.FromResult(customer?.CreditBalance ?? 0);
    }
    
    public Task<CafeCustomer> AddCreditAsync(Guid customerId, decimal amount)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == customerId);
        if (customer != null) customer.CreditBalance += amount;
        return Task.FromResult(customer!);
    }
    
    public Task<List<CafeCustomer>> GetMembersAsync() =>
        Task.FromResult(_customers.Where(c => c.IsMember).ToList());
}

public class CafePricingService : ICafePricingService
{
    private static readonly List<PricingPlan> _plans = InitializePricingPlans();
    private static readonly List<TimePackage> _packages = InitializePackages();
    
    private static List<PricingPlan> InitializePricingPlans()
    {
        return new List<PricingPlan>
        {
            new() { Name = "Standard", StationType = StationType.Standard, HourlyRate = 2.50m, HalfHourRate = 1.50m, MinimumCharge = 1.50m, MinimumMinutes = 30, MemberDiscount = 10, StudentDiscount = 15 },
            new() { Name = "Gaming", StationType = StationType.Gaming, HourlyRate = 5.00m, HalfHourRate = 3.00m, MinimumCharge = 3.00m, MinimumMinutes = 30, MemberDiscount = 10, StudentDiscount = 15 },
            new() { Name = "VIP", StationType = StationType.VIP, HourlyRate = 8.00m, HalfHourRate = 5.00m, MinimumCharge = 5.00m, MinimumMinutes = 30, MemberDiscount = 15, StudentDiscount = 10 },
            new() { Name = "Printing", StationType = StationType.Printing, HourlyRate = 3.00m, HalfHourRate = 2.00m, MinimumCharge = 2.00m, MinimumMinutes = 30, MemberDiscount = 5, StudentDiscount = 10 }
        };
    }
    
    private static List<TimePackage> InitializePackages()
    {
        return new List<TimePackage>
        {
            new() { Name = "5-Hour Standard", Hours = 5, Price = 10.00m, SavesPercent = 20, ValidDays = 30, StationType = StationType.Standard },
            new() { Name = "10-Hour Standard", Hours = 10, Price = 18.00m, SavesPercent = 28, ValidDays = 30, StationType = StationType.Standard },
            new() { Name = "5-Hour Gaming", Hours = 5, Price = 20.00m, SavesPercent = 20, ValidDays = 30, StationType = StationType.Gaming },
            new() { Name = "10-Hour Gaming", Hours = 10, Price = 35.00m, SavesPercent = 30, ValidDays = 30, StationType = StationType.Gaming },
            new() { Name = "Monthly Unlimited", Hours = 999, Price = 75.00m, SavesPercent = 50, ValidDays = 30 }
        };
    }
    
    public Task<List<PricingPlan>> GetAllPlansAsync() => Task.FromResult(_plans.Where(p => p.IsActive).ToList());
    
    public Task<PricingPlan?> GetPlanAsync(Guid id) => Task.FromResult(_plans.FirstOrDefault(p => p.Id == id));
    
    public Task<PricingPlan?> GetPlanByTypeAsync(StationType stationType) =>
        Task.FromResult(_plans.FirstOrDefault(p => p.StationType == stationType && p.IsActive));
    
    public Task<PricingPlan> CreatePlanAsync(PricingPlan plan)
    {
        plan.Id = Guid.NewGuid();
        _plans.Add(plan);
        return Task.FromResult(plan);
    }
    
    public Task<PricingPlan> UpdatePlanAsync(PricingPlan plan)
    {
        var index = _plans.FindIndex(p => p.Id == plan.Id);
        if (index >= 0) _plans[index] = plan;
        return Task.FromResult(plan);
    }
    
    public async Task<decimal> CalculateCostAsync(StationType type, TimeSpan duration, bool isMember)
    {
        var plan = await GetPlanByTypeAsync(type) ?? _plans.First();
        var hours = (decimal)duration.TotalHours;
        
        if (hours < plan.MinimumMinutes / 60m) hours = plan.MinimumMinutes / 60m;
        
        var cost = hours * plan.HourlyRate;
        cost = Math.Max(cost, plan.MinimumCharge);
        
        if (isMember) cost *= (1 - plan.MemberDiscount / 100);
        
        return Math.Round(cost, 2);
    }
    
    public Task<List<TimePackage>> GetPackagesAsync() => Task.FromResult(_packages.Where(p => p.IsActive).ToList());
    
    public Task<TimePackage> CreatePackageAsync(TimePackage package)
    {
        package.Id = Guid.NewGuid();
        _packages.Add(package);
        return Task.FromResult(package);
    }
}

public class CyberCafeSessionService : ICyberCafeSessionService
{
    private static readonly List<CafeSession> _sessions = new();
    private readonly ICafeStationService _stationService;
    private readonly ICafePricingService _pricingService;
    private readonly ICafeCustomerService _customerService;
    
    public CyberCafeSessionService(ICafeStationService stationService, ICafePricingService pricingService, ICafeCustomerService customerService)
    {
        _stationService = stationService;
        _pricingService = pricingService;
        _customerService = customerService;
    }
    
    public async Task<CafeSession> StartSessionAsync(StartSessionRequest request)
    {
        var station = await _stationService.GetByIdAsync(request.StationId);
        if (station == null) throw new InvalidOperationException("Station not found");
        if (station.Status != StationStatus.Available) throw new InvalidOperationException("Station not available");
        
        var plan = await _pricingService.GetPlanByTypeAsync(station.Type);
        var customer = request.CustomerId.HasValue ? await _customerService.GetByIdAsync(request.CustomerId.Value) : null;
        
        var session = new CafeSession
        {
            StationId = request.StationId,
            StationNumber = station.Number,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName ?? customer?.FullName ?? "Guest",
            Type = request.Type,
            StartTime = DateTime.UtcNow,
            PrepaidTime = request.PrepaidTime,
            HourlyRate = plan?.HourlyRate ?? 2.50m,
            Status = SessionStatus.Active
        };
        
        _sessions.Add(session);
        await _stationService.SetStatusAsync(request.StationId, StationStatus.InUse);
        station.CurrentSessionId = session.Id;
        
        return session;
    }
    
    public async Task<CafeSession> EndSessionAsync(Guid sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) throw new InvalidOperationException("Session not found");
        
        session.EndTime = DateTime.UtcNow;
        session.Status = SessionStatus.Completed;
        
        var customer = session.CustomerId.HasValue ? await _customerService.GetByIdAsync(session.CustomerId.Value) : null;
        session.TotalCost = await _pricingService.CalculateCostAsync(
            (await _stationService.GetByIdAsync(session.StationId))?.Type ?? StationType.Standard,
            session.ActiveDuration,
            customer?.IsMember ?? false);
        
        await _stationService.SetStatusAsync(session.StationId, StationStatus.Available);
        
        return session;
    }
    
    public Task<CafeSession?> GetSessionAsync(Guid id) => Task.FromResult(_sessions.FirstOrDefault(s => s.Id == id));
    
    public Task<CafeSession?> GetActiveSessionAsync(Guid stationId) =>
        Task.FromResult(_sessions.FirstOrDefault(s => s.StationId == stationId && s.Status == SessionStatus.Active));
    
    public Task<List<CafeSession>> GetActiveSessionsAsync() =>
        Task.FromResult(_sessions.Where(s => s.Status == SessionStatus.Active || s.Status == SessionStatus.Paused).ToList());
    
    public Task<List<CafeSession>> GetCustomerHistoryAsync(Guid customerId) =>
        Task.FromResult(_sessions.Where(s => s.CustomerId == customerId).OrderByDescending(s => s.StartTime).ToList());
    
    public Task<List<CafeSession>> GetTodaySessionsAsync() =>
        Task.FromResult(_sessions.Where(s => s.StartTime.Date == DateTime.UtcNow.Date).ToList());
    
    public async Task<CafeSession> ExtendSessionAsync(Guid sessionId, TimeSpan additionalTime)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) throw new InvalidOperationException("Session not found");
        session.PrepaidTime = (session.PrepaidTime ?? TimeSpan.Zero) + additionalTime;
        session.Notes = $"Extended by {additionalTime.TotalMinutes} minutes";
        return session;
    }
    
    public async Task<CafeSession> PauseSessionAsync(Guid sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) throw new InvalidOperationException("Session not found");
        if (session.Status != SessionStatus.Active) throw new InvalidOperationException("Session not active");
        session.PausedAt = DateTime.UtcNow;
        session.Status = SessionStatus.Paused;
        return session;
    }
    
    public async Task<CafeSession> ResumeSessionAsync(Guid sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) throw new InvalidOperationException("Session not found");
        if (session.Status != SessionStatus.Paused) throw new InvalidOperationException("Session not paused");
        if (session.PausedAt.HasValue)
            session.TotalPausedTime += DateTime.UtcNow - session.PausedAt.Value;
        session.PausedAt = null;
        session.Status = SessionStatus.Active;
        return session;
    }
    
    public async Task<SessionCost> GetCurrentCostAsync(Guid sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) throw new InvalidOperationException("Session not found");
        
        var station = await _stationService.GetByIdAsync(session.StationId);
        var customer = session.CustomerId.HasValue ? await _customerService.GetByIdAsync(session.CustomerId.Value) : null;
        var cost = await _pricingService.CalculateCostAsync(station?.Type ?? StationType.Standard, session.ActiveDuration, customer?.IsMember ?? false);
        
        return new SessionCost
        {
            SessionId = sessionId,
            Duration = session.ActiveDuration,
            HourlyRate = session.HourlyRate,
            BaseCost = cost,
            Discount = customer?.IsMember == true ? cost * 0.1m : 0,
            TotalCost = cost,
            AmountPaid = session.AmountPaid,
            AmountDue = cost - session.AmountPaid
        };
    }
}

public class CafeReportService : ICafeReportService
{
    private readonly ICyberCafeSessionService _sessionService;
    private readonly ICafeStationService _stationService;
    private readonly ICafeCustomerService _customerService;
    
    public CafeReportService(ICyberCafeSessionService sessionService, ICafeStationService stationService, ICafeCustomerService customerService)
    {
        _sessionService = sessionService;
        _stationService = stationService;
        _customerService = customerService;
    }
    
    public async Task<DailyReport> GetDailyReportAsync(DateTime date)
    {
        var sessions = await _sessionService.GetTodaySessionsAsync();
        return new DailyReport
        {
            Date = date,
            TotalSessions = sessions.Count > 0 ? sessions.Count : 48,
            UniqueCustomers = 35,
            TotalUsageTime = TimeSpan.FromHours(72),
            TotalRevenue = sessions.Sum(s => s.TotalCost) > 0 ? sessions.Sum(s => s.TotalCost) : 285.50m,
            CashRevenue = 180.00m,
            CreditRevenue = 105.50m,
            SessionsByType = new Dictionary<StationType, int> { { StationType.Standard, 28 }, { StationType.Gaming, 15 }, { StationType.VIP, 3 }, { StationType.Printing, 2 } },
            SessionsByHour = Enumerable.Range(8, 14).ToDictionary(h => h, h => Random.Shared.Next(2, 8))
        };
    }
    
    public Task<CafeRevenueReport> GetRevenueReportAsync(DateTime from, DateTime to)
    {
        return Task.FromResult(new CafeRevenueReport
        {
            FromDate = from,
            ToDate = to,
            TotalRevenue = 4850.00m,
            AverageDaily = 161.67m,
            HighestDay = 285.50m,
            LowestDay = 95.00m,
            ByStationType = new Dictionary<StationType, decimal> { { StationType.Standard, 1850 }, { StationType.Gaming, 2100 }, { StationType.VIP, 750 }, { StationType.Printing, 150 } },
            ByPaymentMethod = new Dictionary<string, decimal> { { "Cash", 2500 }, { "Credit", 1850 }, { "Package", 500 } }
        });
    }
    
    public async Task<StationUsageReport> GetStationUsageAsync(DateTime from, DateTime to)
    {
        var stations = await _stationService.GetAllAsync();
        return new StationUsageReport
        {
            FromDate = from,
            ToDate = to,
            Stations = stations.Select(s => new StationUsage
            {
                StationId = s.Id,
                StationNumber = s.Number,
                Type = s.Type,
                TotalSessions = Random.Shared.Next(20, 80),
                TotalUsageTime = TimeSpan.FromHours(Random.Shared.Next(50, 150)),
                Revenue = Random.Shared.Next(200, 600),
                UtilizationPercent = Random.Shared.Next(40, 85)
            }).ToList(),
            MostUsedType = StationType.Gaming,
            AverageUtilization = 65.5m
        };
    }
    
    public async Task<CustomerReport> GetCustomerReportAsync(DateTime from, DateTime to)
    {
        var customers = await _customerService.GetAllAsync();
        return new CustomerReport
        {
            FromDate = from,
            ToDate = to,
            TotalCustomers = customers.Count + 150,
            NewCustomers = 28,
            ReturningCustomers = 125,
            ActiveMembers = customers.Count(c => c.IsMember),
            TopCustomers = customers.OrderByDescending(c => c.TotalSpent).Take(5).Select(c => new TopCustomer
            {
                CustomerId = c.Id,
                Name = c.FullName,
                Visits = c.VisitCount,
                TotalTime = c.TotalTimeUsed,
                TotalSpent = c.TotalSpent
            }).ToList()
        };
    }
    
    public Task<PeakHoursReport> GetPeakHoursAsync(DateTime from, DateTime to)
    {
        return Task.FromResult(new PeakHoursReport
        {
            FromDate = from,
            ToDate = to,
            SessionsByHour = Enumerable.Range(8, 14).ToDictionary(h => h, h => h >= 14 && h <= 20 ? Random.Shared.Next(6, 10) : Random.Shared.Next(2, 5)),
            SessionsByDay = Enum.GetValues<DayOfWeek>().ToDictionary(d => d, d => d == DayOfWeek.Saturday ? 85 : Random.Shared.Next(35, 65)),
            PeakHour = 17,
            PeakDay = DayOfWeek.Saturday,
            AverageSessionsPerHour = 4.8m
        });
    }
}

// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Database Context
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using Microsoft.EntityFrameworkCore;
using Ierahkwa.ForexInvestment.Domain.Entities;

namespace Ierahkwa.ForexInvestment.Persistence;

/// <summary>
/// Entity Framework Core Database Context for Forex Investment System
/// </summary>
public class ForexDbContext : DbContext
{
    public ForexDbContext(DbContextOptions<ForexDbContext> options) : base(options) { }
    
    public DbSet<ForexAccount> ForexAccounts { get; set; }
    public DbSet<Investment> Investments { get; set; }
    public DbSet<InvestmentPlan> InvestmentPlans { get; set; }
    public DbSet<InvestmentDuration> InvestmentDurations { get; set; }
    public DbSet<PlanDurationMapping> PlanDurationMappings { get; set; }
    public DbSet<ForexTransaction> ForexTransactions { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<SignalProvider> SignalProviders { get; set; }
    public DbSet<TradingSignal> TradingSignals { get; set; }
    public DbSet<SignalSubscription> SignalSubscriptions { get; set; }
    public DbSet<Broker> Brokers { get; set; }
    public DbSet<WithdrawalLimit> WithdrawalLimits { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // ForexAccount configuration
        modelBuilder.Entity<ForexAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.Property(e => e.Balance).HasPrecision(18, 8);
            entity.Property(e => e.Equity).HasPrecision(18, 8);
            entity.Property(e => e.Margin).HasPrecision(18, 8);
            entity.Property(e => e.FreeMargin).HasPrecision(18, 8);
            entity.Property(e => e.Profit).HasPrecision(18, 8);
        });
        
        // Investment configuration
        modelBuilder.Entity<Investment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.InvestmentNumber).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(18, 8);
            entity.Property(e => e.ExpectedProfit).HasPrecision(18, 8);
            entity.Property(e => e.ActualProfit).HasPrecision(18, 8);
            entity.Property(e => e.TotalReturn).HasPrecision(18, 8);
            
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Investments)
                  .HasForeignKey(e => e.AccountId);
                  
            entity.HasOne(e => e.Plan)
                  .WithMany(p => p.Investments)
                  .HasForeignKey(e => e.PlanId);
        });
        
        // InvestmentPlan configuration
        modelBuilder.Entity<InvestmentPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.MinAmount).HasPrecision(18, 8);
            entity.Property(e => e.MaxAmount).HasPrecision(18, 8);
            entity.Property(e => e.MinROI).HasPrecision(5, 2);
            entity.Property(e => e.MaxROI).HasPrecision(5, 2);
        });
        
        // PlanDurationMapping configuration
        modelBuilder.Entity<PlanDurationMapping>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PlanId, e.DurationId }).IsUnique();
            
            entity.HasOne(e => e.Plan)
                  .WithMany(p => p.DurationMappings)
                  .HasForeignKey(e => e.PlanId);
                  
            entity.HasOne(e => e.Duration)
                  .WithMany(d => d.PlanMappings)
                  .HasForeignKey(e => e.DurationId);
        });
        
        // ForexTransaction configuration
        modelBuilder.Entity<ForexTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransactionNumber).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(18, 8);
            entity.Property(e => e.Fee).HasPrecision(18, 8);
            entity.Property(e => e.NetAmount).HasPrecision(18, 8);
            
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(e => e.AccountId);
        });
        
        // Trade configuration
        modelBuilder.Entity<Trade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountId);
            entity.HasIndex(e => e.TicketNumber);
            entity.Property(e => e.LotSize).HasPrecision(10, 5);
            entity.Property(e => e.OpenPrice).HasPrecision(18, 8);
            entity.Property(e => e.ClosePrice).HasPrecision(18, 8);
            entity.Property(e => e.Profit).HasPrecision(18, 8);
            
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Trades)
                  .HasForeignKey(e => e.AccountId);
        });
        
        // SignalProvider configuration
        modelBuilder.Entity<SignalProvider>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.TotalProfit).HasPrecision(18, 8);
            entity.Property(e => e.WinRate).HasPrecision(5, 2);
            entity.Property(e => e.MonthlyFee).HasPrecision(18, 8);
        });
        
        // TradingSignal configuration
        modelBuilder.Entity<TradingSignal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProviderId);
            entity.Property(e => e.EntryPrice).HasPrecision(18, 8);
            entity.Property(e => e.StopLoss).HasPrecision(18, 8);
            entity.Property(e => e.TakeProfit1).HasPrecision(18, 8);
            
            entity.HasOne(e => e.Provider)
                  .WithMany(p => p.Signals)
                  .HasForeignKey(e => e.ProviderId);
        });
        
        // SignalSubscription configuration
        modelBuilder.Entity<SignalSubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AccountId, e.ProviderId }).IsUnique();
            
            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId);
                  
            entity.HasOne(e => e.Provider)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(e => e.ProviderId);
        });
        
        // Broker configuration
        modelBuilder.Entity<Broker>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.MinDeposit).HasPrecision(18, 8);
        });
        
        // WithdrawalLimit configuration
        modelBuilder.Entity<WithdrawalLimit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.DailyLimit).HasPrecision(18, 8);
            entity.Property(e => e.MonthlyLimit).HasPrecision(18, 8);
        });
        
        // Seed default data
        SeedDefaultData(modelBuilder);
    }
    
    private void SeedDefaultData(ModelBuilder modelBuilder)
    {
        // Seed brokers
        modelBuilder.Entity<Broker>().HasData(
            new Broker
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Ierahkwa Futurehead Broker",
                Code = "IEFH",
                Description = "Official Ierahkwa Futurehead Forex Broker",
                SupportsMT4 = true,
                SupportsMT5 = true,
                MT5Server = "ierahkwa-mt5.server.com",
                MinDeposit = 100,
                IsActive = true,
                IsFeatured = true
            }
        );
        
        // Seed durations
        var durationIds = new[]
        {
            Guid.Parse("d1111111-1111-1111-1111-111111111111"),
            Guid.Parse("d2222222-2222-2222-2222-222222222222"),
            Guid.Parse("d3333333-3333-3333-3333-333333333333"),
            Guid.Parse("d4444444-4444-4444-4444-444444444444"),
            Guid.Parse("d5555555-5555-5555-5555-555555555555")
        };
        
        modelBuilder.Entity<InvestmentDuration>().HasData(
            new InvestmentDuration { Id = durationIds[0], Name = "24 Hours", Type = DurationType.Hourly, Value = 24, ROIBonus = 0, SortOrder = 1 },
            new InvestmentDuration { Id = durationIds[1], Name = "7 Days", Type = DurationType.Daily, Value = 7, ROIBonus = 1, SortOrder = 2 },
            new InvestmentDuration { Id = durationIds[2], Name = "30 Days", Type = DurationType.Daily, Value = 30, ROIBonus = 2, SortOrder = 3 },
            new InvestmentDuration { Id = durationIds[3], Name = "90 Days", Type = DurationType.Daily, Value = 90, ROIBonus = 5, SortOrder = 4 },
            new InvestmentDuration { Id = durationIds[4], Name = "365 Days", Type = DurationType.Yearly, Value = 1, ROIBonus = 10, SortOrder = 5 }
        );
        
        // Seed investment plans
        var planIds = new[]
        {
            Guid.Parse("p1111111-1111-1111-1111-111111111111"),
            Guid.Parse("p2222222-2222-2222-2222-222222222222"),
            Guid.Parse("p3333333-3333-3333-3333-333333333333"),
            Guid.Parse("p4444444-4444-4444-4444-444444444444")
        };
        
        modelBuilder.Entity<InvestmentPlan>().HasData(
            new InvestmentPlan
            {
                Id = planIds[0],
                Name = "Starter Plan",
                Code = "STARTER",
                Description = "Perfect for beginners",
                Type = PlanType.Standard,
                Category = PlanCategory.Forex,
                MinAmount = 100,
                MaxAmount = 5000,
                MinROI = 5,
                MaxROI = 12,
                RiskLevel = RiskLevel.Low,
                IsFeatured = true,
                SortOrder = 1
            },
            new InvestmentPlan
            {
                Id = planIds[1],
                Name = "Growth Plan",
                Code = "GROWTH",
                Description = "Balanced returns with moderate risk",
                Type = PlanType.Standard,
                Category = PlanCategory.Mixed,
                MinAmount = 1000,
                MaxAmount = 25000,
                MinROI = 10,
                MaxROI = 20,
                RiskLevel = RiskLevel.Medium,
                IsTrending = true,
                IsFeatured = true,
                SortOrder = 2
            },
            new InvestmentPlan
            {
                Id = planIds[2],
                Name = "Premium Plan",
                Code = "PREMIUM",
                Description = "Higher returns for experienced investors",
                Type = PlanType.Premium,
                Category = PlanCategory.Forex,
                MinAmount = 5000,
                MaxAmount = 100000,
                MinROI = 15,
                MaxROI = 30,
                RiskLevel = RiskLevel.High,
                IsTrending = true,
                SortOrder = 3
            },
            new InvestmentPlan
            {
                Id = planIds[3],
                Name = "VIP Elite Plan",
                Code = "VIP",
                Description = "Exclusive high-yield opportunities",
                Type = PlanType.VIP,
                Category = PlanCategory.Mixed,
                MinAmount = 25000,
                MaxAmount = 500000,
                MinROI = 20,
                MaxROI = 45,
                RiskLevel = RiskLevel.VeryHigh,
                IsFeatured = true,
                SortOrder = 4
            }
        );
    }
}

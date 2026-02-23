using NET10.Core.Interfaces;
using NET10.Core.Models.Hotel;

namespace NET10.Infrastructure.Services.Hotel;

public class RealEstateService : IRealEstateService
{
    private static readonly List<RealEstateListing> _listings = InitializeListings();
    private static int _listingCounter = 1000;
    
    private static List<RealEstateListing> InitializeListings()
    {
        return new List<RealEstateListing>
        {
            new RealEstateListing
            {
                ListingNumber = "RE-001001",
                Type = ListingType.ForSale,
                Title = "Luxury Oceanfront Villa - Futurehead Estate",
                Description = "Breathtaking oceanfront villa with private beach access. 6 bedrooms, infinity pool, smart home features. Accept crypto payments including FHT.",
                Price = 8500000,
                Currency = "USD",
                AcceptsCrypto = true,
                CryptoPrice = 3400000,
                CryptoToken = "FHT",
                Size = 12500,
                SizeUnit = "sqft",
                Bedrooms = 6,
                Bathrooms = 8,
                Parking = 4,
                YearBuilt = 2023,
                Features = ["Ocean View", "Private Beach", "Infinity Pool", "Wine Cellar", "Home Theater", "Smart Home", "Solar Panels", "Helipad"],
                Status = ListingStatus.Active,
                AgentName = "Sovereign Realty Team",
                AgentEmail = "realty@ierahkwa.gov",
                Views = 2450
            },
            new RealEstateListing
            {
                ListingNumber = "RE-001002",
                Type = ListingType.ForSale,
                Title = "Downtown Penthouse - Ierahkwa Tower",
                Description = "Stunning penthouse with 360-degree city views. Triple-height ceilings, private elevator, rooftop terrace.",
                Price = 4200000,
                Currency = "USD",
                AcceptsCrypto = true,
                CryptoPrice = 1680000,
                CryptoToken = "FHT",
                Size = 5800,
                SizeUnit = "sqft",
                Bedrooms = 4,
                Bathrooms = 5,
                Parking = 3,
                YearBuilt = 2024,
                Features = ["City View", "Private Elevator", "Rooftop Terrace", "Chef Kitchen", "Smart Home", "Concierge"],
                Status = ListingStatus.Active,
                AgentName = "Sovereign Realty Team",
                AgentEmail = "realty@ierahkwa.gov",
                Views = 1820
            },
            new RealEstateListing
            {
                ListingNumber = "RE-001003",
                Type = ListingType.ForRent,
                Title = "Luxury Apartment - Financial District",
                Description = "Modern 2BR apartment perfect for executives. Walking distance to business center.",
                Price = 4500, // Monthly rent
                Currency = "USD",
                AcceptsCrypto = true,
                CryptoPrice = 1800,
                CryptoToken = "FHT",
                Size = 1400,
                SizeUnit = "sqft",
                Bedrooms = 2,
                Bathrooms = 2,
                Parking = 1,
                YearBuilt = 2022,
                Features = ["Gym", "Pool", "Doorman", "Parking", "Balcony"],
                Status = ListingStatus.Active,
                AgentName = "Sovereign Realty Team",
                AgentEmail = "realty@ierahkwa.gov",
                Views = 890
            },
            new RealEstateListing
            {
                ListingNumber = "RE-001004",
                Type = ListingType.ForSale,
                Title = "Commercial Building - Prime Location",
                Description = "Grade A office building with retail on ground floor. Excellent investment opportunity.",
                Price = 25000000,
                Currency = "USD",
                AcceptsCrypto = true,
                CryptoPrice = 10000000,
                CryptoToken = "FHT",
                Size = 45000,
                SizeUnit = "sqft",
                Bedrooms = 0,
                Bathrooms = 20,
                Parking = 100,
                YearBuilt = 2020,
                Features = ["Retail Space", "Office Space", "Parking Garage", "Conference Center", "Rooftop"],
                Status = ListingStatus.Active,
                AgentName = "Sovereign Commercial",
                AgentEmail = "commercial@ierahkwa.gov",
                Views = 3200
            }
        };
    }
    
    public Task<List<RealEstateListing>> GetAllAsync()
    {
        return Task.FromResult(_listings.ToList());
    }
    
    public Task<List<RealEstateListing>> GetActiveAsync()
    {
        return Task.FromResult(_listings.Where(l => l.Status == ListingStatus.Active).ToList());
    }
    
    public Task<RealEstateListing?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_listings.FirstOrDefault(l => l.Id == id));
    }
    
    public async Task<RealEstateListing> CreateAsync(RealEstateListing listing)
    {
        listing.Id = Guid.NewGuid();
        listing.ListingNumber = await GenerateListingNumberAsync();
        listing.ListedDate = DateTime.UtcNow;
        listing.Status = ListingStatus.Active;
        _listings.Add(listing);
        return listing;
    }
    
    public Task<RealEstateListing> UpdateAsync(RealEstateListing listing)
    {
        var index = _listings.FindIndex(l => l.Id == listing.Id);
        if (index >= 0) _listings[index] = listing;
        return Task.FromResult(listing);
    }
    
    public Task<RealEstateListing> UpdateStatusAsync(Guid listingId, ListingStatus status)
    {
        var listing = _listings.FirstOrDefault(l => l.Id == listingId);
        if (listing != null)
        {
            listing.Status = status;
            if (status == ListingStatus.Sold || status == ListingStatus.Rented)
            {
                listing.SoldDate = DateTime.UtcNow;
            }
        }
        return Task.FromResult(listing!);
    }
    
    public Task<List<RealEstateListing>> SearchAsync(RealEstateSearchCriteria criteria)
    {
        var query = _listings.Where(l => l.Status == ListingStatus.Active);
        
        if (criteria.Type.HasValue)
            query = query.Where(l => l.Type == criteria.Type.Value);
        
        if (!string.IsNullOrEmpty(criteria.City))
            query = query.Where(l => l.Title.Contains(criteria.City, StringComparison.OrdinalIgnoreCase));
        
        if (criteria.MinPrice.HasValue)
            query = query.Where(l => l.Price >= criteria.MinPrice.Value);
        
        if (criteria.MaxPrice.HasValue)
            query = query.Where(l => l.Price <= criteria.MaxPrice.Value);
        
        if (criteria.MinBedrooms.HasValue)
            query = query.Where(l => l.Bedrooms >= criteria.MinBedrooms.Value);
        
        if (criteria.MaxBedrooms.HasValue)
            query = query.Where(l => l.Bedrooms <= criteria.MaxBedrooms.Value);
        
        if (criteria.MinSize.HasValue)
            query = query.Where(l => l.Size >= criteria.MinSize.Value);
        
        if (criteria.AcceptsCrypto.HasValue)
            query = query.Where(l => l.AcceptsCrypto == criteria.AcceptsCrypto.Value);
        
        return Task.FromResult(query.ToList());
    }
    
    public Task<RealEstateListing> IncrementViewsAsync(Guid listingId)
    {
        var listing = _listings.FirstOrDefault(l => l.Id == listingId);
        if (listing != null)
        {
            listing.Views++;
        }
        return Task.FromResult(listing!);
    }
    
    public Task<string> GenerateListingNumberAsync()
    {
        _listingCounter++;
        return Task.FromResult($"RE-{_listingCounter:D6}");
    }
}

public class FutureheadTrustService : IFutureheadTrustService
{
    private static readonly List<FutureheadTrustTransaction> _transactions = [];
    private static decimal _currentFHTRate = 2.5m; // 1 FHT = $2.50 USD
    
    public Task<FutureheadTrustTransaction> CreateTransactionAsync(FutureheadTrustTransaction tx)
    {
        tx.Id = Guid.NewGuid();
        tx.CreatedAt = DateTime.UtcNow;
        tx.ExchangeRate = _currentFHTRate;
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<FutureheadTrustTransaction?> GetByHashAsync(string txHash)
    {
        return Task.FromResult(_transactions.FirstOrDefault(t => t.TxHash == txHash));
    }
    
    public Task<List<FutureheadTrustTransaction>> GetByPropertyAsync(Guid propertyId)
    {
        return Task.FromResult(_transactions
            .Where(t => t.PropertyId == propertyId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList());
    }
    
    public Task<FutureheadTrustTransaction> ConfirmTransactionAsync(string txHash, int confirmations)
    {
        var tx = _transactions.FirstOrDefault(t => t.TxHash == txHash);
        if (tx != null)
        {
            tx.Confirmations = confirmations;
            if (confirmations >= 6)
            {
                tx.Status = FHTTxStatus.Confirmed;
                tx.ConfirmedAt = DateTime.UtcNow;
            }
        }
        return Task.FromResult(tx!);
    }
    
    public Task<decimal> GetFHTExchangeRateAsync()
    {
        // In production, this would fetch from an oracle or exchange
        return Task.FromResult(_currentFHTRate);
    }
    
    public Task<decimal> ConvertUSDToFHTAsync(decimal usdAmount)
    {
        return Task.FromResult(usdAmount / _currentFHTRate);
    }
    
    public Task<decimal> ConvertFHTToUSDAsync(decimal fhtAmount)
    {
        return Task.FromResult(fhtAmount * _currentFHTRate);
    }
    
    public Task<string> GeneratePaymentAddressAsync()
    {
        // Generate a unique payment address
        var address = $"0xFHT{Guid.NewGuid():N}".Substring(0, 42);
        return Task.FromResult(address);
    }
}

public class HotelReportsService : IHotelReportsService
{
    public Task<HotelDashboard> GetDashboardAsync(Guid propertyId)
    {
        // Demo data
        var dashboard = new HotelDashboard
        {
            PropertyId = propertyId,
            Date = DateTime.UtcNow,
            TotalRooms = 200,
            OccupiedRooms = 156,
            AvailableRooms = 44,
            TodayRevenue = 45800,
            WeekRevenue = 312500,
            MonthRevenue = 1250000,
            ADR = 294,
            RevPAR = 229,
            TodayCheckIns = 28,
            TodayCheckOuts = 22,
            PendingReservations = 45,
            CryptoRevenue = 125000,
            FHTRevenue = 50000
        };
        
        return Task.FromResult(dashboard);
    }
    
    public Task<OccupancyReport> GetOccupancyReportAsync(Guid propertyId, DateTime from, DateTime to)
    {
        var days = new List<DailyOccupancy>();
        var current = from;
        var random = new Random();
        
        while (current <= to)
        {
            var occupancy = random.Next(60, 95);
            days.Add(new DailyOccupancy
            {
                Date = current,
                TotalRooms = 200,
                Occupied = occupancy * 2,
                OccupancyRate = occupancy,
                Revenue = occupancy * 300 * 2,
                ADR = 300
            });
            current = current.AddDays(1);
        }
        
        return Task.FromResult(new OccupancyReport
        {
            PropertyId = propertyId,
            FromDate = from,
            ToDate = to,
            Days = days,
            AverageOccupancy = days.Average(d => d.OccupancyRate),
            TotalRevenue = days.Sum(d => d.Revenue)
        });
    }
    
    public Task<RevenueReport> GetRevenueReportAsync(Guid propertyId, DateTime from, DateTime to)
    {
        var report = new RevenueReport
        {
            PropertyId = propertyId,
            FromDate = from,
            ToDate = to,
            TotalRevenue = 1250000,
            RoomRevenue = 1050000,
            AddOnRevenue = 200000,
            TaxCollected = 168000,
            CryptoRevenue = 125000,
            FHTRevenue = 50000
        };
        
        return Task.FromResult(report);
    }
    
    public Task<List<TransactionSummary>> GetDailyTransactionsAsync(Guid propertyId, DateTime date)
    {
        return Task.FromResult(new List<TransactionSummary>
        {
            new() { Date = date, Reference = "TRX-001", GuestName = "John Smith", Description = "Room Payment", Amount = 450, PaymentMethod = "Card" },
            new() { Date = date, Reference = "TRX-002", GuestName = "Maria Garcia", Description = "FHT Payment", Amount = 1200, PaymentMethod = "Crypto" }
        });
    }
    
    public Task<List<TransactionSummary>> GetWeeklyTransactionsAsync(Guid propertyId, DateTime weekStart)
    {
        return GetDailyTransactionsAsync(propertyId, weekStart);
    }
    
    public Task<List<TransactionSummary>> GetMonthlyTransactionsAsync(Guid propertyId, int year, int month)
    {
        return GetDailyTransactionsAsync(propertyId, new DateTime(year, month, 1));
    }
}

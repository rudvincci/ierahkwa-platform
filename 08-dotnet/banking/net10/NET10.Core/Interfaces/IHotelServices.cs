using NET10.Core.Models.Hotel;

namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════
// PROPERTY SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IPropertyService
{
    Task<List<Property>> GetAllAsync();
    Task<List<Property>> GetByTypeAsync(PropertyType type);
    Task<Property?> GetByIdAsync(Guid id);
    Task<Property> CreateAsync(Property property);
    Task<Property> UpdateAsync(Property property);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Property>> SearchAsync(PropertySearchCriteria criteria);
    Task<List<Property>> GetFeaturedAsync();
}

public class PropertySearchCriteria
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public PropertyType? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? Guests { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public List<string>? Amenities { get; set; }
    public bool? AcceptsCrypto { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// ROOM SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IRoomService
{
    Task<List<Room>> GetByPropertyAsync(Guid propertyId);
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room> CreateAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Room>> GetAvailableAsync(Guid propertyId, DateTime checkIn, DateTime checkOut);
    Task<Room> UpdateStatusAsync(Guid roomId, RoomStatus status);
    
    // Room Types
    Task<List<RoomType>> GetRoomTypesAsync(Guid propertyId);
    Task<RoomType> CreateRoomTypeAsync(RoomType roomType);
    Task<RoomType> UpdateRoomTypeAsync(RoomType roomType);
}

// ═══════════════════════════════════════════════════════════════
// GUEST SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IGuestService
{
    Task<List<Guest>> GetAllAsync();
    Task<Guest?> GetByIdAsync(Guid id);
    Task<Guest?> GetByEmailAsync(string email);
    Task<Guest> CreateAsync(Guest guest);
    Task<Guest> UpdateAsync(Guest guest);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Guest>> SearchAsync(string searchTerm);
    Task<Guest> AddLoyaltyPointsAsync(Guid guestId, int points);
    Task<string> GenerateGuestNumberAsync();
}

// ═══════════════════════════════════════════════════════════════
// RESERVATION SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IReservationService
{
    Task<List<Reservation>> GetByPropertyAsync(Guid propertyId);
    Task<List<Reservation>> GetByDateRangeAsync(Guid propertyId, DateTime from, DateTime to);
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<Reservation?> GetByNumberAsync(string reservationNumber);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task<Reservation> UpdateAsync(Reservation reservation);
    Task<bool> CancelAsync(Guid id, string reason);
    
    Task<Reservation> ConfirmAsync(Guid id);
    Task<Booking> CheckInAsync(Guid reservationId);
    Task<Booking> CheckOutAsync(Guid bookingId);
    
    Task<string> GenerateReservationNumberAsync();
    Task<List<Reservation>> GetTodayCheckInsAsync(Guid propertyId);
    Task<List<Reservation>> GetTodayCheckOutsAsync(Guid propertyId);
    
    // Pricing
    Task<ReservationPricing> CalculatePricingAsync(Guid roomId, DateTime checkIn, DateTime checkOut, int guests);
}

public class ReservationPricing
{
    public decimal RoomRate { get; set; }
    public int Nights { get; set; }
    public decimal RoomTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal CleaningFee { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal FHTEquivalent { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// BOOKING SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IBookingService
{
    Task<List<Booking>> GetActiveAsync(Guid propertyId);
    Task<Booking?> GetByIdAsync(Guid id);
    Task<Booking?> GetByRoomAsync(Guid roomId);
    Task<Booking> CreateFromReservationAsync(Guid reservationId);
    Task<Booking> AddChargeAsync(Guid bookingId, BookingCharge charge);
    Task<Booking> ProcessPaymentAsync(Guid bookingId, HotelPayment payment);
    Task<Booking> ExtendStayAsync(Guid bookingId, DateTime newCheckOut);
    Task<Booking> TransferRoomAsync(Guid bookingId, Guid newRoomId);
}

// ═══════════════════════════════════════════════════════════════
// ADD-ON SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IAddOnService
{
    Task<List<AddOn>> GetByPropertyAsync(Guid propertyId);
    Task<AddOn?> GetByIdAsync(Guid id);
    Task<AddOn> CreateAsync(AddOn addOn);
    Task<AddOn> UpdateAsync(AddOn addOn);
    Task<bool> DeleteAsync(Guid id);
    Task<List<AddOn>> GetByCategoryAsync(Guid propertyId, string category);
}

// ═══════════════════════════════════════════════════════════════
// PAYMENT SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IHotelPaymentService
{
    Task<List<HotelPayment>> GetByPropertyAsync(Guid propertyId, DateTime from, DateTime to);
    Task<HotelPayment?> GetByIdAsync(Guid id);
    Task<HotelPayment> ProcessPaymentAsync(HotelPayment payment);
    Task<HotelPayment> ProcessCryptoPaymentAsync(CryptoPaymentRequest request);
    Task<HotelPayment> RefundAsync(Guid paymentId, decimal amount, string reason);
    Task<string> GenerateTransactionNumberAsync();
}

public class CryptoPaymentRequest
{
    public Guid? ReservationId { get; set; }
    public Guid? BookingId { get; set; }
    public decimal USDAmount { get; set; }
    public string TokenSymbol { get; set; } = "FHT";
    public string WalletFrom { get; set; } = string.Empty;
    public string? TxHash { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// HOUSEKEEPING SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IHousekeepingService
{
    Task<List<HousekeepingTask>> GetByPropertyAsync(Guid propertyId);
    Task<List<HousekeepingTask>> GetTodayTasksAsync(Guid propertyId);
    Task<List<HousekeepingTask>> GetByRoomAsync(Guid roomId);
    Task<HousekeepingTask> CreateAsync(HousekeepingTask task);
    Task<HousekeepingTask> UpdateStatusAsync(Guid taskId, NET10.Core.Models.Hotel.TaskStatus status);
    Task<HousekeepingTask> AssignAsync(Guid taskId, string staffName);
    Task<HousekeepingTask> CompleteAsync(Guid taskId, string completedBy);
}

// ═══════════════════════════════════════════════════════════════
// REAL ESTATE SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IRealEstateService
{
    Task<List<RealEstateListing>> GetAllAsync();
    Task<List<RealEstateListing>> GetActiveAsync();
    Task<RealEstateListing?> GetByIdAsync(Guid id);
    Task<RealEstateListing> CreateAsync(RealEstateListing listing);
    Task<RealEstateListing> UpdateAsync(RealEstateListing listing);
    Task<RealEstateListing> UpdateStatusAsync(Guid listingId, ListingStatus status);
    Task<List<RealEstateListing>> SearchAsync(RealEstateSearchCriteria criteria);
    Task<RealEstateListing> IncrementViewsAsync(Guid listingId);
    Task<string> GenerateListingNumberAsync();
}

public class RealEstateSearchCriteria
{
    public ListingType? Type { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }
    public decimal? MinSize { get; set; }
    public decimal? MaxSize { get; set; }
    public bool? AcceptsCrypto { get; set; }
    public List<string>? Features { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// FUTUREHEAD TRUST SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IFutureheadTrustService
{
    Task<FutureheadTrustTransaction> CreateTransactionAsync(FutureheadTrustTransaction tx);
    Task<FutureheadTrustTransaction?> GetByHashAsync(string txHash);
    Task<List<FutureheadTrustTransaction>> GetByPropertyAsync(Guid propertyId);
    Task<FutureheadTrustTransaction> ConfirmTransactionAsync(string txHash, int confirmations);
    Task<decimal> GetFHTExchangeRateAsync();
    Task<decimal> ConvertUSDToFHTAsync(decimal usdAmount);
    Task<decimal> ConvertFHTToUSDAsync(decimal fhtAmount);
    Task<string> GeneratePaymentAddressAsync();
}

// ═══════════════════════════════════════════════════════════════
// REPORTS SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IHotelReportsService
{
    Task<HotelDashboard> GetDashboardAsync(Guid propertyId);
    Task<OccupancyReport> GetOccupancyReportAsync(Guid propertyId, DateTime from, DateTime to);
    Task<RevenueReport> GetRevenueReportAsync(Guid propertyId, DateTime from, DateTime to);
    Task<List<TransactionSummary>> GetDailyTransactionsAsync(Guid propertyId, DateTime date);
    Task<List<TransactionSummary>> GetWeeklyTransactionsAsync(Guid propertyId, DateTime weekStart);
    Task<List<TransactionSummary>> GetMonthlyTransactionsAsync(Guid propertyId, int year, int month);
}

public class RevenueReport
{
    public Guid PropertyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RoomRevenue { get; set; }
    public decimal AddOnRevenue { get; set; }
    public decimal TaxCollected { get; set; }
    public decimal CryptoRevenue { get; set; }
    public decimal FHTRevenue { get; set; }
    public List<DailyRevenue> DailyBreakdown { get; set; } = [];
}

public class DailyRevenue
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}

public class TransactionSummary
{
    public DateTime Date { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

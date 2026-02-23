using System;
using System.Collections.Generic;
using System.Linq;

namespace NET10.Core.Models.Hotel;

// ═══════════════════════════════════════════════════════════════
// PROPERTY - Base for Hotels, Airbnb, Real Estate
// ═══════════════════════════════════════════════════════════════
public class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PropertyType Type { get; set; } = PropertyType.Hotel;
    public string Description { get; set; } = string.Empty;
    
    // Location
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Contact
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    
    // Owner
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    
    // Media
    public List<string> Images { get; set; } = [];
    public string MainImage { get; set; } = string.Empty;
    
    // Rating
    public decimal Rating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    
    // Pricing
    public string Currency { get; set; } = "USD";
    public decimal BasePrice { get; set; }
    public decimal CleaningFee { get; set; }
    public decimal ServiceFee { get; set; }
    
    // Amenities
    public List<string> Amenities { get; set; } = [];
    public List<string> HouseRules { get; set; } = [];
    
    // Capacity
    public int MaxGuests { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Beds { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool AcceptsCrypto { get; set; } = true;
    public List<string> AcceptedTokens { get; set; } = ["USDT", "FHT", "IGT"];
    
    // Real Estate specific
    public decimal SalePrice { get; set; }
    public bool ForSale { get; set; } = false;
    public bool ForRent { get; set; } = true;
    public decimal PropertySize { get; set; } // sqft or sqm
    public int YearBuilt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum PropertyType
{
    Hotel,
    Hostel,
    Resort,
    Apartment,
    House,
    Villa,
    Cabin,
    Condo,
    Loft,
    Commercial,
    Land
}

// ═══════════════════════════════════════════════════════════════
// ROOM / UNIT
// ═══════════════════════════════════════════════════════════════
public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    // Type
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    
    // Details
    public string Description { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string View { get; set; } = string.Empty; // Ocean, Garden, City, etc.
    public int MaxOccupancy { get; set; }
    public int Beds { get; set; }
    public string BedType { get; set; } = string.Empty; // King, Queen, Twin, etc.
    
    // Pricing
    public decimal BaseRate { get; set; }
    public decimal WeekendRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    
    // Status
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public bool IsActive { get; set; } = true;
    public bool NeedsCleaning { get; set; } = false;
    public bool NeedsMaintenance { get; set; } = false;
    
    // Features
    public List<string> Amenities { get; set; } = [];
    public List<string> Images { get; set; } = [];
}

public enum RoomStatus
{
    Available,
    Occupied,
    Reserved,
    Cleaning,
    Maintenance,
    OutOfOrder
}

// ═══════════════════════════════════════════════════════════════
// ROOM TYPE
// ═══════════════════════════════════════════════════════════════
public class RoomType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxOccupancy { get; set; }
    public decimal BaseRate { get; set; }
    public decimal WeekendRate { get; set; }
    public List<string> Amenities { get; set; } = [];
    public string Image { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

// ═══════════════════════════════════════════════════════════════
// GUEST
// ═══════════════════════════════════════════════════════════════
public class Guest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GuestNumber { get; set; } = string.Empty;
    
    // Personal Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    
    // Identity
    public string IdType { get; set; } = string.Empty; // Passport, DL, NationalID
    public string IdNumber { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    
    // Address
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // Payment
    public string WalletAddress { get; set; } = string.Empty;
    public string PreferredPayment { get; set; } = "Card"; // Card, Cash, Crypto
    
    // Membership
    public GuestType Type { get; set; } = GuestType.Regular;
    public int LoyaltyPoints { get; set; } = 0;
    public string MembershipLevel { get; set; } = "Bronze"; // Bronze, Silver, Gold, Platinum
    
    // Stats
    public int TotalBookings { get; set; } = 0;
    public decimal TotalSpent { get; set; } = 0;
    public DateTime? LastVisit { get; set; }
    
    // Preferences
    public string RoomPreference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = []; // VIP, Corporate, etc.
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsBlacklisted { get; set; } = false;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum GuestType
{
    Regular,
    VIP,
    Corporate,
    Government,
    Diplomatic,
    Influencer
}

// ═══════════════════════════════════════════════════════════════
// RESERVATION
// ═══════════════════════════════════════════════════════════════
public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReservationNumber { get; set; } = string.Empty;
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    
    // Guest
    public Guid GuestId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhone { get; set; } = string.Empty;
    public int Adults { get; set; } = 1;
    public int Children { get; set; } = 0;
    public int TotalGuests => Adults + Children;
    
    // Room
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    
    // Dates
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public TimeSpan CheckInTime { get; set; } = new(14, 0, 0);
    public TimeSpan CheckOutTime { get; set; } = new(11, 0, 0);
    public int Nights => (CheckOutDate - CheckInDate).Days;
    
    // Pricing
    public decimal RoomRate { get; set; }
    public decimal TotalRoomCharges => RoomRate * Nights;
    public decimal AddOnsTotal { get; set; }
    public decimal TaxRate { get; set; } = 16;
    public decimal TaxAmount => (TotalRoomCharges + AddOnsTotal) * (TaxRate / 100);
    public decimal ServiceFee { get; set; }
    public decimal CleaningFee { get; set; }
    public decimal Discount { get; set; }
    public decimal GrandTotal => TotalRoomCharges + AddOnsTotal + TaxAmount + ServiceFee + CleaningFee - Discount;
    public decimal AmountPaid { get; set; }
    public decimal Balance => GrandTotal - AmountPaid;
    public string Currency { get; set; } = "USD";
    
    // Add-ons
    public List<ReservationAddOn> AddOns { get; set; } = [];
    
    // Payment
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;
    public string PaymentReference { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public string TokenUsed { get; set; } = string.Empty; // FHT, USDT, etc.
    
    // Status
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public ReservationSource Source { get; set; } = ReservationSource.Direct;
    
    // Special Requests
    public string SpecialRequests { get; set; } = string.Empty;
    public bool EarlyCheckIn { get; set; } = false;
    public bool LateCheckOut { get; set; } = false;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public enum ReservationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    CheckedOut,
    Cancelled,
    NoShow
}

public enum ReservationSource
{
    Direct,
    WalkIn,
    Phone,
    Website,
    Airbnb,
    Booking,
    Expedia,
    OTA,
    Corporate
}

public enum PaymentMethod
{
    Cash,
    Card,
    BankTransfer,
    Crypto,
    PayPal,
    Invoice
}

// ═══════════════════════════════════════════════════════════════
// ADD-ONS / SERVICES
// ═══════════════════════════════════════════════════════════════
public class AddOn
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Food, Service, Tour, etc.
    public decimal Price { get; set; }
    public PriceType PriceType { get; set; } = PriceType.PerItem;
    public bool IsActive { get; set; } = true;
    public string Image { get; set; } = string.Empty;
}

public enum PriceType
{
    PerItem,
    PerNight,
    PerPerson,
    PerHour,
    Flat
}

public class ReservationAddOn
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ReservationId { get; set; }
    public Guid AddOnId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
}

// ═══════════════════════════════════════════════════════════════
// BOOKING (Confirmed Reservation / Check-In)
// ═══════════════════════════════════════════════════════════════
public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string BookingNumber { get; set; } = string.Empty;
    public Guid ReservationId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid GuestId { get; set; }
    public Guid RoomId { get; set; }
    
    // Guest Info (snapshot)
    public string GuestName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    
    // Dates
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime ActualCheckIn { get; set; }
    public DateTime? ActualCheckOut { get; set; }
    
    // Charges
    public List<BookingCharge> Charges { get; set; } = [];
    public decimal TotalCharges => Charges.Sum(c => c.Amount);
    public decimal TotalPayments { get; set; }
    public decimal Balance => TotalCharges - TotalPayments;
    
    // Room Key
    public string KeyCardNumber { get; set; } = string.Empty;
    public int KeyCardsIssued { get; set; } = 1;
    
    // Status
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public string Notes { get; set; } = string.Empty;
}

public enum BookingStatus
{
    Active,
    CheckedOut,
    Extended,
    Transferred
}

public class BookingCharge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Room, F&B, Minibar, Service, etc.
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string PostedBy { get; set; } = string.Empty;
}

// ═══════════════════════════════════════════════════════════════
// PAYMENT TRANSACTION
// ═══════════════════════════════════════════════════════════════
public class HotelPayment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TransactionNumber { get; set; } = string.Empty;
    public Guid PropertyId { get; set; }
    public Guid? ReservationId { get; set; }
    public Guid? BookingId { get; set; }
    public Guid GuestId { get; set; }
    
    // Amount
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Crypto
    public decimal? CryptoAmount { get; set; }
    public string? TokenSymbol { get; set; }
    public string? TxHash { get; set; }
    public string? WalletFrom { get; set; }
    public string? WalletTo { get; set; }
    
    // Payment Info
    public PaymentMethod Method { get; set; }
    public PaymentType Type { get; set; } = PaymentType.Payment;
    public string Reference { get; set; } = string.Empty;
    public string CardLast4 { get; set; } = string.Empty;
    
    // Status
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public string Notes { get; set; } = string.Empty;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ProcessedBy { get; set; } = string.Empty;
}

public enum PaymentType
{
    Payment,
    Deposit,
    Refund,
    Chargeback
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

// ═══════════════════════════════════════════════════════════════
// HOUSEKEEPING
// ═══════════════════════════════════════════════════════════════
public class HousekeepingTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    
    public TaskType TaskType { get; set; } = TaskType.Cleaning;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    
    public string AssignedTo { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CompletedBy { get; set; } = string.Empty;
    
    public List<string> Checklist { get; set; } = [];
    public List<string> CompletedItems { get; set; } = [];
}

public enum TaskType
{
    Cleaning,
    DeepClean,
    TurnDown,
    Inspection,
    Maintenance,
    Repair
}

public enum TaskPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

// ═══════════════════════════════════════════════════════════════
// REAL ESTATE LISTING
// ═══════════════════════════════════════════════════════════════
public class RealEstateListing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ListingNumber { get; set; } = string.Empty;
    public Guid PropertyId { get; set; }
    
    // Listing Info
    public ListingType Type { get; set; } = ListingType.ForSale;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Pricing
    public decimal Price { get; set; }
    public decimal? PricePerSqFt { get; set; }
    public string Currency { get; set; } = "USD";
    public bool AcceptsCrypto { get; set; } = true;
    public decimal? CryptoPrice { get; set; }
    public string CryptoToken { get; set; } = "FHT"; // Futurehead Trust Coin
    
    // Property Details
    public decimal Size { get; set; }
    public string SizeUnit { get; set; } = "sqft";
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Parking { get; set; }
    public int YearBuilt { get; set; }
    public string LotSize { get; set; } = string.Empty;
    
    // Features
    public List<string> Features { get; set; } = [];
    public List<string> Images { get; set; } = [];
    public string VirtualTourUrl { get; set; } = string.Empty;
    
    // Agent
    public Guid AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string AgentPhone { get; set; } = string.Empty;
    public string AgentEmail { get; set; } = string.Empty;
    
    // Status
    public ListingStatus Status { get; set; } = ListingStatus.Active;
    public DateTime ListedDate { get; set; } = DateTime.UtcNow;
    public DateTime? SoldDate { get; set; }
    public decimal? SoldPrice { get; set; }
    
    // Stats
    public int Views { get; set; } = 0;
    public int Inquiries { get; set; } = 0;
    public int SavedCount { get; set; } = 0;
}

public enum ListingType
{
    ForSale,
    ForRent,
    Auction,
    ShortSale
}

public enum ListingStatus
{
    Draft,
    Active,
    Pending,
    UnderContract,
    Sold,
    Rented,
    Withdrawn,
    Expired
}

// ═══════════════════════════════════════════════════════════════
// FUTUREHEAD TRUST INTEGRATION
// ═══════════════════════════════════════════════════════════════
public class FutureheadTrustTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TxHash { get; set; } = string.Empty;
    
    // Transaction Type
    public FHTTransactionType Type { get; set; }
    
    // Related Entity
    public Guid? ReservationId { get; set; }
    public Guid? ListingId { get; set; }
    public Guid? PropertyId { get; set; }
    
    // Amount
    public decimal FHTAmount { get; set; }
    public decimal USDValue { get; set; }
    public decimal ExchangeRate { get; set; }
    
    // Wallets
    public string FromWallet { get; set; } = string.Empty;
    public string ToWallet { get; set; } = string.Empty;
    
    // Status
    public FHTTxStatus Status { get; set; } = FHTTxStatus.Pending;
    public int Confirmations { get; set; } = 0;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}

public enum FHTTransactionType
{
    BookingPayment,
    PropertyDeposit,
    PropertyPurchase,
    RentalPayment,
    Refund,
    TokenizedOwnership
}

public enum FHTTxStatus
{
    Pending,
    Confirmed,
    Failed,
    Refunded
}

// ═══════════════════════════════════════════════════════════════
// REPORTS
// ═══════════════════════════════════════════════════════════════
public class HotelDashboard
{
    public Guid PropertyId { get; set; }
    public DateTime Date { get; set; }
    
    // Occupancy
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public decimal OccupancyRate => TotalRooms > 0 ? (OccupiedRooms * 100m / TotalRooms) : 0;
    
    // Revenue
    public decimal TodayRevenue { get; set; }
    public decimal WeekRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public decimal ADR { get; set; } // Average Daily Rate
    public decimal RevPAR { get; set; } // Revenue Per Available Room
    
    // Reservations
    public int TodayCheckIns { get; set; }
    public int TodayCheckOuts { get; set; }
    public int PendingReservations { get; set; }
    
    // Crypto
    public decimal CryptoRevenue { get; set; }
    public decimal FHTRevenue { get; set; }
}

public class OccupancyReport
{
    public Guid PropertyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<DailyOccupancy> Days { get; set; } = [];
    public decimal AverageOccupancy { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class DailyOccupancy
{
    public DateTime Date { get; set; }
    public int TotalRooms { get; set; }
    public int Occupied { get; set; }
    public decimal OccupancyRate { get; set; }
    public decimal Revenue { get; set; }
    public decimal ADR { get; set; }
}

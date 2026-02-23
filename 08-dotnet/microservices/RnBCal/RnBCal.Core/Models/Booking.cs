namespace RnBCal.Core.Models;

/// <summary>
/// Rental & Booking Order Model
/// IERAHKWA Calendar Sync System
/// </summary>
public class Booking
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    
    // Booking Details
    public BookingType Type { get; set; }
    public BookingStatus Status { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Item Details
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // Car, Bike, Yacht, Hotel, Airbnb, etc.
    
    // Pricing
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public Dictionary<string, string> CustomFields { get; set; } = new();
    
    // IERAHKWA Integration
    public string? IgtTokenId { get; set; }
    public bool PaidWithIGT { get; set; }
    public string? IerahkwaTransactionId { get; set; }
}

public enum BookingType
{
    CarRental,
    BikeRental,
    YachtRental,
    HotelRoom,
    AirbnbProperty,
    EquipmentRental,
    DressRental,
    Other
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    Refunded
}

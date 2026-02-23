using NET10.Core.Interfaces;
using NET10.Core.Models.Hotel;
using RoomType = NET10.Core.Models.Hotel.RoomType;
using RoomStatus = NET10.Core.Models.Hotel.RoomStatus;

namespace NET10.Infrastructure.Services.Hotel;

public class ReservationService : IReservationService
{
    private static readonly List<Reservation> _reservations = [];
    private static readonly List<Booking> _bookings = [];
    private static int _reservationCounter = 1000;
    private static int _bookingCounter = 1000;
    
    public Task<List<Reservation>> GetByPropertyAsync(Guid propertyId)
    {
        return Task.FromResult(_reservations
            .Where(r => r.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList());
    }
    
    public Task<List<Reservation>> GetByDateRangeAsync(Guid propertyId, DateTime from, DateTime to)
    {
        return Task.FromResult(_reservations
            .Where(r => r.PropertyId == propertyId &&
                       r.CheckInDate >= from && r.CheckInDate <= to)
            .OrderBy(r => r.CheckInDate)
            .ToList());
    }
    
    public Task<Reservation?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_reservations.FirstOrDefault(r => r.Id == id));
    }
    
    public Task<Reservation?> GetByNumberAsync(string reservationNumber)
    {
        return Task.FromResult(_reservations.FirstOrDefault(r => 
            r.ReservationNumber.Equals(reservationNumber, StringComparison.OrdinalIgnoreCase)));
    }
    
    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        reservation.Id = Guid.NewGuid();
        reservation.ReservationNumber = await GenerateReservationNumberAsync();
        reservation.CreatedAt = DateTime.UtcNow;
        reservation.Status = ReservationStatus.Pending;
        
        // Calculate pricing
        var pricing = await CalculatePricingAsync(
            reservation.RoomId, 
            reservation.CheckInDate, 
            reservation.CheckOutDate, 
            reservation.TotalGuests);
        
        reservation.RoomRate = pricing.RoomRate;
        reservation.ServiceFee = pricing.ServiceFee;
        reservation.CleaningFee = pricing.CleaningFee;
        
        _reservations.Add(reservation);
        return reservation;
    }
    
    public Task<Reservation> UpdateAsync(Reservation reservation)
    {
        var index = _reservations.FindIndex(r => r.Id == reservation.Id);
        if (index >= 0)
        {
            _reservations[index] = reservation;
        }
        return Task.FromResult(reservation);
    }
    
    public Task<bool> CancelAsync(Guid id, string reason)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == id);
        if (reservation != null)
        {
            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelledAt = DateTime.UtcNow;
            reservation.CancellationReason = reason;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<Reservation> ConfirmAsync(Guid id)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == id);
        if (reservation != null)
        {
            reservation.Status = ReservationStatus.Confirmed;
            reservation.ConfirmedAt = DateTime.UtcNow;
        }
        return Task.FromResult(reservation!);
    }
    
    public async Task<Booking> CheckInAsync(Guid reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation == null)
            throw new InvalidOperationException("Reservation not found");
        
        reservation.Status = ReservationStatus.CheckedIn;
        reservation.CheckedInAt = DateTime.UtcNow;
        
        var booking = new Booking
        {
            BookingNumber = $"BK-{++_bookingCounter:D6}",
            ReservationId = reservation.Id,
            PropertyId = reservation.PropertyId,
            GuestId = reservation.GuestId,
            RoomId = reservation.RoomId,
            GuestName = reservation.GuestName,
            RoomNumber = reservation.RoomNumber,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            ActualCheckIn = DateTime.UtcNow,
            Status = BookingStatus.Active,
            Charges = new List<BookingCharge>
            {
                new BookingCharge
                {
                    Description = "Room Charges",
                    Category = "Room",
                    Amount = reservation.TotalRoomCharges
                },
                new BookingCharge
                {
                    Description = "Tax",
                    Category = "Tax",
                    Amount = reservation.TaxAmount
                }
            },
            TotalPayments = reservation.AmountPaid
        };
        
        _bookings.Add(booking);
        return booking;
    }
    
    public Task<Booking> CheckOutAsync(Guid bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking != null)
        {
            booking.Status = BookingStatus.CheckedOut;
            booking.ActualCheckOut = DateTime.UtcNow;
            
            var reservation = _reservations.FirstOrDefault(r => r.Id == booking.ReservationId);
            if (reservation != null)
            {
                reservation.Status = ReservationStatus.CheckedOut;
                reservation.CheckedOutAt = DateTime.UtcNow;
            }
        }
        return Task.FromResult(booking!);
    }
    
    public Task<string> GenerateReservationNumberAsync()
    {
        _reservationCounter++;
        return Task.FromResult($"RES-{_reservationCounter:D6}");
    }
    
    public Task<List<Reservation>> GetTodayCheckInsAsync(Guid propertyId)
    {
        var today = DateTime.UtcNow.Date;
        return Task.FromResult(_reservations
            .Where(r => r.PropertyId == propertyId &&
                       r.CheckInDate.Date == today &&
                       r.Status == ReservationStatus.Confirmed)
            .ToList());
    }
    
    public Task<List<Reservation>> GetTodayCheckOutsAsync(Guid propertyId)
    {
        var today = DateTime.UtcNow.Date;
        return Task.FromResult(_reservations
            .Where(r => r.PropertyId == propertyId &&
                       r.CheckOutDate.Date == today &&
                       r.Status == ReservationStatus.CheckedIn)
            .ToList());
    }
    
    public Task<ReservationPricing> CalculatePricingAsync(Guid roomId, DateTime checkIn, DateTime checkOut, int guests)
    {
        // Demo pricing logic
        var nights = (checkOut - checkIn).Days;
        var baseRate = 200m; // Would get from room
        
        var pricing = new ReservationPricing
        {
            RoomRate = baseRate,
            Nights = nights,
            RoomTotal = baseRate * nights,
            TaxAmount = baseRate * nights * 0.16m,
            ServiceFee = 25m,
            CleaningFee = 50m,
            GrandTotal = (baseRate * nights * 1.16m) + 75m,
            FHTEquivalent = ((baseRate * nights * 1.16m) + 75m) / 2.5m // Demo FHT rate
        };
        
        return Task.FromResult(pricing);
    }
}

public class GuestService : IGuestService
{
    private static readonly List<Guest> _guests = [];
    private static int _guestCounter = 10000;
    
    public Task<List<Guest>> GetAllAsync()
    {
        return Task.FromResult(_guests.Where(g => g.IsActive).ToList());
    }
    
    public Task<Guest?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_guests.FirstOrDefault(g => g.Id == id));
    }
    
    public Task<Guest?> GetByEmailAsync(string email)
    {
        return Task.FromResult(_guests.FirstOrDefault(g => 
            g.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }
    
    public async Task<Guest> CreateAsync(Guest guest)
    {
        guest.Id = Guid.NewGuid();
        guest.GuestNumber = await GenerateGuestNumberAsync();
        guest.CreatedAt = DateTime.UtcNow;
        _guests.Add(guest);
        return guest;
    }
    
    public Task<Guest> UpdateAsync(Guest guest)
    {
        var index = _guests.FindIndex(g => g.Id == guest.Id);
        if (index >= 0) _guests[index] = guest;
        return Task.FromResult(guest);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var guest = _guests.FirstOrDefault(g => g.Id == id);
        if (guest != null)
        {
            guest.IsActive = false;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<List<Guest>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return Task.FromResult(_guests
            .Where(g => g.IsActive && (
                g.FirstName.ToLower().Contains(term) ||
                g.LastName.ToLower().Contains(term) ||
                g.Email.ToLower().Contains(term) ||
                g.Phone.Contains(term)))
            .ToList());
    }
    
    public Task<Guest> AddLoyaltyPointsAsync(Guid guestId, int points)
    {
        var guest = _guests.FirstOrDefault(g => g.Id == guestId);
        if (guest != null)
        {
            guest.LoyaltyPoints += points;
            
            // Update membership level
            guest.MembershipLevel = guest.LoyaltyPoints switch
            {
                >= 10000 => "Platinum",
                >= 5000 => "Gold",
                >= 1000 => "Silver",
                _ => "Bronze"
            };
        }
        return Task.FromResult(guest!);
    }
    
    public Task<string> GenerateGuestNumberAsync()
    {
        return Task.FromResult($"GST-{++_guestCounter:D6}");
    }
}

public class RoomService : IRoomService
{
    private static readonly List<Room> _rooms = [];
    private static readonly List<RoomType> _roomTypes = InitializeRoomTypes();
    
    private static List<RoomType> InitializeRoomTypes()
    {
        return new List<RoomType>
        {
            new RoomType { Code = "STD", Name = "Standard Room", MaxOccupancy = 2, BaseRate = 150, Description = "Comfortable room with queen bed" },
            new RoomType { Code = "DLX", Name = "Deluxe Room", MaxOccupancy = 3, BaseRate = 220, Description = "Spacious room with king bed and city view" },
            new RoomType { Code = "STE", Name = "Suite", MaxOccupancy = 4, BaseRate = 350, Description = "Luxury suite with living area" },
            new RoomType { Code = "PEN", Name = "Penthouse", MaxOccupancy = 6, BaseRate = 800, Description = "Ultimate luxury with panoramic views" }
        };
    }
    
    public Task<List<Room>> GetByPropertyAsync(Guid propertyId)
    {
        return Task.FromResult(_rooms.Where(r => r.PropertyId == propertyId && r.IsActive).ToList());
    }
    
    public Task<Room?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
    }
    
    public Task<Room> CreateAsync(Room room)
    {
        room.Id = Guid.NewGuid();
        _rooms.Add(room);
        return Task.FromResult(room);
    }
    
    public Task<Room> UpdateAsync(Room room)
    {
        var index = _rooms.FindIndex(r => r.Id == room.Id);
        if (index >= 0) _rooms[index] = room;
        return Task.FromResult(room);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == id);
        if (room != null)
        {
            room.IsActive = false;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<List<Room>> GetAvailableAsync(Guid propertyId, DateTime checkIn, DateTime checkOut)
    {
        return Task.FromResult(_rooms
            .Where(r => r.PropertyId == propertyId && 
                       r.IsActive && 
                       r.Status == RoomStatus.Available)
            .ToList());
    }
    
    public Task<Room> UpdateStatusAsync(Guid roomId, RoomStatus status)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room != null) room.Status = status;
        return Task.FromResult(room!);
    }
    
    public Task<List<RoomType>> GetRoomTypesAsync(Guid propertyId)
    {
        return Task.FromResult(_roomTypes.Where(rt => rt.IsActive).ToList());
    }
    
    public Task<RoomType> CreateRoomTypeAsync(RoomType roomType)
    {
        roomType.Id = Guid.NewGuid();
        _roomTypes.Add(roomType);
        return Task.FromResult(roomType);
    }
    
    public Task<RoomType> UpdateRoomTypeAsync(RoomType roomType)
    {
        var index = _roomTypes.FindIndex(rt => rt.Id == roomType.Id);
        if (index >= 0) _roomTypes[index] = roomType;
        return Task.FromResult(roomType);
    }
}

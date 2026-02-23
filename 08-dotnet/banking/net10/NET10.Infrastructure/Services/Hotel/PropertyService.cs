using NET10.Core.Interfaces;
using NET10.Core.Models.Hotel;

namespace NET10.Infrastructure.Services.Hotel;

public class PropertyService : IPropertyService
{
    private static readonly List<Property> _properties = InitializeProperties();
    
    private static List<Property> InitializeProperties()
    {
        return new List<Property>
        {
            new Property
            {
                Name = "Ierahkwa Grand Hotel",
                Code = "IGH-001",
                Type = PropertyType.Hotel,
                Description = "Luxury 5-star hotel in the heart of the sovereign territory",
                Address = "1 Sovereignty Plaza",
                City = "Ierahkwa City",
                State = "Kanienke",
                Country = "Ierahkwa",
                Phone = "+1-777-IGT-1001",
                Email = "reservations@ierahkwa-grand.gov",
                Rating = 4.9m,
                ReviewCount = 1250,
                BasePrice = 250,
                MaxGuests = 500,
                Bedrooms = 200,
                Bathrooms = 200,
                IsFeatured = true,
                AcceptsCrypto = true,
                AcceptedTokens = ["FHT", "USDT", "IGT", "ETH", "BTC"],
                Amenities = ["Pool", "Spa", "Gym", "Restaurant", "Bar", "Casino", "Conference", "Helipad"]
            },
            new Property
            {
                Name = "Futurehead Beach Villa",
                Code = "FBV-001",
                Type = PropertyType.Villa,
                Description = "Exclusive beachfront villa with private access",
                Address = "100 Ocean Drive",
                City = "Paradise Bay",
                State = "Coastal",
                Country = "Ierahkwa",
                BasePrice = 1500,
                MaxGuests = 12,
                Bedrooms = 6,
                Bathrooms = 7,
                IsFeatured = true,
                AcceptsCrypto = true,
                ForSale = true,
                SalePrice = 5000000,
                PropertySize = 8500,
                Amenities = ["Private Beach", "Pool", "Hot Tub", "Chef Kitchen", "Wine Cellar", "Home Theater"]
            },
            new Property
            {
                Name = "Downtown Luxury Apartment",
                Code = "DLA-001",
                Type = PropertyType.Apartment,
                Description = "Modern apartment in downtown with city views",
                Address = "500 Financial District",
                City = "Ierahkwa City",
                State = "Kanienke",
                Country = "Ierahkwa",
                BasePrice = 180,
                MaxGuests = 4,
                Bedrooms = 2,
                Bathrooms = 2,
                AcceptsCrypto = true,
                ForRent = true,
                ForSale = true,
                SalePrice = 850000,
                PropertySize = 1200,
                Amenities = ["Gym", "Rooftop", "Concierge", "Parking", "Smart Home"]
            }
        };
    }
    
    public Task<List<Property>> GetAllAsync()
    {
        return Task.FromResult(_properties.Where(p => p.IsActive).ToList());
    }
    
    public Task<List<Property>> GetByTypeAsync(PropertyType type)
    {
        return Task.FromResult(_properties.Where(p => p.Type == type && p.IsActive).ToList());
    }
    
    public Task<Property?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_properties.FirstOrDefault(p => p.Id == id));
    }
    
    public Task<Property> CreateAsync(Property property)
    {
        property.Id = Guid.NewGuid();
        property.CreatedAt = DateTime.UtcNow;
        _properties.Add(property);
        return Task.FromResult(property);
    }
    
    public Task<Property> UpdateAsync(Property property)
    {
        var index = _properties.FindIndex(p => p.Id == property.Id);
        if (index >= 0)
        {
            property.UpdatedAt = DateTime.UtcNow;
            _properties[index] = property;
        }
        return Task.FromResult(property);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var property = _properties.FirstOrDefault(p => p.Id == id);
        if (property != null)
        {
            property.IsActive = false;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<List<Property>> SearchAsync(PropertySearchCriteria criteria)
    {
        var query = _properties.Where(p => p.IsActive);
        
        if (!string.IsNullOrEmpty(criteria.City))
            query = query.Where(p => p.City.Contains(criteria.City, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(criteria.Country))
            query = query.Where(p => p.Country.Contains(criteria.Country, StringComparison.OrdinalIgnoreCase));
        
        if (criteria.Type.HasValue)
            query = query.Where(p => p.Type == criteria.Type.Value);
        
        if (criteria.MinPrice.HasValue)
            query = query.Where(p => p.BasePrice >= criteria.MinPrice.Value);
        
        if (criteria.MaxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= criteria.MaxPrice.Value);
        
        if (criteria.MinBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms >= criteria.MinBedrooms.Value);
        
        if (criteria.Guests.HasValue)
            query = query.Where(p => p.MaxGuests >= criteria.Guests.Value);
        
        if (criteria.AcceptsCrypto.HasValue)
            query = query.Where(p => p.AcceptsCrypto == criteria.AcceptsCrypto.Value);
        
        return Task.FromResult(query.ToList());
    }
    
    public Task<List<Property>> GetFeaturedAsync()
    {
        return Task.FromResult(_properties.Where(p => p.IsFeatured && p.IsActive).ToList());
    }
}

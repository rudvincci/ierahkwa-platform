using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using System.Text.Json;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.Citizens.Core.Mongo.Documents;

internal class CitizenDocument : IIdentifiable<Guid>
{
    public CitizenDocument()
    {
    }

    public CitizenDocument(Citizen citizen)
    {
        Id = citizen.Id.Value;
        TenantId = citizen.TenantId.Value;
        FirstName = citizen.FirstName; // Already a string from convenience property
        LastName = citizen.LastName;   // Already a string from convenience property
        Email = citizen.Email?.ToString();
        Phone = citizen.Phone?.ToString();
        AddressJson = citizen.Address != null ? JsonSerializer.Serialize(citizen.Address) : null;
        DateOfBirth = citizen.DateOfBirth;
        Status = citizen.Status.ToString();
        PhotoPath = citizen.PhotoPath;
        CreatedAt = citizen.CreatedAt;
        UpdatedAt = citizen.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AddressJson { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Citizen AsEntity()
    {
        var citizenId = new CitizenId(Id);
        var tenantId = new GovTenantId(TenantId);
        // Name combines firstName and lastName into a single object
        var citizenName = new Mamey.Types.Name(FirstName, LastName);
        var email = string.IsNullOrEmpty(Email) ? null : new Mamey.Types.Email(Email);
        // Phone requires countryCode and number - parse from stored string or use defaults
        Mamey.Types.Phone? phone = null;
        if (!string.IsNullOrEmpty(Phone))
        {
            // Assume format "+1 1234567890" or just store the raw value
            phone = new Mamey.Types.Phone("1", Phone.Replace("+", "").Replace(" ", ""));
        }
        var address = string.IsNullOrEmpty(AddressJson) ? null : JsonSerializer.Deserialize<Mamey.Types.Address>(AddressJson);
        var status = Enum.Parse<CitizenshipStatus>(Status);
        
        var citizen = new Citizen(
            citizenId,
            tenantId,
            citizenName,
            email,
            phone,
            address,
            DateOfBirth,
            status);
        
        typeof(Citizen).GetProperty("PhotoPath")?.SetValue(citizen, PhotoPath);
        typeof(Citizen).GetProperty("CreatedAt")?.SetValue(citizen, CreatedAt);
        typeof(Citizen).GetProperty("UpdatedAt")?.SetValue(citizen, UpdatedAt);
        
        return citizen;
    }
}

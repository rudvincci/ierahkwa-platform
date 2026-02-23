using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using System.Text.Json;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Mongo.Documents;

internal class ApplicationDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public ApplicationDocument()
    {
    }

    public ApplicationDocument(CitizenshipApplication application)
    {
        Id = application.Id.Value;
        TenantId = application.TenantId.Value;
        ApplicationNumber = application.ApplicationNumber.Value;
        FirstName = application.FirstName;
        LastName = application.LastName;
        DateOfBirth = application.DateOfBirth;
        Email = application.Email?.Value;
        Phone = application.Phone?.ToString();
        AddressJson = application.Address != null ? JsonSerializer.Serialize(application.Address) : null;
        Status = application.Status.ToString();
        RejectionReason = application.RejectionReason;
        ExtendedDataJson = application.ExtendedDataJson;
        AccessLogsJson = application.AccessLogsJson;
        CreatedAt = application.CreatedAt;
        UpdatedAt = application.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AddressJson { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string? ExtendedDataJson { get; set; }
    public string? AccessLogsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CitizenshipApplication AsEntity()
    {
        var applicationId = new AppId(Id);
        var tenantId = new TenantId(TenantId);
        var applicationNumber = new ApplicationNumber(ApplicationNumber);
        var applicantName = new Name(FirstName, LastName);
        var email = string.IsNullOrEmpty(Email) ? null : new Email(Email);
        var phone = ParsePhone(Phone);
        var address = string.IsNullOrEmpty(AddressJson) ? null : JsonSerializer.Deserialize<Address>(AddressJson);
        var status = Enum.Parse<ApplicationStatus>(Status);
        
        var application = new CitizenshipApplication(
            applicationId,
            tenantId,
            applicationNumber,
            applicantName,
            DateOfBirth,
            email,
            status);
        
        if (phone != null)
            typeof(CitizenshipApplication).GetProperty("Phone")?.SetValue(application, phone);
        if (address != null)
            typeof(CitizenshipApplication).GetProperty("Address")?.SetValue(application, address);
        typeof(CitizenshipApplication).GetProperty("RejectionReason")?.SetValue(application, RejectionReason);
        typeof(CitizenshipApplication).GetProperty("ExtendedDataJson")?.SetValue(application, ExtendedDataJson);
        typeof(CitizenshipApplication).GetProperty("AccessLogsJson")?.SetValue(application, AccessLogsJson);
        typeof(CitizenshipApplication).GetProperty("CreatedAt")?.SetValue(application, CreatedAt);
        typeof(CitizenshipApplication).GetProperty("UpdatedAt")?.SetValue(application, UpdatedAt);
        
        return application;
    }

    private static Phone? ParsePhone(string? phoneString)
    {
        if (string.IsNullOrEmpty(phoneString)) return null;
        
        // Try to parse phone string in format "+1 1234567890"
        if (phoneString.StartsWith("+"))
        {
            var parts = phoneString.Split(' ', 2);
            if (parts.Length == 2)
            {
                return new Phone(parts[0].TrimStart('+'), parts[1]);
            }
        }
        // Default: assume US country code
        return new Phone("1", phoneString);
    }
}

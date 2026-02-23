using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Types;

namespace Mamey.Government.Modules.Passports.Core.Mongo.Documents;

internal class PassportDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public PassportDocument()
    {
    }

    public PassportDocument(Passport passport)
    {
        Id = passport.Id.Value;
        TenantId = passport.TenantId.Value;
        CitizenId = passport.CitizenId;
        PassportNumber = passport.PassportNumber.Value;
        IssuedDate = passport.IssuedDate;
        ExpiryDate = passport.ExpiryDate;
        Mrz = passport.Mrz;
        DocumentPath = passport.DocumentPath;
        IsActive = passport.IsActive;
        RevokedAt = passport.RevokedAt;
        RevocationReason = passport.RevocationReason;
        CreatedAt = passport.CreatedAt;
        UpdatedAt = passport.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Mrz { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Passport AsEntity()
    {
        var passportId = new Domain.ValueObjects.PassportId(Id);
        var tenantId = new TenantId(TenantId);
        var citizenId = CitizenId;
        var passportNumber = new Domain.ValueObjects.PassportNumber(PassportNumber);
        
        var passport = new Passport(
            passportId,
            tenantId,
            citizenId,
            passportNumber,
            IssuedDate,
            ExpiryDate,
            Mrz);
        
        typeof(Passport).GetProperty("DocumentPath")?.SetValue(passport, DocumentPath);
        typeof(Passport).GetProperty("IsActive")?.SetValue(passport, IsActive);
        typeof(Passport).GetProperty("RevokedAt")?.SetValue(passport, RevokedAt);
        typeof(Passport).GetProperty("RevocationReason")?.SetValue(passport, RevocationReason);
        typeof(Passport).GetProperty("CreatedAt")?.SetValue(passport, CreatedAt);
        typeof(Passport).GetProperty("UpdatedAt")?.SetValue(passport, UpdatedAt);
        
        return passport;
    }
}

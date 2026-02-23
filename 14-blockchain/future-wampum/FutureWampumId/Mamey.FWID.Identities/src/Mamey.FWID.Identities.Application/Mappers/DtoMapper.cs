using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Mappers;

/// <summary>
/// Maps DTOs to domain value objects and vice versa.
/// </summary>
public static class DtoMapper
{
    /// <summary>
    /// Maps a PersonalDetailsDto to a PersonalDetails domain value object.
    /// </summary>
    public static PersonalDetails ToDomain(this PersonalDetailsDto dto)
    {
        if (dto == null) return null!;
        return new PersonalDetails(
            dto.DateOfBirth,
            dto.PlaceOfBirth,
            dto.Gender,
            dto.ClanAffiliation);
    }

    /// <summary>
    /// Maps a ContactInformationDto to a ContactInformation domain value object.
    /// </summary>
    public static ContactInformation ToDomain(this ContactInformationDto dto)
    {
        if (dto == null) return null!;
        
        // Use the simple constructor with just email
        var contactInfo = new ContactInformation(dto.Email);
        
        // Note: Address and Phone mapping would require more complex Mamey.Types construction
        // For now, we only map email. Full address mapping can be added when needed.
        
        return contactInfo;
    }

    /// <summary>
    /// Maps a BiometricDataDto to a BiometricData domain value object.
    /// </summary>
    public static BiometricData ToDomain(this BiometricDataDto dto)
    {
        if (dto == null) return null!;
        
        // Convert base64 string to byte array
        byte[] encryptedTemplate = Array.Empty<byte>();
        if (!string.IsNullOrEmpty(dto.EncryptedTemplate))
            encryptedTemplate = Convert.FromBase64String(dto.EncryptedTemplate);
        
        // Map BiometricType enum
        var type = (BiometricType)(int)dto.Type;
        
        // Map BiometricQuality enum
        var quality = (BiometricQuality)(int)dto.Quality;
        
        return new BiometricData(
            type,
            encryptedTemplate,
            dto.Hash ?? string.Empty,
            dto.TemplateId,
            dto.AlgoVersion,
            dto.Format,
            quality,
            dto.EvidenceJws,
            dto.LivenessScore,
            dto.LivenessDecision);
    }

    /// <summary>
    /// Maps a MfaMethod from Contracts to Domain.
    /// </summary>
    public static MfaMethod ToDomain(this Contracts.MfaMethod method)
    {
        return (MfaMethod)(int)method;
    }

    /// <summary>
    /// Maps a PersonalDetails domain value object to a PersonalDetailsDto.
    /// </summary>
    public static PersonalDetailsDto ToDto(this PersonalDetails domain)
    {
        if (domain == null) return null!;
        return new PersonalDetailsDto
        {
            DateOfBirth = domain.DateOfBirth,
            PlaceOfBirth = domain.PlaceOfBirth,
            Gender = domain.Gender,
            ClanAffiliation = domain.ClanAffiliation
        };
    }

    /// <summary>
    /// Maps a ContactInformation domain value object to a ContactInformationDto.
    /// </summary>
    public static ContactInformationDto ToDto(this ContactInformation domain)
    {
        if (domain == null) return null!;
        return new ContactInformationDto
        {
            Email = domain.Email?.Value,
            Street = domain.Address?.Line,
            City = domain.Address?.City,
            State = domain.Address?.State,
            PostalCode = domain.Address?.Zip5,
            Country = domain.Address?.Country,
            PhoneNumbers = domain.PhoneNumbers?.Select(p => $"{p.CountryCode}{p.Number}").ToList()
        };
    }

    /// <summary>
    /// Maps a BiometricData domain value object to a BiometricDataDto.
    /// </summary>
    public static BiometricDataDto ToDto(this BiometricData domain)
    {
        if (domain == null) return null!;
        return new BiometricDataDto
        {
            Type = (Contracts.BiometricType)(int)domain.Type,
            EncryptedTemplate = Convert.ToBase64String(domain.EncryptedTemplate),
            Hash = domain.Hash,
            TemplateId = domain.TemplateId,
            AlgoVersion = domain.AlgoVersion,
            Format = domain.Format,
            Quality = (Contracts.BiometricQuality)(int)domain.Quality,
            EvidenceJws = domain.EvidenceJws,
            LivenessScore = domain.LivenessScore,
            LivenessDecision = domain.LivenessDecision,
            CapturedAt = domain.CapturedAt
        };
    }
}

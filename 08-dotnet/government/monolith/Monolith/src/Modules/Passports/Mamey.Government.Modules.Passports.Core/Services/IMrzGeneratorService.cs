using System;

namespace Mamey.Government.Modules.Passports.Core.Services;

/// <summary>
/// Service for generating ICAO Machine Readable Zone (MRZ) data for passports.
/// </summary>
public interface IMrzGeneratorService
{
    /// <summary>
    /// Generates MRZ lines for a TD3 (passport) format document.
    /// </summary>
    MrzData GenerateTd3Mrz(MrzInput input);
    
    /// <summary>
    /// Validates MRZ check digits.
    /// </summary>
    bool ValidateMrz(string mrzLine1, string mrzLine2);
}

public record MrzInput(
    string DocumentCode,
    string IssuingState,
    string Surname,
    string GivenNames,
    string PassportNumber,
    string Nationality,
    DateTime DateOfBirth,
    string Sex,
    DateTime ExpirationDate,
    string? PersonalNumber = null);

public record MrzData(
    string Line1,
    string Line2,
    string FullMrz);

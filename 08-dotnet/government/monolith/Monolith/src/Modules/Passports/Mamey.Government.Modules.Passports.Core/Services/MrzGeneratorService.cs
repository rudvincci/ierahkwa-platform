using System;
using System.Text;

namespace Mamey.Government.Modules.Passports.Core.Services;

/// <summary>
/// ICAO 9303 compliant MRZ generator for TD3 (passport) documents.
/// </summary>
internal sealed class MrzGeneratorService : IMrzGeneratorService
{
    private const int Td3LineLength = 44;
    private const string Filler = "<";

    public MrzData GenerateTd3Mrz(MrzInput input)
    {
        // Line 1: P<ISSSUERNAME<<GIVENNAMES<<<<<<<<<<<<<<<<<<<<
        var line1 = GenerateLine1(input);
        
        // Line 2: PASSPORTNOCHECKBIRTHSEXEXPIRCHECKNATIONALITYCHECKOVERALL
        var line2 = GenerateLine2(input);
        
        return new MrzData(line1, line2, $"{line1}\n{line2}");
    }

    public bool ValidateMrz(string mrzLine1, string mrzLine2)
    {
        if (mrzLine1.Length != Td3LineLength || mrzLine2.Length != Td3LineLength)
            return false;

        // Validate individual check digits
        var passportNumber = mrzLine2[..9];
        var passportCheck = mrzLine2[9];
        if (CalculateCheckDigit(passportNumber) != passportCheck)
            return false;

        var birthDate = mrzLine2[13..19];
        var birthCheck = mrzLine2[19];
        if (CalculateCheckDigit(birthDate) != birthCheck)
            return false;

        var expiryDate = mrzLine2[21..27];
        var expiryCheck = mrzLine2[27];
        if (CalculateCheckDigit(expiryDate) != expiryCheck)
            return false;

        return true;
    }

    private static string GenerateLine1(MrzInput input)
    {
        var sb = new StringBuilder(Td3LineLength);
        
        // Document type (P for passport)
        sb.Append(input.DocumentCode.Length == 1 ? input.DocumentCode + Filler : input.DocumentCode[..2]);
        
        // Issuing state (3 chars)
        sb.Append(NormalizeName(input.IssuingState, 3));
        
        // Name section (surname << given names)
        var surname = NormalizeName(input.Surname.ToUpperInvariant());
        var givenNames = NormalizeName(input.GivenNames.ToUpperInvariant());
        var namePart = $"{surname}{Filler}{Filler}{givenNames}";
        
        if (namePart.Length > 39)
            namePart = namePart[..39];
        else
            namePart = namePart.PadRight(39, '<');
        
        sb.Append(namePart);
        
        return sb.ToString();
    }

    private static string GenerateLine2(MrzInput input)
    {
        var sb = new StringBuilder(Td3LineLength);
        
        // Passport number (9 chars) + check digit
        var passportNum = input.PassportNumber.ToUpperInvariant().PadRight(9, '<')[..9];
        sb.Append(passportNum);
        sb.Append(CalculateCheckDigit(passportNum));
        
        // Nationality (3 chars)
        sb.Append(NormalizeName(input.Nationality, 3));
        
        // Date of birth (YYMMDD) + check digit
        var dob = FormatDate(input.DateOfBirth);
        sb.Append(dob);
        sb.Append(CalculateCheckDigit(dob));
        
        // Sex (M, F, or <)
        sb.Append(input.Sex.ToUpperInvariant() switch
        {
            "M" or "MALE" => "M",
            "F" or "FEMALE" => "F",
            _ => Filler
        });
        
        // Expiration date (YYMMDD) + check digit
        var exp = FormatDate(input.ExpirationDate);
        sb.Append(exp);
        sb.Append(CalculateCheckDigit(exp));
        
        // Personal number (14 chars) + check digit
        var personalNum = (input.PersonalNumber ?? "").ToUpperInvariant().PadRight(14, '<')[..14];
        sb.Append(personalNum);
        sb.Append(CalculateCheckDigit(personalNum));
        
        // Overall check digit
        var checkData = $"{passportNum}{CalculateCheckDigit(passportNum)}{dob}{CalculateCheckDigit(dob)}{exp}{CalculateCheckDigit(exp)}{personalNum}{CalculateCheckDigit(personalNum)}";
        sb.Append(CalculateCheckDigit(checkData));
        
        return sb.ToString();
    }

    private static string NormalizeName(string name, int? maxLength = null)
    {
        var normalized = name.ToUpperInvariant()
            .Replace(" ", Filler)
            .Replace("-", Filler)
            .Replace("'", "");
        
        // Remove non-alphanumeric characters except filler
        var result = new StringBuilder();
        foreach (var c in normalized)
        {
            if (char.IsLetterOrDigit(c) || c == '<')
                result.Append(c);
        }
        
        var final = result.ToString();
        
        if (maxLength.HasValue)
        {
            final = final.PadRight(maxLength.Value, '<');
            if (final.Length > maxLength.Value)
                final = final[..maxLength.Value];
        }
        
        return final;
    }

    private static string FormatDate(DateTime date)
    {
        return date.ToString("yyMMdd");
    }

    private static char CalculateCheckDigit(string data)
    {
        var weights = new[] { 7, 3, 1 };
        var total = 0;
        
        for (var i = 0; i < data.Length; i++)
        {
            var c = data[i];
            int value;
            
            if (c == '<')
                value = 0;
            else if (char.IsDigit(c))
                value = c - '0';
            else if (char.IsLetter(c))
                value = char.ToUpper(c) - 'A' + 10;
            else
                value = 0;
            
            total += value * weights[i % 3];
        }
        
        return (char)('0' + (total % 10));
    }
}

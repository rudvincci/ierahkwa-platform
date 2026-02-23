using ArgumentException = System.ArgumentException;

namespace Mamey.TravelIdentityStandards.MachineReadableZone;

public static class MrzGenerator
{
    // Supported Document Types
    

    // MRZ Field Lengths for TD1, TD2, and TD3
    private static readonly int[] MRZLengthsTD1 = { 9, 30, 30 }; // Document number, Name, etc.
    private static readonly int[] MRZLengthsTD2 = { 9, 36 }; // Document number, Name
    private static readonly int[] MRZLengthsTD3 = { 9, 44 }; // Document number, Name

    public static Mrz GenerateMrz(DocumentType type, MrzData data)
    {
        
        // Select MRZ length based on document type
        Mrz mrz;
        switch (type)
        {
            case DocumentType.Passport:
                mrz = GenerateTd3Mrz(data);
                break;
            case DocumentType.IdCard:
                mrz = GenerateTd1Mrz(data);
                break;
            case DocumentType.Visa:
                mrz = GenerateTd2Mrz(data);
                break;
            default:
                throw new NotSupportedException("Unsupported document type.");
        }

        return mrz;
    }

    private static PassportMrz GenerateTd3Mrz(MrzData data)
    {
        ValidateMRZData(data, 14);

        string line1 = MRZUtils.FormatField(
            $"P<{data.IssuingCountry}{data.Surname}<<{data.GivenNames}", 44);

        string documentWithCheck = $"{data.DocumentNumber}{MRZUtils.CalculateCheckDigit(data.DocumentNumber)}";
        string dobWithCheck = $"{MRZUtils.FormatDate(data.DateOfBirth)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.DateOfBirth))}";
        string expiryWithCheck = $"{MRZUtils.FormatDate(data.ExpiryDate)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.ExpiryDate))}";
        string optionalWithCheck = $"{data.OptionalData}{MRZUtils.CalculateCheckDigit(data.OptionalData)}";
        string compositeCheck = $"{documentWithCheck}{dobWithCheck}{expiryWithCheck}{optionalWithCheck}";
        string compositeCheckDigit = MRZUtils.CalculateCheckDigit(compositeCheck).ToString();

        string line2 = MRZUtils.FormatField(
            $"{documentWithCheck}{data.Nationality}{dobWithCheck}{data.Gender}{expiryWithCheck}{optionalWithCheck}{compositeCheckDigit}", 44);

        return new PassportMrz( line1, line2 );
    }

    private static IdCardMrz GenerateTd1Mrz(MrzData data)
    {
        ValidateMRZData(data, 15);

        string documentWithCheck = $"{data.DocumentNumber}{MRZUtils.CalculateCheckDigit(data.DocumentNumber)}";
        string optionalWithCheck = $"{data.OptionalData}{MRZUtils.CalculateCheckDigit(data.OptionalData)}";

        string line1 = MRZUtils.FormatField(
            $"A<{data.IssuingCountry}{documentWithCheck}{data.Nationality}<<{optionalWithCheck}", 30);

        string dobWithCheck = $"{MRZUtils.FormatDate(data.DateOfBirth)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.DateOfBirth))}";
        string expiryWithCheck = $"{MRZUtils.FormatDate(data.ExpiryDate)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.ExpiryDate))}";
        string line2 = MRZUtils.FormatField(
            $"{dobWithCheck}{data.Gender}{expiryWithCheck}{MRZUtils.FormatField(data.OptionalData, 11)}", 30);

        string line3 = MRZUtils.FormatField($"{data.Surname}<<{data.GivenNames}", 30);

        return new IdCardMrz(line1, line2, line3);
    }

    private static VisaMrz GenerateTd2Mrz(MrzData data)
    {
        ValidateMRZData(data, 15);

        string documentWithCheck = $"{data.DocumentNumber}{MRZUtils.CalculateCheckDigit(data.DocumentNumber)}";
        string line1 = MRZUtils.FormatField(
            $"V<{data.IssuingCountry}{documentWithCheck}{data.Surname}<<{data.GivenNames}", 36);

        string dobWithCheck = $"{MRZUtils.FormatDate(data.DateOfBirth)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.DateOfBirth))}";
        string expiryWithCheck = $"{MRZUtils.FormatDate(data.ExpiryDate)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.ExpiryDate))}";
        var optionalWithCheck = string.Empty;
        if (!string.IsNullOrEmpty(data.OptionalData))
        {
            optionalWithCheck = $"{data.OptionalData}{MRZUtils.CalculateCheckDigit(data.OptionalData)}";
        }
        string line2 = MRZUtils.FormatField(
            $"{dobWithCheck}{data.Gender}{expiryWithCheck}{data.Nationality}{optionalWithCheck}", 36);

        return new VisaMrz( line1, line2 );
    }

    private static void ValidateMRZData(MrzData data, int optionalDataLength)
    {
        if (data.OptionalData?.Length > optionalDataLength)
            throw new ArgumentException($"Optional data must not exceed {optionalDataLength} characters.");
    }
}
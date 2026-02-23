namespace Mamey.TravelIdentityStandards.MachineReadableZone;

/// <summary>
/// IdCardMrz - OAIC TD1 MRZ
/// 
/// </summary>
public class IdCardMrz(string line1, string line2, string line3)
    : Mrz(DocumentType.IdCard, Lines, LineLenghtCount)
{
    private const int Lines = 3;
    private const int LineLenghtCount = 30;
    public const string Line1Regex = @"^A<[A-Z]{3}[A-Z0-9]{9}[0-9][A-Z]{3}<<[A-Z0-9<]{15}[0-9]$";
    public const string Line2Regex = @"^[0-9]{6}[0-9][MF<][0-9]{6}[0-9][A-Z0-9<]{11}[0-9]$";
    public const string Line3Regex = @"^[A-Z<]+<<[A-Z<]+$ ";

    public string Line1 { get; } = line1 ?? throw new ArgumentNullException(nameof(line1));
    public string Line2 { get; } = line2 ?? throw new ArgumentNullException(nameof(line2));
    public string Line3 { get; } = line3 ?? throw new ArgumentNullException(nameof(line3));
    
    public override string ToString() => $"{Line1}{Environment.NewLine}{Line2}{Environment.NewLine}{Line3}";
}
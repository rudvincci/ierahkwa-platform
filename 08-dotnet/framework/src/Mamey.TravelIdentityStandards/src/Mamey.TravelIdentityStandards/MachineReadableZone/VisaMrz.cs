namespace Mamey.TravelIdentityStandards.MachineReadableZone;

/// <summary>
/// VisaMrz - OAIC TD2 MRZ
/// 
/// </summary>
public class VisaMrz(string line1, string line2)
    : Mrz(DocumentType.Visa, Lines, LineLenghtCount)
{
    private const int Lines = 2;
    private const int LineLenghtCount = 36;
    private const string Line1Regex = @"^P<[A-Z]{3}[A-Z0-9]{9}[0-9][A-Z<]+<<[A-Z<]+$";
    private const string Line2Regex = @"^[A-Z0-9]{9}[0-9][A-Z]{3}[0-9]{6}[0-9][MF<][0-9]{6}[0-9][A-Z0-9<]{14}[0-9]$";

    public string Line1 { get; } = line1 ?? throw new ArgumentNullException(nameof(line1));

    public string Line2 { get; }= line2 ?? throw new ArgumentNullException(nameof(line2));
    public override string ToString() => $"{Line1}{Environment.NewLine}{Line2}";
}
using System;
using System.Text;

namespace Mamey.Government.Modules.TravelIdentities.Core.Services;

/// <summary>
/// AAMVA CDS 2020 compliant barcode generator for driver's licenses and ID cards.
/// Generates data for PDF417 2D barcode format.
/// </summary>
internal sealed class AamvaBarcodeService : IAamvaBarcodeService
{
    private const string AamvaVersion = "10"; // Version 10 (2020 standard)
    private const string JurisdictionVersion = "01";
    private const char Segment = (char)0x0A; // Line feed as segment terminator
    private const char RecordSeparator = (char)0x1E;
    private const char DataSeparator = (char)0x1F;
    private const string FileType = "ANSI ";

    public AamvaData GenerateBarcodeData(AamvaInput input)
    {
        var header = GenerateHeader(input.Jurisdiction);
        var subfile = GenerateSubfile(input);
        var rawData = $"{header}{subfile}";
        
        return new AamvaData(rawData, header, subfile);
    }

    public byte[] GenerateBarcodeImage(AamvaData data, int width = 400, int height = 150)
    {
        // In production, use a library like ZXing.Net or BarcodeLib
        // This is a placeholder that returns the raw data as bytes
        return Encoding.ASCII.GetBytes(data.RawData);
    }

    public AamvaInput? ParseBarcodeData(string barcodeData)
    {
        try
        {
            // Parse the AAMVA data - simplified implementation
            // In production, implement full parsing of all data elements
            var lines = barcodeData.Split(Segment);
            
            string? docNum = null, firstName = null, lastName = null;
            DateTime? dob = null, issue = null, expiry = null;
            
            foreach (var line in lines)
            {
                if (line.StartsWith("DAQ")) docNum = line[3..];
                if (line.StartsWith("DCS")) lastName = line[3..];
                if (line.StartsWith("DAC")) firstName = line[3..];
                if (line.StartsWith("DBB")) dob = ParseDate(line[3..]);
                if (line.StartsWith("DBD")) issue = ParseDate(line[3..]);
                if (line.StartsWith("DBA")) expiry = ParseDate(line[3..]);
            }
            
            if (docNum is null || firstName is null || lastName is null || 
                dob is null || issue is null || expiry is null)
                return null;
            
            return new AamvaInput(
                docNum, "", firstName, lastName, null,
                dob.Value, "U", "UNK", 0, "", "", "", "",
                issue.Value, expiry.Value, "D");
        }
        catch
        {
            return null;
        }
    }

    private static string GenerateHeader(string jurisdiction)
    {
        var sb = new StringBuilder();
        
        // Compliance indicator
        sb.Append('@');
        
        // Data element separator
        sb.Append(Segment);
        
        // Record separator
        sb.Append(RecordSeparator);
        
        // Segment terminator
        sb.Append(Segment);
        
        // File type
        sb.Append(FileType);
        
        // AAMVA version
        sb.Append(AamvaVersion);
        
        // Jurisdiction version
        sb.Append(JurisdictionVersion);
        
        // Number of entries
        sb.Append("01");
        
        // Jurisdiction code (IIN)
        sb.Append(jurisdiction.PadRight(6, '0')[..6]);
        
        // Subfile designator
        sb.Append("DL");
        
        // Offset (header length)
        sb.Append("0041");
        
        // Length (placeholder, will be calculated)
        sb.Append("0000");
        
        return sb.ToString();
    }

    private static string GenerateSubfile(AamvaInput input)
    {
        var sb = new StringBuilder();
        
        // Subfile type
        sb.Append("DL");
        
        // Required elements
        AddElement(sb, "DCS", input.LastName); // Family name
        AddElement(sb, "DAC", input.FirstName); // First name
        if (!string.IsNullOrEmpty(input.MiddleName))
            AddElement(sb, "DAD", input.MiddleName); // Middle name
        
        AddElement(sb, "DBB", FormatDate(input.DateOfBirth)); // Date of birth
        AddElement(sb, "DBA", FormatDate(input.ExpirationDate)); // Expiration date
        AddElement(sb, "DBD", FormatDate(input.IssueDate)); // Issue date
        
        AddElement(sb, "DAQ", input.DocumentNumber); // Document number
        AddElement(sb, "DAG", input.StreetAddress); // Street address
        AddElement(sb, "DAI", input.City); // City
        AddElement(sb, "DAJ", input.State); // State
        AddElement(sb, "DAK", input.PostalCode); // Postal code
        
        AddElement(sb, "DBC", input.Sex switch // Sex
        {
            "M" or "MALE" => "1",
            "F" or "FEMALE" => "2",
            _ => "9"
        });
        
        AddElement(sb, "DAY", input.EyeColor[..3].ToUpper()); // Eye color
        AddElement(sb, "DAU", $"{input.HeightInches:D3} in"); // Height
        
        AddElement(sb, "DCG", "USA"); // Country
        
        if (!string.IsNullOrEmpty(input.Restrictions))
            AddElement(sb, "DCB", input.Restrictions);
        
        if (!string.IsNullOrEmpty(input.Endorsements))
            AddElement(sb, "DCD", input.Endorsements);
        
        return sb.ToString();
    }

    private static void AddElement(StringBuilder sb, string tag, string value)
    {
        sb.Append(tag);
        sb.Append(value);
        sb.Append(Segment);
    }

    private static string FormatDate(DateTime date)
    {
        return date.ToString("MMddyyyy");
    }

    private static DateTime? ParseDate(string dateStr)
    {
        if (dateStr.Length != 8) return null;
        
        if (int.TryParse(dateStr[..2], out var month) &&
            int.TryParse(dateStr[2..4], out var day) &&
            int.TryParse(dateStr[4..], out var year))
        {
            return new DateTime(year, month, day);
        }
        
        return null;
    }
}

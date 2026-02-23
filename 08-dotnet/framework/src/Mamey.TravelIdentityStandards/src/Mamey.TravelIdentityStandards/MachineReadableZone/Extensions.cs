using System.Text.RegularExpressions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Mamey.TravelIdentityStandards.Tests")]
namespace Mamey.TravelIdentityStandards.MachineReadableZone;

/// <summary>
/// MRZ utilities for generating and validating machine-readable zones.
/// </summary>
internal static class MRZUtils
{
    internal const string Td1Line1Regex = @"^A<[A-Z]{3}[A-Z0-9]{9}[0-9][A-Z]{3}<<[A-Z0-9<]{15}[0-9]$";
    internal const string Td1Line2Regex = @"^[0-9]{6}[0-9][MF<][0-9]{6}[0-9][A-Z0-9<]{11}[0-9]$";
    internal const string TdLine3Regex = @"^[A-Z<]+<<[A-Z<]+$";
    /// <summary>
    /// Formats a string to fit the required length by padding or truncating.
    /// </summary>
    public static string FormatField(string input, int length)
    {
        return Regex.Replace(input.ToUpper(), @"[^A-Z0-9<]", "<").PadRight(length, '<').Substring(0, length);
    }

    /// <summary>
    /// Formats a date to the ICAO YYMMDD format.
    /// </summary>
    public static string FormatDate(DateTime date)
    {
        return date.ToString("yyMMdd");
    }

    /// <summary>
    /// Calculates the MRZ check digit for a given string.
    /// </summary>
    public static int CalculateCheckDigit(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));

        int[] weights = { 7, 3, 1 };
        int sum = 0;

        for (int i = 0; i < input.Length; i++)
        {
            int value = input[i] switch
            {
                >= '0' and <= '9' => input[i] - '0',
                >= 'A' and <= 'Z' => input[i] - 'A' + 10,
                '<' => 0,
                _ => throw new ArgumentException($"Invalid character '{input[i]}' in input.")
            };

            sum += value * weights[i % 3];
        }

        return sum % 10;
    }
}

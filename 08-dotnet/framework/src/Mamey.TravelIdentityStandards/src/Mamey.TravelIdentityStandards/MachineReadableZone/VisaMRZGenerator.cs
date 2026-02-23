// using System.Text.RegularExpressions;
//
// namespace Mamey.TravelIdentityStandards.MachineReadableZone;
//
// /// <summary>
// /// MRZ generator for TD2 (Visa) documents.
// /// </summary>
// internal class VisaMRZGenerator
// {
//     internal const string Td1Line1Regex = @"^A<[A-Z]{3}[A-Z0-9]{9}[0-9][A-Z]{3}<<[A-Z0-9<]{15}[0-9]$";
//     internal const string Td1Line2Regex = @"^[0-9]{6}[0-9][MF<][0-9]{6}[0-9][A-Z0-9<]{11}[0-9]$";
//     internal const string TdLine3Regex = @"^[A-Z<]+<<[A-Z<]+$";
//     public static VisaMrz GenerateMRZ(MrzData data)
//     {
//         ValidateMRZData(data);
//
//         string documentWithCheck = $"{data.DocumentNumber}{MRZUtils.CalculateCheckDigit(data.DocumentNumber)}";
//         string line1 = MRZUtils.FormatField(
//             $"V<{data.IssuingCountry}{documentWithCheck}{data.Surname}<<{data.GivenNames}", 36);
//
//         string dobWithCheck = $"{MRZUtils.FormatDate(data.DateOfBirth)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.DateOfBirth))}";
//         string expiryWithCheck = $"{MRZUtils.FormatDate(data.ExpiryDate)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.ExpiryDate))}";
//         string optionalWithCheck = $"{data.OptionalData}{MRZUtils.CalculateCheckDigit(data.OptionalData)}";
//         string line2 = MRZUtils.FormatField(
//             $"{dobWithCheck}{data.Gender}{expiryWithCheck}{data.Nationality}{optionalWithCheck}", 36);
//
//         return new VisaMrz( line1, line2 );
//     }
//
//     private static void ValidateMRZData(MrzData data)
//     {
//         if (data.OptionalData.Length > 15)
//             throw new ArgumentException("Optional data must not exceed 15 characters for TD2.");
//     }
//
// }
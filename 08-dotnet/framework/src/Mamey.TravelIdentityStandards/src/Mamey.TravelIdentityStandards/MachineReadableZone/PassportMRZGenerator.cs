// using System.Text.RegularExpressions;
//
// namespace Mamey.TravelIdentityStandards.MachineReadableZone;
//
// /// <summary>
// /// MRZ generator for TD3 (Passport) documents.
// /// </summary>
// internal class PassportMRZGenerator
// {
//     public static PassportMrz GenerateMRZ(MrzData data)
//     {
//         ValidateMRZData(data);
//
//         string line1 = MRZUtils.FormatField(
//             $"P<{data.IssuingCountry}{data.Surname}<<{data.GivenNames}", 44);
//
//         string documentWithCheck = $"{data.DocumentNumber}{MRZUtils.CalculateCheckDigit(data.DocumentNumber)}";
//         string dobWithCheck = $"{MRZUtils.FormatDate(data.DateOfBirth)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.DateOfBirth))}";
//         string expiryWithCheck = $"{MRZUtils.FormatDate(data.ExpiryDate)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.ExpiryDate))}";
//         string optionalWithCheck = $"{data.OptionalData}{MRZUtils.CalculateCheckDigit(data.OptionalData)}";
//         string compositeCheck = $"{documentWithCheck}{dobWithCheck}{expiryWithCheck}{optionalWithCheck}";
//         string compositeCheckDigit = MRZUtils.CalculateCheckDigit(compositeCheck).ToString();
//
//         string line2 = MRZUtils.FormatField(
//             $"{documentWithCheck}{data.Nationality}{dobWithCheck}{data.Gender}{expiryWithCheck}{optionalWithCheck}{compositeCheckDigit}", 44);
//
//         return new PassportMrz( line1, line2 );
//     }
//
//     private static void ValidateMRZData(MrzData data)
//     {
//         if (data.OptionalData.Length > 14)
//             throw new ArgumentException("Optional data must not exceed 14 characters for TD3.");
//     }
// }
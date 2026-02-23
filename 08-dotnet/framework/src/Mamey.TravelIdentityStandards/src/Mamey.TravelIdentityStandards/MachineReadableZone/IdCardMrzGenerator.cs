// using System.Text.RegularExpressions;
//
// namespace Mamey.TravelIdentityStandards.MachineReadableZone;
// /// <summary>
// /// MRZ generator for TD1 (ID Card) documents.
// /// </summary>
// internal static class IdCardMrzGenerator
// {
//     internal const string Td1Line1Regex = @"^A<[A-Z]{3}[A-Z0-9]{9}[0-9][A-Z]{3}<<[A-Z0-9<]{15}[0-9]$";
//     internal const string Td1Line2Regex = @"^[0-9]{6}[0-9][MF<][0-9]{6}[0-9][A-Z0-9<]{11}[0-9]$";
//     internal const string TdLine3Regex = @"^[A-Z<]+<<[A-Z<]+$";
//     public static IdCardMrz GenerateMRZ(MrzData data)
//     {
//         ValidateMRZData(data);
//
//         string documentWithCheck = $"{data.DocumentNumber}{MRZUtils.CalculateCheckDigit(data.DocumentNumber)}";
//         string optionalWithCheck = $"{data.OptionalData}{MRZUtils.CalculateCheckDigit(data.OptionalData)}";
//
//         string line1 = MRZUtils.FormatField(
//             $"A<{data.IssuingCountry}{documentWithCheck}{data.Nationality}<<{optionalWithCheck}", 30);
//
//         string dobWithCheck = $"{MRZUtils.FormatDate(data.DateOfBirth)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.DateOfBirth))}";
//         string expiryWithCheck = $"{MRZUtils.FormatDate(data.ExpiryDate)}{MRZUtils.CalculateCheckDigit(MRZUtils.FormatDate(data.ExpiryDate))}";
//         string line2 = MRZUtils.FormatField(
//             $"{dobWithCheck}{data.Gender}{expiryWithCheck}{MRZUtils.FormatField(data.OptionalData, 11)}", 30);
//
//         string line3 = MRZUtils.FormatField($"{data.Surname}<<{data.GivenNames}", 30);
//
//         return new IdCardMrz(line1, line2, line3);
//     }
//
//     private static void ValidateMRZData(MrzData data)
//     {
//         if (data.OptionalData.Length > 15)
//             throw new ArgumentException("Optional data must not exceed 15 characters for TD1.");
//     }
// }
// //
// // internal static class IdCardMrzGenerator
// // {
// //     internal const string Td1Line1Regex = @"^A<[A-Z]{3}[A-Z0-9]{9}[0-9][A-Z]{3}<<[A-Z0-9<]{15}[0-9]$";
// //     internal const string Td1Line2Regex = @"^[0-9]{6}[0-9][MF<][0-9]{6}[0-9][A-Z0-9<]{11}[0-9]$";
// //     internal const string TdLine3Regex = @"^[A-Z<]+<<[A-Z<]+$";
// //     public static IdCardMrz GenerateMRZ(MrzData data)
// //     {
// //         ValidateMRZData(data);
// //
// //         string documentWithCheck = $"{data.DocumentNumber}{CalculateCheckDigit(data.DocumentNumber)}";
// //         string optionalWithCheck = $"{data.OptionalData}{(string.IsNullOrEmpty(data.OptionalData) ? "" : CalculateCheckDigit(data.OptionalData))}";
// //
// //         string line1 = FormatField($"A<{data.IssuingCountry}{documentWithCheck}{data.Nationality}<<{optionalWithCheck}",
// //             30);
// //         string dobWithCheck = $"{FormatDate(data.DateOfBirth)}{CalculateCheckDigit(FormatDate(data.DateOfBirth))}";
// //         string expiryWithCheck = $"{FormatDate(data.ExpiryDate)}{CalculateCheckDigit(FormatDate(data.ExpiryDate))}";
// //   
// //         string line2 =
// //             FormatField(
// //                 $"{dobWithCheck}{data.Gender}{expiryWithCheck}{(string.IsNullOrEmpty(data.OptionalData) ? "" :$"{(data.OptionalData.Length <= 11 ? data.OptionalData : data.OptionalData?.Substring(0, 11))}{CalculateCheckDigit(data.OptionalData)}")}",
// //                 30);
// //         string line3 = FormatField($"{data.Surname}<<{data.GivenNames}", 30);
// //         return new IdCardMrz(line1, line2, line3);
// //
// //     }
// //
// //     internal static void ValidateMRZData(MrzData data)
// //     {
// //         Extensions.ValidateCommonMRZFields(data, 9, 3, 3);
// //         if (!string.IsNullOrEmpty(data.OptionalData) && data.OptionalData.Length > 11)
// //             throw new ArgumentException("Optional data must not exceed 11 characters for TD1.");
// //     }
// //
// //     private static string FormatField(string input, int length)
// //     {
// //         return Regex.Replace(input.ToUpper(), @"[^A-Z0-9<]", "<").PadRight(length, '<').Substring(0, length);
// //     }
// //
// //     private static string FormatDate(DateTime date) => date.ToString("yyMMdd");
// //
// //     private static int CalculateCheckDigit(string input)
// //     {
// //         if (string.IsNullOrEmpty(input)) throw new ArgumentException("Value cannot be null or empty.", nameof(input));
// //         return MRZChecksumCalculator.CalculateCompositeChecksum(input); // Shared checksum calculation
// //     }
// // }
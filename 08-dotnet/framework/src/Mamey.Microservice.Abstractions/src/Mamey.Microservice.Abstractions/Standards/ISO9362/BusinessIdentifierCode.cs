using System.ComponentModel.DataAnnotations;

namespace Mamey.Microservice.Abstractions.Standards.ISO9362
{
    /// <summary>
    /// Business Identifier Code (BIC) is a standard established by
    /// the International Organization for Standarization (ISO).
    /// The standard is defined in ISO9362:2014.
    /// See https://www.swift.com/swift-resource/14256/download for the specification.
    /// </summary>
    public class BusinessIdentifierCode
    {
        public BusinessIdentifierCode()
        {
        }

        [Required, MinLength(4), MaxLength(4)]
        public string BusinessPartyPrefix { get; set; }

        [Required, MinLength(2), MaxLength(2)]
        public string CountryCode { get; set; }

        [Required, MinLength(2), MaxLength(2)]
        public string Suffix { get; set; }

        [MinLength(3), MaxLength(3)]
        public string BranchCode { get; set; }

        public string BIC => $"{BusinessPartyPrefix}{CountryCode}{Suffix}";

        
    }
    public static class BusinessIdentiticationCodeExtensions
    {
        public static BusinessIdentifierCode ParseToBIC(this string bicString)
        {
            if (string.IsNullOrEmpty(bicString))
            {
                throw new ArgumentException($"'{nameof(bicString)}' cannot be null or empty.", nameof(bicString));
            }

            return new BusinessIdentifierCode
            {
                BusinessPartyPrefix = bicString.Substring(0,4),
                CountryCode = bicString.Substring(4, 2),
                Suffix = bicString.Substring(6, 2),
                BranchCode = bicString.Substring(8, 3)
            };
        }
    }
}


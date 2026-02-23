using System.Text;
using Mamey.ISO.Abstractions;
using Mamey.ISO3166;
using Singulink.Numerics;

namespace Mamey.ISO.ISO4217;

public class CurrencyConverter
{
    private readonly IISO4217Service _iso4217Service;
    private readonly IISO3166Service _iso3166Service;
    private static readonly string[] ones = new string[] { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
    private static readonly string[] tens = new string[] { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
    private static readonly string[] thousands = new string[] { "", "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion", "tredecillion", "quattuordecillion", "quindecillion", "sexdecillion", "septendecillion", "octodecillion", "novemdecillion", "vigintillion" };
    private static readonly string[] currencyCodes = new string[] { "USD", "EUR", "GBP" };
    private static readonly string[] currencies = new string[] { "dollars", "euros", "pounds" };

    public CurrencyConverter(IISO4217Service iso4217Service, IISO3166Service iso3166Service)
    {
        _iso4217Service = iso4217Service;
        _iso3166Service = iso3166Service;
    }
    public string ConvertToWords(BigDecimal value, string currencyCode = "")
        => ConvertToWords((decimal)value, currencyCode);
    public string ConvertToWords(decimal value, string currencyCode = "")
    {
        if (value < 0m)
        {
            throw new ArgumentException("Value must be non-negative");
        }

        if (value == 0m)
        {
            return "Zero";
        }

        int fractionalPart = (int)Math.Round((value - (decimal)Math.Floor(value)) * 100);
        long wholePart = (long)Math.Floor(value);

        StringBuilder sb = new StringBuilder();

        // Convert whole part to words
        if (wholePart == 0)
        {
            sb.Append("Zero");
        }
        else
        {
            int numThousandsGroups = (int)Math.Floor(Math.Log10(wholePart) / 3);
            int[] thousandsGroupValues = new int[numThousandsGroups + 1];

            for (int i = 0; i <= numThousandsGroups; i++)
            {
                thousandsGroupValues[i] = (int)(wholePart / (long)Math.Pow(1000, i) % 1000);
            }

            for (int i = numThousandsGroups; i >= 0; i--)
            {
                int thousandsGroupValue = thousandsGroupValues[i];
                if (thousandsGroupValue == 0)
                {
                    continue;
                }

                int hundredsDigit = thousandsGroupValue / 100;
                int tensAndOnes = thousandsGroupValue % 100;

                if (hundredsDigit > 0)
                {
                    sb.Append(ones[hundredsDigit] + " hundred ");
                }

                if (tensAndOnes > 0)
                {
                    if (tensAndOnes < 20)
                    {
                        sb.Append(ones[tensAndOnes] + " ");
                    }
                    else
                    {
                        int tensDigit = tensAndOnes / 10;
                        int onesDigit = tensAndOnes % 10;
                        if (tensDigit > 0)
                        {
                            sb.Append(tens[tensDigit] + " ");
                        }
                        if (onesDigit > 0)
                        {
                            sb.Append(ones[onesDigit] + " ");
                        }
                    }
                }

                if (i > 0)
                {
                    sb.Append(thousands[i] + " ");
                }
            }
        }

        // Convert fractional part to words
        if (fractionalPart > 0)
        {
            sb.Append("and " + fractionalPart.ToString("00") + "/100 ");
        }
        else
        {
            sb.Append("and no cents ");
        }

        // Append currency code and name (if provided)
        if (!string.IsNullOrEmpty(currencyCode))
        {
            var currency = _iso4217Service.GetCurrencyAsync(currencyCode.ToUpper());


            int index = Array.IndexOf(currencyCodes, currencyCode.ToUpper());
            if (index >= 0)
            {
                string currencyName = currencies[index];
                sb.AppendFormat("{0} {1}", currencyName, currencyCode.ToUpper());
            }
            else
            {
                sb.Append(currencyCode.ToUpper());
            }
        }

        return sb.ToString().Trim();
    }

    public string ConvertToWords(string value, string currencyCode = "")
    {
        if (!decimal.TryParse(value, out decimal parsedValue))
        {
            throw new ArgumentException("Invalid value");
        }

        return ConvertToWords(parsedValue, currencyCode);
    }
    
}

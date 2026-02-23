using System.Globalization;
using System.Text.RegularExpressions;

namespace Mamey.ISO.SWIFT;

/// <summary>
/// Parser for MT103 (Customer Credit Transfer) messages
/// </summary>
public static class Mt103Parser
{
    public static Mt103Message Parse(SwiftFinMessage swiftMessage)
    {
        if (swiftMessage.MessageType != "103")
            throw new ArgumentException("Message is not MT103", nameof(swiftMessage));

        var mt103 = new Mt103Message
        {
            SwiftMessage = swiftMessage
        };

        // Field 20: Sender's Reference
        mt103.SenderReference = swiftMessage.GetFieldValue("20") ?? string.Empty;

        // Field 13C: Time Indication (Optional)
        var timeIndication = swiftMessage.GetFieldValue("13C");
        if (!string.IsNullOrEmpty(timeIndication))
        {
            mt103.TimeIndication = ParseTimeIndication(timeIndication);
        }

        // Field 23B: Bank Operation Code
        mt103.BankOperationCode = swiftMessage.GetFieldValue("23B") ?? string.Empty;

        // Field 26T: Transaction Type Code (Optional)
        mt103.TransactionTypeCode = swiftMessage.GetFieldValue("26T");

        // Field 32A: Value Date, Currency Code, Amount
        var field32A = swiftMessage.GetFieldValue("32A");
        if (!string.IsNullOrEmpty(field32A))
        {
            mt103.ValueDateAmount = ParseValueDateAmount(field32A);
        }

        // Field 33B: Currency Code, Amount (Optional)
        var field33B = swiftMessage.GetFieldValue("33B");
        if (!string.IsNullOrEmpty(field33B))
        {
            mt103.InstructedAmount = ParseInstructedAmount(field33B);
        }

        // Field 36: Exchange Rate (Optional)
        var field36 = swiftMessage.GetFieldValue("36");
        if (!string.IsNullOrEmpty(field36) && decimal.TryParse(field36, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
        {
            mt103.ExchangeRate = rate;
        }

        // Field 50A/F/K: Ordering Customer
        mt103.OrderingCustomer = ParseOrderingCustomer(swiftMessage);

        // Field 52A/D: Ordering Institution (Optional)
        mt103.OrderingInstitution = ParseOrderingInstitution(swiftMessage);

        // Field 53A/B/D: Sender's Correspondent (Optional)
        mt103.SenderCorrespondent = swiftMessage.GetFieldValue("53A") ?? 
                                     swiftMessage.GetFieldValue("53B") ?? 
                                     swiftMessage.GetFieldValue("53D");

        // Field 54A/B/D: Receiver's Correspondent (Optional)
        mt103.ReceiverCorrespondent = swiftMessage.GetFieldValue("54A") ?? 
                                      swiftMessage.GetFieldValue("54B") ?? 
                                      swiftMessage.GetFieldValue("54D");

        // Field 56A/C/D: Intermediary Institution (Optional)
        mt103.IntermediaryInstitution = swiftMessage.GetFieldValue("56A") ?? 
                                        swiftMessage.GetFieldValue("56C") ?? 
                                        swiftMessage.GetFieldValue("56D");

        // Field 57A/B/C/D: Account With Institution (Optional)
        mt103.AccountWithInstitution = swiftMessage.GetFieldValue("57A") ?? 
                                      swiftMessage.GetFieldValue("57B") ?? 
                                      swiftMessage.GetFieldValue("57C") ?? 
                                      swiftMessage.GetFieldValue("57D");

        // Field 59A/F: Beneficiary Customer
        mt103.BeneficiaryCustomer = ParseBeneficiaryCustomer(swiftMessage);

        // Field 70: Remittance Information (Optional)
        mt103.RemittanceInformation = swiftMessage.GetFieldValue("70");

        // Field 71A: Details of Charges
        mt103.DetailsOfCharges = swiftMessage.GetFieldValue("71A") ?? "OUR";

        // Field 71F: Sender's Charges (Optional)
        mt103.SendersCharges = swiftMessage.GetFieldValue("71F");

        // Field 71G: Receiver's Charges (Optional)
        mt103.ReceiversCharges = swiftMessage.GetFieldValue("71G");

        // Field 72: Sender to Receiver Information (Optional)
        mt103.SenderToReceiverInformation = swiftMessage.GetFieldValue("72");

        // Field 77B: Regulatory Reporting (Optional)
        mt103.RegulatoryReporting = swiftMessage.GetFieldValue("77B");

        return mt103;
    }

    private static string? ParseTimeIndication(string value)
    {
        // Format: /CLSTIME/0945+0100 or similar
        return value;
    }

    private static ValueDateAmount ParseValueDateAmount(string value)
    {
        // Format: YYMMDDCCYAMOUNT (e.g., 091120EUR15000,11)
        if (value.Length < 9)
            throw new ArgumentException("Invalid 32A field format", nameof(value));

        var dateStr = value.Substring(0, 6);
        var currency = value.Substring(6, 3);
        var amountStr = value.Substring(9).Replace(",", ".");

        if (!DateTime.TryParseExact(dateStr, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            throw new ArgumentException($"Invalid date format in 32A: {dateStr}", nameof(value));
        }

        if (!decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
        {
            throw new ArgumentException($"Invalid amount format in 32A: {amountStr}", nameof(value));
        }

        return new ValueDateAmount
        {
            ValueDate = date,
            Currency = currency,
            Amount = amount
        };
    }

    private static InstructedAmount? ParseInstructedAmount(string value)
    {
        // Format: CCYAMOUNT
        if (value.Length < 4)
            return null;

        var currency = value.Substring(0, 3);
        var amountStr = value.Substring(3).Replace(",", ".");

        if (!decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
            return null;

        return new InstructedAmount
        {
            Currency = currency,
            Amount = amount
        };
    }

    private static OrderingCustomer? ParseOrderingCustomer(SwiftFinMessage message)
    {
        var field50A = message.GetFieldValue("50A");
        var field50F = message.GetFieldValue("50F");
        var field50K = message.GetFieldValue("50K");

        if (!string.IsNullOrEmpty(field50A))
        {
            return new OrderingCustomer { Type = "A", Value = field50A };
        }
        if (!string.IsNullOrEmpty(field50F))
        {
            return new OrderingCustomer { Type = "F", Value = field50F };
        }
        if (!string.IsNullOrEmpty(field50K))
        {
            return new OrderingCustomer { Type = "K", Value = field50K };
        }

        return null;
    }

    private static string? ParseOrderingInstitution(SwiftFinMessage message)
    {
        return message.GetFieldValue("52A") ?? 
               message.GetFieldValue("52D");
    }

    private static BeneficiaryCustomer? ParseBeneficiaryCustomer(SwiftFinMessage message)
    {
        var field59 = message.GetFieldValue("59");
        var field59A = message.GetFieldValue("59A");

        if (!string.IsNullOrEmpty(field59))
        {
            return new BeneficiaryCustomer { Type = "59", Value = field59 };
        }
        if (!string.IsNullOrEmpty(field59A))
        {
            return new BeneficiaryCustomer { Type = "A", Value = field59A };
        }

        return null;
    }
}

/// <summary>
/// Parsed MT103 message structure
/// </summary>
public class Mt103Message
{
    public SwiftFinMessage SwiftMessage { get; set; } = null!;
    public string SenderReference { get; set; } = string.Empty;
    public string? TimeIndication { get; set; }
    public string BankOperationCode { get; set; } = string.Empty;
    public string? TransactionTypeCode { get; set; }
    public ValueDateAmount? ValueDateAmount { get; set; }
    public InstructedAmount? InstructedAmount { get; set; }
    public decimal? ExchangeRate { get; set; }
    public OrderingCustomer? OrderingCustomer { get; set; }
    public string? OrderingInstitution { get; set; }
    public string? SenderCorrespondent { get; set; }
    public string? ReceiverCorrespondent { get; set; }
    public string? IntermediaryInstitution { get; set; }
    public string? AccountWithInstitution { get; set; }
    public BeneficiaryCustomer? BeneficiaryCustomer { get; set; }
    public string? RemittanceInformation { get; set; }
    public string DetailsOfCharges { get; set; } = "OUR";
    public string? SendersCharges { get; set; }
    public string? ReceiversCharges { get; set; }
    public string? SenderToReceiverInformation { get; set; }
    public string? RegulatoryReporting { get; set; }
}

public class ValueDateAmount
{
    public DateTime ValueDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class InstructedAmount
{
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class OrderingCustomer
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class BeneficiaryCustomer
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

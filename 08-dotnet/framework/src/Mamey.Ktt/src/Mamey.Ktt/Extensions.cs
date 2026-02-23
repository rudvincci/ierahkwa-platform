using Mamey.Ktt.Message.Category1;

namespace Mamey.Ktt;

public static class Extensions
{
    public static void message()
    {
        // Create an instance of the MT103 model with sample data
        var mt103 = new MT103
        {
            TransactionReference = "ABC1234567890123",
            BankOperationCode = "CRED",
            ValueDateCurrencyAmount = "241117USD10000,50",
            OrderingCustomerBIC = "BANKUS33XXX",
            OrderingCustomerDetails = "John Doe, 123 Main St, New York, USA",
            BeneficiaryCustomer = "Jane Smith, 9876543210",
            RemittanceInformation = "Payment for invoice #12345",
            DetailsOfCharges = "SHA",
            SenderToReceiverInfo = "Urgent processing required"
        };

        // Define the message template
        string template = @"
        TELEX MT103 FROM {{SenderBankName}}

        From     : {{SenderBankName}}
        Subject  : Payment Notification
        Date     : {{TransactionDateUtc}}
        CUSTOMER’S AND BANK COPY
        --------------------------------------------------------------------------------
        F20: Transaction Reference Number : {{20}}
        F23B: Bank Operation Code         : {{23B}}
        F32A: Value Date/Currency/Amount  : {{32A}}
        F50A: Ordering Customer (BIC)     : {{50A}}
        F50K: Ordering Customer (Details) : {{50K}}
        F59 : Beneficiary Customer        : {{59}}
        F70: Remittance Information       : {{70}}
        F71A: Details of Charges          : {{71A}}
        F72: Sender to Receiver Information: {{72}}
        --------------------------------------------------------------------------------
        ";

        // Build the message using the MessageBuilder
        string filledMessage = MessageBuilder.BuildMessage(template, mt103);

        // Output the result
        Console.WriteLine(filledMessage);
    }
}

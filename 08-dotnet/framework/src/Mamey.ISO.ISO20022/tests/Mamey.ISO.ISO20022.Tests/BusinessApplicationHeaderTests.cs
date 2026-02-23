// using System.Xml;
// using System.Xml.Serialization;
// using Mamey.ISO20022.Messages.Envelope;
// using Mamey.ISO20022.Messages.Header;
// using Mamey.ISO20022.Messages.Payments.PaymentsInitiation;
// using BusinessApplicationHeader = Mamey.ISO20022.Messages.Header.BusinessApplicationHeader;
// using FinancialInstitutionIdentification = Mamey.ISO20022.Messages.Header.FinancialInstitutionIdentification;
//
// namespace Mamey.ISO.ISO20022.Tests;
//
// public class BusinessApplicationHeaderTests
// {
//     [Fact]
//     public async Task CreateMessage_WithValidInput_ShouldSucceed()
//     {
//         
//         // Arrange
//         var header = new BusinessApplicationHeader
//         {
//             CharacterSet = "UTF-8",
//             From = new Party
//             {
//                 OrganisationIdentification = new OrganisationIdentification
//                 {
//                     Name = "Bank A",
//                     Identification = "BANKA12345"
//                 }
//             },
//             To = new Party
//             {
//                 FinancialInstitutionIdentification = new FinancialInstitutionIdentification
//                 {
//                     Name = "Bank B",
//                     BICFI = "BANKBICXX"
//                 }
//             },
//             BusinessMessageIdentifier = "MSG123456789",
//             MessageDefinitionIdentifier = "pacs.008.001.08",
//             BusinessService = "payments",
//             CreationDate = DateTime.UtcNow,
//             PossibleDuplicate = false,
//             Priority = "NORM"
//         };
//
//         var paymentMessage = new CustomerCreditTransferInitiation
//         {
//             Content = new CustomerCreditTransferInitiationContent
//             {
//                 GroupHeader = new GroupHeader
//                 {
//                     MessageIdentification = "MSG123456789",
//                     CreationDateTime = DateTime.UtcNow,
//                     NumberOfTransactions = "2",
//                     ControlSum = 2000.00m,
//                     InitiatingParty = new PartyIdentification
//                     {
//                         Name = "Initiating Company",
//                         Identification = "INIT12345"
//                     }
//                 },
//                 PaymentInformation = new List<PaymentInformation>
//                 {
//                     new PaymentInformation
//                     {
//                         PaymentInformationIdentification = "PMTINF123",
//                         PaymentMethod = "TRF",
//                         BatchBooking = true,
//                         NumberOfTransactions = "2",
//                         ControlSum = 2000.00m,
//                         RequestedExecutionDate = DateTime.UtcNow.AddDays(1),
//                         Debtor = new PartyIdentification
//                         {
//                             Name = "Debtor Company",
//                             Identification = "DEBT12345"
//                         },
//                         DebtorAccount = new CashAccount
//                         {
//                             Identification = "DEBTORACCT123",
//                             Name = "Debtor Account"
//                         },
//                         DebtorAgent = new Mamey.ISO20022.Messages.Payments.PaymentsInitiation.FinancialInstitutionIdentification
//                         {
//                             BICFI = "DEBTBICXX"
//                         },
//                         CreditTransferTransactionInformation = new List<CreditTransferTransactionInformation>
//                         {
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX12345",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 1",
//                                     Identification = "CRED12345"
//                                 },
//                                 CreditorAccount = new CashAccount
//                                 {
//                                     Identification = "CREDITORACCT123",
//                                     Name = "Creditor Account 1"
//                                 }
//                             },
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX67890",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 2",
//                                     Identification = "CRED67890"
//                                 },
//                                 CreditorAccount = new CashAccount
//                                 {
//                                     Identification = "CREDITORACCT678",
//                                     Name = "Creditor Account 2"
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         };
//         var envelope = new BusinessMessageEnvelope()
//         {
//             Header = header,
//             Document = paymentMessage
//         };
//         // var paymentMessage = new PaymentMessage
//         // {
//         //     TransactionId = "TX123",
//         //     Amount = 1000.00m,
//         //     Currency = "USD"
//         // };
//
//         // Act
//         var builder = MessageBuilder<CustomerCreditTransferInitiation>
//             .CreateMessage(paymentMessage)
//             .WithHeader(header);
//
//         await builder.SendAsync();
//
//         // Assert
//         Assert.NotNull(builder);
//     }
//
//     [Fact]
//     public void CreateMessage_WithMissingHeader_ShouldThrowException()
//     {
//         // Arrange
//         var paymentMessage = new CustomerCreditTransferInitiation
//         {
//             Content = new CustomerCreditTransferInitiationContent
//             {
//                 GroupHeader = new GroupHeader
//                 {
//                     MessageIdentification = "MSG123456789",
//                     CreationDateTime = DateTime.UtcNow,
//                     NumberOfTransactions = "2",
//                     ControlSum = 2000.00m,
//                     InitiatingParty = new PartyIdentification
//                     {
//                         Name = "Initiating Company",
//                         Identification = "INIT12345"
//                     }
//                 },
//                 PaymentInformation = new List<PaymentInformation>
//                 {
//                     new PaymentInformation
//                     {
//                         PaymentInformationIdentification = "PMTINF123",
//                         PaymentMethod = "TRF",
//                         BatchBooking = true,
//                         NumberOfTransactions = "2",
//                         ControlSum = 2000.00m,
//                         RequestedExecutionDate = DateTime.UtcNow.AddDays(1),
//                         Debtor = new PartyIdentification
//                         {
//                             Name = "Debtor Company",
//                             Identification = "DEBT12345"
//                         },
//                         DebtorAccount = new CashAccount
//                         {
//                             Identification = "DEBTORACCT123",
//                             Name = "Debtor Account"
//                         },
//                         DebtorAgent = new Mamey.ISO20022.Messages.Payments.PaymentsInitiation.FinancialInstitutionIdentification
//                         {
//                             BICFI = "DEBTBICXX"
//                         },
//                         CreditTransferTransactionInformation = new List<CreditTransferTransactionInformation>
//                         {
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX12345",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 1",
//                                     Identification = "CRED12345"
//                                 },
//                                 CreditorAccount = new CashAccount
//                                 {
//                                     Identification = "CREDITORACCT123",
//                                     Name = "Creditor Account 1"
//                                 }
//                             },
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX67890",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 2",
//                                     Identification = "CRED67890"
//                                 },
//                                 CreditorAccount = new CashAccount
//                                 {
//                                     Identification = "CREDITORACCT678",
//                                     Name = "Creditor Account 2"
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         };
//         // var paymentMessage = new PaymentMessage
//         // {
//         //     TransactionId = "TX123",
//         //     Amount = 1000.00m,
//         //     Currency = "USD"
//         // };
//
//         // Act & Assert
//
//         var builder = MessageBuilder<CustomerCreditTransferInitiation>
//             .CreateMessage(paymentMessage);
//
//         // Act & Assert
//         var exception = Assert.Throws<AggregateException>(() => builder.SendAsync().Wait());
//
//         // Verify the inner exception
//         Assert.IsType<InvalidOperationException>(exception.InnerException);
//         Assert.Equal("Header is mandatory.", exception.InnerException.Message);
//     }
//
//     [Fact]
//     public void ValidateMessage_WithMissingMandatoryFields_ShouldThrowException()
//     {
//         // Arrange
//         var header = new BusinessApplicationHeader
//         {
//             CharacterSet = "UTF-8",
//             From = new Party
//             {
//                 OrganisationIdentification = new OrganisationIdentification
//                 {
//                     Name = "Bank A",
//                     Identification = "BANKA12345"
//                 }
//             },
//             To = new Party
//             {
//                 FinancialInstitutionIdentification = new FinancialInstitutionIdentification
//                 {
//                     Name = "Bank B",
//                     BICFI = "BANKBICXX"
//                 }
//             },
//             BusinessMessageIdentifier = "MSG123456789",
//             MessageDefinitionIdentifier = "pacs.008.001.08",
//             BusinessService = "payments",
//             CreationDate = DateTime.UtcNow,
//             PossibleDuplicate = false,
//             Priority = "NORM"
//         };
//
//         // var paymentMessage = new PaymentMessage
//         // {
//         //     TransactionId = "TX123",
//         //     Amount = 1000.00m,
//         //     Currency = "USD"
//         // };
//
//         var paymentMessage = new CustomerCreditTransferInitiation
//         {
//             Content = new CustomerCreditTransferInitiationContent
//             {
//                 GroupHeader = new GroupHeader
//                 {
//                     MessageIdentification = "MSG123456789",
//                     CreationDateTime = DateTime.UtcNow,
//                     NumberOfTransactions = "2",
//                     ControlSum = 2000.00m,
//                     InitiatingParty = new PartyIdentification
//                     {
//                         Name = "Initiating Company",
//                         Identification = "INIT12345"
//                     }
//                 },
//                 PaymentInformation = new List<PaymentInformation>
//                 {
//                     new PaymentInformation
//                     {
//                         PaymentInformationIdentification = "PMTINF123",
//                         PaymentMethod = "TRF",
//                         BatchBooking = true,
//                         NumberOfTransactions = "2",
//                         ControlSum = 2000.00m,
//                         RequestedExecutionDate = DateTime.UtcNow.AddDays(1),
//                         Debtor = new PartyIdentification
//                         {
//                             Name = "Debtor Company",
//                             Identification = "DEBT12345"
//                         },
//                         DebtorAccount = new CashAccount
//                         {
//                             Identification = "DEBTORACCT123",
//                             Name = "Debtor Account"
//                         },
//                         DebtorAgent = new Mamey.ISO20022.Messages.Payments.PaymentsInitiation.FinancialInstitutionIdentification
//                         {
//                             BICFI = "DEBTBICXX"
//                         },
//                         CreditTransferTransactionInformation = new List<CreditTransferTransactionInformation>
//                         {
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX12345",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 1",
//                                     Identification = "CRED12345"
//                                 },
//                                 CreditorAccount = new CashAccount
//                                 {
//                                     Identification = "CREDITORACCT123",
//                                     Name = "Creditor Account 1"
//                                 }
//                             },
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX67890",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 2",
//                                     Identification = "CRED67890"
//                                 },
//                                 CreditorAccount = new CashAccount
//                                 {
//                                     Identification = "CREDITORACCT678",
//                                     Name = "Creditor Account 2"
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         };
//
//         // Act & Assert
//
//         var builder = MessageBuilder<CustomerCreditTransferInitiation>
//             .CreateMessage(paymentMessage)
//             .WithHeader(header);
//
//         // Act & Assert
//         var exception = Assert.Throws<AggregateException>(() => builder.SendAsync().Wait());
//
//         // Verify the inner exception
//         var innerException = exception.InnerException;
//         Assert.IsType<InvalidOperationException>(innerException);
//         Assert.Equal("BusinessMessageIdentifier is required.", innerException.Message);
//     }
//
//     /// <summary>
// /// This test ensures that the CustomerCreditTransferInitiation message is serialized correctly
// /// into an ISO 20022-compliant XML.
// /// </summary>
// [Fact]
// public void SerializeMessage_ToXml_ShouldSucceed()
// {
//     // Arrange
//     var paymentMessage = new CustomerCreditTransferInitiation
//     {
//         Content = new CustomerCreditTransferInitiationContent
//         {
//             GroupHeader = new GroupHeader
//             {
//                 MessageIdentification = "MSG123456789",
//                 CreationDateTime = DateTime.UtcNow,
//                 NumberOfTransactions = "2",
//                 ControlSum = 2000.00m,
//                 InitiatingParty = new PartyIdentification
//                 {
//                     Name = "Initiating Company",
//                     Identification = "INIT12345"
//                 }
//             },
//             PaymentInformation = new List<PaymentInformation>
//             {
//                 new PaymentInformation
//                 {
//                     PaymentInformationIdentification = "PMTINF123",
//                     PaymentMethod = "TRF",
//                     BatchBooking = true,
//                     NumberOfTransactions = "2",
//                     ControlSum = 2000.00m,
//                     RequestedExecutionDate = DateTime.UtcNow.AddDays(1),
//                     Debtor = new PartyIdentification
//                     {
//                         Name = "Debtor Company",
//                         Identification = "DEBT12345"
//                     },
//                     DebtorAccount = new CashAccount
//                     {
//                         Identification = "DEBTORACCT123",
//                         Name = "Debtor Account"
//                     },
//                     DebtorAgent = new Mamey.ISO20022.Messages.Payments.PaymentsInitiation.FinancialInstitutionIdentification
//                     {
//                         BICFI = "DEBTBICXX"
//                     },
//                     CreditTransferTransactionInformation = new List<CreditTransferTransactionInformation>
//                     {
//                         new CreditTransferTransactionInformation
//                         {
//                             PaymentIdentification = "TRX12345",
//                             Amount = 1000.00m,
//                             Creditor = new PartyIdentification
//                             {
//                                 Name = "Creditor 1",
//                                 Identification = "CRED12345"
//                             },
//                             CreditorAccount = new CashAccount
//                             {
//                                 Identification = "CREDITORACCT123",
//                                 Name = "Creditor Account 1"
//                             }
//                         },
//                         new CreditTransferTransactionInformation
//                         {
//                             PaymentIdentification = "TRX67890",
//                             Amount = 1000.00m,
//                             Creditor = new PartyIdentification
//                             {
//                                 Name = "Creditor 2",
//                                 Identification = "CRED67890"
//                             },
//                             CreditorAccount = new CashAccount
//                             {
//                                 Identification = "CREDITORACCT678",
//                                 Name = "Creditor Account 2"
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//     };
//
//     // Act
//     var xml = SerializeToXml(paymentMessage);
//
//     // Assert
//     Assert.False(string.IsNullOrWhiteSpace(xml), "Serialized XML should not be null or empty.");
//
//     // Check the root element and namespaces
//     Assert.False(xml.StartsWith("<?xml"), "XML declaration should be omitted.");
//     Assert.Contains("<Document xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.001.12\"", xml);
//     Assert.Contains("<CstmrCdtTrfInitn>", xml);
//     Assert.Contains("<GrpHdr>", xml);
//     Assert.Contains("<MsgId>MSG123456789</MsgId>", xml);
//     Assert.Contains("<CreDtTm>", xml);
//     Assert.Contains("<PmtInf>", xml);
//     Assert.Contains("<PmtInfId>PMTINF123</PmtInfId>", xml);
//     Assert.Contains("<CdtTrfTxInf>", xml);
//     Assert.Contains("<PmtId>TRX12345</PmtId>", xml);
//     Assert.Contains("<Amt>1000.00</Amt>", xml);
//     Assert.Contains("<CdtrAcct>", xml);
// }
//
//     /// <summary>
//     /// This test ensures that invalid XML structures throw the correct exceptions during validation.
//     /// </summary>
//     [Fact]
//     public void ValidateXml_WithInvalidStructure_ShouldThrowException()
//     {
//         // Arrange
//         var invalidXml = "<InvalidXml><MissingEndTag>";
//
//         // Act & Assert
//         Assert.Throws<XmlException>(() =>
//         {
//             var doc = new XmlDocument();
//             doc.LoadXml(invalidXml);
//         });
//     }
//
//     /// <summary>
//     /// This test validates that fields like BusinessMessageIdentifier and MessageDefinitionIdentifier
//     /// comply with ISO 20022 constraints.
//     /// </summary>
//     /// <param name="invalidValue"></param>
//     /// <exception cref="InvalidOperationException"></exception>
//     [Theory]
//     [InlineData("12345678901234567890123456789012345678901")] // Exceeds Max35Text
//     [InlineData(null)] // Null value
//     [InlineData("")] // Empty string
//     public void ValidateBusinessMessageIdentifier_ShouldThrowException(string invalidValue)
//     {
//         // Arrange
//         var header = new BusinessApplicationHeader
//         {
//             CharacterSet = "UTF-8",
//             From = new Party
//             {
//                 OrganisationIdentification = new OrganisationIdentification
//                 {
//                     Name = "Bank A",
//                     Identification = "BANKA12345"
//                 }
//             },
//             To = new Party
//             {
//                 FinancialInstitutionIdentification = new FinancialInstitutionIdentification
//                 {
//                     Name = "Bank B",
//                     BICFI = "BANKBICXX"
//                 }
//             },
//             BusinessMessageIdentifier = "MSG123456789",
//             MessageDefinitionIdentifier = "pacs.008.001.08",
//             BusinessService = "payments",
//             CreationDate = DateTime.UtcNow,
//             PossibleDuplicate = false,
//             Priority = "NORM"
//         };
//
//         // Act & Assert
//         Assert.Throws<InvalidOperationException>(() =>
//         {
//             if (string.IsNullOrEmpty(header.BusinessMessageIdentifier) ||
//                 header.BusinessMessageIdentifier.Length > 35)
//             {
//                 throw new InvalidOperationException("BusinessMessageIdentifier must be between 1 and 35 characters.");
//             }
//         });
//     }
//
//     /// <summary>
//     /// This test ensures that the Envelope is set correctly.
//     /// </summary>
//     [Fact]
//     public void ValidateEnvelope_WithMissingReference_ShouldThrowException()
//     {
//         // Arrange
//         var builder = MessageBuilder<CustomerCreditTransferInitiation>
//             .CreateMessage(new CustomerCreditTransferInitiation())
//             .WithHeader(new Mamey.ISO20022.Messages.Header.BusinessApplicationHeader());
//
//         // Act & Assert
//         
//     }
//
//     /// <summary>
//     /// Ensure that multiple transactions in the PaymentInformation are correctly serialized and validated.
//     /// </summary>
//     [Fact]
//     public async Task CreateMessage_WithMultipleTransactions_ShouldSucceed()
//     {
//         // Arrange
//         var header = new BusinessApplicationHeader
//         {
//             CharacterSet = "UTF-8",
//             From = new Party
//             {
//                 OrganisationIdentification = new OrganisationIdentification
//                 {
//                     Name = "Bank A",
//                     Identification = "BANKA12345"
//                 }
//             },
//             To = new Party
//             {
//                 FinancialInstitutionIdentification = new FinancialInstitutionIdentification
//                 {
//                     Name = "Bank B",
//                     BICFI = "BANKBICXX"
//                 }
//             },
//             BusinessMessageIdentifier = "MSG123456789",
//             MessageDefinitionIdentifier = "pacs.008.001.08",
//             BusinessService = "payments",
//             CreationDate = DateTime.UtcNow,
//             PossibleDuplicate = false,
//             Priority = "NORM"
//         };
//
//         var paymentMessage = new CustomerCreditTransferInitiation
//         {
//             Content = new CustomerCreditTransferInitiationContent
//             {
//                 GroupHeader = new GroupHeader
//                 {
//                     MessageIdentification = "MSG123456789",
//                     CreationDateTime = DateTime.UtcNow,
//                     NumberOfTransactions = "2",
//                     ControlSum = 2000.00m,
//                     InitiatingParty = new PartyIdentification
//                     {
//                         Name = "Initiating Company",
//                         Identification = "INIT12345"
//                     }
//                 },
//                 PaymentInformation = new List<PaymentInformation>
//                 {
//                     new PaymentInformation
//                     {
//                         PaymentInformationIdentification = "PMTINF123",
//                         CreditTransferTransactionInformation = new List<CreditTransferTransactionInformation>
//                         {
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX12345",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 1",
//                                     Identification = "CRED12345"
//                                 }
//                             },
//                             new CreditTransferTransactionInformation
//                             {
//                                 PaymentIdentification = "TRX67890",
//                                 Amount = 1000.00m,
//                                 Creditor = new PartyIdentification
//                                 {
//                                     Name = "Creditor 2",
//                                     Identification = "CRED67890"
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         };
//
//         // Act
//         var builder = MessageBuilder<CustomerCreditTransferInitiation>
//             .CreateMessage(paymentMessage)
//             .WithHeader(header);
//
//         await builder.SendAsync();
//
//         // Assert
//         Assert.NotNull(builder);
//     }
//
//     /// <summary>
//     /// Check if optional fields can be omitted without causing errors.
//     /// </summary>
//     [Fact]
//     public async Task CreateMessage_WithoutOptionalFields_ShouldSucceed()
//     {
//         // Arrange
//         var header = new BusinessApplicationHeader
//         {
//             CharacterSet = "UTF-8",
//             From = new Party
//             {
//                 OrganisationIdentification = new OrganisationIdentification
//                 {
//                     Name = "Bank A",
//                     Identification = "BANKA12345"
//                 }
//             },
//             To = new Party
//             {
//                 FinancialInstitutionIdentification = new FinancialInstitutionIdentification
//                 {
//                     Name = "Bank B",
//                     BICFI = "BANKBICXX"
//                 }
//             },
//             BusinessMessageIdentifier = "MSG123456789",
//             MessageDefinitionIdentifier = "pacs.008.001.08",
//             BusinessService = "payments",
//             CreationDate = DateTime.UtcNow,
//             PossibleDuplicate = false,
//             Priority = "NORM"
//         };
//
//         var paymentMessage = new CustomerCreditTransferInitiation
//         {
//             Content = new CustomerCreditTransferInitiationContent
//             {
//                 GroupHeader = new GroupHeader
//                 {
//                     MessageIdentification = "MSG123456789",
//                     CreationDateTime = DateTime.UtcNow,
//                     NumberOfTransactions = "2",
//                     ControlSum = 2000.00m,
//                     InitiatingParty = new PartyIdentification
//                     {
//                         Name = "Initiating Company",
//                         Identification = "INIT12345"
//                     }
//                 }
//             }
//         };
//
//         // Act
//         var builder = MessageBuilder<CustomerCreditTransferInitiation>
//             .CreateMessage(paymentMessage)
//             .WithHeader(header);
//
//         await builder.SendAsync();
//
//         // Assert
//         Assert.NotNull(builder);
//     }
//
//     
//     // Helper method for serialization
//     private string SerializeToXml<T>(T obj)
//     {
//         var serializer = new XmlSerializer(typeof(T));
//         var namespaces = new XmlSerializerNamespaces();
//         namespaces.Add(string.Empty, "urn:iso:std:iso:20022:tech:xsd:pain.001.001.12"); // Add the namespace explicitly
//
//         var settings = new XmlWriterSettings
//         {
//             OmitXmlDeclaration = true, // Remove the XML declaration
//             Indent = true,             // Optional: Pretty-print the XML
//             Encoding = new System.Text.UTF8Encoding(false) // Ensure no BOM
//         };
//
//         using var stringWriter = new StringWriter();
//         using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
//         {
//             serializer.Serialize(xmlWriter, obj, namespaces);
//         }
//
//         return stringWriter.ToString();
//     }
// }
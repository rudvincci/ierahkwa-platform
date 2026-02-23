// using System.Reflection;
// using System.Security.Cryptography;
// using System.Text;
// using System.Xml;
// using System.Xml.Schema;
// using System.Xml.Serialization;
// using Mamey.ISO20022.Messages.Envelope;
// using Mamey.ISO20022.Messages.Header;
//
// namespace Mamey.ISO20022.Messages;
//
// /// <summary>
// /// Fluent builder for creating ISO 20022 messages.
// /// </summary>
// /// <typeparam name="T">The type of the business message payload.</typeparam>
// public class MessageBuilder<T>
// {
//     private BusinessMessageEnvelope _envelope;
//
//     private MessageBuilder(T businessMessage)
//     {
//         _envelope = new BusinessMessageEnvelope
//         {
//             Document = businessMessage,
//             References = new List<Reference22>
//             {
//                 new Reference22
//                 {
//                     Name = "Transaction Reference22",
//                     Issuer = new PartyIdentification()
//                     {
//                         Name = "Mamey"
//                     },
//                     Value = "Ref12345",
//                     UUID = Guid.NewGuid().ToString()
//                 }
//             },
//             SupplementaryData = new List<SupplementaryData>
//             {
//                 new SupplementaryData
//                 {
//                     PlaceAndName = "/Envelope/Supplementary",
//                     Envelope = new { /* Custom Supplementary Data */ }
//                 }
//             }
//         };
//     }
//
//     public static MessageBuilder<T> CreateMessage(T businessMessage)
//     {
//         
//         return new MessageBuilder<T>(businessMessage);
//     }
//
//     // public MessageBuilder<T> WithEnvelope(string reference)
//     // {
//     //     if (string.IsNullOrWhiteSpace(reference))
//     //     {
//     //         throw new InvalidOperationException("Envelope reference is mandatory.");
//     //     }
//     //
//     //     _envelope.SupplementaryData = reference;
//     //     return this;
//     // }
//
//     public MessageBuilder<T> WithHeader(BusinessApplicationHeader header)
//     {
//         _envelope.Header = header;
//         return this;
//     }
//
//     public async Task SendAsync()
//     {
//         ValidateMessage();
//         string xmlMessage = SerializeToXml(_envelope);
//         ValidateXmlAgainstSchema(xmlMessage, "Mamey.ISO20022.Schemas.PaymentsInitiation.pain.001.001.12.xsd");
//         xmlMessage = AddDigitalSignature(xmlMessage);
//         await SendMessageAsync(xmlMessage);
//     }
//
//     private void ValidateMessage()
//     {
//         if (_envelope.Header == null)
//             throw new InvalidOperationException("Header is mandatory.");
//         if (string.IsNullOrEmpty(_envelope.Header.BusinessMessageIdentifier))
//             throw new InvalidOperationException("BusinessMessageIdentifier is required.");
//         if (_envelope.Document == null)
//             throw new InvalidOperationException("Document is mandatory.");
//     }
//
//     private static string SerializeToXml(BusinessMessageEnvelope envelope)
//     {
//         XmlSerializer serializer = new(typeof(BusinessMessageEnvelope), new[] { typeof(T) });
//         var settings = new XmlWriterSettings
//         {
//             Encoding = Encoding.UTF8,
//             Indent = true,
//             OmitXmlDeclaration = false
//         };
//
//         using var stringWriter = new StringWriter();
//         using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
//         {
//             serializer.Serialize(xmlWriter, envelope);
//         }
//         return stringWriter.ToString();
//     }
//
//     private static void ValidateXmlAgainstSchema(string xml, string schemaResourceName)
//     {
//         XmlSchemaSet schemas = new();
//
//         // Load the embedded resource
//         var assembly = Assembly.GetExecutingAssembly();
//         //var resources = assembly.GetManifestResourceNames();
//         var resourceName = "Mamey.ISO20022.Schemas.PaymentsInitiation.pain.001.001.12.xsd";
//
//         using (Stream schemaStream = assembly.GetManifestResourceStream(resourceName))
//         {
//             if (schemaStream == null)
//             {
//                 throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
//             }
//
//             using (XmlReader schemaReader = XmlReader.Create(schemaStream))
//             {
//                 schemas.Add("urn:iso:std:iso:20022:tech:xsd:pain.001.001.12", schemaReader);
//             }
//         }
//
//         XmlDocument doc = new();
//         doc.LoadXml(xml);
//
//         doc.Schemas = schemas;
//
//         doc.Validate((sender, e) =>
//         {
//             if (e.Severity == XmlSeverityType.Error)
//             {
//                 throw new InvalidOperationException($"XML validation error: {e.Message}");
//             }
//         });
//     }
//
//     private static string AddDigitalSignature(string xml)
//     {
//         // Example: Adding a dummy signature. Replace with real cryptographic signing logic.
//         using var rsa = RSA.Create();
//         rsa.KeySize = 2048;
//
//         byte[] data = Encoding.UTF8.GetBytes(xml);
//         byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
//
//         return $"{xml}\n<!-- Signature: {Convert.ToBase64String(signature)} -->";
//     }
//
//     private static async Task SendMessageAsync(string message)
//     {
//         Console.WriteLine("Sending message...");
//         Console.WriteLine(message);
//         await Task.Delay(500);
//     }
// }
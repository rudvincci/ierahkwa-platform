using Azure;
using Azure.Communication.Email;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace Mamey.Emails.ACS;

public class ACSEmailSender : ISender
{
    private readonly EmailClient _emailClient;

    /// <summary>
    /// Creates a sender that uses the given EmailClient, but does not dispose it.
    /// </summary>
    /// <param name="emailClient"></param>
    public ACSEmailSender(EmailClient emailClient)
    {
        _emailClient = emailClient;
    }

    public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
    {
        return Task.Run(() => SendAsync(email, token)).Result;
    }

    public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();

        var emailMessage = CreateEmailMessage(email);

        if (token?.IsCancellationRequested ?? false)
        {
            response.ErrorMessages.Add("Message was cancelled by cancellation token.");
            return response;
        }

        try
        {
            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");

            /// Get the OperationId so that it can be used for tracking the message for troubleshooting
            string operationId = emailSendOperation.Id;
            Console.WriteLine($"Email operation id = {operationId}");
            response.MessageId = operationId;

            return response;
        }
        catch (RequestFailedException ex)
        {
            var message = $"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}";
            response.ErrorMessages.Add(message);
            return response;
        }
    }

    private EmailMessage CreateEmailMessage(IFluentEmail email)
    {
        var data = email.Data;

        var toRecipients = new List<EmailAddress>();
        data.ToAddresses.ForEach(x =>
        {
            toRecipients.Add(new EmailAddress(x.EmailAddress, x.Name));
        });

        var ccRecipients = new List<EmailAddress>();
        data.CcAddresses.ForEach(x =>
        {
            ccRecipients.Add(new EmailAddress(x.EmailAddress, x.Name));
        });

        var bccRecipients = new List<EmailAddress>();
        data.BccAddresses.ForEach(x =>
        {
            bccRecipients.Add(new EmailAddress(x.EmailAddress, x.Name));
        });

        var emailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);
        // Create the email content
        var emailContent = new EmailContent(email.Data.Subject)
        {
            PlainText = email.Data.PlaintextAlternativeBody,
            Html = email.Data.Body
        };

        // Create the EmailMessage
        var emailMessage = new EmailMessage(
            senderAddress: email.Data.FromAddress.EmailAddress, // The email address of the domain registered with the Communication Services resource
            emailRecipients,
            emailContent);
        
        foreach (var header in data.Headers)
        {
            emailMessage.Headers.Add(header.Key, header.Value);
        }

        emailMessage.Attachments.AddRange(GetEmailAttachments(email));

        return emailMessage;
    }
    private IEnumerable<EmailAttachment> GetEmailAttachments(IFluentEmail email)
    {
        List<EmailAttachment> attachments = new List<EmailAttachment>();

        email.Data.Attachments.ForEach(attachment =>
        {
            using (MemoryStream ms = new MemoryStream())
            {
                attachment.Data.CopyTo(ms);
                attachments.Add(new EmailAttachment(attachment.Filename, attachment.ContentType, new BinaryData(ms.ToArray())));
            }
        });
        return attachments;
    }
}

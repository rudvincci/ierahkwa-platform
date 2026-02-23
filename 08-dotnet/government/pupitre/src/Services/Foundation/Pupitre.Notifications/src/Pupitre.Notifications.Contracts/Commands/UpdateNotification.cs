using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Notifications.Contracts.Commands;

[Contract]
public record UpdateNotification : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();
    public string? ProgramCode { get; init; }
    public string? CredentialType { get; init; }
    public DateTime? CompletionDate { get; init; }
    public bool ReissueCredential { get; init; }
    public IDictionary<string, string>? Metadata { get; init; }
    public string? CredentialDocumentBase64 { get; init; }
    public string CredentialMimeType { get; init; } = "application/pdf";
    public string? Nationality { get; init; }
}



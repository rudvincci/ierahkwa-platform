using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Compliance.Contracts.Commands;

[Contract]
public record DeleteComplianceRecord(Guid Id) : ICommand;



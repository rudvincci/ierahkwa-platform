using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Fundraising.Contracts.Commands;

[Contract]
public record DeleteCampaign(Guid Id) : ICommand;



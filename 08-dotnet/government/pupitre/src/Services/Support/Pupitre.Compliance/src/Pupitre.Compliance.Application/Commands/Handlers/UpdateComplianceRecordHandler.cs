using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Compliance.Application.Exceptions;
using Pupitre.Compliance.Contracts.Commands;
using Pupitre.Compliance.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Compliance.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateComplianceRecordHandler : ICommandHandler<UpdateComplianceRecord>
{
    private readonly IComplianceRecordRepository _compliancerecordRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateComplianceRecordHandler(
        IComplianceRecordRepository compliancerecordRepository,
        IEventProcessor eventProcessor)
    {
        _compliancerecordRepository = compliancerecordRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateComplianceRecord command, CancellationToken cancellationToken = default)
    {
        var compliancerecord = await _compliancerecordRepository.GetAsync(command.Id);

        if(compliancerecord is null)
        {
            throw new ComplianceRecordNotFoundException(command.Id);
        }

        compliancerecord.Update(command.Name, command.Tags);
        await _compliancerecordRepository.UpdateAsync(compliancerecord);
        await _eventProcessor.ProcessAsync(compliancerecord.Events);
    }
}



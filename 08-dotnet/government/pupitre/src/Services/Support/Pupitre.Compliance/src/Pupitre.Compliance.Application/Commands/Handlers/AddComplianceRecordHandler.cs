using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Compliance.Application.Exceptions;
using Pupitre.Compliance.Contracts.Commands;
using Pupitre.Compliance.Domain.Entities;
using Pupitre.Compliance.Domain.Repositories;

namespace Pupitre.Compliance.Application.Commands.Handlers;

internal sealed class AddComplianceRecordHandler : ICommandHandler<AddComplianceRecord>
{
    private readonly IComplianceRecordRepository _compliancerecordRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddComplianceRecordHandler(IComplianceRecordRepository compliancerecordRepository,
        IEventProcessor eventProcessor)
    {
        _compliancerecordRepository = compliancerecordRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddComplianceRecord command, CancellationToken cancellationToken = default)
    {
        
        var compliancerecord = await _compliancerecordRepository.GetAsync(command.Id);
        
        if(compliancerecord is not null)
        {
            throw new ComplianceRecordAlreadyExistsException(command.Id);
        }

        compliancerecord = ComplianceRecord.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _compliancerecordRepository.AddAsync(compliancerecord, cancellationToken);
        await _eventProcessor.ProcessAsync(compliancerecord.Events);
    }
}


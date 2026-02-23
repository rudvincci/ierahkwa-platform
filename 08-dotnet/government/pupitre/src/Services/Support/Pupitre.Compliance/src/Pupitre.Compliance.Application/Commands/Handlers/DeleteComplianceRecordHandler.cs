using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Compliance.Application.Exceptions;
using Pupitre.Compliance.Contracts.Commands;
using Pupitre.Compliance.Domain.Repositories;

namespace Pupitre.Compliance.Application.Commands.Handlers;

internal sealed class DeleteComplianceRecordHandler : ICommandHandler<DeleteComplianceRecord>
{
    private readonly IComplianceRecordRepository _compliancerecordRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteComplianceRecordHandler(IComplianceRecordRepository compliancerecordRepository, 
    IEventProcessor eventProcessor)
    {
        _compliancerecordRepository = compliancerecordRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteComplianceRecord command, CancellationToken cancellationToken = default)
    {
        var compliancerecord = await _compliancerecordRepository.GetAsync(command.Id, cancellationToken);

        if (compliancerecord is null)
        {
            throw new ComplianceRecordNotFoundException(command.Id);
        }

        await _compliancerecordRepository.DeleteAsync(compliancerecord.Id);
        await _eventProcessor.ProcessAsync(compliancerecord.Events);
    }
}



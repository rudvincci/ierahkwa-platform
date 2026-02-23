using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using Mamey.Types;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands.Handlers;

internal sealed class CreateApplicationHandler : ICommandHandler<CreateApplication>
{
    private readonly IApplicationRepository _repository;
    private readonly IApplicationNumberService _numberService;

    public CreateApplicationHandler(
        IApplicationRepository repository,
        IApplicationNumberService numberService)
    {
        _repository = repository;
        _numberService = numberService;
    }

    public async Task HandleAsync(CreateApplication command, CancellationToken cancellationToken = default)
    {
        var applicationNumber = await _numberService.GenerateAsync("INK-CITAPP", cancellationToken);

        var applicantName = new Name(command.FirstName, command.LastName);
        
        var application = new CitizenshipApplication(
            new AppId(command.ApplicationId),
            new TenantId(command.TenantId),
            new ApplicationNumber(applicationNumber),
            applicantName,
            command.DateOfBirth,
            !string.IsNullOrWhiteSpace(command.Email) ? new Email(command.Email) : null);

        // Update with phone and address if provided
        Phone? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            phone = new Phone("+1", command.Phone); // Default US country code
        }

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(command.Street))
        {
            address = new Address(
                firmName: string.Empty,
                line: command.Street,
                line2: null,
                line3: null,
                urbanization: null,
                city: command.City ?? string.Empty,
                state: command.State ?? string.Empty,
                zip5: command.PostalCode?.Length >= 5 ? command.PostalCode[..5] : "00000",
                zip4: command.PostalCode?.Length > 5 ? command.PostalCode[5..] : null,
                postalCode: command.PostalCode,
                country: command.Country ?? "US",
                province: null);
        }

        if (phone != null || address != null)
        {
            application.UpdatePersonalDetails(applicantName, command.DateOfBirth, application.Email, phone, address);
        }

        await _repository.AddAsync(application, cancellationToken);
    }
}

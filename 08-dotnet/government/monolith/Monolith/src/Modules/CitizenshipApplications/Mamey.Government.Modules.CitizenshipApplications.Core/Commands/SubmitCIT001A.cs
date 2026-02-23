using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands;

public record SubmitCIT001A : ICommand
{
    public ApplicationId ApplicationId { get; init; }
    // public PersonalInformation
}
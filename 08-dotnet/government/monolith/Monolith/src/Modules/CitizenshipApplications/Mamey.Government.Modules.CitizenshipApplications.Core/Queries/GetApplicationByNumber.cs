using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries;

public record GetApplicationByNumber(string ApplicationNumber) : IQuery<ApplicationDto?>;

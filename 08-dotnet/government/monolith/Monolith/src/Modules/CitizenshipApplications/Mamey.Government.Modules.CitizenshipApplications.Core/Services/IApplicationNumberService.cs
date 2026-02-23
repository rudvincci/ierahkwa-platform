using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

public interface IApplicationNumberService
{
    Task<string> GenerateAsync(string prefix, CancellationToken cancellationToken = default);
}

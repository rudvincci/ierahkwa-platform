using System;
using Mamey.Types;

namespace Mamey.ServiceName.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}


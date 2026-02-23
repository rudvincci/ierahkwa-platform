using System;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}


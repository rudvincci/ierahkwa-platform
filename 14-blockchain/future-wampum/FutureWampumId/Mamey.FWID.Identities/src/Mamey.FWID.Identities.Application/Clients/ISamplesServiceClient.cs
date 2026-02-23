using System;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}


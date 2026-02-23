using System;
using Mamey.Types;

namespace Pupitre.Compliance.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}


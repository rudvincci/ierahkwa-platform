using System;
using Mamey.Types;

namespace Pupitre.Operations.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

